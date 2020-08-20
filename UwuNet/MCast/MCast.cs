using System;
using System.Collections.Generic;
using System.Text;
//using System.Net.Sockets.Multicast;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics.Tracing;

namespace UwuNet.MCast
{
    public class MCast : Protocol<MCast>, IMessaging
    {
        volatile Thread networking;
        volatile bool connected;
        volatile bool running = true;
        Socket s_in;
        Socket s_out;
        string state = "";
        const int BASE_MCAST_PORT = 19900;
        List<BaseMessage> outbox;
        List<BaseMessage> inbox;

        public event GroupMessageHandler MessageReceived;
        public event StateChangedHandler StateChanged;

        //public event OrchestrationMessageHandler FormMessageReceived;
        IPEndPoint mcastEndPoint;

        public string State { 
            get {
                return state;
            } 
        }

        public MCast()
        {
            inbox = new List<BaseMessage>();
            outbox = new List<BaseMessage>();
        }

        public void OnStateChanged(string newstate)
        {
            state = newstate;
            StateChanged?.Invoke(this, State);
        }

        IPAddress ParseConnectString(string connectTo)
        {
            string multicastAddress = "224.0.1.187";
            return IPAddress.Parse(multicastAddress);
        }

        static long IPV4Address(IPAddress addr)
        {
            byte[] addr_b = addr.GetAddressBytes();
            long addr_i = -1;
            if (addr_b.Length == 4) {
                addr_i = (long)(((ulong)addr_b[0] << 24) | ((ulong)addr_b[1] << 16) | ((ulong)addr_b[2] << 8) | ((ulong)addr_b[3] << 0));
            }
            return addr_i;
        }


        public void Connect(string connectTo)
        {
            OnStateChanged("CONNECTING");
            try {
                IPAddress mcastAddress = ParseConnectString(connectTo);
                mcastEndPoint = new IPEndPoint(mcastAddress, BASE_MCAST_PORT);


                // Setup output socket
                s_out = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                s_out.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(mcastAddress));
                s_out.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
                
                s_out.Connect(mcastEndPoint);

                // Setup input socket
                s_in = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                s_in.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                IPEndPoint endpoint_in = new IPEndPoint(IPAddress.Any, BASE_MCAST_PORT);
                s_in.Bind(endpoint_in);
                s_in.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(mcastAddress, IPAddress.Any));
                s_in.ReceiveTimeout = 100;
                
            } catch (Exception e) {
                connected = false;
                throw e;
            }

            networking = new Thread(() => { Run(s_in); });
            networking.Start();
        }

        public void Run(Socket s_svc)
        {
            connected = true;
            OnStateChanged("CONNECTED");
            byte[] buffer = new byte[65536];    // maximum UDP message size
            IOBuffer inbuf = new IOBuffer();
            try {
                while (running) {
                    int rlen = s_svc.Receive(buffer, 0, buffer.Length, SocketFlags.None, out SocketError error);
                    if (error == SocketError.TimedOut) continue;
                    if (error == SocketError.Success) {
                        inbuf.Write(buffer, rlen);
                        while (running) {
                            var msg = Message.Receive(inbuf);
                            if (msg == null) break;
                            var omsg = msg as OrchestrationMessage;
                            if (omsg != null) {
                                MessageReceived?.Invoke(omsg.Body, omsg.Options);
                            }
                        }
                        inbuf.Compress();
                    }
                }
            } finally {
                connected = false;
                running = false;
                OnStateChanged("ERROR");
            }
        }

        public void Stop()
        {
            running = false;
            OnStateChanged("STOPPED");
            networking.Join();
        }
    

        public void SendMessage(string msg, Options opts = Options.LAN)
        {
            OrchestrationMessage xmsg = new OrchestrationMessage(msg, opts);
            SendMessage(xmsg);
        }

        public void SendMessage(BaseMessage msg)
        {
            IOBuffer outbuf = new IOBuffer();
            Message.Transmit(outbuf, msg);
            if (outbuf.Length > 65536) {
                // Message too long for this transport, drop it
                Console.Error.WriteLine($"Message dropped: too long for MCast transport {outbuf.Length}");
            } else {
                var args = new SocketAsyncEventArgs();
                var buf = outbuf.Read(outbuf.Length);
                s_out.Send(buf);
            }
        }
    }
}
