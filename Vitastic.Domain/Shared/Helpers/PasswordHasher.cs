namespace Vitastic.Domain.Shared.Helpers;

public static class PasswordHasher
{
    public static string Hash(string password)
    =>BCrypt.Net.BCrypt.HashPassword(password);

    public static bool Verify(string password, string plainText)
    =>BCrypt.Net.BCrypt.Verify(password, plainText);
}
