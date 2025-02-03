using hh_napi.Domain;
using hh_napi.Persistence.Repositories;

namespace hh_napi.Services
{
    public class DataSourceService : IDataSourceService
    {
        private readonly IDataSourceRepository _dataSourceRepository;

        public DataSourceService(IDataSourceRepository dataSourceRepository)
        {
            _dataSourceRepository = dataSourceRepository;
        }

        public async Task<DataSource?> GetDataSourceByIdAsync(int id) => await _dataSourceRepository.GetByIdAsync(id);
        public async Task<IEnumerable<DataSource>> GetAllDataSourcesAsync() => await _dataSourceRepository.GetAllAsync();

        public async Task<bool> CreateDataSourceAsync(DataSource dataSource)
        {
            await _dataSourceRepository.AddAsync(dataSource);
            return await _dataSourceRepository.SaveChangesAsync();
        }
    }
}