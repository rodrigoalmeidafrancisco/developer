using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shared.Commands._Base;
using Shared.Settings;
using System.Text;

namespace WebApi.Configurations
{
    public static class ConfigWebApiAuthentication
    {
        public static void AddAuthenticationCustom(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Authority = $"Developer_{SettingApp.Application._Environment.ToUpper()}";
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes($"{SettingApp.Application._Environment.ToUpper()}{SettingApp.Parameters.KeyToken}")),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                x.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsJsonAsync(new CommandResult<string>(401, "Não Autorizado", false, "Acesso não autorizado!", null, null));
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsJsonAsync(new CommandResult<string>(403, "Acesso Proibido", false, "Acesso proibido!", null, null));
                    }
                };
            });

            //Configuração dos "scope" de acesso e "apolicy" na aplicação
            if (SettingApp.Application.Policys != null && SettingApp.Application.Policys.Count != 0)
            {
                builder.Services.AddAuthorization(options =>
                {
                    SettingApp.Application.Policys.ForEach(item => { options.AddPolicy(item.Name, policy => { policy.RequireClaim("scope", item.Scopes); }); });
                });
            }
        }
    }
}
