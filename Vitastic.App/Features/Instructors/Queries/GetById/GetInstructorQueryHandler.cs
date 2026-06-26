using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Instructors.Dtos;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Instructors.Queries.GetById;

public sealed class GetInstructorQueryHandler(IInstructorQueryService instructorQuery,IMapper mapper)
    : IQueryHandler<GetInstructorByIdQuery, InstructorDetailDto>
{
    public async Task<Result<InstructorDetailDto>> Handle(GetInstructorByIdQuery query, CancellationToken cancellationToken)
    {
        Result<InstructorId> instructorId = InstructorId.CreateFrom(query.InstructorId);
        if (instructorId.IsFailure)
            return instructorId.Error;
        InstructorDetailDto? instructorDto =await instructorQuery
            .GetByIdAsync(instructorId.Value, cancellationToken);
        if(instructorDto is null)
            return Error.NotFound("GetInstructorQuery.InstructorNotFound","هیچ استادی با این شناسه یافت نشد.");
        return instructorDto;
    }
}
