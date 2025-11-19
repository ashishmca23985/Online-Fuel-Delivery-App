<%@ Page Language="C#"  AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="LoginPage" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Login</title>
    <link rel="shortcut icon" href="Images/favicon.ico" type="image/x-icon" />
    <link href="CommonStyleSheet/Common.css" rel="stylesheet" type="text/css" />

      <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript" language="javascript">
        window.history.forward(1);


        function OnSaveSuccess(CreateNewObject, message) {
            try {
                if (message != '')
                    alert(message);
                if (window.parent != null && window.parent.OnObjectSave != null)
                    window.parent.OnObjectSave(CreateNewObject);
            }
            catch (e) {
                alert("File - Login.aspx\r\nMethod -OnSaveSuccess()\n" + e.description);
            }
        }

        // added by ashish for check password Ist latter shoud be character and Number ...
        function CheckPassword(inputtxt) {
            var passw = /^[A-Za-z]\w{6,14}$/;
            if (inputtxt.value.match(passw)) {
                alert('Correct, try another...')
                return true;
            }
            else {
                alert('Wrong...!')
                return false;
            }
        }

        // added by ashish 
        function validateSave() {

            try {
              
                var Username = document.getElementById("<%= ctlLogin.FindControl("UserName").ClientID %>");
                var Password = document.getElementById("<%= ctlLogin.FindControl("Password").ClientID %>");
             <%--   var Terminal = document.getElementById("<%= ctlLogin.FindControl("Terminal").ClientID %>");
             --%> 
                if (Username.value == "" || Username.value == null)
                {
                    alert('Please Enter User Name !');
                    Username.focus();
                    return false;
                }
                if (Password.value == "" || Password.value == null)
                {
                    alert('Please Enter Password !');
                    Password.focus();
                    return false;
                }

                //if (Terminal.value == "" || Terminal.value == null) {
                //    alert('Please Enter Terminal !');
                //    Password.focus();
                //    return false;
                //}

                }
            catch (e) {
                alert("File - Login.aspx\r\nMethod -OnSaveSuccess()\n" + e.description);
                return false;
            }
        }

   <%--     function validateTerminal() {

            try {

                var Terminal = document.getElementById("<%= ctlLogin.FindControl("Terminal").ClientID %>");

                if (Terminal.value == "" || Terminal.value == null) {
                    alert('Please Enter Terminal !');
                    Terminal.focus();
                    return false;
                }
            }
            catch (e) {
                alert("File - Login.aspx\r\nMethod -OnSaveSuccess()\n" + e.description);
                return false;
            }
        }--%>

        
    </script>
          </telerik:RadCodeBlock>
    
    <style type="text/css">
        .auto-style1 {
            margin-left: 0px;
            width: 145px;
            height: 72px;
        }
        .auto-style2 {
            width: 243px;
            height: 72px;
        }
    </style>
    
    </head>
    <body class="LoginBackground" style="background:url('Images/back3.jpg'); background-repeat:round  ">

    <form id="form1" runat="server">
    <asp:ScriptManager ID="RetrievalScriptManager" runat="server">
    </asp:ScriptManager>
    <table width="100%">        
        <tr>        
            <td width="38%" align="left">
                <table width="100%" class="LoginTextBackground" style="margin-top:0px;">
                 <tr>
                <th align="center" style="text-align: left">
                    <img src="Images/Growvallogo.png" class="auto-style2" /></th>
                <th  style="text-align: right">
                    
                    <img src="Images/easyfuellogo.png" class="auto-style1" /></tr>
                 <tr>
              
                <th align="center" colspan="2">
                    <br />
                    <br />
                    <br />
                    <br />
                     </th>
                </tr>
                <tr>
                <th align="center" style="font-size:55pt; font-family:Garamond; color:#0000cd;" colspan="2">
                      &nbsp;</th>                
                </tr>
                    <tr>
                        <td align="left">
                            &nbsp;</td>
                        <td align="left">
                            <asp:Login ID="ctlLogin" runat="server" DisplayRememberMe="false" 
                                OnAuthenticate="ctlLogin_Authenticate" Height="200px">
                                <TextBoxStyle Width="140px" />
                                <LabelStyle CssClass="tdLoginLabel" />
                                <LayoutTemplate>                                  
                                    <table width="100%">
                                        <tr>
                                            <td align="center">
                                            
                                                <table width="100%" >
                                                    <tr>
                                                        <td style="color: #FF9933;"class="tdBottom">
                                                            User name:
                                                        </td>
                                                        <td>
                                                            <telerik:RadTextBox Width="170px" ID="UserName" runat="server">
                                                            </telerik:RadTextBox>
                                                            </td>
                                                    </tr>
                                                    <tr>
                                                        <td style="color: #FF9933;" class="tdBottom">
                                                            Password:
                                                        </td>
                                                        <td>
                                                            <telerik:RadTextBox Width="170px" ID="Password" runat="server" TextMode="Password">
                                                            </telerik:RadTextBox>                                                           
                                                        </td>
                                                    </tr>
                                                  <%--   <tr>
                                                        <td class="tdLogin">
                                                            Terminal Id:
                                                        </td>
                                                        <td>
                                                            <telerik:RadTextBox Width="170px" ID="Terminal" runat="server">
                                                            </telerik:RadTextBox>                                                           
                                                        </td>
                                                    </tr>--%>
                                                    <tr>
                                                        <td align="right" colspan="2">                                                           
                                                            <asp:ImageButton ID="Login" CommandName="Login" runat="server" 
                                                                ImageUrl="~/CommonImages/key.png" onclick="Login_Click"  OnClientClick="javascript:if(validateSave() == false) return false;"/>&nbsp;&nbsp;&nbsp;
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="center">
                                                <asp:Literal ID="FailureText" runat="server"></asp:Literal>
                                               
                                            </td>
                                        </tr>
                                    </table>
                                </LayoutTemplate>
                            </asp:Login>
                            <asp:Label ID="lblMessage" runat="server" Font-Bold="True" ForeColor="Yellow"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="3" class="tdBottom" style="border-style: none; border-right-style: none; 
                border-left-style: none; border-bottom-style:none; text-align: center; color: #FF9933;">
                Product by<br /> Growval Myanmar<br />
                <%=DateTime.Now.Date.Year %>
            </td>
        </tr>
    </table>
        <input type="hidden" runat="server" id="hdnUserId" value="0" />
                <input type="hidden" runat="server" id="hdnoldUserId" value="0" />
        </form>
</body>
</html>
