using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace QP_WEBPROJECT.vs2.Pages.User
{
    public partial class ReactivateAccount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string confirm = Request.QueryString["confirm"];

                if (confirm == "yes")
                {
                    // Session'dan geçici bilgileri al
                    if (Session["TempUserId"] != null)
                    {
                        int userId = Convert.ToInt32(Session["TempUserId"]);
                        ReactivateUserAccount(userId);
                    }
                    else
                    {
                        ShowError("Oturum bilgisi bulunamadı. Lütfen tekrar giriş yapın.");
                    }
                }
                else
                {
                    // Kullanıcı "Hayır" dedi
                    ShowError("Hesap aktif edilmedi.");
                }
            }
        }

        private void ReactivateUserAccount(int userId)
        {
            try
            {
                string apiUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}/api/auth/reactivate/{userId}";

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(apiUrl);
                req.Method = "PUT";
                req.ContentType = "application/json";

                // Boş body gönder (PUT metodunda gerekli)
                using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                {
                    streamWriter.Write("{}");
                }

                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                using (var reader = new StreamReader(resp.GetResponseStream()))
                {
                    string resultJson = reader.ReadToEnd();
                    dynamic result = new JavaScriptSerializer().Deserialize<dynamic>(resultJson);

                    if (result["status"] == "success")
                    {
                        // ✅ Session'a kullanıcı bilgilerini kaydet (artık giriş yapmış sayılır)
                        Session["UserId"] = Session["TempUserId"];
                        Session["UserType"] = Session["TempUserRole"];
                        Session["UserName"] = Session["TempUserName"];
                        Session["UserEmail"] = Session["TempUserEmail"];

                        // Geçici session'ları temizle
                        Session.Remove("TempUserId");
                        Session.Remove("TempUserRole");
                        Session.Remove("TempUserName");
                        Session.Remove("TempUserEmail");

                        ShowSuccess($"Hoş geldiniz {Session["UserName"]}! Hesabınız başarıyla yeniden aktif edildi.");
                    }
                    else
                    {
                        ShowError(result["message"]);
                    }
                }
            }
            catch (WebException we)
            {
                if (we.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)we.Response)
                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        string errorText = reader.ReadToEnd();
                        ShowError("API Hatası: " + errorText);
                    }
                }
                else
                {
                    ShowError("Bağlantı hatası: " + we.Message);
                }
            }
            catch (Exception ex)
            {
                ShowError("Sunucu hatası: " + ex.Message);
            }
        }

        private void ShowSuccess(string message)
        {
            pnlLoading.Visible = false;
            pnlError.Visible = false;
            pnlSuccess.Visible = true;
            lblSuccessMessage.Text = message;
        }

        private void ShowError(string message)
        {
            pnlLoading.Visible = false;
            pnlSuccess.Visible = false;
            pnlError.Visible = true;
            lblErrorMessage.Text = message;
        }

        protected void btnGoToDashboard_Click(object sender, EventArgs e)
        {
            if (Session["UserType"] != null)
            {
                if (Session["UserType"].ToString() == "admin")
                    Response.Redirect("/Pages/Admin/AdminDashboard.aspx");
                else
                    Response.Redirect("/Pages/User/UserDashboard.aspx");
            }
            else
            {
                Response.Redirect("/Pages/User/Login.aspx");
            }
        }

        protected void btnGoToLogin_Click(object sender, EventArgs e)
        {
            // Tüm session'ları temizle
            Session.Clear();
            Response.Redirect("/Pages/User/Login.aspx");
        }
    }
}