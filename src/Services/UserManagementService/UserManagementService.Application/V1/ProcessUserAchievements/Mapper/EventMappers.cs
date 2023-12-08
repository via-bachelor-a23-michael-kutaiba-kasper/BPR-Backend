using UserManagementService.Application.V1.ProcessUserAchievements.Dto;
using UserManagementService.Domain.Models;
using UserManagementService.Domain.Models.Events;
using UserManagementService.Infrastructure.Util;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Mapper;

internal static class EventMappers
{
    internal static IReadOnlyCollection<Event> FromDtoToDomainEventMapper(IReadOnlyCollection<EventDto> eventDtos)
    {
        return eventDtos.Select(e => new Event
            {
                Keywords = e.Keywords.Select(EnumExtensions.GetEnumValueFromDescription<Keyword>),
                Category = EnumExtensions.GetEnumValueFromDescription<Category>(e.Category),
                GeoLocation = new GeoLocation() { Lat = e.GeoLocation.Lat, Lng = e.GeoLocation.Lng },
                Description = e.Description,
                Attendees = e.Attendees.Select(a => new User()
                {
                    CreationDate = a.CreationDate,
                    DisplayName = a.DisplayName,
                    PhotoUrl = a.PhotoUrl,
                    UserId = a.UserId,
                    LastSeenOnline = a.LastSeenOnline
                }),
                City = e.City,
                Host = new User()
                {
                    CreationDate = e.Host.CreationDate,
                    DisplayName = e.Host.DisplayName,
                    PhotoUrl = e.Host.PhotoUrl,
                    UserId = e.Host.UserId,
                    LastSeenOnline = e.Host.LastSeenOnline
                },
                Id = e.Id,
                Images = e.Images,
                Location = e.Location,
                Title = e.Title,
                Url = e.Url,
                AccessCode = e.AccessCode,
                AdultsOnly = e.AdultsOnly,
                CreatedDate = e.CreatedDate,
                EndDate = e.EndDate,
                IsPaid = e.IsPaid,
                IsPrivate = e.IsPrivate,
                StartDate = e.StartDate,
                LastUpdateDate = e.LastUpdateDate,
                MaxNumberOfAttendees = e.MaxNumberOfAttendees
            })
            .ToList();
    }
}