using GardenLog.SharedInfrastructure.Extensions;

namespace UserManagement.CommandHandlers;

public interface IGardenCommandHandler
{
    Task<string> CreateGarden(CreateGardenCommand request);
    Task<string> CreateGardenBed(CreateGardenBedCommand request);
    Task<int> DeleteGarden(string id);
    Task<int> DeleteGardenBed(string gardenId, string gardenBedId);
    Task<int> UpdateGarden(UpdateGardenCommand request);
    Task<int> UpdateGardenBed(UpdateGardenBedCommand request);
}

public class GardenCommandHandler : IGardenCommandHandler
{
    private readonly IGardenRepository _gardenRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public GardenCommandHandler(IGardenRepository plantLocationRepository, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
    {
        _gardenRepository = plantLocationRepository;
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> CreateGarden(CreateGardenCommand request)
    {

        var garden = Garden.Create(
            request.Name,
            request.City,
            request.StateCode,
            request.Latitude,
            request.Longitude,
            request.Notes,
            _httpContextAccessor.HttpContext?.User.GetUserProfileId());

        _gardenRepository.Add(garden);

        await _unitOfWork.SaveChangesAsync();

        return garden.Id;
    }

    public async Task<int> UpdateGarden(UpdateGardenCommand request)
    {

        var garden = await _gardenRepository.GetByIdAsync(request.GardenId);
        if (garden == null) return 0;

        garden.Update(request.Name, request.City, request.StateCode, request.Latitude, request.Longitude, request.Notes);

        _gardenRepository.Update(garden);

        return await _unitOfWork.SaveChangesAsync();

    }

    public async Task<int> DeleteGarden(string id)
    {
        _gardenRepository.Delete(id);
        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<string> CreateGardenBed(CreateGardenBedCommand request)
    {
        var garden = await _gardenRepository.GetByIdAsync(request.GardenId);

        var id = garden.AddGardenBed(request, _httpContextAccessor.HttpContext?.User.GetUserProfileId());

        _gardenRepository.AddGardenBed(id, garden);

        await _unitOfWork.SaveChangesAsync();

        return garden.Id;
    }

    public async Task<int> UpdateGardenBed(UpdateGardenBedCommand request)
    {
        var garden = await _gardenRepository.GetByIdAsync(request.GardenId);

        garden.UpdateGardenBed(request);

        _gardenRepository.UpdateGardenBed(request.GardenBedId, garden);

        return await _unitOfWork.SaveChangesAsync();
    }

    public async Task<int> DeleteGardenBed(string gardenId, string gardenBedId)
    {
        var garden = await _gardenRepository.GetByIdAsync(gardenId);

        garden.DeleteGardenBed(gardenBedId);

        _gardenRepository.DeleteGardenBed(gardenBedId, garden);

        return await _unitOfWork.SaveChangesAsync();
    }
}
