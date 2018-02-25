using System;

namespace GZipTest.App.Ouput
{
    public interface IFileWriterUow
    {
        Action WriteFileAction(string fileName);
    }
}
