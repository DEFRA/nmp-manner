using AutoMapper;
using Manner.Application.DTOs;
using Manner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<Source, Destination>();
            CreateMap<ClimateDto, Climate>();
            CreateMap<Climate, ClimateDto>();

            CreateMap<ApplicationMethodDto, ApplicationMethod>();
            CreateMap<ApplicationMethod, ApplicationMethodDto>();

            CreateMap<CountryDto, Country>();
            CreateMap<Country, CountryDto>();

            CreateMap<CropTypeDto, CropType>();
            CreateMap<CropType, CropTypeDto>();

            CreateMap<IncorporationDelayDto, IncorporationDelay>();
            CreateMap<IncorporationDelay, IncorporationDelayDto>();

            CreateMap<IncorporationMethodDto, IncorporationMethod>();
            CreateMap<IncorporationMethod, IncorporationMethodDto>();

            CreateMap<ManureGroupDto, ManureGroup>();
            CreateMap<ManureGroup, ManureGroupDto>();

            CreateMap<ManureTypeCategoryDto, ManureTypeCategory>();
            CreateMap<ManureTypeCategory, ManureTypeCategoryDto>();

            CreateMap<ManureTypeDto, ManureType>();
            CreateMap<ManureType, ManureTypeDto>();

            CreateMap<MoistureTypeDto, MoistureType>();
            CreateMap<MoistureType, MoistureTypeDto>();

            CreateMap<RainTypeDto, RainType>();
            CreateMap<RainType, RainTypeDto>();

            CreateMap<SubSoilDto, SubSoil>();
            CreateMap<SubSoil, SubSoilDto>();


            CreateMap<TopSoilDto, TopSoil>();
            CreateMap<TopSoil, TopSoilDto>();

            CreateMap<WindspeedDto, Windspeed>();
            CreateMap<Windspeed, WindspeedDto>();

            // Add more mappings here
        }
    }
}
