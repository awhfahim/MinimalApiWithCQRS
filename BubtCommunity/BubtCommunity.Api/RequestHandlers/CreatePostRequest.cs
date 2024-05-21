namespace BubtCommunity.Api.RequestHandlers;

public class CreatePostRequest
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string? Tags { get; set; }
    
}