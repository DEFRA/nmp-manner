using FluentValidation;
using Manner.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Validators
{
    public class EffectiveRainfallRequestValidator : AbstractValidator<EffectiveRainfallRequest>
    {
        public EffectiveRainfallRequestValidator()
        {
            // Validate EndOfSoilDrainageDate to be between 01/01 and 30/04
            RuleFor(x => x.EndOfSoilDrainageDate)
                .Must(BeWithinValidRange)
                .WithMessage("End of soil drainage date must be between 01/01 and 30/04, but was {PropertyValue:dd/MM/yyyy}");

            // Validate that Postcode is not null or empty
            RuleFor(x => x.Postcode)
                .NotEmpty().WithMessage("Postcode is required.")
                .MinimumLength(3).WithMessage("Postcode must be at least 3 characters long.")
                .MaximumLength(4).WithMessage("Only the first half of the postcode is required. A maximum of 4 characters");
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
