namespace Vitastic.App.Features.Roles.Dtos
{
    public sealed class RoleDto
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public int PermissionCount { get; private set; }

        public RoleDto(Guid id, string name, int permissionCount)
        {
            Id = id;
            Name = name;
            PermissionCount = permissionCount;
        }

        public RoleDto() { }
    }
}
