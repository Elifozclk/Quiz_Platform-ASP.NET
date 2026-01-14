using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace QP_WEBPROJECT.vs2.Pages.Admin
{
    public partial class QuizCreate : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.Button btnCancelResultEdit;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Admin kontrolü
                if (Session["UserType"]?.ToString() != "admin")
                {
                    Response.Redirect("/Pages/User/Login.aspx");
                    return;
                }

                // ✅ Quiz ID parametresi varsa (düzenleme modu)
                if (Request.QueryString["id"] != null)
                {
                    try
                    {
                        int quizId = Convert.ToInt32(Request.QueryString["id"]);
                        System.Diagnostics.Debug.WriteLine($"=== SAYFA İLK YÜKLEME - DÜZENLEME MODU ===");
                        System.Diagnostics.Debug.WriteLine($"Quiz ID: {quizId}");

                        hfCurrentQuizId.Value = quizId.ToString();
                        Session["CurrentQuizId"] = quizId;

                        LoadQuizForEdit(quizId);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Quiz ID çevirme hatası: {ex.Message}");
                        lblMessage1.Text = $"<span class='text-danger'>❌ Geçersiz Quiz ID</span>";
                    }
                }

                if (Request.QueryString["status"] != null)
                {
                    string status = Request.QueryString["status"];
                    LoadQuizList(status);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("=== POSTBACK ===");

                // PostBack: HiddenField boşsa Session'dan al
                if ((string.IsNullOrEmpty(hfCurrentQuizId.Value) || hfCurrentQuizId.Value == "0")
                    && Session["CurrentQuizId"] != null)
                {
                    hfCurrentQuizId.Value = Session["CurrentQuizId"].ToString();
                }

                if (!string.IsNullOrEmpty(hfCurrentQuizId.Value) && hfCurrentQuizId.Value != "0")
                {
                    int quizId = Convert.ToInt32(hfCurrentQuizId.Value);
                    ReloadQuizInfo(quizId);
                }

                // ✅ PostBack sonrası aktif adımı geri yükle
                if (!string.IsNullOrEmpty(hfCurrentStep.Value))
                {
                    int currentStep = Convert.ToInt32(hfCurrentStep.Value);

                    System.Diagnostics.Debug.WriteLine($"PostBack - Current Step: {currentStep}");

                    if (currentStep == 2)
                    {
                        LoadResults();
                        // ✅ Panel 2'yi aktif tut
                        ScriptManager.RegisterStartupScript(this, GetType(), "keepStep2",
                            "setTimeout(function(){ goToStep(2); }, 50);", true);
                    }
                    else if (currentStep == 3)
                    {
                        LoadQuestions();
                        // ✅ Panel 3'ü aktif tut
                        ScriptManager.RegisterStartupScript(this, GetType(), "keepStep3",
                            "setTimeout(function(){ goToStep(3); }, 50);", true);
                    }
                    else if (currentStep == 4)
                    {
                        // ✅ Panel 4'ü aktif tut
                        ScriptManager.RegisterStartupScript(this, GetType(), "keepStep4",
                            "setTimeout(function(){ goToStep(4); }, 50);", true);
                    }
                }
            }
        }

        #region ADIM 1: Quiz Bilgileri

        protected void btnSaveQuiz_Click(object sender, EventArgs e)
        {
            try
            {
                // ✅ DOSYA UPLOAD TESTİ - BAŞLANGIÇ
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine("DOSYA UPLOAD TESTİ BAŞLADI");
                System.Diagnostics.Debug.WriteLine("========================================");

                // FileUpload kontrolünü kontrol et
                System.Diagnostics.Debug.WriteLine($"fuCoverImage kontrolü bulundu mu? {fuCoverImage != null}");
                System.Diagnostics.Debug.WriteLine($"fuCoverImage.HasFile: {fuCoverImage.HasFile}");

                if (fuCoverImage.HasFile)
                {
                    System.Diagnostics.Debug.WriteLine($"Dosya adı: {fuCoverImage.FileName}");
                    System.Diagnostics.Debug.WriteLine($"Dosya boyutu: {fuCoverImage.PostedFile.ContentLength} bytes");
                    System.Diagnostics.Debug.WriteLine($"Content Type: {fuCoverImage.PostedFile.ContentType}");

                    string extension = Path.GetExtension(fuCoverImage.FileName).ToLower();
                    System.Diagnostics.Debug.WriteLine($"Uzantı: {extension}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("❌❌❌ DOSYA SEÇİLMEDİ VEYA GELMEDİ! ❌❌❌");
                    lblMessage1.Text = "<span class='text-danger'>❌ Dosya seçilmedi veya gelmedi!</span>";
                    return;
                }

                // Session kontrolü
                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("❌ Session[UserId] NULL!");
                    lblMessage1.Text = "<span class='text-danger'>❌ Oturum süresi dolmuş. Lütfen tekrar giriş yapın.</span>";
                    return;
                }

                // CKEditor'dan açıklamayı al
                string description = Request.Unvalidated.Form[txtDescription.UniqueID];
                if (string.IsNullOrEmpty(description))
                {
                    description = txtDescription.Text.Trim();
                }

                string title = txtTitle.Text.Trim();

                if (string.IsNullOrEmpty(title))
                {
                    lblMessage1.Text = "<span class='text-danger'>⚠️ Quiz başlığı boş olamaz!</span>";
                    return;
                }

                string slug = GenerateSlug(title);

                string quizType = "PersonalityClassic";
                string optionType = ddlOptionType.SelectedValue;

                int estimatedTime = string.IsNullOrEmpty(txtEstimatedTime.Text) ? 5 : Convert.ToInt32(txtEstimatedTime.Text);
                bool isAnonymous = chkAnonymousAllowed.Checked;
                bool multipleAttempts = chkMultipleAttempts.Checked;
                string status = chkIsActive.Checked ? "Published" : "Draft";
                int createdBy = Convert.ToInt32(Session["UserId"]);

                // ✅ DOSYA YÜKLEME BAŞLIYOR
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine("UploadFile METODU ÇAĞRILIYOR...");
                System.Diagnostics.Debug.WriteLine("========================================");

                string coverUrl = UploadFile(fuCoverImage, "QuizCovers");

                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine($"UploadFile SONUCU: {coverUrl ?? "NULL"}");
                System.Diagnostics.Debug.WriteLine("========================================");

                if (string.IsNullOrEmpty(hfCurrentQuizId.Value) || hfCurrentQuizId.Value == "0")
                {
                    string sql = @"INSERT INTO Quizzes 
                           (Title, Slug, Description, Status, CreatedBy, CreatedAt, QuizType, OptionType,
                            IsAnonymousAllowed, EstimatedTime, AllowMultipleAttempts, CoverImageUrl, PublishedAt)
                           VALUES 
                           (@title, @slug, @desc, @status, @createdBy, NOW(), @quizType, @optionType,
                            @anonymous, @time, @multiple, @cover, @publishedAt)";

                    var parameters = new MySqlParameter[]
                    {
                new MySqlParameter("@title", title),
                new MySqlParameter("@slug", slug),
                new MySqlParameter("@desc", description ?? ""),
                new MySqlParameter("@status", status),
                new MySqlParameter("@createdBy", createdBy),
                new MySqlParameter("@quizType", quizType),
                new MySqlParameter("@optionType", optionType),
                new MySqlParameter("@anonymous", isAnonymous),
                new MySqlParameter("@time", estimatedTime),
                new MySqlParameter("@multiple", multipleAttempts),
                new MySqlParameter("@cover", coverUrl ?? ""),
                new MySqlParameter("@publishedAt", status == "Published" ? (object)DateTime.Now : DBNull.Value)
                    };

                    System.Diagnostics.Debug.WriteLine("SQL ÇALIŞTIRILIYOR...");
                    DbHelper.Execute(sql, parameters);
                    int quizId = Convert.ToInt32(DbHelper.Scalar("SELECT LAST_INSERT_ID()"));

                    hfCurrentQuizId.Value = quizId.ToString();
                    Session["CurrentQuizId"] = quizId;

                    lblMessage1.Text = "<span class='text-success'>✅ Quiz oluşturuldu! ID: " + quizId +
                                      (string.IsNullOrEmpty(coverUrl) ? "<br/>⚠️ Ama görsel yüklenemedi!" : "<br/>✅ Görsel yüklendi!") + "</span>";
                }
                else
                {
                    int quizId = Convert.ToInt32(hfCurrentQuizId.Value);

                    // ⭐ YENİ: Eski kapak görselini al
                    string oldCoverUrl = null;
                    var dtOldCover = DbHelper.Query("SELECT CoverImageUrl FROM Quizzes WHERE Id=@id",
                        new MySqlParameter("@id", quizId));
                    if (dtOldCover.Rows.Count > 0)
                    {
                        oldCoverUrl = dtOldCover.Rows[0]["CoverImageUrl"]?.ToString();
                    }

                    string sqlUpdate = @"UPDATE Quizzes SET 
                        Title=@title, 
                        Description=@desc, 
                        EstimatedTime=@time, 
                        QuizType=@type, 
                        OptionType=@optType, 
                        Theme=@theme, 
                        IsAnonymousAllowed=@anon, 
                        AllowMultipleAttempts=@multi, 
                        Status=@status, 
                        Slug=@slug";

                    var paramList = new List<MySqlParameter>
    {
        new MySqlParameter("@title", title),
        new MySqlParameter("@desc", description),
        new MySqlParameter("@time", estimatedTime),
        new MySqlParameter("@type", quizType),
        new MySqlParameter("@optType", optionType),
        new MySqlParameter("@theme", ddlTheme.SelectedValue),
        new MySqlParameter("@anon", isAnonymous),
        new MySqlParameter("@multi", multipleAttempts),
        new MySqlParameter("@status", status),
        new MySqlParameter("@slug", slug),
        new MySqlParameter("@id", quizId)
    };

                    if (!string.IsNullOrEmpty(coverUrl))
                    {
                        sqlUpdate += ", CoverImageUrl=@cover";
                        paramList.Add(new MySqlParameter("@cover", coverUrl));

                        // ⭐ YENİ: Eski kapak görselini sil (varsa ve boş değilse)
                        if (!string.IsNullOrEmpty(oldCoverUrl))
                        {
                            try
                            {
                                string oldFilePath = Server.MapPath("~/Uploads/QuizCovers/" + System.IO.Path.GetFileName(oldCoverUrl));
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                    System.Diagnostics.Debug.WriteLine($"✅ Eski kapak görseli silindi: {oldFilePath}");
                                }
                            }
                            catch (Exception delEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"⚠️ Eski kapak görseli silinemedi: {delEx.Message}");
                            }
                        }
                    }

                    sqlUpdate += " WHERE Id=@id";

                    DbHelper.Execute(sqlUpdate, paramList.ToArray());

                    lblMessage1.Text = "<span class='text-success'>✅ Quiz başarıyla güncellendi!</span>";
                }

                hfCurrentStep.Value = "2";
                ScriptManager.RegisterStartupScript(this, GetType(), "goToStep2", "goToStep(2);", true);
                int currentQuizId = Convert.ToInt32(hfCurrentQuizId.Value);
                if (currentQuizId > 0)
                {
                    ReloadQuizInfo(currentQuizId);
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"========================================");
                System.Diagnostics.Debug.WriteLine($"❌❌❌ HATA OLUŞTU ❌❌❌");
                System.Diagnostics.Debug.WriteLine($"Mesaj: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                System.Diagnostics.Debug.WriteLine($"========================================");

                lblMessage1.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
            }
        }

        #endregion

        #region ADIM 2: Sonuçlar

        protected void btnAddResult_Click(object sender, EventArgs e)
        {
            try
            {
                if ((string.IsNullOrEmpty(hfCurrentQuizId.Value) || hfCurrentQuizId.Value == "0")
                    && Session["CurrentQuizId"] != null)
                {
                    hfCurrentQuizId.Value = Session["CurrentQuizId"].ToString();
                }

                if (string.IsNullOrEmpty(hfCurrentQuizId.Value) || hfCurrentQuizId.Value == "0")
                {
                    lblMessage2.Text = "<span class='text-warning'>⚠️ Önce quiz oluşturun!</span>";
                    hfCurrentStep.Value = "1";
                    ScriptManager.RegisterStartupScript(this, GetType(), "stay1", "goToStep(1);", true);
                    return;
                }

                int quizId = Convert.ToInt32(hfCurrentQuizId.Value);
                string title = txtResultTitle.Text.Trim();
                string tag = txtResultTag.Text.Trim().ToLower();
                string description = txtResultDescription.Text.Trim();
                string emoji = txtResultEmoji.Text.Trim();

                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(tag))
                {
                    lblMessage2.Text = "<span class='text-warning'>⚠️ Başlık ve etiket zorunlu!</span>";
                    hfCurrentStep.Value = "2";
                    ScriptManager.RegisterStartupScript(this, GetType(), "stay2b", "goToStep(2);", true);
                    return;
                }

                // Görsel yükle
                string imageUrl = UploadFile(fuResultImage, "ResultImages");

                // Düzenleme modunda mı kontrol et
                if (ViewState["EditingResultId"] != null)
                {
                    // GÜNCELLEME
                    int resultId = Convert.ToInt32(ViewState["EditingResultId"]);

                    string oldImageUrl = null;
                    var dtOld = DbHelper.Query("SELECT ImageUrl FROM Results WHERE Id=@id",
                        new MySqlParameter("@id", resultId));
                    if (dtOld.Rows.Count > 0)
                    {
                        oldImageUrl = dtOld.Rows[0]["ImageUrl"]?.ToString();
                    }

                    string sqlUpdate = @"UPDATE Results SET 
                                Title=@title, 
                                Description=@desc, 
                                Tag=@tag, 
                                IconEmoji=@emoji";

                    var paramList = new List<MySqlParameter>
            {
                new MySqlParameter("@title", title),
                new MySqlParameter("@desc", description),
                new MySqlParameter("@tag", tag),
                new MySqlParameter("@emoji", emoji ?? ""),
                new MySqlParameter("@id", resultId)
            };

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        sqlUpdate += ", ImageUrl=@img";
                        paramList.Add(new MySqlParameter("@img", imageUrl));
                    }

                    sqlUpdate += " WHERE Id=@id";

                    DbHelper.Execute(sqlUpdate, paramList.ToArray());

                    lblMessage2.Text = "<span class='text-success'>✅ Sonuç güncellendi!</span>";

                    // Düzenleme modundan çık
                    ViewState["EditingResultId"] = null;
                    btnAddResult.Text = "Ekle";
                    btnCancelResultEdit.Visible = false;

                    pnlExistingResultImage.Visible = false;
                }
                else
                {
                    // YENİ EKLEME
                    string sql = @"INSERT INTO Results 
                           (QuizId, Title, Description, ResultType, Tag, MinScore, MaxScore, ImageUrl, IconEmoji)
                           VALUES (@quizId, @title, @desc, 'personality', @tag, 0, 100, @img, @emoji)";

                    var parameters = new MySqlParameter[]
                    {
                new MySqlParameter("@quizId", quizId),
                new MySqlParameter("@title", title),
                new MySqlParameter("@desc", description),
                new MySqlParameter("@tag", tag),
                new MySqlParameter("@img", imageUrl ?? ""),
                new MySqlParameter("@emoji", emoji ?? "")
                    };

                    DbHelper.Execute(sql, parameters);

                    lblMessage2.Text = "<span class='text-success'>✅ Sonuç eklendi!</span>";
                }

                // Formu temizle
                txtResultTitle.Text = "";
                txtResultTag.Text = "";
                txtResultDescription.Text = "";
                txtResultEmoji.Text = "";

                LoadResults();


            }
            catch (Exception ex)
            {
                // Debug modda hatayı görürsünüz
                System.Diagnostics.Debug.WriteLine($"[HATA - btnAddResult_Click] {ex.ToString()}");

                lblMessage2.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
                hfCurrentStep.Value = "2";
                ScriptManager.RegisterStartupScript(this, GetType(), "stay2err", "goToStep(2);", true);
            }
        }

        protected void btnCancelResultEdit_Click(object sender, EventArgs e)
        {
            // Düzenleme modundan çık
            ViewState["EditingResultId"] = null;
            btnAddResult.Text = "Ekle";
            btnCancelResultEdit.Visible = false;

            // Formu temizle
            txtResultTitle.Text = "";
            txtResultTag.Text = "";
            txtResultDescription.Text = "";
            txtResultEmoji.Text = "";
            pnlExistingResultImage.Visible = false;
            lblMessage2.Text = "<span class='text-info'>✅ Düzenleme iptal edildi.</span>";
            // ✅ hfCurrentStep ve script kaldırıldı
        }
        protected string GetQuestionImagePreview(object imageUrl)
        {
            string imgUrl = imageUrl?.ToString() ?? "";

            if (!string.IsNullOrEmpty(imgUrl))
            {
                string fileName = Path.GetFileName(imgUrl);
                string fullPath = ResolveUrl($"~/Uploads/QuestionImages/{fileName}");

                System.Diagnostics.Debug.WriteLine($"GetQuestionImagePreview: {fileName} → {fullPath}");

                return $"<img src='{fullPath}' style='width:50px;height:50px;object-fit:cover;border-radius:8px;' alt='Question' />";
            }
            else
            {
                return "<span class='text-muted'>-</span>";
            }
        }

        protected string GetPreviewResultIcon(object imageUrl, object iconEmoji)
        {
            string imgUrl = imageUrl?.ToString() ?? "";
            string emoji = iconEmoji?.ToString() ?? "";

            if (!string.IsNullOrEmpty(imgUrl))
            {
                string fileName = Path.GetFileName(imgUrl);
                string fullPath = ResolveUrl($"~/Uploads/ResultImages/{fileName}");

                System.Diagnostics.Debug.WriteLine($"Preview Result Image: {fileName} → {fullPath}");

                // Görsel varsa büyük görsel göster
                return $@"
            <div class='result-image-preview' style='margin-bottom: 15px;'>
                <img src='{fullPath}' 
                     style='width:100%; max-height:150px; object-fit:cover; border-radius:12px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);' 
                     alt='Result Image' 
                     onerror=""this.style.display='none'; this.nextElementSibling.style.display='block';"" />
                <div class='result-icon' style='display:none; font-size:3rem;'>{emoji}</div>
            </div>";
            }
            else if (!string.IsNullOrEmpty(emoji))
            {
                // Emoji varsa emoji göster
                return $"<div class='result-icon' style='font-size:3rem; margin-bottom:15px;'>{emoji}</div>";
            }
            else
            {
                // Hiçbiri yoksa varsayılan ikon
                return "<div class='result-icon' style='font-size:3rem; margin-bottom:15px;'>🏆</div>";
            }
        }

        protected string GetOptionImagePreview(object imageUrl)
        {
            string imgUrl = imageUrl?.ToString() ?? "";

            if (!string.IsNullOrEmpty(imgUrl))
            {
                string fileName = Path.GetFileName(imgUrl);
                string fullPath = ResolveUrl($"~/Uploads/OptionImages/{fileName}");

                System.Diagnostics.Debug.WriteLine($"GetOptionImagePreview: {fileName} → {fullPath}");

                return $"<img src='{fullPath}' style='width:50px;height:50px;object-fit:cover;border-radius:8px;' alt='Option' />";
            }
            else
            {
                return "<span class='text-muted'>-</span>";
            }
        }


        protected string GetResultImagePreview(object imageUrl, object iconEmoji)
        {
            string imgUrl = imageUrl?.ToString() ?? "";
            string emoji = iconEmoji?.ToString() ?? "";

            if (!string.IsNullOrEmpty(imgUrl))
            {
                // Dosya adını al
                string fileName = Path.GetFileName(imgUrl);

                // Tam yolu oluştur
                string fullPath = ResolveUrl($"~/Uploads/ResultImages/{fileName}");

                System.Diagnostics.Debug.WriteLine($"GetResultImagePreview: {fileName} → {fullPath}");

                return $"<img src='{fullPath}' style='width:50px;height:50px;object-fit:cover;border-radius:8px;' alt='Result' />";
            }
            else if (!string.IsNullOrEmpty(emoji))
            {
                return $"<span style='font-size:2rem;'>{emoji}</span>";
            }
            else
            {
                return "<span class='text-muted'>-</span>";
            }
        }

        protected void gvResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteResult")
            {
                try
                {
                    int resultId = Convert.ToInt32(e.CommandArgument);
                    DbHelper.Execute("DELETE FROM Results WHERE Id=@id", new MySqlParameter("@id", resultId));
                    lblMessage2.Text = "<span class='text-success'>✅ Sonuç silindi!</span>";
                    LoadResults();
                    // ✅ hfCurrentStep ve script kaldırıldı
                }
                catch (Exception ex)
                {
                    lblMessage2.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
                    // ✅ hfCurrentStep ve script kaldırıldı
                }
            }
            else if (e.CommandName == "EditResult")
            {
                try
                {
                    int resultId = Convert.ToInt32(e.CommandArgument);

                    string sql = "SELECT * FROM Results WHERE Id=@id";
                    var dt = DbHelper.Query(sql, new MySqlParameter("@id", resultId));

                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        txtResultTitle.Text = row["Title"].ToString();
                        txtResultTag.Text = row["Tag"].ToString();
                        txtResultDescription.Text = row["Description"].ToString();
                        txtResultEmoji.Text = row["IconEmoji"].ToString();
                        string imageUrl = row["ImageUrl"]?.ToString();
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            pnlExistingResultImage.Visible = true;
                            lblExistingResultImage.Text = $"✅ Mevcut görsel: {System.IO.Path.GetFileName(imageUrl)}";

                            // Görseli önizleme alanına yükle
                            string resultImageScript = $@"
                    setTimeout(function() {{
                        var previewContainer = document.querySelector('#previewResultImage .preview-image-container');
                        if (previewContainer) {{
                            previewContainer.innerHTML = '<img src=""{imageUrl}"" style=""max-width: 100%; border-radius: 8px;"" />';
                            document.getElementById('previewResultImage').style.display = 'block';
                        }}
                    }}, 300);
                ";
                            ScriptManager.RegisterStartupScript(this, GetType(), "loadResultImagePreview", resultImageScript, true);
                        }
                        else
                        {
                            pnlExistingResultImage.Visible = true;
                            lblExistingResultImage.Text = "⚠️ Daha önce görsel eklenmemiş";
                            lblExistingResultImage.ForeColor = System.Drawing.Color.Orange;
                        }

                        // Düzenleme modunu işaretle
                        ViewState["EditingResultId"] = resultId;
                        btnAddResult.Text = "💾 Güncelle";
                        btnCancelResultEdit.Visible = true;

                        lblMessage2.Text = "<span class='text-info'>✏️ Düzenleme modunda... Değişikliklerinizi yapıp 'Güncelle' butonuna basın.</span>";
                    }

                    LoadResults();
                    // ✅ hfCurrentStep ve script kaldırıldı
                }
                catch (Exception ex)
                {
                    lblMessage2.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
                    // ✅ hfCurrentStep ve script kaldırıldı
                }
            }
        }

        protected void btnGoToQuestions_Click(object sender, EventArgs e)
        {
            LoadResults();

            if (gvResults.Rows.Count < 2)
            {
                lblMessage2.Text = "<span class='text-warning'>⚠️ En az 2 sonuç tanımlamalısınız!</span>";
                hfCurrentStep.Value = "2";
                ScriptManager.RegisterStartupScript(this, GetType(), "stay2min", "goToStep(2);", true);
                return;
            }

            hfCurrentStep.Value = "3";
            ScriptManager.RegisterStartupScript(this, GetType(), "goToStep3", "goToStep(3);", true);
        }

        private void LoadResults()
        {
            if (string.IsNullOrEmpty(hfCurrentQuizId.Value) || hfCurrentQuizId.Value == "0") return;

            string sql = @"SELECT 
    r.Id, 
    r.Title, 
    r.Tag, 
    r.Description, 
    r.ImageUrl,
    r.IconEmoji,
    r.ResultType,
    (SELECT COUNT(*) FROM Questions WHERE QuizId = r.QuizId) AS QuestionCount
FROM Results r
WHERE r.QuizId=@quizId AND r.ResultType='personality' 
ORDER BY r.Id";

            var dt = DbHelper.Query(sql, new MySqlParameter("@quizId", hfCurrentQuizId.Value));
            gvResults.DataSource = dt;
            gvResults.DataBind();

            lblMessage2.Text = $"<span class='text-success'>{dt.Rows.Count} sonuç yüklendi</span>";

            // ✅ HER SONUÇ İÇİN MEVCUT GÖRSELLERİ JAVASCRIPT İLE GÖSTER
            foreach (DataRow row in dt.Rows)
            {
                string imageUrl = row["ImageUrl"]?.ToString();
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    string fileName = Path.GetFileName(imageUrl);
                    string fullUrl = ResolveUrl($"~/Uploads/ResultImages/{fileName}");

                    System.Diagnostics.Debug.WriteLine($"Result Image: {fileName}");

                    string script = $@"
            console.log('Loading result image: {fileName}');
        ";
                    ScriptManager.RegisterStartupScript(this, GetType(),
                        $"loadResultImage_{row["Id"]}", script, true);
                }
            }

        }

        #endregion

        #region ADIM 3: Sorular ve Seçenekler

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            try
            {
                if ((string.IsNullOrEmpty(hfCurrentQuizId.Value) || hfCurrentQuizId.Value == "0")
                    && Session["CurrentQuizId"] != null)
                {
                    hfCurrentQuizId.Value = Session["CurrentQuizId"].ToString();
                }

                if (string.IsNullOrEmpty(hfCurrentQuizId.Value) || hfCurrentQuizId.Value == "0")
                {
                    lblMessage3.Text = "<span class='text-warning'>⚠️ Önce quiz oluşturun!</span>";
                    hfCurrentStep.Value = "1";
                    ScriptManager.RegisterStartupScript(this, GetType(), "goStep1", "goToStep(1);", true);
                    return;
                }

                string text = txtQuestionText.Text.Trim();
                if (string.IsNullOrEmpty(text))
                {
                    lblMessage3.Text = "<span class='text-warning'>⚠️ Soru metni boş olamaz!</span>";
                    hfCurrentStep.Value = "3";
                    ScriptManager.RegisterStartupScript(this, GetType(), "stay3a", "goToStep(3);", true);
                    return;
                }

                int quizId = Convert.ToInt32(hfCurrentQuizId.Value);
                decimal pointMultiplier = Convert.ToDecimal(txtPointMultiplier.Text);
                string imageUrl = UploadFile(fuQuestionImage, "QuestionImages");

                // Düzenleme modunda mı?
                if (ViewState["EditingQuestionId"] != null)
                {
                    // GÜNCELLEME
                    int questionId = Convert.ToInt32(ViewState["EditingQuestionId"]);
                    int orderNo = Convert.ToInt32(txtQuestionOrder.Text);

                    // ⭐ Eski görseli al
                    string oldImageUrl = null;
                    var dtOldImage = DbHelper.Query("SELECT ImageUrl FROM Questions WHERE Id=@id",
                        new MySqlParameter("@id", questionId));
                    if (dtOldImage.Rows.Count > 0)
                    {
                        oldImageUrl = dtOldImage.Rows[0]["ImageUrl"]?.ToString();
                    }

                    string sqlUpdate = @"UPDATE Questions SET 
        Text=@text, 
        OrderNo=@order, 
        PointsMultiplier=@points";

                    var paramList = new List<MySqlParameter>
    {
        new MySqlParameter("@text", text),
        new MySqlParameter("@order", orderNo),
        new MySqlParameter("@points", pointMultiplier),
        new MySqlParameter("@id", questionId)
    };

                    // ⭐ YENİ: Görseli kaldır checkbox'ı işaretliyse
                    if (chkRemoveQuestionImage.Checked)
                    {
                        sqlUpdate += ", MediaType='none', ImageUrl=NULL";

                        // Eski görseli fiziksel olarak sil
                        if (!string.IsNullOrEmpty(oldImageUrl))
                        {
                            try
                            {
                                string oldFilePath = Server.MapPath("~/Uploads/QuestionImages/" + System.IO.Path.GetFileName(oldImageUrl));
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                    System.Diagnostics.Debug.WriteLine($"✅ Soru görseli silindi: {oldFilePath}");
                                }
                            }
                            catch (Exception delEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"⚠️ Soru görseli silinemedi: {delEx.Message}");
                            }
                        }
                    }
                    // Yeni görsel seçildiyse
                    else if (!string.IsNullOrEmpty(imageUrl))
                    {
                        sqlUpdate += ", MediaType='image', ImageUrl=@imageUrl";
                        paramList.Add(new MySqlParameter("@imageUrl", imageUrl));

                        // Eski görseli sil
                        if (!string.IsNullOrEmpty(oldImageUrl))
                        {
                            try
                            {
                                string oldFilePath = Server.MapPath("~/Uploads/QuestionImages/" + System.IO.Path.GetFileName(oldImageUrl));
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                    System.Diagnostics.Debug.WriteLine($"✅ Eski soru görseli silindi: {oldFilePath}");
                                }
                            }
                            catch (Exception delEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"⚠️ Eski soru görseli silinemedi: {delEx.Message}");
                            }
                        }

                        System.Diagnostics.Debug.WriteLine($"✅ [btnAddQuestion] Soru güncellendi - ImageUrl: {imageUrl}");
                    }

                    sqlUpdate += " WHERE Id=@id";
                    DbHelper.Execute(sqlUpdate, paramList.ToArray());

                    lblMessage3.Text = "<span class='text-success'>✅ Soru güncellendi!</span>";

                    // Düzenleme modundan çık
                    ViewState["EditingQuestionId"] = null;
                    btnAddQuestion.Text = "Soru Ekle";
                    btnCancelQuestionEdit.Visible = false;
                    pnlExistingQuestionImage.Visible = false;
                    chkRemoveQuestionImage.Checked = false;  // ⭐ Checkbox'ı sıfırla
                }
                else
                {
                    // YENİ EKLEME
                    string sqlMaxOrder = "SELECT COALESCE(MAX(OrderNo), 0) FROM Questions WHERE QuizId=@qid";
                    int nextOrder = Convert.ToInt32(DbHelper.Scalar(sqlMaxOrder, new MySqlParameter("@qid", quizId))) + 1;

                    // ✅ ImageUrl parametresi eklendi
                    string sql = @"INSERT INTO Questions 
                   (QuizId, Text, OrderNo, QuestionType, PointsMultiplier, MediaType, ImageUrl)
                   VALUES (@quizId, @text, @order, 'Standard', @points, @media, @imageUrl)";

                    var parameters = new MySqlParameter[]
                    {
        new MySqlParameter("@quizId", quizId),
        new MySqlParameter("@text", text),
        new MySqlParameter("@order", nextOrder),
        new MySqlParameter("@points", pointMultiplier),
        new MySqlParameter("@media", string.IsNullOrEmpty(imageUrl) ? "none" : "image"),
        new MySqlParameter("@imageUrl", (object)imageUrl ?? DBNull.Value)  // ✅ EKLE
                    };

                    DbHelper.Execute(sql, parameters);

                    System.Diagnostics.Debug.WriteLine($"✅ [btnAddQuestion] Soru eklendi - ImageUrl: {imageUrl ?? "NULL"}");

                    lblMessage3.Text = "<span class='text-success'>✅ Soru eklendi! 'Seçenekleri Yönet' butonuna tıklayın.</span>";
                    txtQuestionOrder.Text = (nextOrder + 1).ToString();
                    pnlExistingQuestionImage.Visible = false;
                }

                txtQuestionText.Text = "";
                LoadQuestions();

                hfCurrentStep.Value = "3";
                ScriptManager.RegisterStartupScript(this, GetType(), "stay3b", "goToStep(3);", true);
            }
            catch (Exception ex)
            {
                // Debug modda hatayı görürsünüz
                System.Diagnostics.Debug.WriteLine($"[HATA - btnAddResult_Click] {ex.ToString()}");

                lblMessage2.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
                hfCurrentStep.Value = "2";
                ScriptManager.RegisterStartupScript(this, GetType(), "stay2err", "goToStep(2);", true);
            }
        }

        protected void btnCancelQuestionEdit_Click(object sender, EventArgs e)
        {
            // Düzenleme modundan çık
            ViewState["EditingQuestionId"] = null;
            btnAddQuestion.Text = "Soru Ekle";
            btnCancelQuestionEdit.Visible = false;

            // Formu temizle
            txtQuestionText.Text = "";
            pnlExistingQuestionImage.Visible = false;
            // Sırayı ayarla
            if (!string.IsNullOrEmpty(hfCurrentQuizId.Value) && hfCurrentQuizId.Value != "0")
            {
                int quizId = Convert.ToInt32(hfCurrentQuizId.Value);
                string sqlMaxOrder = "SELECT COALESCE(MAX(OrderNo), 0) FROM Questions WHERE QuizId=@qid";
                int maxOrder = Convert.ToInt32(DbHelper.Scalar(sqlMaxOrder, new MySqlParameter("@qid", quizId)));
                txtQuestionOrder.Text = (maxOrder + 1).ToString();
            }

            lblMessage3.Text = "<span class='text-info'>✅ Düzenleme iptal edildi.</span>";

            hfCurrentStep.Value = "3";
            ScriptManager.RegisterStartupScript(this, GetType(), "cancelQuestionEdit", "goToStep(3);", true);
        }

        protected void gvQuestions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ManageOptions")
            {
                try
                {
                    int questionId = Convert.ToInt32(e.CommandArgument);
                    hfCurrentQuestionId.Value = questionId.ToString();

                    string sql = "SELECT Text FROM Questions WHERE Id=@id";
                    var dt = DbHelper.Query(sql, new MySqlParameter("@id", questionId));

                    if (dt.Rows.Count > 0)
                    {
                        lblCurrentQuestion.Text = dt.Rows[0]["Text"].ToString();
                        lblQuizOptionType.Text = ddlOptionType.SelectedItem.Text;

                        LoadOptions();

                        // ✅ Seçenek sırasını ayarla - bu sorunun son seçeneğinden sonra
                        string sqlMaxOrder = "SELECT COALESCE(MAX(OrderNo), 0) FROM Options WHERE QuestionId=@qId";
                        int maxOrder = Convert.ToInt32(DbHelper.Scalar(sqlMaxOrder, new MySqlParameter("@qId", questionId)));
                        txtOptionOrder.Text = (maxOrder + 1).ToString();

                        pnlManageOptions.Visible = true;

                        // JavaScript'e sonuçları gönder
                        string sqlResults = @"SELECT Id, Title, Tag 
                                              FROM Results 
                                              WHERE QuizId=@quizId AND ResultType='personality' 
                                              ORDER BY Id";
                        var dtResults = DbHelper.Query(sqlResults, new MySqlParameter("@quizId", hfCurrentQuizId.Value));

                        string resultsJson = JsonConvert.SerializeObject(dtResults);

                        string script = $@"
                            goToStep(3);
                            setTimeout(function() {{
                                loadScoreInputs({resultsJson});
                                document.getElementById('{hfScoreData.ClientID}').value = '{{}}';
                            }}, 100);
                        ";
                        ScriptManager.RegisterStartupScript(this, GetType(), "showPanel", script, true);
                    }
                }
                catch (Exception ex)
                {
                    lblMessage3.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
                }
            }
            else if (e.CommandName == "DeleteQuestion")
            {
                try
                {
                    int questionId = Convert.ToInt32(e.CommandArgument);

                    // Silinecek sorunun OrderNo'sunu al
                    string sqlGetOrder = "SELECT OrderNo FROM Questions WHERE Id=@id";
                    int deletedOrder = Convert.ToInt32(DbHelper.Scalar(sqlGetOrder, new MySqlParameter("@id", questionId)));

                    // Seçenekleri sil
                    DbHelper.Execute("DELETE FROM Options WHERE QuestionId=@id",
                        new MySqlParameter("@id", questionId));

                    // Soruyu sil
                    DbHelper.Execute("DELETE FROM Questions WHERE Id=@id",
                        new MySqlParameter("@id", questionId));

                    // ✅ Silinen sıradan sonraki tüm soruların sırasını 1 azalt
                    string sqlReorder = @"UPDATE Questions 
                              SET OrderNo = OrderNo - 1 
                              WHERE QuizId=@qid AND OrderNo > @deletedOrder";

                    DbHelper.Execute(sqlReorder,
                        new MySqlParameter("@qid", hfCurrentQuizId.Value),
                        new MySqlParameter("@deletedOrder", deletedOrder));

                    lblMessage3.Text = "<span class='text-success'>✅ Soru silindi ve sıralar düzenlendi!</span>";
                    LoadQuestions();

                    // ✅ Sıra numarasını güncelle
                    string sqlMaxOrder = "SELECT COALESCE(MAX(OrderNo), 0) FROM Questions WHERE QuizId=@qid";
                    int maxOrder = Convert.ToInt32(DbHelper.Scalar(sqlMaxOrder, new MySqlParameter("@qid", hfCurrentQuizId.Value)));
                    txtQuestionOrder.Text = (maxOrder + 1).ToString();

                    hfCurrentStep.Value = "3";
                    ScriptManager.RegisterStartupScript(this, GetType(), "stay3del", "goToStep(3);", true);
                }
                catch (Exception ex)
                {
                    lblMessage3.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
                }
            }
            else if (e.CommandName == "MoveUpQuestion")
            {
                try
                {
                    int questionId = Convert.ToInt32(e.CommandArgument);
                    MoveQuestion(questionId, "up");
                    LoadQuestions();

                    hfCurrentStep.Value = "3";
                    ScriptManager.RegisterStartupScript(this, GetType(), "stay3move", "goToStep(3);", true);
                }
                catch (Exception ex)
                {
                    lblMessage3.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
                }
            }
            else if (e.CommandName == "EditQuestion")
            {
                try
                {
                    int questionId = Convert.ToInt32(e.CommandArgument);

                    string sql = "SELECT * FROM Questions WHERE Id=@id";
                    var dt = DbHelper.Query(sql, new MySqlParameter("@id", questionId));

                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        txtQuestionText.Text = row["Text"].ToString();
                        txtQuestionOrder.Text = row["OrderNo"].ToString();
                        txtPointMultiplier.Text = row["PointsMultiplier"].ToString();

                        // ⭐ YENİ: Mevcut görseli göster
                        string imageUrl = row["ImageUrl"]?.ToString();
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            pnlExistingQuestionImage.Visible = true;
                            lblExistingQuestionImage.Text = $"✅ Mevcut görsel: {System.IO.Path.GetFileName(imageUrl)}";

                            string questionImageScript = $@"
                    setTimeout(function() {{
                        var previewContainer = document.querySelector('#previewQuestionImage .preview-image-container');
                        if (previewContainer) {{
                            previewContainer.innerHTML = '<img src=""{imageUrl}"" style=""max-width: 100%; border-radius: 8px;"" />';
                            document.getElementById('previewQuestionImage').style.display = 'block';
                        }}
                    }}, 300);
                ";
                            ScriptManager.RegisterStartupScript(this, GetType(), "loadQuestionImagePreview", questionImageScript, true);
                        }
                        else
                        {
                            pnlExistingQuestionImage.Visible = true;
                            lblExistingQuestionImage.Text = "⚠️ Daha önce görsel eklenmemiş";
                            lblExistingQuestionImage.ForeColor = System.Drawing.Color.Orange;
                        }

                        // Düzenleme modunu işaretle
                        ViewState["EditingQuestionId"] = questionId;
                        btnAddQuestion.Text = "💾 Güncelle";
                        btnCancelQuestionEdit.Visible = true;

                        lblMessage3.Text = "<span class='text-info'>✏️ Düzenleme modunda... Değişikliklerinizi yapıp 'Güncelle' butonuna basın.</span>";
                    }

                    LoadQuestions();

                    hfCurrentStep.Value = "3";
                    ScriptManager.RegisterStartupScript(this, GetType(), "editQuestionMode", "goToStep(3);", true);
                }
                catch (Exception ex)
                {
                    lblMessage3.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
                    hfCurrentStep.Value = "3";
                    ScriptManager.RegisterStartupScript(this, GetType(), "stay3editerr", "goToStep(3);", true);
                }
            }
            else if (e.CommandName == "MoveDownQuestion")
            {
                try
                {
                    int questionId = Convert.ToInt32(e.CommandArgument);
                    MoveQuestion(questionId, "down");
                    LoadQuestions();

                    hfCurrentStep.Value = "3";
                    ScriptManager.RegisterStartupScript(this, GetType(), "stay3move2", "goToStep(3);", true);
                }
                catch (Exception ex)
                {
                    lblMessage3.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
                }
            }
        }

        protected void btnAddOption_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(hfCurrentQuestionId.Value) || hfCurrentQuestionId.Value == "0")
                {
                    lblMessage4.Text = "<span class='text-warning'>⚠️ Önce soru seçin!</span>";
                    return;
                }

                int questionId = Convert.ToInt32(hfCurrentQuestionId.Value);
                string text = txtOptionText.Text.Trim();
                string imageUrl = UploadFile(fuOptionImage, "OptionImages");
                int orderNo = Convert.ToInt32(txtOptionOrder.Text);
                string optionType = ddlOptionType.SelectedValue;

                // Validasyon
                if (optionType == "TextOnly" && string.IsNullOrEmpty(text))
                {
                    lblMessage4.Text = "<span class='text-warning'>⚠️ Metin gerekli!</span>";
                    return;
                }
                else if (optionType == "ImageOnly" && string.IsNullOrEmpty(imageUrl))
                {
                    lblMessage4.Text = "<span class='text-warning'>⚠️ Görsel gerekli!</span>";
                    return;
                }
                else if (optionType == "Mixed" && string.IsNullOrEmpty(text) && string.IsNullOrEmpty(imageUrl))
                {
                    lblMessage4.Text = "<span class='text-warning'>⚠️ Metin veya görsel gerekli!</span>";
                    return;
                }

                // Hidden field'dan puanları al
                string scoresJson = hfScoreData.Value;

                if (string.IsNullOrEmpty(scoresJson) || scoresJson == "{}")
                {
                    lblMessage4.Text = "<span class='text-warning'>⚠️ En az bir sonuca 0'dan büyük puan verin!</span>";
                    return;
                }

                var scores = JsonConvert.DeserializeObject<Dictionary<string, int>>(scoresJson);

                if (scores == null || !scores.Values.Any(s => s > 0))
                {
                    lblMessage4.Text = "<span class='text-warning'>⚠️ En az bir sonuca 0'dan büyük puan verin!</span>";
                    return;
                }

                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;

                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Düzenleme modunda mı kontrol et
                    // GÜNCELLEME
                    // GÜNCELLEME
                    // GÜNCELLEME
                    if (ViewState["EditingOptionId"] != null)
                    {
                        int optionId = Convert.ToInt32(ViewState["EditingOptionId"]);
                        int currentOrderNo = Convert.ToInt32(txtOptionOrder.Text);

                        // ⭐ Eski görseli al
                        string oldImageUrl = null;
                        var dtOldImage = DbHelper.Query("SELECT ImageUrl FROM Options WHERE Id=@id",
                            new MySqlParameter("@id", optionId));
                        if (dtOldImage.Rows.Count > 0)
                        {
                            oldImageUrl = dtOldImage.Rows[0]["ImageUrl"]?.ToString();
                        }

                        string sqlUpdate = "UPDATE Options SET Text=@text, OrderNo=@order, PersonalityScores=@scores";

                        // ⭐ YENİ: Görseli kaldır checkbox'ı işaretliyse
                        if (chkRemoveOptionImage.Checked)
                        {
                            sqlUpdate += ", ImageUrl=NULL";

                            // Eski görseli fiziksel olarak sil
                            if (!string.IsNullOrEmpty(oldImageUrl))
                            {
                                try
                                {
                                    string oldFilePath = Server.MapPath("~/Uploads/OptionImages/" + System.IO.Path.GetFileName(oldImageUrl));
                                    if (System.IO.File.Exists(oldFilePath))
                                    {
                                        System.IO.File.Delete(oldFilePath);
                                        System.Diagnostics.Debug.WriteLine($"✅ Seçenek görseli silindi: {oldFilePath}");
                                    }
                                }
                                catch (Exception delEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"⚠️ Seçenek görseli silinemedi: {delEx.Message}");
                                }
                            }
                        }
                        // Yeni görsel seçildiyse
                        else if (!string.IsNullOrEmpty(imageUrl))
                        {
                            sqlUpdate += ", ImageUrl=@img";

                            // Eski görseli sil
                            if (!string.IsNullOrEmpty(oldImageUrl))
                            {
                                try
                                {
                                    string oldFilePath = Server.MapPath("~/Uploads/OptionImages/" + System.IO.Path.GetFileName(oldImageUrl));
                                    if (System.IO.File.Exists(oldFilePath))
                                    {
                                        System.IO.File.Delete(oldFilePath);
                                        System.Diagnostics.Debug.WriteLine($"✅ Eski seçenek görseli silindi: {oldFilePath}");
                                    }
                                }
                                catch (Exception delEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"⚠️ Eski seçenek görseli silinemedi: {delEx.Message}");
                                }
                            }
                        }

                        sqlUpdate += " WHERE Id=@id";

                        using (var cmd = new MySqlCommand(sqlUpdate, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", optionId);
                            cmd.Parameters.AddWithValue("@text", text ?? "");
                            cmd.Parameters.AddWithValue("@order", currentOrderNo);
                            cmd.Parameters.AddWithValue("@scores", scoresJson);

                            if (!chkRemoveOptionImage.Checked && !string.IsNullOrEmpty(imageUrl))
                                cmd.Parameters.AddWithValue("@img", imageUrl);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                lblMessage4.Text = $"<span class='text-success'>✅ Seçenek güncellendi!</span>";
                            }
                        }

                        ViewState["EditingOptionId"] = null;
                        btnAddOption.Text = "✅ Seçenek Ekle";
                        btnCancelEdit.Visible = false;
                        pnlExistingOptionImage.Visible = false;
                        chkRemoveOptionImage.Checked = false;  // ⭐ Checkbox'ı sıfırla
                    }
                    // YENİ EKLEME
                    else
                    {
                        // ✅ Otomatik sıra numarası
                        string sqlMaxOrder = "SELECT COALESCE(MAX(OrderNo), 0) FROM Options WHERE QuestionId=@qId";
                        int nextOrder = Convert.ToInt32(DbHelper.Scalar(sqlMaxOrder, new MySqlParameter("@qId", questionId))) + 1;

                        string sqlInsert = @"INSERT INTO Options 
                        (QuestionId, Text, ImageUrl, OrderNo, ScoreValue, PersonalityTag, PersonalityScores)
                        VALUES (@qId, @text, @img, @order, 1, '', @scores)";

                        using (var cmd = new MySqlCommand(sqlInsert, conn))
                        {
                            cmd.Parameters.AddWithValue("@qId", questionId);
                            cmd.Parameters.AddWithValue("@text", text ?? "");
                            cmd.Parameters.AddWithValue("@img", imageUrl ?? "");
                            cmd.Parameters.AddWithValue("@order", nextOrder);
                            cmd.Parameters.AddWithValue("@scores", scoresJson);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                lblMessage4.Text = $"<span class='text-success'>✅ Seçenek eklendi!</span>";
                            }
                            else
                            {
                                lblMessage4.Text = "<span class='text-danger'>❌ Kayıt yapılamadı!</span>";
                                return;
                            }
                        }
                        pnlExistingOptionImage.Visible = false;
                    }

                    // Formu temizle
                    txtOptionText.Text = "";

                    // ✅ Seçenek sırasını otomatik güncelle
                    string sqlNewMaxOrder = "SELECT COALESCE(MAX(OrderNo), 0) FROM Options WHERE QuestionId=@qId";
                    int currentMaxOrder = Convert.ToInt32(DbHelper.Scalar(sqlNewMaxOrder, new MySqlParameter("@qId", questionId)));
                    txtOptionOrder.Text = (currentMaxOrder + 1).ToString();

                    // Seçenekleri yeniden yükle
                    LoadOptions();

                    // Panel görünür kalsın
                    pnlManageOptions.Visible = true;

                    string sqlQuestion = "SELECT Text FROM Questions WHERE Id=@id";
                    var dtQuestion = DbHelper.Query(sqlQuestion, new MySqlParameter("@id", questionId));
                    if (dtQuestion.Rows.Count > 0)
                    {
                        lblCurrentQuestion.Text = dtQuestion.Rows[0]["Text"].ToString();
                        lblQuizOptionType.Text = ddlOptionType.SelectedItem.Text;
                    }

                    // Puanları sıfırla ve yeniden yükle
                    string sqlResults = @"SELECT Id, Title, Tag 
                                      FROM Results 
                                      WHERE QuizId=@quizId AND ResultType='personality' 
                                      ORDER BY Id";
                    var dtResults = DbHelper.Query(sqlResults, new MySqlParameter("@quizId", hfCurrentQuizId.Value));

                    string resultsJson = JsonConvert.SerializeObject(dtResults);

                    string script = $@"
                    goToStep(3);
                    setTimeout(function() {{
                        loadScoreInputs({resultsJson});
                        document.getElementById('{hfScoreData.ClientID}').value = '{{}}';
                    }}, 100);
                ";
                    ScriptManager.RegisterStartupScript(this, GetType(), "stay3", script, true);
                }
            }
            catch (Exception ex)
            {
                lblMessage4.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
            }
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            // Düzenleme modundan çık
            ViewState["EditingOptionId"] = null;
            btnAddOption.Text = "✅ Seçenek Ekle";
            btnCancelEdit.Visible = false;

            // Formu temizle
            txtOptionText.Text = "";
            pnlExistingOptionImage.Visible = false;
            // Sırayı ayarla
            if (!string.IsNullOrEmpty(hfCurrentQuestionId.Value) && hfCurrentQuestionId.Value != "0")
            {
                int questionId = Convert.ToInt32(hfCurrentQuestionId.Value);
                string sqlMaxOrder = "SELECT COALESCE(MAX(OrderNo), 0) FROM Options WHERE QuestionId=@qId";
                int maxOrder = Convert.ToInt32(DbHelper.Scalar(sqlMaxOrder, new MySqlParameter("@qId", questionId)));
                txtOptionOrder.Text = (maxOrder + 1).ToString();
            }

            lblMessage4.Text = "<span class='text-info'>✅ Düzenleme iptal edildi.</span>";

            pnlManageOptions.Visible = true;

            // Puanları sıfırla
            string sqlResults = @"SELECT Id, Title, Tag 
                                  FROM Results 
                                  WHERE QuizId=@quizId AND ResultType='personality' 
                                  ORDER BY Id";
            var dtResults = DbHelper.Query(sqlResults, new MySqlParameter("@quizId", hfCurrentQuizId.Value));

            string resultsJson = JsonConvert.SerializeObject(dtResults);

            string script = $@"
                goToStep(3);
                setTimeout(function() {{
                    loadScoreInputs({resultsJson});
                    document.getElementById('{hfScoreData.ClientID}').value = '{{}}';
                }}, 100);
            ";
            ScriptManager.RegisterStartupScript(this, GetType(), "cancelEdit", script, true);
        }

        protected void btnCloseOptions_Click(object sender, EventArgs e)
        {
            if (gvOptions.Rows.Count < 2)
            {
                lblMessage4.Text = "<span class='text-warning'>⚠️ En az 2 seçenek ekleyin!</span>";
                return;
            }

            pnlManageOptions.Visible = false;
            hfCurrentQuestionId.Value = "0";

            // Düzenleme modunu temizle
            ViewState["EditingOptionId"] = null;
            btnAddOption.Text = "✅ Seçenek Ekle";
            btnCancelEdit.Visible = false;

            lblMessage3.Text = "<span class='text-success'>✅ Seçenekler kaydedildi!</span>";
            LoadQuestions();

            ScriptManager.RegisterStartupScript(this, GetType(), "closeAndStay", "goToStep(3);", true);
        }

        protected void gvOptions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteOption")
            {
                try
                {
                    int optionId = Convert.ToInt32(e.CommandArgument);
                    int questionId = Convert.ToInt32(hfCurrentQuestionId.Value);

                    // Silinecek seçeneğin OrderNo'sunu al
                    string sqlGetOrder = "SELECT OrderNo FROM Options WHERE Id=@id";
                    int deletedOrder = Convert.ToInt32(DbHelper.Scalar(sqlGetOrder, new MySqlParameter("@id", optionId)));

                    // Seçeneği sil
                    DbHelper.Execute("DELETE FROM Options WHERE Id=@id",
                        new MySqlParameter("@id", optionId));

                    // ✅ Silinen sıradan sonraki tüm seçeneklerin sırasını 1 azalt
                    string sqlReorder = @"UPDATE Options 
                              SET OrderNo = OrderNo - 1 
                              WHERE QuestionId=@qid AND OrderNo > @deletedOrder";

                    DbHelper.Execute(sqlReorder,
                        new MySqlParameter("@qid", questionId),
                        new MySqlParameter("@deletedOrder", deletedOrder));

                    lblMessage4.Text = "<span class='text-success'>✅ Seçenek silindi ve sıralar düzenlendi!</span>";
                    LoadOptions();

                    // ✅ Sıra numarasını güncelle
                    string sqlMaxOrder = "SELECT COALESCE(MAX(OrderNo), 0) FROM Options WHERE QuestionId=@qid";
                    int maxOrder = Convert.ToInt32(DbHelper.Scalar(sqlMaxOrder, new MySqlParameter("@qid", questionId)));
                    txtOptionOrder.Text = (maxOrder + 1).ToString();

                    pnlManageOptions.Visible = true;

                    // Puanları yeniden yükle
                    string sqlResults = @"SELECT Id, Title, Tag 
                              FROM Results 
                              WHERE QuizId=@quizId AND ResultType='personality' 
                              ORDER BY Id";
                    var dtResults = DbHelper.Query(sqlResults, new MySqlParameter("@quizId", hfCurrentQuizId.Value));

                    string resultsJson = JsonConvert.SerializeObject(dtResults);

                    string script = $@"
            goToStep(3);
            setTimeout(function() {{
                loadScoreInputs({resultsJson});
            }}, 100);
        ";
                    ScriptManager.RegisterStartupScript(this, GetType(), "stayOpen", script, true);
                }
                catch (Exception ex)
                {
                    lblMessage4.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
                }
            }
            else if (e.CommandName == "EditOption")
            {
                try
                {
                    int optionId = Convert.ToInt32(e.CommandArgument);

                    string sql = "SELECT * FROM Options WHERE Id=@id";
                    var dt = DbHelper.Query(sql, new MySqlParameter("@id", optionId));

                    if (dt.Rows.Count > 0)
                    {
                        var row = dt.Rows[0];
                        txtOptionText.Text = row["Text"].ToString();
                        txtOptionOrder.Text = row["OrderNo"].ToString();

                        // ⭐ YENİ: Mevcut görseli göster
                        string imageUrl = row["ImageUrl"]?.ToString();
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            pnlExistingOptionImage.Visible = true;
                            lblExistingOptionImage.Text = $"✅ Mevcut görsel: {System.IO.Path.GetFileName(imageUrl)}";

                            string optionImageScript = $@"
                    setTimeout(function() {{
                        var previewContainer = document.querySelector('#previewOptionImage .preview-image-container');
                        if (previewContainer) {{
                            previewContainer.innerHTML = '<img src=""{imageUrl}"" style=""max-width: 100%; border-radius: 8px;"" />';
                            document.getElementById('previewOptionImage').style.display = 'block';
                        }}
                    }}, 300);
                ";
                            ScriptManager.RegisterStartupScript(this, GetType(), "loadOptionImagePreview", optionImageScript, true);
                        }
                        else
                        {
                            pnlExistingOptionImage.Visible = true;
                            lblExistingOptionImage.Text = "⚠️ Daha önce görsel eklenmemiş";
                            lblExistingOptionImage.ForeColor = System.Drawing.Color.Orange;
                        }

                        // Puanları yükle
                        string scoresJson = row["PersonalityScores"].ToString();
                        if (!string.IsNullOrEmpty(scoresJson))
                        {
                            hfScoreData.Value = scoresJson;
                        }

                        // Düzenleme modunu işaretle
                        ViewState["EditingOptionId"] = optionId;
                        btnAddOption.Text = "💾 Güncelle";
                        btnCancelEdit.Visible = true;

                        lblMessage4.Text = "<span class='text-info'>✏️ Düzenleme modunda... Değişikliklerinizi yapıp 'Güncelle' butonuna basın.</span>";
                    }

                    pnlManageOptions.Visible = true;
                    LoadOptions();

                    // Puanları yeniden yükle VE doldur
                    string sqlResults = @"SELECT Id, Title, Tag 
                                      FROM Results 
                                      WHERE QuizId=@quizId AND ResultType='personality' 
                                      ORDER BY Id";
                    var dtResults = DbHelper.Query(sqlResults, new MySqlParameter("@quizId", hfCurrentQuizId.Value));

                    string resultsJson = JsonConvert.SerializeObject(dtResults);

                    // Hem inputları oluştur hem de değerleri doldur
                    string scoresJsonForScript = dt.Rows[0]["PersonalityScores"].ToString();

                    string scriptFinal = $@"
            goToStep(3);
            setTimeout(function() {{
                loadScoreInputs({resultsJson});
                
                // Puanları doldur
                var scores = {scoresJsonForScript};
                for (var tag in scores) {{
                    var input = document.getElementById('score_' + tag);
                    if (input) {{
                        input.value = scores[tag];
                        updateScoreData(tag, scores[tag]);
                    }}
                }}
            }}, 200);
        ";
                    ScriptManager.RegisterStartupScript(this, GetType(), "editMode", scriptFinal, true);
                }
                catch (Exception ex)
                {
                    lblMessage4.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
                }
            }
            else if (e.CommandName == "MoveUpOption")
            {
                try
                {
                    int optionId = Convert.ToInt32(e.CommandArgument);
                    MoveOption(optionId, "up");
                    LoadOptions();

                    pnlManageOptions.Visible = true;

                    string sqlResults = @"SELECT Id, Title, Tag 
                                          FROM Results 
                                          WHERE QuizId=@quizId AND ResultType='personality' 
                                          ORDER BY Id";
                    var dtResults = DbHelper.Query(sqlResults, new MySqlParameter("@quizId", hfCurrentQuizId.Value));

                    string resultsJson = JsonConvert.SerializeObject(dtResults);

                    string script = $@"
                        goToStep(3);
                        setTimeout(function() {{
                            loadScoreInputs({resultsJson});
                        }}, 100);
                    ";
                    ScriptManager.RegisterStartupScript(this, GetType(), "moveOpt", script, true);
                }
                catch (Exception ex)
                {
                    lblMessage4.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
                }
            }
            else if (e.CommandName == "MoveDownOption")
            {
                try
                {
                    int optionId = Convert.ToInt32(e.CommandArgument);
                    MoveOption(optionId, "down");
                    LoadOptions();

                    pnlManageOptions.Visible = true;

                    string sqlResults = @"SELECT Id, Title, Tag 
                                          FROM Results 
                                          WHERE QuizId=@quizId AND ResultType='personality' 
                                          ORDER BY Id";
                    var dtResults = DbHelper.Query(sqlResults, new MySqlParameter("@quizId", hfCurrentQuizId.Value));

                    string resultsJson = JsonConvert.SerializeObject(dtResults);

                    string script = $@"
                        goToStep(3);
                        setTimeout(function() {{
                            loadScoreInputs({resultsJson});
                        }}, 100);
                    ";
                    ScriptManager.RegisterStartupScript(this, GetType(), "moveOpt2", script, true);
                }
                catch (Exception ex)
                {
                    lblMessage4.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
                }
            }
        }

        protected void btnGoToPreview_Click(object sender, EventArgs e)
        {
            LoadQuestions();

            if (gvQuestions.Rows.Count < 3)
            {
                lblMessage3Final.Text = "<span class='text-warning'>⚠️ En az 3 soru ekleyin!</span>";
                return;
            }

            foreach (GridViewRow row in gvQuestions.Rows)
            {
                int questionId = Convert.ToInt32(gvQuestions.DataKeys[row.RowIndex].Value);
                string sql = "SELECT COUNT(*) FROM Options WHERE QuestionId=@id";
                int count = Convert.ToInt32(DbHelper.Scalar(sql, new MySqlParameter("@id", questionId)));

                if (count < 2)
                {
                    lblMessage3Final.Text = "<span class='text-warning'>⚠️ Her sorunun en az 2 seçeneği olmalı!</span>";
                    return;
                }
            }

            // ✅ Önizleme verilerini yükle
            LoadPreviewData();

            // ✅ Adım 4'e geç
            hfCurrentStep.Value = "4";
            ScriptManager.RegisterStartupScript(this, GetType(), "goToStep4", "goToStep(4);", true);
        }

        private void LoadQuestions()
        {
            try
            {
                if (string.IsNullOrEmpty(hfCurrentQuizId.Value) || hfCurrentQuizId.Value == "0")
                {
                    gvQuestions.DataSource = null;
                    gvQuestions.DataBind();
                    return;
                }

                int quizId = Convert.ToInt32(hfCurrentQuizId.Value);

                // ✅ ImageUrl kolonunu da SELECT et
                string sql = @"SELECT 
    q.Id, 
    q.QuizId, 
    q.Text, 
    q.ImageUrl,
    q.OrderNo,
    (SELECT COUNT(*) FROM Options WHERE QuestionId = q.Id) AS OptionCount
FROM Questions q
WHERE q.QuizId=@qid 
ORDER BY q.OrderNo";

                var dt = DbHelper.Query(sql, new MySqlParameter("@qid", quizId));

                gvQuestions.DataSource = dt;
                gvQuestions.DataBind();

                lblMessage3.Text = $"<span class='text-success'>{dt.Rows.Count} soru yüklendi</span>";

                // ✅ HER SORU İÇİN MEVCUT GÖRSELLERİ LOGLA
                foreach (DataRow row in dt.Rows)
                {
                    string imageUrl = row["ImageUrl"]?.ToString();
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        string fileName = Path.GetFileName(imageUrl);
                        System.Diagnostics.Debug.WriteLine($"Question {row["Id"]} has image: {fileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage3.Text = $"<span class='text-danger'>❌ Hata: {ex.Message}</span>";
                System.Diagnostics.Debug.WriteLine($"LoadQuestions Error: {ex.Message}");
            }
        }

        private void LoadOptions()
        {
            if (string.IsNullOrEmpty(hfCurrentQuestionId.Value) || hfCurrentQuestionId.Value == "0") return;

            string sql = @"SELECT 
    Id, 
    QuestionId, 
    Text, 
    ImageUrl,        -- ✅ Bu olmalı
    OrderNo, 
    PersonalityScores 
FROM Options 
WHERE QuestionId=@qid 
ORDER BY OrderNo";

            var dt = DbHelper.Query(sql, new MySqlParameter("@qid", hfCurrentQuestionId.Value));
            gvOptions.DataSource = dt;
            gvOptions.DataBind();
        }

        private void MoveQuestion(int questionId, string direction)
        {
            string sql = "SELECT OrderNo FROM Questions WHERE Id=@id";
            int currentOrder = Convert.ToInt32(DbHelper.Scalar(sql, new MySqlParameter("@id", questionId)));

            int targetOrder = direction == "up" ? currentOrder - 1 : currentOrder + 1;

            // Hedef sıradaki soruyu bul
            string sqlTarget = "SELECT Id FROM Questions WHERE QuizId=@quizId AND OrderNo=@order";
            var dtTarget = DbHelper.Query(sqlTarget,
                new MySqlParameter("@quizId", hfCurrentQuizId.Value),
                new MySqlParameter("@order", targetOrder));

            if (dtTarget.Rows.Count > 0)
            {
                int targetQuestionId = Convert.ToInt32(dtTarget.Rows[0]["Id"]);

                // Sıraları değiştir
                DbHelper.Execute("UPDATE Questions SET OrderNo=@order WHERE Id=@id",
                    new MySqlParameter("@order", targetOrder),
                    new MySqlParameter("@id", questionId));

                DbHelper.Execute("UPDATE Questions SET OrderNo=@order WHERE Id=@id",
                    new MySqlParameter("@order", currentOrder),
                    new MySqlParameter("@id", targetQuestionId));
            }
        }

        private void MoveOption(int optionId, string direction)
        {
            string sql = "SELECT OrderNo, QuestionId FROM Options WHERE Id=@id";
            var dt = DbHelper.Query(sql, new MySqlParameter("@id", optionId));

            if (dt.Rows.Count > 0)
            {
                int currentOrder = Convert.ToInt32(dt.Rows[0]["OrderNo"]);
                int questionId = Convert.ToInt32(dt.Rows[0]["QuestionId"]);

                int targetOrder = direction == "up" ? currentOrder - 1 : currentOrder + 1;

                // Hedef sıradaki seçeneği bul
                string sqlTarget = "SELECT Id FROM Options WHERE QuestionId=@qId AND OrderNo=@order";
                var dtTarget = DbHelper.Query(sqlTarget,
                    new MySqlParameter("@qId", questionId),
                    new MySqlParameter("@order", targetOrder));

                if (dtTarget.Rows.Count > 0)
                {
                    int targetOptionId = Convert.ToInt32(dtTarget.Rows[0]["Id"]);

                    // Sıraları değiştir
                    DbHelper.Execute("UPDATE Options SET OrderNo=@order WHERE Id=@id",
                        new MySqlParameter("@order", targetOrder),
                        new MySqlParameter("@id", optionId));

                    DbHelper.Execute("UPDATE Options SET OrderNo=@order WHERE Id=@id",
                        new MySqlParameter("@order", currentOrder),
                        new MySqlParameter("@id", targetOptionId));
                }
            }
        }



        #endregion

        #region ADIM 4: Önizleme

        private void LoadPreviewData()
        {
            try
            {
                if (string.IsNullOrEmpty(hfCurrentQuizId.Value) || hfCurrentQuizId.Value == "0") return;

                int quizId = Convert.ToInt32(hfCurrentQuizId.Value);

                // Quiz bilgilerini yükle
                string sqlQuiz = "SELECT * FROM Quizzes WHERE Id=@id";
                var dtQuiz = DbHelper.Query(sqlQuiz, new MySqlParameter("@id", quizId));

                if (dtQuiz.Rows.Count > 0)
                {
                    var quiz = dtQuiz.Rows[0];

                    lblPreviewTitle.Text = quiz["Title"].ToString();
                    lblPreviewDescription.Text = quiz["Description"].ToString();
                    lblPreviewTime.Text = quiz["EstimatedTime"].ToString();

                    string quizType = quiz["QuizType"].ToString();
                    lblPreviewType.Text = quizType == "PersonalityClassic" ? "Kişilik Testi" : "Bu mu Şu mu";

                    string status = quiz["Status"].ToString();
                    lblPreviewStatus.Text = status == "Published" ? "Yayında" : "Taslak";

                    string coverUrl = quiz["CoverImageUrl"].ToString();
                    if (!string.IsNullOrEmpty(coverUrl))
                    {
                        imgPreviewCover.ImageUrl = coverUrl;
                        imgPreviewCover.Visible = true;
                    }
                    else
                    {
                        imgPreviewCover.Visible = false;
                    }
                }

                // Sonuçları yükle
                string sqlResults = @"SELECT * FROM Results 
                      WHERE QuizId=@quizId AND ResultType='personality' 
                      ORDER BY Id";
                var dtResults = DbHelper.Query(sqlResults, new MySqlParameter("@quizId", quizId));

                lblPreviewResultCount.Text = dtResults.Rows.Count.ToString();
                rptPreviewResults.DataSource = dtResults;  // GridView yerine Repeater
                rptPreviewResults.DataBind();

                // Soruları yükle
                string sqlQuestions = @"SELECT * FROM Questions 
                        WHERE QuizId=@quizId 
                        ORDER BY OrderNo";
                var dtQuestions = DbHelper.Query(sqlQuestions, new MySqlParameter("@quizId", quizId));

                lblPreviewQuestionCount.Text = dtQuestions.Rows.Count.ToString();
                rptPreviewQuestions.DataSource = dtQuestions;
                rptPreviewQuestions.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage3Final.Text = $"<span class='text-danger'>❌ Önizleme yükleme hatası: {ex.Message}</span>";
            }
        }

        protected void rptPreviewQuestions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var drQuestion = (DataRow)((DataRowView)e.Item.DataItem).Row;
                int questionId = Convert.ToInt32(drQuestion["Id"]);

                // Seçenekleri yükle
                string sqlOptions = @"SELECT * FROM Options 
                              WHERE QuestionId=@qId 
                              ORDER BY OrderNo";
                var dtOptions = DbHelper.Query(sqlOptions, new MySqlParameter("@qId", questionId));

                Repeater rptOptions = (Repeater)e.Item.FindControl("rptPreviewOptions");
                if (rptOptions != null)
                {
                    rptOptions.DataSource = dtOptions;
                    rptOptions.DataBind();
                }
            }
        }

        protected string GetScoreSummary(string scoresJson)
        {
            try
            {
                if (string.IsNullOrEmpty(scoresJson)) return "Puan yok";

                var scores = JsonConvert.DeserializeObject<Dictionary<string, int>>(scoresJson);
                if (scores == null || scores.Count == 0) return "Puan yok";

                var nonZeroScores = scores.Where(s => s.Value > 0).OrderByDescending(s => s.Value).Take(2);

                if (!nonZeroScores.Any()) return "Puan yok";

                return string.Join(", ", nonZeroScores.Select(s => $"{s.Key}: {s.Value}"));
            }
            catch
            {
                return "Puan yok";
            }
        }

        protected void btnPublishQuiz_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(hfCurrentQuizId.Value) || hfCurrentQuizId.Value == "0") return;

                int quizId = Convert.ToInt32(hfCurrentQuizId.Value);

                string sql = @"UPDATE Quizzes SET 
                       Status='Published', 
                       PublishedAt=NOW() 
                       WHERE Id=@id";

                DbHelper.Execute(sql, new MySqlParameter("@id", quizId));

                ScriptManager.RegisterStartupScript(this, GetType(), "success",
                    "alert('✅ Quiz başarıyla yayınlandı!'); window.location.href='QuizCreate.aspx';", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"alert('❌ Hata: {ex.Message}');", true);
            }
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(hfCurrentQuizId.Value) || hfCurrentQuizId.Value == "0") return;

                int quizId = Convert.ToInt32(hfCurrentQuizId.Value);

                string sql = "UPDATE Quizzes SET Status='Draft' WHERE Id=@id";
                DbHelper.Execute(sql, new MySqlParameter("@id", quizId));

                ScriptManager.RegisterStartupScript(this, GetType(), "success",
                    "alert('✅ Quiz taslak olarak kaydedildi!'); window.location.href='QuizCreate.aspx';", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"alert('❌ Hata: {ex.Message}');", true);
            }
        }

        #endregion

        #region Quiz Listesi Yönetimi - Buton Handlers

        protected void btnNewQuiz_Click(object sender, EventArgs e)
        {
            // Sayfayı temizle
            Session.Remove("CurrentQuizId");
            Response.Redirect("QuizCreate.aspx");
        }

        protected void btnShowDrafts_Click(object sender, EventArgs e)
        {
            ClearSearch();
            LoadQuizList("Draft");
            HideAllPanels();
        }

        protected void btnShowPublished_Click(object sender, EventArgs e)
        {
            ClearSearch();
            LoadQuizList("Published");
            HideAllPanels();
        }

        protected void btnCloseList_Click(object sender, EventArgs e)
        {
            pnlQuizList.Visible = false;
            ShowCurrentStep();
        }

        private void HideAllPanels()
        {
            // Tüm step panellerini gizle
            ScriptManager.RegisterStartupScript(this, GetType(), "hideAll",
                @"document.querySelectorAll('.panel-section').forEach(p => p.classList.remove('active'));
          document.querySelectorAll('.step').forEach(s => s.classList.remove('active'));
          document.querySelectorAll('.step-container').forEach(s => s.style.display = 'none');",
                true);
        }

        private void ShowCurrentStep()
        {
            if (!string.IsNullOrEmpty(hfCurrentStep.Value))
            {
                int step = Convert.ToInt32(hfCurrentStep.Value);
                ScriptManager.RegisterStartupScript(this, GetType(), "showStep",
                    $@"document.querySelectorAll('.step-container').forEach(s => s.style.display = 'block');
               goToStep({step});",
                    true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showStep1",
                    @"document.querySelectorAll('.step-container').forEach(s => s.style.display = 'block');
              goToStep(1);",
                    true);
            }
        }

        private void LoadQuizList(string status)
        {
            try
            {
                ViewState["CurrentStatus"] = status;

                string sql = @"SELECT Id, Title, Description, Status, CreatedAt 
                       FROM Quizzes 
                       WHERE Status=@status AND CreatedBy=@userId 
                       ORDER BY CreatedAt DESC";

                var parameters = new MySqlParameter[]
                {
            new MySqlParameter("@status", status),
            new MySqlParameter("@userId", Session["UserId"])
                };

                var dt = DbHelper.Query(sql, parameters);

                gvQuizList.DataSource = dt;
                gvQuizList.DataBind();

                pnlQuizList.Visible = true;
                pnlSearchInfo.Visible = true;
                lblSearchInfo.Text = $"Toplam <strong>{dt.Rows.Count}</strong> quiz listelendi.";

                lblQuizListTitle.Text = status == "Published"
                    ? "<i class='fas fa-check-circle'></i> Yayınlanan Quiz'ler"
                    : "<i class='fas fa-edit'></i> Taslak Quiz'ler";
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"alert('❌ Hata: {ex.Message}');", true);
            }
        }

        protected void gvQuizList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditQuiz")
            {
                try
                {
                    int quizId = Convert.ToInt32(e.CommandArgument);

                    // ✅ Debug: Quiz ID'yi kontrol et
                    System.Diagnostics.Debug.WriteLine($"Düzenleniyor: Quiz ID = {quizId}");

                    Response.Redirect($"QuizCreate.aspx?id={quizId}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Hata: {ex.Message}");
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        $"alert('❌ Hata: {ex.Message}');", true);
                }
            }
            else if (e.CommandName == "DeleteQuiz")
            {
                try
                {
                    int quizId = Convert.ToInt32(e.CommandArgument);

                    System.Diagnostics.Debug.WriteLine($"========== Quiz {quizId} SİLİNİYOR (QuizCreate) ==========");

                    // ✅ DOĞRU CASCADE SIRALAMA:
                    // 1. SubmissionAnswers (En alttaki tablo)
                    // 2. Submissions
                    // 3. Options
                    // 4. Questions
                    // 5. Results
                    // 6. Quizzes

                    // 1️⃣ SubmissionAnswers silme
                    string deleteSubmissionAnswers = @"
            DELETE FROM SubmissionAnswers 
            WHERE SubmissionId IN (
                SELECT Id FROM Submissions WHERE QuizId=@quizId
            )";
                    int affectedAnswers = DbHelper.Execute(deleteSubmissionAnswers, new MySqlParameter("@quizId", quizId));
                    System.Diagnostics.Debug.WriteLine($"✅ {affectedAnswers} SubmissionAnswers silindi");

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
                    DbHelper.Execute("DELETE FROM Quizzes WHERE Id=@id",
                        new MySqlParameter("@id", quizId));
                    System.Diagnostics.Debug.WriteLine($"✅ Quiz {quizId} silindi");

                    System.Diagnostics.Debug.WriteLine($"========== SİLME TAMAMLANDI ==========");

                    // Listeyi yeniden yükle
                    LoadQuizList(ViewState["CurrentStatus"]?.ToString() ?? "Draft");

                    ScriptManager.RegisterStartupScript(this, GetType(), "success",
                        "alert('✅ Quiz başarıyla silindi!');", true);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ DeleteQuiz Error (QuizCreate): {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        $"alert('❌ Hata: {ex.Message}');", true);
                }
            }
            else if (e.CommandName == "DuplicateQuiz")
            {
                try
                {
                    int quizId = Convert.ToInt32(e.CommandArgument);
                    DuplicateQuiz(quizId);

                    // Listeyi yenile
                    LoadQuizList(ViewState["CurrentStatus"]?.ToString() ?? "Draft");

                    ScriptManager.RegisterStartupScript(this, GetType(), "success",
                        "alert('✅ Quiz kopyalandı!');", true);
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "error",
                        $"alert('❌ Hata: {ex.Message}');", true);
                }
            }
        }

        private void DuplicateQuiz(int originalQuizId)
        {
            // Quiz'i kopyala
            string sqlQuiz = "SELECT * FROM Quizzes WHERE Id=@id";
            var dtQuiz = DbHelper.Query(sqlQuiz, new MySqlParameter("@id", originalQuizId));

            if (dtQuiz.Rows.Count == 0) return;

            var quiz = dtQuiz.Rows[0];

            string sqlInsertQuiz = @"INSERT INTO Quizzes 
        (Title, Slug, Description, Status, CreatedBy, CreatedAt, QuizType, OptionType,
         IsAnonymousAllowed, EstimatedTime, AllowMultipleAttempts, CoverImageUrl, 
         ThumbnailUrl, BackgroundColor, Theme)
        VALUES (@title, @slug, @desc, 'Draft', @createdBy, NOW(), @quizType, @optionType,
         @anonymous, @time, @multiple, @cover, @thumb, @bg, @theme)";

            var parameters = new MySqlParameter[]
            {
        new MySqlParameter("@title", quiz["Title"].ToString() + " (Kopya)"),
        new MySqlParameter("@slug", quiz["Slug"].ToString() + "-kopya"),
        new MySqlParameter("@desc", quiz["Description"]),
        new MySqlParameter("@createdBy", Session["UserId"]),
        new MySqlParameter("@quizType", quiz["QuizType"]),
        new MySqlParameter("@optionType", quiz["OptionType"]),
        new MySqlParameter("@anonymous", quiz["IsAnonymousAllowed"]),
        new MySqlParameter("@time", quiz["EstimatedTime"]),
        new MySqlParameter("@multiple", quiz["AllowMultipleAttempts"]),
        new MySqlParameter("@cover", quiz["CoverImageUrl"]),
        new MySqlParameter("@thumb", quiz["ThumbnailUrl"]),
        new MySqlParameter("@bg", quiz["BackgroundColor"]),
        new MySqlParameter("@theme", quiz["Theme"])
            };

            DbHelper.Execute(sqlInsertQuiz, parameters);
            int newQuizId = Convert.ToInt32(DbHelper.Scalar("SELECT LAST_INSERT_ID()"));

            // Sonuçları kopyala
            string sqlResults = "SELECT * FROM Results WHERE QuizId=@id";
            var dtResults = DbHelper.Query(sqlResults, new MySqlParameter("@id", originalQuizId));

            var resultMapping = new Dictionary<int, int>();

            foreach (DataRow result in dtResults.Rows)
            {
                string sqlInsertResult = @"INSERT INTO Results 
            (QuizId, Title, Description, ResultType, Tag, MinScore, MaxScore)
            VALUES (@quizId, @title, @desc, @type, @tag, @min, @max)";

                DbHelper.Execute(sqlInsertResult, new MySqlParameter[]
                {
            new MySqlParameter("@quizId", newQuizId),
            new MySqlParameter("@title", result["Title"]),
            new MySqlParameter("@desc", result["Description"]),
            new MySqlParameter("@type", result["ResultType"]),
            new MySqlParameter("@tag", result["Tag"]),
            new MySqlParameter("@min", result["MinScore"]),
            new MySqlParameter("@max", result["MaxScore"])
                });

                int oldResultId = Convert.ToInt32(result["Id"]);
                int newResultId = Convert.ToInt32(DbHelper.Scalar("SELECT LAST_INSERT_ID()"));
                resultMapping[oldResultId] = newResultId;
            }

            // Soruları kopyala
            string sqlQuestions = "SELECT * FROM Questions WHERE QuizId=@id ORDER BY OrderNo";
            var dtQuestions = DbHelper.Query(sqlQuestions, new MySqlParameter("@id", originalQuizId));

            foreach (DataRow question in dtQuestions.Rows)
            {
                string sqlInsertQuestion = @"INSERT INTO Questions 
            (QuizId, Text, OrderNo, QuestionType, PointsMultiplier, MediaType)
            VALUES (@quizId, @text, @order, @type, @points, @media)";

                DbHelper.Execute(sqlInsertQuestion, new MySqlParameter[]
                {
            new MySqlParameter("@quizId", newQuizId),
            new MySqlParameter("@text", question["Text"]),
            new MySqlParameter("@order", question["OrderNo"]),
            new MySqlParameter("@type", question["QuestionType"]),
            new MySqlParameter("@points", question["PointsMultiplier"]),
            new MySqlParameter("@media", question["MediaType"])
                });

                int oldQuestionId = Convert.ToInt32(question["Id"]);
                int newQuestionId = Convert.ToInt32(DbHelper.Scalar("SELECT LAST_INSERT_ID()"));

                // Seçenekleri kopyala
                string sqlOptions = "SELECT * FROM Options WHERE QuestionId=@id ORDER BY OrderNo";
                var dtOptions = DbHelper.Query(sqlOptions, new MySqlParameter("@id", oldQuestionId));

                foreach (DataRow option in dtOptions.Rows)
                {
                    string sqlInsertOption = @"INSERT INTO Options 
                (QuestionId, Text, ImageUrl, OrderNo, ScoreValue, PersonalityTag, PersonalityScores)
                VALUES (@qId, @text, @img, @order, @score, @tag, @scores)";

                    DbHelper.Execute(sqlInsertOption, new MySqlParameter[]
                    {
                new MySqlParameter("@qId", newQuestionId),
                new MySqlParameter("@text", option["Text"]),
                new MySqlParameter("@img", option["ImageUrl"]),
                new MySqlParameter("@order", option["OrderNo"]),
                new MySqlParameter("@score", option["ScoreValue"]),
                new MySqlParameter("@tag", option["PersonalityTag"]),
                new MySqlParameter("@scores", option["PersonalityScores"])
                    });
                }
            }
        }

        protected string GetQuestionCount(object quizId)
        {
            try
            {
                string sql = "SELECT COUNT(*) FROM Questions WHERE QuizId=@id";
                int count = Convert.ToInt32(DbHelper.Scalar(sql, new MySqlParameter("@id", quizId)));

                return $"<span class='badge bg-primary'>{count}</span>";
            }
            catch
            {
                return "<span class='badge bg-secondary'>0</span>";
            }
        }

        #endregion

        #region Arama İşlemleri

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string status = ViewState["CurrentStatus"]?.ToString() ?? "Draft";
            PerformSearch(status);
        }

        protected void btnClearSearch_Click(object sender, EventArgs e)
        {
            ClearSearch();
            string status = ViewState["CurrentStatus"]?.ToString() ?? "Draft";
            LoadQuizList(status);
        }

        private void ClearSearch()
        {
            txtSearch.Text = "";
            ddlSearchField.SelectedValue = "all";
            ddlSortBy.SelectedValue = "date_desc";
            pnlSearchInfo.Visible = false;
        }

        private void PerformSearch(string status)
        {
            try
            {
                ViewState["CurrentStatus"] = status;

                string searchTerm = txtSearch.Text.Trim();
                string searchField = ddlSearchField.SelectedValue;
                string sortBy = ddlSortBy.SelectedValue;

                string sql = @"SELECT DISTINCT q.Id, q.Title, q.Description, q.Status, q.CreatedAt 
                       FROM Quizzes q
                       LEFT JOIN Results r ON q.Id = r.QuizId
                       WHERE q.Status=@status AND q.CreatedBy=@userId";

                var parameters = new List<MySqlParameter>
        {
            new MySqlParameter("@status", status),
            new MySqlParameter("@userId", Session["UserId"])
        };

                // Arama kriteri ekle
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    switch (searchField)
                    {
                        case "title":
                            sql += " AND q.Title LIKE @search";
                            parameters.Add(new MySqlParameter("@search", $"%{searchTerm}%"));
                            break;

                        case "description":
                            sql += " AND q.Description LIKE @search";
                            parameters.Add(new MySqlParameter("@search", $"%{searchTerm}%"));
                            break;

                        case "results":
                            sql += " AND (r.Title LIKE @search OR r.Tag LIKE @search)";
                            parameters.Add(new MySqlParameter("@search", $"%{searchTerm}%"));
                            break;

                        case "all":
                        default:
                            sql += " AND (q.Title LIKE @search OR q.Description LIKE @search OR r.Title LIKE @search OR r.Tag LIKE @search)";
                            parameters.Add(new MySqlParameter("@search", $"%{searchTerm}%"));
                            break;
                    }
                }

                // Sıralama ekle
                switch (sortBy)
                {
                    case "date_asc":
                        sql += " ORDER BY q.CreatedAt ASC";
                        break;
                    case "title_asc":
                        sql += " ORDER BY q.Title ASC";
                        break;
                    case "title_desc":
                        sql += " ORDER BY q.Title DESC";
                        break;
                    case "date_desc":
                    default:
                        sql += " ORDER BY q.CreatedAt DESC";
                        break;
                }

                var dt = DbHelper.Query(sql, parameters.ToArray());

                gvQuizList.DataSource = dt;
                gvQuizList.DataBind();

                pnlQuizList.Visible = true;

                // Arama bilgisi göster
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    pnlSearchInfo.Visible = true;
                    string fieldText = searchField == "all" ? "tüm alanlarda" :
                                      searchField == "title" ? "başlıklarda" :
                                      searchField == "description" ? "açıklamalarda" : "sonuç etiketlerinde";

                    lblSearchInfo.Text = $"<strong>'{searchTerm}'</strong> terimi için {fieldText} <strong>{dt.Rows.Count}</strong> sonuç bulundu.";
                }
                else
                {
                    pnlSearchInfo.Visible = true;
                    lblSearchInfo.Text = $"Toplam <strong>{dt.Rows.Count}</strong> quiz listelendi.";
                }

                lblQuizListTitle.Text = status == "Published"
                    ? "<i class='fas fa-check-circle'></i> Yayınlanan Quiz'ler"
                    : "<i class='fas fa-edit'></i> Taslak Quiz'ler";
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error",
                    $"alert('❌ Arama hatası: {ex.Message}');", true);
            }
        }

        protected string GetResultTags(object quizId)
        {
            try
            {
                string sql = @"SELECT Title, Tag FROM Results 
                       WHERE QuizId=@id AND ResultType='personality' 
                       ORDER BY Id 
                       LIMIT 5";

                var dt = DbHelper.Query(sql, new MySqlParameter("@id", quizId));

                if (dt.Rows.Count == 0)
                    return "<small class='text-muted'>Sonuç yok</small>";

                var tags = new System.Text.StringBuilder();
                tags.Append("<div class='result-tags-container'>");

                foreach (DataRow row in dt.Rows)
                {
                    string tag = row["Tag"].ToString();
                    string title = row["Title"].ToString();
                    tags.Append($"<span class='result-tag-small' title='{title}'>#{tag}</span>");
                }

                if (dt.Rows.Count == 5)
                {
                    // Daha fazla sonuç varsa göster
                    string sqlCount = "SELECT COUNT(*) FROM Results WHERE QuizId=@id AND ResultType='personality'";
                    int total = Convert.ToInt32(DbHelper.Scalar(sqlCount, new MySqlParameter("@id", quizId)));
                    if (total > 5)
                    {
                        tags.Append($"<span class='result-tag-small'>+{total - 5}</span>");
                    }
                }

                tags.Append("</div>");
                return tags.ToString();
            }
            catch
            {
                return "<small class='text-muted'>-</small>";
            }
        }

        #endregion

        #region Yardımcı Metodlar

        private void LoadQuizForEdit(int quizId)
        {
            try
            {
                string sql = "SELECT * FROM Quizzes WHERE Id=@id";
                var dt = DbHelper.Query(sql, new MySqlParameter("@id", quizId));

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];

                    // ✅ Form alanlarını doldur
                    txtTitle.Text = row["Title"].ToString();
                    txtDescription.Text = row["Description"].ToString();
                    txtEstimatedTime.Text = row["EstimatedTime"].ToString();

                    ddlQuizType.SelectedValue = row["QuizType"].ToString();
                    ddlOptionType.SelectedValue = row["OptionType"].ToString();
                    ddlTheme.SelectedValue = row["Theme"].ToString();
                    
                    chkAnonymousAllowed.Checked = Convert.ToBoolean(row["IsAnonymousAllowed"]);
                    chkMultipleAttempts.Checked = Convert.ToBoolean(row["AllowMultipleAttempts"]);
                    chkIsActive.Checked = row["Status"].ToString() == "Published";

                    string coverImageUrl = row["CoverImageUrl"]?.ToString();
                    if (!string.IsNullOrEmpty(coverImageUrl))
                    {
                        pnlExistingCover.Visible = true;
                        lblExistingCover.Text = $"✅ Mevcut kapak: {System.IO.Path.GetFileName(coverImageUrl)}";

                        // Görüntüyü önizleme alanına yükle
                        string coverScript = $@"
        setTimeout(function() {{
            var previewContainer = document.querySelector('#previewCoverImage .preview-image-container');
            if (previewContainer) {{
                previewContainer.innerHTML = '<img src=""{coverImageUrl}"" style=""max-width: 100%; border-radius: 8px;"" />';
                document.getElementById('previewCoverImage').style.display = 'block';
            }}
        }}, 300);
    ";
                        ScriptManager.RegisterStartupScript(this, GetType(), "loadCoverPreview", coverScript, true);
                    }
                    else
                    {
                        pnlExistingCover.Visible = false;
                    }


                    // ✅ Sonuçları ve soruları yükle
                    LoadResults();
                    LoadQuestions();

                    // ✅ Soru sıra numarasını ayarla
                    if (!string.IsNullOrEmpty(hfCurrentQuizId.Value) && hfCurrentQuizId.Value != "0")
                    {
                        string sqlMaxOrder = "SELECT COALESCE(MAX(OrderNo), 0) FROM Questions WHERE QuizId=@qid";
                        int maxOrder = Convert.ToInt32(DbHelper.Scalar(sqlMaxOrder, new MySqlParameter("@qid", quizId)));
                        txtQuestionOrder.Text = (maxOrder + 1).ToString();
                    }

                    // ✅ CKEditor'ü güncelle
                    string description = row["Description"].ToString().Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "");
                    string script = $@"
                setTimeout(function() {{
                    if (CKEDITOR.instances['{{txtDescription.ClientID}}']) {{
                        CKEDITOR.instances['{{txtDescription.ClientID}}'].setData('{description}');
                    }}
                }}, 500);
            ";

                    ScriptManager.RegisterStartupScript(this, GetType(), "loadCKEditor", script, true);

                    lblMessage1.Text = "<span class='text-info'>✏️ Quiz düzenleme modunda...</span>";
                }
                else
                {
                    lblMessage1.Text = "<span class='text-danger'>❌ Quiz bulunamadı!</span>";
                }
            }
            catch (Exception ex)
            {
                lblMessage1.Text = $"<span class='text-danger'>❌ Yükleme hatası: {ex.Message}</span>";
                System.Diagnostics.Debug.WriteLine($"LoadQuizForEdit Hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Quiz kaydedildikten sonra form alanlarını tekrar yükle
        /// </summary>
        private void ReloadQuizInfo(int quizId)
        {
            try
            {
                string sql = "SELECT CoverImageUrl, Title, Description FROM Quizzes WHERE Id=@id";
                var dt = DbHelper.Query(sql, new MySqlParameter("@id", quizId));

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];

                    // Kapak görselini göster
                    string coverImageUrl = row["CoverImageUrl"]?.ToString();
                    if (!string.IsNullOrEmpty(coverImageUrl))
                    {
                        pnlExistingCover.Visible = true;
                        lblExistingCover.Text = $"✅ Mevcut kapak: {System.IO.Path.GetFileName(coverImageUrl)}";

                        // Görüntüyü önizleme alanına yükle
                        string coverScript = $@"
                    setTimeout(function() {{
                        var previewContainer = document.querySelector('#previewCoverImage .preview-image-container');
                        if (previewContainer) {{
                            previewContainer.innerHTML = '<img src=""{coverImageUrl}"" style=""max-width: 100%; border-radius: 8px;"" />';
                            document.getElementById('previewCoverImage').style.display = 'block';
                        }}
                    }}, 300);
                ";
                        ScriptManager.RegisterStartupScript(this, GetType(), "reloadCoverPreview", coverScript, true);
                    }
                    else
                    {
                        pnlExistingCover.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ReloadQuizInfo Hatası: {ex.Message}");
            }
        }

        private string UploadFile(FileUpload fileUpload, string folder)
        {
            // ✅ Dosya seçilmedi mi kontrol et
            if (!fileUpload.HasFile)
            {
                System.Diagnostics.Debug.WriteLine($"[UploadFile] Dosya seçilmedi ({folder})");
                return null;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"[UploadFile] Başladı - Klasör: {folder}");
                System.Diagnostics.Debug.WriteLine($"[UploadFile] Dosya adı: {fileUpload.FileName}");
                System.Diagnostics.Debug.WriteLine($"[UploadFile] Dosya boyutu: {fileUpload.PostedFile.ContentLength} bytes");

                // ✅ Uzantı kontrolü
                string extension = Path.GetExtension(fileUpload.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };

                if (Array.IndexOf(allowedExtensions, extension) == -1)
                {
                    System.Diagnostics.Debug.WriteLine($"[UploadFile] ❌ Geçersiz uzantı: {extension}");
                    System.Diagnostics.Debug.WriteLine($"[UploadFile] İzin verilenler: {string.Join(", ", allowedExtensions)}");

                    lblMessage1.Text = $"<span class='text-danger'>❌ Geçersiz dosya tipi: {extension}<br/>İzin verilenler: JPG, PNG, GIF, WEBP</span>";
                    return null;
                }

                // ✅ Boyut kontrolü (5MB)
                if (fileUpload.PostedFile.ContentLength > 5 * 1024 * 1024)
                {
                    System.Diagnostics.Debug.WriteLine($"[UploadFile] ❌ Dosya çok büyük: {fileUpload.PostedFile.ContentLength / 1024 / 1024}MB");

                    lblMessage1.Text = "<span class='text-danger'>❌ Dosya boyutu 5MB'dan büyük olamaz!</span>";
                    return null;
                }

                // ✅ Dosya adı oluştur
                string fileName = Guid.NewGuid() + extension;
                System.Diagnostics.Debug.WriteLine($"[UploadFile] Yeni dosya adı: {fileName}");

                // ✅ Klasör yolunu oluştur
                string folderPath = Server.MapPath($"~/Uploads/{folder}/");
                System.Diagnostics.Debug.WriteLine($"[UploadFile] Klasör yolu: {folderPath}");

                // ✅ Klasör yoksa oluştur
                if (!Directory.Exists(folderPath))
                {
                    System.Diagnostics.Debug.WriteLine($"[UploadFile] Klasör oluşturuluyor...");
                    Directory.CreateDirectory(folderPath);
                    System.Diagnostics.Debug.WriteLine($"[UploadFile] ✅ Klasör oluşturuldu");
                }

                // ✅ Tam dosya yolu
                string fullPath = Path.Combine(folderPath, fileName);
                System.Diagnostics.Debug.WriteLine($"[UploadFile] Tam yol: {fullPath}");

                // ✅ Dosyayı kaydet
                fileUpload.SaveAs(fullPath);
                System.Diagnostics.Debug.WriteLine($"[UploadFile] ✅ Dosya kaydedildi!");

                // ✅ Web URL'sini döndür
                string webUrl = $"/Uploads/{folder}/{fileName}";
                System.Diagnostics.Debug.WriteLine($"[UploadFile] ✅ Web URL: {webUrl}");

                return webUrl;
            }
            catch (UnauthorizedAccessException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UploadFile] ❌ YETKİ HATASI: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[UploadFile] Uploads klasörüne yazma izni yok!");

                lblMessage1.Text = "<span class='text-danger'>❌ Dosya yükleme izni yok! IIS ayarlarını kontrol edin.</span>";
                return null;
            }
            catch (DirectoryNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UploadFile] ❌ KLASÖR BULUNAMADI: {ex.Message}");

                lblMessage1.Text = "<span class='text-danger'>❌ Uploads klasörü bulunamadı!</span>";
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UploadFile] ❌ GENEL HATA: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[UploadFile] Stack Trace: {ex.StackTrace}");

                lblMessage1.Text = $"<span class='text-danger'>❌ Dosya yükleme hatası: {ex.Message}</span>";
                return null;
            }
        }

        private string GenerateSlug(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";

            text = text.Replace("ı", "i").Replace("İ", "i")
                       .Replace("ğ", "g").Replace("Ğ", "g")
                       .Replace("ü", "u").Replace("Ü", "u")
                       .Replace("ş", "s").Replace("Ş", "s")
                       .Replace("ö", "o").Replace("Ö", "o")
                       .Replace("ç", "c").Replace("Ç", "c");

            text = text.ToLowerInvariant();
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            text = text.Replace(" ", "-");

            return text;
        }

        #endregion
    }
}