using Manner.Application.MannerLib;

namespace Manner.Application.DTOs;

public class NutrientsResponse
{
    public NutrientsResponse()
    {

    }

    public FieldRef Field { get; set; }

    public List<Outputs> Outputs { get; set; }
    

}
