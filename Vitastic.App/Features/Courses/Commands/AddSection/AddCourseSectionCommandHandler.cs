using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.AddSection
{
    public sealed class AddCourseSectionCommandHandler(ICourseRepository courseRepository
        , IMapper mapper)
        : ICommandHandler<AddCourseSectionCommand, SectionDto>
    {
        public async Task<Result<SectionDto>> Handle(AddCourseSectionCommand request, CancellationToken cancellationToken)
        {
            Result<CourseId> courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            Result<SectionTitle> sectionTitle =SectionTitle.Create(request.Title);
            if (sectionTitle.IsFailure)
                return sectionTitle.Error;
            var sectionTitleIsExist =  await courseRepository.SectionTitleIsExistAsync(courseIdResult.Value,sectionTitle.Value,cancellationToken);
            if(sectionTitleIsExist)
                return Error.Conflict("Course.SectionTitleAlreadyExist", "این عنوان قبلا انتخاب شده، لطفا عنوان دیگری انتخاب کنید.");

            Course? course = await courseRepository.FindAsync(courseIdResult.Value, cancellationToken);
            if (course is null)
                return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            Result<Section> sectionResult = course.AddSection(request.Title, request.DisplayOrder);
            if (sectionResult.IsFailure)
                return sectionResult.Error;

            return mapper.Map<SectionDto>(sectionResult.Value);
        }
    }
}
