using hh_napi.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using hh_napi.Models;
using hh_napi.Services.Interfaces;
using AutoMapper;
using hh_napi.Models.Responses;

namespace hh_napi.Controllers
{
    [ApiController]
    [Route("api/datasources")]
    public class DataSourceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDataSourceService _dataSourceService;

        public DataSourceController(IMapper mapper, IDataSourceService dataSourceService)
        {
            _mapper = mapper;
            _dataSourceService = dataSourceService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDataSourceById(int id, [FromQuery] string? includeRelations = null)
        {
            var dataSource = await _dataSourceService.GetDataSourceByIdAsync(id, includeRelations);
            return dataSource != null ? Ok(_mapper.Map<DataSourceResponse>(dataSource)) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDataSources([FromQuery] PaginationParams pagination, [FromQuery] string? includeRelations = null)
        {
            var pagedDataSources = await _dataSourceService.GetAllDataSourcesAsync(pagination, includeRelations);
            return Ok(pagedDataSources.ConvertTo<DataSourceResponse>(_mapper));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateDataSource([FromBody] DataSource dataSource)
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            dataSource.CreatedByUserId = int.Parse(userIdClaim.Value);


            var success = await _dataSourceService.CreateDataSourceAsync(dataSource);
            return success ? CreatedAtAction(nameof(GetDataSourceById), new { id = dataSource.Id }, _mapper.Map<DataSourceResponse>(dataSource)) : BadRequest();
        }

        [HttpGet("{id}/summary")]
        public async Task<IActionResult> GetDataSourceSummary(int id)
        {
            var dataSourceSummary = await _dataSourceService.GetDataSourceSummaryAsync(id);
            return dataSourceSummary != null ? Ok(_mapper.Map<DataSourceSummaryResponse>(dataSourceSummary)) : NotFound();
        }
    }
}