using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using LicenseeRecords.Models;
using LicenseeRecords.Web.Models;
using LicenseeRecords.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LicenseeRecords.Web.Controllers;

public class ProductController(IProductDataService productDataService, IValidator<Product> validator) : Controller
{
	[HttpGet]
	public IActionResult Create()
	{
		return View();
	}

	[HttpGet]
	public async Task<IActionResult> Edit(int id)
	{
		string? errorMessage;

		(Product? product, errorMessage) = await productDataService.GetProduct(id);

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
		}

		return View(product);
	}

	[HttpPost]
	public async Task<IActionResult> Create(Product product)
	{
		string? errorMessage;
		string? successMessage;

		ValidationResult validationResult = await validator.ValidateAsync(product);
		if (!validationResult.IsValid)
		{
			validationResult.AddToModelState(ModelState, null);
			return View(product);
		}

		(Product? createdProduct, successMessage, errorMessage) = await productDataService.CreateProduct(product);

		if (successMessage is not null)
		{
			AddSuccessMessageToTempData(successMessage);
		}

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
		}

		string? url = Url.Action("index", "home");

		return Redirect(url + "#products");
	}

	[HttpPost]
	public async Task<IActionResult> Edit(Product product)
	{
		string? errorMessage;
		string? successMessage;

		ValidationResult validationResult = await validator.ValidateAsync(product);
		if (!validationResult.IsValid)
		{
			validationResult.AddToModelState(ModelState, null);
			return View(product);
		}

		(successMessage, errorMessage) = await productDataService.UpdateProduct(product.ProductId, product);

		if (successMessage is not null)
		{
			AddSuccessMessageToTempData(successMessage);
		}

		if (errorMessage is not null)
		{
			AddErrorMessageToTempData(errorMessage);
		}

		string? url = Url.Action("index", "home");

		return Redirect(url + "#products");
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
