using FluentValidation;

namespace LicenseeRecords.Models;

public class Product
{
	public int ProductId { get; set; }
	public string? ProductName { get; set; }
}

public class ProductValidator : AbstractValidator<Product>
{
	public ProductValidator()
	{
		RuleFor(p => p.ProductId).NotNull().WithName("Product Id");
		RuleFor(p => p.ProductName).NotEmpty().WithName("Product Name");
	}
}
