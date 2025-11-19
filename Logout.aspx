<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Logout.aspx.cs" Inherits="Logout" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <link href="CSS/VLOGCSS.css" type="text/css" rel="stylesheet" />
    <title>Logout</title>
    <script language="javascript">
    window.history.forward(1);
    function Login()
    {
        window.parent.parent.location.href="Login.aspx";
    }
    function CloseWindow()
    {
        window.open('','_parent','');
        window.close();
    }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <center>
            <div id="divMessage" runat="server" style="font-size: medium; color: Red;">Thank you for using Easyfuel CRM !! <br />
                 You have been successfully logged out.
            </div>
            <div id="divLnk" runat="server">
                <a href="Login.aspx" onclick="return Login();">Login Again!</a>                
            </div>
            <div>
            <img src="Images/Cancel.gif" onclick="CloseWindow()" alt="Close Window" style="cursor:hand;" />
            </div>
        </center>        
    </form>
</body>
</html>
