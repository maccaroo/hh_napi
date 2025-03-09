using System.Collections.Concurrent;
// using Microsoft.Extensions.Options;

namespace hh_napi.Services;

public interface ILoginAttemptService
{
    Task<bool> IsUserLockedOutAsync(string username);
    Task RecordFailedAttemptAsync(string username);
    Task RecordSuccessfulAttemptAsync(string username);
}

public class LoginAttemptService : ILoginAttemptService
{
    private readonly ILogger<LoginAttemptService> _logger;
    private readonly ConcurrentDictionary<string, UserLoginAttempts> _loginAttempts = new();
    private readonly int _maxFailedAttempts;
    private readonly TimeSpan _lockoutDuration;

    public LoginAttemptService(
        ILogger<LoginAttemptService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        
        var rateLimitSettings = configuration.GetSection("RateLimitSettings");
        _maxFailedAttempts = int.Parse(rateLimitSettings["LoginUsernameMaxFailedAttempts"] ?? "5");
        _lockoutDuration = TimeSpan.FromMinutes(int.Parse(rateLimitSettings["LoginUsernameLockoutDurationMinutes"] ?? "15"));
    }

    public Task<bool> IsUserLockedOutAsync(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return Task.FromResult(false);
        }

        var normalizedUsername = username.ToLowerInvariant();
        
        if (_loginAttempts.TryGetValue(normalizedUsername, out var attempts))
        {
            // Check if user is locked out
            if (attempts.FailedCount >= _maxFailedAttempts && 
                DateTime.UtcNow < attempts.LastFailedAttempt.Add(_lockoutDuration))
            {
                _logger.LogWarning("User {Username} is locked out due to too many failed login attempts", username);
                return Task.FromResult(true);
            }
            
            // If lockout period has expired, reset the counter
            if (attempts.FailedCount >= _maxFailedAttempts && 
                DateTime.UtcNow >= attempts.LastFailedAttempt.Add(_lockoutDuration))
            {
                _logger.LogInformation("Lockout period expired for user {Username}, resetting failed attempts", username);
                attempts.FailedCount = 0;
            }
        }
        
        return Task.FromResult(false);
    }

    public Task RecordFailedAttemptAsync(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return Task.CompletedTask;
        }

        var normalizedUsername = username.ToLowerInvariant();
        
        _loginAttempts.AddOrUpdate(
            normalizedUsername,
            // Add new record if username doesn't exist
            _ => new UserLoginAttempts { FailedCount = 1, LastFailedAttempt = DateTime.UtcNow },
            // Update existing record
            (_, attempts) =>
            {
                attempts.FailedCount++;
                attempts.LastFailedAttempt = DateTime.UtcNow;
                return attempts;
            });
        
        var attempts = _loginAttempts[normalizedUsername];
        _logger.LogWarning("Failed login attempt for user {Username}. Attempt {Count} of {Max}",
            username, attempts.FailedCount, _maxFailedAttempts);
        
        return Task.CompletedTask;
    }

    public Task RecordSuccessfulAttemptAsync(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return Task.CompletedTask;
        }

        var normalizedUsername = username.ToLowerInvariant();
        
        // Reset failed attempts on successful login
        if (_loginAttempts.TryGetValue(normalizedUsername, out var attempts))
        {
            attempts.FailedCount = 0;
            attempts.LastSuccessfulAttempt = DateTime.UtcNow;
            _logger.LogInformation("Successful login for user {Username}, reset failed attempts counter", username);
        }
        
        return Task.CompletedTask;
    }

    private class UserLoginAttempts
    {
        public int FailedCount { get; set; }
        public DateTime LastFailedAttempt { get; set; }
        public DateTime LastSuccessfulAttempt { get; set; }
    }
}
