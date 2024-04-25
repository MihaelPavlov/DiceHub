using MediatR;

namespace DH.Domain.Cqrs;

public interface ICommand<TResponse> : IRequest<TResponse>
{
}
