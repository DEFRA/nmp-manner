using FluentValidation;
using Manner.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Validators
{
    public class AutumnCropNitrogenUptakeValodator : AbstractValidator<AutumnCropNitrogenUptakeRequest>
    {
        public AutumnCropNitrogenUptakeValodator()
        {
            RuleFor(x => x.CropTypeId)
                .GreaterThanOrEqualTo(0).WithMessage("CropTypeId must be greater than or equal to 0")
                .NotNull().WithMessage("CropTypeId is required.");
            RuleFor(x => x.ApplicationMonth)
                .NotNull().WithMessage("Application Month number is required.")
                .InclusiveBetween(1, 12).WithMessage("Month number must be beween 1 to 12.");
        }
    }
}
