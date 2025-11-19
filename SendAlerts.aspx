<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" ValidateRequest="false" CodeFile="SendAlerts.aspx.cs" Inherits="SendAlerts" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Send Notification</title>
    <style type="text/css">
        #UpdateProgress1
        {
            color: #fff;
            top: 200px;
            right: 500px;
            position: fixed;
        }
        #UpdateProgress1 img
        {
            vertical-align: middle;
            margin: 2px;
        }
    </style>
</head>
<script type="text/javascript" language="javascript">
    function OpenWindow(param) 
        {
           try {
               var searchText = $get("txtSendTo").value;
               var objectType = 'QUE';
               var objectID = $get("hdnOwnerID").value;

               var url = 'LookUp.aspx?objecttype=' + objectType + '&SearchText=' + searchText;
               var v = radopen(url);
               v.set_modal(true);
               v.moveTo(250,60)
               //v.center();
               v.setSize(900, 500); //height, width
               v.add_close(onWindowClose);
               //v.moveTo(0, 0);
               //v.add_dragStart(OnClientDragStart);
               //v.add_dragEnd(OnClientDragEnd);
               return false;
           }
           catch (e) {
               alert("Error:- Page -> Send Alerts.aspx -> Method -> 'OpenWindow' \n Description:- \n" + e.description);
               return false;
           }
       }
       function onWindowClose(oWnd) {
           try {
                   if (oWnd.argument != null) {
                   var ownerID = $get("hdnOwnerID").value;
                   if(ownerID != "0")
                        ownerID += ",";
                   ownerID += oWnd.argument.Id;
                   $get("hdnOwnerID").value = ownerID;
                   $get("hdnNewOwnerID").value = oWnd.argument.Id;
                   $get("imgServerEvent").click();
               }
               
               return false;
           }
           catch (e) {
               alert("Error :- Page:- \"CaseGeneral.aspx\" Method:- 'onWindowClose' Description:- " + e.description);
               return false;
           }
       }
</script>
<body>
    <form id="frmTemplate" runat="server">
    <asp:ScriptManager ID="scriptmanager1" runat="server">
        <Services>
            <asp:ServiceReference Path="~/Services/GlobalWS.asmx" />
        </Services>
    </asp:ScriptManager>
    <div id="divLoading" style="display: none;" class="progressPopup">
        <span id="spanMessage"></span>
        <br />
        <br />
        <div id="imgProgress" class="progressImage">
        </div>
    </div>

    <AjaxControls:ModalUpdateProgress ID="ModalUpdateProgress1" runat="server" DisplayAfter="0" BackgroundCssClass="modalBackground">
        <ProgressTemplate >
        </ProgressTemplate>
    </AjaxControls:ModalUpdateProgress>
   <%-- <asp:UpdatePanel ID="upPanel" runat ="server" UpdateMode="Conditional" RenderMode ="Inline">
    <ContentTemplate >--%>
            <table width="100%" cellpadding="0" cellspacing="0">
                <tr class="trHeader">
                    <th class="tdSubHeader" colspan="3">
                        Notification
                    </th>
                    <th style="text-align: right;" colspan="3">
                        <asp:Button ID="btnSend" runat="server" Text="Send" CssClass="Button" Width="65px"
                            OnClick="btnSend_Click" />
                    </th>
                </tr>
                <tr>
                    <td colspan="6">
                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <fieldset>
                            <legend id="lgdCriteria" runat="server" class="legendStyle">Template Criteria</legend>
                            <table width="100%" cellpadding="1">
                                <tr>
                                    <td class="tdHeader">
                                        Templates
                                    </td>
                                    <td>
                                        <telerik:RadComboBox ID="cmbTemplates" runat="server" Width="150px" Skin="Office2007">
                                            <CollapseAnimation Duration="200" Type="OutQuint" />                                            
                                        </telerik:RadComboBox>
                                        &nbsp;<span class="mandatory">*</span>
                                    </td>
                                    
                                    <td class="tdHeader">
                                        <asp:Button ID="btn_Go" runat="server" Text="Go" onclick="btn_Go_Click" />                                                                                
                                    </td>
                                    <td style="width:60%">
                                    &nbsp;
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                        <table width="100%">
                            <tr>
                                 <td colspan="6" width="50%">
                                 <br />                                
                                 </td>
                            </tr>
                            <tr>
                                <td>Notification Time : &nbsp;&nbsp; &nbsp; &nbsp;                                                             
                                 
                                    <telerik:RadDateTimePicker ID="dtNotificationTime" DateInput-DateFormat="MMM dd yyyy hh:mm tt"
                                            runat="server" Skin="WebBlue" Width="200px">
                                            <Calendar ID="Calendar1" runat="server">
                                        <FooterTemplate>
                                            <table width="100%" bgcolor="#CAE4FF" cellpadding="1">
                                                <tr>
                                                    <td align="center">
                                                        <a href="#" onclick="SelectToday('<%=dtNotificationTime.ClientID %>');">Today</a> &nbsp;|&nbsp;<a
                                                            href="#" onclick="ClearDate('<%=dtNotificationTime.ClientID %>');">Clear</a>
                                                    </td>
                                                </tr>
                                            </table>
                                        </FooterTemplate>
                                        <SpecialDays>
                                            <telerik:RadCalendarDay Repeatable="Today" >
                                                <ItemStyle BackColor="#FFCC66" ForeColor="Blue" />
                                            </telerik:RadCalendarDay>
                                        </SpecialDays>
                                    </Calendar>
                                    <TimePopupButton />
                                    <TimeView ID="TimeView1" runat="server" Skin="WebBlue">
                                    </TimeView>
                                    <DateInput ID="DateInput1" runat="server" Skin="WebBlue">
                                    </DateInput>
                                    <DatePopupButton />
                                </telerik:RadDateTimePicker>                                                        
                                 </td>
                                 <td>Send Notification As :                                  
                                 </td>
                                 <td colspan="4">           
                                 <asp:CheckBox ID="chkEmail" runat="server" Text="Email" /> &nbsp;&nbsp;&nbsp;                                                    
                                 <asp:CheckBox ID="chkSMS" runat="server" Text="SMS" /> &nbsp;&nbsp;&nbsp;
                                 <asp:CheckBox ID="chkAlert" runat="server" Text="Alert" />
                                 </td>
                            </tr>
                            <tr>
                                <td colspan="6" width="50%">                                
                                    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1"
                                        SelectedIndex="0">
                                        <Tabs>
                                            <telerik:RadTab Text="EMAIL">
                                            </telerik:RadTab>
                                            <telerik:RadTab Text="SMS">
                                            </telerik:RadTab>
                                            <telerik:RadTab Text="ALERTS">
                                            </telerik:RadTab>
                                        </Tabs>
                                    </telerik:RadTabStrip>
                                    <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0">
                                        <telerik:RadPageView ID="RadPageView1" runat="server">
                                        <br />
                                            <fieldset>
                                                <legend id="Legend1" runat="server" class="legendStyle">Template For EMAIL</legend>
                                                <table width="100%" cellpadding="1">
                                                    <tr>
                                                        <td align="right" class="tdHeader" style="width:7%;">
                                                            To
                                                        </td>
                                                        <td>
                                                            <telerik:RadTextBox ID="txtEmailTo" runat="server" InvalidStyleDuration="100" MaxLength="1000"
                                                                LabelCssClass="radLabelCss_WebBlue" Width="50%">
                                                            </telerik:RadTextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" class="tdHeader" style="width:7%;">
                                                            CC
                                                        </td>
                                                        <td>
                                                            <telerik:RadTextBox ID="txtEmailCC" runat="server" InvalidStyleDuration="100" MaxLength="1000"
                                                                LabelCssClass="radLabelCss_WebBlue" Width="50%">
                                                            </telerik:RadTextBox>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" class="tdHeader" style="width:7%;">
                                                            Bcc
                                                        </td>
                                                        <td>
                                                            <telerik:RadTextBox ID="txtEmailBcc" runat="server" InvalidStyleDuration="100" MaxLength="1000"
                                                                LabelCssClass="radLabelCss_WebBlue" Width="50%">
                                                            </telerik:RadTextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" class="tdHeader" style="width:7%;">
                                                            Subject
                                                        </td>
                                                        <td>
                                                            <telerik:RadTextBox ID="txtEmailSubject" runat="server" InvalidStyleDuration="100"
                                                                MaxLength="500" LabelCssClass="radLabelCss_WebBlue" Width="50%">
                                                            </telerik:RadTextBox>
                                                        </td>
                                                    </tr>
                                                     <tr>
                                                        <td align="right" class="tdHeader" style="width:7%;">
                                                            Attachment
                                                        </td>
                                                        <td>
                                                             <telerik:RadUpload ID="RadUpload_attachment" runat="server" OverwriteExistingFiles="true">
                                                                </telerik:RadUpload>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" class="tdHeader" valign="top" style="width:7%;">
                                                            Body
                                                        </td>
                                                        <td>
                                                        <telerik:RadEditor ID="txtEmailBody" runat="server" Height="180px" EditModes="Design"
                                                                Width="100%" AutoResizeHeight="false" Scrolling="Y" BorderStyle="None" ToolsFile="Scripts/BasicTools.xml">
                                                                 <Tools>
                                                                        <telerik:EditorToolGroup Tag="FileManagers">
                                                                            <telerik:EditorTool Name="ImageManager" /> 
                                                                            <telerik:EditorTool Name="TemplateManager" />                                     
                                                                        </telerik:EditorToolGroup> 
                                                                 </Tools>
                                                                <Content>
                                                                </Content>

                                                                <ImageManager ViewPaths="~/EmailDocument/Images" />
                                                                <TemplateManager ViewPaths="~/EmailDocument/Template" />
                                                                
                                                               
                                                            </telerik:RadEditor>
                                                           <%-- <telerik:RadTextBox ID="txtEmailBody" runat="server" InvalidStyleDuration="100" LabelCssClass="radLabelCss_WebBlue" Width="100%" Height="150px">
                                                            </telerik:RadTextBox>
                                                            --%>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="RadPageView2" runat="server">
                                        <br />
                                            <fieldset>
                                                <legend id="Legend2" runat="server" class="legendStyle">Template For SMS</legend>
                                                <table width="100%" cellpadding="0" style="height: 230px">
                                                    <tr>
                                                        <td align="right" class="tdHeader" style="width: 10%;">
                                                            Contact Number
                                                        </td>
                                                        <td>
                                                            <telerik:RadTextBox ID="txtSMSTo" runat="server" InvalidStyleDuration="100" MaxLength="500"
                                                                LabelCssClass="radLabelCss_WebBlue" Width="50%">
                                                            </telerik:RadTextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" class="tdHeader" valign="top" style="width: 10%;">
                                                            Body
                                                        </td>
                                                        <td>
                                                            <telerik:RadTextBox ID="txtSMSBody" runat="server" InvalidStyleDuration="100" MaxLength="500"
                                                                TextMode="MultiLine" Height="200px" LabelCssClass="radLabelCss_WebBlue" Width="50%">
                                                            </telerik:RadTextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </fieldset>
                                        </telerik:RadPageView>
                                        <telerik:RadPageView ID="RadPageView3" runat="server">
                                        <br />
                                            <fieldset>
                                                <legend id="Legend3" runat="server" class="legendStyle">Template For ALERTS</legend>
                                                <table width="100%" cellpadding="0" style="height: 230px">
                                                    <tr>
                                                        <td class="tdHeader">
                                                            Send To
                                                        </td>
                                                        <td colspan="3">
                                                        <asp:UpdatePanel ID="upPanel" runat ="server" UpdateMode="Conditional" RenderMode ="Inline">
                                                        <ContentTemplate >
                                                            <asp:TextBox ID="txtSendTo" MaxLength="100" runat="server" CssClass="textbox"
                                                                Width="45%">
                                                            </asp:TextBox>
                                                            <asp:ImageButton runat="server" ImageUrl="~/Images/attach.gif" ID="imgAttachOwner"
                                                                Text="..." OnClientClick="javascript:OpenWindow('OWNER');"></asp:ImageButton>
                                                        </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" class="tdHeader" style="width:7%;">
                                                            Subject
                                                        </td>
                                                        <td>
                                                            <telerik:RadTextBox ID="txtPopupSubject" runat="server" InvalidStyleDuration="100"
                                                                MaxLength="500" LabelCssClass="radLabelCss_WebBlue" Width="50%">
                                                            </telerik:RadTextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td align="right" class="tdHeader" valign="top" style="width:7%;">
                                                            Text
                                                        </td>
                                                        <td>
                                                            <telerik:RadTextBox ID="txtPopupText" runat="server" InvalidStyleDuration="100" MaxLength="500"
                                                                TextMode="MultiLine" Height="200px" LabelCssClass="radLabelCss_WebBlue" Width="50%">
                                                            </telerik:RadTextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </fieldset>
                                        </telerik:RadPageView>
                                    </telerik:RadMultiPage>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            
            <input type="hidden" runat="server" id="hdnOwnerID" value="0" />
            <input type="hidden" runat="server" id="hdnNewOwnerID" value="0" />
            <input type="hidden" runat="server" id="hdnServerEvent" value="" />
            <asp:ImageButton ID="imgServerEvent" runat="server" Height="1px" Width="1px" ImageUrl="../Images/white.gif" OnClick="imgServerEvent_Click" AlternateText=""/>
        <%-- </ContentTemplate>
    </asp:UpdatePanel>--%>
            <telerik:RadWindowManager VisibleStatusbar="false" Width="450" Height="400" DestroyOnClose="true"
                    ID="RadWindowManager1" runat="server" Modal="true" Animation="None">
            </telerik:RadWindowManager>
    </form>
</body>
</html>
