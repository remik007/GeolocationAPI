
using Geolocation.API.Data.Entities;

namespace Geolocation.Api.Data.Entities
{
    public class Location : BaseEntity
    {
        public int Id { get; set; }
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
       
    }
}
