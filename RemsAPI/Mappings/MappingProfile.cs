using AutoMapper;
using RemsAPI.DTOs;
using RemsAPI.Entities;

namespace RemsAPI.Mappings;

public class MappingProfile : Profile
{
  public MappingProfile()
  {
    CreateMap<City, CityDto>().ReverseMap();
    CreateMap<City, CreateCityDto>().ReverseMap();
    CreateMap<City, UpdateCityDto>().ReverseMap();

    CreateMap<District, DistrictDto>().ReverseMap();
    CreateMap<District, CreateDistrictDto>().ReverseMap();
    CreateMap<District, UpdateDistrictDto>().ReverseMap();

    CreateMap<Neighborhood, NeighborhoodDto>().ReverseMap();
    CreateMap<Neighborhood, CreateNeighborhoodDto>().ReverseMap();
    CreateMap<Neighborhood, UpdateNeighborhoodDto>().ReverseMap();

    CreateMap<Property, PropertyDto>()
      .ForMember(dest => dest.DistrictName, opt => opt.MapFrom(src => src.Neighborhood!.District!.Name))
      .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.Neighborhood!.District!.City!.Name))
      .ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => src.Neighborhood!.DistrictId))
      .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.Neighborhood!.District!.CityId))
      .ForMember(dest => dest.Geometry, opt => opt.MapFrom(src => src.Geometry.AsText()))
      .ReverseMap();
    CreateMap<CreatePropertyDto, Property>().ForMember(dest => dest.Geometry, opt => opt.Ignore()).ReverseMap();
    CreateMap<UpdatePropertyDto, Property>().ForMember(dest => dest.Geometry, opt => opt.Ignore()).ReverseMap();

    CreateMap<Log, LogDto>().ReverseMap();
    CreateMap<Log, LogFilterDto>().ReverseMap();

    CreateMap<User, UserDto>().ReverseMap();
    CreateMap<User, CreateUserDto>().ReverseMap();
    CreateMap<User, UserViewDto>().ReverseMap();
  }

}
