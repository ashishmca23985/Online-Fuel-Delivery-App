using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.Odbc;
using Telerik.Web.UI;
/// <summary>
/// Summary description for classCtiHistory
/// </summary>
public class classCtiHistory
{   
    String strIntreDia = System.Configuration.ConfigurationManager.AppSettings["IDGDB"].ToString();	
    public DataTable  ConnectGetCallMasterTableName(string number, string Type)
    {
        using (OdbcConnection con3 = new OdbcConnection(strIntreDia))
        {
            DataTable dt1 = new DataTable();          
            try
            {
                OdbcCommand cmd5 = new OdbcCommand("call ams_Get_udf_callmaster_table_name (?,?)", con3);
                con3.Open();
                cmd5.CommandType = CommandType.StoredProcedure;
                cmd5.Parameters.Add(new OdbcParameter("@Number", Convert.ToString(number)));
                cmd5.Parameters.Add(new OdbcParameter("@Type", Type));
                using (OdbcDataAdapter oda = new OdbcDataAdapter(cmd5))
                {
                    oda.Fill(dt1);
                }   
                con3.Close();
            }
            catch (Exception ex)
            {
                string Result = ex.Message;
            }
            return dt1;
        }
    }
    public DataTable ConnectGEtSpGetCtiHistory(string strType, string strCLINumber, string strudfCallMasterTableName)
    {
        using (OdbcConnection con3 = new OdbcConnection(strIntreDia))
        {
            DataTable dt1 = new DataTable();
            OdbcCommand cmd5 = null;
            try
            {
                cmd5 = new OdbcCommand("call idg_sp_gethistory_Inbound (?,?)", con3);
                con3.Open();
                cmd5.CommandType = CommandType.StoredProcedure;                 
                cmd5.Parameters.Add(new OdbcParameter("@Number", strCLINumber));
                cmd5.Parameters.Add(new OdbcParameter("@szCallmasterTable", strudfCallMasterTableName));
                using (OdbcDataAdapter oda = new OdbcDataAdapter(cmd5))
                {
                    oda.Fill(dt1);
                }   
                con3.Close();
            }
            catch (Exception ex)
            {
                string Result = ex.Message;
            }
            return dt1;
        }
    }
    public void GEtAgentNameWithEmailId1(RadComboBox cmb,int userid, int No, int ZeroInsert)
    {
        using (OdbcConnection con3 = new OdbcConnection(strIntreDia))
        {
            DataTable dt = new DataTable();
            try
            {
                OdbcCommand cmd5 = new OdbcCommand(" sp_get_AgentNameEmaildropdown_Value ?", con3);
                con3.Open();
                cmd5.CommandType = CommandType.StoredProcedure;
                cmd5.Parameters.Add(new OdbcParameter("@userid", userid));
                using (OdbcDataAdapter oda = new OdbcDataAdapter(cmd5))
                {
                    oda.Fill(dt);
                    cmb.DataSource = dt;
                    if (No == 1)
                    {
                        cmb.DataValueField = "agent_agent_id";
                        cmb.DataTextField = "agent_name";
                    }
                   
                    cmb.DataBind();
                    if (ZeroInsert > 0)
                    {

                    }
                }
                con3.Close();
            }
            catch (Exception ex)
            {
                string Result = ex.Message;
            }
        }
    }
    public DataTable GetAgentDetail(int AgentId)
    {
        using (OdbcConnection con3 = new OdbcConnection(strIntreDia))
        {
            DataTable dt1 = new DataTable();
            OdbcCommand cmd5 = null;
            try
            {
                cmd5 = new OdbcCommand(" udf_GetAgentStatus ?", con3);
                con3.Open();
                cmd5.CommandType = CommandType.StoredProcedure;
                cmd5.Parameters.Add(new OdbcParameter("@nAgentID", AgentId));
                using (OdbcDataAdapter oda = new OdbcDataAdapter(cmd5))
                {
                    oda.Fill(dt1);
                }
                con3.Close();
            }
            catch (Exception ex)
            {
                string Result = ex.Message;
            }
            return dt1;
        }
    }
}
