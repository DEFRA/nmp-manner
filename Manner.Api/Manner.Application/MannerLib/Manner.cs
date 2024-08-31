using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.MannerLib
{
    public class Manner
    {
        public ManureType ManureType {  get; set; }
        public MannerLib.Application Application {  get; set; }
        public MannerLib.FieldData FieldData {  get; set; }
        public MannerLib.Weather Weather { get; set; }
        public MannerLib.Outputs Outputs { get; set; }
        public int CountryCode {  get; set; }
        public int Easting {  get; set; }
        public string Postcode {  get; set; }
        public int Northing { get; set; }
        public int RunType { get; set; }
    }
}
