using AutoMapper;

namespace PlantCatalog.Api.CommandHandlers.Mappers;

public class PlantProfile : Profile
{
    public PlantProfile()
    {
        CreateMap<Plant, PlantViewModel>();
    }
}
