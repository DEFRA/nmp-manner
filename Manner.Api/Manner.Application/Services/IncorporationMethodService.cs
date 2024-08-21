using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manner.Application.Services
{
    [Service(ServiceLifetime.Transient)]
    public class IncorporationMethodService : IIncorporationMethodService
    {
        private readonly IIncorporationMethodRepository _incorporationMethodRepository;
        private readonly IMapper _mapper;

        public IncorporationMethodService(IIncorporationMethodRepository incorporationMethodRepository, IMapper mapper)
        {
            _incorporationMethodRepository = incorporationMethodRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<IncorporationMethodDto>?> FetchAllAsync()
        {
            var methods = await _incorporationMethodRepository.FetchAllAsync();
            return _mapper.Map<IEnumerable<IncorporationMethodDto>>(methods);
        }

        public async Task<IncorporationMethodDto?> FetchByIdAsync(int id)
        {
            var method = await _incorporationMethodRepository.FetchByIdAsync(id);
            return _mapper.Map<IncorporationMethodDto>(method);
        }

        public async Task<IEnumerable<IncorporationMethodDto>?> FetchByAppMethodIdAsync(int methodId)
        {
            var methods = await _incorporationMethodRepository.FetchByAppMethodIdAsync(methodId);
            return _mapper.Map<IEnumerable<IncorporationMethodDto>>(methods);
        }
    }
}
