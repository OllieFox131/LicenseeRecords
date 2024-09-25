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
		string? errorMessage;

		#region Collect Products to Populate Dropdown
		(Product[]? products, errorMessage) = await productDataService.GetProducts();

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
			return RedirectToAction("view", "account", new { id = accountId });
		}

		if (products is null)
		{
			AddErrorMessageToTempData("Something went wrong.");
			return RedirectToAction("view", "account", new { id = accountId });
		}

		productLicenceCreateEditModel.Products = products;
		#endregion

		#region Assign AccountId To Create Back Button
		productLicenceCreateEditModel.AccountId = accountId;
		#endregion

		return View(productLicenceCreateEditModel);
	}

	[Route("account/{accountId}/productlicence/{productLicenceId}/edit")]
	public async Task<IActionResult> Edit(int accountId, int productLicenceId)
	{
		ProductLicence? productLicence;
		ProductLicenceCreateEditModel productLicenceCreateEditModel = new();
		string? errorMessage;

		#region Collect Account to extract Product Licence
		(Account? account, errorMessage) = await accountDataService.GetAccount(accountId);

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
			return RedirectToAction("view", "account", new { id = accountId });
		}

		if (account is null)
		{
			AddErrorMessageToTempData("Something went wrong.");
			return RedirectToAction("view", "account", new { id = accountId });
		}

		productLicence = account.ProductLicence.FirstOrDefault(pl => pl.LicenceId == productLicenceId);

		if (productLicence is null)
		{
			AddErrorMessageToTempData("Something went wrong.");
			return RedirectToAction("view", "account", new { id = accountId });
		}

		productLicenceCreateEditModel.ProductLicence = productLicence;
		#endregion

		#region Collect Products to Populate Dropdown
		(Product[]? products, errorMessage) = await productDataService.GetProducts();

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
			return RedirectToAction("view", "account", new { id = accountId });
		}

		if (products is null)
		{
			AddErrorMessageToTempData("Something went wrong.");
			return RedirectToAction("view", "account", new { id = accountId });
		}

		productLicenceCreateEditModel.Products = products;
		#endregion

		#region Assign AccountId To Create Back Button
		productLicenceCreateEditModel.AccountId = accountId;
		#endregion

		return View(productLicenceCreateEditModel);
	}

	[Route("account/{accountId}/productlicence/create")]
	[HttpPost]
	public async Task<IActionResult> Create(int accountId, ProductLicenceCreateEditModel productLicenceCreateEditModel)
	{
		string? errorMessage;
		string? successMessage;

		#region Collect Products to Manually Assign Product Name
		(Product[]? products, errorMessage) = await productDataService.GetProducts();

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
			return RedirectToAction("view", "account", new { id = accountId });
		}

		if (products is null)
		{
			AddErrorMessageToTempData("Something went wrong.");
			return RedirectToAction("view", "account", new { id = accountId });
		}

		Product? matchingProduct = products.FirstOrDefault(p => p.ProductId == productLicenceCreateEditModel.ProductLicence.Product.ProductId);

		if (matchingProduct is not null)
		{
			productLicenceCreateEditModel.ProductLicence.Product.ProductName = matchingProduct.ProductName;
		}
		#endregion

		#region Assign Products to Populate Dropdown
		productLicenceCreateEditModel.Products = products;
		#endregion

		#region Assign AccountId To Create Back Button
		productLicenceCreateEditModel.AccountId = accountId;
		#endregion

		ValidationResult validationResult = await validator.ValidateAsync(productLicenceCreateEditModel.ProductLicence);
		if (!validationResult.IsValid)
		{
			validationResult.AddToModelState(ModelState, null);
			return View(productLicenceCreateEditModel);
		}

		#region Collect Account to Add Product Licence
		(Account? account, errorMessage) = await accountDataService.GetAccount(accountId);

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
			return RedirectToAction("view", "account", new { id = accountId });
		}

		if (account is null)
		{
			AddErrorMessageToTempData("Something went wrong.");
			return RedirectToAction("view", "account", new { id = accountId });
		}

		account.ProductLicence.Add(productLicenceCreateEditModel.ProductLicence);
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

		return RedirectToAction("view", "account", new { id = accountId });
	}

	[Route("account/{accountId}/productlicence/{productLicenceId}/edit")]
	[HttpPost]
	public async Task<IActionResult> Edit(int accountId, int productLicenceId, ProductLicenceCreateEditModel productLicenceCreateEditModel)
	{
		string? errorMessage;
		string? successMessage;

		#region Collect Products to Manually Assign Product Name
		(Product[]? products, errorMessage) = await productDataService.GetProducts();

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
			return RedirectToAction("view", "account", new { id = accountId });
		}

		if (products is null)
		{
			AddErrorMessageToTempData("Something went wrong.");
			return RedirectToAction("view", "account", new { id = accountId });
		}

		Product? matchingProduct = products.FirstOrDefault(p => p.ProductId == productLicenceCreateEditModel.ProductLicence.Product.ProductId);

		if (matchingProduct is not null)
		{
			productLicenceCreateEditModel.ProductLicence.Product.ProductName = matchingProduct.ProductName;
		}
		#endregion

		#region Assign Products to Populate Dropdown
		productLicenceCreateEditModel.Products = products;
		#endregion

		#region Assign AccountId To Create Back Button
		productLicenceCreateEditModel.AccountId = accountId;
		#endregion

		ValidationResult validationResult = await validator.ValidateAsync(productLicenceCreateEditModel.ProductLicence);
		if (!validationResult.IsValid)
		{
			validationResult.AddToModelState(ModelState, null);
			return View(productLicenceCreateEditModel);
		}

		#region Collect Account to Edit Product Licence
		(Account? account, errorMessage) = await accountDataService.GetAccount(accountId);

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
			return RedirectToAction("view", "account", new { id = accountId });
		}

		if (account is null)
		{
			AddErrorMessageToTempData("Something went wrong.");
			return RedirectToAction("view", "account", new { id = accountId });
		}

		ProductLicence? oldProductLicence = account.ProductLicence.Find(pl => pl.LicenceId == productLicenceId);

		if (oldProductLicence is null)
		{
			AddErrorMessageToTempData("Something went wrong.");
			return RedirectToAction("view", "account", new { id = accountId });
		}

		int positionOfOldLicence = account.ProductLicence.IndexOf(oldProductLicence);

		account.ProductLicence.Insert(positionOfOldLicence, productLicenceCreateEditModel.ProductLicence);
		account.ProductLicence.Remove(oldProductLicence);
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
