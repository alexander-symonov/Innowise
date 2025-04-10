namespace DataGenerator
{
    public class StateMediator
    {
        public volatile int SucsessCounter;
        public volatile int FailedCounter;

        public CancellationToken Token { get; }

        public StateMediator(CancellationToken token)
        {
            Token = token;
        }
    }
}
