using api.Models;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var products = await _service.GetAllAsync();
        return Ok(products);
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(int id)

    {
        var productDetails = await _service.GetByIdAsync(id);
        return Ok(productDetails);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateProductRequest request)
    {
        return Ok(await _service.CreateAsync(request.Name, request.Description, request.Rate));
    }
}