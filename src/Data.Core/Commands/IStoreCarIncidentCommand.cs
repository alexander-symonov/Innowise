using DTO.InsuranceIncidents;

namespace Data.Core.Commands
{
    public interface IStoreCarIncidentCommand
    {
        public Task ExecuteAsync(CarIncident carIncident, CancellationToken cancellationToken);
    }
}
