using hh_napi.Domain;
using hh_napi.Models;
using hh_napi.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace hh_napi.Services
{
    public class DataSourceService : IDataSourceService
    {
        private readonly IDataSourceRepository _dataSourceRepository;

        public DataSourceService(IDataSourceRepository dataSourceRepository)
        {
            _dataSourceRepository = dataSourceRepository;
        }

        public async Task<DataSource?> GetDataSourceByIdAsync(int id, bool includeRelations = false)
        {
            var query = _dataSourceRepository.AsQueryable();

            if (!includeRelations)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(ds => ds.Id == id);
        }
        public async Task<IEnumerable<DataSource>> GetAllDataSourcesAsync(PaginationParams pagination)
        {
            var query = _dataSourceRepository.AsQueryable();
            
            if (!string.IsNullOrEmpty(pagination.Search))
            {
                query = query.Where(ds => ds.Name.Contains(pagination.Search) || ds.Description.Contains(pagination.Search));
            }

            return await query
                .Skip(pagination.Offset)
                .Take(pagination.Limit)
                .ToListAsync();
        }

        public async Task<bool> CreateDataSourceAsync(DataSource dataSource)
        {
            await _dataSourceRepository.AddAsync(dataSource);
            return await _dataSourceRepository.SaveChangesAsync();
        }
    }
}