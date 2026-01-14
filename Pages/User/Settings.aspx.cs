using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using MySql.Data.MySqlClient;

namespace QP_WEBPROJECT.vs2.Pages.User
{
    public partial class Settings : System.Web.UI.Page
    {
        // ========================================
        // PROFIL RESMİ KURALLARI
        // ========================================
        private const int MAX_FILE_SIZE_MB = 5;
        private const int MAX_FILE_SIZE_BYTES = MAX_FILE_SIZE_MB * 1024 * 1024; // 5 MB
        private const int MIN_IMAGE_WIDTH = 200;
        private const int MIN_IMAGE_HEIGHT = 200;
        private const int MAX_IMAGE_WIDTH = 2000;
        private const int MAX_IMAGE_HEIGHT = 2000;
        private readonly string[] ALLOWED_EXTENSIONS = { ".jpg", ".jpeg", ".png" };

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
                string apiUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}/api/auth/user/{userId}";

                var client = (HttpWebRequest)WebRequest.Create(apiUrl);
                client.Method = "GET";

                using (var response = (HttpWebResponse)client.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    dynamic json = new JavaScriptSerializer().Deserialize<dynamic>(result);

                    if (json["status"] == "success")
                    {
                        var userData = json["data"];

                        txtName.Text = userData["Name"];
                        txtEmail.Text = userData["Email"];
                        txtDisplayName.Text = userData["DisplayName"];
                        ddlGender.SelectedValue = userData["Gender"] ?? "";
                        txtBirthday.Text = userData["Birthday"];
                        txtLocation.Text = userData["Location"];
                        txtBio.Text = userData["Bio"];
                        txtInterests.Text = userData["Interests"];

                        // Profil resmi
                        string profileImage = userData["ProfileImage"];
                        imgProfile.ImageUrl = string.IsNullOrEmpty(profileImage)
                            ? "~/Uploads/ProfileImages/default-user.png"
                            : "~/Uploads/ProfileImages/" + profileImage;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Kullanıcı bilgileri yüklenemedi: " + ex.Message, "danger");
            }
        }

        /// <summary>
        /// Profil resmi yükleme - VALIDATION İLE
        /// </summary>
        protected void btnUploadImage_Click(object sender, EventArgs e)
        {
            try
            {
                if (!fuProfileImage.HasFile)
                {
                    ShowMessage("❌ Lütfen bir dosya seçin.", "warning");
                    return;
                }

                // ✅ PROFIL RESMİ DOĞRULAMA
                var validationResult = ValidateProfileImage(fuProfileImage);

                if (!validationResult.IsValid)
                {
                    ShowMessage($"❌ Profil Resmi Hatası: {validationResult.ErrorMessage}", "danger");
                    return;
                }

                // Dosya kaydetme
                string fileName = Guid.NewGuid() + Path.GetExtension(fuProfileImage.FileName);
                string savePath = Server.MapPath("~/Uploads/ProfileImages/" + fileName);

                // Klasör yoksa oluştur
                string folder = Server.MapPath("~/Uploads/ProfileImages/");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                fuProfileImage.SaveAs(savePath);

                // Veritabanını güncelle
                int userId = Convert.ToInt32(Session["UserId"]);
                string sql = "UPDATE Users SET ProfileImage=@img WHERE Id=@id";
                DbHelper.Execute(sql,
                    new MySqlParameter("@img", fileName),
                    new MySqlParameter("@id", userId));

                // Resmi güncelle
                imgProfile.ImageUrl = "~/Uploads/ProfileImages/" + fileName + "?v=" + DateTime.Now.Ticks;

                ShowMessage("✅ Profil resmi başarıyla güncellendi!", "success");

                System.Diagnostics.Debug.WriteLine($"✅ Profil resmi güncellendi: {fileName}");
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ Resim yüklenirken hata oluştu: {ex.Message}", "danger");
                System.Diagnostics.Debug.WriteLine($"Upload Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Varsayılan profil resmini kullan
        /// </summary>
        protected void btnUseDefaultProfileImage_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);

                string sql = "UPDATE Users SET ProfileImage='default-user.png' WHERE Id=@id";
                DbHelper.Execute(sql, new MySqlParameter("@id", userId));

                imgProfile.ImageUrl = "~/Uploads/ProfileImages/default-user.png?v=" + DateTime.Now.Ticks;

                ShowMessage("✅ Varsayılan profil resmi ayarlandı.", "success");
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ Hata: {ex.Message}", "danger");
            }
        }

        /// <summary>
        /// Bilgileri güncelle - PUT metodunu kullan
        /// </summary>
        protected void btnUpdateInfo_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                string apiUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}/api/auth/user/{userId}";

                var data = new
                {
                    Name = txtName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    DisplayName = txtDisplayName.Text.Trim(),
                    Gender = ddlGender.SelectedValue,
                    Birthday = txtBirthday.Text.Trim(),
                    Location = txtLocation.Text.Trim(),
                    Bio = txtBio.Text.Trim(),
                    Interests = txtInterests.Text.Trim()
                };

                string jsonData = new JavaScriptSerializer().Serialize(data);

                var req = (HttpWebRequest)WebRequest.Create(apiUrl);
                req.Method = "PUT";
                req.ContentType = "application/json";

                using (var writer = new StreamWriter(req.GetRequestStream()))
                    writer.Write(jsonData);

                using (var res = (HttpWebResponse)req.GetResponse())
                using (var reader = new StreamReader(res.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    dynamic json = new JavaScriptSerializer().Deserialize<dynamic>(result);

                    if (json["status"] == "success")
                    {
                        ShowMessage("✅ Bilgileriniz başarıyla güncellendi!", "success");

                        // Session'daki bilgileri de güncelle
                        Session["UserName"] = txtName.Text.Trim();
                        Session["UserEmail"] = txtEmail.Text.Trim();
                    }
                    else
                    {
                        ShowMessage($"❌ {json["message"]}", "danger");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ Güncelleme başarısız: {ex.Message}", "danger");
            }
        }

        /// <summary>
        /// Şifre değiştirme
        /// </summary>
        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            try
            {
                string oldPassword = txtOldPassword.Text.Trim();
                string newPassword = txtNewPassword.Text.Trim();
                string newPasswordAgain = txtNewPasswordAgain.Text.Trim();

                // Validasyon
                if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
                {
                    lblPasswordStatus.Text = "❌ Tüm alanları doldurun.";
                    lblPasswordStatus.CssClass = "alert alert-danger";
                    return;
                }

                if (newPassword != newPasswordAgain)
                {
                    lblPasswordStatus.Text = "❌ Yeni şifreler eşleşmiyor.";
                    lblPasswordStatus.CssClass = "alert alert-danger";
                    return;
                }

                if (newPassword.Length < 6)
                {
                    lblPasswordStatus.Text = "❌ Yeni şifre en az 6 karakter olmalı.";
                    lblPasswordStatus.CssClass = "alert alert-danger";
                    return;
                }

                // API'ye istek gönder
                int userId = Convert.ToInt32(Session["UserId"]);
                string apiUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}/api/auth/changepassword/{userId}";

                var data = new
                {
                    OldPassword = oldPassword,
                    NewPassword = newPassword
                };

                string jsonData = new JavaScriptSerializer().Serialize(data);

                var req = (HttpWebRequest)WebRequest.Create(apiUrl);
                req.Method = "PUT";
                req.ContentType = "application/json";

                using (var writer = new StreamWriter(req.GetRequestStream()))
                    writer.Write(jsonData);

                using (var res = (HttpWebResponse)req.GetResponse())
                using (var reader = new StreamReader(res.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    dynamic json = new JavaScriptSerializer().Deserialize<dynamic>(result);

                    if (json["status"] == "success")
                    {
                        lblPasswordStatus.Text = "✅ Şifreniz başarıyla değiştirildi!";
                        lblPasswordStatus.CssClass = "alert alert-success";

                        // Formu temizle
                        txtOldPassword.Text = "";
                        txtNewPassword.Text = "";
                        txtNewPasswordAgain.Text = "";
                    }
                    else
                    {
                        lblPasswordStatus.Text = $"❌ {json["message"]}";
                        lblPasswordStatus.CssClass = "alert alert-danger";
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
                        dynamic errorJson = new JavaScriptSerializer().Deserialize<dynamic>(errorText);
                        lblPasswordStatus.Text = $"❌ {errorJson["message"]}";
                        lblPasswordStatus.CssClass = "alert alert-danger";
                    }
                }
                else
                {
                    lblPasswordStatus.Text = $"❌ Bağlantı hatası: {we.Message}";
                    lblPasswordStatus.CssClass = "alert alert-danger";
                }
            }
            catch (Exception ex)
            {
                lblPasswordStatus.Text = $"❌ Şifre değiştirilemedi: {ex.Message}";
                lblPasswordStatus.CssClass = "alert alert-danger";
            }
        }

        /// <summary>
        /// Hesabı devre dışı bırak
        /// </summary>
        protected void btnDeactivate_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);

                string sql = "UPDATE Users SET IsActive=0 WHERE Id=@id";
                DbHelper.Execute(sql, new MySqlParameter("@id", userId));

                lblAccountStatus.Text = "✅ Hesabınız devre dışı bırakıldı. Çıkış yapılıyor...";
                lblAccountStatus.CssClass = "alert alert-warning";

                // 2 saniye sonra logout
                Response.AddHeader("REFRESH", "2;URL=/Pages/User/Logout.aspx");
            }
            catch (Exception ex)
            {
                lblAccountStatus.Text = $"❌ Hata: {ex.Message}";
                lblAccountStatus.CssClass = "alert alert-danger";
            }
        }

        /// <summary>
        /// Hesap silme talebi oluştur
        /// </summary>
        protected void btnDeleteRequest_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);

                // Daha önce bekleyen talep var mı?
                string checkSql = "SELECT COUNT(*) FROM DeleteRequests WHERE UserId=@uid AND Status='pending'";
                object count = DbHelper.Scalar(checkSql, new MySqlParameter("@uid", userId));

                if (Convert.ToInt32(count) > 0)
                {
                    lblAccountStatus.Text = "⚠️ Zaten bekleyen bir silme talebiniz var.";
                    lblAccountStatus.CssClass = "alert alert-warning";
                    return;
                }

                // Yeni talep oluştur
                string sql = @"INSERT INTO DeleteRequests (UserId, Reason, Status, RequestedAt) 
                              VALUES (@uid, @reason, 'pending', NOW())";
                DbHelper.Execute(sql,
                    new MySqlParameter("@uid", userId),
                    new MySqlParameter("@reason", "Kullanıcı talebi"));

                lblAccountStatus.Text = "✅ Hesap silme talebiniz oluşturuldu. Admin onayı bekleniyor.";
                lblAccountStatus.CssClass = "alert alert-info";
            }
            catch (Exception ex)
            {
                lblAccountStatus.Text = $"❌ Hata: {ex.Message}";
                lblAccountStatus.CssClass = "alert alert-danger";
            }
        }

        /// <summary>
        /// Profil resmini doğrular
        /// </summary>
        private ValidationResult ValidateProfileImage(System.Web.UI.WebControls.FileUpload fileUpload)
        {
            try
            {
                // 1. Dosya boş mu?
                if (!fileUpload.HasFile || fileUpload.PostedFile == null)
                {
                    return new ValidationResult(false, "Dosya seçilmedi.");
                }

                var file = fileUpload.PostedFile;

                // 2. Dosya boyutu kontrolü
                if (file.ContentLength > MAX_FILE_SIZE_BYTES)
                {
                    return new ValidationResult(false,
                        $"Dosya boyutu çok büyük. Maksimum {MAX_FILE_SIZE_MB} MB yükleyebilirsiniz. " +
                        $"(Yüklenen: {Math.Round(file.ContentLength / 1024.0 / 1024.0, 2)} MB)");
                }

                if (file.ContentLength == 0)
                {
                    return new ValidationResult(false, "Dosya boş olamaz.");
                }

                // 3. Dosya uzantısı kontrolü
                string extension = Path.GetExtension(file.FileName).ToLower();
                bool isValidExtension = false;

                foreach (string allowedExt in ALLOWED_EXTENSIONS)
                {
                    if (extension == allowedExt)
                    {
                        isValidExtension = true;
                        break;
                    }
                }

                if (!isValidExtension)
                {
                    return new ValidationResult(false,
                        $"Geçersiz dosya formatı. Sadece {string.Join(", ", ALLOWED_EXTENSIONS)} dosyaları yüklenebilir.");
                }

                // 4. Görsel boyutu kontrolü (Image olarak yükle)
                using (var img = Image.FromStream(file.InputStream))
                {
                    int width = img.Width;
                    int height = img.Height;

                    // Minimum boyut kontrolü
                    if (width < MIN_IMAGE_WIDTH || height < MIN_IMAGE_HEIGHT)
                    {
                        return new ValidationResult(false,
                            $"Görsel boyutu çok küçük. Minimum {MIN_IMAGE_WIDTH}x{MIN_IMAGE_HEIGHT}px olmalı. " +
                            $"(Yüklenen: {width}x{height}px)");
                    }

                    // Maksimum boyut kontrolü
                    if (width > MAX_IMAGE_WIDTH || height > MAX_IMAGE_HEIGHT)
                    {
                        return new ValidationResult(false,
                            $"Görsel boyutu çok büyük. Maksimum {MAX_IMAGE_WIDTH}x{MAX_IMAGE_HEIGHT}px olmalı. " +
                            $"(Yüklenen: {width}x{height}px)");
                    }

                    // Aspect ratio kontrolü (toleranslı - kare önerilir ama zorunlu değil)
                    double aspectRatio = (double)width / height;
                    if (aspectRatio < 0.75 || aspectRatio > 1.33)
                    {
                        return new ValidationResult(false,
                            $"Görsel oranı uygun değil. Tercihen kare (1:1) görsel yükleyin. " +
                            $"(Yüklenen: {width}x{height}px, Oran: {aspectRatio:F2})");
                    }
                }

                // Stream'i başa sar (SaveAs için)
                file.InputStream.Position = 0;

                return new ValidationResult(true, "Geçerli");
            }
            catch (Exception ex)
            {
                return new ValidationResult(false,
                    $"Dosya işlenirken hata oluştu: {ex.Message}");
            }
        }

        /// <summary>
        /// Mesaj göster
        /// </summary>
        private void ShowMessage(string message, string type)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = $"alert alert-{type}";
            lblStatus.Visible = true;
        }

        /// <summary>
        /// Validasyon sonucu
        /// </summary>
        private class ValidationResult
        {
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; }

            public ValidationResult(bool isValid, string errorMessage)
            {
                IsValid = isValid;
                ErrorMessage = errorMessage;
            }
        }
    }
}
