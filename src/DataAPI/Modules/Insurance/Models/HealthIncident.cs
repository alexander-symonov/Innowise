namespace DataAPI.Modules.Insurance.Models
{
    public class HealthIncident
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public HumanAddress Address { get; set; } = new HumanAddress();

        public class HumanAddress
        {
            public string PostalCode { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Street { get; set; } = string.Empty;
            public string Building { get; set; } = string.Empty;
        }
    }
}
