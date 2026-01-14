using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace QP_WEBPROJECT.vs2.Pages.User
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Kullanıcı zaten giriş yapmışsa yönlendir
            if (Session["UserId"] != null)
            {
                string role = Session["UserType"]?.ToString();
                if (role == "admin")
                    Response.Redirect("/Pages/Admin/AdminDashboard.aspx");
                else
                    Response.Redirect("/Pages/User/UserDashboard.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            System.Net.ServicePointManager.Expect100Continue = true;
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender2, certificate, chain, sslPolicyErrors) => true;

            try
            {
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text.Trim();

                string apiUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}/api/auth/login";

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(apiUrl);
                req.Method = "POST";
                req.ContentType = "application/json";

                var data = new { Email = email, Password = password };
                var jsonData = new JavaScriptSerializer().Serialize(data);

                using (var streamWriter = new StreamWriter(req.GetRequestStream()))
                {
                    streamWriter.Write(jsonData);
                }

                try
                {
                    using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                    using (var reader = new StreamReader(resp.GetResponseStream()))
                    {
                        string resultJson = reader.ReadToEnd();
                        dynamic result = new JavaScriptSerializer().Deserialize<dynamic>(resultJson);

                        // ✅ HESAP DEVRe DIŞI DURUMU
                        if (result["status"] == "inactive")
                        {
                            // Session'a geçici olarak kullanıcı bilgilerini kaydet (onay için)
                            Session["TempUserId"] = result["data"]["id"];
                            Session["TempUserName"] = result["data"]["name"];
                            Session["TempUserEmail"] = result["data"]["email"];
                            Session["TempUserRole"] = result["data"]["role"];

                            // JavaScript ile pop-up göster
                            string script = @"
                                <script type='text/javascript'>
                                    if(confirm('Hesabınız daha önce devre dışı bırakılmış. Tekrar aktif etmek ister misiniz?')) {
                                        window.location.href = '/Pages/User/ReactivateAccount.aspx?confirm=yes';
                                    } else {
                                        alert('Hesap aktif edilmedi. Giriş sayfasına yönlendiriliyorsunuz.');
                                        window.location.href = '/Pages/User/Login.aspx';
                                    }
                                </script>";

                            ClientScript.RegisterStartupScript(this.GetType(), "InactiveAccountPopup", script);
                            return;
                        }

                        // ✅ BAŞARILI GİRİŞ
                        if (result["status"] == "success")
                        {
                            Session["UserId"] = result["data"]["id"];
                            Session["UserType"] = result["data"]["role"];
                            Session["UserName"] = result["data"]["name"];
                            Session["UserEmail"] = result["data"]["email"];

                            if (result["data"]["role"] == "admin")
                                Response.Redirect("/Pages/Admin/AdminDashboard.aspx");
                            else
                                Response.Redirect("/Pages/User/UserDashboard.aspx");
                        }
                        else
                        {
                            lblError.Text = result["message"];
                            lblError.Visible = true;
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
                            lblError.Text = "API Hatası: " + errorText;
                            lblError.Visible = true;
                        }
                    }
                    else
                    {
                        lblError.Text = "Bağlantı hatası: " + we.Message;
                        lblError.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Giriş başarısız: " + ex.Message;
                lblError.Visible = true;
            }
        }
    }
}