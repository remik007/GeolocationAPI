using Geolocation.Api.Data.Context;
using Geolocation.Api.Data.Entities;
using Geolocation.Api.DTOs;
using Geolocation.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System.Text;

namespace Goeolocation.Tests
{
    public class GeolocationControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private Mock<IIPStackService> _ipStackServiceMock = new Mock<IIPStackService>();
        public GeolocationControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        //remove old database and add InMemoryDatabase
                        var dbContextOptions = services.SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<GeolocationDbContext>));
                        services.Remove(dbContextOptions);
                        services.AddSingleton<IIPStackService>(_ipStackServiceMock.Object);
                        services.AddDbContext<GeolocationDbContext>(options => options.UseInMemoryDatabase("GeolocationDb"));
                    });
                });
            _client = _factory.CreateClient();
        }

        //[Theory]
        //[InlineData("147.161.237.107")]
        //[InlineData("85.221.139.5")]
        [Fact]
        public async Task GetCoordinates_WithQueryParameters_ReturnsOkResult()
        {
            //arrange
            var location = new LocationDto() { Latitude = 10.123, Longitude = 20.567 };
            _ipStackServiceMock
                .Setup(x => x.GetLocationDetails(It.IsAny<string>()))
                .ReturnsAsync(location);
            //act
            var response = await _client.GetAsync($"api/location/GetCoordinates/testIpAddress");

            //assert
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }


        [Fact]
        public async Task GetById_WithQueryParameters_ReturnsNotFoundResult()
        {
            //arrange
            var id = GetNonExistingLocationId();

            //act
            var response = await _client.GetAsync($"api/location/{id}");

            //assert
            Assert.Equal(StatusCodes.Status404NotFound, (int)response.StatusCode);
        }

        [Fact]
        public async Task GetById_WithQueryParameters_ReturnsOkResult()
        {
            //arrange
            var location = new Location() { Id = 1, Latitude = 52.4069200, Longitude = 16.9299300 };
            await SeedLocation(location);

            //act
            var response = await _client.GetAsync($"api/location/{location.Id}");

            //assert
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact]
        public async Task Add_WithQueryParameters_ReturnsOkResult()
        {
            //arrange
            var addLocationDto = new AddLocationDto() { Latitude = 52.4069200, Longitude = 16.9299300 };
            var json = JsonConvert.SerializeObject(addLocationDto);
            var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            //act
            var response = await _client.PostAsync("api/location/add", httpContent);

            //assert
            Assert.Equal(StatusCodes.Status201Created, (int)response.StatusCode);
        }

        [Theory]
        [InlineData(90.123, 50.345)]
        [InlineData(10.123, 180.345)]
        [InlineData(-90.123, 50.345)]
        [InlineData(10.123, -180.345)]
        [InlineData(null, 50.345)]
        [InlineData(10.123, null)]
        public async Task Add_WithQueryParameters_ReturnsBadRequestResult(Double? latitude, Double? longitude)
        {
            //arrange
            var addLocationDto = new AddLocationDto() { Latitude = latitude, Longitude = longitude };
            var json = JsonConvert.SerializeObject(addLocationDto);
            var httpContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");

            //act
            var response = await _client.PostAsync("api/location/add", httpContent);

            //assert
            Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
        }

        [Fact]
        public async Task Delete_WithQueryParameters_ReturnsOkResult()
        {
            //arrange
            var location = new Location() { Id = 2, Latitude = 52.4069200, Longitude = 16.9299300 };
            await SeedLocation(location);

            //act
            var response = await _client.DeleteAsync($"api/location/delete/{location.Id}");

            //assert
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        }

        [Fact]
        public async Task Delete_WithQueryParameters_ReturnsNotFoundResult()
        {
            //arrange
            var id = GetNonExistingLocationId();

            //act
            var response = await _client.DeleteAsync($"api/location/delete/{id}");

            //assert
            Assert.Equal(StatusCodes.Status404NotFound, (int)response.StatusCode);
        }

        private async Task SeedLocation(Location location)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<GeolocationDbContext>();

            await _dbContext.Geolocations.AddAsync(location);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<int> GetNonExistingLocationId()
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<GeolocationDbContext>();

            int id = _dbContext.Geolocations.Any() ? _dbContext.Geolocations.Max(x => x.Id) : 0;
            return id;
        }
    }
}