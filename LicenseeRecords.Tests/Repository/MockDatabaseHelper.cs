using LicenseeRecords.Models;

namespace LicenseeRecords.Tests.Repository;
public static class MockDatabaseHelper
{
	public static void CreateStandardDatabase(MockDataManager dataManager)
	{
		List<Product> products = [
			new() {ProductId = 1, ProductName = "Product 1"},
			new() {ProductId = 2, ProductName = "Product 2"},
			new() {ProductId = 3, ProductName = "Product 3"},
		];

		List<Account> accounts = [
			new() {AccountId = 1, AccountName = "Account 1"},
			new() {AccountId = 2, AccountName = "Account 2"},
			new() {AccountId = 3, AccountName = "Account 3"},
		];

		dataManager.Products.AddRange(products);
		dataManager.Accounts.AddRange(accounts);

		dataManager.SaveData();
	}
}
