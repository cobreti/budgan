using System.Diagnostics.CodeAnalysis;

namespace BudganInfra.Repositories;

public class BaseRepositoryOperationWithResultValue<RETTYPE> : BaseRepositoryOperation
{
    [field: AllowNull, MaybeNull]
    public RETTYPE ResultValue
    {
        get
        {
            if (EqualityComparer<RETTYPE>.Default.Equals(field, default(RETTYPE)))
            {
                throw new Exception("Invalid operation");
            }

            return field;
        }

        protected set;
    } = default(RETTYPE);
    
    protected void SetSucceeded(RETTYPE resultValue)
    {
        this.ResultValue = resultValue;
        this.SetSucceeded();
    }
}
