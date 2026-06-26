using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Instructors.Dtos;

namespace Vitastic.App.Features.Instructors.Queries.GetById;

public record GetInstructorByIdQuery(Guid InstructorId) : IQuery<InstructorDetailDto>;
