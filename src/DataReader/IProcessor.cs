namespace DataReader
{
    public interface IProcessor
    {
        public Task ProcessAsync(byte[] data, CancellationToken cancellationToken);
    }
}
