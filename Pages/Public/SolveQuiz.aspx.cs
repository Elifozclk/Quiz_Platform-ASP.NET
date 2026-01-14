using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace QP_WEBPROJECT.vs2.Pages.Public
{
    public partial class SolveQuiz : System.Web.UI.Page
    {
        private DataTable dtQuestions;
        private DataTable dtQuiz;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // ✅ Hem ID hem SLUG desteği
                string slug = Request.QueryString["slug"];
                string id = Request.QueryString["id"];

                // ID ile açılmaya çalışılıyorsa, önce slug'a çevir
                if (!string.IsNullOrEmpty(id))
                {
                    System.Diagnostics.Debug.WriteLine($"[Page_Load] ID ile açıldı: {id}");

                    // ID'den slug'ı çek
                    string sqlGetSlug = "SELECT Slug FROM Quizzes WHERE Id=@id";
                    var dtSlug = DbHelper.Query(sqlGetSlug, new MySqlParameter("@id", id));

                    if (dtSlug != null && dtSlug.Rows.Count > 0)
                    {
                        slug = dtSlug.Rows[0]["Slug"].ToString();
                        System.Diagnostics.Debug.WriteLine($"[Page_Load] Slug bulundu: {slug}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[Page_Load] ❌ Quiz ID {id} bulunamadı!");
                        ShowError("Quiz bulunamadı!");
                        return;
                    }
                }

                // Slug kontrolü
                if (string.IsNullOrEmpty(slug))
                {
                    ShowError("Quiz bulunamadı!");
                    return;
                }

                LoadQuiz(slug);
                if (Session["UserType"]?.ToString() == "admin")
                {
                    pnlAdminControls.Visible = true;

                    // Quiz ID'yi al
                    string quizId = Request.QueryString["id"];
                    if (string.IsNullOrEmpty(quizId) && !string.IsNullOrEmpty(slug))
                    {
                        // Slug'dan ID'yi çek
                        string sqlGetId = "SELECT Id FROM Quizzes WHERE Slug=@slug";
                        var dtId = DbHelper.Query(sqlGetId, new MySqlParameter("@slug", slug));
                        if (dtId != null && dtId.Rows.Count > 0)
                        {
                            quizId = dtId.Rows[0]["Id"].ToString();
                        }
                    }

                    // ⭐ DÜZELTME: PostBackUrl yerine NavigateUrl
                    if (!string.IsNullOrEmpty(quizId))
                    {
                        btnAdminEdit.NavigateUrl = $"/Pages/Admin/QuizCreate.aspx?id={quizId}";
                    }
                }
                else
                {
                    pnlAdminControls.Visible = false;
                }
            }
        }



        private void LoadQuiz(string slug)
        {
            try
            {
                // Quiz bilgilerini çek
                bool isAdmin = Session["UserType"]?.ToString() == "admin";

                string sqlQuiz = @"SELECT Id, Title, Description, EstimatedTime, QuizType, Status 
   FROM Quizzes 
   WHERE Slug=@slug";

                // ✅ Admin değilse sadece Published göster
                if (!isAdmin)
                {
                    sqlQuiz += " AND Status='Published'";
                }

                dtQuiz = DbHelper.Query(sqlQuiz, new MySqlParameter("@slug", slug));

                if (dtQuiz.Rows.Count == 0)
                {
                    ShowError("Quiz bulunamadı veya yayında değil!");
                    return;
                }

                var quiz = dtQuiz.Rows[0];
                hfQuizId.Value = quiz["Id"].ToString();

                // Soruları çek
                // Soruları çek
                string sqlQuestions = @"SELECT Id, Text, ImageUrl, OrderNo 
                        FROM Questions 
                        WHERE QuizId=@qid 
                        ORDER BY OrderNo";

                dtQuestions = DbHelper.Query(sqlQuestions,
                    new MySqlParameter("@qid", quiz["Id"]));

                if (dtQuestions.Rows.Count == 0)
                {
                    // ✅ Admin için özel mesaj
                    isAdmin = Session["UserType"]?.ToString().ToLower() == "admin";
                    string status = quiz["Status"]?.ToString() ?? "";

                    if (isAdmin && status == "Draft")
                    {
                        // Taslak + Admin → Düzenle linki ile bilgilendirme
                        int quizId = Convert.ToInt32(quiz["Id"]);
                        string editUrl = $"/Pages/Admin/QuizCreate.aspx?id={quizId}";

                        string adminMessage = $@"
            <div style='background: linear-gradient(135deg, #FFF3CD 0%, #FFE69C 100%); 
                        padding: 30px; border-radius: 15px; border: 3px solid #FFC107;
                        text-align: center; max-width: 600px; margin: 50px auto;'>
                <i class='fas fa-exclamation-triangle' style='font-size: 48px; color: #856404; margin-bottom: 20px;'></i>
                <h2 style='color: #856404; margin-bottom: 15px;'>
                    ⚠️ Taslak Quiz - Soru Eklenmemiş
                </h2>
                <p style='color: #856404; font-size: 16px; line-height: 1.6; margin-bottom: 20px;'>
                    Bu quiz henüz <strong>taslak durumunda</strong> ve <strong>hiç soru eklenmemiş</strong>.<br/>
                    Quiz'i görüntüleyebilmek için önce soru eklemelisiniz.
                </p>
                <a href='{editUrl}' 
                   style='display: inline-block; background: #FFC107; color: #000; 
                          padding: 12px 30px; border-radius: 8px; text-decoration: none;
                          font-weight: bold; font-size: 16px; margin-top: 10px;'>
                    <i class='fas fa-edit'></i> Quiz'i Düzenle ve Soru Ekle
                </a>
                <div style='margin-top: 20px;'>
                    <a href='/Pages/Admin/QuizManagement.aspx' 
                       style='color: #856404; text-decoration: underline;'>
                        ← Quiz Yönetimine Dön
                    </a>
                </div>
            </div>
        ";

                        ShowError(adminMessage);
                    }
                    else
                    {
                        // Normal kullanıcı → Basit mesaj
                        ShowError("Bu quiz'de henüz soru bulunmuyor!");
                    }

                    return;
                }

                // Session'a kaydet
                Session["QuizData"] = dtQuiz;
                Session["QuestionsData"] = dtQuestions;

                // Header bilgilerini doldur
                lblQuizTitle.Text = quiz["Title"].ToString();
                lblQuizDescription.Text = quiz["Description"].ToString();
                lblEstimatedTime.Text = quiz["EstimatedTime"].ToString();
                lblTotalQuestions.Text = dtQuestions.Rows.Count.ToString();

                pnlQuizHeader.Visible = true;

                // Sidebar quiz'leri yükle
                LoadSidebarQuizzes(Convert.ToInt32(quiz["Id"]));
            }
            catch (Exception ex)
            {
                ShowError("Hata: " + ex.Message);
            }
        }

        private void LoadSidebarQuizzes(int currentQuizId)
        {
            try
            {
                // ✅ ThumbnailUrl yerine CoverImageUrl
                string sql = @"SELECT 
                        Id, 
                        Title, 
                        Slug, 
                        CoverImageUrl, 
                        EstimatedTime 
                       FROM Quizzes 
                       WHERE Status='Published' AND Id != @currentId 
                       ORDER BY RAND() 
                       LIMIT 5";

                var dt = DbHelper.Query(sql, new MySqlParameter("@currentId", currentQuizId));

                // ✅ DEBUG: Hangi quiz'lerde resim var?
                System.Diagnostics.Debug.WriteLine("========== SIDEBAR QUIZ'LER ==========");
                foreach (DataRow row in dt.Rows)
                {
                    string coverUrl = row["CoverImageUrl"]?.ToString() ?? "";
                    string hasImage = !string.IsNullOrEmpty(coverUrl) ? "✅ VAR" : "❌ YOK";
                    System.Diagnostics.Debug.WriteLine($"Quiz {row["Id"]}: {row["Title"]}");
                    System.Diagnostics.Debug.WriteLine($"   CoverImageUrl: '{coverUrl}' - {hasImage}");
                }
                System.Diagnostics.Debug.WriteLine("=====================================");

                rptSidebarQuizzes.DataSource = dt;
                rptSidebarQuizzes.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Sidebar quiz yükleme hatası: " + ex.Message);
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        protected void btnStartQuiz_Click(object sender, EventArgs e)
        {
            hfCurrentQuestionIndex.Value = "0";
            hfAnswers.Value = "{}";

            pnlQuizHeader.Visible = false;
            pnlProgress.Visible = true;
            pnlQuestion.Visible = true;

            LoadQuestion(0);
        }

        private void LoadQuestion(int index)
        {
            try
            {
                dtQuestions = (DataTable)Session["QuestionsData"];

                if (dtQuestions == null || index >= dtQuestions.Rows.Count)
                {
                    System.Diagnostics.Debug.WriteLine($"[LoadQuestion] Son soru, sonuç hesaplanıyor... Index: {index}");
                    CalculateResult();
                    return;
                }

                var question = dtQuestions.Rows[index];
                int questionId = Convert.ToInt32(question["Id"]);

                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine($"[LoadQuestion] Soru {index + 1} yükleniyor... QuestionId: {questionId}");

                // Progress güncelle
                lblCurrentQuestion.Text = (index + 1).ToString();
                lblTotalQuestionsProgress.Text = dtQuestions.Rows.Count.ToString();
                lblQuestionNumber.Text = (index + 1).ToString();

                // Soru bilgileri
                lblQuestionText.Text = question["Text"].ToString();

                // ✅ SORU GÖRSELİ - NULL-SAFE VERSİYON
                try
                {
                    // ✅ imgQuestion kontrolü ekle
                    if (imgQuestion != null)
                    {
                        if (question.Table.Columns.Contains("ImageUrl"))
                        {
                            System.Diagnostics.Debug.WriteLine($"[LoadQuestion] ImageUrl kolonu var");

                            if (question["ImageUrl"] != DBNull.Value)
                            {
                                string questionImageUrl = question["ImageUrl"].ToString();

                                System.Diagnostics.Debug.WriteLine($"[LoadQuestion] ImageUrl değeri: '{questionImageUrl}'");

                                if (!string.IsNullOrEmpty(questionImageUrl))
                                {
                                    imgQuestion.ImageUrl = questionImageUrl;
                                    imgQuestion.Visible = true;

                                    System.Diagnostics.Debug.WriteLine($"✅ [LoadQuestion] Soru görseli yüklendi: {questionImageUrl}");
                                }
                                else
                                {
                                    imgQuestion.Visible = false;
                                    System.Diagnostics.Debug.WriteLine($"⚠️ [LoadQuestion] ImageUrl boş string");
                                }
                            }
                            else
                            {
                                imgQuestion.Visible = false;
                                System.Diagnostics.Debug.WriteLine($"⚠️ [LoadQuestion] ImageUrl NULL");
                            }
                        }
                        else
                        {
                            imgQuestion.Visible = false;
                            System.Diagnostics.Debug.WriteLine($"❌ [LoadQuestion] ImageUrl kolonu bulunamadı!");
                            System.Diagnostics.Debug.WriteLine($"[LoadQuestion] Mevcut kolonlar:");
                            foreach (DataColumn col in question.Table.Columns)
                            {
                                System.Diagnostics.Debug.WriteLine($"  - {col.ColumnName}");
                            }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌❌❌ [LoadQuestion] imgQuestion kontrolü NULL! SolveQuiz.aspx'de <asp:Image ID='imgQuestion'> eksik!");
                    }
                }
                catch (Exception imgEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ [LoadQuestion] Görsel yükleme hatası: {imgEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack Trace: {imgEx.StackTrace}");
                    imgQuestion.Visible = false;
                }
                // ✅ SORU GÖRSELİ - EKLENEN KOD BİTİŞ

                // Seçenekleri yükle
                string sqlOptions = @"SELECT Id, Text, ImageUrl, OrderNo, PersonalityScores 
                              FROM Options 
                              WHERE QuestionId=@qid 
                              ORDER BY OrderNo";

                System.Diagnostics.Debug.WriteLine($"[LoadQuestion] Seçenekler yükleniyor...");
                var dtOptions = DbHelper.Query(sqlOptions, new MySqlParameter("@qid", questionId));

                if (dtOptions.Rows.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ [LoadQuestion] Seçenek bulunamadı!");
                    ShowError("Bu soruda seçenek bulunamadı!");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[LoadQuestion] {dtOptions.Rows.Count} seçenek bulundu");

                rptOptions.DataSource = dtOptions;
                rptOptions.DataBind();

                // Session'a kaydet
                Session["CurrentOptions"] = dtOptions;

                // Navigasyon butonları
                btnPrevious.Visible = index > 0;
                btnNext.Text = (index == dtQuestions.Rows.Count - 1) ? "✓ Bitir" : "Sonraki ▶";

                // Progress bar güncelle
                ScriptManager.RegisterStartupScript(this, GetType(), "updateProgress",
                    $"updateProgress({index + 1}, {dtQuestions.Rows.Count});", true);

                System.Diagnostics.Debug.WriteLine($"✅ [LoadQuestion] Soru başarıyla yüklendi");
                System.Diagnostics.Debug.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine($"❌❌❌ [LoadQuestion] HATA! ❌❌❌");
                System.Diagnostics.Debug.WriteLine($"Mesaj: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                System.Diagnostics.Debug.WriteLine("========================================");

                ShowError("Soru yükleme hatası: " + ex.Message);
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                int currentIndex = Convert.ToInt32(hfCurrentQuestionIndex.Value);
                dtQuestions = (DataTable)Session["QuestionsData"];

                System.Diagnostics.Debug.WriteLine($"[btnNext_Click] Mevcut index: {currentIndex}");

                // Cevabı kaydet
                string selectedAnswer = hfCurrentAnswer.Value;

                if (!string.IsNullOrEmpty(selectedAnswer))
                {
                    int selectedOptionId = Convert.ToInt32(selectedAnswer);

                    // Cevapları JSON'a kaydet
                    var answers = new Dictionary<int, int>();

                    if (!string.IsNullOrEmpty(hfAnswers.Value) && hfAnswers.Value != "{}")
                    {
                        answers = JsonConvert.DeserializeObject<Dictionary<int, int>>(hfAnswers.Value);
                    }

                    answers[currentIndex] = selectedOptionId;
                    hfAnswers.Value = JsonConvert.SerializeObject(answers);

                    System.Diagnostics.Debug.WriteLine($"[btnNext_Click] Cevap kaydedildi: Soru {currentIndex} → Option {selectedOptionId}");
                    System.Diagnostics.Debug.WriteLine($"[btnNext_Click] Tüm cevaplar: {hfAnswers.Value}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ [btnNext_Click] Hiçbir seçenek seçilmedi!");
                    // İsteğe bağlı: Kullanıcıyı uyar
                    // ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lütfen bir seçenek seçin!');", true);
                    // return;
                }

                // Sonraki soruya geç veya sonuçları hesapla
                if (currentIndex < dtQuestions.Rows.Count - 1)
                {
                    // Sonraki soru
                    currentIndex++;
                    hfCurrentQuestionIndex.Value = currentIndex.ToString();
                    hfCurrentAnswer.Value = ""; // Yeni soru için cevabı sıfırla

                    System.Diagnostics.Debug.WriteLine($"[btnNext_Click] Sonraki soruya geçiliyor: {currentIndex}");
                    LoadQuestion(currentIndex);
                }
                else
                {
                    // Son soru, sonuçları hesapla
                    System.Diagnostics.Debug.WriteLine($"[btnNext_Click] Son soru, sonuç hesaplanıyor...");
                    CalculateResult();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ [btnNext_Click] HATA: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                ShowError("Bir hata oluştu: " + ex.Message);
            }
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                int currentIndex = Convert.ToInt32(hfCurrentQuestionIndex.Value);

                System.Diagnostics.Debug.WriteLine($"[btnPrevious_Click] Mevcut index: {currentIndex}");

                if (currentIndex > 0)
                {
                    currentIndex--;
                    hfCurrentQuestionIndex.Value = currentIndex.ToString();

                    // Önceki sorunun cevabını geri yükle (varsa)
                    var answers = new Dictionary<int, int>();
                    if (!string.IsNullOrEmpty(hfAnswers.Value) && hfAnswers.Value != "{}")
                    {
                        answers = JsonConvert.DeserializeObject<Dictionary<int, int>>(hfAnswers.Value);

                        if (answers.ContainsKey(currentIndex))
                        {
                            hfCurrentAnswer.Value = answers[currentIndex].ToString();
                            System.Diagnostics.Debug.WriteLine($"[btnPrevious_Click] Önceki cevap yüklendi: {answers[currentIndex]}");
                        }
                        else
                        {
                            hfCurrentAnswer.Value = "";
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"[btnPrevious_Click] Önceki soruya geçiliyor: {currentIndex}");
                    LoadQuestion(currentIndex);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ [btnPrevious_Click] HATA: {ex.Message}");
                ShowError("Bir hata oluştu: " + ex.Message);
            }
        }

        private void SaveAnswer(int questionIndex, int optionId)
        {
            try
            {
                var answers = JsonConvert.DeserializeObject<Dictionary<int, int>>(hfAnswers.Value);
                if (answers == null) answers = new Dictionary<int, int>();

                answers[questionIndex] = optionId;
                hfAnswers.Value = JsonConvert.SerializeObject(answers);
            }
            catch
            {
                var answers = new Dictionary<int, int>();
                answers[questionIndex] = optionId;
                hfAnswers.Value = JsonConvert.SerializeObject(answers);
            }
        }

        private void CalculateResult()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine($"[CalculateResult] BAŞLADI");

                // ✅ NULL KONTROL 1: hfAnswers kontrolü
                if (string.IsNullOrEmpty(hfAnswers.Value) || hfAnswers.Value == "{}")
                {
                    System.Diagnostics.Debug.WriteLine("❌ [CalculateResult] hfAnswers boş!");
                    ShowError("Lütfen en az bir soruyu cevaplayın!");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[CalculateResult] hfAnswers: {hfAnswers.Value}");

                // Cevapları parse et
                var answers = JsonConvert.DeserializeObject<Dictionary<int, int>>(hfAnswers.Value);

                // ✅ NULL KONTROL 2: Deserialize sonucu
                if (answers == null || answers.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("❌ [CalculateResult] Answers deserialize hatası veya boş!");
                    ShowError("Cevaplar işlenemedi! Lütfen tekrar deneyin.");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[CalculateResult] Toplam cevap sayısı: {answers.Count}");

                // Tüm puanları topla
                var totalScores = new Dictionary<string, int>();

                foreach (var answer in answers)
                {
                    int optionId = answer.Value;

                    System.Diagnostics.Debug.WriteLine($"[CalculateResult] Option ID: {optionId} için puan alınıyor...");

                    string sql = "SELECT PersonalityScores FROM Options WHERE Id=@id";
                    var dt = DbHelper.Query(sql, new MySqlParameter("@id", optionId));

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // ✅ NULL KONTROL 3: PersonalityScores
                        object scoresObj = dt.Rows[0]["PersonalityScores"];

                        if (scoresObj == null || scoresObj == DBNull.Value)
                        {
                            System.Diagnostics.Debug.WriteLine($"⚠️ [CalculateResult] Option {optionId} için PersonalityScores NULL!");
                            continue;
                        }

                        string scoresJson = scoresObj.ToString();

                        System.Diagnostics.Debug.WriteLine($"[CalculateResult] PersonalityScores JSON: {scoresJson}");

                        if (!string.IsNullOrEmpty(scoresJson))
                        {
                            try
                            {
                                var scores = JsonConvert.DeserializeObject<Dictionary<string, int>>(scoresJson);

                                if (scores != null)
                                {
                                    foreach (var score in scores)
                                    {
                                        if (!totalScores.ContainsKey(score.Key))
                                            totalScores[score.Key] = 0;

                                        totalScores[score.Key] += score.Value;

                                        System.Diagnostics.Debug.WriteLine($"[CalculateResult]   {score.Key}: +{score.Value} → Toplam: {totalScores[score.Key]}");
                                    }
                                }
                            }
                            catch (JsonException jsonEx)
                            {
                                System.Diagnostics.Debug.WriteLine($"❌ [CalculateResult] JSON parse hatası: {jsonEx.Message}");
                            }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ [CalculateResult] Option {optionId} bulunamadı!");
                    }
                }

                // ✅ NULL KONTROL 4: TotalScores boş mu?
                if (totalScores.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("❌ [CalculateResult] TotalScores boş!");
                    ShowError("Sonuç hesaplanamadı! Lütfen tüm soruları cevaplayın.");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("----------------------------------------");
                System.Diagnostics.Debug.WriteLine("[CalculateResult] Toplam Puanlar:");
                foreach (var ts in totalScores)
                {
                    System.Diagnostics.Debug.WriteLine($"  {ts.Key}: {ts.Value}");
                }

                var maxScore = totalScores.OrderByDescending(x => x.Value).First();
                string resultTag = maxScore.Key;

                System.Diagnostics.Debug.WriteLine($"[CalculateResult] En yüksek puan: {resultTag} = {maxScore.Value}");

                // ✅ NULL KONTROL 5: hfQuizId kontrolü
                if (string.IsNullOrEmpty(hfQuizId.Value))
                {
                    System.Diagnostics.Debug.WriteLine("❌ [CalculateResult] hfQuizId boş!");
                    ShowError("Quiz bilgisi bulunamadı!");
                    return;
                }

                // Sonucu veritabanından çek
                string sqlResult = @"SELECT Id, Title, Description, ImageUrl 
                             FROM Results 
                             WHERE QuizId=@qid AND Tag=@tag";

                System.Diagnostics.Debug.WriteLine($"[CalculateResult] Result sorgusu: QuizId={hfQuizId.Value}, Tag={resultTag}");

                var dtResult = DbHelper.Query(sqlResult,
                    new MySqlParameter("@qid", hfQuizId.Value),
                    new MySqlParameter("@tag", resultTag));

                // ✅ NULL KONTROL 6: Result bulundu mu?
                if (dtResult == null || dtResult.Rows.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ [CalculateResult] Sonuç bulunamadı! QuizId={hfQuizId.Value}, Tag={resultTag}");
                    ShowError($"Sonuç bulunamadı! (Tag: {resultTag}). Lütfen admin ile iletişime geçin.");
                    return;
                }

                var result = dtResult.Rows[0];

                System.Diagnostics.Debug.WriteLine($"✅ [CalculateResult] Sonuç bulundu: {result["Title"]}");

                // ✅ NULL KONTROL 7: Result alanları
                lblResultTitle.Text = result["Title"]?.ToString() ?? "Sonuç";
                lblResultDescription.Text = result["Description"]?.ToString() ?? "";

                // Sonuç görseli
                try
                {
                    // ✅ imgResult kontrolü ekle
                    if (imgResult != null)
                    {
                        if (result.Table.Columns.Contains("ImageUrl"))
                        {
                            object imageUrlObj = result["ImageUrl"];

                            if (imageUrlObj != null && imageUrlObj != DBNull.Value)
                            {
                                string resultImageUrl = imageUrlObj.ToString();

                                System.Diagnostics.Debug.WriteLine($"[CalculateResult] ImageUrl: '{resultImageUrl}'");

                                if (!string.IsNullOrEmpty(resultImageUrl))
                                {
                                    imgResult.ImageUrl = resultImageUrl;
                                    imgResult.Visible = true;

                                    System.Diagnostics.Debug.WriteLine($"✅ [CalculateResult] Sonuç görseli yüklendi: {resultImageUrl}");
                                }
                                else
                                {
                                    imgResult.Visible = false;
                                    System.Diagnostics.Debug.WriteLine($"⚠️ [CalculateResult] ImageUrl boş string");
                                }
                            }
                            else
                            {
                                imgResult.Visible = false;
                                System.Diagnostics.Debug.WriteLine($"⚠️ [CalculateResult] ImageUrl NULL");
                            }
                        }
                        else
                        {
                            imgResult.Visible = false;
                            System.Diagnostics.Debug.WriteLine($"❌ [CalculateResult] ImageUrl kolonu yok");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"❌❌❌ [CalculateResult] imgResult kontrolü NULL! SolveQuiz.aspx'de <asp:Image ID='imgResult'> eksik!");
                    }
                }
                catch (Exception imgEx)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ [CalculateResult] Görsel yükleme hatası: {imgEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack Trace: {imgEx.StackTrace}");
                }

                // Submission kaydet (eğer kullanıcı giriş yaptıysa)
                if (Session["UserId"] != null)
                {
                    try
                    {
                        object resultIdObj = result["Id"];
                        if (resultIdObj != null && resultIdObj != DBNull.Value)
                        {
                            int resultId = Convert.ToInt32(resultIdObj);
                            System.Diagnostics.Debug.WriteLine($"[CalculateResult] Kullanıcı giriş yapmış, submission kaydediliyor...");
                            SaveSubmission(resultId);
                        }
                    }
                    catch (Exception saveEx)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ [CalculateResult] Submission kayıt hatası: {saveEx.Message}");
                        // Hatayı kullanıcıya gösterme, sonuç ekranı çalışsın
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[CalculateResult] Misafir kullanıcı, submission kaydedilmedi");
                }

                // Panel görünürlüğü
                pnlProgress.Visible = false;
                pnlQuestion.Visible = false;
                pnlResult.Visible = true;

                System.Diagnostics.Debug.WriteLine("✅ [CalculateResult] Sonuç ekranı gösteriliyor");
                System.Diagnostics.Debug.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine($"❌❌❌ [CalculateResult] HATA! ❌❌❌");
                System.Diagnostics.Debug.WriteLine($"Mesaj: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                System.Diagnostics.Debug.WriteLine("========================================");

                ShowError("Sonuç hesaplama hatası: " + ex.Message + " (Lütfen tüm soruları cevapladığınızdan emin olun)");
            }
        }

        private void SaveSubmission(int resultId)
        {
            try
            {
                // ✅ Kullanıcı kontrolü
                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("[SaveSubmission] Kullanıcı giriş yapmamış, kayıt atlanıyor.");
                    return; // Anonim kullanıcılar için kayıt yapma
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                int quizId = Convert.ToInt32(hfQuizId.Value);

                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine($"[SaveSubmission] BAŞLADI");
                System.Diagnostics.Debug.WriteLine($"[SaveSubmission] UserId: {userId}");
                System.Diagnostics.Debug.WriteLine($"[SaveSubmission] QuizId: {quizId}");
                System.Diagnostics.Debug.WriteLine($"[SaveSubmission] ResultId: {resultId}");

                // Submission kaydı oluştur
                string sqlSubmission = @"INSERT INTO Submissions 
                                (QuizId, UserId, ResultId, SubmittedAt) 
                                VALUES (@qid, @uid, @rid, NOW())";

                DbHelper.Execute(sqlSubmission,
                    new MySqlParameter("@qid", quizId),
                    new MySqlParameter("@uid", userId),
                    new MySqlParameter("@rid", resultId));

                // Son eklenen Submission ID'sini al
                int submissionId = Convert.ToInt32(DbHelper.Scalar("SELECT LAST_INSERT_ID()"));

                System.Diagnostics.Debug.WriteLine($"[SaveSubmission] ✅ Submission oluşturuldu! ID: {submissionId}");

                // Tüm cevapları kaydet
                // Tüm cevapları kaydet
                var answers = JsonConvert.DeserializeObject<Dictionary<int, int>>(hfAnswers.Value);
                dtQuestions = (DataTable)Session["QuestionsData"];

                System.Diagnostics.Debug.WriteLine($"[SaveSubmission] Cevap sayısı: {answers.Count}");
                System.Diagnostics.Debug.WriteLine($"[SaveSubmission] Soru sayısı: {dtQuestions.Rows.Count}");

                int savedCount = 0;

                foreach (var answer in answers)
                {
                    int questionIndex = answer.Key;
                    int optionId = answer.Value;

                    System.Diagnostics.Debug.WriteLine($"[SaveSubmission] Cevap {savedCount + 1}: QuestionIndex={questionIndex}, OptionId={optionId}");

                    // Question ID'sini al
                    if (questionIndex < dtQuestions.Rows.Count)
                    {
                        int questionId = Convert.ToInt32(dtQuestions.Rows[questionIndex]["Id"]);

                        System.Diagnostics.Debug.WriteLine($"[SaveSubmission]   QuestionId: {questionId}");

                        // Seçeneğin puan bilgisini al
                        string sqlOption = "SELECT PersonalityScores FROM Options WHERE Id=@id";
                        var dtOption = DbHelper.Query(sqlOption, new MySqlParameter("@id", optionId));

                        if (dtOption.Rows.Count > 0)
                        {
                            string scoresJson = dtOption.Rows[0]["PersonalityScores"].ToString();

                            System.Diagnostics.Debug.WriteLine($"[SaveSubmission]   ScoreData: {scoresJson}");

                            // ✅ SubmissionAnswers'a kaydet
                            string sqlAnswer = @"INSERT INTO SubmissionAnswers 
                                        (SubmissionId, QuestionId, OptionId, ScoreData) 
                                        VALUES (@sid, @qid, @oid, @scores)";

                            DbHelper.Execute(sqlAnswer,
                                new MySqlParameter("@sid", submissionId),
                                new MySqlParameter("@qid", questionId),
                                new MySqlParameter("@oid", optionId),
                                new MySqlParameter("@scores", scoresJson ?? "{}"));

                            savedCount++;
                            System.Diagnostics.Debug.WriteLine($"[SaveSubmission]   ✅ Cevap kaydedildi!");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"[SaveSubmission]   ❌ Option bulunamadı! OptionId={optionId}");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[SaveSubmission]   ❌ QuestionIndex aralık dışında! Index={questionIndex}, Max={dtQuestions.Rows.Count}");
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[SaveSubmission] ✅ TAMAMLANDI! {savedCount}/{answers.Count} cevap kaydedildi.");
                System.Diagnostics.Debug.WriteLine("========================================");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine($"[SaveSubmission] ❌❌❌ HATA! ❌❌❌");
                System.Diagnostics.Debug.WriteLine($"Mesaj: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                System.Diagnostics.Debug.WriteLine("========================================");
            }
        }

        protected void btnRetakeQuiz_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            pnlError.Visible = true;
            pnlQuizHeader.Visible = false;
            pnlQuestion.Visible = false;
            pnlResult.Visible = false;
        }
    }
}