using Manner.Application.MannerLib;

namespace Manner.Application.DTOs;

public class NutrientsResponse
{
    public NutrientsResponse()
    {       
        
    }

    public int? FieldID { get; set; }
    public string? FieldName { get; set; }
    public int TotalN { get; set; }
    public int MineralisedN { get; set; }
    public int NitrateNLoss { get; set; }
    public int AmmoniaNLoss {  get; set; }
    public int DenitrifiedNLoss {  get; set; }
    public int CurrentCropAvailableN {  get; set; }
    public int NextGrassNCropCurrentYear {  get; set; }
    public int FollowingCropYear2AvailableN { get; set; }
    public int NitrogenEfficiencePercentage {  get; set; }
    public int TotalP2O5 { get; set; }
    public int CropAvailableP2O5 { get; set; }
    public int TotalK2O { get; set; }
    public int CropAvailableK2O { get; set; }    
    public int TotalSO3 { get; set; }
    public int TotalMgO { get; set; }
}
