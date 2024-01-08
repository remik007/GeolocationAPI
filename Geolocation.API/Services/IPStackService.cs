using Geolocation.Api.DTOs;
using Geolocation.API.Exceptions;
using Newtonsoft.Json;

namespace Geolocation.API.Services
{
    public interface IIPStackService
    {
        Task<LocationDto> GetLocationDetails(string ipAddress);
    }

    public class IPStackService : IIPStackService
    {
        private readonly IPStackSettings _ipStackSettings;
        private readonly ILogger<IPStackService> _logger;
        private readonly HttpClient _httpClient;
        public IPStackService(IPStackSettings ipStackSettings, ILogger<IPStackService> logger, HttpClient httpClient)
        {
            _ipStackSettings = ipStackSettings;
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(_ipStackSettings.url);
            _httpClient.Timeout = new TimeSpan(0, 0, 0, 0, _ipStackSettings.timeoutSeconds);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }
        public async Task<LocationDto> GetLocationDetails(string ipAddress)
        {
            LocationDto locationDto;
            try
            {
                var response = await _httpClient.GetAsync(ipAddress + "?access_key=" + _ipStackSettings.access_key);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                locationDto = JsonConvert.DeserializeObject<LocationDto>(content);
            }
            catch (TaskCanceledException e)
            {
                throw new TimeoutException(e.Message);
            }
            catch
            {
                throw new BadRequestException("IPStack - something went wrong.");
            }
            
            return locationDto;
        }
    }
}
