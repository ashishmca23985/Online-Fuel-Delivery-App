<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AlertPopup.aspx.cs" Inherits="Alerts_AlertPopup" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Alert Popup</title>
</head>

<body>
    <form id="frmAlertPopup" runat="server">
    <asp:ScriptManager ID="smMain" runat="server">
        <Services>
        </Services>
    </asp:ScriptManager>    
        <table width="100%" style="margin:0px;">
            <tr>
                <td align="left">
                    <asp:Button ID="btnDismiss" runat="server"
                        Text="Dismiss" />
                    <asp:Button ID="btnDismissAll" runat="server"
                        Text="Dismiss All" />
                </td>
                <td align="right">
                    <telerik:RadComboBox Width="50px" ID="cmbSnoozeTime" runat="server">
                        <Items>
                            <telerik:RadComboBoxItem Text="00" Value="00" />
                            <telerik:RadComboBoxItem Text="05" Value="05" />
                            <telerik:RadComboBoxItem Text="10" Value="10" />
                            <telerik:RadComboBoxItem Text="15" Value="15" />
                            <telerik:RadComboBoxItem Text="20" Value="20" />
                        </Items>
                    </telerik:RadComboBox>&nbsp;
                    <asp:Button ID="btnSnooze" runat="server"
                        Text="Snooze" />
                    <asp:Button ID="btnSnoozeAll" runat="server"
                        Text="Snooze All" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    <telerik:RadGrid ID="rdgAlerts" runat="server" Skin="WebBlue" AllowMultiRowSelection="true"  
                BorderStyle="None" AllowPaging="True" Width="100%" 
                ShowGroupPanel="True">
                <ClientSettings>
                    <Scrolling AllowScroll="true" UseStaticHeaders="True" />
                    <Selecting AllowRowSelect="True" />
                </ClientSettings>                
                <HeaderContextMenu Skin="WebBlue" EnableTheming="True">
                    <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                </HeaderContextMenu>
                <ItemStyle Wrap="False"></ItemStyle>                
                <MasterTableView CommandItemDisplay="None" TableLayout="Auto" 
                    ItemStyle-Wrap="false" DataKeyNames="id" AutoGenerateColumns="true" GroupLoadMode="Client" Width="100%"
                    AllowPaging="true">                   
                    <ItemStyle Wrap="False"></ItemStyle>
                     <PagerStyle Wrap="false" Mode="NextPrevAndNumeric" PrevPageImageUrl="~/Images/arrowLeft.gif"
                        NextPageImageUrl="~/Images/arrowRight.gif" NextPageText="Next" PrevPageText="Prev"
                        AlwaysVisible="true" FirstPageImageUrl="~/Images/PagingFirst.gif" LastPageImageUrl="~/Images/PagingLast.gif"  />
                    <Columns>   
                        <telerik:GridClientSelectColumn UniqueName="SelectColumn">
                        <HeaderStyle Width="30px" />
                        </telerik:GridClientSelectColumn>
                                     
                    </Columns>
                </MasterTableView>                
            </telerik:RadGrid>
                    <asp:Table  ID="tblAlertDetails" CellSpacing="1" CellPadding="1" Width="100%" runat="server">
                    </asp:Table>
                </td>
            </tr>            
        </table>
    </form>
</body>
</html>
