using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace FBZSystemMvc.Services.ExternalApis;

public class EmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    private const string SendGridEndpoint = "https://api.sendgrid.com/v3/mail/send";
    private const string FromEmail = "noreply@fbzsystem.app";
    private const string FromName = "FBZ Comic System";

    public EmailService(HttpClient httpClient, IConfiguration configuration, ILogger<EmailService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _httpClient.Timeout = TimeSpan.FromSeconds(15);
    }

    public async Task<bool> SendComicEmailAsync(
        string toEmail,
        string comicTitle,
        string authorName,
        string description,
        string coverImageUrl,
        string googleBooksUrl,
        CancellationToken cancellationToken = default)
    {
        if (!IsValidEmail(toEmail))
            return false;

        try
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogWarning("SendGrid:ApiKey is not configured — email not sent");
                return false;
            }

            var msg = BuildMessage(toEmail, comicTitle, authorName, description, coverImageUrl, googleBooksUrl);
            var json = msg.Serialize();

            using var request = new HttpRequestMessage(HttpMethod.Post, SendGridEndpoint);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Accepted)
                return true;

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("SendGrid returned {Status} for '{Title}' to '{Email}': {Body}",
                (int)response.StatusCode, comicTitle, toEmail, body);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SendGrid call failed while sending comic details for '{Title}' to '{Email}'",
                comicTitle, toEmail);
            return false;
        }
    }

    private static SendGridMessage BuildMessage(
        string toEmail,
        string comicTitle,
        string authorName,
        string description,
        string coverImageUrl,
        string googleBooksUrl)
    {
        var from = new EmailAddress(FromEmail, FromName);
        var to = new EmailAddress(toEmail);
        var subject = $"Comic Details: {comicTitle}";
        var htmlBody = BuildHtmlBody(comicTitle, authorName, description, coverImageUrl, googleBooksUrl);
        var plainBody = BuildPlainBody(comicTitle, authorName, description, googleBooksUrl);
        return MailHelper.CreateSingleEmail(from, to, subject, plainBody, htmlBody);
    }

    private static string BuildHtmlBody(
        string comicTitle,
        string authorName,
        string description,
        string coverImageUrl,
        string googleBooksUrl)
    {
        var encodedTitle = System.Net.WebUtility.HtmlEncode(comicTitle);
        var encodedAuthor = System.Net.WebUtility.HtmlEncode(authorName);
        var encodedDesc = System.Net.WebUtility.HtmlEncode(description);

        var authorLine = string.IsNullOrWhiteSpace(authorName)
            ? string.Empty
            : $"""<p style="color:#666; font-size:15px; margin:4px 0 16px;">by {encodedAuthor}</p>""";

        var coverImg = string.IsNullOrWhiteSpace(coverImageUrl)
            ? string.Empty
            : $"""<img src="{System.Net.WebUtility.HtmlEncode(coverImageUrl)}" alt="Cover" style="max-width:180px; border-radius:6px; display:block; margin-bottom:16px;" />""";

        var descSection = string.IsNullOrWhiteSpace(description)
            ? string.Empty
            : $"""<p style="color:#444; line-height:1.6; margin:12px 0;">{encodedDesc}</p>""";

        var booksButton = string.IsNullOrWhiteSpace(googleBooksUrl)
            ? string.Empty
            : $"""
              <div style="margin-top:20px;">
                <a href="{System.Net.WebUtility.HtmlEncode(googleBooksUrl)}"
                   style="display:inline-block; padding:10px 20px; background:#4285f4; color:#fff; text-decoration:none; border-radius:4px; font-weight:bold; font-size:14px;">
                  View on Google Books
                </a>
              </div>
              """;

        return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head><meta charset="UTF-8" /><meta name="viewport" content="width=device-width, initial-scale=1" /></head>
            <body style="font-family:Arial,sans-serif; background:#f4f4f4; margin:0; padding:24px;">
              <div style="max-width:600px; margin:0 auto; background:#fff; border-radius:8px; padding:32px; box-shadow:0 2px 8px rgba(0,0,0,0.08);">
                <h1 style="font-size:22px; color:#1a1a1a; margin:0 0 4px;">{encodedTitle}</h1>
                {authorLine}
                {coverImg}
                {descSection}
                {booksButton}
                <hr style="border:none; border-top:1px solid #eee; margin:28px 0 16px;" />
                <p style="font-size:12px; color:#aaa; margin:0;">Sent from the FBZ Comic System</p>
              </div>
            </body>
            </html>
            """;
    }

    private static string BuildPlainBody(
        string comicTitle,
        string authorName,
        string description,
        string googleBooksUrl)
    {
        var sb = new StringBuilder();
        sb.AppendLine(comicTitle);
        if (!string.IsNullOrWhiteSpace(authorName))
            sb.AppendLine($"by {authorName}");
        if (!string.IsNullOrWhiteSpace(description))
        {
            sb.AppendLine();
            sb.AppendLine(description);
        }
        if (!string.IsNullOrWhiteSpace(googleBooksUrl))
        {
            sb.AppendLine();
            sb.AppendLine($"View on Google Books: {googleBooksUrl}");
        }
        sb.AppendLine();
        sb.AppendLine("Sent from the FBZ Comic System");
        return sb.ToString();
    }

    internal static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return string.Equals(addr.Address, email.Trim(), StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}
