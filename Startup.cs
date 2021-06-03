using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using MitreWebAPI.Controllers.Util;
using MitreWebAPI.Model.ChatBot;
using MitreWebAPI.Util;
using Microsoft.Extensions.Logging.Console;
using MitreWebAPI.Model.Documentos;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MitreWebAPI.Controllers;

namespace MitreWebAPI
{
    /// <summary>
    /// Classe Inicial da API
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Leitura das configurações da API
        /// </summary>
        public IConfiguration Configuration { get; }
     
        /// <summary>
        /// Método inicial de leitura das confihurações da API
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            SenderSMS.SMS_USER = Configuration.GetSection("ConfigAPIGeral").GetSection("sms_user").Value;
            SenderSMS.SMS_PWD = Configuration.GetSection("ConfigAPIGeral").GetSection("sms_pwd").Value; 

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddSingleton<IConfiguration>(Configuration);

            Action<InfoClienteConectado> infoClienteConectado = (opt =>
            {
                opt.cpf_cnpj = "";
                opt.token = "";
            });
            
            services.Configure(infoClienteConectado);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<InfoClienteConectado>>().Value);

            var tokenKey = Configuration.GetValue<string>("TokenKey");
            var key = Encoding.ASCII.GetBytes(tokenKey);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddSingleton<IJWTAuthenticationManager>(new JWTAuthenticationManager(tokenKey));

            services.AddControllers();
            
            services.AddControllers().AddNewtonsoftJson();

            services.AddMvc();

        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<EndRequestMiddleware>();

            app.UseHttpsRedirection();
            
            app.UseExceptionHandler("/error");

            app.UseRouting();

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
