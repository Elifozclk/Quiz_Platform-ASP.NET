<%@ Page Title="Silme Talepleri" Language="C#" MasterPageFile="~/MasterPages/Admin.Master" 
    AutoEventWireup="true" CodeBehind="DeleteRequests.aspx.cs" 
    Inherits="QP_WEBPROJECT.vs2.Pages.Admin.DeleteRequests" %>


<asp:Content ID="HelpTitleContent" ContentPlaceHolderID="HelpTitle" runat="server">
    Silme Talepleri
</asp:Content>


<asp:Content ID="HelpContentMain" ContentPlaceHolderID="HelpContent" runat="server">
    <h4>🗑️ Silme Talepleri</h4>
    <p>
        Kullanıcıların hesap silme isteklerini yönetin.
    </p>
    
    <h4>📊 Talep Sayıları</h4>
    <ul>
        <li><strong>Bekleyen:</strong> Henüz işlem yapılmamış talepler</li>
        <li><strong>Onaylanan:</strong> Kabul edilen ve silinen hesaplar</li>
        <li><strong>Reddedilen:</strong> Reddedilen talepler</li>
    </ul>
    
    <h4>🔄 Talep Durumları</h4>
    <ul>
        <li>
            <strong style="color: #f59e0b;">🕐 Bekliyor:</strong>
            Yeni gelen, incelenmemiş talepler
        </li>
        <li>
            <strong style="color: #10b981;">✅ Onaylandı:</strong>
            Kabul edilen, hesap silinen talepler
        </li>
        <li>
            <strong style="color: #ef4444;">❌ Reddedildi:</strong>
            Uygun görülmeyen talepler
        </li>
    </ul>
    
    <h4>📋 Talep Bilgileri</h4>
    <ul>
        <li>Kullanıcı adı</li>
        <li>Email adresi</li>
        <li>Silme nedeni</li>
        <li>Talep tarihi</li>
        <li>Durum (Bekliyor/Onaylandı/Reddedildi)</li>
    </ul>
    
    <h4>✅ Talebi Onaylama</h4>
    <p>Bir talebi onayladığınızda:</p>
    <ol>
        <li>Kullanıcı hesabı kalıcı olarak silinir</li>
        <li>Tüm verileri silinir (quiz çözümleri, yorumlar)</li>
        <li>İşlem geri alınamaz!</li>
    </ol>
    
    <div class="help-warning">
        ⚠️ Onaylama geri alınamaz! Tüm veriler kalıcı olarak silinir.
    </div>
    
    <h4>❌ Talebi Reddetme</h4>
    <p>Talep reddedilebilir:</p>
    <ul>
        <li>🤖 Sahte talep şüphesi</li>
        <li>📝 Eksik bilgi</li>
        <li>⚖️ Devam eden süreç</li>
    </ul>
    
    <p>Reddettiğinizde:</p>
    <ul>
        <li>Kullanıcı hesabı korunur</li>
        <li>Kullanıcı yeni talep oluşturabilir</li>
    </ul>
    
    <div class="help-tip">
        💡 Talep reddederken açıklama eklemeyi unutmayın.
    </div>
    
    <h4>🔍 Filtreleme</h4>
    <ul>
        <li><strong>Duruma Göre:</strong> Bekleyen / Onaylanan / Reddedilen</li>
        <li><strong>Tarihe Göre:</strong> Son talepler üstte</li>
    </ul>
    
    <h4>⏱️ İşlem Süreleri</h4>
    <p>KVKK uyumluluğu için:</p>
    <ul>
        <li>✅ Talepleri 7 gün içinde yanıtlayın</li>
        <li>✅ En geç 30 gün içinde sonuçlandırın</li>
    </ul>
    
    <h4>✅ Kontrol Listesi</h4>
    <p>Her talebi işlerken:</p>
    <ul>
        <li>☑️ Kullanıcı bilgileri doğru mu?</li>
        <li>☑️ Sebep makul mü?</li>
        <li>☑️ Devam eden işlem var mı?</li>
    </ul>
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .table-warning { background-color: #fff3cd !important; }
        .table-success { background-color: #d1e7dd !important; }
        .table-danger { background-color: #f8d7da !important; }
        
        .action-buttons {
            display: flex;
            gap: 5px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <h2 class="mb-4">
            <i class="fa fa-trash"></i> Hesap Silme Talepleri
        </h2>
        
        <!-- Mesaj -->
        <asp:Label ID="lblMessage" runat="server" CssClass="alert" Visible="false"></asp:Label>
        
        <!-- İstatistik -->
        <div class="mb-3">
            <asp:Label ID="lblStats" runat="server" CssClass="badge bg-secondary p-2 fs-6"></asp:Label>
        </div>
        
        <!-- GridView -->
        <div class="card shadow-sm">
            <div class="card-body p-0">
                <asp:GridView ID="gvRequests" runat="server" 
                    CssClass="table table-hover mb-0"
                    AutoGenerateColumns="False"
                    DataKeyNames="Id"
                    OnRowCommand="gvRequests_RowCommand"
                    OnRowDataBound="gvRequests_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-Width="50px" />
                        <asp:BoundField DataField="UserName" HeaderText="Kullanıcı Adı" />
                        <asp:BoundField DataField="Email" HeaderText="E-posta" />
                        <asp:BoundField DataField="Reason" HeaderText="Sebep" />
                        
                        <asp:TemplateField HeaderText="Durum" ItemStyle-Width="120px">
                            <ItemTemplate>
                                <%# GetStatusBadge(Eval("Status")?.ToString()) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                        <asp:BoundField DataField="RequestedAt" HeaderText="Talep Tarihi" 
                            DataFormatString="{0:dd.MM.yyyy HH:mm}" ItemStyle-Width="150px" />
                        
                        <asp:TemplateField HeaderText="İşlemler" ItemStyle-Width="200px">
                            <ItemTemplate>
                                <div class="action-buttons">
                                    <!-- Onayla ve Sil Butonu -->
                                    <asp:Button ID="btnApprove" runat="server" 
                                        Text="✓ Onayla" 
                                        CssClass="btn btn-danger btn-sm"
                                        CommandName="approve"
                                        CommandArgument='<%# Eval("Id") %>'
                                        OnClientClick="return confirm('Bu kullanıcıyı kalıcı olarak silmek istediğinizden emin misiniz? Bu işlem geri alınamaz!');" 
                                        ToolTip="Talebi onayla ve kullanıcıyı sil" />
                                    
                                    <!-- Reddet Butonu -->
                                    <asp:Button ID="btnReject" runat="server" 
                                        Text="✗ Reddet" 
                                        CssClass="btn btn-secondary btn-sm"
                                        CommandName="reject"
                                        CommandArgument='<%# Eval("Id") %>'
                                        OnClientClick="return confirm('Bu talebi reddetmek istediğinizden emin misiniz?');" 
                                        ToolTip="Talebi reddet" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    
                    <EmptyDataTemplate>
                        <div class="text-center p-5">
                            <i class="fa fa-inbox" style="font-size: 48px; color: #ccc;"></i>
                            <p class="text-muted mt-3">Henüz silme talebi yok.</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Bilgi Kutusu -->
        <div class="alert alert-info mt-4">
            <h5><i class="fa fa-info-circle"></i> Bilgi</h5>
            <ul class="mb-0">
                <li><strong>Bekliyor (Sarı):</strong> Henüz işlem yapılmamış talepler</li>
                <li><strong>Onaylandı (Yeşil):</strong> Kullanıcı silindi</li>
                <li><strong>Reddedildi (Kırmızı):</strong> Talep reddedildi, kullanıcı korundu</li>
            </ul>
        </div>
    </div>
</asp:Content>
