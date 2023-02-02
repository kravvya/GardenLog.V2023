using GardenLog.SharedInfrastructure.Extensions;
using GardenLog.SharedKernel.Interfaces;


namespace PlantHarvest.Api.CommandHandlers;


public interface IPlantTaskCommandHandler
{
    Task<string> CreatePlantTask(CreatePlantTaskCommand request);
    Task<string> DeletePlantTask(string id);
    Task<string> UpdatePlantTask(UpdatePlantTaskCommand request);
    Task<string> CompletePlantTask(UpdatePlantTaskCommand request);
}

public class PlantTaskCommandHandler : IPlantTaskCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPlantTaskRepository _taskRepository;
    private readonly ILogger<PlantTaskCommandHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PlantTaskCommandHandler(IUnitOfWork unitOfWork, IPlantTaskRepository workLogRepository, ILogger<PlantTaskCommandHandler> logger, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _taskRepository = workLogRepository;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    #region Plant Task

    public async Task<string> CreatePlantTask(CreatePlantTaskCommand request)
    {
        _logger.LogInformation("Received request to create a new task {0}", request);

        string userProfileId = _httpContextAccessor.HttpContext?.User.GetUserProfileId(_httpContextAccessor.HttpContext.Request.Headers);


        var task = PlantTask.Create(request.Description, request.Type
            , request.CreatedDateTime, request.TargetDateStart, request.TargetDateStart, request.CompletedDateTime
            , request.HarvestCycleId, request.PlantHarvestCycleId, request.PlantName, request.PlantScheduleId, request.Notes, request.IsSystemGenerated, userProfileId);

        _taskRepository.Add(task);

        await _unitOfWork.SaveChangesAsync();

        return task.Id;
    }

    public async Task<string> UpdatePlantTask(UpdatePlantTaskCommand request)
    {
        _logger.LogInformation("Received request to update task {0}", request);

         var task = await _taskRepository.GetByIdAsync(request.PlantTaskId);

        task.Update(request.TargetDateStart, request.TargetDateEnd, request.CompletedDateTime, request.Notes);

        _taskRepository.Update(task);

        await _unitOfWork.SaveChangesAsync();

        return task.Id;
    }

    public async Task<string> DeletePlantTask(string id)
    {
        _logger.LogInformation("Received request to delete task {0}", id);

        _taskRepository.Delete(id);

        await _unitOfWork.SaveChangesAsync();

        return id;
    }

    public Task<string> CompletePlantTask(UpdatePlantTaskCommand request)
    {
        return UpdatePlantTask(request);
    }
    #endregion

}

