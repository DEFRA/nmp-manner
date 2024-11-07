using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Core.Entities
{
    public class IncorpMethodsIncorpDelays
    {
        public int IncorporationMethodID { get; set; }
        public int IncorporationDelayID { get; set; }

        public IncorporationMethod IncorporationMethod { get; set; }
        public IncorporationDelay IncorporationDelay { get; set; }
    }
}
