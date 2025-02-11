using hh_napi.Persistence.Repositories.Interfaces;

namespace hh_napi.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IDataPointRepository DataPoints { get; }
    public IDataSourceRepository DataSources { get; }
    public IUserRepository Users { get; }
    public IUserCredentialsRepository UserCredentials { get; }

    public UnitOfWork(
        AppDbContext context,
        IDataPointRepository dataPoints,
        IDataSourceRepository dataSources,
        IUserRepository users,
        IUserCredentialsRepository userCredentials
        )
    {
        _context = context;
        DataPoints = dataPoints;
        DataSources = dataSources;
        Users = users;
        UserCredentials = userCredentials;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}