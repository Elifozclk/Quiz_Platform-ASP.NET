<%@ Page Title="Üye Ol" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="QP_WEBPROJECT.vs2.Pages.User.Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .register-box {
            background-color: #F6EEF9;
            padding: 30px;
            border-radius: 12px;
            max-width: 600px;
            margin: auto;
            box-shadow: 0 0 15px rgba(91, 14, 45, 0.1);
        }
        .register-box h3 {
            text-align: center;
            color: #5B0E2D;
            margin-bottom: 25px;
        }
    </style>

    <div class="register-box">
        <h3>Üye Ol</h3>

        <asp:Label ID="lblError" runat="server" CssClass="text-danger d-block text-center"></asp:Label>
        <asp:Label ID="lblSuccess" runat="server" CssClass="text-success d-block text-center"></asp:Label>

        <asp:TextBox ID="txtUserName" runat="server" CssClass="form-control mb-3" Placeholder="Kullanıcı Adı" />
        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control mb-3" Placeholder="E-posta" TextMode="Email" />
        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control mb-3" Placeholder="Şifre" TextMode="Password" />
        <asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control mb-3" Placeholder="Ad Soyad" />
        
        <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control mb-3">
            <asp:ListItem Text="Cinsiyet Seçin" Value="" />
            <asp:ListItem Text="Erkek" Value="male" />
            <asp:ListItem Text="Kadın" Value="female" />
            <asp:ListItem Text="Belirtmek istemiyorum" Value="prefer_not_to_say" />
        </asp:DropDownList>

        <asp:TextBox ID="txtBirthday" runat="server" CssClass="form-control mb-3" TextMode="Date" />
        <asp:TextBox ID="txtLocation" runat="server" CssClass="form-control mb-3" Placeholder="Yaşanılan Yer" />
        <asp:TextBox ID="txtBio" runat="server" CssClass="form-control mb-3" Placeholder="Biyografi" />
        <asp:TextBox ID="txtInterests" runat="server" CssClass="form-control mb-3" Placeholder="Hobiler" />

        <label>Profil Fotoğrafı (isteğe bağlı)</label>
        <div class="form-group">
    <label for="<%= fuProfileImage.ClientID %>">
        <i class="fas fa-image"></i> Profil Resmi
    </label>
    
    <asp:FileUpload ID="fuProfileImage" runat="server" 
                    CssClass="form-control" 
                    accept="image/*" />
    
    <!-- Önizleme -->
    <div class="mt-3 text-center">
        <img id="imgProfilePreview" 
             src="~/Uploads/ProfileImages/default-user.png" 
             alt="Profil Önizleme" 
             style="max-width: 200px; max-height: 200px; border-radius: 50%; border: 3px solid #ddd;" />
    </div>
    
    <!-- Validasyon mesajı -->
    <div id="profileImageValidation" class="mt-2" style="display:none;"></div>
    
    <!-- Kurallar -->
    <small class="form-text text-muted">
        <i class="fas fa-info-circle"></i> <strong>Profil Resmi Kuralları:</strong><br/>
        • Maksimum dosya boyutu: <strong>5 MB</strong><br/>
        • Desteklenen formatlar: <strong>JPG, JPEG, PNG</strong><br/>
        • Minimum boyut: <strong>200x200px</strong><br/>
        • Maksimum boyut: <strong>2000x2000px</strong><br/>
        • Tercihen <strong>kare (1:1)</strong> görsel
    </small>
</div>

        <asp:Button ID="btnRegister" runat="server" Text="Üye Ol" CssClass="btn btn-save w-100" OnClick="btnRegister_Click" />
    </div>

    <script src="<%= ResolveUrl("~/Scripts/ProfileImageValidation.js") %>"></script>
    
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            const fileInput = document.getElementById('<%= fuProfileImage.ClientID %>');
            const imgPreview = document.getElementById('imgProfilePreview');
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

                        // Submit butonunu kontrol et
                        const submitBtn = document.getElementById('<%= btnRegister.ClientID %>');
                        if (submitBtn) {
                            submitBtn.disabled = !isValid;
                        }
                    });
                });
            }
        });
    </script>
</asp:Content>
