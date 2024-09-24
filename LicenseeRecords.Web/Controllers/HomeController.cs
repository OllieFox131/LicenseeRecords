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
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		(homeViewModel.Products, errorMessage) = await productDataService.GetProducts();

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		return View(homeViewModel);
	}

	[HttpPost]
	public async Task<IActionResult> DeleteAccount(int accountId)
	{
		(string? successMessage, string? errorMessage) = await accountDataService.DeleteAccount(accountId);

		if (successMessage is not null)
		{
			TempData["SuccessMessages"] = TempData.TryGetValue("SuccessMessages", out object? value) ? value + ";" + successMessage : successMessage;
		}

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		return RedirectToAction("Index");
	}

	[HttpPost]
	public async Task<IActionResult> DeleteProduct(int productId)
	{
		(string? successMessage, string? errorMessage) = await productDataService.DeleteProduct(productId);

		if (successMessage is not null)
		{
			TempData["SuccessMessages"] = TempData.TryGetValue("SuccessMessages", out object? value) ? value + ";" + successMessage : successMessage;
		}

		if (errorMessage is not null)
		{
			TempData["ErrorMessages"] = TempData.TryGetValue("ErrorMessages", out object? value) ? value + ";" + errorMessage : errorMessage;
		}

		return RedirectToAction("Index");
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
}
