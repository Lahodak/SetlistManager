using Microsoft.EntityFrameworkCore;
using SetlistManager.Common.Models;

namespace SetlistManager.Business.Extentions;

public static class PaginationExtension
{
    public static async Task<PagedResponse<T>> ToPaginatedResultAsync<T>(this IQueryable<T> source, PagedRequest request)
    {
        var items = await source.Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        var count = await source.CountAsync();

        return new PagedResponse<T>
        {
            TotalCount = count,
            Items = items
        };
    }
}