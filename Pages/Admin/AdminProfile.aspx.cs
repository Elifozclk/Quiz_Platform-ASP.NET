using System;

namespace QP_WEBPROJECT.vs2.Pages.Admin
{
    public partial class AdminProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] == null || Session["UserType"].ToString() != "admin")
                {
                    Response.Redirect("/Pages/User/Login.aspx");
                    return;
                }

                lblUserName.Text = Session["UserName"].ToString();
                lblEmail.Text = Session["UserEmail"].ToString();
                lblRole.Text = Session["UserType"].ToString();
            }
        }
    }
}
