# Introduction
<img src="Images/serilog.png" width="64" align="right" alt="Daikin.DotNetLib.Serilog Logo"/>
This is a .NET Standard 2.0 library to support Serilog logging, especially with Microsoft and Azure SQL Server.

The purpose to is to centralize the general repeatitive set up of Serilog for each application.

- Custom Fields
  1. EventId
  2. Application
  3. Version
  4. Client ID (specifically for IdentityServer)
  5. Remote IP (especially for ASP.NET)
  6. Environment (Production, Development, ...)
  7. User
  8. Source
  9. Session
  10. User Agent (browser)
  11. Request ID (especially for ASP.NET)
  12. Data (additional information)
- Add JSON data
- Eliminate XML data
- Enhanced Console support  

# Documentation Overview
Check out the primary [README.md](../README.md) documentation.

# Usage
These examples are based on Microsoft's ASP.NET.  These illustrations will hopefully assist in adapting to individual projects.

The programs referenced will need the following using statement:
```
using Daikin.DotNetLib.Serilog;
```

## Example 1
Modify **Program.cs** and update the *CreateHostBuilder* function:

```
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration
                // Other configurations excluded for brevity
                .WriteTo.DetailedMsSql(typeof(Program), hostingContext.Configuration);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

## Example 2 
Modify **Program.cs**, add the *Configuration* property, and update the *Main* function:
```
public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .Build();

public static void Main(string[] args)
{
    Log.Logger = new LoggerConfiguration()
        // Other configurations excluded for brevity
        .WriteTo.DetailedMsSql(typeof(Program), Configuration);
        .CreateLogger();
    try
    {
        BuildWebHost(args).Run();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Host terminated unexpectedly");
    }
    finally
    {
        Log.CloseAndFlush();
    }
}
```

## Example 3

Modify **Startup.cs** and update the *Configure* method:
```
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    var log = new LoggerConfiguration()
        // Other configurations excluded for brevity
        .WriteTo.DetailedMsSql(typeof(Startup), Configuration)
        .CreateLogger();
    loggerFactory.AddSerilog(log);
    Log.Logger = log;
}
```

~ end ~
