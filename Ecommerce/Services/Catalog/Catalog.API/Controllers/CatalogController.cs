using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

public class CatalogController(IMediator mediator) : ApiController
{
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetProductById(string id, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetProductByIdQuery(id), cancellationToken));
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProductsByName(string name,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetProductsByNameQuery(name), cancellationToken));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAllProducts()
    {
        return Ok(await mediator.Send(new GetAllProductsQuery()));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandResponse>>> GetAllBrands()
    {
        return Ok(await mediator.Send(new GetAllProductsQuery()));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TypeResponse>>> GetAllTypes()
    {
        return Ok(await mediator.Send(new GetAllProductsQuery()));
    }

    [HttpGet("{brand}")]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProductsByBrandName(string brand,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetProductsByBrandQuery(brand), cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(command, cancellationToken));
    }

    [HttpPut]
    public async Task<ActionResult<bool>> UpdateProduct([FromBody] UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(command, cancellationToken));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteProduct(string id, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new DeleteProductCommand(id), cancellationToken));
    }
}