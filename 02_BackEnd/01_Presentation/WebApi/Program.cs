using Shared.Settings;
using WebApi.Configurations;

#region Configurações Builder

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//Obtendo as configurações da API "appsettings"
SettingApp.Start(builder.Configuration, builder.Environment.WebRootPath);

//Configurações da API
builder.AddInitializer();

//Configurações do Swagger
builder.AddSwagger();

#endregion Configurações Builder

#region Configurações APP

WebApplication app = builder.Build();

//Configurações da API
app.UseInitializer();

//Configurações do Swagger
app.UseSwaggerInit();

#endregion Configurações APP


await app.RunAsync();
