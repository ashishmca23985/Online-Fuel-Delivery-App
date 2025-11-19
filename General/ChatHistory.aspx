<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChatHistory.aspx.cs" Inherits="General_ChatHistory" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
    <script type="text/javascript">


        function OpenItem(ObjectURL, strTabName) {
            try {

                if (window.parent.document.getElementById("radSplitter") == null) {

                    window.parent.parent.OpenItem(ObjectURL, strTabName);
                }
                else {

                    window.parent.OpenItem(ObjectURL, strTabName);
                }
                return false;
            }
            catch (e) {
                alert("Error In File CommonGrid.ascx \r\n Method OpenRecord \r\n ErrorMessage : " + e.description);
                return;
            }
        }
        </script>
<body>
  <form id="frmChatHistory" runat="server">
         <asp:ScriptManager ID="smChatGeneral" runat="server">
        <Services>
        </Services>
    </asp:ScriptManager>
   
       
    <table class="ascxMainTable2" style="width:100%" cellpadding="1" cellspacing="1">
        <tr class="trHeader">
            <th class="tdSubHeader" colspan="2">
                General
            </th>
            <th style="text-align:right;width:50%" colspan ="2" >
                
            
            </th>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </td>
        </tr>
        <tr id="tr1" runat="server" >
            <td colspan="4"><table>
        <tr >
            <td class="tdHeader" style="width: 12%">
                Visitor Name
            </td>
            <td>

                <telerik:RadTextBox ID="txtvisitorname" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>
            <td class="tdHeader" style="width: 10%">
             Online Chat By
            </td>
            <td>
                <telerik:RadTextBox ID="txtgalaxyusername" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>
        </tr>
        <tr>
        <td class="tdHeader" style="width: 12%">
                Contact Number
            </td>
            <td>

                <telerik:RadTextBox ID="txtphone" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>
            <td class="tdHeader" style="width: 10%">
               Email
            </td>
            <td>
                <telerik:RadTextBox ID="txtEmail" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>
        </tr>
        
        <tr>
            <td class="tdHeader" style="width: 10%">
               Company Name
            </td>
            <td>
                <telerik:RadTextBox ID="txtcompanyname" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>


            <td class="tdHeader" style="width: 10%">
            Chat Start Time
            </td>
            <td>
                 <telerik:RadTextBox ID="txtTime" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100" 
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
             <a href="#" runat="server" id="callername"></a>
            </td>
        <td class="tdHeader" style="width: 12%">
                &nbsp;</td>
            <td>

                &nbsp;</td>
            <td class="tdHeader" style="width: 10%">
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        
                  <tr id="Tr2"  runat="server">
            <td class="tdHeader" style="width: 12%">
                Query
            </td>
            <td>
                <telerik:RadTextBox ID="txtQuery" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100" TextMode="MultiLine"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>

                       
        </tr>
           </table>     </td></tr>
        <tr>
            <td colspan="4">
                <hr size="1px" width="99%" style="border: 1px dotted #949494" />
            </td>
        </tr>
          <tr>
<td colspan="4">
<div id="HeadingComment" style="color:#525254;" visible="false"  runat="server"><h3>
    LIST OF CHAT :-</h3></div>
</td>
</tr>
    
    
   <tr>
<td colspan="4">


  <table width="100%" border="0" cellspacing="2" cellpadding="0" class="chat">
  <tr>
<td style="width:100%;" align="center">
<%--<div style="max-height:100px;max-width:100%;overflow:auto;">--%>
<asp:Repeater ID="RepComment" runat="server">
    <ItemTemplate>
        <div style=" width: 100%;font-family:Verdana" >
            <div style="text-align: left;font-size:12px;padding-left: 15px;">
                <b>
                  <span style="float: left; padding-right: 32px;width:350px"><%#DataBinder.Eval(Container, "DataItem.fullname")%> </span><span style="padding-left:50px">  <%#DataBinder.Eval(Container, "DataItem.time")%></span>
                </b>
            </div>

          
              <div style="color: Black; text-align: left; padding-left: 25px; padding-top: 5px;line-height:16px; font-size:11px;">

                <%#DataBinder.Eval(Container, "DataItem.chat_message")%>
            </div>

        </div>
  <br />
<hr size="1px" width="99%" style="border: 1px dotted #949494" />
 </ItemTemplate>
    </asp:Repeater>

</td>
</tr></table>
    </td>
       </tr>
        </table></form></body>
</html>
