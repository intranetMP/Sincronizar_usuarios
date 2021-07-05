<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication2._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div style="width: 40%">
        <asp:Label runat="server">Usuario:</asp:Label><br />
        <asp:TextBox runat="server" ID="txtUsuario"></asp:TextBox><br />
        <asp:Label runat="server">Contraseña:</asp:Label><br />
        <asp:TextBox runat="server" ID="txtPass"></asp:TextBox><br />
        <asp:Button runat="server" Text="Cambiar contraseña" OnClick="cambiarContrasena" />
    </div>
</asp:Content>
