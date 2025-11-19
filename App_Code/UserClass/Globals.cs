using System;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Collections;
using System.Web.UI;
using System.Collections.Generic;


public class Notifications
{
    DataBase m_Connection;
    OdbcDataAdapter m_DataAdapterOdbc;
    OdbcCommand m_CommandODBC;

    [Serializable]
    public struct stNotificationInfo
    {
        public string NotificationEmailEnabled;
        public string NotificationEmailSubject;
        public string NotificationEmailMessage;
        public string NotificationEmailTo;
        public string NotificationEmailCc;
        public string NotificationEmailBcc;
        public DateTime NotificationEmailTime;
        public int NotificationEmailAttachCount;
        public string[] NotificationEmailAttachName;
        public string[] NotificationEmailAttachPath;
        public string[] NotificationEmailAttachExtension;
        public long[] NotificationEmailAttachSize;
        public string[] NotificationEmailAttachMimeType;
        

        public string NotificationSmsEnabled;
        public string NotificationSMSMessage;
        public string NotificationSmsTo;
        public DateTime NotificationSmsTime;

        public string NotificationPopupEnabled;
        public string NotificationPopupContactId;
        public string NotificationPopupSubject;
        public string NotificationPopupMessage;
        public DateTime NotificationPopupTime;

        public int NotificationTransactionCreatedById;
        public string NotificationTransactionCreatedByIp;
        public DateTime NotificationTransactionCreatedDate;
        public string NotificationTransactionRelatedTo;
        public int NotificationTransactionRelatedToId;
        public string NotificationTransactionRelatedToName;        
        public string NotificationTransactionCreatedByName;
        public int NotificationTransactionTemplateId;

        public int ResultCode;
        public string ResultString;

    };

    public Notifications()
    {
        m_Connection = new DataBase();
        m_Connection.OpenDB("Galaxy");
    }

    public stNotificationInfo GetDataFromTemplate(string strTemplateId, string strObjectType, int nObjectId)
    {
        string strTemp = "";
        Notifications.stNotificationInfo pNotificationInfo = new stNotificationInfo();
        pNotificationInfo.ResultCode = -1;

        try
        {           

            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " + m_Connection.DB_NULL + "(template_email_subject, '') AS EmailSubject, " +
                        m_Connection.DB_NULL + "(template_email_text, '') AS EmailText, " +
                        m_Connection.DB_NULL + "(template_sms_text, '') AS SmsText, " +
                        m_Connection.DB_NULL + "(template_email_to, '') AS EmailTo, " +
                        m_Connection.DB_NULL + "(template_email_cc, '') AS EmailCc, " +
                        m_Connection.DB_NULL + "(template_email_bcc, '') AS EmailBcc, " +
                        m_Connection.DB_NULL + "(template_sms_to, '') AS SmsTo, " +
                        m_Connection.DB_NULL + "(template_pop_subject, '') AS PopupSubject, " +
                        m_Connection.DB_NULL + "(template_pop_text, '') AS PopupText " +
                        "FROM crm_message_templates " +
                        "WHERE ID = " + strTemplateId + " " +
                        "AND template_for = '" + strObjectType + "' " +
                        "AND template_enabled = 'Y' " + m_Connection.DB_TOP_MYSQL;

            DataTable dtTemplate = new DataTable();
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(dtTemplate);

            // no template for id
            if (dtTemplate.Rows.Count <= 0)
            {
                pNotificationInfo.ResultString = "No template found";
                return pNotificationInfo;
            }

            // fetch transaction data
            DataSet objDataSet = m_Connection.FetchTransactionData(strObjectType, "", "", nObjectId, 330);
            if (Convert.ToInt64(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                pNotificationInfo.ResultString = objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString();
                return pNotificationInfo;
            }

            DataRow dr = dtTemplate.Rows[0];

            // Email Parameters
            pNotificationInfo.NotificationEmailSubject = dr["EmailSubject"].ToString();
            pNotificationInfo.NotificationEmailMessage = dr["EmailText"].ToString();
            pNotificationInfo.NotificationEmailTo = dr["EmailTo"].ToString();
            pNotificationInfo.NotificationEmailCc = dr["EmailCc"].ToString();
            pNotificationInfo.NotificationEmailBcc = dr["EmailBcc"].ToString();

            // Sms Parameters
            pNotificationInfo.NotificationSmsTo = dr["SmsTo"].ToString();
            pNotificationInfo.NotificationSMSMessage = dr["SmsText"].ToString();

            // Popup Parameters
            pNotificationInfo.NotificationPopupSubject = dr["PopupSubject"].ToString();
            pNotificationInfo.NotificationPopupMessage = dr["PopupText"].ToString();
            pNotificationInfo.NotificationPopupContactId = "0";

            // check and replace values of all columns
            DataRow row = objDataSet.Tables[0].Rows[0];
            foreach (DataColumn dc in objDataSet.Tables[0].Columns)
            {
                pNotificationInfo.NotificationEmailSubject = pNotificationInfo.NotificationEmailSubject.Replace("#" + dc.ColumnName.ToUpper() + "#", row[dc].ToString());
                pNotificationInfo.NotificationEmailMessage = pNotificationInfo.NotificationEmailMessage.Replace("#" + dc.ColumnName.ToUpper() + "#", row[dc].ToString());
                pNotificationInfo.NotificationEmailTo = pNotificationInfo.NotificationEmailTo.Replace("#" + dc.ColumnName.ToUpper() + "#", row[dc].ToString());
                pNotificationInfo.NotificationEmailCc = pNotificationInfo.NotificationEmailCc.Replace("#" + dc.ColumnName.ToUpper() + "#", row[dc].ToString());
                pNotificationInfo.NotificationEmailBcc = pNotificationInfo.NotificationEmailBcc.Replace("#" + dc.ColumnName.ToUpper() + "#", row[dc].ToString());
                pNotificationInfo.NotificationSmsTo = pNotificationInfo.NotificationSmsTo.Replace("#" + dc.ColumnName.ToUpper() + "#", row[dc].ToString());
                pNotificationInfo.NotificationSMSMessage = pNotificationInfo.NotificationSMSMessage.Replace("#" + dc.ColumnName.ToUpper() + "#", row[dc].ToString());
                pNotificationInfo.NotificationPopupSubject = pNotificationInfo.NotificationPopupSubject.Replace("#" + dc.ColumnName.ToUpper() + "#", row[dc].ToString());
                pNotificationInfo.NotificationPopupMessage = pNotificationInfo.NotificationPopupMessage.Replace("#" + dc.ColumnName.ToUpper() + "#", row[dc].ToString());
            }
            pNotificationInfo.ResultCode = 0;
        }
        catch (Exception ex)
        {
            pNotificationInfo.ResultString = ex.Message;
        }
        return pNotificationInfo;
    }
    
    public void GenerateEmail(stNotificationInfo pNotificationInfo)
    {
        try
        {
            int nRowCount = 0;
            string strTemp = "";  
            int nMailNo = GenerateMailNumber();
            m_Connection.BeginTransaction(); 
            strTemp = "INSERT INTO EMAIL_mails(mail_number, " +
                                                   "mail_type, " +
                                                   "mail_folder, " +
                                                   "mail_mailbox_id, " +
                                                   "mail_send_date, " +
                                                   "mail_sent_time, " +
                                                   "mail_status, " +
                                                   "mail_to, " +
                                                   "mail_cc, " +
                                                   "mail_bcc, " +
                                                   "mail_subject, " +
                                                   "mail_message_subtype, " +
                                                   "mail_message, " +
                                                   "mail_message_html, " +
                                                   "mail_from_email_id, " +
                                                   "mail_attachments, " +
                                                   "owner_id, " +
                                                   "owner_name, " +
                                                   "related_to, " +
                                                   "related_to_id, " + 
                                                   "related_to_name, " +
                                                   "created_by, " +
                                                   "created_ip, " +
                                                   "created_date, " +
                                                   "modified_by, " + 
                                                   "modified_ip, " +
                                                   "modified_date, " +
                                                   "last_activity_date, " +
                                                   "Useable)" +
                                               "VALUES(" + nMailNo + ", " +
                                                   "'T', " +
                                                   "'Outbox', 1, " +
                                                   "?, " +
                                                   "?, " +
                                                   "'P', " +
                                                   "?, " +
                                                   "?, " +
                                                   "?, " +
                                                   "?, " +
                                                   "'html', ?, ?, ?, ?, " +
                                                   "?, " +
                                                   "?, " +
                                                   "?, " +
                                                   "?, " +
                                                   "?, " +
                                                   "?, " +
                                                   "?, " +
                                                   "?, " +
                                                   "?, " +
                                                   "?, " +
                                                   "?, " +
                                                   "?, " +
                                                   "'Y')";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.Parameters.AddWithValue("@MailSendDate", SqlDbType.DateTime).Value = pNotificationInfo.NotificationEmailTime;
            m_CommandODBC.Parameters.AddWithValue("@MailSendTime", SqlDbType.DateTime).Value = pNotificationInfo.NotificationEmailTime;
            m_CommandODBC.Parameters.AddWithValue("@ToMail", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailTo;
            m_CommandODBC.Parameters.AddWithValue("@CcMail", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailCc;
            m_CommandODBC.Parameters.AddWithValue("@BccMail", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailBcc;
            m_CommandODBC.Parameters.AddWithValue("@Subject", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailSubject;
            m_CommandODBC.Parameters.AddWithValue("@strMsg", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailMessage;
            m_CommandODBC.Parameters.AddWithValue("@strHtmlMsg", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailMessage;
            m_CommandODBC.Parameters.AddWithValue("@MailFrom", SqlDbType.VarChar).Value = System.Configuration.ConfigurationSettings.AppSettings["NOTIFICATIONEMAILFROM"].ToString();
            m_CommandODBC.Parameters.AddWithValue("@attachCount", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailAttachCount.ToString();

            m_CommandODBC.Parameters.AddWithValue("@owner_id", SqlDbType.Int).Value = pNotificationInfo.NotificationTransactionCreatedById;
            m_CommandODBC.Parameters.AddWithValue("@owner_name", SqlDbType.VarChar).Value = pNotificationInfo.NotificationTransactionCreatedByName;            
            m_CommandODBC.Parameters.AddWithValue("@related_to", SqlDbType.VarChar).Value = pNotificationInfo.NotificationTransactionRelatedTo;
            m_CommandODBC.Parameters.AddWithValue("@related_to_id", SqlDbType.Int).Value = pNotificationInfo.NotificationTransactionRelatedToId;
            m_CommandODBC.Parameters.AddWithValue("@related_to_name", SqlDbType.VarChar).Value = pNotificationInfo.NotificationTransactionRelatedToName;
            m_CommandODBC.Parameters.AddWithValue("@created_by", SqlDbType.Int).Value = pNotificationInfo.NotificationTransactionCreatedById;
            m_CommandODBC.Parameters.AddWithValue("@created_ip", SqlDbType.VarChar).Value = pNotificationInfo.NotificationTransactionCreatedByIp;
            m_CommandODBC.Parameters.AddWithValue("@created_date", SqlDbType.DateTime).Value = pNotificationInfo.NotificationTransactionCreatedDate;
            m_CommandODBC.Parameters.AddWithValue("@modified_by", SqlDbType.Int).Value = pNotificationInfo.NotificationTransactionCreatedById;
            m_CommandODBC.Parameters.AddWithValue("@modified_ip", SqlDbType.VarChar).Value = pNotificationInfo.NotificationTransactionCreatedByIp;
            m_CommandODBC.Parameters.AddWithValue("@modified_date", SqlDbType.DateTime).Value = pNotificationInfo.NotificationTransactionCreatedDate;
            m_CommandODBC.Parameters.AddWithValue("@last_activity_date", SqlDbType.DateTime).Value = pNotificationInfo.NotificationTransactionCreatedDate;

            nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteNonQuery());
            if (nRowCount == 0)
            {
                LogMessage("Unable To Generate Mail", "", "");
                m_Connection.Rollback();
                m_CommandODBC.Dispose();
                return;
            }

            int nAttachmentCount = pNotificationInfo.NotificationEmailAttachCount;
            for (int i = nAttachmentCount-1; i >= 0; i--)
            {
                strTemp = "INSERT INTO email_attachement_details(attchmnt_mail_number, attchmnt_in_out, attchmnt_name, attchmnt_file_name," +
                       "attchmnt_content_type, attchmnt_content_subtype, attchmnt_size, attchmnt_description, attchmnt_file_path) " +
                       "VALUES(" + nMailNo + ",'O',?,?,?,?,?,'',?)";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                m_CommandODBC.Parameters.AddWithValue("@strName", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailAttachName[i];
                m_CommandODBC.Parameters.AddWithValue("@strFileName", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailAttachPath[i];
                m_CommandODBC.Parameters.AddWithValue("@strContentType", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailAttachMimeType[i];
                m_CommandODBC.Parameters.AddWithValue("@strContentSubType", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailAttachExtension[i];
                m_CommandODBC.Parameters.AddWithValue("@nFileSize", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailAttachSize[i].ToString();
                m_CommandODBC.Parameters.AddWithValue("@strFilePath", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailAttachPath[i];

                #region --- earlier Code Changed by Diwakar
                //strTemp = "INSERT INTO EMAIL_attachement_details(attchmnt_mail_number, " +
                //          "attchmnt_name, attchmnt_file_name) " +
                //          "VALUES(" + nMailNo + ", ?, ?)";
                //m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);

                //m_CommandODBC.Parameters.AddWithValue("@AttchName", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailAttachName;
                //m_CommandODBC.Parameters.AddWithValue("@AttchFileName", SqlDbType.VarChar).Value = pNotificationInfo.NotificationEmailAttachPath;
                #endregion

                nRowCount = m_CommandODBC.ExecuteNonQuery();
                if (nRowCount == 0)
                {
                    LogMessage("Unable To Generate Mail", "", "");
                    m_Connection.Rollback();
                    m_CommandODBC.Dispose();
                    return;
                }
            }

            m_CommandODBC.Dispose();
            m_Connection.Commit();
        }
        catch (Exception ex)
        {
            LogMessage("Error in GenerateMails - " + ex.Message, "", "");
            m_CommandODBC.Dispose();
            m_Connection.Rollback();
        }
    }
    public void GenerateSMS(stNotificationInfo pNotificationInfo)
    {
        try
        {
            string strTemp = "";
            string SMSTo = "";
            string[] strSMSTo = pNotificationInfo.NotificationSmsTo.Split(',');
            for (int i = 0; i < strSMSTo.Length; i++)
            {
                SMSTo = strSMSTo[i].Trim();

                strTemp = "INSERT INTO viasms_queue (" +
                        "queue_date, " +
                        "queue_type, " +
                        "queue_status, " +
                        "queue_expiry_date, " +
                        "queue_recipient_number, " +
                        "queue_sms_type, " +
                        "queue_sms_text, " +
                        "queue_channel_id, " +
                        "queue_refrence_data_1, " +
                        "queue_refrence_data_2, " +
                        "queue_refrence_data_3, " +
                        "queue_refrence_data_4, " +
                        "queue_refrence_data_5, " +
                        "queue_creation_date) " +
                        "VALUES (" +
                         "?, " +
                        "'T', " +
                        "'P', " +
                        "?, " +
                        "?, " +
                        "'T', " +
                        "?, " +
                        "0, " +
                        "?, " +
                        "?, " +
                        "?, " +
                        "?, " +
                        "?, " +
                        m_Connection.DB_UTC_DATE + ")";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                m_CommandODBC.Parameters.AddWithValue("@SMSSendDate", SqlDbType.DateTime).Value = pNotificationInfo.NotificationSmsTime;
                m_CommandODBC.Parameters.AddWithValue("@SMSExpiryDate", SqlDbType.DateTime).Value = pNotificationInfo.NotificationSmsTime.AddHours(4);
                m_CommandODBC.Parameters.AddWithValue("@ToMobile", SqlDbType.VarChar).Value = SMSTo;
                m_CommandODBC.Parameters.AddWithValue("@Message", SqlDbType.VarChar).Value = pNotificationInfo.NotificationSMSMessage;

                m_CommandODBC.Parameters.AddWithValue("@ObjectType", SqlDbType.VarChar).Value = pNotificationInfo.NotificationTransactionRelatedTo;
                m_CommandODBC.Parameters.AddWithValue("@ObjectId", SqlDbType.Int).Value = pNotificationInfo.NotificationTransactionRelatedToId;
                m_CommandODBC.Parameters.AddWithValue("@TemplateId", SqlDbType.Int).Value = pNotificationInfo.NotificationTransactionTemplateId;
                m_CommandODBC.Parameters.AddWithValue("@ContactId", SqlDbType.Int).Value = pNotificationInfo.NotificationTransactionCreatedById;
                m_CommandODBC.Parameters.AddWithValue("@ContactName", SqlDbType.VarChar).Value = pNotificationInfo.NotificationTransactionCreatedByName;


                int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteNonQuery());
                if (nRowCount == 0)
                {
                    LogMessage("Unable To Generate SMS", "", "");
                }
                m_CommandODBC.Dispose();
            }
        }
        catch (Exception ex)
        {
            LogMessage("Error in GenerateSMS - " + ex.Message, "", "");
            m_CommandODBC.Dispose();
        }
    }

    public void GeneratePopup(stNotificationInfo pNotificationInfo)
    {
        try
        {
            string strTemp = "";
            string ContactTo = "";
            string[] strContactTo = pNotificationInfo.NotificationPopupContactId.Split(',');
            for (int i = 0; i < strContactTo.Length; i++)
            {
                ContactTo = strContactTo[i].Trim();


                strTemp = "INSERT INTO CRM_Alarms (Contact_Id, " +
                            "Open_Time, " +
                            "Subject, " +
                            "Message, " +
                            "Created_By, " +
                            "Created_Date, " +
                            "Created_Ip, " +
                            "Related_To, " +
                            "Related_To_Id, " +
                            "Related_To_Name, " +
                            "Status) " +
                            "VALUES (" +
                            ContactTo + ", " +
                            "?, " +
                            "?, " +
                            "?, " +
                            pNotificationInfo.NotificationTransactionCreatedById + ", " +
                            "?, " +
                            "?, " +
                            "?, " +
                            pNotificationInfo.NotificationTransactionRelatedToId + ", " +
                            "?, " +
                            "'P')";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                m_CommandODBC.Parameters.AddWithValue("@OpenTime", SqlDbType.DateTime).Value = pNotificationInfo.NotificationPopupTime;
                m_CommandODBC.Parameters.AddWithValue("@Subject", SqlDbType.VarChar).Value = pNotificationInfo.NotificationPopupSubject;
                m_CommandODBC.Parameters.AddWithValue("@Message", SqlDbType.VarChar).Value = pNotificationInfo.NotificationPopupMessage;
                m_CommandODBC.Parameters.AddWithValue("@CreatedDate", SqlDbType.DateTime).Value = pNotificationInfo.NotificationTransactionCreatedDate;
                m_CommandODBC.Parameters.AddWithValue("@CreatedIp", SqlDbType.VarChar).Value = pNotificationInfo.NotificationTransactionCreatedByIp;
                m_CommandODBC.Parameters.AddWithValue("@RelatedTo", SqlDbType.VarChar).Value = pNotificationInfo.NotificationTransactionRelatedTo;
                m_CommandODBC.Parameters.AddWithValue("@RelatedToName", SqlDbType.VarChar).Value = pNotificationInfo.NotificationTransactionRelatedToName;

                int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteNonQuery());
                if (nRowCount == 0)
                {
                    LogMessage("Unable To Generate Popup", "", "");
                }
                m_CommandODBC.Dispose();
            }
        }
        catch (Exception ex)
        {
            LogMessage("Error in GeneratePopup - " + ex.Message, "", "");
            m_CommandODBC.Dispose();
        }
    }

    public int GenerateMailNumber()
    {
        int nMailNo = 0;
        try
        {
            string strTemp = "";

            m_Connection.BeginTransaction();
            strTemp = "UPDATE email_parameters SET mail_number = mail_number + 1";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteNonQuery());
            if (nRowCount == 0)
            {
                LogMessage("Unable To Update Mail Parameters.", "", "");
                m_Connection.Rollback();
                return -1;
            }
           
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " mail_number " +
                        "FROM email_parameters " + m_Connection.DB_TOP_MYSQL;
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            nMailNo = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
            if (nMailNo <= 0)
            {
                LogMessage("Unable To Update Mail Parameters.", "", "");
                m_Connection.Rollback();
                return -1;
            }
            else
            {
                m_CommandODBC.Dispose();
                m_Connection.Commit();
            }
        }
        catch (Exception ex)
        {
            LogMessage("Error in GenerateMails - " + ex.Message, "", "");
            m_CommandODBC.Dispose();
            m_Connection.Rollback();
        }
        return nMailNo;
    }

    #region Log Error Messages
    void LogMessage(string szMessage, string szMethodName, string szMethodParams)
    {
        CreateLog objCreateLog = new CreateLog();

        try
        {
            szMessage = "m_Connection.cs - " + szMethodName +
                        "(" + szMethodParams + ") " + szMessage;

            objCreateLog.ErrorLog(szMessage);
        }
        catch (Exception ex)
        {
            string str = ex.Message;
        }
    }
    #endregion
}
