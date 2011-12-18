<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="image.aspx.cs" Inherits="pq.image" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Label ID="lblTitle" runat="server" Text="" /></h2><br />
    <asp:Image ID="imgImage" runat="server" ImageUrl="" />
    <table>
        <tr>
            <td>id</td><td><asp:Label ID="lblImageId" runat="server" Text="" /></td>
        </tr>
        <tr>
            <td>owner</td><td><asp:Label ID="lblOwner" runat="server" Text="" /></td> 
        </tr>
        <tr>
            <td>mime</td><td><asp:Label ID="lblMime" runat="server" Text="" /></td>
        </tr>
        <tr>
            <td>size</td><td><asp:Label ID="lblSize" runat="server" Text="" /></td>
        </tr>
    </table>

</asp:Content>
