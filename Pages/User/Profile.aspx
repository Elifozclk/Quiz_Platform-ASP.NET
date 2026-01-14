<%@ Page Title="Profil" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="QP_WEBPROJECT.vs2.Pages.User.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .profile-card {
            background-color: #F6EEF9;
            border-radius: 16px;
            padding: 30px;
            max-width: 600px;
            margin: auto;
            text-align: center;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.08);
        }

        .profile-pic {
            width: 140px;
            height: 140px;
            object-fit: cover;
            border-radius: 50%;
            border: 3px solid #5B0E2D;
            margin-bottom: 15px;
        }

        .profile-info {
            text-align: left;
            margin-top: 20px;
        }

        .profile-info strong {
            color: #5B0E2D;
        }

        .text-danger {
            color: red;
        }
    </style>

    <div class="profile-card">
        <asp:Image ID="imgProfile" runat="server" CssClass="profile-pic" />
        <h3><asp:Label ID="lblDisplayName" runat="server" /></h3>
        <p><asp:Label ID="lblEmail" runat="server" /></p>

        <div class="profile-info">
            <p><strong>Ad Soyad:</strong> <asp:Label ID="lblName" runat="server" /></p>
            <p><strong>Cinsiyet:</strong> <asp:Label ID="lblGender" runat="server" /></p>
            <p><strong>Doğum Tarihi:</strong> <asp:Label ID="lblBirthday" runat="server" /></p>
            <p><strong>Konum:</strong> <asp:Label ID="lblLocation" runat="server" /></p>
            <p><strong>Hakkımda:</strong> <asp:Label ID="lblBio" runat="server" /></p>
            <p><strong>İlgi Alanları:</strong> <asp:Label ID="lblInterests" runat="server" /></p>
        </div>

        <asp:Label ID="lblError" runat="server" CssClass="text-danger" />
    </div>
</asp:Content>
