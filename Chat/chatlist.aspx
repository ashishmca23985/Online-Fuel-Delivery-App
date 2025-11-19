<%@ Page Language="C#" AutoEventWireup="true" CodeFile="chatlist.aspx.cs" Inherits="Chat_chatlist" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
     <asp:ScriptManager ID="sm" runat="server"></asp:ScriptManager>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnRefreshRecent">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="tblcustomerlist"  />
                    <telerik:AjaxUpdatedControl ControlID="lblMessage"  />
                    
                </UpdatedControls>
            </telerik:AjaxSetting> 
             <telerik:AjaxSetting AjaxControlID="btnsetstatus" EventName="Click">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="spnloginagent"  />
                    <telerik:AjaxUpdatedControl ControlID="imgmodes"  />
                    <telerik:AjaxUpdatedControl ControlID="radcombomodes"  />
                    <telerik:AjaxUpdatedControl ControlID="lblMessage"  />
                    <telerik:AjaxUpdatedControl ControlID="btnsetstatus"  />
                    
                </UpdatedControls>
            </telerik:AjaxSetting>                
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div style="width:100%;text-align:left;padding-left:5px" id="divcustomerlist" runat="server">
     <table width="100%" cellpadding="0" cellspacing="0">
    
    <tr>
        <td align="right" style="padding-top:10px">
            <span id="spnloginagent" runat="server" style="display:block;padding-left:12px;font-size:12px;font-weight:bold;float:left"></span> 
            <asp:LinkButton ID="btnRefreshRecent" runat="server" OnClick="btnRefreshRecent_OnClick" >
            <img style="border:0px;vertical-align:middle;padding-right:5px" alt="Refresh" src="../Images/refresh.gif" />
            </asp:LinkButton>
            
        </td>        
    </tr>
    <tr>
    <td>
   
    </td>
    </tr>
   
      <tr id="trstatus" runat="server">
        <td>
            <div style="width:100%;text-align:left">
                
                <img id="imgmodes" runat="server" style="float:left;padding-top:7px" />
                <telerik:RadComboBox ID="radcombomodes" runat="server" style="float:left">
                <Items>
                <telerik:RadComboBoxItem Text="Available" Value="Available" />
                <telerik:RadComboBoxItem Text="Away" Value="Away" />
                <telerik:RadComboBoxItem Text="Busy" Value="Busy" />
                <telerik:RadComboBoxItem Text="Invisible" Value="Invisible" />
                
                </Items>
                </telerik:RadComboBox>
                <asp:Button ID="btnsetstatus"  runat="server"  Text="Set"  style="float:left"
                    onclick="btnsetstatus_Click"/>
            
            </div>
        </td>        
    </tr>
    <tr>
        <td>
        <br />
        </td>
    </tr>
     <tr>
        <td id="tdcustomerlist">
            
            <asp:Label ID="lblcustomerlist" runat="server" Text=""></asp:Label>
            <asp:Table ID="tblcustomerlist"  CellPadding="1" CellSpacing="0" runat="server">
            </asp:Table>
        </td>
    </tr>
    <tr>
    <td>
     <asp:Label ID="lblMessage" runat="server" Text=""></asp:Label>
    </td>
    </tr>
</table>

        
    </div>
    </form>
    <script type="text/javascript">
   
    
</script>
</body>

</html>
