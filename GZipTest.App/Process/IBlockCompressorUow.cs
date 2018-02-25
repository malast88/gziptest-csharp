using GZipTest.App.Main;
using System;

namespace GZipTest.App.Process
{
    public interface IBlockCompressorUow
    {
        Action CompressAction(JobType jobType);
    }
}
