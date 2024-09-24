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
		(Product? product, string? errorMessage) = await productDataService.GetProduct(id);

		return View(product);
	}

	[HttpPost]
	public async Task<IActionResult> Create(Product product)
	{
		ValidationResult validationResult = await validator.ValidateAsync(product);
		if (!validationResult.IsValid)
		{
			validationResult.AddToModelState(ModelState, null);
			return View(product);
		}

		(Product? createdProduct, string? successMessage, string? errorMessage) = await productDataService.CreateProduct(product);

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
	public async Task<IActionResult> Edit(Product product)
	{
		ValidationResult validationResult = await validator.ValidateAsync(product);
		if (!validationResult.IsValid)
		{
			validationResult.AddToModelState(ModelState, null);
			return View(product);
		}

		(string? successMessage, string? errorMessage) = await productDataService.UpdateProduct(product.ProductId, product);

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


	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
