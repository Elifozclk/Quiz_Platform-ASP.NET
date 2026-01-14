using System;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace QP_WEBPROJECT.vs2
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // ⚡ Web Optimization (bundle) tamamen kapalı — jquery hatası artık oluşmaz
            // System.Web.UI.ScriptManager.ScriptResourceMapping.RemoveDefinition("jquery");
            // ScriptManager.ScriptResourceMapping.AddDefinition("jquery",
            //     new System.Web.UI.ScriptResourceDefinition
            //     {
            //         Path = "https://code.jquery.com/jquery-3.7.1.min.js",
            //         DebugPath = "https://code.jquery.com/jquery-3.7.1.js",
            //         CdnSupportsSecureConnection = true
            //     });

            // 🌐 Web API rotaları
            GlobalConfiguration.Configure(QP_WEBPROJECT.vs2.WebApiConfig.Register);
            
            // 🌐 WebForms rotaları
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // 🚫 Bundling sistemini kapat (jquery hatasını tetikleyen sistem)
            // BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
