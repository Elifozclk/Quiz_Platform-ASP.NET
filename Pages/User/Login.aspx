<%@ Page Title="Giriş Yap" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="QP_WEBPROJECT.vs2.Pages.User.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    .auth-container {
        max-width: 450px;
        margin: 50px auto;
        background-color: #F6EEF9;
        border-radius: 15px;
        box-shadow: 0 4px 12px rgba(91, 14, 45, 0.15);
        padding: 35px;
        color: #5B0E2D;
    }
    .auth-title {
        text-align: center;
        color: #5B0E2D;
        font-weight: 700;
        margin-bottom: 25px;
    }
    .form-control {
        border: 2px solid #5B0E2D;
        color: #5B0E2D;
    }
    .btn-login {
        width: 100%;
        background-color: #5B0E2D;
        border: none;
        color: white;
        font-weight: 600;
        transition: all 0.3s;
    }
    .btn-login:hover {
        background-color: #8B2248;
        transform: scale(1.02);
    }
    a {
        color: #5B0E2D;
        font-weight: 500;
        text-decoration: underline;
    }
</style>

<div class="auth-container">
    <h3 class="auth-title">Giriş Yap</h3>

    <asp:Label ID="lblError" runat="server" CssClass="text-danger d-block text-center mb-3"></asp:Label>

    <div class="mb-3">
        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="E-posta"></asp:TextBox>
    </div>

    <div class="mb-3">
        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" placeholder="Şifre" TextMode="Password"></asp:TextBox>
    </div>

   
    <asp:Button ID="btnLogin" runat="server" Text="Giriş Yap" CssClass="btn-login" OnClick="btnLogin_Click" />


    <div class="text-center">
        <span>Hesabın yok mu?</span>
        <a href="/Pages/User/Register.aspx">Kayıt Ol</a>
    </div>
</div>

</asp:Content>
