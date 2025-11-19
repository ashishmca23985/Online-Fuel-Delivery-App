<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Chatlogin.aspx.cs" Inherits="Chat_Chatlogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Customer Chat </title>
    <link href="../css/bootstrap.css" rel="stylesheet" />
    <link href="../css/teckinfo.css?v=1.1" rel="stylesheet" />
    <style>
        html, body
        {
            padding:0px;
            margin:0px;
            height:100%;
        }


        #RadCaptcha1 img
        {
            width:33%!important;
            float:left;
        }
         #RadCaptcha1 p
        {
            float:left;
            width:60%!important;
            padding-left:5px;
        }
        .container
        {
            margin-left:0px;
        }
       
        
        </style>
</head>
<body class="bg">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scriptmgr" runat="server">
            <Services>
                <asp:ServiceReference Path="~/Services/ChatWS.asmx" />
            </Services>
        </asp:ScriptManager>
        <telerik:RadAjaxManager ID="radajax" runat="server">
             <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnstartchat">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lblMessage" />
                    <telerik:AjaxUpdatedControl ControlID="hdnstatus" />
                    <telerik:AjaxUpdatedControl ControlID="hdnchatid" />
                    <telerik:AjaxUpdatedControl ControlID="hdnpagetype" />
                    <telerik:AjaxUpdatedControl ControlID="hdnpagetype" />
                    <telerik:AjaxUpdatedControl ControlID="divdetails" />
                    <telerik:AjaxUpdatedControl ControlID="RadCaptcha1" LoadingPanelID="RadCustomerPanel" />
                    <telerik:AjaxUpdatedControl ControlID="btnstartchat" LoadingPanelID="RadCustomerPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>

        </telerik:RadAjaxManager>

         <telerik:RadAjaxLoadingPanel ID="RadCustomerPanel" runat="server"  Height="75px" Width="75px"
        Transparency="50">
   <%--     <img alt="Loading..." src='../Images/LoadingProgressBar.gif' style="border: 0;" />--%>
    </telerik:RadAjaxLoadingPanel>


        <!--End Footer Section -->
    


 <div style="height: 24px; width: 100%; height:400px; position: absolute; display:none;  top: 52px; z-index: 90000; opacity: 0.7;" runat="server" id="divprogress">
	
      <%--  <img style="border: 0;margin-left:48%;margin-top:100px;" src="../Images/LoadingProgressBar.gif" alt="Loading...">--%>
     <div class="progress progress-striped active" style="width:250px; height:40px;margin-left:auto;margin-right:auto;margin-top:200px">
        <div class="bar" style="width: 100%;"><h4> Please Wait.......</h4></div>
    </div>
</div>
     <%--<div style="height:50px">
         <div style="width:60%;float:left"></div>
         <div style="width:40%;float:right">
             <div style="float:right;padding-right:50px;padding-top:10px">
                 &nbsp;</div>
         </div>
     </div>--%>  
    <div class="container" style="margin-left: auto; margin-right: auto;width:auto">
       
        <div class="row" style="margin-left: auto; margin-right: auto; ">
             <asp:Label ID="lblMessage" runat="server" Text="" ></asp:Label>
            <div id="divdetails" runat="server">

            <div style="float:left;margin-right:auto" >
                <div class="asp-insetbox asp-insetbox-sidebar">
                    
                    <div>
                        <div style="margin-top: 0px">
                                
								
								
                                <img src="../website/image/easyfuellogo.png" width="100px"  alt="EasyFuel" style="float:right;">
                                
                                <div style="font-family: 'Cabin', sans-serif; font-weight:400;width:53%; float:left; margin-top:30px;" >
                                <h2 style="text-transform: none;"> Contact information &rsaquo;&rsaquo; </h2>
                                </div>
                            <div style="width:100%; float:left">                             
                                &nbsp;</div>
                            
                            
                            <div>
                                
                                <div style="font-family: 'Cabin', sans-serif; font-weight: 400;width:33%;float:left" >Name</div>
                                <input type="text" placeholder="Name" label="First Name" id="txtusername" runat="server" name="Name" maxlength="255" class="input-block-level" style="width:63%;float:left">

                                <div  style="font-family: 'Cabin', sans-serif; font-weight: 400;width:33%;float:left"  >Email</div>
                                <input type="text" placeholder="Email" label="Email" id="txtemail" runat="server" name="Email" maxlength="255" class="input-block-level" style="width:63%;float:left">

                                  <div  style="font-family: 'Cabin', sans-serif; font-weight: 400;width:33%;float:left"  >Company Name</div>
                                <input type="text" placeholder="CompanyName" label="CompanyName" id="txtCompanyName" runat="server" name="CompanyName" maxlength="255" class="input-block-level" style="width:63%;float:left">


                                <div style="font-family: 'Cabin', sans-serif; font-weight: 400;width:33%;float:left"  >Contact Number</div>
                                <input type="text" placeholder="Contact Number" label="Contact Number" id="txtphoneno" runat="server" name="Contact Number" maxlength="255" class="input-block-level" onkeypress="return isNumericKey(event);" style="width:63%;float:left">

                                <div  style="font-family: 'Cabin', sans-serif; font-weight: 400;width:33%;float:left"  >Comments</div>                                
                                <textarea id="txtreason" runat="server" name="Comments" placeholder="How Can We Help You?" rows="4" class="input-block-level" style="width:63%;float:left;resize:none"></textarea>

                                 <div style="clear:both"></div>

                                <telerik:RadCaptcha ID="RadCaptcha1" runat="server" EnableRefreshImage="FALSE" Display="Dynamic" ValidationGroup="Group"  IgnoreCase="false"

                             ErrorMessage="The code you entered is not valid." CaptchaTextBoxLabelCssClass="rcLabelClass"
                           CaptchaTextBoxCssClass="rcTextBoxClass" CaptchaLinkButtonText="" meta:resourcekey="RadCaptcha1Resource1" >
                           <CaptchaImage EnableCaptchaAudio= "false" UseAudioFiles="false" TextChars="LettersAndNumbers"
                                 TextColor="#cadada" BackgroundColor="#3f4f4f" >
                              
                           </CaptchaImage>
                              <TextBoxLabelDecoration Font-Size="10px" Font-Bold="false" />    
                                    
                      </telerik:RadCaptcha>

                                <div style="clear:both"></div>
                            
                                <asp:Button ID="btnstartchat"  style="background-color: #903d8e;  margin-right: 10px; float:right" runat="server" Text="Start Chat" CssClass="form-control btn btn-primary"
                                                OnClientClick="javascript:if(displaychatbox()== false) return false;"
                                                OnClick="btnstartchat_Click" ValidationGroup="Group" />
                             <%--   <button style="background-color: #8CD600;  margin-left: 10px;" class="form-control btn btn-primary" onsubmit="btnstartchat_Click" runat="server" onclick="javascript:if(displaychatbox()== false) return false;" type="submit">Start Chat</button>--%>
                                &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;

                            </div>

                        </div>
                    </div>
                    <!-- Put Sidebar Here-->

                </div>
            </div>
                <%--<div style="float:left;margin-top:50px;margin-left:25px;width:270px">
                 </div>--%>
      </div>
   </div>
</div>
        <%--<div style="clear:both"></div>--%>
          <asp:HiddenField ID="hdnstatus" Value="" runat="server" />
        <asp:HiddenField ID="hdnchatid" Value="" runat="server" />
        <asp:HiddenField ID="hdnpagetype" Value="" runat="server" />
        </form>
</body>




<script type="text/javascript">
   
    var chatwaitnigtime = '<%= chatwaitnigtime%>';
    var time = parseInt(chatwaitnigtime, 10);
    time = time * 60 * 1000;
    
    var varCounter = 0;
    var sendStatus = function () {
      
        if (document.getElementById('hdnstatus').value == "SENDREDUEST" && time > varCounter) {
           
            varCounter += 1000;
           // console.log("sendStatus---- start")
            document.getElementById("divdetails").style.display = 'none';
             document.getElementById('divprogress').style.display = 'block';
            // console.log("time----" + time);
            /// console.log("varCounter--" + varCounter);
            setTimeout(chatResponse, 1000);
            
        } else {
            clearInterval(sendStatus);
            if (time <= varCounter) {
               
                document.getElementById('lblMessage').innerHTML = "Sorry " + document.getElementById('txtusername').value + ".</br> <span style='padiing-left:20px'>All our executives are busy attending other customers on Chat. You are requested to  try after sometime.<span>";
                document.getElementById('divprogress').style.display = 'none';

            }
            
        }
        //console.log("end sendStatus");
    };
    function OnSuccess(result) {

        var Response = result.Response;
       // alert(Response.rows[0].ResultString);
        if (Response.rows.length > 0)
            if (Response.rows[0].ResultString != "") {

              //  alert(Response.rows[0].ResultString);
                document.getElementById('divprogress').style.display = 'none';
                document.getElementById('hdnstatus').value = "RESPONSE";
                window.location.href = 'ChatVisitor.aspx?type=E&chatsessionid=' + Response.rows[0].ResultString;
            }


        

        //window.location.href = 'Chatcustomer.aspx?chatsessionid=8789';

    }
    var chatResponse = function () {
        
  //      document.getElementById('divprogress').style.display = 'block';
        ChatWS.ChatResponseCheck(document.getElementById('hdnchatid').value, OnSuccess);
    }
    setInterval(sendStatus, 1000);


    function displaychatbox() {

        if (document.getElementById('txtusername').value == "" || document.getElementById('txtusername').value == null) {
            alert('Please Enter Your Name');
            document.getElementById('txtusername').focus();
            return false;
        }
         if (document.getElementById('txtCompanyName').value == "" || document.getElementById('txtCompanyName').value == null) {
            alert('Please Enter company name');
            document.getElementById('txtCompanyName').focus();
            return false;
        }
        
        var txtemail = document.getElementById('txtemail');
        if (txtemail.value == "" || txtemail.value == null) {

            alert('Please Enter Your Email');
            txtemail.focus();
            return false;
        }

        if (document.getElementById('txtphoneno').value == "" || document.getElementById('txtphoneno').value == null) {
            alert('Please Enter Your Mobile No.');
            document.getElementById('txtphoneno').focus();
            return false;
        }
        if (document.getElementById('txtreason').value == "" || document.getElementById('txtreason').value == null) {
            alert('Please Enter Your Description');
            document.getElementById('txtreason').focus();
            return false;
        }
        var filter = /^\s*[\w\-\+_]+(\.[\w\-\+_]+)*\@[\w\-\+_]+\.[\w\-\+_]+(\.[\w\-\+_]+)*\s*$/;
        if (!validEmail(txtemail.value))
        {
            alert('Please Enter Valid Email');
            txtemail.focus();
            return false;
        }
        
       // document.getElementById('divprogress').style.display = 'block';
     //   document.getElementById("divdetails").style.display = 'none';
      //  setTimeout(function () { document.getElementById('divprogress').style.display = 'none'; }, 3000);
        
    }
    function validEmail(e) {
        var filter = /^\s*[\w\-\+_]+(\.[\w\-\+_]+)*\@[\w\-\+_]+\.[\w\-\+_]+(\.[\w\-\+_]+)*\s*$/;
        return String(e).search(filter) != -1;
    }
    function isNumericKey(evt) {

        var charCode = (evt.which) ? evt.which : evt.keyCode
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;
        return true;
    }
</script>
</html>
