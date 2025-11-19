using System;
using System.Collections;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data.SqlClient;
using System.Xml;
using System.Data;
using System.Web.Script.Services;
using System.Configuration;
using System.Data.SqlTypes;
using System.Data.Odbc;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for CallListWS
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class ChatWS : System.Web.Services.WebService
{
    DataBase m_Connection;

    string strTemp = "";
    string strParameters = "";

    OdbcCommand m_CommandODBC;
    public ChatWS()
    {

        m_Connection = new DataBase();

    }

    #region update chat status

    [WebMethod(EnableSession = true)]
    public DataSet update_chat_status(string chatseesionid, string status)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            int loginid = 0;
            if (HttpContext.Current.Session["contact_id"] != null)
                loginid = Convert.ToInt32(HttpContext.Current.Session["contact_id"].ToString());
            m_Connection.OpenDB("Galaxy");
            strTemp = " Update crm_chat_sessions set " +
                      " chat_end_time= " + m_Connection.DB_UTC_DATE +
                      " ,chat_status =case when chat_source_id=" + loginid.ToString() + " or  chat_destination_id=" + loginid.ToString() + " then 'Closed' else ? end," +
                      " current_group_member_ids= LEFT( REPLACE ( current_group_member_ids+',' , '," + loginid.ToString() +
                      ",' , ',' ), LEN( REPLACE ( current_group_member_ids+',' , '," + loginid.ToString() + ",' , ',' )) - 1)" +
                      " where chat_session_id=?";
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.AddWithValue("@status", status);
            m_CommandODBC.Parameters.AddWithValue("@chat_session_id", chatseesionid);
            nResultCode = m_CommandODBC.ExecuteNonQuery();
            DataTable dtTable = new DataTable("ChatRequest");
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "update_chat_status", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "update_chat_status", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region save chat history

    [WebMethod]
    public DataSet save_chathistory(string chatseesionid, string chatmasg, string SourceHeader, string DestHeader, int sourceid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "UPDATE crm_chat_sessions set chat_status='Accepted' , chat_conversation_text = " + m_Connection.DB_NULL + "(chat_conversation_text,'') + " +
                    "case when chat_source_id=? then replace(?,'#NAME#',chat_visitor_name) else replace(?,'#NAME#',chat_destination_name) end + ? ," +
                    "chat_source_msg_count=(chat_source_msg_count + (case when chat_source_id=" + sourceid + " then 1 else 0 end))," +
                    "chat_destination_msg_count=(chat_destination_msg_count + (case when chat_source_id=" + sourceid + " then 1 else 0 end))," +
                    "chat_end_time=" + m_Connection.DB_UTC_DATE + " " +
                    "WHERE chat_session_id=?";
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.AddWithValue("@chat_source_id", sourceid);
            m_CommandODBC.Parameters.AddWithValue("@chat_source_header", SourceHeader);
            m_CommandODBC.Parameters.AddWithValue("@chat_dest_header", DestHeader);
            m_CommandODBC.Parameters.AddWithValue("@chat_conversation_text", chatmasg);
            m_CommandODBC.Parameters.AddWithValue("@chat_session_id", chatseesionid);

            nResultCode = m_CommandODBC.ExecuteNonQuery();
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "save_chathistory", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "save_chathistory", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region update Decline Chat Request

    [WebMethod]
    public DataSet DeclineChatRequest(string ChatID, string CustomerID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        string dummycustomerid = "," + CustomerID;
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "UPDATE crm_chat_sessions SET " +
                      "chat_declined_contact_id=" + m_Connection.DB_NULL + "(chat_declined_contact_id,'0') +'" + dummycustomerid + "'" +
                //"chatque_request_type = (case when isnull(contact_id,0) <>0 then chatque_request_type else 'Closed' end) " +
                      "WHERE chat_visitor_id=?";
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.AddWithValue("@id", ChatID);
            nResultCode = m_CommandODBC.ExecuteNonQuery();
            DataTable dtTable = new DataTable("ChatRequest");
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "DeclineChatRequest", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "DeclineChatRequest", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion



    #region update chat status
    public DataSet updateChatRelatedTo(int chatid, string objectType, int objectId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            OdbcCommand m_CommandODBC = new OdbcCommand();
            m_CommandODBC.CommandType = CommandType.Text;
            m_CommandODBC.Connection = m_Connection.oCon;
            strTemp = "UPDATE crm_chat_sessions SET related_to=? ,related_to_id=?" +
                      " WHERE id=?";
            m_CommandODBC.CommandText = strTemp;
            m_CommandODBC.Parameters.AddWithValue("@related_to", objectType);
            m_CommandODBC.Parameters.AddWithValue("@related_to_id", objectId);
            m_CommandODBC.Parameters.AddWithValue("@chat_session_id", chatid);
            nResultCode = m_CommandODBC.ExecuteNonQuery();
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "update_chat_status", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "update_chat_status", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion
    
    #region Get CustomerStatus
    [WebMethod]
    public DataSet GetContactStatus(int loginid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " + m_Connection.DB_NULL + "(session_user_mode,'Available') as session_user_mode,(dbo.GetContactName(session_contact_id))as customername from crm_user_sessions" +
                    " where session_active='Y' and session_contact_id =" + loginid + " " + m_Connection.DB_TOP_MYSQL;

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "customerstatus";

            nResultCode = 0;
            strResult = "pass";
        }

        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetRecentItems", strParameters);
        }

        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetRecentItems", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get Customer List
    [WebMethod]
    public DataSet GetContactList(int loginid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT session_contact_id,(dbo.GetContactName(session_contact_id))as customername," + m_Connection.DB_NULL + "(session_user_mode,'Available') as  session_user_mode from crm_user_sessions " +
                    "WHERE session_active='Y' and session_contact_id <>" + loginid + " and " + m_Connection.DB_NULL + "(session_user_mode,'Available') <>'Invisible'";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "CustomerList";
            nResultCode = 0;
            strResult = "pass";
        }

        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetRecentItems", strParameters);
        }

        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetRecentItems", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    [WebMethod(EnableSession = true)]
    public DataSet GetContact(string prefixText)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));
        int loginid = Convert.ToInt32(HttpContext.Current.Session["contact_id"].ToString());

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT session_contact_id,(dbo.GetContactName(session_contact_id))as customername," + m_Connection.DB_NULL + "(session_user_mode,'Available') as  session_user_mode from crm_user_sessions " +
                    "WHERE session_active='Y' and session_contact_id <>" + loginid + " and " + m_Connection.DB_NULL + "(session_user_mode,'Available') <>'Invisible' and (dbo.GetContactName(session_contact_id)) like '%" + prefixText + "%'";

            //strTemp = "SELECT session_contact_id,(dbo.GetContactName(session_contact_id))as customername," +
            //            " isnull(session_user_mode,'Available') as  session_user_mode " +
            //            " from crm_user_sessions " +
            //            " WHERE session_active='Y'" +
            //            " and session_contact_id not in (select * from dbo.Split(dbo.GetChatMember('22ac6ea2-b83c-4539-ba32-568685237fe7'),','))" +
            //            " and isnull(session_user_mode,'Available') <>'Invisible'" +
            //            " and (dbo.GetContactName(session_contact_id)) like '%"+prefixText+"%'";

            //m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            //m_CommandODBC.CommandType = CommandType.Text;
            //m_CommandODBC.Parameters.Add("id", OdbcType.Int).Value = opfId;
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "CustomerList";
            nResultCode = 0;
            strResult = "pass";

            //List<string> CountryNames = new List<string>();
            //for (int i = 0; i < m_DataSet.Tables[0].Rows.Count; i++)
            //{
            //    CountryNames.Add(m_DataSet.Tables[0].Rows[i][1].ToString());
            //}
            //return CountryNames;
        }

        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetRecentItems", strParameters);
        }

        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetRecentItems", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

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

    #region GetChatHistory
    [WebMethod]
    public DataSet Get_ChatHistory(string chatseesionid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " chat_conversation_text crm_chat_sessions from crm_chat_sessions " +
                      "WHERE chat_session_id=? " + m_Connection.DB_TOP_MYSQL;
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.AddWithValue("@chat_session_id", chatseesionid);
            strResult = Convert.ToString(m_CommandODBC.ExecuteScalar());
            nResultCode = 0;
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetChatHistory", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetChatHistory", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Save_new_Chat_Entry
    [WebMethod]
    public DataSet Save_new_Chat_Entry(string SourceID, string DestinationID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            string seesionid = "";
            m_Connection.OpenDB("Galaxy");

            //strTemp = "SELECT chat_session_id from crm_chat_sessions where ((chat_destination_id=" + DestinationID + " AND chat_source_id=" + SourceID + ") or (chat_destination_id=" + SourceID + " AND chat_source_id=" + DestinationID + ")) AND chat_status='Accepted'";
            strTemp = @"SELECT chat_session_id from crm_chat_sessions where ((chat_destination_id=" + DestinationID +
                @" AND chat_source_id=" + SourceID +
                @") or (chat_destination_id=" + SourceID +
                @" AND chat_source_id=" + DestinationID + ")) AND chat_status<>'Closed' ";

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            strResult = Convert.ToString(m_CommandODBC.ExecuteScalar());
            if (strResult == "")
            {
                seesionid = System.Guid.NewGuid().ToString();
                strTemp = " INSERT INTO crm_chat_sessions " +
                          "(chat_source_id," +
                          "chat_source_msg_count," +
                          "chat_visitor_name," +
                          "chat_visitor_ip," +
                          "chat_status," +
                          "chat_session_id," +
                          "chat_type, " +
                          "chat_destination_id, " +
                          "chat_destination_name," +
                          "useable," +
                          "last_activity_date," +
                          "created_by," +
                          "created_ip," +
                          "created_date," +
                          "modified_by," +
                          "modified_ip," +
                          "modified_date )" +
                          " values(?," +
                          "0," + m_Connection.DB_FUNCTION + "GetContactName(" + SourceID + ")," +
                          "'" + Context.Request.UserHostAddress + "'," +
                          "'New','" + seesionid + "','I',?," + m_Connection.DB_FUNCTION + "GetContactName(" + DestinationID + ")," +
                          "'Y'," + m_Connection.DB_UTC_DATE + "," +
                          "" + SourceID + ",'" + Context.Request.UserHostAddress + "'," + m_Connection.DB_UTC_DATE + "," + SourceID + ",'" + Context.Request.UserHostAddress + "'," + m_Connection.DB_UTC_DATE + ")";

                m_CommandODBC.CommandText = strTemp;
                m_CommandODBC.Parameters.AddWithValue("@chat_source_id", SourceID);
                m_CommandODBC.Parameters.AddWithValue("@chat_destination_id", DestinationID);
                nResultCode = m_CommandODBC.ExecuteNonQuery();
                if (nResultCode > 0)
                {
                    strResult = seesionid;
                }
            }
            else
            {
                nResultCode = 0;
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Save_new_Chat_Entry", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Save_new_Chat_Entry", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    [WebMethod]
    public DataSet Save_new_Chat_Entry_group(string SourceID, string DestinationID, string sessionid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");

            //strTemp = "SELECT chat_session_id from crm_chat_sessions where ((chat_destination_id=" + DestinationID + " AND chat_source_id=" + SourceID + ") or (chat_destination_id=" + SourceID + " AND chat_source_id=" + DestinationID + ")) AND chat_status='Accepted'";
            //  strTemp = "SELECT chat_session_id from crm_chat_sessions where chat_session_id='" + sessionid + "' and ((chat_destination_id=" + DestinationID + " AND chat_source_id=" + SourceID + ") or (chat_destination_id=" + SourceID + " AND chat_source_id=" + DestinationID + ")) AND chat_status<>'Closed'";
            strTemp = "SELECT COUNT(*) from crm_chat_sessions where chat_session_id='" + sessionid + "'and " + DestinationID +
                " in (select Items from " + m_Connection.DB_FUNCTION + "Split(" + m_Connection.DB_FUNCTION + "GetCurrentChatMember('" + sessionid + "'),',')) ";


            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            nResultCode = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
            if (nResultCode == 0)
            {
                strTemp = " update crm_chat_sessions set " +
                          "current_group_member_ids=isnull(current_group_member_ids,'')+'," + DestinationID + "'" +
                          ",group_member_ids=isnull(group_member_ids,'')+'," + DestinationID + "'" +
                          ",modified_by =" + SourceID +
                          ",modified_ip='" + Context.Request.UserHostAddress + "'" +
                          ",modified_date=" + m_Connection.DB_UTC_DATE +
                          " where chat_session_id='" + sessionid + "'";
                m_CommandODBC.CommandText = strTemp;
                nResultCode = m_CommandODBC.ExecuteNonQuery();
                if (nResultCode > 0)
                {
                    strResult = sessionid;
                }
            }
            else
            {
                nResultCode = 0;
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Save_new_Chat_Entry_group", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Save_new_Chat_Entry_group", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion
    #region open_new_Chat_Entry
    [WebMethod]
    public DataSet open_new_Chat_Entry(string SourceID, string timeZoneTimeSpan)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        string chatwaitnigtime = System.Configuration.ConfigurationManager.AppSettings["CHATWAITTIME"].ToString();
        if (chatwaitnigtime == "")
            chatwaitnigtime = "0";
        try
        {

            m_Connection.OpenDB("Galaxy");

            //strTemp = "SELECT chat_session_id from crm_chat_sessions where ((chat_destination_id=" + DestinationID + " AND chat_source_id=" + SourceID + ") or (chat_destination_id=" + SourceID + " AND chat_source_id=" + DestinationID + ")) AND chat_status='Accepted'";
            //strTemp = "SELECT chat_session_id,chat_destination_id from crm_chat_sessions where ((chat_source_id=" + SourceID + ") or (chat_destination_id=" +
            //    SourceID + ")) AND chat_status <> 'Closed' and exists (select chat_session_id from crm_chat where  crm_chat.chat_session_id=crm_chat_sessions.chat_session_id)";
            strTemp = "SELECT chat_type,chat_session_id,chat_destination_id from crm_chat_sessions where " +
                // " chat_type='I' and" +
                        "  ((chat_source_id=" + SourceID + ") or (chat_destination_id=" + SourceID + ") or " + SourceID +
                        " in (select Items from dbo.Split(current_group_member_ids,','))) AND chat_status <> 'Closed'" +
                        " and exists (select chat_session_id from crm_chat where  crm_chat.chat_session_id=crm_chat_sessions.chat_session_id)";
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "chatrequest";


            //strTemp = @"select * from crm_chat_queue " +
            //                "where chatque_queue_id in (select queue_id from crm_queue_members where queue_member_contact_id=" + SourceID + ")" +
            //                "and chatque_request_type='New' " +
            //                "and chatque_request_status='N' " +
            //                "and DATEDIFF(MI,chatque_request_time,GETUTCDATE())<=" + chatwaitnigtime + " and " + SourceID + 
            //                " not in (select * from split(chat_declined_contact_id,','))  order by chatque_request_time";

            strTemp = @"select chat_visitor_id,chat_visitor_name,chat_visitor_email,chat_visitor_query,DATEADD(mi," + timeZoneTimeSpan + ",chat_request_time) as chat_request_time,chat_session_id,chat_visitor_phone,chat_response_type from crm_chat_sessions " +
                          "where chat_queue_id in (select queue_id from crm_queue_members where queue_member_contact_id=" + SourceID + ")" +
                          "and chat_status='New' " +
                          "and chat_response_type='N' " +
                          "and chat_type<>'I' " +
                          "and DATEDIFF(MI,chat_request_time,GETUTCDATE())<" + chatwaitnigtime + " and " + SourceID +
                          " not in (select * from split(chat_declined_contact_id,','))  order by chat_request_time";



            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            DataTable dtTable = new DataTable("ChatRequest");
            m_DataAdapterODBC.Fill(dtTable);
            m_DataSet.Tables.Add(dtTable);


            strResult = "Pass";
            nResultCode = 0;
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "open_new_Chat_Entry", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "open_new_Chat_Entry", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region getrequeststatus
    [WebMethod]
    public DataSet CheckRequestStatus(int chatid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + "isnull(chat_status,'')chat_status from crm_chat_sessions " +
                      "WHERE chat_visitor_id=? " + m_Connection.DB_TOP_MYSQL;
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.AddWithValue("@id", chatid);
            strResult = Convert.ToString(m_CommandODBC.ExecuteScalar());
            nResultCode = 0;
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "CheckRequestStatus", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "CheckRequestStatus", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    #endregion

    #region GetExsistingRequest
    [WebMethod]
    public DataSet GetExsistingRequest(int ContactID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT id,chat_session_id as SeesionID,chat_source_id as SourceID,chat_destination_id as DestinationID from crm_chat_sessions" +
                      " where (chat_source_id=" + ContactID + " or chat_destination_id=" + ContactID + ") AND " + m_Connection.DB_NULL + "(chat_end_time," + m_Connection.DB_UTC_DATE + ")>modified_date AND chat_status='Accepted'";
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "chatsession";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetExsistingRequest", strParameters);
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetExsistingRequest", strParameters);
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        }
        finally
        {
            m_Connection.CloseDB();
        }

        return m_DataSet;
    }
    #endregion

    #region ModifyChatEndTime
    [WebMethod]
    public DataSet ModifyChatEndTime(string ChatSessionID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_Connection.OpenDB("Galaxy");

        try
        {
            strTemp = "Update crm_chat_sessions set modified_date=chat_end_time where chat_session_id = ? ";
            OdbcCommand m_CommandODBC = new OdbcCommand();
            m_CommandODBC.Connection = m_Connection.oCon;
            m_CommandODBC.CommandText = strTemp;
            m_CommandODBC.Parameters.AddWithValue("@ChatSessionID", ChatSessionID);
            nResultCode = m_CommandODBC.ExecuteNonQuery();
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "CheckRequestStatus", strParameters);
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "CheckRequestStatus", strParameters);
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        }
        finally
        {
            m_Connection.CloseDB();
        }

        return m_DataSet;
    }
    #endregion

    #region Update Customer Status
    [WebMethod]
    public DataSet ChangeCustomerStatus(int loginid, string customerstatus)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));
        m_Connection.OpenDB("Galaxy");
        OdbcCommand m_CommandODBC = new OdbcCommand();
        try
        {
            strTemp = "Update crm_contacts Set contact_chat_status='" + customerstatus + "'  " +
                    " where id =" + loginid;
            m_CommandODBC.Connection = m_Connection.oCon;
            m_CommandODBC.CommandText = strTemp;
            nResultCode = m_CommandODBC.ExecuteNonQuery();

            strTemp = "Update crm_user_sessions Set session_user_mode='" + customerstatus + "'  " +
                    " where session_active='Y' and session_contact_id =" + loginid;
            m_CommandODBC.Connection = m_Connection.oCon;
            m_CommandODBC.CommandText = strTemp;
            nResultCode = m_CommandODBC.ExecuteNonQuery();
            strResult = "pass";
        }

        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetRecentItems", strParameters);
        }

        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetRecentItems", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region GetExsistingRequest
    [WebMethod]
    public DataSet GetCustomerName(string chatseesionid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " chat_source_id as SourceID,chat_destination_id as DestinationID,chat_visitor_name,chat_destination_name," +
                      "chat_type,isnull(chat_session_status,'N')chat_session_status from crm_chat_sessions " +
                      "WHERE chat_session_id='" + chatseesionid + "' " + m_Connection.DB_TOP_MYSQL;

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "chatsessiondetail";
            //manju
            strTemp = "select [dbo].[queueToolTip]('" + chatseesionid + "')";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[1].TableName = "details";
            strResult = "Pass";
            nResultCode = 0;
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetCustomerName", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetCustomerName", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    [WebMethod]
    public DataSet GetAllCustomerName(string chatseesionid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select Items id,case when Items =0 then (select top 1 chat_visitor_name from crm_chat_sessions where chat_session_id='" + chatseesionid +
                "') else dbo.GetContact(Items) end as Name  from dbo.Split(dbo.GetChatMember('" + chatseesionid + "'),',')";
            //strTemp = "select chat_source_id as id,dbo.GetContact(chat_source_id) as Name from crm_chat_sessions where chat_session_id='" + chatseesionid + "' union " +
            //    "select chat_destination_id as id,dbo.GetContact(chat_destination_id) as Name from crm_chat_sessions where chat_session_id='" + chatseesionid + "'";
            //strTemp = "SELECT chat_source_id as SourceID,chat_destination_id as DestinationID,dbo.GetContact(chat_source_id) Source,dbo.GetContact(chat_destination_id) from crm_chat_sessions " +
            //          "WHERE chat_session_id='" + chatseesionid + "' ";
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "chatsessiondetail";
            strResult = "Pass";
            nResultCode = 0;
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetAllCustomerName", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetAllCustomerName", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region DisposeSession

    [WebMethod]
    public DataSet DisposeSession(int ContactID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "UPDATE crm_chat_sessions SET chat_status ='Closed' " +
                      " chat_end_time= " + m_Connection.DB_UTC_DATE +
                      " WHERE chat_status not in ('Closed','Offline','New') and (chat_destination_id=" + ContactID + " OR chat_source_id=" + ContactID + ") ";
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            nResultCode = m_CommandODBC.ExecuteNonQuery();
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "DisposeSession", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "DisposeSession", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region GetChatImage

    
    [WebMethod]
    public DataSet GetChatImage(int queueid, string type)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");


            strTemp = "select count(*) from crm_user_sessions where session_active = 'Y' and session_contact_id in (select queue_member_contact_id from crm_queue_members where queue_id=" + queueid + ")";
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            nResultCode = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
            if (nResultCode > 0)
            {
                //strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " image_online as imagepath from crm_chat_images " +
                //       "WHERE queue_id=" + queueid + " " + m_Connection.DB_TOP_MYSQL;

                strTemp = "select image_path from crm_image_master where image_name='Online" + type + "'";
            }
            else
            {
                //strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " image_offline as imagepath from crm_chat_images " +
                //           "WHERE queue_id=" + queueid + " " + m_Connection.DB_TOP_MYSQL;
                strTemp = "select image_path from crm_image_master where image_name='Offline" + type + "'";
            }
            m_CommandODBC.CommandText = strTemp;
            strResult = Convert.ToString(m_CommandODBC.ExecuteScalar());

        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetChatImage", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetChatImage", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region GetChatUser

    [WebMethod]
    public DataSet GetChatUser(int queueid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");


            strTemp = @" select top 1 session_contact_id from crm_user_sessions
                    where session_active = 'Y' 
                    and session_contact_id 
 in (select queue_member_contact_id from crm_queue_members where queue_id=" + queueid + " and not exists" +
"(select * from crm_chat_sessions where chat_type='E' and (chat_status<>'Closed' and DATEDIFF(MI, created_date,GETUTCDATE())<=30) and chat_destination_id =queue_member_contact_id) )";
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            nResultCode = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
            strResult = "Pass";

        }
        catch (OdbcException ex)
        {
            nResultCode = -2;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetChatImage", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetChatImage", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion


    #region GetContactList
    [WebMethod]
    public DataSet GetQueueContactList(int ContactID, int DestinationID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select count(*) as [count] from crm_queue_members  where queue_id=" + DestinationID + " and queue_member_contact_id=" + ContactID + "";
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            nResultCode = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetQueueContactList", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetQueueContactList", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region GetExistingcustomer
    [WebMethod]
    public DataSet GetExistingcustomer(string EmailID, string ContactNO, string SessionID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select distinct " + m_Connection.DB_TOP_SQL + " related_to_id from crm_contact_details where type='Email' " +
                      " and [value]='" + EmailID + "' " +
                      " and related_to_id in (select distinct related_to_id from crm_contact_details where value='" + ContactNO + "' and type='Phone') " + m_Connection.DB_TOP_MYSQL + "";
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            nResultCode = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
            strResult = "Pass";

            if (nResultCode != null)
            {
                if (nResultCode > 0)
                {
                    strTemp = "Update crm_chat_queue Set contact_id=" + nResultCode +
                     "where id=" + SessionID;
                    m_CommandODBC.CommandText = strTemp;
                    nResultCode = m_CommandODBC.ExecuteNonQuery();
                }
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetExistingcustomer", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetExistingcustomer", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion
    
    #region Chat Send and Receive
    [WebMethod(EnableSession = true)]
    public bool SendMessage(string sessionId, string message, string type)
    {
        int nUserID = 0;
        if (HttpContext.Current.Session["contact_id"] != null)
            nUserID = Convert.ToInt32(HttpContext.Current.Session["contact_id"].ToString());
        try
        {
            m_Connection.OpenDB("Galaxy");

            //strTemp = "Insert into crm_chat(chat_session_id,chat_message,chat_send_time,chat_from_id,chat_to_Id,Type)" +
            //         "select ?,?,GETDATE(),?,Items,'" + type + "' from dbo.Split(dbo.GetChatMember(?),',') where Items<>" + nUserID;



            OdbcCommand m_CommandODBC = new OdbcCommand("EXEC CH_insert_chat ?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.AddWithValue("@szSessionId", sessionId);
            m_CommandODBC.Parameters.AddWithValue("@szMessage", message);
            m_CommandODBC.Parameters.AddWithValue("@nUserID", nUserID);
            m_CommandODBC.Parameters.AddWithValue("@szType", type);
            int id = m_CommandODBC.ExecuteNonQuery();

            if (id > 0)
                return true;
            else
                return false;
        }
        catch
        {
            return false;
        }
    }


    [WebMethod(EnableSession = true)]
    public DataSet ReceivedMassege(string sessionId, int chatid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        int nUserID = 0;
        if (HttpContext.Current.Session["contact_id"] != null)
            nUserID = Convert.ToInt32(HttpContext.Current.Session["contact_id"].ToString());
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT chat_id id, chat_message message,chat_from_id fid,chat_send_time time,Type from crm_chat WHERE chat_session_id=? and chat_Id>" + chatid.ToString() + " and (chat_from_id=" +
                nUserID.ToString() + " or chat_to_Id=" + nUserID.ToString() + ")order by chat_send_time";
            //strTemp = "SELECT chat_message,chat_from_id,chat_send_time from crm_chat " +
            //          "WHERE chat_session_id=? order by chat_send_time";

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_CommandODBC.Parameters.Add("@sessionId", OdbcType.VarChar).Value = sessionId;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "chatdetails";
            nResultCode = 0;
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "chatdetails", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "chatdetails", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        // m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }




    #endregion


    [WebMethod]
    public string GetUnreadChat(int userID)
    {
        int nResultCode = -1;
        string strResult = "";
        // DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            OdbcCommand m_CommandODBC = new OdbcCommand("exec usp_unread_chat ?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@nID", OdbcType.Int).Value = userID;
            strResult = Convert.ToString(m_CommandODBC.ExecuteScalar());
        }

        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetUnreadChat", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        //m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return strResult;
    }

    #region Insert in Chat Queue
    [WebMethod]
    public DataSet InsertChatQueue(int chatid, string name, string email, string mobile, string companyname,string desc, int queueid, string requesttype, string type)
    {

        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            //strTemp = @"INSERT INTO crm_chat_queue(contact_id,chatque_name,chatque_email, chatque_mobile, chatque_desc, chatque_request_type," +
            //    " chatque_queue_id, chatque_request_status, chatque_request_time, chatque_request_ip)" +
            //    "VALUES(0,?,?,?,?,?,?,?," + m_Connection.DB_UTC_DATE + ",?)";

            m_CommandODBC = new OdbcCommand("EXEC chat_queue_visitor_request ?,?,?,?,?,?,?,?,?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_CommandODBC.Parameters.AddWithValue("@nChatqueId", chatid);
            m_CommandODBC.Parameters.AddWithValue("@nContactId", 0);
            m_CommandODBC.Parameters.AddWithValue("@szName", name);
            m_CommandODBC.Parameters.AddWithValue("@szEmail", email);
            m_CommandODBC.Parameters.AddWithValue("@szMobile", mobile);
            m_CommandODBC.Parameters.AddWithValue("@companyname", companyname);
            
            m_CommandODBC.Parameters.AddWithValue("@szDesc", desc);
            m_CommandODBC.Parameters.AddWithValue("@szRequestType", requesttype);
            m_CommandODBC.Parameters.AddWithValue("@szQueueId", queueid);
            m_CommandODBC.Parameters.AddWithValue("@szRequestStatus", "N");
            m_CommandODBC.Parameters.AddWithValue("@szRequestIp", Context.Request.UserHostAddress);
            m_CommandODBC.Parameters.AddWithValue("@szType", type);
                       
            nResultCode = Convert.ToInt32(m_CommandODBC.ExecuteScalar()); ;
            //if (nResultCode > 0)
            //{
            //    strTemp = " select @@IDENTITY";
            //    m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            //    m_CommandODBC.CommandType = CommandType.Text;
            //    nResultCode =Convert.ToInt32( m_CommandODBC.ExecuteScalar());
            //    strResult = "Pass";
            //}
            string ss = "abcda";
            char[] aaa = ss.ToCharArray();
            string reap = "";
            int count=0;
            for (int i = 0; i < aaa.Length; i++)
			{
                
                for (int j = 0; j < aaa.Length; j++)
                {
                    if (aaa[i] == aaa[j])
                    {
                        count++;
                    }
                }

                if (aaa.Length - 1 == i && count == 1)
                {
                    reap += aaa[i].ToString();
                    count++;
                }
			}
        }

            //aa="bcd"

          



        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "InsertChatQueue", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "InsertChatQueue", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    [WebMethod]
    public DataSet ChatResponseCheck(string id)
    {
        int nResultCode = -1;
        string strResult = "";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = @"select chat_session_id chatque_session_id from crm_chat_sessions where id=? and chat_status not in ('Closed','Offline')";
            //strTemp = @"select chatque_session_id from crm_chat_queue where id=? and chatque_request_type='Accepted'";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_CommandODBC.Parameters.AddWithValue("@chatque_name", id);
            strResult = Convert.ToString(m_CommandODBC.ExecuteScalar());
            nResultCode = 0;
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ChatResponseCheck", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ChatResponseCheck", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    [WebMethod]
    public DataSet MissChatMail(string id,string query)
    {
        int nResultCode = -1;
        string strResult = "";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            m_Connection.SaveRecentActivity(0, "CHT", Convert.ToInt32(id), "SALESCHATMISS", query, 0, "P", 2455, "");
            strResult = "pass";
            nResultCode = 0;
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "MissChatMail", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "MissChatMail", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    //#region Get Visitor chat request
    //[WebMethod]
    //public DataSet GetChatQueueRequest(int id)
    //{
    //    int nResultCode = -1;
    //    string strResult = "Fail - ";
    //    DataSet m_DataSet = new DataSet();

    //    try
    //    {
    //        m_Connection.OpenDB("Galaxy");
    //        strTemp = @"select * from crm_chat_queue " +
    //                     "where chatque_queue_id in (select queue_id from crm_queue_members where queue_member_contact_id=" + id + ")" +
    //                     //"and chatque_request_type='New' " +
    //                    // "and chatque_request_status='N' " +
    //                     "and DATEDIFF(MI,chatque_request_time,GETUTCDATE())<=60  order by chatque_request_time";
    //        //strTemp = "SELECT " + m_Connection.DB_TOP_SQL5 + " id as ID," + m_Connection.DB_NULL + "(chat_visitor_name,'') as VisitorName," +
    //        //            m_Connection.DB_NULL + "(chat_visitor_query,'') as VisitorQuery,created_date as CreatedDate," +
    //        //            "chat_type as ChatType," + m_Connection.DB_NULL + "(chat_destination_id,0) as DestinationID, chat_session_id as ChatSessionID, " +
    //        //            m_Connection.DB_FUNCTION + "GetContactType(chat_destination_id) as ContactType " +
    //        //        "FROM crm_chat_sessions WHERE chat_status = 'New' " +
    //        //        "ORDER BY chat_start_time " + m_Connection.DB_TOP_MYSQL5;
    //        // AND chat_type = 'E'
    //        OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
    //        OdbcDataAdapter m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
    //        DataTable dtTable = new DataTable("ChatRequest");
    //        m_DataAdapterODBC.Fill(dtTable);
    //        if (dtTable.Rows.Count > 0)
    //        {
    //            nResultCode = 0;
    //            strResult = "Pass";
    //            m_DataSet.Tables.Add(dtTable);
    //        }
    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "GetChatRequest", strParameters);
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "GetChatRequest", strParameters);
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //    }
    //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
    //    return m_DataSet;
    //}
    //#endregion
    #region Get Visitor chat request
    [WebMethod]
    public DataSet GetChatQueueDetails(int id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = @"select * from crm_chat_queue where id=" + id;

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            DataTable dtTable = new DataTable("ChatRequest");
            m_DataAdapterODBC.Fill(dtTable);
            if (dtTable.Rows.Count > 0)
            {
                nResultCode = 0;
                strResult = "Pass";
                m_DataSet.Tables.Add(dtTable);
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetChatQueueDetails", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetChatQueueDetails", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    [WebMethod]
    public DataSet InsertChatRequest(int chatqueueid, int contactId, string sessionId, string ip)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("EXEC chat_visitor_request_accept ?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@nchatQueueId", OdbcType.Int).Value = chatqueueid;
            m_CommandODBC.Parameters.Add("@nContactId", OdbcType.Int).Value = contactId;
            m_CommandODBC.Parameters.Add("@szSessionId", OdbcType.VarChar).Value = sessionId;
            m_CommandODBC.Parameters.Add("@szIp", OdbcType.VarChar).Value = ip;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            nResultCode = 0;
            strResult = "Pass";
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string InsertVisitor(string type,string url,string title, string srchingeng,int ii)
    {
     
        int nResultCode = -1;
        string strResult = "Fail - ";
        string id = "0";
        string device = "PC";

        string ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }

        //string ipAddress =HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];//"122.160.24.159";//
        string previouspageurl = HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
        string host = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
        string useragent = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
        string browser = HttpContext.Current.Request.Browser.Browser;
        string version = HttpContext.Current.Request.Browser.Version;
        string plateform = HttpContext.Current.Request.Browser.Platform;
        
        if (HttpContext.Current.Request.Browser.IsMobileDevice)
            device = "Mobile";
        

         string APIKey = "133b4b51cea45c6ac8165d19083a239d888e68ebd01d26c4958b6ba1f71661bc";
        string apiurl = string.Format("http://api.ipinfodb.com/v3/ip-city/?key={0}&ip={1}&format=json", APIKey, ipAddress);
        Location location=new Location();
        using (WebClient client = new WebClient())
        {
            try
            {
                string json = client.DownloadString(apiurl);
                location = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Location>(json);
                List<Location> locations = new List<Location>();
                locations.Add(location);
            }
            catch (Exception ex)
            {
                
                
            }
            
         //   ip_location = location.RegionName.ToString() + "," + location.CityName.ToString();

        }

        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC insert_visitor ?,?,?,?,? ,?,?,?,?,? ,?,?,?,?,? ,?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@type", OdbcType.VarChar, 1).Value = type;
            m_CommandODBC.Parameters.Add("@country", OdbcType.VarChar, 50).Value = location.CountryName;
            m_CommandODBC.Parameters.Add("@countryCode", OdbcType.VarChar, 50).Value = location.CountryCode;
            m_CommandODBC.Parameters.Add("@city", OdbcType.VarChar, 50).Value = location.CityName;
            m_CommandODBC.Parameters.Add("@RegionName", OdbcType.VarChar, 50).Value = location.RegionName;
            
            m_CommandODBC.Parameters.Add("@ZipCode", OdbcType.VarChar, 50).Value = location.ZipCode;
            m_CommandODBC.Parameters.Add("@Latitude", OdbcType.VarChar, 50).Value = location.Latitude;
            m_CommandODBC.Parameters.Add("@Longitude", OdbcType.VarChar, 50).Value = location.Longitude;
            m_CommandODBC.Parameters.Add("@TimeZone", OdbcType.VarChar, 50).Value = location.TimeZone;
            m_CommandODBC.Parameters.Add("@chatque_browser", OdbcType.VarChar, 50).Value = browser;
			
            m_CommandODBC.Parameters.Add("@chatque_platform", OdbcType.VarChar, 50).Value = plateform;
            m_CommandODBC.Parameters.Add("@chatque_device", OdbcType.VarChar, 50).Value = device;
            m_CommandODBC.Parameters.Add("@chatque_ip_address", OdbcType.VarChar, 50).Value = ipAddress;
            m_CommandODBC.Parameters.Add("@chatque_hostname", OdbcType.VarChar, 50).Value = host;
            m_CommandODBC.Parameters.Add("@chatque_user_agent", OdbcType.VarChar, 50).Value = useragent;
            
            m_CommandODBC.Parameters.Add("@chatque_searching_engine", OdbcType.VarChar, 50).Value = srchingeng??"";
            m_CommandODBC.Parameters.Add("@url", OdbcType.VarChar, 225).Value = url?? "";
            m_CommandODBC.Parameters.Add("@title", OdbcType.VarChar, 50).Value = title?? "";
            m_CommandODBC.Parameters.Add("@id", OdbcType.Int, 4).Value = ii;
         //   m_CommandODBC.Parameters.Add("@version", OdbcType.VarChar, 50).Value = version;
            id = Convert.ToString(m_CommandODBC.ExecuteScalar());
            nResultCode = 0;
            strResult = "Pass";
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

        return id;
    }
   


    [WebMethod]
    public DataSet getchathistory(string sessionId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("EXEC chathistory ?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@id", OdbcType.VarChar).Value = sessionId;

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            nResultCode = 0;
            strResult = "Pass";
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    [WebMethod]
    public DataSet getchatdetails(int Id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("EXEC chathistorydetail ?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@id", OdbcType.Int).Value = Id;

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            nResultCode = 0;
            strResult = "Pass";
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    [WebMethod]
    public DataSet getchattemplatedetails(string Id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");


            strTemp = "select template_name as [Text],template_message as Value from chat_template";
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Templatedata";
                strResult = "Pass";
                nResultCode = 0;

            //strTemp = "select template_pop_subject,template_pop_text from crm_message_templates where template_for='CHT' " + m_Connection.DB_TOP_MYSQL;

            //OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            //m_DataAdapterOdbc.Fill(m_DataSet);
            //m_DataSet.Tables[0].TableName = "TEMPLATE";

            //m_CommandODBC = new OdbcCommand("EXEC getchattemplatedetails ?", m_Connection.oCon);
            //m_CommandODBC.CommandType = CommandType.StoredProcedure;
            //m_CommandODBC.Parameters.Add("@sessionid", OdbcType.VarChar).Value = Id;


            //m_DataAdapterOdbc = new OdbcDataAdapter((m_CommandODBC));
            //m_DataAdapterOdbc.Fill(m_DataSet);
            //m_DataSet.Tables[1].TableName = "details";
            //if (m_DataSet.Tables[1].Columns.Contains("Result"))
            //{
            //    strResult = m_DataSet.Tables[1].Rows[0][0].ToString();
            //    nResultCode = -1;
            //}
            //else
            //{
            //    strResult = "Pass";
            //    nResultCode = 0;
            //}


            //DataTable table = new DataTable();
            //table.TableName = "Templatedata";
            //table.Columns.Add("Text");
            //table.Columns.Add("Value");

            //for (int i = 0; i < m_DataSet.Tables[0].Rows.Count; i++)
            //{
            //    string result = string.Empty;


            //    string[] words = Convert.ToString(m_DataSet.Tables["TEMPLATE"].Rows[i]["template_pop_text"]).Split('#');

            //    foreach (string word in words)
            //    {
            //        if (word != "")
            //        {
            //            if (word.IndexOf(" ") < 0)
            //            {
            //                result += Convert.ToString(m_DataSet.Tables["details"].Rows[0][word]);
            //            }
            //            else
            //            {
            //                result += word;
            //            }
            //        }
            //    }

            //    table.Rows.Add(Convert.ToString(m_DataSet.Tables["TEMPLATE"].Rows[i]["template_pop_subject"]), result);

            //}

            //m_DataSet.Tables.Add(table);
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "template", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "template", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    [WebMethod]
    public string ChatTicket(int chat_id)
    {
        DataBase m_Connection = new DataBase();
        DataTable dt1 = null;
        int nResultCode = -1;
        string strResult = "Fail";
        string strTemp = "";
        int ownerId = 3;
        m_Connection.OpenDB("Galaxy");

        //
        try
        {
            int nFlag = 0;
            string CaseNumber = "";
            string RelatedTo = "";
            string RelatedToID = "";
            string MailSubject = "";
            string email_id = "";
            string productName = "";
            string productcode = "0";
            string customername = "";
            string customerphone = "";
            string customercategory = "";
            XmlDocument xMainDoc = null;
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " id,chat_visitor_email,chat_visitor_phone,chat_visitor_query,chat_visitor_name " +
                     "FROM crm_chat_sessions WHERE id = ? " + m_Connection.DB_TOP_MYSQL;

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@chat_id", OdbcType.Int).Value = chat_id;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            MailSubject = Convert.ToString(dtTable.Rows[0]["chat_visitor_query"]);
            email_id = Convert.ToString(dtTable.Rows[0]["chat_visitor_email"]);
            customername = Convert.ToString(dtTable.Rows[0]["chat_visitor_name"]);
            customerphone = Convert.ToString(dtTable.Rows[0]["chat_visitor_phone"]);
           
            xMainDoc = m_Connection.createParameterXML();
            if (dtTable.Rows.Count > 0)
            {

                strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " related_to_id, related_to " +
                "FROM CRM_Contact_Details WHERE Value = ? AND type = 'Phone' " + m_Connection.DB_TOP_MYSQL;
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
                m_CommandODBC.Parameters.Add("@PhoneId", OdbcType.VarChar).Value = Convert.ToString(dtTable.Rows[0]["chat_visitor_phone"]);
                dtTable = new DataTable("Count");
                m_DataAdapterODBC.Fill(dtTable);
                if (dtTable.Rows.Count > 0 && dtTable.Rows.Count == 1)
                {
                    string nRetaltedToId = Convert.ToString(dtTable.Rows[0]["related_to_id"]);
                    string strRetaltedTo = Convert.ToString(dtTable.Rows[0]["related_to"]);
                    strTemp = "SELECT " + m_Connection.DB_FUNCTION + "GetRelatedName('" + strRetaltedTo + "', " + nRetaltedToId + ")";
                    m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                    string caller_name = Convert.ToString(m_CommandODBC.ExecuteScalar());
                    m_Connection.fillParameterXML(ref xMainDoc, "case_caller_name", caller_name, "varchar", "50");
                    m_Connection.fillParameterXML(ref xMainDoc, "case_caller_id", nRetaltedToId, "int", "0");

               //     m_Connection.fillParameterXML(ref xMainDoc, "case_product_id", m_Connection.DB_FUNCTION + "GetContactProductId(" + nRetaltedToId + ")", "function", "0");
                  //  m_Connection.fillParameterXML(ref xMainDoc, "case_product_name", m_Connection.DB_FUNCTION + "GetContactProductName(" + nRetaltedToId + ")", "function", "0");

                  //  m_Connection.fillParameterXML(ref xMainDoc, "case_subproduct_id", m_Connection.DB_FUNCTION + "GetContactSubProductId(" + nRetaltedToId + ")", "function", "0");
                 //   m_Connection.fillParameterXML(ref xMainDoc, "case_subproduct_name", m_Connection.DB_FUNCTION + "GetContactSubProductName(" + nRetaltedToId + ")", "function", "0");

                }
                else
                {
                    m_Connection.fillParameterXML(ref xMainDoc, "case_caller_name", "", "varchar", "50");
                    m_Connection.fillParameterXML(ref xMainDoc, "case_caller_id", "0", "int", "0");
                }
            }

            // m_Connection.fillParameterXML(ref xMainDoc, "case_product_id", productcode, "int", "0");
            //  m_Connection.fillParameterXML(ref xMainDoc, "case_product_name", productName, "varchar", "100");
            m_Connection.fillParameterXML(ref xMainDoc, "case_source", "C", "varchar", "1");
            m_Connection.fillParameterXML(ref xMainDoc, "case_source_value", email_id, "varchar", "100");

            m_Connection.fillParameterXML(ref xMainDoc, "case_status_id", "1", "int", "0");
            m_Connection.fillParameterXML(ref xMainDoc, "case_status_name", "NEW", "varchar", "100");

            m_Connection.fillParameterXML(ref xMainDoc, "case_subject", MailSubject, "varchar", "100");
            //          m_Connection.fillParameterXML(ref xMainDoc, "case_description", MailMessage, "varchar", "500");
            m_Connection.fillParameterXML(ref xMainDoc, "cht_number", customerphone, "varchar", "50");
          //  m_Connection.fillParameterXML(ref xMainDoc, "cht_product", productName, "varchar", "50");
           // m_Connection.fillParameterXML(ref xMainDoc, "cht_category", customercategory, "varchar", "50");
            m_Connection.fillParameterXML(ref xMainDoc, "cht_name", customername, "varchar", "50");

            m_Connection.fillParameterXML(ref xMainDoc, "owner_id", "3", "int", "0");


            m_Connection.BeginTransaction();
            dt1 = m_Connection.SaveTransactionData("CAS", 0, "Y", DateTime.UtcNow, ownerId, Context.Request.ServerVariables["REMOTE_ADDR"].ToString(), xMainDoc);

            if (Convert.ToInt32(dt1.Rows[0]["ResultCode"]) >= 0)
            {
                RelatedTo = "CAS";
                RelatedToID = Convert.ToString(dt1.Rows[0]["ResultCode"]);
                CaseNumber = dt1.Rows[0]["ResultString"].ToString();
                dt1 = m_Connection.SaveRecentActivity(0, "CAS", Convert.ToInt32(dt1.Rows[0]["ResultCode"]), "OFFLINECHAT", MailSubject, ownerId, "P", 2409, "Auto Ticket created from email");
            }
            else
            {
                m_Connection.Rollback();
            }
            strResult = "Pass";
            nFlag = 1;
            xMainDoc = m_Connection.createParameterXML(); m_Connection.fillParameterXML(ref xMainDoc, "related_to", RelatedTo, "varchar", "3");
            m_Connection.fillParameterXML(ref xMainDoc, "related_to_id", RelatedToID, "int", "0");
            m_Connection.fillParameterXML(ref xMainDoc, "related_to_name", m_Connection.DB_FUNCTION + "GetRelatedName('" + RelatedTo + "', " + RelatedToID + ")", "function", "0");
            m_Connection.fillParameterXML(ref xMainDoc, "owner_id", "3", "int", "0");
            m_Connection.BeginTransaction();
            dt1 = m_Connection.SaveTransactionData("CHT", chat_id, "N", DateTime.UtcNow, ownerId, "", xMainDoc);
            dt1 = m_Connection.SaveRecentActivity(0, "CHT", chat_id, "CREATEDCHAT", MailSubject, ownerId, "P", 2409, "Auto mail for this chat ");
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
    public DataSet getchatdetailsOLD(int Id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("EXEC chathistorydetail_old ?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@id", OdbcType.Int).Value = Id;

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            nResultCode = 0;
            strResult = "Pass";
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }


    [WebMethod]
    public DataSet getchathistoryOLD(string sessionId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("EXEC chathistory_old ?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@id", OdbcType.VarChar).Value = sessionId;

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            nResultCode = 0;
            strResult = "Pass";
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
      [WebMethod]
    public DataSet FetchChatTemplateDetails()
    {
         
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        OdbcDataAdapter m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT id,template_name,template_message,enabled FROM chat_template " +
                       " where id > 0 ";

         

            strTemp += " ORDER BY created_date desc";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet); 
            m_DataSet.Tables[0].TableName = "TemplateDetail";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchChatTemplateDetails", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchChatTemplateDetails", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
      [WebMethod]
      public DataSet InsertUpdateChatTemplate(long nTypeID, string strName, string strmessage, string strEnabled)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            string strTemp = "Select Count(*) as count from chat_template " +
                                " where template_name = ? and id<>" + nTypeID;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);


            if (nName > 0 && nTypeID == 0 || (nName > 1 || (nName == 1 && nTypeID >= 1))) // checking the existence the record
            {
                nResultCode = -1;
                strResult = "Short  Name already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else if (nTypeID == 0 && nTypeID == 0) // for inserting the record
            {

                strTemp = "INSERT into chat_template (template_name,template_message,enabled,created_date)" +
                          " Values (?,?,'" + strEnabled + "',getdate())";

             }

            else  // for updating the record
            {
                strTemp = "UPDATE chat_template  " +
                          "SET  template_name = ?, template_message=?,enabled = '" + strEnabled + "'" +
                          "Where id = " + nTypeID + "";
            }

            m_CommandODBC = new OdbcCommand();
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
             m_CommandODBC.Parameters.Add("@Typemessage", OdbcType.VarChar).Value = strmessage;
            m_CommandODBC.ExecuteNonQuery();

            if (nTypeID == 0)
            {
                nResultCode = 0;
                strResult = "Template Added successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            nResultCode = 0;
            strResult = "Designation Updated successfully!";
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

      [WebMethod]
      [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
      internal object InsertChatHistory(int id, string url, string title)
      {
          int nResultCode = -1;
          string strResult = "Fail - ";
          //  string id = "0";
          DataSet m_DataSet = new DataSet();
          try
          {
              m_Connection.OpenDB("Galaxy");
              m_CommandODBC = new OdbcCommand("EXEC Cht_Insert_Page_History ?,?,?", m_Connection.oCon);
              m_CommandODBC.CommandType = CommandType.StoredProcedure;
              m_CommandODBC.Parameters.Add("@nId", OdbcType.Int, 4).Value = id;
              m_CommandODBC.Parameters.Add("@szUrl", OdbcType.VarChar, 255).Value = url;
              m_CommandODBC.Parameters.Add("@szTilte", OdbcType.VarChar, 255).Value = title;
              m_CommandODBC.ExecuteNonQuery();
              nResultCode = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
              strResult = "Pass";
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

          return nResultCode;
      }



    [WebMethod]
    public DataSet GetChatVisitor()
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        //    string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        //   szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");

            OdbcCommand ODBCCommand = new OdbcCommand("DB_ActiveVisitor", m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(ODBCCommand);
            ODBCCommand.CommandType = CommandType.StoredProcedure;


            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";


            nResultCode = 0;
            strResult = "Pass";

        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ChatQueue", strParameters);
        }
        catch (Exception ex)
        {
            LogMessage(strTemp + strResult, "ChatQueue", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    [WebMethod]
    public DataSet GetChatVisitorTittle()
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        //     string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        //    szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");

            OdbcCommand ODBCCommand = new OdbcCommand("DB_PageTitle", m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(ODBCCommand);
            ODBCCommand.CommandType = CommandType.StoredProcedure;


            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";


            nResultCode = 0;
            strResult = "Pass";

        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ChatQueue", strParameters);
        }
        catch (Exception ex)
        {
            LogMessage(strTemp + strResult, "ChatQueue", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    [WebMethod]
    public DataSet GetChatVisitorURL()
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        //   string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        //   szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");

            OdbcCommand ODBCCommand = new OdbcCommand("DB_PageURL", m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(ODBCCommand);
            ODBCCommand.CommandType = CommandType.StoredProcedure;


            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";


            nResultCode = 0;
            strResult = "Pass";

        }
        catch (OdbcException ex)
      {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ChatQueue", strParameters);
        }
        catch (Exception ex)
        {
            LogMessage(strTemp + strResult, "ChatQueue", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    [WebMethod]
    public DataSet GetChatVisitorCountry()
    {
          int nResultCode = -1;
          string strResult = "No record found";
          DataSet m_DataSet = new DataSet();
        //     string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        //     szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");

            OdbcCommand ODBCCommand = new OdbcCommand("DB_VisitorCountry", m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(ODBCCommand);
            ODBCCommand.CommandType = CommandType.StoredProcedure;


            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";


            nResultCode = 0;
            strResult = "Pass";

        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ChatQueue", strParameters);
        }
        catch (Exception ex)
        {
            LogMessage(strTemp + strResult, "ChatQueue", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    [WebMethod]
    public DataSet GetChatServingAgent()
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        //    string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        //    szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

          try
          {
              m_Connection.OpenDB("Galaxy");

            OdbcCommand ODBCCommand = new OdbcCommand("DB_ServingAgent", m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(ODBCCommand);
            ODBCCommand.CommandType = CommandType.StoredProcedure;


              m_DataAdapterOdbc.Fill(m_DataSet);
              m_DataSet.Tables[0].TableName = "Data";


              nResultCode = 0;
              strResult = "Pass";

          }
          catch (OdbcException ex)
          {
              strResult = ex.Message;
            LogMessage(strTemp + strResult, "ChatQueue", strParameters);
          }
          catch (Exception ex)
          {
            LogMessage(strTemp + strResult, "ChatQueue", strParameters);
          }
          finally
          {
              m_Connection.CloseDB();
          }
          m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
          return m_DataSet;
      }

    [WebMethod]
    public string GetChatFeedback(string id, string satisfaction, string feedback)
    {


        int nResultCode = -1;
        string strResult = "Fail - ";
        //  string id = "0";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC Cht_Insert_feedback ?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@nId", OdbcType.VarChar, 50).Value = id;
            m_CommandODBC.Parameters.Add("@feedback", OdbcType.VarChar, 5).Value = feedback;
            m_CommandODBC.Parameters.Add("@satisfaction", OdbcType.VarChar, 255).Value = satisfaction;
            m_CommandODBC.ExecuteNonQuery();
            nResultCode = 0;
            strResult = "Pass";
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

    [WebMethod]
    public string GetChatNotes(string id, string satisfaction)
    {


        int nResultCode = -1;
        string strResult = "Fail - ";
        //  string id = "0";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC Cht_Insert_Notes ?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@nId", OdbcType.VarChar, 50).Value = id;
           
            m_CommandODBC.Parameters.Add("@feedback", OdbcType.VarChar, 1000).Value = satisfaction;
            m_CommandODBC.ExecuteNonQuery();
            nResultCode = 0;
            strResult = "Pass";
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

    [WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string InsertInquiry(string companyName, string name, string emailField, string telephoneField, string comment)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        //  string id = "0";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC Cht_Insert_CustomerEnquiry ?,?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@companyName", OdbcType.VarChar, 100).Value = companyName;
            m_CommandODBC.Parameters.Add("@name", OdbcType.VarChar, 100).Value = name;
            m_CommandODBC.Parameters.Add("@emailField", OdbcType.VarChar, 100).Value = emailField;
            m_CommandODBC.Parameters.Add("@telephoneField", OdbcType.VarChar, 50).Value = telephoneField;
            m_CommandODBC.Parameters.Add("@comment", OdbcType.VarChar, 500).Value = comment;
            m_CommandODBC.ExecuteNonQuery();
            nResultCode = 0;
            strResult = "Pass";
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



    [WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string InsertOrder(string CompanyName, string pinlocation, string LatiLong, string FuelType, string fuelQuantity, string FillingType, 
                              string ContactNumber, string PermissionRequired, string Name, string Email)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        string Distance = "";
        string Duration = "";
        DataSet m_DataSet = new DataSet();
        
        try
        {
            m_Connection.OpenDB("Galaxy");
            string[] strLatiLongTute = LatiLong.ToString().Split(',');
            string strLatitute = strLatiLongTute[0].ToString();
            string strLongtitute = strLatiLongTute[1].ToString();

            // Auto Assign Ticket Neart
                string strtemp = "select b.driver_id as DriverId,B.driver_name as DriverName, B.vehicle_no as VehicleNo,B.latitute as Latitute, B.longititute as Longitute  " +
                                        " from crm_driverlocation B where updatedate is not null AND CONVERT(DATE,updatedate) = CONVERT(DATE,GETDATE()) ORDER BY updatedate DESC";
                OdbcCommand ODBCCommand = new OdbcCommand(strtemp, m_Connection.oCon);
                OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(ODBCCommand);
                ODBCCommand.CommandType = CommandType.Text;
                m_DataAdapterOdbc.Fill(m_DataSet);
                m_DataSet.Tables[0].TableName = "Data";
                if (m_DataSet.Tables[0].Rows.Count > 0)
                {
                    DataTable dtNew = new DataTable();
                    dtNew.Columns.AddRange(new DataColumn[4] {
                    new DataColumn("Driver", typeof(string)),
                    new DataColumn("DriverId", typeof(string)),
                    new DataColumn("Distance", typeof(string)),
                    new DataColumn("Duration",typeof(string)) });

                foreach (DataRow dr in m_DataSet.Tables[0].Rows)
                    {

                        string Lat = dr["Latitute"].ToString();
                        string Long = dr["Longitute"].ToString();
                        string DriverName = dr["DriverName"].ToString();
                        string DriverId = dr["DriverId"].ToString();
                        string strDrLatLong = Lat + "," + Long;
                    
                        string strResponse = getDistanceTest(LatiLong, strDrLatLong, DriverName); //LatiLong
                        string[] szstrResponse = strResponse.ToString().Split('|');
                        Distance = szstrResponse[0].ToString();
                        Duration = szstrResponse[1].ToString();

                        DataRow row = dtNew.NewRow();
                        row["Driver"] = DriverName;
                        row["DriverId"] = DriverId;
                        row["Distance"] = Distance;
                        row["Duration"] = Duration;
                        dtNew.Rows.Add(row);
                    }
                string driverId = "";
                int nMinimumDtns = Convert.ToInt32(dtNew.AsEnumerable().Min(row => row["Distance"]));
                if (nMinimumDtns > 0)
                {
                    foreach (DataRow drNew in dtNew.Rows)
                    {
                        if(nMinimumDtns==Convert.ToInt32(drNew["Distance"]))
                        {
                            driverId = Convert.ToString(drNew["DriverId"]);
                        }
                    }
                }
                else
                {
                    //m_CommandODBC.Parameters.Add("@AssignId", OdbcType.VarChar, 100).Value = "402";

                    driverId = "402";
                }
                m_CommandODBC = new OdbcCommand("EXEC Insert_OfflineOrder_Ticket ?,?,?,?,?,?,?,?,?,?,?,?", m_Connection.oCon);
                m_CommandODBC.CommandType = CommandType.StoredProcedure;
                m_CommandODBC.Parameters.Add("@CompanyName", OdbcType.VarChar, 100).Value = CompanyName;
                m_CommandODBC.Parameters.Add("@PinLocation", OdbcType.NVarChar, 1500).Value = pinlocation;
                m_CommandODBC.Parameters.Add("@Latitute", OdbcType.VarChar, 100).Value = strLatitute;
                m_CommandODBC.Parameters.Add("@Longitute", OdbcType.VarChar, 100).Value = strLongtitute;
                m_CommandODBC.Parameters.Add("@Fueltype", OdbcType.VarChar, 100).Value = FuelType;
                m_CommandODBC.Parameters.Add("@FuelQty", OdbcType.VarChar, 100).Value = fuelQuantity;
                m_CommandODBC.Parameters.Add("@FillingType", OdbcType.VarChar, 100).Value = FillingType;
                m_CommandODBC.Parameters.Add("@Mobile", OdbcType.VarChar, 100).Value = ContactNumber;
                m_CommandODBC.Parameters.Add("@Permission", OdbcType.VarChar, 100).Value = PermissionRequired;
                m_CommandODBC.Parameters.Add("@Name", OdbcType.VarChar, 100).Value = Name;
                m_CommandODBC.Parameters.Add("@Email", OdbcType.VarChar, 100).Value = Email;
                m_CommandODBC.Parameters.Add("@AssignId", OdbcType.VarChar, 100).Value = driverId;

                //m_CommandODBC.Parameters.Add("@CallerId", OdbcType.VarChar, 100).Direction = ParameterDirection.Output;
                int nCount = m_CommandODBC.ExecuteNonQuery();
                if (nCount > 0)
                {
                    nResultCode = 0;
                    strResult = "Pass";

                }
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

    public class test
    {
        public string Distance { get; set; }
    }
    [WebMethod]
    public string getDistanceTest(string source, string destination, string DriverName)
    {

        string distance = "";
        string duration = "";
        string nDistance = "";

        string content = GetJsonData("https://maps.googleapis.com/maps/api/directions/json?key=AIzaSyAmSZwEGMWpCdpj0_3I-IzAkyF2NOF0AD8&origin=" + source + "&destination=" + destination + "&sensor=false");
        JObject obj = JObject.Parse(content);
        try
        {
            distance = (string)obj.SelectToken("routes[0].legs[0].distance.value");
            duration = (string)obj.SelectToken("routes[0].legs[0].duration.text");
            nDistance = distance;

            //lblDistance.Text = DriverName + " - " + nDistance.ToString();
            //lblDuration.Text = duration;
            // dt.Rows.Add(1, DriverName, nDistance, duration);
        }
        catch (Exception ex)
        {
            //lblMessage.Text = ex.Message;
        }
        return Convert.ToString(distance + "|" + duration);
    }
    [WebMethod]
    protected string GetJsonData(string url)
    {
        string sContents = string.Empty;
        string me = string.Empty;
        try
        {
            if (url.ToLower().IndexOf("https:") > -1)
            {
                System.Net.WebClient client = new System.Net.WebClient();
                byte[] response = client.DownloadData(url);
                sContents = System.Text.Encoding.ASCII.GetString(response);
            }
            else
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(url);
                sContents = sr.ReadToEnd();
                sr.Close();
            }
        }
        catch
        {
            sContents = "unable to connect to server ";
        }
        return sContents;
    }

    [WebMethod]
    public DataSet FetchChatDetails(string szSearchid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
     OdbcDataAdapter   m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            //strTemp = "SELECT [id]"+
            //           ",isnull(chatque_name,'') as VisitorName "+
            //            ",isnull(chatque_desc,'')  as VisitorComment"+
            //            ",isnull(owner_name,'') as AgentName"+
            //            ",convert(varchar(20),isnull(chatque_start_time,''),100) as stime" +
            //            ",dbo.GetChatID(chatque_session_id) as chatid"+
            //            ",isnull(chatque_page_title,'') as Tittle "+
            //            ",isnull(chatque_page_url,'') as URL"+
            //            " FROM crm_chat_queue where chatque_parent_id  IN (SELECT chatque_parent_id FROM crm_chat_queue TEMP WHERE TEMP.chatque_session_id='" + szSearchid + "')";


            //strTemp += " ORDER BY chatque_start_time desc";

            m_DataAdapterOdbc = new OdbcDataAdapter("exec Chat_history_list '" + szSearchid + "'", m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "TemplateDetail";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchTemplateDetails", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchTemplateDetails", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    [WebMethod]
    public DataSet GetWebEnquiry()
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        //    string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        //   szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");
            string strtemp = "select * from chat_visitor_enquiry where  enquiry_date between convert(datetime, convert(varchar, getdate() - 30, 1) + ' 0:0:0') "+
                             " and convert(datetime, convert(varchar, getdate(), 1) +' 23:59:00') order by enquiry_date desc";
            
            OdbcCommand ODBCCommand = new OdbcCommand(strtemp, m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(ODBCCommand);
            ODBCCommand.CommandType = CommandType.Text;
            
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";


            nResultCode = 0;
            strResult = "Pass";

        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetWebEnquiry", strParameters);
        }
        catch (Exception ex)
        {
            LogMessage(strTemp + strResult, "GetWebEnquiry", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

}

public class Location
{
    public Location()
    {
        IPAddress = "";
        CountryName = "";
        CountryCode = "";
        CityName = "";
        RegionName = "";
        ZipCode = "";
        Latitude = "";
        Longitude = "";
        TimeZone = "";
    }
    public string IPAddress { get; set; }
    public string CountryName { get; set; }
    public string CountryCode { get; set; }
    public string CityName { get; set; }
    public string RegionName { get; set; }
    public string ZipCode { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string TimeZone { get; set; }
}


//http://www.ipinfodb.com/register.php
//Your API key is as below:
//133b4b51cea45c6ac8165d19083a239d888e68ebd01d26c4958b6ba1f71661bc
//http://www.aspsnippets.com/Articles/Find-Visitors-Geographic-Location-using-IP-Address-in-ASPNet.aspx
