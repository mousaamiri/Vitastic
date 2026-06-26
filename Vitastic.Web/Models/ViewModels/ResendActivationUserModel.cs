using System.ComponentModel.DataAnnotations;

namespace Vitastic.Web.Models.ViewModels
{
    public sealed record ResendActivationUserModel(
        [property: Required(ErrorMessage = "ایمیل الزامی است")]
        [property: EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است")]
        string Email
    );
}