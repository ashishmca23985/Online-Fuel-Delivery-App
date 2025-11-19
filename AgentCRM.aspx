<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AgentCRM.aspx.cs" Inherits="AgentCRM" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <link href="css/style.css" rel="stylesheet" type="text/css" />
   
    <script src="Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="Scripts/datetimepicker.js?v=1.1" type="text/javascript"></script>
     <%--<script src="Scripts/crm.js?v=1.1" type="text/javascript"></script>--%>
    
</head>
<body runat="server" id="crmbody">
    <form id="crmform" runat="server" action="">
        <asp:ScriptManager ID="sm" runat="server" EnablePageMethods="true">
            <Services>
                <asp:ServiceReference Path="~/Services/CtiWS.asmx" />
            </Services>
        </asp:ScriptManager>
       
        <script type="text/javascript" language="javascript">

            var width = 100;
            function changeWidth(sender) {
                width = width + 20;
                sender.style.width = width;
            }

        </script>

        <div id="menu-wrapper">
          
        </div>

         <div  class ="trHeader">
                                 <h4 runat="server" id="crmheader" align="center"></h4>

             <asp:Label ID="lbltest" Text="fwgwqe asas "
    Width="40px"  Style="word-wrap: normal; word-break: break-all;"/>

                            </div>
    
        <div id="wrapper">
             <asp:Label ID="lblmessage" runat="server" Text=""></asp:Label>
            <div id="page" >
               
                        <div class='div.scroll' >
                          <%--  <table cellspacing="5px" style="background-color: #999999" >
                                <tr>
                                    <asp:Label ID="lblmessage" runat="server" Text=""></asp:Label>
                                    <td>
                                        <span>Disposition Category</span>
                                    </td>
                                    <td>
                                        <select name="crmDispcateg" id="crmDispcateg" runat="server" style="font-family: 'Segoe UI'; font-size: 11px;">
                                        </select>
                                    </td>
                                    <td>
                                        <span>Disposition</span>
                                    </td>
                                    <td>
                                        <select name="crmDisposition" id="crmDisposition" runat="server" style="font-family: 'Segoe UI'; font-size: 11px;">
                                        </select>
                                    </td>
                                    <td>
                                        <input type="button" id="btnSave" runat="server" value="SAVE" onclick="javascript: if (validatecrm() == false) return false; savecrm();" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span>CallbackTime</span>
                                    </td>
                                    <td>
                                        <select name="crmcallback" id="crmcallback" runat="server" style="font-family: 'Segoe UI'; font-size: 11px; display: none; width: 150px">
                                        </select>

                                        <div id="imgdate">

                                            <input type="text" name="callbackdatetime" id="callbackdatetime" runat="server" />
                                            <span style="display: none" id="imgVis" runat="server"><a href="javascript:NewCal('callbackdatetime','ddmmmyyyy',true,24)" id="my_link">
                                                <img src="images/cal.gif" width='16' height='16' border='0' alt='Pick a date' id="Img" runat="server" />
                                            </a>
                                            </span>
                                        </div>
                                    </td>
                                    <td>
                                        <span>Comments</span>
                                    </td>
                                    <td colspan="2">
                                        <textarea name="crmcomments" id="crmcomments" rows="1" cols="30" runat="server"></textarea>
                                    </td>
                                    <tr>

                                        <td>
                                            <span>Alternate No</span>
                                        </td>
                                        <td colspan="2">

                                            <input type="text" id="setcallback" runat="server" disabled="disabled" onkeypress="javascript:return IsNumeric(event);" />
                                            <input type="checkbox" id="crmsetcallback" name="Set Self Callback" onclick="enable_text()" runat="server" />Set Callback
                             
                                        </td>
                                        <td>
                                        
                                        </td>

                                    </tr>
                                </tr>
                            </table>--%>
                            
                            <div id="crmcontent" runat="server">
                            </div>
                            <iframe id="crmframe" style="display: none;"></iframe>
               
                            <br />
                             <div class="trHeader">
                                <h4 runat="server" id="H1" align="center">Call History </h4>
                            </div>

                            <div>

                                <telerik:RadGrid ID="rdCtiHistory" runat="server" AutoGenerateColumns="false"
                                    PageSize="20" CellSpacing="0" ShowFooter="true" AllowPaging="True" Width="100%"
                                    ShowHeader="true" GridLines="Both" OnNeedDataSource="rdCtiHistory_NeedDataSource" Height="400px">
                                    <MasterTableView ShowHeadersWhenNoRecords="true">
                                        <Columns>
                                            <telerik:GridBoundColumn SortExpression="call_number" HeaderText="Call Number" DataField="call_numver">
                                                 <HeaderStyle  Width="09%" />
                                            </telerik:GridBoundColumn>

                                            <telerik:GridBoundColumn SortExpression="Call_Cli" HeaderText="Call CLI" DataField="Call_Cli">
                                                 <HeaderStyle  Width="09%" />
                                            </telerik:GridBoundColumn>

                                            <telerik:GridBoundColumn SortExpression="call_start_time" HeaderText="Call Start Time"
                                                DataField="call_start_time">
                                                 <HeaderStyle Width="15%" />
                                            </telerik:GridBoundColumn>

                                            <telerik:GridBoundColumn SortExpression="call_next_dial_time" HeaderText="Call Next Dial time"
                                                DataField="call_next_dial_time">
                                                 <HeaderStyle  Width="15%" />
                                            </telerik:GridBoundColumn>


                                            <telerik:GridBoundColumn SortExpression="call_agent_name" HeaderText="Call Agent Name"
                                                DataField="call_agent_name">
                                                 <HeaderStyle  Width="13%" />
                                            </telerik:GridBoundColumn>

                                              <telerik:GridBoundColumn SortExpression="call_end_type" HeaderText="Disposition"
                                                DataField="call_end_type">
                                                 <HeaderStyle  Width="10%" />
                                            </telerik:GridBoundColumn>

                                            <telerik:GridBoundColumn SortExpression="call_remark" HeaderText="Call Remark" DataField="call_remark">
                                                 <HeaderStyle Width="40%" />
                                            </telerik:GridBoundColumn>

                                          

                                            <%--     <telerik:GridBoundColumn SortExpression="call_user_parameter3" HeaderText="Patient Name"
                        DataField="call_user_parameter3">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="call_user_parameter4" HeaderText="Interaction Type"
                        DataField="call_user_parameter4">
                    </telerik:GridBoundColumn>--%>

                                            <%--  <telerik:GridBoundColumn SortExpression="call_module" HeaderText="Module Name" DataField="call_module">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn SortExpression="call_type" HeaderText="Call Type" DataField="call_type">
                    </telerik:GridBoundColumn>--%>
                                        </Columns>
                                    </MasterTableView>
                                    <ClientSettings>
                                        <Scrolling AllowScroll="True" SaveScrollPosition="True"></Scrolling>
                                    </ClientSettings>
                                </telerik:RadGrid>


                            </div>
                        </div>

                    </div>
                

        </div>
        <asp:HiddenField ID="hdnDNI" runat="server" />
        <asp:HiddenField ID="hdnHostId" runat="server" />
        <asp:HiddenField ID="HiddenFieldCallType" runat="server" />
        <asp:HiddenField ID="hdfldLED" runat="server" />
        <asp:HiddenField ID="hdCallmaster" runat="server" />
          <asp:HiddenField ID="hddisposition" runat="server" />
        <asp:HiddenField ID="hdCallbackTime" runat="server" />
        <asp:HiddenField ID="hdRemarks" runat="server" />
         <asp:HiddenField ID="hdnCallNumber" runat="server" />
          
        <%-- <telerik:RadWindowManager ID="rdWindow" runat="server" Modal="true">
        </telerik:RadWindowManager>--%>
    </form>
    
    <script language="javascript" type="text/javascript">
        document.getElementById('hdnHostId').value = '<% = Session["HOSTID"] %>';
        document.getElementById('hdnCallNumber').value = '<% = Session["CallNumber"] %>'; 
    </script>

</body>
<script src="Scripts/crm.js?v=1.3" type="text/javascript"></script>
</html>
