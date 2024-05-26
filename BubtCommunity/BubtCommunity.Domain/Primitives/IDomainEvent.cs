using MediatR;

namespace BubtCommunity.Domain.Primitives;

public interface IDomainEvent : INotification
{
    public Guid Id { get; init; }
}
