<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EFunnelReports.aspx.cs" Inherits="Reports" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    
     <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Cache-Control" content="no-cache, must-revalidate" />
    <meta http-equiv="Expires" content="-1" />
</head>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

    <script type="text/javascript" language="javascript">
        function ValidateSearch() {
            try {
                // check if branch is selected
               
                var control = $get('dtFrom');
                if (control.value == '') {
                    alert('Please select From date !');
                    control.focus();
                    return false;
                }
                control = $get('dtTo');
                if (control.value == '') {
                    alert('Please select To date !');
                    control.focus();
                    return false;
                }

               control = $find('cmbDiviceList');
                if (control.get_value() == null || control.get_text() == "") {
                    alert('Please select E-Funnel Id !');
                    control.get_inputDomElement().focus();
                    return false;
                }
            }
            catch (e) {
                alert("File - EFunnelReports.aspx\r\nMethod - ValidateSearch()\n" + e.description);
                return false;
            }
        }

    </script>

</telerik:RadCodeBlock>
<body>
    <form id="Form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rdgReports">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rdgReports" LoadingPanelID="RadAjaxLoadingPanel1" />
                    <telerik:AjaxUpdatedControl ControlID="RadToolTipManager1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
          
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server">
        <img id="imgProgress" alt="Loading..." src="Images/loading.gif" />
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadToolTipManager ID="RadToolTipManager1" HideEvent="ManualClose" Width="350"
        Height="200" runat="server" EnableShadow="true" OnAjaxUpdate="OnAjaxUpdate" RelativeTo="Element"
        Position="MiddleRight">
    </telerik:RadToolTipManager>
    <table width="100%" cellspacing="0">
        <tr class="trHeader">
            <th class="tdSubHeader">
                E-Funnel Reports
            </th>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <table id="Table1" runat="server" cellpadding="4" cellspacing="0">
                    <tr>
                        <td class="tdHeader">
                            From Date
                        </td>
                        <td>
                            <telerik:RadDateTimePicker ID="dtFrom" runat="server" Width="197px" DateInput-DateFormat="dd MMM yyyy hh:mm tt"  >
                            </telerik:RadDateTimePicker>
                        </td>
                        <td class="tdHeader">
                            To Date
                        </td>
                        <td>
                            <telerik:RadDateTimePicker ID="dtTo" runat="server" Width="197px" DateInput-DateFormat="dd MMM yyyy hh:mm tt">
                            </telerik:RadDateTimePicker>
                        </td>

                        <td class="tdHeader">
                            E-Fuunel DeviceId
                        </td>
                        <td>
                            <telerik:RadComboBox ID="cmbDiviceList" MaxHeight="150px" Skin="Office2007" runat="server" Filter="Contains"
                                                        Width="250px" AllowCustomText="true">
                                            <CollapseAnimation Duration="200" Type="OutQuint" />
                                        </telerik:RadComboBox>
                        </td>


                        <td>
                       <%--Parameters: --%><asp:TextBox ID="txtparameter" runat="server" Width="197px" Visible="false" ></asp:TextBox>
                        </td>
                        <td>
                            <asp:Button ID="btnSearch" runat="server" CssClass="Button" Width="65px" Text="Search"
                                OnClientClick="javascript:if(ValidateSearch() == false) return false;" OnClick="btnSearch_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table width="100%" cellspacing="0">
        <tr class="trHeader">
            <th class="tdSubHeader" width="50%">
                <asp:Label ID="lblHeader" runat="server"></asp:Label>
            </th>
            <th  width="50%" align="right">
                Export : <telerik:RadComboBox ID="cmbExport" runat="server" Width="145px" Skin="Office2007">
                    <CollapseAnimation Duration="200" Type="OutQuint" />
                </telerik:RadComboBox>
                &nbsp;
                <asp:ImageButton ID="imgExport" AlternateText="Click to Export" runat="server" OnClick="imgExport_Click"
                    ImageUrl="~/Images/export.gif" />&nbsp;&nbsp;
            </th>
        </tr>
    </table>
    <telerik:RadGrid ID="rdgReports" EnableTheming="true" AutoGenerateColumns="true" AllowMultiRowSelection="true" 
        runat="server" Width="100%" GridLines="Both" EditItemStyle-HorizontalAlign="Justify" ShowFooter="True" 
        HeaderStyle-Font-Bold="false" Skin="Hay" OnNeedDataSource="rdgReports_NeedDataSource"
        AllowSorting="true" AllowPaging="true" ShowGroupPanel="true" GroupingEnabled="true" 
        PageSize="20" Height="500px" OnExcelExportCellFormatting="rdgReports_ExcelExportCellFormatting">
        <HeaderContextMenu CssClass="">
        </HeaderContextMenu>
        <ClientSettings AllowDragToGroup="True">
            <Scrolling UseStaticHeaders="true" ScrollHeight="500px" />
            <Selecting AllowRowSelect="true" />
            <ClientEvents />
        </ClientSettings>

        <ExportSettings>
            <Csv ColumnDelimiter="Tab" RowDelimiter="NewLine" FileExtension="TXT" EncloseDataWithQuotes="true" />
            

        </ExportSettings>


        <GroupingSettings ShowUnGroupButton="true" CaseSensitive="false" />
        <GroupPanel EnableTheming="false">
        </GroupPanel>
        <MasterTableView CommandItemDisplay="None" TableLayout="Auto" AutoGenerateColumns="true">
            <PagerStyle Wrap="false" Mode="NextPrevAndNumeric" PrevPageImageUrl="~/Images/PagingLeft.gif"
                NextPageImageUrl="~/Images/PagingRight.gif" NextPageText="Next" PrevPageText="Prev"
                AlwaysVisible="false" FirstPageImageUrl="~/Images/PagingFirst.gif" LastPageImageUrl="~/Images/PagingLast.gif" />
        </MasterTableView>
    </telerik:RadGrid>
        <asp:HiddenField ID="hdPerportId" runat="server" Value="" />
    </form>
</body>
</html>
