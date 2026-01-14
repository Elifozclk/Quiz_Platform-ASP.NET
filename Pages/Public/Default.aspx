<%@ Page Title="Ana Sayfa" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="QP_WEBPROJECT.vs2.Pages.Public.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        body { background-color: #EDE5F9 !important; }

        h2 { font-weight: 700; color: #5B0E2D; }

        p.text-muted { color: #5B0E2D; opacity: 0.8; }

        .search-box { max-width: 500px; margin: 0 auto 2rem auto; }

        .form-control {
            border: 2px solid #5B0E2D;
            border-radius: 10px 0 0 10px;
            color: #5B0E2D;
            background-color: #fff;
        }

        .form-control:focus {
            box-shadow: 0 0 8px rgba(91, 14, 45, 0.4);
            border-color: #5B0E2D;
        }

        .btn-primary {
            background-color: #5B0E2D;
            border: none;
            border-radius: 0 10px 10px 0;
            color: white;
            transition: all 0.3s ease;
            font-weight: 600;
        }

        .btn-primary:hover {
            background-color: #8B2248;
            color: #fff;
            transform: scale(1.05);
        }

        /* ✅ GRID: 1 satır 4 kart, sonra alt satır */
        div[id$="quizGrid"] {
            display: grid !important;
            grid-template-columns: repeat(4, minmax(0, 1fr)) !important;
            gap: 2rem !important;
            grid-auto-flow: row !important;

            max-width: 1600px;
            margin: 0 auto;
            padding: 0 30px 50px 30px;

            /* olası column/masonry etkisini kapat */
            column-count: initial !important;
            columns: initial !important;
        }

        .quiz-card-wrapper {
            width: 100% !important;
            float: none !important;
            display: block !important;
        }

        /* ✅ Kartlar eşit yükseklik */
        .quiz-card {
            border: none;
            border-radius: 15px;
            transition: all 0.3s ease;
            background: #F6EEF9;
            box-shadow: 0 4px 12px rgba(91, 14, 45, 0.15);

            height: 520px; /* istersen 500 / 540 */
            display: flex;
            flex-direction: column;
        }

        .quiz-card:hover {
            transform: translateY(-6px);
            background-color: #FFF4B0;
            box-shadow: 0 6px 18px rgba(91, 14, 45, 0.25);
        }

        .quiz-image {
            height: 200px;
            object-fit: cover;
            border-radius: 15px 15px 0 0;
            flex-shrink: 0;
        }

        .card-body {
            display: flex !important;
            flex-direction: column !important;
            flex: 1 !important;
            padding: 1.5rem;
        }

        .quiz-title {
            font-weight: 600;
            color: #5B0E2D;
            font-size: 1.1rem;
            line-height: 1.4;
            margin-bottom: 1rem;
            min-height: 3rem;

            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
        }

        /* ✅ Açıklama alanı kart içinde sabit kalsın */
        .quiz-text {
            color: #5B0E2D;
            font-size: 0.9rem;
            opacity: 0.9;
            line-height: 1.6;
            margin-bottom: 1rem;

            flex: 1;

            display: -webkit-box;
            -webkit-line-clamp: 4;
            -webkit-box-orient: vertical;
            overflow: hidden;
        }

        .btn-quiz {
            background-color: #5B0E2D;
            border: none;
            color: white;
            border-radius: 8px;
            transition: all 0.3s ease;
            font-weight: 500;
            padding: 0.6rem 1.2rem;

            margin-top: auto !important;
        }

        .btn-quiz:hover {
            background-color: #8B2248;
            transform: scale(1.05);
        }

        /* ✅ Responsive (istersen aynı kalsın) */
        @media (max-width: 1400px) {
            div[id$="quizGrid"] { grid-template-columns: repeat(3, 1fr) !important; }
        }

        @media (max-width: 992px) {
            div[id$="quizGrid"] { grid-template-columns: repeat(2, 1fr) !important; }
        }

        @media (max-width: 576px) {
            div[id$="quizGrid"] { grid-template-columns: 1fr !important; }
            .quiz-card { height: auto; }
        }
    </style>

    <div class="text-center mb-4">
        <h2>🌸 Kişilik & Eğlence Testleri</h2>
        <p class="text-muted">Kayıt olmadan hemen çözmeye başla!</p>
    </div>

    <!-- Arama Çubuğu -->
    <div class="search-box">
        <div class="input-group">
            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Quiz ara..." />
            <button type="button" class="btn btn-primary" runat="server" onserverclick="BtnSearch_Click">
                <i class="fa fa-search"></i> Ara
            </button>
        </div>
    </div>

    <!-- Quiz Grid -->
    <div class="quiz-grid-container" id="quizGrid" runat="server">
        <asp:Repeater ID="rptQuizzes" runat="server">
            <ItemTemplate>

                <div class="quiz-card-wrapper">
                    <div class="card quiz-card">

                        <%# !string.IsNullOrWhiteSpace(Eval("CoverImageUrl")?.ToString())
                            ? "<img src='" + Eval("CoverImageUrl") + "' alt='Quiz' class='quiz-image' />"
                            : "<div class='quiz-image' style='background: linear-gradient(135deg, #F6EEF9 0%, #e2d4ec 100%); display:flex; align-items:center; justify-content:center;'><i class='fa fa-image' style='font-size: 3rem; color: #8B2248; opacity: 0.2;'></i></div>" %>

                        <div class="card-body text-center">
                            <h5 class="quiz-title"><%# Eval("Title") %></h5>

                            <!-- ✅ ÖNEMLİ: HTML kırpma yok, temizlenmiş metin -->
                            <p class="quiz-text"><%# SafeSnippet(Eval("Description")) %></p>

                            <div class="mb-2">
                                <small class="text-muted">
                                    <i class="fa fa-clock"></i> <%# Eval("EstimatedTime") %> dk
                                </small>
                            </div>

                            <a href='<%# "/Pages/Public/SolveQuiz.aspx?slug=" + Eval("Slug") %>'
                               class="btn btn-quiz mt-2">
                                <i class="fa fa-play"></i> Hemen Çöz
                            </a>
                        </div>

                    </div>
                </div>

            </ItemTemplate>
        </asp:Repeater>
    </div>

    <asp:Panel ID="pnlNoResult" runat="server" Visible="false">
        <div class="alert alert-light border text-center mt-4" style="color:#5B0E2D;">
            Henüz yayınlanmış bir quiz bulunamadı.
        </div>
    </asp:Panel>

</asp:Content>
