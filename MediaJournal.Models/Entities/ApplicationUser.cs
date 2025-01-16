using Microsoft.AspNetCore.Identity;

namespace MediaJournal.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public ICollection<Media> MediaItems { get; set; } = new List<Media>();
}