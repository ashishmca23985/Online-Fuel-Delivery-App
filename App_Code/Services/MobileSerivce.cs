using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Data.Odbc;
using System.Data;
using System.Web.Script.Services;
using System.Configuration;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using System.Xml;
using System.IO;
using System.Net;
using System.Drawing;
using System.IO.Compression;
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class MobileSerivce : System.Web.Services.WebService
{
    CreateLog log = new CreateLog();
    string strResult = "Fail";
    string Message = "";
    string strTemp = string.Empty;
    DataBase m_Connection = new DataBase();
    
    OdbcDataAdapter m_DataAdapterOdbc;
    OdbcDataAdapter m_DataAdapterOdbc2;
    OdbcCommand m_CommandODBC;

    public MobileSerivce()
    {
    }
  
    #region Task Document Count
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetTicketDocCount(string szAPIKey, string szDeviceType, int UserId, string TicketId, string TicketNumber)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        DataTable m_DataTable = new DataTable("document_count");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        strResult = CheckValidation(szAPIKey, szDeviceType, UserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            OdbcDataAdapter m_DataAdapter1;
            
            strTemp = "SELECT count(*) as TicketDocCount FROM crm_documents where related_to='CAS' AND (related_to_id =" + TicketId + " or related_to_name='" + TicketNumber + "')";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter1 = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter1.Fill(m_DataTable);
            string DocCount = m_DataTable.Rows[0]["TicketDocCount"].ToString();
            if (m_DataTable.Rows.Count==0)
            {
                nResultCode = -1;
                strResult = "No Docuement are avaiable";
                this.Context.Response.Write(serializer.Serialize(new { TicketDocuCount = strResult }));
                return;
            }
            else
            {
                foreach (DataRow dr in m_DataTable.Rows)
                {
                    row = new Dictionary<string, object>();
                    row.Add("DocCount", DocCount);

                    foreach (DataColumn col in m_DataTable.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    rows.Add(row);
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
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { TicketDocCount = rows}));
        }
    }
    #endregion
    
    #region Validate User Login and Password
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void ValidateLogin(string strLoginId, string strPwd, string szDeviceType )
    {
        Writelog.WriteError("ValidateLogin");
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        m_DataAdapterOdbc2 = null;
        DataTable dtTable = new DataTable("Login");
        DataTable dtActivities = new DataTable("Activities");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        if (szDeviceType == "")
        {
            nResultCode = -1;
            strResult = "Invalid Device Type";
            this.Context.Response.Write(serializer.Serialize(new { LoginInforamtion = strResult }));
            return;
        }
       
       try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " +
                             m_Connection.DB_NULL + "(contact_role_id,0) As contact_role ," +
                             "id as contact_id," +
                             "conttact_mobile_enable," +
                             "contact_enabled," +
                             "contact_login_id," +
                             "isnull(contact_emailid,'')contact_emailid," +
                             "isnull(contact_phone,'')contact_phone," +
                             "dbo.GetVehicleNo(id) as vehicle_no," +
                             "dbo.GetDriverRegion(id) as driver_region," +
                             "dbo.GetVehicleType(id) as vehicle_type," +
                             "contact_full_name " +
                             "FROM CRM_contacts " +
                             "WHERE (contact_login_id='" + strLoginId + "' or contact_phone ='"+ strLoginId + "' or contact_emailid ='" + strLoginId + "') " +
                             "AND contact_password='" + strPwd + "' " +
                             "AND " + m_Connection.DB_NULL + "(contact_role_id,0)=9 " +
                             "AND contact_enabled='Y' ";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(dtTable);
            if (dtTable.Rows.Count == 0)
            {
                nResultCode = -1;
                strResult = "Invalid user or password";
                this.Context.Response.Write(serializer.Serialize(new { LoginStatus = strResult }));
                return;
                //return JsonConvert.SerializeObject(new { UserInformation = strResult });
            }
            else if (dtTable.Rows[0]["contact_role"].ToString() == "")
            {
                nResultCode = -1;
                strResult = "No role assigned to user";
                this.Context.Response.Write(serializer.Serialize(new { LoginInforamtion = strResult }));
            }
            nResultCode = 0;
            OdbcCommand m_CommandODBC = new OdbcCommand();

            strTemp = "SELECT count(contact_device_api_key) " +
                    "FROM CRM_contact_registration " +
                    "WHERE contact_device_type = '" + szDeviceType + "' " +
                    " AND contact_id = " + dtTable.Rows[0]["contact_id"].ToString() + "";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            string count1 = m_CommandODBC.ExecuteScalar().ToString();
            if (count1.ToString() == "1")
            {
                strTemp = "delete from crm_contact_registration WHERE contact_device_type = '" + szDeviceType + "' " +
                    " AND contact_id = " + dtTable.Rows[0]["contact_id"].ToString() + "";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.CommandType = CommandType.Text;
                m_CommandODBC.ExecuteNonQuery();
            }

            strTemp = "update crm_contact_registration set " +
                        "contact_device_api_key = newid() " +
                        "where contact_id = " + dtTable.Rows[0]["contact_id"].ToString() + " " +
                        "and contact_device_type = '" + szDeviceType + "' ";
                      
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            int nRows = m_CommandODBC.ExecuteNonQuery();
            m_CommandODBC.Dispose();
            if (nRows <= 0)
            {
                strTemp = "INSERT INTO crm_contact_registration (contact_id, contact_device_type, " +
                            "contact_device_api_key) " +
                            "SELECT " + dtTable.Rows[0]["contact_id"].ToString() + ", '" +
                            szDeviceType + "', newid() ";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.CommandType = CommandType.Text;
                nRows = m_CommandODBC.ExecuteNonQuery();
                m_CommandODBC.Dispose();
            }

            strTemp = "select isnull((SELECT top 1 contact_device_api_key " +
                  "FROM CRM_contact_registration " +
                  "WHERE contact_device_type = '" + szDeviceType + "' " +
                  " AND contact_id = " + dtTable.Rows[0]["contact_id"].ToString() +
                  "),'') contact_device_api_key";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            string szAPIKey = m_CommandODBC.ExecuteScalar().ToString();
            m_CommandODBC.Dispose();
            if (szAPIKey == "")
            {
                nResultCode = -1;
                strResult = "Unable to generate API Key";
                this.Context.Response.Write(serializer.Serialize(new { LoginInforamtion = strResult }));
                return;
            }
            foreach (DataRow dr in dtTable.Rows)
            {
                row = new Dictionary<string, object>();
                row.Add("APIKEY", szAPIKey);

                foreach (DataColumn col in dtTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            //  return activities_list

            strTemp = "SELECT roledet_activity_id , roledet_permission_level " +
                        "From crm_role_activity " +
                        "WHERE roledet_role_id = " + dtTable.Rows[0]["contact_role"].ToString() + " " +
                        "AND roledet_activity_id in (select activity_id " +
                        "from crm_activity_master )"; //where activity_mobile = 'Y'
            m_DataAdapterOdbc2 = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc2.Fill(dtActivities);
            List<object> obList = new List<object>();
            foreach (DataRow dr in dtActivities.Rows)
            {
                var ob = new
                {
                    ActivityId = dr["roledet_activity_id"].ToString(),
                    lavel = dr["roledet_permission_level"].ToString()
                };
                obList.Add(ob);
            }
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { LoginInforamtion = rows, Activity = obList }));
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
            if (m_DataAdapterOdbc != null)
                m_DataAdapterOdbc.Dispose();
            if (m_DataAdapterOdbc2 != null)
                m_DataAdapterOdbc2.Dispose();
            dtTable.Dispose();
            dtActivities.Dispose();

            m_Connection.CloseDB();
        }
    }
    #endregion

    #region Get N closed Cases List
    [WebMethod]
    public void GetClosedTicketlists(string szAPIKey, string szDeviceType, int UserId) //string szDateTime, int Days
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("cases");
        DataTable m_DataTable1 = new DataTable("ClodesCases");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        serializer.MaxJsonLength = Int32.MaxValue;
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType, UserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        ArrayList closedrows = new ArrayList();
        try
        {
            m_Connection.OpenDB("Galaxy");
            OdbcDataAdapter m_DataAdapter;
            string temp = @"select id, transaction_number as TicketNo
                                      ,owner_id
                                      ,dbo.GetContactName(assign_to_id) as AssignTo
                                      ,created_by
                                      ,dbo.GetContactName(created_by) as created_by_name
                                      ,dbo.GetSourceName(case_source) as case_source
                                      ,case_caller_id
                                      ,case_caller_name
                                      ,cust_site_name as SiteName,cust_vehicleno as VehicleNo,cust_carmodel as CarModel
                                      ,dbo.GetContactAddress(case_caller_id) as cust_location
                                      ,dbo.GetCustPhone(case_caller_id) as case_caller_phone
                                      ,dbo.GetCustEmail(case_caller_id) as case_caller_emailcrm_cases
                                      ,case_severity_desc
                                      ,case_subject
                                      ,case_status_id
                                      ,case_status_name
                                      ,cust_fule_type,FuelfilledQty AS cust_fule_qty,Fuel_rate,Fuel_cost,final_Fuel_cost,fuel_tax as CommercialTax,Fuel_tax_amount,TotalSiteExpenseAmt,
                                       cust_paymentmode,cust_latitute,cust_longitute,cust_req_type,cust_filling_type,cust_filling_contactno,cust_loc_permission,cust_approchaccess
                                      ,convert(varchar(25),dbo.getLocalZoneTime(open_time,330),113) as open_time
                                      ,convert(varchar(25),dbo.getLocalZoneTime(end_time,330),113) as end_time
                                      ,end_remarks
                                      ,created_ip
                                      ,convert(varchar(25),dbo.getLocalZoneTime(created_date,330),113) as created_date
                                      ,convert(varchar(25),dbo.getLocalZoneTime(first_open_time,330),113) as first_open_time
                                      FROM crm_cases 
                                      where assign_to_id = " + UserId + @" and  Useable='Y' and case_status_id in(2) ORDER BY end_time DESC";

            m_CommandODBC = new OdbcCommand(temp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
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
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { TicketClosedList = rows, closedrows = closedrows, error = strResult }));
        }
    }
    #endregion

    #region Get Open Ticket List
    [WebMethod]
    public void GetOpenTicketList(string szAPIKey, string szDeviceType, int UserId) //, string szDateTime
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("cases");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        serializer.MaxJsonLength = Int32.MaxValue;
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType, UserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            OdbcDataAdapter m_DataAdapter;

            string temp = @"select id, transaction_number as TicketNo
                                      ,owner_id
                                      ,dbo.GetContactName(owner_id) as owner_id_name
                                      ,created_by
                                      ,dbo.GetContactName(created_by) as created_by_name
                                      ,dbo.GetSourceName(case_source) as case_source
                                      ,case_caller_id
                                      ,case_caller_name
                                      ,cust_site_name as SiteName,cust_vehicleno as VehicleNo,cust_carmodel as CarModel
                                      ,dbo.GetContactAddress(case_caller_id) as cust_location
                                      ,dbo.GetCustPhone(case_caller_id) as case_caller_phone
                                      ,dbo.GetCustEmail(case_caller_id) as case_caller_email
                                      ,case_severity_desc
                                      ,case_subject
                                      ,case_status_id
                                      ,case_status_name
                                       ,cust_fule_type,cust_fule_qty,Fuel_rate,Fuel_cost,fuel_tax as CommercialTax,Fuel_tax_amount,cust_paymentmode,cust_latitute,cust_longitute,cust_req_type,cust_filling_type,cust_filling_contactno,cust_loc_permission,cust_approchaccess
                                     ,convert(varchar(25),dbo.getLocalZoneTime(open_time,330),113) as open_time
                                      ,created_ip
                                      ,convert(varchar(25),dbo.getLocalZoneTime(created_date,330),113) as created_date
                                      
                                      FROM crm_cases 
                                      where assign_to_id = " + UserId + @" and  Useable='Y' and case_status_id in(1,3)";
          
            m_CommandODBC = new OdbcCommand(temp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
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
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { TicketOpenList = rows, error = strResult }));
        }
    }
    #endregion

    //#region Update Ticket status is case first time open
    //[WebMethod]
    //public void UpdateTicketStatus(string szAPIKey, string szDeviceType, int UserId, int TicketId, int statusId, string status)
    //{
    //    int nResultCode = -1;
    //    string strResult = "Fail";
    //    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
    //    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
    //    strResult = CheckValidation(szAPIKey, szDeviceType, UserId);
    //    if (strResult != "PASS")
    //    {
    //        this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
    //        return;
    //    }
    //    try
    //    {
    //        string query = @"UPDATE crm_cases SET case_status_id= " + statusId +
    //                        ",case_status_name='" + status + "'" +
    //                        ",first_open_time='" + DateTime.UtcNow.ToString("dd MMM yyyy HH:mm") + "'" +
    //                    ",useable='Y',last_activity_date=getutcdate(),modified_date=getutcdate(),modified_ip='" +
    //            System.Web.HttpContext.Current.Request.UserHostAddress + "',modified_by=" + UserId + " WHERE id=" + TicketId;
    //        m_CommandODBC = new OdbcCommand(query, m_Connection.oCon);
    //        m_CommandODBC.CommandType = CommandType.Text;
    //        int nRows = m_CommandODBC.ExecuteNonQuery();
    //        if (nRows == 0)
    //        {
    //            nResultCode = 0;
    //            strResult = "Ticket not found";
    //        }
    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;

    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //        this.Context.Response.ContentType = "application/json; charset=utf-8";
    //        this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
    //    }
    //}
    //#endregion

    public string CheckValidation(string szAPIKey, string szDeviceType, int UserId)
    {
        Writelog.WriteError("CheckValidation");
        int nResultCode = -1;
        string strResult = "Fail";
        
        if (UserId <= 0)
        {
            nResultCode = -1;
            strResult = "Invalid User ID";
            return strResult;
        }
       
        return "PASS";
    }

    public string errmsg(Exception ex)
    {
        return "[['ERROR','" + ex.Message + "']]";
    }

    #region Get Recent Activity
    public string SaveRecentActivity(int itemid, string ObjectType, int ObjectId, string ObjectAction, string ObjectName, int UserId, string ObjectStatus, int ActivityId, string HistoryText)
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
                m_CommandODBC = new OdbcCommand(strQuery, m_Connection.oCon);
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
                                "VALUES('" + ObjectType + "'," + ObjectId + ",'" + ObjectAction + "', ?," + UserId + ",'" + ObjectStatus + "',getutcdate(),getutcdate()," + ActivityId + ",'" + HistoryText + "')";
            else
                strQuery = "UPDATE CRM_RECENT_ITEMS SET recent_item_status = '" + ObjectStatus + "'," +
                                "recent_item_end_time = getutcdate()," +
                                "recent_item_name = ?," +
                                "recent_item_action = (CASE WHEN recent_item_action = 'New' THEN 'Created' ELSE 'Modified' END), " +
                                "recent_item_history_text = '" + HistoryText + "' " +
                                "WHERE recent_item_id = " + itemid;
            m_CommandODBC = new OdbcCommand(strQuery, m_Connection.oCon);
            m_CommandODBC.Parameters.Add("recent_item_name", OdbcType.VarChar).Value = ObjectName;
            nRowCount = m_CommandODBC.ExecuteNonQuery();
            if (nRowCount > 0)
            {
                nResultCode = 0;
                if (itemid == 0)
                {
                    strQuery = "SELECT @@IDENTITY";
                    m_CommandODBC = new OdbcCommand(strQuery, m_Connection.oCon);
                    itemid = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
                    nResultCode = itemid;
                }
                strResult = "PASS";
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
        return strResult;
    }
    #endregion
 
    #region Update Ticket Status
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void TicketClose(string szAPIKey, string szDeviceType, int UserId, string TicketId, string Remarks,string FuelfilledQty)
    {
        string temp = "";
        OdbcDataAdapter da;
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Tasks");
        DataTable m_DataTable1 = new DataTable("Status");
        OdbcDataAdapter m_DataAdapter1;
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType, UserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        ArrayList closedrows = new ArrayList();
        try
        {
            m_Connection.OpenDB("Galaxy");

            temp = "select count(*) as Count from crm_cases where id = '" + TicketId + "' and case_status_id=2";
            m_CommandODBC = new OdbcCommand(temp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter1 = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter1.Fill(m_DataTable);
            int nRowDupCount = Convert.ToInt32(m_DataTable.Rows[0]["Count"]);          
            if (nRowDupCount > 0)
            {
                nResultCode = 1;
                strResult = "Alreday Closed Ticket !";
                this.Context.Response.ContentType = "application/json; charset=utf-8";
                this.Context.Response.Write(serializer.Serialize(new { CaseUpdate = Message, Result = strResult }));
                return;
            }
            else
            {
                temp = "update crm_cases set case_status_id=2,case_status_name='CLOSE',end_remarks='" + Remarks + "',end_time=GETUTCDATE() where id='" + TicketId + "' ";
                string res = ExecutenonQuery(temp);
                if (res == "1")
                {
                    m_Connection.OpenDB("Galaxy");
                    string strQuery = "INSERT INTO CRM_RECENT_ITEMS(recent_item_object_type," +
                              "recent_item_object_id," +
                              "recent_item_action," +
                              "recent_item_name," +
                              "recent_item_contact_id," +
                              "recent_item_status," +
                              "recent_item_start_time," +
                              "recent_item_end_time," +
                              "recent_item_activity_id," +
                              "recent_item_history_text) " +
                              "VALUES('CAS' ," + TicketId + ",'TICKETCLOSED','" + Remarks + "' ," + UserId + ",'P',GETUTCDATE(),GETUTCDATE(),'2409','Ticket Closed Successfully')";
                    m_CommandODBC = new OdbcCommand(strQuery, m_Connection.oCon);
                    int nRowCount = m_CommandODBC.ExecuteNonQuery();
                    if (nRowCount > 0)
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
                            "recent_item_history_text) " +
                            "VALUES('CAS' ," + TicketId + ",'FUELDELIVERED', 'Fuel Delevired'," + UserId + ",'P',GETUTCDATE(),GETUTCDATE(),'2409','Fuel Delevired Successfully')";
                        m_CommandODBC = new OdbcCommand(strQuery, m_Connection.oCon);
                        int nRowDlrCount = m_CommandODBC.ExecuteNonQuery();
                        if (nRowDlrCount > 0)
                        {
                            int nFuelQtyBal = 0;
                            if (FuelfilledQty != "")
                            {
                                string sQuery = "SELECT #GetDriverFuelQty('" + UserId.ToString() + "')";
                                int strFuelQty = Convert.ToInt32(ExecuteQuery(sQuery));
                                if (strFuelQty > 0)
                                {
                                    nFuelQtyBal = Convert.ToInt32(strFuelQty) - Convert.ToInt32(FuelfilledQty);
                                }
                                else
                                {
                                    strResult = "Pass";
                                    Message = "No Fuel available";
                                    this.Context.Response.ContentType = "application/json; charset=utf-8";
                                    this.Context.Response.Write(serializer.Serialize(new { CaseUpdate = Message, Result = strResult, nFuelQtyBalance = Convert.ToString(nFuelQtyBal) }));
                                }
                                // string UpdateFuelMaster = "update crm_fuel_inward_entry set inward_fuel_rifillqty= " + nFuelQtyBal + " where inward_driver_id='" + UserId + "' ";
                                // string nFuelQtyBalance = ExecutenonQuery(UpdateFuelMaster);
                            }
                            strResult = "Pass";
                            Message = "Ticket Closed Successfully";
                            this.Context.Response.ContentType = "application/json; charset=utf-8";
                            this.Context.Response.Write(serializer.Serialize(new { CaseUpdate = Message, Result = strResult, nFuelQtyBalance = Convert.ToString(nFuelQtyBal) }));
                            m_Connection.CloseDB();
                        }
                    }
                }
                else
                {
                    Message = "Ticket Close Updation Failed";
                    this.Context.Response.ContentType = "application/json; charset=utf-8";
                    this.Context.Response.Write(serializer.Serialize(new { CaseUpdate = Message, Result = strResult }));
                    m_Connection.CloseDB();

                }
            }
            

        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
        }
    }
    #endregion

    public string ExecuteQuery(string strQuery)
    {
        string strResult = "";
        try
        {
            // m_Connection.OpenDB("Galaxy");
            // m_Connection.BeginTransaction();
            strQuery = strQuery.Replace("#", m_Connection.DB_FUNCTION);
            OdbcCommand m_CommandOdbc = new OdbcCommand(strQuery, m_Connection.oCon);
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

            //m_Connection.CloseDB();
            // m_Connection.Commit();
        }
        return strResult;
    }

    public string ExecutenonQuery(string strQuery)
    {
        string strResult = "";
        try
        {
            m_Connection.OpenDB("Galaxy");
            strQuery = strQuery.Replace("#", m_Connection.DB_FUNCTION);
            OdbcCommand m_CommandOdbc = new OdbcCommand(strQuery, m_Connection.oCon);
            strResult = Convert.ToString(m_CommandOdbc.ExecuteNonQuery());
        }
        catch (OdbcException ex)
        {

        }
        catch (Exception ex)
        {
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
            szMessage = "MobileSerivce.cs - " + szMethodName +
                        "(" + szMethodParams + ") " + szMessage;

            objCreateLog.ErrorLog(szMessage);
        }
        catch (Exception ex)
        {
            string str = ex.Message;
        }
    }
    #endregion

 
    #region Get Ticket Status
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetTicketStatus(string szAPIKey, string szDeviceType, int strUserId)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Case");
        DataTable m_DataTable1 = new DataTable("GetStatus");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;

        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(status_name,'') as status_name," +
                           m_Connection.DB_NULL + "(status_id,0) as status_id " +
                           "FROM CRM_Status WHERE status_for ='CAS' ORDER BY status_sequence ASC ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataAdapter da = new OdbcDataAdapter(m_CommandODBC);
            da.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GetStatus = rows, error = strResult }));
        }
    }
    #endregion


    #region Genrate Signature
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GenerateSignature(string szAPIKey, string szDeviceType, int strUserId, int TicketId, string TicketNumber, string strbyte, string extension)
    {
        DataTable m_DataTable = new DataTable("Ticket");
        int nResultCode = -1;
        string strResult = "Fail";
        string query = "";
        string FileName = "";
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        if (TicketNumber == null && strbyte.Length > 0)
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");

            byte[] bytesToCompress = Convert.FromBase64String(strbyte);
            byte[] compress = Compress(bytesToCompress);
            string FilePath = System.Configuration.ConfigurationManager.AppSettings["Filepath"];
            string path = "";
            FilePath += @"CustomerSignature \" + DateTime.Now.ToString("dd-MM-yyyy") + @"\";
            FileName = Convert.ToString(TicketId) + "_" + "Cust" + "." + extension;
            path = FilePath + FileName;
            query = "update crm_cases set case_cust_Sig_Path='" + FilePath + "' where id=" + TicketId + " ";
            m_CommandODBC = new OdbcCommand(query, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();
            if (nRowCount > 0)
            {
                string query2 = "UPDATE crm_cases set custsig_upload_status= 'Y'" +
                                    ", custsign_upload_date= GetUTCDATE() WHERE id = " + TicketId;
                m_CommandODBC = new OdbcCommand(query2, m_Connection.oCon);
                int nRow2Count = m_CommandODBC.ExecuteNonQuery();
            }
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            FilePath += FileName; ;
         //   sw.WriteLine(FilePathN);
         //   sw.WriteLine(query);
            File.WriteAllBytes(FilePath, Convert.FromBase64String(strbyte));
          //  sw.WriteLine("OK");
          //  sw.WriteLine("OK1");
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
           // Writelog.WriteError(ex.Message);
           // sw.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
           // Writelog.WriteError(ex.Message);
          //  sw.WriteLine(ex.Message);
        }
        finally
        {
           // sw.Flush();
           // sw.Close();
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GenerateSignature = rows, error = strResult }));
        }
    }
    #endregion


    //#region Genrate Signature
    //[WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public void GenerateSignature(string szAPIKey, string szDeviceType, int strUserId, int TicketId, string TicketNumber, string SignImgFileName, string FileExtension)
    //{
    //    DataTable m_DataTable = new DataTable("Ticket");
    //    int nResultCode = -1;
    //    string strResult = "Fail";
    //    string query = "";
    //    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
    //    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
    //    Dictionary<string, object> row = null;
    //    strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
    //    if (strResult != "PASS")
    //    {
    //        this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
    //        return;
    //    }
    //    if (TicketNumber == null && SignImgFileName.Length > 0)
    //    {
    //        this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
    //        return;
    //    }
    //    try
    //    {
    //        m_Connection.OpenDB("Galaxy");
    //        FileExtension = "." + FileExtension;
    //        string strFilePath = System.Configuration.ConfigurationManager.AppSettings["Filepath"];
    //        string FilePath = strFilePath + SignImgFileName;
    //        query = "update crm_cases set case_cust_Sig_Path='" + FilePath + "' where id=" + TicketId + " ";
    //        m_CommandODBC = new OdbcCommand(query, m_Connection.oCon);
    //        int nRowCount = m_CommandODBC.ExecuteNonQuery();
    //        if (nRowCount > 0)
    //        {
    //            string query2 = "UPDATE crm_cases set custsig_upload_status= 'Y'" +
    //                                ", custsign_upload_date= GetUTCDATE() WHERE id = " + TicketId;
    //            m_CommandODBC = new OdbcCommand(query2, m_Connection.oCon);
    //            int nRow2Count = m_CommandODBC.ExecuteNonQuery();
    //        }
    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //    }
    //    catch (Exception ex)
    //    {
    //        strResult = ex.Message;
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //        this.Context.Response.ContentType = "application/json; charset=utf-8";
    //        this.Context.Response.Write(serializer.Serialize(new { GenerateSignature = rows, error = strResult }));
    //    }
    //}
    //#endregion

    public static byte[] Compress(byte[] raw)
    {
        using (var memory = new MemoryStream())
        {
            using (var gzip = new GZipStream(memory, CompressionMode.Compress, true))
            {
                gzip.Write(raw, 0, raw.Length);
            }
            return memory.ToArray();
        }
    }
  

    #region Get Ticket Documents List
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetTicketDocList(string szAPIKey, string szDeviceType, int strUserId, string TicketId,string TicketNo)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Srf");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType,  strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = @"SELECT [id] as DocId
                      ,[related_to_name] as TicketNo
                      ,[document_name] as DocName
                      ,[document_file_name] as DocFileName
                      ,[document_path] as FilePath
                      ,[document_description] as DocDiscription
                      ,convert(varchar(25),created_date, 113) as DocUplaodDate
                      FROM  [crm_documents] where Useable='Y' AND (related_to_id = '" + TicketId+ "' or related_to_name='"+ TicketNo + "')" ;
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataAdapter da = new OdbcDataAdapter(m_CommandODBC);
            da.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GetTicketDocList = rows, error = strResult }));
        }
    }
    #endregion

    #region Get Customer Ticket Signature
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetCustSignature(string szAPIKey, string szDeviceType, int strUserId,int TicketId)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        string base64 = "";
        string filePath = "";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Srf");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType,strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = @"SELECT case_cust_Sig_Path from crm_cases where id=" + TicketId + "";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            filePath = m_CommandODBC.ExecuteScalar().ToString();
            if (filePath.Length > 0)
            {
                byte[] array = File.ReadAllBytes(filePath);
                base64 = Convert.ToBase64String(array);
            }
            else
                strResult = "No signature exists";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        catch (Exception e)
        {
            strResult = e.Message;

        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GetCustSignature = base64, error = strResult }));
        }
    }
    #endregion

    public static byte[] ImageToByteArray(string imagefilePath)
    {
        System.Drawing.Image image = System.Drawing.Image.FromFile(imagefilePath);
        byte[] imageByte = ImageToByteArraybyImageConverter(image);
        return imageByte;
    }

    private static byte[] ImageToByteArraybyImageConverter(System.Drawing.Image image)
    {
        ImageConverter imageConverter = new ImageConverter();
        byte[] imageByte = (byte[])imageConverter.ConvertTo(image, typeof(byte[]));
        return imageByte;
    }

    #region Retrieve Documents bytes
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetRelatedDocument(string szAPIKey, string szDeviceType, int UserId, int DocumentId)
    {
        int nResultCode = -1;
        string filePath = "";
        string fileName = "";
        string fileExtension = "";
        string strResult = "Fail";
        string base64 = "";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("cases");
        DataTable m_DataTable1 = new DataTable("Categories");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        serializer.MaxJsonLength = Int32.MaxValue;
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType , UserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        ArrayList closedrows = new ArrayList();
        try
        {
            m_Connection.OpenDB("Galaxy");
            string temp = "select document_path AS DocPath,document_mime_type as DocType,document_extension as DocExtension," +
                "document_file_name as DocFileName from crm_documents where Useable ='Y' AND id=" + DocumentId + "";
            m_CommandODBC = new OdbcCommand(temp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataAdapter da = new OdbcDataAdapter(m_CommandODBC);
            da.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            filePath = m_DataSet.Tables[0].Rows[0]["DocPath"].ToString();
            fileName = m_DataSet.Tables[0].Rows[0]["DocFileName"].ToString();
            fileExtension = m_DataSet.Tables[0].Rows[0]["DocExtension"].ToString();
            if (!File.Exists(filePath))
            {
                strResult = "File does not exists";
                return;
            }

            byte[] array = File.ReadAllBytes(filePath);
            base64 = Convert.ToBase64String(array);
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        catch (Exception e)
        {
            strResult = e.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { RetrieveDocumentsbytes = base64, fileName, fileExtension, error = strResult }));
        }
    }
    #endregion

    #region Update Driver Location based on thier Ticket 
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void UpdateDriverLocation(string szAPIKey, string szDeviceType, int UserId, string TicketNumber ,string VehicleNo,
                                      string DriverCurrentLocName,string Laltitue, string Longitute)
    {
      
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Tasks");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType,UserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "UPDATE crm_driverlocation SET driver_currentpinlocation= '" + DriverCurrentLocName + "', vehicle_no = '" + VehicleNo + "', latitute='" + Laltitue + "' ,longititute='" + Longitute + "' , updatedate = GETDATE(),TicketId ='" + TicketNumber + "' where driver_id='" + UserId + "' ";

            //strTemp = "UPDATE crm_driverlocation SET driver_currentpinlocation= '" + DriverCurrentLocName + "', vehicle_no = '" + VehicleNo + "', latitute='" + Laltitue + "' ,longititute='" + Longitute + "' , updatedate = GETDATE(),TicketId ='" + TicketNumber + "' where TicketId='" + TicketNumber + "' and  (driver_id='" + UserId + "') ";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            int nRows = m_CommandODBC.ExecuteNonQuery();
            if (nRows < 0)
            {
                nResultCode = -1;
                strResult = "unable to update driver location";
            }
            else
            {
                nResultCode = 1;
                strResult = "update driver location successfully.";
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
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { UpdateDriverLocation = rows, error = strResult }));
        }
    }
    #endregion

    #region Get Driver Current Location
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

    public void GetDriverCurrentLocation(string szAPIKey, string szDeviceType, int UserId, string VehicleNo,
                                      string DriverCurrentLocName, string Laltitue, string Longitute)
    {
      
        OdbcDataAdapter da;
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Tasks");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType , UserId);
        if (strResult != "PASS") 
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "select COUNT(*) AS Ncount from crm_driverlocation where driver_id = '" + UserId + "'";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            int count1 = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
            
            if (count1 > 0)
            {
                strTemp = "UPDATE crm_driverlocation SET driver_currentpinlocation= '" + DriverCurrentLocName + "', vehicle_no = '" + VehicleNo + "', latitute='" + Laltitue + "' ,longititute='" + Longitute + "' , updatedate = GETDATE() where driver_id='" + UserId + "' ";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.CommandType = CommandType.Text;
                int nRows = m_CommandODBC.ExecuteNonQuery();
                if (nRows < 0)
                {
                    nResultCode = -1;
                    strResult = "unable to update driver location";
                    return;
                }
                else
                {
                    nResultCode = 1;
                    strResult = "update driver location successfully.";
                    return;
                }
            }
            else
            {
                strTemp = "INSERT INTO crm_driverlocation (driver_id, driver_name, vehicle_no,latitute,longititute,discription,updatedate) " +
                             "values('" + UserId + "',dbo.GetContactName('" + UserId + "'),'" + VehicleNo + "','" + Laltitue + "','" + Longitute + "',dbo.GetContactName('" + UserId + "'), GETDATE())";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.CommandType = CommandType.Text;
               int  nDrRows = m_CommandODBC.ExecuteNonQuery();
                if (nDrRows < 0)
                {
                    nResultCode = -1;
                    strResult = "unable to insert driver location";
                }
                else
                {
                    nResultCode = 1;
                    strResult = "insert driver location successfully.";
                }
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
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GetDriverCurrentLocation = rows, error = strResult }));
        }
    }
    #endregion

    #region Get Invoice Data
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetInvoiceData(string szAPIKey, string szDeviceType, int strUserId, string TicketId, string TicketNo)
    {
        string temp = "";
        OdbcDataAdapter m_DataAdapter;
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("GetInvoiceData");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        ArrayList closedrows = new ArrayList();
        try
        {
            m_Connection.OpenDB("Galaxy");
            temp = "select a.id as TicketId,A.transaction_number AS TicketNo,a.case_caller_name as CustomarName," +
                    "a.cust_site_name as SiteName,a.cust_vehicleno as VehicleNo,a.cust_carmodel as CarModel,b.driver_name as DriverName,"+
                   "dbo.GetContactAddress(case_caller_id) as CustAddress,dbo.GetCustPhone(case_caller_id) as case_caller_phone "+
                    ",dbo.GetCustEmail(case_caller_id) as case_caller_email,a.case_status_name as TicketStatus,A.case_cust_Sig_Path AS CustSignature," +
                    "a.cust_location as CustLocation,a.cust_fule_type as FuelType, b.vehicle_no as VehicleNo,b.gensate_model_name as GensetModel,b.gensate_capecity as GensetCapecity," +
                    "a.cust_fule_qty as fuelQty, b.FuelfilledQty as FuelfilledQty,a.Fuel_rate as FuelRate ,a.Fuel_cost as FuleCost,a.final_Fuel_cost as TotalFuelCost,a.fuel_tax as FuelTax,a.Fuel_tax_amount as TaxAmount," +
                    "b.SiteExpensesValue1 as ConvenyanceExpense,b.SiteExpensesValue2 as LabourExpense,b.SiteExpensesValue3 as OthersExpense,b.TotalSiteExpenseAmt AS TotalSiteExpenseAmt from crm_cases A inner   " +
                    "join crm_documents_checklist B ON A.id = B.related_to_id and A.case_status_id=2 and (a.id= "+ TicketId + " or a.transaction_number = '"+ TicketNo + "') and assign_to_id=" + strUserId + " ";

            m_CommandODBC = new OdbcCommand(temp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
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
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GetInvoiceData = rows, error = strResult }));
        }
    }
    #endregion

    #region Genrate Invoice
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GenrateInvoice(string szAPIKey, string szDeviceType, int strUserId, string TicketId, string TicketNo)
    {
        string temp = "";
        OdbcDataAdapter m_DataAdapter;
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("GenrateInvoice");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        ArrayList closedrows = new ArrayList();
        try
        {
            m_Connection.OpenDB("Galaxy");
            temp = "select a.id as TicketId,A.transaction_number AS TicketNo,a.case_caller_name as CustomarName," +
                    "a.cust_site_name as SiteName,a.cust_vehicleno as VehicleNo,a.cust_carmodel as CarModel,b.driver_name as DriverName," +
                 "  dbo.GetContactAddress(case_caller_id) as CustAddress,dbo.GetCustPhone(case_caller_id) as case_caller_phone " +
                    ",dbo.GetCustEmail(case_caller_id) as case_caller_email,a.case_status_name as TicketStatus,A.case_cust_Sig_Path AS CustSignature," +
                    "a.cust_location as CustLocation,a.cust_fule_type as FuelType, b.vehicle_no as VehicleNo,b.gensate_model_name as GensetModel,b.gensate_capecity as GensetCapecity," +
                    "a.cust_fule_qty as fuelQty, b.FuelfilledQty as FuelfilledQty,a.Fuel_rate as FuelRate ,a.Fuel_cost as FuleCost,a.fuel_tax as FuelTax,a.Fuel_tax_amount as TaxAmount,B.TotalSiteExpenseAmt AS TotalSiteExpenseAmt from crm_cases A inner   " +
                    "join crm_documents_checklist B ON A.id = B.related_to_id and A.case_status_id=2 and (a.id= " + TicketId + " or a.transaction_number = '" + TicketNo + "') and assign_to_id=" + strUserId + " ";

            m_CommandODBC = new OdbcCommand(temp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
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
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GenrateInvoice = rows, error = strResult }));
        }
    }
    #endregion

    // Dashboard

    #region Dash Month Wise Ticket Status
    [WebMethod]
    public void DashMonthyWiseTicketStatus(string szAPIKey, string szDeviceType, int UserId) //, string szDateTime
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("cases");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        serializer.MaxJsonLength = Int32.MaxValue;
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType, UserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            OdbcDataAdapter m_DataAdapter;

            string strTemp = "select SUM(TCKOPEN)'OPEN' ,SUM(TCKCLOSE)'CLOSE' ,SUM(TCKDISCARD)'DISCARD'from ( "+
                            "select assign_to_id,case when case_status_name = 'OPEN' then 1 else 0 end as TCKOPEN "+
	                        ",case when case_status_name = 'CLOSE' then 1 else 0 end as TCKCLOSE "+
	                        ",case when case_status_name = 'DISCARD' then 1 else 0 end as TCKDISCARD "+
                            " from(SELECT  assign_to_id, case_status_name FROM crm_cases where assign_to_id = "+ UserId +" AND created_date between convert(datetime, convert(varchar, getdate() - 30, 1) + ' 0:0:0')"+
                            " and convert(datetime, convert(varchar, getdate(), 1) + ' 23:59:00')) as T) as S group by assign_to_id  ";
            
            //string temp = "select case_status_name as item_name,count(*) as item_value from crm_cases " +
            //                    " where useable = 'Y'  and assign_to_id=" + UserId + " " +
            //                    "and created_date between convert(datetime, convert(varchar, getdate()-30, 1) + ' 0:0:0')  " +
            //                    "and convert(datetime, convert(varchar, getdate(), 1) + ' 23:59:00') group by case_status_name";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
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
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { DashTicketStatus = rows, error = strResult }));
        }
    }
    #endregion

    #region Dash Days Wise Ticket Status
    [WebMethod]
    public void DashDayWiseTicketStatus(string szAPIKey, string szDeviceType, int UserId) //, string szDateTime
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("cases");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        serializer.MaxJsonLength = Int32.MaxValue;
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType, UserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        ArrayList closedrows = new ArrayList();
        try
        {
            m_Connection.OpenDB("Galaxy");
            OdbcDataAdapter m_DataAdapter;

            strTemp = "SELECT  convert(date,created_date,106) as TicketDate, isnull([OPEN],0)[OPEN],isnull([CLOSE],0)[CLOSE],isnull([DISCARD],0)[DISCARD]" +
                       "FROM(select created_date, case_status_name statusname, COUNT(*) TicketCount from crm_cases "+
                       "where Useable = 'Y' and assign_to_id="+ UserId + " "+
                        "and created_date between convert(datetime, convert(varchar, getdate()-30, 1) + ' 0:0:0') and convert(datetime, convert(varchar, getdate(), 1) + ' 23:59:00')  " +
                        "AND case_status_id > 0 group by created_date, case_status_name) AS aa " +
                       "PIVOT(sum(TicketCount) FOR statusname IN([OPEN], [CLOSE],[DISCARD])) AS PivotTable; ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
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
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { DashDayWiseTicketStatus = rows, error = strResult }));
        }
    }
    #endregion

    #region Get CheckList Site Accessbility
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetSiteAccessbility(string szAPIKey, string szDeviceType, int strUserId)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("SiteAccessbility");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;

        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT id, name from crm_general where type = 'SITEACCESSBILITY' ";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataAdapter da = new OdbcDataAdapter(m_CommandODBC);
            da.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { SiteAccessbility = rows, error = strResult }));
        }
    }
    #endregion

    #region Get CheckList GetSiteExpenses
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetSiteExpenses(string szAPIKey, string szDeviceType, int strUserId)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("SiteExpenses");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;

        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT id, name from crm_general where type = 'SITEEXPENSE' ";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataAdapter da = new OdbcDataAdapter(m_CommandODBC);
            da.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { SiteExpenses = rows, error = strResult }));
        }
    }
    #endregion

    #region Upload CheckList ConTent
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void UploadCheckListConTents(string szAPIKey, string szDeviceType, int strUserId, string TicketId, string TicketNo,
                                        string CustLocation, string CustLatLong,string TicketCreationDate,string SiteAccessbiity,
                                        string VehicleNo, string GensetModel, string GensetCapicity, string FuelTankCapicity,
                                        string FuelRate, string CommercialTax,string SiteExpensesValue1,
                                        string SiteExpensesValue2, string SiteExpensesValue3,string ReadingFuelFillingBefore, string ReadingFuelFillingAfter, 
                                        string ReadingGensetHMR,string ReadingFuelTank,string ReadingCMAfterFuelTank,string FuelLevelReadingFlowMetorBefore, string FuelLevelReadingFlowMetorAfter,
                                        string FuelfilledQty) 
    {
        int nResultCode = -1;
        string strResult = "Fail";
        string strQuery = "";

        double nFuelRate = 0;
        int nFuelQty = 0;
        double nFuelComerTax = 0;
        int nSiteExpensesValue1 = 0;
        int nSiteExpensesValue2 = 0;
        int nSiteExpensesValue3 = 0;

        DataSet m_DataSet = new DataSet();
        OdbcDataAdapter m_DataAdapter1;
        DataTable m_DataTable = new DataTable("Srf");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            
            m_Connection.OpenDB("Galaxy");

            strQuery = "select count(*) as Count from crm_documents_checklist where related_to_id ='" + TicketId + "' OR related_to_name = '" + TicketNo + "'";  //  or + "TicketId
            m_CommandODBC = new OdbcCommand(strQuery, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter1 = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter1.Fill(m_DataTable);
            int nRowDupCount = Convert.ToInt32(m_DataTable.Rows[0]["Count"]);
            
            if(nRowDupCount > 0)
            {
                nResultCode = 1;
                strResult = "Alreday Update Checklist Data";
                return;
            }
            else
            {
                if(FuelRate.ToString()!="0" || FuelRate.ToString() != "")
                {
                    nFuelRate = Convert.ToDouble(FuelRate);
                }
                if (FuelfilledQty.ToString() != "0" || FuelfilledQty.ToString() != "")
                {
                    nFuelQty = Convert.ToInt32(FuelfilledQty);
                }
                if (CommercialTax.ToString() != "0" || CommercialTax.ToString() != "")
                {
                    nFuelComerTax = Convert.ToDouble(CommercialTax);
                }
                double nFuelAmount = nFuelRate * nFuelQty;
                double nTaxValue = (nFuelAmount * nFuelComerTax) / 100;
                if(SiteExpensesValue1!="")
                {
                    nSiteExpensesValue1 = Convert.ToInt32(SiteExpensesValue1);
                }
                if (SiteExpensesValue2 != "")
                {
                    nSiteExpensesValue2 = Convert.ToInt32(SiteExpensesValue2);
                }
                if (SiteExpensesValue3 != "")
                {
                    nSiteExpensesValue3 = Convert.ToInt32(SiteExpensesValue3);
                }
                int nSiteExpenseAmt = (nSiteExpensesValue1+ nSiteExpensesValue2 + nSiteExpensesValue3);
                double szToFuelCost = Convert.ToDouble(nFuelAmount + nTaxValue + nSiteExpenseAmt);

                strQuery = "INSERT INTO [dbo].[crm_documents_checklist] " +
                   "([related_to_name],[related_to_id],[created_by]" +
                   ",[created_date],[Useable],[filling_date],[site_location],[cust_latLong],[allocation_date],[site_accessbility] " +
                   ",[vehicle_no],[driver_name],[gensate_model_name],[gensate_capecity],[fueltank_capecity],[Fuel_rate],[Fuel_cost],[final_Fuel_cost],[fuel_tax],[fuel_tax_amount]," +
                    "SiteExpensesValue1,SiteExpensesValue2,SiteExpensesValue3,TotalSiteExpenseAmt,ReadingFuelFillingBefore,ReadingFuelFillingAfter,ReadingGensetHMR,ReadingFuelTank,FuelfilledQty,ReadingCMAfterFuelTank,FuelLevelReadingFlowMetorBefore,FuelLevelReadingFlowMetorAfter,checklist_date) " +

                   "values('" + TicketNo + "','" + TicketId + "','" + strUserId + "',GETDATE(),'Y',GETDATE(),'" + CustLocation + "','" + CustLatLong + "','" + TicketCreationDate + "','" + SiteAccessbiity + "'," +
                            "'" + VehicleNo + "',dbo.GetContactName('" + strUserId + "'),'" + GensetModel + "','" + GensetCapicity + "','" + FuelTankCapicity + "','" + FuelRate + "','" + nFuelAmount + "','" + szToFuelCost + "'," +
                            "'" + CommercialTax + "','" + nTaxValue + "','" + SiteExpensesValue1 + "','" + SiteExpensesValue2 + "','" + SiteExpensesValue3 + "','" + nSiteExpenseAmt + "','" + ReadingFuelFillingBefore + "','" + ReadingFuelFillingAfter + "','" + ReadingGensetHMR + "','" + ReadingFuelTank + "','" + FuelfilledQty + "','" + ReadingCMAfterFuelTank + "','" + FuelLevelReadingFlowMetorBefore + "','" + FuelLevelReadingFlowMetorAfter + "',GETDATE())";
                
                m_CommandODBC = new OdbcCommand(strQuery, m_Connection.oCon);
                int nRowCount = m_CommandODBC.ExecuteNonQuery();
                if (nRowCount < 0)
                {
                    nResultCode = -1;
                    strResult = "unable to insert checklist data";
                }
                else
                {
                    string temp = "update crm_cases set Fuel_rate = '" + FuelRate + "', FuelfilledQty ='" + FuelfilledQty + "',Fuel_cost='" + nFuelAmount + "',final_Fuel_cost='" + szToFuelCost + "',TotalSiteExpenseAmt='" + nSiteExpenseAmt + "',Fuel_tax_amount='" + nTaxValue + "' where id='" + TicketId + "' ";
                    string res = ExecutenonQuery(temp);
                    if (res == "1")
                    {
                        nResultCode = 1;
                        strResult = "Insert checklist data successfully.";
                    }
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
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { UploadCheckListConTent = rows, error = strResult }));
        }
    }
    #endregion

    #region Get Document Bytes
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void UploadTicketDocument(string szAPIKey, string szDeviceType, int strUserId, string strbyte, string FileExtension, 
                                string DocumentName, string DocumentFileName, string TicketId, string TicketNo, string DocNumber)
    {

        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Srf");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            FileExtension = "." + FileExtension;
            string strFilePath = System.Configuration.ConfigurationManager.AppSettings["Filepath"];
            string FilePath = strFilePath + DocumentFileName;
            byte[] fileBytes = Convert.FromBase64String(strbyte);
            m_Connection.OpenDB("Galaxy");
            OdbcDataAdapter m_DataAdapter1;

            strTemp = "SELECT count(*) as FileCount FROM crm_documents where related_to='CAS' AND document_file_name='" + DocumentFileName + "' and (related_to_name='" + TicketNo + "' or related_to_id = " + TicketId + ")";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter1 = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter1.Fill(m_DataTable);
            int DocCount = Convert.ToInt32(m_DataTable.Rows[0]["FileCount"]);

            if (DocCount >= 1)
            {
                nResultCode = -1;
                strResult = "Docuement Alreday upload";
                this.Context.Response.Write(serializer.Serialize(new { GetDocumentBytes = rows, error = strResult }));
                return;
            }
            string query = "insert into crm_documents(document_extension,related_to,related_to_name,related_to_id,document_name,document_file_name,document_path,document_size,owner_id,owner_name,created_by,created_date,modified_by,modified_date,Useable,doc_number)" +
                "values('" + FileExtension + "','CAS','" + TicketNo + "','" + TicketId + "','" + DocumentName + "','" + DocumentFileName + "','" + FilePath + "','" + fileBytes.Length + "','" + strUserId + "',dbo.GetContactName('" + strUserId + "'),'" + strUserId + "',GetUTCDATE(),'" + strUserId + "',GetUTCDATE(),'Y','" + DocNumber + "')";
            
            m_CommandODBC = new OdbcCommand(query, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();
            if (nRowCount > 0)
            {
                string query2 = "UPDATE crm_cases set doc_upload_status= 'Y'" +
                                    ", doc_upload_date= GetUTCDATE() WHERE id = " + TicketId;
                m_CommandODBC = new OdbcCommand(query2, m_Connection.oCon);
                int nRow2Count = m_CommandODBC.ExecuteNonQuery();
            }
            BinaryWriter Writer = null;
            string Name = @"C:\temp\yourfile.png";
            Writer = new BinaryWriter(File.OpenWrite(FilePath));
            Writer.Write(fileBytes);
            Writer.Flush();
            Writer.Close();
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GetDocumentBytes = rows, error = strResult }));
        }
    }
    #endregion

    #region Changed Password
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void ChangedPassword(string strUserId,string strOldPwd, string strNewPwd, string strConfrimNewPwd,string szDeviceType)
    {
        Writelog.WriteError("ChangedPassword");
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterOdbc = null;
        DataTable dtTable = new DataTable("ChangedPassword");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        if (szDeviceType == "")
        {
            nResultCode = -1;
            strResult = "Invalid Device Type";
            this.Context.Response.Write(serializer.Serialize(new { ChangedPassword = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            string strQuery = "SELECT #GetUserPasswordMobile('" + strUserId.ToString() + "')";
            string strUserPassword = ExecuteQuery(strQuery);

          if (strUserPassword.Trim().ToString() != strOldPwd.Trim().ToString())
            {
                nResultCode = -1;
                strResult = "Please enter correct old Password";
                this.Context.Response.Write(serializer.Serialize(new { ChangedPassword = strResult }));
                return;
            }
            if (strNewPwd.Trim().ToString() != strConfrimNewPwd.Trim().ToString())
            {
                nResultCode = -1;
                strResult = "Invalid confirm password";
                this.Context.Response.Write(serializer.Serialize(new { ChangedPassword = strResult }));
                return;
            }
            strTemp = "UPDATE CRM_contacts SET contact_password='" + strNewPwd + "' " +
                       "WHERE (id='" + strUserId + "' or contact_phone ='" + strUserId + "' or contact_emailid ='" + strUserId + "') ";
                
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            int nRows = m_CommandODBC.ExecuteNonQuery();
            if (nRows < 0)
            {
                nResultCode = -1;
                strResult = "unable update  user password";
                this.Context.Response.Write(serializer.Serialize(new { ChangedPassword = strResult }));
                return;
            }
            else
            {

              nResultCode = 1;
              strResult = "update user password successfully.";
             }
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { ChangedPassword = "", error = strResult }));
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
            if (m_DataAdapterOdbc != null)
                m_DataAdapterOdbc.Dispose();
            dtTable.Dispose();
            m_Connection.CloseDB();
        }
    }
    #endregion

    #region Fuel Inward Entry
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void FuelInwardEntry(string szAPIKey, string szDeviceType, int strUserId, string PetrolpumpName, string PetrolpumpLocation,
                                string FuelPrices,string FuelRefillQty,string FuelReqNumber, string InvoiceFuelAmount, string FuelRewardPoint)
                                      
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        OdbcDataAdapter m_DataAdapter1;
        DataTable m_DataTable = new DataTable("Srf");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");

            string strQuery = "select count(*) as Count from crm_fuel_inward_entry where inward_fuelreuest_number ='" + FuelReqNumber + "'";
            m_CommandODBC = new OdbcCommand(strQuery, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter1 = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter1.Fill(m_DataTable);
            int nRowDupCount = Convert.ToInt32(m_DataTable.Rows[0]["Count"]);

            if (nRowDupCount > 0)
            {
                nResultCode = 1;
                strResult = "Alreday available fuel inward entry of Fuel RequestNo :-" + FuelReqNumber + "";
                return;
            }
            
            strQuery = "INSERT INTO [dbo].[crm_fuel_inward_entry] " +
                           "([inward_driver_id],[inward_petropump_name]" +
                           ",[inward_petropump_location],inward_fuel_prices,[inward_fuel_rifillqty],[inward_fuel_entrydate],inward_fuelreuest_number,inward_fuel_amount,inward_reward_point) " +

                           "values('" + strUserId + "','" + PetrolpumpName + "','" + PetrolpumpLocation + "','" + FuelPrices + "','" + FuelRefillQty + "',GETDATE(),'" + FuelReqNumber + "','" + InvoiceFuelAmount + "','" + FuelRewardPoint + "')";

            m_CommandODBC = new OdbcCommand(strQuery, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();
            if (nRowCount < 0)
            {
                nResultCode = -1;
                strResult = "unable to Fuel Inward Entry";
            }
            else
            {
                nResultCode = 1;
                strResult = "insert Fuel Inward Entry successfully.";
                string query2 = "UPDATE crm_requisitions set doc_upload_status= 'Y' , req_status ='CLOSE',req_close_date= GetUTCDATE() " +
                                     ", doc_upload_date= GetUTCDATE(),Invoice_amount='"+ InvoiceFuelAmount + "' WHERE transaction_number = '" + FuelReqNumber + "'";
                m_CommandODBC = new OdbcCommand(query2, m_Connection.oCon);
                int nRow2Count = m_CommandODBC.ExecuteNonQuery();
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { FuelInwardEntry = rows, error = strResult }));
        }
    }
    #endregion

    #region Ticket Discard
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void TicketDiscard(string szAPIKey, string szDeviceType, int UserId, string TicketId, string Remarks)
    {
        string temp = "";
        OdbcDataAdapter da;
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Tasks");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType, UserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            temp = "update crm_cases set case_status_id=4,case_status_name='DISCARD',end_remarks='" + Remarks + "',end_time=Getdate() where id='" + TicketId + "' ";
            string res = ExecutenonQuery(temp);
            m_Connection.CloseDB();
            if (res == "1")
            {
                strResult = "Pass";
                Message = "Ticket Discard Successfully";
                this.Context.Response.ContentType = "application/json; charset=utf-8";
                this.Context.Response.Write(serializer.Serialize(new { TicketDiscard = Message, Result = strResult }));
            }
            else
            {
                Message = "Ticket Discard Failed";
                this.Context.Response.ContentType = "application/json; charset=utf-8";
                this.Context.Response.Write(serializer.Serialize(new { TicketDiscard = Message, Result = strResult }));
            }

        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {

        }
    }
    #endregion

    #region Get Fuel Petropump Lists 
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetFuelPetropumpLists(string szAPIKey, string szDeviceType, int strUserId)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("FuelPetropumpLists");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;

        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT id, station_name as name from crm_fillingstation_master order by station_name";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataAdapter da = new OdbcDataAdapter(m_CommandODBC);
            da.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { FuelPetropumpLists = rows, error = strResult }));
        }
    }
    #endregion

    #region Get Fuel Petropump Data 
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetFuelPetropumpListsData(string szAPIKey, string szDeviceType, int strUserId,string strPetrolpumpId, string strPetrolpumpName)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("FuelPetropumpListsData");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;

        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
             strTemp = "SELECT id, station_name,address,Region,latitute,longitute from crm_fillingstation_master where id= " + strPetrolpumpId + "order by station_name";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataAdapter da = new OdbcDataAdapter(m_CommandODBC);
            da.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GetFuelPetropumpListsData = rows, error = strResult }));
        }
    }
    #endregion


    #region Get Fuel Entry Lists 
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetFuelInwardLists(string szAPIKey, string szDeviceType, int strUserId)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("FuelInwardLists");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;

        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT id, inward_petropump_name,inward_petropump_location,inward_fuel_rifillqty,convert(date,inward_fuel_entrydate,103)inward_fuel_entrydate " +
                " from crm_fuel_inward_entry where inward_driver_id = " + strUserId + " order by inward_fuel_entrydate desc ";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataAdapter da = new OdbcDataAdapter(m_CommandODBC);
            da.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GetFuelInwardLists = rows, error = strResult }));
        }
    }
    #endregion

    // PHASE 2 DEVELEOPEMENT 

    #region Fuel Request Entry
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void FuelRequestEntry(string szAPIKey, string szDeviceType, int strUserId, string PetrolpumpName, string PetrolpumpLocation,
                                string FuelReqQty, string FuelStatus, string QueryType, string FuelRequestNo)

    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Srf");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        string query = "";
        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC InsertUpdate_FuelRequestAPI ?,?,?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@nUserId", OdbcType.Int).Value = strUserId;
            m_CommandODBC.Parameters.Add("@PetrolpumpName", OdbcType.VarChar).Value = PetrolpumpName;
            m_CommandODBC.Parameters.Add("@PetrolpumpLocation", OdbcType.VarChar).Value = PetrolpumpLocation;
            m_CommandODBC.Parameters.Add("@FuelReqQty", OdbcType.VarChar).Value = FuelReqQty;
            m_CommandODBC.Parameters.Add("@FuelStatus", OdbcType.VarChar).Value = FuelStatus;
            m_CommandODBC.Parameters.Add("@FuelRequestNo", OdbcType.VarChar).Value = FuelRequestNo;
            
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";
            string strFuelRequestNo = m_DataSet.Tables[0].Rows[0]["TicketNumber"].ToString();

            nResultCode = 1;
            strResult = "Fuel request successfully -" + strFuelRequestNo;

        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { FuelRequestEntry = rows, error = strResult }));
        }
    }
    #endregion

    #region Get Fuel Request Status
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetFuelRequestStatus(string szAPIKey, string szDeviceType, int strUserId)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("FuelRequestStatus");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;

        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT id, name from crm_general where type = 'FUELSTATUS' ";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataAdapter da = new OdbcDataAdapter(m_CommandODBC);
            da.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GetFuelRequestStatus = rows, error = strResult }));
        }
    }
    #endregion

    #region Get Fuel Request Pending Data 
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetFuelReqPendingList(string szAPIKey, string szDeviceType, int strUserId)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("GetFuelRequestData");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;

        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = " SELECT [id],[transaction_number],convert(varchar(25),dbo.getLocalZoneTime(req_approval_date,330),113) as req_approval_date,[req_status],convert(varchar(25),dbo.getLocalZoneTime(created_date,330),113) as created_date,[inward_driver_id],[inward_petropump_name] " +
                        ",[inward_petropump_location],[inward_fuelQty],req_ApprovalByName ,convert(varchar(25),dbo.getLocalZoneTime(req_close_date,330),113) as FuelReqCloseDate,convert(varchar(25),dbo.getLocalZoneTime(req_reject_date,330),113) as FuelRejectDate,req_reject_remarks as RejectRemarks,Invoice_amount  FROM [dbo].[crm_requisitions] where req_status IN ('PENDING') AND inward_driver_id=" + strUserId + " order by id desc ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataAdapter da = new OdbcDataAdapter(m_CommandODBC);
            da.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GetFuelReqPendingList = rows, error = strResult }));
        }
    }
    #endregion

    #region Get Fuel Request Approved Data 
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetFuelReqApprovedList(string szAPIKey, string szDeviceType, int strUserId)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("GetFuelReqApprovedList");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;

        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = " SELECT [id],[transaction_number],convert(varchar(25),dbo.getLocalZoneTime(req_approval_date,330),113) as req_approval_date,[req_status],convert(varchar(25),dbo.getLocalZoneTime(created_date,330),113) as created_date,[inward_driver_id],[inward_petropump_name] " +
                        ",[inward_petropump_location],[inward_fuelQty],req_ApprovalByName,req_notification_status as SMSStatus ,convert(varchar(25),dbo.getLocalZoneTime(req_close_date,330),113) as FuelReqCloseDate,convert(varchar(25),dbo.getLocalZoneTime(req_reject_date,330),113) as FuelRejectDate,req_reject_remarks as RejectRemarks,Invoice_amount  FROM [dbo].[crm_requisitions] where inward_driver_id=" + strUserId + " and req_status='APPROVED' order by id desc ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataAdapter da = new OdbcDataAdapter(m_CommandODBC);
            da.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GetFuelReqApprovedList = rows, error = strResult }));
        }
    }
    #endregion

    #region Get Fuel Request Reject Data 
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetFuelReqRejectList(string szAPIKey, string szDeviceType, int strUserId)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("GetFuelReqRejectList");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;

        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = " SELECT [id],[transaction_number],convert(varchar(25),dbo.getLocalZoneTime(req_approval_date,330),113) as req_approval_date,[req_status],convert(varchar(25),dbo.getLocalZoneTime(created_date,330),113) as created_date,[inward_driver_id],[inward_petropump_name] " +
                        ",[inward_petropump_location],[inward_fuelQty],req_ApprovalByName,req_notification_status as SMSStatus,convert(varchar(25),dbo.getLocalZoneTime(req_close_date,330),113) as FuelReqCloseDate,convert(varchar(25),dbo.getLocalZoneTime(req_reject_date,330),113) as FuelRejectDate,req_reject_remarks as RejectRemarks,Invoice_amount FROM [dbo].[crm_requisitions] where inward_driver_id=" + strUserId + " and req_status='REJECT' order by id desc ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataAdapter da = new OdbcDataAdapter(m_CommandODBC);
            da.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]); 
                }
                rows.Add(row);
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GetFuelReqRejectList = rows, error = strResult }));
        }
    }
    #endregion

    #region Get Fuel Request Close Data 
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetFuelReqCloseList(string szAPIKey, string szDeviceType, int strUserId)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("GetFuelReqCloseList");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;

        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = " SELECT [id],[transaction_number],convert(varchar(25),dbo.getLocalZoneTime(req_approval_date,330),113) as req_approval_date,[req_status],convert(varchar(25),dbo.getLocalZoneTime(created_date,330),113) as created_date,[inward_driver_id],[inward_petropump_name] " +
                        ",[inward_petropump_location],[inward_fuelQty],req_ApprovalByName,req_notification_status as SMSStatus, convert(varchar(25),dbo.getLocalZoneTime(req_close_date,330),113) as FuelReqCloseDate,convert(varchar(25),dbo.getLocalZoneTime(req_reject_date,330),113) as FuelRejectDate,req_reject_remarks as RejectRemarks,Invoice_amount FROM [dbo].[crm_requisitions] where inward_driver_id=" + strUserId + " and req_status='CLOSE' order by id desc ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataAdapter da = new OdbcDataAdapter(m_CommandODBC);
            da.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GetFuelReqCloseList = rows, error = strResult }));
        }
    }
    #endregion

    #region Upload Fuel Request Documents
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void UploadFuelRequestDoc(string szAPIKey, string szDeviceType, int strUserId, string strbyte, string FileExtension,
                                string DocumentName, string DocumentFileName, string FuelReqId, string FuelReqNumber)
    {

        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Srf");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            FileExtension = "." + FileExtension;
            string strFilePath = System.Configuration.ConfigurationManager.AppSettings["Filepath"];
            string FilePath = strFilePath + DocumentFileName;
            byte[] fileBytes = Convert.FromBase64String(strbyte);
            m_Connection.OpenDB("Galaxy");
            OdbcDataAdapter m_DataAdapter1;
            
            strTemp = "SELECT count(*) as FileCount FROM crm_documents where related_to='RQF' AND document_file_name='" + DocumentFileName + "' and (related_to_name='" + FuelReqNumber + "' or related_to_id = " + FuelReqId + ")";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter1 = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter1.Fill(m_DataTable);
            int DocCount = Convert.ToInt32(m_DataTable.Rows[0]["FileCount"]);

            if (DocCount >= 1)
            {
                nResultCode = -1;
                strResult = "Fuel Invoice Docuement Alreday upload";
                this.Context.Response.Write(serializer.Serialize(new { GetDocumentBytes = rows, error = strResult }));
                return;
            }
            string query = "insert into crm_documents(document_extension,related_to,related_to_name,related_to_id,document_name,document_file_name,document_path,document_size,owner_id,owner_name,created_by,created_date,modified_by,modified_date,Useable)" +
                "values('" + FileExtension + "','RQF','" + FuelReqNumber + "','" + FuelReqId + "','" + DocumentName + "','" + DocumentFileName + "','" + FilePath + "','" + fileBytes.Length + "','" + strUserId + "',dbo.GetContactName('" + strUserId + "'),'" + strUserId + "',GetUTCDATE(),'" + strUserId + "',GetUTCDATE(),'Y')";

            m_CommandODBC = new OdbcCommand(query, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            BinaryWriter Writer = null;
            string Name = @"C:\temp\yourfile.png";
            Writer = new BinaryWriter(File.OpenWrite(FilePath));
            Writer.Write(fileBytes);
            Writer.Flush();
            Writer.Close();
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        catch (Exception ex)
        {
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { UploadFuelRequestDoc = rows, error = strResult }));
        }
    }
    #endregion

    #region Days Wise Fuel Request Status
    [WebMethod]
    public void FuelStatusCount(string szAPIKey, string szDeviceType, int UserId)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("cases");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        serializer.MaxJsonLength = Int32.MaxValue;
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;
        strResult = CheckValidation(szAPIKey, szDeviceType, UserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        ArrayList closedrows = new ArrayList();
        try
        {
            m_Connection.OpenDB("Galaxy");
            OdbcDataAdapter m_DataAdapter;

            string strTemp = "select SUM(PENDING)'PENDING' ,SUM(APPROVED)'APPROVED' ,SUM(REJECT)'REJECT',SUM(CLOSED)'CLOSE' from ( " +
                      "select inward_driver_id,case when req_status = 'PENDING' then 1 else 0 end as PENDING " +
                      ",case when req_status = 'APPROVED' then 1 else 0 end as APPROVED " +
                      ",case when req_status = 'REJECT' then 1 else 0 end as REJECT " +
                       ",case when req_status = 'CLOSE' then 1 else 0 end as CLOSED " +
                      " from(SELECT  inward_driver_id, req_status FROM crm_requisitions where inward_driver_id = " + UserId + " AND created_date between convert(datetime, convert(varchar, getdate() - 30, 1) + ' 0:0:0')" +
                      " and convert(datetime, convert(varchar, getdate(), 1) + ' 23:59:00')) as T) as S group by inward_driver_id  ";

            //strTemp = "SELECT  convert(date,created_date,106) as FuelReqDate, isnull([PENDING],0)[PENDING],isnull([APPROVED],0)[APPROVED],isnull([REJECT],0)[REJECT]" +
            //           "FROM(select created_date, req_status as FuelStatus, COUNT(*) FuelReqCount from crm_requisitions " +
            //           "where Useable = 'Y' and inward_driver_id=" + UserId + " " +
            //            "and created_date between convert(datetime, convert(varchar, getdate()-30, 1) + ' 0:0:0') and convert(datetime, convert(varchar, getdate(), 1) + ' 23:59:00')  " +
            //            "AND req_status is not null group by created_date, req_status) AS aa " +
            //           "PIVOT(sum(FuelReqCount) FOR FuelStatus IN([PENDING],[APPROVED],[REJECT])) AS PivotTable; ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
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
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { FuelStatusCount = rows, error = strResult }));
        }
    }
    #endregion

    #region Get Fuel Reward Point History 
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void GetRewardPointHistory(string szAPIKey, string szDeviceType, int strUserId)
    {
        int nResultCode = -1;
        string strResult = "Fail";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("RewardPointHistory");
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        Dictionary<string, object> row = null;

        strResult = CheckValidation(szAPIKey, szDeviceType, strUserId);
        if (strResult != "PASS")
        {
            this.Context.Response.Write(serializer.Serialize(new { error = strResult }));
            return;
        }
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT id, inward_petropump_name,inward_petropump_location,inward_fuel_rifillqty,inward_fuel_prices as FuelRate,convert(date,inward_fuel_entrydate,103)inward_fuel_entrydate,inward_fuelreuest_number as FuelReqNo,inward_reward_point as Point " +
                " from crm_fuel_inward_entry where inward_fuelreuest_number IS NOT NULL AND inward_driver_id = " + strUserId + " order by inward_fuel_entrydate desc ";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataAdapter da = new OdbcDataAdapter(m_CommandODBC);
            da.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);
            foreach (DataRow dr in m_DataTable.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in m_DataTable.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
            this.Context.Response.ContentType = "application/json; charset=utf-8";
            this.Context.Response.Write(serializer.Serialize(new { GetRewardPointHistory = rows, error = strResult }));
        }
    }
    #endregion


}





