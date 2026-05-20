using Microsoft.OpenApi;
using Shared.Settings;
using System.Reflection;

namespace WebApi.Configurations
{
    public static class ConfigWebApiSwagger
    {
        extension(WebApplicationBuilder builder)
        {
            public void AddSwagger()
            {
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = SettingApp.Application.Name,
                        Version = "v1",
                        Description = "Documentação da API"
                    });

                    // Define o esquema de segurança Bearer JWT
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "Insira o token JWT desta forma: Bearer {seu token}",
                        Name = "Authorization",
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http
                    });

                    // Aplica o requisito de segurança Bearer globalmente em todos os endpoints
                    options.AddSecurityRequirement(doc =>
                    {
                        var requirement = new OpenApiSecurityRequirement();
                        requirement.Add(new OpenApiSecuritySchemeReference("Bearer", doc), []);
                        return requirement;
                    });

                    // Inclui comentários XML para documentação dos endpoints, se disponível
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                        options.IncludeXmlComments(xmlPath);

                    // Evita conflito de nomes em tipos aninhados (e.g., Commands.Create vs Queries.Create)
                    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
                });
            }
        }

        extension(WebApplication app)
        {
            public void UseSwaggerInit()
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("v1/swagger.json", SettingApp.Application.Name);
                    options.DocumentTitle = SettingApp.Application.Name;
                    options.DefaultModelsExpandDepth(-1);
                });
            }
        }
    }
}
