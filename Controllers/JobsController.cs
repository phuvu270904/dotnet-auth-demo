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

    [HttpGet()]
    public async Task<IActionResult> Get()
    {
        var jobs = await _jobService.GetAllJobsAsync();
        return Ok(jobs);
    }

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

    [HttpPost()]
    public async Task<IActionResult> Post([FromBody] JobRequestDto jobRequestDto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _jobService.CreateJobAsync(jobRequestDto, userId);
        return Ok(response);
    }

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