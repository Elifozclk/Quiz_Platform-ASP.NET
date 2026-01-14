<%@ Page Title="Çözdüğüm Quizler" Language="C#" MasterPageFile="~/MasterPages/Site.Master"
    AutoEventWireup="true" CodeBehind="UserQuizHistory.aspx.cs" Inherits="QP_WEBPROJECT.vs2.Pages.User.UserQuizHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .quiz-card {
            background-color: #F6EEF9;
            border-radius: 12px;
            padding: 20px;
            margin-bottom: 15px;
            box-shadow: 0 2px 6px rgba(91, 14, 45, 0.1);
        }

        .quiz-title {
            font-size: 1.3rem;
            font-weight: bold;
            color: #5B0E2D;
        }

        .quiz-meta {
            font-size: 0.95rem;
            color: #444;
        }
    </style>

    <h2 class="text-center mb-4" style="color:#5B0E2D;">Çözdüğüm Quizler</h2>

    <asp:Repeater ID="rptQuizzes" runat="server">
    <ItemTemplate>
    <div class="quiz-card">
        <div class="quiz-title"><%# ((Dictionary<string, object>)Container.DataItem)["QuizTitle"] %></div>
        <div class="quiz-meta">
            <strong>Sonuç:</strong> <%# ((Dictionary<string, object>)Container.DataItem)["ResultTitle"] %><br />
            <strong>Tarih:</strong> <%# ((Dictionary<string, object>)Container.DataItem)["SubmittedAt"] %>
        </div>
    </div>
</ItemTemplate>

</asp:Repeater>

</asp:Content>
