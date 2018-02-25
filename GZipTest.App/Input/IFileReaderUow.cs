using GZipTest.App.Main;
using System;

namespace GZipTest.App.Input
{
    public interface IFileReaderUow
    {
        Action ReadFileAction(string filePath, JobType jobType);
    }
}
