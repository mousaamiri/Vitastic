using Vitastic.Domain.Shared.ValueObjects.Base;

namespace Vitastic.Domain.Entities.Orders.ValueObjects;

public sealed class OrderNumber:ValueObject<string>
{
    private OrderNumber(string value) : base(value) { }
    public static OrderNumber Generate()
    {
        var guidPart = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        var timestampPart = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return new OrderNumber($"ORD-{timestampPart}-{guidPart}");
    }

    public static OrderNumber FromExisting(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Order number cannot be null or empty.", nameof(value));

        return new OrderNumber(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
