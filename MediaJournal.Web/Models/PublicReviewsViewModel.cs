using MediaJournal.Models.Entities;

namespace MediaJournal.Web.Models;

public class PublicReviewsViewModel
{
    public IEnumerable<Media> MediaItems { get; set; } = new List<Media>();

    public Dictionary<MediaType, int> TypeCounts =>
        MediaItems.GroupBy(m => m.Type)
            .ToDictionary(g => g.Key, g => g.Count());

    public int TotalItems => MediaItems.Count();
}