namespace StackOverflow.Domain.Repositories;

public interface IEntity<out T>
{
    T Id { get; }
}