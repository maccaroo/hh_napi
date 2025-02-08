using hh_napi.Domain;
using hh_napi.Models;

namespace hh_napi.Services
{
    public interface IDataSourceService
    {
        Task<DataSource?> GetDataSourceByIdAsync(int id, bool includeRelations = false);
        Task<IEnumerable<DataSource>> GetAllDataSourcesAsync(PaginationParams pagination);
        Task<bool> CreateDataSourceAsync(DataSource dataSource);
    }
}