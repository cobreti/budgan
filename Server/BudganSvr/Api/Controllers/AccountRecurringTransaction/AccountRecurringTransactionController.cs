using System.Globalization;
using BudganGlobal.Errors;
using BudganGlobal.Errors.Exceptions;
using BudganServices.UseCases.AccountTransaction;
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

    [HttpGet]
    [Route("Account/{accountId}/Transactions")]
    public async Task<IActionResult> GetTransactionsByAccount(
        Guid accountId,
        [FromQuery] string startDate,
        [FromQuery] string endDate)
    {
        var start = DateOnly.ParseExact(startDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        var end = DateOnly.ParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var useCase = this._accountRecurringTransactionUseCaseFactory.GetTransactionsByAccountUseCase(accountId, start, end);

        await useCase.ExecuteAsync();

        var model = useCase.ResultValue
            .Select(x => new AccountRecurringTransactionTransactionItem
            {
                Id = x.Id,
                AccountId = x.AccountId,
                FileId = x.FileId,
                CardNumber = x.CardNumber,
                DateInscriptionAsString = x.DateInscriptionAsString,
                Amount = x.Amount,
                Balance = x.Balance,
                BalanceDateOffset = x.BalanceDateOffset,
                Description = x.Description,
                UniqueKey = x.UniqueKey,
                RecurringId = x.RecurringId,
                RecordType = ToRecordTypeString(x.RecordType),
            })
            .ToList();

        return this.Ok(new ApiSuccessResult<List<AccountRecurringTransactionTransactionItem>>(model));
    }

    [HttpGet]
    [Route("Account/{accountId}/Span")]
    public async Task<IActionResult> GetSpan(Guid accountId)
    {
        var useCase = this._accountRecurringTransactionUseCaseFactory.GetSpanUseCase(accountId);

        await useCase.ExecuteAsync();

        if (!useCase.Succeeded)
        {
            return this.Ok(new ApiSuccessResult<GetRecurringTransactionsSpan?>(null));
        }

        var r = useCase.ResultValue;
        var model = new GetRecurringTransactionsSpan
        {
            Start = r.StartAsString,
            End = r.EndAsString,
        };

        return this.Ok(new ApiSuccessResult<GetRecurringTransactionsSpan?>(model));
    }

    [HttpGet]
    [Route("Account/{accountId}/List")]
    public async Task<IActionResult> ListByAccount(Guid accountId)
    {
        var useCase = this._accountRecurringTransactionUseCaseFactory.ListByAccountUseCase(accountId);

        await useCase.ExecuteAsync();

        var model = useCase.ResultValue
            .Select(x => new AccountRecurringTransactionItem
            {
                Id = x.RecurringId,
                PeriodInDays = x.PeriodInDays,
                TransactionCount = x.TransactionCount,
                Description = x.Description,
                AverageAmount = x.AverageAmount,
                FirstOccurrenceDate = x.FirstOccurrenceDateAsString,
                LastOccurrenceDate = x.LastOccurrenceDateAsString,
            })
            .ToList();

        return this.Ok(new ApiSuccessResult<List<AccountRecurringTransactionItem>>(model));
    }

    [HttpGet]
    [Route("List")]
    public async Task<IActionResult> List()
    {
        var useCase = this._accountRecurringTransactionUseCaseFactory.ListUseCase();

        await useCase.ExecuteAsync();

        var model = useCase.ResultValue
            .Select(x => new AccountRecurringTransactionListItem
            {
                Id = x.RecurringId,
                AccountId = x.AccountId,
                PeriodInDays = x.PeriodInDays,
                TransactionCount = x.TransactionCount,
                Description = x.Description,
                AverageAmount = x.AverageAmount,
                FirstOccurrenceDate = x.FirstOccurrenceDateAsString,
                LastOccurrenceDate = x.LastOccurrenceDateAsString,
            })
            .ToList();

        return this.Ok(new ApiSuccessResult<List<AccountRecurringTransactionListItem>>(model));
    }

    [HttpDelete]
    [Route("Account/{accountId}")]
    public async Task<IActionResult> DeleteByAccount(Guid accountId)
    {
        var useCase = this._accountRecurringTransactionUseCaseFactory.DeleteByAccountUseCase(accountId);

        await useCase.ExecuteAsync();

        return this.Ok(new ApiSuccessResult<bool>(true));
    }

    private static string ToRecordTypeString(AccountTransactionRecordType recordType)
    {
        return recordType == AccountTransactionRecordType.Snapshot ? "snapshot" : "normal";
    }
}
