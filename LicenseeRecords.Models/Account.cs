using FluentValidation;

namespace LicenseeRecords.Models;

public class Account
{
	public int AccountId { get; set; }
	public string? AccountName { get; set; }
	public string? AccountStatus { get; set; }
	public List<ProductLicence> ProductLicence { get; set; } = [];
}

public class AccountValidator : AbstractValidator<Account>
{
	public AccountValidator()
	{
		RuleFor(a => a.AccountId).NotNull().WithName("Account Id");
		RuleFor(a => a.AccountName).NotEmpty().WithName("Account Name");
		RuleFor(a => a.AccountStatus).NotEmpty().WithName("Account Status");
	}
}
