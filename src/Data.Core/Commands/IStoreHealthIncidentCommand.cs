using DTO.InsuranceIncidents;

namespace Data.Core.Commands
{
    public interface IStoreHealthIncidentCommand
    {
        public Task ExecuteAsync(HealthIncident carIncident, CancellationToken cancellationToken);
    }
}
