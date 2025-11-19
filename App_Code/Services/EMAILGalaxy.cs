using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.Odbc;
using System.Data;
using System.Xml;


/// <summary>
/// Summary description for EMAILGalaxy
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class EMAILGalaxy : System.Web.Services.WebService
{
    public EMAILGalaxy()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string IncomingEMAIL()
    {
        DataBase m_Connection = new DataBase();
        DataTable dt1 = null;
        int nResultCode = -1;
        string strResult = "Fail";
        string strTemp = "";

        if (HttpContext.Current.Request.QueryString["MailNumber"] == null)
        {
            nResultCode = -1;
            strResult = "Missing Parameter MailNumber.";
            return strResult;
        }
        if (HttpContext.Current.Request.QueryString["OwnerID"] == null)
        {
            nResultCode = -1;
            strResult = "Missing Parameter OwnerID.";
            return strResult;
        }
        int MailNumber = Convert.ToInt32(HttpContext.Current.Request.QueryString["MailNumber"]);//10831;
        int ownerId = Convert.ToInt32(HttpContext.Current.Request.QueryString["OwnerID"]);//20;

        try
        {
            XmlDocument xMainDoc = null;
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " id,mail_subject,mail_message,mail_from_email_id " +
                    "FROM email_mails WHERE mail_number = ? " + m_Connection.DB_TOP_MYSQL;

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@MailNumber", OdbcType.Int).Value = MailNumber;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);


            if (dtTable.Rows.Count > 0)
            {

                int nFlag = 0;
                string CaseNumber = "";
                string RelatedTo = "";
                string RelatedToID = "";
                string MailSubject = Convert.ToString(dtTable.Rows[0]["mail_subject"]);
                int Id = Convert.ToInt32(dtTable.Rows[0]["id"]);
                string MailMessage = Convert.ToString(dtTable.Rows[0]["mail_message"]);
                string MailFrom = Convert.ToString(dtTable.Rows[0]["mail_from_email_id"]).Trim();
             if (MailMessage.Length > 500)
              MailMessage = MailMessage.Substring(0, 500);
                    if (MailSubject.Contains("[") && MailSubject.Contains(":") && MailSubject.Contains("]"))
                    {
                        MailSubject = MailSubject.Replace("Re:", "");
                        int index1 = MailSubject.IndexOf("[");
                        int index2 = MailSubject.IndexOf(":");
                        int index3 = MailSubject.IndexOf("]");
                        RelatedTo = MailSubject.Substring(index1 + 1, 3).Trim();
                        RelatedToID = MailSubject.Substring(index2 + 1, 1).Trim();
                    }
                    else
                    {
                        xMainDoc = m_Connection.createParameterXML();

                        strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " related_to_id, related_to " +
                        "FROM CRM_Contact_Details WHERE Value = ? AND type = 'Email' " + m_Connection.DB_TOP_MYSQL;
                        m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                        m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
                        m_CommandODBC.Parameters.Add("@EmailId", OdbcType.VarChar).Value = MailFrom;
                        dtTable = new DataTable("Count");
                        m_DataAdapterODBC.Fill(dtTable);
                        if (dtTable.Rows.Count == 0)
                        {
                            //  MailFrom = "manju.rana@teckinfo.in";
                            dtTable.Clear();
                            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " id as related_to_id ,'CST' as related_to " +
                                  " from crm_accounts where cust_domain like '%," + MailFrom.Split('@')[1].ToString() + ",%'";
                            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
                            dtTable = new DataTable("Count");
                            m_DataAdapterODBC.Fill(dtTable);
                        }
                        if (dtTable.Rows.Count > 0)
                        {
                            string CustomerId = "0";
                            string nRetaltedToId = Convert.ToString(dtTable.Rows[0]["related_to_id"]);
                            string strRetaltedTo = Convert.ToString(dtTable.Rows[0]["related_to"]);
                            if (strRetaltedTo == "CNT")
                            {
                                strTemp = "SELECT " + m_Connection.DB_FUNCTION + "GetContactAccountID(" + nRetaltedToId + ")";
                                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                                CustomerId = Convert.ToString(m_CommandODBC.ExecuteScalar());

                                m_Connection.fillParameterXML(ref xMainDoc, "case_caller_id", nRetaltedToId, "int", "0");
                                m_Connection.fillParameterXML(ref xMainDoc, "case_caller_name", m_Connection.DB_FUNCTION + "GetContactName(" + nRetaltedToId + ")", "function", "0");
                                m_Connection.fillParameterXML(ref xMainDoc, "case_contact_id", nRetaltedToId, "int", "0");
                                m_Connection.fillParameterXML(ref xMainDoc, "case_contact_name", m_Connection.DB_FUNCTION + "GetContactName(" + nRetaltedToId + ")", "function", "0");

                            }
                            else
                                CustomerId = nRetaltedToId;

                            m_Connection.fillParameterXML(ref xMainDoc, "case_customer_id", CustomerId, "int", "0");
                            m_Connection.fillParameterXML(ref xMainDoc, "case_customer_name", m_Connection.DB_FUNCTION + "GetAccountName(" + CustomerId + ")", "function", "0");
                        }


                        m_Connection.fillParameterXML(ref xMainDoc, "case_source", "E", "varchar", "1");
                        m_Connection.fillParameterXML(ref xMainDoc, "case_source_value", MailFrom, "varchar", "100");

                        m_Connection.fillParameterXML(ref xMainDoc, "case_status_id", "1", "int", "0");
                        m_Connection.fillParameterXML(ref xMainDoc, "case_status_name", "NEW", "varchar", "100");

                        m_Connection.fillParameterXML(ref xMainDoc, "case_subject", MailSubject, "varchar", "100");
                        m_Connection.fillParameterXML(ref xMainDoc, "case_description", MailMessage, "varchar", "500");

                        m_Connection.fillParameterXML(ref xMainDoc, "owner_id", ownerId.ToString(), "int", "0");


                        m_Connection.BeginTransaction();
                        dt1 = m_Connection.SaveTransactionData("CAS", 0, "Y", DateTime.UtcNow, ownerId, Context.Request.ServerVariables["REMOTE_ADDR"].ToString(), xMainDoc);

                        if (Convert.ToInt32(dt1.Rows[0]["ResultCode"]) >= 0)
                        {
                            RelatedTo = "CAS";
                            RelatedToID = Convert.ToString(dt1.Rows[0]["ResultCode"]);
                            CaseNumber = dt1.Rows[0]["ResultString"].ToString();
                            dt1 = m_Connection.SaveRecentActivity(0, "CAS", Convert.ToInt32(dt1.Rows[0]["ResultCode"]), "CREATEDEMAIL", MailSubject, ownerId, "P", 2409, "");
                        }
                        else
                        {
                            m_Connection.Rollback();
                        }
                        strResult = "Pass";
                        nFlag = 1;
                    }

                    xMainDoc = m_Connection.createParameterXML();

                    m_Connection.fillParameterXML(ref xMainDoc, "related_to", RelatedTo, "varchar", "3");
                    m_Connection.fillParameterXML(ref xMainDoc, "related_to_id", RelatedToID, "int", "0");
                    m_Connection.fillParameterXML(ref xMainDoc, "related_to_name", m_Connection.DB_FUNCTION + "GetRelatedName('" + RelatedTo + "', " + RelatedToID + ")", "function", "0");
                    if (nFlag == 1)
                        m_Connection.fillParameterXML(ref xMainDoc, "owner_id", ownerId.ToString(), "int", "0");
                    else
                        m_Connection.fillParameterXML(ref xMainDoc, "owner_id", m_Connection.DB_FUNCTION + "GetOwnerId('" + RelatedTo + "', " + RelatedToID + ")", "function", "0");

                    m_Connection.BeginTransaction();
                    dt1 = m_Connection.SaveTransactionData("EML", Id, "N", DateTime.UtcNow, ownerId, "", xMainDoc);
                    dt1 = m_Connection.SaveRecentActivity(0, "EML", Id, "Created", MailSubject, ownerId, "P", 2409, "");
                    strResult = "Pass";
                }
            
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.Commit();
            m_Connection.CloseDB();
        }
        return strResult;
    }

    [WebMethod]
    public string IncomingTCKEMAIL()
    {
               DataBase m_Connection = new DataBase();
               DataTable dt1 = null;
   //   DataTable dt2 = null;
               int nResultCode = -1;
               string strResult = "Fail";
               string strTemp = "";

            if (HttpContext.Current.Request.QueryString["MailNumber"] == null)
            {
                nResultCode = -1;
                strResult = "Missing Parameter MailNumber.";
                return strResult;
            }
            if (HttpContext.Current.Request.QueryString["OwnerID"] == null)//4 
            {
               nResultCode = -1;
               strResult = "Missing Parameter OwnerID.";
               return strResult;
            }
            if (HttpContext.Current.Request.QueryString["Status"] == null)//Q or R
            {
                nResultCode = -1;
                strResult = "Missing Parameter Status.";
                return strResult;
            }
        //if (HttpContext.Current.Request.QueryString["InboundEmail"] == null)//0 or 1
        //{
        //    nResultCode = -1;
        //    strResult = "Missing Parameter Inbound Email.";
        //    return strResult;
        //}

             int MailNumber = Convert.ToInt32(HttpContext.Current.Request.QueryString["MailNumber"]);
             int ownerId = Convert.ToInt32(HttpContext.Current.Request.QueryString["OwnerID"]);
             string StatusQ = Convert.ToString(HttpContext.Current.Request.QueryString["Status"]);
        //int InboundEmail = Convert.ToInt32(HttpContext.Current.Request.QueryString["InboundEmail"]);
    
        try
        {
            XmlDocument xMainDoc = null;
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " id,"+
                              " mail_subject,mail_message,mail_from_email_id,"+
                              " Mail_bounce_flag,Mail_in_reply_to,Mail_message_id " +
                              " FROM email_mails WHERE mail_number = ? " + m_Connection.DB_TOP_MYSQL;

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@MailNumber", OdbcType.Int).Value = MailNumber;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
        
            if (dtTable.Rows.Count > 0)
            {
                GlobalWS objGlobalWS = new GlobalWS();
                int nFlag = 0;
                string CaseNumber = "";
                string RelatedTo = "";
                string RelatedToID = "";
                string MailSubject = Convert.ToString(dtTable.Rows[0]["mail_subject"]).Replace("Fwd:", "").Replace("FW:", "").Replace("Fw:", "").Replace("Re:", "").Replace("RE:", "");
                string MailMessage = Convert.ToString(dtTable.Rows[0]["mail_message"]);
                string MailFrom = Convert.ToString(dtTable.Rows[0]["mail_from_email_id"]);
                string BounceFlag = Convert.ToString(dtTable.Rows[0]["Mail_bounce_flag"]);
                string ReplyTo = Convert.ToString(dtTable.Rows[0]["Mail_in_reply_to"]);
                string MailMssgeId = Convert.ToString(dtTable.Rows[0]["Mail_message_id"]);
                int Id = Convert.ToInt32(dtTable.Rows[0]["id"]);
                
               // if (MailMessage.Length > 500)
               //     MailMessage = MailMessage.Substring(0, 500);

                if(ReplyTo.Length > 0 )
                {
                    if (ReplyTo.Contains("tsplmailgalaxy") == true)
                    {
                        string[] words= ReplyTo.Split('.');
                        RelatedTo= Convert.ToString(words[3]);
                        RelatedToID = Convert.ToString( words[4]);
                    }
                    else
                    {
                        RelatedTo = objGlobalWS.ExecuteQuery("SELECT #[GetRelatedtoEmail]('" + ReplyTo.Trim() + "')");
                        RelatedToID = objGlobalWS.ExecuteQuery("SELECT #[GetRelatedtoidEmail]('" + ReplyTo.Trim() + "')");
                    }
               }

              else if (BounceFlag == "Y" && ReplyTo.Length==0 )
                   return "Bounce Mail";
              else
                {

          // If  Statu equal to 'R's 
         //  For Q Member 
                    if (StatusQ.Trim() == "R")
                    {
                        strTemp = "select top 1 session_contact_id , " +
                                   " ( select count (owner_id) from crm_cases" +
                                   " where  CONVERT(VARCHAR(10),created_date, 105)=  CONVERT(VARCHAR(10), GETUTCDATE(), 105) " +
                                   " and owner_id=session_contact_id ) as casecount " +
                                   " from crm_user_sessions   where session_contact_id in " +
                                   " ( SELECT queue_member_contact_id  from crm_queue_members where queue_id=" + ownerId + ") and " +
                                   " CONVERT(VARCHAR(10),session_start_time, 105) =  CONVERT(VARCHAR(10), GETUTCDATE(), 105) " +
                                   " and session_active='Y' order by casecount";
                        OdbcCommand m_CommandODBC1 = new OdbcCommand(strTemp, m_Connection.oCon);
                        OdbcDataReader dr = m_CommandODBC1.ExecuteReader();
                        if (dr.HasRows == true)
                        {
                            ownerId = Convert.ToInt32(dr["session_contact_id"]);
                            dr.Close();
                        }
                        else
                            dr.Close();
                    }
                    //end Q Member Code


                    //if (MailSubject.Contains("[") && MailSubject.Contains("#") && MailSubject.Contains("]") && MailSubject.Contains("Ref "))
                    //{
                    //    string[] strSubject = MailSubject.Split(']');
                    //    strSubject = strSubject[0].Split('#');
                    //    RelatedTo = "CAS";
                    //    RelatedToID = objGlobalWS.ExecuteQuery("SELECT #GetCaseId(" + strSubject[1] + ")");

                    //}
                    //else if (objGlobalWS.ExecuteQuery("SELECT #GetCaseIdSubjectCheck('" + MailSubject.Trim() + "','" + MailFrom.Trim() + "')") != "0")
                    //{
                    //    RelatedTo = "CAS";
                    //    RelatedToID = objGlobalWS.ExecuteQuery("SELECT #GetCaseIdSubjectCheck('" + MailSubject.Trim() + "','" + MailFrom.Trim() + "')");
                    //}
                    //else
                    //{

                    xMainDoc = m_Connection.createParameterXML();
                    strTemp =  "SELECT " + m_Connection.DB_TOP_SQL + " related_to_id, related_to " +
                               " FROM CRM_Contact_Details WHERE Value = ? AND type = 'Email' " + m_Connection.DB_TOP_MYSQL;

                    m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                    m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
                    m_CommandODBC.Parameters.Add("@EmailId", OdbcType.VarChar).Value = MailFrom;
                    dtTable = new DataTable("Count");
                    m_DataAdapterODBC.Fill(dtTable);
                    if (dtTable.Rows.Count > 0 && dtTable.Rows.Count == 1)
                    {

                        string CustomerId = "0";
                        string nRetaltedToId = Convert.ToString(dtTable.Rows[0]["related_to_id"]);
                        string strRetaltedTo = Convert.ToString(dtTable.Rows[0]["related_to"]);
                        if (strRetaltedTo == "CNT")
                        {
                            strTemp = "SELECT " + m_Connection.DB_FUNCTION + "GetContactAccountID(" + nRetaltedToId + ")";
                            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                            CustomerId = Convert.ToString(m_CommandODBC.ExecuteScalar());

                            m_Connection.fillParameterXML(ref xMainDoc, "case_caller_id", nRetaltedToId, "int", "0");
                            m_Connection.fillParameterXML(ref xMainDoc, "case_caller_name", m_Connection.DB_FUNCTION + "GetContactName(" + nRetaltedToId + ")", "function", "0");
                            m_Connection.fillParameterXML(ref xMainDoc, "case_product_id", m_Connection.DB_FUNCTION + "GetContactProductId(" + nRetaltedToId + ")", "function", "0");
                            m_Connection.fillParameterXML(ref xMainDoc, "case_product_name", m_Connection.DB_FUNCTION + "GetContactProductName(" + nRetaltedToId + ")", "function", "0");
                            m_Connection.fillParameterXML(ref xMainDoc, "case_subproduct_id", m_Connection.DB_FUNCTION + "GetContactSubProductId(" + nRetaltedToId + ")", "function", "0");
                            m_Connection.fillParameterXML(ref xMainDoc, "case_subproduct_name", m_Connection.DB_FUNCTION + "GetContactSubProductName(" + nRetaltedToId + ")", "function", "0");
                        }
                    }


                    m_Connection.fillParameterXML(ref xMainDoc, "case_source", "E", "varchar", "1");
                    m_Connection.fillParameterXML(ref xMainDoc, "case_source_value", MailFrom, "varchar", "100");

                    m_Connection.fillParameterXML(ref xMainDoc, "case_status_id", "1", "int", "0");
                    m_Connection.fillParameterXML(ref xMainDoc, "case_status_name", "NEW", "varchar", "100");

                    m_Connection.fillParameterXML(ref xMainDoc, "case_subject", MailSubject, "varchar", "100");
                    //  m_Connection.fillParameterXML(ref xMainDoc, "case_description", MailMessage, "varchar", "500");
                    m_Connection.fillParameterXML(ref xMainDoc, "owner_id", ownerId.ToString(), "int", "0");

                    m_Connection.BeginTransaction();
                    dt1 = m_Connection.SaveTransactionData("CAS", 0, "Y", DateTime.UtcNow, ownerId, Context.Request.ServerVariables["REMOTE_ADDR"].ToString(), xMainDoc);
                    
                    //--
                    //if(InboundEmail == 1)
                    //  dt2 = m_Connection.InsertOutboundQueue(MailNumber);

                    //--
                    
                    if (Convert.ToInt32(dt1.Rows[0]["ResultCode"]) >= 0)
                    {
                        RelatedTo = "CAS";
                        RelatedToID = Convert.ToString(dt1.Rows[0]["ResultCode"]);
                        CaseNumber = dt1.Rows[0]["ResultString"].ToString();
                        dt1 = m_Connection.SaveRecentActivity(0, "CAS", Convert.ToInt32(dt1.Rows[0]["ResultCode"]), "CREATEDEMAIL", MailSubject, ownerId, "P", 2409, "Auto Ticket created from email");
                    }
                    else
                    {
                        m_Connection.Rollback();
                    }
                    strResult = "Pass";
                    nFlag = 1;
                }
               
                //for boun and reply
                    xMainDoc = m_Connection.createParameterXML();

                    m_Connection.fillParameterXML(ref xMainDoc, "related_to", RelatedTo, "varchar", "3");
                    m_Connection.fillParameterXML(ref xMainDoc, "related_to_id", RelatedToID, "int", "0");
                    m_Connection.fillParameterXML(ref xMainDoc, "related_to_name", m_Connection.DB_FUNCTION + "GetRelatedName('" + RelatedTo + "', " + RelatedToID + ")", "function", "0");
                    if (nFlag == 1)
                        m_Connection.fillParameterXML(ref xMainDoc, "owner_id", ownerId.ToString(), "int", "0");
                    else
                        m_Connection.fillParameterXML(ref xMainDoc, "owner_id", m_Connection.DB_FUNCTION + "GetOwnerId('" + RelatedTo + "', " + RelatedToID + ")", "function", "0");

                    m_Connection.BeginTransaction();
                    dt1 = m_Connection.SaveTransactionData("EML", Id, "N", DateTime.UtcNow, ownerId, "", xMainDoc);

                    //--
                    //if(InboundEmail == 1)
                    //dt2 = m_Connection.InsertOutboundQueue(MailNumber);
                    //--
                    dt1 = m_Connection.SaveRecentActivity(0, "EML", Id, "Created", MailSubject, ownerId, "P", 2409, "New mail has been attached with Ticket");

                    strResult = "Pass";
                //}
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.Commit();
            m_Connection.CloseDB();
        }
        return strResult;
    
    }

    

  

    #region Log Error Messages
    void LogMessage(string szMessage, string szMethodName, string szMethodParams)
    {
        CreateLog objCreateLog = new CreateLog();

        try
        {
            szMessage = "EmailGalaxy.cs - " + szMethodName +
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

//E:\Teckinfo\GalaxyEmailWS\App_Code\Services
//13.2.26