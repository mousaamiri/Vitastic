namespace Vitastic.App.Features.Instructors.Dtos;

public sealed class InstructorDto
{
    public Guid Id{get; private set;}
    public string FullName{get; private set;}=null!;
    public string Bio {get; private set;}=null!;
    public string Avatar {get; private set;}=null!;
    public string Status {get; private set;}=null!;

    public string Expertise { get; set; }=string.Empty;
    public int TotalRatings { get; set; }
    public decimal AverageRating { get;  set; }
}

