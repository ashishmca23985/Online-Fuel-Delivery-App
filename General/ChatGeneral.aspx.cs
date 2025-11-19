using System;
using System.Data;
using System.Web.UI;
using Telerik.Web.UI;
using System.Data.Odbc;
using System.Xml;


public partial class General_ChatGeneral : ThemeBase
{
    public string RefershRecentItems = "N";
    public int ChatID = 0;

    GlobalWS objGlobalWS = new GlobalWS();
    DataBase m_Connection = new DataBase();

    protected void Page_Load(object sender, EventArgs e)
    {
        m_Connection.OpenDB("Galaxy");

        if (Request.QueryString["ChatID"] != null)
            ChatID = Convert.ToInt32(Request.QueryString["ChatID"]);

        if (!IsPostBack)
        {
            //--check permission level and disable buttons accordingly
            objFrameworkWS.GetObjectPermissionLevel(TB_nUserRoleID, "CHT");
            if (objFrameworkWS.View == false)
            {
                LogMessage("Sorry ! You do not have permission to view chat details", 1);
                return;
            }
            Fill_Related_To();
            GetChatDetails();
            btnSave.Visible = objFrameworkWS.Edit;
        }
    }


    #region GetChatDetails()
    private void GetChatDetails()
    {
        try
        {
            string strColumns = "'ChatType','ChatStatus','RelatedTo','RelatedToName','RelatedToID','ChatSessionID','SourceName','SourcePhone'," +
                "'SourceEmail','SourceIP','SourceQuery','ConversationText','ChatDestinationName','EndTime'," +
                "'CreatedBy','CreatedDate','Useable'";

            DataSet objDataSet = m_Connection.FetchTransactionData("CHT", "crm_chat_sessions", strColumns,ChatID,TB_nTimeZoneTimeSpan);

            if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(objDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }

            DataRow row = objDataSet.Tables["Data"].Rows[0];

            if (row["ChatType"] != null)
                cmbChatType.SelectedValue = Convert.ToString(row["ChatType"]);
            txtchatCreatedBy.Text = Convert.ToString(row["CreatedBy"]);
            txtcreatedatCreatedAt.Text = Convert.ToString(row["CreatedDate"]);
            txtChatSessionID.Text = Convert.ToString(row["ChatSessionID"]);
            if (row["ChatStatus"] != null)
                cmbChatStatus.SelectedValue = Convert.ToString(row["ChatStatus"]);
            if (row["RelatedTo"] != null)
                cmbRelatedTo.SelectedValue = Convert.ToString(row["RelatedTo"]);
            txtchatsourcename.Text = Convert.ToString(row["SourceName"]);
            txtphonenumber.Text = Convert.ToString(row["SourcePhone"]);
            txtvisitoremail.Text = Convert.ToString(row["SourceEmail"]);
            txtvisitorIP.Text = Convert.ToString(row["SourceIP"]);
            txtvisitorquery.Text = Convert.ToString(row["SourceQuery"]);
            divchattext.InnerHtml = Convert.ToString(row["ConversationText"]);
            txtdestinationname.Text = Convert.ToString(row["ChatDestinationName"]);
            txtchatendtime.Text = Convert.ToString(row["EndTime"]);
            txtRelatedToName.Text = Convert.ToString(row["RelatedToName"]);
            hdnRelatedToID.Value = Convert.ToString(row["RelatedToID"]);
            if (hdnRelatedToID.Value != "")
            {
                if (Convert.ToInt32(hdnRelatedToID.Value) > 0)
                {
                    txtRelatedToName.CssClass = "LinkText";
                    txtRelatedToName.Attributes.Add("onclick", "javascript:OpenLink('" + cmbRelatedTo.SelectedValue + "'," + hdnRelatedToID.Value + ",'" + cmbRelatedTo.SelectedItem.Text + " [" + txtRelatedToName.Text + "]');");
                }
            }
            DataTable dt;
            string strUseable = Convert.ToString(row["Useable"]);
            if (strUseable == "Y")
            {
               
                dt = m_Connection.SaveRecentActivity(0, "CHT", ChatID, "View", txtvisitorquery.Text, TB_nContactID, "P", 2410,"");            }
            else
                dt = m_Connection.SaveRecentActivity(0, "CHT", ChatID, "New", txtvisitorquery.Text, TB_nContactID, "B", 2410,"");

            // check result
        }

        catch (Exception e)
        {
            LogMessage(e.Message, 1);
        }
    }
    #endregion
    #region Save Chat general
    protected void btnSave_Click(object sender, EventArgs e)
    {
        SaveChatGeneral("N");
    }

    protected void btnNewSave_Click(object sender, EventArgs e)
    {
        
    }
    public void SaveChatGeneral(string CreateNew)
    {
        try
        {
            XmlDocument xMainDoc = m_Connection.createParameterXML();

            m_Connection.fillParameterXML(ref xMainDoc, "related_to_id", hdnRelatedToID.Value, "int", "0");

            m_Connection.fillParameterXML(ref xMainDoc, "related_to", cmbRelatedTo.SelectedValue, "varchar", "3");

            m_Connection.fillParameterXML(ref xMainDoc, "related_to_name", txtRelatedToName.Text, "varchar", "100");

            if (cmbChatType.SelectedIndex >= 0)
                m_Connection.fillParameterXML(ref xMainDoc, "chat_type", cmbChatType.SelectedItem.Value, "varchar", "1");

            if (cmbChatStatus.SelectedIndex >= 0)
                m_Connection.fillParameterXML(ref xMainDoc, "chat_status", cmbChatStatus.SelectedItem.Text, "varchar", "20");

            m_Connection.fillParameterXML(ref xMainDoc, "chat_visitor_name", txtchatsourcename.Text, "varchar", "100");

            m_Connection.fillParameterXML(ref xMainDoc, "chat_visitor_phone", txtphonenumber.Text, "varchar", "20");

            m_Connection.fillParameterXML(ref xMainDoc, "chat_visitor_email", txtvisitoremail.Text, "varchar", "50");

            m_Connection.fillParameterXML(ref xMainDoc, "chat_visitor_ip", txtvisitorIP.Text, "varchar", "100");

            m_Connection.fillParameterXML(ref xMainDoc, "chat_visitor_query", txtvisitorquery.Text, "varchar", "300");

            m_Connection.fillParameterXML(ref xMainDoc, "chat_conversation_text", divchattext.InnerHtml.ToString(), "varchar", "8000");

            m_Connection.fillParameterXML(ref xMainDoc, "chat_destination_name", txtdestinationname.Text, "varchar", "100");


            lblMessage.Text = "";
            DataTable dt1 = null;
            m_Connection.BeginTransaction();

            dt1 = m_Connection.SaveTransactionData("CHT", ChatID, "N", DateTime.UtcNow, TB_nContactID, Request.UserHostAddress, xMainDoc);

            if (Convert.ToInt32(dt1.Rows[0]["ResultCode"]) >= 0)
            {
                dt1 = m_Connection.SaveRecentActivity(0, "CHT", ChatID, "Modified", txtvisitorquery.Text, TB_nContactID, "P", 2410,"");
                m_Connection.Commit();
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", "javascript:OnSaveSuccess('" + CreateNew + "', 'Chat saved successfully');", true);
            }
            else
            {
                m_Connection.Rollback();
            }
            lblMessage.Text = dt1.Rows[0]["ResultString"].ToString();
        }
        catch (OdbcException ex)
        {
            m_Connection.Rollback();
            LogMessage(ex.Message, 1);
        }
        catch (Exception ex)
        {
            m_Connection.Rollback();
            LogMessage(ex.Message, 1);
        }
        finally
        {
        }
    }
    #endregion

  
    #region Log Message
    void LogMessage(string Message, Int32 param)
    {
        btnSave.Visible = false;

        lblMessage.Text = Message;
        if (param == 1)
            lblMessage.CssClass = "error";
        else
            lblMessage.CssClass = "success";
    }
    #endregion

    #region Fill Related To Dropdown
    protected void Fill_Related_To()
    {
        try
        {
            DataSet dsDataSet = objGlobalWS.GetObjectTableNames();

            if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(dsDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }
            cmbRelatedTo.DataSource = dsDataSet.Tables["TableFor"];
            cmbRelatedTo.DataValueField = "object_code";
            cmbRelatedTo.DataTextField = "object_name";
            cmbRelatedTo.DataBind();

            RadComboBoxItem lstItem = new RadComboBoxItem("", "0");
            cmbRelatedTo.Items.Insert(0, lstItem);
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    protected void imgServerEvent_Click(object sender, EventArgs e)
    {
        
        if (hdnServerEvent.Value == "RELATEDTO")
        {
            string strQuery = "SELECT #GetRelatedName('" + cmbRelatedTo.SelectedValue + "'," + hdnRelatedToID.Value + ")";
            txtRelatedToName.Text = objGlobalWS.ExecuteQuery(strQuery);
            txtRelatedToName.CssClass = "LinkText";
            txtRelatedToName.Attributes.Add("onclick", "javascript:OpenLink('" + cmbRelatedTo.SelectedValue + "'," + hdnRelatedToID.Value + ",'" + cmbRelatedTo.SelectedItem.Text + " [" + txtRelatedToName.Text + "]');");
        }
    }
}
