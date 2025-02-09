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
            CreateMap<DataSource, DataSourceResponse>();
            CreateMap<DataPoint, DataPointResponse>();
        }
    }
}