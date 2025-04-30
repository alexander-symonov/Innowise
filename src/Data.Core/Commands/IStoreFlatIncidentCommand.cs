using DTO.InsuranceIncidents;

namespace Data.Core.Commands
{
    public interface IStoreFlatIncidentCommand
    {
        public Task ExecuteAsync(FlatIncident carIncident, CancellationToken cancellationToken);
    }
}
