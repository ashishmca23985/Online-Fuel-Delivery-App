<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CaseSummary.aspx.cs" Inherits="General_CaseSummary" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function CaseStatus(Query,name,value) {
            try {
                if (name == '')
                    name = "Case View";
                window.parent.OpenItem('ShowList.aspx?ObjectType=CAS&Query=' + Query, name);
                return false;
            }
            catch (e) {
                alert("Error:- Page -> CaseSummary.aspx -> Method -> 'StatusRatng1' \n Description:- \n" + e.description);
                return false;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="RadScriptManager1" />
    <telerik:RadSkinManager ID="QsfSkinManager" runat="server" ShowChooser="true" Skin="Simple"
        Visible="false" />
    <telerik:RadFormDecorator ID="QsfFromDecorator" runat="server" DecoratedControls="All"
        EnableRoundedCorners="false" />

    <script type="text/javascript">
        function onRequestStart(sender, args) {
            if (args.get_eventTarget().indexOf("ExportTo") >= 0) {
                args.set_enableAjax(false);
            }
        }
    </script>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <ClientEvents OnRequestStart="onRequestStart"></ClientEvents>
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rdgRating">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rdgRating"></telerik:AjaxUpdatedControl>
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
     <telerik:RadAjaxLoadingPanel ID="RadCustomerPanel" runat="server" Height="75px" Width="75px"
        Transparency="50">
        <img alt="Loading..." src='../Images/LoadingProgressBar.gif' style="border: 0;" />
    </telerik:RadAjaxLoadingPanel>
    <asp:UpdatePanel ID="upPanel" runat="server" UpdateMode="Conditional" RenderMode="Inline">
        <ContentTemplate>
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
            <table width="100%" id="tblQuickCall" class="ascxMainTable2" cellpadding="1" cellspacing="0">
                <tr class="trHeader">
                    <th class="tdSubHeader">
                       Open Case Summary Report
                    </th>
                    <th style="text-align: right; width: 50%">
                    </th>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td>
                        Location
                    </td>
                    <td>
                        <telerik:RadComboBox ID="cmbLocation" AllowCustomText="false" AutoPostBack="true" Filter="StartsWith" runat="server" OnSelectedIndexChanged="cmbLocation_SelectedIndexChanged"
                            Width="203px" MaxHeight="350px">
                            <CollapseAnimation Duration="200" Type="OutQuint" />
                        </telerik:RadComboBox>
                    </td>
                    <td>
                    <asp:RadioButtonList ID="rbnttype" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Text="Owner Wise" Selected="True" Value="O"></asp:ListItem>
                    <asp:ListItem Text="Team Wise" Value="T"></asp:ListItem>
                    <asp:ListItem Text="Account Wise" Value="A"></asp:ListItem>
                    </asp:RadioButtonList>
                    </td>
                    <td>
                        <asp:Button ID="btnSearch" runat="server" Text="Search" 
                            OnClick="btnSearch_Click" />
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="10">
                        <telerik:RadGrid ID="rdgRating" runat="server" AutoGenerateColumns="False" CellSpacing="0"
                            ShowFooter="false" AllowPaging="True" ShowHeader="true" GridLines="Both" OnPageIndexChanged="rdgRating_PageIndexChanged "
                            PageSize="20" AllowSorting="false" OnPageSizeChanged="rdgRating_PageSizeChanged"
                            MasterTableView-ExpandCollapseColumn-Display="true" Width="100%" HeaderStyle-Wrap="false"
                            EditItemStyle-HorizontalAlign="center" AlternatingItemStyle-BackColor="AntiqueWhite"
                            AllowMultiRowSelection="true" OnItemDataBound="rdgcaserating_ItemDataBound" OnItemCommand="rdgRating_ItemCommand">
                            <ClientSettings>
                                <Selecting AllowRowSelect="false" />
                            </ClientSettings>
                            <MasterTableView Width="100%" DataKeyNames="owner_id" CommandItemDisplay="None">
                                <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" ShowExportToExcelButton="true">
                                </CommandItemSettings>
                                <Columns>
                                  <telerik:GridBoundColumn HeaderText="Location" AllowSorting="true" DataField="AccountName"
                                        ItemStyle-HorizontalAlign="Left">
                                        <HeaderStyle HorizontalAlign="left" Font-Bold="True" Width="10%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderText="Team Name" AllowSorting="true" DataField="TeamName"
                                        ItemStyle-HorizontalAlign="Left">
                                        <HeaderStyle HorizontalAlign="left" Font-Bold="True" Width="10%" />
                                    </telerik:GridBoundColumn>
                                
                                    <telerik:GridBoundColumn HeaderText="Owner Name" AllowSorting="true" DataField="contactName"
                                        ItemStyle-HorizontalAlign="Left">
                                        <HeaderStyle HorizontalAlign="left" Font-Bold="True" Width="10%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderText="Total Case" AllowSorting="true" DataField="Total"
                                        ItemStyle-HorizontalAlign="Left">
                                        <HeaderStyle HorizontalAlign="left" Font-Bold="True" Width="5%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderText="Open" AllowSorting="true" DataField="openCase"
                                        ItemStyle-HorizontalAlign="Left">
                                        <HeaderStyle HorizontalAlign="left" Font-Bold="True" Width="05%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderText="Open Working" AllowSorting="true" DataField="WorkingCase"
                                        ItemStyle-HorizontalAlign="Left">
                                        <HeaderStyle HorizontalAlign="left" Font-Bold="True" Width="05%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderText="Pending for Feedback" AllowSorting="true" DataField="FeedbackCase"
                                        ItemStyle-HorizontalAlign="Left">
                                        <HeaderStyle HorizontalAlign="left" Font-Bold="True" Width="05%" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderText="owner_id" DataField="owner_id" Visible="false">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderText="location_id" DataField="location_id" Visible="false">
                                    </telerik:GridBoundColumn>
                                </Columns>
                                <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column">
                                    <HeaderStyle Width="20px"></HeaderStyle>
                                </RowIndicatorColumn>
                                <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column">
                                    <HeaderStyle Width="20px"></HeaderStyle>
                                </ExpandCollapseColumn>
                            </MasterTableView>
                            <FilterMenu EnableImageSprites="False">
                            </FilterMenu>
                        </telerik:RadGrid>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <table>
        <tr>
            <td>
                <asp:Button ID="btnExport" runat="server" OnClick="btnExport_Click" Text="Export"
                    Width="65px" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
