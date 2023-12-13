using System.IO.Abstractions;
using Ardalis.GuardClauses;
using Budgan.Options;
using Budgan.Options.Runtime;
using Budgan.Services;
using CommandLine;
using Microsoft.Extensions.Options;

namespace Budgan;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Application
{
    public HostApplicationBuilder Builder { get; }
    public string[] Args { get; }
    
    public IHost? App { get; set; }
    
    public Application(string[] args)
    {
        Args = args;
        Builder = Host.CreateApplicationBuilder(args);
    }

    public bool Init()
    {
        Builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        Builder.Configuration.AddJsonFile($"appsettings.{Builder.Environment.EnvironmentName}.json", true, true);

        Builder.Services.Configure<CsvSettings>(Builder.Configuration.GetSection("CSV"));

        Builder.Services
            .AddTransient<IFileSystem, FileSystem>()
            .AddScoped<ITransactionsLoader, TransactionsLoader>()
            .AddScoped<ITransactionsWriter, TransactionsWriter>()
            .AddScoped<ITransactionParser, TransactionParser>()
            .AddSingleton<IState, State>()
            .AddSingleton<ITransactionsMgr, TransactionsesMgr>()
            .AddSingleton<ITransactionsContainerFactory, TransactionsContainerFactory>()
            .AddSingleton<ICommandLineParser, CommandLineParser>()
            .AddSingleton<IBankTransactionLayoutSettings, BankTransactionLayoutSettings>();
        
        Builder.Logging.AddConsole();

        App = Builder.Build();

        return true;
    }

    public void Run()
    {
        try
        {
            Guard.Against.Null(App, message: "no host application instance");

            var cmdlineParser = App.Services.GetService<ICommandLineParser>();
            Guard.Against.Null(cmdlineParser);

            cmdlineParser.Parse(Args);

            // var stateService = App.Services.GetService<IState>();
            // Guard.Against.Null(stateService, message: "unble to access application state");
            //
            // stateService.UpdateFromCommandLineArgs(Args);
            // if (!stateService.Valid)
            // {
            //     throw new Exception("Invalid state from command line");
            // }
            //
            // var transactionsLoader = App.Services.GetService<ITransactionsLoader>();
            // Guard.Against.Null(transactionsLoader, message: "unable to find SourceLoader service");
            //
            // transactionsLoader.Load();
            //
            // var transactionsWriter = App.Services.GetService<ITransactionsWriter>();
            // Guard.Against.Null(transactionsWriter, message: "unable to find TransactionsWriter service");
            //
            // var transactionsMgr = App.Services.GetService<ITransactionsMgr>();
            // Guard.Against.Null(transactionsMgr, "unable to get transactions mgr");
            //
            // transactionsWriter.Write("test.csv", transactionsMgr.GetAllTransactions());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
