using FluentValidation;
using FluentValidation.Results;
using LicenseeRecords.Models;
using LicenseeRecords.WebAPI.Helpers;
using LicenseeRecords.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static LicenseeRecords.WebAPI.Exceptions.CustomExceptions;

namespace LicenseeRecords.WebAPI.Controllers;
[Route("[controller]")]
[ApiController]
public class ProductController(IProductRepository productRepository, IValidator<Product> validator) : ControllerBase
{
	//GET: product
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public ActionResult<Product[]> GetProducts()
	{
		return Ok(productRepository.GetProducts());
	}

	//GET: product/5
	[HttpGet("{productId}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public ActionResult<Product> GetProduct(int productId)
	{
		try
		{
			return Ok(productRepository.GetProduct(productId));
		}
		catch (NotFoundException ex)
		{
			return NotFound(ex.Message);
		}
	}

	//POST: product
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	public ActionResult<Product> CreateProduct(Product product)
	{
		try
		{
			ValidationResult result = validator.Validate(product);
			if (!result.IsValid)
			{
				return BadRequest($"Product provided is not valid.\n{ValidationFailureHelper.ConvertErrorArrayToString(result.Errors)}");
			}

			Product createdProduct = productRepository.CreateProduct(product);
			return CreatedAtAction(nameof(GetProduct), new { productId = createdProduct.ProductId }, createdProduct);
		}
		catch (ConflictException ex)
		{
			return Conflict(ex.Message);
		}
	}

	//PUT: product/5
	[HttpPut("{productId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult UpdateProduct(int productId, Product product)
	{
		try
		{
			ValidationResult result = validator.Validate(product);
			if (!result.IsValid)
			{
				return BadRequest($"Product provided is not valid.\n{ValidationFailureHelper.ConvertErrorArrayToString(result.Errors)}");
			}

			productRepository.UpdateProduct(productId, product);
			return NoContent();
		}
		catch (BadRequestException ex)
		{
			return BadRequest(ex.Message);
		}
		catch (NotFoundException ex)
		{
			return NotFound(ex.Message);
		}
	}

	//DELETE: product/5
	[HttpDelete("{productId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult DeleteProduct(int productId)
	{
		try
		{
			productRepository.DeleteProduct(productId);
			return NoContent();
		}
		catch (NotFoundException ex)
		{
			return NotFound(ex.Message);
		}
	}
}