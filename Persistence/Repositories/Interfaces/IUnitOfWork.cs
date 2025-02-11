namespace hh_napi.Persistence.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IDataPointRepository DataPoints { get; }
    IDataSourceRepository DataSources { get; }
    IUserRepository Users { get; }
    IUserCredentialsRepository UserCredentials { get; }
    Task<bool> SaveChangesAsync();
}