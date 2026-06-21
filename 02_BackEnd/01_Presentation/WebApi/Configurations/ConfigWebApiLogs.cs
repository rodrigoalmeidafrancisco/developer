using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Shared.Settings;
using System.Globalization;

namespace WebApi.Configurations
{
    public static class ConfigWebApiLogs
    {
        private const string CustomLogPropertyName = "CustomLog";
        private static readonly HashSet<string> ExcludedSqlSourceContexts =
        [
            "Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware",
            "Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware"
        ];

        public static void AddSerilog(this WebApplicationBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            var outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}";
            var cultureInfo = CultureInfo.GetCultureInfo("pt-BR");

            var sinkOptions = new MSSqlServerSinkOptions()
            {
                AutoCreateSqlTable = true,
                TableName = "Logs",
                SchemaName = "Serilog"
            };

            var columnOptions = new ColumnOptions();
            columnOptions.Store.Add(StandardColumn.LogEvent);
            columnOptions.Store.Add(StandardColumn.TraceId);
            columnOptions.Store.Remove(StandardColumn.Properties);
            columnOptions.Store.Remove(StandardColumn.MessageTemplate);
            columnOptions.Id.DataType = System.Data.SqlDbType.BigInt;

            builder.Host.UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", SettingApp.Application?.Name ?? builder.Environment.ApplicationName)
                    .Enrich.WithProperty("Environment", SettingApp.Application?._Environment ?? builder.Environment.EnvironmentName)
                    .WriteTo.Console(
                        outputTemplate: outputTemplate,
                        formatProvider: cultureInfo,
                        restrictedToMinimumLevel: LogEventLevel.Information);

                if (!string.IsNullOrWhiteSpace(SettingApp.ConnectionStrings?.Default))
                {
                    loggerConfiguration.WriteTo.Logger(sqlLoggerConfiguration => sqlLoggerConfiguration
                        .Filter.ByIncludingOnly(ShouldPersistToDatabase)
                        .WriteTo.MSSqlServer(
                            connectionString: SettingApp.ConnectionStrings.Default,
                            sinkOptions: sinkOptions,
                            columnOptions: columnOptions,
                            formatProvider: cultureInfo));
                }
            });

        }

        private static bool ShouldPersistToDatabase(LogEvent logEvent)
        {
            ArgumentNullException.ThrowIfNull(logEvent);

            if (logEvent.Properties.ContainsKey(CustomLogPropertyName))
            {
                return true;
            }

            if (logEvent.Level < LogEventLevel.Error)
            {
                return false;
            }

            if (logEvent.Properties.TryGetValue("SourceContext", out LogEventPropertyValue sourceContextValue) &&
                sourceContextValue is ScalarValue { Value: string sourceContext } &&
                ExcludedSqlSourceContexts.Contains(sourceContext))
            {
                return false;
            }

            return true;
        }

        public static void UseSerilog(this WebApplication app)
        {
            // Garante que a aplicação foi criada antes de configurar o pipeline HTTP.
            ArgumentNullException.ThrowIfNull(app);

            // Configura o middleware de logging de requisições do Serilog para registrar detalhes de cada chamada HTTP.
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondeu {StatusCode} em {Elapsed:0.0000} ms";
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("TraceIdentifier", httpContext.TraceIdentifier);
                };
            });
        }
    }
}
