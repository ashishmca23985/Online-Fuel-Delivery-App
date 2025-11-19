<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChatData.aspx.cs" Inherits="Chat_ChatData" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<script language="javascript" type="text/javascript">
 function getchathistory(szchatsessionid)
    {
      
        if(szchatsessionid =='')
        {
            clearTimeout(timeoutid);
        }
        else
        {
            ChatWS.GetChatHistory(szchatsessionid,onsucesschat); 
            //timeoutid=setTimeout("getchathistory('"+szchatsessionid+"')",2000);
          
        }
    }
    
    function onsucesschat(result)
    {
        var responseCode = result.Response.rows[0].ResultCode;
        var responseString = result.Response.rows[0].ResultString;
        if (responseCode != 0) //error
        {
        
        }
        else
        {
            var szraddisplaycontent = $find("radeditordisplay"); 
            //szraddisplaycontent.set_html('');
            //szraddisplaycontent.set_editable(true);
            szraddisplaycontent.set_html(responseString);
            //szraddisplaycontent.set_editable(false);
           // var szhtmlcontent = $find("radchatwriter"); 
            //var szchatmsg= szhtmlcontent.get_html();
            //szhtmlcontent.set_html('');
            //szhtmlcontent.pasteHtml(szchatmsg);
           // szhtmlcontent.setFocus();
           var sid = '<%=Session["csid"].ToString() %>';
           setTimeout("getchathistory('" + sid + "')", 2000);
        }
    }
    
</script>
<body onload="javascript:getchathistory('<%=Session["csid"].ToString() %>');" >
    <form id="form1" runat="server">
     <asp:ScriptManager ID="scriptmgr" runat="server">
        <Services>
        <asp:ServiceReference Path="~/Services/ChatWS.asmx" /> 
        </Services>
        </asp:ScriptManager>
   <telerik:RadEditor ID="radeditordisplay" runat="server" Skin="WebBlue" Width="100%"
                                                Height="200px" EditModes="Design" Style="float: left" ToolbarMode="ShowOnFocus"
                                                ToolsWidth="1px">
                                                <Tools>
                                                    <telerik:EditorToolGroup>
                                                    </telerik:EditorToolGroup>
                                                </Tools>
                                                <Content>
                                                </Content>
                                            </telerik:RadEditor>
    </form>
</body>
</html>
