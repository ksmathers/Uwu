using System;
using System.Collections.Generic;
using System.Text;

namespace UwuNet
{
    /// <summary>
    /// An adaptor for IOBuffer that attaches String and Integer serialization methods.   
    /// 
    /// The String serialization is done using a modified UTF8 encoding in which an ASCII NUL 
    /// is written using two bytes so that the '0' byte can be reserved as an end of string marker.   
    /// The UTF8 encoding is otherwise complete including code pages beyond the common multi-language
    /// plane.
    /// 
    /// Integer serialization is done LSB first (Little-Endian) and 
    /// is not aligned to word address boundaries but is packed into the IOBuffer.
    /// </summary>
    public class Marshal
    {
        IOBuffer iobuf;
        const uint PLANE1 = 0x10000;

        public Marshal(IOBuffer iobuf = null)
        {
            if (iobuf == null) iobuf = new IOBuffer();
            this.iobuf = iobuf;
        }

        public IOBuffer IOBuf {
            get { return iobuf; }
        }

        /// <summary>
        /// Returns the number of bytes needed to store a particular UTF-32 code point using our modified UTF-8 encoding
        /// </summary>
        /// <param name="c">a UTF-32 (20-bit) character</param>
        /// <returns></returns>
        public int EncodedLength(int c)
        {
            if (c < 0) {
                throw new NotImplementedException("Unimplemented character range");
            }
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

        /// <summary>
        /// The semi-value of a UTF-16 surrogate, which when combined with another surrogate 
        /// can result in a UTF-32 code point.
        /// </summary>
        /// <param name="c">a character in the high or low surrogate range</param>
        /// <returns>a UTF-32 rune</returns>
        public int SurrogateValue(char c)
        {
            uint ival = 0;
            if (Char.IsHighSurrogate(c)) {
                ival = (((uint)(c & 0x3ff)) << 10) + PLANE1;
            } else if (Char.IsLowSurrogate(c)) {
                ival = ((uint)(c & 0x3ff)) << 0;
            }
            return (int)ival;
        }

        /// <summary>
        /// Returns the high and low surrogate pair that represent a UTF-32 codepoint
        /// </summary>
        /// <param name="rune">UTF-32 codepoint</param>
        /// <returns>high and low surrogate pair</returns>
        public (char, char) SurrogatePair(int rune)
        {
            uint urune = (uint)rune - PLANE1;
            char clow = (char)(0xdc00 + ((urune >> 0) & 0x3ff));
            char chigh = (char)(0xd800 + ((urune >> 10) & 0x3ff));
            return (chigh, clow);
        }

        /// <summary>
        /// Iterator for stepping through a string one UTF-32 codepoint at a time
        /// </summary>
        /// <param name="s">a string to iterate through</param>
        /// <returns>the next rune in s</returns>
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

        /// <summary>
        /// Calculates the UTF-8 encoded length of a string, where C# strings are natively stored as UTF-16.
        /// </summary>
        /// <param name="s">the string</param>
        /// <returns>resultant length in bytes</returns>
        public int EncodedLength(string s)
        {
            int ll = 0;
            foreach (int rune in NextRune(s)) {
                ll += EncodedLength(rune);
            }
            return ll;
        }


        /// <summary>
        /// Writes a single UTF-32 codepoint as a set of byte using UTF-8 encoding
        /// </summary>
        /// <param name="c"></param>
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

        /// <summary>
        /// Reads a set of bytes from the stream sufficient to complete a UTF-32 codepoint
        /// </summary>
        /// <returns>the rune</returns>
        public int ReadUTF8()
        {
            uint a = iobuf.Read();
            uint ichar = 0;
            if ((a & 0x80) == 0) {
                ichar = a;
            }
            else if ((a & 0xE0) == 0xC0) {
                uint b = iobuf.Read();
                ichar = ((a & 0x1f) << 6) | ((b & 0x3f) << 0);
            }
            else if ((a & 0xF0) == 0xE0) {
                uint b = iobuf.Read();
                uint c = iobuf.Read();
                ichar = ((a & 0x0f) << 12) | ((b & 0x3f) << 6) | ((c & 0x3f) << 0);
            }
            else if ((a & 0xF8) == 0xF0) {
                uint b = iobuf.Read();
                uint c = iobuf.Read();
                uint d = iobuf.Read();
                ichar = ((a & 0x03) << 18) | ((b & 0x3f) << 12) | ((c & 0x3f) << 6) | ((d & 0x3f) << 0);
            }
            else {
                throw new NotImplementedException("Character not in implemented range");
            }
            return (int)ichar;
        }

        /// <summary>
        /// Reads a zero terminatedstring from a IOBuffer using UTF8 encoding, except that 0 is the string terminator and NUL should have been saved in two bytes
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
                    (shigh, slow) = SurrogatePair(rune);     
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

        /// <summary>
        /// Reads a signed 32-bit integer from the IOBuffer
        /// </summary>
        /// <returns></returns>
        public int ReadInt32()
        {
            byte[] buf = iobuf.Read(4);
            return (int)BytesToUInt32(buf);
        }

        /// <summary>
        /// Writes a signed 32-bit integer to the IOBuffer
        /// </summary>
        /// <param name="val"></param>
        public void WriteInt32(int val)
        {
            byte[] buf = UInt32ToBytes((uint)val);
            iobuf.Write(buf);
        }

        /// <summary>
        /// Writes an unsigned 32-bit integer to the IOBuffer
        /// </summary>
        /// <param name="val"></param>
        public void WriteUInt32(uint val)
        {
            byte[] buf = UInt32ToBytes(val);
            iobuf.Write(buf);
        }

        /// <summary>
        /// Fetches a signed 32-bit integer from the buffer without moving the read position
        /// </summary>
        /// <param name="offset">relative to the current read position</param>
        /// <returns></returns>
        public int PeekInt32(int offset=0)
        {
            byte[] buf = iobuf.Peek(offset, 4);
            return (int)BytesToUInt32(buf);
        }

        /// <summary>
        /// Writes a signed 32-bit integer to the buffer without moving the write position
        /// </summary>
        /// <param name="offset">in bytes relative to rpos</param>
        /// <param name="val"></param>
        public void PokeInt32(int offset, int val)
        {
            byte[] buf = UInt32ToBytes((uint)val);
            iobuf.Poke(offset, buf);
        }

        /// <summary>
        /// Convert uint to four bytes, LSB first
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        byte[] UInt32ToBytes(uint val)
        {
            byte[] buf = new byte[4];
            buf[0] = (byte)((val >> 0) & 0xff);
            buf[1] = (byte)((val >> 8) & 0xff);
            buf[2] = (byte)((val >> 16) & 0xff);
            buf[3] = (byte)((val >> 24) & 0xff);
            return buf;
        }

        /// <summary>
        /// Convert four bytes to uint, LSB first
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        uint BytesToUInt32(byte[] buf)
        {
            return (uint)(buf[0] << 0) | (uint)(buf[1] << 8) | (uint)(buf[2] << 16) | (uint)(buf[3] << 24);
        }
    }
}
