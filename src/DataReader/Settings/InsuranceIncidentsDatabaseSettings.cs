namespace DataReader.Settings
{
    public class InsuranceIncidentsDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string CarIncidentsCollectionName { get; set; } = null!;
        public string FlatIncidentsCollectionName { get; set; } = null!;
        public string HealthIncidentsCollectionName { get; set; } = null!;
    }
}
