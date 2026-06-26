namespace Vitastic.Web.Models.ViewModels;

public record RegisterUserModel(
    string Email,
    string UserName,
    string Password,
    string ConfirmPassword,
    bool AcceptTerms);
