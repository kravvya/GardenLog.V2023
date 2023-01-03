namespace GardenLog.SharedKernel.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
