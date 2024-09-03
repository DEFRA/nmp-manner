using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Calculators;
[Service(ServiceLifetime.Transient)]
public class NutrientsCalculator : INutrientsCalculator
{
    public void CalculateNutrients()
    {

    }

    public Task<CalculateNutrientsResponse> CalculateNutrients(CalculateNutrientsRequest calculateNutrientsRequest)
    {
        CalculateNutrientsResponse ret = new CalculateNutrientsResponse();


        return ret;
    }

    public Outputs CalculateNutrientsOutputsValues(DTOs.MannerApplication application,ManureType manureType)
    {
        Outputs ret = new Outputs();
        ret.P2O5Total = application.ManureDetails.TotalP2O5 * application.ApplicationRate.Value;
        ret.K2OTotal = application.ManureDetails.TotalK2O * application.ApplicationRate.Value;
        ret.MgOTotal = application.ManureDetails.TotalMgO * application.ApplicationRate.Value;
        ret.SO3Total = application.ManureDetails.TotalSO3 * application.ApplicationRate.Value;
        ret.P2O5CropAvailable = application.ManureDetails.TotalP2O5 * application.ApplicationRate.Value * (manureType.P2O5Available / 100);
        ret.K2OCropAvailable = application.ManureDetails.TotalK2O * application.ApplicationRate.Value * (manureType.K2OAvailable / 100);

        return ret;
    }
}
