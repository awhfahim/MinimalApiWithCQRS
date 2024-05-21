using BubtCommunity.Api.RequestHandlers;
using BubtCommunity.Application.Posts.CreatePost;
using MediatR;

namespace BubtCommunity.Api.EndPoints.PostEndPoints;

public class CreatePost : IEndPoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/posts", async (CreatePostRequest request, ISender sender, CancellationToken token) =>
            {
                var command = new CreatePostCommand(request.Title, request.Content, request.Tags!);
                var post = await sender.Send(command, token);
                return post.IsFailure ? Results.BadRequest(post.Error) : Results.Ok(post);
            })
            .Produces<Guid>(201)
            .Accepts<CreatePostRequest>("application/json")
            .WithTags("Posts")
            .WithDescription("Create a new post")
            .HasApiVersion(1);

    }
}