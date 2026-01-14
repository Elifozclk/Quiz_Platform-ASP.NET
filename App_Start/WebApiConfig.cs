using System.Web.Http;

namespace QP_WEBPROJECT.vs2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // ✅ ÖNCE: Attribute route'ları (ÖNCELİK 1)
            // [Route("api/auth/login")] gibi attribute'lar için
            config.MapHttpAttributeRoutes();

            // ✅ İKİNCİ: Attribute route'lu controller'lar için (QuizController)
            config.Routes.MapHttpRoute(
                name: "AttributeApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // ✅ ÜÇÜNCÜ: Action bazlı route (eski API'ler için FALLBACK)
            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}