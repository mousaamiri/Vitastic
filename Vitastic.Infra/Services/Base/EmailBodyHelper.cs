namespace Vitastic.Infra.Services.Base
{
    public static class EmailBodyHelper
    {
        public static string ActivationMessageBody(string email, string activationLink)
        {
            var htmlBody = $@"
<!DOCTYPE html>
<html lang=""fa"">
<head>
    <meta charset=""UTF-8"">
    <title>فعالسازی حساب کاربری</title>
</head>
<body style=""font-family: Tahoma, sans-serif; line-height: 1.6;"">
    <p>سلام <strong>{email}</strong> 🌱</p>
    
    <p>برای فعال‌سازی حساب کاربری‌تان لطفاً روی دکمه یا لینک زیر کلیک کنید:</p>
    
    <p style=""text-align: center;"">
        <a href=""{activationLink}"" style=""display: inline-block; padding: 10px 20px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px;"">
            فعال‌سازی حساب
        </a>
    </p>

    <p>یا اگر نمی‌خواهید دکمه را استفاده کنید، می‌توانید لینک زیر را در مرورگر خود کپی کنید:</p>
    <p><a href=""{activationLink}"">{activationLink}</a></p>

    <hr>
    <p style=""color: gray; font-size: 0.9em;"">
        اگر شما این درخواست را نداده‌اید، لطفاً این ایمیل را نادیده بگیرید.
    </p>
</body>
</html>
";
            return htmlBody;
        }

        public static string ResetPasswordEmailBody(string email, string resetLink)
        {
            var htmlBody = $@"
<!DOCTYPE html>
<html lang=""fa"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>بازیابی رمز عبور</title>
</head>
<body style=""font-family: Tahoma, sans-serif; background-color: #f9f9f9; color: #333; margin: 0; padding: 0;"">
    <div style=""max-width: 600px; margin: 40px auto; background: #fff; border-radius: 8px;
                box-shadow: 0 2px 10px rgba(0,0,0,0.05); padding: 30px;"">

        <h2 style=""color: #1976D2; text-align: center;"">بازیابی رمز عبور</h2>

        <p>سلام <strong>{email}</strong> 👋</p>

        <p>
            شما (یا شخص دیگری) درخواست بازیابی رمز عبور برای حساب‌تان داده‌اید.
            اگر این درخواست از طرف شما بوده است، لطفاً روی دکمه زیر کلیک کنید تا رمز جدید تنظیم کنید:
        </p>

        <p style=""text-align: center; margin: 30px 0;"">
            <a href=""{resetLink}""
               style=""display: inline-block; background-color: #1976D2; color: #fff;
                      padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: bold;"">
                بازیابی رمز عبور
            </a>
        </p>

        <p>در صورت عدم کارکرد دکمه بالا، لینک زیر را در مرورگر خود باز کنید:</p>
        <p style=""direction: ltr; word-break: break-all; background: #f1f1f1; padding: 10px; border-radius: 5px;"">
            <a href=""{resetLink}"">{resetLink}</a>
        </p>

        <hr style=""border: none; border-top: 1px solid #ddd; margin: 30px 0;"">

        <p style=""color: gray; font-size: 0.9em;"">
            اگر شما این درخواست را نداده‌اید، می‌توانید این ایمیل را نادیده بگیرید.
        </p>

        <p style=""color: #aaa; font-size: 0.8em; text-align: center; margin-top: 40px;"">
            © {DateTime.UtcNow.Year} Vitastic. All rights reserved.
        </p>
    </div>
</body>
</html>";

            return htmlBody;
        }
    }
}
