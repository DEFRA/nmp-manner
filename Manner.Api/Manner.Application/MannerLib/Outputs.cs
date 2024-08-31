using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.MannerLib;

public class Outputs
{
    public Outputs()
    {
        Registration.RegistrationIndicator = 0; // causes registration to take place

        TotalNitrogenApplied = 0d;
        PotentialCropAvailableN = 0d;
        NH3NLoss = 0d;
        N2ONLoss = 0d;
        N2NLoss = 0d;
        NO3NLoss = 0d;
        MineralisedN = 0d;
        PotentialEconomicValue = 0d;
        P2O5CropAvailable = 0d;
        P2O5Total = 0d;
        K2OCropAvailable = 0d;
        K2OTotal = 0d;
        SO3Total = 0d;
        MgOTotal = 0d;
        ResultantNAvailable = 0d;
        ResultantNAvailableSecondCut = 0d;
        ResultantNAvailableYear2 = 0d;
        CropUptake = 0d;
    }
    public double TotalNitrogenApplied { get; set; }
    public double PotentialCropAvailableN { get; set; }

    public double NH3NLoss { get; set; }
    public double N2ONLoss { get; set; }
    public double N2NLoss { get; set; }
    public double NO3NLoss { get; set; }
    public double MineralisedN { get; set; }
    public double PotentialEconomicValue { get; set; }
    public double P2O5CropAvailable { get; set; }
    public double P2O5Total { get; set; }
    public double K2OCropAvailable { get; set; }
    public double K2OTotal { get; set; }
    public double SO3Total { get; set; }
    public double MgOTotal { get; set; }

    public double ResultantNAvailable { get; set; }

    public double ResultantNAvailableSecondCut { get; set; }

    public double ResultantNAvailableYear2 { get; set; }
    public double CropUptake { get; set; }

}
