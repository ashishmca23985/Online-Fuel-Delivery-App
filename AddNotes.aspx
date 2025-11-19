<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddNotes.aspx.cs" Inherits="General_AddNotes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Add Note</title>
</head>
<body>
 <script type="text/javascript">
   function validateSave() {
           try {
              
               control = $find('txtNote');
              if (control.get_value() == '') {
                   alert('Please Enter Note !');
                   control.focus();
                   return false;
               }
              
              
        control = $get('dtstatus');
             if (control.value == '') {
                 alert('Please enter next status time !');
                 control.focus();
                 return false;
             }
             

           }
           catch (e) {
               alert("File - AddNotes.aspx\r\nMethod -validateSave()\n" + e.description);
               return false;
           }
       }
               function CloseWindow(parse) {
        try {
           
                var oWnd = GetRadWindow();
           
            oWnd.close();
        }
        catch (e) {
            alert("Error:- Page -> AddNote.aspx -> Method -> 'CloseWindow' \n Description:- \n" + e.description);
            return false;
        }
    } 
     function GetRadWindow() {
        try {
            var oWindow = null;

            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;

            return oWindow;
        }
        catch (e) {
            alert("Error:- Page -> Lookup.aspx -> Method -> \"GetRadWindow\" \n Description:- \n" + e.description);
            return false;
        }
    }   
 
 </script>
  
    <form id="frmaddnotes" runat="server">
  <asp:ScriptManager ID="smCaseGeneral" runat="server">
    </asp:ScriptManager>   
 
    <div>
    <table width="100%" class="ascxMainTable2" cellpadding="2" cellspacing="4">
     <tr class="trHeader">
                <th class="tdSubHeader" colspan="2">
                    Add Note
                </th>
                <th style="text-align: Right; width: 50%" colspan="2">
                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="javascript:if(validateSave() == false) return false;" />
                     <asp:Button ID="btnClose" runat="server" Visible="false" Text="Close"  OnClientClick="CloseWindow('Close');" />
                 
                </th>
            </tr>
            <tr>
                <td colspan="4">
                  <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </td>
            </tr>

    <tr >
    <td colspan="6"> 
    <table width="100%" class="ascxMainTable2" cellpadding="2" cellspacing="2">
               <tr>
              
    <td class="tdHeader" style="width:25%" >Notes &nbsp;<span class="mandatory">*</span></td>
    <td colspan="4" >   <telerik:RadTextBox ID="txtNote"  Skin="Office2007" TextMode="MultiLine"
                        InvalidStyleDuration="100" Height="100px" LabelCssClass="radLabelCss_WebBlue" runat="server"
                        Width="500">
                    </telerik:RadTextBox>
                </td>
    </tr>
    <tr ><td class="tdHeader"  style="width:25%">Next Status Time &nbsp;<span class="mandatory">*</span></td>
    <td colspan="4">        <telerik:RadDateTimePicker ID="dtstatus" DateInput-DateFormat="MMM dd yyyy hh:mm tt"
                        runat="server" Skin="WebBlue"  Width="200px">
                    </telerik:RadDateTimePicker></td> 
    </tr>
   </table></td></tr>
        
    </table>
    </div>
    </form>
</body>
</html>
