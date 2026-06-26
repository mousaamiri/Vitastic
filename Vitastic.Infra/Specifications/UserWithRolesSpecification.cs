using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Infra.Specifications;

public class UserWithRolesSpecification:Specification<User,UserId>
{
    public UserWithRolesSpecification(UserId userId)
        : base(u => u.Id == userId)
    {
        AddInclude("UserRoles.Role");
    }

}public class UserByEmailSpecification : Specification<User, UserId>
{
    public UserByEmailSpecification(Email email)
        : base(u => u.Email == email)
    {
    }
}
public class UserByUsernameSpecification : Specification<User, UserId>
{
    public UserByUsernameSpecification(UserName userName)
        : base(u => u.UserName == userName)
    {
    }
}
public class UserByIdSpecification : Specification<User, UserId>
{
    public UserByIdSpecification(UserId userId)
        : base(u => u.Id == userId)
    {
    }
}
public class UserWithRoleSpecification : Specification<User, UserId>
{
    public UserWithRoleSpecification(UserId userId, RoleId roleId)
        : base(u => u.Id == userId)
    {
         AddInclude("UserRoles.Role");
    }
}
