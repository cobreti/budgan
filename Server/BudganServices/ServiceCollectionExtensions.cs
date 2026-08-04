using BudganServices.UseCases.Account;
using BudganServices.UseCases.AccountRecurringTransaction;
using BudganServices.UseCases.AccountTransaction;
using BudganServices.UseCases.ColumnsMapping;
using Microsoft.Extensions.DependencyInjection;

namespace BudganServices;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBudganServices(this IServiceCollection services)
    {
        services.AddScoped<IColumnsMappingUseCaseFactory, ColumnsMappingUseCaseFactory>();
        services.AddScoped<IAccountUseCaseFactory, AccountUseCaseFactory>();
        services.AddScoped<IAccountTransactionUseCaseFactory, AccountTransactionUseCaseFactory>();
        services.AddScoped<IAccountRecurringTransactionUseCaseFactory, AccountRecurringTransactionUseCaseFactory>();

        return services;
    }
}
