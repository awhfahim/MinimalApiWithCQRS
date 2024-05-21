using BubtCommunity.Domain.Shared;
using MediatR;

namespace BubtCommunity.Application.Abstractions;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
                    where TCommand : ICommand;
                    
public interface ICommandHandler<in TCommand, TResponse>
    : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>;