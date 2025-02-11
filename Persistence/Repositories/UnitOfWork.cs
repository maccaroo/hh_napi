using hh_napi.Persistence.Repositories.Interfaces;

namespace hh_napi.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private bool _disposed = false;

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
        Dispose(true);
        GC.SuppressFinalize(this);  // Avoid unnecessary finalization
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed) // Prevent multiple disposals
        {
            if (disposing)
            {
                // Dispose managed resources here
                _context.Dispose();
            }

            // Dispose unmanaged resources here

            _disposed = true;
        }
    }
}