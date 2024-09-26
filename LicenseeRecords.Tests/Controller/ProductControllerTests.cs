using FluentValidation;
using FluentValidation.Results;
using LicenseeRecords.Models;
using LicenseeRecords.WebAPI.Controllers;
using LicenseeRecords.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using static LicenseeRecords.WebAPI.Exceptions.CustomExceptions;

namespace LicenseeRecords.Tests.Controller;

[TestFixture]
public class ProductControllerTests
{
	private IProductRepository _repository;
	private IValidator<Product> _validator;
	private ProductController _controller;

	[SetUp]
	public void Setup()
	{
		_repository = Substitute.For<IProductRepository>();
		_validator = Substitute.For<IValidator<Product>>();
		_controller = new ProductController(_repository, _validator);
	}

	[Test]
	public void GetProducts_ReturnsOkObjectResult()
	{
		// Arrange
		Product[] Products = [
			new() {ProductId = 1, ProductName = "Product 1"},
			new() {ProductId = 2, ProductName = "Product 2"},
			new() {ProductId = 3, ProductName = "Product 3"}
		];

		_repository.GetProducts().Returns(Products);

		// Act
		ActionResult<Product[]> result = _controller.GetProducts();
		OkObjectResult? okResult = result.Result as OkObjectResult;

		// Assert
		Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
		Assert.That(okResult, Is.Not.Null);
		Assert.That(okResult.Value, Is.EqualTo(Products));
	}

	[Test]
	public void GetProduct_ValidId_ReturnsOkObjectResult()
	{
		// Arrange
		const int ProductId = 1;
		Product Product = new() { ProductId = ProductId, ProductName = $"Product {ProductId}" };

		_repository.GetProduct(ProductId).Returns(Product);

		// Act
		ActionResult<Product> result = _controller.GetProduct(ProductId);
		OkObjectResult? okResult = result.Result as OkObjectResult;

		// Assert
		Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
		Assert.That(okResult, Is.Not.Null);
		Assert.That(okResult.Value, Is.EqualTo(Product));
	}

	[Test]
	public void GetProduct_InvalidId_ReturnsNotFoundObjectResult()
	{
		// Arrange
		const int invalidId = 999;
		string exceptionMessage = $"No Product Found With ID: {invalidId}";

		_repository.GetProduct(invalidId).Throws(new NotFoundException(exceptionMessage));

		// Act
		ActionResult<Product> result = _controller.GetProduct(invalidId);
		NotFoundObjectResult? notFoundResult = result.Result as NotFoundObjectResult;

		// Assert
		Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
		Assert.That(notFoundResult, Is.Not.Null);
		Assert.That(notFoundResult.Value, Is.EqualTo(exceptionMessage));
	}

	[Test]
	public void CreateProduct_ValidProduct_ReturnsCreatedAtActionResult()
	{
		// Arrange
		const int ProductId = 1;
		Product Product = new() { ProductId = ProductId, ProductName = $"Product {ProductId}" };
		Product createdProduct = new() { ProductId = ProductId, ProductName = $"Product {ProductId}" };

		_validator.Validate(Product).Returns(new ValidationResult());
		_repository.CreateProduct(Product).Returns(createdProduct);

		// Act
		ActionResult<Product> result = _controller.CreateProduct(Product);
		CreatedAtActionResult? createdAtActionResult = result.Result as CreatedAtActionResult;

		// Assert
		Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
		Assert.That(createdAtActionResult, Is.Not.Null);
		Assert.That(createdAtActionResult.Value, Is.EqualTo(createdProduct));
		Assert.That(createdAtActionResult.ActionName, Is.EqualTo(nameof(_controller.GetProduct)));
		Assert.That(createdAtActionResult.RouteValues!["ProductId"], Is.EqualTo(createdProduct.ProductId));
	}

	[Test]
	public void CreateProduct_InvalidProduct_ReturnsBadRequestObjectResult()
	{
		// Arrange
		const int ProductId = 1;
		Product Product = new() { ProductId = ProductId, ProductName = $"Product {ProductId}" };

		_validator.Validate(Product).Returns(new ValidationResult(new List<ValidationFailure>
		{
			new("Test Failure", "Test Validation Failure")
		}));

		// Act
		ActionResult<Product> result = _controller.CreateProduct(Product);
		BadRequestObjectResult? badRequestObjectResult = result.Result as BadRequestObjectResult;
		string? validationErrorsString = badRequestObjectResult?.Value as string;

		// Assert
		Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
		Assert.That(validationErrorsString, Does.Contain("Test Validation Failure"));
	}

	[Test]
	public void CreateProduct_DuplicateProduct_ReturnsConflictObjectResult()
	{
		// Arrange
		const int duplicateId = 1;
		Product Product = new() { ProductId = duplicateId, ProductName = $"Product {duplicateId}" };
		string exceptionMessage = $"Product With ID: {duplicateId} Already Exists";

		_validator.Validate(Product).Returns(new ValidationResult());
		_repository.CreateProduct(Product).Throws(new ConflictException(exceptionMessage));

		// Act
		ActionResult<Product> result = _controller.CreateProduct(Product);
		ConflictObjectResult? conflictObjectResult = result.Result as ConflictObjectResult;

		// Assert
		Assert.That(conflictObjectResult, Is.InstanceOf<ConflictObjectResult>());
		Assert.That(conflictObjectResult, Is.Not.Null);
		Assert.That(conflictObjectResult.Value, Is.EqualTo(exceptionMessage));
	}

	[Test]
	public void UpdateProduct_ValidProduct_ReturnsNoContentResult()
	{
		// Arrange
		const int ProductId = 1;
		Product Product = new() { ProductId = ProductId, ProductName = $"Updated Product {ProductId}" };

		_validator.Validate(Product).Returns(new ValidationResult());
		_repository.UpdateProduct(ProductId, Product);

		// Act
		IActionResult result = _controller.UpdateProduct(ProductId, Product);

		// Assert
		Assert.That(result, Is.InstanceOf<NoContentResult>());
	}

	[Test]
	public void UpdateProduct_InvalidProduct_ReturnsBadRequestObjectResult()
	{
		// Arrange
		const int ProductId = 1;
		Product Product = new() { ProductId = ProductId, ProductName = $"Updated Product {ProductId}" };

		_validator.Validate(Product).Returns(new ValidationResult(new List<ValidationFailure>
		{
			new("Test Failure", "Test Validation Failure")
		}));

		// Act
		IActionResult result = _controller.UpdateProduct(ProductId, Product);
		BadRequestObjectResult? badRequestResult = result as BadRequestObjectResult;
		string? validationErrorsString = badRequestResult?.Value as string;

		// Assert
		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		Assert.That(badRequestResult?.Value, Is.InstanceOf<string>());
		Assert.That(validationErrorsString, Does.Contain("Test Validation Failure"));
	}

	[Test]
	public void UpdateProduct_NonMatchingId_ReturnsBadRequestObjectResult()
	{
		// Arrange
		const int ProductId = 1;
		const int nonMatchingProductId = 1;
		Product Product = new() { ProductId = ProductId, ProductName = $"Updated Product {ProductId}" };
		const string exceptionMessage = "Request ID Does Not Match Product ID";

		_validator.Validate(Product).Returns(new ValidationResult());
		_repository.When(x => x.UpdateProduct(nonMatchingProductId, Product)).Do(x => { throw new BadRequestException(exceptionMessage); });

		// Act
		IActionResult result = _controller.UpdateProduct(nonMatchingProductId, Product);
		BadRequestObjectResult? badRequestResult = result as BadRequestObjectResult;

		// Assert
		Assert.That(badRequestResult, Is.InstanceOf<BadRequestObjectResult>());
		Assert.That(badRequestResult, Is.Not.Null);
		Assert.That(badRequestResult.Value, Is.EqualTo(exceptionMessage));
	}

	[Test]
	public void UpdateProduct_DuplicateProduct_ReturnsNotFoundObjectResult()
	{
		// Arrange
		const int invalidId = 999;
		Product Product = new() { ProductId = invalidId, ProductName = $"Product {invalidId}" };
		string exceptionMessage = $"No Product Found With ID: {invalidId}";

		_validator.Validate(Product).Returns(new ValidationResult());
		_repository.When(x => x.UpdateProduct(invalidId, Product)).Do(x => { throw new NotFoundException(exceptionMessage); });

		// Act
		IActionResult result = _controller.UpdateProduct(invalidId, Product);
		NotFoundObjectResult? notFoundObjectResult = result as NotFoundObjectResult;

		// Assert
		Assert.That(notFoundObjectResult, Is.InstanceOf<NotFoundObjectResult>());
		Assert.That(notFoundObjectResult, Is.Not.Null);
		Assert.That(notFoundObjectResult.Value, Is.EqualTo(exceptionMessage));
	}

	[Test]
	public void DeleteProduct_ValidId_ReturnsNoContentResult()
	{
		// Arrange
		const int ProductId = 1;

		_repository.DeleteProduct(ProductId);

		// Act
		IActionResult result = _controller.DeleteProduct(ProductId);

		// Assert
		Assert.That(result, Is.InstanceOf<NoContentResult>());
	}

	[Test]
	public void DeleteProduct_InvalidId_ReturnsNotFoundObjectResult()
	{
		// Arrange
		const int invalidId = 999;
		string exceptionMessage = $"No Product Found With ID: {invalidId}";

		_repository.When(x => x.DeleteProduct(invalidId)).Do(x => { throw new NotFoundException(exceptionMessage); });

		// Act
		IActionResult result = _controller.DeleteProduct(invalidId);
		NotFoundObjectResult? notFoundObjectResult = result as NotFoundObjectResult;

		// Assert
		Assert.That(notFoundObjectResult, Is.InstanceOf<NotFoundObjectResult>());
		Assert.That(notFoundObjectResult, Is.Not.Null);
		Assert.That(notFoundObjectResult.Value, Is.EqualTo(exceptionMessage));
	}
}
