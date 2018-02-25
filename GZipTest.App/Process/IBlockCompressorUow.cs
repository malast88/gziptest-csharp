using System;

namespace GZipTest.App.Process
{
    public interface IBlockCompressorUow
    {
        Func<object> WorkMethod { get; }
    }
}
