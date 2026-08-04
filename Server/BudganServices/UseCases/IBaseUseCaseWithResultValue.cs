namespace BudganServices.UseCases;

public interface IBaseUseCaseWithResultValue<RESTYPE> : IBaseUseCase
{
    RESTYPE ResultValue { get; }
}
