using FluentValidation;
using FluentValidation.Results;
using LicenseeRecords.Models;
using LicenseeRecords.WebAPI.Helpers;
using LicenseeRecords.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static LicenseeRecords.WebAPI.Exceptions.CustomExceptions;

namespace LicenseeRecords.WebAPI.Controllers;
[Route("[controller]")]
[ApiController]
public class AccountController(IAccountRepository accountRepository, IValidator<Account> validator) : ControllerBase
{
	//GET: account
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public ActionResult<Account[]> GetAccounts()
	{
		return Ok(accountRepository.GetAccounts());
	}

	//GET: account/5
	[HttpGet("{accountId}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public ActionResult<Account> GetAccount(int accountId)
	{
		try
		{
			return Ok(accountRepository.GetAccount(accountId));
		}
		catch (NotFoundException ex)
		{
			return NotFound(ex.Message);
		}
	}

	//POST: account
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	public ActionResult<Account> CreateAccount(Account account)
	{
		try
		{
			ValidationResult result = validator.Validate(account);
			if (!result.IsValid)
			{
				return BadRequest($"Account provided is not valid.\n{ValidationFailureHelper.ConvertErrorArrayToString(result.Errors)}");
			}

			Account createdAccount = accountRepository.CreateAccount(account);
			return CreatedAtAction(nameof(GetAccount), new { accountId = createdAccount.AccountId }, createdAccount);
		}
		catch (ConflictException ex)
		{
			return Conflict(ex.Message);
		}
	}

	//PUT: account/5
	[HttpPut("{accountId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult UpdateAccount(int accountId, Account account)
	{
		try
		{
			ValidationResult result = validator.Validate(account);
			if (!result.IsValid)
			{
				return BadRequest($"Account provided is not valid.\n{ValidationFailureHelper.ConvertErrorArrayToString(result.Errors)}");
			}

			accountRepository.UpdateAccount(accountId, account);
			return NoContent();
		}
		catch (BadRequestException ex)
		{
			return BadRequest(ex.Message);
		}
		catch (NotFoundException ex)
		{
			return NotFound(ex.Message);
		}
	}

	//DELETE: account/5
	[HttpDelete("{accountId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult DeleteAccount(int accountId)
	{
		try
		{
			accountRepository.DeleteAccount(accountId);
			return NoContent();
		}
		catch (NotFoundException ex)
		{
			return NotFound(ex.Message);
		}
	}
}