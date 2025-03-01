using hh_napi.Domain;
using hh_napi.Models;
using hh_napi.Models.Responses;
using hh_napi.Persistence.Repositories.Interfaces;
using hh_napi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hh_napi.Services;

public class DataPointService : BaseService<DataPoint>, IDataPointService
{

    public DataPointService(IUnitOfWork unitOfWork, ILogger<BaseService<DataPoint>> logger) : base(logger, unitOfWork) { }

    public async Task<DataPoint?> GetDataPointByIdAsync(int id, string? includeRelations = null)
    {
        var query = _unitOfWork.DataPoints.AsQueryable().AsNoTracking();
        query = ApplyIncludes(query, includeRelations);

        return await query.FirstOrDefaultAsync(dp => dp.Id == id);
    }
    public async Task<PagedResponse<DataPoint>> GetAllDataPointsAsync(int dataSourceId, PaginationParams pagination, string? includeRelations = null)
    {
        var query = _unitOfWork.DataPoints.AsQueryable().AsNoTracking();
        query = query.Where(dp => dp.DataSourceId == dataSourceId);

        query = ApplyIncludes(query, includeRelations);
        query = ApplySearch(query, pagination.Search);

        return await GetPagedResponseAsync(query, pagination);
    }

    public async Task<bool> CreateDataPointAsync(DataPoint dataPoint)
    {
        await _unitOfWork.DataPoints.AddAsync(dataPoint);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> DeleteDataPointAsync(int id)
    {
        var dataPoint = await _unitOfWork.DataPoints.GetByIdAsync(id);
        if (dataPoint == null)
        {
            _logger.LogWarning("DataPoint with id {Id} not found", id);
            return false;
        }

        _unitOfWork.DataPoints.Delete(dataPoint);
        return await _unitOfWork.SaveChangesAsync();
    }
}
