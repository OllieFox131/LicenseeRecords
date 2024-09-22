using LicenseeRecords.Models;
using LicenseeRecords.Web.Models;
using LicenseeRecords.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LicenseeRecords.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IAccountDataService _accountDataService;
		private readonly IProductDataService _productDataService;

		public HomeController(ILogger<HomeController> logger, IAccountDataService accountDataService, IProductDataService productDataService)
		{
			_logger = logger;
			_accountDataService = accountDataService;
			_productDataService = productDataService;
		}

		public async Task<IActionResult> Index()
		{
			HomeViewModel homeViewModel = new();

			string? errorMessage;

			(homeViewModel.Accounts, errorMessage) = await _accountDataService.GetAccounts();

			if (errorMessage is not null)
			{
				homeViewModel.ErrorMessages.Add(errorMessage);
			}

			(homeViewModel.Products, errorMessage) = await _productDataService.GetProducts();

			if (errorMessage is not null)
			{
				homeViewModel.ErrorMessages.Add(errorMessage);
			}

			if (TempData["SuccessMessages"] is string successMessages)
			{
				homeViewModel.SuccessMessages.AddRange(successMessages.Split(";"));
			}

			if (TempData["ErrorMessages"] is string errorMessages)
			{
				homeViewModel.ErrorMessages.AddRange(errorMessages.Split(';'));
			}

			return View(homeViewModel);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteAccount(int accountId)
		{
			HomeViewModel homeViewModel = new();

			string? successMessage;
			string? errorMessage;

			(successMessage, errorMessage) = await _accountDataService.DeleteAccount(accountId);

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
			HomeViewModel homeViewModel = new();

			string? successMessage;
			string? errorMessage;

			(successMessage, errorMessage) = await _productDataService.DeleteProduct(productId);

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
}
