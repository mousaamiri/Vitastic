namespace Vitastic.Api.Features.Instructors.Requests;

public record CreateInstructorRequest(Guid UserId, string Bio,string Expert);
public record UpdateInstructorRequest(string NewBio);
