using System.Net;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecommendationService.API.Controllers.V1.InterestSurvey.Dtos;
using RecommendationService.Application.V1.GetInterestSurvey;
using RecommendationService.Application.V1.StoreInterestSurveyResult;
using RecommendationService.Application.V1.StoreInterestSurveyResult.Exceptions;
using RecommendationService.Domain.Events;
using RecommendationService.Domain.Util;
using UserNotFoundException = RecommendationService.Application.V1.GetRecommendations.Exceptions.UserNotFoundException;

namespace RecommendationService.API.Controllers.V1.InterestSurvey;

[ApiController]
[Route("api/v1/interestSurvey")]
public class InterestSurveyController : ControllerBase
{
    private readonly ILogger<InterestSurveyController> _logger;
    private readonly IMediator _mediator;

    public InterestSurveyController(ILogger<InterestSurveyController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ReadInterestSurveyDto>> GetInterestSurvey([FromQuery] string userId)
    {
        try
        {
            var possiblyFilledOutSurvey = await _mediator.Send(new GetInterestSurveyRequest(userId));
            if (possiblyFilledOutSurvey is null)
            {
                return Ok(null);
            }

            var dto = new ReadInterestSurveyDto
            {
                User = new ReadUserDto
                {
                    UserId = possiblyFilledOutSurvey.User.UserId,
                    CreationDate = possiblyFilledOutSurvey.User.CreationDate,
                    DisplayName = possiblyFilledOutSurvey.User.DisplayName,
                    PhotoUrl = possiblyFilledOutSurvey.User.PhotoUrl,
                    LastSeenOnline = possiblyFilledOutSurvey.User.LastSeenOnline
                },
                Categories = possiblyFilledOutSurvey.Categories.Select(category => category.GetDescription())
                    .ToList(),
                Keywords = possiblyFilledOutSurvey.Keywords.Select(keyword => keyword.GetDescription()).ToList()
            };

            return Ok(dto);
        }
        catch (Exception e) when (e is Application.V1.GetInterestSurvey.Exceptions.UserNotFoundException)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to retrieve interest survey for: {userId}");
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ReadInterestSurveyDto>> StoreInterestSurvey(
        [FromBody] StoreInterestSurveyDto request)
    {
        try
        {
            var domainSurvey = new Domain.Events.InterestSurvey
            {
                User = new User {UserId = request.UserId},
                Categories = request.Categories.Select(EnumExtensions.GetEnumValueFromDescription<Category>).ToList(),
                Keywords = request.Keywords.Select(EnumExtensions.GetEnumValueFromDescription<Keyword>).ToList()
            };

            var storedSurvey = await _mediator.Send(new StoreInterestSurveyRequest(request.UserId, domainSurvey));

            return Ok(storedSurvey);
        }
        catch (Exception e) when (e is UserNotFoundException)
        {
            return NotFound(e.Message);
        }
        catch (Exception e) when (e is InterestSurveyAlreadyCompletedException)
        {
            return Conflict(e.Message);
        }
        catch (Exception e) when (e is InterestSurveyValidationError)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to store interest survey for user: {request.UserId}");
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }
    }
}