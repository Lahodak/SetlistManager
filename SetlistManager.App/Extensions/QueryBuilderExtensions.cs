using Microsoft.AspNetCore.Http.Extensions;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Extensions;

public static class QueryBuilderExtensions
{
    public static string ToPagedRequestUri(this PagedRequest request, string endpoint)
    {
        var queryBuilder = new QueryBuilder
        {
            { nameof(request.PageSize), request.PageSize.ToString() },
            { nameof(request.PageIndex), request.PageIndex.ToString() }
        };

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            queryBuilder.Add(nameof(request.Query), request.Query);
        }

        var uri = new UriBuilder(endpoint)
        {
            Query = queryBuilder.ToString()
        };

        return uri.ToString();
    }

    public static string ToContentPagedRequestUri(this ContentPagedRequest request, string endpoint)
    {
        var queryBuilder = new QueryBuilder
        {
            { nameof(request.PageSize), request.PageSize.ToString() },
            { nameof(request.PageIndex), request.PageIndex.ToString() }
        };

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            queryBuilder.Add(nameof(request.Query), request.Query);
        }

        queryBuilder.Add(nameof(request.ContentType), request.ContentType.ToString());

        var uri = new UriBuilder(endpoint)
        {
            Query = queryBuilder.ToString()
        };

        return uri.ToString();
    }

    public static string ToUri(this StatsRequest request, string endpoint)
    {
        var queryBuilder = new QueryBuilder
        {
            { nameof(request.Subject), request.Subject.ToString() },
            { nameof(request.Metric), request.Metric.ToString() },
            { nameof(request.Range), request.Range.ToString() },
            { nameof(request.Limit), request.Limit.ToString() }
        };

        var uri = new UriBuilder(endpoint)
        {
            Query = queryBuilder.ToString()
        };

        return uri.ToString();
    }
}