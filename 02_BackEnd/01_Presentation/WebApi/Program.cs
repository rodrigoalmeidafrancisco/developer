using WebApi.Configurations;

#region Configurações Builder

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.AddInitializer(); //Configurações da API
builder.AddAppInsights(); //Configurações do Application Insights
builder.AddSwagger(); //Configurações do Swagger
builder.AddAuthenticationCustom(); //Configurações de Autenticação e Autorização

#endregion Configurações Builder

#region Configurações APP

WebApplication app = builder.Build();


app.UseInitializer(); //Configurações da API
app.UseSwaggerInit(); //Configurações do Swagger
app.UseAppInsights(); //Configurações do Application Insights

#endregion Configurações APP


await app.RunAsync();
