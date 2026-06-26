using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Wallets.ValueObjects;

public sealed class WalletId:GuidBasedId<WalletId>
{
    //Constructor
    private WalletId(Guid value) : base(value) { }
    //Override
    protected override WalletId Create(Guid value) => new(value);
    public static WalletId New() => new(Guid.NewGuid());
    public static Result<WalletId> CreateFrom(Guid value) =>
        CreateFrom(value, guid => new WalletId(guid), BaseIdErrors.Empty);
    public static Result<WalletId> CreateFrom(string value) =>
        CreateFrom(
            value,
            guid => new WalletId(guid),
            BaseIdErrors.Empty,
            BaseIdErrors.InvalidFormat(value));
}
