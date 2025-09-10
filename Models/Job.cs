using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Models;

public class Job
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Tasks { get; set; } = string.Empty;

    public string? Description { get; set; }
    
    // Foreign Key
    [Required]
    public int CreatedBy { get; set; }

    // Navigation property
    [ForeignKey("CreatedBy")]
    public User? User { get; set; }
}