using BubtCommunity.Application.Abstractions;
using BubtCommunity.Domain.Shared;

namespace BubtCommunity.Application.Posts.CreatePost;

public class CreatePostCommandHandler : ICommandHandler<CreatePostCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var postId = Guid.NewGuid();
        //var response = Result.Create();
        return Result.Failure<Guid>(new Error("401", "Unauthorized Access"));
    }
}