using InfraRecordType = BudganInfra.DBContext.Tables.AccountTransactionRecordType;

namespace BudganServices.UseCases.AccountTransaction;

public static class AccountTransactionRecordTypeConverter
{
    public static AccountTransactionRecordType FromDao(InfraRecordType recordType)
    {
        return recordType == InfraRecordType.Snapshot
            ? AccountTransactionRecordType.Snapshot
            : AccountTransactionRecordType.Normal;
    }

    public static InfraRecordType ToDao(AccountTransactionRecordType recordType)
    {
        return recordType == AccountTransactionRecordType.Snapshot
            ? InfraRecordType.Snapshot
            : InfraRecordType.Normal;
    }
}
