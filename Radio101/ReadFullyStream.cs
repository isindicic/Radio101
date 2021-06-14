using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace NAudioDemo.Mp3StreamingDemo
{
    public class ReadFullyStream : Stream
    {
        private readonly Stream sourceStream;
        private long pos; // psuedo-position
        private readonly byte[] readAheadBuffer;
        private int readAheadLength;
        private int readAheadOffset;

        private int icyMetaData = 0;
        private byte[] icyDataByteArray;
        public string icyData = "";
        public EventHandler OnIcyData = null;

        public ReadFullyStream(Stream sourceStream) : this(sourceStream, 0)
        {
        }

        public ReadFullyStream(Stream sourceStream, int icyMetaData)
        {
            this.sourceStream = sourceStream;
            this.icyMetaData = icyMetaData;
            readAheadBuffer = new byte[4096 + icyMetaData];
            icyDataByteArray = new byte[4096];
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new InvalidOperationException();
        }

        public override long Length
        {
            get { return pos; }
        }

        public override long Position
        {
            get
            {
                return pos;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;
            while (bytesRead < count)
            {
                int readAheadAvailableBytes = readAheadLength - readAheadOffset;
                int bytesRequired = count - bytesRead;
                if (readAheadAvailableBytes > 0)
                {
                    int toCopy = Math.Min(readAheadAvailableBytes, bytesRequired);
                    Array.Copy(readAheadBuffer, readAheadOffset, buffer, offset + bytesRead, toCopy);
                    bytesRead += toCopy;
                    readAheadOffset += toCopy;
                }
                else
                {
                    readAheadOffset = 0;
                    if (this.icyMetaData > 0)
                    {
                        int toRead = icyMetaData + 1;
                        int r = 0;

                        while(r != toRead)
                            r += sourceStream.Read(readAheadBuffer, r, toRead - r);

                        readAheadLength = icyMetaData;
                        int icyLen = 16 * readAheadBuffer[icyMetaData];
                        if (icyLen > 0)
                        {
                            toRead = icyLen;
                            if(this.icyDataByteArray.Length < toRead)
                                this.icyDataByteArray = new byte[toRead];
                            r = 0;

                            while (r != toRead)
                                r += sourceStream.Read(icyDataByteArray, r, toRead - r);

                            int zeroIndex = Array.IndexOf(icyDataByteArray, (byte)0);
                            byte[] ba;
                            if (zeroIndex != -1)
                                ba = this.icyDataByteArray.Take(zeroIndex).ToArray();
                            else
                                ba = this.icyDataByteArray;

                            this.icyData = System.Text.Encoding.Default.GetString(ba);
                            if (this.OnIcyData != null)
                                this.OnIcyData(this, EventArgs.Empty);
                        }
                    }
                    else
                        readAheadLength = sourceStream.Read(readAheadBuffer, 0, readAheadBuffer.Length);
                    //Debug.WriteLine(String.Format("Read {0} bytes (requested {1})", readAheadLength, readAheadBuffer.Length));
                    if (readAheadLength == 0)
                    {
                        break;
                    }
                }
            }
            pos += bytesRead;
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }
    }
}
