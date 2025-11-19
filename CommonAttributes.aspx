<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CommonAttributes.aspx.cs" Inherits="CommonAttributes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<script type="text/javascript" language="javascript">
    function validateSave() {
        try {
            var control = $get('hdnOwnerID');
            if (control.value == '0') {
                alert('Please Select Owner !');
                return false;
            }

            return true;
        }
        catch (e) {
            alert("File - CommonAttributes.aspx\r\nMethod -validateSave()\n" + e.description);
            return false;
        }
    }

    function OpenWindow(param) {
        try {
            var searchText = '';
            var objectType = '';
            var objectID = '';

            switch (param) {
                case 'OWNER':
                    searchText = $get("txtOwnerName").value;
                    objectType = 'QUE';
                    objectID = $get("hdnOwnerID").value;
                    break;                
            }
            if (objectType == '' || objectType == '0')
                return;

            gLookupSource = param;
            var url = 'LookUp.aspx?objecttype=' + objectType + '&SearchText=' + searchText + '&SourceID=' + objectID;
            var v = radopen(url);
            v.set_modal(true);

            v.center();
            v.setSize(700, 500); //height, width
            v.add_close(onWindowClose);
            return false;
        }
        catch (e) {
            alert("Error:- Page -> CommonAttributes.aspx -> Method -> 'OpenWindow' \n Description:- \n" + e.description);
            return false;
        }
    }
    function onWindowClose(oWnd) {
        try {
            if (oWnd.argument != null) {
                switch (gLookupSource) {
                    case 'OWNER':
                        $get("hdnOwnerID").value = oWnd.argument.Id;
                        $get("hdnServerEvent").value = 'OWNER';
                        break;                    
                }
                $get("imgServerEvent").click();
            }
            return false;
        }
        catch (e) {
            alert("Error :- Page:- \"CommonAttributes.aspx\" Method:- 'onWindowClose' Description:- " + e.description);
            return false;
        }
    }
</script>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="script1" runat ="server">
    </asp:ScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    <AjaxSettings>
       
    </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server">
        <img id="imgProgress" alt="Loading..." src="images/LoadingProgressBar.gif"/>
    </telerik:RadAjaxLoadingPanel>
<asp:UpdatePanel ID="upPanel" runat="server" UpdateMode="Conditional" RenderMode="Inline">
    <ContentTemplate> 
         <asp:Label ID="lblMessage" runat="server"></asp:Label>  
   
        <table width="100%" id="tbl_DeletedStatus" runat="server">
            <tr class="trHeader">
                <th id="th1" runat="server" class="tdSubHeader" width="100%" colspan="3">
                    Record Deletion Confirmation
                </th>
                <td align="right">
                </td>
            </tr>
            <tr>
                <td style="text-align: right">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td id="tdDeleteConfirmation" runat="server" align="center">
                    &nbsp;
                </td>
            </tr>
        </table>

     
            <input type="hidden" runat="server" id="hdnServerEvent" value="" />
            <input type="hidden" value="0" id="hdnOwnerID" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>         
        <telerik:RadWindowManager VisibleStatusbar="false" Width="450" Height="400" DestroyOnClose="true"
        ID="RadWindowManager1" runat="server" Modal="true" Animation="None">
    </telerik:RadWindowManager>

    </form>
</body>
</html>
