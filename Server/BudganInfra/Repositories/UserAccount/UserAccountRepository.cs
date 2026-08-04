using BudganInfra.DBContext;

namespace BudganInfra.Repositories.UserAccount;

internal class UserAccountRepository : IUserAccountRepository
{
    public UserAccountRepository(DataContext dbContext)
    {
    }
}