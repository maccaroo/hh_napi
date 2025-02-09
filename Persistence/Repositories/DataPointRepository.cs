using hh_napi.Domain;
using hh_napi.Persistence.Repositories.Interfaces;

namespace hh_napi.Persistence.Repositories;

public class DataPointRepository : Repository<DataPoint>, IDataPointRepository
{
    public DataPointRepository(AppDbContext context) : base(context) { }
}
