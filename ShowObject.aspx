<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ShowObject.aspx.cs" Inherits="ShowObject" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

    <script type="text/javascript">
       
        function RefreshRecentItems() {
            if (window.parent != null) {
                window.parent.RefreshRecentItems();
            }
        }

        // this is function is called from Tabs like CaseGeneral/ TaskGeneral etc
        function OnObjectSave(CreateNewObject,id,type) {
            RefreshRecentItems();
            if (CreateNewObject == 'Y') {
                if (id != undefined && type != undefined) {
                    window.parent.DeleteSelectedTab('New ' + type, type + '[' + id + ']');
                    window.parent.OpenItem("ShowObject.aspx?ID=0&ObjectType=<%=objectType%>&Source=<%=sourceId %>&SourceType=<%=sourceType %>", 'New ' + type)
                }
                else {
                    
                    window.parent.OpenItem("ShowObject.aspx?ID=0&ObjectType=<%=objectType%>&Source=<%=sourceId %>&SourceType=<%=sourceType %>", 'New')
                }
                return false;
             //   
            }
            else {
                var objectType = "<%=objectType%>";
                var objectId = "<%=objectId%>";
                FrameworkWS.GetCustomTab(objectType, objectId, GetCustomTabSuccess, OnError, onServerRequestTimeOut);
            }
        }

        function getType(type) {
            var result = "";
            switch (type) {
                case "CAS":
                    result = "Cases"
                    break
                case "CNT":
                    result = "Costomer"
                    break
                default:
                    result = "";
            }
            return result;
        }
        function GetCustomTabSuccess(result) {
            try {
                var responseCode = result.Response.rows[0].ResultCode;
                var responseString = result.Response.rows[0].ResultString;
                if (responseCode < 0) {
                    alert(responseString);
                    return;
                }
                var tabStrip = $find("<%= rdTabCommon.ClientID %>");
                if (tabStrip) {
                    var tab = tabStrip.findTabByValue('Custom');
                    if (tab) {
                        var prevTitle = tab.get_text();
                        var newTitle = result.Data.rows[0].TabName;
                        if (newTitle == "" || newTitle == null)
                            newTitle = "Custom";
                        if (prevTitle != newTitle) {
                            tab.set_text(newTitle);
                            var customFrame = document.getElementById("ifcustom");
                            var url = result.Data.rows[0].TabUrl;
                            if (url != "" && url != null) {
                                url = url.replace("#ID#", "<%=objectId%>");
                                customFrame.src = url;
                            }
                        }
                    }
                }
            }
            catch (e) {
                alert("File - ShowObject.aspx\r\nMethod - GetCustomTabSuccess\n" + e.description);
            }
        }
        function onServerRequestTimeOut() {
            alert('Server request timeout');
        }
        function OnError(args) {
            alert(args._message);
        }
        function OpenAlertWindow() {
            try {

                var objectID = '<%=objectId %>';
                var objectType = '<%=objectType %>';

                var url = 'SendAlerts.aspx?ObjectType=' + objectType + '&ObjectID=' + objectID;

                var v = radopen(url);
                v.set_modal(true);
                v.moveTo(250, 60)
                //v.center();
                v.setSize(900, 500); //height, width
                //v.add_close(onWindowClose);
            }
            catch (e) {
                alert("Error:- Page -> CaseGeneral.aspx -> Method -> 'OpenAlertWindow' \n Description:- \n" + e.description);
                return false;
            }
        }
    </script>

</telerik:RadCodeBlock>
<body>
    <form id="frmCommon" runat="server">
    <asp:ScriptManager ID="sm" runat="server">
        <Services>
            <asp:ServiceReference Path="~/Services/FrameworkWS.asmx" />
        </Services>
    </asp:ScriptManager>
    <table width="100%" cellpadding="1" cellspacing="0">
        <tr>
            <th id="thCaption" runat="server" class="tdMainHeader1">
                <img id="imgType" runat="server" src="" alt="" />
                <asp:Label ID="lblCaption" runat="server"></asp:Label>
            </th>
            <th style="text-align: right; width: 50%;" class="tdMainHeader1">
                <input type="button" class="Button" style="width: 120px" visible="false" runat="server" value="Send Notification"
                    id="btnSendAlert" onclick="javascript:OpenAlertWindow();" />
            </th>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <telerik:RadTabStrip ID="rdTabCommon" runat="server" ReorderTabsOnSelect="false"
                    MultiPageID="rdMPCommon" SelectedIndex="0" Height="100%" Align="Justify" >
                </telerik:RadTabStrip>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <telerik:RadMultiPage ID="rdMPCommon" runat="server" Height="100%">
                </telerik:RadMultiPage>
            </td>
        </tr>
    </table>
    <telerik:RadWindowManager VisibleStatusbar="false" Width="450" Height="400" DestroyOnClose="true"
        ID="RadWindowManager1" runat="server" Modal="true" Animation="None">
    </telerik:RadWindowManager>
    </form>
</body>
</html>
