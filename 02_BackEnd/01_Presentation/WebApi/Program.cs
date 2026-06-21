using Shared.Settings;
using WebApi.Configurations;

#region Configurações Builder

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Carrega e centraliza as configurações da API a partir dos arquivos de ambiente.
SettingApp.Start(builder.Configuration, builder.Environment.WebRootPath);

builder.AddSerilog(); //Configurações do Serilog
builder.AddInitializer(); //Configurações da API
builder.AddAppInsights(); //Configurações do Application Insights
builder.AddSwagger(); //Configurações do Swagger
builder.AddAuthenticationCustom(); //Configurações de Autenticação e Autorização

#endregion Configurações Builder

#region Configurações APP

WebApplication app = builder.Build();

app.UseSerilog(); //Configurações do Serilog
app.UseInitializer(); //Configurações da API
app.UseSwaggerInit(); //Configurações do Swagger
app.UseAppInsights(); //Configurações do Application Insights

#endregion Configurações APP


await app.RunAsync();
