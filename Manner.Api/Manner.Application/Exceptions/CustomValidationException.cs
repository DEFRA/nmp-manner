using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Exceptions
{
    public class CustomValidationException : ValidationException
    {
        public CustomValidationException(IEnumerable<ValidationFailure> errors)
        : base("Validation failed", errors)
        {
        }
    }
}
