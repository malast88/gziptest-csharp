using GZipTest.App.Main;
using System;
using System.IO;

namespace GZipTest.App.Gzip
{
    public class GzipStream : IGzipStream
    {
        public byte[] Compress(byte[] buffer, JobType jobType)
        {
            if (jobType == JobType.Compress)
            {
                return Compress(buffer);
            }
            if (jobType == JobType.Decompress)
            {
                return Decompress(buffer);
            }
            throw new InvalidOperationException($"unsupported jobtype '{jobType}'");
        }

        private byte[] Compress(byte[] buffer)
        {
            using (var outputMs = new MemoryStream())
            {
                using (var gzip = new System.IO.Compression.GZipStream(outputMs,
                    System.IO.Compression.CompressionMode.Compress))
                {
                    gzip.Write(buffer, 0, buffer.Length);
                    gzip.Close();

                    return outputMs.ToArray();
                }
            }
        }

        private byte[] Decompress(byte[] buffer)
        {
            using (var inputMs = new MemoryStream(buffer))
            {
                using (var gzip = new System.IO.Compression.GZipStream(inputMs,
                    System.IO.Compression.CompressionMode.Decompress))
                {
                    inputMs.Flush();

                    byte[] buf = new byte[32768];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        while (true)
                        {
                            int read = gzip.Read(buf, 0, buf.Length);
                            if (read <= 0)
                                return ms.ToArray();
                            ms.Write(buf, 0, read);
                        }
                    }
                }
            }
        }
    }
}
