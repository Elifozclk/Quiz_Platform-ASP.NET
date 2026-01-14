using System;

namespace QP_WEBPROJECT.vs2.Pages.Admin
{
    public partial class AdminLogout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Redirect("/Pages/User/Login.aspx");
        }
    }
}
