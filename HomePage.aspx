<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HomePage.aspx.cs" EnableEventValidation="false"
    EnableViewState="false" ValidateRequest="false" Inherits="HomePage" %>

<!DOCTYPE html>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="UserControls/RecentItems.ascx" TagName="RecentItems" TagPrefix="uc1" %>
<%@ Register Src="UserControls/GlobalSearch.ascx" TagName="GlobalSearch" TagPrefix="uc2" %>
<%@ Register Src="UserControls/QuickCase.ascx" TagName="QuickCase" TagPrefix="uc3" %>
<%@ Register Src="UserControls/QueueShortCut.ascx" TagName="QueueShortCut" TagPrefix="uc4" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Easyfuel- Home</title>
    <link rel="shortcut icon" href="Images/favicon.png" type="image/x-icon" />
    <link href="css/Common.css?v=1.1" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/jquery-1.4.2.min.js"></script>
     
    <style type="text/css">
        .HideControl
        {
            display: none;
        }
        html, body, form
        {
            height: 100%;
            margin: 0px;
            padding: 0px;
            overflow: auto;
            position:relative;
        }
        .RadTabStrip .rtsLink img
        {
            border: 0;
            position: relative;
            right: -2px;
            margin-top: 3px;
        }
        #rdMainPageView > div
        {
            height: 100%;
        }

        .auto-style1 {
            width: 156px;
            height: 52px;
        }

    </style>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript" language="javascript">

            var nUserId = '<%= TB_nContactID %>';
            var nTimeZoneTimeSpan = '<%= TB_nTimeZoneTimeSpan%>';
            var focused = true;
            var oldtile = document.title;
            var newtitle = "";
            var chatcount = 3;
            var promtflage = false;

            var chatinterval;

            function clearTitle() {
                
                newtitle = '';
                document.title = oldtile;
                focused = true;
                changetitle();
                clearInterval(chatinterval);
            }
            window.onfocus = function () {
                newtitle = '';
                document.title = oldtile;
                focused = true;
                changetitle();
                clearInterval(chatinterval);
            };

            window.onblur = function () {
                newtitle = '';
                focused = false;
                //changetitle();
               chatinterval = setInterval('changetitle()', 1000);
            };

            function changetitle() {
                if (focused == false) {
                    
                 //   setTimeout(function () { ChatWS.GetUnreadChat(nUserId, unreadSuccess); }, 1000);

                    if (oldtile == document.title && newtitle != '') {

                        document.title = newtitle;
                    }
                    else {
                        document.title = oldtile;
                    }
                }

            }
            function unreadSuccess(result) {
                newtitle = result;
                // document.title = newtitle;

            }
            

            function CommonHomepageTimer() {
                GetUserAlerts();
                RefreshRecentItems();
                RefreshQueue();

                ChatWS.GetExsistingRequest(nUserId, onSuccessExsistingChatRequset);
             }
            <%--function RefreshCallRequest() {
                var userName = '<%=TB_strUserName %>';
                OpenItem('DashBoard.aspx', 'View DashBoard');

                //setTimeout('RefreshCallRequest();', 3000);

            }--%>

            function RefreshCallRequest() {
            try {
                var CLI = '<%=phone%>';
                var email = '<%=session_id%>';
                var name = '<%=session_id%>'            
                if (CLI != '') {
                    //alert(CLI);
                    OpenItem('TCK/Landing.aspx?CallType=C&CLI='+ CLI +'', CLI);
                }  
            }
            catch (e) {
                alert("File - HomePages.aspx\r\nMethod -RefreshCallRequest()\n" + e.description);
            }
            }

            function onSuccessExsistingCallRequset(result) {
                var rowCount = 0;
                if (result.CallDetails.rows[rowCount] != null) {
                    var nCLI = result.CallDetails.rows[rowCount].CLI;
                    var strURL = result.CallDetails.rows[rowCount].Url;
                    OpenItem(strURL, nCLI);
                }

            }

            var gBlockPopup = '<%= TB_strShowPopup %>';
            var docwidth = 0;
            function OpenWindow(url) {
                try {
                    var v = radopen(url);
                    v.set_modal(true);

                    v.center();
                    v.setSize(300, 350); //, width,height
                    return false;
                }
                catch (e) {
                    alert("Error:- Page -> Homepage.aspx -> Method -> 'OpenWindow' \n Description:- \n" + e.description);
                    return false;
                }
            }

            var controlQueueRefreshButtonID = null;
            var controlRecentItemsRefreshButtonID = null;
            function pageLoad(sender, args) {
                try {
                    if (args._isPartialLoad == false) {
                        GetUserAlerts();
                        RefreshRecentItems();
                        RefreshQueue();
                        OpenItem('DashBoard.aspx', 'View DashBoard');
                        setInterval('chatrequestopen()', 5000);
                        RefreshCallRequest();
                   
                    }
                }
                catch (e) {
                    window.status = "Exception in PageLoad[Homepage.aspx]-" + e.description;
                    return;
                }
            }

            //--Close Session (Call Logout.aspx) on the basis of bLogout
            function TerminateSession() {
                try {

                    //if (document.getElementById('lblAgentStatusValue').innerHTML != "IDLE") {
                    //    alert("Currently Agent is not Idle status")
                    //    return false;
                    //    }
                    if (confirm('Do you really want to logout!'))
                       //PageMethods.Logout(fncLogoutSuccess, fncLogoutFailure);
                    document.location.href = "Logout.aspx";
                    return false;
                }
                catch (e) {
                    window.status = "Exception in TerminateSession[Homepage.aspx]-" + e.description;
                    return;
                }
            }

            function fncLogoutSuccess(result) {
                if (result == "fail")
                    document.location.href = "Logout.aspx";
                //        alert(resut);
            }
            function fncLogoutFailure(result) {
            }

            function ApplyChildrenImages(sender, args, param) {
                var item = args.get_item();
                var parentItem = item.get_parent();
                for (var i = 0; i < parentItem.get_items().get_count() ; i++) {
                    var img = parentItem.get_items().getItem(i).set_imageUrl("Images/unchecked.gif");
                }
            }

            function SetUserMainHeaderSettings(param, szEnabled) {
                FrameworkWS.wmSetUserHeader(nUserId, szEnabled, param, GotResponseHeader, OnError, onServerRequestTimeOut);
                return false;
            }

            function GotResponseHeader(result) {
                try {
                    var colLen = result.columns.length;
                    var responseCode = result.rows[0][result.columns[colLen - 2]];
                    var responseString = result.rows[0][result.columns[colLen - 1]];
                    if (responseCode != 0) //error
                    {
                        EnableDisable(1, false, responseString);
                        return;
                    }
                    if (result.rows[0][result.columns[0]] == null) {
                        EnableDisable(2, false);
                        return;
                    }
                }
                catch (e) {
                    alert("File - HomePage.aspx\r\nMethod - GotResponseHeader\n" + e.description);
                }
            }



            function SetUserTheme(szTheme) {
                szTheme = document.getElementById("hdnApplyTheme").value;
                FrameworkWS.SetUserTheme(nUserId, szTheme, GotResponseTheme, OnError, onServerRequestTimeOut);
                return false;
            }

            function GotResponseTheme(result) {
                try {
                    var colLen = result.columns.length;
                    var responseCode = result.rows[0][result.columns[colLen - 2]];
                    var responseString = result.rows[0][result.columns[colLen - 1]];
                    if (responseCode != 0) //error
                    {
                        EnableDisable(1, false, responseString);
                        return;
                    }
                    if (result.rows[0][result.columns[0]] == null) {
                        EnableDisable(2, false);
                        return;
                    }
                    alert('Theme changed sucessfully \n\n changes will be reflect after relogin');
                }
                catch (e) {
                    alert("File - HomePage.aspx\r\nMethod - GotResponseTheme\n" + e.description);
                }
            }

            var nPopupRefreshInterval = '<%= ConfigurationManager.AppSettings["PopRefreshInterval"] %>';


            function GetUserAlerts() {
                FrameworkWS.GetUserAlerts(nUserId, GotResponse, OnError, onServerRequestTimeOut);
                return false;
            }

            function GotResponse(result) {
                try {
                    var szMarqueeText = "";
                    var szUrl = "";
                    var colLen = result.columns.length;
                    var responseCode = result.rows[0].ResultCode;
                    var responseString = result.rows[0].ResultString;
                    if (responseCode < 0) {
                        document.getElementById("mrqUserAlerts").innerHTML = "";
                        document.getElementById("divAlertImage").innerHTML = "";
                    }
                    else {
                        for (var rowIndex = 0; rowIndex < result.rows.length; ++rowIndex) {
                            var row = result.rows[rowIndex];
                            var szText = row.subject;
                            szUrl = 'ShowObject.aspx?objectType=' + row.related_to + '&ID=' + row.related_to_id;
                            var szFontColor = "Blue";
                            var szBackColor = "White";
                            szMarqueeText += "<a style='cursor:hand;color:" + szFontColor + ";background-color:" + szBackColor + ";' id='lnkMarquee_" + rowIndex + "' onclick=\"OpenItem('" + szUrl + "');\">" + szText + "</a>&nbsp;&nbsp;";
                        }
                        document.getElementById("mrqUserAlerts").innerHTML = szMarqueeText;
                        if (szMarqueeText == "") {
                            document.getElementById("divAlertImage").innerHTML = "";
                        }
                        else {
                            document.getElementById("divAlertImage").innerHTML = "<img id='imgAlert' src='Images/alert.gif' alt='Alerts' style='cursor:hand;' onclick=OpenWindow('Alerts/AlertPopup.aspx') />";
                            if (gBlockPopup == "Y") {
                                OpenWindow('AlertPopup.aspx');
                            }
                        }
                    }
                    setTimeout('GetUserAlerts();', nPopupRefreshInterval);
                }
                catch (e) {
                    alert("File - HomePage.aspx\r\nMethod - GotResponse\n" + e.description);
                }
            }

            function EnableDisable(code, flag, data, control) {
                try {
                    if (code == 1)
                        $get("tdMessage").className = "error";
                    else
                        $get("tdMessage").className = "success";

                    if (flag == true) {
                        $get("tdMessage").innerHTML = strTemp;
                    }
                    else {
                        if (data != undefined)
                            $get("tdMessage").innerHTML = data;
                        else
                            $get("tdMessage").innerHTML = "";
                    }
                }
                catch (e) {
                    alert("File - HomePage.aspx\r\nMethod - EnableDisable\n" + e.description);
                }
            }

            function onServerRequestTimeOut() {
                alert('Server request timeout');
            }

            function OnError(args) {
                window.status = args._message;
                alert(args._message);
            }

            function rdMenu_ItemClick(sender, args) {

                var item = args.get_item();
                if (item.get_level() == "1") {
                    var itemValue = item.get_value();
                    var szEnabled = "";
                    var szparam = "";
                    if (itemValue == "Toolbar") {
                        if (document.getElementById("trToolbarMain").style.display == 'block' || document.getElementById("trToolbarMain").style.display == '') {
                            document.getElementById("trToolbarMain").style.display = 'none';
                            item.set_text("Show Toolbar");
                            szEnabled = "N";
                        }
                        else {
                            document.getElementById("trToolbarMain").style.display = 'block';
                            item.set_text("Hide Toolbar");
                            szEnabled = "Y";
                        }
                        szparam = 2;
                    }
                    else if (itemValue == "Alerts") {
                        if (document.getElementById("trAlerts").style.display == 'block' || document.getElementById("trAlerts").style.display == '') {
                            document.getElementById("trAlerts").style.display = 'none';
                            args.get_item().set_text("Show Alerts");
                            szEnabled = "N";
                        }
                       
                        else {
                            document.getElementById("trAlerts").style.display = 'block';
                            args.get_item().set_text("Hide Alerts");
                            szEnabled = "Y";
                        }
                        szparam = 3;
                    }
                    else if (itemValue == "BlockPopup") {
                        if (item.get_text() == 'Show Popup') {
                            args.get_item().set_text("Hide Popup");
                            szEnabled = "Y";
                            gBlockPopup = "Y";
                        }
                        else {
                            args.get_item().set_text("Show Popup");
                            szEnabled = "N";
                            gBlockPopup = "N";
                        }
                        szparam = 4;
                    }
                    else if (item.get_text() == "Change Password") {
                        OpenWindow(itemValue);
                    }
                    else if (itemValue == "Theme") { return; }
                    else {
                        if (args.get_item().get_value() != null && args.get_item().get_value() != "")
                            OpenItem(args.get_item().get_value(), (args._item._parent._text + ' ' + args._item.get_text()));
                    }


                    if (szparam != "")
                        SetUserMainHeaderSettings(szparam, szEnabled);
                }
                else if (item.get_level() == "2") {
                    var parentItem = item.get_parent().get_text();
                    var parentItemValue = item.get_parent().get_value();
                    var parentItemSelectedIndex = item.get_parent().get_index();
                    if (item.get_text() == "Apply") {
                        opt = confirm("Are you sure you want to save the changes?");
                        if (!opt)
                            return false;

                        SetUserTheme();
                    }
                    else if (parentItem == "Theme") {
                        document.getElementById("hdnApplyTheme").value = item.get_text();
                        ApplyChildrenImages(sender, args)
                        item.set_imageUrl("Images/checked.gif");
                        return false;
                    }
                    else if (parentItemSelectedIndex == "0") {
                        // Menu item without image? Ignore.
                        if (!item.get_imageElement()) return;
                        var urlPath = item.get_imageUrl().substr(0, item.get_imageUrl().toUpperCase().lastIndexOf("COMMONIMAGES"));

                        if (item.get_imageUrl().replace(urlPath, "") == "Images/unchecked.gif" || item.get_imageUrl().replace(urlPath, "") == "") {
                            item.set_imageUrl("Images/checked.gif");
                        }
                        else {
                            item.set_imageUrl("Images/unchecked.gif");
                        }
                    }
                }

                else {
                    if (item.get_text() == "Logout")
                        TerminateSession();
                    else if (args.get_item().get_value() != null && args.get_item().get_value() != "")
                        OpenItem(args.get_item().get_value(), (args._item._parent._text + ' ' + args._item.get_text()));
                }
                spliterresize();
            }



            function toolbarMain_ButtonClick(sender, args) {

                OpenItem(args.get_item().get_value(), (args._item.get_toolTip() + ' ' + args._item.get_text()));
            }

            function OpenItem(URL, Name) {
                try {

                    AddTab(URL, Name);
                }
                catch (e) {
                    alert("File - HomePage.aspx\r\nMethod - OpenItem\n" + e.description);
                }
            }

            function OpenQueueItem(URL, Name) {
                try {
                    if (confirm("Are you sure to self assign?") == false)
                        return;
                    OpenItem(URL, Name);
                    setTimeout('RefreshQueue();', 2000);
                }
                catch (e) {
                    alert("File - HomePage.aspx\r\nMethod - OpenQueueItem\n" + e.description);
                }
            }
            function DeleteSelectedTab(name, newname) {
                var tabStrip = $find("<%= rdMainTabStrip.ClientID %>");
                var multiPage = $find("<%= rdMainPageView.ClientID %>");

                var tab = tabStrip.findTabByText(name);
                DeleteTab(tab);
                if (newname != "") {
                    var newtab = tabStrip.findTabByText(newname);
                    if (tab) {
                        //tab.set_text(newname);
                        //                    var pageView = tab.get_pageView();
                        //                    multiPage.get_pageViews().remove(pageView);
                        //                    tabStrip.get_tabs().remove(tab);
                    }
                }
                //if (newtab) {
                //    if (newtab) {
                //        pageView = newtab.get_pageView();
                //        pageView.set_selected(true);
                //        pageView.show();
                //        newtab.set_selected(true);
                //    }
                //}
            }



            function CloseCurrentOpenNewTab(url, newname, name) {
                var tabStrip = $find("<%= rdMainTabStrip.ClientID %>");
                var multiPage = $find("<%= rdMainPageView.ClientID %>");
                AddTab(url, newname);
                var newtab = tabStrip.findTabByText(newname);
                var tab = tabStrip.findTabByText(name);
                if (tab) {
                    var pageView = tab.get_pageView();
                    multiPage.get_pageViews().remove(pageView);
                    tabStrip.get_tabs().remove(tab);
                }

                if (newtab) {
                    if (newtab) {
                        pageView = newtab.get_pageView();
                        pageView.set_selected(true);
                        pageView.show();
                        newtab.set_selected(true);
                    }
                }
            }
            var tabcount = 1;
            function AddTab(url, name) {

                var tabStrip = $find("<%= rdMainTabStrip.ClientID %>");
                var tab = tabStrip.findTabByText(name);
                if (tab) {
                    tab.set_selected(true);
                    return;
                }
                tab = new Telerik.Web.UI.RadTab();
                tab.set_text(name);

                //alert(getDocHeight());
                var multiPage = $find("<%= rdMainPageView.ClientID %>");
                    var pageView = new Telerik.Web.UI.RadPageView();
                    pageView.set_id(tabcount);
                    multiPage.get_pageViews().add(pageView);
                    tab.set_pageViewID(tabcount);

                    tabStrip.get_tabs().add(tab);
                //pageView.set_contentUrl(url);
                // added by ashish
                    var id = name.replace(" ", "_");
                    pageView.get_element().innerHTML = "<iframe id=\"" + id + "\" src=\"" + url + "\" frameBorder=\"0\" style=\"width: 100%; height: 100%;\"></iframe>";
                var height = (spliterSize() - 30);
                //pageView.get_element().style.height = height + "px";
                    pageView.get_element().style.height = "94%";
                // pageView.get_element().style.height = "690px";
                    pageView.set_selected(true);
                    tab.set_selected(true);
                    tabcount += 1;

                    AttachCloseImage(tab, "Images/close.gif");
                    return;
                }

                function getDocHeight() {
                    var D = document;
                    return Math.max(
                Math.max(D.body.scrollHeight, D.documentElement.scrollHeight),
                Math.max(D.body.offsetHeight, D.documentElement.offsetHeight),
                Math.max(D.body.clientHeight, D.documentElement.clientHeight)
            );
                }

                function CreateCloseImage(closeImageUrl) {
                    var closeImage = document.createElement("img");
                    closeImage.src = closeImageUrl;
                    closeImage.alt = "Close this tab";
                    return closeImage;
                }

                function AttachCloseImage(tab, closeImageUrl) {
                    var closeImage = CreateCloseImage(closeImageUrl);
                    closeImage.AssociatedTab = tab;
                    closeImage.onclick = function (e) {
                        if (!e) e = event;
                        if (!e.target) e = e.srcElement;

                        DeleteTab(tab);
                        e.cancelBubble = true;
                        if (e.stopPropagation) {
                            e.stopPropagation();
                        }

                        return false;
                    }
                    tab.get_innerWrapElement().appendChild(closeImage);
                }

                function DeleteTab(tab) {

                    var tabStrip = $find("<%= rdMainTabStrip.ClientID %>");
                var multiPage = $find("<%= rdMainPageView.ClientID %>");

                var tabToSelect = tab.get_nextTab();
                if (!tabToSelect)
                    tabToSelect = tab.get_previousTab();

                var pageView = tab.get_pageView();
                multiPage.get_pageViews().remove(pageView);

                var pageViews = multiPage.get_pageViews();
                for (var i = 0; i < pageViews.get_count() ; i++) {
                    pageViews.getPageView(i).hide();
                }

                tabStrip.get_tabs().remove(tab);

                if (tabToSelect)
                    tabToSelect.set_selected(true);
            }

            function RefreshRecentItems() {
                if (controlRecentItemsRefreshButtonID) {
                    $get(controlRecentItemsRefreshButtonID).click();
                }
            }

            function RefreshQueue() {
                if (controlQueueRefreshButtonID) {
                    //  $get(controlQueueRefreshButtonID).click();
                    setTimeout('RefreshQueue();', 60000);
                }
            }

            function GetNewChatRequset() {
                try {
                    setTimeout('GetNewChatRequset();', 30000);
                    GetExsistingRequest();
                }
                catch (e) {
                    alert("File - HomePage.aspx\r\nMethod - GetNewChatRequset\n" + e.description);
                }
            }
            var chatWindow = new Array();
            var userCount = 0;

            Array.prototype.exists = function (search) {
                for (var i = 0; i < this.length; i++)
                    if (this[i] !== undefined && this[i] == search) return true;

                return false;
            }
            function GetExsistingRequest() {

                ChatWS.GetExsistingRequest(nUserId, onSuccessExsistingChatRequset);
            }
            function onSuccessExsistingChatRequset(result) {

                var rowCount = 0;
                while (result.chatsession.rows[rowCount] != null) {
                    var DestinationID = result.chatsession.rows[rowCount].DestinationID;
                    var SourceID = result.chatsession.rows[rowCount].SourceID;
                    var SeesionID = result.chatsession.rows[rowCount].SeesionID;
                    var type = result.chatsession.rows[rowCount].chat_type;
                    if (nUserId == SourceID || DestinationID == nUserId) {
                        var searchResult = chatWindow.exists(SeesionID);
                        if (searchResult == false) {

                            var url = "Chat/Chatcustomer.aspx?type=" + type + "&chatsessionid=" + SeesionID;
                            //  var url = "Chat/Chatbox.aspx?chatsessionid=" + SeesionID;

                            var v = radopen(url);
                            v.set_modal(false);
                            v.moveTo(400, 600);
                            v.setSize(300, 350); //, width,height
                            v.add_close(onPromptWindowClose);

                            ChatWS.ModifyChatEndTime(SeesionID);

                            chatWindow[userCount] = SeesionID;
                            userCount++;
                        }
                    }
                    rowCount++;
                }
            }
            var count = 0;

            var chatindex = 0;
            function onPromptWindowClose(oWnd) {
                //console.log("chat id---------------" + oWnd.argument.ChatID);
                try {

                    if (oWnd.argument != null && oWnd.argument != undefined) {
                        for (var i = 0; i < chatWindow.length; i++) {
                            //console.log("chatWindow-" + i + "---" + chatWindow[i]);
                            if (chatWindow[i] !== undefined && chatWindow[i] == oWnd.argument.ChatID) {
                                //console.log("delete-" + i + "---" + chatWindow[i]);
                                if (!isNaN(oWnd.argument.ChatID)) {
                                    var szcontactid = '<%=TB_nContactID %>';
                                    var szchatid = oWnd.argument.ChatID;
                                   // ChatWS.DeclineChatRequest(szchatid, szcontactid, onSuccessClosedChat)
                                }
                                else {
                                    ChatWS.update_chat_status(oWnd.argument.ChatID, 'Closed', onSuccessClosedChat)
                                }
                                chatindex = i;

                                //delete chatWindow[i];
                                for (var j = i; j < chatWindow.length - 1; j++) {
                                    chatWindow[j] = chatWindow[j++]
                                }
                                promtflage = true;
                                chatWindow.pop();
                                userCount--;
                                //  chatWindow[i] = 0;
                                //console.log("delete-" + i + "---chatWindow length------" + chatWindow.length);
                              //  alert("userCount----------------" + userCount + "---------------------chatcount------------------" + chatcount);
                                break;
                            }
                        }
                        // //console.log(chatWindow[0], chatWindow[1]);
                    }
                    relocatateposition();
                    return false;
                }
                catch (e) {
                    alert("Error :- Page:- \"Homepage.aspx\" Method:- 'onPromptWindowClose' Description:- " + e.description);
                    return false;
                }
            }
            function onSuccessClosedChat(result) {
                //write code 
            }
            function updateArrayonAccept(sessionid) {
                chatWindow[userCount] = sessionid;
                userCount++;
            }
            function chatrequestopen() {
              
                if (document.getElementById('lblAgentStatusValue').innerHTML != "BUSY") {
                    if (userCount < chatcount)
                        ChatWS.open_new_Chat_Entry(nUserId,nTimeZoneTimeSpan, onsucesschat1);
                    //      ChatWS.GetChatQueueRequest(nUserId, onSuccessChatRequset);
                }
            }
            function onsucesschat1(result) {
                if (promtflage == true) {
                    promtflage = false;
                    return;
                }
                //console.log("----START onsucesschat1");
                var responseCode = result.Response.rows[0].ResultCode;
                var responseString = result.Response.rows[0].ResultString;
              
                if (responseCode == "-1")// error
                {
                    alert("Error :- Page:- \"Homepage.aspx\" Method:- 'Insertchatrequest' Description:- " + e.description);
                    return false;
                }
                else {

                    var items = result.chatrequest;
                    //   alert(items.rows.length);
                    var data = [];
                    for (var i = 0; i < items.rows.length; i++) {

                        var item = items.rows[i];

                        var chat_session_id = item.chat_session_id;
                        //console.log("chat_session_id--" + chat_session_id);
                        var chat_destination_id = item.chat_destination_id;
                        //console.log("chat_destination_id--" + chat_destination_id);
                        var sessionid = chat_session_id;
                        //console.log("sessionid--" + sessionid);
                        var type = item.chat_type;
                        var url = 'chat/chatcustomer.aspx?type=' + type + '&chatsessionid=' + sessionid;

                        var searchResult = chatWindow.exists(chat_session_id);
                        //console.log("searchResult--" + searchResult)
                        if (searchResult == false && promtflage==false) {
                         
                            //console.log("url--" + url + ", userCount-" + userCount + ",sessionid--" + sessionid);
                            OpenWindow1(url);

                            chatWindow[userCount] = sessionid;
                            userCount++;
                        }

                    }
                    //for Visiter chat request

                    var Response = result.ChatRequest;
                    if (Response == null)
                        return;
                  
                    var Position = 900;
                  
                    for (var j = 0; j < Response.rows.length; j++) {

                      
                       
                        //console.log("Visitor request::: loop start");
                        var ChatID = Response.rows[j].chat_visitor_id;
                        var VisitorName = Response.rows[j].chat_visitor_name;
                        var VisitorQuery = Response.rows[j].chat_visitor_query;
                        var CreatedDate = Response.rows[j].chat_request_time;
                        var chatque_email = Response.rows[j].chat_visitor_email;
                        var chatque_mobile = Response.rows[j].chat_visitor_phone;
                        var product_id = Response.rows[j].product_id;
                        ChatSessionID = Response.rows[j].chat_session_id;
                       
                        //console.log("Visitor request::: ChatID-----" + ChatID);
                        //console.log("Visitor request::: ChatSessionID-----" + ChatSessionID);

                        var searchResult = chatWindow.exists(ChatSessionID);
                        //console.log("Visitor request::: ChatSessionID exists-----" + searchResult);

                        if (searchResult == false) {
                            searchResult = chatWindow.exists(ChatID);
                            //console.log("Visitor request::: ChatSessionID not exists-----" + searchResult);
                        }

                        if (searchResult == false && Response.rows[j].chat_response_type == 'N' && ChatSessionID == null && promtflage == false) {
                           
                              var url = "Chat/ChatPrompt.aspx?id=" + ChatID + "&CallType=C&CLI=" + chatque_mobile + "&email=" + chatque_email + "&vname=" + VisitorName +"&product="+product_id+ "&vquery=" + VisitorQuery + "&cdate=" + CreatedDate;
                            //  alert(url);
                          //    console.log("Visitor request::: url-----" + url + "   userCount:::" + userCount + "---ChatID::::" + ChatID);
                            OpenWindow1(url);
                            chatWindow[userCount] = ChatID;
                            userCount++;
                            newtitle = "New Chat Request";
                            //console.log("focused-----" + focused);
                          //  if (focused == false) {
                                //console.log("focused-----" + newtitle);
                                    document.title = newtitle;
                            //}
                        }
                        //console.log("Visitor request::: loop end");
                    }
                }
            }

            function Insertchatrequest(sourceid, destinationid) {
                if (userCount < chatcount)
                    ChatWS.Save_new_Chat_Entry(sourceid, destinationid, onsucesschat)
            }

            function onsucesschat(result) {
                var responseCode = result.Response.rows[0].ResultCode;
                var responseString = result.Response.rows[0].ResultString;
                if (responseCode == "-1")// error
                {
                    alert("Error :- Page:- \"Homepage.aspx\" Method:- 'Insertchatrequest' Description:- " + e.description);
                    return false;
                }
                else {
                    var sessionid = responseString;
                    var url = 'chat/chatcustomer.aspx?type=I&chatsessionid=' + sessionid;
                    //  var url = 'chat/chatbox.aspx?chatsessionid=' + sessionid;
                    var searchResult = chatWindow.exists(responseString);
                    if (searchResult == false) {
                        OpenWindow1(url);
                        chatWindow[userCount] = sessionid;
                        userCount++;
                    }
                }
            }
            function onFailed(result) {
                try {

                }
                catch (e) {

                }
            }

            function OpenWindow1(url) {
                try {
                    var v = radopen(url);
                    v.set_modal(false);
                    v.center();
                    v.setSize(300, 350); //, width,height
                    PositionWindow(v);
                    v.add_close(onPromptWindowClose);
                    return false;
                }
                catch (e) {
                    alert("Error:- Page -> Homepage.aspx -> Method -> 'OpenWindow' \n Description:- \n" + e.description);
                    return false;
                }
            }
            function PositionWindow(oWindow) {
                // var oWindow = GetSelectedWindow();
                if (!oWindow) {
                    return;
                }

                var Y = document.body.clientHeight + document.body.scrollTop - oWindow.GetHeight();
                var X = document.body.clientWidth + document.body.scrollLeft - oWindow.GetWidth() - docwidth;
                //  alert("document.body.clientWidth: " + document.body.clientWidth + ",document.body.scrollLeft:" + document.body.scrollLeft + "- oWindow.GetWidth():" + oWindow.GetWidth() + " - docwidth:" + docwidth);
                var windowidth = oWindow.GetWidth();
                docwidth = docwidth + windowidth;
                //  alert("X:" + X + "  Y:" + Y);
                Y = (Y > 0) ? Y : 0;
                X = (X > 0) ? X : 0;
                oWindow.MoveTo(X, Y);
            }
            var tempdocwidth = 0;
            function relocatateposition() {
                var oManager = GetRadWindowManager();
                var windows = oManager.get_windows();
                tempdocwidth = docwidth;
                if (windows.length > 0) {
                    docwidth = docwidth - windows[0].GetWidth();
                }
                // alert(chatindex);
                for (var i = windows.length; i > chatindex; i--) {
                    //  alert(chatindex);
                    PositionWindow1(windows[i - 1]);
                }

            }
            function PositionWindow1(oWindow) {
                // var oWindow = GetSelectedWindow();
                if (!oWindow) {
                    return;
                }

                var Y = document.body.clientHeight + document.body.scrollTop - oWindow.GetHeight();
                //  alert("document.body.clientWidth" + document.body.clientWidth);
                var windowidth = oWindow.GetWidth();

                tempdocwidth = tempdocwidth - windowidth;
                //        var X = document.body.clientWidth + document.body.scrollLeft - oWindow.GetWidth() - docwidth;
                var X = document.body.clientWidth + document.body.scrollLeft - tempdocwidth;

                // alert('X: '+X+'  Y :  '+Y);

                // alert(tempdocwidth);
                Y = (Y > 0) ? Y : 0;
                X = (X > 0) ? X : 0;
                oWindow.MoveTo(X, Y);
            }
            function spliterSize() {
                var myWidth = 0, myHeight = 0;
                var table = document.getElementById("tbleheader");
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
                //window.alert('Width = ' + myWidth);
                //window.alert('Height = ' + myHeight);
                // alert(myHeight - table.offsetHeight);


                //var height = (myHeight - table.offsetHeight);

                // document.getElementById('divmain').style.height = myHeight+'px'
                // window.document.getElementById('Radsplitter1').style.height = height + "px;bottom:0px;";
                var tableheight = document.getElementById("tbleheader").clientHeight;
                var tblivr = document.getElementById("tblivr").clientHeight;
                var trhide = document.getElementById("trhide").clientHeight;

                var height = (myHeight - tableheight - tblivr - trhide);
                //  alert(document.getElementById("tbleheader").clientHeight);
                return height - 2;
            }
            function SplitterLoaded(splitter, arg) {
                var pane = splitter.getPaneById('<%= LeftPane.ClientID %>');
                //  //var height = pane.getContentElement().scrollHeight;
                var height = spliterSize();
                splitter.set_height(height);
                pane.set_height(height);
            }


            function spliterresize() {
                var splitter1 = $find('<%=Radsplitter1.ClientID%>')
                var pane = splitter1.getPaneById('<%= LeftPane.ClientID %>');
                //var height = pane.getContentElement().scrollHeight;
                var height = spliterSize();
                splitter1.set_height(height);
                pane.set_height(height);


            }
            function OpenWindowPopup(url, height, width) {
                try {

                    var v = radopen(url);
                    v.set_modal(true);
                    
                    v.center();
                    v.MoveTo(100, 100);
                    v.setSize(height, width); //height, width
                    //   v.add_close(onWindowPopupClose);
                    v.add_close(onWindowPopupClose);
                    
                    return false;
                }
                catch (e) {
                    alert("Error:- Page -> HomePage.aspx -> Method -> 'OpenWindowPopup' \n Description:- \n" + e.description);
                    return false;
                }
            }
            function onWindowPopupClose(oWnd) {
                try {
                    if (oWnd.argument != null) {

                    }
                    return false;
                }
                catch (e) {
                    alert("Error :- Page:- \"HomePage.aspx\" Method:- 'onWindowPopupClose' Description:- " + e.description);
                    return false;
                }
            }
        </script>

        <script type="text/javascript" language="javascript">
            function btnCTIHomeDisable() {
                window.frames["ifCtiAgent"].btnWrapcall();
            }
            // get DNI
            function getDNI() {
                return window.frames["ifCtiAgent"].GetDNI();
            }
            // Save CRM Value using CTI_Panel
            function getAgentCRMSave(DispCode, Bucket, xCallType, Code, strDate, CallNumber, gServiceId, remark) {
                try {

                    return document.getElementById(CallNumber).contentWindow.SaveCRMByCTI1(DispCode, Bucket, xCallType, Code, strDate, CallNumber, gServiceId, remark);

                }
                catch (e) {
                    alert("Error:- Page -> Homepage.aspx -> Method -> 'OpenWindow' \n Description:- \n" + e.description);
                    return false;
                }
            }
            function removeIFrame(CallNumber) {
                alert(CallNumber);

                var frame = document.getElementById(CallNumber);
                alert(frame);
                frame.parentNode.removeChild(frame);
            }
            function fillIvr(url) {
                var assoc = parseQueryString(url);
            }

            function parseQueryString(url) {

                var q = url.split("?")
                if (q.length <= 1)
                    return;
                //alert(q[1]);


                var qs = {};
                if (q[1].length) {
                    var keys = q[1].split("&"), k, kv, key, val, v;
                    // alert("keys-----------" + keys);
                    for (k = keys.length; k--;) {
                        kv = keys[k].split("=");
                        key = kv[0];
                        //alert("key----" + key);
                        val = decodeURIComponent(kv[1]);
                        //alert("val------------" + val);
                        IVRDetails(key, val);
                    }
                }
            }

            function IVRDetails(key, value) {
                switch (key) {
                    case "Menu":
                        document.getElementById('lblMenu').innerHTML = value;
                        break;
                    case "CLI":
                        document.getElementById('lblCLI').innerHTML = value;
                        break;
                    case "Language":
                        document.getElementById('lblLanguage').innerHTML = value;
                        break;
                    case "CallType":
                        var tblivr = document.getElementById('tblivr');
                        if (value == '<%= configcalltype %>') {

                            tblivr.style.display = 'block'
                            tblivr.style.width = '100%'
                        }
                        else
                            tblivr.style.display = 'none'
                        spliterresize();
                        break;
                    default:
                        "";
                }
            }

            function DeleteTabByCTI(tab) {
                // alert('delete tab' + tab);
                return tab;
            }


            function showheader() {
                document.getElementById('tblivr').style.display = 'block';
                document.getElementById('trhide').style.display = 'none';
                spliterresize();
            }

            function Hideheader() {
                document.getElementById('tblivr').style.display = 'none';
                document.getElementById('trhide').style.display = 'block';
                spliterresize();
            }
            $(window).resize(function () {
                spliterresize();
            });
        </script>

    </telerik:RadCodeBlock>
</head>
<body id="test" runat="server" class="bodyColor">
    <form id="form1" runat="server">

    <asp:ScriptManager ID="sm" runat="server" EnablePageMethods ="true">
        <Services>
            <asp:ServiceReference Path="~/Services/FrameworkWS.asmx" />
            <asp:ServiceReference Path="~/Services/ChatWS.asmx" />
            <asp:ServiceReference Path="~/Services/CtiWS.asmx" />
        </Services>
        <%--   <Scripts>
        <asp:ScriptReference Path="~/Scripts/HomePage.js" />
        </Scripts>--%>
    </asp:ScriptManager>

    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnRefreshRecent">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="uclRecentItems" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnRefreshQueue">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="uclQueue" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <div style="width: 100%;height:100%" id="divmain">
        <table width="100%" id="tbleheader">
            <tr id="trMainMenu">
                <td valign="top">
                    <img id="imgLogo" runat="server" style="vertical-align: top;" alt="Logo" src="" />
                </td>
                <td id="tdMessage" valign="top" align="center">
                    <div style="margin-top: 7px; margin-left: 100px;">
                        <div id="divAlertImage" style="float: left;">
                        </div>
                        <div style="float: left; margin-left: 7px;">
                            <asp:Label ID="lblUserInfo" runat="server" CssClass="HeaderUserLoginInfo"></asp:Label>
                            <asp:Label ID="lblUserLoginDate" runat="server" CssClass="HeaderUserLoginInfo"></asp:Label>
                            <asp:Label ID="lblMessage" runat="server"></asp:Label>
                        </div>
                    </div>
                </td>
                <td valign="top" colspan="2" style="float: right;">
                    <telerik:RadMenu ID="rdMenu" Width="100%" EnableOverlay="true" EnableShadows="true"
                        runat="server" OnClientItemClicked="rdMenu_ItemClick">
                    </telerik:RadMenu>
                   <%--  <img src="Images/genesis.png" align="right" class="auto-style1" />--%>
                  
                </td>
            </tr>
            <tr >
                <td valign="top" colspan="4" >
                    <span id="trToolbarMain" runat="server" >
                    <telerik:RadToolBar ID="toolbarMain" runat="server" Width="100%" OnClientButtonClicked="toolbarMain_ButtonClick">
                    </telerik:RadToolBar>
                        </span>
                </td>
            </tr>
            <tr style="display:none">
                <td class="trMarquee" align="right" valign="top" colspan="4">
                  
                </td>
            </tr>
            
            <tr style="display: none">
                <td style="text-align: right; width: 60%">
                    <asp:Label ID="lblAgentName" runat="server" ForeColor="Black" CssClass="HeaderUserLoginInfo"
                        Font-Bold="true"></asp:Label>
                    :
                </td>
                <td style="text-align: left; width: 40%">
                    <asp:Label ID="lblAgentStatusValue" runat="server" Width="35%" Text="IDLE" CssClass="HeaderUserLoginInfo"
                        Font-Bold="true" Style="display: block"></asp:Label>
                </td>
            </tr>
            
        </table>
        <div class="trMarquee">
              <span  id="trAlerts" runat="server" style="display: none;">
                    <marquee id="mrqUserAlerts" direction="left" width="100%" scrolldelay="150" onmouseout="this.start()"
                        onmouseover="this.stop()"></marquee>
                        </span>
        </div>
       <div style="margin-left:auto;margin-right:auto">
            <div id="trhide"  onclick="showheader()" style="display:block;width:50px;height:5px;cursor:pointer; text-align:center;background-image:url(Images/dotheader.png);background-repeat:no-repeat;" ></div>
        </div>
        <div style="width:100%;display:block;background-color:#fafaa8;height:22px;vertical-align:middle" class="tdItem" id="tblivr"  onclick="Hideheader()">
            <div style="width:10%;float:left;padding-top:3px;padding-left:20px;">
            <span style="width:10%">CLI :</span>
            <asp:Label ID="lblCLI" runat="server" Text="" ></asp:Label>
            </div>
            <div style="width:10%;float:left;padding-top:3px;padding-left:15px;">
            <span style="width:10%">Agent Id :</span>
            <asp:Label ID="lblAgentId" runat="server"  Text=""></asp:Label>
            </div>
              <div style="width:10%;float:left;padding-top:3px;padding-left:15px;">
            <span style="width:10%">Terminal :</span>
            <asp:Label ID="lblTerminal" runat="server"  Text=""></asp:Label>
            </div>
              <div style="width:10%;float:left;padding-top:3px;padding-left:15px;">
            <span style="width:10%">Call Id :</span>
            <asp:Label ID="lblLeadId" runat="server"  Text=""></asp:Label>
            </div>
              <div style="width:10%;float:left;padding-top:3px;padding-left:15px;">
            <span style="width:10%">Campaign :</span>
            <asp:Label ID="lblCampaign" runat="server" Text=""></asp:Label>
            </div>
          

            <div style="clear:both"></div>
        </div>

  
        <telerik:RadSplitter ID="Radsplitter1" SplitBarsSize="1px" BackColor="#4E8AA4" Orientation="Vertical"
            runat="server" Width="100%" VisibleDuringInit="false" OnClientLoaded="SplitterLoaded">
            <telerik:RadPane ID="LeftPane" runat="server" Width="22px" Scrolling="None">

            </telerik:RadPane>
           
            <telerik:RadPane ID="rdpRightPane" Scrolling="None" runat="server" Width="94%" Height="100%" >
              <table width="100%" >
                    <tr style="height: 30px">
                        <td>
                            <telerik:RadTabStrip ID="rdMainTabStrip" runat="server" MultiPageID="rdMainPageView"
                                SelectedIndex="0" Align="Left">
                            </telerik:RadTabStrip>
                        </td>
                    </tr>
          
                </table>
          
                <div style="height: 100%; vertical-align: top; background-color:white">
                     <telerik:RadMultiPage ID="rdMainPageView" runat="server" Style="height: 100%; background-image:url();background-position:center; background-repeat:no-repeat">
                                   
                                <telerik:RadPageView ID="RadPageView1" runat="server"  ContentUrl="" >
                                   
                                 </telerik:RadPageView>
                            </telerik:RadMultiPage>
                </div>
            </telerik:RadPane>
            <%-- <telerik:RadPane ID="rdpRightPane1" Scrolling="None" runat="server" Width="1010px">
                <iframe id="IFrame1" runat="server" frameborder="0" name="IFrame1" scrolling="no"
                    width="100%" height="100%" src ="AgentCRM.aspx"></iframe>
            </telerik:RadPane>--%>
        </telerik:RadSplitter>
        <%--  </telerik:RadPane>
        </telerik:RadSplitter>--%>
    </div>
    <telerik:RadWindowManager VisibleStatusbar="false" DestroyOnClose="true" 
        ID="RadWindowManager1" runat="server" Modal="true" Animation="None" Behaviors="Close,Minimize">
    </telerik:RadWindowManager>
    <asp:HiddenField ID="hdnApplyTheme" runat="server" />
     <asp:UpdatePanel ID="UPtimer" runat="server" RenderMode="Inline" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Timer ID="tmrupdate" runat="server" OnTick="tmrupdate_Tick" Interval="30000">
            </asp:Timer>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
