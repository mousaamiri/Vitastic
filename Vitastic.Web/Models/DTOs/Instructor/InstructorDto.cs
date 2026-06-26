namespace Vitastic.Web.Models.DTOs.Instructor;
public enum InstructorStatusDto
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    PendingApproval = 4,
    Rejected = 5
}
public sealed record InstructorDto
{
    public Guid Id{get;  set;}
    public string FullName{get;  set;}=null!;
    public string Bio {get;  set;}=null!;
    public string Avatar {get;  set;}=null!;
    public string Email {get;  set;}=null!;
    public string Expertise { get; set; }=string.Empty;
    public int TotalRatings { get; set; }
    public decimal AverageRating { get;  set; }
    public InstructorStatusDto Status{get; init;}

    public InstructorDto() { }
}
