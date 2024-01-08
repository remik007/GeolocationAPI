using FluentValidation;

namespace Geolocation.Api.DTOs.Validators
{
    public class LocationDtoValidator : AbstractValidator<LocationDto>
    {
        public LocationDtoValidator()
        {
            RuleFor(x => x.Latitude).NotNull();
            RuleFor(x => x.Longitude).NotNull();
        }
    }
}
