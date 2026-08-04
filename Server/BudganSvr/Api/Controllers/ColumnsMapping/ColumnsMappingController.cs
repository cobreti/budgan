using BudganGlobal.Errors;
using BudganGlobal.Errors.Exceptions;
using BudganServices.UseCases.ColumnsMapping;
using BudganServices.UseCases.ColumnsMapping.AddOrUpdate;
using BudganSvr.Api.Controllers.ColumnsMapping.Models;
using BudganSvr.Api.Types;
using Microsoft.AspNetCore.Mvc;

namespace BudganSvr.Api.Controllers.ColumnsMapping;

[ApiController]
[Route("api/[controller]")]
public class ColumnsMappingController : ControllerBase
{
    private readonly IColumnsMappingUseCaseFactory _columnsMappingUseCaseFactory;

    public ColumnsMappingController(IColumnsMappingUseCaseFactory columnsMappingUseCaseFactory)
    {
        this._columnsMappingUseCaseFactory = columnsMappingUseCaseFactory;
    }
    
    [HttpPost]
    [Route("AddOrUpdate")]
    public async Task<IActionResult> AddOrUpdate(AddOrUpdateColumnsMapping model)
    {
        try
        {
            var boModel = new BOAddOrUpdateColumnsMapping
            {
                Id = model.Id,
                Name = model.Name,
                CardNumberColumnIndex = model.CardNumberColumnIndex,
                CardNumberColumnText = model.CardNumberColumnText,
                DateInscriptionColumnIndex = model.DateInscriptionColumnIndex,
                DateInscriptionColumnText = model.DateInscriptionColumnText,
                AmountColumnIndex = model.AmountColumnIndex,
                AmountColumnText = model.AmountColumnText,
                DescriptionColumnIndex = model.DescriptionColumnIndex,
                DescriptionColumnText = model.DescriptionColumnText,
            };
            var addOrUpdateUseCase = this._columnsMappingUseCaseFactory.AddOrUpdateUseCase(boModel);

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
    public async Task<IActionResult> ListColumnsMapping()
    {
        var listColumnsMappingUseCase = this._columnsMappingUseCaseFactory.ListColumnsMappingUseCase();

        await listColumnsMappingUseCase.ExecuteAsync();

        var model = listColumnsMappingUseCase.ResultValue
            .Select(x => new ListColumnsMapping
                {
                    Id = x.Id,
                    Name = x.Name,
                    CardNumberColumnIndex = x.CardNumberColumnIndex,
                    CardNumberColumnText = x.CardNumberColumnText,
                    DateInscriptionColumnIndex = x.DateInscriptionColumnIndex,
                    DateInscriptionColumnText = x.DateInscriptionColumnText,
                    AmountColumnIndex = x.AmountColumnIndex,
                    AmountColumnText = x.AmountColumnText,
                    DescriptionColumnIndex = x.DescriptionColumnIndex,
                    DescriptionColumnText = x.DescriptionColumnText,
                }
            )
            .ToList();

        return this.Ok(model);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetColumnsMapping(Guid id)
    {
        var useCase = this._columnsMappingUseCaseFactory.GetColumnsMappingUseCase(id);

        await useCase.ExecuteAsync();

        if (!useCase.Succeeded)
        {
            return this.NotFound();
        }

        var r = useCase.ResultValue;
        var model = new GetColumnsMapping
        {
            Id = r.Id,
            Name = r.Name,
            CardNumberColumnIndex = r.CardNumberColumnIndex,
            CardNumberColumnText = r.CardNumberColumnText,
            DateInscriptionColumnIndex = r.DateInscriptionColumnIndex,
            DateInscriptionColumnText = r.DateInscriptionColumnText,
            AmountColumnIndex = r.AmountColumnIndex,
            AmountColumnText = r.AmountColumnText,
            DescriptionColumnIndex = r.DescriptionColumnIndex,
            DescriptionColumnText = r.DescriptionColumnText
        };
        
        return this.Ok(model);
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteColumnsMapping(Guid id)
    {
        var useCase = this._columnsMappingUseCaseFactory.DeleteColumnsMappingUseCase(id);

        await useCase.ExecuteAsync();

        if (!useCase.Succeeded)
        {
            return this.NotFound();
        }

        return this.Ok(useCase.ResultValue);
    }
}
