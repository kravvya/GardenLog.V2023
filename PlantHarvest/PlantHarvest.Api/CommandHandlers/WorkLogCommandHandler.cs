using GardenLog.SharedInfrastructure.Extensions;
using GardenLog.SharedKernel.Interfaces;
using PlantHarvest.Domain.WorkLogAggregate;

namespace PlantHarvest.Api.CommandHandlers;


public interface IWorkLogCommandHandler
{
    Task<string> CreateWorkLog(CreateWorkLogCommand request);
    Task<string> DeleteWorkLog(string id);
    Task<string> UpdateWorkLog(UpdateWorkLogCommand request);
}

public class WorkLogCommandHandler : IWorkLogCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWorkLogRepository _workLogRepository;
    private readonly ILogger<WorkLogCommandHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WorkLogCommandHandler(IUnitOfWork unitOfWork, IWorkLogRepository workLogRepository, ILogger<WorkLogCommandHandler> logger, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _workLogRepository = workLogRepository;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    #region Work log

    public async Task<string> CreateWorkLog(CreateWorkLogCommand request)
    {
        _logger.LogInformation("Received request to create a new worklog {0}", request);

        string userProfileId = _httpContextAccessor.HttpContext?.User.GetUserProfileId(_httpContextAccessor.HttpContext.Request.Headers);


        var workLog = WorkLog.Create(
                 log: request.Log,
                 entity: request.RelatedEntity,
                 entityId: request.RelatedEntityid,
                 eventDateTime: request.EventDateTime,
                 reason: request.Reason,
                 userProfileId: userProfileId);

        _workLogRepository.Add(workLog);

        await _unitOfWork.SaveChangesAsync();

        return workLog.Id;
    }

    public async Task<string> UpdateWorkLog(UpdateWorkLogCommand request)
    {
        _logger.LogInformation("Received request to update work log {0}", request);

        string userProfileId = _httpContextAccessor.HttpContext?.User.GetUserProfileId(_httpContextAccessor.HttpContext.Request.Headers);

       var workLog = await _workLogRepository.GetByIdAsync(request.WorkLogId);

        workLog.Update(request.Log, request.RelatedEntity, request.RelatedEntityid, request.EventDateTime, request.Reason);

        _workLogRepository.Update(workLog);

        await _unitOfWork.SaveChangesAsync();

        return workLog.Id;
    }

    public async Task<string> DeleteWorkLog(string id)
    {
        _logger.LogInformation("Received request to delete work log {0}", id);

        _workLogRepository.Delete(id);

        await _unitOfWork.SaveChangesAsync();

        return id;
    }
    #endregion

}

