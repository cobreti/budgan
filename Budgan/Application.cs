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
    // public StringWriter HelpWriter { get; } = new();
    // public Parser Parser { get; }
    
    public Application(string[] args)
    {
        Args = args;
        Builder = Host.CreateApplicationBuilder(args);
        
        // Parser = new Parser(c =>
        // {
        //     c.CaseSensitive = false;
        //     c.AutoVersion = false;
        //     c.AutoHelp = true;
        //     c.HelpWriter = HelpWriter;
        // });
    }

    public bool Init()
    {
        Builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        Builder.Configuration.AddJsonFile($"appsettings.{Builder.Environment.EnvironmentName}.json", true, true);

        Builder.Services.Configure<CsvSettings>(Builder.Configuration.GetSection("CSV"));

        Builder.Services
            .AddTransient<IFileSystem, FileSystem>()
            .AddScoped<ISourceLoader, SourceLoader>()
            .AddSingleton<IState, State>();
        
        Builder.Logging.AddConsole();

        App = Builder.Build();

        return true;
    }

    public void Run()
    {
        try
        {
            Guard.Against.Null(App, message: "no host application instance");

            var stateService = App.Services.GetService<IState>();
            Guard.Against.Null(stateService, message: "unble to access application state");

            stateService.UpdateFromCommandLineArgs(Args);
            if (!stateService.Valid)
            {
                throw new Exception("Invalid state from command line");
            }
            
            var sourceLoader = App.Services.GetService<ISourceLoader>();
            Guard.Against.Null(sourceLoader, message: "unable to find SourceLoader service");
            
            sourceLoader.Load();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    // public bool UpdateStateFromCommandLineArgs()
    // {
    //     var ret = true;
    //     
    //     Parser.ParseArguments<CommandLineOptions>(Args)
    //         .WithParsed(c =>
    //         {
    //             // Builder.Services.Configure<FoldersOptions>(options =>
    //             // {
    //             //     options.Source = c.Source;
    //             //     options.Output = c.Output;
    //             // });
    //             // Builder.Services.Configure<LayoutOptions>(options =>
    //             // {
    //             //     options.Name = c.Layout;
    //             // });
    //         })
    //         .WithNotParsed(errors =>
    //         {
    //             foreach (var error in errors)
    //             {
    //                 Console.WriteLine(error);
    //             }
    //
    //             ret = false;
    //         });
    //
    //     var helpText = HelpWriter.ToString();
    //     if (helpText.Length > 0)
    //     {
    //         Console.WriteLine(helpText);
    //     }
    //
    //     return ret;
    // }
}
