<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="pq.login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="smScriptManager" runat="server" ScriptMode="Auto" ></asp:ScriptManager>
    <asp:UpdatePanel runat="server" ID="up1" UpdateMode="Always">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnLogin" EventName="Click" />
        </Triggers>
        <ContentTemplate>
            <fieldset>
                <asp:Label ID="lblLoginError" runat="server" Text="" />
                <table>
                    <tr>
                        <td><asp:Label ID="lblUsername" runat="server" Text="Username: " /></td>
                        <td><asp:TextBox ID="txtUsername" runat="server"  CssClass="textEntry" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblPassword" runat="server" Text="Password: " /></td>
                        <td><asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="passwordEntry" /></td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="submitButton" />
                        </td>
                    </tr>
                </table>
            </fieldset>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>
