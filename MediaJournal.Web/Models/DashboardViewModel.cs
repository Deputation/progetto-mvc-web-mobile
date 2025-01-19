using MediaJournal.Models.Entities;

namespace MediaJournal.Web.Models;

public class DashboardViewModel
{
    public List<Media> MediaItems { get; set; } = new();
    public int TotalItems { get; set; }
    public Dictionary<MediaType, int> ItemsByType { get; set; } = new();
    public double AverageRating { get; set; }
    public Dictionary<int, int> RatingDistribution { get; set; } = new();
    public List<Media> RecentItems { get; set; } = new();
    public MediaType MostReviewedType { get; set; }
}