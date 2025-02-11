using hh_napi.Domain;
using hh_napi.Models;
using hh_napi.Models.Responses;
using hh_napi.Persistence.Repositories.Interfaces;
using hh_napi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hh_napi.Services;

public class DataSourceService : BaseService<DataSource>, IDataSourceService
{
    private readonly IUnitOfWork _unitOfWork;

    public DataSourceService(IUnitOfWork unitOfWork, ILogger<BaseService<DataSource>> logger) : base(logger)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DataSource?> GetDataSourceByIdAsync(int id, string? includeRelations = null)
    {
        var query = _unitOfWork.DataSources.AsQueryable().AsNoTracking();
        query = ApplyIncludes(query, includeRelations);
        var dataSource = await query.FirstOrDefaultAsync(ds => ds.Id == id);
        
        if (dataSource == null)
        {
            _logger.LogWarning($"DataSource with id {id} not found");
        }

        return dataSource;
    }
    public async Task<PagedResponse<DataSource>> GetAllDataSourcesAsync(PaginationParams pagination, string? includeRelations = null)
    {
        var query = _unitOfWork.DataSources.AsQueryable().AsNoTracking();
        query = ApplyIncludes(query, includeRelations);
        query = ApplySearch(query, pagination.Search);

        return await GetPagedResponseAsync(query, pagination);
    }

    public async Task<bool> CreateDataSourceAsync(DataSource dataSource)
    {
        await _unitOfWork.DataSources.AddAsync(dataSource);
        return await _unitOfWork.SaveChangesAsync();
    }
}
