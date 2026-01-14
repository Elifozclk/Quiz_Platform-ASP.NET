<%@ Page Title="Admin Profil" Language="C#" MasterPageFile="~/MasterPages/Admin.Master"
    AutoEventWireup="true" CodeBehind="AdminProfile.aspx.cs" Inherits="QP_WEBPROJECT.vs2.Pages.Admin.AdminProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-4">
        <h2 class="text-center mb-4">👤 Admin Profil Bilgileri</h2>
        <asp:Label ID="lblInfo" runat="server" CssClass="text-success text-center d-block mb-3"></asp:Label>
        <div class="card p-4 shadow">
            <p><strong>Kullanıcı Adı:</strong> <asp:Label ID="lblUserName" runat="server" /></p>
            <p><strong>Email:</strong> <asp:Label ID="lblEmail" runat="server" /></p>
            <p><strong>Rol:</strong> <asp:Label ID="lblRole" runat="server" /></p>
        </div>
    </div>
</asp:Content>
