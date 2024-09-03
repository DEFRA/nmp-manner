using Manner.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Interfaces
{
    public interface ICalculateResultService
    {
        Task<NutrientsResponse> CalculateNutrientsAsync(CalculateNutrientsRequest request);
    }
}
