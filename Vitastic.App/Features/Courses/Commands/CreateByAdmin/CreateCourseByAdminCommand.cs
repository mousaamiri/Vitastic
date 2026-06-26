using Vitastic.App.Common.Abstractions.Files;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Courses.Dtos;

namespace Vitastic.App.Features.Courses.Commands.CreateByAdmin;
public sealed record CreateCourseByAdminCommand(
    string Title,
    string Description,
    string ShortDescription,
    string Slug,
    CourseLevelDto Level,
    Guid InstructorId,
    IReadOnlyCollection<Guid>? TagIds,
    IReadOnlyCollection<Guid>? CategoryIds,
    bool HasCertificate = false,
    IFile? ImageFile = null,
    IFile? ThumbnailFile = null,
    IFile? DemoVideoFile = null
) : ICommand<Guid>;
