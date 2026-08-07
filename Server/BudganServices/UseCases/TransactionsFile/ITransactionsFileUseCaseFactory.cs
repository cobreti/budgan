using BudganServices.UseCases.TransactionsFile.GetListByAccount;
using BudganServices.UseCases.TransactionsFile.Save;

namespace BudganServices.UseCases.TransactionsFile;

public interface ITransactionsFileUseCaseFactory
{
    ISaveTransactionsFileUseCase SaveUseCase(BOSaveTransactionsFile model);
    IGetListByAccountTransactionsFileUseCase GetListByAccountUseCase(Guid accountId);
}
