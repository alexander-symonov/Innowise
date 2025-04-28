using DTO.InsuranceIncidents;

namespace DataReader.Services
{
    interface ISaveInsuranceDataService
    {
        void SaveCarIncident(CarIncident message);
        void SaveInsuranceData(string insuranceData, string type);
    }
}
