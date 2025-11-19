<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreateAlerts.aspx.cs" Inherits="Alerts_CreateAlerts" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>New Alert</title>

    <script src="../Scripts/Vb.js" type="text/javascript"></script>

    <style type="text/css">
        .HiddenControl
        {
            display: none;
        }
    </style>
</head>

<script language="javascript" type="text/javascript">

    function validate()
    {   
        var datepicker = document.getElementById("<%=txtAlertDate.ClientID%>").value;          
        if (datepicker=="") 
        { 
           alert("please select date"); 
           return false;
        }
        else if(Trim(document.getElementById("<%=txtSubject.ClientID%>").value)=="")
        {
            alert("please enter subject"); 
            return false;    
        }    
    }
    
    function fncShowUserList()
    {
        var winFeature = 'width=700px,height=400px,top=140px,left=150px,status=1,resizable=1,scrollbars=0';
        var strUserName  = $get("<%=txtAlertUser.ClientID %>").value
        var wnd = window.open("UsersList.aspx?UserName="+strUserName+"",'Users',winFeature);   
    }
    
    //--close or cancel depending where this page opnes up as popup/frame
    function CloseCancel(param)
    {
        if(param == 'C')
            self.close();            
        else
            history.back();            
    }
    
    
     // Remove list box items 
    function RemoveItem()
    {
        var i;
        var selectBoxFrom;
        
        try
        {              
            selectBoxFrom = document.getElementById("lstUsers");

            for(i = selectBoxFrom.options.length - 1; i >= 0; i--)
            {
                if(selectBoxFrom.options[i].selected)
                {
                    document.getElementById("hdnUserIds").value = selectBoxFrom.options[i].value;
                    selectBoxFrom.remove(i);
                    document.getElementById("lnkHiddenClickRemove").click();
                }
            }
        }
        catch(e)
        {
            alert("Error In File CreateAlerts.aspx \r\n Method RemoveItem \r\n ErrorMessage : " + e.description);
            return;
        }
    }
    function OnClientCommandExecuting(editor, args)
    {
        try
        {
            var name = args.get_name();
            var val = args.get_value();
            if (name == "Emoticons")
            {
                editor.pasteHtml("<img src='" + val + "'>"); 
                //Cancel the further execution of the command as such a command does not exist in the editor command list
                args.set_cancel(true); 
            }
            
            if (name == "DynamicDropdown")
            {
                getSelectedAutoReplyMessage(val);
                args.set_cancel(true); 
            }
        }
        catch(e)
        {
            alert("Error In File CreateAlert.aspx \r\n Method OnClientCommandExecuting \r\n ErrorMessage : " + e.description);
            return;
        }
    }
    
    function getSelectedAutoReplyMessage(AutoReplyID)
    {
        try
        {
            EmailManagerWS.FetchTemplates(AutoReplyID,"", gotSelectedAutoReplyMessage);
        }
        catch(e)
        {
            alert("Error In File CreateAlert.aspx \r\n Method getSelectedAutoReplyMessage \r\n ErrorMessage : " + e.description);
            return;
        }
    }
    
    function gotSelectedAutoReplyMessage(result) 
    {   
        try
        {
            var responseCode = result.Response.rows[0]["ResultCode"];
            var responseString = result.Response.rows[0]["ResultString"];
            if(responseCode != 0) //error
            {
                return; 
            }
            else
            {
                var msg = result.Template.rows[0]["autoreply_message_html"] + "<br/>" + $find("txtEmailMessage").get_html();
                $find("txtEmailMessage").set_html(msg);
            }
        }
        catch(e)
        {
            alert("Error In File CreateAlert.aspx \r\n Method gotSelectedAutoReplyMessage \r\n ErrorMessage : " + e.description);
            return;
        }
    }
</script>

<body>
    <form id="frmAlertConfiguration" runat="server">
    <asp:ScriptManager ID="smMain" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <th id="thCaption" runat="server" class="tdMainHeader1">
                        <asp:Label runat="server" ID="lblCaption"></asp:Label>
                    </th>
                    <td style="text-align: right;" class="tdMainHeader1" nowrap="nowrap">
                        <asp:Button ID="btnSave" OnClientClick="return validate();" OnClick="btnSave_Click"
                            runat="server" Text="Send" Width="65px" />
                        <asp:Button ID="btnCancel" Width="65px" runat="server" Text="Cancel" />
                        <telerik:RadFormDecorator ID="frmDecorator" runat="server" />
                        <telerik:RadWindowManager ID="rwm" runat="server" Skin="WebBlue" Behaviors="Close,Minimize,Move"
                            DestroyOnClose="true" InitialBehaviors="Maximize">
                        </telerik:RadWindowManager>
                    </td>
                </tr>
            </table>
            <table width="100%">
                <tr>
                    <td colspan="5">
                        <asp:Label ID="lblMessage" runat="server" colspan="4"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="tdHeader" >
                        User
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txtAlertUser" MaxLength="100" runat="server" CssClass="textbox"
                            Width="298px">
                        </asp:TextBox>
                        <asp:ImageButton runat="server" ImageUrl="~/Images/attach.gif" ID="imgAttachOwner"
                            Text="..." OnClientClick="return fncShowUserList();"></asp:ImageButton>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="tdHeader" valign="top" >
                        To [Users]
                    </td>
                    <td valign="top" width="20%">
                        <asp:ListBox ID="lstUsers" runat="server" Width="300px" Height="120px"></asp:ListBox>
                        &nbsp;<span class="mandatory">*</span>
                        <asp:LinkButton ID="lnkHiddenClick" CssClass="HiddenControl" runat="server" OnClick="lnkHiddenClick_Click"></asp:LinkButton>
                        <asp:LinkButton ID="lnkHiddenClickRemove" CssClass="HiddenControl" runat="server" OnClick="lnkHiddenClickRemove_Click"></asp:LinkButton>
                    </td>
                    <td valign="top">
                        <input type="button" id="btnRemove" value="Remove >>" style="width: 65px" class="Button"
                            onclick="RemoveItem();" />
                    </td>
                </tr>
                <td class="tdHeader">
                    Date
                </td>
                <td colspan="3">
                    <telerik:RadDateTimePicker ID="txtAlertDate" DateInput-DateFormat="dd MMM yyyy hh:mm tt"
                        Skin="WebBlue" runat="server">
                        <Calendar>
                            <FooterTemplate>
                                <table width="100%" bgcolor="#CAE4FF" cellpadding="1">
                                    <tr>
                                        <td align="center">
                                            <a href="#" onclick="SelectToday('<%=txtAlertDate.ClientID %>');">Today</a> &nbsp;|&nbsp;<a
                                                href="#" onclick="ClearDate('<%=txtAlertDate.ClientID %>');">Clear</a>
                                        </td>
                                    </tr>
                                </table>
                            </FooterTemplate>
                            <SpecialDays>
                                <telerik:RadCalendarDay Repeatable="Today">
                                    <ItemStyle BackColor="#FFCC66" ForeColor="Blue" />
                                </telerik:RadCalendarDay>
                            </SpecialDays>
                        </Calendar>
                    </telerik:RadDateTimePicker>
                </td>
                </tr>
                <tr>
                    <td class="tdHeader">
                        Subject
                    </td>
                    <td colspan="3">
                        <telerik:RadTextBox ID="txtSubject" Skin="Office2007" InvalidStyleDuration="100"
                            LabelCssClass="radLabelCss_WebBlue" runat="server" Width="600px">
                        </telerik:RadTextBox>&nbsp;<span class="mandatory">*</span>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <telerik:RadEditor ID="txtDescription" runat="server" Height="290" EditModes="Design"
                            Width="99%" AutoResizeHeight="false" Scrolling="Y" BorderStyle="None" ToolsFile="~/Scripts/BasicTools.xml">
                            <ImageManager EnableImageEditor="true" />
                            <Content>                        
                            </Content>
                        </telerik:RadEditor>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:HiddenField ID="hdnUserIds" runat="server" />
                        <asp:HiddenField ID="hdnUserNames" runat="server" />                      
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
