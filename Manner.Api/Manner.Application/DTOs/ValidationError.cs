using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Manner.Application.DTOs;

public class ValidationError
{
    public ValidationError()
    {
        Type= string.Empty;
        Title= string.Empty;
        Errors = new Dictionary<string, string[]>();
        TraceId= string.Empty;
    }

    public string Type { get; set; }
    public string Title { get; set; }   
   
    public int Status { get; set; }
    
    public Dictionary<string, string[]> Errors { get; set; }
    
    public string TraceId { get; set; }


}
