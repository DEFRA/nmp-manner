using FluentValidation;
using Manner.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Validators
{
    public class CalculateNutrientsRequestValidator : AbstractValidator<CalculateNutrientsRequest>
    {
        public CalculateNutrientsRequestValidator()
        {
            RuleFor(x => x.Postcode)
                .NotNull().NotEmpty().WithMessage("Postcode is required.")                
                .MinimumLength(3).WithMessage("Postcode must be at least 3 characters long.")
                .MaximumLength(4).WithMessage("Only the first half of the postcode is required. A maximum of 4 characters");
            RuleFor(x => x.CountryID)
               .Must(NotDefaultValue).WithMessage("CountryID not set")
               .NotNull().WithMessage("CountryID is required.");

            RuleFor(x => x.Field.CropTypeID)
               .Must(NotDefaultValue).WithMessage("CropTypeID not set")
               .NotNull().WithMessage("CropTypeID is required.");
            RuleFor(x => x.Field.CropTypeID)
                .Must(NotDefaultValue).WithMessage("CropTypeID not set")
                .NotNull().WithMessage("CropTypeID is required.");
            RuleFor(x => x.Field.TopsoilID)
                .Must(NotDefaultValue).WithMessage("TopsoilID not set")
                .NotNull().WithMessage("TopsoilID is required.");
            RuleFor(x => x.Field.SubsoilID)
                .Must(NotDefaultValue).WithMessage("TopsoilID not set")
                .NotNull().WithMessage("TopsoilID is required.");
            RuleFor(x => x.Field.IsInNVZ)
              .NotNull().WithMessage("IsInNVZ is required.");
            RuleForEach(a => a.ManureApplications).SetValidator(new ManureApplicationValidator());
            
            
        }

       
        private bool NotDefaultValue(int Id)
        {
            // Validates that the date is between 1st Jan and 30th Apr of any year accouting for leap years.
            return Id != default;
        }

        private bool NotDefaultValue(double value)
        {
            // Validates that the date is between 1st Jan and 30th Apr of any year accouting for leap years.
            return value != default;
        }
               

        private bool NotDefaultValue(decimal value)
        {
            // Validates that the date is between 1st Jan and 30th Apr of any year accouting for leap years.
            return value != default;
        }

        
    }
}
