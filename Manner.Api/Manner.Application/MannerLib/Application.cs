using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Manner.Application.MannerLib.Enumerations;

namespace Manner.Application.MannerLib
{
    public class Application
    {
        public Application() 
        {
            Registration.RegistrationIndicator = 0; // causes registration to take place

            Rate = 0d;
            ApplicationMethodEnum = ApplicationMethodEnum.DischargeSpreader;
            ApplicationMethodString = "";
            MethodOfIncorporationEnum = MethodOfIncorporationEnum.ShallowInjection;
            MethodOfIncorporationString = "";
            DelayToIncorporationEnum = DelayToIncorporationEnum.Injection;
            DelayToIncorporationString = "";
        }
        public double Rate { get; set; }
        public double RateGrass { get; set; }
        public double RateArable { get; set; }
        public ApplicationMethodEnum ApplicationMethodEnum { get; set; }
        public string ApplicationMethodString { get; set; }
        public MethodOfIncorporationEnum MethodOfIncorporationEnum { get; set; }
        public string MethodOfIncorporationString { get; set; }
        public DelayToIncorporationEnum DelayToIncorporationEnum { get; set; }
        public string DelayToIncorporationString { get; set; }
        //{
        //    get
        //    {
        //        return _DelayToIncorporationString;
        //    }
        //    set
        //    {
        //        // Apply whitespace rules appropriatley
        //        //value = LiquidTechnologies.Runtime.Net35.WhitespaceUtils.Preserve(value);
        //        _DelayToIncorporationString = value;
        //    }
        //}
        //protected string _DelayToIncorporationString;


    }
}
