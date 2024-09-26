using FluentValidation.TestHelper;
using LicenseeRecords.Models;

namespace LicenseeRecords.Tests.Validator;

[TestFixture]
public class AccountValidatorTest
{
	private AccountValidator validator;

	[SetUp]
	public void Setup()
	{
		validator = new AccountValidator();
	}

	[Test]
	public void EmptyAccount_ShouldHaveErrors()
	{
		Account account = new();
		TestValidationResult<Account> result = validator.TestValidate(account);
		result.ShouldHaveValidationErrorFor(a => a.AccountName);
		result.ShouldHaveValidationErrorFor(a => a.AccountStatus);
	}
}