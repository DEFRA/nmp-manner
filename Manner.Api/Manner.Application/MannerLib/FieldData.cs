using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Manner.Application.MannerLib.Enumerations;

namespace Manner.Application.MannerLib;

public class FieldData
{
    public FieldData()
    {
        FieldID = string.Empty;
        CropTypeName= string.Empty;
        CropTypeEnum= CropTypeEnum.Grass;
        TopsoilName = string.Empty;
        SubsoilName= string.Empty;
        TopsoilMoistureEnum = Enumerations.TopsoilMoistureEnum.Dry;
        TopsoilMoistureName= string.Empty;
    }

    public string FieldID { get; set; }
    public Enumerations.CropTypeEnum CropTypeEnum {  get; set; }
    //public int CropTypeId { get; set; }
    public string CropTypeName { get; set; }
    public Enumerations.SoilType Topsoil {  get; set; }
    public string TopsoilName { get; set; }
    public Enumerations.SoilType Subsoil {  get; set; }
    public string SubsoilName {  get; set; }

    public Enumerations.TopsoilMoistureEnum TopsoilMoistureEnum { get; set; }
    public string TopsoilMoistureName {  get; set; }

    public int CropNUptake {  get; set; }

}
