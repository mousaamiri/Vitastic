using Vitastic.App.Common.Abstractions.Files;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Shared.Repositories;

namespace Vitastic.App.Features.Courses.Commands.UpdateByAdmin;
public sealed record UpdateCourseByAdminCommand(
    Guid CourseId,
    string Title,
    string Description,
    string ShortDescription,
    string Slug,
    CourseLevelDto Level,
    Guid InstructorId,
    IReadOnlyCollection<Guid>? TagIds,
    IReadOnlyCollection<Guid>? CategoryIds,
    bool HasCertificate,
    IFile? ImageFile = null,
    IFile? ThumbnailFile = null,
    IFile? DemoVideoFile = null
) : ICommand;
