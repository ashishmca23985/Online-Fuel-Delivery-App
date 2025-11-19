<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GalaxyConfig.aspx.cs" Inherits="GalaxyConfig" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .pages-grids {
border-collapse: separate;
border-spacing: 1px;
width:99%;
margin-left:1%; 
}

.pages-grids th {
font-family: "Trebuchet MS";
font-size: 8pt;
color: #ffffff;
padding: 6px 6px;
border: solid 1px #b3c6cd;
background-color:#A4A4A4;
background-position: bottom;
background-repeat: repeat-x repeat-y;
text-align: left;
}
.pages-grids tr {

/*height:14px;*/
}
.pages-grids tr:hover {
background-color:#D8D8D8;
}
.pages-grids td
 {
font-family: "Trebuchet MS";
font-size: 12px;
color: #8A0808;
padding: 6px 6px;
border: solid 0px #dedede;
border-bottom: solid 1px #dedede;
background-color:#D8D8D8;
text-align: left;

}
.pages-grids font {
color: #de0000;
font-size: 11px;
line-height: 14px;
}

.pages-grids a {
font-family: "Microsoft Sans Serif";
font-size: 12px;
color: #1a5296;
text-decoration: underline;
}

.pages-grids a:hover {
color: #454545;
}
    </style>
</head>

<body>
    <form runat="server" id="form1">
        <asp:ScriptManager ID="smScriptManager" runat="server" /> 
        <asp:UpdatePanel ID="bpBannerPanel" runat="server" UpdateMode="Conditional"> 
            <ContentTemplate>
            <div id="dv" runat="server" align="center"></div>
            <asp:Timer ID="tmrTimer" OnTick="tmrTimer_Tick" runat="server" Interval="3000" /> 
            </ContentTemplate>
        </asp:UpdatePanel>
        
    </form>
</body>
</html>
