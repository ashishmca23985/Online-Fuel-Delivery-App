using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.Odbc;
using System.Data;

/// <summary>
/// Summary description for SMSGalaxy
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class SMSGalaxy : System.Web.Services.WebService {

    DataBase m_Connection;
    OdbcDataAdapter m_DataAdapterOdbc;
    OdbcCommand m_CommandODBC;

    public SMSGalaxy () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string IncomingSMS(string strMessage, string strFrom, DateTime strReceiveDate)
    {
        m_Connection = new DataBase();
        int nResultCode = -1;
        string strResult = "Fail";
        string strTemp = "";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "Select id,owner_id from crm_tasks " + " where task_number = ?";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@TaskNumber", OdbcType.VarChar).Value = strMessage.Trim();
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            if (dtTable.Rows.Count > 0)
            {
                int TaskID = Convert.ToInt32(dtTable.Rows[0]["id"]);
                int OwnerID = Convert.ToInt32(dtTable.Rows[0]["owner_id"]);
                string strEndRemarks = "Closed by SMS from number : " + strFrom + " on : " + strReceiveDate;
                strTemp = "UPDATE crm_tasks  " +
                          "SET  task_status_id = 5, task_status_desc = 'CLOSE', end_remarks = ? " +
                           " Where id = " + TaskID.ToString();
                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@EndRemarks", OdbcType.VarChar).Value = strEndRemarks;
                int rValue = m_CommandODBC.ExecuteNonQuery();
                m_Connection.SaveRecentActivity(0, "TSK", TaskID, "CLOSED", strEndRemarks, OwnerID, "P", 2409, "");
                if (rValue > 0)
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
            szMessage = "GlobalWS.cs - " + szMethodName +
                        "(" + szMethodParams + ") " + szMessage;

            objCreateLog.ErrorLog(szMessage);
        }
        catch (Exception ex)
        {
            string str = ex.Message;
        }
    }
    #endregion


    #region GetTimerRequest
    public DataSet GetTimerRequest(long nUserID, int nUserTimeZoneSpan)
    {
        string strTemp = "";
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL10 + " " + m_Connection.DB_NULL + "(recent_item_name, '') as item_name," +
                    m_Connection.DB_FUNCTION + "GetRelatedName(recent_item_object_type,recent_item_object_id) AS item_related_name," +
                    m_Connection.DB_FUNCTION + "GetObjectName(recent_item_object_type) AS item_object_name," +
                    "REPLACE(" + m_Connection.DB_FUNCTION + "GetImageInfo(0,recent_item_activity_id),'#IP#','" + szBrowsedIP + "') AS item_image_path," +
                    "recent_item_object_type,recent_item_object_id " +
                    "FROM CRM_Recent_Items " +
                    "WHERE recent_item_contact_id = " + nUserID + " " +
                    "AND " + m_Connection.DB_NULL + "(recent_item_display_status,1) = 1 " +
                    "AND recent_item_status <> 'B' " +
                    "ORDER BY recent_item_start_time DESC " + m_Connection.DB_TOP_MYSQL10 + " ";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable dtTable = new DataTable("RecentItems");
            m_DataAdapterOdbc.Fill(dtTable);
            m_DataSet.Tables.Add(dtTable);

            
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL5 + " id as ID," + m_Connection.DB_NULL + "(chat_visitor_name,'') as VisitorName," +
                       m_Connection.DB_NULL + "(chat_visitor_query,'') as VisitorQuery,created_date as CreatedDate," +
                       "chat_type as ChatType," + m_Connection.DB_NULL + "(chat_destination_id,0) as DestinationID, chat_session_id as ChatSessionID, " +
                       m_Connection.DB_FUNCTION + "GetContactType(chat_destination_id) as ContactType " +
                   "FROM crm_chat_sessions WHERE chat_status = 'New' " +
                   "ORDER BY chat_start_time " + m_Connection.DB_TOP_MYSQL5;
            // AND chat_type = 'E'
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            dtTable = new DataTable("NewChatRequest");
            m_DataAdapterOdbc.Fill(dtTable);
            m_DataSet.Tables.Add(dtTable);

            

            strTemp = "SELECT id,chat_session_id as SeesionID,chat_source_id as SourceID,chat_destination_id as DestinationID from crm_chat_sessions" +
                     " where (chat_source_id=" + nUserID + " or chat_destination_id=" + nUserID + ") AND " + m_Connection.DB_NULL + "(chat_end_time," + m_Connection.DB_UTC_DATE + ")>modified_date AND chat_status='Accepted'";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            dtTable = new DataTable("ExistingChatRequest");
            m_DataAdapterOdbc.Fill(dtTable);
            m_DataSet.Tables.Add(dtTable);



            strTemp = "SELECT " + m_Connection.DB_TOP_SQL10 + " subject,related_to,related_to_id, id " +
                         "FROM crm_alarms " +
                         "WHERE contact_id=" + nUserID + " " +
                         "AND Open_time <= " + m_Connection.DB_UTC_DATE + " " +
                         "AND status = 'P' " + m_Connection.DB_TOP_MYSQL10;
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            dtTable = new DataTable("NewChatRequest");
            m_DataAdapterOdbc.Fill(dtTable);
            m_DataSet.Tables.Add(dtTable);

            if (m_Connection.nDatabaseType == 2)
            {
                strTemp = "SELECT id," +
                            "'<img id=imgMob src=\"Images/cases.png\" />' as image_path," +
                            "case_subject as subject," +
                            "concat('[', GetContactName(owner_id), '] From: ', case_source_value) as title," +
                            m_Connection.DB_NULL + "(DATE_FORMAT(GetLocalZoneTime(created_date," + nUserTimeZoneSpan + "),'%d %b %Y %H:%i'),'') as created_date," +
                            "date_format(modified_date,'%d/%m/%Y %H:%i:%S') as modified_date," +
                            "substring(case_description, 100) as description, " +
                            "'CAS' as Type, " +
                            "b.queue_id as queue_id," +
                            "'Cases [' + a.transaction_number + ']' as tab_text " +
                            "FROM crm_cases a, crm_queue_members b " +
                            "WHERE a.owner_id = b.queue_id " +
                            "AND a.case_status_id <> 2 " +
                            "AND b.queue_member_contact_id=" + nUserID;
                strTemp += "UNION SELECT id," +
                            "'<img id=imgMob src=\"Images/tasks.png\" />' as image_path," +
                            "task_subject as subject," +
                            "concat('[', GetContactName(owner_id), '] From: ', GetContactName(created_by)) as title," +
                            m_Connection.DB_NULL + "(DATE_FORMAT(GetLocalZoneTime(created_date," + nUserTimeZoneSpan + "),'%d %b %Y %H:%i'),'') as created_date," +
                            "date_format(modified_date,'%d/%m/%Y %H:%i:%S') as modified_date," +
                            "'' as description, " +
                            "'TSK' as Type, " +
                            "b.queue_id as queue_id," +
                            "'Tasks [' + a.transaction_number + ']' as tab_text " +
                            "FROM crm_tasks a, crm_queue_members b " +
                            "WHERE a.owner_id = b.queue_id " +
                            "AND a.task_status_id <> 5 " +
                            "AND b.queue_member_contact_id=" + nUserID;
            }
            else
            {
                strTemp = "SELECT id," +
                            "'<img id=imgMob src=\"Images/cases.png\" />' as image_path," +
                            "case_subject as subject," +
                            "('[' + dbo.GetContactName(owner_id) +  '] From: ' + case_source_value) as title," +
                            m_Connection.DB_NULL + "(convert(varchar,dbo.GetLocalZoneTime(created_date," + nUserTimeZoneSpan + "),13),'') as created_date," +
                            "convert(varchar, modified_date,13) as modified_date," +
                            "left(case_description, 100) as description, " +
                            "'CAS' as Type, " +
                            "b.queue_id as queue_id," +
                            "'Cases [' + a.transaction_number + ']' as tab_text " +
                            "FROM crm_cases a, crm_queue_members b " +
                            "WHERE a.owner_id = b.queue_id " +
                            "AND a.case_status_id <> 2 " +
                            "AND b.queue_member_contact_id=" + nUserID;

                strTemp += "UNION SELECT id," +
                            "'<img id=imgMob src=\"Images/tasks.png\" />' as image_path," +
                            "task_subject as subject," +
                            "('[' + dbo.GetContactName(owner_id) + '] From: ' + dbo.GetContactName(created_by)) as title," +
                            m_Connection.DB_NULL + "(convert(varchar,dbo.GetLocalZoneTime(created_date," + nUserTimeZoneSpan + "),13),'') as created_date," +
                            "convert(varchar,modified_date,13) as modified_date," +
                            "'' as description, " +
                            "'TSK' as Type, " +
                            "b.queue_id as queue_id, " +
                            "'Tasks [' + a.transaction_number + ']' as tab_text " +
                            "FROM crm_tasks a, crm_queue_members b " +
                            "WHERE a.owner_id = b.queue_id " +
                            "AND a.task_status_id <> 5 " +
                            "AND b.queue_member_contact_id=" + nUserID;
            }
            strTemp += " ORDER BY created_date desc";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            dtTable = new DataTable("QueueData");
            m_DataAdapterOdbc.Fill(dtTable);
            m_DataSet.Tables.Add(dtTable);

            
        }

        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetRecentItems", "");
        }

        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetRecentItems", "");
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion



    
}

