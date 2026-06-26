using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Instructors.Dtos;

namespace Vitastic.App.Features.Instructors.Queries.List;

public record ListInstructorsQuery(
    int PageNumber,
    int PageSize,
    InstructorStatusDto? Status = null)
    : IQuery<PaginatedResult<InstructorDto>>;
