using Microsoft.EntityFrameworkCore;
using hh_napi.Models.Responses;
using System.Linq.Dynamic.Core;
using hh_napi.Attributes;
using System.Reflection;

namespace hh_napi.Services;

public abstract class BaseService<T> where T : class
{
    protected IQueryable<T> ApplyIncludes(IQueryable<T> query, string? includeRelations = null)
    {
        if (!string.IsNullOrEmpty(includeRelations))
        {
            var relations = includeRelations.Split(',');
            foreach (var relation in relations)
            {
                Console.WriteLine($"Including relation: {relation.Trim()}");  // Debugging Output
                query = query.Include(relation.Trim());
            }
        }

        return query;
    }

    protected IQueryable<T> ApplySearch(IQueryable<T> query, string? search)
    {
        var searchableFields = typeof(T)
            .GetProperties()
            .Where(p => p.GetCustomAttribute<SearchableAttribute>() != null)
            .Select(p => p.Name)
            .ToArray();

        if (!string.IsNullOrEmpty(search) && searchableFields.Length > 0)
        {
            var searchFilters = searchableFields
                .Select(field => $"({field}.ToLower().Contains(@0))")
                .ToArray();

            var searchQuery = string.Join(" || ", searchFilters);
            query = query.Where(searchQuery, search.ToLower().Trim());
        }

        return query;
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