<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MyProfile.aspx.cs" Inherits="MyProfile" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Charting" Assembly="Telerik.Web.UI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>My Home Page</title>
        <link rel="stylesheet" type="text/css" href="CSS/inner.css" />
        <link href="CSS/StyleSheet.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <form id="mainForm" runat="server" method="post" style="width: 100%">
            <asp:ScriptManager ID="ScriptManager" runat="server" />
            <!-- content start -->
            <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
                <AjaxSettings>
                    <telerik:AjaxSetting AjaxControlID="dropDownCategory">
                        <UpdatedControls>
                            <telerik:AjaxUpdatedControl ControlID="RadChart1" />
                        </UpdatedControls>
                    </telerik:AjaxSetting>
                </AjaxSettings>
            </telerik:RadAjaxManager>
            <telerik:RadSplitter id="RadSplitter1" runat="server" height="630px" width=1000 CssClass="Telerik" ResizeMode="EndPane">
                <telerik:RadPane id="navigationPane" runat="server" >
                        <!-- 
                            Left Pane Content
                        -->
                    
                        <div style="border:solid 1px Green; padding:5px; margin:6px 3px 3px 3px;">
                        <div style="background-color:#cccccc; letter-spacing:2px; padding:3px 3px 3px 7px; font-size:13px; font-weight:bold; color:Maroon; ">Session Details</div>
                        
                        <table width="100%" style="font-family:Verdana; font-size:12px;" >
					        <tr>
					            <td style="width:70%; color:Black; font-weight:bold;">Number of Logins : </td>
					            <td runat="server" id="tdLoginCount" style="color:Green;">0</td>
					        </tr>
					        <tr><td></td></tr>
					        <tr>
					            <td style="width:70%; color:Black; font-weight:bold;">Total Login Duration : </td>
					            <td runat="server" id="tdTotalLogin" style="color:Green;">00:00:00</td>
					        </tr>
					        <tr><td></td></tr>
					        <tr>
					            <td style="width:70%; color:Black; font-weight:bold;">Effective Login Duration : </td>
					            <td runat="server" id="tdTotalEffLogin" style="color:Green;">00:00:00</td>
					        </tr>
					        <tr><td></td></tr>
					        <tr>
					            <td style="width:70%; color:Black; font-weight:bold;">Idle Duration : </td>
					            <td runat="server" id="tdTotalIdle" style="color:Green;">00:00:00</td>
					        </tr>
					        <tr><td></td></tr>
					        <tr>
					            <td style="width:70%; color:Black; font-weight:bold;">Incall Duration : </td>
					            <td runat="server" id="tdTotalIncall" style="color:Green;">00:00:00</td>
					        </tr>
					        <tr><td></td></tr>
					        <tr>
					            <td style="width:70%; color:Black; font-weight:bold;">Break Duration : </td>
					            <td runat="server" id="tdTotalBreak" style="color:Green;">00:00:00</td>
					        </tr>
					        <tr>
					            <td colspan="2"><hr style="color:#cccccc;" /></td>
					        </tr>
					        <tr>
							    <td style="width:70%; color:Black; font-weight:bold;">Total Calls : </td>
							    <td runat="server" id="tdTotalCalls" style="color:Green;">0</td>
						    </tr>
						    <tr>
						        <td style="width:70%; color:Black; font-weight:bold;">Inbound Calls : </td>
						        <td runat="server" id="tdInboundCalls" style="color:Green;">0</td>
						    </tr>
						    <tr>
						        <td style="width:70%; color:Black; font-weight:bold;">Outbound Calls : </td>
						        <td runat="server" id="tdOutboundCalls" style="color:Green;">0</td>
						    </tr>
						    <tr>
							    <td style="width:70%; color:Black; font-weight:bold;">Manual Calls : </td>
							    <td runat="server" id="tdManualCalls" style="color:Green;">0</td>
						    </tr>
				        </table>
				    </div>    
				    <div style="border:solid 1px Green; padding:5px; margin:10px 3px 3px 3px;">
                        <div style="background-color:#cccccc; letter-spacing:2px; padding:3px 3px 3px 7px; font-size:13px; font-weight:bold; color:Maroon; ">Call Statistics</div>
                   
                        <table width="100%" style="font-family:Verdana; font-size:12px;" >
					        <tr>
					            <td style="width:70%; color:Black; font-weight:bold;">Answering Machine : </td>
					            <td runat="server" id="tdBucket1" style="color:Green;">0</td>
					        </tr>
					        <tr><td></td></tr>
					        <tr>
					            <td style="width:70%; color:Black; font-weight:bold;">Network Problem : </td>
					            <td runat="server" id="tdBucket2" style="color:Green;">0</td>
					        </tr>
					        <tr><td></td></tr>
					        <tr>
					            <td style="width:70%; color:Black; font-weight:bold;">Invalid Numbers : </td>
					            <td runat="server" id="tdBucket3" style="color:Green;">0</td>
					        </tr>
					        <tr><td></td></tr>
					        <tr>
					            <td style="width:70%; color:Black; font-weight:bold;">Dialer Errors : </td>
					            <td runat="server" id="tdBucket4" style="color:Green;">0</td>
					        </tr>
					        <tr><td></td></tr>
					        <tr>
					            <td style="width:70%; color:Black; font-weight:bold;">Call Drops : </td>
					            <td runat="server" id="tdBucket5" style="color:Green;">0</td>
					        </tr>
					        <tr><td></td></tr>
					        <tr>
					            <td style="width:70%; color:Black; font-weight:bold;">Do Not Call : </td>
					            <td runat="server" id="tdBucket6" style="color:Green;">0</td>
					        </tr>
					        <tr><td></td></tr>
					        <tr>
							    <td style="width:70%; color:Black; font-weight:bold;">Callback : </td>
							    <td runat="server" id="tdBucket7" style="color:Green;">0</td>
						    </tr>
					        <tr><td></td></tr>
						    <tr>
						        <td style="width:70%; color:Black; font-weight:bold;">Success : </td>
						        <td runat="server" id="tdBucket8" style="color:Green;">0</td>
						    </tr>
					        <tr><td></td></tr>
						    <tr>
						        <td style="width:70%; color:Black; font-weight:bold;">Retry : </td>
						        <td runat="server" id="tdBucket9" style="color:Green;">0</td>
						    </tr>
					        <tr><td></td></tr>
						    <tr>
							    <td style="width:70%; color:Black; font-weight:bold;">Consumed : </td>
							    <td runat="server" id="tdBucket10" style="color:Green;">0</td>
						    </tr>
						    <tr><td></td></tr>
				        </table>
                   
                    </div>
                     <div style="border:solid 1px Green; padding:5px; margin:10px 3px 3px 3px;display:none " >
                        <div style="background-color:#cccccc; letter-spacing:2px; padding:3px 3px 3px 7px; font-size:13px; font-weight:bold; color:Maroon; ">Voice Mails</div>
                   
                   
                        <table width="100%" style="font-family:Verdana; font-size:12px;" >
					        <tr>
					            <td style="width:70%; color:Black; font-weight:bold;">New : </td>
					            <td runat="server" id="tdVMNew" style="color:Green;">0</td>
					        </tr>
					        <tr><td></td></tr>
					        <tr>
					            <td style="width:70%; color:Black; font-weight:bold;">Saved : </td>
					            <td runat="server" id="tdVMSaved" style="color:Green;">0</td>
					        </tr>
						    <tr><td></td></tr>
				        </table>
                   
                    </div>
                </telerik:RadPane>
                <telerik:RadSplitBar id="RadSplitbar1" runat="server" CollapseMode="Forward"></telerik:RadSplitBar>
                    <telerik:RadPane id="RadPane2" runat="server" scrolling="none" width="700"> <!-- 
                                Nested Horizontal splitter
                            -->
                        <div style="margin:7px 20px 5px 5px;">
                        <div style="float:right;">
                        <span style="font-weight:bold; font-size:13px"> Skin : </span><telerik:RadComboBox ID="RadComboBoxSkin" runat="server" Width="200px" />
                        <asp:Button ID="btnSetSkin" runat="server" Text="Set As Default" 
                            onclick="btnSetSkin_Click"/></div>
                            <br />
                        <telerik:RadChart ID="RadChart1" runat="server" ChartTitle-TextBlock-Text="Disposition Statistics" DefaultType="Pie" Width="950px" AutoTextWrap="true"
                            OnItemDataBound="RadChart1_ItemDataBound" Skin="Office2007" PlotArea-Appearance-Border-PenStyle="DashDotDot" FillStyle-FillSettings-GradientMode=Circle>
                            <Appearance Dimensions-Width="600px" Dimensions-Height="530px" Border-Visible="false" >
                            </Appearance>
                            <Series>
                                <telerik:ChartSeries Name="Series 1" Type="Pie" DataYColumn="count">
                                    <Appearance LegendDisplayMode="ItemLabels" Border-PenStyle="DashDotDot">
                                    </Appearance>
                                </telerik:ChartSeries>
                            </Series>
                            <PlotArea  DataTable-Appearance-FillStyle-MainColor="Blue" ></PlotArea>
                            <Legend Appearance-ItemAppearance-FillStyle-MainColor="Red"></Legend>
                        </telerik:RadChart>
                        
                        </div>
                    </telerik:RadPane>
                </telerik:RadSplitter>
            <br />
        </form>
    </body>
</html>