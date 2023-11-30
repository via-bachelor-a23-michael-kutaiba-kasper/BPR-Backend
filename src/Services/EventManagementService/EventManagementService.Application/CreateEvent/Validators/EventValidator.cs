using EventManagementService.Application.CreateEvent.Exceptions;
using EventManagementService.Domain.Models.Events;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.CreateEvent.Validators;

internal static class EventValidator
{
    internal static void ValidateEvents(Event eEvent)
    {
        if
        (
            string.IsNullOrEmpty(eEvent.Title) ||
            string.IsNullOrWhiteSpace(eEvent.Title)
        )
        {
            throw new EventValidationException("Event is missing Title");
        }

        if
        (
            eEvent.StartDate == DateTimeOffset.MinValue ||
            eEvent.StartDate < DateTimeOffset.UtcNow.AddHours(1)
        )
        {
            throw new EventValidationException("Event start date is either null or is in the past");
        }

        if
        (
            eEvent.EndDate == DateTimeOffset.MinValue ||
            eEvent.EndDate < DateTimeOffset.UtcNow.AddHours(1)
        )
        {
            throw new EventValidationException("Event end date is either null or older than start or current date");
        }

        if
        (
            eEvent.CreatedDate == DateTimeOffset.MinValue ||
            eEvent.CreatedDate < DateTimeOffset.UtcNow.Date.AddHours(1)
        )
        {
            throw new EventValidationException("Event created date is either null or greater than end or start dates");
        }
        
        if
        (
            string.IsNullOrEmpty(eEvent.Location) ||
            string.IsNullOrWhiteSpace(eEvent.Location)
        )
        {
            throw new EventValidationException("Event location is either null or empty");
        }
        if
        (
            string.IsNullOrEmpty(eEvent.City) ||
            string.IsNullOrWhiteSpace(eEvent.City)
        )
        {
            throw new EventValidationException("Event city is either null or empty");
        }

        if (eEvent.GeoLocation.Lat is < -90.0f or > 90.0f)
        {
            throw new EventValidationException("Event geo location latitude is invalid");
        }

        if (eEvent.GeoLocation.Lng is < -180.0f or > 180.0f)
        {
            throw new EventValidationException("Event geo location longitude is invalid");
        }

        if
        (
            eEvent.Keywords.Count() < 3 ||
            eEvent.Keywords.Count() > 5
        )
        {
            throw new EventValidationException("Event keywords are either less or greater that the required");
        }

        if
        (
            string.IsNullOrEmpty(eEvent.AccessCode) ||
            string.IsNullOrWhiteSpace(eEvent.AccessCode)
        )
        {
            throw new EventValidationException("Event access code is either null or empty");
        }
    }

    internal static void ValidateUser(bool userExists)
    {
        if (!userExists)
        {
            throw new EventValidationException("User does not exist");
        }
    }
}