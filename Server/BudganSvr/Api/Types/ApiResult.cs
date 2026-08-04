namespace BudganSvr.Api.Types;

/// <summary>
/// Represents the outcome of an API operation.
/// </summary>
public class ApiResult
{
    public bool Succeeded { get; set; }
}

/// <summary>
/// Represents a successful API operation and the value it produced.
/// </summary>
/// <typeparam name="TYPE">The type of the value returned by the operation.</typeparam>
public class ApiSuccessResult<TYPE> : ApiResult
{
    public ApiSuccessResult(TYPE successValue)
    {
        this.SuccessValue = successValue;
        this.Succeeded = true;
    }

    public TYPE SuccessValue { get; set; }
}

/// <summary>
/// Represents a failed API operation and the associated error information.
/// </summary>
/// <typeparam name="ERROR_TYPE">The type describing the error that occurred.</typeparam>
public class ApiErrorResult<ERROR_TYPE> : ApiResult
{
    public ApiErrorResult(ERROR_TYPE errorValue)
    {
        this.ErrorValue = errorValue;
        this.Succeeded = false;
    }

    public ERROR_TYPE ErrorValue { get; set; }
}
