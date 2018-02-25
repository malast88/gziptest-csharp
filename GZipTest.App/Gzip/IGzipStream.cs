using System.IO;

namespace GZipTest.App.Gzip
{
    public interface IGzipStream
    {
        byte[] Compress(byte[] buffer);
    }
}
