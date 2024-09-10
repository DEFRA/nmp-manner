using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Manner.Application.Services;

[Service(ServiceLifetime.Transient)]
public class ApplicationMethodService : IApplicationMethodService
{
    private readonly IApplicationMethodRepository _applicationMethodRepository;
    private readonly IMapper _mapper;
    public ApplicationMethodService(IApplicationMethodRepository applicationMethodRepository, IMapper mapper)
    {
        _applicationMethodRepository = applicationMethodRepository;
        _mapper= mapper;
    }

    public async Task<IEnumerable<ApplicationMethodDto>?> FetchAllAsync()
    {
        return _mapper.Map<IEnumerable<ApplicationMethodDto>>(await _applicationMethodRepository.FetchAllAsync());
    }

    public async Task<ApplicationMethodDto?> FetchByIdAsync(int id)
    {
        return _mapper.Map<ApplicationMethodDto>(await _applicationMethodRepository.FetchByIdAsync(id));
    }

    public async Task<IEnumerable<ApplicationMethodDto>?> FetchByCriteriaAsync(bool? isLiquid = null, int? fieldType = null)
    {
        var methods = await _applicationMethodRepository.FetchByCriteriaAsync(isLiquid, fieldType);
        return _mapper.Map<IEnumerable<ApplicationMethodDto>>(methods);
    }
}
