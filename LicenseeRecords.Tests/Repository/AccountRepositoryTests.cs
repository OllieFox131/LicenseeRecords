using LicenseeRecords.Models;
using LicenseeRecords.Tests.Repository;
using LicenseeRecords.WebAPI.Repositories.Repositories;
using static LicenseeRecords.WebAPI.Exceptions.CustomExceptions;

namespace FleetApi.Tests.Repositories;

[TestFixture]
public class AccountRepositoryTests
{
	private MockDataManager _dataManager;
	private AccountRepository _repository;

	[SetUp]
	public void SetUp()
	{
		_dataManager = new MockDataManager();

		MockDatabaseHelper.CreateStandardDatabase(_dataManager);

		_repository = new AccountRepository(_dataManager);
	}

	[Test]
	public void GetAccounts_ShouldReturnAllAccounts()
	{
		Account[] Accounts = _repository.GetAccounts();

		Assert.That(Accounts, Has.Length.EqualTo(3));
	}

	[Test]
	public void GetAccount_ValidId_ShouldReturnAccount()
	{
		const int AccountId = 1;
		Account Account = _repository.GetAccount(AccountId);

		Assert.That(Account, Is.Not.Null);
		Assert.That(Account.AccountName, Is.EqualTo($"Account {AccountId}"));
	}

	[Test]
	public void GetAccount_InvalidId_ShouldThrowNotFoundException()
	{
		const int invalidId = 999;
		NotFoundException ex = Assert.Throws<NotFoundException>(() => _repository.GetAccount(invalidId));

		Assert.That(ex.Message, Is.EqualTo($"No Account Found With ID: {invalidId}"));
	}

	[Test]
	public void CreateAccount_ValidAccount_ShouldAddAccount()
	{
		const int newAccountId = 4;
		Account Account = new() { AccountName = $"Account {newAccountId}" };

		Account returnedAccount = _repository.CreateAccount(Account);
		Account retrievedAccount = _repository.GetAccount(returnedAccount.AccountId);

		Assert.That(returnedAccount, Is.Not.Null);
		Assert.That(retrievedAccount, Is.Not.Null);
		Assert.That(returnedAccount.AccountName, Is.EqualTo(Account.AccountName));
		Assert.That(retrievedAccount.AccountName, Is.EqualTo(Account.AccountName));
	}

	[Test]
	public void CreateAccount_DuplicateAccount_ShouldThrowConflictException()
	{
		const int AccountId = 1;

		Account Account = _repository.GetAccount(AccountId);

		ConflictException ex = Assert.Throws<ConflictException>(() => _repository.CreateAccount(Account));

		Assert.That(Account, Is.Not.Null);
		Assert.That(Account.AccountName, Is.EqualTo($"Account {AccountId}"));
		Assert.That(ex.Message, Is.EqualTo($"Account With ID: {Account.AccountId} Already Exists"));
	}

	[Test]
	public void UpdateAccount_ValidAccount_ShouldUpdateAccount()
	{
		const int AccountId = 1;

		Account Account = _repository.GetAccount(AccountId);

		Assert.That(Account, Is.Not.Null);
		Assert.That(Account.AccountName, Is.EqualTo($"Account {AccountId}"));

		Account.AccountName = $"Updated Account {AccountId}";

		_repository.UpdateAccount(Account.AccountId, Account);

		Account updatedAccount = _repository.GetAccount(Account.AccountId);

		Assert.That(updatedAccount.AccountName, Is.EqualTo($"Updated Account {AccountId}"));
	}

	[Test]
	public void UpdateAccount_RequestIdAccountIdMismatch_ShouldThrowBadRequestException()
	{
		const int AccountId = 1;
		const int invalidId = 999;

		Account Account = _repository.GetAccount(AccountId);

		BadRequestException ex = Assert.Throws<BadRequestException>(() => _repository.UpdateAccount(invalidId, Account));

		Assert.That(Account, Is.Not.Null);
		Assert.That(Account.AccountName, Is.EqualTo($"Account {AccountId}"));
		Assert.That(ex.Message, Is.EqualTo("Request ID Does Not Match Account ID"));
	}

	[Test]
	public void UpdateAccount_InvalidId_ShouldThrowNotFoundException()
	{
		const int invalidId = 999;
		Account Account = new() { AccountId = invalidId, AccountName = $"Account {invalidId}" };

		NotFoundException ex = Assert.Throws<NotFoundException>(() => _repository.UpdateAccount(invalidId, Account));

		Assert.That(ex.Message, Is.EqualTo($"No Account Found With ID: {Account.AccountId}"));
	}

	[Test]
	public void DeleteAccount_Valid_ShouldRemoveAccount()
	{
		const int AccountId = 1;

		Account Account = _repository.GetAccount(AccountId);

		_repository.DeleteAccount(AccountId);

		NotFoundException ex = Assert.Throws<NotFoundException>(() => _repository.GetAccount(AccountId));

		Assert.That(Account, Is.Not.Null);
		Assert.That(Account.AccountName, Is.EqualTo($"Account {AccountId}"));
		Assert.That(ex.Message, Is.EqualTo($"No Account Found With ID: {AccountId}"));
	}

	[Test]
	public void DeleteAccount_InvalidId_ShouldThrowNotFoundException()
	{
		const int invalidId = 999;

		NotFoundException ex = Assert.Throws<NotFoundException>(() => _repository.DeleteAccount(invalidId));

		Assert.That(ex.Message, Is.EqualTo($"No Account Found With ID: {invalidId}"));
	}
}