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

/// <summary>
/// Summary description for CallListWS
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class CallListWS : System.Web.Services.WebService
{
    DataBase m_Connection = new DataBase();
    string strTemp = "";

    #region Response Table
    private DataTable GetResponseTable(long nResultCode, string strResult)
    {
        DataTable dtTable = new DataTable("Response");

        DataColumn dc;
        dc = new DataColumn();
        dc.DataType = System.Type.GetType("System.String");
        dc.ColumnName = "ResultCode";
        dtTable.Columns.Add(dc);

        dc = new DataColumn();
        dc.DataType = System.Type.GetType("System.String");
        dc.ColumnName = "ResultString";
        dtTable.Columns.Add(dc);

        DataRow dr = dtTable.NewRow();
        dr[0] = nResultCode.ToString();
        dr[1] = strResult;
        dtTable.Rows.Add(dr);

        return dtTable;
    }
    #endregion

    #region Response Table
    private DataTable GetResponseTable1(DataTable dtTable, long nResultCode, string strResult)
    {
        int nLastCol = dtTable.Columns.Count;
        int nRowCount = dtTable.Rows.Count;

        DataColumn dc;
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
            dtTable.Rows[0][nLastCol++] = nResultCode.ToString();
            dtTable.Rows[0][nLastCol++] = strResult;
        }
        else
        {
            DataRow dr = dtTable.NewRow();
            dr[nLastCol++] = nResultCode.ToString();
            dr[nLastCol++] = strResult;
            dtTable.Rows.Add(dr);
        }
        return dtTable;
    }
    #endregion
        
    #region GetServiceDetails According Given Criteria.
    [WebMethod]
    public DataSet GetServiceDetails(string strLoginId)
    {
        OdbcDataAdapter m_OdbcDataAdapter;
        DataTable dtTable;
        DataSet m_DataSet = new DataSet();
        int ResultCode = -1;
        string ResultString = "Fail";

        try
        {
            m_Connection.OpenDB("IDGDB");
            int nAgentId = 0;

            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " + m_Connection.DB_NULL + "(agent_agent_id, -1) AS agent_agent_id, " +
                    m_Connection.DB_NULL + "(agent_name, '') AS agent_name,  " +
                    m_Connection.DB_NULL + "(agent_default_chart_skin, 'Default') AS agent_skin  " +
                    "FROM CTI_Agents " +
                    "WHERE agent_login_id = '" + strLoginId + "' " + m_Connection.DB_TOP_MYSQL;

            dtTable = new DataTable("AgentDetails");
            m_OdbcDataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_OdbcDataAdapter.Fill(dtTable);
            m_OdbcDataAdapter.Dispose();
            if (dtTable.Rows.Count > 0)
            {
                nAgentId = Convert.ToInt32(dtTable.Rows[0]["agent_agent_id"]);
                Session["AgentId"] = nAgentId;
                m_DataSet.Tables.Add(dtTable);
            }

            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " + m_Connection.DB_NULL + "(callmaster_db_name, 'CALLMASTER') AS CallmasterDb " +
                       "FROM CTI_Parameters " + m_Connection.DB_TOP_MYSQL;

            OdbcCommand selectCommand = new OdbcCommand(strTemp, m_Connection.oCon);
            string callmasterdb = Convert.ToString(selectCommand.ExecuteScalar());

            if(nAgentId > 0)
            {
                DateTime dt = DateTime.Now;
                string szYear = Convert.ToString(dt.Year);
                string szMonth = (dt.Month < 10) ? "0" + Convert.ToString(dt.Month) : Convert.ToString(dt.Month);
                string szDay = (dt.Day < 10) ? "0" + Convert.ToString(dt.Day) : Convert.ToString(dt.Day);
                string strCallmaster = "cti_call_master_" + szYear + "_" + szMonth + "_" + szDay;

			    strTemp = "SELECT call_end_type as disposition, count(*) as count " +
					        "FROM " + callmasterdb + m_Connection.DB_SEPERATOR + strCallmaster + " " +
        					"WHERE call_agent_id = " + nAgentId.ToString() + " " +
                            "GROUP BY call_end_type";

                dtTable = new DataTable("CallStats");
                m_OdbcDataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                m_OdbcDataAdapter.Fill(dtTable);
                m_OdbcDataAdapter.Dispose();
                if (dtTable.Rows.Count > 0)
                {
                    m_DataSet.Tables.Add(dtTable);
                }

                string strDate = szYear + "-" + szMonth + "-" + szDay;

                strTemp = "SELECT COUNT(*) AS cnt FROM cti_sessions " +
                            "WHERE session_start_time >= '" + strDate + " 00:00:00' AND session_start_time < '" + strDate + " 23:59:59' " +
			                "AND session_agent_id = " + nAgentId.ToString();

                selectCommand = new OdbcCommand(strTemp, m_Connection.oCon);
                int nLoginCount = Convert.ToInt32(selectCommand.ExecuteScalar());

                string strStatsTable = "CTI_Hours_Statistics_"  + szYear + "_" + szMonth;
                strTemp = "SELECT hcall_agent_id, " +
                            "SUM(hcall_total_calls) AS TotalCalls, " +
                            "SUM(hcall_acd_calls) AS AcdCalls, " +
                            "SUM(hcall_manual_calls) AS ManualCalls, " +
                            "SUM(hcall_in_calls) AS InboundCalls, " +
                            "SUM(hcall_out_calls) AS OutboundCalls, " +
                            "SUM(hcall_bucket_01) AS Bucket1, " +
                            "SUM(hcall_bucket_02) AS Bucket2, " +
                            "SUM(hcall_bucket_03) AS Bucket3, " +
                            "SUM(hcall_bucket_04) AS Bucket4, " +
                            "SUM(hcall_bucket_05) AS Bucket5, " +
                            "SUM(hcall_bucket_06) AS Bucket6, " +
                            "SUM(hcall_bucket_07) AS Bucket7, " +
                            "SUM(hcall_bucket_08) AS Bucket8, " +
                            "SUM(hcall_bucket_09) AS Bucket9, " +
                            "SUM(hcall_bucket_10) AS Bucket10, " +
                            m_Connection.DB_TIME_FORMAT + "(SUM(hcall_login_duration)) AS LoginDuration, " +
                            m_Connection.DB_TIME_FORMAT + "(SUM(hcall_login_duration) - SUM(hcall_busy_duration)) AS EffectiveLoginDuration, " +
                            m_Connection.DB_TIME_FORMAT + "(SUM(hcall_idle_duration)) AS IdleDuration, " +
                            m_Connection.DB_TIME_FORMAT + "(SUM(hcall_idle_noivr_duration)) AS inivrdur, " +
                            m_Connection.DB_TIME_FORMAT + "(SUM(hcall_idle_noleads_duration)) AS inleaddur, " +
                            m_Connection.DB_TIME_FORMAT + "(SUM(hcall_incall_duration)) AS IncallDuration, " +
                            m_Connection.DB_TIME_FORMAT + "(SUM(hcall_reserve_duration)) AS resdur, " +
                            m_Connection.DB_TIME_FORMAT + "(SUM(hcall_busy_duration)) AS BreakDuration, " +
                            m_Connection.DB_TIME_FORMAT + "(SUM(hcall_wrapup_duration)) AS wrpdur " +
                            "FROM " + strStatsTable + " " +
                            "WHERE hcall_agent_id = " + nAgentId.ToString() + " " +
                            "AND hcall_datetime between '" + strDate + " 00:00:00' AND '" + strDate + " 23:59:59' " +
                            "GROUP BY hcall_agent_id";
                dtTable = new DataTable("SessionStats");
                m_OdbcDataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                m_OdbcDataAdapter.Fill(dtTable);
                dtTable.Columns.Add("LoginCount");
                dtTable.Rows[0]["LoginCount"] = nLoginCount;
                m_OdbcDataAdapter.Dispose();
                if (dtTable.Rows.Count > 0)
                {
                    m_DataSet.Tables.Add(dtTable);
                }

                string VmExtension = System.Configuration.ConfigurationManager.AppSettings["VOICEMAILPORT"];
                if (VmExtension.Length <= 0)
                    VmExtension = "0";

                strTemp = "SELECT " + m_Connection.DB_NULL + "(count_new, 0) AS New, " +
                            m_Connection.DB_NULL + "(count_saved, 0) AS Saved " +
                            "FROM VM_Extension_Master " +
                            "WHERE Real_Extension = " + VmExtension;

                dtTable = new DataTable("VMStats");
                m_OdbcDataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                m_OdbcDataAdapter.Fill(dtTable);
                m_OdbcDataAdapter.Dispose();
                if (dtTable.Rows.Count > 0)
                {
                    m_DataSet.Tables.Add(dtTable);
                }
            }
        }
        catch (OdbcException ex)
        {
            ResultString = ex.Message;
        }
        catch (Exception ex)
        {
            ResultString = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(GetResponseTable(ResultCode, ResultString));
        return m_DataSet;
    }
    #endregion
    #region SetAgentSkin According to agent.
    [WebMethod]
    public void SetAgentSkin(string skin, string strLoginId)
    {
        string ResultString = "Fail";

        try
        {
            m_Connection.OpenDB("IDGDB");

            strTemp = "UPDATE CTI_Agents SET agent_default_chart_skin = '" + skin + "' " +
                    "WHERE agent_login_id = '" + strLoginId + "'";

            OdbcCommand updateCommand = new OdbcCommand(strTemp, m_Connection.oCon);
            updateCommand.ExecuteNonQuery();
        }
        catch (OdbcException ex)
        {
            ResultString = ex.Message;
        }
        catch (Exception ex)
        {
            ResultString = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
        }
        return;
    }
    #endregion
}