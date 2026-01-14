using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace QP_WEBPROJECT.vs2.Pages.User
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("/Pages/User/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadUserProfile();
            }
        }

        private void LoadUserProfile()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);

                // ✅ YENİ API ROUTE: /api/auth/user/{id} (eski: /api/auth/getuser/{id})
                string apiUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}/api/auth/user/{userId}";

                var request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = "GET";

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string json = reader.ReadToEnd();
                    var serializer = new JavaScriptSerializer();
                    var result = serializer.Deserialize<Dictionary<string, object>>(json);

                    if (result["status"].ToString() == "success")
                    {
                        // ✅ API response'da "data" objesi içinde dönüyor
                        var userData = result["data"] as Dictionary<string, object>;

                        lblName.Text = userData["Name"]?.ToString() ?? "";
                        lblEmail.Text = userData["Email"]?.ToString() ?? "";
                        lblDisplayName.Text = userData["DisplayName"]?.ToString() ?? "";
                        lblGender.Text = userData["Gender"]?.ToString() ?? "";
                        lblBirthday.Text = userData["Birthday"]?.ToString() ?? "";
                        lblLocation.Text = userData["Location"]?.ToString() ?? "";
                        lblBio.Text = userData["Bio"]?.ToString() ?? "";
                        lblInterests.Text = userData["Interests"]?.ToString() ?? "";

                        string profileImage = userData.ContainsKey("ProfileImage") && !string.IsNullOrEmpty(userData["ProfileImage"]?.ToString())
                            ? userData["ProfileImage"].ToString()
                            : "default-user.png";

                        imgProfile.ImageUrl = "~/Uploads/ProfileImages/" + profileImage;
                    }
                    else
                    {
                        lblError.Text = "Kullanıcı bilgileri alınamadı.";
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
            catch (Exception ex)
            {
                lblError.Text = "Profil bilgileri yüklenemedi: " + ex.Message;
                lblError.Visible = true;
            }
        }
    }
}