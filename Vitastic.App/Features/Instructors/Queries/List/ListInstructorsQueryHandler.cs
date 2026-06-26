using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Instructors.Dtos;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Instructors.Queries.List;

public sealed class ListInstructorsQueryHandler(IInstructorQueryService instructorQueryService)
    : IQueryHandler<ListInstructorsQuery, PaginatedResult<InstructorDto>>
{
    public async Task<Result<PaginatedResult<InstructorDto>>> Handle(ListInstructorsQuery query,
        CancellationToken cancellationToken)
    {
        (IReadOnlyList<InstructorDto> items, var total)  = await instructorQueryService
            .GetPagedAsync(query.PageNumber, query.PageSize, query.Status,cancellationToken);
        return new PaginatedResult<InstructorDto>(items, total,query.PageNumber, query.PageSize);
    }
}
