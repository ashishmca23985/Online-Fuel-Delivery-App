<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CompaignMortgage.aspx.cs" Inherits="CompaignMortgage" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControls" Namespace="AjaxControls" TagPrefix="AjaxControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Campaign Mortgage</title>
    <style  type="text/css">
    .tdheader
    {
    background-color:Silver;
    height:20px;
    width:100%

    }
    
    </style>
</head>
<body>
    <script type="text/javascript">
     
    function divopenclose(element,detail)
    {
     

       
        if(detail=="divpersonaldetails")
        {
         document.getElementById("divfinancial").style.display='none' 
         document.getElementById("divaccountantdetails").style.display='none' 
         document.getElementById("divcontactdetails").style.display='none' 
         document.getElementById("divadddetails").style.display='none' 
        }
        else  if(detail=="divfinancial")
        {
         document.getElementById("divpersonaldetails").style.display='none' 
         document.getElementById("divaccountantdetails").style.display='none' 
         document.getElementById("divcontactdetails").style.display='none' 
         document.getElementById("divadddetails").style.display='none' 
        }
        else  if(detail=="divaccountantdetails")
        {
         document.getElementById("divfinancial").style.display='none' 
         document.getElementById("divpersonaldetails").style.display='none' 
         document.getElementById("divcontactdetails").style.display='none' 
         document.getElementById("divadddetails").style.display='none' 
        }
        else  if(detail=="divcontactdetails")
        {
         document.getElementById("divfinancial").style.display='none' 
         document.getElementById("divaccountantdetails").style.display='none' 
         document.getElementById("divpersonaldetails").style.display='none' 
         document.getElementById("divadddetails").style.display='none' 
        }
        else  if(detail=="divadddetails")
        {
         document.getElementById("divfinancial").style.display='none' 
         document.getElementById("divaccountantdetails").style.display='none' 
         document.getElementById("divcontactdetails").style.display='none' 
         document.getElementById("divpersonaldetails").style.display='none' 
        }
        
        if(document.getElementById(detail).style.display == 'block')
        {
            document.getElementById(detail).style.display='none'  ;
            var header1='';
            header1=document.getElementById(element).innerHTML;
            header1=  header1.replace('(-)','(+)') ;
            document.getElementById(element).innerHTML = header1 ;
        }
        else
        {
            document.getElementById(detail).style.display='block'  ;
            var header1='';
            header1=document.getElementById(element).innerHTML;
            header1=  header1.replace('(+)','(-)') ;
            document.getElementById(element).innerHTML = header1 ;
        }
     }
    
    </script>
    
     <form id="form1" runat="server">
       <asp:ScriptManager ID="scriptmgr" runat="server">
    </asp:ScriptManager>
    <div style="width: 100%">
        <asp:UpdatePanel ID="UpMortgage" runat="server" UpdateMode="Conditional" RenderMode="Inline">
            <ContentTemplate>
                <div style="color: Red;">
                    <center>
                        <asp:Label ID="lblMessage" runat="server"></asp:Label>
                    </center>
                </div>
                 <table width="100%" id="tblQuickCall" class="ascxMainTable2" cellpadding="1" cellspacing="0" style="margin-top:5px">
                    <tr class="trHeader">
                    <th class="tdSubHeader" colspan="2">
                        Campaign Mortgage
                    </th>
                    <th style="text-align: right; width: 50%" colspan="2">
                    <asp:Button ID="btnSave" runat="server" CssClass="Button" Width="65px" 
                    Text="Save" OnClick="btnUpdate_Click"/>
                    </th>
                    </tr>
                    </table>
                <table width="100%" style="margin-top:15px"  class="ascxMainTable2" >
                    <tr>                     
                         <th colspan="5" style="text-align:right; width:50%;" class="tdheader">
                             <asp:CheckBox ID="chkSold" runat="server" Text="SOLD" style="font-weight:bold;" />
                        </th>                                  
                    </tr>
                    <%--Personal Details--%>
                    <tr>
                       <div id="divpersonaldetailsheading" style="display:block;cursor:pointer;" >
                         <th id="tdPersonalDetail" colspan="5" style="text-align: left;width:50%;cursor:pointer;" onclick="javascript:divopenclose('tdPersonalDetail','divpersonaldetails');" class="tdheader">
                            (-) Personal Detail
                        </th>
                       </div>
                       
                    </tr>
                    <tr>
                        <td colspan="5">
                            <div id="divpersonaldetails" style="display:block" >
                                <table width="100%"  class="ascxMainTable2" >
                                    <tr>
                                        <td width="18%">
                                            Title
                                        </td>
                                        <td style="width: 22%;">
                                            <telerik:RadComboBox ID="cmbTitle" MaxHeight="320px" Skin="Office2007" runat="server"
                                                Width="203.5px">
                                            </telerik:RadComboBox>
                                        </td>
                                        <td width="2%">
                                            Suffix
                                        </td>
                                        <td style="float: left;">
                                            <telerik:RadTextBox ID="txtSuffix" MaxLength="100" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td width="8%">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Surname
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtSurname" MaxLength="100" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            Given Name
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtGivenName" MaxLength="100" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Date of Birth
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadDateTimePicker ID="txtDOB" DateInput-DateFormat="MMM dd yyyy hh:mm tt"
                                                runat="server" Skin="WebBlue" Width="250px">
                                            </telerik:RadDateTimePicker>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Australian Resident
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtAusiResident" MaxLength="100" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            Australian Citizen
                                        </td>
                                        <td>
                                            <telerik:RadComboBox ID="cmbAustralian" MaxHeight="320px" Skin="Office2007" runat="server"
                                                Width="203.5px">
                                                <Items>
                                                    <%--  <telerik:RadComboBoxItem Value="1" Text="Yes" />
                                            <telerik:RadComboBoxItem Value="2" Text="No" />--%>
                                                </Items>
                                            </telerik:RadComboBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Drivers License
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtDrivingLicense" MaxLength="9" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            State of Issue
                                        </td>
                                        <td>
                                            <telerik:RadComboBox ID="cmbStates" MaxHeight="320px" Skin="Office2007" runat="server"
                                                Width="203.5px">
                                            </telerik:RadComboBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Marital Status
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadComboBox ID="cmbMaritalStatus" MaxHeight="320px" Skin="Office2007" runat="server"
                                                Width="203.5px">
                                            </telerik:RadComboBox>
                                        </td>
                                        <td>
                                            TFN
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtTFN" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            No Of Dependents
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadNumericTextBox ID="txtdependents" MaxLength="9" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            Adults
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtAdults" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td style="width: 23%;">
                                            Minors
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtMinors" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <%--End  Personal Details --%>
                    
                    <%--  Financial Details --%>
                    <tr>
                         <%--Financail  Details--%>
                        <td colspan="5">
                                <table width="100%">
                                    <tr>
                                        
                                        <td id="tdFinancialDetails" colspan="4" style="font-weight:bold;cursor:pointer;" onclick="javascript:divopenclose('tdFinancialDetails','divfinancial');" class="tdheader">
                                            (+) Financial Details
                                        </td>
                                        
                                    </tr>
                                </table>
                                <div id="divfinancial" style="display:none">
                                <table>
                                    
                                    <tr>
                                        <td>
                                            Gross Annual Income $
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadNumericTextBox ID="txtGrossIncome" MaxLength="100" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            Household Income
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtHouseIncome" MaxLength="100" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Existing Mortgage
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadComboBox ID="cmbMortgage" MaxHeight="320px" Skin="Office2007" runat="server"
                                                Width="203.5px">
                                            </telerik:RadComboBox>
                                        </td>
                                        <td>
                                            Monthly Repayment
                                        </td>
                                        <td>
                                            Balance Owing
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Name of the Lender 1
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtMortgageLender1_Name1" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtMonthlyRepaymentLender1" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtBalanceOwingLender1" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Name of the Lender 2
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtMortgageLender2_Name1" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtMonthlyRepaymentLender2" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtBalanceOwingLender2" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Name of the Lender 3
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtMortgageLender3_Name1" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtMonthlyRepaymentLender3" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtBalanceOwingLender3" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="font-weight: bold;">
                                            Personal Loan/Debts
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadComboBox ID="cmbPersonalLoan" MaxHeight="320px" Skin="Office2007" runat="server"
                                                Width="203.5px">
                                            </telerik:RadComboBox>
                                        </td>
                                        <td>
                                            Monthly Repayment
                                        </td>
                                        <td>
                                            Balance Owing
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Name of the Lender 1
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtLoanRepaymentLender1_Name1" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtLoanMonthlyRepaymentLender1" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtLoanBalanceOwingLender1" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Name of the Lender 2
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtLoanRepaymentLender2_Name1" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtLoanMonthlyRepaymentLender2" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtLoanBalanceOwingLender2" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Name of the Lender 3
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtLoanRepaymentLender3_Name1" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtLoanMonthlyRepaymentLender3" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtLoanBalanceOwingLender3" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Credit/Store Carm_DataSet
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadComboBox ID="cmbCreditCard" MaxHeight="320px" Skin="Office2007" runat="server"
                                                Width="203.5px">
                                            </telerik:RadComboBox>
                                        </td>
                                        <td>
                                            Monthly Repayment
                                        </td>
                                        <td>
                                            Balance Owing
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Name of the Lender 1
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtCreditRepaymentLender1_Name1" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtCreditMonthlyRepaymentLender1" MaxLength="100"
                                                runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtCreditBalanceOwingLender1" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Name of the Lender 2
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtCreditRepaymentLender2_Name1" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtCreditMonthlyRepaymentLender2" MaxLength="100"
                                                runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtCreditBalanceOwingLender2" MaxLength="100" runat="server"
                                                Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Name of the Lender 3
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtCreditRepaymentLender3_Name1" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtCreditMonthlyRepaymentLender3" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtCreditBalanceOwing_Name3" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            Asset Details
                                        </td>
                                        <td>
                                            Current Value
                                        </td>
                                        <td>
                                            Amount Insured
                                        </td>
                                        <td>
                                            Insurance Company
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            House/Land 1
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtHouseLand1" runat="server" Height="20px" Width="200px"
                                                TextMode="MultiLine">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtHouseLandName1" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtLand1" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtHouse1" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            House/Land 2
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtHouseLand2" runat="server" Height="20px" Width="200px"
                                                TextMode="MultiLine">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtHouseLandName2" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtLand2" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtHouse2" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Car 1
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtCarOneName1" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtCarOneName2" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtCarOneName3" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtCarOneName4" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Car 2
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtCarTwoName1" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtCarTwoName2" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtCarTwoName3" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtCarTwoName4" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Bank Accounts
                                        </td>
                                        <td align="left" style="width: 23%;">
                                            Name
                                        </td>
                                        <td>
                                            Account Number
                                        </td>
                                        <td>
                                            Balance $
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Institution 1
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtInstitution1_Name1" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtInstitution1_Name2" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtInstitution1_Name3" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Institution 2
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtInstitution2_Name1" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtInstitution2_Name2" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtInstitution2_Name3" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Institution 3
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtInstitution3_Name1" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtInstitution3_Name2" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtInstitution3_Name3" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" style="text-align: center;">
                                            Other Assets
                                        </td>
                                        <td style="width: 23%;">
                                            &nbsp;
                                        </td>
                                        <td align="left" style="text-align: center;">
                                            Current Value
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Jewellery
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtJewellery" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtJewellery1" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td colspan="2">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Home Contents
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtHomeContent" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtHomeContent1" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 18%;">
                                            Life Insurance - Face Value $
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtLifeInsuranceFaceValue" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtLifeInsurance" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td colspan="2">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 18%;">
                                            Super Annuation (Current Value) $
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtSuperAnnuation" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtAnnuation" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td colspan="2">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 18%;">
                                            Shares & Other Investments
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtShareInvestment" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtInvestment" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 18%;">
                                            Insurance - Income/Home/Business
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtInsuranceIncomeOne" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            Insurance Value
                                        </td>
                                        <td>
                                            Insuring Company
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 18%;">
                                            Income Protection Insurance
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadComboBox ID="cmbIncomeProtection" MaxHeight="320px" Skin="Office2007"
                                                runat="server" Width="203.5px">
                                            </telerik:RadComboBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtIncomeProtection" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtIncomeProtection1" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 18%;">
                                            Home Contents Insurance
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadComboBox ID="cmbHomeContents" MaxHeight="320px" Skin="Office2007" runat="server"
                                                Width="203.5px">
                                            </telerik:RadComboBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtHomeContents" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtHomeContents1" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 18%;">
                                            Business Protection Insurance
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadComboBox ID="cmbBusiness" MaxHeight="320px" Skin="Office2007" runat="server"
                                                Width="203.5px">
                                            </telerik:RadComboBox>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtBusiness" MaxLength="100" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtBusiness1" MaxLength="100" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                        <td colspan="4" style="font-weight: bold;">
                            Mortgage/Remortgage Requirement
                        </td>
                    </tr>
                                    <tr>
                        <td colspan="5">
                            <div id="div3">
                                <table width="100%">
                                    <tr>
                                        <td style="width: 18%;">
                                            Loan Amount $
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadNumericTextBox ID="txtLocalAmount" MaxLength="100" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 18%;">
                                            Repayment Tenure
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadNumericTextBox ID="txtRepayment" runat="server" Width="200px">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 18%;">
                                            Interest Type
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadComboBox ID="cmbInterestType" MaxHeight="320px" Skin="Office2007" runat="server"
                                                Width="203.5px">
                                            </telerik:RadComboBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 18%;">
                                            <asp:Button ID="btnBorrower" runat="server" Text="Link Co-Borrower" />
                                        </td>
                                        <td style="width: 23%;">
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 18%;">
                                            Comments
                                        </td>
                                        <td colspan="4">
                                            <telerik:RadTextBox ID="txtComments" runat="server" TextMode="MultiLine" Width="500px"
                                                Height="50px">
                                            </telerik:RadTextBox>
                                        </td>
                                    </tr>
                                 </table>
                             </div>
                        </td>   
                    </tr>
                                 
                                </table>
                           </div>   
                        </td>
                    </tr>
                   
                    <%--End  Financial Details --%>
                    
                    <%--  Address Details --%>
                    <tr>
                        <td colspan="5">
                            
                                <table width="100%">
                                
                                    <tr>
                                        <td id="tdAddressDetails" colspan="5" style="font-weight: bold;cursor:pointer;" onclick="javascript:divopenclose('tdAddressDetails','divadddetails');" class="tdheader">
                                           (+) Address Details
                                        </td>
                                       
                                    </tr>
                              
                                </table>
                                 <div  id="divadddetails" style="display:none">
                                <table width="100%">
                                   
                                    <tr>
                                        <td>
                                            Home Address Line 1
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtHomeAddressLine1" Height="20px" runat="server" TextMode="MultiLine"
                                                Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td colspan="3">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Home Address Line 2
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtHomeAddressLine2_Text1" runat="server" Height="20px" Width="200px"
                                                TextMode="MultiLine">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtHomeAddressLine2_Text2" Height="20px" Width="200px" TextMode="MultiLine"
                                                runat="server">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtHomeAddressLine2_Text3" Height="20px" Width="200px" TextMode="MultiLine"
                                                runat="server">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Suburb
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtSuburb" runat="server" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            State
                                        </td>
                                        <td>
                                            <telerik:RadComboBox ID="cmbSuburbState" MaxHeight="320px" Skin="Office2007" runat="server"
                                                Width="203.5px">
                                            </telerik:RadComboBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Post Code
                                        </td>
                                        <td style="width: 23%;">
                                            <telerik:RadTextBox ID="txtAddressPostCode" Width="200px" runat="server">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            Years at Address
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtYearAtAddress" Width="200px" runat="server">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="5">
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="5" style="text-align: center;">
                                            Previous Address - If staying in current Address for less than 3 Years
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="5">
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Previous Address
                                        </td>
                                        <td colspan="3">
                                            <telerik:RadTextBox ID="txtPreviousAddress" runat="server" Height="20px" TextMode="MultiLine"
                                                Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            State
                                        </td>
                                        <td>
                                            <telerik:RadComboBox ID="cmbPreviousState" MaxHeight="320px" Skin="Office2007" runat="server"
                                                Width="203.5px">
                                            </telerik:RadComboBox>
                                        </td>
                                        <td style="width: 18%;">
                                            Post Code
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtPreviousAddressPostCode" runat="server" Height="15px"
                                                TextMode="MultiLine" Width="200px">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Years at Address
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="txtPreviousYearAtAddress" Width="200px" runat="server">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                        <td colspan="2">
                                            &nbsp;
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            State
                                        </td>
                                        <td>
                                            <telerik:RadComboBox ID="cmbPreviousYearState" MaxHeight="320px" Skin="Office2007"
                                                runat="server" Width="203.5px">
                                            </telerik:RadComboBox>
                                        </td>
                                        <td>
                                            Post Code
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtPreviousYearPost" Width="200px" runat="server">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            Current Residential Status
                                        </td>
                                        <td>
                                            <telerik:RadComboBox ID="cmbCurrentResidential" MaxHeight="320px" Skin="Office2007"
                                                runat="server" Width="203.5px">
                                            </telerik:RadComboBox>
                                        </td>
                                        <td>
                                            <telerik:RadTextBox ID="txtCurrentResidential" Width="200px" runat="server">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td rowspan="2">
                                            Post Address
                                        </td>
                                        <td colspan="3" style="height: 30px;">
                                            <telerik:RadTextBox ID="txtPostAddress" MaxLength="7" Width="200px" runat="server">
                                            </telerik:RadTextBox>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="5">
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                                </div>
                            
                        </td>
                    </tr>
                    <%--End Address Details --%>
                    
                    <%--Contact Details --%>
                    <tr>
                        <td colspan="5">
                           
                                <table width="100%">
                                    <tr>
                                         
                                        <td id="tdContactDetails"  colspan="5" style="font-weight:bold;display:block;cursor:pointer;" onclick="javascript:divopenclose('tdContactDetails','divcontactdetails');" class="tdheader">
                                           (+) Contact Details
                                        </td>
                                        
                                    </tr>
                                   </table>
                                   <div id="divcontactdetails" style="display:none">
                                   <table>
                                        
                                        <tr>
                                            <td>
                                                Home Phone Number
                                            </td>
                                            <td style="width: 23%">
                                                <telerik:RadNumericTextBox ID="txtHomePhone" MaxLength="10" Width="200px" runat="server">
                                                </telerik:RadNumericTextBox>
                                            </td>
                                            <td colspan="2">
                                                &nbsp;
                                            </td>
                                            <td width="18%">
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Work Phone Number
                                            </td>
                                            <td style="width: 23%;">
                                                <telerik:RadNumericTextBox ID="txtWorkPhone" MaxLength="10" Width="200px" runat="server">
                                                </telerik:RadNumericTextBox>
                                            </td>
                                            <td>
                                                Extension
                                            </td>
                                            <td>
                                                <telerik:RadTextBox ID="txtExtension" Width="200px" runat="server">
                                                </telerik:RadTextBox>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Mobile Phone Number
                                            </td>
                                            <td style="width: 23%;">
                                                <telerik:RadNumericTextBox ID="txtMobileNo" MaxLength="10" Width="200px" runat="server">
                                                </telerik:RadNumericTextBox>
                                            </td>
                                            <td>
                                                Day Time Number
                                            </td>
                                            <td>
                                                <telerik:RadComboBox ID="cmbDayTime" MaxHeight="320px" Skin="Office2007" runat="server"
                                                    Width="203.5px">
                                                </telerik:RadComboBox>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Fax Number
                                            </td>
                                            <td style="width: 23%;">
                                                <telerik:RadNumericTextBox ID="txtFax" MaxLength="10" Width="200px" runat="server">
                                                </telerik:RadNumericTextBox>
                                            </td>
                                            <td>
                                                Evening Number
                                            </td>
                                            <td>
                                                <telerik:RadComboBox ID="cmbEveningNumber" MaxHeight="320px" Skin="Office2007" runat="server"
                                                    Width="203.5px">
                                                </telerik:RadComboBox>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                E-Mail ID
                                            </td>
                                            <td colspan="3">
                                                <telerik:RadTextBox ID="txtEmail" Width="200px" runat="server">
                                                </telerik:RadTextBox>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="font-weight: bold; outline: 1px solid line;">
                                                Employment Details
                                            </td>
                                            <td style="width: 23%;">
                                                &nbsp;
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Employment Status
                                            </td>
                                            <td style="width: 23%;">
                                                <telerik:RadComboBox ID="cmbEmpStatus" MaxHeight="320px" Skin="Office2007" runat="server"
                                                    Width="203.5px">
                                                </telerik:RadComboBox>
                                            </td>
                                            <td colspan="2">
                                                <telerik:RadTextBox ID="txtEmpStatus" Width="200px" runat="server">
                                                </telerik:RadTextBox>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 18%">
                                                1 Employer Name, Address & Number
                                            </td>
                                            <td colspan="3">
                                                <telerik:RadTextBox ID="txtEmpDetail1" TextMode="MultiLine" Width="200px" runat="server">
                                                </telerik:RadTextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td style="width: 18%">
                                                2 Employer Name, Address & Number
                                            </td>
                                            <td colspan="3">
                                                <telerik:RadTextBox ID="txtEmpDetail2" TextMode="MultiLine" Width="200px" runat="server">
                                                </telerik:RadTextBox>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Occupation
                                            </td>
                                            <td style="width: 23%;">
                                                <telerik:RadTextBox ID="txtOccupation" Width="200px" runat="server">
                                                </telerik:RadTextBox>
                                            </td>
                                            <td>
                                                JobTitle
                                            </td>
                                            <td>
                                                <telerik:RadTextBox ID="txtJobTitle" Width="200px" runat="server">
                                                </telerik:RadTextBox>
                                            </td>
                                            <td>
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                     </div>
                            </div>
                        </td>
                    </tr>
                    <%--End Contact Details --%>
                    
                    <%--Accountant Details --%>
                    <tr>
                       
                        <td id="tdAccountantDetails" colspan="5" style="font-weight:bold;outline: 1px solid line;display:block;cursor:pointer;" onclick="javascript:divopenclose('tdAccountantDetails','divaccountantdetails');" class="tdheader">
                          (+) Accountant Details
                        </td>
                       
                    </tr>
                    <tr>
                        <td>
                            <div id="divaccountantdetails" style="display:none">
                            <table style="width:100%">
                            <tr>
                                <td width="18%">
                                    Accounting Firm Name
                                </td>
                                <td colspan="3">
                                    <telerik:RadTextBox ID="txtAccFirmName" Width="400px" runat="server">
                                    </telerik:RadTextBox>
                                </td>
                                <td width="18%">
                                    &nbsp;
                                </td>
                        </tr>
                            <tr>
                                <td>
                                    Accountants Name
                                </td>
                                <td colspan="3">
                                    <telerik:RadTextBox ID="txtAccName" Width="400px" runat="server">
                                    </telerik:RadTextBox>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Address
                                </td>
                                <td colspan="3">
                                    <telerik:RadTextBox ID="txtAccAddress" Width="400px" runat="server">
                                    </telerik:RadTextBox>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Phone Number
                                </td>
                                <td colspan="3">
                                    <telerik:RadNumericTextBox ID="txtAccPhoneNumber" Width="400px" runat="server">
                                    </telerik:RadNumericTextBox>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                          
                                    
                            </table>
                            </div>
                        </td>
                    </tr>
                    <%--End  Accountant Details --%>
                    
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
