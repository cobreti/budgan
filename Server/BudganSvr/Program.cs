using BudganInfra;
using BudganServices;

if (Environment.GetEnvironmentVariable("WAIT_FOR_DEBUGGER") == "true")
{
    Console.WriteLine($"Waiting for debugger — PID {Environment.ProcessId}. Attach and then set WAIT_FOR_DEBUGGER=false or send SIGUSR1.");
    while (!System.Diagnostics.Debugger.IsAttached)
        Thread.Sleep(100);
    Console.WriteLine("Debugger attached.");
}

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "wwwroot",
});

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddBudganServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
        // apply automatic migrations only in debug
        //  use Migration bundles for production
    app.Services.ApplyMigrations();
    
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();

app.Run();
