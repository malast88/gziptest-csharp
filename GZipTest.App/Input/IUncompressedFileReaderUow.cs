using System;

namespace GZipTest.App.Input
{
    public interface IUncompressedFileReaderUow
    {
        Action ReadFileAction(string filePath);
    }
}
