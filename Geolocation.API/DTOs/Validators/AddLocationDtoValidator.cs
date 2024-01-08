using FluentValidation;

namespace Geolocation.Api.DTOs.Validators
{
    public class AddLocationDtoValidator : AbstractValidator<AddLocationDto>
    {
        public AddLocationDtoValidator()
        {
            RuleFor(x => x.Latitude)
                .NotNull()
                .LessThanOrEqualTo(90)
                .GreaterThanOrEqualTo(-90);

            RuleFor(x => x.Longitude)
                .NotNull()
                .LessThanOrEqualTo(180)
                .GreaterThanOrEqualTo(-180);
        }
    }
}
