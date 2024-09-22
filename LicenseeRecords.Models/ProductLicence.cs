using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace LicenseeRecords.Models;

public class ProductLicence
{
	[Key]
	public int LicenceId { get; set; }
	public string LicenceStatus { get; set; } = string.Empty;
	public DateTime LicenceFromDate { get; set; }
	public DateTime? LicenceToDate { get; set; }
	public Product Product { get; set; } = new();
}


public class ProductLicenceValidator : AbstractValidator<ProductLicence>
{
	public ProductLicenceValidator()
	{
		RuleFor(pl => pl.LicenceId).NotNull();
		RuleFor(pl => pl.LicenceStatus).NotEmpty();
		RuleFor(pl => pl.LicenceFromDate).NotEmpty();
		RuleFor(pl => pl.LicenceToDate).NotEmpty();
		RuleFor(pl => pl.LicenceToDate).GreaterThanOrEqualTo(pl => pl.LicenceFromDate);
	}
}
