using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.DTOs
{
    public class StandardResponse
    {
        public StandardResponse() 
        {
            Success = false;
            Message = string.Empty;
            Errors = new List<string>();
        }
        public bool Success { get; set; }
        public string Message { get; set; } 

        public dynamic? Data { get; set; }

        public List<string> Errors { get; set; }

    }
}
