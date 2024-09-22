using LicenseeRecords.Models;

namespace LicenseeRecords.Web.Services.Interfaces;
public interface IProductDataService
{
	Task<(Product[]? products, string? errorMessage)> GetProducts();
	Task<(Product? product, string? errorMessage)> GetProduct(int productId);
	Task<(Product? product, string? successMessage, string? errorMessage)> CreateProduct(Product product);
	Task<(string? successMessage, string? errorMessage)> UpdateProduct(int productId, Product product);
	Task<(string? successMessage, string? errorMessage)> DeleteProduct(int productId);
}