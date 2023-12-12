using EventManagementService.Domain.Models.Events;
using RecommendationService.API.Controllers.V1.Recommendation.Dtos;
using RecommendationService.Domain.Util;

namespace RecommendationService.API.Controllers.V1.Recommendation.Mappers;

internal static class EventMapper
{
    internal static ReadEventDto FromEventToDto(Event eEvent)
    {
        return new ReadEventDto
        {
            Id = eEvent.Id,
            Title = eEvent.Title,
            StartDate = eEvent.StartDate,
            LastUpdateDate = eEvent.LastUpdateDate,
            EndDate = eEvent.EndDate,
            CreatedDate = eEvent.CreatedDate,
            Host = new ReadUserDto
            {
                UserId = eEvent.Host.UserId,
                LastSeenOnline = eEvent.Host.LastSeenOnline,
                DisplayName = eEvent.Host.DisplayName,
                PhotoUrl = eEvent.Host.PhotoUrl,
                CreationDate = eEvent.Host.CreationDate,
            },
            IsPaid = eEvent.IsPaid,
            Attendees = eEvent.Attendees.Select(user => new ReadUserDto
            {
                UserId = user.UserId,
                DisplayName = user.DisplayName,
                PhotoUrl = user.PhotoUrl,
                CreationDate = user.CreationDate,
                LastSeenOnline = user.LastSeenOnline
            }).ToList(),
            Description = eEvent.Description,
            Category = eEvent.Category.GetDescription(),
            Keywords = eEvent.Keywords.Select(kw => kw.GetDescription()),
            AdultsOnly = eEvent.AdultsOnly,
            IsPrivate = eEvent.IsPrivate,
            MaxNumberOfAttendees = eEvent.MaxNumberOfAttendees,
            Location = eEvent.Location,
            GeoLocation = new ReadGeolocationDto
            {
                Lat = eEvent.GeoLocation.Lat,
                Lng = eEvent.GeoLocation.Lng
            },
            Images = eEvent.Images,
            City = eEvent.City
        };
    }
}