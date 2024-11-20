// Startup.cs
using System;
using System.Text;
using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Owin;
using System.Web.Http;
using Microsoft.Owin.Cors;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Web.Http.Owin;

[assembly: OwinStartup(typeof(Server.Startup))]

namespace Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Enable CORS for all origins, headers, and methods
            app.UseCors(CorsOptions.AllowAll);

            // Configure JWT Authentication
            var issuer = "yourdomain.com";
            var audience = "yourdomain.com";
            var secret = ConfigurationManager.AppSettings["JwtSecretKey"];
            var key = Convert.FromBase64String(secret);

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }
            });

            // Configure Web API
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}
