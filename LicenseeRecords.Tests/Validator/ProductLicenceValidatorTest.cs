using FluentValidation.TestHelper;
using LicenseeRecords.Models;

namespace LicenseeRecords.Tests.Validator;

[TestFixture]
public class ProductLicenceValidatorTest
{
	private ProductLicenceValidator validator;

	[SetUp]
	public void Setup()
	{
		validator = new ProductLicenceValidator();
	}

	[Test]
	public void EmptyProductLicence_ShouldHaveErrors()
	{
		ProductLicence productLicence = new();
		TestValidationResult<ProductLicence> result = validator.TestValidate(productLicence);
		result.ShouldHaveValidationErrorFor(pl => pl.LicenceStatus);
		result.ShouldHaveValidationErrorFor(pl => pl.LicenceFromDate);
	}

	[Test]
	public void LicenceToDateLessThanLicenceFromDate_ShouldHaveErrors()
	{
		ProductLicence productLicence = new() { LicenceFromDate = DateTime.Now.AddDays(1), LicenceToDate = DateTime.Now };
		TestValidationResult<ProductLicence> result = validator.TestValidate(productLicence);
		result.ShouldHaveValidationErrorFor(pl => pl.LicenceToDate);
	}

	[Test]
	public void LicenceToDateGreaterThanLicenceFromDate_ShouldHaveErrors()
	{
		ProductLicence productLicence = new() { LicenceFromDate = DateTime.Now, LicenceToDate = DateTime.Now.AddDays(1) };
		TestValidationResult<ProductLicence> result = validator.TestValidate(productLicence);
		result.ShouldNotHaveValidationErrorFor(pl => pl.LicenceToDate);
	}
}