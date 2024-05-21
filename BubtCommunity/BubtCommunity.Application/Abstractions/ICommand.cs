using BubtCommunity.Domain.Shared;
using MediatR;

namespace BubtCommunity.Application.Abstractions;

public interface ICommand : IRequest<Result>;
public interface ICommand<TResponse> : IRequest<Result<TResponse>>;