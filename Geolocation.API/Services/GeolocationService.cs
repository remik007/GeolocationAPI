using AutoMapper;
using Geolocation.API.Exceptions;
using Geolocation.Api.Data.Context;
using Geolocation.Api.Data.Entities;
using Geolocation.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Geolocation.API.Services
{
    public interface IGeolocationService
    {
        Task<int> AddLocation(AddLocationDto addLocationDto);
        Task<int> DeleteLocation(int id);
        Task<LocationDto> GetById(int id);
    }

    public class GeolocationService : IGeolocationService
    {
        private readonly GeolocationDbContext _context;
        private readonly IMapper _mapper;
        public GeolocationService(GeolocationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<int> AddLocation(AddLocationDto addLocationDto)
        {
            var location = _mapper.Map<Location>(addLocationDto);
            await _context.Geolocations.AddAsync(location);
            await _context.SaveChangesAsync();
            return location.Id;
        }

        public async Task<int> DeleteLocation(int id)
        {
            var location = _context.Geolocations.FirstOrDefault(x => x.Id == id);
            if (location == null)
                throw new NotFoundException("Location entry not found");
            _context.Geolocations.Remove(location);
            await _context.SaveChangesAsync();
            return location.Id;
        }

        public async Task<LocationDto> GetById(int id)
        {
            var location = await _context.Geolocations.FirstOrDefaultAsync(x => x.Id == id);
            var locationDto = _mapper.Map<LocationDto>(location);
            if (location == null)
                throw new NotFoundException("Location entry not found");
            await _context.SaveChangesAsync();
            return locationDto;
        }
    }
}
