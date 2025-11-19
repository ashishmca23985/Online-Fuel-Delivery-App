<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="loginform" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <fieldset>
        <legend>Login Credentials</legend>
        
        <table border="0" width='100%'>
            <tr>
                <td>
                    <span style="color:#FF8000; font-size:10px;">Login Id :</span>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtLoginId" runat="server" Width="98%" style="border:solid 1px #cccccc;" Text=""></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style='width: 98%;'>
                    <span style="color:#FF8000; font-size:10px;">Password :</span>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID='txtPassword' Style='width: 98%; height: 15px; border:solid 1px #cccccc;' MaxLength="12" runat="server"
                        TextMode="Password" />
                </td>
            </tr>
            <tr>
                <td>
                    <span style="color:#FF8000; font-size:10px;">Terminal Id :</span>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtTerminalId" Width="98%" runat="server" style="border:solid 1px #cccccc;" Text=""></asp:TextBox>
                </td>
            </tr>
           <%-- <tr>
                <td>
                    <span style="color:#FF8000; font-size:10px;">Base Terminal Id :</span>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtBaseTerminalId" Width="98%" runat="server" style="border:solid 1px #cccccc;" Text=""></asp:TextBox>
                </td>
            </tr>--%>
            <tr>
                <td align="left" colspan="2">
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Button class="cti_Button" Style="width: 99%;" ID="btnLogin" Text="Login" 
                        runat="server" OnClick="btnLogin_Click" />
                </td>
            </tr>
            <tr>
                <th colspan="2">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </th>
            </tr>
        </table>
    </fieldset>
    </form>
</body>
</html>