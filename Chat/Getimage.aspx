<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Getimage.aspx.cs" Inherits="Chat_Getimage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <img id="imagechat"  runat="server" style="height:74px;width:123px;margin:20px;cursor:pointer;"  />
        <img onclick="window.open('Chatlogin.aspx?queue_id=55','','width=470,height=500')" style="height:74px;width:123px;margin:20px;cursor:pointer;" id="Img1" src="image.aspx?queue_id=55">
    </div>
    </form>
</body>
</html>
