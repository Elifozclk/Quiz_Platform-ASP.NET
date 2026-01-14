using System;

namespace QP_WEBPROJECT.vs2.Pages.Admin
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserType"] == null || Session["UserType"].ToString() != "admin")
            {
                Response.Redirect("/Pages/User/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblWelcome.Text = $"Hoş geldin, {Session["UserName"]}!";
                LoadQuickStatistics();
            }
        }

        /// <summary>
        /// Hızlı istatistikleri yükle (sadece kartlarda görünecek sayılar)
        /// </summary>
        private void LoadQuickStatistics()
        {
            try
            {
                // Toplam Quiz sayısı
                var totalQuizzes = DbHelper.Scalar("SELECT COUNT(*) FROM Quizzes");
                lblTotalQuizzes.Text = totalQuizzes?.ToString() ?? "0";

                // Toplam Kullanıcı sayısı
                var totalUsers = DbHelper.Scalar("SELECT COUNT(*) FROM Users WHERE Role='user'");
                lblTotalUsers.Text = totalUsers?.ToString() ?? "0";

                // Aktif Kullanıcılar (son 30 günde giriş yapmış)
                var activeUsers = DbHelper.Scalar(@"
                    SELECT COUNT(DISTINCT UserId) 
                    FROM Submissions 
                    WHERE SubmittedAt >= DATE_SUB(NOW(), INTERVAL 30 DAY)");
                lblActiveUsers.Text = activeUsers?.ToString() ?? "0";

                // Bekleyen Silme Talepleri
                var pendingRequests = DbHelper.Scalar("SELECT COUNT(*) FROM DeleteRequests WHERE Status='pending'");
                lblPendingRequests.Text = pendingRequests?.ToString() ?? "0";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("İstatistik yükleme hatası: " + ex.Message);
            }
        }
    }
}