using MyWebApi.DTOs.Jobs.Request;
using MyWebApi.DTOs.Jobs.Response;

namespace MyWebApi.Interfaces;

public interface IJobService
{
    Task<object> GetAllJobsAsync();
    Task<object?> GetJobByIdAsync(int id);
    Task<JobResponseDto> CreateJobAsync(JobRequestDto jobRequestDto, int userId);
    Task<JobResponseDto?> UpdateJobAsync(int id, JobRequestDto jobRequestDto);
    Task<bool> DeleteJobAsync(int id);
}
