using FluentValidation;

namespace BookManager.Application.Features.Books.Shared;

public abstract class BaseBookCommandValidator<T> : AbstractValidator<T>
    where T : BookCommandBase
{
    protected BaseBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(150)
            .WithMessage("Title must not exceed 150 characters.");

        RuleFor(x => x.Author)
            .NotEmpty()
            .WithMessage("Author is required.")
            .MaximumLength(100)
            .WithMessage("Author must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description must not exceed 1,000 characters.");

        RuleFor(x => x.ISBN)
            .NotEmpty()
            .WithMessage("ISBN is required.")
            .Matches(@"^\d{10}(\d{3})?$")
            .WithMessage("ISBN must be either 10 or 13 digits long.");

        RuleFor(x => x.PageCount)
            .GreaterThan(0)
            .When(x => x.PageCount.HasValue)
            .WithMessage("Page count must be greater than zero.");

        RuleFor(x => x.PublishedDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Published date cannot be in the future.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity must be zero or greater.");

        RuleFor(x => x.Source)
            .NotEmpty()
            .WithMessage("Source is required.")
            .MaximumLength(255)
            .WithMessage("Source must not exceed 255 characters.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category ID must be a positive number.");
    }
}