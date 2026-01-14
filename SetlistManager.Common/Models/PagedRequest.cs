namespace SetlistManager.Common.Models;

public class PagedRequest
{
    public int PageSize { get; set; }
    public int PageIndex { get; set; } = 0;
    public string? Query { get; set; }
    public ContentType ContentType { get; set; } = ContentType.All;
}

public enum ContentType
{
    Private,
    Public,
    All
}