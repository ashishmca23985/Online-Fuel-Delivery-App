using System;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Collections;
using System.Web.UI;
using System.Collections.Generic;
using SharedVO;
using System.Xml;
using System.Security.Cryptography;


[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

[ScriptService]
public class GlobalWS : System.Web.Services.WebService
{
    DataBase m_Connection;
    OdbcDataAdapter m_DataAdapterOdbc;
    OdbcCommand m_CommandODBC;

    string strTemp = "";
    string strParameters = "";
    string strIPAddress = HttpContext.Current.Request.UserHostAddress;

    public string strNoOfLicense = "";
    public int intNoOfLicenseUsed = 0;

    public GlobalWS()
    {
        m_Connection = new DataBase();
    }

    #region Get Tax Schemes
    [WebMethod]
    public DataSet FetchTaxSchemes()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
        }

        try
        {
            strTemp = "SELECT scheme_id,scheme_name FROM crm_tax_schemes " +
                      "ORDER BY scheme_name ";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchTaxSchemes", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchTaxSchemes", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get From General
    [WebMethod]
    public DataSet FetchGeneralValues(string szType)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        strParameters = szType;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT name, code FROM crm_general " +
                      "WHERE type='" + szType + "' order by name";
            //"ORDER BY name";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "General";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchGeneralValues", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchGeneralValues", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get Time Zone List
    [WebMethod]
    public DataSet FetchTimeZone()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT timezone_id," +
                      m_Connection.DB_NULL + "(timezone_name, '') as timezone_name," +
                      m_Connection.DB_NULL + "(timezone_timespan_minutes, 0) as timezone_timespan_minutes,";
            if (m_Connection.nDatabaseType == 2) //-- MySql
                strTemp += m_Connection.DB_NULL + "(concat(cast(timezone_id as varchar(10)),',',cast(timezone_timespan_minutes as varchar(10))),'') as timezone_value ";
            else
                strTemp += m_Connection.DB_NULL + "(cast(timezone_id as varchar(10))+','+cast(timezone_timespan_minutes as varchar(10)),'') as timezone_value ";

            strTemp += "FROM crm_time_zone WHERE timezone_id >0";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "TimeZone";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchTimeZone", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchTimeZone", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get Views based on Related To and User ID
    [WebMethod]
    public DataSet GetViewsForUser(string strRelatedTo, int nUserID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        strParameters = strRelatedTo;
        try
        {
            //manju 22-1-13 SELECT view_id, view_name
            DataTable m_DataTable = new DataTable("UserViews");
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT view_id, view_name,view_default " +
                "FROM CRM_Views " +
                "WHERE related_to = '" + strRelatedTo + "' " +
                "AND (created_by=" + nUserID + " " +
                "OR view_private='N') " +
                "ORDER BY view_name";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "UserViews";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetViewsForUser", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetViewsForUser", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get View details with Filter Criterias based on View ID
    [WebMethod]
    public DataSet FetchViewDetails(int nViewID, string ObjectType)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {

            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(related_to,'') as view_related_to," +
                      m_Connection.DB_NULL + "(view_name,'') as view_name," +
                      m_Connection.DB_NULL + "(view_description,'') as view_description," +
                      m_Connection.DB_NULL + "(view_custom_filter,'N') as view_custom_filter," +
                      m_Connection.DB_NULL + "(view_pagesize,0) as view_pagesize," +
                      m_Connection.DB_NULL + "(view_group_expression,'') as view_group_column," +
                      m_Connection.DB_NULL + "(view_sort_expression,'') as view_sort_column," +
                      m_Connection.DB_NULL + "(view_private,'N') as view_private," +
                      m_Connection.DB_NULL + "(view_filter_condition,'') as view_filter_condition," +
                      m_Connection.DB_NULL + "(view_order_by_expression,'') as view_order_by_expression," +
                      m_Connection.DB_NULL + "(view_default,'N') as view_default," +
                      m_Connection.DB_NULL + "(created_by,0) as view_created_by," +
                      m_Connection.DB_NULL + "(created_date,'') as view_created_date, ";

            if (m_Connection.nDatabaseType == 2) //-- MySql
                strTemp += "DATE_FORMAT(created_date,' %b %d %Y %h:%i %p') as CreatedDate,DATE_FORMAT(modified_date,'%b %d %Y %h:%i %p') as ModifiedDate,";
            else
                strTemp += "Convert(varchar(20),created_date,100) as CreatedDate,Convert(varchar(20),modified_date,100) as ModifiedDate,";

            strTemp += "CASE WHEN created_by=0 then 'System' else " + m_Connection.DB_NULL + "(" + m_Connection.DB_FUNCTION + "GetContactName(created_by),'') end as CreatedBy," +
                    "CASE WHEN modified_by=0 then 'System' else " + m_Connection.DB_NULL + "(" + m_Connection.DB_FUNCTION + "GetContactName(modified_by),'') end as ModifiedBy," +
                    m_Connection.DB_NULL + "(created_ip,'') as view_group_column " +
                    "FROM CRM_Views " +
                    "WHERE view_id=" + nViewID;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "ViewDetail";

            //--get filters based on View ID
            strTemp = "SELECT " + m_Connection.DB_NULL + "(filter_column_name,'') as filter_column_name," +
                      m_Connection.DB_FUNCTION + "GetTableDefColumnHeader(filter_column_name) as ColumnHeader," +
                      m_Connection.DB_NULL + "(filter_operator,'') as filter_operator," +
                      m_Connection.DB_FUNCTION + "GetGeneralName('OPERATOR',filter_operator) as OperatorName," +
                      m_Connection.DB_NULL + "(filter_value,'') as filter_value," +
                      m_Connection.DB_NULL + "(filter_separator,'') as filter_separator " +
                      "FROM CRM_View_Filters " +
                      "WHERE filter_view_id=" + nViewID;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable m_DataTable2 = new DataTable("FilterDetail");
            m_DataAdapterOdbc.Fill(m_DataTable2);
            m_DataSet.Tables.Add(m_DataTable2);

            strTemp = "SELECT " + m_Connection.DB_NULL + "(a.tabledef_column_name, '') as column_expression," +
                    m_Connection.DB_NULL + "(b.view_column_name, a.tabledef_column_header) as column_name," +
                    m_Connection.DB_NULL + "(a.tabledef_column_description,'') as column_description," +
                    "a.tabledef_id as tabledef_id," +
                    m_Connection.DB_NULL + "(a.tabledef_column_type,'') as column_type," +
                    "(case when b.id is null then 'false' else 'true' end) as column_selected," +
                    m_Connection.DB_NULL + "(b.view_column_width, a.tabledef_column_width) as column_width " +
                    "FROM crm_table_columns a LEFT OUTER JOIN CRM_View_Columns b " +
                    "ON a.tabledef_id = b.view_column_tabledef_id " +
                    "AND b.view_column_view_id=" + nViewID + " " +
                    "WHERE a.tabledef_column_visible = 'Y' " +
                    "AND a.tabledef_for='" + ObjectType + "' " +
                    "ORDER BY tabledef_column_sequence";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable m_DataTable3 = new DataTable("ViewColumns");
            m_DataAdapterOdbc.Fill(m_DataTable3);
            m_DataSet.Tables.Add(m_DataTable3);

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchViewDetails", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchViewDetails", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Create/Save View
    [WebMethod]
    public DataSet SaveView(long nSessionID, int nViewID, string strViewDesc, string strViewName,
                            string strRelatedTo, int nPageSize,
                            string strPrivate,
                            string strFilterCondition, string[] arrTableDefIDs,
                            string[] arrFilterColumn, string[] arrFilterOperator,
                            string[] arrFilterValue, string[] arrFilterSeparator,
                            string strCustomFilter, string strOrderByCriteria, long nUserID, string[] arrSequence)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        string strColumnID = string.Empty;

        try
        {
            m_Connection.OpenDB("Galaxy");
            m_Connection.BeginTransaction();
            //--get columns[visible='N'] and merge into Columnslist [strColumns]
            if (nViewID == 0)
            {
                strTemp = "INSERT INTO CRM_Views(view_name, " +
                            "view_description, " +
                            "related_to, " +
                            "view_pagesize, " +
                            "view_private, " +
                            "view_filter_condition, " +
                            "created_by, " +
                            "created_date, " +
                            "created_ip, " +
                            "modified_by, " +
                            "modified_date, " +
                            "modified_ip, " +
                            "view_custom_filter, " +
                            "view_order_by_expression) " +
                            "VALUES " +
                            "(?, ?,'" + strRelatedTo + "'," + nPageSize + "," +
                            "?," +
                            "?," + nUserID + "," + m_Connection.DB_UTC_DATE + ",?," + nUserID + "," +
                            m_Connection.DB_UTC_DATE + ",?,'" + strCustomFilter + "',?)";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                m_CommandODBC.Parameters.Add("@ViewName", OdbcType.VarChar).Value = strViewName;
                m_CommandODBC.Parameters.Add("@ViewDesc", OdbcType.VarChar).Value = strViewDesc;
                m_CommandODBC.Parameters.Add("@Private", OdbcType.VarChar).Value = strPrivate;
                m_CommandODBC.Parameters.Add("@FilterCondition", OdbcType.VarChar).Value = strFilterCondition;
                m_CommandODBC.Parameters.Add("@IP", OdbcType.VarChar).Value = strIPAddress;
                m_CommandODBC.Parameters.Add("@IP", OdbcType.VarChar).Value = strIPAddress;
                m_CommandODBC.Parameters.Add("@OrderByCriteria", OdbcType.VarChar).Value = strOrderByCriteria;
            }
            else
            {
                strTemp = "UPDATE CRM_Views SET " +
                         "view_name = ?, " +
                         "view_description = ?, " +
                         "view_pagesize = " + nPageSize + ", " +
                         "view_private = ?, " +
                         "view_filter_condition =?, " +
                         "modified_ip = ?, " +
                         "view_order_by_expression = ?, " +
                         "view_custom_filter = '" + strCustomFilter + "', " +
                         "modified_by = " + nUserID + ", " +
                         "modified_date = " + m_Connection.DB_UTC_DATE + " " +
                         "WHERE view_id = " + nViewID;

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                m_CommandODBC.Parameters.Add("@ViewName", OdbcType.VarChar).Value = strViewName;
                m_CommandODBC.Parameters.Add("@ViewDesc", OdbcType.VarChar).Value = strViewDesc;
                m_CommandODBC.Parameters.Add("@Private", OdbcType.VarChar).Value = strPrivate;
                m_CommandODBC.Parameters.Add("@FilterCondition", OdbcType.VarChar).Value = strFilterCondition;
                m_CommandODBC.Parameters.Add("@IP", OdbcType.VarChar).Value = strIPAddress;
                m_CommandODBC.Parameters.Add("@OrderByCriteria", OdbcType.VarChar).Value = strOrderByCriteria;

            }

            int nCount = m_CommandODBC.ExecuteNonQuery();

            if (nCount != 1)
            {
                nResultCode = -1;
                strResult = "Fail - Unable to update view ";
                LogMessage(strTemp + strResult, "SaveView", strParameters);
                m_Connection.CloseDB();
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            if (nViewID == 0 && nCount == 1)
            {
                strTemp = "SELECT @@identity AS idty";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                nViewID = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
                m_CommandODBC.Dispose();
            }

            //--delete and save filter criterias
            strTemp = "DELETE FROM CRM_View_Filters " +
                      "WHERE filter_view_id = " + nViewID;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            int nRows = m_CommandODBC.ExecuteNonQuery();

            for (int i = 0; i < arrFilterColumn.Length; i++)
            {
                if (arrFilterColumn[i] == null || arrFilterColumn[i] == string.Empty)
                    continue;

                strTemp = "INSERT INTO CRM_View_Filters(filter_view_id," +
                          "filter_column_name,filter_operator,filter_value," +
                          "filter_separator) " +
                          "VALUES(" + nViewID + ", ?,?,?,?)";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                m_CommandODBC.Parameters.Add("@ColName", OdbcType.VarChar).Value = arrFilterColumn[i];
                m_CommandODBC.Parameters.Add("@Operator", OdbcType.VarChar).Value = arrFilterOperator[i];
                m_CommandODBC.Parameters.Add("@Value", OdbcType.VarChar).Value = arrFilterValue[i];
                m_CommandODBC.Parameters.Add("@Separator", OdbcType.VarChar).Value = arrFilterSeparator[i];
                m_CommandODBC.ExecuteNonQuery();
            }

            //--save customize columns

            strTemp = "DELETE FROM CRM_View_Columns " +
                         "WHERE view_column_view_id=" + nViewID + " ";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            nRows = m_CommandODBC.ExecuteNonQuery();

            string strInsertIDs = string.Empty;
            string strCompleteIDs = string.Empty;
            for (int i = 0; i < arrTableDefIDs.Length; i++)
            {
                if (arrTableDefIDs[i] != null && arrTableDefIDs[i] != string.Empty)
                {



                    strTemp = "INSERT INTO CRM_View_Columns(view_column_view_id, " +
                     "view_column_name, " +
                     "view_column_sequence," +
                     "view_column_width, " +
                     "view_column_tabledef_id) " +
                     "SELECT " + nViewID + "," +
                     "tabledef_column_header," + arrSequence[i] + "," +
                     "tabledef_column_width," +
                     "tabledef_id " +
                     "FROM crm_table_columns " +
                     "WHERE tabledef_id = " + arrTableDefIDs[i];

                    m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                    m_CommandODBC.ExecuteNonQuery();
                }

            }

            //if (strInsertIDs.Length > 0)
            //{

            //    strTemp = "INSERT INTO CRM_View_Columns(view_column_view_id, " +
            //              "view_column_name, " +
            //              "view_column_sequence," +
            //              "view_column_width, " +
            //              "view_column_tabledef_id) " +
            //              "SELECT " + nViewID + "," +
            //              "tabledef_column_header,tabledef_column_sequence," +
            //              "tabledef_column_width," +
            //              "tabledef_id " +
            //              "FROM crm_table_columns " +
            //              "WHERE tabledef_id in(" + strInsertIDs + ") ";

            //    m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            //    m_CommandODBC.ExecuteNonQuery();
            //}

            //if (strCompleteIDs.Length > 0)
            //{
            //    strTemp = "DELETE FROM CRM_View_Columns " +
            //              "WHERE view_column_view_id=" + nViewID + " " +
            //              "AND view_column_tabledef_id not in(" + strCompleteIDs + ")";

            //    m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            //    nRows = m_CommandODBC.ExecuteNonQuery();
            //}

            /*--------*/
            strTemp = "SELECT COUNT(*) " +
                    "FROM CRM_view_settings " +
                    "WHERE contact_id=" + nUserID + " " +
                    "AND table_name='" + strRelatedTo + "'";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());

            //--if no recent record found then insert
            if (nRowCount == 0)
            {
                strTemp = "INSERT INTO CRM_view_settings(contact_id," +
                          "filter_recent_activity,recent_activity_days," +
                          "filter_my_only,table_name,selected_view_id) " +
                          "VALUES " +
                          "(" + nUserID + ",'N',0,'N'," +
                          "'" + strRelatedTo + "'," + nViewID + ")";
            }
            // else if (nRowCount == 1)
            //manju

            else if (nRowCount > 0)
            {
                strTemp = "UPDATE CRM_view_settings " +
                          "Set selected_view_id=" + nViewID + " where contact_id=" + nUserID + " " +
                          "And table_name = '" + strRelatedTo + "'";
            }
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteNonQuery());

            if (nRowCount > 0)
            {
                m_Connection.Commit();
                nResultCode = 0;
                strResult = "pass";
            }
        }
        catch (OdbcException ex)
        {
            m_Connection.Rollback();
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "SaveView", strParameters);
        }
        catch (Exception ex)
        {
            m_Connection.Rollback();
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "SaveView", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get Columns from Table Definitions
    [WebMethod]
    public DataSet GetTableColumns(string strViewFor)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT tabledef_id," +
                      m_Connection.DB_NULL + "(tabledef_column_header,'') as tabledef_column_header," +
                      m_Connection.DB_NULL + "(tabledef_column_visible,'N') as tabledef_column_visible," +
                      m_Connection.DB_NULL + "(tabledef_column_name,'') as tabledef_column_name," +
                      m_Connection.DB_NULL + "(tabledef_column_description,'') as tabledef_column_description," +
                      m_Connection.DB_NULL + "(tabledef_operator,'') as tabledef_operator," +
                      m_Connection.DB_NULL + "(tabledef_column_type, 'T') as tabledef_column_type," +
                      m_Connection.DB_NULL + "(tabledef_column_width, 0) as tabledef_column_width," +
                      m_Connection.DB_NULL + "(tabledef_grouping_sort_column, '') as tabledef_grouping_sort_column," +
                      m_Connection.DB_FUNCTION + "getGeneralName('ColType'," + m_Connection.DB_NULL + "(tabledef_column_type,'T')) as ColType, ";

            if (m_Connection.nDatabaseType == 2) //-- MySql
            {
                strTemp += "concat(cast(tabledef_id as char),'|',tabledef_column_name,'|',tabledef_operator,'|''|''') as ColumnValue," +
                          "concat(ifnull(tabledef_column_name,''),'|', ifnull(tabledef_column_type,'T'),'|') as ColumnValue2 ";
            }
            else
            {
                strTemp += "convert(varchar,tabledef_id)+'|'+tabledef_column_name+'|'+tabledef_operator+'|''|' as ColumnValue," +
                    "(" + m_Connection.DB_NULL + "(tabledef_column_name,'')+'|'+" + m_Connection.DB_NULL + "(tabledef_column_type,'T')+'|') as ColumnValue2 ";
            }

            strTemp += "FROM crm_table_columns " +
                        "WHERE tabledef_for = '" + strViewFor + "' " +
                        "AND (tabledef_search_enabled = 'Y' OR (tabledef_search_enabled is null AND tabledef_column_visible = 'Y' )) " +
                        "ORDER BY tabledef_column_sequence ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataSet = new DataSet();
            m_DataAdapterOdbc.Fill(m_DataSet);
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "getViewMaster", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "getViewMaster", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get View Settings Details [User Based]
    [WebMethod]
    public DataSet GetViewSettingsForUser(int nUserID, string strRelatedTo, bool ApplyViewFilter)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        DataTable m_DataTable = new DataTable("RecentDetails");
        strParameters = +nUserID + "," + strRelatedTo;

        try
        {
            m_Connection.OpenDB("Galaxy");
            int nRecentActivityDays = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultActivityDays"]);
            if (ApplyViewFilter == false)
                strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " 'N' as recent_activity," +
                     nRecentActivityDays + "as recent_activity_days," +
                     "'Y' as my_account," +
                     " '" + strRelatedTo + "' as table_name," +
                      m_Connection.DB_FUNCTION + "GetDefaultView('" + strRelatedTo + "') as selected_view_id " +
                     "FROM crm_views " +
                     "WHERE view_default='Y' " +
                     "AND related_to='" + strRelatedTo + "' " + m_Connection.DB_TOP_MYSQL;

            else
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " + m_Connection.DB_NULL + "(filter_recent_activity, 'N') as recent_activity," +
                     m_Connection.DB_NULL + "(recent_activity_days, 0) as recent_activity_days," +
                     m_Connection.DB_NULL + "(filter_my_only, 'N') as my_account," +
                     m_Connection.DB_NULL + "(table_name, '') as table_name," +
                     m_Connection.DB_NULL + "(selected_view_id, " + m_Connection.DB_FUNCTION + "GetDefaultView('" + strRelatedTo + "')) as selected_view_id " +
                     "FROM CRM_view_settings " +
                     "WHERE contact_id=" + nUserID + " " +
                     "AND table_name='" + strRelatedTo + "' " + m_Connection.DB_TOP_MYSQL;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataTable);
            m_DataAdapterOdbc.Dispose();

            //--if no recent record found then insert
            if (m_DataTable.Rows.Count == 0)
            {
                strTemp = "INSERT INTO CRM_view_settings(contact_id," +
                          "filter_recent_activity,recent_activity_days," +
                          "filter_my_only,table_name,selected_view_id) " +
                          "VALUES " +
                          "(" + nUserID + ",'N'," + nRecentActivityDays + ",'Y'," +
                          "'" + strRelatedTo + "'," + m_Connection.DB_FUNCTION + "GetDefaultView('" + strRelatedTo + "'))";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.ExecuteNonQuery();
                m_CommandODBC.Dispose();

                strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " + m_Connection.DB_NULL + "(filter_recent_activity, 'N') as recent_activity," +
                         m_Connection.DB_NULL + "(recent_activity_days, 0) as recent_activity_days," +
                         m_Connection.DB_NULL + "(filter_my_only, 'N') as my_account," +
                         m_Connection.DB_NULL + "(table_name, '') as table_name," +
                         m_Connection.DB_NULL + "(selected_view_id, 0) as selected_view_id " +
                         "FROM CRM_view_settings " +
                         "WHERE contact_id=" + nUserID + " " +
                         "AND table_name='" + strRelatedTo + "' " + m_Connection.DB_TOP_MYSQL;

                m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                m_DataAdapterOdbc.Fill(m_DataTable);
                m_DataAdapterOdbc.Dispose();
            }

            m_DataSet.Tables.Add(m_DataTable);
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetViewSettingsForUser", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetViewSettingsForUser", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    [WebMethod]
    public DataSet GetViewSettingsForUser(int nUserID, string strRelatedTo)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        DataTable m_DataTable = new DataTable("RecentDetails");
        strParameters = +nUserID + "," + strRelatedTo;

        try
        {
            m_Connection.OpenDB("Galaxy");
            int nRecentActivityDays = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultActivityDays"]);
            //if (!ApplyOwnerFilter)
            //    strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " 'N' as recent_activity," +
            //         nRecentActivityDays + "as recent_activity_days," +
            //         "'Y' as my_account," +
            //         " '"+strRelatedTo + "' as table_name," +
            //          m_Connection.DB_FUNCTION + "GetDefaultView('" + strRelatedTo + "') as selected_view_id " +
            //         "FROM crm_views " +
            //         "WHERE view_default='Y' " + 
            //         "AND related_to='" + strRelatedTo + "' " + m_Connection.DB_TOP_MYSQL;

            //else
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " + m_Connection.DB_NULL + "(filter_recent_activity, 'N') as recent_activity," +
                     m_Connection.DB_NULL + "(recent_activity_days, 0) as recent_activity_days," +
                     m_Connection.DB_NULL + "(filter_my_only, 'N') as my_account," +
                     m_Connection.DB_NULL + "(table_name, '') as table_name," +
                     m_Connection.DB_NULL + "(selected_view_id, " + m_Connection.DB_FUNCTION + "GetDefaultView('" + strRelatedTo + "')) as selected_view_id " +
                     "FROM CRM_view_settings " +
                     "WHERE contact_id=" + nUserID + " " +
                     "AND table_name='" + strRelatedTo + "' " + m_Connection.DB_TOP_MYSQL;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataTable);
            m_DataAdapterOdbc.Dispose();

            //--if no recent record found then insert
            if (m_DataTable.Rows.Count == 0)
            {
                strTemp = "INSERT INTO CRM_view_settings(contact_id," +
                          "filter_recent_activity,recent_activity_days," +
                          "filter_my_only,table_name,selected_view_id) " +
                          "VALUES " +
                          "(" + nUserID + ",'N'," + nRecentActivityDays + ",'Y'," +
                          "'" + strRelatedTo + "'," + m_Connection.DB_FUNCTION + "GetDefaultView('" + strRelatedTo + "'))";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.ExecuteNonQuery();
                m_CommandODBC.Dispose();

                strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " + m_Connection.DB_NULL + "(filter_recent_activity, 'N') as recent_activity," +
                         m_Connection.DB_NULL + "(recent_activity_days, 0) as recent_activity_days," +
                         m_Connection.DB_NULL + "(filter_my_only, 'N') as my_account," +
                         m_Connection.DB_NULL + "(table_name, '') as table_name," +
                         m_Connection.DB_NULL + "(selected_view_id, 0) as selected_view_id " +
                         "FROM CRM_view_settings " +
                         "WHERE contact_id=" + nUserID + " " +
                         "AND table_name='" + strRelatedTo + "' " + m_Connection.DB_TOP_MYSQL;

                m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                m_DataAdapterOdbc.Fill(m_DataTable);
                m_DataAdapterOdbc.Dispose();
            }

            m_DataSet.Tables.Add(m_DataTable);
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetViewSettingsForUser", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetViewSettingsForUser", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Save User Custom Columns [width and sequence]
    [WebMethod]
    public DataSet SaveViewLayout(long nUserID, int nViewID,
                                     ArrayList arrColumns, string strViewRelatedTo, string strMyAccount, string strRecentActivity, int nRecentDays, string groupExpression, string sortExpression, string filterExpression)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            m_Connection.BeginTransaction();
            int i = 1;

            strTemp = "UPDATE crm_views SET view_group_expression = ?, " +
                        "view_sort_expression = ?, " +
                        "view_filter_expression = ? " +
                        "WHERE view_id = " + nViewID;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.Parameters.Add("@GroupExpression", OdbcType.VarChar).Value = groupExpression;
            m_CommandODBC.Parameters.Add("@SortExpression", OdbcType.VarChar).Value = sortExpression;
            m_CommandODBC.Parameters.Add("@FilterExpression", OdbcType.VarChar).Value = filterExpression;

            int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteNonQuery());

            if (nRowCount > 0)
            {
                foreach (Pair p in arrColumns)
                {
                    strTemp = "UPDATE CRM_View_Columns SET " +
                              "view_column_sequence=" + (i++) + "," +
                              "view_column_width='" + p.Second + "' " +
                              "WHERE view_column_view_id=" + nViewID + " " +
                              "AND view_column_name='" + p.First + "'";

                    m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                    m_CommandODBC.ExecuteNonQuery();
                }
                strTemp = "SELECT COUNT(*) " +
                        "FROM CRM_view_settings " +
                        "WHERE contact_id=" + nUserID + " " +
                        "AND table_name='" + strViewRelatedTo + "'";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());

                //--if no recent record found then insert
                if (nRowCount == 0)
                {
                    strTemp = "INSERT INTO CRM_view_settings(contact_id," +
                              "filter_recent_activity,recent_activity_days," +
                              "filter_my_only,table_name,selected_view_id) " +
                              "VALUES " +
                              "(" + nUserID + ",'" + strRecentActivity + "'," + nRecentDays + ",'" + strMyAccount + "'," +
                              "'" + strViewRelatedTo + "'," + m_Connection.DB_FUNCTION + "GetDefaultView('" + strViewRelatedTo + "'))";
                }
                else
                {
                    strTemp = "UPDATE CRM_view_settings " +
                              "Set filter_recent_activity ='" + strRecentActivity + "',recent_activity_days=" + nRecentDays + "," +
                              "filter_my_only='" + strMyAccount + "', selected_view_id=" + nViewID + " where contact_id=" + nUserID + " " +
                              "And table_name = '" + strViewRelatedTo + "'";
                }
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteNonQuery());

                if (nRowCount > 0)
                {
                    nResultCode = 0;
                    strResult = "pass";
                }
                m_Connection.Commit();
            }
        }

        catch (OdbcException ex)
        {
            m_Connection.Rollback();
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "SaveViewLayout", strParameters);
        }

        catch (Exception ex)
        {
            m_Connection.Rollback();
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "SaveViewLayout", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get Queue Data
    [WebMethod]
    public DataSet GetQueueData(int ContactID, int nUserTimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

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
                            "AND b.queue_member_contact_id=" + ContactID;
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
                            "AND b.queue_member_contact_id=" + ContactID;
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
                            "AND b.queue_member_contact_id=" + ContactID;

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
                            "AND b.queue_member_contact_id=" + ContactID;
            }
            strTemp += " ORDER BY created_date desc";
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "QueueData";
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

    #region Get Recent Items
    public DataSet GetRecentItems(long nUserID)
    {
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
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "RecentItems";

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

    #region  FetchAccountTeams
    [WebMethod]

    public DataSet FetchAccountTeams(int nCustomerID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(accteam_account_id,0) as accteam_account_id," +
                      m_Connection.DB_NULL + "(accteam_id,0) as accteam_id," +
                      m_Connection.DB_NULL + "(" + m_Connection.DB_FUNCTION + "GetTeamMemberCount(accteam_id," + nCustomerID + "),0) as accteam_member_count," +
                      m_Connection.DB_NULL + "(accteam_name,'') as accteam_name " +
                      "FROM CRM_Account_Teams " +
                      "WHERE accteam_account_id = " + nCustomerID + " " +
                      "ORDER BY accteam_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "Team";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Fetch_Account_Team", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Fetch_Account_Team", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  FetchAccountDesignations
    [WebMethod]
    public DataSet FetchAccountDesignations(int nCustomerID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(accdesig_account_id,0) as accdesig_account_id," +
                      m_Connection.DB_NULL + "(accdesig_id,0) as accdesig_id," +
                      m_Connection.DB_NULL + "(accdesig_level,0) as accdesig_level," +
                      m_Connection.DB_NULL + "(accdesig_name,'') as accdesig_name " +
                      "FROM CRM_Account_Designations " +
                      "WHERE accdesig_account_id = " + nCustomerID + " " +
                      "ORDER BY accdesig_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "Designations";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchAccountDesignations", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchAccountDesignations", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  FetchNonAccountDesignations
    [WebMethod]
    public DataSet FetchNonAccountDesignations(int nCustomerID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(desig_id,0) as desig_id," +
                      m_Connection.DB_NULL + "(desig_name,'') as desig_name " +
                      "FROM crm_designations WHERE desig_enabled = 'Y' " +
                      "AND desig_name NOT IN (SELECT accdesig_name FROM CRM_Account_Designations WHERE accdesig_account_id = " + nCustomerID + ") " +
                      "ORDER BY desig_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Designation";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchNonAccountDesignations", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchNonAccountDesignations", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  FetchAccountDepartments
    [WebMethod]
    public DataSet FetchAccountDepartments(int nCustomerID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(accdept_account_id,0) as accdept_account_id," +
                      m_Connection.DB_NULL + "(accdept_id,0) as accdept_id," +
                      m_Connection.DB_NULL + "(" + m_Connection.DB_FUNCTION + "GetDepartmentMemberCount(accdept_id," + nCustomerID + "),0) as accdept_member_count," +
                      m_Connection.DB_NULL + "(accdept_name,'') as accdept_name " +
                      "FROM CRM_Account_Departments " +
                      "WHERE accdept_account_id = " + nCustomerID + " " +
                      "ORDER BY accdept_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "Departments";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchAccountDepartments", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchAccountDepartments", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  FetchNonAccountDepartments
    [WebMethod]
    public DataSet FetchNonAccountDepartments(int nCustomerID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(dept_id,0) as dept_id," +
                      m_Connection.DB_NULL + "(dept_name,'') as dept_name " +
                      "FROM crm_departments WHERE dept_enabled = 'Y' " +
                      "AND dept_name NOT IN (SELECT accdept_name FROM CRM_Account_Departments WHERE accdept_account_id = " + nCustomerID + ") " +
                      "ORDER BY dept_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Team";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchNonAccountDepartmentss", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchNonAccountDepartmentss", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  FetchNonAccountTeams
    [WebMethod]
    public DataSet FetchNonAccountTeams(int nCustomerID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(team_id,0) as team_id," +
                      m_Connection.DB_NULL + "(team_name,'') as team_name " +
                      "FROM crm_teams WHERE team_enabled = 'Y' " +
                      "AND team_name NOT IN (SELECT accteam_name FROM CRM_Account_Teams WHERE accteam_account_id = " + nCustomerID + ") " +
                      "ORDER BY team_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Team";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchNonAccountTeams", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchNonAccountTeams", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Getting All Desigantion
    [WebMethod]
    public DataSet FetchDesignation(long nSessionID, string strEnabled)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        strParameters = nSessionID + "," + strEnabled;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(desig_id,0) as desig_id," + m_Connection.DB_NULL + "(desig_name,'') as desig_name" +
                      " FROM crm_designations WHERE desig_enabled = '" + strEnabled + "'";

            strTemp += " ORDER BY desig_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable dtTable = new DataTable("Designation");
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Department";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchDesignation", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchDesignation", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Getting All States on the basis of Country
    [WebMethod]
    public DataSet FetchState(string strCountryCode)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(state_code,'') as state_code," +
                      m_Connection.DB_NULL + "(state_name,'') as state_name," +
                      m_Connection.DB_NULL + "(state_country_code,'') as state_country_code " +
                      "FROM CRM_States ";

            if (strCountryCode != "")
                strTemp += "WHERE state_country_code='" + strCountryCode + "' ";

            strTemp += " ORDER BY state_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "State";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchState", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchState", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Getting All Cities on the basis of States
    [WebMethod]
    public DataSet FetchCity(string strStateCode)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(city_code,'') as city_code," +
                      m_Connection.DB_NULL + "(city_name,'') as city_name," +
                      m_Connection.DB_NULL + "(city_state_code,'') as city_state_code " +
                      "FROM crm_cities ";

            if (strStateCode != "")
                strTemp += "WHERE city_state_code='" + strStateCode + "' ";

            strTemp += " ORDER BY city_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "City";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCity", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCity", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region  Getting All Cities on the basis of States
    [WebMethod]
    public string ExecuteQuery(string strQuery)
    {
        string strResult = "";

        try
        {
            m_Connection.OpenDB("Galaxy");
            strQuery = strQuery.Replace("#", m_Connection.DB_FUNCTION);
            OdbcCommand m_CommandOdbc = new OdbcCommand(strQuery, m_Connection.oCon);
            strResult = Convert.ToString(m_CommandOdbc.ExecuteScalar());
        }
        catch (OdbcException ex)
        {
            LogMessage(strTemp + ex.Message, "FetchCity", strParameters);
        }
        catch (Exception ex)
        {
            LogMessage(strTemp + ex.Message, "FetchCity", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        return strResult;
    }
    #endregion

    #region  FetchContactPhones
    [WebMethod]
    public DataSet FetchContactDetails(string ContactType, string ObjectType, string ObjectId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            if (ContactType == "Phone")
                strTemp = "SELECT (a.caption_name + '-' + " + m_Connection.DB_NULL + "(b.value,'NA')) as caption, " + m_Connection.DB_NULL + "(b.value, 'NA' + convert(varchar,b.id)) as value " +
                "FROM crm_phone_captions a LEFT OUTER JOIN crm_contact_details b " +
                "ON a.caption_name = b.caption AND b.type = '" + ContactType + "' AND b.related_to = '" + ObjectType + "' AND b.related_to_id = " + ObjectId + " " +
                "WHERE a.caption_type = '" + ContactType + "' " +
                "ORDER BY a.caption_default desc, b.value DESC ";

            else
                strTemp = "SELECT a.caption_name as caption, b.value as value " +
                    "FROM crm_phone_captions a LEFT OUTER JOIN crm_contact_details b " +
                    "ON a.caption_name = b.caption AND b.type = '" + ContactType + "' AND b.related_to = '" + ObjectType + "' AND b.related_to_id = " + ObjectId + " " +
                    "WHERE a.caption_type = '" + ContactType + "' " +
                    "ORDER BY a.caption_code ";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "ContactDetails";
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

    #region  SaveContactDetails
    [WebMethod]
    public DataSet SaveContactDetails(string ContactType, string ObjectType, string ObjectId, string Caption, string Value)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "UPDATE crm_contact_details SET " +
                "value='" + Value + "' " +
                "WHERE type = '" + ContactType + "' " +
                "AND caption = '" + Caption + "' " +
                "AND related_to = '" + ObjectType + "' " +
                "AND related_to_id = " + ObjectId;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();
            if (nRowCount == 0)
            {
                strTemp = "INSERT INTO crm_contact_details (type,caption,related_to,related_to_id,value) " +
                    "VALUES('" + ContactType + "','" + Caption + "','" + ObjectType + "'," + ObjectId + ",'" + Value + "')";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                nRowCount = m_CommandODBC.ExecuteNonQuery();
            }
            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region  Getting All Status
    [WebMethod]
    public DataSet FetchCRMStatus(string strStatusFor)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        strParameters = "nSessionID";
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(status_name,'') as status_name," +
                      m_Connection.DB_NULL + "(status_id,0) as status_id " +
                      "FROM CRM_Status ";

            if (!string.IsNullOrEmpty(strStatusFor))
                strTemp += "WHERE status_for like '%" + strStatusFor + "%' and "+ m_Connection.DB_NULL + "(status_name,'')<>'NEW' ";

            strTemp += " ORDER BY status_sequence ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataSet = new DataSet();
            m_DataAdapterOdbc.Fill(m_DataSet);
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCRMStatus", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCRMStatus", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Get All Types
    [WebMethod]
    public DataSet FetchTaskTypes()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(type_name,'') as type_name," +
                        m_Connection.DB_NULL + "(type_id,0) as type_id " +
                        "FROM CRM_Types " +
                        "WHERE  type_enabled = 'Y' and type_for ='T' " +
                        "ORDER BY type_name ASC";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Types";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchType", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchType", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region  Get All Categories
    [WebMethod]
    public DataSet FetchTaskCategories(int nParentCategID, int nTaskTypeID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(categ_name,'') as categ_name," +
                     m_Connection.DB_NULL + "(categ_id,0) as categ_id," +
                     m_Connection.DB_NULL + "(categ_enabled,'') as categ_enabled," +
                     m_Connection.DB_NULL + "(categ_task_types,'') as categ_task_types," +
                     m_Connection.DB_NULL + "(categ_parent_id,0) as categ_parent_id " +
                     "FROM crm_task_categories " +
                     "WHERE categ_enabled = 'Y' ";
            if (nParentCategID > 0)
                strTemp += "AND categ_parent_id=" + nParentCategID + " ";
            else if (nTaskTypeID > 0)
                strTemp += "AND categ_task_types like '%," + nTaskTypeID + ",%' ";
            strTemp += "ORDER BY categ_name ASC";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Categories";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchTaskCategories", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchTaskCategories", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Get Service Types
    [WebMethod]
    public DataSet FetchServiceTypes(string CaseTypeID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(type_id,0) as type_id," +
                     m_Connection.DB_NULL + "(type_name,'') as type_name " +
                     "FROM CRM_Types " +
                     "WHERE  type_enabled = 'Y' " +
                     "AND type_for = 'S' " +
                     "AND type_case_types like '" + CaseTypeID + ",%' " +
                     "ORDER BY type_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Types";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseTypes", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseTypes", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region  Get All Types
    [WebMethod]
    public DataSet FetchCaseTypes()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(type_id,0) as type_id," +
                     m_Connection.DB_NULL + "(type_name,'') as type_name," +
                     m_Connection.DB_NULL + "(type_reference_field_caption,'') as type_reference_field_caption," +
                     m_Connection.DB_NULL + "(type_service_reference_caption,'') as type_service_reference_caption " +
                     "FROM CRM_Types " +
                     "WHERE  type_enabled = 'Y' " +
                     "AND type_for = 'C' " +
                     "ORDER BY type_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Types";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseTypes", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseTypes", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region  Get All Types
    [WebMethod]
    public DataSet FetchCaseTypes( string type)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(type_id,0) as type_id," +
                     m_Connection.DB_NULL + "(type_name,'') as type_name," +
                     m_Connection.DB_NULL + "(type_reference_field_caption,'') as type_reference_field_caption," +
                     m_Connection.DB_NULL + "(type_service_reference_caption,'') as type_service_reference_caption " +
                     "FROM CRM_Types " +
                     "WHERE  type_enabled = 'Y' " +
                     "AND type_for = 'C' " +
                     "And type_name like '%" +type+"%'"+
                     "ORDER BY type_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Types";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseTypes", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseTypes", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region  Get All Categories
    [WebMethod]
    public DataSet FetchCaseCategories(string ParentCategID, string CaseTypeID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(categ_name,'') as categ_name," +
                     m_Connection.DB_NULL + "(categ_id,0) as categ_id," +
                     m_Connection.DB_NULL + "(categ_enabled,'') as categ_enabled," +
                     m_Connection.DB_NULL + "(categ_case_types,'') as categ_case_types," +
                     m_Connection.DB_NULL + "(categ_parent_id,0) as categ_parent_id " +
                     "FROM CRM_Categories " +
                     "WHERE categ_enabled = 'Y' ";

            if (ParentCategID.Length > 0)
                strTemp += "AND categ_parent_id=" + ParentCategID + " ";
            else if (CaseTypeID.Length > 0)
                strTemp += "AND categ_case_types = '" + CaseTypeID + "' ";
            strTemp += "ORDER BY categ_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "Categories";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseCategories", "");
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseCategories", "");
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }

    [WebMethod]
    public DataSet FetchSubCategoriesForTemplate(string ParentCategID, string Level, int nTemplateID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
        strTemp = @"SELECT  SUBSTRING(
                    (
                    SELECT ',' + CAST(case when template_subcateg_ids=',,' then '' else template_subcateg_ids end AS VARCHAR) FROM crm_message_templates where template_for='ESC' AND template_categ_id=" + ParentCategID + 
                    " and template_Level="+Level;
        if (nTemplateID > 0)
            strTemp += " and id<>" + nTemplateID;
        strTemp += " FOR XML PATH('')), 2,10000) AS A";
        m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
        string categ_ids = Convert.ToString(m_CommandODBC.ExecuteScalar());
       categ_ids= categ_ids.Replace(",,,", ",");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(categ_name,'') as categ_name," +
                     m_Connection.DB_NULL + "(categ_id,0) as categ_id " +
                    
                     " FROM CRM_Categories " +
                     " WHERE categ_enabled = 'Y' ";

            if (ParentCategID.Length > 0)
                strTemp += "AND categ_parent_id=" + ParentCategID + " ";
            if (categ_ids.Length > 0)
                strTemp += "AND categ_id not in ("+categ_ids.Substring(1,categ_ids.Length-2)+ ")";
            strTemp += "ORDER BY categ_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "Categories";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseCategories", "");
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseCategories", "");
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }

    [WebMethod]
    public DataSet FetchCaseCategories1(string ParentCategID, string CaseTypeID,string context)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(categ_name,'') as categ_name," +
                     m_Connection.DB_NULL + "(categ_id,0) as categ_id," +
                     m_Connection.DB_NULL + "(categ_enabled,'') as categ_enabled," +
                     m_Connection.DB_NULL + "(categ_case_types,'') as categ_case_types," +
                     m_Connection.DB_NULL + "(categ_parent_id,0) as categ_parent_id " +
                     "FROM CRM_Categories " +
                     "WHERE categ_enabled = 'Y' "+
                     " AND categ_name like '%"+context+"%'";

            if (ParentCategID.Length > 0)
                strTemp += "AND categ_parent_id=" + ParentCategID + " ";
            else if (CaseTypeID.Length > 0)
                strTemp += "AND categ_case_types = '" + CaseTypeID + "' ";
            strTemp += "ORDER BY categ_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "Categories";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseCategories", "");
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseCategories", "");
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Save Notes
    [WebMethod]
    public DataSet Save_Notes(string strRelatedTo, string RelatedToID, string NoteContent, int nUserID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "INSERT INTO CRM_Notes(related_to_id,related_to," +
                      "note_desc,created_date,created_ip,created_by) " +
                      "VALUES " +
                      "(" + RelatedToID + ",?," +
                      "?," + m_Connection.DB_UTC_DATE + "," + "'" + strIPAddress + "'," + nUserID + ")";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.Parameters.Add("@strRelatedTo", OdbcType.VarChar).Value = strRelatedTo;
            m_CommandODBC.Parameters.Add("@strNoteDesc", OdbcType.VarChar).Value = NoteContent;

            m_CommandODBC.ExecuteNonQuery();
            nResultCode = 0;
            strResult = "pass";
        }

        catch (OdbcException ex)
        {
            m_Connection.Rollback();
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }

        catch (Exception ex)
        {
            m_Connection.Rollback();
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

    #region Get notes details on the basis of RelatedTo ID
    [WebMethod]
    public DataSet Get_Notes(string RelatedTo, string RelatedToID, int nUserTimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(id,0) as note_id," + m_Connection.DB_NULL + "(related_to,0) as note_related_to," +
                        m_Connection.DB_NULL + "(note_desc,'') as note_desc, ";
            if (m_Connection.nDatabaseType == 2) //-- MySql
                strTemp += m_Connection.DB_NULL + "(DATE_FORMAT(GetLocalZoneTime(created_date," + nUserTimeZoneSpan + "),'%d %b %Y %H:%i'),'') as note_created_date,";
            else
                strTemp += m_Connection.DB_NULL + "(convert(varchar(17),dbo.GetLocalZoneTime(created_date," + nUserTimeZoneSpan + "),113),'') as note_created_date,";

            strTemp += m_Connection.DB_NULL + "(created_ip,'') as note_created_ip," +
                m_Connection.DB_FUNCTION + "GetContactName(created_by) as note_created_emp_name " +
                "FROM CRM_Notes " +
                "WHERE related_to_id = " + RelatedToID + " " +
                "AND related_to='" + RelatedTo + "' " +
                "ORDER BY created_date DESC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Notes";

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

    #region GetHistory
    [WebMethod]
    public DataSet GetHistory(string RelatedTo, string RelatedToID, int nUserTimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT recent_item_history_text as Description,recent_item_action as Action," + m_Connection.DB_FUNCTION + "GetContactName(recent_item_contact_id) as [Action By],";
            if (m_Connection.nDatabaseType == 2) //-- MySql
                strTemp += //m_Connection.DB_NULL + "(DATE_FORMAT(GetLocalZoneTime(recent_item_start_time," + nUserTimeZoneSpan + "),'%d %b %Y %H:%i'),'') as StartTime," +
                            m_Connection.DB_NULL + "(DATE_FORMAT(GetLocalZoneTime(recent_item_end_time," + nUserTimeZoneSpan + "),'%d %b %Y %H:%i'),'') as [Action at] ";
            else
                strTemp += //m_Connection.DB_NULL + "(convert(varchar(20),dbo.GetLocalZoneTime(recent_item_start_time," + nUserTimeZoneSpan + "),113),'') as StartTime," +
                            m_Connection.DB_NULL + "(convert(varchar(20),dbo.GetLocalZoneTime(recent_item_end_time," + nUserTimeZoneSpan + "),113),'') as [Action at] ";

            strTemp += "FROM crm_recent_items " +
                "WHERE recent_item_object_id = " + RelatedToID + " " +
                "AND recent_item_object_type='" + RelatedTo + "' " +
                "AND recent_item_action <> 'View' " +
                "ORDER BY recent_item_start_time DESC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "History";

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


    #region GetHistory for json
    [WebMethod]
    [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    public DataSet GetHistoryjson(string RelatedTo, string RelatedToID, int nUserTimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT isnull(recent_item_history_text,'') as Description,isnull(recent_item_action,'') as Action," + m_Connection.DB_FUNCTION + "GetContactName(recent_item_contact_id) as ActionBy,";
            if (m_Connection.nDatabaseType == 2) //-- MySql
                strTemp += //m_Connection.DB_NULL + "(DATE_FORMAT(GetLocalZoneTime(recent_item_start_time," + nUserTimeZoneSpan + "),'%d %b %Y %H:%i'),'') as StartTime," +
                            m_Connection.DB_NULL + "(DATE_FORMAT(GetLocalZoneTime(recent_item_end_time," + nUserTimeZoneSpan + "),'%d %b %Y %H:%i'),'') as Actionat ";
            else
                strTemp += //m_Connection.DB_NULL + "(convert(varchar(20),dbo.GetLocalZoneTime(recent_item_start_time," + nUserTimeZoneSpan + "),113),'') as StartTime," +
                            m_Connection.DB_NULL + "(convert(varchar(20),dbo.GetLocalZoneTime(recent_item_end_time," + nUserTimeZoneSpan + "),113),'') as Actionat ";

            strTemp += "FROM crm_recent_items " +
                "WHERE recent_item_object_id = " + RelatedToID + " " +
                "AND recent_item_object_type='" + RelatedTo + "' " +
                "AND recent_item_action <> 'View' " +
                "ORDER BY recent_item_start_time DESC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "History";

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

    #region  FetchRoles
    [WebMethod]
    public DataSet FetchRoles()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(role_name,'') as role_name," +
                      m_Connection.DB_NULL + "(role_id,0) as role_id " +
                      "FROM CRM_roles WHERE role_enabled = 'Y' " +
                      "ORDER BY role_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Role";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchRoles", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchRoles", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region  Get All Country
    [WebMethod]
    public DataSet FetchCountry()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(country_name,'') as country_name," +
                      m_Connection.DB_NULL + "(country_code,'') as country_code " +
                      "FROM CRM_Countries WHERE country_code <> ''";

            strTemp += " ORDER BY country_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Country";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCountry", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCountry", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region  Create basic Filters of different objects [on the basis login account type]
    [WebMethod]
    public DataSet CreateFilter(int nLoginContactID, string strFilterFor, int nLoginAccountID,
                                string strLoginAccType, int nLoginDeptID, int nLoginDesigLevel)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable dtFilter = new DataTable("Filter");
        string szTemp = string.Empty;

        string strApplyHierarchy = string.Empty;
        string strContactIDs = string.Empty;
        bool bGetDeptMembers = false;
        bool bGetTeamMembers = false;

        try
        {
            m_Connection.OpenDB("Galaxy");

            strApplyHierarchy = Convert.ToString(ConfigurationManager.AppSettings["ApplyHierarchy"]);

            DataColumn dc = new DataColumn();
            dc.DataType = System.Type.GetType("System.String");
            dc.ColumnName = "FilterString";
            dtFilter.Columns.Add(dc);

            if (strApplyHierarchy == "B")
            {
                bGetDeptMembers = true;
                bGetTeamMembers = true;
            }
            else if (strApplyHierarchy == "D")
                bGetDeptMembers = true;
            else if (strApplyHierarchy == "T")
                bGetTeamMembers = true;

            //--get all contacts [who are coming under loggedinContact hierarachy level dept wise]
            if (bGetDeptMembers == true)
            {
                strTemp = "SELECT " +
                          m_Connection.DB_NULL + "(accdept_id, 0) as accdept_id," +
                          m_Connection.DB_NULL + "(contact_level, 0) as contact_level," +
                          m_Connection.DB_NULL + "(account_id, 0) as account_id  " +
                          "FROM crm_department_members " +
                          "WHERE contact_id=" + nLoginContactID;

                m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                DataTable dtTable = new DataTable();
                m_DataAdapterOdbc.Fill(dtTable);
                for (int i = 0; i < dtTable.Rows.Count; i++)
                {
                    strTemp = "SELECT " + m_Connection.DB_NULL + "(contact_id, 0) as contact_id " +
                              "FROM crm_department_members " +
                              "WHERE accdept_id=" + dtTable.Rows[i]["accdept_id"].ToString() + " " +
                              "AND contact_level<" + dtTable.Rows[i]["contact_level"].ToString() + "";

                    m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                    DataTable dtTable1 = new DataTable();
                    m_DataAdapterOdbc.Fill(dtTable1);
                    for (int j = 0; j < dtTable1.Rows.Count; j++)
                    {
                        if (strContactIDs == string.Empty)
                            strContactIDs = dtTable1.Rows[j]["contact_id"].ToString();
                        else
                            strContactIDs += "," + dtTable1.Rows[j]["contact_id"].ToString();
                    }
                }
            }
            //--get all contacts [who are coming under loggedinContact hierarachy level team wise]
            if (bGetTeamMembers == true)
            {
                strTemp = "SELECT " + m_Connection.DB_NULL + "(accteam_id, 0) as accteam_id," +
                             m_Connection.DB_NULL + "(contact_level, 0) as contact_level," +
                             m_Connection.DB_NULL + "(account_id, 0) account_id  " +
                             "FROM crm_team_members " +
                             "WHERE contact_id=" + nLoginContactID;

                m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                DataTable dtTable = new DataTable();
                m_DataAdapterOdbc.Fill(dtTable);
                for (int i = 0; i < dtTable.Rows.Count; i++)
                {
                    strTemp = "SELECT " + m_Connection.DB_NULL + "(contact_id, 0) as contact_id " +
                              "FROM crm_team_members " +
                              "WHERE accteam_id=" + dtTable.Rows[i]["accteam_id"].ToString() + " " +
                              "AND contact_level<" + dtTable.Rows[i]["contact_level"].ToString() + "";

                    m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                    DataTable dtTable1 = new DataTable();
                    m_DataAdapterOdbc.Fill(dtTable1);
                    for (int j = 0; j < dtTable1.Rows.Count; j++)
                    {
                        if (strContactIDs == string.Empty)
                            strContactIDs = dtTable1.Rows[j]["contact_id"].ToString();
                        else
                            strContactIDs += "," + dtTable1.Rows[j]["contact_id"].ToString();
                    }
                }
            }

            if (strContactIDs == string.Empty)
                strContactIDs = nLoginContactID.ToString();
            else
                strContactIDs += "," + nLoginContactID.ToString();

            strTemp = string.Empty;
            if (strFilterFor == "CST") //-- for account
            {
                strTemp = "AND (owner_id in (" + strContactIDs + ") OR created_by in (" + strContactIDs + ") " +
                    " OR id in (select map_account_id from crm_mappings where map_to_contact_id = " + nLoginContactID + ")) ";
            }
            else if (strFilterFor == "CAS") //--for case
            {
                strTemp = "AND (owner_id in (" + strContactIDs + ") OR created_by in (" + strContactIDs + ") OR assign_to_id in (" + strContactIDs + ") " +
                          " OR case_customer_id in (select map_account_id from crm_mappings where map_to_contact_id = " + nLoginContactID + ")) ";
            }
            
            else if (strFilterFor == "CNT") //--for contact
            {
                strTemp = "AND (owner_id in (" + strContactIDs + ") OR created_by in (" + strContactIDs + ") " +
                          //"OR contact_id in (select map_contact_id from crm_mappings where map_to_contact_id = " + nLoginContactID + ")+
                          ") ";
            }
            else if (strFilterFor == "INV") //--for product
            {
                strTemp = "AND (owner_id in (" + strContactIDs + ") OR created_by in (" + strContactIDs + ") " +
                          "OR id in (select map_inventory_id from crm_mappings where map_to_contact_id =" + nLoginContactID + ")) ";
            }
            else if (strFilterFor == "CHT") //--for product
            {
                //strTemp ="AND (chat_source_id=" + nSourceID + " or chat_destination_id=" + nSourceID + ")"
             //   strTemp = "AND (owner_id in (" + strContactIDs + ") OR created_by in (" + strContactIDs + "))";
            }
            else if (strFilterFor == "DOC") //--for product
            {
                strTemp = "AND (owner_id in (" + strContactIDs + ") OR created_by in (" + strContactIDs + ")or document_shareable='N') ";
            }
            else
            {
                strTemp = "AND (owner_id in (" + strContactIDs + ") OR created_by in (" + strContactIDs + ")) ";
            }

            DataRow dr = dtFilter.NewRow();
            dr[0] = strTemp;
            dtFilter.Rows.Add(dr);

            nResultCode = 0;
            strResult = "Pass";

            m_DataSet.Tables.Add(dtFilter);
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        }
        finally
        {
            m_Connection.CloseDB();
        }
        return m_DataSet;
    }
    #endregion
   
    #region  Create basic Filters of different objects [on the basis login account type and Team and its level]
    [WebMethod]
    public DataSet CreateFilterOwnerBy(int nLoginContactID, string strFilterFor, int nLoginAccountID,
                                string strLoginAccType, int nLoginDeptID, int nLoginDesigLevel)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable dtFilter = new DataTable("Filter");
        string szTemp = string.Empty;

        string strApplyHierarchy = string.Empty;
        string strContactIDs = string.Empty;
     //   bool bGetDeptMembers = false;
     //   bool bGetTeamMembers = false;

        try
        {
            m_Connection.OpenDB("Galaxy");

            //strApplyHierarchy = Convert.ToString(ConfigurationManager.AppSettings["ApplyHierarchy"]);

            DataColumn dc = new DataColumn();
            dc.DataType = System.Type.GetType("System.String");
            dc.ColumnName = "FilterString";
            dtFilter.Columns.Add(dc);

            //if (strApplyHierarchy == "B")
            //{
            //    bGetDeptMembers = true;
            //    bGetTeamMembers = true;
            //}
            //else if (strApplyHierarchy == "D")
            //    bGetDeptMembers = true;
            //else if (strApplyHierarchy == "T")
            //    bGetTeamMembers = true;

            //--get all contacts [who are coming under loggedinContact hierarachy level dept wise]
            //if (bGetDeptMembers == true)
            //{
            //    strTemp = "SELECT " +
            //              m_Connection.DB_NULL + "(accdept_id, 0) as accdept_id," +
            //              m_Connection.DB_NULL + "(contact_level, 0) as contact_level," +
            //              m_Connection.DB_NULL + "(account_id, 0) as account_id  " +
            //              "FROM crm_department_members " +
            //              "WHERE contact_id=" + nLoginContactID;

            //    m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            //    DataTable dtTable = new DataTable();
            //    m_DataAdapterOdbc.Fill(dtTable);
            //    for (int i = 0; i < dtTable.Rows.Count; i++)
            //    {
            //        strTemp = "SELECT " + m_Connection.DB_NULL + "(contact_id, 0) as contact_id " +
            //                  "FROM crm_department_members " +
            //                  "WHERE accdept_id=" + dtTable.Rows[i]["accdept_id"].ToString() + " " +
            //                  "AND contact_level<" + dtTable.Rows[i]["contact_level"].ToString() + "";

            //        m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            //        DataTable dtTable1 = new DataTable();
            //        m_DataAdapterOdbc.Fill(dtTable1);
            //        for (int j = 0; j < dtTable1.Rows.Count; j++)
            //        {
            //            if (strContactIDs == string.Empty)
            //                strContactIDs = dtTable1.Rows[j]["contact_id"].ToString();
            //            else
            //                strContactIDs += "," + dtTable1.Rows[j]["contact_id"].ToString();
            //        }
            //    }
            //}
            //--get all contacts [who are coming under loggedinContact hierarachy level team wise]
            //if (bGetTeamMembers == true)
            //{
                strTemp = "SELECT " + m_Connection.DB_NULL + "(accteam_id, 0) as accteam_id," +
                             m_Connection.DB_NULL + "(contact_level, 0) as contact_level," +
                             m_Connection.DB_NULL + "(account_id, 0) account_id  " +
                             "FROM crm_team_members " +
                             "WHERE contact_id=" + nLoginContactID;

                m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                DataTable dtTable = new DataTable();
                m_DataAdapterOdbc.Fill(dtTable);
                for (int i = 0; i < dtTable.Rows.Count; i++)
                {
                    strTemp = "SELECT " + m_Connection.DB_NULL + "(contact_id, 0) as contact_id " +
                              "FROM crm_team_members " +
                              "WHERE accteam_id=" + dtTable.Rows[i]["accteam_id"].ToString() + " " +
                              "AND contact_level<" + dtTable.Rows[i]["contact_level"].ToString() + "";

                    m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                    DataTable dtTable1 = new DataTable();
                    m_DataAdapterOdbc.Fill(dtTable1);
                    for (int j = 0; j < dtTable1.Rows.Count; j++)
                    {
                        if (strContactIDs == string.Empty)
                            strContactIDs = dtTable1.Rows[j]["contact_id"].ToString();
                        else
                            strContactIDs += "," + dtTable1.Rows[j]["contact_id"].ToString();
                    }
                }
            //}

            if (strContactIDs == string.Empty)
                strContactIDs = nLoginContactID.ToString();
            else
                strContactIDs += "," + nLoginContactID.ToString();

            strTemp = string.Empty;
            //if (strFilterFor == "CST") //-- for account
            //{
            //    strTemp = "AND (owner_id in (" + strContactIDs + ") OR created_by in (" + strContactIDs + ") " +
            //        " OR id in (select map_account_id from crm_mappings where map_to_contact_id = " + nLoginContactID + ")) ";
            //}
            //else
                if (strFilterFor == "CAS") //--for case
            {
                strTemp = "AND (owner_id in (" + strContactIDs + ") " +
                          " OR case_customer_id in (select map_account_id from crm_mappings where map_to_contact_id = " + nLoginContactID + ")) ";
            }
            //else if (strFilterFor == "CNT") //--for contact
            //{
            //    strTemp = "AND (owner_id in (" + strContactIDs + ") OR created_by in (" + strContactIDs + ") " +
            //              "OR contact_id in (select map_contact_id from crm_mappings where map_to_contact_id = " + nLoginContactID + ")) ";
            //}
            //else if (strFilterFor == "INV") //--for product
            //{
            //    strTemp = "AND (owner_id in (" + strContactIDs + ") OR created_by in (" + strContactIDs + ") " +
            //              "OR id in (select map_inventory_id from crm_mappings where map_to_contact_id =" + nLoginContactID + ")) ";
            //}
                else
                {
                    strTemp = "AND (owner_id in (" + strContactIDs + ") OR created_by in (" + strContactIDs + ")) ";
                }

            DataRow dr = dtFilter.NewRow();
            dr[0] = strTemp;
            dtFilter.Rows.Add(dr);

            nResultCode = 0;
            strResult = "Pass";

            m_DataSet.Tables.Add(dtFilter);
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        }
        finally
        {
            m_Connection.CloseDB();
        }
        return m_DataSet;
    }
    #endregion

    #region GetQueueMembers
    [WebMethod]
    public DataSet GetQueueMembers(int QueueID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT ";
            if (m_Connection.nDatabaseType == 2) //-- MySql
            {
                strTemp += m_Connection.DB_NULL + "(concat(" + m_Connection.DB_FUNCTION + "GetContactCustName(queue_member_contact_id),'-'," +
                      m_Connection.DB_FUNCTION + "GetContactName(queue_member_contact_id)),'') as member_name,";
            }
            else
            {
                strTemp += m_Connection.DB_NULL + "(" + m_Connection.DB_FUNCTION + "GetContactCustName(queue_member_contact_id)+'-'+" +
                      m_Connection.DB_FUNCTION + "GetContactName(queue_member_contact_id),'') as member_name,";
            }

            strTemp += m_Connection.DB_NULL + "(queue_member_contact_id,0) as contact_id," +
                       m_Connection.DB_NULL + "(queue_member_contact_level,0) as contact_level " +
                       "FROM CRM_queue_members " +
                       "WHERE  queue_id = " + QueueID +
                       " ORDER BY queue_member_contact_level ";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Members";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetQueueMembers", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetQueueMembers", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetDepartmentMembers
    [WebMethod]
    public DataSet GetDepartmentMembers(string DepartmentID, int nAccountID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT ";
            if (m_Connection.nDatabaseType == 2) //-- MySql
            {
                strTemp += m_Connection.DB_NULL + "(concat(" + m_Connection.DB_FUNCTION + "GetContactCustName(contact_id),'-'," +
                      m_Connection.DB_FUNCTION + "GetContactName(contact_id)),'') as member_name,";
            }
            else
            {
                strTemp += m_Connection.DB_NULL + "(" + m_Connection.DB_FUNCTION + "GetContactCustName(contact_id)+'-'+" +
                      m_Connection.DB_FUNCTION + "GetContactName(contact_id),'') as member_name,";
            }

            strTemp += m_Connection.DB_NULL + "(contact_id,0) as contact_id," +
                       m_Connection.DB_NULL + "(contact_level,0) as contact_level " +
                       "FROM CRM_department_members " +
                       "WHERE  accdept_id = " + DepartmentID + " AND contact_id > 0 AND account_id = " + nAccountID +
                       " ORDER BY contact_level ";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Members";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetDepartmentMembers", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetDepartmentMembers", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetTeamMembers
    [WebMethod]
    public DataSet GetTeamMembers(string TeamID, int nAccountID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT ";
            if (m_Connection.nDatabaseType == 2) //-- MySql
            {
                strTemp += m_Connection.DB_NULL + "(concat(" + m_Connection.DB_FUNCTION + "GetContactCustName(contact_id),'-'," +
                      m_Connection.DB_FUNCTION + "GetContactName(contact_id)),'') as member_name,";
            }
            else
            {
                strTemp += m_Connection.DB_NULL + "(" + m_Connection.DB_FUNCTION + "GetContactCustName(contact_id)+'-'+" +
                      m_Connection.DB_FUNCTION + "GetContactName(contact_id),'') as member_name,";
            }

            strTemp += m_Connection.DB_NULL + "(contact_id,0) as contact_id," +
                       m_Connection.DB_NULL + "(contact_level,0) as contact_level " +
                       "FROM CRM_team_members " +
                       "WHERE  accteam_id = " + TeamID + " AND contact_id > 0 AND account_id = " + nAccountID +
                       " ORDER BY contact_level ";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Members";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetTeamMembers", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetTeamMembers", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Getting All Status
    [WebMethod]
    public DataSet FetchStatusReason(string StatusID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(reason_id,0) as reason_id," +
                        m_Connection.DB_NULL + "(reason_description,'') as reason_description " +
                        "FROM CRM_reasons WHERE reason_status_id=" + StatusID + " " +
                        "ORDER BY reason_description ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "StatusReason";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchStatusReason", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchStatusReason", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region  Getting All Status
    [WebMethod]
    public DataSet FetchStatusCaseReason(string StatusID,string CaseId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        if (StatusID == "")
            StatusID = "0";
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(reason_id,0) as reason_id," +
                        m_Connection.DB_NULL + "(reason_description,'') as reason_description " +
                        "FROM CRM_reasons WHERE reason_status_id=" + StatusID + " ";
            if (CaseId != "")
                strTemp += " and reason_type_ids like '%," + CaseId + ",%'";
                 strTemp+="ORDER BY reason_description ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "StatusReason";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchStatusReason", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchStatusReason", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    #region  Getting All Status
    [WebMethod]
    public DataSet FetchStatusReason(string StatusID, string CaseId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        if (StatusID == "")
            StatusID = "0";
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(reason_id,0) as reason_id," +
                        m_Connection.DB_NULL + "(reason_description,'') as reason_description " +
                        "FROM CRM_reasons WHERE reason_status_id=" + StatusID + " ";
            if (CaseId != "")
                strTemp += " and reason_type_ids like '%," + CaseId + ",%'";
            strTemp += "ORDER BY reason_description ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "StatusReason";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchStatusReason", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchStatusReason", strParameters);
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
    public DataSet FetchStatusCaseReason(string StatusID, string CaseId,string context)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        if (StatusID == "")
            StatusID = "0";
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(reason_id,0) as reason_id," +
                        m_Connection.DB_NULL + "(reason_description,'') as reason_description " +
                        "FROM CRM_reasons WHERE reason_status_id=" + StatusID + " "+
                        " And reason_description like '%"+context+"%' ";
            if (CaseId != "")
                strTemp += " and reason_type_ids like '%," + CaseId + ",%'";
            strTemp += "ORDER BY reason_description ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "StatusReason";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchStatusReason", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchStatusReason", strParameters);
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

    #region Get Common Data List [Based On User's Custom Columns]
    [WebMethod]
    public DataSet FetchCommonData(string szSearchCloumnName,
                                  string szSearchText,
                                  int nLoginContactID,
                                  int nLoginAccountID, string strLoginAccType,
                                  bool bApplyOwnerFilter, int nLoginDeptID,
                                  int nLoginDesigLevel, int nViewID, bool bApplyViewFilter,
                                  int nUserTimeZoneSpan, string strCallType, string strNewCriteria,
                                  bool IsPhone,
                                  string strViewTable)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        string szColumns = string.Empty;
        string szFilter = string.Empty;
        string szSearchOperator = string.Empty;
        string szOrderBy = string.Empty;

        try
        {
            //--get basic filter if user is not ADMIN
            if (bApplyOwnerFilter == true)
            {
                DataSet dsDataSet = CreateFilter(nLoginContactID, strCallType, nLoginAccountID,
                                                            strLoginAccType, nLoginDeptID, nLoginDesigLevel);
                if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
                    return dsDataSet;

                if (dsDataSet.Tables["Filter"].Rows.Count > 0)
                    szFilter = dsDataSet.Tables["Filter"].Rows[0]["FilterString"].ToString();
            }
            m_Connection.OpenDB("Galaxy");

            //--extract filter condition and select columns from view
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " view_name," +
                    m_Connection.DB_NULL + "(view_pagesize,50) as view_pagesize," +
                    m_Connection.DB_NULL + "(view_group_expression,'') as view_group_expression," +
                    m_Connection.DB_NULL + "(view_sort_expression,'') as view_sort_expression," +
                    m_Connection.DB_NULL + "(view_filter_expression,'') as view_filter_expression," +
                    m_Connection.DB_NULL + "(view_private,'') as view_private," +
                    m_Connection.DB_NULL + "(view_filter_condition,'') as view_filter_condition," +
                    m_Connection.DB_NULL + "(view_order_by_expression,'') as view_order_by_expression " +
                    "FROM CRM_Views " +
                    "WHERE view_id=" + nViewID + " " + m_Connection.DB_TOP_MYSQL;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable dtView = new DataTable("ViewDetail");
            m_DataAdapterOdbc.Fill(dtView);

            if (dtView.Rows.Count > 0)
            {
                szOrderBy += " " + dtView.Rows[0]["view_order_by_expression"].ToString();
                if (bApplyViewFilter == true)
                    szFilter += " " + dtView.Rows[0]["view_filter_condition"].ToString();
            }
            else
            {
                nResultCode = -1;
                strResult = "Fail - No Columns/View found for FetchCommonData !";
                LogMessage(strTemp + strResult, "FetchCommonData", strParameters);
                m_Connection.CloseDB();
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            //--get column details from view_columns
            strTemp = "SELECT " + m_Connection.DB_NULL + "(tabledef_column_name, '') as table_column_name," +
                      m_Connection.DB_NULL + "(view_column_name, '') as ColumnName," +
                      m_Connection.DB_NULL + "(view_column_width, 0) as ColWidth," +
                      m_Connection.DB_NULL + "(tabledef_column_visible, 'N') as ColVisible," +
                      m_Connection.DB_NULL + "(tabledef_grouping_sort_column, '') as GroupSortColumn " +
                      "FROM CRM_View_Columns,crm_table_columns " +
                      "WHERE view_column_view_id=" + nViewID + " " +
                      "AND tabledef_column_visible = 'Y' " +
                      "AND view_column_tabledef_id = tabledef_id " +
                      "ORDER BY view_column_sequence";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable dtTableDefinition = new DataTable("TableDefinition");
            m_DataAdapterOdbc.Fill(dtTableDefinition);
            if (dtTableDefinition.Rows.Count == 0)
            {
                nResultCode = -1;
                strResult = "No columns defined in the View";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            for (int i = 0; i < dtTableDefinition.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(szColumns))
                    szColumns = Convert.ToString(dtTableDefinition.Rows[i]["table_column_name"]) + " as " + Convert.ToString(dtTableDefinition.Rows[i]["ColumnName"]);
                else
                    szColumns += "," + Convert.ToString(dtTableDefinition.Rows[i]["table_column_name"]) + " as " + Convert.ToString(dtTableDefinition.Rows[i]["ColumnName"]);
            }
            szColumns = "SELECT ID," +
                 m_Connection.DB_FUNCTION + "GetRelatedName('" + strCallType + "', ID) as IDName," +
                szColumns + " FROM " + strViewTable + " WHERE " + m_Connection.DB_NULL + "(Useable,'Y') = 'Y' ";

            DataTable dtTable = new DataTable();
            if (szSearchCloumnName == "All" && !string.IsNullOrEmpty(szSearchText.Trim())) //-- filter based on 'All'
            {
                strTemp = "SELECT tabledef_column_name,tabledef_operator,tabledef_column_type " +
                        "FROM crm_table_columns " +
                        "WHERE tabledef_for='" + strCallType + "' " +
                        "AND tabledef_search_all = 'Y' ";

                m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                m_DataAdapterOdbc.Fill(dtTable);
                if (dtTable.Rows.Count == 0)
                {
                    nResultCode = -1;
                    strResult = "No columns defined for Search 'All'";
                    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                    return m_DataSet;
                }
            }

            m_CommandODBC = new OdbcCommand();
            if (dtTable.Rows.Count > 0)
            {
                string szTemp = string.Empty;
                string szColumnType = "";
                for (int j = 0; j < dtTable.Rows.Count; j++)
                {
                    szSearchCloumnName = dtTable.Rows[j]["tabledef_column_name"].ToString();
                    szSearchOperator = dtTable.Rows[j]["tabledef_operator"].ToString().ToLower();
                    szColumnType = dtTable.Rows[j]["tabledef_column_type"].ToString().ToUpper();

                    //--check if search text is valid date [if column is of Date type]
                    if (szColumnType == "D" && !m_Connection.IsValidDate(szSearchText))
                        continue;
                    //--check if search text is valid numeric [if column is of Numeric type]
                    if (szColumnType == "N" && !m_Connection.IsValidNumeric(szSearchText))
                        continue;

                    if (string.IsNullOrEmpty(szTemp))
                        szTemp += "AND(";
                    else
                        szTemp += "OR ";

                    if (szSearchOperator == "like")
                    {
                        szTemp += szSearchCloumnName + " " + szSearchOperator + " ? ";
                        m_CommandODBC.Parameters.Add("@SearchTextwithLike", OdbcType.VarChar).Value = "%" + szSearchText + "%";
                    }
                    else if (szSearchOperator == "in")
                    {
                        szTemp += szSearchCloumnName + " " + szSearchOperator + " (?) ";
                        m_CommandODBC.Parameters.Add("@SearchText", OdbcType.VarChar).Value = szSearchText;
                    }
                    else
                    {
                        szTemp += szSearchCloumnName + " " + szSearchOperator + " ? ";
                        m_CommandODBC.Parameters.Add("@SearchText", OdbcType.VarChar).Value = szSearchText;
                    }
                }
                if (!string.IsNullOrEmpty(szTemp))
                    szTemp += ") ";

                szFilter += szTemp;
            }
            else if (szSearchCloumnName != "All" && !string.IsNullOrEmpty(szSearchText.Trim()))
            {
                szFilter += "AND " + szSearchCloumnName + " like ? ";
                m_CommandODBC.Parameters.Add("@SearchTextwithLike", OdbcType.VarChar).Value = "%" + szSearchText + "%";
            }
            StringBuilder strTempNew = new StringBuilder();
            strTempNew.Append(szColumns);
            strTempNew.Append(" ");
            strTempNew.Append(szFilter);
            strTempNew.Append(" ");
            strTempNew.Append(strNewCriteria);
            strTempNew.Append(" ");
            strTempNew.Append(szOrderBy);

            //--replace timezone(time span in minutes) wherever #TIMESPAN# is present to get user's local time
            strTempNew = strTempNew.Replace("#TIMESPAN#", nUserTimeZoneSpan.ToString());
            strTempNew = strTempNew.Replace("#ID#", nLoginContactID.ToString());
            if (m_Connection.nDatabaseType == 2) //-- MySql
                strTempNew = strTempNew.Replace("dbo.", "");
            if (IsPhone)
                strTempNew = strTempNew.Replace("#Dial#", "<a href=# id=lnkDial style=color:Blue; onclick=javascript:fncMakeCall();>Dial</a>");
            else
                strTempNew = strTempNew.Replace("#Dial#", "");

            m_CommandODBC.CommandText = strTempNew.ToString();
            m_CommandODBC.Connection = m_Connection.oCon;

            m_DataAdapterOdbc = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "Common";
            m_DataSet.Tables.Add(dtTableDefinition);
            m_DataSet.Tables.Add(dtView);

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCommonData", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCommonData", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get Common Data List [Based On User's Custom Columns]
    [WebMethod]
    public DataSet FetchViewData(string szSearchCloumnName,
                                  string szSearchText,
                                  int nLoginContactID,
                                  int nLoginAccountID, string strLoginAccType,
                                  bool bApplyOwnerFilter, int nLoginDeptID,
                                  int nLoginDesigLevel, int nViewID, bool bApplyViewFilter,
                                  int nUserTimeZoneSpan, string strCallType, string strNewCriteria,
                                  bool IsPhone,
                                  string strViewTable, int offset, int count)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        string szColumns = string.Empty;
        string szFilter = string.Empty;
        string szSearchOperator = string.Empty;
        string szOrderBy = string.Empty;

        try
        {
            //--get basic filter if user is not ADMIN
            if (bApplyOwnerFilter == true)
            {
                DataSet dsDataSet = CreateFilter(nLoginContactID, strCallType, nLoginAccountID,
                                                            strLoginAccType, nLoginDeptID, nLoginDesigLevel);
                if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
                    return dsDataSet;

                if (dsDataSet.Tables["Filter"].Rows.Count > 0)
                    szFilter = dsDataSet.Tables["Filter"].Rows[0]["FilterString"].ToString();
            }
            m_Connection.OpenDB("Galaxy");

            //--extract filter condition and select columns from view
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " view_name," +
                    m_Connection.DB_NULL + "(view_pagesize,50) as view_pagesize," +
                    m_Connection.DB_NULL + "(view_group_expression,'') as view_group_expression," +
                    m_Connection.DB_NULL + "(view_sort_expression,'') as view_sort_expression," +
                    m_Connection.DB_NULL + "(view_filter_expression,'') as view_filter_expression," +
                    m_Connection.DB_NULL + "(view_private,'') as view_private," +
                    m_Connection.DB_NULL + "(view_filter_condition,'') as view_filter_condition," +
                    m_Connection.DB_NULL + "(view_order_by_expression,'') as view_order_by_expression " +
                    "FROM CRM_Views " +
                    "WHERE view_id=" + nViewID + " " + m_Connection.DB_TOP_MYSQL;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable dtView = new DataTable("ViewDetail");
            m_DataAdapterOdbc.Fill(dtView);

            if (dtView.Rows.Count > 0)
            {
                szOrderBy += " " + dtView.Rows[0]["view_order_by_expression"].ToString();
                if (bApplyViewFilter == true)
                    szFilter += " " + dtView.Rows[0]["view_filter_condition"].ToString();
            }
            else
            {
                nResultCode = -1;
                strResult = "Fail - No Columns/View found for FetchCommonData !";
                LogMessage(strTemp + strResult, "FetchCommonData", strParameters);
                m_Connection.CloseDB();
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            //--get column details from view_columns
            strTemp = "SELECT " + m_Connection.DB_NULL + "(tabledef_column_name, '') as table_column_name," +
                      m_Connection.DB_NULL + "(view_column_name, '') as ColumnName," +
                      m_Connection.DB_NULL + "(view_column_width, 0) as ColWidth," +
                      m_Connection.DB_NULL + "(tabledef_column_visible, 'N') as ColVisible," +
                      m_Connection.DB_NULL + "(tabledef_grouping_sort_column, '') as GroupSortColumn " +
                      "FROM CRM_View_Columns,crm_table_columns " +
                      "WHERE view_column_view_id=" + nViewID + " " +
                      "AND tabledef_column_visible = 'Y' " +
                      "AND view_column_tabledef_id = tabledef_id " +
                      "ORDER BY view_column_sequence";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable dtTableDefinition = new DataTable("TableDefinition");
            m_DataAdapterOdbc.Fill(dtTableDefinition);
            if (dtTableDefinition.Rows.Count == 0)
            {
                nResultCode = -1;
                strResult = "No columns defined in the View";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            for (int i = 0; i < dtTableDefinition.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(szColumns))
                    szColumns = Convert.ToString(dtTableDefinition.Rows[i]["table_column_name"]) + " as " + Convert.ToString(dtTableDefinition.Rows[i]["ColumnName"]);
                else
                    szColumns += "," + Convert.ToString(dtTableDefinition.Rows[i]["table_column_name"]) + " as " + Convert.ToString(dtTableDefinition.Rows[i]["ColumnName"]);
            }


            szColumns = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY id ASC) AS rownum,ID," +
                        m_Connection.DB_FUNCTION + "GetRelatedName('" + strCallType + "', ID) as IDName," +
                        szColumns + " FROM " + strViewTable + " WHERE " + m_Connection.DB_NULL + "(Useable,'Y') = 'Y' ";

            DataTable dtTable = new DataTable();
            if (szSearchCloumnName == "All" && !string.IsNullOrEmpty(szSearchText.Trim())) //-- filter based on 'All'
            {
                strTemp = "SELECT tabledef_column_name,tabledef_operator,tabledef_column_type " +
                        "FROM crm_table_columns " +
                        "WHERE tabledef_for='" + strCallType + "' " +
                        "AND tabledef_search_all = 'Y' ";

                m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                m_DataAdapterOdbc.Fill(dtTable);
                if (dtTable.Rows.Count == 0)
                {
                    nResultCode = -1;
                    strResult = "No columns defined for Search 'All'";
                    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                    return m_DataSet;
                }
            }

            m_CommandODBC = new OdbcCommand();
            if (dtTable.Rows.Count > 0)
            {
                string szTemp = string.Empty;
                string szColumnType = "";
                for (int j = 0; j < dtTable.Rows.Count; j++)
                {
                    szSearchCloumnName = dtTable.Rows[j]["tabledef_column_name"].ToString();
                    szSearchOperator = dtTable.Rows[j]["tabledef_operator"].ToString().ToLower();
                    szColumnType = dtTable.Rows[j]["tabledef_column_type"].ToString().ToUpper();

                    //--check if search text is valid date [if column is of Date type]
                    if (szColumnType == "D" && !m_Connection.IsValidDate(szSearchText))
                        continue;
                    //--check if search text is valid numeric [if column is of Numeric type]
                    if (szColumnType == "N" && !m_Connection.IsValidNumeric(szSearchText))
                        continue;

                    if (string.IsNullOrEmpty(szTemp))
                        szTemp += "AND(";
                    else
                        szTemp += "OR ";

                    if (szSearchOperator == "like")
                    {
                        szTemp += szSearchCloumnName + " " + szSearchOperator + " ? ";
                        m_CommandODBC.Parameters.Add("@SearchTextwithLike", OdbcType.VarChar).Value = "%" + szSearchText + "%";
                    }
                    else if (szSearchOperator == "in")
                    {
                        szTemp += szSearchCloumnName + " " + szSearchOperator + " (?) ";
                        m_CommandODBC.Parameters.Add("@SearchText", OdbcType.VarChar).Value = szSearchText;
                    }
                    else
                    {
                        szTemp += szSearchCloumnName + " " + szSearchOperator + " ? ";
                        m_CommandODBC.Parameters.Add("@SearchText", OdbcType.VarChar).Value = szSearchText;
                    }
                }
                if (!string.IsNullOrEmpty(szTemp))
                    szTemp += ") ";

                szFilter += szTemp;
            }
            else if (szSearchCloumnName != "All" && !string.IsNullOrEmpty(szSearchText.Trim()))
            {
                szFilter += "AND " + szSearchCloumnName + " like ? ";
                m_CommandODBC.Parameters.Add("@SearchTextwithLike", OdbcType.VarChar).Value = "%" + szSearchText + "%";
            }

            StringBuilder strTempNew = new StringBuilder();
            strTempNew.Append(szColumns);
            strTempNew.Append(" ");
            strTempNew.Append(szFilter);
            strTempNew.Append(" ");
            strTempNew.Append(strNewCriteria);


            //--replace timezone(time span in minutes) wherever #TIMESPAN# is present to get user's local time
            strTempNew = strTempNew.Replace("#TIMESPAN#", nUserTimeZoneSpan.ToString());
            if (m_Connection.nDatabaseType == 2) //-- MySql
                strTempNew = strTempNew.Replace("dbo.", "");
            if (IsPhone)
                strTempNew = strTempNew.Replace("#Dial#", "<a href=# id=lnkDial style=color:Blue; onclick=javascript:fncMakeCall();>Dial</a>");
            else
                strTempNew = strTempNew.Replace("#Dial#", "");
            strTempNew.Append(") AS data ");
            if (count > 0 && szOrderBy == "")
                strTempNew.Append("WHERE rownum between " + (offset + 1) + " and " + count);

            strTempNew.Append(" ");
            strTempNew.Append(szOrderBy);

            m_CommandODBC.CommandText = strTempNew.ToString();
            m_CommandODBC.Connection = m_Connection.oCon;

            m_DataAdapterOdbc = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "Common";
            m_DataSet.Tables.Add(dtTableDefinition);
            m_DataSet.Tables.Add(dtView);

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCommonData", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCommonData", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get View Details [Based On User's Custom Columns]
    [WebMethod]
    public DataSet FetchViewDetails(string szSearchCloumnName,
                                  string szSearchText,
                                  int nLoginContactID,
                                  int nLoginAccountID, string strLoginAccType,
                                  bool bApplyOwnerFilter, int nLoginDeptID,
                                  int nLoginDesigLevel, int nViewID, bool bApplyViewFilter,
                                  int nUserTimeZoneSpan, string strCallType, string strNewCriteria,
                                  bool IsPhone,
                                  string strViewTable)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        string szColumns = string.Empty;
        string szFilter = string.Empty;
        string szSearchOperator = string.Empty;
        string szOrderBy = string.Empty;

        try
        {
            //--get basic filter if user is not ADMIN
            if (bApplyOwnerFilter == true)
            {
                DataSet dsDataSet = CreateFilter(nLoginContactID, strCallType, nLoginAccountID,
                                                            strLoginAccType, nLoginDeptID, nLoginDesigLevel);
                if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
                    return dsDataSet;

                if (dsDataSet.Tables["Filter"].Rows.Count > 0)
                    szFilter = dsDataSet.Tables["Filter"].Rows[0]["FilterString"].ToString();
            }
            m_Connection.OpenDB("Galaxy");

            //--extract filter condition and select columns from view
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " view_name," +
                    m_Connection.DB_NULL + "(view_pagesize,50) as view_pagesize," +
                    m_Connection.DB_NULL + "(view_group_expression,'') as view_group_expression," +
                    m_Connection.DB_NULL + "(view_sort_expression,'') as view_sort_expression," +
                    m_Connection.DB_NULL + "(view_filter_expression,'') as view_filter_expression," +
                    m_Connection.DB_NULL + "(view_private,'') as view_private," +
                    m_Connection.DB_NULL + "(view_filter_condition,'') as view_filter_condition," +
                    m_Connection.DB_NULL + "(view_order_by_expression,'') as view_order_by_expression " +
                    "FROM CRM_Views " +
                    "WHERE view_id=" + nViewID + " " + m_Connection.DB_TOP_MYSQL;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable dtView = new DataTable("ViewDetail");
            m_DataAdapterOdbc.Fill(dtView);

            if (dtView.Rows.Count > 0)
            {
                szOrderBy += " " + dtView.Rows[0]["view_order_by_expression"].ToString();
                if (bApplyViewFilter == true)
                    szFilter += " " + dtView.Rows[0]["view_filter_condition"].ToString();
            }
            else
            {
                nResultCode = -1;
                strResult = "Fail - No Columns/View found for FetchCommonData !";
                LogMessage(strTemp + strResult, "FetchCommonData", strParameters);
                m_Connection.CloseDB();
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            //--get column details from view_columns
            strTemp = "SELECT " + m_Connection.DB_NULL + "(tabledef_column_name, '') as table_column_name," +
                      m_Connection.DB_NULL + "(view_column_name, '') as ColumnName," +
                      m_Connection.DB_NULL + "(view_column_width, 0) as ColWidth," +
                      m_Connection.DB_NULL + "(tabledef_column_visible, 'N') as ColVisible," +
                      m_Connection.DB_NULL + "(tabledef_grouping_sort_column, '') as GroupSortColumn " +
                      "FROM CRM_View_Columns,crm_table_columns " +
                      "WHERE view_column_view_id=" + nViewID + " " +
                      "AND tabledef_column_visible = 'Y' " +
                      "AND view_column_tabledef_id = tabledef_id " +
                      "ORDER BY view_column_sequence";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable dtTableDefinition = new DataTable("TableDefinition");
            m_DataAdapterOdbc.Fill(dtTableDefinition);
            if (dtTableDefinition.Rows.Count == 0)
            {
                nResultCode = -1;
                strResult = "No columns defined in the View";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            //--get email_count
            strTemp = "SELECT count(*) as count FROM " + strViewTable + " WHERE " + m_Connection.DB_NULL + "(Useable,'Y') = 'Y' ";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable dtRecordCount = new DataTable("RecordCount");
            m_DataAdapterOdbc.Fill(dtRecordCount);

            for (int i = 0; i < dtTableDefinition.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(szColumns))
                    szColumns = Convert.ToString(dtTableDefinition.Rows[i]["table_column_name"]) + " as " + Convert.ToString(dtTableDefinition.Rows[i]["ColumnName"]);
                else
                    szColumns += "," + Convert.ToString(dtTableDefinition.Rows[i]["table_column_name"]) + " as " + Convert.ToString(dtTableDefinition.Rows[i]["ColumnName"]);
            }
            szColumns = "SELECT " + szColumns + " FROM " + strViewTable + " WHERE " + m_Connection.DB_NULL + "(Useable,'Y') = 'Y' ";

            DataTable dtTable = new DataTable();
            if (szSearchCloumnName == "All" && !string.IsNullOrEmpty(szSearchText.Trim())) //-- filter based on 'All'
            {
                strTemp = "SELECT tabledef_column_name,tabledef_operator,tabledef_column_type " +
                        "FROM crm_table_columns " +
                        "WHERE tabledef_for='" + strCallType + "' " +
                        "AND tabledef_search_all = 'Y' ";

                m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                m_DataAdapterOdbc.Fill(dtTable);
                if (dtTable.Rows.Count == 0)
                {
                    nResultCode = -1;
                    strResult = "No columns defined for Search 'All'";
                    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                    return m_DataSet;
                }
            }

            m_CommandODBC = new OdbcCommand();
            if (dtTable.Rows.Count > 0)
            {
                string szTemp = string.Empty;
                string szColumnType = "";
                for (int j = 0; j < dtTable.Rows.Count; j++)
                {
                    szSearchCloumnName = dtTable.Rows[j]["tabledef_column_name"].ToString();
                    szSearchOperator = dtTable.Rows[j]["tabledef_operator"].ToString().ToLower();
                    szColumnType = dtTable.Rows[j]["tabledef_column_type"].ToString().ToUpper();

                    //--check if search text is valid date [if column is of Date type]
                    if (szColumnType == "D" && !m_Connection.IsValidDate(szSearchText))
                        continue;
                    //--check if search text is valid numeric [if column is of Numeric type]
                    if (szColumnType == "N" && !m_Connection.IsValidNumeric(szSearchText))
                        continue;

                    if (string.IsNullOrEmpty(szTemp))
                        szTemp += "AND(";
                    else
                        szTemp += "OR ";

                    if (szSearchOperator == "like")
                    {
                        szTemp += szSearchCloumnName + " " + szSearchOperator + " ? ";
                        m_CommandODBC.Parameters.Add("@SearchTextwithLike", OdbcType.VarChar).Value = "%" + szSearchText + "%";
                    }
                    else if (szSearchOperator == "in")
                    {
                        szTemp += szSearchCloumnName + " " + szSearchOperator + " (?) ";
                        m_CommandODBC.Parameters.Add("@SearchText", OdbcType.VarChar).Value = szSearchText;
                    }
                    else
                    {
                        szTemp += szSearchCloumnName + " " + szSearchOperator + " ? ";
                        m_CommandODBC.Parameters.Add("@SearchText", OdbcType.VarChar).Value = szSearchText;
                    }
                }
                if (!string.IsNullOrEmpty(szTemp))
                    szTemp += ") ";

                szFilter += szTemp;
            }
            else if (szSearchCloumnName != "All" && !string.IsNullOrEmpty(szSearchText.Trim()))
            {
                szFilter += "AND " + szSearchCloumnName + " like ? ";
                m_CommandODBC.Parameters.Add("@SearchTextwithLike", OdbcType.VarChar).Value = "%" + szSearchText + "%";
            }
            StringBuilder strTempNew = new StringBuilder();
            strTempNew.Append(szColumns);
            strTempNew.Append(" ");
            strTempNew.Append(szFilter);
            strTempNew.Append(" ");
            strTempNew.Append(strNewCriteria);
            strTempNew.Append(" ");
            strTempNew.Append(szOrderBy);

            //--replace timezone(time span in minutes) wherever #TIMESPAN# is present to get user's local time
            strTempNew = strTempNew.Replace("#TIMESPAN#", nUserTimeZoneSpan.ToString());
            if (m_Connection.nDatabaseType == 2) //-- MySql
                strTempNew = strTempNew.Replace("dbo.", "");
            if (IsPhone)
                strTempNew = strTempNew.Replace("#Dial#", "<a href=# id=lnkDial style=color:Blue; onclick=javascript:fncMakeCall();>Dial</a>");
            else
                strTempNew = strTempNew.Replace("#Dial#", "");

            m_CommandODBC.CommandText = strTempNew.ToString();
            m_CommandODBC.Connection = m_Connection.oCon;

            m_DataAdapterOdbc = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "Common";
            m_DataSet.Tables.Add(dtTableDefinition);
            m_DataSet.Tables.Add(dtView);
            m_DataSet.Tables.Add(dtRecordCount);

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCommonData", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCommonData", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Create/Save View
    [WebMethod]
    public DataSet SaveReports(long nSessionID, int nSchdID, string strTitle, int nUnit,
                            string strType, string strEnabled, int nMailboxId,
                            string strSendEmail, string strToEmail, string strCcEmail,
                            string strSubject, string strBody,
                            string strNextUpdateTime, string strColumns, string strReortFor, string strFilterCriteria,
                            string strCustomFilter, string strTableName, string strAttachmentPath, long nUserID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        strParameters = "";
        string strColumnID = string.Empty;
        string strTable = string.Empty;
        string strArchiveTable = string.Empty;

        try
        {
            m_Connection.OpenDB("Galaxy");
            m_Connection.BeginTransaction();

            if (strNextUpdateTime == string.Empty)
                strNextUpdateTime = "NULL";
            else
                strNextUpdateTime = Convert.ToDateTime(strNextUpdateTime).ToString("yyyy-MM-dd hh:mm:ss");
            if (nSchdID == 0)
            {
                strTemp = "INSERT INTO crm_scheduled_reports(schd_title,schd_unit," +
                          "schd_type,schd_enabled,schd_mailbox_id," +
                          "schd_send_email,schd_email_to,schd_email_cc," +
                          "schd_subject,schd_message," +
                          "schd_next_update_time,schd_stored_proc,schd_for,schd_filter_criteria,schd_custom_criteria,schd_for_table,schd_attachment_path) VALUES " +
                          "(?," + nUnit + ",?,'" + strEnabled + "'," + nMailboxId + "," +
                          "'" + strSendEmail + "', ?, ?, ?," +
                          "?,'" + strNextUpdateTime + "',?,?,?,'" + strCustomFilter + "',?,?) ";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                m_CommandODBC.Parameters.Add("@Title", OdbcType.VarChar).Value = strTitle;
                m_CommandODBC.Parameters.Add("@Type", OdbcType.VarChar).Value = strType;
                m_CommandODBC.Parameters.Add("@ToEmail", OdbcType.VarChar).Value = strToEmail;
                m_CommandODBC.Parameters.Add("@CcEmail", OdbcType.VarChar).Value = strCcEmail;
                m_CommandODBC.Parameters.Add("@Subject", OdbcType.VarChar).Value = strSubject;
                m_CommandODBC.Parameters.Add("@Body", OdbcType.VarChar).Value = strBody;
                // m_CommandODBC.Parameters.Add("@NextUpdateTime", OdbcType.VarChar).Value = strNextUpdateTime;
                m_CommandODBC.Parameters.Add("Columns", OdbcType.VarChar).Value = strColumns;
                m_CommandODBC.Parameters.Add("ReortFor", OdbcType.VarChar).Value = strReortFor;
                m_CommandODBC.Parameters.Add("FilterCriteria", OdbcType.VarChar).Value = strFilterCriteria;
                m_CommandODBC.Parameters.Add("TableName", OdbcType.VarChar).Value = strTableName;
                m_CommandODBC.Parameters.Add("AttachmentPath", OdbcType.VarChar).Value = strAttachmentPath;

            }
            else
            {
                strTemp = "UPDATE crm_scheduled_reports SET " +
                         "schd_title=?," +
                         "schd_unit=" + nUnit + "," +
                         "schd_type= ?," +
                         "schd_enabled='" + strEnabled + "'," +
                         "schd_mailbox_id=" + nMailboxId + "," +
                         "schd_send_email='" + strSendEmail + "'," +
                         "schd_email_to=?," +
                         "schd_email_cc=?," +
                         "schd_subject=?," +
                         "schd_message=?," +
                         "schd_next_update_time='" + strNextUpdateTime + "'," +
                         "schd_stored_proc=?," +
                         "schd_for=?," +
                         "schd_filter_criteria=?," +
                         "schd_last_update_time= " + m_Connection.DB_UTC_DATE + "," +
                         "schd_custom_criteria = '" + strCustomFilter + "'," +
                         "schd_for_table=?," +
                         "schd_attachment_path=? " +
                         "WHERE schd_id=" + nSchdID;

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                m_CommandODBC.Parameters.Add("@Title", OdbcType.VarChar).Value = strTitle;
                m_CommandODBC.Parameters.Add("@Type", OdbcType.VarChar).Value = strType;
                m_CommandODBC.Parameters.Add("@ToEmail", OdbcType.VarChar).Value = strToEmail;
                m_CommandODBC.Parameters.Add("@CcEmail", OdbcType.VarChar).Value = strCcEmail;
                m_CommandODBC.Parameters.Add("@Subject", OdbcType.VarChar).Value = strSubject;
                m_CommandODBC.Parameters.Add("@Body", OdbcType.VarChar).Value = strBody;
                m_CommandODBC.Parameters.Add("Columns", OdbcType.VarChar).Value = strColumns;
                m_CommandODBC.Parameters.Add("ReortFor", OdbcType.VarChar).Value = strReortFor;
                m_CommandODBC.Parameters.Add("FilterCriteria", OdbcType.VarChar).Value = strFilterCriteria;
                m_CommandODBC.Parameters.Add("TableName", OdbcType.VarChar).Value = strTableName;
                m_CommandODBC.Parameters.Add("AttachmentPath", OdbcType.VarChar).Value = strAttachmentPath;

            }

            int nCount = m_CommandODBC.ExecuteNonQuery();

            if (nCount != 1)
            {
                nResultCode = -1;
                strResult = "Fail - Unable to update Report ";
                LogMessage(strTemp + strResult, "SaveReports", strParameters);
                m_Connection.CloseDB();
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            if (nSchdID == 0 && nCount == 1)
            {
                strTemp = "SELECT @@identity AS idty";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                nSchdID = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
                m_CommandODBC.Dispose();
            }

            nResultCode = nSchdID;
            strResult = "pass";
            m_Connection.Commit();
        }

        catch (OdbcException ex)
        {
            m_Connection.Rollback();
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "SaveReports", strParameters);
        }

        catch (Exception ex)
        {
            m_Connection.Rollback();
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "SaveReports", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get table for from Table Definitions
    [WebMethod]
    public DataSet GetObjectTableNames()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            if (m_Connection.nDatabaseType == 2)
                strTemp = "select distinct object_name, object_table_name, concat(object_code,'$',object_table_name) as table_name,object_code FROM crm_tables where object_code in ('CST','CAS','CNT','USR','CHT','QUE','DOC','EML','PNS','RQF') crm_tables ORDER BY object_name";
            else
                strTemp = "select distinct object_name, object_table_name, (object_code + '$' + object_table_name) as table_name,object_code FROM crm_tables where object_code in ('CST','CAS','CNT','USR','CHT','QUE','DOC','EML','PNS','RQF') ORDER BY object_name";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "TableFor";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "TableDefor", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "TableDefor", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get table for from Table Definitions
    [WebMethod]
    public DataSet FillTemplateColumn(long nSessionID, string strTableFor)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT tabledef_id,tabledef_column_header FROM crm_table_columns WHERE tabledef_for='" + strTableFor + "' ";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "TableFor";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "TableDefor", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "TableDefor", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get GetTemplates from the table crm_message_templates
    [WebMethod]
    public DataSet GetTemplateDetails(int nTemplateID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " + m_Connection.DB_NULL + "(id,0) as template_id," +
                      m_Connection.DB_NULL + "(template_type,'') as template_type," +
                      m_Connection.DB_NULL + "(template_for,'') as template_for," +
                      m_Connection.DB_NULL + "(template_email_subject,'') as template_email_subject," +
                      m_Connection.DB_NULL + "(template_email_text,'') as template_email_text," +
                      m_Connection.DB_NULL + "(template_sms_text,'') as template_sms_text," +
                      m_Connection.DB_NULL + "(template_email_to,'') as template_email_to," +
                      m_Connection.DB_NULL + "(template_email_cc,'') as template_email_cc," +
                      m_Connection.DB_NULL + "(template_email_bcc,'') as template_email_bcc," +
                      m_Connection.DB_NULL + "(template_sms_to,'') as template_sms_to," +
                      m_Connection.DB_NULL + "(template_pop_subject,'') as template_pop_subject," +
                      m_Connection.DB_NULL + "(template_pop_text,'') as template_pop_text," +
                      m_Connection.DB_NULL + "(template_enabled,'') as template_enabled, " +
                      m_Connection.DB_NULL + "(template_email_enable,'0') as template_email_enable, " +
                      m_Connection.DB_NULL + "(template_sms_enable,'0') as template_sms_enable, " +
                      m_Connection.DB_NULL + "(template_alarm_enable,'0') as template_alarm_enable, " +
                      m_Connection.DB_NULL + "(template_desc,'') as template_description ," +
                       m_Connection.DB_NULL + "(mailbox_id,0) as template_mailbox_id ," +
                       m_Connection.DB_NULL + "(template_type_id,0) as template_type_id, " +
                       m_Connection.DB_NULL + "(template_categ_id,0) as template_categ_id, " +
                       m_Connection.DB_NULL + "(template_subcateg_ids,'') as template_subcateg_ids, " +
                       m_Connection.DB_NULL + "(template_Level,0) as template_Level " +
                      "FROM crm_message_templates WHERE id= " + nTemplateID + " " + m_Connection.DB_TOP_MYSQL;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "TemplateFor";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetTemplateDetails", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetTemplateDetails", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get Template details with Filter Criterias based on Template ID
    [WebMethod]
    public DataSet FetchTemplateDetails(string szSearchText,string chart)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            if (chart != string.Empty)
                strTemp = "SELECT id,template_type,(select [OBJECT_NAME]   from crm_tables where object_code=template_for) as title,template_for,template_pop_subject,template_pop_text," + m_Connection.DB_NULL + "(template_desc,'') as template_desc " +
                       "FROM crm_message_templates WHERE id > 0 and template_for<>'ESC' ";

            else
                strTemp = "SELECT id,template_type,(select [OBJECT_NAME]   from crm_tables where object_code=template_for) as title,template_for,template_email_subject,template_pop_text," + m_Connection.DB_NULL + "(template_desc,'') as template_desc " +
                       "FROM crm_message_templates WHERE id > 0 and template_for<>'ESC' ";
            
            if (szSearchText != string.Empty)
                strTemp += " AND template_email_subject like '%" + szSearchText + "%'";

            if (chart!=string.Empty)
                strTemp += " AND template_for = 'CHT'  and isnull(template_type,'') = '' ";
           else
                strTemp += " and isnull(template_type,'') <> '' ";

            strTemp += " ORDER BY template_for";

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
    #endregion

    #region Get Escalation Matrix Template details with Filter Criterias 
    [WebMethod]
    public DataSet FetchEscalationMatrix(string szSearchText)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
                 strTemp = @"select  id,dbo.GetTypeName(template_type_id) TypeName,dbo.GetCategoryName(template_categ_id) CategoryName,
                            dbo.GetTempleteSubCategories(template_subcateg_ids,',') SubCategories,template_for,
                           'Level'+convert(varchar(10), template_Level)template_Level, template_email_subject,template_enabled,dbo.GetContact(created_by)CreatedBy,created_date
                            from crm_message_templates
                            Where template_for='ESC' ";

          

            if (szSearchText != string.Empty)
                strTemp += " AND template_email_subject like '%" + szSearchText + "%'";

          
         
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
            LogMessage(strTemp + strResult, "FetchEscalationMatrix", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchEscalationMatrix", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Create/Save Buzz
    [WebMethod]
    public DataSet SaveBuzz(long nSessionID, int nParentID, string strComments)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();


        strParameters = "";
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "INSERT INTO crm_buzz(buzz_parent_id,buzz_description, buzz_date) VALUES " +
                            "(" + nParentID + ",?," + m_Connection.DB_UTC_DATE + ") ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.Parameters.Add("@Comments", OdbcType.VarChar).Value = strComments;

            int nCount = m_CommandODBC.ExecuteNonQuery();

            if (nCount == 1)
            {
                nResultCode = 0;
                strResult = "pass";
            }
        }
        catch (OdbcException ex)
        {
            m_Connection.Rollback();
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "SaveBuzz", strParameters);
        }
        catch (Exception ex)
        {
            m_Connection.Rollback();
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "SaveBuzz", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get Buzz details
    [WebMethod]
    public DataSet FetchBuzzDetails(long nSessionID, int nParentID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            if (m_Connection.nDatabaseType == 2)
                strTemp = "SELECT buzz_id,buzz_description,DATE_FORMAT(buzz_date, '%d %b %H:%i') as buzz_date,get_buzz_parent(buzz_id) as commentscount from crm_buzz " +
                    "where buzz_parent_id =0 Order By 1 desc";
            else
                strTemp = "SELECT buzz_id,buzz_description,convert(varchar, buzz_date, 9) as buzz_date,get_buzz_parent(buzz_id) as commentscount from crm_buzz " +
                "where buzz_parent_id =0 Order By 1 desc";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "BuzzDetail";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchBuzzDetails", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchBuzzDetails", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get Sub Buzz details on the basic of BUZZID
    [WebMethod]
    public DataSet FetchSubBuzzDetails(long nSessionID, int nParentID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT buzz_id,buzz_description, buzz_parent_id,(select count(*) from crm_Buzz where buzz_parent_id = " + nParentID + ") as count from crm_buzz " +
                      "where buzz_parent_id= " + nParentID + "";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataSet = new DataSet();
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "BuzzSubDetail";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchBuzzDetails", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchBuzzDetails", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Delete Team On the basis of Account
    [WebMethod]
    public DataSet RemoveAccountTeam(string TeamID, int nAccountID)
    {
        long nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            m_Connection.BeginTransaction();

            strTemp = "SELECT COUNT(*) FROM CRM_Team_Members " +
                    "WHERE accteam_id = " + TeamID + " " +
                    "AND account_id=" + nAccountID;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            int nCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
            if (nCount > 0)
            {
                nResultCode = -1;
                m_Connection.Rollback();
                strResult = "Unable to delete this Team, as some contacts are attached to this Team !";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            strTemp = "DELETE FROM crm_account_Teams " +
                      "WHERE accteam_id = " + TeamID + "";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.ExecuteNonQuery();

            m_Connection.Commit();
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            m_Connection.Rollback();
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Delete_team_Info", strParameters);
        }
        catch (Exception ex)
        {
            m_Connection.Rollback();
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Delete_team_Info", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Save Account Team and Members on the basis of Account ID and Team ID
    [WebMethod]
    public DataSet SaveAccountTeam(string strAccteamName, int nAccountID)
    {
        long nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_Connection.BeginTransaction();

            strTemp = "SELECT COUNT(*) FROM CRM_Account_Teams " +
                       "WHERE accteam_name = ? " +
                       "AND accteam_account_id = " + nAccountID + "";

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.Parameters.Add("@strAccteamName", OdbcType.VarChar).Value = strAccteamName;
            int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());

            if (nRowCount > 0)
            {
                nResultCode = -1;
                m_Connection.Rollback();
                strResult = "This Team has already been added !";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            strTemp = "INSERT INTO CRM_Account_Teams( accteam_name,accteam_account_id)" +
                      " VALUES ('" + strAccteamName + "'," + nAccountID + ") ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);

            int nRow = Convert.ToInt32(m_CommandODBC.ExecuteNonQuery());
            if (nRow == 0)
            {
                nResultCode = -1;
                strResult = "Fail - Unable to update Save_Account_Team";
                m_Connection.Rollback();
                m_Connection.CloseDB();
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            m_Connection.Commit();

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "Save_Account_Team", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "Save_Account_Team", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();

        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Saving Team Members
    [WebMethod]
    public DataSet SaveTeamMember(string AccTeamID, string MemberID, string MemberLevel, int nAccountID)
    {
        long nResultCode = -1;
        string strResult = "Fail - ";

        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT COUNT(*) FROM crm_team_members " +
                         "WHERE contact_id = " + MemberID + " " +
                         "AND account_id =" + nAccountID + " " +
                         "AND accteam_id = " + AccTeamID + " ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());

            if (nRowCount > 0)
            {
                strTemp = "UPDATE crm_team_members " +
                          "SET contact_level='" + MemberLevel + "' " +
                         "WHERE contact_id = " + MemberID + " " +
                         "AND account_id =" + nAccountID + " " +
                         "AND accteam_id = " + AccTeamID + " ";
            }
            else
            {
                strTemp = "INSERT INTO crm_team_members " +
                          "(accteam_id,contact_id,account_id,contact_level) " +
                          "Values " +
                          "(" + AccTeamID + "," + MemberID + "," +
                          nAccountID + " ,'" + MemberLevel + "') ";
            }
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.ExecuteNonQuery();

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "SaveTeamMember", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "SaveTeamMember", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Remove Team Member
    [WebMethod]
    public DataSet RemoveTeamMember(string AccTeamID, string MemberID, int nAccountID)
    {
        long nResultCode = -1;
        string strResult = "Fail - ";

        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "DELETE FROM crm_team_members " +
                         "WHERE contact_id = " + MemberID + " " +
                         "AND account_id =" + nAccountID + " " +
                         "AND accteam_id = " + AccTeamID + " ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "SaveTeamMember", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "SaveTeamMember", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region RemoveQueueMember
    [WebMethod]
    public DataSet RemoveQueueMember(string MemberID, int QueueID)
    {
        long nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "DELETE FROM crm_queue_members " +
                       "WHERE queue_id=" + QueueID + " " +
                       "AND queue_member_contact_id = " + MemberID;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.ExecuteNonQuery();

            m_Connection.Commit();
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            m_Connection.Rollback();
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "RemoveQueueMember", strParameters);
        }
        catch (Exception ex)
        {
            m_Connection.Rollback();
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "RemoveQueueMember", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region SaveQueueMember
    [WebMethod]
    public DataSet SaveQueueMember(string MemberID, int QueueID)
    {
        long nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_Connection.BeginTransaction();

            strTemp = "SELECT COUNT(*) FROM CRM_queue_members " +
                       "WHERE queue_id=" + QueueID + " " +
                       "AND queue_member_contact_id = " + MemberID;

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());

            if (nRowCount > 0)
            {
                nResultCode = -1;
                m_Connection.Rollback();
                strResult = "This member has already been added !";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            strTemp = "INSERT INTO CRM_queue_members( queue_id,queue_member_contact_id)" +
                      " VALUES (" + QueueID + "," + MemberID + ") ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);

            int nRow = Convert.ToInt32(m_CommandODBC.ExecuteNonQuery());
            if (nRow == 0)
            {
                nResultCode = -1;
                strResult = "Fail - Unable to update crm_queue_members";
                m_Connection.Rollback();
                m_Connection.CloseDB();
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            m_Connection.Commit();

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "SaveQueueMember", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "SaveQueueMember", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();

        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Delete Department On the basis of Account
    [WebMethod]
    public DataSet RemoveAccountDepartment(string DepartmentID, int nAccountID)
    {
        long nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            m_Connection.BeginTransaction();

            strTemp = "SELECT COUNT(*) FROM CRM_Department_Members " +
                    "WHERE accdept_id = " + DepartmentID + " " +
                    "AND account_id=" + nAccountID;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            int nCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
            if (nCount > 0)
            {
                nResultCode = -1;
                m_Connection.Rollback();
                strResult = "Unable to delete this Department, as some contacts are attached to this Team !";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            strTemp = "DELETE FROM crm_account_Departments " +
                      "WHERE accdept_id = " + DepartmentID + "";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.ExecuteNonQuery();

            m_Connection.Commit();
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            m_Connection.Rollback();
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Delete_Department_Info", strParameters);
        }
        catch (Exception ex)
        {
            m_Connection.Rollback();
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Delete_Department_Info", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Save Account Department and Members on the basis of Account ID and Team ID
    [WebMethod]
    public DataSet SaveAccountDepartment(string strAccdepartmentName, int nAccountID)
    {
        long nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_Connection.BeginTransaction();

            strTemp = "SELECT COUNT(*) FROM CRM_Account_Departments " +
                       "WHERE accdept_name = ? " +
                       "AND accdept_account_id = " + nAccountID + "";

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.Parameters.Add("@strAccdepartmentName", OdbcType.VarChar).Value = strAccdepartmentName;
            int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());

            if (nRowCount > 0)
            {
                nResultCode = -1;
                m_Connection.Rollback();
                strResult = "This Department has already been added !";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            strTemp = "INSERT INTO CRM_Account_Departments( accdept_name,accdept_account_id)" +
                      " VALUES ('" + strAccdepartmentName + "'," + nAccountID + ") ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);

            int nRow = Convert.ToInt32(m_CommandODBC.ExecuteNonQuery());
            if (nRow == 0)
            {
                nResultCode = -1;
                strResult = "Fail - Unable to update Save_Account_Department";
                m_Connection.Rollback();
                m_Connection.CloseDB();
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            m_Connection.Commit();

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "Save_Account_Department", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "Save_Account_Department", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();

        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Saving Department Members
    [WebMethod]
    public DataSet SaveDepartmentMember(string AccDepartmentID, string MemberID, string MemberLevel, int nAccountID)
    {
        long nResultCode = -1;
        string strResult = "Fail - ";

        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT COUNT(*) FROM crm_department_members " +
                         "WHERE contact_id = " + MemberID + " " +
                         "AND account_id =" + nAccountID + " " +
                         "AND accdept_id = " + AccDepartmentID + " ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());

            if (nRowCount > 0)
            {
                strTemp = "UPDATE crm_department_members " +
                          "SET contact_level='" + MemberLevel + "' " +
                         "WHERE contact_id = " + MemberID + " " +
                         "AND account_id =" + nAccountID + " " +
                         "AND accdept_id = " + AccDepartmentID + " ";
            }
            else
            {
                strTemp = "INSERT INTO crm_department_members " +
                          "(accdept_id,contact_id,account_id,contact_level) " +
                          "Values " +
                          "(" + AccDepartmentID + "," + MemberID + "," +
                          nAccountID + " ,'" + MemberLevel + "') ";
            }
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.ExecuteNonQuery();

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "SaveDepartmentMember", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "SaveDepartmentMember", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Remove Department Member
    [WebMethod]
    public DataSet RemoveDepartmentMember(string AccDepartmentID, string MemberID, int nAccountID)
    {
        long nResultCode = -1;
        string strResult = "Fail - ";

        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "DELETE FROM crm_department_members " +
                         "WHERE contact_id = " + MemberID + " " +
                         "AND account_id =" + nAccountID + " " +
                         "AND accdept_id = " + AccDepartmentID + " ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "SaveDepartmentMember", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "SaveDepartmentMember", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region RemoveAccountDesignations
    [WebMethod]
    public DataSet RemoveAccountDesignations(string DesignationID, int nAccountID)
    {
        long nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "DELETE FROM crm_account_Designations " +
                      "WHERE accdesig_id = " + DesignationID + "";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.ExecuteNonQuery();

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            m_Connection.Rollback();
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "RemoveAccountDesignations", strParameters);
        }
        catch (Exception ex)
        {
            m_Connection.Rollback();
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "RemoveAccountDesignations", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region SaveAccountDesignation
    [WebMethod]
    public DataSet SaveAccountDesignation(string strAccDesignationsName, int nAccountID, string Level)
    {
        long nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT COUNT(*) FROM crm_account_Designations " +
                       "WHERE accdesig_name = ? " +
                       "AND accdesig_account_id = " + nAccountID + "";

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.Parameters.Add("@strAccdepartmentName", OdbcType.VarChar).Value = strAccDesignationsName;
            int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());

            if (nRowCount > 0)
            {
                nResultCode = -1;
                m_Connection.Rollback();
                strResult = "This Designation has already been added !";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            strTemp = "INSERT INTO crm_account_Designations( accdesig_name,accdesig_account_id, accdesig_level)" +
                      " VALUES ('" + strAccDesignationsName + "'," + nAccountID + "," + Level + ") ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            Convert.ToInt32(m_CommandODBC.ExecuteNonQuery());

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "SaveAccountDesignation", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            m_Connection.Rollback();
            LogMessage(strTemp + strResult, "SaveAccountDesignation", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();

        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  FetchProductList()
    [WebMethod]
    public DataSet FetchProductList()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(product_id,0) as product_id," +
                      m_Connection.DB_NULL + "(product_name,'') as product_name " +
                      "FROM crm_products " +
                      "WHERE product_enabled = 'Y' ";

            strTemp += "ORDER BY product_name";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "Products";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchProductList", "");
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchProductList", "");
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetRoleActivities
    [WebMethod]
    /*** fetch Role BS ***/
    public DataSet GetRoleActivities(int nRoleId, string strCategory)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            if (m_Connection.nDatabaseType == 2) //-- MySql
            {
                strTemp = "SELECT concat(cast(roledet_activity_id as varchar(10)),'|',cast(" + m_Connection.DB_NULL + "(roledet_permission_level,0)as varchar(10))) as ColumnValue, ";
            }
            else
            {
                strTemp = "SELECT(Convert(varchar(10), roledet_activity_id)+'|'+Convert(varchar(10)," + m_Connection.DB_NULL + "(roledet_permission_level,0))) as ColumnValue, ";
            }
            strTemp += "activity_name as AcitivityName " +
                     "FROM crm_Role_Activity,crm_activity_master " +
                     "WHERE roledet_role_id=" + nRoleId +
                     "AND activity_id=roledet_activity_id ";
            if (strCategory != "All")
                strTemp += " AND " + m_Connection.DB_NULL + "(activity_description,'') = '" + strCategory + "' ";
            strTemp += " ORDER BY AcitivityName";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "AssignedActivity";

            if (m_Connection.nDatabaseType == 2) //-- MySql
            {
                strTemp = "SELECT concat(cast(activity_id as varchar(10)),'|62' as ColumnValue, ";
            }
            else
            {
                strTemp = "SELECT Convert(varchar(10), activity_id)+'|62' as ColumnValue, ";
            }
            strTemp += "activity_name as AcitivityName " +
                       "from crm_Activity_master " +
                       "where activity_id not in " +
                       "(select roledet_activity_id from crm_Role_Activity where roledet_role_id=" + nRoleId + ") ";
            if (strCategory != "All")
                strTemp += " AND " + m_Connection.DB_NULL + "(activity_description,'') = '" + strCategory + "' ";
            strTemp += "ORDER BY activity_name";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[1].TableName = "UnAssignedActivity";

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

    #region  Getting All Activity Category
    [WebMethod]
    public DataSet FetchActivityCategory()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select distinct activity_description from crm_Activity_master where activity_description is not null";

            strTemp += " ORDER BY activity_description";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "ActivityCategory";

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

    #region  Insert Roles Activity
    [WebMethod]
    public DataSet InsertRolesActivity(int ActivityID, int RoleID, int Permission)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "INSERT INTO crm_role_activity (roledet_activity_id,roledet_role_id,roledet_permission_level) " +
                "VALUES(" + ActivityID.ToString() + "," + RoleID.ToString() + "," + Permission.ToString() + ")";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Update Roles Activity
    [WebMethod]
    public DataSet UpdateRolesActivityPermission(int ActivityID, int RoleID, int Permission)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "UPDATE crm_role_activity SET roledet_permission_level = " + Permission +
                " WHERE roledet_activity_id = " + ActivityID + " AND roledet_role_id = " + RoleID;
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Delete Roles Activity
    [WebMethod]
    public DataSet DeleteRolesActivity(string ActivityID, int RoleID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "DELETE FROM crm_role_activity WHERE roledet_role_id = " + RoleID.ToString() + " AND roledet_activity_id IN (" + ActivityID + ")";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetCaseInventories
    [WebMethod]
    public DataSet GetCaseInventories(int CaseID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            //13-12 manju
            m_Connection.OpenDB("Galaxy");
            strTemp = "select id, inventory_id, " + m_Connection.DB_FUNCTION + "GetSerialNo(inventory_id) as SerialNo," +
                " isnull(remarks,'') as Problem, " +
                m_Connection.DB_FUNCTION + "GetInventoryFields(inventory_id,'install') as InventoryInstallationDate," +
                m_Connection.DB_FUNCTION + "GetInventoryFields(inventory_id,'status') as InventoryStatus," +
                m_Connection.DB_FUNCTION + "GetInventoryFields(inventory_id,'product') as Product," +
                m_Connection.DB_FUNCTION + "GetTaskOwnerName(task_id) as TaskOwnerName," +
                m_Connection.DB_FUNCTION + "GetTasksubject(task_id) as TaskSubject," +
                m_Connection.DB_FUNCTION + "GetTaskStatus(task_id) as TaskStatus," +
                m_Connection.DB_FUNCTION + "GetTaskOpenTime(task_id) as TaskOpenTime," +
                m_Connection.DB_FUNCTION + "INVServiceToolTip(inventory_id) as ToolTipText " +
                "from crm_case_inventories where case_id=" + CaseID;
            //end 13-12

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "CaseInventories";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetCaseInventories", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetCaseInventories", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetCaseInventories
    [WebMethod]
    public DataSet GetAccountInventories(int AccountID, int CaseID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select inv_serial_no, id from crm_inventories where related_to_id =" + AccountID +
                "and id not in (select inventory_id from crm_case_inventories where case_id = " + CaseID + ")";


            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "AccountInventories";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetAccountInventories", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetAccountInventories", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Insert Case Inventory
    [WebMethod]
    public DataSet InsertCaseInventory(int CaseID, int InventoryID, string strText, int TaskID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "INSERT INTO crm_case_inventories (case_id,inventory_id,related_text,task_id) " +
                "VALUES(" + CaseID.ToString() + "," + InventoryID.ToString() + ",?," + TaskID + ")";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.Add("@strText", OdbcType.VarChar).Value = strText;
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  DeleteCaseInventory
    [WebMethod]
    public DataSet DeleteCaseInventory(int ID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "DELETE FROM crm_case_inventories WHERE id =" + ID;
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Update Task ID in CaseInventory
    [WebMethod]
    public DataSet UpdateCaseInventory(int ID, int TaskID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "UPDATE CRM_CASE_INVENTORIES SET task_id = " + TaskID + " WHERE id =" + ID;
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetInventoryDetails
    [WebMethod]
    public DataSet GetInventoryDetails(int InventoryID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select inv_serial_no, inv_status,inv_product_name,inv_warranty_end_date,inv_installation_date " +
                "from crm_inventories where id=" + InventoryID;


            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Inventory";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetInventoryDetails", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetInventoryDetails", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Get Approved Part details
    [WebMethod]
    public DataSet FetchApprovedPartDetails(int TaskID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT crm_requisitions.req_assigned_to_name, crm_requisitions.owner_name, crm_inventories_part_details.part_no," +
                        "crm_inventories_part_details.description, crm_inventories_part_details.quantity FROM crm_inventories_part_details " +
                        "INNER JOIN crm_requisitions ON crm_inventories_part_details.requisition_id = crm_requisitions.id " +
                        "AND crm_inventories_part_details.is_invoiced = 'Y' AND crm_inventories_part_details.task_id = " + TaskID;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "PartDetails";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchInventoryPartDetails", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchInventoryPartDetails", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get Part details
    [WebMethod]
    public DataSet FetchInventoryPartDetails(int FormID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT id, part_no, description, quantity, is_invoiced FROM crm_inventories_part_details " +
                        "WHERE  requisition_id=" + FormID;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "PartDetails";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchInventoryPartDetails", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchInventoryPartDetails", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region FetchRequisitionForms
    [WebMethod]
    public DataSet FetchRequisitionForms(int TaskID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT * FROM crm_requisitions WHERE related_to='TSK' AND useable='Y' " +
                        "AND related_to_id = " + TaskID;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "RQFDetails";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchRequisitionForms", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchRequisitionForms", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region  Insert Case Inventory
    [WebMethod]
    public DataSet InsertUpdateInventoryPartDetails(int Id, int TaskID, string strPartno, string strDesc, int Quantity, string strInvoiced, int FormID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            if (Id <= 0)
            {
                strTemp = "INSERT INTO crm_inventories_part_details (task_id, part_no, description, quantity, is_invoiced, requisition_id) " +
                    "VALUES(" + TaskID.ToString() + ",'" + strPartno + "',?," + Quantity.ToString() + ",'" + strInvoiced + "', " + FormID + ")";
            }
            else
            {
                strTemp = "UPDATE crm_inventories_part_details set part_no = '" + strPartno + "'" +
                            ", description = ?, quantity = " + Quantity + ", requisition_id = " + FormID +
                            " WHERE id = " + Id;
            }

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.Add("@strDesc", OdbcType.VarChar).Value = strDesc;
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Delete Inventory Part Details
    [WebMethod]
    public DataSet DeleteInventoryPartDetails(int Id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "DELETE FROM crm_inventories_part_details WHERE id = " + Id;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Insert Case Inventory
    [WebMethod]
    public DataSet UpdateInventoryPartStatus(int FormID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "UPDATE crm_inventories_part_details SET is_invoiced = 'Y'" +
                        " WHERE requisition_id = " + FormID;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Fetch Accounts by CLI
    [WebMethod]
    public DataSet FetchAccountsByCLI(string strCLI)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT id FROM crm_accounts WHERE cust_phone LIKE '%" + strCLI + "%' " +
                        "OR id IN (SELECT DISTINCT related_to_id FROM crm_contact_details " +
                        "WHERE related_to = 'CST' " +
                        "AND upper([type]) = 'PHONE' " +
                        "AND value LIKE '%" + strCLI + "%')";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "Contact";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchAccountsByCLI", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchAccountsByCLI", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Get message Templates
    [WebMethod]
    public DataSet FetchTemplates(string szType)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        strParameters = szType;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT ID, template_type as name FROM crm_message_templates " +
                      "WHERE template_for='" + szType + "' " +
                      "ORDER BY template_type";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "General";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchTemplates", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchTemplates", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region  Getting All States on the basis of Country
    [WebMethod]
    public DataSet FetchStateForAppointment(string strCountryCode)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(state_code,'') as state_code," +
                      m_Connection.DB_NULL + "(state_name,'') as state_name," +
                      m_Connection.DB_NULL + "(state_country_code,'') as state_country_code " +
                      "FROM CRM_States ";
            strTemp += "WHERE state_code in (select distinct cust_state_code from crm_accounts where cust_state_code is not null and cust_state_code <> '')";

            if (strCountryCode != "")
                strTemp += " AND state_country_code='" + strCountryCode + "' ";
            strTemp += " ORDER BY state_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "State";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchState", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchState", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Getting All States on the basis of Country
    [WebMethod]
    public DataSet FetchCustomerCityWise(string strCityCode)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select id, cust_name from crm_accounts WHERE cust_type = 'B' AND cust_enabled = 'Y'";
            if (strCityCode != "0")
                strTemp += " AND cust_city_code = '" + strCityCode + "'";
            strTemp += " ORDER BY cust_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Branch";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCustomerCityWise", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCustomerCityWise", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Get file content[web method]
    [WebMethod]
    public byte[] DownloadFile(string szFilePath, ref string strExtn)
    {
        szFilePath = szFilePath.Replace("*", "");
        szFilePath = szFilePath.Replace("?", "");
        szFilePath = szFilePath.Replace("<", "");
        szFilePath = szFilePath.Replace(">", "");
        szFilePath = szFilePath.Replace("\"", "/");
        szFilePath = szFilePath.Replace("|", "");
        string szTempFilePath = szFilePath;
        byte[] Buffer;
        try
        {
            strExtn = "";
            if (System.IO.File.Exists(szTempFilePath))
                strExtn = Path.GetExtension(szTempFilePath);

            FileStream MyFileStream = new FileStream(szTempFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            Int64 FileSize = MyFileStream.Length;
            Buffer = new byte[(int)FileSize];
            MyFileStream.Read(Buffer, 0, (int)(MyFileStream.Length));
            MyFileStream.Close();
            return Buffer;
        }
        catch (Exception)
        {
            Buffer = new byte[(int)0];
            return Buffer;
        }

    }
    #endregion

    #region GetAttachmentFilePath
    [WebMethod]
    public DataSet GetAttachmentFile(int AttachmentID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "select attchmnt_file_path as FilePath,attchmnt_name as [FileName] " +
                "from email_attachement_details where attchmnt_id = " + AttachmentID;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "MailAttachment";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetAttachmentFile", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetAttachmentFile", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetAttachment Files for Email
    [WebMethod]
    public DataSet GetEmailAttachFile(int nMailNumber, string strMailType)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");


            strTemp = "SELECT attchmnt_id AS ID, attchmnt_file_path AS FilePath,attchmnt_name AS 'FileName' " +
                                "FROM email_attachement_details WHERE attchmnt_mail_number = " + nMailNumber +
                                " AND attchmnt_name IS NOT NULL";
            //if (strMailType != "T")
            //{
            //    if (m_Connection.nDatabaseType == 2)
            //        strTemp += " ORDER BY attchmnt_id LIMIT 2, 100";
            //    else
            //        strTemp += " AND attchmnt_id NOT IN (SELECT TOP 2 attchmnt_id FROM email_attachement_details WHERE attchmnt_mail_number = " + nMailNumber + ") " +
            //                                "ORDER BY attchmnt_id";
            //}

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "MailAttachment";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetEmailAttachFile(", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetEmailAttachFile(", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Get Distributor List
    [WebMethod]
    public DataSet FetchDistributorList()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT id,  (contact_full_name + ' [' + related_to_name + ']') AS contact_full_name FROM crm_contacts WHERE contact_type_id = 'D' AND contact_enabled = 'Y' " +
                      "ORDER BY contact_fname";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Distributor";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchGeneralValues", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchGeneralValues", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region  FetchDoctorList()
    [WebMethod]
    public DataSet FetchDoctorList(int AccountID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT id, contact_full_name, (contact_title + ' ' + contact_full_name) as DoctorName FROM crm_contacts " +
                      "WHERE contact_title = 'Dr.' AND contact_enabled = 'Y' AND related_to = 'CST' " +
                      "AND related_to_id = " + AccountID;

            strTemp += " ORDER BY contact_full_name";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "Doctors";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchDoctorList", "");
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchDoctorList", "");
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Check Inventory Serial Number Duplicacy
    [WebMethod]
    public DataSet CheckDuplicateSerialNo(string strSerialNo)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT COUNT(*) AS nCount FROM crm_inventories " +
                "WHERE inv_serial_no = '" + strSerialNo + "'";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());

            if (nRowCount >= 0)
            {
                nResultCode = nRowCount;
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Insert Cancelled Appointment
    [WebMethod]
    public DataSet InsertCancelledAppointment(int TaskID, string strMode)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "insert into crm_appointment_cancel select *, '" + strMode + "' from crm_tasks" +
                        " WHERE id = " + TaskID;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (strMode == "C")
            {
                strTemp = "UPDATE crm_tasks SET udf_schd_contact_id = null, udf_schd_customer_id = null, udf_schd_start_time = null, " +
                          "udf_schd_end_time = null, udf_schd_description = null, udf_schd_status = null, udf_schd_account_id = null" +
                           " WHERE id = " + TaskID;

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                nRowCount = m_CommandODBC.ExecuteNonQuery();
            }

            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Insert Attachment File
    [WebMethod]
    public DataSet InsertAttachments(int MailNumber, string strInOut, string strName, string strFileName, string strContentType, string strContentSubType, long Size, string strDesc, string strFilePath)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            //strTemp = "DELETE FROM email_attachement_details " +
            //           "WHERE attchmnt_mail_number = " + MailNumber;

            //m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            //int nRowCount = m_CommandODBC.ExecuteNonQuery();

            strTemp = "INSERT INTO email_attachement_details(attchmnt_mail_number, attchmnt_in_out, attchmnt_name, attchmnt_file_name," +
                        "attchmnt_content_type, attchmnt_content_subtype, attchmnt_size, attchmnt_description, attchmnt_file_path) " +
                        "VALUES(" + MailNumber + ",?,?,?,?,?," + Size + ",?,?)";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.Add("@strInOut", OdbcType.VarChar).Value = strInOut;
            m_CommandODBC.Parameters.Add("@strName", OdbcType.VarChar).Value = strName;
            m_CommandODBC.Parameters.Add("@strFileName", OdbcType.VarChar).Value = strFilePath; //strFileName
            m_CommandODBC.Parameters.Add("@strContentType", OdbcType.VarChar).Value = strContentType;
            m_CommandODBC.Parameters.Add("@strContentSubType", OdbcType.VarChar).Value = strContentSubType;
            m_CommandODBC.Parameters.Add("@strDesc", OdbcType.VarChar).Value = strDesc;
            m_CommandODBC.Parameters.Add("@strFilePath", OdbcType.VarChar).Value = strFilePath;

            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Insert Attachment File for Forward Mails
    [WebMethod]
    public DataSet InsertAttachmentsForForwardMails(string OldMailNumber, string NewMailNumber, string OldMailType)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "INSERT INTO email_attachement_details(attchmnt_mail_number," +
                        "attchmnt_in_out," +
                        "attchmnt_name," +
                        "attchmnt_file_name," +
                        "attchmnt_content_type," +
                        "attchmnt_content_subtype," +
                        "attchmnt_size," +
                        "attchmnt_description) " +
                        "SELECT " + NewMailNumber + "," +
                        "attchmnt_in_out," +
                        "attchmnt_name," +
                        "attchmnt_file_path," +
                        "attchmnt_content_type," +
                        "attchmnt_content_subtype," +
                        "attchmnt_size," +
                        "attchmnt_description " +
                        "FROM email_attachement_details WHERE attchmnt_mail_number = " + OldMailNumber + " " +
                        "AND attchmnt_name IS NOT NULL ORDER BY attchmnt_id";
            //if (OldMailType == "R")
            //    strTemp += "AND attchmnt_id NOT IN (SELECT TOP 2 attchmnt_id FROM email_attachement_details WHERE attchmnt_mail_number = " + OldMailNumber + ") ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);

            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Delete Attachment File
    [WebMethod]
    public DataSet DeleteAttachment(int Id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "UPDATE email_mails SET mail_attachments = mail_attachments-1 " +
                       "WHERE mail_number = (SELECT attchmnt_mail_number FROM email_attachement_details WHERE attchmnt_id = " + Id + ")";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);

            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            strTemp = "DELETE FROM email_attachement_details " +
                       "WHERE attchmnt_id = " + Id;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);

            nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  FetchContactSignature()
    [WebMethod]
    public DataSet FetchContactSignature(int ContactID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(contact_signature,'') as Signature FROM crm_contacts WHERE id = " + ContactID;
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "Signature";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchContactSignature", "");
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchContactSignature", "");
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Delete Data()
    [WebMethod]
    public DataSet DeleteRecordsfromDatabase(string ObjectType, string ObjectID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " object_table_name,object_permission_activity_id " +
                "FROM crm_tables WHERE object_code = '" + ObjectType + "' " + m_Connection.DB_TOP_MYSQL;
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable dtTable = new DataTable("TableDefinition");
            m_DataAdapterOdbc.Fill(dtTable);

            if (dtTable.Rows.Count <= 0 || dtTable.Rows[0]["object_table_name"].ToString() == "")
            {
                strResult = "Object definition not found for " + ObjectType;
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            string ObjectTarget = Convert.ToString(dtTable.Rows[0]["object_table_name"]);

            strTemp = "SELECT " + m_Connection.DB_FUNCTION + "GetRelatedName('" + ObjectType + "',id) as transaction_number " +
                        "FROM " + ObjectTarget + " WHERE id IN(" + ObjectID + ") AND " + m_Connection.DB_FUNCTION +
                        "GetDeleteStatus('" + ObjectType + "',id) = 'N'";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "NotDeleted";

            strTemp = "DELETE FROM " + ObjectTarget + " WHERE id IN(" + ObjectID + ") AND " + m_Connection.DB_FUNCTION +
                      "GetDeleteStatus('" + ObjectType + "',id) = 'Y'";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);

            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount >= 0)
            {
                nResultCode = 0;
                strResult = "Pass";
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "DeleteRecordsfromDatabase", "");
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "DeleteRecordsfromDatabase", "");
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion



    #region  Delete Data()
    [WebMethod]
    public DataSet DeleteRecordsEmail(string ObjectType, int ObjectID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " object_table_name,object_permission_activity_id " +
                "FROM crm_tables WHERE object_code = '" + ObjectType + "' " + m_Connection.DB_TOP_MYSQL;
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable dtTable = new DataTable("TableDefinition");
            m_DataAdapterOdbc.Fill(dtTable);

            if (dtTable.Rows.Count <= 0 || dtTable.Rows[0]["object_table_name"].ToString() == "")
            {
                strResult = "Object definition not found for " + ObjectType;
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            string ObjectTarget = Convert.ToString(dtTable.Rows[0]["object_table_name"]);

            //strTemp = "SELECT " + m_Connection.DB_FUNCTION + "GetRelatedName('" + ObjectType + "',id) as transaction_number " +
            //            "FROM " + ObjectTarget + " WHERE id IN(" + ObjectID + ") AND " + m_Connection.DB_FUNCTION +
            //            "GetDeleteStatus('" + ObjectType + "',id) = 'N'";
            //m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            //m_DataAdapterOdbc.Fill(m_DataSet);

            //m_DataSet.Tables[0].TableName = "NotDeleted";

            strTemp = "DELETE FROM " + ObjectTarget + " WHERE id =" + ObjectID;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);

            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount >= 0)
            {
                nResultCode = 0;
                strResult = "Pass";
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "DeleteRecordsfromDatabase", "");
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "DeleteRecordsfromDatabase", "");
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Check Inventory Serial Number Duplicacy in Non FMC Inventories
    [WebMethod]
    public DataSet CheckNonFMCDuplicateSerialNo(string strSerialNo)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT COUNT(*) AS nCount FROM crm_external_inventories " +
                "WHERE inv_serial_no = '" + strSerialNo + "'";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());

            if (nRowCount >= 0)
            {
                nResultCode = nRowCount;
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetCaseInventories
    [WebMethod]
    public DataSet GetAccountInventoriesServiceReport(int AccountID, int CaseID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select inv_serial_no, id from crm_inventories where related_to_id = " + AccountID;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "AccountInventories";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetAccountInventories", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetAccountInventories", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Insert Attachment File for Notification
    [WebMethod]
    public DataSet InsertAttachmentsForNotification(string FileName, string FilePath, string ObjectType, int ObjectId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT COUNT(*) FROM crm_email_attachments " +
                      "WHERE ObjectType = '" + ObjectType + "' AND ObjectId = " + ObjectId;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = Convert.ToInt32(m_CommandODBC.ExecuteScalar());

            if (nRowCount <= 0)
            {

                strTemp = "INSERT INTO crm_email_attachments(ObjectType," +
                            "ObjectId," +
                            "AttachmentName," +
                            "AttachmentPath, " +
                            "CreateFile) " +
                            "VALUES('" + ObjectType + "'," + ObjectId + ",'" + FileName + "','" + FilePath + "','Y')";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                nRowCount = m_CommandODBC.ExecuteNonQuery();
            }
            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Getting All Mailbox
    [WebMethod]
    public DataSet FetchMailbox(int RelatedTo)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        strParameters = "nSessionID";
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT DISTINCT " + m_Connection.DB_NULL + "(mailbox_name,'') as mailbox_name, " +
                      m_Connection.DB_NULL + "(id,0) as mailbox_id " +
                      "FROM email_mailboxes " +
                      "WHERE id IN (SELECT map_mailbox_id FROM crm_mappings WHERE map_to_contact_id = " + RelatedTo + " " +
                      "OR map_to_contact_id = (SELECT queue_id FROM crm_queue_members WHERE queue_member_contact_id = " + RelatedTo + ")) " +
                      "ORDER BY mailbox_name";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataSet = new DataSet();
            m_DataAdapterOdbc.Fill(m_DataSet);
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCRMStatus", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCRMStatus", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    //5-1-13 manju
    #region add next note Time in case
    [WebMethod]
    public DataSet addStatus_case(string Caseid, string case_next_status_time)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "UPDATE [crm_cases]  SET [next_status_time] = '" + case_next_status_time + "' WHERE id=" + Caseid;
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.ExecuteNonQuery();
            nResultCode = 0;
            strResult = "pass";
        }

        catch (OdbcException ex)
        {
            m_Connection.Rollback();
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }

        catch (Exception ex)
        {
            m_Connection.Rollback();
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
    
    #region GetAllaccount
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public DataSet FetchAllaccount()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            strTemp = "Select id,cust_name ,(case when cust_name2='' OR cust_name2 = 'NULL'  then 'NA' else cust_name2 end)AS cust_name2,cust_short_name from crm_accounts where cust_enabled='Y' and Useable ='Y' and cust_name !='' order by cust_name";


            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_accounts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_accounts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_accounts", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetAllcontact
    [WebMethod]
    public DataSet Fetchallcontact()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            strTemp = "select CAST(id AS VARCHAR(20))+'|'+CASE WHEN contact_phone = '' THEN 'NA' ELSE contact_phone END  +'|'+CASE WHEN contact_emailid = '' THEN 'NA' ELSE contact_emailid END as contactinfo,contact_full_name from crm_contacts where contact_enabled='Y' and Useable ='Y' and contact_type_id !='E' and contact_type_id !='Q' and contact_full_name !=''"; 

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_contacts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Get All contact Filter by Account Id
    [WebMethod]
    public DataSet Fetchcontact(int AccountId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            strTemp = "select id,contact_full_name from crm_contacts where related_to_id = "+AccountId+
                " and contact_enabled='Y' and Useable ='Y' and contact_type_id !='E' and contact_type_id !='Q' and contact_full_name !=''";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_contacts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetAllLocation
    //[WebMethod]
    //public DataSet FetchallLocation()
    //{
    //    int nResultCode = -1;
    //    string strResult = "Fail - ";
    //    DataSet m_DataSet = new DataSet();
    //    try
    //    {

    //        m_Connection.OpenDB("Galaxy");
    //        strTemp = "select id as LocationId, cust_name from crm_accounts where cust_short_name='teckinfo'";


    //        m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
    //        m_DataAdapterOdbc.Fill(m_DataSet);
    //        m_DataSet.Tables[0].TableName = "crm_accounts";
    //        nResultCode = 0;
    //        strResult = "Pass";
    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "crm_accounts", strParameters);
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "crm_accounts", strParameters);
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //    }
    //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
    //    return m_DataSet;

    //}
    #endregion
    #region GetTeam
    [WebMethod]
    public DataSet FetchTeam(int locationid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            strTemp = "select accteam_id as accountteamid,accteam_name from crm_account_teams where accteam_account_id=" + locationid;


            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_account_teams";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_account_teams", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_account_teams", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion
    #region Getowner
    [WebMethod]
    public DataSet Fetchowner(int locationid, int accountteamid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            if (locationid == 0 && accountteamid == 0)
            {
                strTemp = "select Isnull(CAST(id AS VARCHAR(10)),'0') +','+isnull(CAST(related_to_id AS VARCHAR(10)),'0') as id, contact_full_name from crm_contacts where id in(select contact_id from crm_team_members)";
            }
            else if (locationid != 0 && accountteamid != 0)
            {
                strTemp = "select Isnull(CAST(id AS VARCHAR(10)),'0') +','+isnull(CAST(related_to_id AS VARCHAR(10)),'0') as id, contact_full_name from crm_contacts where id in(select contact_id from crm_team_members where account_id = " + locationid + " and accteam_id = " + accountteamid + ")";
            }
            else
            {
                strTemp = "select Isnull(CAST(id AS VARCHAR(10)),'0') +','+isnull(CAST(related_to_id AS VARCHAR(10)),'0') as id, contact_full_name from crm_contacts where id in(select contact_id from crm_team_members where account_id = " + locationid + " or accteam_id = " + accountteamid + ")";
            }

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_contacts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Getowner only
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,UseHttpGet=true)]
    public DataSet FetchownerLocationWise(int locationid, int accountteamid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            if (locationid == 0 && accountteamid == 0)
            {
                strTemp = "select Isnull(id,0) as id, contact_full_name from crm_contacts where id in(select contact_id from crm_team_members)";
            }
            else if (locationid != 0 && accountteamid != 0)
            {
                strTemp = "select Isnull(id,0) as id, contact_full_name from crm_contacts where id in(select contact_id from crm_team_members where account_id = " + locationid + " and accteam_id = " + accountteamid + ")";
            }
            else
            {
                strTemp = "select Isnull(id,0) as id, contact_full_name from crm_contacts where id in(select contact_id from crm_team_members where account_id = " + locationid + " or accteam_id = " + accountteamid + ")";
            }

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_contacts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetspecLocation
    [WebMethod]
    public DataSet get_teammembers(int account_id, int contact_id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");

            strTemp = "select accteam_id from crm_team_members where account_id = " + account_id + " and contact_id =" + contact_id;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_contacts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    //
    #region role type
    [WebMethod]
    public DataSet roletype(int roleid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT [dbo].[adminrole] (" + roleid + ")";


            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "RoleType";


            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "RoleType", strParameters);
        }

        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Filter view id
    [WebMethod]
    public DataSet FetchViewName(string relatedto)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT view_id ,ISNULL(view_name,'') as view_name  FROM CRM_Views  where related_to='" + relatedto + "'";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "viewname";
            strTemp = "select  " + m_Connection.DB_NULL + "(tabledef_column_header, '') as column_name ,tabledef_id as tabledef_id from crm_table_columns where tabledef_for='" + relatedto + "'and tabledef_column_visible = 'Y' ORDER BY tabledef_column_sequence";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[1].TableName = "FetchViewColumn";


            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ViewRelatedTo", strParameters);
        }

        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion


    //#region  Getting All Reporting to column
    //[WebMethod]
    //public DataSet ReportingTo()
    //{
    //    int nResultCode = -1;
    //    string strResult = "Fail - ";
    //    DataSet m_DataSet = new DataSet();
    //    m_DataAdapterOdbc = null;

    //    try
    //    {
    //        m_Connection.OpenDB("Galaxy");


    //        strTemp = "SELECT " + m_Connection.DB_NULL + "(crm_contacts.id,0) as id," +
    //                  m_Connection.DB_NULL + "(crm_contacts.contact_full_name,'') as name" +
    //                  " from crm_contacts inner join crm_accounts on crm_contacts.related_to_id=crm_accounts.id where crm_contacts. Useable ='Y' and crm_accounts.cust_short_name='Teckinfo' order by ltrim(crm_contacts.contact_full_name)";


    //        m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
    //        m_DataAdapterOdbc.Fill(m_DataSet);
    //        m_DataSet.Tables[0].TableName = "ReportingToColumn";
    //        nResultCode = 0;
    //        strResult = "Pass";
    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "ReportingToColumn", strParameters);
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "ReportingToColumn", strParameters);
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //    }
    //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
    //    return m_DataSet;
    //}
    //#endregion

    #region  Getting All Reporting to column
    [WebMethod]
    public DataSet ReportingToByAccount(int accountId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT  " + m_Connection.DB_NULL + "(crm_contacts.id,0) as id," + m_Connection.DB_NULL + "(crm_contacts.contact_full_name,'')+'('+ RIGHT('0000'+CAST(crm_contacts.id as varchar(4)),4)+')' as name from crm_contacts inner join crm_accounts on crm_contacts.related_to_id=crm_accounts.id where crm_contacts. Useable ='Y' and crm_accounts.cust_enabled='Y'  and crm_contacts.contact_enabled='Y' and crm_contacts.related_to_id=" + accountId + "  order by ltrim(crm_contacts.contact_full_name)";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "ReportingToColumn";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ReportingToColumn", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ReportingToColumn", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region  Getting All ReportToID
    [WebMethod]
    public DataSet GetContactReportingToID(string Contact_id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT cust_reportingtocolumn as ReportToID from crm_contacts WHERE ID= " + Contact_id;
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "ReportingToColumn";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ReportToID", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ReportToID", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public List<string> prefixText(string prefixText)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        List<string> CountryNames = new List<string>();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select transaction_number from crm_cases where transaction_number  like '" + prefixText + "%'";


            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable dt = new DataTable();
            m_DataAdapterOdbc.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CountryNames.Add(dt.Rows[i][0].ToString());
            }

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ReportingToColumn", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ReportingToColumn", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        //return m_DataSet;
        return CountryNames;
    }
    #endregion

    //#region  Getting EmployeeODStatus
    //[WebMethod]
    //public DataSet EmployeeODStatus()
    //{
    //    int nResultCode = -1;
    //    string strResult = "Fail - ";
    //    DataSet m_DataSet = new DataSet();
    //    m_DataAdapterOdbc = null;

    //    try
    //    {
    //        m_Connection.OpenDB("Galaxy");
    //        strTemp = "SELECT " + m_Connection.DB_NULL + "(crm_contacts.id,0) as id," +
    //                  m_Connection.DB_NULL + "(crm_contacts.contact_full_name,'') as name" +
    //                  " from crm_contacts inner join crm_accounts on crm_contacts.related_to_id=crm_accounts.id where crm_contacts. Useable ='Y' and crm_accounts.cust_short_name='Teckinfo'";


    //        m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
    //        m_DataAdapterOdbc.Fill(m_DataSet);
    //        m_DataSet.Tables[0].TableName = "EmployeeODStatus";
    //        nResultCode = 0;
    //        strResult = "Pass";
    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "EmployeeODStatus", strParameters);
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "EmployeeODStatus", strParameters);
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //    }
    //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
    //    return m_DataSet;
    //}
    //#endregion


    #region  Getting All EmployeeName Reporting To
    [WebMethod]
    public DataSet
        GetEmployeeNameReportingTo(int id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");


            strTemp = "SELECT " + m_Connection.DB_NULL + "(crm_contacts.id,0) as id," +
                      m_Connection.DB_NULL + "(crm_contacts.contact_full_name,'') +'('+ RIGHT('0000'+CAST(id as varchar(4)),4)+')' as name" +
                      " from crm_contacts where Useable ='Y'AND contact_enabled = 'Y' and cust_reportingtocolumn = " + id + " order by crm_contacts.contact_full_name";


            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "ReportingToColumn";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ReportingToColumn", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ReportingToColumn", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion


    #region  Getting All EmployeeName Reporting To HR
    [WebMethod]
    public DataSet GetEmployeeNameReportingToHR()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;


        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT " + m_Connection.DB_NULL + "(crm_contacts.id,0) as id," +
                      m_Connection.DB_NULL + "(crm_contacts.contact_full_name,'') +'('+ RIGHT('0000'+CAST(id as varchar(4)),4)+')' as name" +
                      " from crm_contacts where Useable ='Y' AND contact_enabled = 'Y' ORDER BY contact_full_name ";


            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "ReportingToHRColumn";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ReportingToHRColumn", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ReportingToHRColumn", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    //JP Maurya

    #region  Get All Types By Type
    [WebMethod]
    public DataSet FetchCaseTypesById(string type)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(type_id,0) as type_id," +
                     m_Connection.DB_NULL + "(type_name,'') as type_name," +
                     m_Connection.DB_NULL + "(type_reference_field_caption,'') as type_reference_field_caption," +
                     m_Connection.DB_NULL + "(type_service_reference_caption,'') as type_service_reference_caption " +
                     "FROM CRM_Types " +
                     "WHERE  type_enabled = 'Y' " +
                     "AND type_for = '" + type + "' " +
                     "ORDER BY type_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Types";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseTypes", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseTypes", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion


    #region  Get All Types By Type
    [WebMethod]
    public DataSet FetchTicketTypes()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(type_id,0) as type_id," +
                     m_Connection.DB_NULL + "(type_name,'') as type_name " +
                     "FROM CRM_Types " +
                     "WHERE  type_enabled = 'Y' " +
                     "AND type_for = 'C' " +
                     "AND SLA_Required='Yes' "+
                     "ORDER BY type_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Types";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseTypes", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseTypes", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    //#region GetAllaccount filter by short name
    //[WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
    //public DataSet Fetchaccountbyshort()
    //{
    //    int nResultCode = -1;
    //    string strResult = "Fail - ";
    //    DataSet m_DataSet = new DataSet();
    //    try
    //    {

    //        m_Connection.OpenDB("Galaxy");
    //        strTemp = "Select id,cust_name from crm_accounts where cust_enabled='Y' and Useable ='Y' and cust_name !='' and cust_short_name='Teckinfo' order by cust_name";


    //        m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
    //        m_DataAdapterOdbc.Fill(m_DataSet);
    //        m_DataSet.Tables[0].TableName = "crm_accounts";
    //        nResultCode = 0;
    //        strResult = "Pass";
    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "crm_accounts", strParameters);
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "crm_accounts", strParameters);
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //    }
    //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
    //    return m_DataSet;

    //}
    //#endregion

    //#region  Getting All Reporting with Id to column
    //[WebMethod]
    //public DataSet ReportingToWithID()
    //{
    //    int nResultCode = -1;
    //    string strResult = "Fail - ";
    //    DataSet m_DataSet = new DataSet();
    //    m_DataAdapterOdbc = null;

    //    try
    //    {
    //        m_Connection.OpenDB("Galaxy");


    //        strTemp = "SELECT " + m_Connection.DB_NULL + "(crm_contacts.id,0) as id," +
    //                  m_Connection.DB_NULL + "(crm_contacts.contact_full_name,'')+'('+ RIGHT('0000'+CAST(crm_contacts.id as varchar(4)),4)+')' as name" +
    //                  " from crm_contacts inner join crm_accounts on crm_contacts.related_to_id=crm_accounts.id where  crm_contacts. Useable ='Y' and crm_contacts.contact_enabled='Y'"+
    //                  " and crm_accounts.cust_short_name='Teckinfo' order by ltrim(crm_contacts.contact_full_name)";


    //        m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
    //        m_DataAdapterOdbc.Fill(m_DataSet);
    //        m_DataSet.Tables[0].TableName = "ReportingToColumn";
    //        nResultCode = 0;
    //        strResult = "Pass";
    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "ReportingToColumn", strParameters);
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "ReportingToColumn", strParameters);
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //    }
    //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
    //    return m_DataSet;
    //}
    //#endregion
    //end

    #region Get Search Open case summary Owner Wise
    [WebMethod]
    public DataSet GetSearchOpenCaseSummary1(int nLoginContactID,string strCallType,int nLoginAccountID,string strLoginAccType,int nLoginDeptID,int nLoginDesigLevel,string type,bool bApplyOwnerFilter)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("RatingSheet");
        string szFilter = "";
        string strTempFirst = "";
        string strTempSecond = "";
        string strTempThird = "";
        try
        {
           
          
            if (bApplyOwnerFilter == true)
            {
                DataSet dsDataSet = CreateFilterOwnerBy(nLoginContactID, strCallType, nLoginAccountID, strLoginAccType, nLoginDeptID, nLoginDesigLevel);
                if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
                    return dsDataSet;

                if (dsDataSet.Tables["Filter"].Rows.Count > 0)
                    szFilter = dsDataSet.Tables["Filter"].Rows[0]["FilterString"].ToString();
            }
            m_Connection.OpenDB("Galaxy");
            // strTemp = "set nocount on ;With T as (SELECT owner_id,isnull([OPEN],0) openCase, isnull([OPEN WORKING],0) WorkingCase, isnull([PENDING FOR FEEDBACK],0) FeedbackCase FROM (SELECT  isnull(owner_id,0) owner_id,[dbo].[GetStatusName]('CAS',case_status_id) AS CaseStatus,count(*) as CaseCount FROM crm_cases WHERE Useable = 'Y' AND  case_status_id in (1,3,7)  AND (owner_id in (129,130,192,188,123) OR created_by in (129,130,192,188,123)  OR case_customer_id in (select map_account_id from crm_mappings where map_to_contact_id = 123)) group by owner_id,case_status_id ) p PIVOT (sum(CaseCount) FOR CaseStatus IN ([OPEN], [OPEN WORKING], [PENDING FOR FEEDBACK]) ) AS pvt) select case when dbo.GetAccountName(crm_contacts.related_to_id)='' then 'Not Available' else dbo.GetAccountName(crm_contacts.related_to_id)end AccountName ,case when dbo.GetTeamName(crm_contacts.contact_team_id)='' then 'Not Available' else dbo.GetTeamName(crm_contacts.contact_team_id) end TeamName ,isnull(contact_full_name,'Not Available') contactName,T.openCase,T.WorkingCase,T.FeedbackCase,T.openCase+T.WorkingCase+T.FeedbackCase as Total, T.owner_id,0 location_id from T left outer join crm_contacts on t.owner_id=id order by AccountName,TeamName,contactName ";

            if (type == "O")
            {
                strTempFirst = "dbo.GetAccountName(dbo.GetContactAccountID(owner_id)) as AccountName,dbo.GetTeamName(dbo.GetContactTeamID(owner_id)) TeamName,dbo.GetContactName(owner_id)contactName,owner_id,0 location_id";
                strTempSecond = " owner_id ";
                strTempThird = " group by owner_id,case_status_id ";
            }
            else if (type == "T")
            {
                strTempFirst = "dbo.GetAccountName(dbo.GetTeamAccountID(Team_Id)) as AccountName,'' TeamName,dbo.GetTeamName(Team_Id) contactName,Team_Id owner_id,0 location_id ";
                strTempSecond = " dbo.GetContactTeamID(COALESCE(owner_id,created_by)) Team_Id ";
                strTempThird = " group by dbo.GetContactTeamID(COALESCE(owner_id,created_by)),case_status_id ";
            }
            else
            {
                strTempFirst = "dbo.GetAccountName(Location_Id) as AccountName,'' TeamName,dbo.GetAccountName(case_customer_id) contactName, case_customer_id owner_id, location_id";
                strTempSecond = " dbo.GetContactAccountID(owner_id) Location_Id,case_customer_id ";
                strTempThird = " and isnull(case_customer_id,0)>0 group by dbo.GetContactAccountID(owner_id),case_customer_id,case_status_id";
            }
            
            //if (type == "O")
            //    strTemp += " owner_id ";
            //else if (type == "T")
            //    strTemp += " dbo.GetContactTeamID(owner_id) Team_Id ";
            //else
            //    strTemp += " dbo.GetContactAccountID(owner_id) Location_Id,case_customer_id ";
            
            //strTemp += ",[dbo].[GetStatusName]('CAS',case_status_id) AS CaseStatus,count(*) as CaseCount FROM crm_cases WHERE Useable = 'Y' AND  case_status_id in (1,3,7)";
            //strTemp += szFilter;

            strTemp = "SELECT " + strTempFirst + ",isnull([OPEN],0) openCase, isnull([OPEN WORKING],0) WorkingCase, isnull([PENDING FOR FEEDBACK],0) FeedbackCase ,isnull([OPEN],0)+ isnull([OPEN WORKING],0) + isnull([PENDING FOR FEEDBACK],0) Total " +
               "FROM(SELECT  " + strTempSecond + ",[dbo].[GetStatusName]('CAS',case_status_id) AS CaseStatus,count(*) as CaseCount FROM crm_cases WHERE Useable = 'Y' AND  case_status_id in (1,3,7)"+
               szFilter + strTempThird + ") p      PIVOT      (      sum(CaseCount)      FOR CaseStatus IN ([OPEN], [OPEN WORKING], [PENDING FOR FEEDBACK])) AS pvt order by AccountName,TeamName,contactName";  
            
            //if (type == "O")
            //    strTemp += " group by owner_id,case_status_id ";
            //else if (type == "T")
            //    strTemp += " group by dbo.GetContactTeamID(owner_id),case_status_id ";
            //else
            //    strTemp += " and isnull(case_customer_id,0)>0 group by dbo.GetContactAccountID(owner_id),case_customer_id,case_status_id";
            
            //strTemp += ") p      PIVOT      (      sum(CaseCount)      FOR CaseStatus IN ([OPEN], [OPEN WORKING], [PENDING FOR FEEDBACK])) AS pvt order by AccountName,TeamName,contactName";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            //m_DataAdapter.SelectCommand = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
           // m_DataSet.Tables[0].TableName = "CategoriesRating";
            //m_DataSet.Tables.Add(m_DataTable);

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

    // test case genreal pages method by ashish

    #region FetchCustomercontact
    [WebMethod]
    public DataSet FetchCustomercontact(string ContactID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            strTemp = "select CAST(id AS VARCHAR(20))+'|'+CASE WHEN contact_phone = '' THEN 'NA' ELSE contact_phone END  +'|'+CASE WHEN contact_emailid = '' THEN 'NA' ELSE contact_emailid END as contactinfo,contact_full_name from crm_contacts where contact_enabled='Y' and Useable ='Y' and contact_type_id !='E' and contact_type_id !='Q' and contact_full_name !='' AND related_to_id =" + ContactID.ToString();



            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_contacts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion


    #region on the basis login account type and Team and its level
    [WebMethod]
    public DataSet CheckReportingToByOwnerID(int nOwnerId,int nReportingToId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
      
        string szTemp = string.Empty;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT top 1 id FROM crm_contacts WHERE id=" + nOwnerId.ToString() + " AND " + m_Connection.DB_NULL + "(cust_reportingtocolumn,0)=" + nReportingToId.ToString();
         
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable dtTable = new DataTable();
            m_DataAdapterOdbc.Fill(dtTable);
            if (dtTable.Rows.Count == 0)
            {
                strTemp = "SELECT Top 1 crm_team_members.accteam_id id FROM crm_team_members INNER JOIN (SELECT "+
                    m_Connection.DB_NULL+"(accteam_id, 0) AS accteam_id, "+m_Connection.DB_NULL+"(contact_level, 0) AS contact_level,"+
                    m_Connection.DB_NULL+" (account_id, 0) AS account_id FROM  crm_team_members WHERE (contact_id = "+nOwnerId.ToString()+
                    ")) AS team ON "+m_Connection.DB_NULL+"(crm_team_members.contact_level, 0) > team.contact_level AND "+m_Connection.DB_NULL+
                    "(crm_team_members.accteam_id, 0) = team.accteam_id AND "+m_Connection.DB_NULL+"(crm_team_members.account_id, 0) = team.account_id "+
                    "WHERE(crm_team_members.contact_id = "+nReportingToId.ToString()+")";

                m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                m_DataAdapterOdbc.Fill(dtTable);
            }
          
            nResultCode = 0;
            strResult = "Pass";

            m_DataSet.Tables.Add(dtTable);
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        }
        catch (Exception ex)
        {
            nResultCode = -1; 
            strResult = ex.Message;
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        }
        finally
        {
            m_Connection.CloseDB();
        }
        return m_DataSet;
    }
    #endregion

    //#region  Getting All Reporting with Id to column
    //[WebMethod]
    //public List<Contactslist> AllContact()
    //{
    //    List<Contactslist> contactlist = new List<Contactslist>();

    //    int nResultCode = -1;
    //    string strResult = "Fail - ";
    //    DataSet m_DataSet = new DataSet();
    //    m_DataAdapterOdbc = null;

    //    try
    //    {
    //        m_Connection.OpenDB("Galaxy");


    //        strTemp = "SELECT " + m_Connection.DB_NULL + "(crm_contacts.id,0) as id," +
    //                  m_Connection.DB_NULL + "(crm_contacts.contact_full_name,'')+'('+ RIGHT('0000'+CAST(crm_contacts.id as varchar(4)),4)+')' as name" +
    //                  " from crm_contacts inner join crm_accounts on crm_contacts.related_to_id=crm_accounts.id where  crm_contacts. Useable ='Y' and crm_contacts.contact_enabled='Y'" +
    //                  " and crm_accounts.cust_short_name='Teckinfo' order by ltrim(crm_contacts.contact_full_name)";
    //        m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
    //        m_CommandODBC.CommandType = CommandType.Text;
    //         OdbcDataReader obj_result = m_CommandODBC.ExecuteReader();
    //         while (obj_result.Read())
    //         {
    //             Contactslist obj = new Contactslist();
    //             obj.Id = Convert.ToInt32(obj_result["id"]);
    //             obj.contact = obj_result["name"].ToString();
    //             contactlist.Add(obj);
    //         }
    //       // m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
    //      //  m_DataAdapterOdbc.Fill(m_DataSet);
    //      //  m_DataSet.Tables[0].TableName = "ReportingToColumn";
    //        nResultCode = 0;
    //        strResult = "Pass";
    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "ReportingToColumn", strParameters);
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "ReportingToColumn", strParameters);
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //    }
    //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
    //    return contactlist;
    //}
    //#endregion

    // new code 10 oct 2013

    #region Fetch All Teckinfo Location
    [WebMethod]
    public DataSet GetAllCustLocation(string nText)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "Select id,cust_name ,cust_emailid,cust_phone from crm_accounts " +
                          "where cust_name like '" + nText + "%' and cust_enabled='Y' and Useable ='Y' order by cust_name";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_contacts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Fetch All Teckinfo Location,Team,Contact
    [WebMethod]
    public DataSet GetAllCustLocationTeam(string nText)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select convert(varchar(15),account_id)+'|'+convert(varchar(15),accteam_id)+'|'+convert(varchar(15),id) as id,Name,Team,Location,account_id,accteam_id from vw_Location_Team_Owner where Name+' '+Team+' '+Location like '%" + nText + "%' order by Name";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Contact";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Contact", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Contact", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region  Get All Types
    [WebMethod(EnableSession = true)]
    public DataTable GetCaseTypes()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        int RoleId= Convert.ToInt32(HttpContext.Current.Session["role_id"].ToString());
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select " + m_Connection.DB_NULL + "(crm_types.type_id,0) type_id," + m_Connection.DB_NULL + "(crm_types.type_name,'')type_name, " +
                m_Connection.DB_NULL + "(crm_types.type_reference_field_caption,'') as type_reference_field_caption," +
                m_Connection.DB_NULL + "(crm_types.type_service_reference_caption,'') as type_service_reference_caption " +
                "from crm_types,crm_role_case_activity where type_enabled = 'Y' and  type_for='C' and crm_types.type_id=crm_role_case_activity.type_id " +
                "and crm_role_case_activity.role_id=" + RoleId + " ORDER BY type_name ASC";
            //strTemp = "SELECT " + m_Connection.DB_NULL + "(type_id,0) as type_id," +
            //         m_Connection.DB_NULL + "(type_name,'') as type_name," +
            //         m_Connection.DB_NULL + "(type_reference_field_caption,'') as type_reference_field_caption," +
            //         m_Connection.DB_NULL + "(type_service_reference_caption,'') as type_service_reference_caption " +
            //         "FROM CRM_Types " +
            //         "WHERE  type_enabled = 'Y' " +
            //         "AND type_for = 'C' " +
            //         "ORDER BY type_name ASC";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Types";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseTypes", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCaseTypes", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet.Tables["Types"];
    }
    #endregion

    [WebMethod(CacheDuration = 0)]
    public DataTable BindCallerDetails(string strLocationID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "select CAST(id AS VARCHAR(20))+'|'+CASE WHEN contact_phone = '' THEN 'NA' ELSE contact_phone END  +'|'+CASE WHEN contact_emailid = '' THEN 'NA' ELSE contact_emailid END as contactinfo," +
               "contact_full_name from crm_contacts where contact_enabled='Y' and Useable ='Y' and contact_type_id !='E' and contact_type_id !='Q' and contact_full_name !='' and related_to_id =  '" + strLocationID + "' ";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_contacts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet.Tables["crm_contacts"];

    }

    //#region GetAllLocation
    //[WebMethod]
    //public DataTable GetallLocation()
    //{
    //    int nResultCode = -1;
    //    string strResult = "Fail - ";
    //    DataSet m_DataSet = new DataSet();
    //    try
    //    {

    //        m_Connection.OpenDB("Galaxy");

    //        strTemp = "select id as LocationId, cust_name from crm_accounts where cust_short_name='teckinfo'";


    //        m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
    //        m_DataAdapterOdbc.Fill(m_DataSet);
    //        m_DataSet.Tables[0].TableName = "crm_accounts";
    //        nResultCode = 0;
    //        strResult = "Pass";
    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "crm_accounts", strParameters);
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;
    //        LogMessage(strTemp + strResult, "crm_accounts", strParameters);
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //    }
    //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
    //    return m_DataSet.Tables["crm_accounts"];

    //}
    //#endregion

    #region GetTeam
    [WebMethod(CacheDuration = 0)]
    public DataTable GetTeam(int locationid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            strTemp = "select accteam_id as accountteamid,accteam_name from crm_account_teams where accteam_account_id=" + locationid;
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_account_teams";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_account_teams", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_account_teams", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet.Tables["crm_account_teams"];

    }
    #endregion

    #region Getowner
    [WebMethod(CacheDuration = 0)]
    public DataTable Getowner(int locationid, int accountteamid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            if (locationid == 0 && accountteamid == 0)
            {
                strTemp = "select Isnull(CAST(id AS VARCHAR(10)),'0') +','+isnull(CAST(related_to_id AS VARCHAR(10)),'0') as id, contact_full_name from crm_contacts where contact_enabled = 'Y'AND  id in(select contact_id from crm_team_members)";
            }
            else if (locationid != 0 && accountteamid != 0)
            {
                strTemp = "select Isnull(CAST(id AS VARCHAR(10)),'0') +','+isnull(CAST(related_to_id AS VARCHAR(10)),'0') as id, contact_full_name from crm_contacts where contact_enabled = 'Y'AND id in(select contact_id from crm_team_members where account_id = " + locationid + " and accteam_id = " + accountteamid + ")";
            }
            else
            {
                strTemp = "select Isnull(CAST(id AS VARCHAR(10)),'0') +','+isnull(CAST(related_to_id AS VARCHAR(10)),'0') as id, contact_full_name from crm_contacts where contact_enabled = 'Y'AND id in(select contact_id from crm_team_members where account_id = " + locationid + " or accteam_id = " + accountteamid + ")";
            }

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_contacts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_contacts", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet.Tables["crm_contacts"];

    }
    #endregion

    #region for GetCaseDetails()
    [WebMethod]
    public DataSet GetCaseDetails(int ObjectId, int nUserTimeZoneSpan)
    {
        string strColumns = "'ID','CaseNumber','CreatedBy','CreatedTime','AccountId','Account','AccountAddress','ValidityStatus'," +
            "'CallerId','Caller','CallerAddress','CallerEmail','CallerPhone'," +
            "'ContactId','Contact','ContactAddress','ContactEmail','ContactPhone'," +
            "'TicketSource','Source','TicketSourceValue','CallBackNumber'," +

              "'EndTime','OpenTime','Activity','CloseTime','NextStatus','TargetEndTime','ReferenceData'," +
             
              "'IsClosable','NotificationModifyby','Notificationcloser','Description','EndRemarks','ServiceTypeId','SeverityId','Subject'," +
              "'OwnerId','OwnerName','Teamid','Teamname','Locationid','Locationname'," +
              "'AssignId','AssignName','AssignTeamId','AssignTeamName','AssignLocationId','AssignLocationName'," +
              "'TypeId','CaseType','CategoryId','Category','SubCategoryId','SubCategory','StatusId','Status','StatusReason','StatusReasonId'";

      return  FetchTransactionData("CAS", "crm_cases", strColumns, ObjectId, nUserTimeZoneSpan);
    }
    #endregion

    #region for GetLeadDetails()
    [WebMethod]
    public DataSet GetLeadDetails(int ObjectId, int nUserTimeZoneSpan)
    {
        string strColumns = @"'LeadNumber','Account','Address','City','State','MobileNo','LandlineNo','Landline2No','OwnerLocationName','OwnerLocationId ',
                               'OwnerTeamName','OwnerTeamId','CloseTime','NextStatus','LeadDescription','Email','StatusId','StatusReasonId','EndRemarks',
                               'AccountAddress','ID','OwnerName','OwnerId','LeadSource','Team','LeadSourceValue','Status','StatusReason','OpenTime',
                               'TargetEndTime','CreatedBy','CreatedTime','EndTime','Subject','Contact','IsClosable','AccountId','ContactId',
                               'ContactAddress','AssignName','AssignId','AssignLocationId','AssignLocationName','AssignTeamName','AssignTeamId',
                               'ProductInterested','ProductValue'";

        return FetchTransactionData("LED", "crm_leads", strColumns, ObjectId, nUserTimeZoneSpan);
    }
    #endregion


    #region FetchTransactionData
    public DataSet FetchTransactionData(string ObjectType, string ObjectTarget, string strColumns, int ObjectId, int nUserTimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_Connection.OpenDB("Galaxy");
        try
        {
            OdbcDataAdapter m_DataAdapterOdbc;
            string strTemp = "";
            if (ObjectTarget.Length <= 0)
            {
                strTemp = "SELECT" + m_Connection.DB_TOP_SQL + " object_table_name FROM crm_tables where object_code = '" + ObjectType + "' " + m_Connection.DB_TOP_MYSQL;
                OdbcCommand m_OdbcCommand = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                ObjectTarget = m_OdbcCommand.ExecuteScalar().ToString();
            }
            string strQuery = "SELECT " + m_Connection.DB_TOP_SQL + " " + m_Connection.DB_NULL + "(Useable,'') as Useable";
            //--get column details from view_columns
            strTemp = "SELECT " + m_Connection.DB_NULL + "(tabledef_column_name, '') as table_column_name," +
                      "tabledef_column_header " +
                      "FROM crm_table_columns " +
                      "WHERE tabledef_for = '" + ObjectType + "' " +
                      "AND " + m_Connection.DB_NULL + "(tabledef_column_name,'') <> '' ";

            if (strColumns.Length > 0)
            {
                strTemp += "AND tabledef_column_header in (" + strColumns + ")";
            }

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            DataTable dtTableDefinition = new DataTable("TableDefinition");
            m_DataAdapterOdbc.Fill(dtTableDefinition);
            if (dtTableDefinition.Rows.Count == 0)
            {
                nResultCode = -1;
                strResult = "No columns defined in the crm_table_columns";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            foreach (DataRow row in dtTableDefinition.Rows)
            {
                strQuery += "," + row["table_column_name"] + " as " + row["tabledef_column_header"];
            }

            strQuery += " FROM " + ObjectTarget + " WHERE id = " + ObjectId + " " + m_Connection.DB_TOP_MYSQL;
            strQuery = strQuery.Replace("#TIMESPAN#", nUserTimeZoneSpan.ToString());

            m_DataAdapterOdbc = new OdbcDataAdapter(strQuery, m_Connection.oCon);
            DataTable dtTableData = new DataTable("Data");
            m_DataAdapterOdbc.Fill(dtTableData);
            if (dtTableData.Rows.Count > 0)
            {
                nResultCode = 0;
                strResult = "Pass";
                m_DataSet.Tables.Add(dtTableData);
                //strTemp = "UPDATE " + ObjectTarget + " SET last_activity_date=" + DB_UTC_DATE + " WHERE id=" + ObjectId;
                //OdbcCommand m_OdbcCommand = new OdbcCommand(strTemp, oCon, m_TransactionODBC);
                //m_OdbcCommand.ExecuteNonQuery();
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
        }

        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    [WebMethod]
    public DataTable SaveCaseStatus(string CaseStatusid, string CaseStatus, int CaseID, int TB_nContactID)
    {
        XmlDocument xMainDoc = m_Connection.createParameterXML();
        m_Connection.fillParameterXML(ref xMainDoc, "case_status_id", CaseStatusid, "int", "0");
        m_Connection.fillParameterXML(ref xMainDoc, "case_status_name", CaseStatus, "varchar", "100");
        m_Connection.fillParameterXML(ref xMainDoc, "first_open_time", DateTime.UtcNow.ToString("dd MMM yyyy HH:mm"), "datetime", "0");
        m_Connection.OpenDB("Galaxy");
        return m_Connection.SaveTransactionData("CAS", CaseID, "N", DateTime.UtcNow, TB_nContactID, strIPAddress, xMainDoc);
    }
    [WebMethod]
    public DataTable SaveLeadStatus(string LeadStatusid, string LeadStatus, int LeadID, int TB_nContactID)
    {
        XmlDocument xMainDoc = m_Connection.createParameterXML();
        m_Connection.fillParameterXML(ref xMainDoc, "lead_status_id", LeadStatusid, "int", "0");
        m_Connection.fillParameterXML(ref xMainDoc, "lead_status_name", LeadStatus, "varchar", "100");
        m_Connection.fillParameterXML(ref xMainDoc, "first_open_time", DateTime.UtcNow.ToString("dd MMM yyyy HH:mm"), "datetime", "0");
        m_Connection.OpenDB("Galaxy");
        return m_Connection.SaveTransactionData("LED", LeadID, "N", DateTime.UtcNow, TB_nContactID, strIPAddress, xMainDoc);
    }
    [WebMethod]
    public DataTable SaveRecentActivity(int itemid, string ObjectType, int ObjectId, string ObjectAction, string ObjectName, int UserId, string ObjectStatus, int ActivityId, string HistoryText)
    {
        int nResultCode = 0;
        string strResult = "";
        try
        {
            string strQuery = "";
            OdbcCommand m_CommandODBC = null;
            int nRowCount = 0;
            m_Connection.OpenDB("Galaxy");
            if (ObjectStatus == "P")
            {
                strQuery = "UPDATE CRM_RECENT_ITEMS SET recent_item_display_status = 0 " +
                "WHERE recent_item_object_type = '" + ObjectType + "' " +
                "AND recent_item_object_id = " + ObjectId + " " +
                "AND recent_item_contact_id = " + UserId + " " +
                "AND recent_item_id <> " + itemid;
                m_CommandODBC = new OdbcCommand("", m_Connection.oCon, m_Connection.m_TransactionODBC);
                m_CommandODBC = new OdbcCommand(strQuery, m_Connection.oCon, m_Connection.m_TransactionODBC);
                nRowCount = m_CommandODBC.ExecuteNonQuery();
            }

            if (itemid == 0)
                strQuery = "INSERT INTO CRM_RECENT_ITEMS(recent_item_object_type," +
                                "recent_item_object_id," +
                                "recent_item_action," +
                                "recent_item_name," +
                                "recent_item_contact_id," +
                                "recent_item_status," +
                                "recent_item_start_time," +
                                "recent_item_end_time," +
                                "recent_item_activity_id," +
                                "recent_item_history_text) " +
                                "VALUES('" + ObjectType + "'," + ObjectId + ",'" + ObjectAction + "', ?," + UserId + ",'" + ObjectStatus + "'," + m_Connection.DB_UTC_DATE + "," + m_Connection.DB_UTC_DATE + "," + ActivityId + ",'" + HistoryText + "')";
            else
                strQuery = "UPDATE CRM_RECENT_ITEMS SET recent_item_status = '" + ObjectStatus + "'," +
                                "recent_item_end_time = " + m_Connection.DB_UTC_DATE + "," +
                                "recent_item_name = ?," +
                                "recent_item_action = (CASE WHEN recent_item_action = 'New' THEN 'Created' ELSE 'Modified' END), " +
                                "recent_item_history_text = '" + HistoryText + "' " +
                                "WHERE recent_item_id = " + itemid;

            m_CommandODBC = new OdbcCommand("", m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC = new OdbcCommand(strQuery, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.Parameters.Add("recent_item_name", OdbcType.VarChar).Value = ObjectName;
            nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
            {
                nResultCode = 0;
                if (itemid == 0)
                {
                    strQuery = "SELECT @@IDENTITY";
                    m_CommandODBC = new OdbcCommand(strQuery, m_Connection.oCon, m_Connection.m_TransactionODBC);
                    itemid = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
                    nResultCode = itemid;
                }
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
        }
        return m_Connection.GetResponseTable(nResultCode, strResult);
    }

    #region fetch notes
    [WebMethod]
    public DataSet fatchStatus_case(int Caseid)
    {
        int nResultCode = -1;
        string strTemp = "";
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        OdbcDataAdapter m_DataAdapterOdbc;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select top 1 note_desc from crm_notes where related_to='CAS'  and related_to_id=" + Caseid + " order by id desc";

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "case_notes";

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
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region fetch notes
    [WebMethod]
    public DataSet FatchNotes(int ObjectId,string strObject)
    {
        int nResultCode = -1;
        string strTemp = "";
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        OdbcDataAdapter m_DataAdapterOdbc;

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select top 1 note_desc from crm_notes where related_to='" + strObject + "'  and related_to_id=" + ObjectId + " order by id desc";

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "case_notes";

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
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region add next note Time in case/Lead
    [WebMethod]
    public DataSet addStatus_case(string Objectid,string ObjectTyoe, string case_next_status_time)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            if (ObjectTyoe=="CAS")
            strTemp = "UPDATE crm_cases  SET next_status_time = '" + case_next_status_time + "' WHERE id=" + Objectid;
            else
            strTemp = "UPDATE crm_leads  SET next_status_time = '" + case_next_status_time + "' WHERE id=" + Objectid;
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.ExecuteNonQuery();
            nResultCode = 0;
            strResult = "pass";
        }

        catch (OdbcException ex)
        {
            m_Connection.Rollback();
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }

        catch (Exception ex)
        {
            m_Connection.Rollback();
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

    #region ValidityStatusWarantyExpiry_date
    [WebMethod]
    public DataSet ValidityStatusWarantyExpiry_date(string AccountId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT TOP 1 cust_ValidityStatus,convert (varchar ,cust_Expiry_date,106) as cust_Expiry_date from crm_accounts Where id =" + AccountId;
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_accounts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_accounts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_accounts", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetAllaccount
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json,UseHttpGet=true)]
    public DataSet Fetchaccount(string text)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            strTemp = "Select id,cust_name ,(case when cust_name2='' OR cust_name2 = 'NULL'  then 'NA' else cust_name2 end)AS cust_name2,cust_short_name from crm_accounts where cust_enabled='Y' and Useable ='Y' and cust_name !='' and cust_name+' '+ isnull(cust_name2,'')+' ' +isnull(cust_short_name,'') like '%" + text + "%' order by cust_name";


            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_accounts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_accounts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_accounts", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = false)]
    public DataSet FetchaccountFilter(string text)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            strTemp = "Select id,cust_name ,(case when cust_name2='' OR cust_name2 = 'NULL'  then 'NA' else cust_name2 end)AS cust_name2,cust_short_name from crm_accounts where cust_enabled='Y' and Useable ='Y' and cust_name !='' and cust_name+' '+ isnull(cust_name2,'')+' ' +isnull(cust_short_name,'') like '%" + text + "%' order by cust_name";


            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_accounts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_accounts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_accounts", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    // added by ashish on 14-05-2014 for Liceance based Galaxy for Nigeria Location
    //for lie
    [WebMethod]
    public string GetLicenseInfo()
    {
        string strCon = "IDGDB";
        using (OdbcConnection con3 = m_Connection.OpenDB(strCon))
        {
            string strAppCount = string.Empty;
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            OdbcCommand cmd5 = new OdbcCommand();
            OdbcCommand cmdGal = new OdbcCommand();
            try
            {
               //cmd5 = new OdbcCommand("SELECT * from cti_license_information  where app_name = 'Galaxy' ", con3);
                cmd5 = new OdbcCommand("SELECT * from cti_license_information  where app_name = 'PARAM1' ", con3);
                using (OdbcDataAdapter oda = new OdbcDataAdapter(cmd5))
                {
                    oda.Fill(dt);
                    strNoOfLicense = Convert.ToString((dt.Rows[0]["app_license_count"]));
                    strAppCount = Convert.ToString(dt.Rows[0]["app_login_count"]);
                }
            }
            catch (Exception ex)
            {
                string Result = ex.Message;
            }
            finally
            {
                m_Connection.CloseDB();
            }
            return strNoOfLicense + "," + strAppCount;
        }
    }

   

   

    

    [WebMethod]
    public int CounterLogin(string strLoginId)
    {
        string strCon = "GALAXY";
        int nCounterLogin = 0;
        OdbcCommand cmd = null;
        using (OdbcConnection con3 = m_Connection.OpenDB(strCon))
        {
            string strquery = "select count(*) from CRM_User_Sessions where session_active ='Y' and session_login_id<>'" + strLoginId + "'";
            try
            {
                cmd = new OdbcCommand(strquery, con3);
                return nCounterLogin = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch
            {
                return nCounterLogin;
            }
            finally
            {
                m_Connection.CloseDB();
            }
        }
    }

    public int ValidateUserCount(string strLoginId)
    {
        string strCon = "GALAXY";
        int VCount = 0;
        OdbcCommand cmd = null;
        using (OdbcConnection con3 = m_Connection.OpenDB(strCon))
        {
            string strquery = "select count(*) from CRM_User_Sessions where session_active ='Y' and session_login_id='" + strLoginId + "'";
            try
            {
                cmd = new OdbcCommand(strquery, con3);
                return VCount = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch
            {
                return VCount;
            }
            finally
            {
                m_Connection.CloseDB();
            }
        }
    }

    public int RemoveUserSession(string strLoginId)
    {
        string strCon = "GALAXY";
        int nRemoveSession = 0;
        OdbcCommand cmd = null;
        using (OdbcConnection con3 = m_Connection.OpenDB(strCon))
        {

            string strquery = "UPDATE CRM_User_Sessions SET session_active ='N' where session_login_id='" + strLoginId + "'"; //session_start_time <DATEADD(SECOND,-10,GETDATE()) and 
             try
            {
                cmd = new OdbcCommand(strquery, con3);
                return nRemoveSession = cmd.ExecuteNonQuery();
            }
            catch
            {
                return nRemoveSession;
            }
            finally
            {
                m_Connection.CloseDB();
            }
        }
    }

   
    [WebMethod]
    public DataSet FetchSLA(string CategID, string CaseTypeID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = " select" + m_Connection.DB_NULL + "(Categ_SLALevel0,0) as SLA,isnull((select top 1 sla_required from crm_types where type_id=1),'No') SLAType  from crm_categories where categ_id=" + Convert.ToInt32(CategID) + " and categ_case_types like '%" + CaseTypeID + "%' and categ_enabled='Y'";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "Categories";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Categories", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Categories", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }

    #region Fetch All Contact
    [WebMethod]
    public DataSet GetContactList(string id)
    {
        
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "select id, contact_full_name,related_to_name,cust_location, cust_latitute + ',' + cust_longitute as LatLong, cust_req_type, " +
                        "cust_filling_type, cust_filling_contactno, cust_loc_permission,cust_approchaccess, cust_fule_type,"+
                        "cust_fule_qty, cust_paymentmode, cust_service, cust_longitute from crm_contacts  where Useable = 'Y' " +
                        "and contact_enabled = 'Y' and contact_type_id = 'C'and id=" + id;

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "contacts";
        
            nResultCode = 1;
            strResult = "";
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

    private string Decrypt(string cipherText)
    {
        string EncryptionKey = "MAKV2SPBNI99212";
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }


    #region  Getting All Mailbox
    [WebMethod]
    public DataSet FetchALLMailbox()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        strParameters = "nSessionID";
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT DISTINCT " + m_Connection.DB_NULL + "(mailbox_name,'') as mailbox_name,   CAST(id as varchar(10)) +'|'+mailbox_emailid as mailbox ,  " +
                      m_Connection.DB_NULL + "(id,0) as mailbox_id " +
                      "FROM email_mailboxes where " + m_Connection.DB_NULL + "(mailbox_name,'') <> ''  and mailbox_enabled='Y'  ORDER BY mailbox_name";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataSet = new DataSet();
            m_DataAdapterOdbc.Fill(m_DataSet);
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCRMStatus", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchCRMStatus", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion
    #region GetAttachmentPath
    [WebMethod]
    public DataSet GetAttachmentPath(string ID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "select isnull(attchmnt_file_path,'') as attchmnt_file_path from email_attachement_details where attchmnt_content_type='image' and attchmnt_content_id= '<" + ID + ">'";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Path";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Path", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Path", strParameters);
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
    public DataSet FetchSubCategories(string subCategID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
           // strTemp = " select" + m_Connection.DB_NULL + "(Categ_SLALevel0,0) as SLA,isnull((select top 1 sla_required from crm_types where type_id=1),'No') SLAType  from crm_categories where categ_id=" + Convert.ToInt32(CategID) + " and categ_case_types like '%" + CaseTypeID + "%' and categ_enabled='Y'";

            strTemp = "select isnull(Categ_cbnSLA,0) as cbnSLA,categ_code  from crm_categories where categ_id=" + subCategID + " and categ_code<>''";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            m_DataSet.Tables[0].TableName = "SubCategories";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "SubCategories", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "SubCategories", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }

    #region  InsertDriverLocation
    [WebMethod]
    public DataSet InsertDriverLocation(string strDriverId, string strLattitue, string strLongtitue, string CustLocation,
                                         string TicketId,string VehicleID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "INSERT INTO crm_driverlocation (driver_id,driver_name,latitute,longititute,customer_pinlocation,TicketId,vehicle_no) " +
                  "VALUES('" + strDriverId + "',dbo.GetContactName('" + strDriverId + "'),'" + strLattitue + "','" + strLongtitue + "','" + CustLocation + "','" + TicketId + "','" + VehicleID + "')";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();
            
            if (nRowCount > 0)
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Import Lead Data Histoty
    [WebMethod]
    public DataSet ImportHistory(string strFileName, string strFileDataCount, string strAccountID, string strUserID, string strBatchNo)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        strParameters = "nSessionID";

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "INSERT INTO crm_lead_import_history(batch_service_id,batch_filename, batch_no, batch_total_records, batch_import_count,batch_import_date, batch_import_by) " +
                       "VALUES " +
                       "(" + strAccountID + ",'" + strFileName + "','" + strBatchNo + "'," + strFileDataCount + ", " + strFileDataCount + "," +
                        "GETDATE(),'" + strUserID + "')";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            nResultCode = 0;
            strResult = "Pass";
            if (nResultCode == 0)
            {
                strTemp = "TRUNCATE TABLE crm_accounts_import ";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                int nRows = m_CommandODBC.ExecuteNonQuery();

            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ImportHistory", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ImportHistory", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }

        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetAllaccount
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public DataSet Fetchaccount()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            strTemp = "Select id,cust_name ,(case when cust_name2='' OR cust_name2 = 'NULL'  then 'NA' else cust_name2 end)AS cust_name2,cust_short_name from crm_accounts where cust_enabled='Y' and Useable ='Y' and cust_name !='' order by cust_name";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_accounts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_accounts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_accounts", strParameters);
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
    public DataSet GetFuelType()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            strTemp = "select distinct invoice_cal_type from crm_invocie_calculation_master where upload_date is not null";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "GetFuelType";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetFuelType", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetFuelType", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }

    #endregion

    #region GetAllaccount
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public DataSet Fetchaccounts()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            strTemp = "Select id,cust_name ,(case when cust_name2='' OR cust_name2 = 'NULL'  then 'NA' else cust_name2 end)AS cust_name2,cust_short_name from crm_accounts where cust_enabled='Y' and Useable ='Y' and cust_name !='' order by cust_name";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_accounts";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_accounts", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_accounts", strParameters);
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
    public DataSet GetAccountSiteName(string AccountId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            strTemp = "select id, branch_code as name from crm_contacts  where contact_type_id = 'C' AND related_to_id= " + AccountId + " order by branch_code";
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "SiteList";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "SiteList", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetFuelType", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }

    #endregion

    #region Import Ticket Data Histoty
    [WebMethod]
    public DataSet ImportTicketHistory(string strFileName, string strFileDataCount, string strAccountID, string strUserID, string strBatchNo)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        strParameters = "nSessionID";

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "INSERT INTO crm_lead_import_history(batch_service_id,batch_filename, batch_no, batch_total_records, batch_import_count,batch_import_date, batch_import_by) " +
                       "VALUES " +
                       "(" + strAccountID + ",'" + strFileName + "','" + strBatchNo + "'," + strFileDataCount + ", " + strFileDataCount + "," +
                        "GETDATE(),'" + strUserID + "')";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            nResultCode = 0;
            strResult = "Pass";
            if (nResultCode == 0)
            {
                strTemp = "TRUNCATE TABLE crm_ticket_import ";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
                int nRows = m_CommandODBC.ExecuteNonQuery();

            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ImportTicketHistory", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "ImportTicketHistory", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }

        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Update Vehicle Master
    [WebMethod]
    public DataSet UpdateVehicleMaster(string DriverId, string VehicleNo, string VehicleModel)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "UPDATE crm_vehicle_master SET " +
                        "vehicle_no='" + VehicleNo + "',vehicle_Type='" + VehicleModel + "'" +
                        "WHERE vehicle_DriverID = " + DriverId;
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();
            if(nRowCount>0)
            {
                nResultCode = 0;
                strResult = "Pass";
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "UpdateVehicleMaster", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "UpdateVehicleMaster", strParameters);
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