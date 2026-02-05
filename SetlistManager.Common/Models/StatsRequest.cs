using System.ComponentModel.DataAnnotations;

namespace SetlistManager.Common.Models;

public class StatsRequest
{
    [Required]
    public StatsSubject? Subject { get; set; }
    [Required]
    public StatsRange? Range { get; set; }
    [Required]
    public StatsMetric? Metric { get; set; }
    [Required]
    [Range(1, 100)]
    public int? Limit { get; set; } = 10;
}