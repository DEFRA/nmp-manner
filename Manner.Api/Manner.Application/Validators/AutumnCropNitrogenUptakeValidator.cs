using FluentValidation;
using Manner.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Validators
{
    public class AutumnCropNitrogenUptakeValidator : AbstractValidator<AutumnCropNitrogenUptakeRequest>
    {
        public AutumnCropNitrogenUptakeValidator()
        {
            RuleFor(x => x.CropTypeId)
                .GreaterThanOrEqualTo(0).WithMessage("CropTypeId must be greater than or equal to 0")
                .NotNull().WithMessage("CropTypeId is required.");
                //.NotEmpty().WithMessage("CropTypeId is required.")
                //.MaximumLength(3).WithMessage("CropTypeId must not exceed 3 digits.");

            RuleFor(x => x.ApplicationMonth)
                .NotNull().WithMessage("Application Month number is required.")
                .InclusiveBetween(1, 12).WithMessage("Month number must be beween 1 to 12.");
            //.GreaterThan(0).WithMessage("Month number must be greater than zero.")
            //.LessThan(13).WithMessage("Month number must be less than 12.")

        }
    }
}
