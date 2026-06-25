using AutoMapper;
using Manner.Application.DTOs;
using Manner.Application.Interfaces;
using Manner.Core.Attributes;
using Manner.Core.Entities;
using Manner.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manner.Application.Services
{
    [Service(ServiceLifetime.Transient)]
    public class IncorporationMethodService(ILogger<IncorporationMethodService> logger, IIncorporationMethodRepository incorporationMethodRepository, IMapper mapper) : IIncorporationMethodService
    {
        private readonly IIncorporationMethodRepository _incorporationMethodRepository = incorporationMethodRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<IncorporationMethodService> _logger = logger;

        public async Task<IEnumerable<IncorporationMethodDto>?> FetchAllAsync()
        {
            _logger.LogTrace("IncorporationMethodService : FetchAllAsync() callled");
            var methods = await _incorporationMethodRepository.FetchAllAsync();
            return _mapper.Map<IEnumerable<IncorporationMethodDto>>(methods);
        }

        public async Task<IncorporationMethodDto?> FetchByIdAsync(int id)
        {
            _logger.LogTrace("IncorporationMethodService : FetchByIdAsync({Id}) callled", id);
            var method = await _incorporationMethodRepository.FetchByIdAsync(id);
            return _mapper.Map<IncorporationMethodDto>(method);
        }

        public async Task<IEnumerable<IncorporationMethodDto>?> FetchByAppMethodIdAsync(int methodId)
        {
            _logger.LogTrace("IncorporationMethodService : FetchByAppMethodIdAsync({MethodId}) callled", methodId);
            var methods = await _incorporationMethodRepository.FetchByAppMethodIdAsync(methodId);
            return _mapper.Map<IEnumerable<IncorporationMethodDto>>(methods);
        }

        public async Task<IEnumerable<IncorporationMethodDto>?> FetchByAppMethodIdAndApplicableForAsync(int methodId, string applicableFor)
        {
            _logger.LogTrace("IncorporationMethodService : FetchByAppMethodIdAndApplicableForAsync({MethodId},{ApplicableFor}) callled", methodId, applicableFor);
            var methods = await _incorporationMethodRepository.FetchByAppMethodIdAndApplicableForAsync(methodId, applicableFor);
            return _mapper.Map<IEnumerable<IncorporationMethodDto>>(methods);
        }
    }
}
