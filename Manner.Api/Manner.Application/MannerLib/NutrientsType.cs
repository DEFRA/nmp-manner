using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.MannerLib;

public class NutrientsType
{

    private MannerLib.Nutrient _nutrient;

    public NutrientsType()
    {

    }

    public event NutrientValueChangedEventHandler NutrientValueChanged;

    public delegate void NutrientValueChangedEventHandler();
    public event NutrientOutOfRangeEventHandler NutrientOutOfRange;

    public delegate void NutrientOutOfRangeEventHandler(string srange);

    public double Value
    {
        get
        {
            return _nutrient.Value;
        }
        set
        {

            if (value >= _nutrient.Min & value <= _nutrient.Max)
            {
                _nutrient.Value = value;
                NutrientValueChanged?.Invoke();
            }
            else
            {
                string srange;
                srange = "Min value: " + _nutrient.Min + ", Max value: " + _nutrient.Max;
                NutrientOutOfRange?.Invoke(srange);
            }

        }
    }

    public double Max
    {
        get
        {
            return _nutrient.Max;
        }
        set
        {
            _nutrient.Max = value;
        }
    }
    public double Min
    {
        get
        {
            return _nutrient.Min;
        }
        set
        {
            _nutrient.Min = value;
        }
    }

    public int AvailablePercentage
    {
        get
        {
            return _nutrient.AvailablePercent;
        }
        set
        {
            _nutrient.AvailablePercent = value;
        }
    }

    internal MannerLib.Nutrient LocalNutrient
    {
        set
        {
            _nutrient = value;
        }
    }

}
