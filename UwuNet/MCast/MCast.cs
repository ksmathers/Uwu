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

namespace UwuNet.MCast
{
    public class MCast : Protocol<MCast>, IMessaging
    {
        Thread networking;
        volatile bool connected;
        volatile bool running = true;
        Socket mcastSocket;
        const int BASE_MCAST_PORT = 19900;

        public event OrchestrationMessageHandler MessageReceived;
        public event OrchestrationMessageHandler FormMessageReceived;
        IPEndPoint mcastEndPoint;

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
            networking = new Thread(() => { Run(connectTo); });
            networking.Start();
        }

        public void Run(string connectTo)
        {
            try {
                mcastSocket = new Socket(AddressFamily.InterNetwork,
                                         SocketType.Dgram,
                                         ProtocolType.Udp);

                //IPAddress localIPAddr = IPAddress.Parse(Console.ReadLine());
                IPAddress localIP = IPAddress.Any;
                IPEndPoint localEP = new IPEndPoint(IPV4Address(localIP), BASE_MCAST_PORT);
                IPAddress mcastAddress = ParseConnectString(connectTo);
                mcastEndPoint = new IPEndPoint(mcastAddress, BASE_MCAST_PORT);
                mcastSocket.Bind(localEP);
                // Define a MulticastOption object specifying the multicast group
                // address and the local IPAddress.
                // The multicast group address is the same as the address used by the server.
                var mcastOption = new MulticastOption(mcastAddress, localIP);

                mcastSocket.SetSocketOption(SocketOptionLevel.IP,
                                            SocketOptionName.AddMembership,
                                            mcastOption);
                connected = true;
            } catch (Exception e) {
                connected = false;
                throw e;
            }

            IOBuffer inbuf = new IOBuffer();
            while (running) {
                List<ArraySegment<byte>> buffers = new List<ArraySegment<byte>>();
                int mrec = mcastSocket.Receive(buffers);
                foreach (var buf in buffers) {
                    inbuf.Write(buf.Array);
                }
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

        public void Stop()
        {
            running = false;
            networking.Join();
        }
    

        public void Send(string target, byte[] data)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string msg, Options opts = Options.LAN)
        {
            byte[] buffer = ASCIIEncoding.UTF8.GetBytes(msg);
            mcastSocket.SendTo(buffer, mcastEndPoint);
        }
    }
}
