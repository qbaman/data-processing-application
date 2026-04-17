namespace FBZSystemMvc.Services.ExternalApis;

public interface IEmailService
{
    Task<bool> SendComicEmailAsync(
        string toEmail,
        string comicTitle,
        string authorName,
        string description,
        string coverImageUrl,
        string googleBooksUrl,
        CancellationToken cancellationToken = default);
}
