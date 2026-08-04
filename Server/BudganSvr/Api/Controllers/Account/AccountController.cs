using BudganGlobal.Errors;
using BudganGlobal.Errors.Exceptions;
using BudganServices.UseCases.Account;
using BudganServices.UseCases.Account.AddOrUpdate;
using BudganSvr.Api.Controllers.Account.Models;
using BudganSvr.Api.Types;
using Microsoft.AspNetCore.Mvc;

namespace BudganSvr.Api.Controllers.Account;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountUseCaseFactory _accountUseCaseFactory;

    public AccountController(IAccountUseCaseFactory accountUseCaseFactory)
    {
        this._accountUseCaseFactory = accountUseCaseFactory;
    }

    [HttpPost]
    [Route("AddOrUpdate")]
    public async Task<IActionResult> AddOrUpdate(AddOrUpdateAccount model)
    {
        try
        {
            var boModel = new BOAddOrUpdateAccount
            {
                Id = model.Id,
                Timestamp = model.Timestamp,
                Name = model.Name,
                ColumnsMappingId = model.ColumnsMappingId,
                AccountType = model.AccountType,
            };
            var addOrUpdateUseCase = this._accountUseCaseFactory.AddOrUpdateUseCase(boModel);

            await addOrUpdateUseCase.ExecuteAsync();

            var result = new ApiSuccessResult<Guid>(addOrUpdateUseCase.ResultValue);

            return Ok(result);
        }
        catch (BudganException ex)
        {
            if (ex.BudganError == BudganErrorValue.ResourceNotFound)
            {
                return this.NotFound();
            }

            throw;
        }
    }

    [HttpGet]
    [Route("List")]
    public async Task<IActionResult> ListAccount()
    {
        var listAccountUseCase = this._accountUseCaseFactory.ListAccountUseCase();

        await listAccountUseCase.ExecuteAsync();

        var model = listAccountUseCase.ResultValue
            .Select(x => new ListAccount
            {
                Id = x.Id,
                Name = x.Name,
                ColumnsMappingId = x.ColumnsMappingId,
                AccountType = x.AccountType,
            })
            .ToList();

        var result = new ApiSuccessResult<List<ListAccount>>(model);

        return this.Ok(result);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetAccount(Guid id)
    {
        var useCase = this._accountUseCaseFactory.GetAccountUseCase(id);

        await useCase.ExecuteAsync();

        if (!useCase.Succeeded)
        {
            return this.NotFound();
        }

        var r = useCase.ResultValue;
        var model = new GetAccount
        {
            Id = r.Id,
            Timestamp = r.Timestamp,
            Name = r.Name,
            ColumnsMappingId = r.ColumnsMappingId,
            AccountType = r.AccountType,
        };

        return this.Ok(new ApiSuccessResult<GetAccount>(model));
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        var useCase = this._accountUseCaseFactory.DeleteAccountUseCase(id);

        await useCase.ExecuteAsync();

        if (!useCase.Succeeded)
        {
            return this.NotFound();
        }

        return this.Ok(new ApiSuccessResult<Guid>(useCase.ResultValue));
    }
}
