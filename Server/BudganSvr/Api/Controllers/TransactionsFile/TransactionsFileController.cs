using BudganGlobal.Errors;
using BudganGlobal.Errors.Exceptions;
using BudganServices.UseCases.TransactionsFile;
using BudganSvr.Api.Controllers.TransactionsFile.Models;
using BudganSvr.Api.Types;
using Microsoft.AspNetCore.Mvc;

namespace BudganSvr.Api.Controllers.TransactionsFile;

[ApiController]
[Route("api/[controller]")]
public class TransactionsFileController : ControllerBase
{
    private readonly ITransactionsFileUseCaseFactory _transactionsFileUseCaseFactory;

    public TransactionsFileController(ITransactionsFileUseCaseFactory transactionsFileUseCaseFactory)
    {
        this._transactionsFileUseCaseFactory = transactionsFileUseCaseFactory;
    }

    [HttpPost]
    [Route("Save")]
    public async Task<IActionResult> Save(SaveTransactionsFile model)
    {
        try
        {
            var boModel = new BOSaveTransactionsFile
            {
                AccountId = model.AccountId,
                Content = model.Content,
                Filename = model.Filename,
                InsertionDate = model.InsertionDate,
            };

            var useCase = this._transactionsFileUseCaseFactory.SaveUseCase(boModel);

            await useCase.ExecuteAsync();

            var result = new ApiSuccessResult<Guid>(useCase.ResultValue);

            return Ok(result);
        }
        catch (BudganException ex)
        {
            if (ex.BudganError == BudganErrorValue.DuplicateTransactionsFile)
            {
                return this.Conflict(new ApiErrorResult<string>(ex.BudganError.ErrorMessage));
            }

            throw;
        }
    }

    [HttpGet]
    [Route("ListByAccount/{accountId}")]
    public async Task<IActionResult> ListByAccount(Guid accountId)
    {
        var useCase = this._transactionsFileUseCaseFactory.GetListByAccountUseCase(accountId);

        await useCase.ExecuteAsync();

        var model = useCase.ResultValue
            .Select(x => new Models.TransactionsFile
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    Content = x.Content,
                    Filename = x.Filename,
                    InsertionDate = x.InsertionDate,
                }
            )
            .ToList();

        return this.Ok(model);
    }

    [HttpGet]
    [Route("List")]
    public async Task<IActionResult> List()
    {
        var useCase = this._transactionsFileUseCaseFactory.GetListUseCase();

        await useCase.ExecuteAsync();

        var model = useCase.ResultValue
            .Select(x => new Models.TransactionsFile
                {
                    Id = x.Id,
                    AccountId = x.AccountId,
                    Content = x.Content,
                    Filename = x.Filename,
                    InsertionDate = x.InsertionDate,
                }
            )
            .ToList();

        return this.Ok(model);
    }
}
