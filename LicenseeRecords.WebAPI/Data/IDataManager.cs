using LicenseeRecords.Models;

namespace LicenseeRecords.WebAPI.Data;
public interface IDataManager
{
	List<Account> Accounts { get; }
	List<Product> Products { get; }

	void SaveData();
}