using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace QP_WEBPROJECT.vs2.Pages.User
{
    public partial class Register : System.Web.UI.Page
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

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            lblSuccess.Text = "";

            try
            {
                string profileImage = "default-user.png"; // Varsayılan resim

                // ✅ PROFIL RESMİ YÜKLEME VE DOĞRULAMA
                if (fuProfileImage.HasFile)
                {
                    // Dosya validasyonu
                    var validationResult = ValidateProfileImage(fuProfileImage);

                    if (!validationResult.IsValid)
                    {
                        lblError.Text = $"❌ Profil Resmi Hatası: {validationResult.ErrorMessage}";
                        lblError.CssClass = "alert alert-danger";
                        return;
                    }

                    // Dosya adı ve kaydetme
                    string fileName = Guid.NewGuid() + Path.GetExtension(fuProfileImage.FileName);
                    string savePath = Server.MapPath("~/Uploads/ProfileImages/" + fileName);

                    // Klasör yoksa oluştur
                    string folder = Server.MapPath("~/Uploads/ProfileImages/");
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    fuProfileImage.SaveAs(savePath);
                    profileImage = fileName;

                    System.Diagnostics.Debug.WriteLine($"✅ Profil resmi kaydedildi: {fileName}");
                }

                // Kullanıcı kaydı
                var data = new
                {
                    UserName = txtUserName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Password = txtPassword.Text.Trim(),
                    DisplayName = txtDisplayName.Text.Trim(),
                    Gender = ddlGender.SelectedValue,
                    Birthday = txtBirthday.Text.Trim(),
                    Location = txtLocation.Text.Trim(),
                    Bio = txtBio.Text.Trim(),
                    Interests = txtInterests.Text.Trim(),
                    ProfileImage = profileImage
                };

                string apiUrl = $"{Request.Url.Scheme}://{Request.Url.Authority}/api/auth/register";
                var client = (HttpWebRequest)WebRequest.Create(apiUrl);
                client.Method = "POST";
                client.ContentType = "application/json";

                string json = new JavaScriptSerializer().Serialize(data);
                using (var streamWriter = new StreamWriter(client.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                var response = (HttpWebResponse)client.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    dynamic obj = new JavaScriptSerializer().Deserialize<dynamic>(result);

                    if (obj["status"] == "success")
                    {
                        lblSuccess.Text = "✅ " + obj["message"] + " Giriş sayfasına yönlendiriliyorsunuz...";
                        lblSuccess.CssClass = "alert alert-success";

                        // 2 saniye sonra login sayfasına yönlendir
                        Response.AddHeader("REFRESH", "2;URL=/Pages/User/Login.aspx");
                    }
                    else
                    {
                        lblError.Text = "❌ " + obj["message"];
                        lblError.CssClass = "alert alert-danger";
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "❌ Kayıt başarısız: " + ex.Message;
                lblError.CssClass = "alert alert-danger";
                System.Diagnostics.Debug.WriteLine($"Register Error: {ex.Message}");
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
