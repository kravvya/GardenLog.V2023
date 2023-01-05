using AutoMapper;

namespace PlantCatalog.Api.CommandHandlers.Mappers;

public class PlantProfile : Profile
{
    public PlantProfile()
    {
        CreateMap<Plant, PlantViewModel>()
            .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src =>src.Id));
    }
}
