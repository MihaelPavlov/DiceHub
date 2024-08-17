using MediatR;

namespace DH.Domain.Cqrs;

/// <summary>
/// Defines a handler for executing commands that return a result of the specified type.
/// </summary>
/// <typeparam name="TCommand">The type of the command to handle.</typeparam>
/// <typeparam name="TResult">The type of the result returned by the command.</typeparam>
public interface IDefaultCommandHandler<TCommand, TResult> : IRequestHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Handles the execution of the specified command.
    /// </summary>
    /// <param name="request">The command to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the command execution.</returns>
    Task<TResult> Handle(TCommand request, CancellationToken cancellationToken);
}
