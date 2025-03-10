using AutoMapper;
using hh_napi.Domain;
using hh_napi.Models;
using hh_napi.Models.Responses;
using hh_napi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hh_napi.Controllers
{
    [ApiController]
    [Route("api/datasources/{dataSourceId}/datapoints")]
    [Authorize]
    [ApiVersion("1.0")]
    public class DataPointController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDataPointService _dataPointService;

        public DataPointController(IMapper mapper, IDataPointService dataPointService)
        {
            _mapper = mapper;
            _dataPointService = dataPointService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDataPointById(int dataSourceId, int id, [FromQuery] string? includeRelations = null)
        {
            var dataPoint = await _dataPointService.GetDataPointByIdAsync(id, includeRelations);
            return dataPoint != null ? Ok(_mapper.Map<DataPointResponse>(dataPoint)) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDataPoints(int dataSourceId, [FromQuery] PaginationParams pagination)
        {
            var pagedDataPoints = await _dataPointService.GetAllDataPointsAsync(dataSourceId, pagination);
            return Ok(pagedDataPoints.ConvertTo<DataPointResponse>(_mapper));
        }

        [HttpPost]
        public async Task<IActionResult> CreateDataPoint(int dataSourceId, [FromBody] DataPoint dataPoint)
        {
            dataPoint.DataSourceId = dataSourceId;

            var success = await _dataPointService.CreateDataPointAsync(dataPoint);
            return success ? CreatedAtAction(nameof(GetDataPointById), new { dataSourceId = dataPoint.DataSourceId, id = dataPoint.Id }, dataPoint) : BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDataPoint(int dataSourceId, int id)
        {
            var success = await _dataPointService.DeleteDataPointAsync(id);
            return success ? NoContent() : BadRequest();
        }
    }
}