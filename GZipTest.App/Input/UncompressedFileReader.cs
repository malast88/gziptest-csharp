using GZipTest.App.Threading;

namespace GZipTest.App.Input
{
    public class UncompressedFileReader : IUncompressedFileReader
    {
        private readonly IThreading _threading;
        private readonly IUncompressedFileReaderUow _uow;

        public UncompressedFileReader(IThreading threading,
            IUncompressedFileReaderUow uow)
        {
            _threading = threading;
            _uow = uow;
        }

        public void ReadFile(string filePath)
        {
            _threading.ThreadStart(_uow.ReadFileAction(filePath));
        }
    }
}
