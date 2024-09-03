namespace Manner.Application.DTOs;

public class Outputs
{
    public Outputs()
    {        
        TotalNitrogenApplied = 0;
        PotentialCropAvailableN = 0;
        NH3NLoss = 0;
        N2ONLoss = 0;
        N2NLoss = 0;
        NO3NLoss = 0;
        MineralisedN = 0;
        PotentialEconomicValue = 0;
        P2O5CropAvailable = 0;
        P2O5Total = 0;
        K2OCropAvailable = 0;
        K2OTotal = 0;
        SO3Total = 0;
        MgOTotal = 0;
        ResultantNAvailable = 0;
        ResultantNAvailableSecondCut = 0;
        ResultantNAvailableYear2 = 0;
        CropUptake = 0;
    }
    public decimal TotalNitrogenApplied { get; set; }
    public decimal PotentialCropAvailableN { get; set; }

    public decimal NH3NLoss { get; set; }
    public decimal N2ONLoss { get; set; }
    public decimal N2NLoss { get; set; }
    public decimal NO3NLoss { get; set; }
    public decimal MineralisedN { get; set; }
    public decimal PotentialEconomicValue { get; set; }
    public decimal P2O5CropAvailable { get; set; }
    public decimal P2O5Total { get; set; }
    public decimal K2OCropAvailable { get; set; }
    public decimal K2OTotal { get; set; }
    public decimal SO3Total { get; set; }
    public decimal MgOTotal { get; set; }

    public decimal ResultantNAvailable { get; set; }

    public decimal ResultantNAvailableSecondCut { get; set; }

    public decimal ResultantNAvailableYear2 { get; set; }
    public decimal CropUptake { get; set; }

}
