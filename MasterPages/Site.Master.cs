using Antlr.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QP_WEBPROJECT.vs2.MasterPages
{
    public partial class Site : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] != null)
                {
                    string role = Session["UserType"]?.ToString();

                    // Kullanıcı bilgisi
                    lblTopUserName.Text = Session["UserName"]?.ToString();

                    // Admin mi?
                    if (role == "admin")
                    {
                        // Adminse tüm navbarı gizle (isteğe bağlı)
                        phGuest.Visible = false;
                        phUser.Visible = false;
                    }
                    else
                    {
                        phGuest.Visible = false;
                        phUser.Visible = true;
                    }
                }
                else
                {
                    // Giriş yapılmamışsa sadece guest göster
                    phGuest.Visible = true;
                    phUser.Visible = false;
                }
            }
        }



    }
}