using BudganGlobal.Errors;

namespace BudganInfra.Repositories;

public interface IRepositoryOperation
{
    bool Succeeded { get; }
    BudganErrorValue BudganErrorValue { get; }
    Task ExecuteAsync();
}