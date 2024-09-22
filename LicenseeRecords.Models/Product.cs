using FluentValidation;

namespace LicenseeRecords.Models;

public class Product
{
	public int ProductId { get; set; }
	public string ProductName { get; set; } = string.Empty;
}


public class ProductValidator : AbstractValidator<Product>
{
	public ProductValidator()
	{
		RuleFor(p => p.ProductId).NotNull();
		RuleFor(p => p.ProductName).NotEmpty();
	}
}

