<%@ Page Title="Ayarlar" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="QP_WEBPROJECT.vs2.Pages.User.Settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
    .settings-panel {
        background-color: #F6EEF9;
        padding: 30px;
        border-radius: 12px;
        box-shadow: 0 0 10px rgba(91, 14, 45, 0.1);
        max-width: 700px;
        margin: auto;
    }

    .settings-title {
        color: #5B0E2D;
        font-weight: 700;
        font-size: 1.5rem;
        margin-bottom: 20px;
        text-align: center;
    }

    .btn-save {
        background-color: #5B0E2D;
        border: none;
        color: white;
        font-weight: 600;
        width: 100%;
        border-radius: 10px;
        padding: 10px 14px;
    }

    .btn-save:hover {
        background-color: #8B2248;
    }

    /* ---- Profil foto kartı ---- */
    .profile-card{
        background:#F6EEF9;
        border:1px solid rgba(91,14,45,.08);
        border-radius:14px;
        padding:18px;
        box-shadow:0 6px 18px rgba(91,14,45,.08);
    }

    .profile-header{
        text-align:center;
        margin-bottom:14px;
    }

    .profile-title{
        margin:0;
        color:#5B0E2D;
        font-weight:800;
    }

    .profile-sub{
        color:rgba(91,14,45,.7);
    }

    .profile-body{
        display:flex;
        flex-direction:column;
        gap:14px;
    }

    .current-photo{
        display:flex;
        justify-content:center;
    }

    .avatar-lg{
        width:120px;
        height:120px;
        border-radius:50%;
        border:2px solid rgba(91,14,45,.15);
        background:#fff;
        object-fit:cover;
    }

    .photo-actions{
        display:flex;
        gap:14px;
        align-items:stretch;
    }

    .upload-box, .default-box{
        flex:1;
        background:#fff;
        border:1px solid rgba(91,14,45,.10);
        border-radius:12px;
        padding:12px;
    }

    .or-sep{
        display:flex;
        align-items:center;
        justify-content:center;
        color:rgba(91,14,45,.55);
        font-weight:700;
        padding:0 4px;
        min-width: 44px;
    }

    .default-preview{
        display:flex;
        align-items:center;
        gap:10px;
    }

    .avatar-sm{
        width:44px;
        height:44px;
        border-radius:50%;
        border:1px solid rgba(91,14,45,.15);
        object-fit:cover;
        background:#fff;
    }

    .default-title{
        color:#5B0E2D;
        font-weight:800;
        line-height:1.1;
    }

    .default-desc{
        color:rgba(91,14,45,.65);
        font-size:.85rem;
    }

    .btn-primary-soft{
        background:#5B0E2D;
        border:none;
        color:#fff;
        font-weight:700;
        border-radius:10px;
        padding:10px 14px;
    }
    .btn-primary-soft:hover{
        background:#8B2248;
    }

    .btn-outline-soft{
        background:#FFF;
        border:1px solid rgba(91,14,45,.25);
        color:#5B0E2D;
        font-weight:700;
        border-radius:10px;
        padding:10px 14px;
    }
    .btn-outline-soft:hover{
        background:#FFF4B0;
        border-color:rgba(91,14,45,.35);
    }

    @media (max-width: 768px){
        .photo-actions{ flex-direction:column; }
        .or-sep{ display:none; }
    }
</style>


    <div class="settings-panel">
        <h3 class="settings-title">Hesap Bilgileri</h3>

        <asp:Label ID="lblStatus" runat="server" CssClass="text-success d-block text-center mb-3"></asp:Label>
<!-- 📸 Profil Fotoğrafı Kartı -->
<div class="form-group">
    <label><i class="fas fa-user-circle"></i> Profil Resmi</label>
    
    <!-- Mevcut resim -->
    <div class="text-center mb-3">
        <asp:Image ID="imgProfile" runat="server" 
                   CssClass="img-thumbnail" 
                   style="max-width: 200px; max-height: 200px; border-radius: 50%;" />
    </div>
    
    <!-- Yeni resim seç -->
    <asp:FileUpload ID="fuProfileImage" runat="server" 
                    CssClass="form-control mb-2" 
                    accept="image/*" />
    
    <!-- Validasyon mesajı -->
    <div id="profileImageValidation" class="mb-2" style="display:none;"></div>
    
    <!-- Butonlar -->
    <div class="btn-group w-100">
        <asp:Button ID="btnUploadImage" runat="server" 
                    Text="Resmi Yükle" 
                    CssClass="btn btn-primary" 
                    OnClick="btnUploadImage_Click" />
        
        <asp:Button ID="btnUseDefaultProfileImage" runat="server" 
                    Text="Varsayılan Resmi Kullan" 
                    CssClass="btn btn-secondary" 
                    OnClick="btnUseDefaultProfileImage_Click" />
    </div>
    
    <!-- Kurallar -->
    <small class="form-text text-muted mt-2">
        <strong>Kurallar:</strong> Max 5 MB, JPG/PNG, 200x200px - 2000x2000px, Tercihen kare
    </small>
</div>
        <div class="row">
            <div class="col-md-6 mb-3">
                <label>Ad Soyad</label>
                <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6 mb-3">
                <label>E-posta</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
            </div>
        </div>

        <div class="row">
            <div class="col-md-6 mb-3">
                <label>Takma Ad</label>
                <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6 mb-3">
                <label>Cinsiyet</label>
                <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Seçiniz" Value="" />
                    <asp:ListItem Text="Erkek" Value="male" />
                    <asp:ListItem Text="Kadın" Value="female" />
                    <asp:ListItem Text="Nonbinary" Value="nonbinary" />
                    <asp:ListItem Text="Belirtmek istemiyorum" Value="prefer_not_to_say" />
                </asp:DropDownList>
            </div>
        </div>

        <div class="row">
            <div class="col-md-6 mb-3">
                <label>Doğum Tarihi</label>
                <asp:TextBox ID="txtBirthday" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-6 mb-3">
                <label>Yer</label>
                <asp:TextBox ID="txtLocation" runat="server" CssClass="form-control" />
            </div>
        </div>

        <div class="mb-3">
            <label>Biyografi</label>
            <asp:TextBox ID="txtBio" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
        </div>
        <div class="mb-3">
            <label>Hobiler</label>
            <asp:TextBox ID="txtInterests" runat="server" CssClass="form-control" />
        </div>

        <asp:Button ID="btnUpdateInfo" runat="server" Text="Bilgileri Güncelle" CssClass="btn btn-save mb-4" OnClick="btnUpdateInfo_Click" />

        <hr />
        <h3 class="settings-title">Şifre Değiştir</h3>
        <asp:Label ID="lblPasswordStatus" runat="server" CssClass="text-danger d-block text-center mb-3"></asp:Label>

        <div class="mb-3">
            <label>Mevcut Şifre</label>
            <asp:TextBox ID="txtOldPassword" runat="server" CssClass="form-control" TextMode="Password" />
        </div>
        <div class="mb-3">
            <label>Yeni Şifre</label>
            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" />
        </div>
        <div class="mb-3">
            <label>Yeni Şifre (Tekrar)</label>
            <asp:TextBox ID="txtNewPasswordAgain" runat="server" CssClass="form-control" TextMode="Password" />
        </div>

        <asp:Button ID="btnChangePassword" runat="server" Text="Şifreyi Güncelle" CssClass="btn btn-save" OnClick="btnChangePassword_Click" />
    </div>
        <hr />
<h3 class="settings-title">Hesap İşlemleri</h3>

<asp:Label ID="lblAccountStatus" runat="server" CssClass="text-danger d-block text-center mb-3"></asp:Label>

<div class="mb-3 text-center">
    <asp:Button ID="btnDeactivate" runat="server" Text="Hesabımı Devre Dışı Bırak" CssClass="btn btn-warning me-3" OnClick="btnDeactivate_Click" />
    <asp:Button ID="btnDeleteRequest" runat="server" Text="Hesabımı Silmek İstiyorum" CssClass="btn btn-danger" OnClick="btnDeleteRequest_Click" />
</div>

    <script src="<%= ResolveUrl("~/Scripts/ProfileImageValidation.js") %>"></script>
    
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            const fileInput = document.getElementById('<%= fuProfileImage.ClientID %>');
            const imgPreview = document.getElementById('<%= imgProfile.ClientID %>');
            const uploadBtn = document.getElementById('<%= btnUploadImage.ClientID %>');
            const validationMessage = document.getElementById('profileImageValidation');

            if (fileInput) {
                fileInput.addEventListener('change', function() {
                    // Önizleme göster
                    if (imgPreview) {
                        previewImage(fileInput, imgPreview);
                    }

                    // Validasyon yap
                    validateProfileImage(fileInput, function(isValid, message) {
                        if (validationMessage) {
                            validationMessage.innerHTML = message;
                            validationMessage.className = isValid ? 'alert alert-success' : 'alert alert-danger';
                            validationMessage.style.display = 'block';
                        }

                        // Upload butonunu kontrol et
                        if (uploadBtn) {
                            uploadBtn.disabled = !isValid;
                        }
                    });
                });
            }
        });
    </script>

</asp:Content>
