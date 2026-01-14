<%@ Page Title="Kullanıcı Yönetimi" Language="C#" MasterPageFile="~/MasterPages/Admin.Master"
    AutoEventWireup="true" CodeBehind="UserManagement.aspx.cs" Inherits="QP_WEBPROJECT.vs2.Pages.Admin.UserManagement" %>


<asp:Content ID="HelpTitleContent" ContentPlaceHolderID="HelpTitle" runat="server">
    Kullanıcı Yönetimi
</asp:Content>


<asp:Content ID="HelpContentMain" ContentPlaceHolderID="HelpContent" runat="server">
    <h4>👥 Kullanıcı Yönetimi</h4>
    <p>
        Kayıtlı kullanıcıları görüntüleyin ve yönetin.
    </p>
    
    <h4>📊 Kullanıcı Sayıları</h4>
    <ul>
        <li><strong>Toplam Kullanıcı:</strong> Tüm kayıtlı kullanıcılar</li>
        <li><strong>Aktif Kullanıcı:</strong> Son 30 günde giriş yapanlar</li>
        <li><strong>Admin Sayısı:</strong> Admin yetkili kullanıcılar</li>
    </ul>
    
    <h4>🔧 Kullanıcı İşlemleri</h4>
    <ul>
        <li><strong>Görüntüle:</strong> Kullanıcı detaylarını gör</li>
        <li><strong>Düzenle:</strong> Kullanıcı bilgilerini güncelle</li>
        <li><strong>Sil:</strong> Kullanıcıyı sistemden kaldır</li>
    </ul>
    
    <div class="help-warning">
        ⚠️ Kullanıcı silindiğinde tüm verileri de silinir (quiz çözümleri, yorumlar).
    </div>
    
    <h4>🔍 Arama ve Filtreleme</h4>
    <ul>
        <li><strong>İsme Göre:</strong> Kullanıcı adı ile ara</li>
        <li><strong>Email'e Göre:</strong> Email adresi ile ara</li>
        <li><strong>Role Göre:</strong> Admin / Kullanıcı</li>
    </ul>
    
    <h4>📋 Kullanıcı Bilgileri</h4>
    <p>Her kullanıcı için gösterilen bilgiler:</p>
    <ul>
        <li>Kullanıcı adı</li>
        <li>Email adresi</li>
        <li>Rol (Admin / Kullanıcı)</li>
        <li>Kayıt tarihi</li>
        <li>Son giriş tarihi</li>
        <li>Çözülen quiz sayısı</li>
    </ul>
    
    <div class="help-tip">
        💡 Kullanıcı detaylarını görüntüleyerek aktivitelerini takip edebilirsiniz.
    </div>
    
    <h4>🛡️ Güvenlik</h4>
    <p>Şüpheli durumlar:</p>
    <ul>
        <li>⚠️ Kısa sürede çok fazla hesap</li>
        <li>⚠️ Aynı IP'den birden fazla hesap</li>
        <li>⚠️ Sahte email adresleri</li>
    </ul>
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .admin-panel {
            background-color: #F6EEF9;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 0 10px rgba(91, 14, 45, 0.1);
        }
    </style>

    <div class="admin-panel">
        <h3 class="mb-4 text-center">Kullanıcı Listesi</h3>

        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger fw-bold mb-2 d-block" />
        <!-- 🔍 Arama Çubuğu -->
<div class="input-group mb-3">
    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Kullanıcı adı veya e-posta ara..." />
    <asp:Button ID="btnSearch" runat="server" Text="Ara" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
    <asp:Button ID="btnClear" runat="server" Text="Temizle" CssClass="btn btn-outline-secondary" OnClick="btnClear_Click" />
</div>


        <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False"
            CssClass="table table-bordered"
            HeaderStyle-CssClass="table-dark"
            OnRowCommand="gvUsers_RowCommand"
            OnRowDataBound="gvUsers_RowDataBound"
            DataKeyNames="Id">
            <Columns>
                <asp:BoundField DataField="UserName" HeaderText="Kullanıcı Adı" />
                <asp:BoundField DataField="Email" HeaderText="E-posta" />
                <asp:BoundField DataField="Role" HeaderText="Rol" />
                <asp:BoundField DataField="IsActiveText" HeaderText="Aktif Mi?" />

                <asp:ButtonField ButtonType="Button" CommandName="ToggleActive" Text="Aktif/Pasif"
                    HeaderText="Durum" ControlStyle-CssClass="btn btn-sm btn-warning" />

                <asp:ButtonField ButtonType="Button" CommandName="ToggleRole" Text="Rol Değiştir"
                    HeaderText="Rol" ControlStyle-CssClass="btn btn-sm btn-info" />

                <asp:ButtonField ButtonType="Button" CommandName="DeleteUser" Text="Sil"
                    HeaderText="Sil" ControlStyle-CssClass="btn btn-sm btn-danger" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
