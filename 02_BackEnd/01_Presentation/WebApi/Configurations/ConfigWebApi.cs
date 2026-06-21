using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Logging;
using Shared.Commands._Base;
using Shared.Settings;
using System.IO.Compression;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApi.Configurations
{
    public static class ConfigWebApi
    {
        private const string AllowAllCorsPolicyName = "AllowAll";

        public static void AddInitializer(this WebApplicationBuilder builder)
        {
            // Garante que o builder foi informado antes de iniciar a configuração da aplicação.
            ArgumentNullException.ThrowIfNull(builder);

            // Habilita dados sensíveis de identidade apenas em desenvolvimento para facilitar o diagnóstico local.
            IdentityModelEventSource.ShowPII = builder.Environment.IsDevelopment();

            // Prepara a aplicação para funcionar corretamente quando hospedada com integração ao IIS.
            builder.WebHost.UseIISIntegration();

            // Aplica o proxy padrão do processo para todas as saídas HTTP quando essa opção estiver habilitada.
            if (SettingApp.Parameters.Proxy.Enable)
            {
                if (string.IsNullOrWhiteSpace(SettingApp.Parameters.Proxy.UrlPorta))
                {
                    throw new InvalidOperationException("A configuração do proxy está habilitada, mas a UrlPorta não foi informada.");
                }

                HttpClient.DefaultProxy = new WebProxy(new Uri(SettingApp.Parameters.Proxy.UrlPorta, UriKind.Absolute), true, SettingApp.Parameters.Proxy.ByPassArray)
                {
                    UseDefaultCredentials = false,
                    Credentials = CredentialCache.DefaultCredentials
                };
            }

            // Define o comportamento padrão de serialização JSON usado nas respostas dos controllers.
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                // Define a política de nomenclatura das propriedades JSON para camelCase, que é o padrão em APIs REST.
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                // Formata a saída JSON com indentação apenas em desenvolvimento para facilitar a leitura.
                options.JsonSerializerOptions.WriteIndented = builder.Environment.IsDevelopment();
                // Ignora referências cíclicas para evitar erros de serialização.
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            // Registra suporte à exploração de endpoints para recursos como Swagger e descoberta de APIs.
            builder.Services.AddEndpointsApiExplorer();

            // Cria uma política de CORS aberta para permitir chamadas de qualquer origem, método e cabeçalho.
            builder.Services.AddCors(options => options.AddPolicy(AllowAllCorsPolicyName, policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            // Desativa a resposta automática de model state inválido para permitir tratamento manual das validações.
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Habilita compactação das respostas para reduzir o tráfego enviado pela API.
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/json"]);
            });

            // Define o nível de compressão usado pelo Gzip para equilibrar desempenho e tamanho da resposta.
            builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

            // Registra cache distribuído em memória para compartilhar dados temporários entre componentes da aplicação.
            builder.Services.AddDistributedMemoryCache();
        }

        public static void UseInitializer(this WebApplication app)
        {
            // Garante que a aplicação foi criada antes de configurar o pipeline HTTP.
            ArgumentNullException.ThrowIfNull(app);

            // Redireciona requisições HTTP para HTTPS para reforçar a comunicação segura.
            app.UseHttpsRedirection();

            // Habilita a resolução da página padrão e a publicação de arquivos estáticos da pasta wwwroot.
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Ativa a compactação das respostas usando a configuração registrada nos serviços.
            app.UseResponseCompression();

            // Em desenvolvimento exibe detalhes completos de erro; nos demais ambientes força políticas HTTP mais seguras.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Inicializa o roteamento para localizar o endpoint correto de cada requisição.
            app.UseRouting();

            // Aplica a política de CORS configurada para liberar o acesso dos clientes permitidos.
            app.UseCors(AllowAllCorsPolicyName);

            // Executa a autenticação da requisição para identificar o usuário atual.
            app.UseAuthentication(); // Autenticação

            // Valida autorização e perfis antes de permitir acesso aos endpoints protegidos.
            app.UseAuthorization(); // Roles

            // Mapeia os controllers da API como endpoints HTTP disponíveis.
            app.MapControllers();

            // Retorna uma resposta padronizada quando nenhuma rota configurada corresponder à requisição recebida.
            app.MapFallback(static async context =>
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new CommandResult<string>(404, "Rota não encontrada", false, $"O caminho '{context.Request.Path}' não corresponde a nenhum endpoint válido.", null, null));
            });
        }

    }
}
