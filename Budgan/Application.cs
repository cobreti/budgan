using System.IO.Abstractions;
using Ardalis.GuardClauses;
using Budgan.Services.CommandLineParsing;
using BudganEngine.Options;
using BudganEngine.Services.ConfigLoader;
using BudganEngine.Services.Implementations;
using BudganEngine.Services.Indexes;
using BudganEngine.Services.Interfaces;
using BudganEngine.Services.TransactionsProcessors;
using CsvHelper;
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

        Builder.Services
            .Configure<AppConfig>(Builder.Configuration.GetSection("Budgan"))
            .PostConfigure<AppConfig>(config =>
            {
                var dateValue = config.DateFormat ?? "YYYYMMDD"; 
                config.DateFormat = dateValue.Replace('Y', 'y').Replace('D', 'd');
            });

        Builder.Services
            .AddTransient<IFileSystem, FileSystem>()
            .AddScoped<ITransactionsLoader, TransactionsLoader>()
            .AddScoped<ITransactionsWriter, TransactionsWriter>()
            .AddScoped<ITransactionParser, TransactionParser>()
            .AddSingleton<ITransactionsRepository, TransactionsRepository>()
            .AddSingleton<ITransactionsContainerFactory, TransactionsContainerFactory>()
            .AddSingleton<ICommandLineParser, CommandLineParser>()
            .AddSingleton<IBankTransactionLayoutSettings, BankTransactionLayoutSettings>()
            .AddSingleton<IConfigLoaderFactory, ConfigLoaderFactory>()
            .AddSingleton<ITransactionsByDescription, TransactionsByDescription>()
            .AddTransient<IConfigSectionRepository, ConfigSectionRepository>()
            .AddTransient<ICsvReaderFactory, CsvReaderFactory>()
            // .AddTransient<ITextReaderFactory, TextReaderFactory>()
            .AddScoped<IConfigLoader, ConfigLoader>();

        Builder.Services
            .AddTransient<IIdentityTransactionsProcessor, IdentityTransactionsProcessor>();
        
        Builder.Logging.AddConsole();

        App = Builder.Build();

        return true;
    }

    public void Run()
    {
        try
        {
            Guard.Against.Null(App, message: "no host application instance");

            var config = App.Services.GetService<IOptions<AppConfig>>();
            
            var cmdlineParser = App.Services.GetService<ICommandLineParser>();
            Guard.Against.Null(cmdlineParser);

            cmdlineParser.Parse(Args);

            var index = App.Services.GetService<ITransactionsByDescription>();
            Guard.Against.Null(index);

            index.Build();

            Console.WriteLine("after index build");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
