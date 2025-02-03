using hh_napi.Domain;

namespace hh_napi.Services
{
    public interface IDataPointService
    {
        Task<DataPoint?> GetDataPointByIdAsync(int id);
        Task<IEnumerable<DataPoint>> GetAllDataPointsAsync();
        Task<bool> CreateDataPointAsync(DataPoint dataPoint);
    }
}