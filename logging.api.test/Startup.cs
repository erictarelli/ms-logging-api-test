﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.Elasticsearch;

namespace logging.api.test
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddLogging(loggingBuilder =>
            {
                var elasticSearchNodeUri = Configuration.GetValue<string>("LoggingOptions:NodeUri");

                // 1- Crear configuracion
                var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticSearchNodeUri))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat =$"Eric-Logs-{DateTime.Now:yyyy-MM-dd}"
                    
                })
                .WriteTo.Console(
                    outputTemplate:"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] - {Message} {Properties} {NewLine}"
                    )
                .Enrich.With<CustomEnricher>();
                // 2- Crear logger
                var logger = loggerConfiguration.CreateLogger();
                // 3- Inyectar el servicio
                loggingBuilder.Services.AddSingleton<ILoggerFactory>(
                    provider => new SerilogLoggerFactory(logger, dispose: false));

            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}