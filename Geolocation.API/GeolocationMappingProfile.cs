using AutoMapper;
using Geolocation.Api.Data.Entities;
using Geolocation.Api.DTOs;

namespace Geolocation.API
{
    public class GeolocationMappingProfile : Profile
    {
        public GeolocationMappingProfile()
        {
            CreateMap<Location, LocationDto>();
            CreateMap<AddLocationDto, Location>();
        }
    }
}
