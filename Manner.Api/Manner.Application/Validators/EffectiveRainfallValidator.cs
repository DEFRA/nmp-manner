using FluentValidation;
using Manner.Application.DTOs;

namespace Manner.Application.Validators
{
    public class EffectiveRainfallRequestValidator : AbstractValidator<EffectiveRainfallRequest>
    {
        public EffectiveRainfallRequestValidator()
        {
            // Validate EndSoilDrainageDate to be between 01/01 and 30/04
            RuleFor(x => x.EndSoilDrainageDate)
                .Must(BeWithinValidRange)
                .WithMessage("End of soil drainage date must be between 01/01 and 30/04, but was {PropertyValue:dd/MM/yyyy}");

            // Validate that Postcode is not null or empty
            RuleFor(x => x.Postcode)
                .NotEmpty().WithMessage("Postcode is required.")
                .MinimumLength(3).WithMessage("Postcode must be at least 3 characters long.");
        }

        private bool BeWithinValidRange(DateOnly date)
        {
            // Validates that the date is between 1st Jan and 30th Apr of any year accouting for leap years.
            return (date.Month == 1) ||
                   (date.Month == 2) ||
                   (date.Month == 3) ||
                   (date.Month == 4 && date.Day <= 30);
        }
    }
}
