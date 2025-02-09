using hh_napi.Domain;
using hh_napi.Models;

namespace hh_napi.Services.Interfaces;

public interface IDataPointService
{
    Task<DataPoint?> GetDataPointByIdAsync(int id, string? includeRelations = null);
    Task<IEnumerable<DataPoint>> GetAllDataPointsAsync(int dataSourceId, PaginationParams pagination, string? includeRelations = null);
    Task<bool> CreateDataPointAsync(DataPoint dataPoint);
}
