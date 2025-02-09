using hh_napi.Domain;
using hh_napi.Models;
using hh_napi.Models.Responses;
using hh_napi.Persistence.Repositories;
using hh_napi.Persistence.Repositories.Interfaces;
using hh_napi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hh_napi.Services;

public class DataSourceService : BaseService<DataSource>, IDataSourceService
{
    private readonly IDataSourceRepository _dataSourceRepository;

    public DataSourceService(IDataSourceRepository dataSourceRepository)
    {
        _dataSourceRepository = dataSourceRepository;
    }

    public async Task<DataSource?> GetDataSourceByIdAsync(int id, string? includeRelations = null)
    {
        var query = _dataSourceRepository.AsQueryable().AsNoTracking();
        query = ApplyIncludes(query, includeRelations);

        return await query.FirstOrDefaultAsync(ds => ds.Id == id);
    }
    public async Task<PagedResponse<DataSource>> GetAllDataSourcesAsync(PaginationParams pagination, string? includeRelations = null)
    {
        var query = _dataSourceRepository.AsQueryable().AsNoTracking();
        query = ApplyIncludes(query, includeRelations);
        query = ApplySearch(query, pagination.Search);

        return await GetPagedResponseAsync(query, pagination.Offset, pagination.Limit);
    }

    public async Task<bool> CreateDataSourceAsync(DataSource dataSource)
    {
        await _dataSourceRepository.AddAsync(dataSource);
        return await _dataSourceRepository.SaveChangesAsync();
    }
}
