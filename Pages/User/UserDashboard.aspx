<%@ Page Title="Kullanıcı Paneli" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="UserDashboard.aspx.cs"
    Inherits="QP_WEBPROJECT.vs2.Pages.User.UserDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        body { background-color: #EDE5F9; }

        .dashboard-header {
            background: linear-gradient(135deg, #5B0E2D 0%, #8B2248 100%);
            color: white;
            padding: 30px;
            border-radius: 15px;
            margin-bottom: 30px;
            box-shadow: 0 4px 15px rgba(91, 14, 45, 0.3);
        }

        .dashboard-header h2 { margin: 0; font-weight: 700; color: white; }
        .dashboard-header p { margin: 10px 0 0 0; opacity: 0.9; color: white; }

        .section-title {
            color: #5B0E2D;
            font-weight: 600;
            font-size: 1.3rem;
            margin: 30px 0 20px 0;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        /* ✅ Default.aspx ile aynı grid */
        .quiz-grid-container {
            display: grid !important;
            grid-template-columns: repeat(4, minmax(0, 1fr)) !important;
            gap: 2rem !important;
            grid-auto-flow: row !important;

            max-width: 1600px;
            margin: 0 auto;
            padding: 0 10px 30px 10px;

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

            height: 520px; /* istersen 500/540 */
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
            width: 100%;
        }

        .card-body {
            display: flex !important;
            flex-direction: column !important;
            flex: 1 !important;
            padding: 1.5rem;
            text-align: center;
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

        .quiz-meta {
            margin-bottom: 10px;
            font-size: 0.85rem;
            color: #8B2248;
        }

        .btn-quiz {
            background-color: #5B0E2D;
            border: none;
            color: white;
            border-radius: 8px;
            transition: all 0.3s ease;
            font-weight: 600;
            padding: 0.6rem 1.2rem;

            margin-top: auto !important;
            text-decoration: none;
            display: inline-block;
        }

        .btn-quiz:hover {
            background-color: #8B2248;
            transform: scale(1.05);
            color: white;
            text-decoration: none;
        }

        /* ✅ Responsive (Default gibi) */
        @media (max-width: 1400px) {
            .quiz-grid-container { grid-template-columns: repeat(3, 1fr) !important; }
        }

        @media (max-width: 992px) {
            .quiz-grid-container { grid-template-columns: repeat(2, 1fr) !important; }
        }

        @media (max-width: 576px) {
            .quiz-grid-container { grid-template-columns: 1fr !important; }
            .quiz-card { height: auto; }
        }
    </style>

    <div class="container">

        <!-- Dashboard Header -->
        <div class="dashboard-header">
            <h2>
                <i class="fa fa-star"></i> Hoş geldin,
                <strong><%= Session["UserName"] != null ? Session["UserName"].ToString() : "Kullanıcı" %></strong>!
            </h2>
            <p>Seni bekleyen harika quiz'leri keşfet ve eğlenmeye başla!</p>
        </div>

        <!-- Error Message -->
        <asp:Label ID="lblError" runat="server" CssClass="alert alert-danger d-block" Visible="false" />

        <!-- Section Title -->
        <div class="section-title">
            <i class="fa fa-fire"></i> Önerilen Quiz'ler
        </div>

        <!-- ✅ Quiz Grid (Default görünümü) -->
        <div class="quiz-grid-container" id="quizGridUser" runat="server">
            <asp:Repeater ID="rptQuizzes" runat="server">
                <ItemTemplate>
                    <div class="quiz-card-wrapper">
                        <div class="card quiz-card">

                            <%# !string.IsNullOrWhiteSpace(Eval("CoverImageUrl")?.ToString())
                                ? "<img src='" + Eval("CoverImageUrl") + "' alt='Quiz' class='quiz-image' />"
                                : "<div class='quiz-image' style='background: linear-gradient(135deg, #F6EEF9 0%, #e2d4ec 100%); display:flex; align-items:center; justify-content:center;'><i class='fa fa-image' style='font-size: 3rem; color: #8B2248; opacity: 0.2;'></i></div>" %>

                            <div class="card-body">
                                <h5 class="quiz-title"><%# Eval("Title") %></h5>

                                <!-- ✅ Kritik: HTML kırpma yok. Güvenli snippet. -->
                                <p class="quiz-text"><%# SafeSnippet(Eval("Description")) %></p>

                                <div class="quiz-meta">
                                    <small class="text-muted">
                                        <i class="fa fa-clock"></i> <%# Eval("EstimatedTime") %> dk
                                    </small>
                                    &nbsp; • &nbsp;
                                    <small class="text-muted">
                                        <i class="fa fa-fire"></i> Popüler
                                    </small>
                                </div>

                                <a href='<%# "/Pages/Public/SolveQuiz.aspx?slug=" + Eval("Slug") %>'
                                   class="btn btn-quiz mt-2">
                                    <i class="fa fa-play-circle"></i> Hemen Başla
                                </a>
                            </div>

                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <!-- No Quiz Panel -->
        <asp:Panel ID="pnlNoQuiz" runat="server" Visible="false">
            <div class="alert alert-info text-center mt-4">
                <i class="fa fa-info-circle"></i>
                <strong>Henüz yayınlanmış quiz bulunamadı.</strong>
                <p class="mb-0">Yeni quiz'ler eklendiğinde burada görünecek!</p>
            </div>
        </asp:Panel>

    </div>

</asp:Content>
