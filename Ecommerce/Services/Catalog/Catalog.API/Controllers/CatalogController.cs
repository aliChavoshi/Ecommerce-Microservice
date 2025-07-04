﻿using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Common.Logging.Correlations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiVersion("1.0")]
public class CatalogController : ApiController
{
    private readonly IMediator _mediator;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(IMediator mediator, ILogger<CatalogController> logger,
        ICorrelationIdGenerator correlation)
    {
        _mediator = mediator;
        _logger = logger;
        _logger.LogInformation("CorrelationId {correlationId}", correlation.Get());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetProductById(string id, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetProductByIdQuery(id), cancellationToken));
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProductsByName(string name,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetProductsByName called {ProductName}", name);
        return Ok(await _mediator.Send(new GetProductsByNameQuery(name), cancellationToken));
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAllProducts(
        [FromQuery] GetAllProductsQuery request)
    {
        return Ok(await _mediator.Send(request));
    }

    [HttpGet]
    public async Task<ActionResult<List<BrandResponse>>> GetAllBrands()
    {
        return Ok(await _mediator.Send(new GetAllBrandsQuery()));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TypeResponse>>> GetAllTypes()
    {
        return Ok(await _mediator.Send(new GetAllTypesQuery()));
    }

    [HttpGet("{brand}")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProductsByBrandName(string brand,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetProductsByBrandQuery(brand), cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [HttpPut]
    public async Task<ActionResult<bool>> UpdateProduct([FromBody] UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteProduct(string id, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new DeleteProductCommand(id), cancellationToken));
    }

    [HttpGet]
    public IActionResult Test()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault();
        Console.WriteLine("Authorization Header: " + authHeader);
        return Ok(); // موقت برای بررسی
    }
}