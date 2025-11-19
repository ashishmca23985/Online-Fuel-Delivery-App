<%@ Page Language="C#" AutoEventWireup="true" CodeFile="About.aspx.cs" Inherits="About" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form runat="server" id="form1">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <table class="aspxMainTable">
        <tr>
            <th class="tdMainHeader1">
                <img alt="GalaxyLogo" src="Images\favicon.ico" />
                About - Galaxy CRM [LoggedIn User]
            </th>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <fieldset class="FieldStyle">
                    <legend class="legendClass">LoggedIn Info<br /><br /></legend>
                    <table width="99%" cellpadding="4" cellspacing="1">
                        <tr class="row1">
                            <td style="width: 20%">
                                User Name
                            </td>
                            <td class="tdAboutItem">
                                <asp:Label ID="lblUserName" runat="server"></asp:Label><br />
                            </td>
                        </tr>
                        <tr class="row1">
                            <td>
                                Contact Name
                            </td>
                            <td class="tdAboutItem">
                                <asp:Label ID="lblEmployeeName" runat="server"></asp:Label><br />
                            </td>
                        </tr>
                        
                        <tr class="row1">
                            <td>
                                Role Name
                            </td>
                            <td class="tdAboutItem">
                                <asp:Label ID="lblRoleName" runat="server"></asp:Label><br />
                            </td>
                        </tr>
                        
                      
                      
                        <tr class="row1">
                            <td>
                                User IP
                            </td>
                            <td class="tdAboutItem">
                                <asp:Label ID="lblIP" runat="server"></asp:Label><br />
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td>
                <br />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
