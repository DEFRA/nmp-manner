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
    public ApplicationMethodService(IApplicationMethodRepository applicationMethodRepository)
    {
        _applicationMethodRepository = applicationMethodRepository;
    }

    public async Task<IEnumerable<ApplicationMethodDto>?> FetchAllAsync()
    {
        return null;// await _applicationMethodRepository.FetchAllAsync();
    }

    public async Task<ApplicationMethodDto?> FetchByIdAsync(int id)
    {
        return null; // await _applicationMethodRepository.FetchByIdAsync(id);
    }
}
