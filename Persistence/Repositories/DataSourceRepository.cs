using hh_napi.Domain;
using hh_napi.Persistence.Repositories.Interfaces;

namespace hh_napi.Persistence.Repositories;

public class DataSourceRepository : Repository<DataSource>, IDataSourceRepository
{
    public DataSourceRepository(AppDbContext context) : base(context) { }
}
