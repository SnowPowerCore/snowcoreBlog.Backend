namespace snowcoreBlog.Backend.Infrastructure.Models;

public class AltchaStoredChallenge
{
    public required string Challenge { get; set; }
    
    public required DateTimeOffset ExpiryUtc { get; set; }
}