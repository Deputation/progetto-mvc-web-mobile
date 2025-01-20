using System.ComponentModel.DataAnnotations;

namespace MediaJournal.Models.Entities;

public class Media
{
    public int ID { get; set; }
        
    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;
        
    [Required]
    public MediaType Type { get; set; }
        
    [Required]
    [DataType(DataType.Date)]
    public DateTime CompletedDate { get; set; }
        
    [Required]
    [Range(1, 10)]
    public int Rating { get; set; }
        
    [StringLength(5000)]
    public string? Review { get; set; }
        
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    
    [Required]
    public bool IsPublic { get; set; }
}