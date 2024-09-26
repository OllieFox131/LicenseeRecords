using FluentValidation;
using FluentValidation.Results;
using LicenseeRecords.Models;
using LicenseeRecords.WebAPI.Controllers;
using LicenseeRecords.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using static LicenseeRecords.WebAPI.Exceptions.CustomExceptions;

namespace LicenseeRecords.Tests.Controller;

[TestFixture]
public class AccountControllerTests
{
	private IAccountRepository _repository;
	private IValidator<Account> _validator;
	private AccountController _controller;

	[SetUp]
	public void Setup()
	{
		_repository = Substitute.For<IAccountRepository>();
		_validator = Substitute.For<IValidator<Account>>();
		_controller = new AccountController(_repository, _validator);
	}

	[Test]
	public void GetAccounts_ReturnsOkObjectResult()
	{
		// Arrange
		Account[] accounts = [
			new() {AccountId = 1, AccountName = "Account 1"},
			new() {AccountId = 2, AccountName = "Account 2"},
			new() {AccountId = 3, AccountName = "Account 3"}
		];

		_repository.GetAccounts().Returns(accounts);

		// Act
		ActionResult<Account[]> result = _controller.GetAccounts();
		OkObjectResult? okResult = result.Result as OkObjectResult;

		// Assert
		Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
		Assert.That(okResult, Is.Not.Null);
		Assert.That(okResult.Value, Is.EqualTo(accounts));
	}

	[Test]
	public void GetAccount_ValidId_ReturnsOkObjectResult()
	{
		// Arrange
		const int accountId = 1;
		Account account = new() { AccountId = accountId, AccountName = $"Account {accountId}" };

		_repository.GetAccount(accountId).Returns(account);

		// Act
		ActionResult<Account> result = _controller.GetAccount(accountId);
		OkObjectResult? okResult = result.Result as OkObjectResult;

		// Assert
		Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
		Assert.That(okResult, Is.Not.Null);
		Assert.That(okResult.Value, Is.EqualTo(account));
	}

	[Test]
	public void GetAccount_InvalidId_ReturnsNotFoundObjectResult()
	{
		// Arrange
		const int invalidId = 999;
		string exceptionMessage = $"No Account Found With ID: {invalidId}";

		_repository.GetAccount(invalidId).Throws(new NotFoundException(exceptionMessage));

		// Act
		ActionResult<Account> result = _controller.GetAccount(invalidId);
		NotFoundObjectResult? notFoundResult = result.Result as NotFoundObjectResult;

		// Assert
		Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
		Assert.That(notFoundResult, Is.Not.Null);
		Assert.That(notFoundResult.Value, Is.EqualTo(exceptionMessage));
	}

	[Test]
	public void CreateAccount_ValidAccount_ReturnsCreatedAtActionResult()
	{
		// Arrange
		const int accountId = 1;
		Account account = new() { AccountId = accountId, AccountName = $"Account {accountId}" };
		Account createdAccount = new() { AccountId = accountId, AccountName = $"Account {accountId}" };

		_validator.Validate(account).Returns(new ValidationResult());
		_repository.CreateAccount(account).Returns(createdAccount);

		// Act
		ActionResult<Account> result = _controller.CreateAccount(account);
		CreatedAtActionResult? createdAtActionResult = result.Result as CreatedAtActionResult;

		// Assert
		Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
		Assert.That(createdAtActionResult, Is.Not.Null);
		Assert.That(createdAtActionResult.Value, Is.EqualTo(createdAccount));
		Assert.That(createdAtActionResult.ActionName, Is.EqualTo(nameof(_controller.GetAccount)));
		Assert.That(createdAtActionResult.RouteValues!["accountId"], Is.EqualTo(createdAccount.AccountId));
	}

	[Test]
	public void CreateAccount_InvalidAccount_ReturnsBadRequestObjectResult()
	{
		// Arrange
		const int accountId = 1;
		Account account = new() { AccountId = accountId, AccountName = $"Account {accountId}" };

		_validator.Validate(account).Returns(new ValidationResult(new List<ValidationFailure>
		{
			new("Test Failure", "Test Validation Failure")
		}));

		// Act
		ActionResult<Account> result = _controller.CreateAccount(account);
		BadRequestObjectResult? badRequestObjectResult = result.Result as BadRequestObjectResult;
		string? validationErrorsString = badRequestObjectResult?.Value as string;

		// Assert
		Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
		Assert.That(validationErrorsString, Does.Contain("Test Validation Failure"));
	}

	[Test]
	public void CreateAccount_DuplicateAccount_ReturnsConflictObjectResult()
	{
		// Arrange
		const int duplicateId = 1;
		Account account = new() { AccountId = duplicateId, AccountName = $"Account {duplicateId}" };
		string exceptionMessage = $"Account With ID: {duplicateId} Already Exists";

		_validator.Validate(account).Returns(new ValidationResult());
		_repository.CreateAccount(account).Throws(new ConflictException(exceptionMessage));

		// Act
		ActionResult<Account> result = _controller.CreateAccount(account);
		ConflictObjectResult? conflictObjectResult = result.Result as ConflictObjectResult;

		// Assert
		Assert.That(conflictObjectResult, Is.InstanceOf<ConflictObjectResult>());
		Assert.That(conflictObjectResult, Is.Not.Null);
		Assert.That(conflictObjectResult.Value, Is.EqualTo(exceptionMessage));
	}

	[Test]
	public void UpdateAccount_ValidAccount_ReturnsNoContentResult()
	{
		// Arrange
		const int accountId = 1;
		Account account = new() { AccountId = accountId, AccountName = $"Updated Account {accountId}" };

		_validator.Validate(account).Returns(new ValidationResult());
		_repository.UpdateAccount(accountId, account);

		// Act
		IActionResult result = _controller.UpdateAccount(accountId, account);

		// Assert
		Assert.That(result, Is.InstanceOf<NoContentResult>());
	}

	[Test]
	public void UpdateAccount_InvalidAccount_ReturnsBadRequestObjectResult()
	{
		// Arrange
		const int accountId = 1;
		Account account = new() { AccountId = accountId, AccountName = $"Updated Account {accountId}" };

		_validator.Validate(account).Returns(new ValidationResult(new List<ValidationFailure>
		{
			new("Test Failure", "Test Validation Failure")
		}));

		// Act
		IActionResult result = _controller.UpdateAccount(accountId, account);
		BadRequestObjectResult? badRequestResult = result as BadRequestObjectResult;
		string? validationErrorsString = badRequestResult?.Value as string;

		// Assert
		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		Assert.That(badRequestResult?.Value, Is.InstanceOf<string>());
		Assert.That(validationErrorsString, Does.Contain("Test Validation Failure"));
	}

	[Test]
	public void UpdateAccount_NonMatchingId_ReturnsBadRequestObjectResult()
	{
		// Arrange
		const int accountId = 1;
		const int nonMatchingAccountId = 1;
		Account account = new() { AccountId = accountId, AccountName = $"Updated Account {accountId}" };
		const string exceptionMessage = "Request ID Does Not Match Account ID";

		_validator.Validate(account).Returns(new ValidationResult());
		_repository.When(x => x.UpdateAccount(nonMatchingAccountId, account)).Do(x => { throw new BadRequestException(exceptionMessage); });

		// Act
		IActionResult result = _controller.UpdateAccount(nonMatchingAccountId, account);
		BadRequestObjectResult? badRequestResult = result as BadRequestObjectResult;

		// Assert
		Assert.That(badRequestResult, Is.InstanceOf<BadRequestObjectResult>());
		Assert.That(badRequestResult, Is.Not.Null);
		Assert.That(badRequestResult.Value, Is.EqualTo(exceptionMessage));
	}

	[Test]
	public void UpdateAccount_DuplicateAccount_ReturnsNotFoundObjectResult()
	{
		// Arrange
		const int invalidId = 999;
		Account account = new() { AccountId = invalidId, AccountName = $"Account {invalidId}" };
		string exceptionMessage = $"No Account Found With ID: {invalidId}";

		_validator.Validate(account).Returns(new ValidationResult());
		_repository.When(x => x.UpdateAccount(invalidId, account)).Do(x => { throw new NotFoundException(exceptionMessage); });

		// Act
		IActionResult result = _controller.UpdateAccount(invalidId, account);
		NotFoundObjectResult? notFoundObjectResult = result as NotFoundObjectResult;

		// Assert
		Assert.That(notFoundObjectResult, Is.InstanceOf<NotFoundObjectResult>());
		Assert.That(notFoundObjectResult, Is.Not.Null);
		Assert.That(notFoundObjectResult.Value, Is.EqualTo(exceptionMessage));
	}

	[Test]
	public void DeleteAccount_ValidId_ReturnsNoContentResult()
	{
		// Arrange
		const int accountId = 1;

		_repository.DeleteAccount(accountId);

		// Act
		IActionResult result = _controller.DeleteAccount(accountId);

		// Assert
		Assert.That(result, Is.InstanceOf<NoContentResult>());
	}

	[Test]
	public void DeleteAccount_InvalidId_ReturnsNotFoundObjectResult()
	{
		// Arrange
		const int invalidId = 999;
		string exceptionMessage = $"No Account Found With ID: {invalidId}";

		_repository.When(x => x.DeleteAccount(invalidId)).Do(x => { throw new NotFoundException(exceptionMessage); });

		// Act
		IActionResult result = _controller.DeleteAccount(invalidId);
		NotFoundObjectResult? notFoundObjectResult = result as NotFoundObjectResult;

		// Assert
		Assert.That(notFoundObjectResult, Is.InstanceOf<NotFoundObjectResult>());
		Assert.That(notFoundObjectResult, Is.Not.Null);
		Assert.That(notFoundObjectResult.Value, Is.EqualTo(exceptionMessage));
	}
}
