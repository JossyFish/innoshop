using Auth.Core.Data;
using Auth.Core.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands.AddProduct;
using ProductService.Application.Commands.BulkProduct;
using ProductService.Application.Commands.ChangeProduct;
using ProductService.Application.Commands.Delete;
using ProductService.Application.Commands.DeleteAll;
using ProductService.Application.Queries.GetById;
using ProductService.Application.Queries.GetMyProducts;
using ProductService.Application.Queries.GetProducts;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HasPermission(Permission.Create)]
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddProductCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Accepted();
        }

        [HasPermission(Permission.Update)]
        [HttpPost("change")]
        public async Task<IActionResult> Change([FromBody] ChangeProductCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Accepted();
        }

        [HasPermission(Permission.Update)]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(Guid productId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteCommand {ProductId = productId } , cancellationToken);
            return Ok();
        }

        [HasPermission(Permission.Update)]
        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAll(CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteAllCommand(), cancellationToken);
            return Ok();
        }

        [HasPermission(Permission.Update)]
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkProduct([FromBody] BulkProductCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HasPermission(Permission.Read)]
        [HttpGet("my-products")]
        public async Task<IActionResult> GetMyProducts([FromQuery] GetMyProductsQuery query, CancellationToken cancellationToken)
        {
            var products = await _mediator.Send(query, cancellationToken);
            return Ok(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetById (Guid productId, CancellationToken cancellationToken)
        {
            var product = await _mediator.Send(new GetByIdQuery { Id = productId }, cancellationToken);
            return Ok(product);
        }
    }
}
