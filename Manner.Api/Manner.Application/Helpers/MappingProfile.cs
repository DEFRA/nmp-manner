using AutoMapper;
using Manner.Application.DTOs;
using Manner.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Helpers;
public class MappingProfile : Profile
{
    public MappingProfile()
    {        
        CreateMap<ClimateDto, Climate>().MaxDepth(5).PreserveReferences();
        CreateMap<Climate, ClimateDto>().MaxDepth(5).PreserveReferences();

        CreateMap<ApplicationMethodDto, ApplicationMethod>().MaxDepth(5).PreserveReferences();
        CreateMap<ApplicationMethod, ApplicationMethodDto>().MaxDepth(5).PreserveReferences();

        CreateMap<CountryDto, Country>().MaxDepth(5).PreserveReferences();
        CreateMap<Country, CountryDto>().MaxDepth(5).PreserveReferences();

        CreateMap<CropTypeDto, CropType>().MaxDepth(5).PreserveReferences();
        CreateMap<CropType, CropTypeDto>().MaxDepth(5).PreserveReferences();

        CreateMap<IncorporationDelayDto, IncorporationDelay>().MaxDepth(5).PreserveReferences();
        CreateMap<IncorporationDelay, IncorporationDelayDto>().MaxDepth(5).PreserveReferences();

        CreateMap<IncorporationMethodDto, IncorporationMethod>().MaxDepth(5).PreserveReferences();
        CreateMap<IncorporationMethod, IncorporationMethodDto>().MaxDepth(5).PreserveReferences();

        CreateMap<ManureGroupDto, ManureGroup>().MaxDepth(5).PreserveReferences();
        CreateMap<ManureGroup, ManureGroupDto>().MaxDepth(5).PreserveReferences();
        CreateMap<ManureTypeCategoryDto, ManureTypeCategory>().MaxDepth(5).PreserveReferences();
        CreateMap<ManureTypeCategory, ManureTypeCategoryDto>().MaxDepth(5).PreserveReferences();

        CreateMap<ManureTypeDto, ManureType>().MaxDepth(5).PreserveReferences();
        CreateMap<ManureType, ManureTypeDto>().MaxDepth(5).PreserveReferences();
        CreateMap<MoistureTypeDto, MoistureType>().MaxDepth(5).PreserveReferences();
        CreateMap<MoistureType, MoistureTypeDto>().MaxDepth(5).PreserveReferences();

        CreateMap<RainTypeDto, RainType>().MaxDepth(5).PreserveReferences();
        CreateMap<RainType, RainTypeDto>().MaxDepth(5).PreserveReferences();

        CreateMap<SubSoilDto, SubSoil>().MaxDepth(5).PreserveReferences();
        CreateMap<SubSoil, SubSoilDto>().MaxDepth(5).PreserveReferences();

        CreateMap<TopSoilDto, TopSoil>().MaxDepth(5).PreserveReferences();
        CreateMap<TopSoil, TopSoilDto>().MaxDepth(5).PreserveReferences();

        CreateMap<WindspeedDto, Windspeed>().MaxDepth(5).PreserveReferences();
        CreateMap<Windspeed, WindspeedDto>().MaxDepth(5).PreserveReferences();

        CreateMap<ClimateTypeDto, ClimateType>().MaxDepth(5).PreserveReferences();
        CreateMap<ClimateType, ClimateTypeDto>().MaxDepth(5).PreserveReferences();

        CreateMap<NutrientDto, Nutrient>().MaxDepth(5).PreserveReferences();
        CreateMap<Nutrient, NutrientDto>().MaxDepth(5).PreserveReferences();

        CreateMap<NutrientProductDto, NutrientProduct>().MaxDepth(5).PreserveReferences();
        CreateMap<NutrientProduct, NutrientProductDto>().MaxDepth(5).PreserveReferences();

        // Add more mappings here
    }
}
