using Manner.Core.Entities;

namespace Manner.Core.Interfaces;
public interface IClimateRepository : IRepository<Climate>
{
    Task<Climate?> FetchByPostcodeAsync(string postcode);
}
