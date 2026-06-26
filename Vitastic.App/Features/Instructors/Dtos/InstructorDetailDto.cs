namespace Vitastic.App.Features.Instructors.Dtos;

public class InstructorDetailDto
{
    public Guid Id { get; set; }
    public string InstructorFullName { get; set; }
    public string Bio { get; set; }
    public string Avatar { get; set; }
    public string Status { get; set; }
    public string Email { get; set; }
    public List<string> Skills { get; set; }

    public InstructorDetailDto() { } // Zero-argument constructor
}
