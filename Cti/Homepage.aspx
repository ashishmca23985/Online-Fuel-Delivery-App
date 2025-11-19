<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Homepage.aspx.cs" Inherits="Master_Homepage" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <title>Untitled Page</title>
    <link href="CSS/BtnStyle.css" rel="stylesheet" type="text/css" />
    <link href="../CSS/cssTExt.css" rel="stylesheet" type="text/css" />
    <link href="CSS/StyleSheet.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .clock
        {
            vertical-align: middle;
            font-family: Arial, Sans-Serif;
            font-size: 13px;
            font-weight: normal;
            color: #000;
        }

        .clocklg
        {
            vertical-align: middle;
            font-family: Arial, Sans-Serif;
            font-size: 10px;
            font-weight: normal;
            color: #555;
        }
        .RadCalendarPopup
        {
            left:0px !important;
        }
        
    </style>
    <style type="text/css">
        legend.green-color
        {
            color: black;
            text-decoration: none;
        }

        a:link
        {
            color: #0000FF;
        }
        /* unvisited link */ a:visited
        {
            color: #006666;
        }
        /* visited link */ a:hover
        {
            color: #00CC99;
        }
        /* mouse over link */ a:active
        {
            color: #000033;
        }
        /* selected link */
    </style>
</head>
<%--<body background="../Images/CTIpanelImage.jpg">--%>
<body style="">

    <script>
        // OUR FUNCTION WHICH IS EXECUTED ON LOAD OF THE PAGE.
        function digitized() {
            var dt = new Date();    // DATE() CONSTRUCTOR FOR CURRENT SYSTEM DATE AND TIME.
            var hrs = dt.getHours();
            var min = dt.getMinutes();
            var sec = dt.getSeconds();
            min = Ticking(min);
            sec = Ticking(sec);

            document.getElementById('dc').innerHTML = hrs + ":" + min;
            document.getElementById('dc_second').innerHTML = sec;
            if (hrs > 12) { document.getElementById('dc_hour').innerHTML = 'PM'; }
            else { document.getElementById('dc_hour').innerHTML = 'AM'; }

            var time

            // THE ALL IMPORTANT PRE DEFINED JAVASCRIPT METHOD.
            time = setTimeout('digitized()', 1000);
        }

        function Ticking(ticVal) {
            if (ticVal < 10) {
                ticVal = "0" + ticVal;
            }
            return ticVal;
        }
    </script>

    <script language="javascript" type="text/javascript">

        var gServiceId = -1;
        var gModule = "";
        var gCtiMessage = "";
        var gPStatus = "";
        var gStatusCode = 0;
        var nConfirmed = 0;
        var gCallNumber = 0;

        function pageLoad(sender, args) {
            if (args._isPartialLoad == false) {
                var timePicker = $find("<%=RadCallbackTime.ClientID %>");
                if (timePicker != null) {
                    setMinDate('<%=DateTime.Now%>');
                    timePicker.set_enabled(false);
                }
                
                window.setInterval("GetMessage()", 3000);
                //                window.parent.RAD_SPLITTER_PANE_EXT_CONTENT_rdpRightPane.location.href = "AgentHomePage.aspx";
            }
        }

        function SetAgentLogout() {
            //session error prob check
             window.parent.location = '../Logout.aspx';
            //window.parent.LogoutFrame();
        }
        function SetAgentStatusIdle(Agent) {
            try {

                var radToolTip = null;
                radToolTip = $find("<%= RadToolTip1.ClientID %>");
                // radToolTip.set_targetControl(obj);
                radToolTip.set_text("To go on break.");
                //document.getElementById("btnUnavailableD2").value = "Unavailable";
                document.getElementById("btnUnavailableD2").src = "../Images/Break.png";
                document.getElementById("tdAgentStatusValue").style.backgroundColor = "White";
                /* For Status Shown Globally*/
                window.parent.document.getElementById("lblAgentStatusValue").style.backgroundColor = "White";
                var b = document.getElementById("btnEndCall");
                // b.disabled = true;
                nConfirmed = 0;
                var timePicker = $find("<%=RadCallbackTime.ClientID %>");
                if (timePicker != null) {
                    timePicker.set_enabled(false);
                }
                ClearFields();
            }
            catch (e) {
                alert("File - Homepage.aspx\r\nMethod - SetAgentStatusIdle\n" + e.description);
            }
        }
        function SetAgentStatusBusy(Agent) {
            try {
                //document.getElementById("btnUnavailableD2").value = "Resume";

                var radToolTip = null;
                radToolTip = $find("<%= RadToolTip1.ClientID %>");
                                    // radToolTip.set_targetControl(obj);
                                    radToolTip.set_text("Begin again or continue after a pause or interruption.");
                                    document.getElementById("btnUnavailableD2").src = "../Images/Resume.png";
                                    document.getElementById("tdAgentStatusValue").style.backgroundColor = "Red";
                                    /* For Status Shown Globally*/
                                    window.parent.document.getElementById("lblAgentStatusValue").style.backgroundColor = "Red";
                                    var b = document.getElementById("btnEndCall");
                                    //b.disabled = true;
                                    nConfirmed = 0;
                                }
                                catch (e) {
                                    alert("File - Homepage.aspx\r\nMethod - SetAgentStatusBusy\n" + e.description);
                                }
                            }

                            function onSuccessGetCLI(result) {
                                window.parent.OpenNuanceRequest(result, 1, 'Nuance');
                            }
                            /////karan shah
                            function SetAgentStatusIncall(Agent) {
                                try {
                                    //document.getElementById("btnUnavailableD2").value = "Unavailable";

                                    var radToolTip = null;
                                    radToolTip = $find("<%= RadToolTip1.ClientID %>");
                                // radToolTip.set_targetControl(obj);
                                radToolTip.set_text("To go on break.");
                                document.getElementById("btnUnavailableD2").src = '../Images/Break.png';
                                document.getElementById("tdAgentStatusValue").style.backgroundColor = "Yellow";
                                /* For Status Shown Globally*/
                                window.parent.document.getElementById("lblAgentStatusValue").style.backgroundColor = "Yellow";
                                var b = document.getElementById("btnEndCall");
                                //b.disabled = true;
                                var strCallType = Agent.ACallType;

                                if (Agent.ACallNumber != gCallNumber) {
                                    if (Agent.AAddon == "") {
                                        if (strCallType == "C") {
                                            strCallbackText = "(Callback Call)";

                                            //  var m1 = "<html>You will never forget <b>" + strCallbackText + "</b></html>";

                                            // var clr = 'Yellow';
                                            // var html = '<font color="' + clr + '">' + strCallbackText + ' </font>';

                                            var bConfirmed = confirm("There is a new call from number: " + Agent.ACli + "." + strCallbackText + " Do you want to open window?");


                                        }
                                        else {
                                            if (Agent.AModule == "1")
                                                var bConfirmed = confirm("There is a new call from number: " + Agent.ACli + "  and Language: English. Do you want to open window?");
                                            else if (Agent.AModule == "2")
                                                var bConfirmed = confirm("There is a new call from number: " + Agent.ACli + "  and Language: Bengali. Do you want to open window?");
                                            else if (Agent.AModule == "3")
                                                var bConfirmed = confirm("There is a new call from number: " + Agent.ACli + "  and Language: Hindi. Do you want to open window?");
                                            else
                                                var bConfirmed = confirm("There is a new call from number: " + Agent.ACli + ". Do you want to open window?");

                                        }
                                        nConfirmed = 1;
                                        if (bConfirmed == true) {
                                            //karan shah      
                                            // window.parent.OpenItem(Agent.AUrl, "Patient Details"); 
                                            window.parent.parseQueryString(Agent.AUrl);
                                            window.parent.OpenItem(Agent.AUrl, Agent.ACli);
                                        }
                                        //                        else {
                                        //                            PageMethods.CloseCall("MDC", "", "", "", fncCloseCallSuccess, fncCloseCallFailure);
                                        //                        }
                                    }
                                    else {
                                        alert("There is a new call for number: " + Agent.ACli);
                                        window.parent.parseQueryString(Agent.AUrl);
                                        window.parent.OpenItem(Agent.AUrl, Agent.ACli);


                                    }

                                    var szDialPrefix = '<%=szDialPrefix %>';
                                        if (Agent.ADialable == 1) {
                                            var szdialnumber = szDialPrefix + Agent.ACli;
                                            var command = "PageMethods.Dial('" + szdialnumber + "','" + Agent.ACallNumber + "');";
                                            if (Agent.APreviewTime == "0")
                                                Agent.APreviewTime = 1;
                                            setTimeout(command, Agent.APreviewTime * 1000);
                                        }
                                    }

                                    FillDispositions(Agent.AServiceId, Agent.AModule);
                                    if (Agent.AServiceId != gServiceId) {

                                        gServiceId = Agent.AServiceId;
                                        gModule = Agent.AModule;
                                    }
                                    ClearFields();
                                }
                                catch (e) {
                                    alert("File - Homepage.aspx\r\nMethod - SetAgentStatusIncall\n" + e.description);
                                }
                            }
                            function SetAgentStatusReserve(Agent) {
                                try {

                                    var radToolTip = null;
                                    radToolTip = $find("<%= RadToolTip1.ClientID %>");
                                    // radToolTip.set_targetControl(obj);
                                    radToolTip.set_text("To go on break.");
                                    ///document.getElementById("btnUnavailableD2").value = "Unavailable";
                                    document.getElementById("btnUnavailableD2").src = "../Images/Break.png";
                                    document.getElementById("tdAgentStatusValue").style.backgroundColor = "Pink";
                                    /* For Status Shown Globally*/
                                    window.parent.document.getElementById("lblAgentStatusValue").style.backgroundColor = "Pink";
                                    var b = document.getElementById("btnEndCall");
                                    // b.disabled = true;
                                    nConfirmed = 0;
                                }
                                catch (e) {
                                    alert("File - Homepage.aspx\r\nMethod - SetAgentStatusBusy\n" + e.description);
                                }
                            }

                            function SetAgentStatusWrapup(Agent) {
                                try {

                                    var radToolTip = null;
                                    radToolTip = $find("<%= RadToolTip1.ClientID %>");
                                    // radToolTip.set_targetControl(obj);
                                    radToolTip.set_text("To go on break.");
                                    //document.getElementById("btnUnavailableD2").value = "Unavailable";
                                    document.getElementById("btnUnavailableD2").src = "../Images/Break.png";
                                    document.getElementById("tdAgentStatusValue").style.backgroundColor = "Green";
                                    /* For Status Shown Globally*/
                                    window.parent.document.getElementById("lblAgentStatusValue").style.backgroundColor = "Green";
                                    var b = document.getElementById("btnEndCall");
                                    b.disabled = false;
                                    if (gStatusCode == 0) {
                                        window.parent.OpenItem(Agent.AUrl, Agent.ACli);
                                        //window.parent.rdpRightPane.location.href = Agent.AUrl;
                                    }
                                    if (Agent.AServiceId != gServiceId) {
                                        FillDispositions(Agent.AServiceId, Agent.AModule);
                                        gServiceId = Agent.AServiceId;
                                        gModule = Agent.AModule;
                                    }
                                }
                                catch (e) {
                                    alert("File - Homepage.aspx\r\nMethod - SetAgentStatusWrapup\n" + e.description);
                                }
                            }

                            function SetCtiMessage(Agent) {
                                try {
                                    /* For Status Shown Globally*/
                                    if (window.parent.document.getElementById("lblAgentStatusValue") != null)
                                        window.parent.document.getElementById("lblAgentStatusValue").innerHTML = Agent.AStatus;
                                    document.getElementById("tdAgentStatusValue").innerHTML = Agent.AStatus;
                                    document.getElementById("tdCallANI").innerHTML = GenerateANI(Agent);
                                    document.getElementById("tdCallNumber").innerHTML = Agent.ACallNumber;
                                    document.getElementById("tdCallService").innerHTML = Agent.AServiceName;
                                    document.getElementById("tdCallModule").innerHTML = Agent.AModule;
                                    document.getElementById("tdCallType").innerHTML = Agent.ACallType;
                                    document.getElementById("tdCallLeadId").innerHTML = Agent.ACallLeadId;
                                    document.getElementById("tdCallDni").innerHTML = Agent.ADni;
                                    document.getElementById("tdCallCity").innerHTML = Agent.ACity;
                                    document.getElementById("tdCallState").innerHTML = Agent.AState;
                                    document.getElementById("tdCallRegion").innerHTML = Agent.ARegion;
                                    document.getElementById("tdCallAddon").innerHTML = Agent.AAddon;
                                    document.getElementById("tdCallDialable").innerHTML = Agent.ADialable;
                                    document.getElementById("tdCallPreviewTime").innerHTML = Agent.APreviewTime;
                                    document.getElementById("spancli").innerHTML = Agent.ACli;
                                }
                                catch (e) {
                                    alert("File - Homepage.aspx\r\nMethod - SetCtiMessage\n" + e.description);
                                    //window.setTimeout("GetMessage()", 3000);
                                }
                            }
                            function GenerateANI(Agent) {
                                var command = "<a href='#' onclick=\"javascript:OpenCLIDetails('" + Agent.ACli + "', '" + Agent.ACallType + "', '" + Agent.ACallNumber + "', '" + Agent.ACallLeadId + "', '" + Agent.AModule + "', '" + Agent.AServiceName + "', '" + Agent.AServiceId + "', '" + Agent.AAddon + "', '" + Agent.AUrl + "', '" + Agent.AUrl + "');\">" + Agent.ACli + "</a>";
                                return command;
                            }
                            function OpenCLIDetails(Cli, CallType, CallNumber, LeadId, Module, ServiceName, ServiceId, Addon, Url) {

                                var bConfirmed = confirm("There is a new call from number: " + Cli + ". Do you want to open window?");
                                nConfirmed = 1;
                                if (bConfirmed == true) {
                                    window.parent.OpenItem(Url, Cli);

                                }
                            }

                            var counta = 0;

                            function GetMessage() {
                                var date=new Date();
                                console.log(date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds() + ":" + date.getMilliseconds() + "     start       GetMessage");
                                try {
                                    PageMethods.GetAgentStatus(OnGetAgentStatusSuccess, OnGetAgentStatusFailure);
                                }
                                catch (e) {
                                    date = new Date();
                                    console.log(date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds() + ":" + date.getMilliseconds() + "    Error       GetMessage");
                                    //                alert("File - Homepage.aspx\r\nMethod - GetMessage\n" + e.description);
                                }
                             //   window.setTimeout("GetMessage()", 3000);
                                date = new Date();
                                console.log(date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds() + ":" + date.getMilliseconds() + "    End       GetMessage");
                            }
                            function OnGetAgentStatusSuccess(result, userContext, methodName) {
                                try {
                                   // console.log(result);
                                    var Agent = eval('(' + result + ')');
                                    if (Agent.RCode != "-3") {
                                        if (Agent.AStatusCode != gStatusCode || Agent.ACallNumber != gCallNumber) {
                                            SetCtiMessage(Agent);
                                            gStatusCode = Agent.AStatusCode;
                                            switch (Agent.AStatusCode) {
                                                case "0":
                                                    SetAgentLogout();
                                                    break;
                                                case "1":
                                                    SetAgentStatusIdle(Agent);
                                                    break;
                                                case "2":
                                                    SetAgentStatusBusy(Agent);
                                                    break;
                                                case "3":
                                                    SetAgentStatusIncall(Agent);
                                                    break;
                                                case "4":
                                                    SetAgentStatusReserve(Agent);
                                                    break;
                                                case "5":
                                                    SetAgentStatusWrapup(Agent);
                                                    break;
                                            }
                                            gCallNumber = Agent.ACallNumber;
                                        }
                                    }
                                    var dt = new Date();
                                    var hrs = dt.getHours();
                                    document.getElementById("tdAgentInState").innerHTML = Agent.AStatusSince;
                                    document.getElementById("tdPortInState").innerHTML = Agent.PStatusSince;
                                    if (Agent.ACtiMessage != gCtiMessage) {
                                        document.getElementById("lblMessage").innerHTML = Agent.ACtiMessage;
                                        gCtiMessage = Agent.ACtiMessage;
                                        if (Agent.ACtiMessage == "Close Call Failed - 0") {
                                            PageMethods.ChangeStatus(0, fncBreakSuccess, fncBreakFailure);
                                        }
                                    }
                                    if (Agent.PStatus != gPStatus && Agent.PStatus != 'IDLE') {

                                        document.getElementById("tdPortStatusValue").innerHTML = Agent.PStatus.toUpperCase();
                                        gPStatus = Agent.PStatus;
                                    }
                                    document.getElementById("btnUnavailableD2").enable = true;

                                }
                                catch (e) {
                                    alert("File - Homepage.aspx\r\nMethod - OnGetAgentStatusSuccess\n" + e.description);
                                }
                            }
                            function Ticking(ticVal) {
                                if (ticVal < 10) {
                                    ticVal = "0" + ticVal;
                                }
                                return ticVal;
                            }
                            function OnGetAgentStatusFailure(error, userContext, methodName) {
                                try {
                                    //                alert(error.get_message());
                                    //                window.setTimeout("GetMessage()", 2000);
                                }
                                catch (e) {
                                    //                alert("File - Homepage.aspx\r\nMethod - OnGetAgentStatusFailure\n" + e.description);
                                }
                            }
                            function FillDispositions(ServiceId, Module) {
                                try {
                                    CtiWS.CtiWS.GetServiceGroupDispositions(ServiceId, Module, OnSuccess_FillDispositions, OnFailure_FillDispositions, 'NY');
                                }
                                catch (e) {
                                    alert("File - CTI\Homepage.aspx\r\nMethod - FillDispositions\n" + e.description);
                                }
                            }
                            function OnSuccess_FillDispositions(result, userContext, methodName) {
                                try {

                                    if (result == null)
                                        return;
                                    var combo = $find('<%=cmbDisposition.ClientID %>');
                                    if (combo != null) {
                                        combo.clearItems();

                                        for (var rowIndex = 0; rowIndex < result.rows.length; rowIndex++) {
                                            var row = result.rows[rowIndex];
                                            var comboItem = new Telerik.Web.UI.RadComboBoxItem();
                                            comboItem.set_value(row.Code);
                                            comboItem.set_text(row.Description);
                                            comboItem.get_attributes().setAttribute("Code", row.Code);
                                            combo.get_items().add(comboItem);
                                        }
                                    }
                                    combo.get_items().getItem(0).select();

                                }
                                catch (e) {
                                    alert("File - CTI\Homepage.aspx\r\nMethod - OnSuccess_FillDispositions\n" + e.description);
                                }
                            }
                            function OnFailure_FillDispositions(result, userContext, methodName) {

                                alert(result);

                            }
                            function FillSubDispositions(sender, eventArgs) {
                                try {
                                    var item = eventArgs.get_item();
                                    if (item.get_attributes().getAttribute("Code") == undefined || item.get_attributes().getAttribute("Code") == null) {
                                        alert('Select A Group Disposition');
                                        return false;
                                    }
                                    var Code = item.get_attributes().getAttribute("Code");
                                    CtiWS.CtiWS.GetServiceSubDispositions(gServiceId, Code, gModule, OnSuccess_FillsubDispositions, OnFailure_FillsubDispositions);
                                }
                                catch (e) {
                                    //                alert("File - Homepage.aspx\r\nMethod - FillSubDispositions\n" + e.description);
                                }
                            }
                            function OnSuccess_FillsubDispositions(result) {
                                try {
                                    if (result == null)
                                        return;
                                    var combo = $find("<%= cmbSubDisposition.ClientID %>");
                                combo.clearItems();
                                combo.clearSelection();
                                for (var rowIndex = 0; rowIndex < result.rows.length; rowIndex++) {
                                    var row = result.rows[rowIndex];
                                    var comboItem = new Telerik.Web.UI.RadComboBoxItem();
                                    comboItem.set_value(row.Code);
                                    comboItem.set_text(row.Description);
                                    comboItem.get_attributes().setAttribute("Code", row.Code);
                                    comboItem.get_attributes().setAttribute("Bucket", row.Bucket);
                                    combo.get_items().add(comboItem);
                                }
                            }
                            catch (e) {
                                //                alert("File - Homepage.aspx\r\nMethod - OnSuccess_FillsubDispositions\n" + e.description);
                            }
                        }
                        function OnFailure_FillsubDispositions(result) {
                            alert(result);
                        }
                        function ValidateBucket(sender, eventArgs) {
                            try {
                                var item = eventArgs.get_item();
                                if (item == null) {
                                    return false;
                                }
                                var Bucket = item.get_attributes().getAttribute("Bucket");

                                if (item.get_attributes().getAttribute("Code") == undefined || item.get_attributes().getAttribute("Code") == null) {
                                    return false;
                                }
                                var timePicker = $find("<%=RadCallbackTime.ClientID %>");
                                    if (timePicker != null) {
                                        timePicker.set_enabled(false);
                                        if (Bucket == "07") {
                                            timePicker.set_enabled(true);
                                            return false;
                                        }
                                    }
                                }
                                catch (e) {
                                    //alert("File - Homepage.aspx\r\nMethod - FillSubDispositions\n" + e.description);
                                }
                                return false;
                            }
                            function fncCloseCall() {

                                try {
                                    if (document.getElementById('txtCloseCallRemarks').value == "") {
                                        alert('Please enter remarks !!!');
                                        return false;
                                    }
                                    var combo = $find('<%=cmbDisposition.ClientID %>');
                                    if (combo.get_text() == null || combo.get_text() == "") {
                                        alert('Select a Disposition');
                                        combo.get_inputDomElement().focus();
                                        return false;
                                    }
                                    var DispCode = combo.get_value();
                                    combo = $find('<%=cmbSubDisposition.ClientID %>');
                                    if (combo.get_text() == null || combo.get_text() == "") {
                                        alert('Select a Sub Disposition');
                                        combo.get_inputDomElement().focus();
                                        return false;
                                    }
                                    var item = combo.get_selectedItem();
                                    var Code = item.get_attributes().getAttribute("Code");
                                    var Bucket = item.get_attributes().getAttribute("Bucket");
                                    if (Code != "") {
                                        var strDate = "";
                                        if (Bucket == "07") {
                                            var timePicker = $find("<%=RadCallbackTime.ClientID %>").get_dateInput();
                                            if (timePicker != null) {
                                                strDate = timePicker._text;
                                                if (strDate.length == 0) {
                                                    alert('Callback Time Is Required');
                                                    timePicker.focus();
                                                    return false;
                                                }
                                                var currentTime = new Date();
                                                var selectedTime = new Date(strDate);
                                                if (selectedTime < currentTime) {
                                                    alert("Callback Time can't be less than current time");
                                                    timePicker.focus();
                                                    return false;
                                                }
                                            }
                                        }
                                        var CallNumber = document.getElementById("spancli").innerHTML;
                                        var xCallType = document.getElementById("tdCallType").innerHTML;
                                        var szCallRemarks = document.getElementById('txtCloseCallRemarks').value;
                                        var remark = szCallRemarks.replace(/[^\0-9a-zA-Z.]/g, " ");
                                        remark = remark.replace(/[;\\/:~+%^#@&*?\"<>|&'\$\-\(\)]/g, " ");
                                        remark = remark.replace(/(\r\n|\n|\r)/gm, " ");

                                        var strLeadId = document.getElementById("tdCallLeadId").innerHTML;
                                        var connectionString = '<%=ConfigurationManager.AppSettings["CRMManager"]%>'
                                        // if(connectionString=="1")
                                        if (xCallType != "I" && strLeadId != "0") {

                                            window.parent.getAgentCRMSave(DispCode, Bucket, xCallType, Code, strDate, CallNumber, gServiceId, remark);
                                        }
                                        else if (strLeadId == "0") {
                                            PageMethods.CloseCall(Code, strDate, remark, fncCloseCallSuccess, fncCloseCallFailure);
                                            //window.parent.SaveCall(DispCode, Bucket, xCallType, Code, strDate, CallNumber, gServiceId, remark)
                                        }
                                    }
                                    else {
                                        alert('Select a Sub Disposition');
                                        combo.get_inputDomElement().focus();
                                    }
                                    return false;
                                }
                                catch (e) {
                                    alert("File - Homepage.aspx\r\nMethod - fncCloseCall\n" + e.description);
                                }
                                return false;
                            }
                            function fncCloseCallSuccess(result) {
                                alert('Call Wraup Up Sucsessfully!!!');

                            }

                            function fncCloseCallFailure(result) {
                            }

                            function fncSendPbxDigitsSuccess(result) {
                            }
                            function fncSendPbxDigitsFailure(result) {
                            }
                            function fncBreak() {
                                try {

                                    var e = $find("cmbBreakModes");
                                    if (e == null || e == 'undefined') {

                                        var radToolTip = null;
                                        radToolTip = $find("<%= RadToolTip1.ClientID %>");
                                        // radToolTip.set_targetControl(obj);
                                        radToolTip.set_text("To go on break.");
                                        document.getElementById("btnUnavailableD2").src = "../Images/Break.png";
                                        return false;
                                    }
                                    else {
                                        var BreakMode = e.get_value();
                                        PageMethods.ChangeStatus(BreakMode, fncBreakSuccess, fncBreakFailure);
                                    }
                                }
                                catch (e) {
                                    //                alert("File - Homepage.aspx\r\nMethod - fncBreak\n" + e.description);
                                }
                                return false;
                            }
                            function fncBreakSuccess(result) {
                                document.getElementById("btnUnavailableD2").disabled = false;
                            }
                            function fncBreakFailure(result) {
                            }
                            function fncDial(param) {
                                try {
                                    var szdialnumber = document.getElementById('txtNumber').value;
                                    if (szdialnumber == "") {
                                        alert('Please Enter Number To Dial');
                                        document.getElementById('txtNumber').focus();
                                        return false;
                                    }
                                    else {
                                        if (gCallNumber > 0) {
                                            var szDialPrefix = '<%=szDialPrefix %>';
                                            var tempdialnumber = szDialPrefix + szdialnumber;
                                            PageMethods.SendPbxDigits(tempdialnumber, fncconfrenceSuccess, fnconfrenceFailure);
                                            return false;
                                        }
                                        else {
                                            var e = $find("cmbServices");
                                            if (e == null || e == 'undefined') {
                                                return false;
                                            }
                                            else {
                                                if (e.get_text() == "") {
                                                    alert('Select a campaign to Dial');
                                                    e.get_inputDomElement().focus();
                                                    return false;
                                                }
                                                var Dni = e.get_value();
                                                var Cli = document.getElementById('txtNumber').value;
                                                PageMethods.MakeCall(Dni, Cli, fncDialSuccess, fncDialFailure);
                                            }
                                        }
                                    }
                                }
                                catch (e) {
                                    //                alert("File - Homepage.aspx\r\nMethod - fncDial\n" + e.description);
                                }
                                return false;
                            }
                            function fncDialSuccess(result) {
                                //        alert(resut);
                            }
                            function fncDialFailure(result) {
                            }
                            function fncLogout() {
                                if (confirm('Do you really want to logout!'))
                                    PageMethods.Logout(fncLogoutSuccess, fncLogoutFailure);
                                return false;
                            }
                            function fncLogoutSuccess(result) {
                                //window.parent.location = '../logout.aspx';
                                //        alert(resut);
                            }
                            function fncLogoutFailure(result) {
                            }
                            function fncAnswer() {
                                PageMethods.Answer(fncAnswerSuccess, fncAnswerFailure);
                                return false;
                            }
                            function fncAnswerSuccess(result) {
                                //        alert(resut);
                            }

                            function fncAnswerFailure(result) {
                            }
                            function fncDecline() {
                                PageMethods.Decline(fncDeclineSuccess, fncDeclineFailure);
                                return false;
                            }
                            function fncDeclineSuccess(result) {
                                //        alert(resut);
                            }
                            function fncDeclineFailure(result) {
                            }
                            function fncHangup() {
                                PageMethods.Hangup(fncLogoutSuccess, fncLogoutFailure);
                                return false;
                            }
                            function fncHangupSuccess(result) {
                                //        alert(resut);
                            }
                            function fncHangupFailure(result) {
                            }
                            function fncHold() {
                                PageMethods.Hold(fncHoldSuccess, fncHoldFailure);
                                return false;
                            }
                            function fncHoldSuccess(result) {
                                //        alert(resut);
                            }
                            function fncHoldFailure(result) {
                            }
                            function fncXfer() {

                                var sznumber = document.getElementById('txtNumber').value;
                                if (sznumber == '') {
                                    alert('Please Enter Number On Which You Want To Transfer');
                                    document.getElementById('txtNumber').focus();
                                    return false;
                                }
                                PageMethods.Xfer(sznumber, fncXferSuccess, fncXferFailure);
                                //karan shah mDC type of disposition
                                PageMethods.CloseCall("XFER", "", "", "", fncCloseCallSuccess, fncCloseCallFailure);
                                return false;
                            }
                            // added by ashish for External Call transfer
                            function fncXferExternal() {

                                var sznumber = document.getElementById('txtNumber').value;
                                if (sznumber == '') {
                                    alert('Please Enter Number On Which You Want To Transfer');
                                    document.getElementById('txtNumber').focus();
                                    return false;
                                }
                                if (sznumber.length > 4)
                                    sznumber = '0' + sznumber;
                                PageMethods.XferExternal111(sznumber, fncXferSuccess, fncXferFailure);
                                return false;
                            }
                            //added by ashish for Conference
                            function fncConference() {
                                var sznumber = document.getElementById('txtNumber').value;
                                if (sznumber == '') {
                                    alert('Please Enter Number On Which You Want To Transfer');
                                    document.getElementById('txtNumber').focus();
                                    return false;
                                }
                                PageMethods.Confrence(sznumber, fncXferSuccess, fncXferFailure);
                                return false;
                            }


                            function fncXferSuccess(result) {
                                //        alert(resut);
                            }
                            function fncXferFailure(result) {
                            }
                            function fncRecord() {
                                PageMethods.Record(fncRecordSuccess, fncRecordFailure);
                                return false;
                            }
                            function fncRecordSuccess(result) {
                                //        alert(resut);
                            }
                            function fncRecordFailure(result) {
                            }
                            function fncAdvance() {
                                if (document.getElementById("trAdvanceOptions").style.display == "block") {
                                    document.getElementById("trAdvanceOptions").style.display = 'none';
                                    document.getElementById("lnkAdvance").innerHTML = "Advance &raquo;&raquo;";
                                }
                                else {
                                    document.getElementById("trAdvanceOptions").style.display = 'block';
                                    document.getElementById("lnkAdvance").innerHTML = "Advance &laquo;&laquo;";
                                }
                            }
                            function fncAgentStatusTransfer() {

                                if (document.getElementById("tblAgentStatusTransfer").style.display == "block") {
                                    document.getElementById("tblAgentStatusTransfer").style.display = 'none';
                                    document.getElementById("imgAgentStatusTransfer").innerHTML = "Other Agent's Status   &raquo;&raquo;";
                                }
                                else {
                                    document.getElementById("tblAgentStatusTransfer").style.display = 'block';
                                    document.getElementById("imgAgentStatusTransfer").innerHTML = "Other Agent's Status   &laquo;&laquo;";
                                }
                            }
                            function fncWrapup() {

                                if (document.getElementById("tblWrapup").style.display == "block") {
                                    document.getElementById("tblWrapup").style.display = 'none';
                                    document.getElementById("imgWrapup").innerHTML = "Close Call &raquo;&raquo;";
                                }
                                else {
                                    document.getElementById("tblWrapup").style.display = 'block';
                                    document.getElementById("imgWrapup").innerHTML = "Close Call &laquo;&laquo;";
                                }
                            }
                            function setMinDate(minDate) {
                                var timePicker = $find("<%= RadCallbackTime.ClientID %>");
                            if (timePicker != null) {
                                if (minDate.checked) {
                                    var date1 = new Date();
                                    date1.setHours(5, 0, 0, 0);
                                    timePicker.set_minDate(date1);
                                }
                                else {
                                    var date1 = new Date();
                                    date1.setHours(0, 0, 0, 0);
                                    timePicker.set_minDate(date1);
                                }
                            }
                        }
                        function setMaxDate(maxDate) {
                            var date1 = new Date();
                            date1.setHours(20, 0, 0, 0);
                            var timePicker = $find("<%= RadCallbackTime.ClientID %>");
                            if (maxDate.checked) {
                                timePicker.set_maxDate(date1);
                            }
                            else {
                                var date1 = new Date();
                                date1.setHours(24, 0, 0, 0);
                                timePicker.set_maxDate(date1);
                            }
                        }
                        function ClearFields() {
                            document.getElementById('txtCloseCallRemarks').value = '';
                            var combodisposition = $find('<%=cmbDisposition.ClientID %>');
                            combodisposition.clearSelection();
                            var combosubdisposition = $find('<%=cmbSubDisposition.ClientID %>');
                            combosubdisposition.clearSelection();
                            var timePicker = $find("<%=RadCallbackTime.ClientID %>");
                            timePicker._dateInput.clear();
                        }
                        function fncconfrenceSuccess() {
                        }
                        function fnconfrenceFailure() {
                        }
                        function Confrence() {
                            PageMethods.Confrence('', fncconfrenceSuccess, fnconfrenceFailure);
                            return false;
                        }
    </script>

    <form id="frmSoftPhone" runat="server">
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" EnablePageMethods="true">
            <Services>
                <asp:ServiceReference Path="~/Services/CtiWS.asmx" />
            </Services>
        </asp:ToolkitScriptManager>
        <table width="100%" id="aaaa" runat="server">
            <tr>
                <td id="tdMessage">
                    <asp:Label ID="LblMsg" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <table width="100%">
                        <tr>
                            <td width="80%">
                                <b>
                                    <marquee behavior="scroll" direction="left" scrollamount="5" id="lblMessage" runat="server"></marquee>
                                </b>
                            </td>
                            <td align="left" width="20%">
                                <asp:ImageButton ID="btnLogout" runat="server" Width="20" Height="20" ImageUrl="~/Images/Logouticon.png"
                                    ToolTip="Logout" OnClientClick="javascript: return fncLogout();" />
                            </td>
                        </tr>
                    </table>
                </td>
                <%-- </tr>
        <tr>
            <td align="right" style="padding-right: 5px">--%>
            </tr>
            <tr>
                <td>
                    <table class="cti_mainFieldSet" width="100%" height="100%">
                        <%--DIALOG TYPE 2--%>
                        <tr>
                            <td width="100%" valign="top" id='tblDialPad' runat="server">
                                <table width="100%">
                                    <tr>
                                        <td colspan="2">
                                            <font color="Blue" style="font-size: 10px">Campaign:</font>
                                            <telerik:RadComboBox ID="cmbServices" runat="server" CssClass="boxes">
                                            </telerik:RadComboBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td width="46%">
                                            <asp:TextBox ID='txtNumber' placeholder="Enter The Number" CssClass="boxes" runat="server"
                                                onkeypress="return ValidateNumberOnly(event);" oninput="ValidateNumberPaste (event)"
                                                MaxLength="15" />
                                        </td>
                                        <td width="30%" valign="middle">
                                            <asp:ImageButton ID="btnDial" Height="20" Width="20" ImageUrl="~/Images/phone.png" runat="server" OnClientClick="javascript: return fncDial('Dial');" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <table width="100%">
                                                <tr>
                                                    <td>
                                                        <font color="Blue" style="font-size: 10px">Break:</font>
                                                    </td>
                                                    <td>
                                                        <telerik:RadComboBox ID="cmbBreakModes" runat="server" CssClass="boxes" Width="100%">
                                                        </telerik:RadComboBox>
                                                    </td>
                                                    <td align="right">
                                                        <asp:ImageButton ID="btnUnavailableD2" runat="server" ImageUrl="~/Images/Break.png"
                                                            OnClientClick="javascript: return fncBreak();" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="trstatus">
                                        <td colspan="2" id="td1234" runat="server">
                                            <fieldset>
                                                <legend class="green-color"><b>Call Status</b></legend>
                                                <table width="100%">
                                                    <tr>
                                                        <td id="td1">
                                                            <asp:Image runat="server" ID="Image1" Width="24" Height="24" ToolTip="Extension Detail" ImageUrl="~/Images/Extension.png" />
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="lblTerminalId" ForeColor="Black" runat="server"></asp:Label>
                                                        </td>
                                                        <td>
                                                            <div>
                                                                <span id="tdPortStatusValue" style="height: 15px; background-color: White;width:100%;float:left;">
                                                                    <font color="#000000">IDLE</font>
                                                                </span>
                                                              
                                                                <span id="tdPortInState" style="background-color: #000; border: #999 1px inset; padding: 0.5px; color: #0FF; font-size: 9px; font-weight: bold; letter-spacing: 0.5px; display: inline;width:100%">
                                                                     <font color="#000000">
                                                                        <label>
                                                                            00:00:00</label></font>
                                                                </span>
                                                            </div>
                                                            <%--<table>
                                                                <tr>
                                                                    <td id="tdPortStatusValue"  runat="server" style="height: 12px; background-color: White">
                                                                        
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td id="tdPortInState" runat="server" width="100%" style="background-color: #000; border: #999 1px inset; padding: 0.5px; color: #0FF; font-size: 9px; font-weight: bold; letter-spacing: 0.5px; display: inline;">
                                                                        <font color="#000000">
                                                                        <label>
                                                                            00:00:00</label></font>
                                                                    </td>
                                                                </tr>
                                                            </table>--%>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td id="tdAgentStatus">
                                                            <asp:Image runat="server" Width="24" Height="24" ID="imgAgent" ToolTip="Extension Detail" ImageUrl="~/Images/femaleicon.png" />
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="lblAgenName" ForeColor="Black" runat="server"></asp:Label>
                                                        </td>
                                                        <td>
                                                             <div>
                                                                <span id="tdAgentStatusValue" style="height: 15px; background-color: White;width:100%;float:left;">
                                                                    <font color="#000000">Available</font>
                                                                </span>
                                                              
                                                                <span id="tdAgentInState" style="background-color: #000; border: #999 1px inset; padding: 0.5px; color: #0FF; font-size: 9px; font-weight: bold; letter-spacing: 0.5px; display: inline;width:100%">
                                                                     <font color="#000000">
                                                                        <label>
                                                                            00:00:00</label></font>
                                                                </span>
                                                            </div>
                                                           <%-- <table>
                                                                <tr>
                                                                    <td id="tdAgentStatusValue" runat="server" style="height: 12px;">
                                                                        <font color="#000000">Available</font>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td id="tdAgentInState" runat="server"  width="100%" style="background-color: #000; border: #999 1px inset; padding: 0.5px; color: #0FF; font-size: 9px; font-weight: bold; letter-spacing: 0.5px; display: inline;">
                                                                        <font color="#000000">00:00:00</font>
                                                                    </td>
                                                                </tr>
                                                            </table>--%>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td id="tdAni" style="height: 12px; padding-left: 4px">
                                                            <asp:Image ID="imgCli" Width="16" Height="16" runat="server" ImageUrl="~/Images/contacts.png" />
                                                        </td>
                                                        <td></td>
                                                        <td id="tdCallANI" style="height: 12px;">
                                                            <font color="#000000"></font>
                                                            
                                                        </td>
                                                    </tr>
                                                </table>
                                                <span id="spancli" style="display:none"></span>
                                            </fieldset>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <fieldset>
                                                <legend class="green-color"><b><a id="lnkAdvance" href="#" style="text-decoration: none; color: black;"
                                                    onclick="javascript:fncAdvance();">Advance &raquo;&raquo;</a></b></legend>
                                                <table width="100%">
                                                    <tr>
                                                        <td colspan="2" align="left" style="height: 12px;"></td>
                                                    </tr>
                                                    <tr id="trAdvanceOptions" style="display: none; width: 100%;">
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td style="width: 80%; height: 12px;">
                                                                        <font color="Blue">Call Number </font>
                                                                    </td>
                                                                    <td id="tdCallNumber" style="height: 12px; color: black">
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="height: 12px;">
                                                                        <font color="Blue">Service </font>
                                                                    </td>
                                                                    <td id="tdCallService" style="height: 12px; color: black"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="height: 12px;">
                                                                        <font color="Blue">Module </font>
                                                                    </td>
                                                                    <td id="tdCallModule" style="height: 12px; color: black"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="height: 12px;">
                                                                        <font color="Blue">Call Type </font>
                                                                    </td>
                                                                    <td id="tdCallType" style="height: 12px; color: black"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="height: 12px;">
                                                                        <font color="Blue">Lead Id </font>
                                                                    </td>
                                                                    <td id="tdCallLeadId" style="height: 12px; color: black"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="height: 12px;">
                                                                        <font color="Blue">DNI </font>
                                                                    </td>
                                                                    <td id="tdCallDni" style="height: 12px; color: black"></td>
                                                                </tr>
                                                                <tr style="display: none">
                                                                    <td style="height: 12px;">
                                                                        <font color="Blue">City </font>
                                                                    </td>
                                                                    <td id="tdCallCity" style="height: 12px; color: black"></td>
                                                                </tr>
                                                                <tr style="display: none">
                                                                    <td style="height: 12px;">
                                                                        <font color="Blue">State </font>
                                                                    </td>
                                                                    <td id="tdCallState" style="height: 12px; color: black"></td>
                                                                </tr>
                                                                <tr style="display: none">
                                                                    <td style="height: 12px;">
                                                                        <font color="Blue">Region </font>
                                                                    </td>
                                                                    <td id="tdCallRegion" style="height: 12px; color: black"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="height: 12px;">
                                                                        <font color="Blue">Additional Info </font>
                                                                    </td>
                                                                    <td id="tdCallAddon" style="height: 12px; color: black"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="height: 12px;">
                                                                        <font color="Blue">Dialable </font>
                                                                    </td>
                                                                    <td id="tdCallDialable" style="height: 12px; color: black"></td>
                                                                </tr>
                                                                <tr>
                                                                    <td style="height: 12px;">
                                                                        <font color="Blue">Preview Time </font>
                                                                    </td>
                                                                    <td id="tdCallPreviewTime" style="height: 12px; color: black"></td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </fieldset>
                                        </td>
                                    </tr>
                                    <tr style="display:none">
                                        <td colspan="2" id="trAgentStatusTransfer" runat="server">
                                            <fieldset>
                                                <legend class="green-color"><b><a id="imgAgentStatusTransfer" href="#" style="text-decoration: none; color: Black;"
                                                    onclick="javascript:fncAgentStatusTransfer();">Other Agent's Status
                                                &raquo;&raquo;</a></b></legend>
                                                <asp:UpdatePanel ID="ShowButtonPanel" runat="server" UpdateMode="conditional">
                                                    <ContentTemplate>
                                                        <table width="100%" runat="server" id="tblAgentStatusTransfer" style="display: none">
                                                            <tr>
                                                                <td style="height: 12px; width: 40%">
                                                                    <font color="Blue">Agent Name:</font>
                                                                </td>
                                                                <td>
                                                                    <telerik:RadComboBox ID="ddlAgentNameTransfer" Width="90%" runat="server" OnSelectedIndexChanged="ddlAgentNameTransfer_SelectedIndexChanged"
                                                                        AutoPostBack="true" CssClass="boxes">
                                                                    </telerik:RadComboBox>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Agent Status:
                                                                </td>
                                                                <td align="left">
                                                                    <font color="black">
                                                                    <asp:Label ID="lblagentstatus" runat="server"></asp:Label></font>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Terminal ID:
                                                                </td>
                                                                <td align="left">
                                                                    <font color="black">
                                                                    <asp:Label ID="lblnagentterminalid" runat="server"></asp:Label></font>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>Extension Status:
                                                                </td>
                                                                <td align="left">
                                                                    <font color="black">
                                                                    <asp:Label ID="lblagentextstatus" runat="server"></asp:Label></font>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </fieldset>
                                        </td>
                                    </tr>
                                </table>
                                <fieldset>
                                    <legend class="green-color"><b>Telephony Command</b></legend>
                                    <table width="100%" >
                                        <tr>
                                            <td>
                                                <asp:ImageButton ID="btnHangup" Style="display: none;" Width="20" Height="20" runat="server" ImageUrl="~/Images/hang-up.png"
                                                    OnClientClick="javascript: return fncHangup();" />
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="btnHold" Style="display: none;" Width="20" Height="20" runat="server" ImageUrl="~/Images/HoldIcon.png"
                                                    Enabled="true" OnClientClick="javascript: return fncHold();" />
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="btnTransfer" Width="20" Height="20" Style="display: none;" ImageUrl="~/Images/transfer.jpg"
                                                    runat="server" OnClientClick="javascript: return fncXfer();" />
                                            </td>
                                            
                                            <td>
                                                <asp:ImageButton ID="btnExTransfer" Width="25" Height="25" Style="display: none;" ImageUrl="~/Images/ScreenTransfer.png"
                                                    runat="server" OnClientClick="javascript: return fncXferExternal();" />
                                            </td>
                                            
                                            
                                            
                                            <td>
                                                <asp:ImageButton ID="btnconfrence" Style="display: none;" Width="20" Height="20" runat="server" ImageUrl="~/Images/ConfrenceIcon.png" 
                                                OnClientClick="javascript: return fncConference();"/>
                                            </td>
                                            <td>
                                                <asp:ImageButton ID="btnconfrenceDisconn" Style="display: none" Width="20" Height="20" runat="server" ImageUrl="~/Images/ConfrenceIconDis.png"
                                                OnClientClick="javascript: return fncXferExternal();" />
                                            </td>
                                        </tr>
                                    </table>
                                    
                                    
                                    
                                </fieldset>
                                <telerik:RadToolTip ID="RadToolTip1" runat="server" TargetControlID="btnUnavailableD2"
                                        IsClientID="false" Skin="Windows7" VisibleOnPageLoad="false" AutoCloseDelay="0"  Width="200px" HideEvent="LeaveToolTip">
                                        To go on break.
                                    </telerik:RadToolTip>
                                    <telerik:RadToolTip ID="RadToolTip" runat="server" TargetControlID="btnDial" IsClientID="false"
                                        Skin="Windows7" VisibleOnPageLoad="false" AutoCloseDelay="0"  Width="200px" HideEvent="LeaveToolTip">
                                        <u><b>Dial:</b></u><br />
                                        Before clicking on this button first dial a number.
                                    </telerik:RadToolTip>
                                    <telerik:RadToolTip ID="RadToolTip2" runat="server" TargetControlID="btnHangup" IsClientID="false"
                                        Skin="Windows7" VisibleOnPageLoad="false" AutoCloseDelay="0"  Width="200px" HideEvent="LeaveToolTip">
                                        <u><b>Hangup:</b></u><br />
                                        To disconnect the call.
                                    </telerik:RadToolTip>
                                    <telerik:RadToolTip ID="RadToolTip3" runat="server" TargetControlID="btnHold" IsClientID="false"
                                        Skin="Windows7" VisibleOnPageLoad="false" AutoCloseDelay="0"  Width="200px" HideEvent="LeaveToolTip">
                                        <u><b>Hold:</b></u><br />
                                        To hold the call.
                                    </telerik:RadToolTip>
                                    <telerik:RadToolTip ID="RadToolTip4" runat="server" TargetControlID="btnTransfer"  Width="200px" HideEvent="LeaveToolTip"
                                        IsClientID="false" Skin="Windows7" VisibleOnPageLoad="false" AutoCloseDelay="0">
                                        <u><b>Transfer:</b></u>
                                        <br />
                                        To Screen Transfer call to an extension or a no.
                                    <br />
                                        <u><b>Method:</b></u>
                                        <br />
                                        Enter the number/ext. on the above textbox & click Transfer button.
                                    </telerik:RadToolTip>
                                    <telerik:RadToolTip ID="RadToolTip5" runat="server" TargetControlID="btnconfrence"  Width="200px" HideEvent="LeaveToolTip"
                                        IsClientID="false" Skin="Windows7" VisibleOnPageLoad="false" AutoCloseDelay="0" >
                                        <u><b>Conference:</b></u><br />
                                        To do multiple party conference .<br />
                                        <u><b>Method:</b></u><br />
                                        Step1:- Hold the existing call by clicking to Conference button .<br />
										Step2:- Click on Disconnect button .<br />
                                        Step3:- Enter the conference party no. in above textbox & click dial .<br />
                                        Step4:- On connect,click Conference button again.
                                    </telerik:RadToolTip>
                                    <telerik:RadToolTip ID="RadToolTip6" runat="server" TargetControlID="btnconfrenceDisconn"  Width="200px" HideEvent="LeaveToolTip"
                                        IsClientID="false" Skin="Windows7" VisibleOnPageLoad="false" AutoCloseDelay="0">
                                        <u><b>Disconnect:</b></u><br />
                                        To disconnect the conference .<br />
                                    </telerik:RadToolTip>
                                    
                                      <telerik:RadToolTip ID="RadToolTip7" runat="server" TargetControlID="btnExTransfer"  Width="200px" HideEvent="LeaveToolTip"
                                        IsClientID="false" Skin="Windows7" VisibleOnPageLoad="false" AutoCloseDelay="0">
                                        <u><b>Transfer:</b></u>
                                        <br />
                                        To transfer a call to an extension or a number.
                                    <br />
                                        <u><b>Method:</b></u>
                                        <br />
                                        Enter the number/ext. on the above textbox & click Transfer button.
                                    </telerik:RadToolTip>
                                <table width="100%">
                                    <tr>
                                        <td colspan="2">
                                            <fieldset>
                                                <legend><b><a id="imgWrapup" href="#" style="text-decoration: none; color: black;"
                                                    onclick="javascript:fncWrapup();">Close Call &raquo;&raquo;</a> </b></legend>
                                                <table width="100%" runat="server" id="tblWrapup" style="display: block">
                                                    <tr>
                                                        <td>
                                                            <span style="color: Blue; font-size: 10px;">Disp:</span>
                                                        </td>
                                                        <td>
                                                            <telerik:RadComboBox ID="cmbDisposition" Width="95%" runat="server" OnClientSelectedIndexChanging="FillSubDispositions"
                                                                CssClass="boxes">
                                                            </telerik:RadComboBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <span style="color: Blue; font-size: 10px;">Sub Disp:</span>
                                                        </td>
                                                        <td>
                                                            <telerik:RadComboBox ID="cmbSubDisposition" Width="95%" runat="server" OnClientSelectedIndexChanging="ValidateBucket"
                                                                CssClass="boxes">
                                                            </telerik:RadComboBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <span style="color: Blue; font-size: 10px;">Callback:</span>
                                                        </td>
                                                        <td>
                                                            <telerik:RadDateTimePicker ID="RadCallbackTime" Width="95%" Height ="50%" TimeView-Interval="0:30:0"
                                                                runat="server" CssClass="boxes" PopupDirection="BottomLeft" TimeView-StartTime ="9:0:0"  TimeView-EndTime="21:30:00" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <span style="color: Blue; font-size: 10px;">Remarks :</span>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtCloseCallRemarks" Style="resize: none" TextMode="MultiLine" CssClass="boxes"
                                                                runat="server" Height="50" Width="135"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr style="width: 100%" align="right">
                                                        <td colspan="2">
                                                            <asp:ImageButton ID="btnEndCall" runat="server" ImageUrl="~/Images/WrapupCallIcon.png"
                                                                ToolTip="Wrapup Call" OnClientClick="javascript: return fncCloseCall();" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </fieldset>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>

        <script language="javascript" type="text/javascript">
            function Count(text) {
                //asp.net textarea maxlength doesnt work; do it by hand
                var maxlength = 300; //set your value here (or add a parm and pass it in)
                var object = document.getElementById(text.id)  //get your object
                if (object.value.length > maxlength) {
                    object.focus(); //set focus to prevent jumping
                    object.value = text.value.substring(0, maxlength); //truncate the value
                    object.scrollTop = object.scrollHeight; //scroll to the end to prevent jumping
                    alert('Remarks must be less then 300 characters');
                    return false;
                }
                return true;
            }
            function ValidateAlphabetOnly(e) {
                var keyCode = (typeof e.which == "number") ? e.which : e.keyCode
                if (keyCode != 0) if (keyCode != 8)
                    if ((keyCode < 65 || keyCode > 90) && (keyCode < 97 || keyCode > 123) && keyCode != 32) {
                        if (window.event)
                            window.event.returnValue = false;       // IE
                        else
                            e.preventDefault();
                    }
            }
            function ValidateAlphabetPaste(event) {
                var totalCharacterCount = event.target.value;
                var strValidChars = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
                var strChar;
                var FilteredChars = "";
                for (i = 0; i < totalCharacterCount.length; i++) {
                    strChar = totalCharacterCount.charAt(i);
                    if (strValidChars.indexOf(strChar) != -1) {
                        FilteredChars = FilteredChars + strChar;
                    }
                }
                event.target.value = FilteredChars;
                return false;
            }
            function ValidateNumberOnly(e) {
                var keyCode = (typeof e.which == "number") ? e.which : e.keyCode
                if (keyCode != 120) if (keyCode != 118) if (keyCode != 0) if (keyCode != 8)
                    if ((keyCode < 48 || keyCode > 57)) {
                        if (window.event)
                            window.event.returnValue = false;       // IE
                        else
                            e.preventDefault();
                    }
            }
            function ValidateNumberPaste(event) {

                var totalCharacterCount = event.target.value;
                var strValidChars = "0123456789";
                var strChar;
                var FilteredChars = "";
                for (i = 0; i < totalCharacterCount.length; i++) {
                    strChar = totalCharacterCount.charAt(i);
                    if (strValidChars.indexOf(strChar) != -1) {
                        FilteredChars = FilteredChars + strChar;
                    }
                }
                event.target.value = FilteredChars;
                return false;
            }
        </script>

        <table width="100%" style="background-color: Transparent">
            <tr>
            </tr>
            <tr>
                <td align="center">
                    <img id="img1" runat="server" style="height: 40px; width: 170px" alt="Logo" src="~/Images/TeckinfoLOGO.png" />
                </td>
            </tr>
        </table>
        <%--<asp:Label ID="lblCustomerid" runat="server" Style="display: none">
    </asp:Label>--%>
    </form>
    
</body>
</html>
