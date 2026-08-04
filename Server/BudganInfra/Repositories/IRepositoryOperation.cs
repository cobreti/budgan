namespace BudganInfra.Repositories;

public interface IRepositoryOperation
{
    bool Succeeded { get; }
    Task ExecuteAsync();
}