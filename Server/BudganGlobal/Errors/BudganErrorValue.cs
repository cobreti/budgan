namespace BudganGlobal.Errors;

public class BudganErrorValue
{
    private static readonly Dictionary<string, BudganErrorValue> ByCode = new();

    public static BudganErrorValue ResourceNotFound = new BudganErrorValue("ResourceNotFound", "Resource not found");
    public static BudganErrorValue Exception = new BudganErrorValue("ExceptionOccurred", "An exception occured : information in logs");
    public static BudganErrorValue DuplicateAccountTransaction = new BudganErrorValue("DuplicateAccountTransaction", "A transaction with this unique key already exists for this account");
    public static BudganErrorValue DuplicateAccountRecurringTransaction = new BudganErrorValue("DuplicateAccountRecurringTransaction", "A recurring transaction pattern with this id already exists");
    public static BudganErrorValue DuplicateTransactionsFile = new BudganErrorValue("DuplicateTransactionsFile", "A file with this name has already been imported for this account");
    public static BudganErrorValue DuplicateAccount = new BudganErrorValue("DuplicateAccount", "An account with this id already exists");
    public static BudganErrorValue DuplicateColumnsMapping = new BudganErrorValue("DuplicateColumnsMapping", "A columns mapping with this id already exists");

    public string ErrorCode { get; }
    public string ErrorMessage { get; }

    protected BudganErrorValue(string errorCode, string errorMessage)
    {
        this.ErrorCode = errorCode;
        this.ErrorMessage = errorMessage;

        ByCode.Add(errorCode, this);
    }

    public static BudganErrorValue FromCode(string errorCode)
    {
        return ByCode[errorCode];
    }
}