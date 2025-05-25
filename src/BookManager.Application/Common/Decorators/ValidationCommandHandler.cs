using BookManager.Application.Interfaces.Messaging;
using FluentResults;
using FluentValidation;

namespace BookManager.Application.Common.Decorators;

internal sealed class ValidationCommandHandler<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> innerHandler,
    IEnumerable<IValidator<TCommand>> validators)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TCommand>(command);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .ToList();

        if (!failures.Any())
            return await innerHandler.Handle(command, cancellationToken);

        var errors = failures
            .Select(f => new Error(f.ErrorMessage).WithMetadata("PropertyName", f.PropertyName))
            .ToList();

        return Result.Fail<TResponse>(errors);
    }
}