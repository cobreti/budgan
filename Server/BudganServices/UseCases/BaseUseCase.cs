namespace BudganServices.UseCases;

public class BaseUseCase
{
    public bool Succeeded { get; protected set; } = true;

    protected void SetSucceeded()
    {
        this.Succeeded = true;
    }

    protected void SetFailed()
    {
        this.Succeeded = false;
    }
}