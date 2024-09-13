using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.DTOs;

public class CalculateNutrientsResponse
{
    public CalculateNutrientsResponse()
    {
        Results = new List<NutrientsResponse>();
    }
    public List<NutrientsResponse> Results { get; set; }
}

