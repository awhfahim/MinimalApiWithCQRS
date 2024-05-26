using BubtCommunity.Domain.Primitives;
using StackOverflow.Domain.Repositories;

namespace BubtCommunity.Domain.Members;

public sealed class Member : AggregateRoot, IAuditableEntity
{
    private Member(Guid id, Email email, FirstName firstName, LastName lastName)
        : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    private Member()
    {
    }

    public string Email { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime? ModifiedOnUtc { get; set; }

    public static Member Create(
        Guid id,
        Email email,
        FirstName firstName,
        LastName lastName)
    {
        var member = new Member(
            id,
            email,
            firstName,
            lastName);

        member.RaiseDomainEvent(new MemberRegisteredDomainEvent(
            Guid.NewGuid(),
            member.Id));

        return member;
    }

    public void ChangeName(FirstName firstName, LastName lastName)
    {
        if (!FirstName.Equals(firstName) || !LastName.Equals(lastName))
        {
            RaiseDomainEvent(new MemberNameChangedDomainEvent(
                Guid.NewGuid(), Id));
        }

        FirstName = firstName;
        LastName = lastName;
    }
}