using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using RecommendationService.API.Controllers.V1.Recommendation.Dtos;
using RecommendationService.Domain.Events;
using RecommendationService.Domain.Util;

namespace RecommendationService.API.Controllers.V1.Recommendation.Mappers;

internal static class EventMapper
{
    internal static Event ProcessIncomingEvent(ReadEventDto eventDto)
    {
        Category category;
        var keywords = new List<Keyword>();
        try
        {
            category = EnumExtensions.GetEnumValueFromDescription<Category>(eventDto.Category);
        }
        catch (Exception e)
        {
            category = Category.UnAssigned;
        }

        foreach (var ky in eventDto.Keywords)
        {
            try
            {
                keywords.Add(EnumExtensions.GetEnumValueFromDescription<Keyword>(ky));
            }
            catch (Exception e)
            {
                keywords.Add(Keyword.UnAssigned);
            }
        }

        return new Event
        {
            Title = eventDto.Title,
            StartDate = eventDto.StartDate,
            LastUpdateDate = eventDto.LastUpdateDate,
            EndDate = eventDto.EndDate,
            CreatedDate = eventDto.CreatedDate,
            Host = new User
            {
                LastSeenOnline = eventDto.Host.LastSeenOnline,
                DisplayName = eventDto.Host.DisplayName,
                PhotoUrl = eventDto.Host.PhotoUrl,
                UserId = eventDto.Host.UserId,
            },
            Attendees = eventDto.Attendees.Select(user => new User
            {
                UserId = user.UserId,
                DisplayName = user.DisplayName,
                PhotoUrl = user.PhotoUrl,
                CreationDate = user.CreationDate,
                LastSeenOnline = user.LastSeenOnline
            }).ToList(),
            IsPaid = eventDto.IsPaid,
            Description = eventDto.Description,
            Category = category,
            Keywords = keywords,
            AdultsOnly = eventDto.AdultsOnly,
            IsPrivate = eventDto.IsPrivate,
            MaxNumberOfAttendees = eventDto.MaxNumberOfAttendees,
            Location = eventDto.Location,
            GeoLocation = new GeoLocation
            {
                Lng = eventDto.GeoLocation.Lng,
                Lat = eventDto.GeoLocation.Lat
            },
            City = eventDto.City
        };
    }

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

    internal static IReadOnlyCollection<ReadEventDto> FromEventListToDtoList(IReadOnlyCollection<Event> events)
    {
        return events.Select(ev => new ReadEventDto
            {
                Id = ev.Id,
                Title = ev.Title,
                StartDate = ev.StartDate,
                LastUpdateDate = ev.LastUpdateDate,
                EndDate = ev.EndDate,
                CreatedDate = ev.CreatedDate,
                Host = new ReadUserDto
                {
                    UserId = ev.Host.UserId,
                    LastSeenOnline = ev.Host.LastSeenOnline,
                    DisplayName = ev.Host.DisplayName,
                    PhotoUrl = ev.Host.PhotoUrl,
                    CreationDate = ev.Host.CreationDate,
                },
                IsPaid = ev.IsPaid,
                Attendees = ev.Attendees.Select(user => new ReadUserDto
                    {
                        UserId = user.UserId,
                        DisplayName = user.DisplayName,
                        PhotoUrl = user.PhotoUrl,
                        CreationDate = user.CreationDate,
                        LastSeenOnline = user.LastSeenOnline
                    })
                    .ToList(),
                Description = ev.Description,
                Category = ev.Category.GetDescription(),
                Keywords = ev.Keywords.Select(kw => kw.GetDescription()),
                AdultsOnly = ev.AdultsOnly,
                IsPrivate = ev.IsPrivate,
                MaxNumberOfAttendees = ev.MaxNumberOfAttendees,
                Location = ev.Location,
                GeoLocation = new ReadGeolocationDto { Lat = ev.GeoLocation.Lat, Lng = ev.GeoLocation.Lng },
                Images = ev.Images,
                City = ev.City
            })
            .ToList();
    }
}