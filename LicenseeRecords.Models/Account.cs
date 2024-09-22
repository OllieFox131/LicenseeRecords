using FluentValidation;

namespace LicenseeRecords.Models;

public class Account
{
	public int AccountId { get; set; }
	public string AccountName { get; set; } = string.Empty;
	public string AccountStatus { get; set; } = string.Empty;
	public List<ProductLicence> ProductLicence { get; set; } = [];
}

public class AccountValidator : AbstractValidator<Account>
{
	public AccountValidator()
	{
		RuleFor(a => a.AccountId).NotNull();
		RuleFor(a => a.AccountName).NotEmpty();
		RuleFor(a => a.AccountStatus).NotEmpty();
	}
}
