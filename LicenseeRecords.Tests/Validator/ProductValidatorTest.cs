using FluentValidation.TestHelper;
using LicenseeRecords.Models;

namespace LicenseeRecords.Tests.Validator;

[TestFixture]
public class ProductValidatorTest
{
	private ProductValidator validator;

	[SetUp]
	public void Setup()
	{
		validator = new ProductValidator();
	}

	[Test]
	public void EmptyProduct_ShouldHaveErrors()
	{
		Product product = new();
		TestValidationResult<Product> result = validator.TestValidate(product);
		result.ShouldHaveValidationErrorFor(p => p.ProductName);
	}
}