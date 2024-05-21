using System.ComponentModel.DataAnnotations;

namespace BubtCommunity.Api.Options;

public class ConnectionStringOptions
{
    public const string SectionName = "ConnectionStrings";
    [Required] public required string BubtCommunityDb { get; init; }
}