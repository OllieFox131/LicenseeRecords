using LicenseeRecords.Models;

namespace LicenseeRecords.WebAPI.Repositories.Interfaces;
public interface IProductRepository
{
	public Product[] GetProducts();
	public Product GetProduct(int productId);
	public Product CreateProduct(Product product);
	public void UpdateProduct(int productId, Product product);
	public void DeleteProduct(int productId);
}