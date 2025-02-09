using AutoMapper;
using hh_napi.Domain;
using hh_napi.Models;
using hh_napi.Models.Responses;
using hh_napi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace hh_napi.Controllers
{
    [ApiController]
    [Route("api/datasources/{dataSourceId}/datapoints")]
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
        public async Task<IActionResult> GetDataPointById(int dataSourceId, int id)
        {
            var dataPoint = await _dataPointService.GetDataPointByIdAsync(id);
            return dataPoint != null ? Ok(_mapper.Map<DataPointResponse>(dataPoint)) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDataPoints(int dataSourceId, [FromQuery] PaginationParams pagination)
        {
            var dataPoints = await _dataPointService.GetAllDataPointsAsync(dataSourceId, pagination);
            var response = _mapper.Map<IEnumerable<DataPointResponse>>(dataPoints);
            return Ok(response);
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