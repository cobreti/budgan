using System.IO.Abstractions;
using Ardalis.GuardClauses;
using Budgan.Options;
using Budgan.Services;
using CommandLine;

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
    public StringWriter HelpWriter { get; } = new();
    public Parser Parser { get; }
    
    public Application(string[] args)
    {
        Args = args;
        Builder = Host.CreateApplicationBuilder(args);
        
        Parser = new Parser(c =>
        {
            c.CaseSensitive = false;
            c.AutoVersion = false;
            c.AutoHelp = true;
            c.HelpWriter = HelpWriter;
        });
    }

    public bool Init()
    {
        Builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        Builder.Configuration.AddJsonFile($"appsettings.{Builder.Environment.EnvironmentName}.json", true, true);

        if (!SetOptionsFromCommandLineArgs())
            return false;
        
        Builder.Services
            .AddTransient<IFileSystem, FileSystem>()
            .AddScoped<ISourceLoader, SourceLoader>();
        
        Builder.Logging.AddConsole();

        App = Builder.Build();

        return true;
    }

    public void Run()
    {
        try
        {
            Guard.Against.Null(App, message: "no host application instance");
            
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

    public bool SetOptionsFromCommandLineArgs()
    {
        var ret = true;
        
        Parser.ParseArguments<CommandLineOptions>(Args)
            .WithParsed(c =>
            {
                Builder.Services.Configure<FoldersOptions>(options =>
                {
                    options.Source = c.Source;
                    options.Output = c.Output;
                });
            })
            .WithNotParsed(errors =>
            {
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }

                ret = false;
            });

        var helpText = HelpWriter.ToString();
        if (helpText.Length > 0)
        {
            Console.WriteLine(helpText);
        }

        return ret;
    }
}
