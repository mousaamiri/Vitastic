using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs;
using Vitastic.Web.Models.DTOs.Course;
using Vitastic.Web.Models.ViewModels;
using SimpleCourseDto = Vitastic.Web.Models.DTOs.Course.SimpleCourseDto;
#pragma warning disable CA1305

namespace Vitastic.Web.Infrastructure.Services;

public interface ICourseService
{
    Task<PaginatedApiResponse<SimpleCourseDto>> GetLastestCoursesAsync(int count, CancellationToken token = default);
    Task<PaginatedApiResponse<SimpleCourseDto>> GetPopularCoursesAsync(int count, CancellationToken token = default);

    Task<PaginatedApiResponse<SimpleCourseDto>> SearchCoursesAsync(CourseFilterDto parameters,
        CancellationToken token = default);

    Task<PaginatedApiResponse<SimpleCourseDto>> GetMyCoursesAsync(CourseFilterDto parameters,
        CancellationToken token = default);

    Task<PaginatedApiResponse<CategoryViewModel>> GetCategoriesForFilterAsync(CancellationToken token = default);

    Task<ApiResponse> UpsertCourseByAdminAsync(UpsertCreateCourseRequest request, Guid? courseId = null,
        CancellationToken token = default);

    Task<ApiResponse<CourseDto>> GetCourseByIdAsync(Guid courseId, CancellationToken token);
    Task<ApiResponse<string>> GetCourseTitle(Guid courseId, CancellationToken token = default);
    Task<ApiResponse> UpsertCourseSections(Guid modelCourseId, IEnumerable<SectionDto> sections, CancellationToken token = default);
}

internal sealed class CourseService(IApiClient apiClient) : ICourseService
{
    public async Task<PaginatedApiResponse<SimpleCourseDto>> GetLastestCoursesAsync(int count,
        CancellationToken token = default)
        =>
            await apiClient.GetPaginatedAsync<SimpleCourseDto>(
                "Courses",
                new { pageNumber = 1, pageSize = count, onlyParents = false }, token);

    public async Task<PaginatedApiResponse<SimpleCourseDto>> GetPopularCoursesAsync(int count,
        CancellationToken token = default) =>
        await apiClient.GetPaginatedAsync<SimpleCourseDto>(
            "Courses",
            new { pageNumber = 1, pageSize = count, onlyParents = false }, token);

    public async Task<PaginatedApiResponse<SimpleCourseDto>> SearchCoursesAsync(CourseFilterDto parameters,
        CancellationToken token = default)
        => await apiClient.GetPaginatedAsync<SimpleCourseDto>(
            "Courses/Search", parameters, token);

    public async Task<PaginatedApiResponse<SimpleCourseDto>> GetMyCoursesAsync(CourseFilterDto parameters,
        CancellationToken token = default)
        => await apiClient.GetPaginatedAsync<SimpleCourseDto>(
            "Courses/my-courses", parameters, token);

    public async Task<PaginatedApiResponse<CategoryViewModel>> GetCategoriesForFilterAsync(
        CancellationToken token = default)
        => await apiClient.GetPaginatedAsync<CategoryViewModel>(
            "Categories",
            new { pageNumber = 1, pageSize = 50, onlyParents = false }, token);


    public async Task<ApiResponse> UpsertCourseByAdminAsync(
        UpsertCreateCourseRequest request,
        Guid? courseId = null,
        CancellationToken token = default)
    {
        var url = courseId is null ? "Courses/by-admin" : $"Courses/{courseId.Value}/by-admin";
        var content = new MultipartFormDataContent();
        //Add simple properties
        content.Add(new StringContent(request.Title, Encoding.UTF8), "Title");
        content.Add(new StringContent(request.Description, Encoding.UTF8), "Description");
        content.Add(new StringContent(request.ShortDescription, Encoding.UTF8), "ShortDescription");
        content.Add(new StringContent(request.Slug, Encoding.UTF8), "Slug");
        content.Add(new StringContent(((int)request.Level).ToString(), Encoding.UTF8), "Level");
        content.Add(new StringContent(request.InstructorId.ToString(), Encoding.UTF8), "InstructorId");
        content.Add(new StringContent(request.HasCertificate.ToString(), Encoding.UTF8), "HasCertificate");

        // Add TagIds (multiple values with same key)
        if (request.TagIds?.Any() == true)
        {
            foreach (Guid tagId in request.TagIds)
            {
                content.Add(new StringContent(tagId.ToString(), Encoding.UTF8), "TagIds");
            }
        }

        // Add CategoryIds (multiple values with same key)
        if (request.CategoryIds?.Any() == true)
        {
            foreach (Guid categoryId in request.CategoryIds)
            {
                content.Add(new StringContent(categoryId.ToString(), Encoding.UTF8), "CategoryIds");
            }
        }

        //Add fies
        if (request.ImageFile is not null)
        {
            var imageStream = new StreamContent(request.ImageFile.OpenReadStream());
            imageStream.Headers.ContentType = new MediaTypeHeaderValue(
                request.ImageFile.ContentType ?? "application/octet-stream");
            content.Add(imageStream, nameof(request.ImageFile), request.ImageFile.FileName);
        }

        if (request.DemoVideoFile is not null)
        {
            var imageStream = new StreamContent(request.DemoVideoFile.OpenReadStream());
            imageStream.Headers.ContentType = new MediaTypeHeaderValue(
                request.DemoVideoFile.ContentType ?? "application/octet-stream");
            content.Add(imageStream, nameof(request.DemoVideoFile), request.DemoVideoFile.FileName);
        }

        return courseId is null
            ? await apiClient.PostMultipartAsync(url, content, token)
            : await apiClient.PutMultipartAsync(url, content, token);
    }

    public async Task<ApiResponse<CourseDto>> GetCourseByIdAsync(Guid courseId, CancellationToken token)
        => await apiClient.GetAsync<CourseDto>(
            $"Courses/{courseId}", token);

    public async Task<ApiResponse<string>> GetCourseTitle(Guid courseId, CancellationToken token = default)
        => await apiClient.GetAsync<string>($"Courses/{courseId}/title", token);

    public async Task<ApiResponse> UpsertCourseSections(Guid courseId, IEnumerable<SectionDto> request, CancellationToken token = default)
    {
        var content = new MultipartFormDataContent();

        var sectionIndex = 0;
        foreach (SectionDto section in request)
        {
            content.Add(new StringContent(section.Title ?? string.Empty, Encoding.UTF8), $"Sections[{sectionIndex}].Title");
            content.Add(new StringContent(section.DisplayOrder.ToString(), Encoding.UTF8), $"Sections[{sectionIndex}].DisplayOrder");

            if (section.Id != Guid.Empty)
                content.Add(new StringContent(section.Id.ToString(), Encoding.UTF8), $"Sections[{sectionIndex}].Id");

            var episodeIndex = 0;
            foreach (EpisodeDto episode in section.Episodes)
            {
                content.Add(new StringContent(episode.Title ?? string.Empty, Encoding.UTF8), $"Sections[{sectionIndex}].Episodes[{episodeIndex}].Title");
                content.Add(new StringContent(episode.DisplayOrder.ToString(), Encoding.UTF8), $"Sections[{sectionIndex}].Episodes[{episodeIndex}].DisplayOrder");
                content.Add(new StringContent(episode.Duration.ToString(), Encoding.UTF8), $"Sections[{sectionIndex}].Episodes[{episodeIndex}].Duration");
                content.Add(new StringContent(episode.IsFree.ToString(), Encoding.UTF8), $"Sections[{sectionIndex}].Episodes[{episodeIndex}].IsFree");
                content.Add(new StringContent(episode.Price.ToString(CultureInfo.InvariantCulture), Encoding.UTF8), $"Sections[{sectionIndex}].Episodes[{episodeIndex}].Price");

                if (episode.Id != Guid.Empty)
                    content.Add(new StringContent(episode.Id.ToString(), Encoding.UTF8), $"Sections[{sectionIndex}].Episodes[{episodeIndex}].Id");

                if (episode.VideoFile != null)
                {
                    var videoStream = new StreamContent(episode.VideoFile.OpenReadStream());
                    videoStream.Headers.ContentType = new MediaTypeHeaderValue(episode.VideoFile.ContentType);
                    content.Add(videoStream, $"Sections[{sectionIndex}].Episodes[{episodeIndex}].VideoFile", episode.VideoFile.FileName);
                }
                episodeIndex++;
            }

            sectionIndex++;
        }


        return await apiClient.PostMultipartAsync($"Courses/{courseId}/sections/upsert", content, token);
    }
}
