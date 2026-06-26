namespace Vitastic.Api.Features.Carts.Responses;

public sealed record CartItemResponse(
    Guid Id,
    Guid CourseId,
    string CourseTitle,
    string? CourseImageName,
    decimal UnitPrice,
    string Currency,
    string CourseInstructorName,
    DateTimeOffset CreatedAt
);
public sealed record CartResponse
{
    public Guid Id { get;init; }
    public Guid? UserId { get; init; }
    public string? SessionId { get; init; } = null;
    public string UserFullName { get;init; }
    public List<CartItemResponse> Items { get;init; }
    public decimal ItemsTotal { get;init; }
    public string Currency { get;init; }
    public int ItemsCount { get;init; }
    public DateTimeOffset LastModifiedAt { get;init; }

    public CartResponse()
    {

    }
}
