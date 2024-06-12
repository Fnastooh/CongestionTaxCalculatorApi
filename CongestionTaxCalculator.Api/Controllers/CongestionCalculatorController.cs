using congestion.calculator;
using CongestionTaxCalculator.Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace CongestionTaxCalculator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CongestionCalculatorController : ControllerBase
    {
        private ICalculationHandler _calculationHandler;
        public CongestionCalculatorController(ICalculationHandler calculationHandler)
        {
            _calculationHandler = calculationHandler;
        }
        [HttpPost("CalculateData")]
        public async Task<IActionResult> CalculateData([FromBody] List<VehicleData> inputData)
        { 
            var result = await _calculationHandler.GetResults(inputData);
            return Ok(result);
        }
    }
}
