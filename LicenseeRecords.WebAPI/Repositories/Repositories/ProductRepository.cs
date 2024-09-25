using LicenseeRecords.Models;
using LicenseeRecords.WebAPI.Data;
using LicenseeRecords.WebAPI.Helpers;
using LicenseeRecords.WebAPI.Repositories.Interfaces;
using Microsoft.VisualBasic.FileIO;
using static LicenseeRecords.WebAPI.Exceptions.CustomExceptions;

namespace LicenseeRecords.WebAPI.Repositories.Repositories;
public class ProductRepository(IDataManager dataManager) : IProductRepository
{
	public Product[] GetProducts()
	{
		return dataManager.Products.ToArray();
	}

	public Product GetProduct(int productId)
	{
		return dataManager.Products.FirstOrDefault(a => a.ProductId == productId) ?? throw new NotFoundException($"No Product Found With ID: {productId}");
	}

	public Product CreateProduct(Product product)
	{
		new IdHelper(dataManager).AutoAssignNewIdToProduct(product);

		if (ProductExists(product.ProductId))
		{
			throw new ConflictException($"Product With ID: {product.ProductId} Already Exists");
		}

		dataManager.Products.Add(product);
		dataManager.SaveData();

		return product;
	}

	public void UpdateProduct(int productId, Product product)
	{
		if (productId != product.ProductId)
		{
			throw new BadRequestException("Request ID Does Not Match Product ID");
		}

		if (!ProductExists(product.ProductId))
		{
			throw new NotFoundException($"No Product Found With ID: {productId}");
		}

		Product oldProduct = dataManager.Products.Find(a => a.ProductId == productId) ?? throw new NotFoundException($"No Product Found With ID: {productId}");

		int positionOfOldProduct = dataManager.Products.IndexOf(oldProduct);

		dataManager.Products.Insert(positionOfOldProduct, product);
		dataManager.Products.Remove(oldProduct);

		dataManager.Accounts
			.SelectMany(a => a.ProductLicence)
			.Select(pl => pl.Product)
			.Where(pl => pl.ProductId == productId)
			.ToList()
			.ForEach(pl => pl.ProductName = product.ProductName);

		dataManager.SaveData();
	}

	public void DeleteProduct(int productId)
	{
		Product product = dataManager.Products.Find(a => a.ProductId == productId) ?? throw new NotFoundException($"No Product Found With ID: {productId}");

		if (dataManager.Accounts.SelectMany(a => a.ProductLicence).Select(pl => pl.Product).Any(p => p.ProductId == productId))
		{
			throw new ConflictException($"{product.ProductName} is in use and cannot be deleted.");
		}

		dataManager.Products.Remove(product);
		dataManager.SaveData();
	}

	private bool ProductExists(int productId)
	{
		return (dataManager.Products?.Any(a => a.ProductId == productId)).GetValueOrDefault();
	}
}