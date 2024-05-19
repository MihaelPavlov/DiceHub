using MediatR;

namespace DH.Domain.Cqrs;

/// <summary>
/// Represents a command in the CQRS (Command Query Responsibility Segregation) pattern that returns a response of the specified type.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the command.</typeparam>
public interface ICommand<TResponse> : IRequest<TResponse>
{
}
