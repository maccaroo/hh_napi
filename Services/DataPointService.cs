using hh_napi.Domain;
using hh_napi.Persistence.Repositories;

namespace hh_napi.Services
{
    public class DataPointService : IDataPointService
    {
        private readonly IDataPointRepository _dataPointRepository;

        public DataPointService(IDataPointRepository dataPointRepository)
        {
            _dataPointRepository = dataPointRepository;
        }

        public async Task<DataPoint?> GetDataPointByIdAsync(int id) => await _dataPointRepository.GetByIdAsync(id);
        public async Task<IEnumerable<DataPoint>> GetAllDataPointsAsync() => await _dataPointRepository.GetAllAsync();

        public async Task<bool> CreateDataPointAsync(DataPoint dataPoint)
        {
            await _dataPointRepository.AddAsync(dataPoint);
            return await _dataPointRepository.SaveChangesAsync();
        }
    }
}