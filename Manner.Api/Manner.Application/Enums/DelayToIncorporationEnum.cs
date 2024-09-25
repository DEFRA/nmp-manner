namespace Manner.Application.Enums;
public partial class Enumerations
{    
    #region Enumeration 'DelayToIncorporation'        
    public enum DelayToIncorporationEnum
    {
        Injection = 1,
        LessThan2Hours = 2,
        n2To4Hours = 3,
        n4To6Hours = 4,
        n6To12Hours = 5,
        n12To24Hours = 6,
        n1To2Days = 7,
        n3To5Days = 8,
        n3To7Days = 9,
        n6To12Days = 10,
        GreaterThan7Days = 11,
        GreaterThan12Days = 12,
        n12To32Days = 13,
        GreaterThan32Days = 14,
        NotIncorporated = 15
    }

    #endregion

}
