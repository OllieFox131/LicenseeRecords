using LicenseeRecords.Web.Models;
using LicenseeRecords.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LicenseeRecords.Web.Controllers;

public class HomeController(IAccountDataService accountDataService, IProductDataService productDataService) : Controller
{
	public async Task<IActionResult> Index()
	{
		HomeViewModel homeViewModel = new();
		string? errorMessage;

		(homeViewModel.Accounts, errorMessage) = await accountDataService.GetAccounts();

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
		}

		(homeViewModel.Products, errorMessage) = await productDataService.GetProducts();

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
		}

		return View(homeViewModel);
	}

	[HttpPost]
	public async Task<IActionResult> DeleteAccount(int accountId)
	{
		string? errorMessage;
		string? successMessage;

		(successMessage, errorMessage) = await accountDataService.DeleteAccount(accountId);

		if (successMessage is not null)
		{
			AddSuccessMessageToTempData(successMessage);
		}

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
		}

		return RedirectToAction("Index");
	}

	[HttpPost]
	public async Task<IActionResult> DeleteProduct(int productId)
	{
		string? errorMessage;
		string? successMessage;

		(successMessage, errorMessage) = await productDataService.DeleteProduct(productId);

		if (successMessage is not null)
		{
			AddSuccessMessageToTempData(successMessage);
		}

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
		}

		string? url = Url.Action("index");

		return Redirect(url + "#products");
	}

	public IActionResult Privacy()
	{
		return View();
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
