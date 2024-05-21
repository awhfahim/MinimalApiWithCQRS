using BubtCommunity.Application.Abstractions;

namespace BubtCommunity.Application.Posts.CreatePost;

public record CreatePostCommand(string Title ,string Content, string Tags) : ICommand<Guid>;