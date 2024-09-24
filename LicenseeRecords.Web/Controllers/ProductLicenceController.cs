using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using LicenseeRecords.Models;
using LicenseeRecords.Web.Models;
using LicenseeRecords.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LicenseeRecords.Web.Controllers;

public class ProductLicenceController(IAccountDataService accountDataService, IValidator<ProductLicence> validator) : Controller
{
	[Route("account/{accountId}/productlicence/create")]
	public IActionResult Create(int accountId)
	{
		return View();
	}

	[Route("account/{accountId}/productlicence/{productLicenceId}/edit")]
	public async Task<IActionResult> Edit(int accountId, int productLicenceId)
	{
		string? errorMessage;
		ProductLicence? productLicence;

		(Account? account, errorMessage) = await accountDataService.GetAccount(accountId);

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		if (account is null)
		{
			errorMessage = "Something went wrong.";

			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;

			return RedirectToAction("view", "account", new { id = accountId });
		}

		productLicence = account.ProductLicence.FirstOrDefault(pl => pl.LicenceId == productLicenceId);

		if (productLicence is null)
		{
			errorMessage = "Something went wrong.";

			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;

			return RedirectToAction("view", "account", new { id = accountId });
		}

		return View(productLicence);
	}

	[Route("account/{accountId}/productlicence/create")]
	[HttpPost]
	public async Task<IActionResult> Create(int accountId, ProductLicence productLicence)
	{
		ValidationResult validationResult = await validator.ValidateAsync(productLicence);
		if (!validationResult.IsValid)
		{
			validationResult.AddToModelState(ModelState, null);
			return View(productLicence);
		}

		string? errorMessage;
		string? successMessage;

		(Account? account, errorMessage) = await accountDataService.GetAccount(accountId);

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		if (account is null)
		{
			errorMessage = "Something went wrong.";

			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;

			return RedirectToAction("view", "account", new { id = accountId });
		}

		account.ProductLicence.Add(productLicence);

		(successMessage, errorMessage) = await accountDataService.UpdateAccount(account.AccountId, account);

		if (successMessage is not null)
		{
			TempData["SuccessMessages"] = TempData.TryGetValue("SuccessMessages", out object? value) ? value + ";" + successMessage : successMessage;
		}

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		return RedirectToAction("view", "account", new { id = accountId });
	}

	[Route("account/{accountId}/productlicence/{productLicenceId}/edit")]
	[HttpPost]
	public async Task<IActionResult> Edit(int accountId, int productLicenceId, ProductLicence productLicence)
	{
		ValidationResult validationResult = await validator.ValidateAsync(productLicence);
		if (!validationResult.IsValid)
		{
			validationResult.AddToModelState(ModelState, null);
			return View(productLicence);
		}

		string? errorMessage;
		string? successMessage;

		(Account? account, errorMessage) = await accountDataService.GetAccount(accountId);

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		if (account is not null)
		{
			ProductLicence? oldProductLicence = account.ProductLicence.Find(pl => pl.LicenceId == productLicenceId);

			int positionOfOldLicence = account.ProductLicence.IndexOf(oldProductLicence!);

			account.ProductLicence.Insert(positionOfOldLicence, productLicence);
			account.ProductLicence.Remove(oldProductLicence!);

			(successMessage, errorMessage) = await accountDataService.UpdateAccount(account.AccountId, account);

			if (successMessage is not null)
			{
				TempData["SuccessMessages"] = TempData.TryGetValue("SuccessMessages", out object? value) ? value + ";" + successMessage : successMessage;
			}

			if (errorMessage is not null)
			{
				TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
			}
		}

		return RedirectToAction("view", "account", new { id = accountId });
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
