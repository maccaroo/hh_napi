using Microsoft.EntityFrameworkCore;
using hh_napi.Models.Responses;
using System.Linq.Dynamic.Core;
using hh_napi.Attributes;
using System.Reflection;
using hh_napi.Models;
using System.Linq.Expressions;

namespace hh_napi.Services;

public abstract class BaseService<T> where T : class
{
    protected readonly ILogger<BaseService<T>> _logger;

    protected BaseService(ILogger<BaseService<T>> logger)
    {
        _logger = logger;
    }

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

    protected async Task<PagedResponse<T>> GetPagedResponseAsync(
        IQueryable<T> query,
        PaginationParams pagination)
    {
        var defaultOrderProperty = typeof(T)
            .GetProperties()
            .FirstOrDefault(p => p.GetCustomAttribute<DefaultOrderByAttribute>() != null);

        if (defaultOrderProperty != null)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var property = Expression.Property(param, defaultOrderProperty.Name);
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), defaultOrderProperty.PropertyType);
            var lambda = Expression.Lambda(delegateType, property, param);  // p => p.defaultOrderProperty

            // Find the OrderBy method
            var orderByMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), defaultOrderProperty.PropertyType);

            query = (IQueryable<T>)orderByMethod.Invoke(null, [query, lambda])!;
        }
        else
        {
            _logger.LogWarning($"No default ordering field defined for {typeof(T).Name}.  Add [DefaultOrderBy] to a property.");
        }

        var total = query.Count();
        var data = await query.Skip(pagination.Offset).Take(pagination.Limit).ToListAsync();
        return new PagedResponse<T>(data, pagination.Offset, pagination.Limit, total);
    }
}