<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Image.aspx.cs" Inherits="Chat_Image" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="height:150px;">
       <img id="imagechat"   runat="server" style="height:74px;width:123px;margin:20px;cursor:pointer;" />
         <asp:Label ID="lblMessage" runat="server"></asp:Label>
        
    </div>
    </form>
</body>
</html>
