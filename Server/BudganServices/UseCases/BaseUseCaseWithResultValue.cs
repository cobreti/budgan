using System.Diagnostics.CodeAnalysis;

namespace BudganServices.UseCases;

public class BaseUseCaseWithResultValue<RESTYPE> : BaseUseCase
{
    [field: AllowNull, MaybeNull]
    public RESTYPE ResultValue
    {
        get
        {
            if (EqualityComparer<RESTYPE>.Default.Equals(field, default(RESTYPE)))
            {
                throw new Exception("Invalid operation");
            }

            return field;
        }

        protected set;
    } = default(RESTYPE);
    
    protected void SetSucceeded(RESTYPE resultValue)
    {
        this.ResultValue = resultValue;
        this.SetSucceeded();
    }
}