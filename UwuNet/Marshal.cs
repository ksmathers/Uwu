﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Sources;

namespace UwuNet
{
    public class Marshal
    {
        IOBuffer iobuf;

        public Marshal(IOBuffer iobuf = null)
        {
            if (iobuf == null) iobuf = new IOBuffer();
            this.iobuf = iobuf;
        }


        public int EncodedLength(int c)
        {
            int cval = c;
            if (c == 0) {
                return 2;
            }
            if (c <= 0x7f) {
                return 1;
            } 
            if (c <= 0x7ff) {
                return 2;
            }
            if (c <= 0xffff) {
                return 3;
            }
            if (c <= 0x10ffff) {
                return 4;
            }
            throw new NotImplementedException("Unimplemented character range");
        }

        public int SurrogateValue(char c)
        {
            int ival = 0;
            if (Char.IsHighSurrogate(c)) {
                ival = (c & 0x3ff) << 10;
            } else if (Char.IsLowSurrogate(c)) {
                ival = (c & 0x3ff) << 0;
            }
            return ival;
        }

        public (char, char) SurrogatePair(int rune)
        {
            char clow = (char)(0xdc00 + ((rune >> 0) & 0x3ff));
            char chigh = (char)(0xd800 + ((rune >> 10) & 0x3ff));
            return (clow, chigh);
        }

        IEnumerable<int> NextRune(string s)
        {
            for (int i = 0; i < s.Length; i++) {
                char c = s[i];
                if (!Char.IsSurrogate(c)) {
                    yield return c;
                } else {
                    i++;
                    if (i < s.Length) {
                        char d = s[i];
                        if (Char.IsSurrogatePair(c, d)) {
                            int ival = SurrogateValue(c) | SurrogateValue(d);
                            yield return ival;
                        } else {
                            throw new ArgumentOutOfRangeException("Invalid surrogate pair in string");
                        }
                    } else {
                        throw new ArgumentException("String ends on half of a surrogate pair");
                    }
                }
            }
        }

        public int EncodedLength(string s)
        {
            int ll = 0;
            foreach (int rune in NextRune(s)) {
                ll += EncodedLength(rune);
            }
            return ll;
        }

        internal int PeekUInt32()
        {
            throw new NotImplementedException();
        }

        public void WriteUTF8(int c)
        {
            int ival = (int)c;
            int elen = EncodedLength(c);
            byte[] buf = new byte[elen];
            if (c == 0) {
                buf[0] = (byte)(0xC0);
                buf[1] = (byte)(0x80);
            } 
            else if (c <= 0x7f) {
                buf[0] = (byte)(0x00 | ((ival >> 0) & 0x7f));
            }
            else if (c <= 0x7ff) {
                buf[0] = (byte)(0xC0 | ((ival >> 6) & 0x1f));
                buf[1] = (byte)(0x80 | ((ival >> 0) & 0x3f));
            }
            else if (c <= 0xffff) {
                buf[0] = (byte)(0xE0 | ((ival >> 12) & 0x0f));
                buf[1] = (byte)(0x80 | ((ival >> 6) & 0x3f));
                buf[2] = (byte)(0x80 | ((ival >> 0) & 0x3f));
            }
            else if (c <= 0x10ffff) {
                buf[0] = (byte)(0xf0 | ((ival >> 18) & 0x07));
                buf[1] = (byte)(0x80 | ((ival >> 12) & 0x3f));
                buf[2] = (byte)(0x80 | ((ival >> 6) & 0x3f));
                buf[3] = (byte)(0x80 | ((ival >> 0) & 0x3f));
            }
            iobuf.Write(buf);
        }

        public int ReadUTF8()
        {
            int a = iobuf.Read();
            int ichar = -1;
            if ((a & 0x80) == 0) {
                ichar = a;
            }
            else if ((a & 0xE0) == 0xC0) {
                int b = iobuf.Read();
                ichar = ((a & 0x1f) << 6) | ((b & 0x3f) << 0);
            }
            else if ((a & 0xF0) == 0xE0) {
                int b = iobuf.Read();
                int c = iobuf.Read();
                ichar = ((a & 0x0f) << 12) | ((b & 0x3f) << 6) | ((c & 0x3f) << 0);
            }
            else if ((a & 0xF8) == 0xF0) {
                int b = iobuf.Read();
                int c = iobuf.Read();
                int d = iobuf.Read();
                ichar = ((a & 0x03) << 18) | ((b & 0x3f) << 12) | ((c & 0x3f) << 6) | ((d & 0x3f) << 0);
            }
            else {
                throw new NotImplementedException("Character not in implemented range");
            }
            return ichar;
        }

        /// <summary>
        /// Reads a zero terminatedstring from a ByteBuffer using UTF8 encoding, except that 0 is the string terminator and NUL should have been saved in two bytes
        /// </summary>
        /// <returns>String read</returns>
        public string ReadString()
        {
            var buf = new StringBuilder();
            while (true) {
                if (iobuf.Peek(0) == 0) {
                    iobuf.Read();
                    return buf.ToString();
                }
                var rune = ReadUTF8();
                if (rune > 0xffff) {
                    char slow, shigh;
                    (slow, shigh) = SurrogatePair(rune);     
                    buf.Append(shigh);
                    buf.Append(slow);
                } else {
                    buf.Append((char)rune);
                }
            }
        }

        /// <summary>
        /// Writes a zero terminated string to a ByteBuffer using UTF8 encoding, except that NUL is encoded in two bytes so that the zero value is reserved as the terminator.
        /// </summary>
        /// <param name="val">Value to be written</param>
        public void WriteString(string val)
        {
            foreach (var rune in NextRune(val)) {
                WriteUTF8(rune);
            }
            iobuf.Write(0);
        }

        public int ReadInt32()
        {
            byte[] buf = iobuf.Read(4);
            return BytesToInt32(buf);
        }




        public void WriteInt32(int val)
        {
            byte[] buf = new byte[4];
            buf[0] = (byte)((val >> 0) & 0xff);
            buf[1] = (byte)((val >> 8) & 0xff);
            buf[2] = (byte)((val >> 16) & 0xff);
            buf[3] = (byte)((val >> 32) & 0xff);
            iobuf.Write(buf);
        }

        public int PeekInt32(int offset)
        {
            byte[] buf = iobuf.Peek(offset, 4);
            return BytesToInt32(buf);
        }

        public void PokeInt32(int offset, int val)
        {
            byte[] buf = Int32ToBytes(val);
            iobuf.Poke(offset, buf);
        }

        byte[] Int32ToBytes(int val)
        {
            byte[] buf = new byte[4];
            buf[0] = (byte)((val >> 0) & 0xff);
            buf[1] = (byte)((val >> 8) & 0xff);
            buf[2] = (byte)((val >> 16) & 0xff);
            buf[3] = (byte)((val >> 24) & 0xff);
            return buf;
        }

        int BytesToInt32(byte[] buf)
        {
            return (buf[0] << 0) | (buf[1] << 8) | (buf[2] << 16) | (buf[3] << 24);
        }
    }
}
