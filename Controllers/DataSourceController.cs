using hh_napi.Domain;
using hh_napi.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAllDataSources()
        {
            var dataSources = await _dataSourceService.GetAllDataSourcesAsync();
            return Ok(dataSources);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDataSource([FromBody] DataSource dataSource)
        {
            var success = await _dataSourceService.CreateDataSourceAsync(dataSource);
            return success ? CreatedAtAction(nameof(GetDataSourceById), new { id = dataSource.Id }, dataSource) : BadRequest();
        }
    }
}