using System.ComponentModel.DataAnnotations;

namespace Vitastic.Web.Models.ViewModels;

public sealed record ForgetPasswordUserModel(
    [Required(ErrorMessage = "ایمیل الزامی است")]
    [EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است")]
    string Email
);
