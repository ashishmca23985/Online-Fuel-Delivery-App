    <%@ Page Language="C#" AutoEventWireup="true" CodeFile="LookUp.aspx.cs" Inherits="LookUp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
   
   <script language="javascript" type="text/javascript">
   function validateSave() {
       try {
           var name = $get('txtSearchText').value;
           if (name == "") {
               alert('Blank Value');
               return false;
           }
           return confirm('Create [' + name + ']');
       }
       catch (e) {
           alert("File - Lookup.aspx\r\nMethod -validateSave()\n" + e.description);
       }
   }

    function GridSingleClick(sender, args) {

        try {
            var RowIndex = args.get_itemIndexHierarchical();
        }
        catch (e) {
            alert("Error In File Lookup.aspx \r\n Method GetCaseID \r\n ErrorMessage : " + e.description);
            return;
        }
    }
    function GridDoubleClick(sender, args) {
        try {
            var RowIndex = args.get_itemIndexHierarchical();
            var SelectedObjectId = sender.get_masterTableView().get_dataItems()[RowIndex].getDataKeyValue("ID");
            CloseWindow(SelectedObjectId);
            return false;
        }
        catch (e) {
            alert("Error In File Lookup.aspx \r\n Method OpenRecord \r\n ErrorMessage : " + e.description);
            return;
        }
    }
    function GetRadWindow() {
        try {
            var oWindow = null;

            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;

            return oWindow;
        }
        catch (e) {
            alert("Error:- Page -> Lookup.aspx -> Method -> \"GetRadWindow\" \n Description:- \n" + e.description);
            return false;
        }
    }
    function CloseWindow(objectID) {
        try {
            if (objectID < 0)
                return;
           var oldObjectID = '<%=oldObjectID %>';

            var oWnd = GetRadWindow();
            if (oldObjectID != objectID) {
                var oArg = new Object();
                oArg.Id = objectID;
                oWnd.argument = oArg;
               
            }
            oWnd.close();
        }
        catch (e) {
            alert("Error:- Page -> Lookup.aspx -> Method -> 'CloseWindow' \n Description:- \n" + e.description);
            return false;
        }
    }    
    
</script>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="script1" runat ="server">
    </asp:ScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="rdgCommon">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="rdgCommon" LoadingPanelID="RadAjaxLoadingPanel1" />
            </UpdatedControls>
        </telerik:AjaxSetting>        
        <telerik:AjaxSetting AjaxControlID="btnSearch" EventName="Click">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="hdnCurrentViewId" />
                <telerik:AjaxUpdatedControl ControlID="lblMessage" />
                <telerik:AjaxUpdatedControl ControlID="rdgCommon" LoadingPanelID="RadAjaxLoadingPanel1" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server">
        <img id="imgProgress" alt="Loading..." src="images/LoadingProgressBar.gif"/>
    </telerik:RadAjaxLoadingPanel>
    
<table width="100%" id="tbl_Search">
    <tr class="trHeader">
        <th id="thCustomerHeader" runat="server" class="tdSubHeader" width="100%" colspan="3">
            Account
        </th>
    </tr>
    <tr>
        <td colspan="3">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td style="text-align: right">
            Search In
        </td>
        <td align="left">
            <telerik:RadComboBox ID="cmbSearch" 
                runat="server" Width="150px" Skin="Office2007">
                <CollapseAnimation Duration="200" Type="OutQuint" />
            </telerik:RadComboBox>
            <telerik:RadTextBox ID="txtSearchText" EmptyMessage="Enter search text here" runat="server"
                Skin="Office2007" InvalidStyleDuration="100" LabelCssClass="radLabelCss_WebBlue"
                Width="150px">
            </telerik:RadTextBox>
            <asp:ImageButton ID="btnSearch" OnClick="SearchData" runat="server" ImageUrl="~/Images/Search1.gif" />
        </td>
        <td align="right">
            <asp:Button ID="btnNew" class="Button" runat="server" OnClick="btnNew_Click" Text="New !" OnClientClick="javascript:if(validateSave() == false) return false;"/>
            <asp:Button ID="btnReset" class="Button" runat="server" Text="Close" OnClientClick="javascript:CloseWindow(0);"/>
        </td>
    </tr>
    <tr>
        <td colspan="3">

            <table width="99%">
    <tr>
        <td id="tdAccountGrid" colspan="3">
            <telerik:RadGrid ID="rdgCommon" runat="server" Skin="WebBlue" AllowMultiRowSelection="false" 
                BorderStyle="None" AllowPaging="True" Width="100%" PageSize="16"
                ItemStyle-Wrap="false" GridLines="None" OnNeedDataSource="rdgCommon_NeedDataSource" OnColumnCreated="rdgCommon_ColumnCreated"
                AllowSorting="True"
                ShowGroupPanel="false">
                <ClientSettings AllowDragToGroup="True" AllowColumnsReorder="True" 
                    ReorderColumnsOnClient="true">
                    <Resizing EnableRealTimeResize="True" ResizeGridOnColumnResize="false"
                        AllowColumnResize="True"></Resizing>
                    <Scrolling AllowScroll="false" UseStaticHeaders="True" />
                    <Selecting AllowRowSelect="True" />
                </ClientSettings>
                <HeaderContextMenu Skin="WebBlue" EnableTheming="True">
                    <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                </HeaderContextMenu>
                <GroupPanel EnableTheming="False">
                </GroupPanel>
                <ItemStyle Wrap="False"></ItemStyle>
                <MasterTableView CommandItemDisplay="None" TableLayout="Fixed" 
                    ItemStyle-Wrap="false" AutoGenerateColumns="true" GroupLoadMode ="Client"  Width="100%"
                    AllowPaging="true">
                    <ItemStyle Wrap="False"></ItemStyle>
                     <PagerStyle Wrap="false" Mode="NextPrevAndNumeric" PrevPageImageUrl="~/Images/arrowLeft.gif"
                        NextPageImageUrl="~/Images/arrowRight.gif" NextPageText="Next" PrevPageText="Prev"
                        AlwaysVisible="true" FirstPageImageUrl="~/Images/PagingFirst.gif" LastPageImageUrl="~/Images/PagingLast.gif"  />
                    <Columns>                                        
                    </Columns>
                </MasterTableView>
                <FilterMenu Skin="WebBlue" EnableTheming="True">
                    <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                </FilterMenu>
                <SortingSettings SortedBackColor="FloralWhite" />
            </telerik:RadGrid>        
        </td>
    </tr>             
    </table>
        </td>
    </tr>
</table>

    <input id="hdnCurrentViewId" runat="server" type="hidden" />
    </form>
</body>
</html>
