namespace MyWebApi.DTOs.Jobs.Response;

public class JobResponseDto
{
    public int Id { get; set; }
    public string Tasks { get; set; }
    public string Description { get; set; }

    public int CreatedBy { get; set; }
}