using System;
using System.Data;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace QP_WEBPROJECT.vs2.Pages.Admin
{
    public partial class Statistics : System.Web.UI.Page
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
                LoadAllStatistics();
                lblLastUpdate.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
            }
        }

        private void LoadAllStatistics()
        {
            LoadUserStatistics();
            LoadQuizStatistics();
            LoadSubmissionStatistics();
            LoadSystemStatistics();
        }

        #region KULLANICI İSTATİSTİKLERİ

        private void LoadUserStatistics()
        {
            try
            {
                // Toplam Kullanıcılar
                int totalUsers = Convert.ToInt32(DbHelper.Scalar("SELECT COUNT(*) FROM Users WHERE Role='user'") ?? 0);
                lblTotalUsers.Text = totalUsers.ToString();

                // Bu hafta eklenen
                int usersWeek = Convert.ToInt32(DbHelper.Scalar(
                    "SELECT COUNT(*) FROM Users WHERE Role='user' AND CreatedAt >= DATE_SUB(NOW(), INTERVAL 7 DAY)") ?? 0);
                lblUsersThisWeek.Text = usersWeek.ToString();

                // Aktif kullanıcılar
                lblActiveUsers.Text = DbHelper.Scalar(@"
                    SELECT COUNT(DISTINCT UserId) FROM Submissions 
                    WHERE SubmittedAt >= DATE_SUB(NOW(), INTERVAL 30 DAY)")?.ToString() ?? "0";

                // Devre dışı hesaplar
                lblInactiveUsers.Text = DbHelper.Scalar("SELECT COUNT(*) FROM Users WHERE IsActive=0")?.ToString() ?? "0";

                // Ortalama yaş
                try
                {
                    var avgAge = DbHelper.Query(@"
                        SELECT AVG(TIMESTAMPDIFF(YEAR, Birthday, CURDATE())) AS AvgAge 
                        FROM Users 
                        WHERE Birthday IS NOT NULL AND Role='user'");

                    if (avgAge.Rows.Count > 0 && avgAge.Rows[0]["AvgAge"] != DBNull.Value)
                    {
                        lblAverageAge.Text = Math.Round(Convert.ToDouble(avgAge.Rows[0]["AvgAge"]), 1).ToString();
                    }
                    else
                    {
                        lblAverageAge.Text = "N/A";
                    }
                }
                catch
                {
                    lblAverageAge.Text = "N/A";
                }

                // Cinsiyet dağılımı
                int femaleCount = Convert.ToInt32(DbHelper.Scalar("SELECT COUNT(*) FROM Users WHERE Gender='Kadın' AND Role='user'") ?? 0);
                int maleCount = Convert.ToInt32(DbHelper.Scalar("SELECT COUNT(*) FROM Users WHERE Gender='Erkek' AND Role='user'") ?? 0);

                lblFemaleUsers.Text = femaleCount.ToString();
                lblMaleUsers.Text = maleCount.ToString();

                if (totalUsers > 0)
                {
                    lblFemalePercent.Text = Math.Round((double)femaleCount / totalUsers * 100, 1).ToString();
                    lblMalePercent.Text = Math.Round((double)maleCount / totalUsers * 100, 1).ToString();
                }
                else
                {
                    lblFemalePercent.Text = "0";
                    lblMalePercent.Text = "0";
                }

                // Grafik verileri
                PrepareGenderChart(femaleCount, maleCount, totalUsers);
                PrepareAgeChart();
                PrepareRegistrationTrendChart();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadUserStatistics hatası: " + ex.Message);
            }
        }

        private void PrepareGenderChart(int female, int male, int total)
        {
            int other = total - female - male;

            var data = new
            {
                labels = new[] { "Kadın", "Erkek", "Belirtilmemiş" },
                datasets = new[]
                {
                    new
                    {
                        data = new[] { female, male, other },
                        backgroundColor = new[] { "#ec4899", "#3b82f6", "#6b7280" }
                    }
                }
            };

            string script = $@"
                <script>
                window.addEventListener('load', function() {{
                    const ctx = document.getElementById('genderChart');
                    if (ctx) {{
                        new Chart(ctx, {{
                            type: 'doughnut',
                            data: {new JavaScriptSerializer().Serialize(data)},
                            options: {{
                                responsive: true,
                                maintainAspectRatio: false,
                                plugins: {{ legend: {{ position: 'bottom' }} }}
                            }}
                        }});
                    }}
                }});
                </script>
            ";
            ClientScript.RegisterStartupScript(this.GetType(), "genderChart", script, false);
        }

        private void PrepareAgeChart()
        {
            try
            {
                var ageGroups = DbHelper.Query(@"
                    SELECT 
                        CASE 
                            WHEN TIMESTAMPDIFF(YEAR, Birthday, CURDATE()) < 18 THEN '<18'
                            WHEN TIMESTAMPDIFF(YEAR, Birthday, CURDATE()) BETWEEN 18 AND 24 THEN '18-24'
                            WHEN TIMESTAMPDIFF(YEAR, Birthday, CURDATE()) BETWEEN 25 AND 34 THEN '25-34'
                            WHEN TIMESTAMPDIFF(YEAR, Birthday, CURDATE()) BETWEEN 35 AND 44 THEN '35-44'
                            WHEN TIMESTAMPDIFF(YEAR, Birthday, CURDATE()) BETWEEN 45 AND 54 THEN '45-54'
                            ELSE '55+'
                        END AS AgeGroup,
                        COUNT(*) AS Count
                    FROM Users
                    WHERE Birthday IS NOT NULL AND Role='user'
                    GROUP BY AgeGroup
                    ORDER BY AgeGroup");

                var labels = new List<string>();
                var counts = new List<int>();

                foreach (DataRow row in ageGroups.Rows)
                {
                    labels.Add(row["AgeGroup"].ToString());
                    counts.Add(Convert.ToInt32(row["Count"]));
                }

                var data = new
                {
                    labels = labels.ToArray(),
                    datasets = new[]
                    {
                        new
                        {
                            label = "Kullanıcı Sayısı",
                            data = counts.ToArray(),
                            backgroundColor = "#5B0E2D"
                        }
                    }
                };

                string script = $@"
                    <script>
                    window.addEventListener('load', function() {{
                        const ctx = document.getElementById('ageChart');
                        if (ctx) {{
                            new Chart(ctx, {{
                                type: 'bar',
                                data: {new JavaScriptSerializer().Serialize(data)},
                                options: {{
                                    responsive: true,
                                    maintainAspectRatio: false,
                                    plugins: {{ legend: {{ display: false }} }}
                                }}
                            }});
                        }}
                    }});
                    </script>
                ";
                ClientScript.RegisterStartupScript(this.GetType(), "ageChart", script, false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PrepareAgeChart hatası: " + ex.Message);
            }
        }

        private void PrepareRegistrationTrendChart()
        {
            try
            {
                var trend = DbHelper.Query(@"
                    SELECT 
                        DATE(CreatedAt) AS Date,
                        COUNT(*) AS Count
                    FROM Users
                    WHERE CreatedAt >= DATE_SUB(CURDATE(), INTERVAL 7 DAY) AND Role='user'
                    GROUP BY DATE(CreatedAt)
                    ORDER BY Date");

                var labels = new List<string>();
                var counts = new List<int>();

                for (int i = 6; i >= 0; i--)
                {
                    var date = DateTime.Today.AddDays(-i);
                    labels.Add(date.ToString("dd MMM"));

                    int count = 0;
                    foreach (DataRow row in trend.Rows)
                    {
                        if (Convert.ToDateTime(row["Date"]).Date == date.Date)
                        {
                            count = Convert.ToInt32(row["Count"]);
                            break;
                        }
                    }
                    counts.Add(count);
                }

                var data = new
                {
                    labels = labels.ToArray(),
                    datasets = new[]
                    {
                        new
                        {
                            label = "Yeni Kayıtlar",
                            data = counts.ToArray(),
                            borderColor = "#10b981",
                            backgroundColor = "rgba(16, 185, 129, 0.1)",
                            fill = true
                        }
                    }
                };

                string script = $@"
                    <script>
                    window.addEventListener('load', function() {{
                        const ctx = document.getElementById('registrationTrendChart');
                        if (ctx) {{
                            new Chart(ctx, {{
                                type: 'line',
                                data: {new JavaScriptSerializer().Serialize(data)},
                                options: {{
                                    responsive: true,
                                    maintainAspectRatio: false,
                                    plugins: {{ legend: {{ display: false }} }}
                                }}
                            }});
                        }}
                    }});
                    </script>
                ";
                ClientScript.RegisterStartupScript(this.GetType(), "registrationTrendChart", script, false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PrepareRegistrationTrendChart hatası: " + ex.Message);
            }
        }

        #endregion

        #region QUIZ İSTATİSTİKLERİ

        private void LoadQuizStatistics()
        {
            try
            {
                // Toplam Quiz
                int totalQuizzes = Convert.ToInt32(DbHelper.Scalar("SELECT COUNT(*) FROM Quizzes") ?? 0);
                lblTotalQuizzes.Text = totalQuizzes.ToString();

                // Bu ay eklenen
                int quizzesMonth = Convert.ToInt32(DbHelper.Scalar(
                    "SELECT COUNT(*) FROM Quizzes WHERE CreatedAt >= DATE_SUB(NOW(), INTERVAL 30 DAY)") ?? 0);
                lblQuizzesThisMonth.Text = quizzesMonth.ToString();

                // Yayınlanmış
                int publishedCount = Convert.ToInt32(DbHelper.Scalar("SELECT COUNT(*) FROM Quizzes WHERE Status='Published'") ?? 0);
                lblPublishedQuizzes.Text = publishedCount.ToString();

                // Yayınlanma oranı
                if (totalQuizzes > 0)
                {
                    lblPublishRate.Text = Math.Round((double)publishedCount / totalQuizzes * 100, 1).ToString();
                }
                else
                {
                    lblPublishRate.Text = "0";
                }

                // Taslak
                lblDraftQuizzes.Text = DbHelper.Scalar("SELECT COUNT(*) FROM Quizzes WHERE Status='Draft'")?.ToString() ?? "0";

                // Toplam soru sayısı (DÜZELTİLDİ)
                int totalQuestions = Convert.ToInt32(DbHelper.Scalar("SELECT COUNT(*) FROM Questions") ?? 0);
                lblTotalQuestions.Text = totalQuestions.ToString();

                // Ortalama soru sayısı
                if (totalQuizzes > 0)
                {
                    lblAvgQuestions.Text = Math.Round((double)totalQuestions / totalQuizzes, 1).ToString();
                }
                else
                {
                    lblAvgQuestions.Text = "0";
                }

                // Toplam seçenek sayısı (DÜZELTİLDİ)
                int totalOptions = Convert.ToInt32(DbHelper.Scalar("SELECT COUNT(*) FROM Options") ?? 0);
                lblTotalOptions.Text = totalOptions.ToString();

                // Ortalama seçenek sayısı
                if (totalQuestions > 0)
                {
                    lblAvgOptions.Text = Math.Round((double)totalOptions / totalQuestions, 1).ToString();
                }
                else
                {
                    lblAvgOptions.Text = "0";
                }

                // En popüler quiz
                try
                {
                    var popularQuiz = DbHelper.Query(@"
                        SELECT q.Title, COUNT(s.Id) AS SolveCount
                        FROM Quizzes q
                        LEFT JOIN Submissions s ON q.Id = s.QuizId
                        WHERE q.Status='Published'
                        GROUP BY q.Id, q.Title
                        ORDER BY SolveCount DESC
                        LIMIT 1");

                    if (popularQuiz.Rows.Count > 0)
                    {
                        string title = popularQuiz.Rows[0]["Title"].ToString();
                        int count = Convert.ToInt32(popularQuiz.Rows[0]["SolveCount"]);
                        lblMostPopularQuiz.Text = title;
                        lblPopularQuizCount.Text = count.ToString();
                    }
                    else
                    {
                        lblMostPopularQuiz.Text = "N/A";
                        lblPopularQuizCount.Text = "0";
                    }
                }
                catch
                {
                    lblMostPopularQuiz.Text = "N/A";
                    lblPopularQuizCount.Text = "0";
                }

                // Grafik verileri
                PrepareQuizStatusChart(publishedCount, totalQuizzes - publishedCount);
                PrepareQuestionCountChart();
                PrepareTopQuizzesChart();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadQuizStatistics hatası: " + ex.Message);
            }
        }

        private void PrepareQuizStatusChart(int published, int draft)
        {
            var data = new
            {
                labels = new[] { "Yayınlanan", "Taslak" },
                datasets = new[]
                {
                    new
                    {
                        data = new[] { published, draft },
                        backgroundColor = new[] { "#10b981", "#f59e0b" }
                    }
                }
            };

            string script = $@"
                <script>
                window.addEventListener('load', function() {{
                    const ctx = document.getElementById('quizStatusChart');
                    if (ctx) {{
                        new Chart(ctx, {{
                            type: 'pie',
                            data: {new JavaScriptSerializer().Serialize(data)},
                            options: {{
                                responsive: true,
                                maintainAspectRatio: false,
                                plugins: {{ legend: {{ position: 'bottom' }} }}
                            }}
                        }});
                    }}
                }});
                </script>
            ";
            ClientScript.RegisterStartupScript(this.GetType(), "quizStatusChart", script, false);
        }

        private void PrepareQuestionCountChart()
        {
            try
            {
                // Quiz'lerin soru sayılarını hesapla
                var questionCounts = DbHelper.Query(@"
                    SELECT 
                        q.Id AS QuizId,
                        COUNT(qs.Id) AS QuestionCount
                    FROM Quizzes q
                    LEFT JOIN Questions qs ON q.Id = qs.QuizId
                    GROUP BY q.Id");

                // Aralıklara göre grupla
                var ranges = new Dictionary<string, int>
                {
                    { "1-5", 0 },
                    { "6-10", 0 },
                    { "11-15", 0 },
                    { "16+", 0 }
                };

                foreach (DataRow row in questionCounts.Rows)
                {
                    int count = Convert.ToInt32(row["QuestionCount"]);

                    if (count >= 1 && count <= 5)
                        ranges["1-5"]++;
                    else if (count >= 6 && count <= 10)
                        ranges["6-10"]++;
                    else if (count >= 11 && count <= 15)
                        ranges["11-15"]++;
                    else if (count >= 16)
                        ranges["16+"]++;
                }

                var labels = new List<string>();
                var counts = new List<int>();

                foreach (var range in ranges)
                {
                    labels.Add(range.Key + " soru");
                    counts.Add(range.Value);
                }

                // Eğer hiç veri yoksa varsayılan göster
                if (counts.Sum() == 0)
                {
                    labels.Clear();
                    counts.Clear();
                    labels.Add("Henüz quiz yok");
                    counts.Add(0);
                }

                var data = new
                {
                    labels = labels.ToArray(),
                    datasets = new[]
                    {
                        new
                        {
                            label = "Quiz Sayısı",
                            data = counts.ToArray(),
                            backgroundColor = "#8b5cf6"
                        }
                    }
                };

                string script = $@"
                    <script>
                    window.addEventListener('load', function() {{
                        const ctx = document.getElementById('questionCountChart');
                        if (ctx) {{
                            new Chart(ctx, {{
                                type: 'bar',
                                data: {new JavaScriptSerializer().Serialize(data)},
                                options: {{
                                    responsive: true,
                                    maintainAspectRatio: false,
                                    plugins: {{ legend: {{ display: false }} }},
                                    scales: {{
                                        y: {{
                                            beginAtZero: true,
                                            ticks: {{ stepSize: 1 }}
                                        }}
                                    }}
                                }}
                            }});
                        }}
                    }});
                    </script>
                ";
                ClientScript.RegisterStartupScript(this.GetType(), "questionCountChart", script, false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PrepareQuestionCountChart hatası: " + ex.Message);

                // Hata durumunda boş grafik göster
                var data = new
                {
                    labels = new[] { "Veri yok" },
                    datasets = new[]
                    {
                        new
                        {
                            label = "Quiz Sayısı",
                            data = new[] { 0 },
                            backgroundColor = "#8b5cf6"
                        }
                    }
                };

                string script = $@"
                    <script>
                    window.addEventListener('load', function() {{
                        const ctx = document.getElementById('questionCountChart');
                        if (ctx) {{
                            new Chart(ctx, {{
                                type: 'bar',
                                data: {new JavaScriptSerializer().Serialize(data)},
                                options: {{
                                    responsive: true,
                                    maintainAspectRatio: false,
                                    plugins: {{ legend: {{ display: false }} }}
                                }}
                            }});
                        }}
                    }});
                    </script>
                ";
                ClientScript.RegisterStartupScript(this.GetType(), "questionCountChart_error", script, false);
            }
        }

        private void PrepareTopQuizzesChart()
        {
            try
            {
                var topQuizzes = DbHelper.Query(@"
                    SELECT q.Title, COUNT(s.Id) AS SolveCount
                    FROM Quizzes q
                    LEFT JOIN Submissions s ON q.Id = s.QuizId
                    WHERE q.Status='Published'
                    GROUP BY q.Id, q.Title
                    ORDER BY SolveCount DESC
                    LIMIT 5");

                var labels = new List<string>();
                var counts = new List<int>();

                foreach (DataRow row in topQuizzes.Rows)
                {
                    labels.Add(row["Title"].ToString());
                    counts.Add(Convert.ToInt32(row["SolveCount"]));
                }

                var data = new
                {
                    labels = labels.ToArray(),
                    datasets = new[]
                    {
                        new
                        {
                            label = "Çözüm Sayısı",
                            data = counts.ToArray(),
                            backgroundColor = "#3b82f6"
                        }
                    }
                };

                string script = $@"
                    <script>
                    window.addEventListener('load', function() {{
                        const ctx = document.getElementById('topQuizzesChart');
                        if (ctx) {{
                            new Chart(ctx, {{
                                type: 'bar',
                                data: {new JavaScriptSerializer().Serialize(data)},
                                options: {{
                                    indexAxis: 'y',
                                    responsive: true,
                                    maintainAspectRatio: false,
                                    plugins: {{ legend: {{ display: false }} }}
                                }}
                            }});
                        }}
                    }});
                    </script>
                ";
                ClientScript.RegisterStartupScript(this.GetType(), "topQuizzesChart", script, false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PrepareTopQuizzesChart hatası: " + ex.Message);
            }
        }

        #endregion

        #region ÇÖZÜM İSTATİSTİKLERİ

        private void LoadSubmissionStatistics()
        {
            try
            {
                // Toplam Çözümler
                lblTotalSubmissions.Text = DbHelper.Scalar("SELECT COUNT(*) FROM Submissions")?.ToString() ?? "0";

                // Bu hafta
                int submissionsWeek = Convert.ToInt32(DbHelper.Scalar(
                    "SELECT COUNT(*) FROM Submissions WHERE SubmittedAt >= DATE_SUB(NOW(), INTERVAL 7 DAY)") ?? 0);
                lblSubmissionsThisWeek.Text = submissionsWeek.ToString();

                // Bugün
                lblTodaySubmissions.Text = DbHelper.Scalar(
                    "SELECT COUNT(*) FROM Submissions WHERE DATE(SubmittedAt) = CURDATE()")?.ToString() ?? "0";

                // Günlük ortalama
                try
                {
                    var avgDaily = DbHelper.Query(@"
                        SELECT AVG(DailyCount) AS AvgDaily
                        FROM (
                            SELECT DATE(SubmittedAt) AS Date, COUNT(*) AS DailyCount
                            FROM Submissions
                            WHERE SubmittedAt >= DATE_SUB(NOW(), INTERVAL 30 DAY)
                            GROUP BY DATE(SubmittedAt)
                        ) AS DailyCounts");

                    if (avgDaily.Rows.Count > 0 && avgDaily.Rows[0]["AvgDaily"] != DBNull.Value)
                    {
                        lblAvgDaily.Text = Math.Round(Convert.ToDouble(avgDaily.Rows[0]["AvgDaily"]), 1).ToString();
                    }
                    else
                    {
                        lblAvgDaily.Text = "0";
                    }
                }
                catch
                {
                    lblAvgDaily.Text = "0";
                }

                // Kullanıcı başına ortalama
                try
                {
                    var avgPerUser = DbHelper.Query(@"
                        SELECT AVG(UserSubmissions) AS AvgPerUser
                        FROM (
                            SELECT UserId, COUNT(*) AS UserSubmissions
                            FROM Submissions
                            GROUP BY UserId
                        ) AS UserCounts");

                    if (avgPerUser.Rows.Count > 0 && avgPerUser.Rows[0]["AvgPerUser"] != DBNull.Value)
                    {
                        lblAvgPerUser.Text = Math.Round(Convert.ToDouble(avgPerUser.Rows[0]["AvgPerUser"]), 1).ToString();
                    }
                    else
                    {
                        lblAvgPerUser.Text = "0";
                    }
                }
                catch
                {
                    lblAvgPerUser.Text = "0";
                }

                // En aktif kullanıcı
                try
                {
                    var topUser = DbHelper.Query(@"
                        SELECT u.UserName, COUNT(s.Id) AS SubmissionCount
                        FROM Users u
                        INNER JOIN Submissions s ON u.Id = s.UserId
                        GROUP BY u.Id, u.UserName
                        ORDER BY SubmissionCount DESC
                        LIMIT 1");

                    if (topUser.Rows.Count > 0)
                    {
                        lblTopUser.Text = topUser.Rows[0]["UserName"].ToString();
                        lblTopUserCount.Text = topUser.Rows[0]["SubmissionCount"].ToString();
                    }
                    else
                    {
                        lblTopUser.Text = "N/A";
                        lblTopUserCount.Text = "0";
                    }
                }
                catch
                {
                    lblTopUser.Text = "N/A";
                    lblTopUserCount.Text = "0";
                }

                // Son çözüm
                try
                {
                    var lastSubmission = DbHelper.Query(@"
                        SELECT SubmittedAt 
                        FROM Submissions 
                        ORDER BY SubmittedAt DESC 
                        LIMIT 1");

                    if (lastSubmission.Rows.Count > 0)
                    {
                        DateTime lastTime = Convert.ToDateTime(lastSubmission.Rows[0]["SubmittedAt"]);
                        TimeSpan diff = DateTime.Now - lastTime;

                        if (diff.TotalMinutes < 60)
                        {
                            lblLastSubmission.Text = $"{(int)diff.TotalMinutes} dk önce";
                        }
                        else if (diff.TotalHours < 24)
                        {
                            lblLastSubmission.Text = $"{(int)diff.TotalHours} saat önce";
                        }
                        else
                        {
                            lblLastSubmission.Text = lastTime.ToString("dd.MM HH:mm");
                        }
                    }
                    else
                    {
                        lblLastSubmission.Text = "N/A";
                    }
                }
                catch
                {
                    lblLastSubmission.Text = "N/A";
                }

                // Grafik verileri
                PrepareSubmissionTrendChart();
                PrepareHourlyDistributionChart();
                PrepareWeeklyDistributionChart();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadSubmissionStatistics hatası: " + ex.Message);
            }
        }

        private void PrepareSubmissionTrendChart()
        {
            try
            {
                var trend = DbHelper.Query(@"
                    SELECT 
                        DATE(SubmittedAt) AS Date,
                        COUNT(*) AS Count
                    FROM Submissions
                    WHERE SubmittedAt >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)
                    GROUP BY DATE(SubmittedAt)
                    ORDER BY Date");

                var labels = new List<string>();
                var counts = new List<int>();

                for (int i = 6; i >= 0; i--)
                {
                    var date = DateTime.Today.AddDays(-i);
                    labels.Add(date.ToString("dd MMM"));

                    int count = 0;
                    foreach (DataRow row in trend.Rows)
                    {
                        if (Convert.ToDateTime(row["Date"]).Date == date.Date)
                        {
                            count = Convert.ToInt32(row["Count"]);
                            break;
                        }
                    }
                    counts.Add(count);
                }

                var data = new
                {
                    labels = labels.ToArray(),
                    datasets = new[]
                    {
                        new
                        {
                            label = "Çözümler",
                            data = counts.ToArray(),
                            borderColor = "#8b5cf6",
                            backgroundColor = "rgba(139, 92, 246, 0.1)",
                            fill = true
                        }
                    }
                };

                string script = $@"
                    <script>
                    window.addEventListener('load', function() {{
                        const ctx = document.getElementById('submissionTrendChart');
                        if (ctx) {{
                            new Chart(ctx, {{
                                type: 'line',
                                data: {new JavaScriptSerializer().Serialize(data)},
                                options: {{
                                    responsive: true,
                                    maintainAspectRatio: false,
                                    plugins: {{ legend: {{ display: false }} }}
                                }}
                            }});
                        }}
                    }});
                    </script>
                ";
                ClientScript.RegisterStartupScript(this.GetType(), "submissionTrendChart", script, false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PrepareSubmissionTrendChart hatası: " + ex.Message);
            }
        }

        private void PrepareHourlyDistributionChart()
        {
            try
            {
                var hourly = DbHelper.Query(@"
                    SELECT 
                        HOUR(SubmittedAt) AS Hour,
                        COUNT(*) AS Count
                    FROM Submissions
                    GROUP BY HOUR(SubmittedAt)
                    ORDER BY Hour");

                var labels = new List<string>();
                var counts = new List<int>();

                for (int i = 0; i < 24; i++)
                {
                    labels.Add($"{i:D2}:00");

                    int count = 0;
                    foreach (DataRow row in hourly.Rows)
                    {
                        if (Convert.ToInt32(row["Hour"]) == i)
                        {
                            count = Convert.ToInt32(row["Count"]);
                            break;
                        }
                    }
                    counts.Add(count);
                }

                var data = new
                {
                    labels = labels.ToArray(),
                    datasets = new[]
                    {
                        new
                        {
                            label = "Çözüm Sayısı",
                            data = counts.ToArray(),
                            backgroundColor = "#f59e0b"
                        }
                    }
                };

                string script = $@"
                    <script>
                    window.addEventListener('load', function() {{
                        const ctx = document.getElementById('hourlyDistributionChart');
                        if (ctx) {{
                            new Chart(ctx, {{
                                type: 'bar',
                                data: {new JavaScriptSerializer().Serialize(data)},
                                options: {{
                                    responsive: true,
                                    maintainAspectRatio: false,
                                    plugins: {{ legend: {{ display: false }} }}
                                }}
                            }});
                        }}
                    }});
                    </script>
                ";
                ClientScript.RegisterStartupScript(this.GetType(), "hourlyDistributionChart", script, false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PrepareHourlyDistributionChart hatası: " + ex.Message);
            }
        }

        private void PrepareWeeklyDistributionChart()
        {
            try
            {
                var weekly = DbHelper.Query(@"
                    SELECT 
                        DAYOFWEEK(SubmittedAt) AS DayOfWeek,
                        COUNT(*) AS Count
                    FROM Submissions
                    GROUP BY DAYOFWEEK(SubmittedAt)
                    ORDER BY DayOfWeek");

                var dayNames = new[] { "Pazar", "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma", "Cumartesi" };
                var labels = new List<string>();
                var counts = new List<int>();

                for (int i = 1; i <= 7; i++)
                {
                    labels.Add(dayNames[i == 1 ? 0 : i - 1]);

                    int count = 0;
                    foreach (DataRow row in weekly.Rows)
                    {
                        if (Convert.ToInt32(row["DayOfWeek"]) == i)
                        {
                            count = Convert.ToInt32(row["Count"]);
                            break;
                        }
                    }
                    counts.Add(count);
                }

                var data = new
                {
                    labels = labels.ToArray(),
                    datasets = new[]
                    {
                        new
                        {
                            data = counts.ToArray(),
                            backgroundColor = new[] { "#ef4444", "#f59e0b", "#10b981", "#3b82f6", "#8b5cf6", "#ec4899", "#6b7280" }
                        }
                    }
                };

                string script = $@"
                    <script>
                    window.addEventListener('load', function() {{
                        const ctx = document.getElementById('weeklyDistributionChart');
                        if (ctx) {{
                            new Chart(ctx, {{
                                type: 'pie',
                                data: {new JavaScriptSerializer().Serialize(data)},
                                options: {{
                                    responsive: true,
                                    maintainAspectRatio: false,
                                    plugins: {{ legend: {{ position: 'bottom' }} }}
                                }}
                            }});
                        }}
                    }});
                    </script>
                ";
                ClientScript.RegisterStartupScript(this.GetType(), "weeklyDistributionChart", script, false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PrepareWeeklyDistributionChart hatası: " + ex.Message);
            }
        }

        #endregion

        #region SİSTEM İSTATİSTİKLERİ

        private void LoadSystemStatistics()
        {
            try
            {
                // Bekleyen silme talepleri
                lblPendingDeletes.Text = DbHelper.Scalar("SELECT COUNT(*) FROM DeleteRequests WHERE Status='pending'")?.ToString() ?? "0";

                // Onaylanan talepler
                lblApprovedDeletes.Text = DbHelper.Scalar("SELECT COUNT(*) FROM DeleteRequests WHERE Status='approved'")?.ToString() ?? "0";

                // Veritabanı boyutu
                try
                {
                    var dbSize = DbHelper.Query(@"
                        SELECT 
                            SUM(data_length + index_length) / 1024 / 1024 AS SizeMB
                        FROM information_schema.tables
                        WHERE table_schema = DATABASE()");

                    if (dbSize.Rows.Count > 0 && dbSize.Rows[0]["SizeMB"] != DBNull.Value)
                    {
                        lblDatabaseSize.Text = Math.Round(Convert.ToDouble(dbSize.Rows[0]["SizeMB"]), 2).ToString() + " MB";
                    }
                    else
                    {
                        lblDatabaseSize.Text = "N/A";
                    }
                }
                catch
                {
                    lblDatabaseSize.Text = "N/A";
                }

                // Tablo sayısı
                try
                {
                    var tableCount = DbHelper.Query(@"
                        SELECT COUNT(*) AS TableCount
                        FROM information_schema.tables
                        WHERE table_schema = DATABASE()");

                    if (tableCount.Rows.Count > 0)
                    {
                        lblTableCount.Text = tableCount.Rows[0]["TableCount"].ToString();
                    }
                    else
                    {
                        lblTableCount.Text = "N/A";
                    }
                }
                catch
                {
                    lblTableCount.Text = "N/A";
                }

                // Admin sayısı
                lblAdminCount.Text = DbHelper.Scalar("SELECT COUNT(*) FROM Users WHERE Role='admin'")?.ToString() ?? "0";

                // Platform yaşı
                try
                {
                    var firstUser = DbHelper.Query(@"
                        SELECT MIN(CreatedAt) AS FirstDate
                        FROM Users");

                    if (firstUser.Rows.Count > 0 && firstUser.Rows[0]["FirstDate"] != DBNull.Value)
                    {
                        DateTime firstDate = Convert.ToDateTime(firstUser.Rows[0]["FirstDate"]);
                        TimeSpan age = DateTime.Now - firstDate;

                        if (age.TotalDays < 30)
                        {
                            lblPlatformAge.Text = $"{(int)age.TotalDays} gün";
                        }
                        else if (age.TotalDays < 365)
                        {
                            lblPlatformAge.Text = $"{(int)(age.TotalDays / 30)} ay";
                        }
                        else
                        {
                            lblPlatformAge.Text = $"{(int)(age.TotalDays / 365)} yıl";
                        }
                    }
                    else
                    {
                        lblPlatformAge.Text = "N/A";
                    }
                }
                catch
                {
                    lblPlatformAge.Text = "N/A";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadSystemStatistics hatası: " + ex.Message);
            }
        }

        #endregion
    }
}