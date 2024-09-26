using LicenseeRecords.Models;
using LicenseeRecords.Tests.Repository;
using LicenseeRecords.WebAPI.Repositories.Repositories;
using static LicenseeRecords.WebAPI.Exceptions.CustomExceptions;

namespace FleetApi.Tests.Repositories;

[TestFixture]
public class ProductRepositoryTests
{
	private MockDataManager _dataManager;
	private ProductRepository _repository;

	[SetUp]
	public void SetUp()
	{
		_dataManager = new MockDataManager();

		MockDatabaseHelper.CreateStandardDatabase(_dataManager);

		_repository = new ProductRepository(_dataManager);
	}

	[Test]
	public void GetProducts_ShouldReturnAllProducts()
	{
		Product[] Products = _repository.GetProducts();

		Assert.That(Products, Has.Length.EqualTo(3));
	}

	[Test]
	public void GetProduct_ValidId_ShouldReturnProduct()
	{
		const int ProductId = 1;
		Product Product = _repository.GetProduct(ProductId);

		Assert.That(Product, Is.Not.Null);
		Assert.That(Product.ProductName, Is.EqualTo($"Product {ProductId}"));
	}

	[Test]
	public void GetProduct_InvalidId_ShouldThrowNotFoundException()
	{
		const int invalidId = 999;
		NotFoundException ex = Assert.Throws<NotFoundException>(() => _repository.GetProduct(invalidId));

		Assert.That(ex.Message, Is.EqualTo($"No Product Found With ID: {invalidId}"));
	}

	[Test]
	public void CreateProduct_ValidProduct_ShouldAddProduct()
	{
		const int newProductId = 4;
		Product Product = new() { ProductName = $"Product {newProductId}" };

		Product returnedProduct = _repository.CreateProduct(Product);
		Product retrievedProduct = _repository.GetProduct(returnedProduct.ProductId);

		Assert.That(returnedProduct, Is.Not.Null);
		Assert.That(retrievedProduct, Is.Not.Null);
		Assert.That(returnedProduct.ProductName, Is.EqualTo(Product.ProductName));
		Assert.That(retrievedProduct.ProductName, Is.EqualTo(Product.ProductName));
	}

	[Test]
	public void CreateProduct_DuplicateProduct_ShouldThrowConflictException()
	{
		const int ProductId = 1;

		Product Product = _repository.GetProduct(ProductId);

		ConflictException ex = Assert.Throws<ConflictException>(() => _repository.CreateProduct(Product));

		Assert.That(Product, Is.Not.Null);
		Assert.That(Product.ProductName, Is.EqualTo($"Product {ProductId}"));
		Assert.That(ex.Message, Is.EqualTo($"Product With ID: {Product.ProductId} Already Exists"));
	}

	[Test]
	public void UpdateProduct_ValidProduct_ShouldUpdateProduct()
	{
		const int ProductId = 1;

		Product Product = _repository.GetProduct(ProductId);

		Assert.That(Product, Is.Not.Null);
		Assert.That(Product.ProductName, Is.EqualTo($"Product {ProductId}"));

		Product.ProductName = $"Updated Product {ProductId}";

		_repository.UpdateProduct(Product.ProductId, Product);

		Product updatedProduct = _repository.GetProduct(Product.ProductId);

		Assert.That(updatedProduct.ProductName, Is.EqualTo($"Updated Product {ProductId}"));
	}

	[Test]
	public void UpdateProduct_RequestIdProductIdMismatch_ShouldThrowBadRequestException()
	{
		const int ProductId = 1;
		const int invalidId = 999;

		Product Product = _repository.GetProduct(ProductId);

		BadRequestException ex = Assert.Throws<BadRequestException>(() => _repository.UpdateProduct(invalidId, Product));

		Assert.That(Product, Is.Not.Null);
		Assert.That(Product.ProductName, Is.EqualTo($"Product {ProductId}"));
		Assert.That(ex.Message, Is.EqualTo("Request ID Does Not Match Product ID"));
	}

	[Test]
	public void UpdateProduct_InvalidId_ShouldThrowNotFoundException()
	{
		const int invalidId = 999;
		Product Product = new() { ProductId = invalidId, ProductName = $"Product {invalidId}" };

		NotFoundException ex = Assert.Throws<NotFoundException>(() => _repository.UpdateProduct(invalidId, Product));

		Assert.That(ex.Message, Is.EqualTo($"No Product Found With ID: {Product.ProductId}"));
	}

	[Test]
	public void DeleteProduct_Valid_ShouldRemoveProduct()
	{
		const int ProductId = 1;

		Product Product = _repository.GetProduct(ProductId);

		_repository.DeleteProduct(ProductId);

		NotFoundException ex = Assert.Throws<NotFoundException>(() => _repository.GetProduct(ProductId));

		Assert.That(Product, Is.Not.Null);
		Assert.That(Product.ProductName, Is.EqualTo($"Product {ProductId}"));
		Assert.That(ex.Message, Is.EqualTo($"No Product Found With ID: {ProductId}"));
	}

	[Test]
	public void DeleteProduct_InvalidId_ShouldThrowNotFoundException()
	{
		const int invalidId = 999;

		NotFoundException ex = Assert.Throws<NotFoundException>(() => _repository.DeleteProduct(invalidId));

		Assert.That(ex.Message, Is.EqualTo($"No Product Found With ID: {invalidId}"));
	}
}