<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Home/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Index</h2>
    <div><%=Html.ActionLink("Reset","Reset")%></div>
    <div><%=Html.ActionLink("Red", "Red")%></div>
    <div><%=Html.ActionLink("Black", "Black")%></div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
