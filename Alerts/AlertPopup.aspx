<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AlertPopup.aspx.cs" Inherits="Alerts_AlertPopup" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Alert Popup</title>
    <link href="../CommonStyleSheet/Common.css" rel="stylesheet" type="text/css" />
</head>
<script type="text/javascript" language="javascript">
    var nSessionID = 0;
    
    function OpenCreateAlert(subject,senderid,sendername)
    {
        var szTemp = "";
        nSessionID = '<%=TB_nSessionID %>';
        szTemp = "?Subject="+subject+"&UserId="+
                 senderid+"&UserName="+sendername+"&AlertId=1"+"&AType=B";

        window.location.href = "../Alerts/CreateAlerts.aspx"+szTemp, "Create", 'height=300px,width=500px,toolbar=0,resizable=0,scrollbars=1';
    }
    function Dismiss(chkBoxID)
    {
        document.getElementById(chkBoxID).checked = true;
        GetSelectedAlert(3);
    }
    function GetSelectedAlert(param) 
    {   
        var opt;
		var tblAlerts = $get("tblAlertDetails");
		var nRowCount = tblAlerts.rows.length
		var szUncheckAlertIds = "";
		var j;		
		var nCheckCount = 0;
		
		var nUserId = 2;
		var nAlertMasterId = 2;	
		var nSnoozeTime = document.getElementById("<%=cmbSnoozeTime.ClientID %>").value;
		var szAlertIds = "";		
	    for(j=0;j<nRowCount;j++)
	    {
		    itemCheck1="chkSnooze"+j;			
		    if(document.all[itemCheck1]!=undefined)
		    {       
		        if(document.all[itemCheck1].checked==true)
			    {  
                      szAlertIds += document.all[itemCheck1].name+",";             			         
			    }
			    else if((document.all[itemCheck1].checked==false) && (param==2 || param== 4)) 
			    {
			        szUncheckAlertIds += document.all[itemCheck1].name+",";             			         
			    }
		    }
	    }
		
		if(szAlertIds != "")
		    szAlertIds = szAlertIds.substr(0,szAlertIds.lastIndexOf(","))
		else
		    szAlertIds = "0";
		if(szUncheckAlertIds != "")
		    szUncheckAlertIds = szUncheckAlertIds.substr(0,szUncheckAlertIds.lastIndexOf(","))
		else
		    szUncheckAlertIds = "0";
		if(param==2 || param== 4)    
		{
		    szAlertIds += ","+szUncheckAlertIds;
		}

		FrameworkWS.wmAlertPopupSetStatus(nAlertMasterId, nUserId, nSnoozeTime, szAlertIds, param, GotResponse, OnError, onServerRequestTimeOut);			
		self.close();
		return false;	
    }    
    function GotResponse(result)
    {
        try
        {            
            var colLen = result.columns.length;            
            var responseCode = result.rows[0][result.columns[colLen-2]];
            var responseString = result.rows[0][result.columns[colLen-1]];            
            if(responseCode != 0) //error
            {
               EnableDisable(1, false, responseString);
               return;
            }
            if(result.rows[0][result.columns[0]] == null)
            {
               EnableDisable(2, false);
               return; 
            }
            //alert(responseString);            
        }
        catch(e)
        {   
            alert("File - AlertPopup.aspx\r\nMethod - GotResponse\n" + e.description);
        }              
    }
    function EnableDisable(code, flag, data, control)
    {
        try
        {    
            if(code == 1)
                $get("tdMessage").className = "error";
            else
                $get("tdMessage").className = "success";            
            
            if(flag == true)
            {                
                $get("tdMessage").innerHTML = strTemp;
            }
            else
            {
                if(data != undefined)
                    $get("tdMessage").innerHTML = data;
                else
                    $get("tdMessage").innerHTML = "";
            }
        }
        catch(e)
        {
	        alert("File - AlertPopup.aspx\r\nMethod - EnableDisable\n" + e.description);
        }
    }
    function onServerRequestTimeOut()
    {
       alert('Server request timeout'); 
    }
    function OnError(args)
    {
        alert(args.toString());
    }    
    function checkedAll () 
    { 
        var tblAlerts = $get("tblAlertDetails");
		var nRowCount = tblAlerts.rows.length			 
	    var itemCheck = ""; 	   
        for(j=0;j<nRowCount;j++)
        {
	        itemCheck="chkSnooze"+j;		       	
	        if(document.all[itemCheck]!=undefined)
	        {       
	            if ($get("checkall").checked == true)
                {
	                document.all[itemCheck].checked=true;
	            }
	            else
	            {
	                document.all[itemCheck].checked=false;	                
	            }
	        }
        }         
    }

</script>


<body>
    <form id="frmAlertPopup" runat="server">
    <asp:ScriptManager ID="smMain" runat="server">
        <Services>
            <asp:ServiceReference Path="~/Services/FrameworkWS.asmx" />
        </Services>
    </asp:ScriptManager>    
        <table width="100%" style="margin:0px;">
           <%-- <tr>
                <td colspan="2" class="tdHeader1"> 
                <input type="checkbox" name="checkall" id="checkall" title="Select All"  onclick="javascript:checkedAll();"/>
                Select All                
                </td>
            </tr>--%>
            <tr>
                <td align="left">
                    <asp:Button ID="btnDismiss" runat="server" OnClientClick="return GetSelectedAlert(3);"
                        Text="Dismiss" />
                    <asp:Button ID="btnDismissAll" runat="server" OnClientClick="return GetSelectedAlert(4);"
                        Text="Dismiss All" />
                    </td><td align="right">
                    <telerik:RadComboBox Width="50px" ID="cmbSnoozeTime" runat="server">
                        <Items>
                            <telerik:RadComboBoxItem Text="00" Value="00" />
                            <telerik:RadComboBoxItem Text="05" Value="05" />
                            <telerik:RadComboBoxItem Text="10" Value="10" />
                            <telerik:RadComboBoxItem Text="15" Value="15" />
                            <telerik:RadComboBoxItem Text="20" Value="20" />
                        </Items>
                    </telerik:RadComboBox>&nbsp;
                    <asp:Button ID="btnSnooze" runat="server" OnClientClick="return GetSelectedAlert(1);"
                        Text="Snooze" />
                    <asp:Button ID="btnSnoozeAll" runat="server" OnClientClick='return GetSelectedAlert(2);'
                        Text="Snooze All" />
                        
                        <asp:Button ID="btnClose" runat="server" OnClientClick="javascript:self.close();"
                        Text="Close" Width="60px" />
                </td>
            </tr>
            <tr>
                <td colspan="2" id="tdMessage">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    <asp:Table ID="tblAlertDetails" CellSpacing="1" 
                    CellPadding="1" Width="100%" runat="server">
                    </asp:Table>
                </td>
            </tr>            
        </table>
    </form>
</body>
</html>
