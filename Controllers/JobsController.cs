using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWebApi.DTOs.Jobs.Request;
using MyWebApi.DTOs.Jobs.Response;
using MyWebApi.Interfaces;

namespace MyWebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    
    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    /// <summary>
    /// Get all jobs.
    /// </summary>
    [HttpGet()]
    public async Task<IActionResult> Get()
    {
        var jobs = await _jobService.GetAllJobsAsync();
        return Ok(jobs);
    }

    /// <summary>
    /// Get a job by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var job = await _jobService.GetJobByIdAsync(id);
        if (job == null)
        {
            return NotFound();
        }
        return Ok(job);
    }

    /// <summary>
    /// Create a new job.
    /// </summary>
    [HttpPost()]
    public async Task<IActionResult> Post([FromBody] JobRequestDto jobRequestDto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _jobService.CreateJobAsync(jobRequestDto, userId);
        return Ok(response);
    }

    /// <summary>
    /// Update an existing job.
    /// </summary>
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] JobRequestDto jobRequestDto)
    {
        var response = await _jobService.UpdateJobAsync(id, jobRequestDto);
        
        if (response == null)
        {
            return NotFound(new { Message = "Job not found" });
        }

        return Ok(response);
    }

    /// <summary>
    /// Delete a job by ID.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _jobService.DeleteJobAsync(id);
        
        if (!success)
        {
            return NotFound(new { Message = "Job not found" });
        }
        
        return Ok(new { message = "Job deleted successfully" });
    }
}