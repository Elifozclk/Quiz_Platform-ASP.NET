using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace QP_WEBPROJECT.vs2.Pages.Admin
{
    public partial class QuizManagement : System.Web.UI.Page
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
                LoadStatistics();
                LoadQuizzes();
            }
        }

        /// <summary>
        /// İstatistikleri yükle
        /// </summary>
        private void LoadStatistics()
        {
            try
            {
                // Toplam quiz sayısı
                var totalQuizzes = DbHelper.Scalar("SELECT COUNT(*) FROM Quizzes");
                lblTotalQuizzes.Text = totalQuizzes?.ToString() ?? "0";

                // Yayınlanan quiz sayısı
                var publishedQuizzes = DbHelper.Scalar("SELECT COUNT(*) FROM Quizzes WHERE Status='Published'");
                lblPublishedQuizzes.Text = publishedQuizzes?.ToString() ?? "0";

                // Taslak quiz sayısı
                var draftQuizzes = DbHelper.Scalar("SELECT COUNT(*) FROM Quizzes WHERE Status='Draft'");
                lblDraftQuizzes.Text = draftQuizzes?.ToString() ?? "0";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("İstatistik yükleme hatası: " + ex.Message);
            }
        }

        /// <summary>
        /// Quiz'leri yükle (filtrelemeli)
        /// </summary>
        private void LoadQuizzes()
        {
            try
            {
                string statusFilter = ddlStatusFilter.SelectedValue;
                string searchText = txtSearch.Text.Trim();

                string sql = @"
                    SELECT 
                        q.Id,
                        q.Title,
                        q.Description,
                        q.Status,
                        q.CreatedAt,
                        u.UserName AS CreatorName,
                        (SELECT COUNT(*) FROM Questions WHERE QuizId = q.Id) AS QuestionCount
                    FROM Quizzes q
                    LEFT JOIN Users u ON q.CreatedBy = u.Id
                    WHERE 1=1";

                var parameters = new System.Collections.Generic.List<MySqlParameter>();

                // Durum filtresi
                if (statusFilter != "all")
                {
                    sql += " AND q.Status = @status";
                    parameters.Add(new MySqlParameter("@status", statusFilter));
                }

                // Arama filtresi
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    sql += " AND (q.Title LIKE @search OR q.Description LIKE @search)";
                    parameters.Add(new MySqlParameter("@search", "%" + searchText + "%"));
                }

                sql += " ORDER BY q.CreatedAt DESC";

                DataTable dt = DbHelper.Query(sql, parameters.ToArray());
                gvQuizzes.DataSource = dt;
                gvQuizzes.DataBind();
            }
            catch (Exception ex)
            {
                ShowMessage("Quiz'ler yüklenirken hata oluştu: " + ex.Message, "danger");
            }
        }

        /// <summary>
        /// Filtrele butonu
        /// </summary>
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadQuizzes();
        }

        /// <summary>
        /// GridView komutları
        /// </summary>
        protected void gvQuizzes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int quizId = Convert.ToInt32(e.CommandArgument);

                if (e.CommandName == "EditQuiz")
                {
                    // ✅ DÜZENLE: QuizCreate.aspx'e yönlendir
                    Response.Redirect($"/Pages/Admin/QuizCreate.aspx?id={quizId}");
                }
                else if (e.CommandName == "ViewQuiz")
                {
                   
                    string url = $"/Pages/Public/SolveQuiz.aspx?id={quizId}";

                    // JavaScript ile yeni pencerede aç
                    string script = $@"
        var newWindow = window.open('{url}', '_blank');
        if (!newWindow) {{
            alert('Popup engelleyici aktif! Aynı pencerede açılıyor...');
            window.location.href = '{url}';
        }}
    ";

                    ScriptManager.RegisterStartupScript(this, GetType(),
                        "openQuiz" + quizId, script, true);
                }
                else if (e.CommandName == "ToggleStatus")
                {
                    // Durumu değiştir (Published <-> Draft)
                    ToggleQuizStatus(quizId);
                    LoadQuizzes(); // Listeyi yenile
                }
                else if (e.CommandName == "DeleteQuiz")
                {
                    // Quiz'i sil
                    DeleteQuiz(quizId);
                    LoadQuizzes(); // Listeyi yenile
                }
            }
            catch (Exception ex)
            {
                // Hata mesajı göster
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"alert('Hata: {ex.Message}');", true);
            }
        }

        /// <summary>
        /// Quiz durumunu değiştir (Published ↔ Draft)
        /// </summary>
        private void ToggleQuizStatus(int quizId)
        {
            try
            {
                // Mevcut durumu al
                string sql = "SELECT Status FROM Quizzes WHERE Id=@id";
                var currentStatus = DbHelper.Scalar(sql, new MySqlParameter("@id", quizId))?.ToString();

                // Yeni durum
                string newStatus = currentStatus == "Published" ? "Draft" : "Published";

                // Güncelle
                string updateSql = @"UPDATE Quizzes 
                            SET Status=@status, 
                                PublishedAt=@publishedAt 
                            WHERE Id=@id";

                DbHelper.Execute(updateSql,
                    new MySqlParameter("@status", newStatus),
                    new MySqlParameter("@publishedAt", newStatus == "Published" ? (object)DateTime.Now : DBNull.Value),
                    new MySqlParameter("@id", quizId));

                // Başarı mesajı
                string message = newStatus == "Published"
                    ? "Quiz başarıyla yayınlandı!"
                    : "Quiz taslağa alındı!";

                ScriptManager.RegisterStartupScript(this, GetType(), "success",
                    $"alert('{message}');", true);

                System.Diagnostics.Debug.WriteLine($"Quiz {quizId} durumu: {currentStatus} → {newStatus}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ToggleQuizStatus Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Quiz'i sil (CASCADE DELETE)
        /// </summary>
        private void DeleteQuiz(int quizId)
        {
            try
            {
                // ✅ DOĞRU CASCADE SIRALAMA:
                // 1. SubmissionAnswers (OptionId → Options, SubmissionId → Submissions)
                // 2. Submissions (QuizId → Quizzes)
                // 3. Options (QuestionId → Questions)
                // 4. Questions (QuizId → Quizzes)
                // 5. Results (QuizId → Quizzes)
                // 6. Quizzes

                System.Diagnostics.Debug.WriteLine($"========== Quiz {quizId} SİLİNİYOR ==========");

                // 1️⃣ SubmissionAnswers silme (En alttaki tablo)
                string deleteSubmissionAnswers = @"
            DELETE FROM SubmissionAnswers 
            WHERE SubmissionId IN (
                SELECT Id FROM Submissions WHERE QuizId=@quizId
            )";
                int affectedSubmissionAnswers = DbHelper.Execute(deleteSubmissionAnswers, new MySqlParameter("@quizId", quizId));
                System.Diagnostics.Debug.WriteLine($"✅ {affectedSubmissionAnswers} SubmissionAnswers silindi");

                // 2️⃣ Submissions silme
                string deleteSubmissions = "DELETE FROM Submissions WHERE QuizId=@quizId";
                int affectedSubmissions = DbHelper.Execute(deleteSubmissions, new MySqlParameter("@quizId", quizId));
                System.Diagnostics.Debug.WriteLine($"✅ {affectedSubmissions} Submissions silindi");

                // 3️⃣ Options silme
                string deleteOptions = @"
            DELETE FROM Options 
            WHERE QuestionId IN (
                SELECT Id FROM Questions WHERE QuizId=@quizId
            )";
                int affectedOptions = DbHelper.Execute(deleteOptions, new MySqlParameter("@quizId", quizId));
                System.Diagnostics.Debug.WriteLine($"✅ {affectedOptions} Options silindi");

                // 4️⃣ Questions silme
                string deleteQuestions = "DELETE FROM Questions WHERE QuizId=@quizId";
                int affectedQuestions = DbHelper.Execute(deleteQuestions, new MySqlParameter("@quizId", quizId));
                System.Diagnostics.Debug.WriteLine($"✅ {affectedQuestions} Questions silindi");

                // 5️⃣ Results silme
                string deleteResults = "DELETE FROM Results WHERE QuizId=@quizId";
                int affectedResults = DbHelper.Execute(deleteResults, new MySqlParameter("@quizId", quizId));
                System.Diagnostics.Debug.WriteLine($"✅ {affectedResults} Results silindi");

                // 6️⃣ Quiz silme (En son)
                string deleteQuiz = "DELETE FROM Quizzes WHERE Id=@quizId";
                DbHelper.Execute(deleteQuiz, new MySqlParameter("@quizId", quizId));
                System.Diagnostics.Debug.WriteLine($"✅ Quiz {quizId} silindi");

                System.Diagnostics.Debug.WriteLine($"========== SİLME TAMAMLANDI ==========");

                // Başarı mesajı
                ScriptManager.RegisterStartupScript(this, GetType(), "deleted",
                    "alert('Quiz başarıyla silindi!');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ DeleteQuiz Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// GridView satır oluşturulurken
        /// </summary>
        protected void gvQuizzes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Yayınla/Taslak butonu için metin değiştir
                Button btnToggleStatus = (Button)e.Row.FindControl("btnToggleStatus");

                if (btnToggleStatus != null)
                {
                    string status = DataBinder.Eval(e.Row.DataItem, "Status").ToString();

                    if (status == "Published")
                    {
                        btnToggleStatus.Text = "Taslağa Al";
                        btnToggleStatus.CssClass = "btn-action btn-draft";
                    }
                    else
                    {
                        btnToggleStatus.Text = "Yayınla";
                        btnToggleStatus.CssClass = "btn-action btn-publish";
                    }
                }
            }
        }

        /// <summary>
        /// Durum badge'i oluştur
        /// </summary>
        protected string GetStatusBadge(string status)
        {
            if (string.IsNullOrEmpty(status))
                return "<span class='status-badge draft'>Taslak</span>";

            if (status == "Published")
                return "<span class='status-badge published'><i class='fa fa-check-circle'></i> Yayınlandı</span>";
            else
                return "<span class='status-badge draft'><i class='fa fa-file-pen'></i> Taslak</span>";
        }

        /// <summary>
        /// Mesaj göster
        /// </summary>
        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = $"alert alert-{type}";
            lblMessage.Visible = true;
        }
    }
}