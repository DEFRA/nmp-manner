using Manner.Application.DTOs;
using Manner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Interfaces
{
    public interface INutrientsCalculator
    {
        Task<CalculateNutrientsResponse> CalculateNutrients(CalculateNutrientsRequest calculateNutrientsRequest);
        Outputs CalculateNutrientsOutputsValues(MannerApplication application, ManureType manureType);
    }
}
