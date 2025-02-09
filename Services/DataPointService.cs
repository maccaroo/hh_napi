using hh_napi.Domain;
using hh_napi.Models;
using hh_napi.Models.Responses;
using hh_napi.Persistence.Repositories;
using hh_napi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hh_napi.Services
{
    public class DataPointService : BaseService<DataPoint>, IDataPointService
    {
        private readonly IDataPointRepository _dataPointRepository;

        public DataPointService(IDataPointRepository dataPointRepository)
        {
            _dataPointRepository = dataPointRepository;
        }

        public async Task<DataPoint?> GetDataPointByIdAsync(int id, string? includeRelations = null)
        {
            var query = _dataPointRepository.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(includeRelations))
            {
                var relations = includeRelations.Split(',');
                foreach (var relation in relations)
                {
                    query = query.Include(relation.Trim());
                }
            }

            return await query.FirstOrDefaultAsync(dp => dp.Id == id);
        }
        public async Task<PagedResponse<DataPoint>> GetAllDataPointsAsync(int dataSourceId, PaginationParams pagination, string? includeRelations = null)
        {
            var query = _dataPointRepository.AsQueryable().AsNoTracking();

            if (!string.IsNullOrEmpty(includeRelations))
            {
                var relations = includeRelations.Split(',');
                foreach (var relation in relations)
                {
                    query = query.Include(relation.Trim());
                }
            }

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                query = query.Where(dp => dp.Value.Contains(pagination.Search));
            }

            return await GetPagedResponseAsync(query, pagination.Offset, pagination.Limit);
        }

        public async Task<bool> CreateDataPointAsync(DataPoint dataPoint)
        {
            await _dataPointRepository.AddAsync(dataPoint);
            return await _dataPointRepository.SaveChangesAsync();
        }
    }
}