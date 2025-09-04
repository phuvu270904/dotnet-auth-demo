using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Models;

public class Job
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Tasks { get; set; } = string.Empty;

    public string? Description { get; set; }
}