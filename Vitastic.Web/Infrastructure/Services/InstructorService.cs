using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs.Instructor;

namespace Vitastic.Web.Infrastructure.Services;

public interface IInstructorService
{
    Task<PaginatedApiResponse<InstructorDto>> GetTopInstructorsAsync(int count=10, CancellationToken token=default);
}

internal sealed class InstructorService (IApiClient apiClient): IInstructorService
{
    public async Task<PaginatedApiResponse<InstructorDto>> GetTopInstructorsAsync(int count=10, CancellationToken token=default)
    {
        PaginatedApiResponse<InstructorDto> instructors = await apiClient.GetPaginatedAsync<InstructorDto>(
            "Instructors",
            new { pageNumber = 1, pageSize = count, onlyParents = false }, token);
        return instructors;
    }


}
