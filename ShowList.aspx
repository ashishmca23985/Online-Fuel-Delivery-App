<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ShowList.aspx.cs" EnableEventValidation="false" ValidateRequest="false" Inherits="ShowList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="Scripts/jquery-1.4.2.min.js"></script>
</head>
<telerik:RadCodeBlock ID="codeShowList" runat="server">
    <script type="text/javascript">
        var objectName = "";
        var ObjectURL = "";
        function OpenWindow(url) {
            try {
                var v = radopen(url);
                v.set_modal(true);

                v.setSize(1050, 500); //height, width
                v.moveTo(10, 10);
                v.add_close(onWindowClose);

                return false;
            }
            catch (e) {
                alert("Error:- Page -> ShowList.aspx -> Method -> 'OpenWindow' \n Description:- \n" + e.description);
                return false;
            }
        }

        function onWindowClose() {
            try {

                $get('<%=hdnSelectedID.ClientID %>').value = "0|";
                $get('<%=lnkRefresh.ClientID %>').click();
                return false;
            }
            catch (e) {
                alert("Error :- Page:- \"ShowList.aspx\" Method:- 'onWindowClose' Description:- " + e.description);
                return false;
            }
        }

        var viewComboControlID = '';
        function pageLoad(sender, args) {
            try {
                if (args._isPartialLoad == false) {

                }
            }
            catch (e) {
                alert("Error In File ShowList.aspx \r\n Method PageLoad \r\n ErrorMessage : " + e.description);
                return;
            }
        }

        function AddItem(param) {
           
            try {
                var strTabName = 'New ' + '<%=objectTypeNameadd%>';
                if (window.parent.document.getElementById("radSplitter") == null) {

                    window.parent.parent.OpenItem(param, strTabName);
                }
                else {

                    window.parent.OpenItem(param, strTabName);
                }
                
                return false;
            }
            catch (e) {
                alert("Error In File CommonGrid.ascx \r\n Method AddItem \r\n ErrorMessage : " + e.description);
                return false;
            }
        }
        
        function EditItem() {
            try {
                
                var strTabName = '<%=objectTypeNameadd%>' + '[' + objectName + ']';
                if (ObjectURL == "") {
                    alert("Please select a row to edit !");
                    return false;
                }
                var cli = document.getElementById('hdnurl').value;
                if (cli != '' && cli != '0|')
                    ObjectURL = ObjectURL + cli;
                if (window.parent.document.getElementById("radSplitter") == null) {
                   
                    window.parent.parent.OpenItem(ObjectURL, strTabName);                    
                }
                else {
                   
                    window.parent.OpenItem(ObjectURL, strTabName);
                }
                
                return false;
            }
            catch (e) {
                alert("Error In File CommonGrid.ascx \r\n Method EditItem \r\n ErrorMessage : " + e.description);
                return false;
            }
        }
        function GetID(sender, args) {

            try {
                var RowIndex = args.get_itemIndexHierarchical();
                //--get row values from selected row
                var objectID = sender.get_masterTableView().get_dataItems()[RowIndex].getDataKeyValue("ID");
                objectName = sender.get_masterTableView().get_dataItems()[RowIndex].getDataKeyValue("IDName");
                var objectType = '<%=objectType %>';
                if (objectType == 'CHT')
                    ObjectURL = "General/ChatHistory.aspx?ID=" + objectID;
                else
                    ObjectURL = "ShowObject.aspx?ObjectType=" + objectType + "&ID=" + objectID;
            }
            catch (e) {
                alert("Error In File CommonGrid.ascx \r\n Method GetCaseID \r\n ErrorMessage : " + e.description);
                return;
            }
        }

        function GetSelectedID(sender, args) {          
            try {
                var RowIndex = args.get_itemIndexHierarchical();
                //--get row values from selected row
                var objectID = sender.get_masterTableView().get_dataItems()[RowIndex].getDataKeyValue("ID");
                var SelectedID = $get('<%=hdnSelectedID.ClientID %>').value;
                var RID = "|" + objectID + "|";
               
                if (SelectedID.indexOf(RID) == -1) {
                    SelectedID += objectID + "|";
                    $get('<%=hdnSelectedID.ClientID %>').value = SelectedID;
                 
                }
            }
            catch (e) {
                alert("Error In File CommonGrid.ascx \r\n Method GetCaseID \r\n ErrorMessage : " + e.description);
                return;
            }
        }
        
        function GetDeSelectedID(sender, args) {        
            try {
                var RowIndex = args.get_itemIndexHierarchical();
                //--get row values from selected row
                var objectID = sender.get_masterTableView().get_dataItems()[RowIndex].getDataKeyValue("ID");
                var SelectedID = $get('<%=hdnSelectedID.ClientID %>').value;
                var RID = "|" + objectID + "|";
                SelectedID = SelectedID.replace(RID, "|");
                $get('hdnSelectedID').value = SelectedID;
            }
            catch (e) {
                alert("Error In File CommonGrid.ascx \r\n Method GetCaseID \r\n ErrorMessage : " + e.description);
                return;
            }
        }

        function OpenCommaonAttributesWindow(param) {
            try {
                var nID = $get('hdnSelectedID').value;
                if (nID == "0|") {
                    alert("Please, select one record!");
                    return false;
                }
                if (confirm('Are you sure you want to delete?')) {
                    var strURL = "CommonAttributes.aspx?ObjectType=" + '<%=objectType %>' + "&ID=" + nID + "&action=" + param;
                    OpenWindow(strURL);
                }
                
                return false;
            }
            catch (e) {
                alert("Error In File ShowList.aspx \r\n Method OpenCommaonAttributesWindow \r\n ErrorMessage : " + e.description);
                return;
            }
        }
        
        function OpenRecord(sender, args) {
            try {
              
                var RowIndex = args.get_itemIndexHierarchical();
                //--get row values from selected row
                var objectID = sender.get_masterTableView().get_dataItems()[RowIndex].getDataKeyValue("ID");
                objectName = sender.get_masterTableView().get_dataItems()[RowIndex].getDataKeyValue("IDName");
                
                var objectType = '<%=objectType %>';
                if (objectType == 'CHT')
                    ObjectURL = "General/ChatHistory.aspx?ID=" + objectID;
                else
                    ObjectURL = "ShowObject.aspx?ObjectType=" + objectType + "&ID=" + objectID;
            //    ObjectURL = "ShowObject.aspx?ObjectType=" + '<%=objectType %>' + "&ID=" + objectID;
                
                EditItem();
                return false;
            }
            catch (e) {
                alert("Error In File CommonGrid.ascx \r\n Method OpenRecord \r\n ErrorMessage : " + e.description);
                return;
            }
        }

        function ShowHide(paramDisplay, paramTable) {
            try {
                document.getElementById("tblSearch").style.display = 'none';
                document.getElementById("tblCustomize").style.display = 'none';
                document.getElementById("tblExport").style.display = 'none';

                document.getElementById(paramTable).style.display = paramDisplay;
                return false;
            }
            catch (e) {
                alert("Error In File ShowList.aspx \r\n Method ShowHide \r\n ErrorMessage : " + e.description);
                return;
            }
        }
        function IsInt(s) {
            return (s.toString().search(/^-?[0-9]+$/) == 0);
        }

        function EditView(paramEdit, paramType) {
            var nViewID = 0;
            var szTemp = '';

            try {
                if (!IsInt($find(viewComboControlID).get_value()))
                    paramEdit = false;

                if (paramEdit == true) {
                    nViewID = $find(viewComboControlID).get_value();
                }
                szTemp = 'NewView.aspx?RelatedTo=<%=objectType %>&RelatedToName=<%=objectTypeName %>&ViewID=' + nViewID + '&Type=' + paramType;
                OpenWindow(szTemp);
            }
            catch (e) {
                alert("Error In Method EditView \r\n ErrorMessage : " + e.description);
                return;
            }
        }
        function spliterSize() {
           // debugger;
            var myWidth = 0, myHeight = 0;
            if (typeof (window.innerWidth) == 'number') {
                //Non-IE
                myWidth = window.innerWidth;
                myHeight = window.innerHeight;
            } else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
                //IE 6+ in 'standards compliant mode'
                myWidth = document.documentElement.clientWidth;
                myHeight = document.documentElement.clientHeight;
            } else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
                //IE 4 compatible
                myWidth = document.body.clientWidth;
                myHeight = document.body.clientHeight;
            }
            var height = (myHeight - 32);
            //var height = (myHeight - table.offsetHeight);

            //document.getElementById('divmain').style.height = myHeight+'px'
            // window.document.getElementById('Radsplitter1').style.height = height + "px;bottom:0px;";
            return height - 2;
        }
        function SplitterLoaded(splitter, arg) {
            //debugger;
            var pane = splitter.getPaneById('<%= rdpLeft.ClientID %>');
            var height = spliterSize();
            splitter.set_height(height);
           // pane.set_height(height);

            var grid = $find("<%= rdgCommon.ClientID %>");
             // height = height-32 ;

            grid.get_element().style.height = height + "px";
            grid.repaint();
        }
        function setGridHeight() {
            var grid = $find("<%= rdgCommon.ClientID %>");
            var height = spliterSize();
           // height = height - 32;
            grid.get_element().style.height = height+ "px";
            grid.repaint();

        }
        $(window).resize(function () {
            var splitter = $find('radSplitterShowlist');
           var pane = splitter.getPaneById('<%= rdpLeft.ClientID %>');
            var height = spliterSize();
           // alert(height);
            splitter.set_height(height);
             pane.set_height(height);
           // document.getElementById('radSplitterShowlist').style.height = height+'px';
            var grid = $find("<%= rdgCommon.ClientID %>");
            //  height = height-32 ;

            grid.get_element().style.height = height + "px";
           grid.repaint();
        });
    </script>
  
</telerik:RadCodeBlock>
<body>
    <form id="frmCommonList" runat="server">
    <asp:ScriptManager ID="smCommon" runat="server">
    </asp:ScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    <ClientEvents OnResponseEnd="setGridHeight" />
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="rdgCommon">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="rdgCommon" LoadingPanelID="RadAjaxLoadingPanel1" />
            </UpdatedControls>
        </telerik:AjaxSetting>        
        <telerik:AjaxSetting AjaxControlID="lnkRefresh" EventName="Click">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="lblCount" />
                <telerik:AjaxUpdatedControl ControlID="hdnCurrentViewId" />
                <telerik:AjaxUpdatedControl ControlID="hdnCurrentViewTitle" />
                <telerik:AjaxUpdatedControl ControlID="lblMessage" />
                <telerik:AjaxUpdatedControl ControlID="rdgCommon" LoadingPanelID="RadAjaxLoadingPanel1" />
                <telerik:AjaxUpdatedControl ControlID="RadToolTipManager1" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnSearch" EventName="Click">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="lblCount" />
                <telerik:AjaxUpdatedControl ControlID="hdnCurrentViewId" />
                <telerik:AjaxUpdatedControl ControlID="hdnCurrentViewTitle" />
                <telerik:AjaxUpdatedControl ControlID="lblMessage" />
                <telerik:AjaxUpdatedControl ControlID="rdgCommon" LoadingPanelID="RadAjaxLoadingPanel1" />
                <telerik:AjaxUpdatedControl ControlID="RadToolTipManager1" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="lnkSelectView" EventName="Click">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="lblCount" />
                <telerik:AjaxUpdatedControl ControlID="hdnCurrentViewId" />
                <telerik:AjaxUpdatedControl ControlID="hdnCurrentViewTitle" />
                <telerik:AjaxUpdatedControl ControlID="lblMessage" />
                <telerik:AjaxUpdatedControl ControlID="rdgCommon" LoadingPanelID="RadAjaxLoadingPanel1" />
                <telerik:AjaxUpdatedControl ControlID="RadToolTipManager1" />
            </UpdatedControls>
        </telerik:AjaxSetting>
         <telerik:AjaxSetting AjaxControlID="treeView1" EventName="NodeClick">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="lblCount" />
                <telerik:AjaxUpdatedControl ControlID="hdnCurrentViewId" />
                <telerik:AjaxUpdatedControl ControlID="hdnCurrentViewTitle" />
                <telerik:AjaxUpdatedControl ControlID="lblMessage" />
                <telerik:AjaxUpdatedControl ControlID="rdgCommon" LoadingPanelID="RadAjaxLoadingPanel1" />
                <telerik:AjaxUpdatedControl ControlID="RadToolTipManager1" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="lnkSetAsDefault">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="lblMessage" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server">
        <img id="imgProgress" alt="Loading..."  src="images/LoadingProgressBar.gif"/>
    </telerik:RadAjaxLoadingPanel>
    <table width="100%" cellspacing ="0" height="100%">
    <tr class="trHeader">
        <th class="tdMainHeader1" style="width:30%" id="tdMainHeader" runat="server">
            <div id="divCaseHeader" runat="server">
                <img id="imgType" runat="server" src="Images/cases.png" />
                <asp:Label ID="lblType" runat ="server"></asp:Label>
            </div>
        </th>
         <td id="tdMainHeader1" runat="server" class="SubMenuTable" align="right">
            <table id="tblSubMenuOption" runat="server" cellpadding="4" cellspacing="0">
                <tr>                    
                    <td>
                        <asp:Label ID="lblCount" runat="server" Text=""></asp:Label>
                    </td>
                    <td class="SubMenuTD">
                        <a id="lnkSetAsDefault" runat="server" href="#" onserverclick="SaveColumnDetails" >Set As Default</a>
                    </td>
                    <td class="SubMenuTD">
                        <asp:LinkButton ID="lnkRefresh" runat="server" OnClick="SearchData" ><img style="border: 0px; vertical-align: middle;" alt="Refresh" src="Images/refresh.gif" />Refresh </asp:LinkButton>
                    </td>
                    <td class="SubMenuTD">
                        <asp:LinkButton ID="btnAdd" runat="server" CausesValidation="false"><img style="border:0px;vertical-align:middle;" alt="Add" src="Images/AddRecord.gif" /> Add </asp:LinkButton>
                    </td>
                    <td class="SubMenuTD">
                        <asp:LinkButton ID="btnEdit" CommandArgument="Edit" runat="server"><img style="border:0px;vertical-align:middle;" alt="Edit" src="Images/edit.gif" /> Edit </asp:LinkButton>
                    </td>                    
                    <%--<td valign="middle" class="SubMenuTD">
                        <asp:LinkButton ID="lnkChangeAttributes" runat="server" OnClientClick="return OpenCommaonAttributesWindow('U');"><img style="border: 0px; vertical-align: middle;" alt="Export" src="Images/edit.gif" />Multi Edit </asp:LinkButton>                        
                    </td>--%>
                    <td valign="middle" class="SubMenuTD">
                        <asp:LinkButton ID="lnkDelete" runat="server" OnClientClick="return OpenCommaonAttributesWindow('D');"><img style="border: 0px; vertical-align: middle;" alt="Export" src="CommonImages/Delete.gif" />Delete </asp:LinkButton>                        
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td colspan="2" style="margin-left: 40px">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            
        </td>
    </tr>   
    </table>
    <table width="99.8%" cellspacing ="0" style="height:100%">
    <tr>
        <td >
        <telerik:RadSplitter ID="radSplitterShowlist" SplitBarsSize="1px" Orientation="Vertical"
                runat="server" Width="100%" VisibleDuringInit="false" OnClientLoaded="SplitterLoaded" Height="100%">
                <telerik:RadPane ID="rdpLeft" runat="server" Scrolling="None" Width="20%" BackColor="">
                                <!--Search Part -->
            <table id="tblSearch" width="100%" style="display: block;">
                <tr>
                    <td width="25%" class="tdHeader" style="text-align: right">
                        Search In
                    </td>
                    <td>
                        <telerik:RadComboBox ID="cmbSearch" MaxHeight="320px" runat="server" Width="170px" Skin="Office2007">
                            <CollapseAnimation Duration="200" Type="OutQuint" />
                        </telerik:RadComboBox>
                    </td>
                 </tr>
                 <tr>
                 <td>
                 </td>
                 <td>
                        <telerik:RadTextBox ID="txtSearchText" EmptyMessage="Enter search text here" runat="server"
                            Skin="Office2007" InvalidStyleDuration="100" LabelCssClass="radLabelCss_WebBlue"
                            Width="165px">
                        </telerik:RadTextBox>                        
                 </td>                 
                 </tr>
                 <tr>
                    <td width="25%" class="tdHeader" style="text-align: left">
                        Created At
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="dtCreatedDateFrom" Width="70px" ShowPopupOnFocus="true" DatePopupButton-Visible="false" 
                        DateInput-DateFormat="MMM dd yyyy" runat="server" Skin="WebBlue" >
                        </telerik:RadDatePicker>&nbsp; - &nbsp;
                        <telerik:RadDatePicker ID="dtCreatedDateTo" Width="70px" ShowPopupOnFocus="true" DatePopupButton-Visible="false" DateInput-DateFormat="MMM dd yyyy" runat="server" Skin="WebBlue" >
                        </telerik:RadDatePicker>
                    </td>
                 </tr>                 
                 <tr id="tropendate" runat="server">
                    <td width="25%" class="tdHeader" style="text-align: left">
                        Open Date
                    </td>
                    <td>
                         <telerik:RadDatePicker ID="dtOpenDateFrom" Width="70px" ShowPopupOnFocus="true" DatePopupButton-Visible="false" 
                        DateInput-DateFormat="MMM dd yyyy" runat="server" Skin="WebBlue" >
                        </telerik:RadDatePicker>&nbsp; - &nbsp;
                        <telerik:RadDatePicker ID="dtOpenDateTo" Width="70px" ShowPopupOnFocus="true" DatePopupButton-Visible="false" DateInput-DateFormat="MMM dd yyyy" runat="server" Skin="WebBlue" >
                        </telerik:RadDatePicker>
                    </td>
                 </tr>                 
                 <tr id="trclosedate" runat="server">
                    <td width="25%" class="tdHeader" style="text-align: left">
                        Close Date
                    </td>
                    <td>
                         <telerik:RadDatePicker ID="dtCloseDateFrom" Width="70px" ShowPopupOnFocus="true" DatePopupButton-Visible="false" 
                        DateInput-DateFormat="MMM dd yyyy" runat="server" Skin="WebBlue" >
                        </telerik:RadDatePicker>&nbsp; - &nbsp;
                        <telerik:RadDatePicker ID="dtCloseDateTo" Width="70px" ShowPopupOnFocus="true" DatePopupButton-Visible="false" DateInput-DateFormat="MMM dd yyyy" runat="server" Skin="WebBlue" >
                        </telerik:RadDatePicker>
                    </td>
                 </tr>                 
                 <tr style="display:none">
                    <td width="25%" class="tdHeader" style="text-align: left">
                        Owner 
                    </td>
                    <td>
                        <telerik:RadTextBox ID="txtOwnerName" EmptyMessage="Enter owner name here" runat="server"
                            Skin="Office2007" InvalidStyleDuration="100" LabelCssClass="radLabelCss_WebBlue"
                            Width="120px" >
                        </telerik:RadTextBox> 
                        
                    </td>
                 </tr>
                 <tr >
                    <td width="25%" class="tdHeader" style="text-align: left">
                      <span style="display:none">  Status</span>
                    </td>
                    <td>
                        
                        <asp:CheckBox ID="chkMyAccount" runat="server" Text="Self" />                        
                        <asp:ImageButton ID="btnSearch" OnClick="SearchData" runat="server" ImageUrl="~/Images/Search1.gif" />
                        <telerik:RadTextBox ID="txtStatus" EmptyMessage="Under Development" runat="server"
                            Skin="Office2007" InvalidStyleDuration="100" LabelCssClass="radLabelCss_WebBlue"
                            Width="120px" Enabled="false" style="display:none">
                        </telerik:RadTextBox> 
                    </td>
                 </tr>
                 <tr>                    
                    <td valign="top">
                        &nbsp;&nbsp;
                    </td>                
                    <td valign="top">                                            
                        <asp:CheckBox ID="chkRecentActivity" runat="server" Text="Last " /> 
                        <telerik:RadTextBox ID="txtRecentDays"  onpaste="return false;" onkeypress="if (event.keyCode < 48 || event.keyCode > 57) event.returnValue = false;"
                            Skin="Office2007" Width="40px" EnableTheming="false" runat="server" MaxLength="2">
                        </telerik:RadTextBox> days activities
                                                
                    </td>
                </tr>
                    <tr>
                    <td colspan="2">
                        <hr />
                    </td>
                </tr>   
                  <tr >                    
                    <td  class="tdHeader">
                  <div id="trExport" visible="true" runat="server" >    Export </div>
                    </td>                
                    <td class="SubMenuTD">
                    <div id="trExportcontrol" visible="true" runat="server">                                       
                       <telerik:RadComboBox ID="cmbExport" runat="server" Width="145px" Skin="Office2007">
                            <CollapseAnimation Duration="200" Type="OutQuint" />
                        </telerik:RadComboBox>&nbsp;
                        <asp:ImageButton ID="imgExport" AlternateText="Click to Export" runat="server" OnClick="imgExport_Click" ImageUrl="~/Images/export.gif" />&nbsp;&nbsp;                        
                   </div> 
                                                
                    </td>
                </tr>
                            
              
               <%-- <tr id="trExportDesign" runat="server">                   
                    <td colspan="2">
                        <hr />
                    </td>
                </tr>   --%>
                
                <tr>
                    <td colspan="2" style="font-size:14px; color:Green; font-weight:bold;">
                        Personal Folders
                    </td>
                </tr>
                <tr>
                    <td colspan="2">                    
                        <telerik:RadTreeView ID="treeView1" runat="server" ShowLineImages="false" 
                            onnodeclick="treeView1_NodeClick"> 
                        </telerik:RadTreeView>
                    </td>
                </tr>                            
                <%--<tr>
                    <td style="text-align: right" class="tdHeader" valign="top">
                        View
                    </td>
                    <td class="SubMenuTD">    
                        <telerik:RadComboBox ID="cmbView" runat="server" Width="150px" Skin="Office2007">
                            <CollapseAnimation Duration="200" Type="OutQuint" />
                        </telerik:RadComboBox><br />
                        &nbsp;<a id="lnkSelectView" runat="server" href="#" onserverclick="SelectView">Go</a> | 
                        <a id="A1" href="javascript:EditView(true)">Edit View</a> | 
                        <a id="A2" href="javascript:EditView(false)" >New View</a><br /> 
                        &nbsp;<a id="A3" href="javascript:EditView(true,'C')" >Create Copy</a> |
                        <a id="lnkSetAsDefault" runat="server" href="#" onserverclick="SaveColumnDetails" >Set As Default</a>
                    </td>
                </tr>--%>
            </table>
                 
                </telerik:RadPane>         
                <telerik:RadSplitBar ID="rptSldBar1" runat="server" Width="3px" CollapseMode="Forward"></telerik:RadSplitBar>          
                <telerik:RadPane ID="rdpRight" runat="server" Width="79%" Scrolling="X">
                   <telerik:RadGrid ID="rdgCommon" runat="server" Skin="WebBlue" AllowMultiRowSelection="true"  
                BorderStyle="None" AllowPaging="True" Width="100%" 
                ItemStyle-Wrap="false" GridLines="None" AllowSorting="True" 
                ShowGroupPanel="True" OnNeedDataSource="rdgCommon_NeedDataSource" OnColumnCreated="rdgCommon_ColumnCreated">
                <ClientSettings AllowDragToGroup="true" AllowRowsDragDrop="true" AllowColumnsReorder="true" 
                    ReorderColumnsOnClient="true">                    
                    <Resizing EnableRealTimeResize="True" AllowResizeToFit="true" ResizeGridOnColumnResize="false"
                        AllowColumnResize="True"></Resizing>
                    <Scrolling AllowScroll="true" UseStaticHeaders="True" />
                    <Selecting AllowRowSelect="True" />
                 
                </ClientSettings>
                <GroupingSettings ShowUnGroupButton="true"  CaseSensitive="false"/>
                <HeaderContextMenu Skin="WebBlue" EnableTheming="True">
                    <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                </HeaderContextMenu>
                <GroupPanel EnableTheming="false">
                </GroupPanel>
                <ItemStyle Wrap="False"></ItemStyle>
                <ExportSettings IgnorePaging="true" OpenInNewWindow="true">
                    <Pdf PageHeight="297mm"  PageWidth="510mm" PageTitle="" PaperSize ="A4" 
                        PageLeftMargin="0px" Author="Easyfuel CRM"   />
                    <Excel Format="ExcelML" />
                 </ExportSettings>
                <MasterTableView CommandItemDisplay="None" TableLayout="Auto" 
                    ItemStyle-Wrap="false" DataKeyNames="id" AutoGenerateColumns="true" GroupLoadMode="Client" Width="100%" 
                    AllowPaging="true">
                   
                    <ItemStyle Wrap="False"></ItemStyle>

                     <PagerStyle Wrap="false" Mode="NextPrevAndNumeric" PrevPageImageUrl="~/Images/arrowLeft.gif"
                        NextPageImageUrl="~/Images/arrowRight.gif" NextPageText="Next" PrevPageText="Prev"
                        AlwaysVisible="true" FirstPageImageUrl="~/Images/PagingFirst.gif" LastPageImageUrl="~/Images/PagingLast.gif"  />
                    <Columns>   
                        <telerik:GridClientSelectColumn UniqueName="SelectColumn" Display="false">
                        <HeaderStyle Width="0px" />
                        </telerik:GridClientSelectColumn>                        
                    </Columns>
                </MasterTableView>
                <FilterMenu Skin="WebBlue" EnableTheming="True">
                    <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
                </FilterMenu>
                <SortingSettings />
            </telerik:RadGrid>
                </telerik:RadPane>
           </telerik:RadSplitter> 
            
            
        </td>
    </tr>
        <telerik:RadToolTipManager runat="server" ID="RadToolTipManager1" Position="Center"
        RelativeTo="Element" Animation="Resize" HideEvent="LeaveToolTip" ShowDelay="1000"
        Skin="WebBlue" OnAjaxUpdate="OnAjaxUpdate">
    </telerik:RadToolTipManager>
</table>
<telerik:RadWindowManager VisibleStatusbar="false" Width="450" Height="400" DestroyOnClose="true"
    ID="RadWindowManager1" runat="server" Modal="true" Animation="None">
    
</telerik:RadWindowManager>
            <input type="hidden" id="hdnFormURL" />
            <input type="hidden" id="hdnCurrentViewId" runat="server"/>
            <input type="hidden" id="hdnCurrentViewTitle" runat="server"/>
            <input type="hidden" id="hdnSelectedID" runat="server" value="0|"/>
            <input type="hidden" id="hdnurl" runat="server" value="0|"/>
    </form>
</body>
</html>



