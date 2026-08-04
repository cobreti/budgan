namespace BudganServices.UseCases;

public interface IBaseUseCase
{
    bool Succeeded { get; }
    Task ExecuteAsync();
}