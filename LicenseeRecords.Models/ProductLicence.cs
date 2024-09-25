using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace LicenseeRecords.Models;

public class ProductLicence
{
	[Key]
	public int LicenceId { get; set; }
	public string? LicenceStatus { get; set; }
	public DateTime? LicenceFromDate { get; set; }
	public DateTime? LicenceToDate { get; set; }
	public Product Product { get; set; } = new();
}


public class ProductLicenceValidator : AbstractValidator<ProductLicence>
{
	public ProductLicenceValidator()
	{
		RuleFor(pl => pl.LicenceId).NotNull().WithName("Licence Id");
		RuleFor(pl => pl.LicenceStatus).NotEmpty().WithName("Licence Status");
		RuleFor(pl => pl.LicenceFromDate).NotEmpty().WithName("Licence From Date");
		RuleFor(pl => pl.LicenceToDate).GreaterThanOrEqualTo(pl => pl.LicenceFromDate).WithName("Licence To Date");
		RuleFor(pl => pl.Product).SetValidator(new ProductValidator());
	}
}
