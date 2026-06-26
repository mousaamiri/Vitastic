namespace Vitastic.Api.Features.Tags.Requests;

public sealed record CreateTagRequest(string Name);
public sealed record UpdateTagNameRequest(string NewName);
