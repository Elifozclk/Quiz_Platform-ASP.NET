<%@ Page Title="Quiz Yönetimi" Language="C#" MasterPageFile="~/MasterPages/Admin.Master" 
    AutoEventWireup="true" CodeBehind="QuizManagement.aspx.cs" 
    Inherits="QP_WEBPROJECT.vs2.Pages.Admin.QuizManagement" %>



<asp:Content ID="HelpTitleContent" ContentPlaceHolderID="HelpTitle" runat="server">
    Quiz Yönetimi
</asp:Content>


<asp:Content ID="HelpContentMain" ContentPlaceHolderID="HelpContent" runat="server">
    <h4>📝 Quiz Yönetimi</h4>
    <p>
        Tüm quiz'lerinizi görüntüleyin, düzenleyin ve yönetin.
    </p>
    
    <h4>🔧 İşlem Butonları</h4>
    <ul>
        <li>
            <strong style="color: #3b82f6;">Düzenle:</strong> 
            Quiz'i düzenleme sayfasında açar
        </li>
        <li>
            <strong style="color: #8b5cf6;">Görüntüle:</strong> 
            Quiz'i kullanıcı görünümünde önizler
        </li>
        <li>
            <strong style="color: #10b981;">Yayınla:</strong> 
            Taslak quiz'i yayına alır (kullanıcılar görebilir)
        </li>
        <li>
            <strong style="color: #f59e0b;">Taslağa Al:</strong> 
            Yayınlanmış quiz'i taslağa döndürür
        </li>
        <li>
            <strong style="color: #ef4444;">Sil:</strong> 
            Quiz'i kalıcı olarak siler
        </li>
    </ul>
    
    <div class="help-warning">
        ⚠️ Silme işlemi geri alınamaz! Tüm sorular ve seçenekler de silinir.
    </div>
    
    <h4>🔍 Filtreleme</h4>
    <ul>
        <li><strong>Durum:</strong> Tümü / Yayınlanan / Taslak</li>
        <li><strong>Arama:</strong> Quiz başlığına göre ara</li>
    </ul>
    
    <h4>📊 Quiz Durumları</h4>
    <ul>
        <li>
            <strong style="color: #10b981;">✅ Yayınlandı:</strong> 
            Kullanıcılar görebilir ve çözebilir
        </li>
        <li>
            <strong style="color: #f59e0b;">📝 Taslak:</strong> 
            Sadece admin görebilir
        </li>
    </ul>
    
    <div class="help-tip">
        💡 Quiz'leri önce taslak olarak oluşturup test edin, sonra yayınlayın.
    </div>
    
    <h4>✅ Önerilen İş Akışı</h4>
    <ol>
        <li>Quiz oluştur (otomatik taslak olur)</li>
        <li>"Düzenle" ile soruları tamamla</li>
        <li>"Görüntüle" ile test et</li>
        <li>"Yayınla" butonuna bas</li>
    </ol>
    
    <h4>📋 Görüntülenen Bilgiler</h4>
    <ul>
        <li>Quiz başlığı</li>
        <li>Açıklama</li>
        <li>Durum (Yayınlandı/Taslak)</li>
        <li>Oluşturulma tarihi</li>
        <li>Soru sayısı</li>
        <li>Oluşturan admin</li>
    </ul>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>


        .btn-edit {
    background: #3b82f6 !important;
    color: white;
}

.btn-edit:hover {
    background: #2563eb !important;
}

/* Görüntüle butonu - Mor */
.btn-view {
    background: #8b5cf6 !important;
    color: white;
}

.btn-view:hover {
    background: #7c3aed !important;
}

/* Yayınla butonu - Yeşil */
.btn-publish {
    background: #10b981 !important;
    color: white;
}

.btn-publish:hover {
    background: #059669 !important;
}

/* Taslağa Al butonu - Turuncu */
.btn-draft {
    background: #f59e0b !important;
    color: white;
}

.btn-draft:hover {
    background: #d97706 !important;
}

/* Sil butonu - Kırmızı */
.btn-delete {
    background: #ef4444 !important;
    color: white;
}

.btn-delete:hover {
    background: #dc2626 !important;
}
        .quiz-management-header {
            background: linear-gradient(135deg, #5B0E2D 0%, #8B2248 100%);
            color: white;
            padding: 30px;
            border-radius: 15px;
            margin-bottom: 30px;
            box-shadow: 0 4px 15px rgba(91, 14, 45, 0.3);
        }

        .quiz-management-header h2 {
            margin: 0;
            font-weight: 700;
        }

        .quiz-management-header p {
            margin: 10px 0 0 0;
            opacity: 0.9;
        }

        .stats-row {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }

        .stat-card {
            background: white;
            padding: 20px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            text-align: center;
        }

        .stat-card .icon {
            font-size: 2.5rem;
            margin-bottom: 10px;
        }

        .stat-card.published .icon { color: #10b981; }
        .stat-card.draft .icon { color: #f59e0b; }
        .stat-card.total .icon { color: #3b82f6; }

        .stat-card .number {
            font-size: 2rem;
            font-weight: 700;
            color: #5B0E2D;
            margin-bottom: 5px;
        }

        .stat-card .label {
            color: #6b7280;
            font-weight: 500;
        }

        .filter-section {
            background: white;
            padding: 20px;
            border-radius: 12px;
            margin-bottom: 20px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }

        .filter-row {
            display: flex;
            gap: 15px;
            align-items: center;
            flex-wrap: wrap;
        }

        .filter-group {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .btn-filter {
            background: #5B0E2D;
            color: white;
            border: none;
            padding: 8px 20px;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
        }

        .btn-filter:hover {
            background: #8B2248;
        }

        .btn-create {
            background: #10b981;
            color: white;
            border: none;
            padding: 10px 25px;
            border-radius: 8px;
            font-weight: 600;
            text-decoration: none;
            display: inline-block;
            transition: all 0.3s;
        }

        .btn-create:hover {
            background: #059669;
            color: white;
        }

        .quiz-table-container {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            overflow: hidden;
        }

        .quiz-table {
            width: 100%;
            border-collapse: collapse;
        }

        .quiz-table thead {
            background: #F6EEF9;
        }

        .quiz-table th {
            padding: 15px;
            text-align: left;
            font-weight: 700;
            color: #5B0E2D;
            border-bottom: 2px solid #5B0E2D;
        }

        .quiz-table td {
            padding: 15px;
            border-bottom: 1px solid #e5e7eb;
        }

        .quiz-table tbody tr:hover {
            background: #fef3f8;
        }

        .status-badge {
            padding: 5px 12px;
            border-radius: 20px;
            font-size: 0.85rem;
            font-weight: 600;
            display: inline-block;
        }

        .status-badge.published {
            background: #d1fae5;
            color: #065f46;
        }

        .status-badge.draft {
            background: #fef3c7;
            color: #92400e;
        }

        .action-buttons {
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
        }

        .btn-action {
            padding: 6px 12px;
            border-radius: 6px;
            font-size: 0.875rem;
            font-weight: 600;
            border: none;
            cursor: pointer;
            transition: all 0.3s;
        }

        .btn-edit {
            background: #3b82f6;
            color: white;
        }

        .btn-edit:hover {
            background: #2563eb;
        }

        .btn-view {
            background: #8b5cf6;
            color: white;
        }

        .btn-view:hover {
            background: #7c3aed;
        }

        .btn-delete {
            background: #ef4444;
            color: white;
        }

        .btn-delete:hover {
            background: #dc2626;
        }

        .btn-publish {
            background: #10b981;
            color: white;
        }

        .btn-publish:hover {
            background: #059669;
        }

        .empty-state {
            text-align: center;
            padding: 60px 20px;
            color: #6b7280;
        }

        .empty-state i {
            font-size: 4rem;
            color: #d1d5db;
            margin-bottom: 20px;
        }

        .empty-state h3 {
            color: #5B0E2D;
            margin-bottom: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Header -->
    <div class="quiz-management-header">
        <h2><i class="fa fa-list-check"></i> Quiz Yönetimi</h2>
        <p>Tüm quiz'lerinizi buradan yönetebilir, düzenleyebilir ve yayınlayabilirsiniz.</p>
    </div>

    <!-- İstatistikler -->
    <div class="stats-row">
        <div class="stat-card total">
            <div class="icon"><i class="fa fa-clipboard-list"></i></div>
            <div class="number"><asp:Label ID="lblTotalQuizzes" runat="server" Text="0" /></div>
            <div class="label">Toplam Quiz</div>
        </div>
        
        <div class="stat-card published">
            <div class="icon"><i class="fa fa-circle-check"></i></div>
            <div class="number"><asp:Label ID="lblPublishedQuizzes" runat="server" Text="0" /></div>
            <div class="label">Yayınlanan</div>
        </div>
        
        <div class="stat-card draft">
            <div class="icon"><i class="fa fa-file-pen"></i></div>
            <div class="number"><asp:Label ID="lblDraftQuizzes" runat="server" Text="0" /></div>
            <div class="label">Taslak</div>
        </div>
    </div>

    <!-- Filtre ve Yeni Oluştur -->
    <div class="filter-section">
        <div class="filter-row">
            <div class="filter-group">
                <label><strong>Durum:</strong></label>
                <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-select">
                    <asp:ListItem Value="all" Selected="True">Tümü</asp:ListItem>
                    <asp:ListItem Value="Published">Yayınlanan</asp:ListItem>
                    <asp:ListItem Value="Draft">Taslak</asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="filter-group">
                <label><strong>Arama:</strong></label>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" 
                    placeholder="Quiz başlığı ara..." />
            </div>

            <asp:Button ID="btnFilter" runat="server" Text="Filtrele" 
                CssClass="btn-filter" OnClick="btnFilter_Click" />

            <div style="margin-left: auto;">
                <a href="QuizCreate.aspx" class="btn-create">
                    <i class="fa fa-plus"></i> Yeni Quiz Oluştur
                </a>
            </div>
        </div>
    </div>

    <!-- Mesaj -->
    <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false" />

    <!-- Quiz Tablosu -->
    <div class="quiz-table-container">
        <asp:GridView ID="gvQuizzes" runat="server" 
            CssClass="quiz-table"
            AutoGenerateColumns="False"
            DataKeyNames="Id"
            OnRowCommand="gvQuizzes_RowCommand"
            OnRowDataBound="gvQuizzes_RowDataBound"
            ShowHeader="true"
            GridLines="None">
            
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-Width="60px" />
                
                <asp:TemplateField HeaderText="Quiz Başlığı">
                    <ItemTemplate>
                        <strong><%# Eval("Title") %></strong>
                        <br />
                        <small style="color: #6b7280;"><%# Eval("Description") %></small>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Durum" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <%# Eval("Status").ToString() == "Published" 
                            ? "<span class='status-badge published'><i class='fa fa-check-circle'></i> Yayınlandı</span>" 
                            : "<span class='status-badge draft'><i class='fa fa-file-pen'></i> Taslak</span>" %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="QuestionCount" HeaderText="Soru Sayısı" 
                    ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Center" />

                <asp:TemplateField HeaderText="Oluşturan" ItemStyle-Width="150px">
                    <ItemTemplate>
                        <%# Eval("CreatorName") %>
                        <br />
                        <small style="color: #6b7280;"><%# Convert.ToDateTime(Eval("CreatedAt")).ToString("dd.MM.yyyy") %></small>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="İşlemler" ItemStyle-Width="280px">
                    <ItemTemplate>
                        <div class="action-buttons">
                            <!-- Düzenle -->
                            <asp:Button ID="btnEdit" runat="server" 
                                Text="Düzenle" 
                                CssClass="btn-action btn-edit"
                                CommandName="EditQuiz"
                                CommandArgument='<%# Eval("Id") %>' />

                            <!-- Görüntüle -->
                           <asp:Button ID="btnView" runat="server" 
    Text="Görüntüle" 
    CssClass="btn-action btn-view"
    CommandName="ViewQuiz"
    CommandArgument='<%# Eval("Id") %>' />

                            <!-- Yayınla/Taslak Yap -->
                            <asp:Button ID="btnToggleStatus" runat="server" 
                                CssClass="btn-action btn-publish"
                                CommandName="ToggleStatus"
                                CommandArgument='<%# Eval("Id") %>' />

                            <!-- Sil -->
                            <asp:Button ID="btnDelete" runat="server" 
                                Text="Sil" 
                                CssClass="btn-action btn-delete"
                                CommandName="DeleteQuiz"
                                CommandArgument='<%# Eval("Id") %>'
                                OnClientClick="return confirm('Bu quiz\'i silmek istediğinizden emin misiniz? Tüm sorular ve sonuçlar da silinecektir.');" />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>

            <EmptyDataTemplate>
                <div class="empty-state">
                    <i class="fa fa-inbox"></i>
                    <h3>Henüz quiz oluşturulmamış</h3>
                    <p>Yeni bir quiz oluşturmak için yukarıdaki "Yeni Quiz Oluştur" butonuna tıklayın.</p>
                </div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
    <script type="text/javascript">
        function viewQuiz(quizId) {
            console.log('viewQuiz called with ID:', quizId);

            // Yolu kontrol et
            var url = '/Pages/Public/SolveQuiz.aspx?id=' + quizId;
            console.log('Opening URL:', url);

            // Yeni pencerede aç
            var newWindow = window.open(url, '_blank');

            // Popup blocker kontrolü
            if (!newWindow || newWindow.closed || typeof newWindow.closed == 'undefined') {
                alert('⚠️ Popup engelleyici aktif!\n\nLütfen bu site için popup\'ları etkinleştirin.');

                // Fallback: Aynı pencerede aç
                if (confirm('Aynı pencerede açmak ister misiniz?')) {
                    window.location.href = url;
                }
            } else {
                console.log('Window opened successfully');
            }

            return false; // Form submit'i engelle
        }
    </script>


</asp:Content>
