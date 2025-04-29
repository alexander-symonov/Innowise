namespace DataReader.Settings
{
    public class KafkaSettings
    {
        public string BrokersList { get; set; }
        public string TopicPrefix { get; set; }
        public string GroupPrefix { get; set; }
    }
}
