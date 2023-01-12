namespace GardenLog.SharedKernel.Interfaces;

public interface IUnitOfWork
{
    void AddCommand(Func<Task> func);
   T GetCollection<T, Y>(string collectionName);
    void OnConfiguring();
    Task<int> SaveChangesAsync();
}
