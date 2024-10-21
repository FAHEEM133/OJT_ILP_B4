using Application.DTOs;
using AutoMapper;
using Domain.Model;
using Domain.Enums;
using Domain.Enums.Domain.Enums;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Market to MarketDetailsDto
            CreateMap<Market, MarketDetailsDto>()
                .ForMember(dest => dest.Region, opt => opt.MapFrom(src => Enum.GetName(typeof(Region), src.Region)))
                .ForMember(dest => dest.SubRegion, opt => opt.MapFrom(src => Enum.GetName(typeof(SubRegion), src.SubRegion)))
                .ForMember(dest => dest.MarketSubGroups, opt => opt.MapFrom(src => src.MarketSubGroups));

            // MarketSubGroup to MarketSubGroupDto
            CreateMap<MarketSubGroup, MarketSubGroupDTO>();
        }
    }
}


