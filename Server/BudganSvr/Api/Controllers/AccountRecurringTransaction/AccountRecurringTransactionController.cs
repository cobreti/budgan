using BudganGlobal.Errors;
using BudganGlobal.Errors.Exceptions;
using BudganServices.UseCases.AccountRecurringTransaction;
using BudganServices.UseCases.AccountRecurringTransaction.ReplaceForAccount;
using BudganSvr.Api.Controllers.AccountRecurringTransaction.Models;
using BudganSvr.Api.Types;
using Microsoft.AspNetCore.Mvc;

namespace BudganSvr.Api.Controllers.AccountRecurringTransaction;

[ApiController]
[Route("api/[controller]")]
public class AccountRecurringTransactionController : ControllerBase
{
    private readonly IAccountRecurringTransactionUseCaseFactory _accountRecurringTransactionUseCaseFactory;

    public AccountRecurringTransactionController(IAccountRecurringTransactionUseCaseFactory accountRecurringTransactionUseCaseFactory)
    {
        this._accountRecurringTransactionUseCaseFactory = accountRecurringTransactionUseCaseFactory;
    }

    [HttpPut]
    [Route("Account/{accountId}")]
    public async Task<IActionResult> ReplaceForAccount(Guid accountId, [FromBody] List<AccountRecurringTransactionItem> items)
    {
        try
        {
            var boModel = new BOReplaceAccountRecurringTransactionsForAccount
            {
                AccountId = accountId,
                Items = items
                    .Select(x => new BOAccountRecurringTransactionItem
                    {
                        RecurringId = x.Id,
                        PeriodInDays = x.PeriodInDays,
                        TransactionCount = x.TransactionCount,
                        Description = x.Description,
                        AverageAmount = x.AverageAmount,
                        FirstOccurrenceDateAsString = x.FirstOccurrenceDate,
                        LastOccurrenceDateAsString = x.LastOccurrenceDate,
                    })
                    .ToList(),
            };

            var useCase = this._accountRecurringTransactionUseCaseFactory.ReplaceForAccountUseCase(boModel);

            await useCase.ExecuteAsync();

            return this.Ok(new ApiSuccessResult<bool>(true));
        }
        catch (BudganException ex)
        {
            if (ex.BudganError == BudganErrorValue.ResourceNotFound)
            {
                return this.NotFound();
            }

            if (ex.BudganError == BudganErrorValue.DuplicateAccountRecurringTransaction)
            {
                return this.Conflict(new ApiErrorResult<string>(ex.BudganError.ErrorMessage));
            }

            throw;
        }
    }
}
