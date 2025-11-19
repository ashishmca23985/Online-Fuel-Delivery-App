using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using System.Data.Odbc;
using System.Web.Services.Protocols;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Services;
using System.Configuration;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
public class FrameworkWS : System.Web.Services.WebService
{
    DataBase m_Connection = new DataBase();
    OdbcDataAdapter m_DataAdapterOdbc;
    OdbcCommand m_CommandODBC;
    DataSet m_DataSet;
    public bool View = false;
    public bool Add = false;
    public bool Edit = false;
    public bool Delete = false;
    public bool Print = false;
    public bool Export = false;
    public bool Admin = false;
    public bool BulkUpdate = false;
    int P_ADD = 2;
    int P_EDIT = 4;
    int P_DELETE = 8;
    int P_PRINT = 16;
    int P_EXPORT = 32;
    int P_ADMIN = 64;
    int P_BULKUPDATE = 128;

    long nResultCode = -1;
    long nIdentity = 0;
    string strResult = "Fail - ";
    string szBrowsedIP = string.Empty;
    string strTemp = string.Empty;

    public FrameworkWS()
    {
    }

    #region Response Table
    private DataTable GetResponseTable(DataTable dtTable, long nResultCode, string strResult, long nIdentity, string param1, string param2)
    {
        int nLastCol = dtTable.Columns.Count;
        int nRowCount = dtTable.Rows.Count;
        DataColumn dc;
        dc = new DataColumn();
        dc.DataType = System.Type.GetType("System.String");
        dc.ColumnName = "Identity";
        dtTable.Columns.Add(dc);

        dc = new DataColumn();
        dc.DataType = System.Type.GetType("System.String");
        dc.ColumnName = "param1";
        dtTable.Columns.Add(dc);

        dc = new DataColumn();
        dc.DataType = System.Type.GetType("System.String");
        dc.ColumnName = "param2";
        dtTable.Columns.Add(dc);

        dc = new DataColumn();
        dc.DataType = System.Type.GetType("System.String");
        dc.ColumnName = "ResultCode";
        dtTable.Columns.Add(dc);

        dc = new DataColumn();
        dc.DataType = System.Type.GetType("System.String");
        dc.ColumnName = "ResultString";
        dtTable.Columns.Add(dc);

        if (nRowCount > 0)
        {
            dtTable.Rows[0][nLastCol++] = nIdentity;
            dtTable.Rows[0][nLastCol++] = param1;
            dtTable.Rows[0][nLastCol++] = param2;
            dtTable.Rows[0][nLastCol++] = nResultCode.ToString();
            dtTable.Rows[0][nLastCol++] = strResult;
        }
        else
        {
            DataRow dr = dtTable.NewRow();
            dr[nLastCol++] = nIdentity;
            dr[nLastCol++] = param1;
            dr[nLastCol++] = param2;
            dr[nLastCol++] = nResultCode.ToString();
            dr[nLastCol++] = strResult;
            dtTable.Rows.Add(dr);
        }
        return dtTable;
    }
    #endregion

    #region Update Customer Status
    [WebMethod]
    public DataSet ChangeContactStatus(int contactid, string status)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "Update crm_user_sessions Set session_user_mode='" + status + "'  " +
                    " where session_active='Y' and session_contact_id =" + contactid;
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            nResultCode = m_CommandODBC.ExecuteNonQuery();
            strResult = "pass";
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

    #region Validate Login
    [WebMethod]
    public DataTable ValidateLogin(string strLoginId, string strPwd, string strHost)
        {
        DataTable dtTable = new DataTable("Login");

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " +
                        m_Connection.DB_NULL + "(contact_role_id,0) As contact_role ," +  //MYSQL                           
                        "id as contact_id," +
                        "contact_login_id," +
                        "contact_current_theme," +
                        "contact_full_name," +
                        m_Connection.DB_NULL + "(contact_show_menubar,'Y') as  contact_show_menubar," +
                        m_Connection.DB_NULL + "(contact_show_toolbar,'Y') as  contact_show_toolbar," +
                        m_Connection.DB_NULL + "(contact_show_alertbar,'Y')  as  contact_show_alertbar," +
                        m_Connection.DB_NULL + "(contact_show_popup,'N')  as  contact_show_popup," +
                        m_Connection.DB_NULL + "(related_to, '') as AccountType," +
                        m_Connection.DB_NULL + "(related_to_name, '') as AccountName," +
                        m_Connection.DB_NULL + "(related_to_id, 0) as AccountID," +
                        m_Connection.DB_NULL + "(contact_team_id, 0) as TeamID," +
                        m_Connection.DB_NULL + "(" + m_Connection.DB_FUNCTION + "GetTeamName(contact_team_id), '') as TeamName," +
                        m_Connection.DB_NULL + "(contact_dept_id, 0) as DeptID," +
                        m_Connection.DB_NULL + "(contact_desig_id, 0) as DesigID," +
                        m_Connection.DB_NULL + "(contact_timezone_id, 0) as TimeZoneID," +
                        m_Connection.DB_NULL + "(contact_timezone_timespan, 0) as TimeZoneTimeSpan," +
                        m_Connection.DB_NULL + "(contact_cti_url, '') as CtiUrl," +
                        m_Connection.DB_NULL + "(" + m_Connection.DB_FUNCTION + "GetDesigLevel(contact_desig_id),0) as DesigLevel " +
                        "FROM CRM_contacts " +
                        "WHERE contact_login_id= ? " +
                        "AND contact_password= ? " +
                        "AND " + m_Connection.DB_NULL + "(contact_role_id,0) > 0 " +
                        "AND contact_enabled='Y' and contact_type_id='E' " + m_Connection.DB_TOP_MYSQL;

      
            m_DataAdapterOdbc = new OdbcDataAdapter();
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.SelectCommand = m_CommandODBC;
            // Add parameters and set values.
            m_CommandODBC.Parameters.Add("@contact_login_id", OdbcType.VarChar).Value = strLoginId;
            m_CommandODBC.Parameters.Add("@contact_password", OdbcType.VarChar).Value = strPwd;

            dtTable = new DataTable();
            m_DataAdapterOdbc.Fill(dtTable);
            nResultCode = 0;
            strResult = "Pass";

            if (dtTable.Rows.Count == 0)
            {
                nResultCode = -1;
               strResult = "Invalid user or password";
                return GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", "");
            }
            else if (dtTable.Rows[0]["contact_role"].ToString() == "")
            {
                nResultCode = -1;
                strResult = "No role assigned to user";
                return GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", "");
            }

            //	Expire existing sessions for the same user
            strTemp = "UPDATE CRM_User_Sessions " +
                "SET session_end_time = " + m_Connection.DB_UTC_DATE + "," +
                "session_active = 'N' " +
                "WHERE session_contact_id = " + dtTable.Rows[0]["contact_id"].ToString() + " " +
                "AND session_active <> 'N'";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRows = m_CommandODBC.ExecuteNonQuery();
            m_CommandODBC.Dispose();

            //strTemp = "UPDATE CRM_User_Sessions SET session_end_time = ?, session_active = ?" +
            //          "WHERE session_contact_id = ? AND session_active <> 'N'";

            //OdbcCommand m_CommODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            ////m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            //m_CommODBC.Parameters.Add("@session_end_time", OdbcType.VarChar).Value = m_Connection.DB_UTC_DATE;
            //m_CommODBC.Parameters.Add("@session_active", OdbcType.Char).Value = "N";
            //m_CommODBC.Parameters.Add("@session_contact_id", OdbcType.Int).Value = dtTable.Rows[0]["contact_id"].ToString();
            //int nRows = Convert.ToInt32(m_CommODBC.ExecuteNonQuery());
            //m_CommODBC.Dispose();           
           
            //	Create new session 

            strTemp = "INSERT INTO CRM_User_Sessions (" +
             "session_start_time, " +
             "session_host, session_active,session_login_id," +
             "session_theme,session_role_id,session_contact_id,session_display_name,session_user_mode) " +
             "VALUES (" + m_Connection.DB_UTC_DATE + ",'" +
             strHost + "','Y','" + strLoginId + "'," +
             "'" + dtTable.Rows[0]["contact_current_theme"].ToString() + "','" + dtTable.Rows[0]["contact_role"].ToString() + "'," +
             dtTable.Rows[0]["contact_id"].ToString() + ",'" + dtTable.Rows[0]["contact_full_name"].ToString() + "'," + m_Connection.DB_FUNCTION + "GetChatStatus(" + dtTable.Rows[0]["contact_id"].ToString() + "))";

            //strTemp = "INSERT INTO CRM_User_Sessions(session_start_time, " +
            //             "session_host, " +
            //             "session_active, " +
            //             "session_login_id, " +
            //             "session_theme, " +
            //             "session_role_id, " +
            //             "session_contact_id, " +
            //             "session_display_name, " +
            //             "session_user_mode " +                         
            //             "VALUES " +
            //             "(?,?,?,?,?,?,?,?)";

            //m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
           
            //m_CommandODBC.Parameters.Add("@session_start_time", OdbcType.VarChar).Value = m_Connection.DB_UTC_DATE;
            //m_CommandODBC.Parameters.Add("@session_host", OdbcType.VarChar).Value = strHost;
            //m_CommandODBC.Parameters.Add("@session_active", OdbcType.Char).Value = "Y";
            //m_CommandODBC.Parameters.Add("@session_login_id", OdbcType.VarChar).Value = strLoginId;
            //m_CommandODBC.Parameters.Add("@session_theme", OdbcType.VarChar).Value = dtTable.Rows[0]["contact_current_theme"].ToString();
            //m_CommandODBC.Parameters.Add("@session_role_id", OdbcType.VarChar).Value = dtTable.Rows[0]["contact_role"].ToString();
            //m_CommandODBC.Parameters.Add("@session_contact_id", OdbcType.Int).Value = dtTable.Rows[0]["contact_id"].ToString();
            //m_CommandODBC.Parameters.Add("@session_display_name", OdbcType.VarChar).Value = dtTable.Rows[0]["contact_full_name"].ToString();
            //m_CommandODBC.Parameters.Add("@session_user_mode", OdbcType.VarChar).Value = m_Connection.DB_FUNCTION + "GetChatStatus(" + dtTable.Rows[0]["contact_id"].ToString()+"))";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            nRows = m_CommandODBC.ExecuteNonQuery();
            m_CommandODBC.Dispose();

            if (nRows != 1)
            {
                nResultCode = -1;
                strResult = "Fail - Unable to create sessions";
                return GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", "");
            }

            //  Get Identity Column
            nRows = 0;
            strTemp = "SELECT @@identity AS idty";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            nIdentity = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
            m_CommandODBC.Dispose();

            if (nIdentity <= 0)
            {
                nResultCode = -1;
                strResult = "Fail - Unable to get identity column";
                return GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", "");
            }
            nResultCode = 0;
            strResult = Convert.ToString(nIdentity);
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
        return GetResponseTable(dtTable, nResultCode, strResult, 0, "", "");
    }
    #endregion

    #region Get Header Information
    [WebMethod]
    /*** fetch home page header info ***/
    public DataSet GetHomePageInformation(string strRoleId)
    {
        DataSet m_DataSet = new DataSet();

        szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");
            DataTable dtTable1 = new DataTable("HeaderInfo");
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " header_welcome_message," +
                    "REPLACE(" + m_Connection.DB_FUNCTION + "GetImageInfo(header_image_id,0),'#IP#','" + szBrowsedIP + "') AS header_logo," +
                    "header_logout_text," +
                    "REPLACE(" + m_Connection.DB_FUNCTION + "GetImageInfo(header_customization_image_id, 0),'#IP#','" + szBrowsedIP + "') AS customization_image," +
                    m_Connection.DB_FUNCTION + "GetImageSize(header_image_id) AS header_logo_size," +
                    m_Connection.DB_FUNCTION + "GetImageSize(header_customization_image_id) AS customization_image_size," +
                    "header_image_id,header_customization_image_id " +
                    "FROM CRM_header_information " + m_Connection.DB_TOP_MYSQL;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(dtTable1);
            m_DataSet.Tables.Add(dtTable1);

            DataTable dtTable2 = new DataTable("ThemeInfo");
            strTemp = "SELECT theme_id,theme_name FROM CRM_themes";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(dtTable2);
            m_DataSet.Tables.Add(dtTable2);

            DataTable dtTable3 = new DataTable("ToolBarInfo");

            strTemp = "SELECT activity_id,activity_name," + m_Connection.DB_NULL + "(activity_description,'') as activity_description," +
                    "REPLACE(" + m_Connection.DB_FUNCTION + "GetImageInfo(activity_image_id,activity_id),'#IP#','" + szBrowsedIP + "') AS image_path," +
                    "REPLACE(activity_action_url,'#IP#','" + szBrowsedIP + "') as activity_action_url," +
                    "activity_action_container " +
                    "FROM CRM_activity_master,CRM_role_activity " +
                    "WHERE activity_id=roledet_activity_id " +
                    "AND roledet_role_id in (" + strRoleId + ") " +
                    "AND activity_toolbar='Y' " +
                    "ORDER BY activity_order_by";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(dtTable3);
            m_DataSet.Tables.Add(dtTable3);

            DataTable dtTable4 = new DataTable("MenuInfo");

            strTemp = "SELECT activity_id,activity_name," + m_Connection.DB_NULL + "(activity_description,'') as activity_description," +
                    "REPLACE(activity_action_url,'#IP#','" + szBrowsedIP + "') as activity_action_url," +
                    "activity_action_container " +
                    "FROM CRM_activity_master,CRM_role_activity " +
                    "WHERE activity_id=roledet_activity_id " +
                    "AND roledet_role_id in (" + strRoleId + ") " +
                    "AND activity_parent_id=0 AND activity_menu='Y' " +
                    "ORDER BY activity_order_by";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(dtTable4);
            m_DataSet.Tables.Add(dtTable4);

            DataTable dtTable5 = new DataTable("PanelInfo");

            strTemp = "SELECT activity_id,activity_name," + m_Connection.DB_NULL + "(activity_description,'') as activity_description," +
                    "REPLACE(" + m_Connection.DB_FUNCTION + "GetImageInfo(activity_image_id,activity_id),'#IP#','" + szBrowsedIP + "') AS image_path," +
                    "REPLACE(activity_action_url,'#IP#','" + szBrowsedIP + "') as activity_action_url," +
                    "activity_action_container " +
                    "FROM CRM_activity_master,CRM_role_activity " +
                    "WHERE activity_id=roledet_activity_id " +
                    "AND roledet_role_id in (" + strRoleId + ") " +
                    "AND activity_panel='Y' " +
                    "ORDER BY activity_order_by";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(dtTable5);
            m_DataSet.Tables.Add(dtTable5);

            nResultCode = 0;
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
        DataTable dtTable6 = new DataTable("Response");
        m_DataSet.Tables.Add(GetResponseTable(dtTable6, nResultCode, strResult, nIdentity, "", ""));
        return m_DataSet;
    }
    #endregion

    #region Get Sub Menu List
    [WebMethod]
    public DataTable GetSubMenuList(int nParentId, string szLoginRoleId)
    {
        nResultCode = -1;
        strResult = "Fail-";
        DataTable dtTable = new DataTable("SubMenuList");
        szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT activity_id, activity_name,activity_order_by,activity_parent_id," +
                      m_Connection.DB_NULL + "(activity_description,'') as activity_description," +
                      "activity_image_id,activity_menu," +
                      "REPLACE(activity_action_url,'#IP#','" + szBrowsedIP + "') as activity_action_url ," +
                      "activity_action_container " +
                      "FROM CRM_activity_master,CRM_role_activity " +
                      "WHERE activity_parent_id=" + nParentId + " " +
                      "AND activity_menu='Y'  " +
                      "AND activity_id=roledet_activity_id " +
                      "AND roledet_role_id IN (" + szLoginRoleId + ") " +
                      "ORDER BY activity_id ";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(dtTable);
            if (dtTable.Rows.Count > 0)
            {
                nResultCode = 0;
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
        return GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", "");

    }
    #endregion

    #region GetTabList
    [WebMethod]
    public DataSet GetTabList(string strObjectType, string strRoleId, string strObjectId)
    {
        nResultCode = -1;
        strResult = "Fail-";
        DataSet m_DataSet = new DataSet();
        DataTable dtTable = new DataTable("Response");
        szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT tab_activity_id," + m_Connection.DB_NULL + "(tab_name,activity_name) as tab_name," +
                    "REPLACE(" + m_Connection.DB_FUNCTION + "GetImageInfo(activity_image_id,tab_activity_id),'#IP#','" + szBrowsedIP + "') AS image_path," +
                    "case when " + m_Connection.DB_NULL+ "(tab_general,'N') = 'N' then " +
                    "REPLACE(activity_action_url,'#IP#','" + szBrowsedIP + "') else " +
                    "REPLACE(tab_action_url,'#IP#','" + szBrowsedIP + "') end " +
                    "as action_url," +
                    "tab_show_image,tab_display_sequence_no," +
                    m_Connection.DB_NULL+ "(tab_general,'N') as general," +
                    m_Connection.DB_FUNCTION + "GetCount(activity_id,tab_object_code," + strObjectId + ",tab_display_count) as tab_count " +
                    "FROM CRM_table_tabs,CRM_activity_master,CRM_role_activity " +
                    "WHERE " + m_Connection.DB_NULL + "(tab_enabled,'Y')='Y' " +
                    "AND tab_object_code='" + strObjectType + "' " +
                    "AND activity_id=tab_activity_id " +
                    "AND roledet_activity_id=activity_id " +
                    "AND roledet_role_id in (" + strRoleId + ") " +
                    "ORDER BY tab_display_sequence_no";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables["Table"].TableName = "Main";
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
        m_DataSet.Tables.Add(GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", ""));
        return m_DataSet;
    }
    #endregion

    #region Get permission level of Activity on the basis of UserRoleID
    [WebMethod]
    public DataSet GetObjectPermissionLevel(int nUserRoleID, string objectType)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        m_DataSet = new DataSet();
        DataTable dtTable = new DataTable("Response");
        szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        try
        {
            m_Connection.OpenDB("Galaxy");  
            if(nUserRoleID == 0)
                strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " 0 as Level," +
                          m_Connection.DB_NULL + "(object_permission_activity_id,0) as ActivityID,object_name,object_table_name, " +
                          "REPLACE(" + m_Connection.DB_FUNCTION + "GetImageInfo(object_image_id,0),'#IP#','" + szBrowsedIP + "') AS image_path " +
                          ",object_add_name, object_add_url " +
                          "FROM CRM_Tables " +
                          "WHERE object_code='" + objectType + "' " + m_Connection.DB_TOP_MYSQL;
            else
                strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " + m_Connection.DB_NULL + "(roledet_permission_level,0) as Level," +
                          m_Connection.DB_NULL + "(roledet_activity_id,0) as ActivityID,object_name,object_table_name, " +
                          "REPLACE(" + m_Connection.DB_FUNCTION + "GetImageInfo(object_image_id,roledet_activity_id),'#IP#','" + szBrowsedIP + "') AS image_path " +
                          ",object_add_name, object_add_url " +
                          "FROM CRM_Tables, CRM_Role_Activity " +
                          "WHERE roledet_role_id=" + nUserRoleID + " " +
                          "AND roledet_activity_id=object_permission_activity_id " +
                          "AND object_code='" + objectType + "' " + m_Connection.DB_TOP_MYSQL;

            OdbcDataAdapter m_SqlAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_SqlAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Permisson";

            if (m_DataSet.Tables["Permisson"].Rows.Count > 0)
            {
                View = true;
                int nLevel = Convert.ToInt32(m_DataSet.Tables["Permisson"].Rows[0]["Level"]);

                if ((nLevel & P_ADD) != 0)
                    Add = true;

                if ((nLevel & P_EDIT) != 0)
                    Edit = true;

                if ((nLevel & P_DELETE) != 0)
                    Delete = true;

                if ((nLevel & P_PRINT) != 0)
                    Print = true;

                if ((nLevel & P_EXPORT) != 0)
                    Export = true;

                if ((nLevel & P_ADMIN) != 0)
                    Admin = true;

                if ((nLevel & P_BULKUPDATE) != 0)
                    BulkUpdate = true;

            }
            else
            {
                View = false;
                Add = false;
                Admin = false;
                Export = false;
                Print = false;
                Delete = false;
                Edit = false;
                BulkUpdate = false;
            }
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
        m_DataSet.Tables.Add(GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", ""));
        return m_DataSet;
    }
    #endregion

    #region Set User Main Header Setting
    [WebMethod]
    public DataTable wmSetUserHeader(int nUserId, string szEnabled, int param)
    {
        DataTable dtTable = new DataTable("Response");
        nResultCode = -1;
        strResult = "Fail-";
        try
        {
            m_Connection.OpenDB("Galaxy");
            if (param == 1)
                strTemp = "UPDATE crm_contacts set contact_show_menubar= '" + szEnabled + "' " +
                            "WHERE id=" + nUserId + "";
            else if (param == 2)
                strTemp = "UPDATE crm_contacts set contact_show_toolbar= '" + szEnabled + "' " +
                            "WHERE id=" + nUserId + "";
            else if (param == 3)
                strTemp = "UPDATE crm_contacts set contact_show_alertbar = '" + szEnabled + "' " +
                            "WHERE id=" + nUserId + "";
            else if (param == 4)
                strTemp = "UPDATE crm_contacts set contact_show_popup = '" + szEnabled + "' " +
                            "WHERE id=" + nUserId + "";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.ExecuteNonQuery();
            m_CommandODBC.Dispose();

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
        return GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", "");
    }
    #endregion

    #region Set User Theme
    [WebMethod]
    public DataTable SetUserTheme(int nUserId, string strThemeName)
    {
        DataTable dtTable = new DataTable("Theme");

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "UPDATE crm_contacts SET  " +
                     "contact_current_theme ='" + strThemeName + "'  " +
                     "WHERE id=" + nUserId;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
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
        return GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", "");

    }
    #endregion

    #region Dispose Session
    [WebMethod]
    public DataSet DisposeSession(int nSessionID)
    {
        int nRows = 0;
        DataTable dtTable = new DataTable("Response");
        DataSet dsDataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            //	insert in session 
            strTemp = "UPDATE CRM_User_Sessions " +
                "SET session_end_time = " + m_Connection.DB_UTC_DATE + " ," +
                "session_active = 'N' " +
                "WHERE session_id = " + nSessionID + " " +
                "AND session_end_time IS NULL " +
                "AND session_active <> 'N'";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            nRows = m_CommandODBC.ExecuteNonQuery();
            m_CommandODBC.Dispose();

            if (nRows != 1)
            {
                nResultCode = -1;
                strResult = "Fail - Unable to update session end time";

                dsDataSet.Tables.Add(GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", ""));
                return dsDataSet;
            }
            nResultCode = 0;
            strResult = "Session Ended Successfully";
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
        dsDataSet.Tables.Add(GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", ""));
        return dsDataSet;
    }
    #endregion

    #region Get User Alerts
    [WebMethod]
    public DataTable GetUserAlerts(int nUserId)
    {
        DataTable dtTable = new DataTable("Response");
        nResultCode = -1;
        strResult = "Fail-";

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL10 + " subject,related_to,related_to_id, id,Open_time,Message " +
                          "FROM crm_alarms " +
                          "WHERE contact_id=" + nUserId + " " +
                          "AND Open_time <= " + m_Connection.DB_UTC_DATE + " " +
                          "AND status = 'P' " + m_Connection.DB_TOP_MYSQL10;
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(dtTable);
            if (dtTable.Rows.Count > 0)
            {
                nResultCode = 0;
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
        return GetResponseTable(dtTable, nResultCode, strResult, 0, "", "");
    }
    #endregion

    #region Update User Alert Status
    [WebMethod]
    public DataTable UpdateUserAlertStatus(string strAlertId)
    {
        DataTable dtTable = new DataTable("Response");
        nResultCode = -1;
        strResult = "Fail-";

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "UPDATE crm_alarms SET Status = 'D' " +
                          "WHERE id in(" + strAlertId + ") " +
                          "AND status = 'P' ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
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
        return GetResponseTable(dtTable, nResultCode, strResult, 0, "", "");
    }
    #endregion

    #region GetCustomTab
    [WebMethod]
    public DataSet GetCustomTab(string objectType, string objectId)
    {
        DataTable dtTable = new DataTable("Response");
        nResultCode = -1;
        strResult = "Fail-";
        m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            string strColumns = "'TabName','TabUrl'";
            m_DataSet = m_Connection.FetchTransactionData(objectType, "", strColumns, Convert.ToInt32(objectId), 0);
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
        m_DataSet.Tables.Add(GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", ""));
        return m_DataSet;
    }
    #endregion

    //a added by ashish to update ctiagent password 

    #region Update CTI_Agent Login Password
    [WebMethod]
    public DataTable UpdateCTILoginPassword(string strLoginId, string strPwd)
    {
        DataTable dtTable = new DataTable("CTIPassword");
        try
        {
            m_Connection.OpenDB("IDGDB");

            strTemp = "Select agent_login_id,agent_login_password from cti_agents " +
               "WHERE agent_login_id = '" + strLoginId + "'";

            m_DataAdapterOdbc = new OdbcDataAdapter();
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.SelectCommand = m_CommandODBC;
            dtTable = new DataTable();
            m_DataAdapterOdbc.Fill(dtTable);

            if (dtTable.Rows.Count > 0)
            {
                strTemp = "UPDATE cti_agents " +
                    "SET agent_login_password = '" + strPwd + "' " +
                    "WHERE agent_login_id = '" + strLoginId + "'";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                int nRows = m_CommandODBC.ExecuteNonQuery();
                m_CommandODBC.Dispose();
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
        return GetResponseTable(dtTable, nResultCode, strResult, 0, "", "");
    }
    #endregion

    #region to check Validate Login
    [WebMethod]
    public DataTable CheckValidateLogin(string strLoginId, string strPwd, string strHost)
    {
        DataTable dtTable = new DataTable("Login");

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " +
                      m_Connection.DB_NULL + "(contact_role_id,0) As contact_role ," +  //MYSQL                           
                      "id as contact_id," +
                      "agent_name AS CTI_AgentName," +
                      "agent_id as CTI_AgentId," +
                      "contact_login_id," +
                      "contact_current_theme," +
                      "contact_full_name," +
                      m_Connection.DB_NULL + "(contact_show_menubar,'Y') as  contact_show_menubar," +
                      m_Connection.DB_NULL + "(contact_show_toolbar,'Y') as  contact_show_toolbar," +
                      m_Connection.DB_NULL + "(contact_show_alertbar,'Y')  as  contact_show_alertbar," +
                      m_Connection.DB_NULL + "(contact_show_popup,'N')  as  contact_show_popup," +
                      m_Connection.DB_NULL + "(related_to, '') as AccountType," +
                      m_Connection.DB_NULL + "(related_to_name, '') as AccountName," +
                      m_Connection.DB_NULL + "(related_to_id, 0) as AccountID," +
                      m_Connection.DB_NULL + "(contact_team_id, 0) as TeamID," +
                      m_Connection.DB_NULL + "(" + m_Connection.DB_FUNCTION + "GetTeamName(contact_team_id), '') as TeamName," +
                      m_Connection.DB_NULL + "(contact_dept_id, 0) as DeptID," +
                      m_Connection.DB_NULL + "(contact_desig_id, 0) as DesigID," +
                      m_Connection.DB_NULL + "(contact_timezone_id, 0) as TimeZoneID," +
                      m_Connection.DB_NULL + "(contact_timezone_timespan, 0) as TimeZoneTimeSpan," +
                      m_Connection.DB_NULL + "(contact_cti_url, '') as CtiUrl," +
                      m_Connection.DB_NULL + "(" + m_Connection.DB_FUNCTION + "GetDesigLevel(contact_desig_id),0) as DesigLevel " +
                      "FROM CRM_contacts " +
                      "WHERE contact_login_id= ? " +
                      "AND contact_password= ? " +
                      "AND " + m_Connection.DB_NULL + "(contact_role_id,0) > 0 " +
                      "AND contact_enabled='Y' and contact_type_id='E' " + m_Connection.DB_TOP_MYSQL;


            m_DataAdapterOdbc = new OdbcDataAdapter();
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.SelectCommand = m_CommandODBC;
            // Add parameters and set values.
            m_CommandODBC.Parameters.Add("@contact_login_id", OdbcType.VarChar).Value = strLoginId;
            m_CommandODBC.Parameters.Add("@contact_password", OdbcType.VarChar).Value = strPwd;

            dtTable = new DataTable();
            m_DataAdapterOdbc.Fill(dtTable);
            nResultCode = 0;
            strResult = "Pass";

            if (dtTable.Rows.Count == 0)
            {
                nResultCode = -1;
                strResult = "Invalid user or password";
                return GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", "");
            }
            else if (dtTable.Rows[0]["contact_role"].ToString() == "")
            {
                nResultCode = -1;
                strResult = "No role assigned to user";
                return GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", "");
            }

            //	Expire existing sessions for the same user
            strTemp = "UPDATE CRM_User_Sessions " +
                "SET session_end_time = " + m_Connection.DB_UTC_DATE + "," +
                "session_active = 'N' " +
                "WHERE session_contact_id = " + dtTable.Rows[0]["contact_id"].ToString() + " " +
                "AND session_active <> 'N'";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRows = m_CommandODBC.ExecuteNonQuery();
            m_CommandODBC.Dispose();

            nRows = 0;
            strTemp = "SELECT @@identity AS idty";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            nIdentity = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
            m_CommandODBC.Dispose();

            if (nIdentity <= 0)
            {
                nResultCode = -1;
                strResult = "Fail - Unable to get identity column";
                return GetResponseTable(dtTable, nResultCode, strResult, nIdentity, "", "");
            }
            nResultCode = 0;
            strResult = Convert.ToString(nIdentity);
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
        return GetResponseTable(dtTable, nResultCode, strResult, 0, "", "");
    }
    #endregion
 }
