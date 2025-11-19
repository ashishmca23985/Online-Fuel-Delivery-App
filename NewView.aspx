<%@ Page Language="C#" AutoEventWireup="true" CodeFile="NewView.aspx.cs" Inherits="NewView" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View</title>


</head>
<script language="javascript" type="text/javascript">
    function EnableCriteria() {
        if ($get("chkCustomFilter").checked == true) {
            $find("<%=txtCiteria.ClientID %>").set_enabled(true);
            $find("<%=txtCiteria.ClientID %>").set_value("AND ");
            $get("<%=trCustomFilter.ClientID %>").style.display = "block";
            $get("<%=trFilterCriteria.ClientID %>").style.display = "none";
        }
        else {
            $find("<%=txtCiteria.ClientID %>").set_enabled(true);
            $get("<%=trCustomFilter.ClientID %>").style.display = "none";
            $get("<%=trFilterCriteria.ClientID %>").style.display = "block";
        }
    }

    function GetRadWindow() {
        try {
            var oWindow = null;

            if (window.radWindow) {
                oWindow = window.radWindow;
            }
            else if (window.frameElement.radWindow) {
                oWindow = window.frameElement.radWindow;
            }

            return oWindow;
        }
        catch (e) {
            alert("Error:- Page -> NewView.aspx -> Method -> \"GetRadWindow\" \n Description:- \n" + e.description);
            return false;
        }
    }

    function CloseWindow() {
        try {
            return;
            var oWnd = GetRadWindow();
            oWnd.close();
        }
        catch (e) {
            alert("Error:- Page -> NewView.aspx -> Method -> 'CloseWindow' \n Description:- \n" + e.description);
            return false;
        }
    }


    function validateOrder() {
        try {
            var control = $find('CmbColumnName');
            if (control.get_selectedIndex() <= 0) {
                alert('Please select Column Name  !');
                control.get_inputDomElement().focus();
                return false;
            }
            control = $find('cmborder');
            if (control.get_selectedIndex() <= 0) {
                alert('Please select type !');
                control.get_inputDomElement().focus();
                return false;
            }
        }
        catch (e) {
            alert("Error:- Page -> NewView.aspx -> Method -> 'order by' \n");
            return false;
        }
    }



</script>

<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scriptmanager1" runat="server">
        </asp:ScriptManager>
        <AjaxControls:ModalUpdateProgress ID="ModalUpdateProgress1" runat="server" DisplayAfter="0" BackgroundCssClass="modalBackground">
            <ProgressTemplate>
                <img src="Images/LoadingProgressBar.gif" />

            </ProgressTemplate>
        </AjaxControls:ModalUpdateProgress>

        <asp:UpdatePanel ID="upPanel" runat="server" UpdateMode="Conditional" RenderMode="Inline">
            <ContentTemplate>


                <%--  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    <AjaxSettings>
           <telerik:AjaxSetting AjaxControlID="btnSave">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lblMessage"  LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnSaveNew">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lblMessage" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rdgCriteria">
                <UpdatedControls>
                   
                    <telerik:AjaxUpdatedControl ControlID="rdgCriteria" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
           
            <telerik:AjaxSetting AjaxControlID="cmbRelatedto">
                <UpdatedControls>
                   
                    <telerik:AjaxUpdatedControl ControlID="lblMessage" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbview">
                <UpdatedControls>
                  
                    <telerik:AjaxUpdatedControl ControlID="lblMessage" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>   
   
   
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server"   >
     <div style="width: 1000px; height: 362px; padding-top: 24px;
        text-align: center;">
        <img id="imgProgress" alt="Loading..." src="images/LoadingProgressBar.gif"/>
    </div>
    </telerik:RadAjaxLoadingPanel>
                --%>


                <table width="100%" class="aspxMainTable" cellpadding="1" cellspacing="0" border="0">
                    <tr>
                        <th id="thCaption" class="tdMainHeader1">
                            <img alt="View" id="imgView" runat="server" src="Images/viewurl.gif" />
                            <asp:Label ID="lblCaption" Text="Create New View" runat="server"></asp:Label>
                        </th>
                        <th style="text-align: right; width: 60%" class="tdMainHeader1">
                            <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="Save_Click" Width="65px" />
                            <asp:Button ID="btnSaveNew" runat="server" Text="Save & Create New View" Width="150px" OnClick="btnSaveNew_Click" />
                        </th>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblMessage" runat="server" ForeColor="#CC3300"></asp:Label>
                        </td>
                    </tr>
                    <tr>

                        <td colspan="2">
                            <table width="38%">

                                <tr>
                                    <td align="right" class="tdHeader">
                                        <b><u>Related To</u></b>
                                    </td>
                                    <td>
                                        <telerik:RadComboBox ID="cmbRelatedto" AutoPostBack="true" OnSelectedIndexChanged="cmbRelatedto_SelectedIndexChanged" runat="server">
                                        </telerik:RadComboBox>
                                    </td>
                                    <td colspan="4" align="right" class="tdHeader"></td>

                                </tr>
                            </table>
                        </td>

                    </tr>
                    <tr>
                        <td colspan="4">
                            <table width="100%" id="tbview" runat="server" visible="false">

                                <tr>
                                    <td align="right" style="width: 10%" class="tdHeader">View</td>
                                    <td style="width: 30%">
                                        <telerik:RadListBox runat="server" OnSelectedIndexChanged="lbview_SelectedIndexChanged" AutoPostBack="true" ID="lbview" Height="170px" Width="160px">
                                        </telerik:RadListBox>
                                        <td colspan="2">
                                            <fieldset style="width: 410px">
                                                <legend class="legendStyle">Select Columns To Display</legend>
                                                <br />

                                                <telerik:RadListBox runat="server" ID="RadListBoxSource" Height="170px" Width="180px"
                                                    AllowTransfer="true" TransferToID="RadListBoxDestination">

                                                    <ButtonSettings ShowTransferAll="false" VerticalAlign="Middle"></ButtonSettings>
                                                </telerik:RadListBox>
                                                <telerik:RadListBox runat="server" AllowReorder="true" ID="RadListBoxDestination" Height="170px" Width="180px">
                                                </telerik:RadListBox>


                                            </fieldset>
                                        </td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <table width="100%" id="tddetail" runat="server" visible="false">

                                <tr id="trCreatedAt" runat="server">
                                    <td align="right" class="tdHeader">Created By
                                    </td>
                                    <td>
                                        <telerik:RadTextBox ID="txtCreatedBy" ReadOnly="true" MaxLength="100" Skin="Office2007"
                                            InvalidStyleDuration="100" LabelCssClass="radLabelCss_WebBlue" runat="server"
                                            Width="150px">
                                        </telerik:RadTextBox>
                                    </td>
                                    <td align="right" class="tdHeader">Created At
                                    </td>
                                    <td>
                                        <telerik:RadTextBox ID="txtCreatedAt" ReadOnly="true" MaxLength="100" Skin="Office2007"
                                            InvalidStyleDuration="100" LabelCssClass="radLabelCss_WebBlue" runat="server"
                                            Width="150px">
                                        </telerik:RadTextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" class="tdHeader" width="10%">View Name
                                    </td>
                                    <td width="30%" class="tdHeader">
                                        <telerik:RadTextBox ID="txtViewName" runat="server" InvalidStyleDuration="100" MaxLength="50"
                                            LabelCssClass="radLabelCss_WebBlue" Width="150px">
                                        </telerik:RadTextBox><span class="mandatory">*</span>&nbsp;
                            <asp:CheckBox ID="chkPrivate" runat="server" Text="Private" />
                                    </td>
                                    <td align="right" class="tdHeader" width="7%">Page Size
                                    </td>
                                    <td class="tdHeader">
                                        <telerik:RadComboBox ID="cmbPageSize" runat="server" Width="50px">
                                            <CollapseAnimation Duration="200" Type="OutQuint" />
                                            <Items>
                                                <telerik:RadComboBoxItem Text="10" Value="10" />
                                                <telerik:RadComboBoxItem Text="20" Value="20" />
                                                <telerik:RadComboBoxItem Text="30" Value="30" />
                                                <telerik:RadComboBoxItem Text="40" Value="40" />
                                                <telerik:RadComboBoxItem Text="50" Value="50" />
                                            </Items>
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" class="tdHeader" valign="top">Description
                                    </td>
                                    <td colspan="4">
                                        <telerik:RadTextBox ID="txtViewDescription" runat="server" InvalidStyleDuration="100" Rows="1"
                                            MaxLength="500" LabelCssClass="radLabelCss_WebBlue" TextMode="MultiLine" Width="650px">
                                        </telerik:RadTextBox>
                                    </td>
                                </tr>
                                <tr>

                                    <td style="color: #993300;" colspan="4">
                                        <b><u>Specify Filter Criteria</u></b>
                                        <asp:CheckBox ID="chkCustomFilter" runat="server" onclick="EnableCriteria()" Text="Apply Custom Criteria" />
                                    </td>
                                </tr>


                                <tr>
                                    <td colspan="4">
                                        <div id="trCustomFilter" style="display: none;" runat="server">
                                            <telerik:RadTextBox ID="txtCiteria" TextMode="MultiLine" runat="server" MaxLength="2000" Height="80px" Width="70%">
                                            </telerik:RadTextBox>

                                        </div>
                                        <div id="trFilterCriteria" runat="server" visible="true">
                                            <telerik:RadGrid ID="rdgCriteria" AllowMultiRowSelection="true" runat="server" Skin="WebBlue"
                                                Width="70%" AutoGenerateColumns="False" CellSpacing="0" BorderStyle="Solid"
                                                GridLines="None" ShowFooter="true"
                                                OnItemDataBound="rdgCriteria_ItemDataBound"
                                                OnItemCommand="rdgCriteria_ItemCommand" Height="120px">
                                                <ClientSettings>
                                                    <Scrolling AllowScroll="true" UseStaticHeaders="false" />
                                                </ClientSettings>
                                                <FooterStyle BackColor="#DADADA" />
                                                <MasterTableView CommandItemDisplay="None" CellSpacing="0" TableLayout="Fixed"
                                                    EditMode="InPlace" AutoGenerateColumns="False" DataKeyNames="filter_column_name"
                                                    AllowPaging="false" ShowFooter="true">
                                                    <Columns>
                                                        <telerik:GridTemplateColumn HeaderStyle-Width="2%">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblSno" runat="server"></asp:Label></ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn SortExpression="ColumnHeader" HeaderText="Column">
                                                            <HeaderStyle Width="20%" />
                                                            <ItemTemplate>
                                                                <telerik:RadComboBox ID="edit_ColumnName" runat="server" Width="200px">
                                                                </telerik:RadComboBox>
                                                                <asp:Label ID="lblColName" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "filter_column_name") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                <telerik:RadComboBox ID="add_ColumnName" Width="200px" runat="server">
                                                                </telerik:RadComboBox>
                                                            </FooterTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn SortExpression="OperatorName" HeaderText="Operator">
                                                            <HeaderStyle Width="15%" />
                                                            <ItemTemplate>
                                                                <telerik:RadComboBox ID="edit_Operator" Width="150px" runat="server">
                                                                </telerik:RadComboBox>
                                                                <asp:Label ID="lblOperator" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "filter_operator") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                <telerik:RadComboBox ID="add_Operator" Width="150px" runat="server">
                                                                </telerik:RadComboBox>
                                                            </FooterTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn SortExpression="filter_value" HeaderText="Value">
                                                            <HeaderStyle Width="25%" />
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="edit_FilterValue" Width="250px" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "filter_value") %>'></asp:TextBox>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                <asp:TextBox ID="add_FilterValue" Width="250px" runat="server"></asp:TextBox>
                                                            </FooterTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridTemplateColumn SortExpression="filter_separator" HeaderText="Separator">
                                                            <HeaderStyle Width="8%" />
                                                            <ItemTemplate>
                                                                <telerik:RadComboBox ID="edit_Separator" runat="server" Width="60px">
                                                                    <Items>
                                                                        <telerik:RadComboBoxItem Text="AND" Value="AND" />
                                                                        <telerik:RadComboBoxItem Text="OR" Value="OR" />
                                                                    </Items>
                                                                </telerik:RadComboBox>
                                                                <asp:Label ID="lblSeparator" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "filter_separator") %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                <telerik:RadComboBox ID="add_Separator" Width="60px" runat="server">
                                                                    <Items>
                                                                        <telerik:RadComboBoxItem Text="AND" Value="AND" />
                                                                        <telerik:RadComboBoxItem Text="OR" Value="OR" />
                                                                    </Items>
                                                                </telerik:RadComboBox>
                                                            </FooterTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridButtonColumn ConfirmText="Delete this criteria?" ButtonType="LinkButton" CommandName="Delete"
                                                            Text="Delete" UniqueName="DeleteColumn">
                                                            <HeaderStyle Width="5%" />
                                                            <ItemStyle ForeColor="Blue" />
                                                        </telerik:GridButtonColumn>
                                                        <telerik:GridTemplateColumn>
                                                            <HeaderStyle Width="5%" />
                                                            <FooterTemplate>
                                                                <asp:LinkButton ID="lnkAdd" ForeColor="Blue" runat="server" CommandName="Add" Text="Add"></asp:LinkButton>
                                                            </FooterTemplate>
                                                        </telerik:GridTemplateColumn>
                                                    </Columns>
                                                </MasterTableView>
                                            </telerik:RadGrid>
                                        </div>
                                    </td>
                                </tr>


                                <tr style="position: relative">
                                    <td style="color: #993300; line-height: 10px" valign="top"><b><u>Specify Order Criteria</u> </b>
                                        <br />
                                        <span class="tdHeader" style="al" class>Column Name</span>
                                    </td>

                                    <td colspan="3">
                                        <table width="50%">

                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="ChkId" runat="server" Text="ID" /><asp:RadioButtonList ID="radbutid" runat="server" RepeatDirection="Horizontal">
                                                        <asp:ListItem Value="id|asc" Selected="True" Text="Asc"></asp:ListItem>
                                                        <asp:ListItem Value="id|desc" Text="Desc"></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                                <td>
                                                    <asp:CheckBox ID="ChkCreatedDate" runat="server" Text="Created Date" />
                                                    <asp:RadioButtonList ID="radbutcreated_date" runat="server" RepeatDirection="Horizontal">
                                                        <asp:ListItem Value="created_date|asc" Selected="True" Text="Asc"></asp:ListItem>
                                                        <asp:ListItem Value="created_date|desc" Text="Desc"></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                                <td>
                                                    <asp:CheckBox ID="ChkModifiedDate" Text="Modified Date" runat="server" />
                                                    <asp:RadioButtonList ID="radbutmodified_date" runat="server" RepeatDirection="Horizontal">
                                                        <asp:ListItem Value="modified_date|asc" Selected="True" Text="Asc"></asp:ListItem>
                                                        <asp:ListItem Value="modified_date|desc" Text="Desc"></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                                <td>
                                                    <asp:CheckBox ID="ChkOwnerId" Text="Owner Id" runat="server" />
                                                    <asp:RadioButtonList ID="radbutowner_id" runat="server" RepeatDirection="Horizontal">
                                                        <asp:ListItem Value="owner_id|asc" Selected="True" Text="Asc"></asp:ListItem>
                                                        <asp:ListItem Value="owner_id|desc" Text="Desc"></asp:ListItem>
                                                    </asp:RadioButtonList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>

                                <tr style="display: none">
                                    <td colspan="4">

                                        <telerik:RadTextBox ID="txtOredrByCriteria" ReadOnly="true" TextMode="MultiLine" runat="server" MaxLength="2000" Height="20px" Width="100%">
                                        </telerik:RadTextBox>

                                    </td>
                                </tr>


                            </table>
                        </td>
                    </tr>
                </table>
                <div style="clear: both"></div>
                <asp:HiddenField ID="hdnroletype" Value="1" runat="server"></asp:HiddenField>

            </ContentTemplate>
        </asp:UpdatePanel>

    </form>
</body>
</html>

