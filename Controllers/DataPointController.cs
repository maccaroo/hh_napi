using hh_napi.Domain;
using hh_napi.Services;
using Microsoft.AspNetCore.Mvc;

namespace hh_napi.Controllers
{
    [ApiController]
    [Route("api/datapoints")]
    public class DataPointController : ControllerBase
    {
        private readonly IDataPointService _dataPointService;

        public DataPointController(IDataPointService dataPointService)
        {
            _dataPointService = dataPointService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDataPointById(int id)
        {
            var dataPoint = await _dataPointService.GetDataPointByIdAsync(id);
            return dataPoint != null ? Ok(dataPoint) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDataPoints()
        {
            var dataPoints = await _dataPointService.GetAllDataPointsAsync();
            return Ok(dataPoints);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDataPoint([FromBody] DataPoint dataPoint)
        {
            var success = await _dataPointService.CreateDataPointAsync(dataPoint);
            return success ? CreatedAtAction(nameof(GetDataPointById), new { id = dataPoint.Id }, dataPoint) : BadRequest();
        }
    }
}