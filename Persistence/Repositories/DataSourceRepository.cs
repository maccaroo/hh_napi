using hh_napi.Domain;

namespace hh_napi.Persistence.Repositories
{
    public class DataSourceRepository : Repository<DataSource>, IDataSourceRepository
    {
        public DataSourceRepository(AppDbContext context) : base(context) { }
    }
}