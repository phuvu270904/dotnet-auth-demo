using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.DTOs.Jobs.Request;
using MyWebApi.DTOs.Jobs.Response;
using MyWebApi.Interfaces;
using MyWebApi.Models;

namespace MyWebApi.Services;

public class JobService : IJobService
{
    private readonly AppDbContext _db;

    public JobService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<object> GetAllJobsAsync()
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

        return jobs;
    }

    public async Task<object?> GetJobByIdAsync(int id)
    {
        var job = await _db.Jobs.FindAsync(id);
        return job;
    }

    public async Task<JobResponseDto> CreateJobAsync(JobRequestDto jobRequestDto, int userId)
    {
        var job = new Job
        {
            Tasks = jobRequestDto.Tasks!,
            Description = jobRequestDto.Description,
            CreatedBy = userId
        };

        await _db.Jobs.AddAsync(job);
        await _db.SaveChangesAsync();

        return new JobResponseDto
        {
            Id = job.Id,
            Tasks = job.Tasks,
            Description = job.Description!,
            CreatedBy = job.CreatedBy
        };
    }

    public async Task<JobResponseDto?> UpdateJobAsync(int id, JobRequestDto jobRequestDto)
    {
        var jobToUpdate = await _db.Jobs.FindAsync(id);
        if (jobToUpdate == null)
            return null;

        _db.Jobs.Update(jobToUpdate);

        if (jobRequestDto.Tasks != null)
            jobToUpdate.Tasks = jobRequestDto.Tasks;

        if (jobRequestDto.Description != null)
            jobToUpdate.Description = jobRequestDto.Description;

        await _db.SaveChangesAsync();

        return new JobResponseDto
        {
            Id = id,
            Tasks = jobToUpdate.Tasks,
            Description = jobToUpdate.Description!,
            CreatedBy = jobToUpdate.CreatedBy
        };
    }

    public async Task<bool> DeleteJobAsync(int id)
    {
        var jobToDelete = await _db.Jobs.FindAsync(id);
        if (jobToDelete == null)
            return false;

        _db.Jobs.Remove(jobToDelete);
        await _db.SaveChangesAsync();
        return true;
    }
}
