using FluentValidation;
using Manner.Application.DTOs;

namespace Manner.Application.Validators;

public class ManureApplicationValidator : AbstractValidator<ManureApplication>
{
    public ManureApplicationValidator()
    {
        //TO: Need to reviste to apply validation        

        // Total N must be equal to or more than the sum of Ammonium-N + Uric acid N + Nitrate N
        
    }

#pragma warning disable S1144
    private static bool ValidateApplication(ManureApplication application)
#pragma warning restore S1144
    {
        return application.ApplicationDate > application.EndOfDrainageDate;
    }
}
