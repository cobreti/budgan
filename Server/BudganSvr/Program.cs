using BudganInfra;
using BudganServices;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;

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

const string corsPolicyName = "BudganCorsPolicy";

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    const string schemeId = "Bearer";
    options.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste an Azure AD access token (JWT)."
    });
    options.AddSecurityRequirement(document =>
    {
        var schemeRef = new OpenApiSecuritySchemeReference(schemeId, document);
        return new OpenApiSecurityRequirement { { schemeRef, [] } };
    });
});
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddBudganServices();

builder.Services.AddAuthentication(Constants.Bearer)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
        // No .AllowCredentials(): Bearer-token API, not cookie-based — credentialed CORS isn't needed.
    });
});

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
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(corsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
