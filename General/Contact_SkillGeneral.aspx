<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Contact_SkillGeneral.aspx.cs" Inherits="Contact_SkillGeneral" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <script src="Scripts/Vb.js" type="text/javascript"></script>
    <script src="Scripts/Contact.js" type="text/javascript"></script>
</head>
<script type="text/javascript">

   function SKL_SetParameters()
   { 
        CNT_AvailableSkillControlID = "<%=lstASkills.ClientID %>";    
        CNT_MemberSkillControlID = "<%=lstMSkills.ClientID %>";
        nRelatedToDeptAccountID = '<%=Request.QueryString["RelatedToDeptAccountID"] %>';
        ContactID = '<%=Request.QueryString["SourceID"] %>';
        ContactLabelControlID = "<%=lblMessage.ClientID %>";
        CON_Get_Member_Skill();    
   }    
</script>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="scriptmanager1" runat ="server" >
    <Services>
        <asp:ServiceReference Path ="~/Services/ContactWS.asmx" />
    </Services>
    </asp:ScriptManager>
    <div id="divLoading" style="display: none;" class="progressPopup">
        <span id="spanMessage"></span>
        <br />
        <br />
        <div id="imgProgress" class="progressImage">
        </div>
    </div>
    <table width="60%" class="ascxMainTable2" border="0" cellpadding="0" cellspacing="0">
    <tr class="trHeader">
        <th class="tdSubHeader" width="100%" colspan="2">
            <asp:Label ID="lblHeader" runat="server" Text="Skill Set"></asp:Label>
        </th>
        <th style="text-align: right; width: 50%;"  colspan="2">        
            <input type="button" runat="server" id="btnSaveSkill" onclick="return CON_Save_Skills();"
            value="Save" style="width: 65px" class="Button" />            
        </th>
    </tr>
    <tr>
        <td colspan="3">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td>
            <table width="100%" border="0">
                <tr>
                    <td class="tdHeader" style="width: 40%">
                        Available Skill Set
                    </td>
                    <td>
                    </td>
                    <td class="tdHeader">
                        Associated Skil Set
                    </td>
                </tr>
                <tr>
                    <td class="tdHeader" align="right">
                        <asp:ListBox ID="lstASkills" runat="server" CssClass="listbox" Width="300px" Skin="Office2007"
                            Height="200px"></asp:ListBox>
                    </td>
                    <td align="center">
                        <input type="button" id="btnAdd" onclick="AddItem('<%=lstASkills.ClientID %>','<%=lstMSkills.ClientID %>')" class="Button" skin="Office2007"
                            value="Add >>" style="width: 70px" /><br />
                        <br />
                        <input type="button" id="btnRemove" onclick="RemoveItem('<%=lstASkills.ClientID %>','<%=lstMSkills.ClientID %>')" class="Button" skin="Office2007"
                            value="<< Remove" style="width: 70px" />
                    </td>
                    <td>
                        <asp:ListBox ID="lstMSkills" runat="server" CssClass="listbox" Width="300px"
                            Skin="Office2007" Height="200px"></asp:ListBox>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
    </form>
</body>
</html>
