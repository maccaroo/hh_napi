using hh_napi.Domain;
using hh_napi.Models;
using hh_napi.Models.Responses;

namespace hh_napi.Services.Interfaces;

public interface IDataPointService
{
    Task<DataPoint?> GetDataPointByIdAsync(int id, string? includeRelations = null);
    Task<PagedResponse<DataPoint>> GetAllDataPointsAsync(int dataSourceId, PaginationParams pagination, string? includeRelations = null);
    Task<bool> CreateDataPointAsync(DataPoint dataPoint);
}
