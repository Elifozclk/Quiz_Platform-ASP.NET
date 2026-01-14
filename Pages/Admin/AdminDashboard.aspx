<%@ Page Title="Admin Panel" Language="C#" MasterPageFile="~/MasterPages/Admin.master" 
    AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" 
    Inherits="QP_WEBPROJECT.vs2.Pages.Admin.AdminDashboard" %>


<asp:Content ID="HelpTitleContent" ContentPlaceHolderID="HelpTitle" runat="server">
    Dashboard Yardım
</asp:Content>


<asp:Content ID="HelpContentMain" ContentPlaceHolderID="HelpContent" runat="server">
    <h4>📊 Dashboard</h4>
    <p>
        Admin panelinin ana sayfasıdır. Sistem özetini görüntüler ve 
        hızlı erişim sağlar.
    </p>
    
    <h4>🎯 Hızlı Erişim Kartları</h4>
    <ul>
        <li><strong>Yeni Quiz:</strong> Quiz oluşturma sayfasına git</li>
        <li><strong>Quiz Yönetimi:</strong> Mevcut quiz'leri yönet</li>
        <li><strong>İstatistikler:</strong> Sayısal verileri gör</li>
        <li><strong>Kullanıcılar:</strong> Kullanıcı yönetimine git</li>
        <li><strong>Silme Talepleri:</strong> Bekleyen talepleri gör</li>
        <li><strong>Site Önizle:</strong> Kullanıcı görünümünü aç</li>
    </ul>
    
    <div class="help-tip">
        💡 Kartlara tıklayarak ilgili sayfaya gidebilirsiniz.
    </div>
    
    <h4>📈 İstatistik Bilgileri</h4>
    <ul>
        <li><strong>Quiz Sayısı:</strong> Toplam oluşturulan quiz</li>
        <li><strong>Kullanıcılar:</strong> Kayıtlı kullanıcı sayısı</li>
        <li><strong>Aktif Kullanıcı:</strong> Son 30 günde aktif olanlar</li>
        <li><strong>Bekleyen Talepler:</strong> İşlem bekleyen silme talepleri</li>
    </ul>
    
    <h4>🔍 Üst Menü</h4>
    <ul>
        <li><strong>Dashboard:</strong> Bu sayfa</li>
        <li><strong>Quiz Yönetimi:</strong> Quiz listesi ve işlemler</li>
        <li><strong>Quiz Oluştur:</strong> Yeni quiz oluşturma</li>
        <li><strong>Kullanıcılar:</strong> Kullanıcı yönetimi</li>
        <li><strong>Profil:</strong> Admin profil bilgileri</li>
        <li><strong>Çıkış:</strong> Güvenli çıkış</li>
    </ul>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        body {
            background: linear-gradient(135deg, #fef3f8 0%, #f0f4ff 50%, #fef9f3 100%);
        }

        .admin-header {
            background: linear-gradient(135deg, #5B0E2D 0%, #8B2248 100%);
            color: white;
            padding: 25px 30px;
            border-radius: 12px;
            margin-bottom: 30px;
            box-shadow: 0 4px 15px rgba(91, 14, 45, 0.3);
        }

        .admin-header h2 {
            margin: 0;
            color: white;
            font-weight: 700;
            font-size: 1.8rem;
        }

        .welcome-text {
            font-size: 1rem;
            opacity: 0.95;
            margin-bottom: 5px;
        }

        /* Kompakt Grid */
        .quick-actions-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 18px;
        }

        .action-card {
            background: white;
            padding: 20px;
            border-radius: 12px;
            text-align: center;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
            transition: all 0.3s ease;
            border: 2px solid transparent;
            text-decoration: none;
            display: block;
        }

        .action-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 8px 20px rgba(91, 14, 45, 0.15);
            border-color: #5B0E2D;
        }

        .action-card .icon {
            font-size: 2.5rem;
            margin-bottom: 12px;
            transition: transform 0.3s;
        }

        .action-card:hover .icon {
            transform: scale(1.1);
        }

        .action-card.quiz-create .icon { color: #10b981; }
        .action-card.quiz-management .icon { color: #3b82f6; }
        .action-card.users .icon { color: #8b5cf6; }
        .action-card.delete-requests .icon { color: #ef4444; }
        .action-card.statistics .icon { color: #f59e0b; }
        .action-card.site-preview .icon { color: #14b8a6; }

        .action-card .title {
            font-size: 1.1rem;
            font-weight: 700;
            color: #5B0E2D;
            margin-bottom: 8px;
        }

        .action-card .description {
            color: #6b7280;
            font-size: 0.85rem;
            line-height: 1.4;
            margin-bottom: 10px;
        }

        .action-card .badge {
            display: inline-block;
            background: rgba(91, 14, 45, 0.1);
            color: #5B0E2D;
            padding: 4px 12px;
            border-radius: 15px;
            font-size: 0.8rem;
            font-weight: 600;
        }

        .stat-badge {
            background: linear-gradient(135deg, #5B0E2D 0%, #8B2248 100%);
            color: white;
            padding: 5px 15px;
            border-radius: 20px;
            font-weight: 700;
            font-size: 0.95rem;
            display: inline-block;
        }
    </style>

    <!-- Admin Header -->
    <div class="admin-header">
        <div class="welcome-text">
            <asp:Label ID="lblWelcome" runat="server" />
        </div>
        <h2><i class="fa fa-shield-alt"></i> Admin Kontrol Paneli</h2>
    </div>

    <!-- Kompakt Hızlı İşlemler Grid -->
    <div class="quick-actions-grid">
        
        <!-- 1. Yeni Quiz Oluştur -->
        <a href="/Pages/Admin/QuizCreate.aspx" class="action-card quiz-create">
            <div class="icon"><i class="fa fa-plus-circle"></i></div>
            <div class="title">Yeni Quiz</div>
            <div class="description">
                Quiz oluştur
            </div>
            <div class="badge">Oluştur</div>
        </a>

        <!-- 2. Quiz Yönetimi -->
        <a href="/Pages/Admin/QuizManagement.aspx" class="action-card quiz-management">
            <div class="icon"><i class="fa fa-list-check"></i></div>
            <div class="title">Quiz Yönetimi</div>
            <div class="description">
                Düzenle ve yönet
            </div>
            <div class="stat-badge">
                <asp:Label ID="lblTotalQuizzes" runat="server" Text="0" /> Quiz
            </div>
        </a>

        <!-- 3. İstatistikler -->
        <a href="/Pages/Admin/Statistics.aspx" class="action-card statistics">
            <div class="icon"><i class="fa fa-chart-line"></i></div>
            <div class="title">İstatistikler</div>
            <div class="description">
                Detaylı veriler
            </div>
            <div class="stat-badge">
                <asp:Label ID="lblTotalUsers" runat="server" Text="0" /> Kullanıcı
            </div>
        </a>

        <!-- 4. Kullanıcı Yönetimi -->
        <a href="/Pages/Admin/UserManagement.aspx" class="action-card users">
            <div class="icon"><i class="fa fa-users"></i></div>
            <div class="title">Kullanıcılar</div>
            <div class="description">
                Kullanıcı yönetimi
            </div>
            <div class="stat-badge">
                <asp:Label ID="lblActiveUsers" runat="server" Text="0" /> Aktif
            </div>
        </a>

        <!-- 5. Silme Talepleri -->
        <a href="/Pages/Admin/DeleteRequests.aspx" class="action-card delete-requests">
            <div class="icon"><i class="fa fa-trash-alt"></i></div>
            <div class="title">Silme Talepleri</div>
            <div class="description">
                Talepleri incele
            </div>
            <div class="stat-badge">
                <asp:Label ID="lblPendingRequests" runat="server" Text="0" /> Bekliyor
            </div>
        </a>

        <!-- 6. Site Önizleme -->
        <a href="/Pages/Public/Default.aspx" class="action-card site-preview" target="_blank">
            <div class="icon"><i class="fa fa-eye"></i></div>
            <div class="title">Site Önizle</div>
            <div class="description">
                Kullanıcı görünümü
            </div>
            <div class="badge">Yeni Sekme</div>
        </a>

    </div>

</asp:Content>
