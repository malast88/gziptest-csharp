using System.IO;

namespace GZipTest.App.Io
{
    public class IoImpl : IIo
    {
        public Stream FileOpenRead(string filePath)
        {
            return File.OpenRead(filePath);
        }

        public Stream FileOpenWrite(string filePath)
        {
            return File.OpenWrite(filePath);
        }
    }
}
