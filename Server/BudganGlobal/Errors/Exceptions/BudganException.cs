using System.Runtime.Serialization;

namespace BudganGlobal.Errors.Exceptions;

[Serializable]
public class BudganException : Exception
{
    public BudganErrorValue BudganError { get; }
    
    public BudganException(BudganErrorValue budganError) : base(budganError.ErrorMessage)
    {
        this.BudganError = budganError;
    }
    
    protected BudganException(string message, BudganErrorValue budganError, Exception innerException) : base(message, innerException)
    {
        this.BudganError = budganError;
    }

    protected BudganException(string message, BudganErrorValue budganError) : base(message)
    {
        this.BudganError = budganError;
    }
}
