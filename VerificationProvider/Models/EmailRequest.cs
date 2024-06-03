namespace VerificationProvider.Models;

public class EmailRequest
{
    public string RecipientAddress { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string HtmlContent { get; set; } = null!;
    public string PlainTextContent { get; set; } = null!;

}
