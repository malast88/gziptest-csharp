using System.IO;

namespace GZipTest.App.Io
{
    public interface IIo
    {
        Stream FileOpenRead(string filePath);
    }
}
