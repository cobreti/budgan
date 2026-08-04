using BudganInfra.DBContext;
using BudganInfra.DBContext.Tables;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.Account.Save;

public class SaveAccountRepoOp : BaseRepositoryOperationWithResultValue<Guid>, ISaveAccountRepoOp
{
    private readonly DataContext _context;
    private readonly DaoSaveAccount _daoSaveAccount;

    public SaveAccountRepoOp(DataContext context, DaoSaveAccount daoSaveAccount)
    {
        this._context = context;
        this._daoSaveAccount = daoSaveAccount;
    }
    
    public async Task ExecuteAsync()
    {
        if (this._daoSaveAccount.Id == null)
        {
            await this.Add();
        }
        else
        {
            await this.Update();
        }
    }

    private async Task Add()
    {
        var id = Guid.CreateVersion7();
        var accountEntity = new DBContext.Tables.Account
        {
            Id = id,
            Name = this._daoSaveAccount.Name,
            AccountType = this._daoSaveAccount.AccountType,
            ColumnsMappingId = this._daoSaveAccount.ColumnsMappingId,
        };

        await _context.AddAsync(accountEntity);
        await _context.SaveChangesAsync();

        this.SetSucceeded(id);
    }

    private async Task Update()
    {
        ArgumentNullException.ThrowIfNull(this._daoSaveAccount.Id);

        var id = Guid.Parse(this._daoSaveAccount.Id);
        var entity = await this._context.Accounts.FirstOrDefaultAsync(x => x.Id == id);

        ValidateCanPerformUpdate(entity, this._daoSaveAccount);

        entity.Name = this._daoSaveAccount.Name;
        entity.AccountType = this._daoSaveAccount.AccountType;
        entity.ColumnsMappingId = this._daoSaveAccount.ColumnsMappingId;

        this._context.Update(entity);
        await _context.SaveChangesAsync();

        this.SetSucceeded(id);
    }
}