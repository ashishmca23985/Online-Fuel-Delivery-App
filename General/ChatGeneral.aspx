<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChatGeneral.aspx.cs" Inherits="General_ChatGeneral"  EnableEventValidation="false"  ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<script type="text/javascript">

  function OpenWindow(param) {
         try {
             var searchText = '';
             var objectType = '';
             var objectID = '';
             switch (param) {
                 case 'RELATEDTO':
                     searchText = $get("txtRelatedToName").value;
                     objectType = $find("cmbRelatedTo").get_value();
                     objectID = $get("hdnRelatedToID").value;
                     break;
                 case 'OWNER':
                     searchText = $get("txtOwnerName").value;
                     objectID = $get("hdnOwnerID").value;
                     objectType = 'QUE';
                     break;
             }
             if (objectType == '')
                 return;
             gLookupSource = param;
             var url = '../LookUp.aspx?objecttype=' + objectType + '&SearchText=' + searchText + '&SourceID=' + objectID;
             var v = radopen(url);
             v.set_modal(true);

             v.center();
             v.setSize(700, 500); //height, width
             v.add_close(onWindowClose);

             //            v.moveTo(0, 0);
             //v.add_dragStart(OnClientDragStart);
             //v.add_dragEnd(OnClientDragEnd);
             return false;
         }
         catch (e) {
             alert("Error:- Page -> ChatGeneral.aspx -> Method -> 'OpenWindow' \n Description:- \n" + e.description);
             return false;
         }
     }
     function onWindowClose(oWnd) {
         try {
             if (oWnd.argument != null) {
                 switch (gLookupSource) {
                     case 'RELATEDTO':
                         $get("hdnRelatedToID").value = oWnd.argument.Id;
                         $get("hdnServerEvent").value = 'RELATEDTO';
                         break;
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
             alert("Error :- Page:- \"ChatGeneral.aspx\" Method:- 'onWindowClose' Description:- " + e.description);
             return false;
         }
     }
     function OpenLink(object, id, Name) {
         if (confirm('Are you sure?') == false)
             return;
         if (window.parent.parent != null && window.parent.parent.rdpRightPane != null)
             window.parent.parent.OpenItem("ShowObject.aspx?ObjectType=" + object + "&ID=" + id, Name);
     }


</script>
<body>
    <form id="frmChatGeneral" runat="server">
   <asp:ScriptManager ID="smChatGeneral" runat="server">
        <Services>
        </Services>
    </asp:ScriptManager>
    <AjaxControls:ModalUpdateProgress ID="ModalUpdateProgress1" runat="server" DisplayAfter="0" BackgroundCssClass="modalBackground">
        <ProgressTemplate >
        </ProgressTemplate>
    </AjaxControls:ModalUpdateProgress>
    <asp:UpdatePanel ID="upPanel" runat ="server" UpdateMode="Conditional" RenderMode ="Inline" >
    <ContentTemplate >
       
    <table class="ascxMainTable2" cellpadding="1" cellspacing="0">
        <tr class="trHeader">
            <th class="tdSubHeader" colspan="2">
                General
            </th>
            <th style="text-align:right;width:50%" colspan ="2" >
                
                <asp:Button runat="server" OnClick="btnSave_Click" class="Button" style="width:65px" Text="Save" ID="btnSave"  />
            </th>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </td>
        </tr>
        <tr id="trchatCreatedBy" runat="server">
            <td class="tdHeader" style="width: 12%">
                Created By
            </td>
            <td>
                <telerik:RadTextBox ID="txtchatCreatedBy" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>
            <td class="tdHeader" style="width: 10%">
                Created At
            </td>
            <td>
                <telerik:RadTextBox ID="txtcreatedatCreatedAt" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <hr size="1px" width="99%" style="border: 1px dotted #949494" />
            </td>
        </tr>
        <tr id="tr1">
            <td class="tdHeader">
                Chat Type
            </td>
            <td>
                <telerik:RadComboBox ID="cmbChatType" MaxHeight="320px" Skin="Office2007" runat="server"
                    Width="260px">
                    <CollapseAnimation Duration="200" Type="OutQuint" />
                    <Items>
                    <telerik:RadComboBoxItem Text="External" Value="E" />
                    <telerik:RadComboBoxItem Text="Internal" Value="I" />
                    </Items>
                </telerik:RadComboBox>
                &nbsp;<span class="mandatory">*</span>
            </td>
            <td class="tdHeader">
                Chat SessionID
            </td>
            <td>
                 <telerik:RadTextBox ID="txtChatSessionID" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>
        </tr>
        <tr >
            <td class="tdHeader">
                Chat Status
            </td>
            <td>
                <telerik:RadComboBox ID="cmbChatStatus" MaxHeight="320px" Skin="Office2007" runat="server"
                    Width="260px">
                    <CollapseAnimation Duration="200" Type="OutQuint" />
                    <Items>
                    <telerik:RadComboBoxItem Text="Accepted" Value="Accepted" />
                    <telerik:RadComboBoxItem Text="New" Value="New" />
                    <telerik:RadComboBoxItem Text="Closed" Value="Closed" />
                    </Items>
                </telerik:RadComboBox>
                &nbsp;<span class="mandatory">*</span>
            </td>
            <td class="tdHeader">
                Related To
            </td>
            <td>
                <telerik:RadComboBox ID="cmbRelatedTo" MaxHeight="320px"
                    Skin="Office2007" runat="server" Width="100px">
                    <CollapseAnimation Duration="200" Type="OutQuint" />
                </telerik:RadComboBox>
                &nbsp;
                <asp:TextBox ID="txtRelatedToName" runat="server" Width="157px"
                    MaxLength="50">
                </asp:TextBox>
                <asp:ImageButton runat="server" ImageUrl="~/Images/attach.gif" ID="ImageButton3"
                    Text="..." OnClientClick="javascript:OpenWindow('RELATEDTO');"></asp:ImageButton>
            </td>
        </tr>
       <tr >
            <td class="tdHeader" style="width: 12%">
                Chat Source Name
            </td>
            <td>
                <telerik:RadTextBox ID="txtchatsourcename" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>
            <td class="tdHeader" style="width: 10%">
               Chat Source Phone No
            </td>
            <td>
                <telerik:RadTextBox ID="txtphonenumber" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>
        </tr> 
       <tr >
            <td class="tdHeader" style="width: 12%">
                Chat Visitor Email
            </td>
            <td>
                <telerik:RadTextBox ID="txtvisitoremail" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>
            <td class="tdHeader" style="width: 10%">
              Chat Visitor Ip
            </td>
            <td>
                <telerik:RadTextBox ID="txtvisitorIP" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>
        </tr> 
       
       
       <tr >
            <td class="tdHeader" style="width: 12%">
                Chat Destination Name
            </td>
            <td>
                <telerik:RadTextBox ID="txtdestinationname" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>
            <td class="tdHeader" style="width: 10%">
              Chat End Time
            </td>
            <td>
                <telerik:RadTextBox ID="txtchatendtime" ReadOnly="true" Skin="Office2007" InvalidStyleDuration="100"
                    LabelCssClass="radLabelCss_WebBlue" runat="server" Width="263px">
                </telerik:RadTextBox>
            </td>
        </tr> 
         <tr>
            <td valign="top" class="tdHeader">
              Chat  Visitor Query
            </td>
            <td colspan= "3">
                <telerik:RadTextBox ID="txtvisitorquery" MaxLength="200" Skin="Office2007" InvalidStyleDuration="100"
                    runat="server" Width="870px" TextMode="MultiLine">
                </telerik:RadTextBox>
                <span class="mandatory">*</span>
            </td>
        </tr>
        
         <tr>
            <td valign="top" class="tdHeader">
                Chat Text
            </td>
            <td colspan="3">
                <div id="divchattext" runat="server" style="width:870px;border-color:Gray;border-width:1px;border-style:solid;height:300px;overflow:scroll"></div>
            </td>
        </tr>

    </table>
    <input type="hidden" id="hdnRelatedToID" value="0" runat="server" />
    <input type="hidden" id="hdnOwnerID" value="0" runat="server" />
    <input type="hidden" id="hdnOldOwnerID" value="0" runat="server" />
    <input type="hidden" runat="server" id="hdnServerEvent" value="" />
    <input type="hidden" id="hdnConsultantID" value="0" runat="server" />    

    <asp:ImageButton ID="imgServerEvent" runat="server" Height="1px" Width="1px" ImageUrl="../Images/white.gif" OnClick="imgServerEvent_Click" AlternateText=""/>

    </ContentTemplate>
    </asp:UpdatePanel>
    <telerik:RadWindowManager VisibleStatusbar="false" Width="450" Height="400" DestroyOnClose="true"
    ID="RadWindowManager1" runat="server" Modal="true" Animation="None">
    
    </telerik:RadWindowManager>
    </form>
</body>
</html>
