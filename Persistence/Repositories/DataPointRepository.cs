using hh_napi.Domain;

namespace hh_napi.Persistence.Repositories
{
    public class DataPointRepository : Repository<DataPoint>, IDataPointRepository
    {
        public DataPointRepository(AppDbContext context) : base(context) { }
    }
}