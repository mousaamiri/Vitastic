namespace Vitastic.App.Common.Settings;

#region ==================== Client Settings ====================

/// <summary>
/// Stores allowed client base URLs for link generation
/// Used to validate that callback URLs come from trusted clients
/// </summary>
public sealed class ClientSettings
{
    public const string SectionName = "AllowedClients";

    /// <summary>
    /// List of trusted client base URLs (e.g., web frontend, mobile deep link)
    /// </summary>
    public List<AllowedClient> Clients { get; set; } = [];

    /// <summary>
    /// Validates if a callback URL is from a trusted client
    /// </summary>
    public bool IsAllowedUrl(string callbackUrl)
    {
        if (string.IsNullOrWhiteSpace(callbackUrl))
            return false;

        return Clients.Any(c =>
            callbackUrl.StartsWith(c.BaseUrl, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets a client by name (e.g., "web", "mobile")
    /// </summary>
    public AllowedClient? GetClientByName(string name)
    {
        return Clients.FirstOrDefault(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}

public sealed class AllowedClient
{
    /// <summary>
    /// Client identifier (e.g., "web", "mobile", "desktop")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Base URL of the client (e.g., "https://mysite.com")
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;
}

#endregion
