namespace Vitastic.Web.Models.DTOs.Auth;

public sealed record ResendActivationDto(
    string Email,
    string CallbackUrl
);