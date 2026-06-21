using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.DataContracts;
using Shared.Settings;

namespace WebApi.Configurations
{
    public static class ConfigWebApiAppInsights
    {
        public static void AddAppInsights(this WebApplicationBuilder builder)
        {
            // Garante que o builder foi informado antes de configurar os serviços.
            ArgumentNullException.ThrowIfNull(builder);

            // Encerra o processamento caso a connection string do Application Insights não esteja definida.
            if (string.IsNullOrWhiteSpace(SettingApp.ApplicationInsights.ConnectionString))
            {
                return;
            }

            // Registra o Application Insights na aplicação.
            builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions()
            {
                // Define a versão da aplicação enviada nas telemetrias.
                ApplicationVersion = SettingApp.Application._Build,
                // Define a connection string usada para enviar dados ao Application Insights.
                ConnectionString = SettingApp.ApplicationInsights.ConnectionString,
            });
        }

        public static void UseAppInsights(this WebApplication app)
        {
            // Garante que a aplicação foi informada antes de usar os serviços configurados.
            ArgumentNullException.ThrowIfNull(app);

            // Encerra o processamento caso a connection string do Application Insights não esteja definida.
            if (string.IsNullOrWhiteSpace(SettingApp.ApplicationInsights.ConnectionString))
            {
                return;
            }

            // Obtém o cliente de telemetria registrado no container de injeção de dependência.
            var telemetry = app.Services.GetService<TelemetryClient>();

            // Encerra o processamento caso o cliente de telemetria não esteja disponível.
            if (telemetry is null)
            {
                return;
            }

            // Garante que a connection string esteja definida também no cliente de telemetria.
            telemetry.TelemetryConfiguration.ConnectionString ??= SettingApp.ApplicationInsights.ConnectionString;

            // Identifica a instância da máquina que está gerando a telemetria.
            telemetry.Context.Cloud.RoleInstance = Environment.MachineName;

            // Adiciona o nome da aplicação como propriedade global da telemetria.
            AddPropertyIfHasValue(telemetry.Context.GlobalProperties, "ApplicationName", SettingApp.Application.Name);

            // Adiciona o tipo da aplicação como propriedade global da telemetria.
            AddPropertyIfHasValue(telemetry.Context.GlobalProperties, "ApplicationType", SettingApp.Application.Type);

            // Adiciona o ambiente da aplicação como propriedade global da telemetria.
            AddPropertyIfHasValue(telemetry.Context.GlobalProperties, "Environment", SettingApp.Application._Environment);

            // Adiciona o identificador de build como propriedade global da telemetria.
            AddPropertyIfHasValue(telemetry.Context.GlobalProperties, "Build", SettingApp.Application._Build);

            // Adiciona a versão de release como propriedade global da telemetria.
            AddPropertyIfHasValue(telemetry.Context.GlobalProperties, "Release", SettingApp.Application._Release);

            // Adiciona a versão do .NET em execução como propriedade global da telemetria.
            AddPropertyIfHasValue(telemetry.Context.GlobalProperties, "DotNetVersion", Environment.Version.ToString());

            // Registra um evento quando a aplicação terminar o processo de inicialização.
            app.Lifetime.ApplicationStarted.Register(() =>
            {
                // Envia um trace informando que a aplicação foi iniciada com sucesso.
                telemetry.TrackTrace("Aplicação iniciada", SeverityLevel.Information);
            });
        }


        #region Métodos Privados

        private static void AddPropertyIfHasValue(IDictionary<string, string> globalProperties, string key, string value)
        {
            // Ignora a inclusão quando o valor da propriedade não foi informado.
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            // Adiciona a propriedade global apenas se ela ainda não existir.
            globalProperties.TryAdd(key, value);
        }

        #endregion Métodos Privados
    }
}
