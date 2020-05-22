using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace logging.api.test
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //Log.Logger = new LoggerConfiguration()
            //.MinimumLevel.Debug()
            //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            //.Enrich.FromLogContext()
            //.WriteTo.Console(new ElasticsearchJsonFormatter(inlineFields: true))
            // .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200/"))
            // {
            //     AutoRegisterTemplate = true,
            //     IndexFormat = $"Eric-Logs-{DateTime.Now:yyyy-MM-dd}"

            // })
            //.CreateLogger();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            //.UseSerilog()
            .UseStartup<Startup>();
    }
}
