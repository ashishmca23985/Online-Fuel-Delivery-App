<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Reports.aspx.cs" Inherits="Reports" %>

<%@ Register Assembly="Telerik.ReportViewer.WebForms"
    Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
     <asp:ScriptManager ID="script1" runat ="server">
    </asp:ScriptManager>
    <table width="100%" id="tbl_Search" style="display: block;">
        <tr class="tdMainHeader1">
            <th id="thCustomerHeader" runat="server" class="tdSubHeader" width="100%">Reports
            </th>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblMessage" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
    <table width="100%">        
        <tr>
            <td style="width:7%; background-color:LightBlue; padding:10px; vertical-align:top;">
            <telerik:RadTreeView runat="Server" ID="RadTreeView1" 
                    onnodeclick="RadTreeView1_NodeClick">
                    <Nodes>                       
                        <telerik:RadTreeNode runat="server" Text="Ticket Summary Report" Expanded="true" PostBack="false">
                            <Nodes>
                                <telerik:RadTreeNode runat="server" Text="Status Wise" Value="CAS-Status">
                                </telerik:RadTreeNode>
                                <telerik:RadTreeNode runat="server" Text="Owner Wise" Value="CAS-Owner">
                                </telerik:RadTreeNode>
                                <telerik:RadTreeNode runat="server" Text="Account Wise" Value="CAS-Account">
                                </telerik:RadTreeNode>
                                 <telerik:RadTreeNode runat="server" Text="Site ID Wise" Value="CAS-SiteId">
                                </telerik:RadTreeNode>
                                <telerik:RadTreeNode runat="server" Text="Driver Wise" Value="CAS-Assign">
                                </telerik:RadTreeNode>
                                
                             </Nodes>
                        </telerik:RadTreeNode>

                           <telerik:RadTreeNode runat="server" Text="Invoice Summary Report" Expanded="true" PostBack="false">
                            <Nodes>
                                <telerik:RadTreeNode runat="server" Text="Invoice Report" Value="INV-Account">
                                </telerik:RadTreeNode>
                                <telerik:RadTreeNode runat="server" Text="Invoice Summary" Value="INV-SummaeryAccount">
                                </telerik:RadTreeNode>                                
                             </Nodes>
                        </telerik:RadTreeNode>
                 
                        <telerik:RadTreeNode runat="server" Text="E-Funnel Summary Report" Expanded="true" PostBack="false">
                            <Nodes>
                                <telerik:RadTreeNode runat="server" Text="E-Funnel Report " Value="EFN-Account">
                                </telerik:RadTreeNode>
                               <%-- <telerik:RadTreeNode runat="server" Text="Invoice Summary" Value="INV-SummaeryAccount">
                                </telerik:RadTreeNode>     --%>                           
                             </Nodes>
                        </telerik:RadTreeNode>
                 

                           <telerik:RadTreeNode runat="server" Text="Activity Report" Expanded="true" PostBack="false">
                            <Nodes>
                                <telerik:RadTreeNode runat="server" Text="Recent Activity" Value="ACT-Created">
                                </telerik:RadTreeNode>   
                                <telerik:RadTreeNode runat="server" Text="Top 5" Value="TOP-Five">
                                </telerik:RadTreeNode>    
                                 <telerik:RadTreeNode runat="server" Text="Fuel Inward" Value="Inward">
                                </telerik:RadTreeNode> 


                            </Nodes>
                        </telerik:RadTreeNode>
                         
                    </Nodes>
                </telerik:RadTreeView>
            </td>
            <td style="padding:5px; width:90%; vertical-align:top;">
            
                <telerik:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="580px">
                </telerik:ReportViewer>
            </td>
        </tr>
    </table>
    
    </form>
</body>
</html>
