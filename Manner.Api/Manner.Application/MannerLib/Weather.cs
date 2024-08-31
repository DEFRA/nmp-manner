using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.MannerLib;

public class Weather
{
    public Weather() {
        Registration.RegistrationIndicator = 0; // causes registration to take place            
        RainfallTotal = 0d;
        EvapotranspirationTotal = 0d;
        WindspeedEnum = Enumerations.WindSpeed.CalmGentle0To3BeaufortScale;            
        RainfallEnum = Enumerations.Rainfall.NoRainfallWithin6HoursOfSpreading;            
        WindspeedString = string.Empty;
        RainfallString = string.Empty;
    }

    public DateTime DateOfApplication { get; set; }
    public DateTime EndOfSoilDrainage { get; set; }
    public double RainfallTotal { get; set; }
    public double EvapotranspirationTotal { get; set; }
    public Enumerations.WindSpeed WindspeedEnum {  get; set; }
    public string WindspeedString {  get; set; }
    public Enumerations.Rainfall RainfallEnum {  get; set; }
    public string RainfallString { get; set; }


}
