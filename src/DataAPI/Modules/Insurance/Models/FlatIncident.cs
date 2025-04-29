namespace DataAPI.Modules.Insurance.Models
{
    public class FlatIncident
    {
        public required FlatAddress Address { get; set; }
        public required string OwnerNumber { get; set; }
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
        public class FlatAddress
        {
            public required string PostalCode { get; set; }
            public required string Country { get; set; }
            public required string City { get; set; }
            public required string Street { get; set; }
            public required string Building { get; set; }
        }
    }
}
