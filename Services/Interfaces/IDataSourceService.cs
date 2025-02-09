using hh_napi.Domain;
using hh_napi.Models;
using hh_napi.Models.Responses;

namespace hh_napi.Services.Interfaces;

public interface IDataSourceService
{
    Task<DataSource?> GetDataSourceByIdAsync(int id, string? includeRelations = null);
    Task<PagedResponse<DataSource>> GetAllDataSourcesAsync(PaginationParams pagination, string? includeRelations = null);
    Task<bool> CreateDataSourceAsync(DataSource dataSource);
}
