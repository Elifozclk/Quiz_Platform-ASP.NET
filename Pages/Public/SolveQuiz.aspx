<%@ Page Title="Quiz Çöz" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="SolveQuiz.aspx.cs" 
    Inherits="QP_WEBPROJECT.vs2.Pages.Public.SolveQuiz" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <!-- ⭐⭐⭐ ADMIN KONTROL ÇUBUĞU ⭐⭐⭐ -->
    <asp:Panel ID="pnlAdminControls" runat="server" Visible="false">
        <div style="background: linear-gradient(135deg, #ff6b6b 0%, #ee5a6f 100%); 
                    color: white; 
                    padding: 15px 20px; 
                    border-radius: 12px; 
                    margin: 20px auto; 
                    max-width: 1400px; 
                    display: flex; 
                    justify-content: space-between; 
                    align-items: center;
                    box-shadow: 0 4px 15px rgba(255, 107, 107, 0.3);">
            
            <div style="display: flex; align-items: center; gap: 10px;">
                <i class="fas fa-crown" style="font-size: 1.5rem;"></i>
                <strong style="font-size: 1.1rem;">Admin Önizleme Modu</strong>
            </div>
            
            <div style="display: flex; gap: 10px;">
                <asp:HyperLink ID="btnAdminEdit" runat="server" 
                    CssClass="btn btn-light btn-sm"
                    style="display: inline-flex; align-items: center; gap: 8px; font-weight: 600; padding: 8px 16px;">
                    <i class="fas fa-edit"></i> Düzenle
                </asp:HyperLink>
                
                <a href="/Pages/Admin/QuizManagement.aspx" 
                   class="btn btn-light btn-sm" 
                   style="display: inline-flex; align-items: center; gap: 8px; font-weight: 600; padding: 8px 16px;">
                    <i class="fas fa-list"></i> Quiz Yönetimi
                </a>
                
                <a href="/Pages/Admin/AdminDashboard.aspx" 
                   class="btn btn-light btn-sm" 
                   style="display: inline-flex; align-items: center; gap: 8px; font-weight: 600; padding: 8px 16px;">
                    <i class="fas fa-tachometer-alt"></i> Dashboard
                </a>
            </div>
        </div>
    </asp:Panel>

    <!-- Hidden Fields -->
    <asp:HiddenField ID="hfCurrentQuestionIndex" runat="server" Value="0" />
    <asp:HiddenField ID="hfAnswers" runat="server" Value="{}" />
    <asp:HiddenField ID="hfQuizId" runat="server" />
    <asp:HiddenField ID="hfCurrentAnswer" runat="server" Value="0" />

    <style>
        body {
            background: linear-gradient(135deg, #fef3f8 0%, #f0f4ff 50%, #fef9f3 100%);
        }

        .quiz-main-container {
            max-width: 1400px;
            margin: 40px auto;
            padding: 0 20px;
        }

        .quiz-layout {
            display: grid;
            grid-template-columns: 1fr 350px;
            gap: 30px;
        }

        .quiz-content {
            min-width: 0;
        }

        .sidebar {
            position: sticky;
            top: 20px;
            height: fit-content;
        }

        /* Quiz Header */
        .quiz-header {
            background: linear-gradient(135deg, rgba(91, 14, 45, 0.95) 0%, rgba(139, 34, 72, 0.95) 100%);
            color: white;
            padding: 40px;
            border-radius: 20px;
            text-align: center;
            margin-bottom: 30px;
            box-shadow: 0 8px 25px rgba(91, 14, 45, 0.3);
        }

        .quiz-header h1 {
            font-size: 2.5rem;
            font-weight: 700;
            margin: 0 0 15px 0;
            color: white;
        }

        .quiz-header p {
            font-size: 1.1rem;
            margin: 0 0 20px 0;
            opacity: 0.95;
        }

        .quiz-meta {
            display: flex;
            justify-content: center;
            gap: 30px;
            margin-top: 20px;
        }

        .quiz-meta-item {
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: 1rem;
        }

        /* Progress Bar */
        .progress-container {
            background: white;
            padding: 20px;
            border-radius: 15px;
            margin-bottom: 25px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
        }

        .progress-text {
            text-align: center;
            color: #5B0E2D;
            font-weight: 600;
            margin-bottom: 10px;
            font-size: 1.1rem;
        }
        /* Question Image */
.question-image {
    width: 100%;
    max-width: 600px;
    height: auto;
    max-height: 400px;
    object-fit: cover;
    border-radius: 15px;
    margin: 0 auto 25px auto;
    display: block;
    box-shadow: 0 4px 15px rgba(91, 14, 45, 0.15);
}

        .progress {
            height: 15px;
            border-radius: 10px;
            background-color: #f0e6f6;
        }

        .progress-bar {
            background: linear-gradient(90deg, #5B0E2D 0%, #8B2248 100%);
            border-radius: 10px;
            transition: width 0.5s ease;
        }

        /* Question Card */
        .question-card {
            background: white;
            border-radius: 20px;
            padding: 40px;
            margin-bottom: 30px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
            animation: slideIn 0.5s ease;
        }

        @keyframes slideIn {
            from {
                opacity: 0;
                transform: translateY(20px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .question-number {
            display: inline-block;
            background: linear-gradient(135deg, #5B0E2D 0%, #8B2248 100%);
            color: white;
            padding: 8px 20px;
            border-radius: 25px;
            font-weight: 600;
            font-size: 0.95rem;
            margin-bottom: 20px;
        }

        .question-text {
            font-size: 1.4rem;
            font-weight: 600;
            color: #5B0E2D;
            margin-bottom: 30px;
            line-height: 1.6;
        }

        /* Options */
        .options-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin-bottom: 20px;
        }

        .option-card {
            background: #F6EEF9;
            border: 3px solid #e2d4ec;
            border-radius: 15px;
            padding: 20px;
            cursor: pointer;
            transition: all 0.3s ease;
            text-align: center;
        }

        .option-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 6px 20px rgba(91, 14, 45, 0.15);
            background: #FFF4B0;
            border-color: #5B0E2D;
        }

        .option-card.selected {
            background: #5B0E2D;
            border-color: #5B0E2D;
            color: white;
            transform: scale(1.05);
        }

        .option-card.selected .option-text {
            color: white;
        }

        .option-card.selected:hover {
            background: #8B2248;
        }

        .option-image {
            width: 100%;
            height: 120px;
            object-fit: cover;
            border-radius: 10px;
            margin-bottom: 15px;
        }

        .option-text {
            font-size: 0.95rem;
            font-weight: 500;
            color: #5B0E2D;
        }

        /* Navigation Buttons */
        .quiz-navigation {
            display: flex;
            justify-content: space-between;
            gap: 15px;
            margin-top: 30px;
        }

        .btn-quiz {
            padding: 15px 35px;
            border-radius: 12px;
            font-weight: 600;
            font-size: 1rem;
            border: none;
            cursor: pointer;
            transition: all 0.3s ease;
            display: inline-flex;
            align-items: center;
            gap: 10px;
        }

        .btn-primary {
            background: linear-gradient(135deg, #5B0E2D 0%, #8B2248 100%);
            color: white;
        }

        .btn-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(91, 14, 45, 0.3);
        }

        .btn-secondary {
            background: #e2d4ec;
            color: #5B0E2D;
        }

        .btn-secondary:hover {
            background: #d4c3e0;
        }

        /* Sidebar - Diğer Quiz'ler */
        /* ============================================ */
/*         SIDEBAR - DİĞER QUIZ'LER            */
/* ============================================ */

.sidebar-card {
    background: white;
    border-radius: 20px;
    padding: 25px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
    overflow: hidden;
}

.sidebar-title {
    font-size: 1.3rem;
    font-weight: 700;
    color: #5B0E2D;
    margin-bottom: 20px;
    display: flex;
    align-items: center;
    gap: 10px;
}

.sidebar-quiz-item {
    background: #F6EEF9;
    border: 2px solid #e2d4ec;
    border-radius: 12px;
    padding: 15px;
    margin-bottom: 15px;
    transition: all 0.3s ease;
    cursor: pointer;
    overflow: hidden;
    display: block;
}

.sidebar-quiz-item:hover {
    background: #FFF4B0;
    border-color: #5B0E2D;
    transform: translateX(5px);
}

.sidebar-quiz-item:last-child {
    margin-bottom: 0;
}

/* Görsel Container */
.sidebar-quiz-thumb-container {
    width: 100%;
    height: 100px;
    overflow: hidden;
    border-radius: 8px;
    margin-bottom: 10px;
    background: linear-gradient(135deg, #F6EEF9 0%, #e2d4ec 100%);
    position: relative;
}

/* Görsel */
.sidebar-quiz-thumb {
    width: 100%;
    height: 100%;
    object-fit: cover;
    display: block;
}

/* Görsel Placeholder */
.sidebar-quiz-thumb-placeholder {
    width: 100%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    background: linear-gradient(135deg, #F6EEF9 0%, #e2d4ec 100%);
}

.sidebar-quiz-thumb-placeholder i {
    font-size: 2rem;
    color: #8B2248;
    opacity: 0.3;
}

/* Quiz Başlığı */
.sidebar-quiz-title {
    font-weight: 600;
    color: #5B0E2D;
    font-size: 0.95rem;
    margin-bottom: 8px;
    overflow: hidden;
    text-overflow: ellipsis;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    line-height: 1.3;
    word-break: break-word;
}

/* Quiz Meta */
.sidebar-quiz-meta {
    display: flex;
    justify-content: space-between;
    font-size: 0.8rem;
    color: #8B2248;
    flex-wrap: wrap;
    gap: 5px;
}

.sidebar-quiz-meta span {
    display: flex;
    align-items: center;
    gap: 4px;
}



        /* Result Screen */
        /* Result Screen */
.result-container {
    background: white;
    border-radius: 20px;
    padding: 50px;
    text-align: center;
    box-shadow: 0 8px 30px rgba(0, 0, 0, 0.1);
    animation: slideIn 0.6s ease;
}

/* ✅ RESULT IMAGE - EKLE */
.result-image {
    width: 100%;
    max-width: 500px;
    height: auto;
    max-height: 350px;
    object-fit: contain;        /* ← cover yerine contain */
    border-radius: 20px;
    margin: 0 auto 30px auto;
    display: block;
    box-shadow: 0 8px 30px rgba(91, 14, 45, 0.2);
}

.result-icon {
    font-size: 5rem;
    margin-bottom: 20px;
}

        .result-title {
            font-size: 2.5rem;
            font-weight: 700;
            color: #5B0E2D;
            margin-bottom: 20px;
        }

        .result-description {
            font-size: 1.2rem;
            color: #6c4377;
            line-height: 1.8;
            margin-bottom: 30px;
        }

        .result-actions {
            display: flex;
            justify-content: center;
            gap: 15px;
            flex-wrap: wrap;
        }

        /* Responsive */
        @media (max-width: 992px) {
            .quiz-layout {
                grid-template-columns: 1fr;
            }

            .sidebar {
                position: relative;
                top: 0;
            }
        }
    </style>

    <div class="quiz-main-container">
        <div class="quiz-layout">
            <!-- Sol Taraf - Quiz İçeriği -->
            <div class="quiz-content">
                <!-- Quiz Header Panel -->
                <asp:Panel ID="pnlQuizHeader" runat="server" Visible="true">
                    <div class="quiz-header">
                        <h1><asp:Label ID="lblQuizTitle" runat="server" /></h1>
                        <p><asp:Label ID="lblQuizDescription" runat="server" /></p>
                        <div class="quiz-meta">
                            <div class="quiz-meta-item">
                                <i class="fa fa-question-circle"></i>
                                <span><asp:Label ID="lblTotalQuestions" runat="server" /> Soru</span>
                            </div>
                            <div class="quiz-meta-item">
                                <i class="fa fa-clock"></i>
                                <span><asp:Label ID="lblEstimatedTime" runat="server" /> dakika</span>
                            </div>
                        </div>
                        <asp:Button ID="btnStartQuiz" runat="server" Text="🚀 Başla" 
                            CssClass="btn-quiz btn-primary mt-4" OnClick="btnStartQuiz_Click" />
                    </div>
                </asp:Panel>

                <!-- Progress Bar -->
                <asp:Panel ID="pnlProgress" runat="server" Visible="false">
                    <div class="progress-container">
                        <div class="progress-text">
                            Soru <asp:Label ID="lblCurrentQuestion" runat="server" /> / 
                            <asp:Label ID="lblTotalQuestionsProgress" runat="server" />
                        </div>
                        <div class="progress">
                            <div class="progress-bar" id="progressBar" style="width: 0%"></div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Question Panel -->
                <asp:Panel ID="pnlQuestion" runat="server" Visible="false">
    <div class="question-card">
        <span class="question-number">Soru <asp:Label ID="lblQuestionNumber" runat="server" /></span>
        
        <!-- ✅ SORU GÖRSELİ EKLENDI -->
        <asp:Image ID="imgQuestion" runat="server" Visible="false" 
                   CssClass="question-image mb-3" />
        
        <div class="question-text">
            <asp:Label ID="lblQuestionText" runat="server" />
        </div>

        <div class="options-grid">
            <asp:Repeater ID="rptOptions" runat="server">
                <ItemTemplate>
                    <div class="option-card" onclick="selectOption(<%# Eval("Id") %>)">
                        <%# !string.IsNullOrEmpty(Eval("ImageUrl")?.ToString()) 
                            ? "<img src='" + Eval("ImageUrl") + "' class='option-image' />" : "" %>
                        <div class="option-text"><%# Eval("Text") %></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div class="quiz-navigation">
            <asp:Button ID="btnPrevious" runat="server" Text="◀ Önceki" 
                CssClass="btn-quiz btn-secondary" OnClick="btnPrevious_Click" Visible="false" />
            <div style="flex-grow: 1;"></div>
            <asp:Button ID="btnNext" runat="server" Text="Sonraki ▶" 
                CssClass="btn-quiz btn-primary" OnClick="btnNext_Click" />
        </div>
    </div>
</asp:Panel>

                <!-- Result Panel -->
                <asp:Panel ID="pnlResult" runat="server" Visible="false">
                    <div class="result-container">
                        <asp:Image ID="imgResult" runat="server" Visible="false" 
                   CssClass="result-image mb-4" />
                        <div class="result-icon"></div>
                        <h2 class="result-title"><asp:Label ID="lblResultTitle" runat="server" /></h2>
                        <p class="result-description"><asp:Label ID="lblResultDescription" runat="server" /></p>
                        
                        <div class="result-actions">
                            <a href="/Pages/Public/Default.aspx" class="btn-quiz btn-primary">
                                <i class="fa fa-home"></i> Ana Sayfa
                            </a>
                            <asp:Button ID="btnRetakeQuiz" runat="server" Text="🔄 Tekrar Çöz" 
                                CssClass="btn-quiz btn-secondary" OnClick="btnRetakeQuiz_Click" />
                        </div>
                    </div>
                </asp:Panel>

                <!-- Error Panel -->
                <asp:Panel ID="pnlError" runat="server" Visible="false">
                    <div class="alert alert-danger">
                        <i class="fa fa-exclamation-triangle"></i>
                        <asp:Literal ID="lblError" runat="server"></asp:Literal>
                    </div>
                </asp:Panel>
            </div>

            <!-- Sağ Taraf - Diğer Quiz'ler Sidebar -->
            <div class="sidebar">
                <div class="sidebar-card">
                    <div class="sidebar-title">
                        <i class="fa fa-fire"></i> Diğer Quiz'ler
                    </div>
                    
                    <asp:Repeater ID="rptSidebarQuizzes" runat="server">
    <ItemTemplate>
        <a href='<%# "/Pages/Public/SolveQuiz.aspx?slug=" + Eval("Slug") %>' 
           style="text-decoration: none; color: inherit;">
            <div class="sidebar-quiz-item">
                <!-- ✅ Görsel Container Eklendi -->
                <div class="sidebar-quiz-thumb-container">
                    <%# !string.IsNullOrEmpty(Eval("CoverImageUrl")?.ToString()) 
                        ? "<img src='" + Eval("CoverImageUrl") + "' class='sidebar-quiz-thumb' alt='Quiz Cover' />" 
                        : "<div class='sidebar-quiz-thumb sidebar-quiz-thumb-placeholder'><i class='fa fa-image'></i></div>" %>
                </div>
                
                <div class="sidebar-quiz-title"><%# Eval("Title") %></div>
                <div class="sidebar-quiz-meta">
                    <span><i class="fa fa-clock"></i> <%# Eval("EstimatedTime") %> dk</span>
                    <span><i class="fa fa-star"></i> Popüler</span>
                </div>
            </div>
        </a>
    </ItemTemplate>
</asp:Repeater>
                </div>
            </div>
        </div>
    </div>

    <script>
        let selectedOptionId = null;

        function selectOption(optionId) {
            document.querySelectorAll('.option-card').forEach(card => {
                card.classList.remove('selected');
            });

            event.currentTarget.classList.add('selected');
            selectedOptionId = optionId;

            document.getElementById('<%= hfCurrentAnswer.ClientID %>').value = optionId;
        }

        function updateProgress(current, total) {
            const percentage = (current / total) * 100;
            document.getElementById('progressBar').style.width = percentage + '%';
        }
    </script>
</asp:Content>