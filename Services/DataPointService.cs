using hh_napi.Domain;
using hh_napi.Models;
using hh_napi.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IEnumerable<DataPoint>> GetAllDataPointsAsync(int dataSourceId, PaginationParams pagination){
            var query = _dataPointRepository.AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                query = query.Where(ds => ds.Value.Contains(pagination.Search));
            }

            return await query
                .Where(dp => dp.DataSourceId == dataSourceId)
                .Skip(pagination.Offset)
                .Take(pagination.Limit)
                .ToListAsync();
        }

        public async Task<bool> CreateDataPointAsync(DataPoint dataPoint)
        {
            await _dataPointRepository.AddAsync(dataPoint);
            return await _dataPointRepository.SaveChangesAsync();
        }
    }
}