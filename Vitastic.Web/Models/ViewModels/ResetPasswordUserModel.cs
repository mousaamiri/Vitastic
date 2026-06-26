using Humanizer;

namespace Vitastic.Web.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

public class ResetPasswordUserModel
{
    public string Token { get;  set; }

    [Required(ErrorMessage = "رمز عبور جدید الزامی است")]
    [MinLength(6, ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر باشد")]
    public string NewPassword { get;  set; }

    [Required(ErrorMessage = "تکرار رمز عبور الزامی است")]
    [Compare(nameof(NewPassword), ErrorMessage = "رمز عبور و تکرار آن مطابقت ندارند")]
    public string ConfirmPassword { get;  set; }

    public ResetPasswordUserModel(string token, string newPassword, string confirmPassword)
    {
        Token = token;
        NewPassword = newPassword;
        ConfirmPassword = confirmPassword;
    }
    public ResetPasswordUserModel() { }

}
