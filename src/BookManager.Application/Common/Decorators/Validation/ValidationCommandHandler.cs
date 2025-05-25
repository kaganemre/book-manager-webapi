using BookManager.Application.Interfaces.Messaging;
using FluentResults;
using FluentValidation;

namespace BookManager.Application.Common.Decorators.Validation;

internal sealed class ValidationCommandBaseHandler<TCommand>(
    ICommandHandler<TCommand> innerHandler,
    IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var failures = await ValidationHelper.Validate(command, validators, cancellationToken);

        if (!failures.Any())
            return await innerHandler.Handle(command, cancellationToken);

        return ValidationHelper.HandleValidationResult(failures);
    }
}
internal sealed class ValidationCommandHandler<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler,
    IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var failures = await ValidationHelper.Validate(command, validators, cancellationToken);

        if (!failures.Any())
            return await innerHandler.Handle(command, cancellationToken);

        return ValidationHelper.HandleValidationResult<TResponse>(failures);
    }
}