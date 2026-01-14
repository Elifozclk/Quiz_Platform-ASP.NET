<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReactivateAccount.aspx.cs" Inherits="QP_WEBPROJECT.vs2.Pages.User.ReactivateAccount" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Hesap Yeniden Aktifleştirme</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .reactivate-card {
            background: white;
            padding: 40px;
            border-radius: 15px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.2);
            max-width: 500px;
            text-align: center;
        }
        .spinner-border {
            width: 3rem;
            height: 3rem;
        }
        .success-icon {
            font-size: 60px;
            color: #28a745;
        }
        .error-icon {
            font-size: 60px;
            color: #dc3545;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="reactivate-card">
            <asp:Panel ID="pnlLoading" runat="server" Visible="true">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Yükleniyor...</span>
                </div>
                <h4 class="mt-3">Hesabınız yeniden aktif ediliyor...</h4>
            </asp:Panel>

            <asp:Panel ID="pnlSuccess" runat="server" Visible="false">
                <div class="success-icon">✓</div>
                <h3 class="text-success mt-3">Başarılı!</h3>
                <p class="mt-3">
                    <asp:Label ID="lblSuccessMessage" runat="server" CssClass="text-muted"></asp:Label>
                </p>
                <asp:Button ID="btnGoToDashboard" runat="server" Text="Panelime Git" 
                    CssClass="btn btn-primary mt-3" OnClick="btnGoToDashboard_Click" />
            </asp:Panel>

            <asp:Panel ID="pnlError" runat="server" Visible="false">
                <div class="error-icon">✗</div>
                <h3 class="text-danger mt-3">Hata!</h3>
                <p class="mt-3">
                    <asp:Label ID="lblErrorMessage" runat="server" CssClass="text-muted"></asp:Label>
                </p>
                <asp:Button ID="btnGoToLogin" runat="server" Text="Giriş Sayfasına Dön" 
                    CssClass="btn btn-secondary mt-3" OnClick="btnGoToLogin_Click" />
            </asp:Panel>
        </div>
    </form>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
