using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Telerik.Web.UI;
using System.Xml;
using System.Data.SqlClient;
using System.Data.SqlTypes;

public partial class SendAlerts : ThemeBase
{
    GlobalWS objGlobalWS = new GlobalWS();
    Notifications objNotifications = new Notifications();    
    DataBase m_Connection = new DataBase();
    public int ObjectID = 0;
    public string ObjectType = "";

    #region Page Load For Template Page
    protected void Page_Load(object sender, EventArgs e)
    {
        m_Connection.OpenDB("Galaxy");

        if (Request.QueryString["ObjectID"] != null && Request.QueryString["ObjectType"] != null)
        {
            ObjectID = Convert.ToInt32(Request.QueryString["ObjectID"]);
            ObjectType = Request.QueryString["ObjectType"];

        }

        if (Page.IsPostBack)
            return;
        fillTemplates();

        if (cmbTemplates.Items.Count > 0)
            FetchTemplateData();
        //else
        //{
        //    lblMessage.Text = "No templates available, You can send mannual notification!";
        //}
        dtNotificationTime.SelectedDate = DateTime.UtcNow.Add(new TimeSpan(0, TB_nTimeZoneTimeSpan, 0)); // Set Current Date Time 

        DataSet dsSignature = objGlobalWS.FetchContactSignature(TB_nContactID);
        if (Convert.ToInt32(dsSignature.Tables["Response"].Rows[0]["ResultCode"]) != 0)
        {
            LogMessage(Convert.ToString(dsSignature.Tables["Response"].Rows[0]["ResultString"]), 1);
            return;
        }

        ViewState["Signature"] = dsSignature.Tables["Signature"].Rows[0]["Signature"].ToString();
        txtEmailBody.Content += Convert.ToString(ViewState["Signature"]);

    }
    #endregion

    #region Filling Combo of Category for Customers
    private void fillTemplates()
    {
        try
        {
            DataSet objDataSet = objGlobalWS.FetchTemplates(ObjectType);

            if (Convert.ToInt64(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
                return;
            }
            cmbTemplates.DataSource = objDataSet.Tables[0];
            cmbTemplates.DataValueField = "ID";
            cmbTemplates.DataTextField = "name";
            cmbTemplates.DataBind();
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }

    }
    #endregion


    protected void btn_Go_Click(object sender, EventArgs e)
    {
        FetchTemplateData();
    }
    protected void btnSend_Click(object sender, EventArgs e)
    {
        Notifications.stNotificationInfo pNotificationInfo = new Notifications.stNotificationInfo();

        pNotificationInfo.NotificationEmailTo = txtEmailTo.Text;
        pNotificationInfo.NotificationEmailCc = txtEmailCC.Text;
        pNotificationInfo.NotificationEmailBcc = txtEmailBcc.Text;
        pNotificationInfo.NotificationEmailSubject = txtEmailSubject.Text;
        pNotificationInfo.NotificationEmailMessage = txtEmailBody.Content;

        pNotificationInfo.NotificationEmailTime = (DateTime)dtNotificationTime.SelectedDate.Value.AddMinutes(-TB_nTimeZoneTimeSpan);

        // For Attachment in Email by Diwakar

        int nFileCount = RadUpload_attachment.UploadedFiles.Count;
        pNotificationInfo.NotificationEmailAttachCount = nFileCount;
        if (nFileCount > 0)
        {
            pNotificationInfo.NotificationEmailAttachName = new string[10];
            pNotificationInfo.NotificationEmailAttachPath = new string[10];
            pNotificationInfo.NotificationEmailAttachExtension = new string[10];
            pNotificationInfo.NotificationEmailAttachSize = new long[10];
            pNotificationInfo.NotificationEmailAttachMimeType = new string[10];
        
        }
        for (int i = 0; i < nFileCount; i++)
        {
            string strFilePath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["AttachmentPath"]);
            strFilePath += "\\" + RadUpload_attachment.UploadedFiles[i].GetName();
            RadUpload_attachment.UploadedFiles[i].SaveAs(strFilePath, true);

            pNotificationInfo.NotificationEmailAttachName[i] = RadUpload_attachment.UploadedFiles[i].GetName();
            pNotificationInfo.NotificationEmailAttachPath[i] = strFilePath;
            pNotificationInfo.NotificationEmailAttachExtension[i] = RadUpload_attachment.UploadedFiles[i].GetExtension();
            pNotificationInfo.NotificationEmailAttachSize[i] = RadUpload_attachment.UploadedFiles[i].ContentLength;
            pNotificationInfo.NotificationEmailAttachMimeType[i] = Convert.ToString(RadUpload_attachment.UploadedFiles[i].ContentType);

        }




        pNotificationInfo.NotificationSmsTo = txtSMSTo.Text;
        pNotificationInfo.NotificationSMSMessage= txtSMSBody.Text;
        pNotificationInfo.NotificationSmsTime = (DateTime)dtNotificationTime.SelectedDate.Value.AddMinutes(-TB_nTimeZoneTimeSpan);


        pNotificationInfo.NotificationPopupSubject = txtPopupSubject.Text;
        pNotificationInfo.NotificationPopupMessage = txtPopupText.Text;
        pNotificationInfo.NotificationPopupContactId = Convert.ToString(hdnOwnerID.Value);
        pNotificationInfo.NotificationPopupTime = (DateTime)dtNotificationTime.SelectedDate.Value.AddMinutes(-TB_nTimeZoneTimeSpan);

         
        pNotificationInfo.NotificationTransactionCreatedByIp = Request.UserHostAddress;
        pNotificationInfo.NotificationTransactionCreatedDate = DateTime.UtcNow;
        pNotificationInfo.NotificationTransactionRelatedToName = "";

        pNotificationInfo.NotificationTransactionCreatedById = TB_nContactID;
        pNotificationInfo.NotificationTransactionCreatedByName = TB_strEmployeeName;
        pNotificationInfo.NotificationTransactionRelatedToId = ObjectID;
        pNotificationInfo.NotificationTransactionRelatedTo = ObjectType;
        if (cmbTemplates.SelectedIndex > 0)
            pNotificationInfo.NotificationTransactionTemplateId = Convert.ToInt32(cmbTemplates.SelectedValue);
        else
            pNotificationInfo.NotificationTransactionTemplateId = 0;

        if (chkEmail.Checked)
        {
            objNotifications.GenerateEmail(pNotificationInfo);

        }
        if (chkSMS.Checked)
            objNotifications.GenerateSMS(pNotificationInfo);
        if (chkAlert.Checked)
            objNotifications.GeneratePopup(pNotificationInfo);
        lblMessage.Text = "Notifications has been sent succesfully!";
    }

    protected void imgServerEvent_Click(object sender, EventArgs e)
    {
        string strQuery = "SELECT #GetContactName(" + hdnNewOwnerID.Value + ")";
        if (txtSendTo.CssClass == "LinkText")
            txtSendTo.Text += " , ";
        txtSendTo.Text += objGlobalWS.ExecuteQuery(strQuery);
        txtSendTo.CssClass = "LinkText";        
    }


    public void FetchTemplateData()
    {
        if (cmbTemplates.SelectedIndex < 0)
            return;
        Notifications.stNotificationInfo pNotificationInfo = objNotifications.GetDataFromTemplate(cmbTemplates.SelectedValue, ObjectType, ObjectID);
        if (pNotificationInfo.ResultCode == -1)
        {
            lblMessage.Text = pNotificationInfo.ResultString;
            return;
        }
        txtEmailTo.Text = pNotificationInfo.NotificationEmailTo;
        txtEmailCC.Text = pNotificationInfo.NotificationEmailCc;
        txtEmailBcc.Text = pNotificationInfo.NotificationEmailBcc;
        txtEmailSubject.Text = pNotificationInfo.NotificationEmailSubject;
        txtEmailBody.Content = pNotificationInfo.NotificationEmailMessage;
        txtEmailBody.Content += Convert.ToString(ViewState["Signature"]);

        txtSMSTo.Text = pNotificationInfo.NotificationSmsTo;
        txtSMSBody.Text = pNotificationInfo.NotificationSMSMessage;

        txtPopupSubject.Text = pNotificationInfo.NotificationPopupSubject;
        txtPopupText.Text = pNotificationInfo.NotificationPopupMessage;                
        

    }


    #region Log Message
    void LogMessage(string errorMessage, Int32 param)
    {
        if (param == 1)
        {
            lblMessage.Text = errorMessage;
            lblMessage.CssClass = "error";
        }
        else
        {
            lblMessage.Text = errorMessage;
            lblMessage.CssClass = "success";
            // Page.ClientScript.RegisterStartupScript(this.GetType(), "Script", "javascript:alert('" + e.Message + "')", true);
        }
    }
    #endregion       

   


}
