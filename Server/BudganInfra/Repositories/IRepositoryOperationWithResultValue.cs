namespace BudganInfra.Repositories;

public interface IRepositoryOperationWithResultValue<RETTYPE> : IRepositoryOperation
{
    RETTYPE ResultValue { get; }
}
