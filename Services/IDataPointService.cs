using hh_napi.Domain;
using hh_napi.Models;

namespace hh_napi.Services
{
    public interface IDataPointService
    {
        Task<DataPoint?> GetDataPointByIdAsync(int id);
        Task<IEnumerable<DataPoint>> GetAllDataPointsAsync(int dataSourceId, PaginationParams pagination);
        Task<bool> CreateDataPointAsync(DataPoint dataPoint);
    }
}