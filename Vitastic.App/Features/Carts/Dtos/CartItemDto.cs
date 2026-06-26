// Application/UseCases/Carts/DTOs/CartItemDto.cs
namespace Vitastic.App.Features.Carts.Dtos;

public sealed record CartItemDto
{

    public Guid Id { get; init; }
    public Guid CourseId { get; init; }
    public string CourseTitle { get; init; }=string.Empty;
    public string? CourseImageName { get; init; }
    public decimal UnitPrice { get; init; }
    public string Currency { get; init; }="IRT";
    public string CourseInstructorName { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public CartItemDto() { }
}
public sealed record CartDto
{

    public Guid Id { get; init; }
    public Guid? UserId { get; init; }
    public string? SessionId { get; init; }
    public string UserFullName { get; init; }
    public List<CartItemDto> Items { get; init; }
    public decimal ItemsTotal { get; init; }
    public int ItemsCount { get; init; }
    public string Currency { get; init; }
    public DateTimeOffset LastModifiedAt { get; init; }

    public CartDto() { }
    public CartDto(Guid id, Guid userId, string userFullName,
        List<CartItemDto> items,decimal itemsTotal,int itemsCount,
        string currency,DateTimeOffset lastModifiedAt)
    {
        Id = id;
        UserId = userId;
        UserFullName = userFullName;
        Items = items;
        ItemsTotal = itemsTotal;
        ItemsCount = itemsCount;
        Currency = currency;
        LastModifiedAt = lastModifiedAt;
    }

}

