using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.Odbc;
using System.Data;
using System.Web.Script.Services;
using System.Configuration;

/// <summary>
/// Summary description for JDService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
[System.Web.Script.Services.ScriptService]
public class JDService : System.Web.Services.WebService
{
    DataBase m_Connection = new DataBase();
    string strTemp = "";
    string strParameters = "";
    string strIPAddress = HttpContext.Current.Request.UserHostAddress;
    OdbcDataAdapter m_DataAdapterOdbc;
    public JDService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    #region PendingCase
    [WebMethod]
    public DataSet PendingCase()
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            OdbcCommand cmd = new OdbcCommand("JD_PendingCases", m_Connection.oCon);
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
            LogMessage(strResult, "PendingCase", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strResult, "PendingCase", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region GetAccount

    [WebMethod]
    public DataSet GetAccounts()
    {
        int num = -1;
        string szMessage = "No record found";
        DataSet dataSet = new DataSet();
        string str2 = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        str2 = Convert.ToString(str2).Substring(0, str2.IndexOf("/"));
        try
        {
            this.m_Connection.OpenDB("Galaxy");
            OdbcCommand selectCommand = new OdbcCommand("JD_GetAccounts", this.m_Connection.oCon)
            {
                CommandType = CommandType.StoredProcedure
            };
            new OdbcDataAdapter(selectCommand).Fill(dataSet);
            dataSet.Tables[0].TableName = "Accounts";
            num = 0;
            szMessage = "Pass";
        }
        catch (OdbcException exception)
        {
            szMessage = exception.Message;
            this.LogMessage(szMessage, "GetAccounts", this.strParameters);
        }
        catch (Exception exception2)
        {
            szMessage = exception2.Message;
            this.LogMessage(szMessage, "GetAccounts", this.strParameters);
        }
        finally
        {
            this.m_Connection.CloseDB();
        }
        dataSet.Tables.Add(this.m_Connection.GetResponseTable((long)num, szMessage));
        return dataSet;
    }


    #endregion

    #region JD_GetCLIDetails
    [WebMethod]

    public DataSet GetJDContactDetails(string strCLI)
    {
        int num = -1;
        string szMessage = "No record found";
        DataSet dataSet = new DataSet();
        string str2 = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        str2 = Convert.ToString(str2).Substring(0, str2.IndexOf("/"));
        try
        {
            this.m_Connection.OpenDB("Galaxy");
            OdbcCommand selectCommand = new OdbcCommand("JD_GetCLIDetails ?", this.m_Connection.oCon)
            {
                CommandType = CommandType.StoredProcedure
            };
            selectCommand.Parameters.Add("@Cli", OdbcType.VarChar).Value = strCLI;
            new OdbcDataAdapter(selectCommand).Fill(dataSet);
            dataSet.Tables[0].TableName = "Accounts";
            num = 0;
            szMessage = "Pass";
        }
        catch (OdbcException exception)
        {
            szMessage = exception.Message;
            this.LogMessage(szMessage, "GetAccounts", this.strParameters);
        }
        catch (Exception exception2)
        {
            szMessage = exception2.Message;
            this.LogMessage(szMessage, "GetAccounts", this.strParameters);
        }
        finally
        {
            this.m_Connection.CloseDB();
        }
        dataSet.Tables.Add(this.m_Connection.GetResponseTable((long)num, szMessage));
        return dataSet;
    }





    #endregion

    #region GetAccountInventories
    [WebMethod]

    public DataSet GetAccountInventories(int AccountID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            //13-12 manju
            //strTemp = "select inv_serial_no, inv_product_name, inv_status, CONVERT(VARCHAR(17), inv_warranty_start_date, 13) AS inv_warranty_start_date," +
            //            "CONVERT(VARCHAR(17), inv_warranty_end_date, 13) AS inv_warranty_end_date,CONVERT(VARCHAR(17), inv_contract_start_date, 13) AS inv_contract_start_date," +
            //            "CONVERT(VARCHAR(17), inv_installation_date, 13) AS inv_installation_date,CONVERT(VARCHAR(17), inv_sale_date, 13) AS inv_sale_date," +
            //            "dbo.GetContactName(created_by) AS created_by,CONVERT(VARCHAR(17), created_date, 13) AS created_date,udf_inv_pms_schedule,udf_inv_pm_kit," +
            //            "udf_hour_meter_reading,CONVERT(VARCHAR(17), udf_machine_expiry_date, 13) AS udf_machine_expiry_date,CONVERT(VARCHAR(17), udf_last_pms_date, 13) AS udf_last_pms_date," +
            //            "CONVERT(VARCHAR(17), udf_next_pms_date, 13) AS udf_next_pms_date,udf_last_pms_status,CONVERT(VARCHAR(17), udf_next_expiry_date, 13) AS udf_next_expiry_date " +
            //            "from crm_inventories where related_to = 'CST' AND related_to_id=" + AccountID;
           
            ////13-12-12 manju
            
                        strTemp = "select id,inv_serial_no, inv_product_name, inv_status, CONVERT(VARCHAR(17), inv_warranty_start_date, 13) AS inv_warranty_start_date," +
                                   "CONVERT(VARCHAR(17), inv_warranty_end_date, 13) AS inv_warranty_end_date,CONVERT(VARCHAR(17), inv_contract_start_date, 13) AS inv_contract_start_date," +
                                   "CONVERT(VARCHAR(17), inv_installation_date, 13) AS inv_installation_date,CONVERT(VARCHAR(17), inv_sale_date, 13) AS inv_sale_date," +
                                   "dbo.GetContactName(created_by) AS created_by,CONVERT(VARCHAR(17), created_date, 13) AS created_date,udf_inv_pms_schedule,udf_inv_pm_kit," +
                                   "udf_hour_meter_reading,CONVERT(VARCHAR(17), udf_machine_expiry_date, 13) AS udf_machine_expiry_date,CONVERT(VARCHAR(17), udf_last_pms_date, 13) AS udf_last_pms_date," +
                                   "CONVERT(VARCHAR(17), udf_next_pms_date, 13) AS udf_next_pms_date,udf_last_pms_status,CONVERT(VARCHAR(17), udf_next_expiry_date, 13) AS udf_next_expiry_date " +
                                   ",remark,[dbo].[Licencedetail_ToolTip](id) as tooltip,case when [dbo].[Licencedetail_ToolTip](id)!=' ' then 'Details' else ' ' end as displaydetail from crm_inventories where related_to = 'CST' AND related_to_id=" + AccountID;
            
         

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "AccountInventories";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "AccountInventories", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "AccountInventories", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion
    
    #region GetCtiCallRequest
    [WebMethod]
    public DataSet GetCtiCallRequest(string AccountID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT * FROM crm_call_details where Status = 'Y' AND Agent_id = '" + AccountID + "' ORDER BY id DESC";


            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "CallDetails";
            nResultCode = 0;
            strResult = "Pass";

            int nId = Convert.ToInt32(m_DataSet.Tables[0].Rows[0]["id"]);
            strTemp = "UPDATE crm_call_details SET Status = 'N' WHERE id = " + nId;
            OdbcCommand m_Command = new OdbcCommand(strTemp, m_Connection.oCon);
            m_Command.ExecuteNonQuery();
            
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "CallDetails", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "CallDetails", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion


    #region SetCallParameter
    [WebMethod]
    public DataSet SaveCallRequestParameters(string strCLI, string strAgentId, string strURL)
    {
        int nResultCode = -1;
        string strResult = "Error";
        DataSet m_DataSet = new DataSet();
        
        try
        {

            m_Connection.OpenDB("Galaxy");
            OdbcCommand cmd = new OdbcCommand("EXEC sp_SetCallParameters ?,?,?", m_Connection.oCon);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@CLI", OdbcType.VarChar).Value = strCLI;
            cmd.Parameters.Add("@Agent_id", OdbcType.VarChar).Value = strAgentId;
            cmd.Parameters.Add("@Url", OdbcType.VarChar).Value = strURL;
            
            nResultCode = cmd.ExecuteNonQuery();
            strResult = "Pass";
            
        }
        catch (OdbcException ex)
        {
            strResult = ex.Message;
            LogMessage(strResult, "SaveCallRequestParameters", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strResult, "SaveCallRequestParameters", strParameters);
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

   // 18-12 add for lience manju

    #region  Getting All INV_Licence Cities
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
                     "FROM crm_cities where city_state_code='" + strStateCode + "'";

            //strTemp = "SELECT ISNULL(city_name ,'') as city_name FROM crm_cities order by city_name ASC ";

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

    #region  Getting All States Location
    [WebMethod]
    public DataSet FetchLocation(string strCountryCode)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");
            // strTemp = "SELECT ISNULL(state_name ,'') as state_name FROM CRM_States order by state_name ASC ";

            strTemp = "SELECT " + m_Connection.DB_NULL + "(state_code,'') as state_code," +
                    m_Connection.DB_NULL + "(state_name,'') as state_name," +
                    m_Connection.DB_NULL + "(state_country_code,'') as state_country_code " +
                    "FROM CRM_States  ";

            //strTemp = "SELECT " + m_Connection.DB_NULL + "(state_code,'') as state_code," +
            //          m_Connection.DB_NULL + "(state_name,'') as state_name," +
            //          "FROM CRM_States ";


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
    [WebMethod]
    public DataSet fetchLicense(int id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;

        try
        {
            m_Connection.OpenDB("Galaxy");


            strTemp = "SELECT * FROM [crm_License_info] where related_to_id=" + id;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_License_info";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchLicense", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchLicense", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }

    #endregion


  //  end
    //25-12

    [WebMethod]
    public DataSet GetAgentAccounts(string related_to)
    {
        int num = -1;
        string szMessage = "No record found";
        DataSet dataSet = new DataSet();
        string str2 = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        str2 = Convert.ToString(str2).Substring(0, str2.IndexOf("/"));
        try
        {
            this.m_Connection.OpenDB("Galaxy");
            OdbcCommand selectCommand = new OdbcCommand("Agent_GetAccounts ?", this.m_Connection.oCon)
            {
                CommandType = CommandType.StoredProcedure
            };
            selectCommand.Parameters.Add("@id", OdbcType.VarChar).Value = int.Parse(related_to);
            new OdbcDataAdapter(selectCommand).Fill(dataSet);
            dataSet.Tables[0].TableName = "Accounts";
            num = 0;
            szMessage = "Pass";
        
            
        }
        catch (OdbcException exception)
        {
            szMessage = exception.Message;
            this.LogMessage(szMessage, "GetAgentAccounts", this.strParameters);
        }
        catch (Exception exception2)
        {
            szMessage = exception2.Message;
            this.LogMessage(szMessage, "GetAgentAccounts", this.strParameters);
        }
        finally
        {
            this.m_Connection.CloseDB();
        }
        dataSet.Tables.Add(this.m_Connection.GetResponseTable((long)num, szMessage));
        return dataSet;
    }


    [WebMethod]

    public DataSet GetAgentContactDetails(string strCLI)
    {
        int num = -1;
        string szMessage = "No record found";
        DataSet dataSet = new DataSet();
        string str2 = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        str2 = Convert.ToString(str2).Substring(0, str2.IndexOf("/"));
        try
        {
            this.m_Connection.OpenDB("Galaxy");
            OdbcCommand selectCommand = new OdbcCommand("Agent_GetCLIDetails ?", this.m_Connection.oCon)
            {
                CommandType = CommandType.StoredProcedure
            };
            selectCommand.Parameters.Add("@Cli", OdbcType.VarChar).Value = strCLI;
            new OdbcDataAdapter(selectCommand).Fill(dataSet);
            dataSet.Tables[0].TableName = "Accounts";
            num = 0;
            szMessage = "Pass";
        }
        catch (OdbcException exception)
        {
            szMessage = exception.Message;
            this.LogMessage(szMessage, "GetAccounts", this.strParameters);
        }
        catch (Exception exception2)
        {
            szMessage = exception2.Message;
            this.LogMessage(szMessage, "GetAccounts", this.strParameters);
        }
        finally
        {
            this.m_Connection.CloseDB();
        }
        dataSet.Tables.Add(this.m_Connection.GetResponseTable((long)num, szMessage));
        return dataSet;
    }

    #region AgentPendingCase
    [WebMethod]
    public DataSet AgentPendingCase(string relatedto)
    {
        int nResultCode = -1;
        string strResult = "No record found";
        DataSet m_DataSet = new DataSet();
        try
        {

            m_Connection.OpenDB("Galaxy");
            
            OdbcCommand selectCommand = new OdbcCommand("Agent_PendingCases ?", this.m_Connection.oCon)
            {
                CommandType = CommandType.StoredProcedure
            };
            selectCommand.Parameters.Add("@id", OdbcType.VarChar).Value = int.Parse(relatedto);
            new OdbcDataAdapter(selectCommand).Fill(m_DataSet);
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
            LogMessage(strResult, "AgentPendingCase", strParameters);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
            LogMessage(strResult, "AgentPendingCase", strParameters);
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




