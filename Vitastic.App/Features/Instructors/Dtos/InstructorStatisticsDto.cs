namespace Vitastic.App.Features.Instructors.Dtos;

public sealed record InstructorStatisticsDto(
    int TotalInstructors,
    int ActiveInstructors,
    int InactiveInstructors,
    int PendingInstructors);
