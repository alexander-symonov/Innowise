namespace DataReader
{
    public interface IProcessorFactory
    {
        IProcessor CreateProcessor(string processorType);
    }
}
