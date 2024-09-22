using LicenseeRecords.Models;
using LicenseeRecords.Web.Services.Interfaces;

namespace LicenseeRecords.Web.Services.DataServices;
public class ProductDataService : IProductDataService
{
	private readonly HttpClient _httpClient;

	public ProductDataService(IHttpClientFactory httpClientFactory)
	{
		_httpClient = httpClientFactory.CreateClient("LicenseeRecordsApi");
	}

	public async Task<(Product[]? products, string? errorMessage)> GetProducts()
	{
		HttpResponseMessage response = await _httpClient.GetAsync("product");

		if (response.IsSuccessStatusCode)
		{
			Product[]? products = await response.Content.ReadFromJsonAsync<Product[]>();

			return products is not null ? (products, null) : (null, "An unexpected error occurred.");
		}
		else
		{
			string? errorMessage = await response.Content.ReadAsStringAsync();

			return errorMessage is not null ? ((Product[]?, string?))(null, errorMessage) : (null, "An unexpected error occurred.");
		}
	}

	public async Task<(Product? product, string? errorMessage)> GetProduct(int productId)
	{
		HttpResponseMessage response = await _httpClient.GetAsync($"product/{productId}");

		if (response.IsSuccessStatusCode)
		{
			Product? product = await response.Content.ReadFromJsonAsync<Product>();
			return (product, null);
		}
		else
		{
			string? errorMessage = await response.Content.ReadAsStringAsync();

			return errorMessage is not null ? ((Product?, string?))(null, errorMessage) : (null, "An unexpected error occurred.");
		}
	}

	public async Task<(Product? product, string? successMessage, string? errorMessage)> CreateProduct(Product product)
	{
		HttpResponseMessage response = await _httpClient.PostAsJsonAsync("product", product);

		if (response.IsSuccessStatusCode)
		{
			Product? createdProduct = await response.Content.ReadFromJsonAsync<Product>();
			return createdProduct is not null ? (createdProduct, $"{createdProduct.ProductName} Successfully Created", null) : (null, null, "Product Created Successfully But An Unexpected Error Occurred");
		}
		else
		{
			string? errorMessage = await response.Content.ReadAsStringAsync();

			return errorMessage is not null ? ((Product?, string?, string?))(null, null, errorMessage) : (null, null, "An Unexpected Error Occurred");
		}
	}

	public async Task<(string? successMessage, string? errorMessage)> UpdateProduct(int productId, Product product)

	{
		HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"product/{productId}", product);

		if (response.IsSuccessStatusCode)
		{
			return ($"Successfully Updated Product: {product.ProductName}", null);
		}
		else
		{
			string? errorMessage = await response.Content.ReadAsStringAsync();

			return errorMessage is not null ? (null, errorMessage) : (null, "An Unexpected Error Occurred");
		}
	}

	public async Task<(string? successMessage, string? errorMessage)> DeleteProduct(int productId)

	{
		HttpResponseMessage response = await _httpClient.DeleteAsync($"product/{productId}");

		if (response.IsSuccessStatusCode)
		{
			return ("Product Successfully Deleted", null);
		}
		else
		{
			string? errorMessage = await response.Content.ReadAsStringAsync();

			return errorMessage is not null ? (null, errorMessage) : (null, "An Unexpected Error Occurred");
		}
	}
}