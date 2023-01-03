﻿namespace GardenLog.SharedKernel.Interfaces;

public interface IRepository<T> : IUnitOfWork where T : class, IAggregateRoot
{
    void Add(T entity);
    Task<T> GetByIdAsync(string id);
    void Update(T entity);
    void Delete(string id);
}