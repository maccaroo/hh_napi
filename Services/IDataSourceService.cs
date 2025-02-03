using hh_napi.Domain;

namespace hh_napi.Services
{
    public interface IDataSourceService
    {
        Task<DataSource?> GetDataSourceByIdAsync(int id);
        Task<IEnumerable<DataSource>> GetAllDataSourcesAsync();
        Task<bool> CreateDataSourceAsync(DataSource dataSource);
    }
}