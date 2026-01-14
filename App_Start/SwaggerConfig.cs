using System.Web.Http;
using Swashbuckle.Application;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(QP_WEBPROJECT.vs2.App_Start.SwaggerConfig), "Register")]

namespace QP_WEBPROJECT.vs2.App_Start
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "QP_WEBPROJECT.vs2");
                })
                .EnableSwaggerUi();
        }
    }
}
