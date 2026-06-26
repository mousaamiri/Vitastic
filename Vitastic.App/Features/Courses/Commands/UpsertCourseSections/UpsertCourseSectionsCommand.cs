using System.Collections.ObjectModel;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Courses.Dtos;

namespace Vitastic.App.Features.Courses.Commands.UpsertCourseSections;

public record UpsertCourseSectionsCommand(Guid CourseId, List<SectionDto> Sections) : ICommand;
