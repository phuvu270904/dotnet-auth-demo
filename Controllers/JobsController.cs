using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.DTOs.Jobs.Request;
using MyWebApi.DTOs.Jobs.Response;
using MyWebApi.Models;

namespace MyWebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly AppDbContext _db;
    
    public JobsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet()]
    public async Task<IActionResult> Get()
    {
        var jobs = await _db.Jobs
            .Include(j => j.User)
            .Select(j => new {
                j.Id,
                j.Tasks,
                j.Description,
                CreatedBy = new {
                    j.User!.Id,
                    j.User.Username,
                }
            })
            .ToListAsync();

        return Ok(jobs);
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
    public async Task<IActionResult> Post([FromBody] JobRequestDto jobRequestDto)
    {
        var job = new Job
        {
            Tasks = jobRequestDto.Tasks,
            Description = jobRequestDto.Description,
            CreatedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
        };
        await _db.Jobs.AddAsync(job);
        await _db.SaveChangesAsync();
        var response = new JobResponseDto
        {
            Id = job.Id,
            Tasks = job.Tasks,
            Description = job.Description,
            CreatedBy = job.CreatedBy
        };
        return Ok(response);
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
            CreatedBy = taskToUpdate.CreatedBy
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