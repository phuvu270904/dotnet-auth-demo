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
    public IActionResult Get(int id)
    {
        var task = _db.Jobs.Find(id);
        if (task == null)
        {
            return NotFound();
        }
        return Ok(task);
    }

    [HttpPost()]
    public IActionResult Post([FromBody] Job job)
    {
        _db.Jobs.Add(job);
        _db.SaveChanges();
        return Ok(job);
    }

    [HttpPatch("{id}")]
    public IActionResult Patch(int id, [FromBody] JobRequestDto jobRequestDto)
    {
        var taskToUpdate = _db.Jobs.Find(id);
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
        
        _db.SaveChanges();
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var taskToDelete = _db.Jobs.Find(id);
        _db.Jobs.Remove(taskToDelete);
        _db.SaveChanges();
        return Ok();
    }
}