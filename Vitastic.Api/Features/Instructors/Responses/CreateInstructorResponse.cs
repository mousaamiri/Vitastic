namespace Vitastic.Api.Features.Instructors.Responses;
public sealed record InstructorDetailResponse(
    Guid Id ,
    string FullName ,
    string Bio ,
    string Avatar ,
    string Status ,
    string Email ,
    List<string> Skills
    );

public sealed record InstructorResponse
{
    public Guid Id{get; private set;}
    public string FullName{get; private set;}=null!;
    public string Bio {get; private set;}=null!;
    public string Avatar {get; private set;}=null!;
    public string Email {get; private set;}=null!;

    public string Expertise { get; set; }=string.Empty;
    public int TotalRatings { get; set; }
    public decimal AverageRating { get;  set; }
    public InstructorStatusResponse Status{get; init;}

    public InstructorResponse() { }
}
public sealed record InstructorStatisticsResponse(
    int TotalInstructors,
    int ActiveInstructors,
    int InactiveInstructors,
    int PendingInstructors);
public enum InstructorStatusResponse
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    PendingApproval = 4,
    Rejected = 5
}
