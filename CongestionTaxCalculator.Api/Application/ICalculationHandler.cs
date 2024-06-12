using congestion.calculator;

namespace CongestionTaxCalculator.Api.Application
{
    public interface ICalculationHandler
    {
        Task<VehicleTaxResult[]> GetResults(List<VehicleData> inputData);
    }
}