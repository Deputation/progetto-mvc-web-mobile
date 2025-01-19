using MediaJournal.Models.Entities;

namespace MediaJournal.Web.Models;

public class MediaDetailsViewModel
{
    public Media Media { get; set; } = null!;
    public bool HasReview => !string.IsNullOrWhiteSpace(Media.Review);
}