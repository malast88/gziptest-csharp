using GZipTest.App.Threading;

namespace GZipTest.App.Ouput
{
    public class FileWriter : IFileWriter
    {
        private readonly IThreading _threading;
        private readonly IFileWriterUow _uow;

        public FileWriter(IThreading threading,
            IFileWriterUow uow)
        {
            _threading = threading;
            _uow = uow;
        }

        public void WriteFile(string fileName)
        {
            _threading.ThreadStart(_uow.WriteFileAction(fileName));
        }
    }
}
