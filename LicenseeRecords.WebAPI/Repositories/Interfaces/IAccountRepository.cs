using LicenseeRecords.Models;

namespace LicenseeRecords.WebAPI.Repositories.Interfaces;
public interface IAccountRepository
{
	public Account[] GetAccounts();
	public Account GetAccount(int accountId);
	public Account CreateAccount(Account account);
	public void UpdateAccount(int accountId, Account account);
	public void DeleteAccount(int accountId);
}