using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Xml;
using System.Data.Odbc;
using Telerik.Web.UI;

public class DataBase
{
    public OdbcConnection oCon = null;
    public int nDatabaseType = 1;    
    public string DB_DATE_FUNCTION = string.Empty;
    public string DB_NOLOCK = string.Empty;
    public string DB_ROWLOCK = string.Empty;
    public string DB_TOP_SQL = string.Empty;
    public string DB_TOP_MYSQL = string.Empty;
    public string DB_SEPERATOR = string.Empty;
    public string DB_DATALENGTH = string.Empty;
    public string DB_DATEDIFF = string.Empty;
    public string DB_NULL = string.Empty;
    public string DB_DATEDAY = string.Empty;
    public string DB_DATEMONTH = string.Empty;
    public string DB_DATEYEAR = string.Empty;
    public string DB_DATEHOUR = string.Empty;
    public string DB_DATEMINUTE = string.Empty;
    public string DB_DATESECOND = string.Empty;
    public string DB_EXECUTE_PROCEDURE = string.Empty;
    public string DB_PROCEDURE_DELIMETER1 = string.Empty;
    public string DB_PROCEDURE_DELIMETER2 = string.Empty;
    public string DB_FUNCTION = string.Empty;
    public string DB_SPLIT_FUNCTION = string.Empty;
    public string DB_IDENTITY = string.Empty;
    public string DB_UTC_DATE = string.Empty;
    public string DB_CONCAT = string.Empty;
    public string DB_TOP_MYSQL10 = string.Empty;
    public string DB_TOP_MYSQL5 = string.Empty;
    public string DB_TOP_SQL10 = string.Empty;
    public string DB_TOP_SQL5 = string.Empty;
    public string DB_TIME_FORMAT = string.Empty;

    public string DB_TOPU_SQL = string.Empty;

    public OdbcTransaction m_TransactionODBC = null;

    public void BeginTransaction() 
    {
        if (m_TransactionODBC == null)
            m_TransactionODBC = oCon.BeginTransaction();
    }

    public void Rollback()
    {
        if (m_TransactionODBC != null)
        {
            m_TransactionODBC.Rollback();
            m_TransactionODBC = null;
        }
    }

    public void Commit()
    {
        if (m_TransactionODBC != null)
        {
            m_TransactionODBC.Commit();
            m_TransactionODBC = null;
        }
    }

    public OdbcConnection OpenDB(string strDatabaseName)
    {
        if (oCon != null)
        {
            CloseDB();
        }
        string strConnection = System.Configuration.ConfigurationManager.AppSettings[strDatabaseName];
        oCon = new OdbcConnection(strConnection);
        oCon.Open();
        string strConnectedDataBase = oCon.Driver;        
        if (strConnectedDataBase.Contains("myodbc"))   //-- MySql
        {
            nDatabaseType = 2;
            DB_DATE_FUNCTION = "NOW()";
            DB_NOLOCK = " NOLOCK ";
            DB_TOP_MYSQL = " LIMIT 1";
            DB_TOP_MYSQL10 = " LIMIT 10";
            DB_TOP_MYSQL5 = " LIMIT 5";
            DB_SEPERATOR = ".";
            DB_DATALENGTH = "CHARACTER_LENGTH";
            DB_DATEDIFF = "TIMESTAMPDIFF";
            DB_NULL = " IFNULL";
            DB_DATEDAY = "DAY(";
            DB_DATEMONTH = "MONTH(";
            DB_DATEYEAR = "YEAR(";
            DB_DATEHOUR = "HOUR(";
            DB_DATEMINUTE = "MINUTE(";
            DB_DATESECOND = "SECOND(";
            DB_EXECUTE_PROCEDURE = "CALL ";
            DB_PROCEDURE_DELIMETER1 = "(";
            DB_PROCEDURE_DELIMETER2 = ")";
            DB_SPLIT_FUNCTION = "CALL ";
            DB_IDENTITY = "@@identity";
            DB_CONCAT = "concat";
            DB_UTC_DATE = "UTC_TIMESTAMP()";
            DB_TIME_FORMAT = "sec_to_time";
        }
        else //--Sql Server
        {
            nDatabaseType = 1;
            DB_DATE_FUNCTION = "GETDATE()";
            DB_NOLOCK = " NOLOCK ";
            DB_ROWLOCK = " WITH (ROWLOCK)";
            DB_TOP_SQL = " TOP 1 ";
            DB_TOP_SQL10 = " TOP 10 ";
            DB_TOP_SQL5 = " TOP 5 ";
            DB_SEPERATOR = "..";
            DB_DATALENGTH = "DATALENGTH";
            DB_DATEDIFF = "DATEDIFF";
            DB_NULL = " ISNULL";
            DB_DATEDAY = "DATEPART(DAY, ";
            DB_DATEMONTH = "DATEPART(MONTH, ";
            DB_DATEYEAR = "DATEPART(YEAR, ";
            DB_DATEHOUR = "DATEPART(HOUR, ";
            DB_DATEMINUTE = "DATEPART(MINUTE, ";
            DB_DATESECOND = "DATEPART(SECOND, ";
            DB_EXECUTE_PROCEDURE = "EXEC ";
            DB_PROCEDURE_DELIMETER1 = " ";
            DB_PROCEDURE_DELIMETER2 = "";
            DB_FUNCTION = "dbo.";
            DB_SPLIT_FUNCTION = "SELECT * FROM dbo.";
            DB_IDENTITY = "@@identity";
            DB_UTC_DATE = "GetUTCDATE()";
            DB_TIME_FORMAT = "dbo.idg_fnc_TimeFormat";
        }
        return oCon;
    }
   
    public void CloseDB()
    {
        if (oCon != null)
        {
            if(m_TransactionODBC != null)
                m_TransactionODBC.Rollback();
            if(oCon.State == ConnectionState.Open)
                oCon.Close();
            oCon.Dispose();
            oCon = null;
        }
    }

    #region Get User's Local Time (Time Zone)
    public DateTime GetLocalZoneTime(DateTime dtDate, int nMinutes)
    {
        TimeSpan tSpan = new TimeSpan(0, nMinutes, 0);
        dtDate = dtDate.Add(tSpan);
        return dtDate;
    }
    #endregion

    #region Replace Special Charaters
    public string ReplaceCharacters(string szStr)
    {
        szStr = szStr.Replace("%26", "&");
        return szStr;
    }
    #endregion

    #region Round Off Minutes in DateTime
    public DateTime RoundOffMinutes(DateTime dtTime)
    {
        int nRemainder = 0;
        int nMinute = dtTime.Minute;

        nRemainder = nMinute % 5;
        if (nRemainder != 0)
        {
            dtTime = dtTime.AddMinutes(5 - nRemainder);
        }
        return dtTime;
    }
    #endregion

    #region Check if date is valid or not
    public bool IsValidDate(string strDate)
    {
        DateTime tempDateTime;
        if (DateTime.TryParse(strDate, out tempDateTime))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region Check for valid numeric
    public bool IsValidNumeric(string strNumber)
    {
        int tempInt;
        double tempFloat;
        if (int.TryParse(strNumber, out tempInt))
            return true;
        else
        {
            if (Double.TryParse(strNumber, out tempFloat))
                return true;

            return false;
        }
    }
    #endregion

    #region Check for valid Int
    public int CheckIntValue(string szValue)
    {
        try
        {
            return Convert.ToInt32(szValue);
        }
        catch (Exception)
        {
            return 0;
        }
    }
    #endregion

    #region Response Table
    public DataTable GetResponseTable(long nResultCode, string strResult)
    {
        DataTable dtTable = new DataTable("Response");

        DataColumn dc;
        dc = new DataColumn();
        dc.DataType = System.Type.GetType("System.Int32");
        dc.ColumnName = "ResultCode";
        dtTable.Columns.Add(dc);

        dc = new DataColumn();
        dc.DataType = System.Type.GetType("System.String");
        dc.ColumnName = "ResultString";
        dtTable.Columns.Add(dc);

        DataRow dr = dtTable.NewRow();
        dr[0] = nResultCode;
        dr[1] = strResult;
        dtTable.Rows.Add(dr);

        return dtTable;
    }
    #endregion    

    #region FetchTransactionData
    public DataSet FetchTransactionData(string ObjectType, string ObjectTarget, string strColumns, int ObjectId, int nUserTimeZoneSpan)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
    
        try
        {
            OdbcDataAdapter m_DataAdapterOdbc;
            string strTemp = "";
            if (ObjectTarget.Length <= 0)
            {
                strTemp = "SELECT" + DB_TOP_SQL + " object_table_name FROM crm_tables where object_code = '" + ObjectType + "' " + DB_TOP_MYSQL;
                OdbcCommand m_OdbcCommand = new OdbcCommand(strTemp, oCon, m_TransactionODBC);
                ObjectTarget = m_OdbcCommand.ExecuteScalar().ToString();
            }
            string strQuery = "SELECT " + DB_TOP_SQL + " " + DB_NULL + "(Useable,'') as Useable";
            //--get column details from view_columns
            strTemp = "SELECT " + DB_NULL + "(tabledef_column_name, '') as table_column_name," +
                      "tabledef_column_header " +
                      "FROM crm_table_columns " +
                      "WHERE tabledef_for = '" + ObjectType + "' " +
                      "AND " + DB_NULL + "(tabledef_column_name,'') <> '' ";

            if (strColumns.Length > 0)
            {
                strTemp += "AND tabledef_column_header in (" + strColumns + ")";
            }

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, oCon);
            DataTable dtTableDefinition = new DataTable("TableDefinition");
            m_DataAdapterOdbc.Fill(dtTableDefinition);
            if (dtTableDefinition.Rows.Count == 0)
            {
                nResultCode = -1;
                strResult = "No columns defined in the crm_table_columns";
                m_DataSet.Tables.Add(GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            foreach (DataRow row in dtTableDefinition.Rows)
            {
                strQuery += "," + row["table_column_name"] + " as " + row["tabledef_column_header"];
            }

            strQuery += " FROM " + ObjectTarget + " WHERE id = " + ObjectId + " " + DB_TOP_MYSQL;
            strQuery = strQuery.Replace("#TIMESPAN#", nUserTimeZoneSpan.ToString());

            m_DataAdapterOdbc = new OdbcDataAdapter(strQuery, oCon);
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
       
        m_DataSet.Tables.Add(GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    public DataTable ChangeTransactionOwner(string ObjectType, string ObjectId, string CurrentOwnerId, string NewOwnerId)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        try
        {
            string strTemp = "";
            strTemp = "SELECT " + DB_TOP_SQL + " object_table_name,object_permission_activity_id " +
                "FROM crm_tables WHERE object_code = '" + ObjectType + "' " + DB_TOP_MYSQL;
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, oCon);
            m_DataAdapterOdbc.SelectCommand.Transaction = m_TransactionODBC;
            DataTable dtTable = new DataTable("TableDefinition");
            m_DataAdapterOdbc.Fill(dtTable);

            if (dtTable.Rows.Count <= 0 || dtTable.Rows[0]["object_table_name"].ToString() == "")
            {
                strResult = "Object definition not found for " + ObjectType;
                return GetResponseTable(nResultCode, strResult);
            }

            string ObjectTarget = Convert.ToString(dtTable.Rows[0]["object_table_name"]);
            string ActivityId = Convert.ToString(dtTable.Rows[0]["object_permission_activity_id"]);

            // modified_by, modified_ip, modified_date
            string strQuery = "UPDATE " + ObjectTarget + " SET owner_id=" + NewOwnerId ;
            if (CurrentOwnerId == "-1")
                strQuery += " WHERE id in(" + ObjectId + ")";
            else
                strQuery += " WHERE id=" + ObjectId + " AND owner_id=" + CurrentOwnerId;

            OdbcCommand m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();
            if (nRowCount > 0)
            {
                if (CurrentOwnerId != "-1")
                {
                    strQuery = "INSERT INTO CRM_RECENT_ITEMS(recent_item_object_type," +
                                    "recent_item_object_id," +
                                    "recent_item_action," +
                                    "recent_item_name," +
                                    "recent_item_contact_id," +
                                    "recent_item_status," +
                                    "recent_item_start_time," +
                                    "recent_item_end_time," +
                                    "recent_item_activity_id," +
                                    "recent_item_display_status) " +
                                    "VALUES('" + ObjectType + "'," + ObjectId + ",'REASSIGNMENT', ''," + NewOwnerId + ",'P'," + DB_UTC_DATE + "," + DB_UTC_DATE + "," + ActivityId + ",0)";
                    m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);
                    nRowCount = m_CommandODBC.ExecuteNonQuery();
                }
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
        }
        return GetResponseTable(nResultCode, strResult);
    }

    public DataTable SaveTransactionData(string ObjectType, int ObjectId, string GenerateTransactionNumber, DateTime TrnsactionDate, int UserId, string Host, XmlDocument ObjectFields)
    {
        int nRowCount = 0;
        int nResultCode = -1;
        string strResult = "Fail";
        try
        {
            string strTemp = "";

            strTemp = "SELECT " + DB_TOP_SQL + " object_table_name,object_transaction_number_format,object_transaction_series_month,object_transaction_series_year,object_owner_assignment,object_custom_tab_field " +
                "FROM crm_tables WHERE object_code = '" + ObjectType + "' " + DB_TOP_MYSQL;
            

            //strTemp = "SELECT " + DB_TOP_SQL + " object_table_name,object_transaction_number_format,object_transaction_series_month,object_transaction_series_year,object_owner_assignment,object_custom_tab_field " +
            //    "FROM crm_tables WHERE object_code = '" + ObjectType + "' " + DB_TOP_MYSQL;

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, oCon);
            m_DataAdapterOdbc.SelectCommand.Transaction = m_TransactionODBC;
            DataTable dtTable = new DataTable("TableDefinition");
            m_DataAdapterOdbc.Fill(dtTable);

            if (dtTable.Rows.Count <= 0 || dtTable.Rows[0]["object_table_name"].ToString() == "")
            {
                strResult = "Object definition not found for " + ObjectType;
                return GetResponseTable(nResultCode, strResult);
            }

            string ObjectTarget = Convert.ToString(dtTable.Rows[0]["object_table_name"]);
            string CustomFieldName = Convert.ToString(dtTable.Rows[0]["object_custom_tab_field"]);
            string CustomFieldValue = "";

            // transaction_number

            string TransactionNumber = "";
            if (GenerateTransactionNumber == "Y" && dtTable.Rows[0]["object_transaction_number_format"].ToString().Length > 0)
            {
                string TransactionMonth = dtTable.Rows[0]["object_transaction_series_month"].ToString();
                string TransactionYear = dtTable.Rows[0]["object_transaction_series_year"].ToString();
                int nYear = 0;
                int nMonth = 0;
                if (TransactionYear == "Y")
                    nYear = TrnsactionDate.Year;
                if (TransactionMonth == "Y")
                    nMonth = TrnsactionDate.Month;

                string nTransactionNumber = getTransactionNumber(ObjectType, nYear, nMonth);
                TransactionNumber = dtTable.Rows[0]["object_transaction_number_format"].ToString();
                TransactionNumber = TransactionNumber.Replace("#MONTH#", TrnsactionDate.Month.ToString("00"));
                TransactionNumber = TransactionNumber.Replace("#YEAR#", TrnsactionDate.Year.ToString("0000"));
                TransactionNumber = TransactionNumber.Replace("#NUMBER#", nTransactionNumber.ToString());
            }

            XmlElement root = ObjectFields.DocumentElement;
            XmlNodeList NodeListINFO = root.GetElementsByTagName("FIELD");
            string strQuery = "";
            string strFields = "";
            string strValue = "";
            string strUseable = "N";
            OdbcCommand m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);
            if (NodeListINFO.Count > 0)
            {
                string strName, Value, Type;
                int Length;
                foreach (XmlNode field in NodeListINFO)
                {                    
                    strName = field["NAME"].InnerText;
                    Value = field["VALUE"].InnerText;
                    Type = field["TYPE"].InnerText;
                    if (field["LENGTH"].InnerText == "")
                        Length = 0;
                    else
                        Length = Convert.ToInt32(field["LENGTH"].InnerText);

                    string strTmpVal = "?";

                    if (String.Compare(Type, "function", true)== 0)
                        strTmpVal = Value;
                    else if (String.Compare(Type, "decimal", true) == 0)
                    {
                        if (Value == "")
                            Value = "0";
                        m_CommandODBC.Parameters.Add(strName, OdbcType.Decimal).Value = Value;
                    }
                    else if (String.Compare(Type, "int", true) == 0)
                    {
                        if (Value == "")
                            Value = "0";
                        m_CommandODBC.Parameters.Add(strName, OdbcType.Int).Value = Value;
                    }
                    else if (String.Compare(Type, "varchar", true) == 0)
                    {
                        m_CommandODBC.Parameters.Add(strName, OdbcType.VarChar, Value.Length).Value = Value;
                    }
                    else if (String.Compare(Type, "nvarchar", true) == 0)
                    {
                        m_CommandODBC.Parameters.Add(strName, OdbcType.NVarChar, Value.Length).Value = Value;
                    }
                    else if (String.Compare(Type, "text", true) == 0)
                    {
                        m_CommandODBC.Parameters.Add(strName, OdbcType.Text).Value = Value;
                    }
                    else if (String.Compare(Type, "datetime", true) == 0)
                    {
                        if (Value == "")
                            strTmpVal = "null";
                        else
                            m_CommandODBC.Parameters.Add(strName, OdbcType.DateTime).Value = Value;
                    }

                    if (ObjectId > 0)
                        strFields += "," + strName + "=" + strTmpVal;
                    else
                    {
                        strFields += "," + strName;
                        strValue += "," + strTmpVal;
                    }
                    if (CustomFieldName == strName)
                        CustomFieldValue = Value;
                }
                strUseable = "Y";
            }

            // transaction_number
            if (TransactionNumber.Length > 0)
            {
                if (ObjectId > 0)
                    strFields += ",transaction_number=?";
                else
                {
                    strFields += ",transaction_number";
                    strValue += ",?";
                }
                m_CommandODBC.Parameters.Add("transaction_number", OdbcType.VarChar, TransactionNumber.Length).Value = TransactionNumber;
            }

            // custom tab 
            if (CustomFieldValue.Length > 0)
            {
                if (ObjectId > 0)
                    strFields += ",custom_tab_name=" + DB_FUNCTION + "GetCustom('" + ObjectType + "', 'NAME', ?)" +
                        ",custom_tab_url=" + DB_FUNCTION + "GetCustom('" + ObjectType + "', 'URL', ?)";
                else
                {
                    strFields += ",custom_tab_name,custom_tab_url";
                    strValue += DB_FUNCTION + "GetCustom('" + ObjectType + "', 'NAME', ?)," + DB_FUNCTION + "GetCustom('" + ObjectType + "', 'URL', ?)";
                }
                m_CommandODBC.Parameters.Add("custom_tab_name", OdbcType.VarChar, CustomFieldValue.Length).Value = CustomFieldValue;
                m_CommandODBC.Parameters.Add("custom_tab_url", OdbcType.VarChar, CustomFieldValue.Length).Value = CustomFieldValue;
            }

            if (ObjectId > 0)
            {
                // modified_by, modified_ip, modified_date
                strQuery = "UPDATE " + ObjectTarget + " SET useable='Y',last_activity_date=" + DB_UTC_DATE + ",modified_date=" + DB_UTC_DATE + ",modified_ip='" + Host + "',modified_by=" + UserId + strFields + " WHERE id=" + ObjectId;                
            }
            else
            {
                // created_by, created_ip, created_date
                strQuery = "INSERT INTO " + ObjectTarget + " (useable, last_activity_date,created_by, created_ip, created_date,modified_by, modified_ip, modified_date" + strFields + ") Values ('" + strUseable + "'," + DB_UTC_DATE + "," + UserId + ",'" + Host + "'," + DB_UTC_DATE + "," + UserId + ",'" + Host + "'," + DB_UTC_DATE + strValue + ") ";
            }
            m_CommandODBC.CommandText = strQuery;
            m_CommandODBC.CommandType = CommandType.Text;
            nRowCount = m_CommandODBC.ExecuteNonQuery();
            if (nRowCount > 0)
            {
                nResultCode = 0;
                if (ObjectId == 0)
                {
                    strQuery = "SELECT @@IDENTITY";
                    m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);
                    ObjectId = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
                    nResultCode = ObjectId;
                }
                strResult = TransactionNumber;
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
        return GetResponseTable(nResultCode, strResult);
    }

    public DataTable SaveRecentActivity(int itemid, string ObjectType, int ObjectId, string ObjectAction, string ObjectName, int UserId, string ObjectStatus, int ActivityId, string HistoryText)
    {
        int nResultCode = 0;
        string strResult = "";
        try
        {
            string strQuery = "";
            OdbcCommand m_CommandODBC = null;
            int nRowCount = 0;

            if (ObjectStatus == "P")
            {
                strQuery = "UPDATE CRM_RECENT_ITEMS SET recent_item_display_status = 0 " +
                "WHERE recent_item_object_type = '" + ObjectType + "' " +
                "AND recent_item_object_id = " + ObjectId + " " +
                "AND recent_item_contact_id = " + UserId + " " +
                "AND recent_item_id <> " + itemid;
                m_CommandODBC = new OdbcCommand("", oCon, m_TransactionODBC);
                m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);
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
                                "VALUES('" + ObjectType + "'," + ObjectId + ",'" + ObjectAction + "', ?," + UserId + ",'" + ObjectStatus + "'," + DB_UTC_DATE + "," + DB_UTC_DATE + "," + ActivityId + ",'" + HistoryText + "')";
            else
                strQuery = "UPDATE CRM_RECENT_ITEMS SET recent_item_status = '" + ObjectStatus + "'," +
                                "recent_item_end_time = " + DB_UTC_DATE + "," +
                                "recent_item_name = ?," +
                                "recent_item_action = (CASE WHEN recent_item_action = 'New' THEN 'Created' ELSE 'Modified' END), " +
                                "recent_item_history_text = '" + HistoryText + "' " +
                                "WHERE recent_item_id = " + itemid;

            m_CommandODBC = new OdbcCommand("", oCon, m_TransactionODBC);
            m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);
            m_CommandODBC.Parameters.Add("recent_item_name", OdbcType.VarChar).Value = ObjectName;
            nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
            {
                nResultCode = 0;
                if (itemid == 0)
                {
                    strQuery = "SELECT @@IDENTITY";
                    m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);
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
        return GetResponseTable(nResultCode, strResult);
    }
    //changes manju

    public DataTable SaveRecentActivity(int itemid, string ObjectType, int ObjectId, string ObjectAction, string ObjectName, int UserId, string ObjectStatus, int ActivityId)
    {
        int nResultCode = 0;
        string strResult = "";
        try
        {
            string strQuery = "";
            OdbcCommand m_CommandODBC = null;
            int nRowCount = 0;

            if (ObjectStatus == "P")
            {
                strQuery = "UPDATE CRM_RECENT_ITEMS SET recent_item_display_status = 0 " +
                "WHERE recent_item_object_type = '" + ObjectType + "' " +
                "AND recent_item_object_id = " + ObjectId + " " +
                "AND recent_item_contact_id = " + UserId + " " +
                "AND recent_item_id <> " + itemid;
                m_CommandODBC = new OdbcCommand("", oCon, m_TransactionODBC);
                m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);
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
                                "recent_item_activity_id)" +
                                
                                "VALUES('" + ObjectType + "'," + ObjectId + ",'" + ObjectAction + "', ?," + UserId + ",'" + ObjectStatus + "'," + DB_UTC_DATE + "," + DB_UTC_DATE + "," + ActivityId + ")";
            else
                strQuery = "UPDATE CRM_RECENT_ITEMS SET recent_item_status = '" + ObjectStatus + "'," +
                                "recent_item_end_time = " + DB_UTC_DATE + "," +
                                "recent_item_name = ?," +
                                "recent_item_action = (CASE WHEN recent_item_action = 'New' THEN 'Created' ELSE 'Modified' END) " +
                                
                                "WHERE recent_item_id = " + itemid;

            m_CommandODBC = new OdbcCommand("", oCon, m_TransactionODBC);
            m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);
            m_CommandODBC.Parameters.Add("recent_item_name", OdbcType.VarChar).Value = ObjectName;
            nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
            {
                nResultCode = 0;
                if (itemid == 0)
                {
                    strQuery = "SELECT @@IDENTITY";
                    m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);
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
        return GetResponseTable(nResultCode, strResult);
    }


    public string getTransactionNumber(string Transaction_type, int Year, int Month)
    {
        int nRowCount = 0;
        
        DataSet m_DataSet = new DataSet();

        string strTemp = "";
        string nTranNumber = "0";
        try
        {
            //--Generate Case Number [format - YYYYMM000001]
            strTemp = "UPDATE crm_transaction_numbers " + DB_ROWLOCK + " SET " +
                        "last_number = " + DB_NULL + "(last_number,0) + 1 " +
                        "WHERE type='" + Transaction_type + "' " +
                        "AND month=" + Month + " " +
                        "AND year=" + Year;

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, oCon, m_TransactionODBC);
            nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount == 0)
            {
                //--if current financial year/month entry not present then insert
                strTemp = "INSERT INTO crm_transaction_numbers(year,month," +
                            "type,prefix," +
                            "last_number)VALUES(" + Year + "," +
                            Month + "," +
                            "'" + Transaction_type + "','',1)";

                m_CommandODBC = new OdbcCommand(strTemp, oCon, m_TransactionODBC);
                m_CommandODBC.ExecuteNonQuery();
            }
            m_CommandODBC.Dispose();

            strTemp = "SELECT " + DB_TOP_SQL + " RIGHT('00000' + CONVERT(VARCHAR, last_number), 5) " +
                    "FROM crm_transaction_numbers " +
                    "WHERE type='" + Transaction_type + "' " +
                    "AND month=" + Month + " " +
                    "AND year=" + Year + " " + DB_TOP_MYSQL;

            m_CommandODBC = new OdbcCommand(strTemp, oCon, m_TransactionODBC);
            nTranNumber = Convert.ToString(m_CommandODBC.ExecuteScalar());
        }
        catch (OdbcException)
        {
            Rollback();
        }
        catch (Exception)
        {
            Rollback();
        }
        finally
        {
            //CloseDB();
        }
        return nTranNumber;
    }

    public void fillParameterXML(ref XmlDocument xMainDoc, string Name, string Value, string Type, string Length)
    {
        XmlNode fieldsNode = xMainDoc.GetElementsByTagName("FIELDS").Item(0);

        XmlNode fieldNode = xMainDoc.CreateElement("FIELD");
        fieldsNode.AppendChild(fieldNode);

        XmlNode nameNode = xMainDoc.CreateElement("NAME");
        nameNode.AppendChild(xMainDoc.CreateTextNode(Name));
        fieldNode.AppendChild(nameNode);

        XmlNode valueNode = xMainDoc.CreateElement("VALUE");
        valueNode.AppendChild(xMainDoc.CreateTextNode(Value));
        fieldNode.AppendChild(valueNode);

        XmlNode typeNode = xMainDoc.CreateElement("TYPE");
        typeNode.AppendChild(xMainDoc.CreateTextNode(Type));
        fieldNode.AppendChild(typeNode);

        XmlNode lengthNode = xMainDoc.CreateElement("LENGTH");
        lengthNode.AppendChild(xMainDoc.CreateTextNode(Length));
        fieldNode.AppendChild(lengthNode);
    }

    public XmlDocument createParameterXML()
    {
        XmlDocument xMainDoc = new XmlDocument();
        XmlNode docNode = xMainDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
        xMainDoc.AppendChild(docNode);
        XmlNode fieldsNode = xMainDoc.CreateElement("FIELDS");
        xMainDoc.AppendChild(fieldsNode);
        return xMainDoc;
    }



    //13-12-12 manju
    #region tagInventary
     public bool settag(int inventoryid,string remarks,int caseid)
    {
        bool flage = false;
        try
        {
            
            string strQuery = "INSERT INTO [crm_case_inventories]([case_id],[inventory_id],[related_text],[task_id] ,[inserted_date],[remarks])  VALUES  (" + caseid + "," + inventoryid + ",'',0,GetUTCDATE(),'" + remarks + "')";
            OdbcCommand m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);

            m_CommandODBC.CommandText = strQuery;
            m_CommandODBC.CommandType = CommandType.Text;
            int nRowCount = m_CommandODBC.ExecuteNonQuery();
           flage=(nRowCount >0)?true:false;
        }
        catch (OdbcException)
        {
            Rollback();
        }
        catch (Exception)
        {
            Rollback();
        }
        finally
        {
            
        }
        return flage;
    }
    #endregion
    //13-12-12 end manju
    //5-1-13 manju
    #region fetch notes
   
     public DataSet fatchStatus_case(int Caseid)
     {
         int nResultCode = -1;
         string strTemp = "";
         string strResult = "Fail - ";
         DataSet m_DataSet = new DataSet();
         OdbcDataAdapter m_DataAdapterOdbc;
       
         try
         {
           
             strTemp = "select top 1 note_desc from crm_notes where related_to='CAS'  and related_to_id=" + Caseid + " order by id desc";

             OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, oCon, m_TransactionODBC);

             m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, oCon);
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
         m_DataSet.Tables.Add(GetResponseTable(nResultCode, strResult));
         return m_DataSet;
     }
     #endregion

    //end 
     public DataTable SaveTransactionDataDiscard(string ObjectType, int ObjectId, string GenerateTransactionNumber, DateTime TrnsactionDate, int UserId, string Host)
     {
         int nRowCount = 0;
         int nResultCode = -1;
         string strResult = "Fail";
         try
         {
             string strTemp = "";
             strTemp = "SELECT " + DB_TOP_SQL + " object_table_name,object_transaction_number_format,object_transaction_series_month,object_transaction_series_year,object_owner_assignment,object_custom_tab_field " +
                 "FROM crm_tables WHERE object_code = '" + ObjectType + "' " + DB_TOP_MYSQL;
             OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, oCon);
             m_DataAdapterOdbc.SelectCommand.Transaction = m_TransactionODBC;
             DataTable dtTable = new DataTable("TableDefinition");
             m_DataAdapterOdbc.Fill(dtTable);

             if (dtTable.Rows.Count <= 0 || dtTable.Rows[0]["object_table_name"].ToString() == "")
             {
                 strResult = "Object definition not found for " + ObjectType;
                 return GetResponseTable(nResultCode, strResult);
             }

             string ObjectTarget = Convert.ToString(dtTable.Rows[0]["object_table_name"]);
             string CustomFieldName = Convert.ToString(dtTable.Rows[0]["object_custom_tab_field"]);
             string CustomFieldValue = "";

             // transaction_number

             string TransactionNumber = "";
             string strQuery = "";

             OdbcCommand m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);

             if (ObjectId > 0)
             {
                 // modified_by, modified_ip, modified_date
                 strQuery = "UPDATE " + ObjectTarget + " SET useable='N',last_activity_date=" + DB_UTC_DATE + ",modified_date=" + DB_UTC_DATE + ",modified_ip='" + Host + "',modified_by=" + UserId + " WHERE id=" + ObjectId;


                 m_CommandODBC.CommandText = strQuery;
                 m_CommandODBC.CommandType = CommandType.Text;
                 nRowCount = m_CommandODBC.ExecuteNonQuery();
                 if (nRowCount > 0)
                 {
                     nResultCode = 0;
                     if (ObjectId == 0)
                     {
                         strQuery = "SELECT @@IDENTITY";
                         m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);
                         ObjectId = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
                         nResultCode = ObjectId;
                     }
                     strResult = TransactionNumber;
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
         }
         return GetResponseTable(nResultCode, strResult);
     }

     public DataTable SaveRuleHistory(WorkFlowHistory ob)
     {
         int nResultCode = 0;
         string strResult = "";
         try
         {
          
             string strQuery = "";
             OdbcCommand m_CommandODBC = null;
             int nRowCount = 0;
             
                 strQuery = "INSERT INTO crm_workflow_history( related_to,"+
                                " related_to_id,"+
                               
                                " action_by_id,"+
                                " action_by_name,"+
                                " action_by_ip_address,"+
                                " action_remarks,"+
                                " action_current_level,"+
                                " action_current_level_name,"+
                                " action_next_level,"+
                                " action_next_level_name)"+
                                "VALUES(?,?,?,?,?,?,?,?,?,?)";
             m_CommandODBC = new OdbcCommand("", oCon, m_TransactionODBC);
             m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);
             m_CommandODBC.Parameters.Add("related_to", OdbcType.VarChar).Value = ob.relatedTo;
             m_CommandODBC.Parameters.Add("related_to_id", OdbcType.Int).Value = ob.relatedToId;
             //m_CommandODBC.Parameters.Add("action_date", OdbcType.DateTime).Value = DateTime.UtcNow;
             m_CommandODBC.Parameters.Add("action_by_id", OdbcType.Int).Value = ob.userId;
             m_CommandODBC.Parameters.Add("action_by_name", OdbcType.VarChar).Value = ob.userName;
             m_CommandODBC.Parameters.Add("action_by_ip_address", OdbcType.VarChar).Value = ob.IPaddress;
             m_CommandODBC.Parameters.Add("action_remarks", OdbcType.VarChar).Value = ob.remarks;
             m_CommandODBC.Parameters.Add("action_current_level", OdbcType.Int).Value = ob.currentLevel;
             m_CommandODBC.Parameters.Add("action_current_level_name", OdbcType.VarChar).Value = ob.CurrentLavelName;
             m_CommandODBC.Parameters.Add("action_next_level", OdbcType.Int).Value = ob.nextLevel;
             m_CommandODBC.Parameters.Add("action_next_level_name", OdbcType.VarChar).Value = ob.nextLavelName;
             nRowCount = m_CommandODBC.ExecuteNonQuery();

             if (nRowCount > 0)
             {
                nResultCode = 0;
                strQuery = "SELECT @@IDENTITY";
                m_CommandODBC = new OdbcCommand(strQuery, oCon, m_TransactionODBC);
                nResultCode = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
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
         return GetResponseTable(nResultCode, strResult);
     }
     #region  Execute any query
     public string ExecuteQuery(string strQuery)
     {
         string strResult = "";
         try
         {
             OpenDB("ConDB");
             strQuery = strQuery.Replace("#", DB_FUNCTION);
             OdbcCommand m_CommandOdbc = new OdbcCommand(strQuery, oCon);
             strResult = Convert.ToString(m_CommandOdbc.ExecuteScalar());
         }
         catch (OdbcException ex)
         {
         }
         catch (Exception ex)
         {
         }
         finally
         {
             CloseDB();
         }
         return strResult;
     }
     #endregion

     public DataTable InsertOutboundQueue(int MailNumber)
     {
         int nResultCode = 0;
         string strResult = "";
         try
         {

             string strQuery = "";


             OdbcCommand m_CommandOdbc = new OdbcCommand("InsertOutboundQueue ?", oCon, m_TransactionODBC);
             m_CommandOdbc.CommandType = CommandType.StoredProcedure;
             m_CommandOdbc.Parameters.Add("@MailNumber", OdbcType.Int).Value = MailNumber;
             int i = Convert.ToInt32(m_CommandOdbc.ExecuteNonQuery());
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
         return GetResponseTable(nResultCode, strResult);
     }
    
}
