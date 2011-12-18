<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="pq._Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
           
            <h1><asp:Label ID="lblUsername" runat="server" Text="" /></h1><br />
            <fieldset>
                <br />
                <table>
                    <tr>
                        <td><asp:Label ID="lblTitle" runat="server" Text="Title (optional)" /></td><td><asp:TextBox ID="txtTitle" runat="server" Text="" CssClass="textEntry" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblUpload" runat="server" Text="Upload New Image" />&nbsp;&nbsp;</td><td><asp:FileUpload ID="imgFileUpload" runat="server" CssClass="textEntry" /></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="submit" runat="server" UseSubmitBehavior="true" Text="Upload" />
                        </td>
                    </tr>
                </table>
            </fieldset>
</asp:Content>

