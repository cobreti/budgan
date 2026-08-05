using BudganGlobal.Errors;
using BudganGlobal.Errors.Exceptions;
using BudganInfra.DBContext.Tables;
using BudganInfra.Repositories.AccountTransaction.GetPageByAccount;
using BudganServices.UseCases.AccountTransaction;
using BudganServices.UseCases.AccountTransaction.Create;
using BudganServices.UseCases.AccountTransaction.SetSnapshot;
using BudganSvr.Api.Controllers.AccountTransaction.Models;
using BudganSvr.Api.Types;
using Microsoft.AspNetCore.Mvc;

namespace BudganSvr.Api.Controllers.AccountTransaction;

[ApiController]
[Route("api/[controller]")]
public class AccountTransactionController : ControllerBase
{
    private readonly IAccountTransactionUseCaseFactory _accountTransactionUseCaseFactory;

    public AccountTransactionController(IAccountTransactionUseCaseFactory accountTransactionUseCaseFactory)
    {
        this._accountTransactionUseCaseFactory = accountTransactionUseCaseFactory;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountTransaction model)
    {
        try
        {
            var boModel = new BOCreateAccountTransaction
            {
                AccountId = model.AccountId,
                FileId = model.FileId,
                CardNumber = model.CardNumber,
                DateInscriptionAsString = model.DateInscriptionAsString,
                Amount = model.Amount,
                Description = model.Description,
            };

            var useCase = this._accountTransactionUseCaseFactory.CreateUseCase(boModel);

            await useCase.ExecuteAsync();

            return this.Ok(new ApiSuccessResult<Guid>(useCase.ResultValue));
        }
        catch (BudganException ex)
        {
            if (ex.BudganError == BudganErrorValue.ResourceNotFound)
            {
                return this.NotFound();
            }

            if (ex.BudganError == BudganErrorValue.DuplicateAccountTransaction)
            {
                return this.Conflict(new ApiErrorResult<string>(ex.BudganError.ErrorMessage));
            }

            throw;
        }
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetAccountTransaction(Guid id)
    {
        var useCase = this._accountTransactionUseCaseFactory.GetUseCase(id);

        await useCase.ExecuteAsync();

        if (!useCase.Succeeded)
        {
            return this.NotFound();
        }

        var r = useCase.ResultValue;
        var model = new GetAccountTransaction
        {
            Id = r.Id,
            Timestamp = r.Timestamp,
            AccountId = r.AccountId,
            FileId = r.FileId,
            CardNumber = r.CardNumber,
            DateInscriptionAsString = r.DateInscriptionAsString,
            Amount = r.Amount,
            Balance = r.Balance,
            BalanceDateOffset = r.BalanceDateOffset,
            Description = r.Description,
            UniqueKey = r.UniqueKey,
            RecurringId = r.RecurringId,
            RecordType = ToRecordTypeString(r.RecordType),
        };

        return this.Ok(new ApiSuccessResult<GetAccountTransaction>(model));
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteAccountTransaction(Guid id)
    {
        var useCase = this._accountTransactionUseCaseFactory.DeleteUseCase(id);

        await useCase.ExecuteAsync();

        if (!useCase.Succeeded)
        {
            return this.NotFound();
        }

        return this.Ok(new ApiSuccessResult<Guid>(useCase.ResultValue));
    }

    [HttpGet]
    [Route("Account/{accountId}/List")]
    public async Task<IActionResult> ListByAccount(Guid accountId)
    {
        var useCase = this._accountTransactionUseCaseFactory.ListByAccountUseCase(accountId);

        await useCase.ExecuteAsync();

        var model = useCase.ResultValue
            .Select(x => new AccountTransactionListItem
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

        return this.Ok(new ApiSuccessResult<List<AccountTransactionListItem>>(model));
    }

    [HttpGet]
    [Route("Account/{accountId}/Count")]
    public async Task<IActionResult> GetCountByAccount(Guid accountId)
    {
        var useCase = this._accountTransactionUseCaseFactory.GetCountByAccountUseCase(accountId);

        await useCase.ExecuteAsync();

        return this.Ok(new ApiSuccessResult<int>(useCase.ResultValue!.Value));
    }

    [HttpGet]
    [Route("Account/{accountId}/Page")]
    public async Task<IActionResult> GetPageByAccount(
        Guid accountId,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? sortField,
        [FromQuery] string? sortDirection)
    {
        var useCase = this._accountTransactionUseCaseFactory.GetPageByAccountUseCase(
            accountId,
            page,
            pageSize,
            ParseSortField(sortField),
            ParseSortDirection(sortDirection));

        await useCase.ExecuteAsync();

        var r = useCase.ResultValue;
        var model = new GetPageAccountTransaction
        {
            TotalCount = r.TotalCount,
            Items = r.Items
                .Select(x => new AccountTransactionPageItem
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
                .ToList(),
        };

        return this.Ok(new ApiSuccessResult<GetPageAccountTransaction>(model));
    }

    [HttpGet]
    [Route("Account/{accountId}/Snapshot")]
    public async Task<IActionResult> GetSnapshot(Guid accountId)
    {
        var useCase = this._accountTransactionUseCaseFactory.GetSnapshotUseCase(accountId);

        await useCase.ExecuteAsync();

        if (!useCase.Succeeded)
        {
            return this.Ok(new ApiSuccessResult<GetAccountTransactionSnapshot?>(null));
        }

        var r = useCase.ResultValue;
        var model = new GetAccountTransactionSnapshot
        {
            Id = r.Id,
            Timestamp = r.Timestamp,
            AccountId = r.AccountId,
            FileId = r.FileId,
            CardNumber = r.CardNumber,
            DateInscriptionAsString = r.DateInscriptionAsString,
            Amount = r.Amount,
            Balance = r.Balance,
            BalanceDateOffset = r.BalanceDateOffset,
            Description = r.Description,
            UniqueKey = r.UniqueKey,
            RecurringId = r.RecurringId,
            RecordType = ToRecordTypeString(r.RecordType),
        };

        return this.Ok(new ApiSuccessResult<GetAccountTransactionSnapshot?>(model));
    }

    [HttpPut]
    [Route("Account/{accountId}/Snapshot")]
    public async Task<IActionResult> SetSnapshot(Guid accountId, SetAccountTransactionSnapshot model)
    {
        try
        {
            var boModel = new BOSetAccountTransactionSnapshot
            {
                AccountId = accountId,
                DateAsString = model.DateAsString,
                Amount = model.Amount,
            };

            var useCase = this._accountTransactionUseCaseFactory.SetSnapshotUseCase(boModel);

            await useCase.ExecuteAsync();

            return this.Ok(new ApiSuccessResult<Guid>(useCase.ResultValue));
        }
        catch (BudganException ex)
        {
            if (ex.BudganError == BudganErrorValue.ResourceNotFound)
            {
                return this.NotFound();
            }

            if (ex.BudganError == BudganErrorValue.DuplicateAccountTransaction)
            {
                return this.Conflict(new ApiErrorResult<string>(ex.BudganError.ErrorMessage));
            }

            throw;
        }
    }

    [HttpDelete]
    [Route("Account/{accountId}/Snapshot")]
    public async Task<IActionResult> DeleteSnapshot(Guid accountId)
    {
        var useCase = this._accountTransactionUseCaseFactory.DeleteSnapshotUseCase(accountId);

        await useCase.ExecuteAsync();

        return this.Ok(new ApiSuccessResult<bool>(true));
    }

    private static string ToRecordTypeString(AccountTransactionRecordType recordType)
    {
        return recordType == AccountTransactionRecordType.Snapshot ? "snapshot" : "normal";
    }

    private static AccountTransactionSortField ParseSortField(string? sortField)
    {
        return sortField?.ToLowerInvariant() switch
        {
            "cardnumber" => AccountTransactionSortField.CardNumber,
            "description" => AccountTransactionSortField.Description,
            "amount" => AccountTransactionSortField.Amount,
            _ => AccountTransactionSortField.DateInscription,
        };
    }

    private static SortDirection ParseSortDirection(string? sortDirection)
    {
        return sortDirection?.ToLowerInvariant() switch
        {
            "desc" => SortDirection.Descending,
            _ => SortDirection.Ascending,
        };
    }
}
