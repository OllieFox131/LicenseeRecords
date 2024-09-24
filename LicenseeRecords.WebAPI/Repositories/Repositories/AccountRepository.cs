using LicenseeRecords.Models;
using LicenseeRecords.WebAPI.Data;
using LicenseeRecords.WebAPI.Helpers;
using LicenseeRecords.WebAPI.Repositories.Interfaces;
using static LicenseeRecords.WebAPI.Exceptions.CustomExceptions;

namespace LicenseeRecords.WebAPI.Repositories.Repositories;
public class AccountRepository(IDataManager dataManager) : IAccountRepository
{
	public Account[] GetAccounts()
	{
		return dataManager.Accounts.ToArray();
	}

	public Account GetAccount(int accountId)
	{
		return dataManager.Accounts.FirstOrDefault(a => a.AccountId == accountId) ?? throw new NotFoundException($"No Account Found With ID: {accountId}");
	}

	public Account CreateAccount(Account account)
	{
		new IdHelper(dataManager).AutoAssignNewIdsToNewAccount(account);

		if (AccountExists(account.AccountId))
		{
			throw new ConflictException($"Account With ID: {account.AccountId} Already Exists");
		}

		dataManager.Accounts.Add(account);
		dataManager.SaveData();

		return account;
	}

	public void UpdateAccount(int accountId, Account account)
	{
		new IdHelper(dataManager).AutoAssignNewIdsToUpdatedAccount(account);

		if (accountId != account.AccountId)
		{
			throw new BadRequestException("Request ID Does Not Match Account ID");
		}

		if (!AccountExists(account.AccountId))
		{
			throw new NotFoundException($"No Account Found With ID: {accountId}");
		}

		Account oldAccount = dataManager.Accounts.Find(a => a.AccountId == accountId) ?? throw new NotFoundException($"No Account Found With ID: {accountId}");

		int positionOfOldAccount = dataManager.Accounts.IndexOf(oldAccount);

		dataManager.Accounts.Insert(positionOfOldAccount, account);
		dataManager.Accounts.Remove(oldAccount);

		dataManager.SaveData();
	}

	public void DeleteAccount(int accountId)
	{
		Account account = dataManager.Accounts.Find(a => a.AccountId == accountId) ?? throw new NotFoundException($"No Account Found With ID: {accountId}");

		dataManager.Accounts.Remove(account);
		dataManager.SaveData();
	}

	private bool AccountExists(int accountId)
	{
		return (dataManager.Accounts?.Any(a => a.AccountId == accountId)).GetValueOrDefault();
	}
}