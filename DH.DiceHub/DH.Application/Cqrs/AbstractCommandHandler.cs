﻿using DH.Domain.Cqrs;

namespace DH.Application.Cqrs;

public abstract class AbstractCommandHandler<TCommand, TResult> : IDefaultCommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
    public async Task<TResult> Handle(TCommand request, CancellationToken cancellationToken)
    {
        return await HandleAsync(request, cancellationToken);
    }

    protected abstract Task<TResult> HandleAsync(TCommand request, CancellationToken cancellationToken);
}
