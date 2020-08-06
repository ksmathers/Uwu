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

        /// <summary>
        /// An in memory buffer for serialized data that is either being read from, or written to an IO stream.
        /// </summary>
        public IOBuffer()
        {
            data = null;
        }

        /// <summary>
        /// Returns the current write position of the buffer.   Useful when writing length-prefixed blocks of data
        /// for preallocating the length field and remembering the location for later replacement using Poke()
        /// </summary>
        public int WriteOffset { get { return wpos; } }

        /// <summary>
        /// Rounds 'v' up to the next multiple of 'q'
        /// </summary>
        /// <param name="v"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        int Quantize(int v, int q)
        {
            var nv = v + q - 1;
            nv -= (nv % q);
            return nv; 
        }


        /// <summary>
        /// Extends the IOBuffer by at least 'nbytes'.
        /// </summary>
        /// <param name="nbytes">bytes to add</param>
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

        /// <summary>
        /// Writes a byte array into the buffer.   If 'len' is provided then only the first 'len' bytes
        /// are copied.  If omitted then the entire buffer is copied.
        /// </summary>
        /// <param name="buf">source buffer</param>
        /// <param name="len">source len (or omit to use all of buf)</param>
        public void Write(byte[] buf, int len=-1)
        {
            if (len == -1) len = buf.Length;
            Extend(len);
            Array.Copy(buf, 0, data, wpos, len);
            wpos += len;
        }

        /// <summary>
        /// Writes a set of bytes into the buffer, useful for Span or other iterable subranges of arrays.
        /// </summary>
        /// <param name="buf">byte range</param>
        public void Write(IEnumerable<byte> buf)
        {
            Write(buf.ToArray());
        }

        /// <summary>
        /// Reads a single byte from the buffer
        /// </summary>
        /// <returns></returns>
        public byte Read()
        {
            if (rpos < data.Length) return data[rpos++];
            throw new EndOfStreamException();
        }

        /// <summary>
        /// Writes a single byte to the buffer
        /// </summary>
        /// <param name="value"></param>
        public void Write(byte value)
        {
            Extend(1);
            data[wpos++] = value;
        }

        /// <summary>
        /// Returns a single byte at an offset relative to the current read position without moving the read position
        /// </summary>
        /// <param name="index">the offset in bytes</param>
        /// <returns></returns>
        public byte Peek(int index)
        {
            if (rpos + index < data.Length) return data[rpos + index];
            throw new EndOfStreamException();
        }

        /// <summary>
        /// Returns an array of bytes starting from 'index' as an offset relative to the current read position, and having 'len' bytes.
        /// Throws EndOfStreamException() if the index is out of bounds.
        /// </summary>
        /// <param name="index">offset in bytes</param>
        /// <param name="len">how many bytes to read</param>
        /// <returns>A copy of the selected data</returns>
        public byte[] Peek(int index, int len)
        {
            if (index >= 0 && rpos + index + len < data.Length) {
                byte[] buf = new byte[len];
                Array.Copy(data, rpos + index, buf, 0, len);
                return buf;
            }
            throw new EndOfStreamException();
        }

        /// <summary>
        /// Replaces the value of a byte at an offset relative to the current read position.   Use together with WriteOffset to 
        /// replace already written data within a buffer as it is being built
        /// </summary>
        /// <param name="index">offset in bytes relative to rpos</param>
        /// <param name="value">byte to write</param>
        public void Poke(int index, byte value)
        {
            if (rpos + index < data.Length) data[rpos + index] = value;
            else throw new EndOfStreamException();
        }

        /// <summary>
        /// Replaces a set of bytes previously written to the buffer.  Use together with WriteOffset to replace data already
        /// written into the buffer as it was being built.
        /// </summary>
        /// <param name="index">offset in bytes relative to rpos</param>
        /// <param name="buf">byte to write</param>
        public void Poke(int index, byte[] buf)
        {
            //TODO: BUGFIX / index is relative to rpos, but WriteOffset is relative to the start of the buffer. This only works because 
            //both start at zero and we call Poke() before reading any data from the message.   
            if (index >= 0 && rpos + index + buf.Length < data.Length) {
                Array.Copy(buf, 0, data, rpos + index, buf.Length);
            } else {
                throw new EndOfStreamException();
            }
        }

        /// <summary>
        /// Read a number of bytes from the buffer.
        /// </summary>
        /// <param name="nbytes"></param>
        /// <returns></returns>
        public byte[] Read(int nbytes)
        {
            if (rpos + nbytes > wpos) throw new EndOfStreamException();
            byte[] buf = new byte[nbytes];
            Array.Copy(data, rpos, buf, 0, nbytes);
            rpos += nbytes;
            return buf;
        }

        /// <summary>
        /// Resets rpos to zero by removing all of the previously read data from the buffer.   Called after demarshalling a 
        /// message, this cuts out the data from the previous message keeping only data not yet processed.
        /// </summary>
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
