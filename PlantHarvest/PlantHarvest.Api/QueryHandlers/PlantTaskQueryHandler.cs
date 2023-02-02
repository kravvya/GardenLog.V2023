namespace PlantHarvest.Api.QueryHandlers;


public interface IPlantTaskQueryHandler
{
    Task<IReadOnlyCollection<PlantTaskViewModel>> GetPlantTasks();
    Task<IReadOnlyCollection<PlantTaskViewModel>> GetActivePlantTasks();
}


public class PlantTaskQueryHandler : IPlantTaskQueryHandler
{
    private readonly IPlantTaskRepository _taskRepository;
    private readonly ILogger<PlantTaskQueryHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PlantTaskQueryHandler(IPlantTaskRepository taskRepository, ILogger<PlantTaskQueryHandler> logger, IHttpContextAccessor httpContextAccessor)
    {
        _taskRepository = taskRepository;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task<IReadOnlyCollection<PlantTaskViewModel>> GetPlantTasks()
    {
        _logger.LogInformation("Received request to get all tasks");
        string userProfileId = _httpContextAccessor.HttpContext?.User.GetUserProfileId(_httpContextAccessor.HttpContext.Request.Headers);
        return await _taskRepository.GetPlantTasksForUser(userProfileId);
    }

    public async Task<IReadOnlyCollection<PlantTaskViewModel>> GetActivePlantTasks()
    {
        _logger.LogInformation("Received request to get all tasks");
        string userProfileId = _httpContextAccessor.HttpContext?.User.GetUserProfileId(_httpContextAccessor.HttpContext.Request.Headers);
        return await _taskRepository.GetActivePlantTasksForUser(userProfileId);
    }

}