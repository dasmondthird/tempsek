// SwaggerConfig.cs (Optional for Swagger UI)
using System.Web.Http;
using WebActivatorEx;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(Server.SwaggerConfig), "Register")]

namespace Server
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "SystemInfoServer API");
                })
                .EnableSwaggerUi();
        }
    }
}
