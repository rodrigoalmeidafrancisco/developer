using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Logging;
using Shared.Commands._Base;
using Shared.Settings;
using System.Net;
using System.Text.Json;

namespace WebApi.Configurations
{
    public static class ConfigWebApi
    {
        public static void AddInitializer(this WebApplicationBuilder builder)
        {
            //habilitar a visualização de logs de PII
            IdentityModelEventSource.ShowPII = true;

            //Configura para utilizar o IIS, quando publicar.
            builder.WebHost.UseIISIntegration();

            //Configura para exibir os logs no console ao debugar a aplicação.
            builder.Logging.ClearProviders().AddConsole();

            //Obtendo as configurações da API "appsettings"
            SettingApp.Start(builder.Configuration, builder.Environment.WebRootPath);

            //Configurando o proxy
            if (SettingApp.Parameters.Proxy.Enable)
            {
                HttpClient.DefaultProxy = new WebProxy(new Uri(SettingApp.Parameters.Proxy.UrlPorta), true, SettingApp.Parameters.Proxy.ByPassArray)
                {
                    UseDefaultCredentials = false,
                    Credentials = CredentialCache.DefaultCredentials
                };
            }

            //Configura os parâmetros do System.Text.Json para o Retorno da API   
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = true;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCors(x => x.AddPolicy("AllowAll", y => { y.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));

            //Permite fazer a validação do ComponentModel.Annotations
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //Comprime o Json no Retorno da API, diminuindo o seu tamanho
            builder.Services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/json"]);
            });

            //Configuração para que o IMemoryCache seja distribuido entre os servidores no balance. 
            builder.Services.AddDistributedMemoryCache();
        }

        public static void UseInitializer(this WebApplication app)
        {
            //Informo que irei utilizar arquivos estáticos (wwwroot)
            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //Padrão de rotas do MVC
            app.UseRouting();
            app.MapControllers();

            //Força a API responder apenas em HTTPS
            app.UseHttpsRedirection();

            //Poder realizar chamadas localhost em tempo de desenvolvimento
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication(); // Autenticação
            app.UseAuthorization(); // Roles

            //Configura o Response
            app.MapFallback(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new CommandResult<string>(404, "Rota não encontrada", false, $"O caminho '{context.Request.Path}' não corresponde a nenhum endpoint válido.", null, null));
            });
        }

    }
}
