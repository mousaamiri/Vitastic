namespace Vitastic.Web.Models.DTOs.Auth
{
    public sealed record ActivateUserDto(
        string Email,
        string Token
    );
}
