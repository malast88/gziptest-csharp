using System.IO;

namespace GZipTest.App.Gzip
{
    public class GzipStream : IGzipStream
    {
        public byte[] Compress(byte[] buffer)
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
    }
}
