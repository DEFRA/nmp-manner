using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.MannerLib;

public class DryMatter
{
    private DryMatterType _nutrient;

    public event DryMatterValueChangedEventHandler DryMatterValueChanged;

    public delegate void DryMatterValueChangedEventHandler();
    public event DryMatterOutOfRangeEventHandler DryMatterOutOfRange;

    public delegate void DryMatterOutOfRangeEventHandler(string srange);

    public double Min
    {
        get { return _nutrient.Min; }
        set { _nutrient.Min = value; }

    }
    public double Max
    {
        get { return _nutrient.Max; }
        set { _nutrient.Max = value; }
    }
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
                DryMatterValueChanged?.Invoke();
            }
            else
            {
                string srange;
                srange = "Min value: " + _nutrient.Min + ", Max value: " + _nutrient.Max;
                DryMatterOutOfRange?.Invoke(srange);
            }

        }
    }

    internal DryMatterType LocalDrymatter
    {
        set
        {
            _nutrient = value;
        }
    }
}
