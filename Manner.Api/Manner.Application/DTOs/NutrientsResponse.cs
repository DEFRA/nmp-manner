using Manner.Application.MannerLib;

namespace Manner.Application.DTOs;

public class NutrientsResponse
{
    public NutrientsResponse()
    {
        Field = new FieldRef();
        Outputs = new Outputs();
    }
    public FieldRef Field { get; set; }

    public Outputs Outputs { get; set; }   

}
