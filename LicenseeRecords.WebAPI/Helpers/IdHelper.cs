using LicenseeRecords.Models;
using LicenseeRecords.WebAPI.Data;

namespace LicenseeRecords.WebAPI.Helpers;
public class IdHelper(IDataManager dataManager)
{
	public void AutoAssignNewIdsToNewAccount(Account account)
	{
		if (account.AccountId == 0)
		{
			account.AccountId = GetNextAccountId();
		}

		AutoAssignLicenceIds(account);
	}

	public void AutoAssignNewIdsToUpdatedAccount(Account account)
	{
		AutoAssignLicenceIds(account);
	}

	public void AutoAssignNewIdToProduct(Product product)
	{
		if (product.ProductId == 0)
		{
			product.ProductId = GetNextProductId();
		}
	}

	private int GetNextAccountId()
	{
		return dataManager.Accounts
			.Select(a => a.AccountId)
			.Max() + 1;
	}

	private int GetNextLicenceId()
	{
		return dataManager.Accounts
			.SelectMany(a => a.ProductLicence)
			.Max(pl => pl.LicenceId) + 1;
	}

	private int GetNextProductId()
	{
		return dataManager.Products
			.Select(p => p.ProductId)
			.Max() + 1;
	}

	private void AutoAssignLicenceIds(Account account)
	{
		int nextLicenceId = GetNextLicenceId();

		foreach (ProductLicence productLicence in account.ProductLicence)
		{
			if (productLicence.LicenceId == 0)
			{
				productLicence.LicenceId = nextLicenceId;
				nextLicenceId++;
			}
		}
	}
}
