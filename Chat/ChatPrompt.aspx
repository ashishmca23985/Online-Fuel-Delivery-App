<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChatPrompt.aspx.cs" Inherits="ChatPrompt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Chat Request</title>
</head>
<body onload="javascript:CheckRequestStatus();">
    <form id="form1" runat="server">
    <script type="text/javascript">
        var aa = '<%= chatwaitnigtime%>';
        var chatwaitnigtime = parseInt(aa,10);
        function pageLoad(sender, args) {
            
            try {
                setInterval('settime()', 20000);
            if (args._isPartialLoad == false) {
                
                var oWnd = GetRadWindow();
                if (oWnd != null) {
                    var oArg = new Object();
                    oArg.ChatID = '<%=Id %>';
                    oWnd.argument = oArg;
                }
                }

            }
            catch (e) {
                alert("Error In File chatprompt.aspx \r\n Method PageLoad \r\n ErrorMessage : " + e.description);
                return;
            }
        }
        var timecount = 0;
        function settime() {
            // console.log("chatwaitnigtime---------------" + chatwaitnigtime + "------------" + timecount);
           
        
            if ((chatwaitnigtime * 3) > timecount) {
                timecount = timecount + 1;
            }
            else {
              
                window.parent.parent.clearTitle();
                CloseWindow();

            }
          
        }
    function CheckRequestStatus()
    {
         var szchatid='<%=Id %>';
         ChatWS.CheckRequestStatus(szchatid,onreqSuccess)
         timeoutid=setTimeout("CheckRequestStatus()",2000);
    }
      function onreqSuccess(result)
    {
        try
        {   
            var responseCode = result.Response.rows[0].ResultCode;
            var responseString = result.Response.rows[0].ResultString;
            if (responseCode =="-1") //error
            {
                alert("Error:- Page -> chatprompt.aspx -> Method -> \"onreqSuccess\" \n Description:- \n" + e.description);
                return false;
            }
            else if (responseString == 'Accepted' || responseString == 'Expired')
            {
                // ////console.log("onreqSuccess-------------");
                window.parent.parent.clearTitle();
                CloseWindow();
            }
        }
        catch(e)
        {

        } 
    }
    function declinerequest()
    {
        var szcontactid='<%=TB_nContactID %>';
        var szchatid='<%=Id %>';
        ChatWS.DeclineChatRequest(szchatid,szcontactid,onSuccess,onFailed)
        
    }
    
    function onSuccess(result)
    {
        try
        {
            //console.log("onSuccess-------------");
            CloseWindow();
        }
        catch(e)
        {
        
        }
    }
    function onFailed(result)
    {
        alert(result);
    }
    
      function GetRadWindow() {
        try {
            var oWindow = null;

            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;

            return oWindow;
        }
        catch (e) {
            alert("Error:- Page -> chatprompt.aspx -> Method -> \"GetRadWindow\" \n Description:- \n" + e.description);
            return false;
        }
    }
      function CloseWindow() {
          try {
              var oWnd = GetRadWindow();
              if (oWnd != null) {
                  var oArg = new Object();
                  oArg.ChatID = '<%=Id %>';
                  oWnd.argument = oArg;
                  oWnd.close();
              }
        }
        catch (e) {
            alert("Error:- Page -> Lookup.aspx -> Method -> 'CloseWindow' \n Description:- \n" + e.description);
            return false;
        }
      }
       
        function OnSaveSuccess(CreateNewObject, sessionid,id) {
            try {
                var CLI = '<%=CLI%>';
                var email = '<%=email%>';
                var name = '<%=VisitorName%>'
                if (sessionid != '') {
                    window.parent.OpenItem("TCK/Landing.aspx?ChatID=" + id + "&CallType=C&CLI=" + CLI + "&email=" + email + "&name=" + name, CLI);

                    location.href = "ChatCustomer.aspx?source=1&type=E&chatsessionid=" + sessionid ;
                }  
                
            }
            catch (e) {
                alert("File - chatprompt.aspx\r\nMethod -OnSaveSuccess()\n" + e.description);
            }
        }
        function OnAccept(name) {
            alert("This Web Chat is already accepted by " + name);
            CloseWindow();
        }
</script>
    <div>
    <asp:ScriptManager ID="scriptmgr" runat="server">
    <Services>
    <asp:ServiceReference Path="~/Services/ChatWS.asmx" /> 
    </Services>
    </asp:ScriptManager> 
    </div>
    <div align="center" style="border:solid 1px #cccccc; padding:7px;">
     <asp:Label ID="lblMessage" runat="server"></asp:Label>
    <%=promptString %>
    <br />
        <asp:Button ID="btnAccept" runat="server" Text="Accept" 
            onclick="btnAccept_Click" style="background-color:#903d8e;color:white;"/>&nbsp;&nbsp;&nbsp;
        <input id="btnCancel" type="button" value="Decline" runat="server"  class="Button" onclick="javascript:declinerequest();" style="background-color:red; color:white;"/>
    </div>
    </form>
</body>

</html>
