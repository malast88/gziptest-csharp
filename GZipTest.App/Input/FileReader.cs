using GZipTest.App.Main;
using GZipTest.App.Threading;

namespace GZipTest.App.Input
{
    public class FileReader : IFileReader
    {
        private readonly IThreading _threading;
        private readonly IFileReaderUow _uow;

        public FileReader(IThreading threading,
            IFileReaderUow uow)
        {
            _threading = threading;
            _uow = uow;
        }

        public void ReadFile(string filePath, JobType jobType)
        {
            _threading.ThreadStart(_uow.ReadFileAction(filePath, jobType));
        }
    }
}
