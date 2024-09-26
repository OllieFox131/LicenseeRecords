using LicenseeRecords.Models;
using LicenseeRecords.WebAPI.Data;

namespace LicenseeRecords.Tests.Repository;
public class MockDataManager : IDataManager
{
	public List<Product> Products { get; private set; } = [];
	public List<Account> Accounts { get; private set; } = [];

	public MockDataManager()
	{
	}

	public void SaveData()
	{
	}
}
