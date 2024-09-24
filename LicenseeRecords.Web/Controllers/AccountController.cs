using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using LicenseeRecords.Models;
using LicenseeRecords.Web.Models;
using LicenseeRecords.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LicenseeRecords.Web.Controllers;

public class AccountController(ILogger<AccountController> logger, IAccountDataService accountDataService, IValidator<Account> validator) : Controller
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
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		return View(account);
	}

	[HttpGet]
	public async Task<IActionResult> View(int id)
	{
		(Account? account, string? errorMessage) = await accountDataService.GetAccount(id);

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		return View(account);
	}

	[HttpPost]
	public async Task<IActionResult> Create(Account account)
	{
		ValidationResult validationResult = await validator.ValidateAsync(account);
		if (!validationResult.IsValid)
		{
			validationResult.AddToModelState(ModelState, null);
			return View(account);
		}

		(Account? createdAccount, string? successMessage, string? errorMessage) = await accountDataService.CreateAccount(account);

		if (successMessage is not null)
		{
			TempData["SuccessMessages"] = TempData.TryGetValue("SuccessMessages", out object? value) ? value + ";" + successMessage : successMessage;
		}

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		return RedirectToAction("Index", "Home");
	}

	[HttpPost]
	public async Task<IActionResult> Edit(Account account)
	{
		ValidationResult validationResult = await validator.ValidateAsync(account);
		if (!validationResult.IsValid)
		{
			validationResult.AddToModelState(ModelState, null);
			return View(account);
		}

		(string? successMessage, string? errorMessage) = await accountDataService.UpdateAccount(account.AccountId, account);

		if (successMessage is not null)
		{
			TempData["SuccessMessages"] = TempData.TryGetValue("SuccessMessages", out object? value) ? value + ";" + successMessage : successMessage;
		}

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		return RedirectToAction("Index", "Home");
	}

	[HttpPost]
	public async Task<IActionResult> DeleteProductLicence(int accountId, int productLicenceId)
	{
		string? errorMessage;

		(Account? account, errorMessage) = await accountDataService.GetAccount(accountId);

		if (account is null)
		{
			errorMessage = "Something went wrong.";

			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;

			return RedirectToAction("Index", "Home");
		}

		ProductLicence? productLicence = account.ProductLicence.Find(pl => pl.LicenceId == productLicenceId);

		if (productLicence is null)
		{
			errorMessage = "Something went wrong.";

			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;

			return RedirectToAction("Index", "Home");
		}

		account.ProductLicence.Remove(productLicence);

		(string? successMessage, errorMessage) = await accountDataService.UpdateAccount(accountId, account);

		if (successMessage is not null)
		{
			TempData["SuccessMessages"] = TempData.TryGetValue("SuccessMessages", out object? value) ? value + ";" + successMessage : successMessage;
		}

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		return RedirectToAction("View", "Account", new { id = accountId });
	}


	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
