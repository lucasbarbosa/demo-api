namespace DemoApi.Application.Validators.Products;

using DemoApi.Application.Models.Products;
using FluentValidation;
public class ProductValidator : AbstractValidator<ProductViewModel>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(p => p.Weight)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than 0");
    }
}