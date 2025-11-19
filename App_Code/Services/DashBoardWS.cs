using System;
using System.Web.Services;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using System.Web;
using System.Web.Script.Services;
using System.Configuration;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[ScriptService]
public class DashBoardWS : System.Web.Services.WebService
{
    DataBase m_Connection = new DataBase();
    string strTemp = "";
    string strParameters = "";
    string strIPAddress = HttpContext.Current.Request.UserHostAddress;
    OdbcCommand m_CommandODBC;

    public DashBoardWS()
    {
    }

    #region GetUserDashboard
    [WebMethod]
    public DataSet GetUserDashboard(int nUserID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " dashlet_dock_state as dockstate FROM crm_user_dashlets " +
                      "WHERE dashlet_user_id=" + nUserID + " " + m_Connection.DB_TOP_MYSQL;

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "DockState";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetUserDashboard", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetUserDashboard", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region SaveUserDashboard
    [WebMethod]
    public DataSet SaveUserDashboard(string dockstate, string dashletes, int nUserID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " id FROM crm_user_dashlets " +
                      "WHERE dashlet_user_id=" + nUserID + " " + m_Connection.DB_TOP_MYSQL;

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            int nID = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
            if (nID == 0)
            {
                strTemp = "INSERT INTO crm_user_dashlets(dashlet_user_id, dashlet_dock_state, dashlet_id) VALUES" +
                          "(" + nUserID + ",?,?)";
            }
            else
            {
                strTemp = "UPDATE crm_user_dashlets SET " +
                        "dashlet_dock_state=?,dashlet_id=? " +
                        "WHERE id=" + nID;
            }
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.Parameters.Add("@strDockState", OdbcType.VarChar).Value = dockstate;
            m_CommandODBC.Parameters.Add("@strDockID", OdbcType.VarChar).Value = dashletes;
            nID = m_CommandODBC.ExecuteNonQuery();
            if (nID > 0)
            {
                nResultCode = 0;
                strResult = "Pass";
            }
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "SaveUserDashboard", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "SaveUserDashboard", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region FetchDashletDetails
    [WebMethod]
    public DataSet FetchDashletDetails(string DashletID, string UserID, string TimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " dashlet_type,dashlet_SP_name FROM crm_dashlet_master WHERE dashlet_id=" + DashletID + " " + m_Connection.DB_TOP_MYSQL;

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Dashlet";

            if (m_DataSet.Tables[0].Rows.Count > 0)
            {
                string strProc = m_DataSet.Tables[0].Rows[0]["dashlet_SP_name"].ToString();
                strProc = strProc.Trim();
                if (strProc.Length > 0)
                {
                    strTemp = m_Connection.DB_EXECUTE_PROCEDURE + strProc;
                    strTemp = strTemp.Replace("#USERID#", UserID);
                    strTemp = strTemp.Replace("#TIMESPAN#", TimeZoneSpan);
                    strTemp = strTemp.Replace("#IP#", szBrowsedIP);
                    m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                    m_DataAdapterOdbc.Fill(m_DataSet);
                    m_DataSet.Tables[1].TableName = "Data";
                }
                nResultCode = 0;
                strResult = "Pass";
            }
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchDashletDetails", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchDashletDetails", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region FetchOpenCases
    [WebMethod]
    public DataSet FetchOpenCases(string UserID, string TimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT id," +
                "'<a href=\"javascript:window.parent.parent.OpenItem(''ShowObject.aspx?ObjectType=CAS&ID=' + convert(varchar,id) + ''', ''Cases [' + transaction_number + ']'')\">' + transaction_number + '</a>' as title," +
                "(case when " + m_Connection.DB_NULL + "(case_customer_name,'') = '' then 'Cust. Not Available' else case_customer_name end) as account," +
                "convert(varchar(12)," + m_Connection.DB_FUNCTION + "getLocalZoneTime(open_time," + TimeZoneSpan + "),13) as open_date, case_status_name as status," +
                "case_category_name as Category, case_subcategory_name as SubCategory " +
                "FROM crm_cases WHERE useable='Y' and case_status_id <> 2 AND  assign_to_id = "+ UserID + " or  owner_id = " + UserID +
                " ORDER BY open_time";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";

            // if (m_DataSet.Tables[0].Rows.Count > 0)
            //{
            nResultCode = 0;
            strResult = "Pass";
            //}
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchOpenCases", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchOpenCases", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion
    #region FetchOpenCases
    [WebMethod]
    public DataSet EscalationCases(string UserID, string TimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp ="SELECT id,"+
            " '<a href=\"javascript:window.parent.parent.OpenItem(''ShowObject.aspx?ObjectType=CAS&ID=' + convert(varchar,id) + ''', ''Ticket [' + transaction_number + ']'')\">' + transaction_number + '</a>' as title,"+
            " ISNULL(case_caller_name,'') as  Customer,convert(varchar(12),dbo.getLocalZoneTime(open_time,"+TimeZoneSpan+"),13) as OpenDate,"+
            " convert(varchar(12),dbo.getLocalZoneTime(target_end_time," + TimeZoneSpan + "),13) as TargetDate, case_category_name as Category ,"+
            "SlaLevel  as SlaLevel,isnull(case_type_name,'')  as Type, dbo.getcontact(owner_id) as OwnerName  FROM crm_cases " +
            "WHERE useable='Y' and   SlaLevel <>0   "+
            " and case_status_id =3 ORDER BY SlaLevel desc";
              OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";

            // if (m_DataSet.Tables[0].Rows.Count > 0)
            //{
            nResultCode = 0;
            strResult = "Pass";
            //}
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchOpenCases", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchOpenCases", strParameters);
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
    #region StatisticsCases
    public DataSet StatisticsCases(string UserID, string TimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");
            if(UserID=="1")
            {
                strTemp = "SELECT id,cust_location as CustLoc,cust_filling_type as FillingType,cust_fule_type as FuelType,cust_fule_qty as FuelQty," +
                          " '<a href=\"javascript:window.parent.parent.OpenItem(''ShowObject.aspx?ObjectType=CAS&ID=' + convert(varchar,id) + ''', ''Ticket [' + transaction_number + ']'')\">' + transaction_number + '</a>' as title," +
                          " '<a href=\"javascript:window.parent.parent.OpenItem(''ShowObject.aspx?ObjectType=CNT&ID=' + convert(varchar,case_caller_id) + ''', ''Customer [' + case_caller_name + ']'')\">' + isnull(case_caller_name,'') + '</a>' Customer," +
                          "  convert(varchar(12),dbo.getLocalZoneTime(created_date," + TimeZoneSpan + "),13) as OpenDate,dbo.getContactName(assign_to_id) as AssignTo," +
                          " DATEDIFF(DAY, created_date,GETUTCDATE()) as Ageing FROM crm_cases " +
                          "   WHERE useable='Y' and case_status_id in(1,3) and created_date between convert(datetime, convert(varchar, getdate()-7, 1) + ' 0:0:0') "+ 
                            " and convert(datetime, convert(varchar, getdate(), 1) +' 23:59:00') ORDER BY created_date desc";

            }

            else
            {
                strTemp = "SELECT id,cust_location as CustLoc,cust_filling_type as FillingType,cust_fule_type as FuelType,cust_fule_qty as FuelQty," +
          " '<a href=\"javascript:window.parent.parent.OpenItem(''ShowObject.aspx?ObjectType=CAS&ID=' + convert(varchar,id) + ''', ''Ticket [' + transaction_number + ']'')\">' + transaction_number + '</a>' as title," +
          " '<a href=\"javascript:window.parent.parent.OpenItem(''ShowObject.aspx?ObjectType=CNT&ID=' + convert(varchar,case_caller_id) + ''', ''Customer [' + case_caller_name + ']'')\">' + isnull(case_caller_name,'') + '</a>' Customer," +
          "  convert(varchar(12),dbo.getLocalZoneTime(created_date," + TimeZoneSpan + "),13) as OpenDate,dbo.getContactName(assign_to_id) as AssignTo," +
          " DATEDIFF(DAY, created_date,GETUTCDATE()) as Ageing FROM crm_cases " +
          "   WHERE useable='Y' and case_status_id in(1,3) and  assign_to_id = " + UserID + " or owner_id = " + UserID +
          "  and and created_date between convert(datetime, convert(varchar, getdate()-7, 1) + ' 0:0:0') " +
                            " and convert(datetime, convert(varchar, getdate(), 1) +' 23:59:00') ORDER BY created_date desc";

            }
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";

            // if (m_DataSet.Tables[0].Rows.Count > 0)
            //{
            nResultCode = 0;
            strResult = "Pass";
            //}
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchOpenCases", strParameters);
        }
        catch (Exception ex)
        {
            LogMessage(strTemp + strResult, "FetchOpenCases", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion
    #region FetchOpenTasks
    [WebMethod]
    public DataSet FetchOpenTasks(string UserID, string TimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT id," +
                "'<a href=\"javascript:javascript:window.parent.parent.OpenItem(''ShowObject.aspx?ObjectType=TSK&ID=' + convert(varchar,id) + ''', ''Tasks [' + transaction_number + ']'')\">' + transaction_number + '</a>' as title," +
                "(case when " + m_Connection.DB_NULL + "(udf_tech_hospital,'') = '' then 'Cust. Not Available' else udf_tech_hospital end) as account," +
                "convert(varchar(12)," + m_Connection.DB_FUNCTION + "getLocalZoneTime(open_time," + TimeZoneSpan + "),13) as open_date, task_status_desc as status," +
                m_Connection.DB_FUNCTION + "GetTaskCategoryName(task_type_id) as Category," +
                m_Connection.DB_FUNCTION + "GetCaseSubCategoryName(id) as CaseSubCategory " +
                "FROM crm_tasks WHERE useable='Y' and task_status_id <> 5 AND owner_id = " + UserID +
                " ORDER BY open_time";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";

            // if (m_DataSet.Tables[0].Rows.Count > 0)
            //{
            nResultCode = 0;
            strResult = "Pass";
            //}
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchOpenTasks", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchOpenTasks", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region FetchReassignedCases
    [WebMethod]
    public DataSet FetchReassignedCases(string UserID, string TimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT recent_item_object_id as id," +
                "'<a href=\"javascript:OpenItem(''ShowObject.aspx?ObjectType=CAS&ID=' + convert(varchar,recent_item_object_id) + ''', ''Cases [' + dbo.GetRelatedName('CAS', recent_item_object_id) + ']'')\">' + dbo.GetRelatedName('CAS', recent_item_object_id) + '</a>' as title," +
                "(case when " + m_Connection.DB_NULL + "(dbo.GetCaseAccountName(recent_item_object_id),'') = '' then 'Cust. Not Available' else dbo.GetCaseAccountName(recent_item_object_id) end) as account," +
                "convert(varchar(12)," + m_Connection.DB_FUNCTION + "getLocalZoneTime(recent_item_start_time," + TimeZoneSpan + "),13) as open_date," +
                "convert(varchar(12)," + m_Connection.DB_FUNCTION + "getLocalZoneTime(recent_item_start_time," + TimeZoneSpan + "),13) as Reassignment_date," +
                "dbo.GetContactName(recent_item_contact_id) as Reassignment_to," +
                "dbo.GetCaseFields(recent_item_contact_id,'SubCategory') as SubCategory " +
                "FROM crm_recent_items WHERE recent_item_object_type='CAS' and recent_item_action = 'REASSIGNMENT' " +
                "AND recent_item_contact_id = " + UserID + " " +
                "ORDER BY recent_item_start_time";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";

            // if (m_DataSet.Tables[0].Rows.Count > 0)
            // {
            nResultCode = 0;
            strResult = "Pass";
            // }
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchReassignedCases", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchReassignedCases", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region FetchDashlets
    [WebMethod]
    public DataSet FetchDashlets(int nUserID, int nRoleID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " + m_Connection.DB_NULL + "(dashlet_id,'') from CRM_User_Dashlets WHERE dashlet_user_id=" + nUserID + " " + m_Connection.DB_TOP_MYSQL;
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            string strDashlets = Convert.ToString(m_CommandODBC.ExecuteScalar());
            //manju
            strTemp = "SELECT a.dashlet_id,a.dashlet_name,";
         //   strTemp = "SELECT a.dashlet_id,case when CHARINDEX('#', a.dashlet_name) >0 then (SELECT LEFT(a.dashlet_name,LEN(a.dashlet_name) -1) + CAST(count(isnull(id,0)) as varchar(30))  FROM crm_cases "+
           //           "WHERE useable='Y' and case_status_id =3 and  owner_id ="+nUserID+")   else dashlet_name end as dashlet_name,";
            if (m_Connection.nDatabaseType == 2)
                strTemp += "concat(a.dashlet_type, '|', convert(varchar(10),a.dashlet_id)) as dashlet_value ";
            else
                strTemp += "a.dashlet_type + '|' + convert(varchar(10),a.dashlet_id) as dashlet_value ";
            strTemp += "FROM CRM_Dashlet_Master a, CRM_Role_Activity b " +
                "WHERE a.dashlet_enabled='Y' ";
            if (strDashlets != "")
                strTemp += "AND a.dashlet_id not in (" + strDashlets + ") ";

            strTemp += "AND a.dashlet_activity_id = b.roledet_activity_id " +
                "AND b.roledet_role_id=" + nRoleID + " " +
                "ORDER BY dashlet_name";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Dashlets";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchDashlets", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchDashlets", strParameters);
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
            szMessage = "DashBoard.cs - " + szMethodName +
                        "(" + szMethodParams + ") " + szMessage;

            objCreateLog.ErrorLog(szMessage);
        }
        catch (Exception ex)
        {
            string str = ex.Message;
        }
    }
    #endregion

    #region TeamOpenCases
    [WebMethod]
    public DataSet TeamOpenCases(string UserID, string TimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {

            m_Connection.OpenDB("Galaxy");
            OdbcCommand cmd = new OdbcCommand("DB_TeamOpenCase", m_Connection.oCon);
            cmd.CommandType = CommandType.StoredProcedure;

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(cmd);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";

            // if (m_DataSet.Tables[0].Rows.Count > 0)
            //{
            nResultCode = 0;
            strResult = "Pass";
            //}
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strResult, "TeamOpenCases", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strResult, "TeamOpenCases", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion



    #region TeamOpenTask
    [WebMethod]
    public DataSet TeamOpenTask(string UserID, string TimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {

            m_Connection.OpenDB("Galaxy");
            OdbcCommand cmd = new OdbcCommand("DB_TeamOpenTaskCase", m_Connection.oCon);
            cmd.CommandType = CommandType.StoredProcedure;

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(cmd);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";

            // if (m_DataSet.Tables[0].Rows.Count > 0)
            //{
            nResultCode = 0;
            strResult = "Pass";
            //}
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strResult, "TeamOpenTask", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strResult, "TeamOpenTask", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region ApproalforReporting
    [WebMethod]
    public DataSet GetPendingOD(string UserID, string TimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT created_by AS id," +
  "'<a href=\"javascript:window.parent.parent.OpenItem(''General/OTM.aspx?ObjectType=OD&id=' + convert(varchar,created_by) + ''', ''Account [' + customer_name + ']'')\">' + customer_name  + '</a>' as Name," +
       "customer_name,dbo.GetContact(reporttoapproved_id) as ApproveBy,convert(varchar(12)," + m_Connection.DB_FUNCTION + "getLocalZoneTime(Time_Entry_Date," + TimeZoneSpan + "),13) as Time_Entry_Date," +
  " Remarks as Remarks,Approval_Status as Status,Approval_Status_By_HR ,ISNULL( CONVERT(varchar(15),Approval_Date,106),'N/A') AS ApprovalDate,ISNULL( CONVERT(varchar(15),Approval_Date_By_HR,106),'N/A') AS Approval_Date_By_HR, ApprovalRemark,Reject_Remark_by_hr From  crm_timesheet WHERE  SheetType = 'OD'  AND  created_by = " + UserID +
          " AND Time_Entry_Date>=(GETDATE()-15) ORDER BY Time_Entry_Date DESC";

        //    strTemp = "SELECT created_by AS id," +
        //"'<a href=\"javascript:window.parent.parent.OpenItem(''General/OTM.aspx?ObjectType=OD&id=' + convert(varchar,created_by) + ''', ''Account [' + customer_name + ']'')\">' + customer_name  + '</a>' as Name," +
        //     "customer_name,dbo.GetContact(reporttoapproved_id) as ApproveBy,convert(varchar(12)," + m_Connection.DB_FUNCTION + "getLocalZoneTime(Time_Entry_Date," + TimeZoneSpan + "),13) as Time_Entry_Date," +
        //" Remarks as Remarks,Approval_Status as Status,Approval_Status_By_HR ,ISNULL( CONVERT(varchar(15),Approval_Date,106),'N/A') AS ApprovalDate,ISNULL( CONVERT(varchar(15),Approval_Date_By_HR,106),'N/A') AS Approval_Date_By_HR, ApprovalRemark,Reject_Remark_by_hr From  crm_timesheet WHERE  SheetType = 'OD'  AND  created_by = " + UserID +
        //        " AND MONTH(Time_Entry_Date) = MONTH(GETDATE()) AND YEAR(Time_Entry_Date)=YEAR(GETDATE()) ORDER BY Time_Entry_Date DESC";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "GetPendingOD";
            nResultCode = 0;
            strResult = "Pass";
            
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetPendingOD", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetPendingOD", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Approalfor TeamPending OD Form
    [WebMethod]
    public DataSet ApproalforTeamPending(string UserID, string TimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "Select dbo.GetContactName(created_by) + ' (' + CONVERT(VARCHAR, COUNT(*)) + ')' as id," +
              "'<a href=\"javascript:window.parent.parent.OpenItem(''General/ApprovedTimeSheet.aspx?ObjectType=OD&id=' + convert(varchar,created_by) + ''', ''View Team OD [' + dbo.GetContact(created_by)  + ']'')\">' + dbo.GetContact(created_by)+ ' (' + CONVERT(VARCHAR, COUNT(*)) + ')'    + '</a>' as Name" +
                     " FROM crm_timesheet  where created_by IN (select id from crm_contacts where cust_reportingtocolumn = " + UserID + ") AND   Approval_Status = 'PENDING' AND SheetType = 'OD' GROUP BY created_by";
            //AND MONTH(Time_Entry_Date) = MONTH(GETDATE()) AND YEAR(Time_Entry_Date)=YEAR(GETDATE())

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "ApproalforTeamPending";

             nResultCode = 0;
            strResult = "Pass";
           
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ApproalforTeamPending", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ApproalforTeamPending", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region ApproalforHR
    [WebMethod]
    public DataSet ApproalforHR(string UserID, string TimeZoneSpan)
    {
        
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));
        //string uid = "88,86,2,26";

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT dbo.GetContactName(created_by) + ' (' + CONVERT(VARCHAR, COUNT(*)) + ')' as id," +
              "'<a href=\"javascript:window.parent.parent.OpenItem(''General/HRApprovalTimeSheet.aspx?ObjectType=OD&id=' + convert(varchar,created_by) + ''', ''View HR OD [' + dbo.GetContact(created_by)  + ']'')\">' + dbo.GetContact(created_by)+ ' (' + CONVERT(VARCHAR, COUNT(*)) + ')'    + '</a>' as Name" +
           " FROM  crm_timesheet where created_by in(select id from crm_contacts where hrapproved_id IN ('88,86,2,26'))  " +
           "AND  Approval_Status = 'APPROVED' AND Approval_Status_By_HR ='PENDING' AND SheetType = 'OD' GROUP BY created_by" +
           " ";
            //AND MONTH(Time_Entry_Date) = MONTH(GETDATE()) AND YEAR(Time_Entry_Date)=YEAR(GETDATE())

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "ApproalforHR";

            // if (m_DataSet.Tables[0].Rows.Count > 0)
            //{
            nResultCode = 0;
            strResult = "Pass";
            //}
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchApproalforHR", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchApproalforHR", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get monthly Approval Alert 
    [WebMethod]
    public DataSet GetApprovalAlert(int EmployeeID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("EXEC TS_trn_approval_Alert ?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@nEmployeeID", OdbcType.Int).Value = EmployeeID;
            //m_CommandODBC.Parameters.Add("@dtFromDate", OdbcType.DateTime).Value = startDate;
           // m_CommandODBC.Parameters.Add("@dtTODate", OdbcType.DateTime).Value = EndDate;
          
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
    #endregion
    #region Get monthly Approval HR Alert
    [WebMethod]
    public DataSet GetApprovalHRAlert()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("EXEC TS_trn_approval_hr_Alert", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
           // m_CommandODBC.Parameters.Add("@dtFromDate", OdbcType.DateTime).Value = startDate;
           // m_CommandODBC.Parameters.Add("@dtTODate", OdbcType.DateTime).Value = EndDate;

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
    #endregion

    #region TeamOpenLead
    [WebMethod]
    public DataSet TeamOpenLead(int UserID, int TimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {

            m_Connection.OpenDB("Galaxy");
            OdbcCommand cmd = new OdbcCommand("DB_TeamOpenLead", m_Connection.oCon);
            cmd.CommandType = CommandType.StoredProcedure;

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(cmd);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";

            // if (m_DataSet.Tables[0].Rows.Count > 0)
            //{
            nResultCode = 0;
            strResult = "Pass";
            //}
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strResult, "TeamOpenLead", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strResult, "TeamOpenLead", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion
    #region FetchOpenLeads
    [WebMethod]
    public DataSet FetchOpenLeads(string UserID, string TimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT id,'<a href=\"javascript:window.parent.parent.OpenItem(''ShowObject.aspx?ObjectType=LED&ID=' + convert(varchar,id) + ''', ''Leads [' + transaction_number + ']'')\">' +transaction_number+ '</a>' as title," +
                    " (case when " + m_Connection.DB_NULL + "(lead_customer_name,'') = '' then 'Cust. Not Available' else lead_customer_name end) as account," +
                    " (case when " + m_Connection.DB_NULL + "(lead_contact_name,'') = '' then 'Cust. Not Available' else lead_contact_name end) as Contact," +
                    " convert(varchar(12)," + m_Connection.DB_FUNCTION + "GetLocalZoneTime(open_time," + TimeZoneSpan + "),13) as open_date, lead_status_name as status," +
                    " lead_status_reason_desc reason"+
                    " FROM crm_leads WHERE useable='Y' and lead_status_name <> 'CLOSE' AND  assign_to_id =   "+UserID+
                    " ORDER BY open_time";
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";

            // if (m_DataSet.Tables[0].Rows.Count > 0)
            //{
            nResultCode = 0;
            strResult = "Pass";
            //}
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchOpenCases", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchOpenCases", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion


    public DataSet GetChatQue(int UserID, int nUserTimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT id," +
              
                            "case_subject as subject," +
                            " case_source_value as mobile," +
                            m_Connection.DB_NULL + "(convert(varchar,dbo.GetLocalZoneTime(created_date," + nUserTimeZoneSpan + "),13),'') as created_date," +
                            "convert(varchar, modified_date,13) as modified_date," +
                            "left(case_description, 100) as description, " +
                            " case_product_name as porduct, "+
                            " case_caller_name as Name, " +
                            "'CAS' as Type, " +
                         
                            "b.queue_id as queue_id," +
                            
                            "'Ticket [' + a.transaction_number + ']' as tab_text " +
                            "FROM crm_cases a, crm_queue_members b " +
                            "WHERE a.owner_id = b.queue_id and  queue_id=3   " +
                            "AND a.case_status_id <> 2 " +
                            "AND b.queue_member_contact_id=" + UserID;
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";

            // if (m_DataSet.Tables[0].Rows.Count > 0)
            //{
            nResultCode = 0;
            strResult = "Pass";
           
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "EmailQueue", strParameters);
        }
        catch (Exception ex)
        {
            LogMessage(strTemp + strResult, "EmailQueue", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }


    public DataSet GetEmailQue(int UserID, int nUserTimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT id," +
                // "dbo.Mailno(a.id) as mailNo,dbo.Mail_id(a.id) as mailId, "+
                            "case_subject as subject," +
                            " case_source_value as title," +
                            m_Connection.DB_NULL + "(convert(varchar,dbo.GetLocalZoneTime(created_date," + nUserTimeZoneSpan + "),13),'') as created_date," +
                            "convert(varchar, modified_date,13) as modified_date," +
                            "left(case_description, 100) as description, " +
                            "'CAS' as Type, " +
                            "b.queue_id as queue_id," +
                            "'Ticket [' + a.transaction_number + ']' as tab_text " +
                            "FROM crm_cases a, crm_queue_members b " +
                            "WHERE a.owner_id = b.queue_id and  queue_id=4  " +
                            "AND a.case_status_id <> 2 " +
                            "AND b.queue_member_contact_id=" + UserID;
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";

            // if (m_DataSet.Tables[0].Rows.Count > 0)
            //{
            nResultCode = 0;
            strResult = "Pass";
            //}
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "EmailQueue", strParameters);
        }
        catch (Exception ex)
        {
            LogMessage(strTemp + strResult, "EmailQueue", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    // 
    #region open fuel request 
    public DataSet OpenFuelRequest( string UserID, string TimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");
                strTemp = "SELECT id,transaction_number,req_status as Status," +
                          " '<a href=\"javascript:window.parent.parent.OpenItem(''ShowObject.aspx?ObjectType=RQF&ID=' + convert(varchar,id) + ''', ''FuelRequestNo [' + transaction_number + ']'')\">' + transaction_number + '</a>' as title," +
                          "  convert(varchar(12),dbo.getLocalZoneTime(created_date," + TimeZoneSpan + "),13) as CreateDate,dbo.getContactName(inward_driver_id) as DriverId," +
                          "  inward_fuelQty,inward_petropump_name FROM crm_requisitions " +
                          "   WHERE useable='Y' and req_status in('PENDING') and created_date between convert(datetime, convert(varchar, getdate()-30, 1) + ' 0:0:0') " +
                            " and convert(datetime, convert(varchar, getdate(), 1) +' 23:59:00') ORDER BY created_date desc";
            
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";

            // if (m_DataSet.Tables[0].Rows.Count > 0)
            //{
            nResultCode = 0;
            strResult = "Pass";
            //}
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "OpenFuelRequest", strParameters);
        }
        catch (Exception ex)
        {
            LogMessage(strTemp + strResult, "OpenFuelRequest", strParameters);
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