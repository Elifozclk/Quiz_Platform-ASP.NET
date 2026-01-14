using MySql.Data.MySqlClient;
using QP_WEBPROJECT.vs2.Pages.User;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using ZstdSharp.Unsafe;

namespace QP_WEBPROJECT.vs2.Pages.User
{
    public partial class UserDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Kullanıcı giriş kontrolü - daha esnek
            if (Session["UserId"] == null)
            {
                Response.Redirect("/Pages/User/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadRecommendedQuizzes();
            }
        }
        protected string SafeSnippet(object descObj)
        {
            var text = descObj?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(text))
                return "Hemen çözmeye başla!";

            // HTML taglerini temizle
            text = Regex.Replace(text, "<.*?>", string.Empty);

            // HTML entity decode (&nbsp; vs.)
            text = HttpUtility.HtmlDecode(text);

            // kırp
            const int maxLen = 100;
            if (text.Length > maxLen)
                text = text.Substring(0, maxLen) + "...";

            // güvenli bas
            return HttpUtility.HtmlEncode(text);
        }
        private void LoadRecommendedQuizzes()
        {
            try
            {
                // Son eklenen yayınlanmış quiz'leri getir
                string sql = @"SELECT 
                Id, 
                Title, 
                Description, 
                Slug, 
                CoverImageUrl, 
                EstimatedTime 
               FROM Quizzes 
               WHERE Status='Published' 
               ORDER BY CreatedAt DESC 
               LIMIT 12";

                var dt = DbHelper.Query(sql);

                // Debug için
                System.Diagnostics.Debug.WriteLine($"UserDashboard - Quiz Sayısı: {dt.Rows.Count}");

                if (dt.Rows.Count > 0)
                {
                    // ✅ DEBUG: Hangi quiz'lerde resim var?
                    System.Diagnostics.Debug.WriteLine("========== USER DASHBOARD QUIZ LİSTESİ ==========");
                    foreach (DataRow row in dt.Rows)
                    {
                        string coverUrl = row["CoverImageUrl"]?.ToString() ?? "";
                        string hasImage = !string.IsNullOrEmpty(coverUrl) ? "✅ VAR" : "❌ YOK";
                        System.Diagnostics.Debug.WriteLine($"Quiz {row["Id"]}: {row["Title"]}");
                        System.Diagnostics.Debug.WriteLine($"   CoverImageUrl: '{coverUrl}' - {hasImage}");
                    }
                    System.Diagnostics.Debug.WriteLine("=================================================");

                    rptQuizzes.DataSource = dt;
                    rptQuizzes.DataBind();
                    pnlNoQuiz.Visible = false;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("UserDashboard - Hiç quiz bulunamadı!");
                    rptQuizzes.DataSource = null;
                    rptQuizzes.DataBind();
                    pnlNoQuiz.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UserDashboard HATA: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                lblError.Text = "Quiz'ler yüklenirken hata oluştu: " + ex.Message;
                lblError.Visible = true;
                pnlNoQuiz.Visible = true;
            }
        }
    }
}
