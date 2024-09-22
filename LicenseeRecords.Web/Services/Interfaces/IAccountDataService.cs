using LicenseeRecords.Models;

namespace LicenseeRecords.Web.Services.Interfaces;
public interface IAccountDataService
{
	Task<(Account[]? accounts, string? errorMessage)> GetAccounts();
	Task<(Account? account, string? errorMessage)> GetAccount(int accountId);
	Task<(Account? account, string? successMessage, string? errorMessage)> CreateAccount(Account account);
	Task<(string? successMessage, string? errorMessage)> UpdateAccount(int accountId, Account account);
	Task<(string? successMessage, string? errorMessage)> DeleteAccount(int accountId);
}