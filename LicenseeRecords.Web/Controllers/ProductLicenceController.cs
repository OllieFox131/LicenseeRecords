using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using LicenseeRecords.Models;
using LicenseeRecords.Web.Models;
using LicenseeRecords.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LicenseeRecords.Web.Controllers;

public class ProductLicenceController(IAccountDataService accountDataService, IProductDataService productDataService, IValidator<ProductLicence> validator) : Controller
{
	[Route("account/{accountId}/productlicence/create")]
	public async Task<IActionResult> Create(int accountId)
	{
		ProductLicenceCreateEditModel productLicenceCreateEditModel = new();

		(Product[]? products, string? errorMessage) = await productDataService.GetProducts();

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		if (products is null)
		{
			errorMessage = "Something went wrong.";

			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;

			return RedirectToAction("view", "account", new { id = accountId });
		}

		productLicenceCreateEditModel.Products = products;

		return View(productLicenceCreateEditModel);
	}

	[Route("account/{accountId}/productlicence/{productLicenceId}/edit")]
	public async Task<IActionResult> Edit(int accountId, int productLicenceId)
	{
		string? errorMessage;
		ProductLicence? productLicence;
		ProductLicenceCreateEditModel productLicenceCreateEditModel = new();

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

		productLicenceCreateEditModel.ProductLicence = productLicence;

		(Product[]? products, errorMessage) = await productDataService.GetProducts();

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		if (products is null)
		{
			errorMessage = "Something went wrong.";

			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;

			return RedirectToAction("view", "account", new { id = accountId });
		}

		productLicenceCreateEditModel.Products = products;

		return View(productLicenceCreateEditModel);
	}

	[Route("account/{accountId}/productlicence/create")]
	[HttpPost]
	public async Task<IActionResult> Create(int accountId, ProductLicenceCreateEditModel productLicenceCreateEditModel)
	{
		string? errorMessage;
		string? successMessage;

		(Product[]? products, errorMessage) = await productDataService.GetProducts();

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		if (products is null)
		{
			errorMessage = "Something went wrong.";

			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;

			return RedirectToAction("view", "account", new { id = accountId });
		}

		Product? matchingProduct = products.FirstOrDefault(p => p.ProductId == productLicenceCreateEditModel.ProductLicence.Product.ProductId);

		if (matchingProduct is not null)
		{
			productLicenceCreateEditModel.ProductLicence.Product.ProductName = matchingProduct.ProductName;
		}

		productLicenceCreateEditModel.Products = products;

		ValidationResult validationResult = await validator.ValidateAsync(productLicenceCreateEditModel.ProductLicence);
		if (!validationResult.IsValid)
		{
			validationResult.AddToModelState(ModelState, null);
			return View(productLicenceCreateEditModel);
		}

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

		account.ProductLicence.Add(productLicenceCreateEditModel.ProductLicence);

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
	public async Task<IActionResult> Edit(int accountId, int productLicenceId, ProductLicenceCreateEditModel productLicenceCreateEditModel)
	{
		string? errorMessage;
		string? successMessage;

		(Product[]? products, errorMessage) = await productDataService.GetProducts();

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		if (products is null)
		{
			errorMessage = "Something went wrong.";

			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;

			return RedirectToAction("view", "account", new { id = accountId });
		}

		Product? matchingProduct = products.FirstOrDefault(p => p.ProductId == productLicenceCreateEditModel.ProductLicence.Product.ProductId);

		if (matchingProduct is not null)
		{
			productLicenceCreateEditModel.ProductLicence.Product.ProductName = matchingProduct.ProductName;
		}

		productLicenceCreateEditModel.Products = products;

		ValidationResult validationResult = await validator.ValidateAsync(productLicenceCreateEditModel.ProductLicence);
		if (!validationResult.IsValid)
		{
			validationResult.AddToModelState(ModelState, null);
			return View(productLicenceCreateEditModel);
		}

		(Account? account, errorMessage) = await accountDataService.GetAccount(accountId);

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		if (account is not null)
		{
			ProductLicence? oldProductLicence = account.ProductLicence.Find(pl => pl.LicenceId == productLicenceId);

			int positionOfOldLicence = account.ProductLicence.IndexOf(oldProductLicence!);

			account.ProductLicence.Insert(positionOfOldLicence, productLicenceCreateEditModel.ProductLicence);
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
