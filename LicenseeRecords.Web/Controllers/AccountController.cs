using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using LicenseeRecords.Models;
using LicenseeRecords.Web.Models;
using LicenseeRecords.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LicenseeRecords.Web.Controllers;

public class AccountController(IAccountDataService accountDataService, IValidator<Account> validator) : Controller
{
	[HttpGet]
	public IActionResult Create()
	{
		return View();
	}

	[HttpGet]
	public async Task<IActionResult> Edit(int id)
	{
		(Account? account, string? errorMessage) = await accountDataService.GetAccount(id);

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
		}

		return View(account);
	}

	[HttpGet]
	public async Task<IActionResult> View(int id)
	{
		string? errorMessage;

		(Account? account, errorMessage) = await accountDataService.GetAccount(id);

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
		}

		return View(account);
	}

	[HttpPost]
	public async Task<IActionResult> Create(Account account)
	{
		string? errorMessage;
		string? successMessage;

		ValidationResult validationResult = await validator.ValidateAsync(account);
		if (!validationResult.IsValid)
		{
			validationResult.AddToModelState(ModelState, null);
			return View(account);
		}

		(Account? createdAccount, successMessage, errorMessage) = await accountDataService.CreateAccount(account);

		if (successMessage is not null)
		{
			AddSuccessMessageToTempData(successMessage);
		}

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
		}

		return RedirectToAction("index", "home");
	}

	[HttpPost]
	public async Task<IActionResult> Edit(Account account)
	{
		string? errorMessage;
		string? successMessage;

		ValidationResult validationResult = await validator.ValidateAsync(account);
		if (!validationResult.IsValid)
		{
			validationResult.AddToModelState(ModelState, null);
			return View(account);
		}

		#region Assign Product Licences to the Updated Account
		(Account? oldAccount, errorMessage) = await accountDataService.GetAccount(account.AccountId);

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
		}

		if (oldAccount is null)
		{
			return RedirectToAction("index", "home");
		}

		account.ProductLicence = oldAccount.ProductLicence;
		#endregion

		(successMessage, errorMessage) = await accountDataService.UpdateAccount(account.AccountId, account);
		
		if (successMessage is not null)
		{
			AddSuccessMessageToTempData(successMessage);
		}

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
		}

		return RedirectToAction("index", "home");
	}

	[HttpPost]
	public async Task<IActionResult> DeleteProductLicence(int accountId, int productLicenceId)
	{
		string? errorMessage;
		string? successMessage;

		#region Remove Product Licence From Account To Then Update The Entire Account
		(Account? account, errorMessage) = await accountDataService.GetAccount(accountId);

		if (account is null)
		{
			AddErrorMessageToTempData("Something went wrong.");
			return RedirectToAction("index", "home");
		}

		ProductLicence? productLicence = account.ProductLicence.Find(pl => pl.LicenceId == productLicenceId);

		if (productLicence is null)
		{
			AddErrorMessageToTempData("Something went wrong.");
			return RedirectToAction("index", "home");
		}

		account.ProductLicence.Remove(productLicence);
		#endregion

		(successMessage, errorMessage) = await accountDataService.UpdateAccount(accountId, account);

		if (successMessage is not null)
		{
			AddSuccessMessageToTempData(successMessage);
		}

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
		}

		return RedirectToAction("view", "account", new { id = accountId });
	}


	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}

	private void AddErrorMessageToTempData(string? errorMessage)
	{
		TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
	}

	private void AddSuccessMessageToTempData(string? successMessage)
	{
		TempData["SuccessMessages"] = TempData.TryGetValue("SuccessMessages", out object? value) ? value + ";" + successMessage : successMessage;
	}
}
