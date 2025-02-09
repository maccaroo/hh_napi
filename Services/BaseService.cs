using hh_napi.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace hh_napi.Services;

public abstract class BaseService<T> where T : class
{
    protected IQueryable<T> ApplyPagination(IQueryable<T> query, int offset, int limit, out int total)
    {
        total = query.Count();
        return query.Skip(offset).Take(limit);
    }

    protected async Task<PagedResponse<TResult>> GetPagedResponseAsync<TResult>(
        IQueryable<TResult> query,
        int offset,
        int limit)
    {
        var total = query.Count();
        var data = await query.Skip(offset).Take(limit).ToListAsync();
        return new PagedResponse<TResult>(data, offset, limit, total);
    }
}