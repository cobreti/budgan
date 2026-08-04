using BudganGlobal.Errors;

namespace BudganGlobal;

public readonly struct MResult<T>
{
    public bool Succeeded { get; }
    public T? Value { get; }
    public BudganErrorValue? ErrorValue { get; }
    
    private MResult(bool succeeded, T? value, BudganErrorValue? errorValue)
    {
        this.Succeeded = succeeded;
        this.Value = value;
        this.ErrorValue = errorValue;
    }
    
    public static MResult<T> Success(T value)
    {
        return new MResult<T>(true, value, null);
    }

    public static MResult<T> Failure(BudganErrorValue budganErrorValue)
    {
        return new MResult<T>(false, default, budganErrorValue);
    }
}

