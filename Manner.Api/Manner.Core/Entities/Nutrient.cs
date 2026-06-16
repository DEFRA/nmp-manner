using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Core.Entities
{
    public class Nutrient
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string MeasurementUnit { get; set; } = string.Empty;
        public decimal UnitRate { get; set; } = 0.0m;
        public string CurrencyCode { get; set; } = "GBP";

    }
}
