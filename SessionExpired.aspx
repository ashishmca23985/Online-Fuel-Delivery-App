<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SessionExpired.aspx.cs" Inherits="SessionExpired" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Session Expired</title>    
</head>
<script type="text/javascript">
    function Login() {
        window.parent.parent.location.href = "Login.aspx";
    }
</script>
<body>
    <form id="form1" runat="server">
    <table width="100%" align="center">
			<tr>
				<td align="center">
				    <font color="#ff0033" style="font-size:12px"><b>Sorry ! your session has been expired. Please Login Again.</b></font>
				</td>
			</tr>
			<tr>
				<td align="center">
    				<a id="lnkLogin" href="Login.aspx" onclick="return Login();"><font color="Blue" size="3"><b>Login Again!</b></font></a>
				</td>
			</tr>
		</table>
    </form>
</body>
</html>
