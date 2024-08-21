using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Core.Entities
{
    public class ApplicationMethodsIncorpMethods
    {
        public int ApplicationMethodID { get; set; }
        public int IncorporationMethodID { get; set; }

        public ApplicationMethod ApplicationMethod { get; set; }
        public IncorporationMethod IncorporationMethod { get; set; }
    }
}
