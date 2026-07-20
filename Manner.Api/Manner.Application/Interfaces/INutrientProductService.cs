using Manner.Application.DTOs;
using Manner.Core.Entities;
namespace Manner.Application.Interfaces;

public interface INutrientProductService : IService<NutrientProductDto>
{
    Task<IEnumerable<NutrientProductDto>?> FetchByNutrientIdAsync(int nutrientId);
}