using System;
using System.Net;
using System.Web.Script.Serialization;

namespace QP_WEBPROJECT.vs2.Pages.User
{
    public partial class UserQuizHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] != null)
                {
                    int userId = Convert.ToInt32(Session["UserId"]);
                    LoadQuizHistory(userId);
                }
                else
                {
                    Response.Redirect("/Pages/User/Login.aspx");
                }
            }
        }

        private void LoadQuizHistory(int userId)
        {
            string apiUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}/api/user/submissions/{userId}";

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                string response = client.DownloadString(apiUrl);

                System.Diagnostics.Debug.WriteLine(response); // JSON çıktısını göreceğiz

                dynamic json = new JavaScriptSerializer().Deserialize<dynamic>(response);
                if (json["status"] == "success")
                {
                    var data = json["data"];
                    rptQuizzes.DataSource = data;
                    rptQuizzes.DataBind();
                }
            }
        }

    }
}
