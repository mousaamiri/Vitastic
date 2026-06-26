using Vitastic.Web.Areas.AdminPanel.Controllers;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs.Auth;
using Vitastic.Web.Models.DTOs.Role;

namespace Vitastic.Web.Areas.AdminPanel.Models.Users;

public sealed class UserManagerViewModel()
{
    public UserDetailDto UserDetailDto { get; set; }
    public PaginatedData<RoleDto> Roles { get; set; }
    public UserManagerController.Mode Mode { get; set; }
    public string? NewPassword { get; set; }
}
