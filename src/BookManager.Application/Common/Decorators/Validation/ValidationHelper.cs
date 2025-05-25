using FluentResults;
using FluentValidation;
using FluentValidation.Results;

namespace BookManager.Application.Common.Decorators.Validation;

internal static class ValidationHelper
{
    public static async Task<List<ValidationFailure>> Validate<T>(
        T instance,
        IEnumerable<IValidator<T>> validators,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<T>(instance);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        return validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();
    }

    private static List<Error> ToErrors(List<ValidationFailure> failures)
    {
        return failures
            .Select(f => new Error(f.ErrorMessage)
                .WithMetadata("PropertyName", f.PropertyName))
            .ToList();
    }

    public static Result HandleValidationResult(List<ValidationFailure> failures)
    {
        return Result.Fail(ToErrors(failures));
    }

    public static Result<T> HandleValidationResult<T>(List<ValidationFailure> failures)
    {
        return Result.Fail<T>(ToErrors(failures));
    }


}