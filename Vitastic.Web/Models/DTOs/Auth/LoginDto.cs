namespace Vitastic.Web.Models.DTOs.Auth;

public sealed record LoginDto
{
    public string Identifier {get; set;}   // ایمیل یا نام کاربری
    public  string Password {get;set;}

    public LoginDto()
    {

    }
}
