using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml.Schema;

namespace UwuNet
{
    public class IOBuffer
    {
        byte[] data;
        int rpos = 0;
        int wpos = 0;
        int blocksize = 512;

        public IOBuffer()
        {
            data = null;
        }

        public int WriteOffset { get { return wpos; } }

        int Quantize(int v, int q)
        {
            var nv = v + q - 1;
            nv -= (nv % q);
            return nv; 
        }

        public void Extend(int nbytes)
        {
            var ll = nbytes + wpos;
            if (data == null || ll > data.Length) {
                var ndata = new byte[Quantize(ll, blocksize)];
                if (data != null) data.CopyTo(ndata, 0);
                data = ndata;
            }
        }

        /// <summary>
        /// Returns the current length in bytes of the unread data in the ByteBuffer.
        /// </summary>
        public int Length {
            get { return wpos - rpos; }
        }

        public void Write(byte[] buf, int len=-1)
        {
            if (len == -1) len = buf.Length;
            Extend(len);
            Array.Copy(buf, 0, data, wpos, len);
            wpos += len;
        }

        public void Write(IEnumerable<byte> buf)
        {
            Write(buf.ToArray());
        }

        public byte Read()
        {
            if (rpos < data.Length) return data[rpos++];
            throw new EndOfStreamException();
        }

        public void Write(byte value)
        {
            Extend(1);
            data[wpos++] = value;
        }

        public byte Peek(int index)
        {
            if (rpos + index < data.Length) return data[rpos + index];
            throw new EndOfStreamException();
        }

        public byte[] Peek(int index, int len)
        {
            if (index >= 0 && rpos + index + len < data.Length) {
                byte[] buf = new byte[len];
                Array.Copy(data, rpos + index, buf, 0, len);
                return buf;
            }
            throw new EndOfStreamException();
        }

        public void Poke(int index, byte value)
        {
            if (rpos + index < data.Length) data[rpos + index] = value;
            else throw new EndOfStreamException();
        }

        public void Poke(int index, byte[] buf)
        {
            if (index >= 0 && rpos + index + buf.Length < data.Length) {
                Array.Copy(buf, 0, data, rpos + index, buf.Length);
            } else {
                throw new EndOfStreamException();
            }
        }

        public byte[] Read(int nbytes)
        {
            if (rpos + nbytes > wpos) throw new EndOfStreamException();
            byte[] buf = new byte[nbytes];
            Array.Copy(data, rpos, buf, 0, nbytes);
            rpos += nbytes;
            return buf;
        }

        public void Compress()
        {
            var nbuf = new byte[wpos-rpos];
            Array.Copy(data, rpos, nbuf, 0, wpos - rpos);
            data = nbuf;
            rpos = 0;
            wpos = nbuf.Length;
        }
    }
}
