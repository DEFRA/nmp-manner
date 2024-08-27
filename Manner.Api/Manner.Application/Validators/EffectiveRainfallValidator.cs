using FluentValidation;
using Manner.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Validators
{
    public class EffectiveRainfallValidator : AbstractValidator<EffectiveRainfallRequest>
    {
        public EffectiveRainfallValidator()
        {
            RuleFor(x => x.CropTypeId)
                .GreaterThanOrEqualTo(0).WithMessage("CropTypeId must be greater than or equal to 0")
                .NotNull().WithMessage("CropTypeId is required.");
            RuleFor(x => x.ApplicationDate)
                .NotNull().WithMessage("Application Month number is required.")
                .LessThan(x=>x.EndOfDrainageDate).WithMessage("Application Date should be less than end of drainage date.");
            RuleFor(x => x.EndOfDrainageDate)
                .NotNull().WithMessage("End of Drainage Date is required.")
                .GreaterThan(x => x.ApplicationDate).WithMessage("End of Drainage Date should be greater than application date.");
            RuleFor(x => x.Postcode)
                .NotNull().WithMessage("Postcode should not be null.")
                .Empty().WithMessage("Postcode should not be empty.")
                .MaximumLength(4).WithMessage("Postcode must not exceed 4 characters.")
                .MinimumLength(3).WithMessage("Postcode should have at least 3 characters.");            
        }
    }
}
