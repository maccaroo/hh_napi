using hh_napi.Domain;
using hh_napi.Models;
using hh_napi.Services;
using Microsoft.AspNetCore.Mvc;

namespace hh_napi.Controllers
{
    [ApiController]
    [Route("api/datasources/{dataSourceId}/datapoints")]
    public class DataPointController : ControllerBase
    {
        private readonly IDataPointService _dataPointService;

        public DataPointController(IDataPointService dataPointService)
        {
            _dataPointService = dataPointService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDataPointById(int dataSourceId, int id)
        {
            var dataPoint = await _dataPointService.GetDataPointByIdAsync(id);
            return dataPoint != null ? Ok(dataPoint) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDataPoints(int dataSourceId, [FromQuery] PaginationParams pagination)
        {
            var dataPoints = await _dataPointService.GetAllDataPointsAsync(dataSourceId, pagination);
            return Ok(dataPoints);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDataPoint(int dataSourceId, [FromBody] DataPoint dataPoint)
        {
            dataPoint.DataSourceId = dataSourceId;
            
            var success = await _dataPointService.CreateDataPointAsync(dataPoint);
            return success ? CreatedAtAction(nameof(GetDataPointById), new { dataSourceId = dataPoint.DataSourceId, id = dataPoint.Id }, dataPoint) : BadRequest();
        }
    }
}