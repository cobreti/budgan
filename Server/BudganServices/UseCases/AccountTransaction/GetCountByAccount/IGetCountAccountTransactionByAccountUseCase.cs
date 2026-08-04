namespace BudganServices.UseCases.AccountTransaction.GetCountByAccount;

// Uses int? rather than int: BaseUseCaseWithResultValue<T>.ResultValue throws if the stored
// value equals default(T), and a fresh account with zero transactions is a valid, common result.
public interface IGetCountAccountTransactionByAccountUseCase : IBaseUseCaseWithResultValue<int?>
{
}
