namespace SetlistManager.Common.Brevo.Models;

public class EmailTemplate
{
    public string Subject { get; set; } = default!;
    public string HtmlContent { get; set; } = default!;
    public string TextContent { get; set; } = default!;
}