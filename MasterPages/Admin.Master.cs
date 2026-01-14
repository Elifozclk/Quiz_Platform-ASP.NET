using System;

namespace QP_WEBPROJECT.vs2.MasterPages
{
    public partial class Admin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Admin kontrolü - UserType kullan
            if (Session["UserType"] == null || Session["UserType"].ToString() != "admin")
            {
                Response.Redirect("/Pages/User/Login.aspx");
            }
        }
    }
}