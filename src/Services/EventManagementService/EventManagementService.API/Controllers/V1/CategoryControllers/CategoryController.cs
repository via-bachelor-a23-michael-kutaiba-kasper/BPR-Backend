using System.Net;
using EventManagementService.Application.V1.FetchCategories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementService.API.Controllers.V1.CategoryControllers;

[ApiController]
[Route("api/v1/categories")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("allCategories")]
    public async Task<ActionResult<string>> GetAlLCategories()
    {
        try
        {
            var categories = await _mediator.Send(new FetchCategoriesRequest());
            return Ok(categories);
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}