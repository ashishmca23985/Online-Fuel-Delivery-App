<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AgentHomePage.aspx.cs" Inherits="AgentHomePage" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Homepage</title>
    <link rel="stylesheet" type="text/css" href="CSS/inner.css" />
    <link href="CSS/StyleSheet.css" rel="stylesheet" type="text/css" />
   
</head>
<telerik:RadCodeBlock ID="radCode" runat ="server">
<script type ="text/javascript" language ="javascript" >
    function SetMyProfileTab(url, tabname)
    {
        if(url != "")
            document.getElementById("IfCustomerProfile").src = url;
    }
</script>
</telerik:RadCodeBlock>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="smanager" runat="server" EnablePageMethods="true" >
    </asp:ScriptManager>
     <telerik:RadAjaxManager  ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnStatus">
                <UpdatedControls>
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div align="center">
        <iframe id="IfCustomerProfile" runat ="server" width ="100%"  height ="700px" frameborder=0 ></iframe>
    </div>
           <asp:Button ID="btnStatus" Text="Refresh"  runat ="server" Width ="0px" OnClick="btnStatus_Click"/>
    <div  style="color:Teal; font-weight: bold; font-size:smaller" align=right>
        Powered By Teckinfo
    </div>
    </form>
</body>
</html>
