using System;

namespace QP_WEBPROJECT.vs2.Pages.User
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Clear();      // Tüm Session'ları temizle
            Session.Abandon();    // Oturumu sonlandır
            Response.Redirect("/Pages/Public/Default.aspx");
        }
    }
}
