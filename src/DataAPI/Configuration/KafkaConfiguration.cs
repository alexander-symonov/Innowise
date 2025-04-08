namespace DataAPI.Configuration
{
    public class KafkaConfiguration
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public int MessageTimeoutMs { get; set; }
    }
}
