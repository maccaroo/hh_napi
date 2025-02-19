using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace hh_napi.Persistence.Repositories.Extensions;

public static class RepositoryExtensions
{
    public static async Task<int> CountAsync<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate)
    {
        return await query.Where(predicate).CountAsync();
    }
}