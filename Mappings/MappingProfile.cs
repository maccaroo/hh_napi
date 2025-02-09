using AutoMapper;
using hh_napi.Domain;
using hh_napi.Models.Responses;

namespace hh_napi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserResponse>();

            CreateMap<DataSource, DataSourceResponse>()
                .ForMember(dest => dest.CreatedByUser, opt => opt.Condition(src => src.CreatedByUser != null))
                .ForMember(dest => dest.CreatedByUser, opt => opt.MapFrom(src => src.CreatedByUser));

            CreateMap<DataPoint, DataPointResponse>()
                .ForMember(dest => dest.DataSource, opt => opt.Condition(src => src.DataSource != null))
                .ForMember(dest => dest.DataSource, opt => opt.MapFrom(src => src.DataSource));
        }
    }
}