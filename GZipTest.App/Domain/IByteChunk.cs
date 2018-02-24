namespace GZipTest.App.Domain
{
    public interface IByteChunk: IId
    {
        byte[] Data { get; set; }
    }
}
