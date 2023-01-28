using GardenLog.SharedInfrastructure.Extensions;

namespace UserManagement.QueryHandlers;

public interface IGardenQueryHandler
{
    Task<GardenViewModel> GetGarden(string id);
    Task<GardenBedViewModel> GetGardenBed(string gardenId, string id);
    Task<IReadOnlyCollection<GardenBedViewModel>> GetGardenBeds(string id);
    Task<IReadOnlyCollection<GardenViewModel>> GetGardens();
}

public class GardenQueryHandler : IGardenQueryHandler
{
    private readonly IGardenRepository _gardenRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public GardenQueryHandler(IGardenRepository plantLocationRepository, IHttpContextAccessor httpContextAccessor)
    {
        _gardenRepository = plantLocationRepository;
        _httpContextAccessor = httpContextAccessor;

    }
    public Task<GardenViewModel> GetGarden(string id) => _gardenRepository.GetGarden(id);

    public Task<IReadOnlyCollection<GardenBedViewModel>> GetGardenBeds(string id) => _gardenRepository.GetGardenBeds(id);

    public Task<GardenBedViewModel> GetGardenBed(string gardenId, string id) => _gardenRepository.GetGardenBed(gardenId, id);

    public Task<IReadOnlyCollection<GardenViewModel>> GetGardens() => _gardenRepository.GetGardens(_httpContextAccessor.HttpContext.User.GetUserProfileId());

}
