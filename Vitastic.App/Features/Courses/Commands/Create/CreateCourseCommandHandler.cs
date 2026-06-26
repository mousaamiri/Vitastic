using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.Enums;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.Create
{
    public sealed class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand, Guid>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public CreateCourseCommandHandler(ICourseRepository courseRepository, IMapper mapper)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var instructorIdResult = InstructorId.CreateFrom(request.InstructorId);
            if(instructorIdResult.IsFailure)
                return instructorIdResult.Error;

            var courseResult = Course.Create(
                request.Title,
                request.Description,
                request.ShortDescription,
                request.Slug,
                (CourseLevel)request.Level,
                instructorIdResult.Value,
                request.Currency
            );

            if (courseResult.IsFailure)
                return courseResult.Error;

            await _courseRepository.AddAsync(courseResult.Value,cancellationToken);

            return courseResult.Value.Id.Value;
        }
    }
}
