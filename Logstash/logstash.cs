using Serilog;
using Serilog.Events;
using Serilog.Sinks.Http.BatchFormatters;


namespace WebAPI.Logstash
{
    public class logstash
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Program>();
        })
        .UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration
                .Enrich.FromLogContext()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Information)
                .WriteTo.Http(
                    requestUri: "http://localhost:5000",
                    batchFormatter: new ArrayBatchFormatter(),
                    textFormatter: new Serilog.Formatting.Json.JsonFormatter(),
                    restrictedToMinimumLevel: LogEventLevel.Information
                );
        });
    }
}
