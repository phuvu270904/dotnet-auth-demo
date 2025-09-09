using Microsoft.AspNetCore.Mvc;
using MyWebApi.Data;
using MyWebApi.DTOs.Jobs.Request;
using MyWebApi.DTOs.Jobs.Response;
using MyWebApi.Models;

namespace MyWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly AppDbContext _db;
    
    public JobsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet()]
    public IActionResult Get()
    {
        return Ok(_db.Jobs.ToList());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var task = await _db.Jobs.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        return Ok(task);
    }

    [HttpPost()]
    public async Task<IActionResult> Post([FromBody] Job job)
    {
        await _db.Jobs.AddAsync(job);
        await _db.SaveChangesAsync();
        return Ok(job);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] JobRequestDto jobRequestDto)
    {
        var taskToUpdate = await _db.Jobs.FindAsync(id);
        _db.Jobs.Update(taskToUpdate);
        
        if (jobRequestDto.Tasks != null)
            taskToUpdate.Tasks = jobRequestDto.Tasks;

        if (jobRequestDto.Description != null)
            taskToUpdate.Description = jobRequestDto.Description;

        var response = new JobResponseDto
        {
            Id = id,
            Tasks = taskToUpdate.Tasks,
            Description = taskToUpdate.Description,
        };

        await _db.SaveChangesAsync();
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var taskToDelete = await _db.Jobs.FindAsync(id);
        if (taskToDelete == null)
        {
            return NotFound(new { Message = "Job not found" });
        }
        _db.Jobs.Remove(taskToDelete);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Job deleted successfully" });
    }
}