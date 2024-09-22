using LicenseeRecords.Models;
using Newtonsoft.Json;

namespace LicenseeRecords.WebAPI.Data;

public class DataManager : IDataManager
{
	public List<Product> Products { get; private set; } = [];
	public List<Account> Accounts { get; private set; } = [];

	private readonly string _productsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Data", "products.json");
	private readonly string _accountsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Data", "accounts.json");

	public DataManager()
	{
		LoadJsonData();
	}

	private void LoadJsonData()
	{
		if (File.Exists(_productsFilePath))
		{
			string productsJson = File.ReadAllText(_productsFilePath);
			Products = JsonConvert.DeserializeObject<List<Product>>(productsJson) ?? [];
		}

		if (File.Exists(_accountsFilePath))
		{
			string accountsJson = File.ReadAllText(_accountsFilePath);
			Accounts = JsonConvert.DeserializeObject<List<Account>>(accountsJson) ?? [];
		}
	}

	public void SaveData()
	{
		File.WriteAllText(_productsFilePath, JsonConvert.SerializeObject(Products, Formatting.Indented));
		File.WriteAllText(_accountsFilePath, JsonConvert.SerializeObject(Accounts, Formatting.Indented));
	}
}
