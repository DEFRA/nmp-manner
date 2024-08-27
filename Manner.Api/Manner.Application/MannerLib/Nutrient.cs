using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.MannerLib
{
    public class Nutrient
    {
        public Nutrient(string displayName) 
        {
            Registration.RegistrationIndicator = 0; // causes registration to take place

            Value = 0d;
            Min = 0d;
            Max = 90d;
            AvailablePercent = 0;
            DisplayValue = displayName;
        }

        public double Value {  get; set; }
        public double Min {  get; set; }
        public double Max { get; set; }
        public int AvailablePercent {  get; set; }
        public string DisplayValue {  get; set; }
        public double AvailableAutumnOsrGrassPercentage {  get; set; }
        public double AvailableSpringPercentage {  get; set; }
        public double AvailableAutumnOtherPercentage {  get; set; }

        private bool CheckValueRange(double value)
        {

            if (value > Max | value < Min)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
