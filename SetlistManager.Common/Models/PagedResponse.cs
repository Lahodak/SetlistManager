namespace SetlistManager.Common.Models;

public class PagedResponse<T>
{
    public int TotalCount { get; set; }
    public List<T> Items { get; set; } = [];
}