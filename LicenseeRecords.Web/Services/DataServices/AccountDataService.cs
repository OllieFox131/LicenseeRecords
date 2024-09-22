using LicenseeRecords.Models;
using LicenseeRecords.Web.Services.Interfaces;

namespace LicenseeRecords.Web.Services.DataServices;
public class AccountDataService : IAccountDataService
{
	private readonly HttpClient _httpClient;

	public AccountDataService(IHttpClientFactory httpClientFactory)
	{
		_httpClient = httpClientFactory.CreateClient("LicenseeRecordsApi");
	}

	public async Task<(Account[]? accounts, string? errorMessage)> GetAccounts()
	{
		HttpResponseMessage response = await _httpClient.GetAsync("account");

		if (response.IsSuccessStatusCode)
		{
			Account[]? accounts = await response.Content.ReadFromJsonAsync<Account[]>();

			return accounts is not null ? (accounts, null) : (null, "An unexpected error occurred.");
		}
		else
		{
			string? errorMessage = await response.Content.ReadAsStringAsync();

			return errorMessage is not null ? ((Account[]?, string?))(null, errorMessage) : (null, "An unexpected error occurred.");
		}
	}

	public async Task<(Account? account, string? errorMessage)> GetAccount(int accountId)
	{
		HttpResponseMessage response = await _httpClient.GetAsync($"account/{accountId}");

		if (response.IsSuccessStatusCode)
		{
			Account? account = await response.Content.ReadFromJsonAsync<Account>();
			return (account, null);
		}
		else
		{
			string? errorMessage = await response.Content.ReadAsStringAsync();

			return errorMessage is not null ? ((Account?, string?))(null, errorMessage) : (null, "An unexpected error occurred.");
		}
	}

	public async Task<(Account? account, string? successMessage, string? errorMessage)> CreateAccount(Account account)
	{
		HttpResponseMessage response = await _httpClient.PostAsJsonAsync("account", account);

		if (response.IsSuccessStatusCode)
		{
			Account? createdAccount = await response.Content.ReadFromJsonAsync<Account>();
			return createdAccount is not null ? (createdAccount, $"{createdAccount.AccountName} Successfully Created", null) : (null, null, "Account Created Successfully But An Unexpected Error Occurred");
		}
		else
		{
			string? errorMessage = await response.Content.ReadAsStringAsync();

			return errorMessage is not null ? ((Account?, string?, string?))(null, null, errorMessage) : (null, null, "An Unexpected Error Occurred");
		}
	}

	public async Task<(string? successMessage, string? errorMessage)> UpdateAccount(int accountId, Account account)

	{
		HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"account/{accountId}", account);

		if (response.IsSuccessStatusCode)
		{
			return ($"Successfully Updated Account: {account.AccountName}", null);
		}
		else
		{
			string? errorMessage = await response.Content.ReadAsStringAsync();

			return errorMessage is not null ? (null, errorMessage) : (null, "An Unexpected Error Occurred");
		}
	}

	public async Task<(string? successMessage, string? errorMessage)> DeleteAccount(int accountId)

	{
		HttpResponseMessage response = await _httpClient.DeleteAsync($"account/{accountId}");

		if (response.IsSuccessStatusCode)
		{
			return ("Account Successfully Deleted", null);
		}
		else
		{
			string? errorMessage = await response.Content.ReadAsStringAsync();

			return errorMessage is not null ? (null, errorMessage) : (null, "An Unexpected Error Occurred");
		}
	}
}