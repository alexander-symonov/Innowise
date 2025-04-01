namespace DataReader
{
    interface IProcessor
    {
        public Task ProcessAsync(byte[] data, CancellationToken cancellationToken);
    }
}
