using hh_napi.Domain;
using hh_napi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using hh_napi.Models;

namespace hh_napi.Controllers
{
    [ApiController]
    [Route("api/datasources")]
    public class DataSourceController : ControllerBase
    {
        private readonly IDataSourceService _dataSourceService;

        public DataSourceController(IDataSourceService dataSourceService)
        {
            _dataSourceService = dataSourceService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDataSourceById(int id)
        {
            var dataSource = await _dataSourceService.GetDataSourceByIdAsync(id);
            return dataSource != null ? Ok(dataSource) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDataSources([FromQuery] PaginationParams pagination)
        {
            var dataSources = await _dataSourceService.GetAllDataSourcesAsync(pagination);
            return Ok(dataSources);
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
            return success ? CreatedAtAction(nameof(GetDataSourceById), new { id = dataSource.Id }, dataSource) : BadRequest();
        }
    }
}