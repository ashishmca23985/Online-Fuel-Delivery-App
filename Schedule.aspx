<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Schedule.aspx.cs" Inherits="Schedule" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>

<script language="javascript" type="text/javascript">
    var prevCtl = "";
    var currentCtl = "";
    var TimeSlot = "";
    function check(obj) {
        currentCtl = obj.id;
        if (obj.parentElement.className.length == 4) {
            TimeSlot = (obj.parentElement.className);
            TimeSlot = TimeSlot.substr(0, 1);
        }
        else if (obj.parentElement.className.length == 5) {
            TimeSlot = (obj.parentElement.className);
            TimeSlot = TimeSlot.substr(0, 2);
        }
        if (document.getElementById(obj.id).checked) {
            if (prevCtl.length > 0) {
                document.getElementById(prevCtl).checked = false;
            }
            prevCtl = obj.id;
        } else {
            prevCtl = "";
        }
    }

    
    function ValidateGetSchedule() {
        var ctl;

        ctl = $find("<%= cmbSchedularConsultant.ClientID %>");
        if (ctl.get_value() == "") {
            alert("Select Tax Consultant!");
            ctl.get_inputDomElement().focus();
            return false;
        }
        var dateInput = $find('<%= rdDate.ClientID %>');//.get_dateInput();
        var strRemDate = dateInput._initialValue;
        if (dateInput.get_selectedDate == null) {
            alert("Select Date!");
            return false;
        }
        
    }
    
    function ValidateGetScheduleBranchWise() {
        var ctl;

        ctl = $find("<%= cmbBranch.ClientID %>");
        if (ctl.get_value() == "") {
            alert("Select Branch!");
            ctl.get_inputDomElement().focus();
            return false;
        }
        var dateInput = $find('<%= rdDate.ClientID %>');//.get_dateInput();
        var strRemDate = dateInput._initialValue;
        if (dateInput.get_selectedDate == null) {
            alert("Select Date!");
            return false;
        }
        
        
    }

    function ValidateSave() {
        var ctl;
        try {
            ctl = $find("<%= cmbSchedularConsultant.ClientID %>");
            if (ctl.get_value() == "") {
                alert("Select Phlebotomist!");
                ctl.get_inputDomElement().focus();
                return false;
            }
            var dateInput = $find('<%= rdDate.ClientID %>');//.get_dateInput();
            var strRemDate = dateInput._initialValue;
            if (dateInput.get_selectedDate() == null) {
                alert("Select Date!");
                return false;
            }
            if (TimeSlot == "") {
                alert("Select time slot for schedule");
                return false;
            }
        }
        catch (e) {
            alert("Error:- Page -> phleboschedule.aspx -> Method -> 'ValidateSave' \n Description:- \n" + e.description);
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
            alert("Error:- Page -> consultantschedule.ascx -> Method -> \"GetRadWindow\" \n Description:- \n" + e.description);
            return false;
        }
    }
    function CloseWindow(param) {
        try {      
            var oWnd = GetRadWindow();
            var oArg = new Object();

            oArg.Id = $get("<%= hdnConsultantId.ClientID %>").value;
            oArg.Name = $get("<%= hdnConsultantName.ClientID %>").value;
            oArg.ScheduledDate = $get("<%= hdnScheduledDate.ClientID %>").value;
            
            oWnd.argument = oArg;
            oWnd.close();
            
        }
        catch (e) {
            alert("Error:- Page -> consultantschedule.ascx -> Method -> 'CloseWindow' \n Description:- \n" + e.description);
            return false;
        }
    }
</script>

<style type="text/css">
    .YellowGreen
    {
        background-color: YellowGreen;
    }
</style>

<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="smSchedule" runat="server" LoadScriptsBeforeUI="false" ScriptMode="Release">
        <Services>
            <asp:ServiceReference Path="~/Services/ScheduleWS.asmx" />
        </Services>
    </asp:ScriptManager>
    <div>
        <asp:UpdatePanel ID="upScheduler" runat="server">
            <ContentTemplate>
                <table width="100%">
                
                 <tr>
                    
                        <td class="tdLabel">
                            <asp:Label ID="Label3" runat="server"> State</asp:Label>
                        </td>
                        <td class="tdValue">
                            <telerik:RadComboBox ID="cmbStates" runat="server" Width="130px" 
                                DropDownWidth="200" AutoPostBack="true" 
                                onselectedindexchanged="cmbStates_SelectedIndexChanged">
                                <CollapseAnimation Type="None" Duration="1" />
                                <ExpandAnimation Type="None" Duration="1" />
                            </telerik:RadComboBox>                            
                        </td>
                        
                        <td class="tdLabel">
                            <asp:Label ID="Label4" runat="server"> City</asp:Label>
                        </td>
                        <td class="tdValue">
                            <telerik:RadComboBox ID="cmbCity" runat="server" Width="130px" DropDownWidth="200" AutoPostBack="true" 
                                onselectedindexchanged="cmbCity_SelectedIndexChanged">
                                <CollapseAnimation Type="None" Duration="1" />
                                <ExpandAnimation Type="None" Duration="1" />
                            </telerik:RadComboBox>                            
                        </td>
                        <td align="left" class="tdLabel">
                            <asp:Label ID="Label5" runat="server"> Branch</asp:Label>
                        </td>
                        <td class="tdValue">
                           <telerik:RadComboBox ID="cmbBranch" runat="server" Width="130px" DropDownWidth="200" AutoPostBack="true" 
                                onselectedindexchanged="cmbBranch_SelectedIndexChanged">
                                <CollapseAnimation Type="None" Duration="1" />
                                <ExpandAnimation Type="None" Duration="1" />
                            </telerik:RadComboBox>  
                        </td>
                    </tr>
                
                
                    <tr>                    
                        
                        <td class="tdLabel">
                            <asp:Label ID="lblMessage" runat="server"> Tax Consultant(s)</asp:Label>
                        </td>
                        <td class="tdValue">
                            <telerik:RadComboBox ID="cmbSchedularConsultant" runat="server" Width="130px" DropDownWidth="200">
                                <CollapseAnimation Type="None" Duration="1" />
                                <ExpandAnimation Type="None" Duration="1" />
                            </telerik:RadComboBox>                            
                        </td>
                        <td align="left" class="tdLabel">
                            <asp:Label ID="Label2" runat="server">Date</asp:Label>
                        </td>
                        <td class="tdValue">
                            <telerik:RadDatePicker ID="rdDate" runat="server" Width="130px" DateInput-EmptyMessage="Collection Date"
                                MinDate="01/01/2000" MaxDate="01/01/3000" DateInput-DateFormat="dd/MM/yyyy">
                                <Calendar>
                                
                                    <SpecialDays>
                                        <telerik:RadCalendarDay Repeatable="Today">
                                            <ItemStyle BackColor="YellowGreen" />
                                        </telerik:RadCalendarDay>
                                    </SpecialDays>
                                </Calendar>
                            </telerik:RadDatePicker>&nbsp;
                                                       <%-- <asp:Button ID="btnFixedAppointment" runat="server" Text="Fix Schedule" OnClientClick="javascript:CloseWindow();" />--%>
                          
                        </td>
                        
                         <td class="tdLabel">
                                                    

                            <%--<asp:Label ID="Label1" runat="server"> Contact Type</asp:Label>--%>
                        </td>
                        <td class="tdLabel">
                              <asp:Button ID="btnSchedule" Width="90px" runat="server" Text="Get Schedule" OnClientClick="return ValidateGetSchedule();"
                                OnClick="btnSchedule_Click" />&nbsp;
                                <asp:Button ID="btnScheduleBranchwise" Width="90px" runat="server" Text="Branch Wise" OnClientClick="return ValidateGetScheduleBranchWise();"
                                OnClick="btnScheduleBranchwise_Click" />&nbsp;
                          

                            <%--<asp:Label ID="Label1" runat="server"> Contact Type</asp:Label>--%>
                        </td>
                       <%-- <td class="tdValue">
                            <telerik:RadComboBox ID="cmdContactType" runat="server" Width="100px" 
                                DropDownWidth="200" 
                                onselectedindexchanged="cmdContactType_SelectedIndexChanged" AutoPostBack="true">
                                <CollapseAnimation Type="None" Duration="1" />
                                <ExpandAnimation Type="None" Duration="1" />
                            </telerik:RadComboBox>                         
                        </td>--%>
                    </tr>
                    <tr>
                        <td>
                           <asp:Button ID="btnPrevious" runat="server" Text="<<" OnClick="btnPrevious_Click" />&nbsp;
                            <asp:Button ID="btnNext" runat="server" Text=">>" OnClick="btnNext_Click" />                            
                        </td>
                        <td colspan="4">
                        <table style="width: 100%; border: solid 1px gray;">
                                <tr style="font-family: Verdana; color: Black;">
                                    <td style="background-color: PaleTurquoise; width: 15px;">
                                    </td>
                                    <td>
                                        Total Free
                                    </td>
                                    <td style="background-color: LightPink; width: 15px;">
                                    </td>
                                    <td>
                                        Partial Free
                                    </td>
                                    <td style="background-color: LightGray; width: 15px;">
                                    </td>
                                    <td>
                                        Total Busy
                                    </td>
                                                                    
                                </tr>
                            </table>
                        
                        </td>
                        <td><asp:Button ID="btnFixedAppointment" Width="90px" runat="server" Text="Fix Schedule"
                                OnClientClick="return ValidateSave();" OnClick="btnAppointment_Click" />  
                        </td>
                    </tr>
                </table>
                
                <div id="DivSchedule" runat="server" style="display:block;"> 
                
                <telerik:RadGrid ID="rgdSchedule" runat="server" AutoGenerateColumns="false" BorderStyle="Solid"
                    OnItemDataBound="rgdSchedule_ItemDataBound" GridLines="Both">
                    <MasterTableView runat="server" GridLines="Both">
                        <Columns>
                            <%-- <telerik:GridBoundColumn DataField="Slab" UniqueName="Slab" HeaderText="Time Slot"
                        ItemStyle-Width="100px">
                    </telerik:GridBoundColumn>--%>
                            <telerik:GridTemplateColumn HeaderText="Time Slot" ItemStyle-Width="8%">
                                <ItemTemplate>
                                    <asp:Label ID="lblSlab" runat="server" Text='<%# Eval("Slab") %>'></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="00-15" ItemStyle-Width="12%" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkCode" CssClass='<%# Eval("Slab") %>' runat="server" Text='<%# Eval("First") %>'
                                        onclick="check(this);" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="15-30" ItemStyle-Width="12%" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkCode2" CssClass='<%# Eval("Slab") %>' runat="server" Text='<%# Eval("Second") %>'
                                        onclick="check(this);" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="30-45" ItemStyle-Width="12%" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkCode3" CssClass='<%# Eval("Slab") %>' runat="server" Text='<%# Eval("Third") %>'
                                        onclick="check(this);" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="45-59" ItemStyle-Width="12%" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkCode4" CssClass='<%# Eval("Slab") %>' runat="server" Text='<%# Eval("Fourth") %>'
                                        onclick="check(this);" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Scheduler ID" Display="false">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkCode5" runat="server" Text='<%# Eval("ID") %>' onclick="check(this);" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Next1" ItemStyle-Width="11%">
                                <ItemTemplate>
                                    <asp:Label ID="lblNext1" runat="server" Text='<%# Eval("Next1") %>' Visible="false"></asp:Label>
                                    <asp:LinkButton ID="lnkNext1" runat="server" Text='<%# Eval("Next1") %>' CommandArgument="1"
                                        EnableTheming="false" OnClick="NextSchedule"></asp:LinkButton>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Next2" ItemStyle-Width="11%">
                                <ItemTemplate>
                                    <asp:Label ID="lblNext2" runat="server" Text='<%# Eval("Next2") %>' Visible="false"></asp:Label>
                                    <asp:LinkButton ID="lnkNext2" runat="server" Text='<%# Eval("Next2") %>' CommandArgument="2"
                                        EnableTheming="false" OnClick="NextSchedule"></asp:LinkButton>
                                    <%--<asp:Button ID="btnNext2" runat="server" Text="Go" OnClick="btnNextSchedule" CommandArgument="2" EnableTheming="false" />--%>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Next3" ItemStyle-Width="11%">
                                <ItemTemplate>
                                    <asp:Label ID="lblNext3" runat="server" Text='<%# Eval("Next3") %>' Visible="false"></asp:Label>
                                    <asp:LinkButton ID="lnkNext3" runat="server" Text='<%# Eval("Next3") %>' CommandArgument="3"
                                        EnableTheming="false" OnClick="NextSchedule"></asp:LinkButton>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Next4" ItemStyle-Width="11%">
                                <ItemTemplate>
                                    <asp:Label ID="lblNext4" runat="server" Text='<%# Eval("Next4") %>' Visible="false"></asp:Label>
                                    <asp:LinkButton ID="lnkNext4" runat="server" Text='<%# Eval("Next4") %>' CommandArgument="4"
                                        EnableTheming="false" OnClick="NextSchedule"></asp:LinkButton>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Leave" Display="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblLeave" runat="server" Text='<%# Eval("Enabled") %>' Visible="true"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
                
                </div>
                <asp:HiddenField ID="hdnConsultantName" runat="server" Value="" />
                <asp:HiddenField ID="hdnConsultantId" runat="server" Value="" />
                <asp:HiddenField ID="hdnScheduledDate" runat="server" Value="" />
                
                <div id="DivScheduleBranchWise" runat="server" style="display:block;"> 
                
                <telerik:RadGrid ID="rdgDataBranchWise" runat="server" AutoGenerateColumns="false" BorderStyle="Solid"
                     GridLines="Both" Skin="Hay" OnItemDataBound="rdgDataBranchWise_ItemDataBound" >
                    <MasterTableView runat="server" GridLines="Both">
                        <Columns>
                            <telerik:GridTemplateColumn HeaderText="Time Slot" ItemStyle-Width="8%" ItemStyle-BackColor="Gray">
                                <ItemTemplate>                                   
                                    <asp:Label ID="lblSchdHour" runat="server" ForeColor="Green" Font-Bold="true" Text='<%# Eval("schd_hour") %>'></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="00-15" ItemStyle-Width="12%" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                <telerik:RadToolTip ID="RadToolTip1" runat="server" Width="200" TargetControlID="lblSchdStatus1" Position="TopRight" Skin="Hay" Animation="Resize">
                                        <%# Eval("schd_status_tooltip1")%>                    
                                    </telerik:RadToolTip>
                                    <asp:Label ID="lblSchdStatus1" runat="server" Height="15px" Text='<%# Eval("schd_status1") %>'></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="15-30" ItemStyle-Width="12%" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                 <telerik:RadToolTip ID="RadToolTip2" runat="server" Width="200" TargetControlID="lblSchdStatus2" Position="TopRight" Skin="Hay" Animation="Resize">
                                        <%# Eval("schd_status_tooltip2")%>                    
                                    </telerik:RadToolTip>
                                    <asp:Label ID="lblSchdStatus2" runat="server" Text='<%# Eval("schd_status2") %>'></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="30-45" ItemStyle-Width="12%" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                 <telerik:RadToolTip ID="RadToolTip3" runat="server" Width="200" TargetControlID="lblSchdStatus3" Position="TopRight" Skin="Hay" Animation="Resize">
                                        <%# Eval("schd_status_tooltip3")%>                    
                                    </telerik:RadToolTip>
                                    <asp:Label ID="lblSchdStatus3" runat="server" Text='<%# Eval("schd_status3") %>'></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="45-59" ItemStyle-Width="12%" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                 <telerik:RadToolTip ID="RadToolTip4" runat="server" Width="200" TargetControlID="lblSchdStatus4" Position="TopRight" Skin="Hay" Animation="Resize">
                                        <%# Eval("schd_status_tooltip4")%>                    
                                    </telerik:RadToolTip>
                                <asp:Label ID="lblSchdStatus4" runat="server" Text='<%# Eval("schd_status4") %>'></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            
                </div>
                
                </ContentTemplate>
        </asp:UpdatePanel>
        <telerik:RadWindowManager ID="rdWindow" runat="server" Modal="true">
        </telerik:RadWindowManager>
        
    </div>
    </form>
</body>
</html>
