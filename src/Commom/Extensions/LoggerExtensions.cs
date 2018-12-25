using Common.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;

namespace Common.Extensions
{
    public static class LoggerExtensions
    {
        public static void UseSerilog(
            this IApplicationBuilder app,
            ILoggerFactory loggerFactory,
            IServiceConfiguration serviceConfiguration)
        {
            var settings = serviceConfiguration.SerilogSettings;
            if (string.IsNullOrEmpty(settings.Level))
            {
                throw new ArgumentException("Log level can not be empty.", nameof(settings.Level));
            }
            var level = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), settings.Level, true);
            loggerFactory.AddSerilog();
            var configuration = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Is(level);
            if (settings.ConsoleEnabled)
            {
                configuration.WriteTo.ColoredConsole(level);
            }
            if (!settings.ElkEnabled)
            {
                Log.Logger = configuration.CreateLogger();

                return;
            }
            if (string.IsNullOrEmpty(settings.ApiUrl))
            {
                throw new ArgumentException("ELK API URL can not be empty.", nameof(settings.ApiUrl));
            }
            Log.Logger = configuration
               .WriteTo.Elasticsearch(
                new ElasticsearchSinkOptions(new Uri(settings.ApiUrl))
                {
                    MinimumLogEventLevel = level,
                    AutoRegisterTemplate = true,
                    IndexFormat = string.IsNullOrEmpty(settings.IndexFormat) ?
                        "logstash-{0:yyyy.MM.dd}" :
                        settings.IndexFormat,
                    ModifyConnectionSettings = x =>
                        settings.UseBasicAuth ?
                        x.BasicAuthentication(settings.Username, settings.Password) :
                        x
                })
               .CreateLogger();
        }
    }
}