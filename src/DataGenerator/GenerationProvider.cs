namespace DataGenerator
{
    internal class GenerationProvider
    {
        private EventGenerator _eventGenerator = new EventGenerator();
        public string BrokersList { get; set; } = string.Empty;
        public string ApiUri { get; set; } = string.Empty;
        public bool ByApi { get; set; }
        public int DelayMs { get; set; }
        public string TopicNamePrefix { get; internal set; } = string.Empty;
        public bool Active { get; set; } = true;

        public async Task Generate(string type, int count, StateMediator state)
        {
            if (ByApi)
            {
                await _eventGenerator.SendMessageToApi(ApiUri, type, count, DelayMs, state);
            }
            else
            {
                await _eventGenerator.GenerateForKafka(BrokersList, TopicNamePrefix, type, count, DelayMs, state);
            }
        }
    }
}
