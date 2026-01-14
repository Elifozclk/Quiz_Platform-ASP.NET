using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;

namespace QP_WEBPROJECT.vs2.Api
{
    public class AuthController : ApiController
    {
       

        // ========== AUTHENTICATION ==========

        /// <summary>
        /// Kullanıcı girişi
        /// </summary>
        [HttpPost]
        [Route("api/auth/login")]
        public IHttpActionResult Login([FromBody] dynamic body)
        {
            try
            {
                string email = body.Email;
                string password = body.Password;

                var sql = "SELECT Id, UserName AS Name, Email, PasswordHash, Role, IsActive FROM Users WHERE Email=@e LIMIT 1";
                var dt = DbHelper.Query(sql, new MySqlParameter("@e", email));

                if (dt.Rows.Count == 0)
                    return Json(new { status = "error", message = "E-posta bulunamadı." });

                var row = dt.Rows[0];
                string storedHash = row["PasswordHash"].ToString();

                if (!VerifyPassword(password, storedHash))
                    return Json(new { status = "error", message = "Şifre hatalı." });

                int isActive = Convert.ToInt32(row["IsActive"]);

                // ✅ Hesap devre dışıysa özel durum döndür
                if (isActive == 0)
                {
                    return Json(new
                    {
                        status = "inactive",
                        message = "Hesabınız daha önce devre dışı bırakılmış.",
                        data = new
                        {
                            id = row["Id"],
                            name = row["Name"],
                            email = row["Email"],
                            role = row["Role"]
                        }
                    });
                }

                return Json(new
                {
                    status = "success",
                    message = "Giriş başarılı.",
                    data = new
                    {
                        id = row["Id"],
                        name = row["Name"],
                        email = row["Email"],
                        role = row["Role"]
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = "Sunucu hatası: " + ex.Message });
            }
        }


        /// <summary>
        /// Kullanıcı kaydı
        /// </summary>
        /// <summary>
        /// Kullanıcı kaydı
        /// </summary>
        [HttpPost]
        [Route("api/auth/register")]
        public IHttpActionResult Register([FromBody] dynamic body)
        {
            try
            {
                // 🔒 Default profil resmi (Uploads/ProfileImages içinde dosya adı olarak tutuluyor)
                const string DEFAULT_PROFILE_IMAGE = "default-user.png";

                // dynamic'ten güvenli string çekme
                string userName = body?.UserName != null ? (string)body.UserName : "";
                string email = body?.Email != null ? (string)body.Email : "";
                string password = body?.Password != null ? (string)body.Password : "";

                string displayName = body?.DisplayName != null ? (string)body.DisplayName : "";
                string gender = body?.Gender != null ? (string)body.Gender : "";
                string birthday = body?.Birthday != null ? (string)body.Birthday : "";
                string location = body?.Location != null ? (string)body.Location : "";
                string bio = body?.Bio != null ? (string)body.Bio : "";
                string interests = body?.Interests != null ? (string)body.Interests : "";

                // ✅ Kritik: boş / null / whitespace gelirse default ata
                string profileImage = body?.ProfileImage != null ? (string)body.ProfileImage : "";
                profileImage = string.IsNullOrWhiteSpace(profileImage) ? DEFAULT_PROFILE_IMAGE : profileImage;

                // Basit zorunlu alan kontrolleri
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                    return Json(new { status = "error", message = "UserName, Email ve Password zorunludur." });

                // E-posta veya kullanıcı adı var mı?
                var check = DbHelper.Query("SELECT Id FROM Users WHERE Email=@e OR UserName=@u",
                    new MySqlParameter("@e", email),
                    new MySqlParameter("@u", userName));

                if (check.Rows.Count > 0)
                    return Json(new { status = "error", message = "Bu e-posta veya kullanıcı adı zaten kayıtlı." });

                string hash = HashPassword(password);

                string sql = @"INSERT INTO Users 
            (UserName, Email, PasswordHash, Role, DisplayName, Gender, Birthday, Location, Bio, Interests, ProfileImage, CreatedAt, IsActive, IsEmailVerified, RegisteredVia)
            VALUES
            (@u, @e, @p, 'user', @d, @g, @b, @l, @bio, @int, @img, NOW(), 1, 0, 'email')";

                DbHelper.Execute(sql,
                    new MySqlParameter("@u", userName),
                    new MySqlParameter("@e", email),
                    new MySqlParameter("@p", hash),
                    new MySqlParameter("@d", displayName ?? ""),
                    new MySqlParameter("@g", gender ?? ""),
                    new MySqlParameter("@b", string.IsNullOrWhiteSpace(birthday) ? DBNull.Value : (object)birthday),
                    new MySqlParameter("@l", location ?? ""),
                    new MySqlParameter("@bio", bio ?? ""),
                    new MySqlParameter("@int", interests ?? ""),
                    new MySqlParameter("@img", profileImage) // ✅ artık boş gelirse de default garanti
                );

                return Json(new { status = "success", message = "Kayıt başarılı! Giriş yapabilirsiniz." });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = "Kayıt başarısız: " + ex.Message });
            }
        }

        /// <summary>
        /// Devre dışı hesabı yeniden aktif etme
        /// </summary>
        [HttpPut]
        [Route("api/auth/reactivate/{userId}")]
        public IHttpActionResult ReactivateAccount(int userId)
        {
            try
            {
                // Kullanıcı var mı kontrol et
                var userCheck = DbHelper.Query("SELECT Id, IsActive FROM Users WHERE Id=@id",
                    new MySqlParameter("@id", userId));

                if (userCheck.Rows.Count == 0)
                    return Json(new { status = "error", message = "Kullanıcı bulunamadı." });

                int currentStatus = Convert.ToInt32(userCheck.Rows[0]["IsActive"]);

                if (currentStatus == 1)
                    return Json(new { status = "warning", message = "Hesap zaten aktif." });

                // Hesabı aktif et
                DbHelper.Execute("UPDATE Users SET IsActive = 1 WHERE Id = @id",
                    new MySqlParameter("@id", userId));

                return Json(new { status = "success", message = "Hesabınız başarıyla yeniden aktif edildi." });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = "Sunucu hatası: " + ex.Message });
            }
        }





        // ========== KULLANICI YÖNETİMİ ==========

        /// <summary>
        /// Kullanıcı bilgilerini getir (READ)
        /// </summary>
        [HttpGet]
        [Route("api/auth/user/{id}")]
        public IHttpActionResult GetUser(int id)
        {
            try
            {
                var sql = @"SELECT UserName AS Name, Email, DisplayName, Gender, Birthday, Location, Bio, Interests, ProfileImage, Role, IsActive, CreatedAt
                    FROM Users WHERE Id=@id";

                var dt = DbHelper.Query(sql, new MySqlParameter("@id", id));

                if (dt.Rows.Count == 0)
                    return Json(new { status = "error", message = "Kullanıcı bulunamadı." });

                var row = dt.Rows[0];
                return Json(new
                {
                    status = "success",
                    data = new
                    {
                        Name = row["Name"],
                        Email = row["Email"],
                        DisplayName = row["DisplayName"],
                        Gender = row["Gender"],
                        Birthday = row["Birthday"] == DBNull.Value ? "" : Convert.ToDateTime(row["Birthday"]).ToString("yyyy-MM-dd"),
                        Location = row["Location"],
                        Bio = row["Bio"],
                        Interests = row["Interests"],
                        ProfileImage = row["ProfileImage"]?.ToString() ?? "",
                        Role = row["Role"],
                        IsActive = Convert.ToInt32(row["IsActive"]),
                        CreatedAt = row["CreatedAt"] == DBNull.Value ? "" : Convert.ToDateTime(row["CreatedAt"]).ToString("yyyy-MM-dd")
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = "Sunucu hatası: " + ex.Message });
            }
        }

        /// <summary>
        /// Kullanıcı bilgilerini güncelle (UPDATE)
        /// </summary>
        [HttpPut]
        [Route("api/auth/user/{id}")]
        public IHttpActionResult UpdateUser(int id, [FromBody] dynamic body)
        {
            try
            {
                string name = body.Name;
                string email = body.Email;
                string displayName = body.DisplayName;
                string gender = body.Gender;
                string birthday = body.Birthday;
                string location = body.Location;
                string bio = body.Bio;
                string interests = body.Interests;

                // Email çakışma kontrolü
                var check = DbHelper.Query("SELECT Id FROM Users WHERE Email=@e AND Id != @id",
                    new MySqlParameter("@e", email),
                    new MySqlParameter("@id", id));

                if (check.Rows.Count > 0)
                    return Json(new { status = "error", message = "Bu e-posta başka bir kullanıcı tarafından kullanılıyor." });

                object birthParam = DBNull.Value;
                if (DateTime.TryParse(birthday, out DateTime parsedDate))
                    birthParam = parsedDate;

                string sql = @"UPDATE Users SET 
                    UserName=@n,
                    Email=@e,
                    DisplayName=@d,
                    Gender=@g,
                    Birthday=@b,
                    Location=@l,
                    Bio=@bio,
                    Interests=@i
                    WHERE Id=@id";

                int affected = DbHelper.Execute(sql,
                    new MySqlParameter("@n", name),
                    new MySqlParameter("@e", email),
                    new MySqlParameter("@d", displayName ?? ""),
                    new MySqlParameter("@g", gender ?? ""),
                    new MySqlParameter("@b", birthParam),
                    new MySqlParameter("@l", location ?? ""),
                    new MySqlParameter("@bio", bio ?? ""),
                    new MySqlParameter("@i", interests ?? ""),
                    new MySqlParameter("@id", id)
                );

                if (affected == 0)
                    return Json(new { status = "warning", message = "Kullanıcı bulunamadı veya değişiklik yapılmadı." });

                return Json(new { status = "success", message = "Bilgiler başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = "Sunucu hatası: " + ex.Message });
            }
        }

        

        // ========== ŞİFRE İŞLEMLERİ ==========


        /// <summary>
        /// Şifre değiştirme
        /// </summary>
        [HttpPut]
        [Route("api/auth/changepassword/{userId}")]
        public IHttpActionResult ChangePassword(int userId, [FromBody] dynamic body)
        {
            try
            {
                string oldPassword = body.OldPassword;
                string newPassword = body.NewPassword;

                // Kullanıcı kontrolü
                var sql = "SELECT PasswordHash FROM Users WHERE Id=@id";
                var dt = DbHelper.Query(sql, new MySqlParameter("@id", userId));

                if (dt.Rows.Count == 0)
                    return Json(new { status = "error", message = "Kullanıcı bulunamadı." });

                string storedHash = dt.Rows[0]["PasswordHash"].ToString();

                // Eski şifre kontrolü
                if (!VerifyPassword(oldPassword, storedHash))
                    return Json(new { status = "error", message = "Eski şifre hatalı." });

                // Yeni şifre validasyonu
                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                    return Json(new { status = "error", message = "Yeni şifre en az 6 karakter olmalı." });

                // Yeni şifreyi hash'le ve güncelle
                string newHash = HashPassword(newPassword);
                string updateSql = "UPDATE Users SET PasswordHash=@hash WHERE Id=@id";
                DbHelper.Execute(updateSql,
                    new MySqlParameter("@hash", newHash),
                    new MySqlParameter("@id", userId));

                return Json(new { status = "success", message = "Şifreniz başarıyla değiştirildi." });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = "Sunucu hatası: " + ex.Message });
            }
        }

        // ========================================
        // NOT: Bu metodu AuthController.cs dosyasına ekleyin
        // ========================================




        // ========== KULLANICI SUBMISSIONS ==========

        /// <summary>
        /// Kullanıcının quiz geçmişi
        /// </summary>
        [HttpGet]
        [Route("api/user/submissions/{userId}")]
        public IHttpActionResult GetUserSubmissions(int userId)
        {
            try
            {
                string sql = @"
                    SELECT s.Id AS SubmissionId, q.Title AS QuizTitle, r.Title AS ResultTitle, s.SubmittedAt
                    FROM Submissions s
                    INNER JOIN Quizzes q ON q.Id = s.QuizId
                    LEFT JOIN Results r ON r.Id = s.ResultId
                    WHERE s.UserId = @uid
                    ORDER BY s.SubmittedAt DESC";

                var dt = DbHelper.Query(sql, new MySqlParameter("@uid", userId));

                var list = dt.AsEnumerable().Select(row => new
                {
                    QuizTitle = row["QuizTitle"]?.ToString(),
                    ResultTitle = row["ResultTitle"]?.ToString(),
                    SubmittedAt = row["SubmittedAt"] != DBNull.Value
                        ? Convert.ToDateTime(row["SubmittedAt"]).ToString("yyyy-MM-dd HH:mm")
                        : ""
                });

                return Json(new { status = "success", data = list });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = "Sunucu hatası: " + ex.Message });
            }
        }

        // ========== ADMIN: DELETE REQUESTS YÖNETİMİ ==========

        /// <summary>
        /// Tüm silme taleplerini getir
        /// </summary>
   


        /// <summary>
        /// Silme talebini onayla (DELETE)
        /// </summary>
        [HttpDelete]
        [Route("api/admin/deleterequest/{requestId}")]
        public IHttpActionResult ApproveDeleteRequest(int requestId)  // ← [FromBody] kaldırıldı
        {
            try
            {
                // AdminId'yi parametreden değil, veritabanından alalım
                // Veya query string'den: ?adminId=1

                var check = DbHelper.Query("SELECT UserId FROM DeleteRequests WHERE Id=@id",
                    new MySqlParameter("@id", requestId));

                if (check.Rows.Count == 0)
                    return Json(new { status = "error", message = "Talep bulunamadı." });

                int userId = Convert.ToInt32(check.Rows[0]["UserId"]);

                // Kullanıcıyı sil (cascade işlemleri)
                // 1. SubmissionAnswers
                DbHelper.Execute(@"
            DELETE sa FROM SubmissionAnswers sa
            INNER JOIN Submissions s ON sa.SubmissionId = s.Id
            WHERE s.UserId = @uid",
                    new MySqlParameter("@uid", userId));

                // 2. Submissions
                DbHelper.Execute("DELETE FROM Submissions WHERE UserId = @uid",
                    new MySqlParameter("@uid", userId));

                // 3. Kullanıcının quizlerini sil
                // 3a. Quiz submission answers
                DbHelper.Execute(@"
            DELETE sa FROM SubmissionAnswers sa
            INNER JOIN Submissions s ON sa.SubmissionId = s.Id
            INNER JOIN Quizzes q ON s.QuizId = q.Id
            WHERE q.CreatedBy = @uid",
                    new MySqlParameter("@uid", userId));

                // 3b. Quiz submissions
                DbHelper.Execute(@"
            DELETE s FROM Submissions s
            INNER JOIN Quizzes q ON s.QuizId = q.Id
            WHERE q.CreatedBy = @uid",
                    new MySqlParameter("@uid", userId));

                // 3c. Options
                DbHelper.Execute(@"
            DELETE o FROM Options o
            INNER JOIN Questions q ON o.QuestionId = q.Id
            INNER JOIN Quizzes qz ON q.QuizId = qz.Id
            WHERE qz.CreatedBy = @uid",
                    new MySqlParameter("@uid", userId));

                // 3d. Questions
                DbHelper.Execute(@"
            DELETE qn FROM Questions qn
            INNER JOIN Quizzes q ON qn.QuizId = q.Id
            WHERE q.CreatedBy = @uid",
                    new MySqlParameter("@uid", userId));

                // 3e. Results
                DbHelper.Execute(@"
            DELETE r FROM Results r
            INNER JOIN Quizzes q ON r.QuizId = q.Id
            WHERE q.CreatedBy = @uid",
                    new MySqlParameter("@uid", userId));

                // 3f. Quizzes
                DbHelper.Execute("DELETE FROM Quizzes WHERE CreatedBy = @uid",
                    new MySqlParameter("@uid", userId));

                // 4. DeleteRequest'i güncelle
                DbHelper.Execute(@"UPDATE DeleteRequests SET Status='approved', ReviewedAt=NOW() WHERE Id=@id",
                    new MySqlParameter("@id", requestId));

                // 5. Kullanıcıyı sil
                DbHelper.Execute("DELETE FROM Users WHERE Id=@uid",
                    new MySqlParameter("@uid", userId));

                return Json(new { status = "success", message = "Kullanıcı başarıyla silindi." });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = "Sunucu hatası: " + ex.Message });
            }
        }

        /// <summary>
        /// Silme talebini reddetme işlemi
        /// </summary>
        [HttpPut]
        [Route("api/admin/deleterequest/{requestId}/reject")]
        public IHttpActionResult RejectDeleteRequest(int requestId)
        {
            try
            {
                var session = HttpContext.Current?.Session;
                if (session == null || session["UserType"]?.ToString() != "admin")
                    return Json(new { status = "error", message = "Yetkisiz." });

                int adminId = Convert.ToInt32(session["UserId"]);

                // Talebi reddet ve admin bilgisini işle
                string sql = "UPDATE deleterequests SET Status='rejected', ReviewedAt=NOW(), ReviewedBy=@aid WHERE Id=@id";
                DbHelper.Execute(sql, new MySqlParameter("@aid", adminId), new MySqlParameter("@id", requestId));

                return Json(new { status = "success", message = "Talep reddedildi." });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message });
            }
        }

        // ========== YARDIMCI METOTLAR ==========

        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }
    }
}