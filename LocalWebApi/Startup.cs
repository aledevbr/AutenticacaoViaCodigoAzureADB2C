using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication; // * AddAzureADB2CBearer()
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI; // * AddAuthentication()
using Microsoft.AspNetCore.Authentication.JwtBearer; // * JwtBearerDefaults.AuthenticationScheme
using Microsoft.AspNetCore.Authorization; // * AuthorizationPolicyBuilder()
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LocalWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // * Adiciona a autenticação
            services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
                .AddAzureADB2CBearer(options =>
                {
                    // * Instância do Azure AD B2C
                    options.Instance = "https://seudominio.b2clogin.com/";
                    // * Domínio ou ID do locatário do Azure AD B2C
                    options.Domain = "seudominio.onmicrosoft.com";
                    // * Fluxo do usuário
                    options.SignUpSignInPolicyId = "B2C_1_post_ropc";
                    // * ID do aplicativo (cliente) => post-ApiWeb
                    options.ClientId = "e97f0bde-d845-4683-9717-cfc234c82659";
                });

            services.AddControllers();

            // * Ativa o token como forma de autorização de acesso aos recursos
            services.AddAuthorization(
                options =>
                {
                    options.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication(); // * Adiciona a autenticação
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
