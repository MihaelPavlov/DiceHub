using MediatR;

namespace DH.Domain.Cqrs;

public interface IDefaultCommandHandler<TCommand, TResult> : IRequestHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> Handle(TCommand request, CancellationToken cancellationToken);
}
