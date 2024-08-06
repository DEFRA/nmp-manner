using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.DTOs
{
    public class AutumnCropNitrogenUptakeRequest
    {
        public int CropTypeId { get; set; }
        /// <summary>
        /// January = 1
        /// Febuary = 2
        /// March = 3
        /// April = 4
        /// May = 5
        /// June = 6
        /// July = 7
        /// August = 8
        /// September = 9
        /// October = 10
        /// November = 11
        /// December = 12
        /// </summary>
        public int ApplicationMonth { get; set; }

    }
}
