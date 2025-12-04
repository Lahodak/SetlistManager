namespace SetlistManager.Common.Models;

public class PagedRequest
{
    public int PageSize { get; set; }
    public int PageIndex { get; set; } = 0;
    public string? Query { get; set; }
}