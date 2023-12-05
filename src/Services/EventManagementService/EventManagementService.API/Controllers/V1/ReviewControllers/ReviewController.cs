using System.Net;
using EventManagementService.API.Controllers.V1.ReviewControllers.Dtos;
using EventManagementService.API.Controllers.V1.ReviewControllers.Mappers;
using EventManagementService.Application.V1.ReviewEvent;
using EventManagementService.Application.V1.ReviewEvent.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementService.API.Controllers.V1.ReviewControllers;

[ApiController]
[Route("api/v1/reviews")]
public class ReviewController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReviewController> _logger;

    public ReviewController(IMediator mediator, ILogger<ReviewController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    [HttpPost("createReview")]

    public async Task<ActionResult<CreateReviewResponseDto>> CreateNewReview([FromBody] ReviewDto reviewDto)
    {
        try
        {
            var review = await _mediator.Send(new ReviewEventRequest(ReviewMapper.ProcessIncomingReview(reviewDto)));
            var response = new CreateReviewResponseDto
            {
                Result = ReviewMapper.FromReviewToDto(review),
                status = new StatusCode
                {
                    Code = HttpStatusCode.OK,
                    Message = "Review have been successfully created"
                }
            };

            return Ok(response);
        }
        catch (Exception e) when(e is ReviewAlreadyExistException)
        {
            return StatusCode((int)HttpStatusCode.BadRequest);
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}