<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChatVisitor.aspx.cs" Inherits="Chat_ChatVisitor" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=sourcename%></title>
    <link href="../css/Common.css" rel="stylesheet" type="text/css" />
    <link href="../css/chat.css?v=1.1" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/jquery-1.4.2.min.js"></script>

    <style type="text/css">
        #divmsg
        {
            text-align: center;
        }

        .header
        {
            float: none;
            color: white;
            font-size: 18px;
            font-family: Verdana;
        }

        .main
        {
            width: 100%;
            height: 100%;
            overflow: auto;
        }
    </style>
</head>
<body onload="javascript:UpdateLocalMessage();" style="height: 100%" onunload="ClosedWwindow()" onbeforeunload="beforeunload()">

    <script type="text/javascript">
        window.history.forward();
        var szchatmsg = "";
        var timeoutid = "";
        var szchatsessionid = "";
        var sourceID = "";
        var tempsourceID = "";
        var sourceheader = "";
        var destinationheader = "";
        var scrollheight = "";
        var pastscolltop = "";
        var data = [];
        var chatmsgid = -1;
        var userid = '<%= sourcid %>';
        var type = '<%=Convert.ToString(Request.QueryString["type"]!=null ? Request.QueryString["type"] : "I") %>';
        var currentDate = new Date();
        var chatsession = false;
        var chatendmessage = "Session Expired";

        function pageLoad(sender, args) {
            szchatsessionid = '<%=Convert.ToString(Request.QueryString["chatsessionid"]) %>';
            try {
                // coockiesexpirydate();
                var container = $("#divdisplaytext");
                container.html("");
                ChatWS.GetAllCustomerName(szchatsessionid, ChatList)
                setInterval('UpdateLocalMessage()', 2000);

                currentDate = new Date('<%=currentDate %>');
                setInterval('UpdateTime()', 60000);
                setInterval('settime()', 1000);

                if (args._isPartialLoad == false) {
                    CloseWindow();
                }

            }
            catch (e) {
                alert("Error In File chatprompt.aspx \r\n Method PageLoad \r\n ErrorMessage : " + e.description);
                return;
            }
        }
        function UpdateTime() {
            ChatWS.UpdateChatVisitorTime(szchatsessionid, OnSeccessUpdatetime);

        }
        function OnSeccessUpdatetime(result) {

        }
        function getCookie(name) {

            var nameEquals = name + "=";
            var crumbs = document.cookie.split(';');

            for (var i = 0; i < crumbs.length; i++) {
                var crumb = crumbs[i];
                var strvalue = crumb.trim();
                if (strvalue.indexOf(nameEquals) == 0) {

                    return unescape(strvalue.substring(nameEquals.length, strvalue.length));
                }
            }
            return null;
        }

        function coockiesexpirydate() {
            alert(JSON.stringify(document.cookie));
            var crumbs = document.cookie.split(';');

            for (var i = 0; i < crumbs.length; i++) {
                var crumb = crumbs[i];
                var strvalue = crumb.trim();
                alert(strvalue);
                if (strvalue.indexOf("expires=") == 0) {

                    alert(strvalue.substring(nameEquals.length, strvalue.length));
                }
            }
            return null;


        }

        function settime() {
            var a = new Date(currentDate);
            var aa = new Date(a.setSeconds(a.getSeconds() + 1)); //currentDate.setTime(currentDate.getTime() + 1000);
            currentDate = aa;
        }
        function ChatList(args) {
            data = [];
            var chatsessiondetail = args.chatsessiondetail;

            for (var i = 0; i < chatsessiondetail.rows.length; i++) {
                var d = {};
                var item = chatsessiondetail.rows[i];
                d.id = item.id;
                d.name = item.Name;
                d.css = "tlk f" + i;
                data.push(d);
            }

            //  UpdateLocalMessage();
        }

        function startchat(type) {
            if (chatsession == true) {
                callmessage();
                return false;
            }
            var txtmsg = document.getElementById('txtmsg');
            if (type == "V") {
                if (confirm("Are you sure you want to closed chat?") == true) {
                    chatsession = true;
                    ChatWS.SendMessage(szchatsessionid, chatendmessage, type, UpdateLocalMessage)
                   
                }
            }
            else
            if (txtmsg.value != "") {
                ChatWS.SendMessage(szchatsessionid, txtmsg.value, type, UpdateLocalMessage)
                // ChatWS.SendMessage(szchatsessionid, txtmsg.value, type, UpdateMessageFaileCallBack)

                txtmsg.value = "";
            }


        }
        function UpdateLocalMessage() {

        }
        function UpdateLocalMessage() {
            if(chatsession == true)
                window.close();
            ChatWS.ReceivedMassege(szchatsessionid, chatmsgid, UpdateMessageSuccessCallBack, UpdateMessageFaileCallBack)
        }
        function UpdateMessageFaileCallBack(args) {
            //csaspnetajaxwebchat.transition.LeaveChatRoom(null);
        }
        var j = 0;
        var id = -1;
        var name = "";
        var d = document.createElement("DIV");
        var css = "tlk f";
        var date = new Date();
        var flage = false;
        var newcontact = false;
        var welcomflage = false;

        function UpdateMessageSuccessCallBack(args) {
            
            var lblstatus = document.getElementById('lblstatus');
            var container = $("#divdisplaytext");
            // container.html("");
            var chatmsg = '';
            var chatid = 0;
            var chatdetails = args.chatdetails;
            //console.log("chatdetails----" + chatdetails.rows.length);
            if (type == "E" && chatdetails.rows.length == 0 && welcomflage == false && userid == 0) {
                
                var ob = new Object();
                ob.Type = "M";
                ob.message = "Welcome to Easyfuel,</br> how may I help you?";
                ob.id = -1;
                ob.time = currentDate;

                for (k = 0; k < data.length; k++) {
                    var itemd = data[k];
                    if (itemd.id != 0) {
                        ob.fid = itemd.id;
                        welcomflage = true;
                        ob.id = 0;
                    }
                }
                chatdetails.rows.push(ob);
                //console.log("-------------------------------------------------------")
                //console.log( JSON.stringify(chatdetails));
            }
            //console.log("after add chatdetails length----" + chatdetails.rows.length);
            for (var i = 0; i < chatdetails.rows.length; i++) {
                var item = chatdetails.rows[i];
                if (chatmsgid < item.id) {
                    if (item.Type == "M") {
                        lblstatus.innerHTML = "";
                        if (id != item.fid) {
                            newcontact = false;
                            for (k = 0; k < data.length; k++) {
                                var itemd = data[k];
                                if (itemd.id == item.fid) {
                                    newcontact = true;
                                    name = itemd.name;
                                    id = itemd.id;
                                    css = itemd.css;
                                    d = document.createElement("DIV");
                                    $(d)
                        .appendTo(container)
                        .addClass(css)
                        .end()
                        .append("<span class=\"_talker\">" + (name) + "</span>")
                        .append("</br><hr style=\"color:\#CCCC7A;background-color:\#CCCC7A;height:1px\"></hr>");
                                    flage = true;
                                    break;
                                }
                            }

                        }
                        if (newcontact == false) {
                            ChatWS.GetAllCustomerName(szchatsessionid, ChatList);
                            return;
                        }
                        if (chatmsg != item.message || chatid != item.fid) {
                            chatmsg = item.message;
                            chatid = item.fid;
                            //manju rana
                            var sub = '';
                            if (item.message.indexOf("http") > -1) {
                                var positionOfVar = item.message.indexOf("http:\\");
                                var positionoflast = item.message.indexOf(" ", positionOfVar);
                                if (positionoflast == '-1') {
                                    sub = item.message.substring(positionOfVar);
                                }
                                else {
                                    sub = item.message.substring(positionOfVar, positionoflast);
                                }

                                var finalstring = "<a href=\"" + sub + "\" target='_blank'> " + sub + "</a>";
                                item.message = item.message.replace(sub, finalstring);
                                //console.log("positionOfVar" + positionOfVar);
                                //console.log("positionoflast" + positionoflast);
                                //console.log("substring" + sub);

                            }

                            $(d)
                    .append("<span class=\"_msg\">" + item.message + "</span>")
                    .append("<BR /> ")
                            date = item.time;
                            container.scrollTop(container[0].scrollHeight - container.height());
                        }
                        chatmsgid = item.id;
                    }
                    else if ((item.Type == "C" || item.Type == "T") && userid != item.fid) {


                        for (k = 0; k < data.length; k++) {
                            var itemd = data[k];
                            if (itemd.id == item.fid) {
                                lblstatus.innerHTML = itemd.name + " " + item.message;
                                break;
                            }
                        }
                    }
                    else if ((item.Type == "E")) {
                        chatsession = true;
                        callmessage();
                        chatmsgid = item.id;
                    }

                    // chatmsgid = item.id;
                }
            }
            //  var currentdate = new Date();
            var n = currentDate.getTime();
            var m = date.getTime();
            // alert(date.getTime());
            if ((((n - m) / (60000)) >= 1) && (flage == true)) {
                //  alert("hello");
                $(d)
                    .append("<span class=\"_time\"> Sent at " + date.format("HH:mm tt") + " on " + date.format("dddd") + "</span>")
                    .append("<BR /> ");
                container.scrollTop(container[0].scrollHeight - container.height());
                flage = false;
            }
            //   setTimeout(function() { UpdateLocalMessage(); }, 2000);
        }
        function onSuccess(result) {
            try {

            }
            catch (e) {

            }
        }
        function onFailed(result) {
            try {

            }
            catch (e) {

            }

        }
        function callmessage() {
            alert(chatendmessage);
            window.close();
            return;
        }
        function SetDivPosition() {
            var intY = document.getElementById("divdisplaytext").scrollTop;
            var divheight = document.getElementById("divdisplaytext").scrollHeight;
            if (pastscolltop == "") {
                scrollToBottomOfdiv(intY);
                pastscolltop = document.getElementById("divdisplaytext").scrollTop;
            }
            else {
                if (pastscolltop <= scrollheight) {
                    scrollToBottomOfdiv(intY);

                }

            }
        }
        function scrollToBottomOfdiv(intY) {
            mydiv = document.getElementById('divdisplaytext');
            mydiv.scrollTop = mydiv.scrollHeight;
            scrollheight = intY;
        }
        function GetRadWindow() {
            try {
                var oWindow = null;
                if (window.frameElement == null)
                    return null;

                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;

                return oWindow;
            }
            catch (e) {
                alert("Error:- Page -> CHAT CUSTOMER -> Method -> \"GetRadWindow\" \n Description:- \n" + e.description);
                return false;
            }
        }

        function CloseWindow() {
            try {
                var oWnd = GetRadWindow();
                if (oWnd != null) {
                    var oArg = new Object();
                    oArg.ChatID = '<%=chatsessionid %>';
                    oWnd.argument = oArg;
                }
            }
            catch (e) {
                alert("Error:- Page -> Lookup.aspx -> Method -> 'CloseWindow' \n Description:- \n" + e.description);
                return false;
            }
        }

        var visble = false;

        function onsucesschatgroup(result) {
        }
        var backspace = true;
        function sendMessage(e) {
            if (chatsession == true) {
                callmessage();
                return false;
            }
            var dd = document.getElementById('txtmsg').value;


            if (e.keyCode == 13) {
                backspace = true;
                startchat('M');
                // dd = "";
                document.getElementById('txtmsg').value = "";


            }
            else if (e.keyCode == 8 && dd.length == 1) {
                if (backspace == true) {
                    backspace = false;
                    ChatWS.SendMessage(szchatsessionid, "Cleared", "C", UpdateLocalMessage)
                }

            }
            else if (e.keyCode != 8 && e.keyCode != 13 && dd.length == 1) {
                backspace = true;
                ChatWS.SendMessage(szchatsessionid, "Typing", "T", UpdateLocalMessage)
            }

        }
        function cleartext(e) {
            if (e.keyCode == 13) {
                document.getElementById('txtmsg').value = "";
            }
        }
        function ClosedWwindow() {
           //alert(navigator.appName);
           //if (navigator.appName == "Netscape") {
           //    ChatWS.SendMessage(szchatsessionid, chatendmessage, "V", UpdateLocalMessage);
           //}
           //else {
           //    // if (chatsession == false) {
           //    if (confirm("Are you sure closed chat window?") == true) {
           //        chatsession = true;
           //        ChatWS.SendMessage(szchatsessionid, chatendmessage, "V", UpdateLocalMessage);

           //    }
           //    else {
           //        return;
           //    }
           //    // }
           //}
        }




        //var isNS = (navigator.appName == "Netscape") ? 1 : 0;
        //if (navigator.appName == "Netscape") {
        //    document.captureEvents(Event.MOUSEDOWN || Event.MOUSEUP)
        //}
        //window.onbeforeunload = confirmExit;

        //function confirmExit() {
        //    if (confirm("Are you sure closed chat window?") == true) {
        //        chatsession = true;
        //        ChatWS.SendMessage(szchatsessionid, chatendmessage, "V", UpdateLocalMessage)

        //    }
        //    else {
        //        return;
        //    }


          
        //}

     //   window.onbeforeunload = function (event) {
            //alert("jhgh");
            //if (confirm("Are you sure closed chat window?") == true) {
            //    chatsession = true;
              //  ChatWS.SendMessage(szchatsessionid, chatendmessage, "V", UpdateLocalMessage)

           // }
          //  else {
          //      return;
           // }
        //    debugger;
        //    event = event || window.event;

        //    var confirmClose = 'Are you sure?';

        //    // For IE and Firefox prior to version 4
        //    if (event) {
        //        event.returnValue = confirmClose;
        //    }

        //    // For Safari
        //    return confirmClose;

     //   }
        function beforeunload() {

           // alert(navigator.appName);
            
        }
    </script>

    <form id="form1" runat="server" defaultbutton="btnSendMessage">
        <asp:ScriptManager ID="scriptmgr" runat="server">
            <Services>
                <asp:ServiceReference Path="~/Services/ChatWS.asmx" />
            </Services>
        </asp:ScriptManager>
        <div class="main">
                <div id="divmsg">                    
                    <asp:Label ID="lblmsg" runat="server" Text="Welcome To Easyfuel" CssClass="header"></asp:Label>
                </div>
            
            <asp:Label ID="lblmessage" runat="server" Height="16px"></asp:Label>
            
            <div id="divdisplaytext" style="min-height: 206px; width: 100%; word-wrap: break-word; font-size: 11px; overflow: auto; border: 1px solid rgba(230, 229, 229, 0.76); padding: 4px; position: relative">
            </div>
            
            <div style="clear: both;">
            </div>
            <div style="height: 15px; padding-left: 15px; text-align: center; font-size: 9px; color: Gray; font-weight: normal;">
            <span id="lblstatus"></span>
                </div>
            <div style="clear: both;">
            </div>
            <div style="width: 99%; float: right;">
                <div style="width: 90%; float: left">
                    <asp:TextBox ID="txtmsg" runat="server" Style="resize: none; width: 100%" TextMode="MultiLine"
                        onkeyup="cleartext(event)" onkeydown="sendMessage(event);" Placeholder="Type a message"></asp:TextBox>
                </div>
                <div style="width: 8%; float: right; bottom: 0; margin-top: 0px">
                    <asp:ImageButton ID="ImageButton1" runat="server" ToolTip="End Chat"  ImageUrl="~/Images/close_chat.png" OnClientClick="startchat('V'); return false;" />   
                    <asp:ImageButton ID="btnSendMessage" runat="server" ImageUrl="~/Images/key_enter.png" OnClientClick="startchat('M');return false;" />
                </div>
            </div>
            <%-- <asp:Button ID="btnSendMessage" runat="server"  style="height:0px;padding:0px;border:0px;float:right;display:none; " Width="5%"
        Text="Send"  />--%>
        </div>
    </form>


<script>
        function spliterSize() {
            var myWidth = 0, myHeight = 0;
            var table = document.getElementById("txtmsg");
            //   alert(table.offsetHeight);
            if (typeof (window.innerWidth) == 'number') {
                //Non-IE
                myWidth = window.innerWidth;
                myHeight = window.innerHeight;
            } else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
                //IE 6+ in 'standards compliant mode'
                myWidth = document.documentElement.clientWidth;
                myHeight = document.documentElement.clientHeight;
            } else if (document.body && (document.body.clientWidth || document.body.clientHeight)) {
                //IE 4 compatible
                myWidth = document.body.clientWidth;
                myHeight = document.body.clientHeight;
            }
            //  window.alert('Width = ' + myWidth);
            //  window.alert('Height = ' + myHeight);
            // alert(myHeight - table.offsetHeight);


            //var height = (myHeight - table.offsetHeight);

            //document.getElementById('divdisplaytext').style.height = myHeight+'px'
            // window.document.getElementById('Radsplitter1').style.height = height + "px;bottom:0px;";
            var tableheight = document.getElementById("txtmsg").clientHeight;
            var height = (myHeight - tableheight);
            //  alert(document.getElementById("tbleheader").clientHeight);
            document.getElementById('divdisplaytext').style.height = height - 58 + 'px'


            return height - 2;
        }
        spliterSize();
        $(window).resize(function () {
            spliterSize();
        });
    </script>
</body>
</html>
