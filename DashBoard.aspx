<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DashBoard.aspx.cs" Inherits="DashBoard" %>

<%@ Register Src="~/UserControls/ShowDashlet.ascx" TagName="GalaxyDashlet" TagPrefix="ucList" %>
<%@ Register Src="~/UserControls/Graph.ascx" TagName="Graph" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <%--<script type="text/javascript" src="https://www.google.com/jsapi"></script>
    
     <link href="css/datastyles/kendo.common.min.css" rel="stylesheet" />
    <link href="css/datastyles/kendo.dataviz.min.css" rel="stylesheet" />
    <link href="css/datastyles/kendo.rtl.min.css" rel="stylesheet" />
    <script src="Scripts/jquery.min.js"></script>
   <script src="js/kendo.web.min.js"></script>
    <script src="Scripts/kendo.dataviz.min.js"></script>
    <script src="js/kendo.dataviz.svg.min.js"></script>
    <link href="CommonStyleSheet/examples-offline.css?v=1.1" rel="stylesheet">
    <script src="Scripts/console.js"></script>
    <script src="Scripts/chart.js?v=1.1" type="text/javascript"></script>
    --%>
    <style>
        .k-chart
        {
            height: 270px !important;
        }
    </style>
</head>
<script type="text/javascript" language="javascript">
    function pageLoad(sender, args) {
        try {
            // setInterval('docksplit()',1000);
            if (args._isPartialLoad == false) {
                setTimeout('RefreshDashboard();', RefreshInterval);
            }
        }
        catch (e) {
            window.status = "Exception in PageLoad[Dashboard.aspx]-" + e.description;
            return;
        }
    }
    var RefreshInterval = '<%= ConfigurationManager.AppSettings["PopRefreshInterval"] %>';
    function RefreshDashboard() {

        setTimeout('RefreshDashboard();', RefreshInterval);
        $get("imgServerEvent").click();
        //location.reload();
    }
    function OpenItem(paramURL, paramName) {
        if (window.parent != null && window.parent.rdpRightPane != null)
            window.parent.OpenItem(paramURL, paramName);
    }
    function OpenQueueItem(URL, Name) {
        try {
            if (confirm("Are you sure to self assign?") == false)
                return;
            OpenItem(URL, Name);
            setTimeout('RefreshQueue();', 2000);
        }
        catch (e) {
            alert("File - HomePage.aspx\r\nMethod - OpenQueueItem\n" + e.description);
        }
    }
    //  function docksplit() {
    //  alert(document.getElementById('<%=hdndock.ClientID%>').value);
    //    var a=document.getElementById('<%=hdndock.ClientID%>').value;
    //  var b = a.split("|");
    //for (var i = 0; i < b.length; i++)
    //  {
    //    if (b[i] != "") {
    //  var c = b[i].split(",");
    //if (c.length == 4) {
    //  if (createChart != undefined) {
    //  console.log(c[0].toString());
    //    console.log(c[1].toString());
    //      console.log(c[2].toString()); console.log(c[3].toString());
    //        createChart(c[0].toString(), c[1].toString(), c[2].toString(), c[3].toString());
    //      }
    //     }
    //  }
    //}

    //}
</script>
<body style="width: 100%; height: 100%">
    <form id="Form1" method="post" runat="server">
        <telerik:RadScriptManager ID="ScriptManager" runat="server" />
        <telerik:RadFormDecorator ID="RadFormDecorator1" runat="server" DecoratedControls="All" />
        <asp:UpdatePanel runat="server" ID="UpdatePanel2" ChildrenAsTriggers="true" UpdateMode="Conditional">
            <ContentTemplate>
                <table style="width: 100%; height: 100%">
                    <tr class="trHeader">
                        <th class="tdSubHeader" colspan="1">Dashboard
                        </th>
                        <th style="text-align: right; width: 50%; vertical-align: top;" colspan="2">
                            <telerik:RadComboBox ID="cmbDashlets" MaxHeight="320px" Skin="Office2007"
                                runat="server" Width="203.5px">
                                <CollapseAnimation Duration="200" Type="OutQuint" />
                            </telerik:RadComboBox>
                            <asp:Button runat="server" ID="ButtonAddDock" Text="Add Dashlet" OnClick="ButtonAddDock_Click" />
                            <asp:Button runat="server" ID="ButtonSaveDock" Text="Save Dashboard" OnClick="ButtonSaveDock_Click" />
                            <asp:Button runat="server" ID="ButtonClear" Text="Clear Dashboard" OnClick="ButtonClear_Click" />
                            <asp:ImageButton ID="ButtonRefresh" runat="server" ImageUrl="~/Images/refresh.gif" OnClick="imgServerEvent_Click" AlternateText="" />
                            <%--OnClientClick="location.reload(); return false;"--%>

                        </th>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:Label ID="lblMessage" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <telerik:RadDockLayout runat="server" ID="RadDockLayout1" OnSaveDockLayout="RadDockLayout1_SaveDockLayout"
                                OnLoadDockLayout="RadDockLayout1_LoadDockLayout">
                                <table width="100%" id="TableLayout" cellspacing="0" cellpadding="3" runat="server">
                                    <tr id="trZone">
                                        <td align="left" valign="top" id="tdZone1">
                                            <telerik:RadDockZone runat="server" ID="RadDockZone1" Width="98%" MinHeight="350px"
                                                Style="float: left; margin-right: 10px; background: #ffffff; border-style: ridge;"
                                                BorderColor="WhiteSmoke" Skin="Office2007" Orientation="Vertical" FitDocks="true">
                                                
                                            </telerik:RadDockZone>
                                        </td>
                                        <td align="left" valign="top" id="tdZone2">
                                            <telerik:RadDockZone runat="server" ID="RadDockZone2" Width="98%" MinHeight="350px"
                                                Style="float: left; margin-right: 10px; background: #ffffff; border-style: ridge;"
                                                BorderColor="WhiteSmoke" Skin="Office2007" Orientation="Vertical" FitDocks="true">

                                            </telerik:RadDockZone>
                                        </td>
                                        <td align="left" valign="top" id="tdZone3">
                                            <telerik:RadDockZone runat="server" ID="RadDockZone3" Width="98%" MinHeight="350px"
                                                Style="float: left; margin-right: 10px; background: #ffffff; border-style: ridge;"
                                                BorderColor="WhiteSmoke" Skin="Office2007" Orientation="Vertical" FitDocks="true">
                                            </telerik:RadDockZone>
                                        </td>
                                    </tr>
                                </table>
                            </telerik:RadDockLayout>
                        </td>
                    </tr>
                </table>

                <asp:ImageButton ID="imgServerEvent" runat="server" Height="1px" Width="1px" ImageUrl="~/Images/white.gif" OnClick="imgServerEvent_Click" AlternateText="" />
                <asp:HiddenField ID="hdndock" runat="server" Value="" />
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ButtonAddDock" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="ButtonSaveDock" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="imgServerEvent" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="ButtonRefresh" EventName="Click" />
                <%-- <asp:PostBackTrigger ControlID="ButtonAddDock" />
            <asp:PostBackTrigger ControlID="ButtonSaveDock"  />
            <asp:PostBackTrigger ControlID="imgServerEvent" />
            <asp:PostBackTrigger ControlID="ButtonRefresh" />--%>
            </Triggers>
        </asp:UpdatePanel>
        <div style="width: 0px; height: 0px; overflow: hidden; position: absolute; left: -10000px;">
            Hidden UpdatePanel, which is used to help with saving state when minimizing, moving
        and closing docks. This way the docks state is saved faster (no need to update the
        docking zones).
        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
        </asp:UpdatePanel>
        </div>
        <%--<asp:HiddenField ID="hdntheme" runat="server" Value="default" />--%>
        <%--<input type="hidden" id="hdntheme" runat="server" />--%>
    </form>
    <%----%>
    <script>
     
    </script>
</body>

</html>

