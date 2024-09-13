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
        Outputs CalculateNutrients();
        
    }
}
