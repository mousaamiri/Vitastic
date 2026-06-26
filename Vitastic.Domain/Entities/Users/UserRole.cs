using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.Domain.Entities.Users;
public class UserRole:BaseEntity<UserRoleId>
{
    public RoleId RoleId { get; private set; }
    public UserId UserId { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }=DateTimeOffset.UtcNow;

    public UserRole() { }//For ef
    private UserRole(UserRoleId id, RoleId roleId, UserId userId)
        : base(id)
    {
        RoleId = roleId;
        UserId = userId;
    }
    public static UserRole Create(RoleId roleId, UserId userId)
    {
        return new UserRole(UserRoleId.New(), roleId, userId);
    }
}
