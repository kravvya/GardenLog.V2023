

namespace PlantHarvest.Domain.WorkLogAggregate;

public interface IWorkLogRepository : IRepository<WorkLog>
{
    Task<IReadOnlyCollection<WorkLogViewModel>> GetWorkLogsByEntity(WorkLogEntityEnum entityType, string entityId, string userProfileId);
}
