using MediaJournal.Models.Entities;

namespace MediaJournal.Web.Models;

public class MediaLibraryViewModel
{
    public IEnumerable<Media> MediaItems { get; set; }

    public Dictionary<MediaType, int> TypeCounts =>
        MediaItems.GroupBy(m => m.Type)
            .ToDictionary(g => g.Key, g => g.Count());

    public int TotalItems => MediaItems.Count();
}