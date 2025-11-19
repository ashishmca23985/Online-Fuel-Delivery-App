using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Newtonsoft.Json;
using SharedVO;

/// <summary>
/// Summary description for ReportWS
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
 [System.Web.Script.Services.ScriptService]
public class ReportWS : System.Web.Services.WebService {
    DataBase m_Connection = new DataBase();
    OdbcCommand m_CommandODBC;
    OdbcDataAdapter m_DataAdapterODBC;
    string strTemp = "";
    string strIPAddress = HttpContext.Current.Request.UserHostAddress;
    string strParameters = "";
    public ReportWS () {
    }

    [WebMethod(EnableSession=true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<LeadStatus> DashBoard(string deslet)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        m_Connection.OpenDB("Galaxy");
        DataSet m_DataSet = new DataSet();
        List<LeadStatus> ob = new List<LeadStatus>();

        string szBrowsedIP = Convert.ToString(HttpContext.Current.Request.Url).Replace("http://", "");
        szBrowsedIP = Convert.ToString(szBrowsedIP).Substring(0, szBrowsedIP.IndexOf("/"));

        string userid = Convert.ToString(HttpContext.Current.Session["contact_id"]);
        string timespan = Convert.ToString(HttpContext.Current.Session["TimeZoneTimeSpan"]);
        try
        {
        strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " dashlet_type,dashlet_SP_name FROM crm_dashlet_master WHERE dashlet_id=" + deslet + " " + m_Connection.DB_TOP_MYSQL;

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
                strTemp = strTemp.Replace("#USERID#", userid);
                strTemp = strTemp.Replace("#TIMESPAN#", timespan);
                strTemp = strTemp.Replace("#IP#", szBrowsedIP);
                m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                m_DataAdapterOdbc.Fill(m_DataSet);
                m_DataSet.Tables[1].TableName = "Data";


                foreach (DataRow row in m_DataSet.Tables[1].Rows)
                {

                    ob.Add(new LeadStatus()
                    {
                        Name = row["item_name"].ToString(),
                        count = Convert.ToInt32(row["item_value"]),
                    });
                }
            }
            nResultCode = 0;
            strResult = "Pass";
        }



      
            //m_Connection.OpenDB("Galaxy");
            //m_CommandODBC = new OdbcCommand("EXEC DB_GraphsourceSummary ?,?", m_Connection.oCon);
            //m_CommandODBC.CommandType = CommandType.StoredProcedure;
            //m_CommandODBC.Parameters.Add("@UserID", OdbcType.Int).Value = userid;
            //m_CommandODBC.Parameters.Add("@TimeSpan", OdbcType.Int).Value = timespan;
            //OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            //m_DataAdapter.Fill(m_DataSet);
            //m_DataSet.Tables[0].TableName = "Data";
            //nResultCode = 0;
            //strResult = "Pass";

            //foreach (DataRow row in m_DataSet.Tables[0].Rows)
            //{

            //    ob.Add(new LeadStatus()
            //    {
            //        Name = row["item_name"].ToString(),
            //        count = Convert.ToInt32(row["item_value"]),
            //    });
            //}

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
        return ob;
    }




    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<LeadStatus> Ticket30DayReport()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        List<LeadStatus> ob = new List<LeadStatus>();



        int userid = Convert.ToInt32(HttpContext.Current.Session["contact_id"]);
        int timespan = Convert.ToInt32(HttpContext.Current.Session["TimeZoneTimeSpan"]);

        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC DB_30DayCaseSummaryGraph ?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@UserID", OdbcType.Int).Value = userid;
            m_CommandODBC.Parameters.Add("@TimeSpan", OdbcType.Int).Value = timespan;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";
            nResultCode = 0;
            strResult = "Pass";

            foreach (DataRow row in m_DataSet.Tables[0].Rows)
            {

                ob.Add(new LeadStatus()
                {
                    Name = row["item_name"].ToString(),
                    count = Convert.ToInt32(row["item_value"]),
                });
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
        return ob;
    }



    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public object LeadOwnerWiseReport(DateTime dtfrom, DateTime dtto, int empId)
    {
        object status = new object();
        object reason = new object();
        object source = new object();
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        List<LeadStatus> ob = new List<LeadStatus>();
        List<LeadStatus> ob1 = new List<LeadStatus>();
        List<LeadStatus> ob2 = new List<LeadStatus>();
        List<object> data = new List<object>();
        
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC MIS_Owner_Lead ?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@dtFrom", OdbcType.DateTime).Value = dtfrom;
            m_CommandODBC.Parameters.Add("@dtTo", OdbcType.DateTime).Value = dtto;
            m_CommandODBC.Parameters.Add("@nId", OdbcType.Int).Value = empId;
            //DataSet ds = new DataSet();
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            nResultCode = 0;
            strResult = "Pass";

            foreach (DataRow row in m_DataSet.Tables[0].Rows)
            {
                ob.Add(new LeadStatus()
                {
                    Name = row["Name"].ToString(),
                    count = Convert.ToInt32(row["leadcount"]),
                });
            }
            foreach (DataRow row in m_DataSet.Tables[1].Rows)
            {
                ob1.Add(new LeadStatus()
                {
                    Name = row["Name"].ToString(),
                    count = Convert.ToInt32(row["leadcount"]),
                });
            }
            foreach (DataRow row in m_DataSet.Tables[2].Rows)
            {
                ob2.Add(new LeadStatus()
                {
                    Name = row["Name"].ToString(),
                    count = Convert.ToInt32(row["leadcount"]),
                });
            }

            status = ob.Select(t => new
            {
                 Name= t.Name,
                count = t.count,
             
            }).ToList();
            reason = ob1.Select(t => new
            {
                Name = t.Name,
                count = t.count,

            }).ToList();
            source = ob2.Select(t => new
            {
                Name = t.Name,
                count = t.count,

            }).ToList();
          
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
        var item = new { status = status,
                         reason = reason,
                         source = source
        };
        string tmp = JsonConvert.SerializeObject(data);
        return item;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public object LeadAssignWiseReport(DateTime dtfrom, DateTime dtto, int empId)
    {
        object status = new object();
        object reason = new object();
        object source = new object();
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        List<LeadStatus> ob = new List<LeadStatus>();
        List<LeadStatus> ob1 = new List<LeadStatus>();
        List<LeadStatus> ob2 = new List<LeadStatus>();
        List<object> data = new List<object>();

        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC MIS_Assign_Lead ?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@dtFrom", OdbcType.DateTime).Value = dtfrom;
            m_CommandODBC.Parameters.Add("@dtTo", OdbcType.DateTime).Value = dtto;
            m_CommandODBC.Parameters.Add("@nId", OdbcType.Int).Value = empId;
            //DataSet ds = new DataSet();
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            nResultCode = 0;
            strResult = "Pass";

            foreach (DataRow row in m_DataSet.Tables[0].Rows)
            {
                ob.Add(new LeadStatus()
                {
                    Name = row["Name"].ToString(),
                    count = Convert.ToInt32(row["leadcount"]),
                });
            }
            foreach (DataRow row in m_DataSet.Tables[1].Rows)
            {
                ob1.Add(new LeadStatus()
                {
                    Name = row["Name"].ToString(),
                    count = Convert.ToInt32(row["leadcount"]),
                });
            }
            foreach (DataRow row in m_DataSet.Tables[2].Rows)
            {
                ob2.Add(new LeadStatus()
                {
                    Name = row["Name"].ToString(),
                    count = Convert.ToInt32(row["leadcount"]),
                });
            }

            status = ob.Select(t => new
            {
                Name = t.Name,
                count = t.count,

            }).ToList();
            reason = ob1.Select(t => new
            {
                Name = t.Name,
                count = t.count,

            }).ToList();
            source = ob2.Select(t => new
            {
                Name = t.Name,
                count = t.count,

            }).ToList();

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
        var item = new
        {
            status = status,
            reason = reason,
            source = source
        };
        string tmp = JsonConvert.SerializeObject(data);
        return item;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<LeadStatusBar> LeadOwnerAssignWiseReport(DateTime dtfrom, DateTime dtto, int empId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        List<LeadStatusBar> ob = new List<LeadStatusBar>();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC MIS_Owner_Assign_Lead ?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@dtFrom", OdbcType.DateTime).Value = dtfrom;
            m_CommandODBC.Parameters.Add("@dtTo", OdbcType.DateTime).Value = dtto;
            m_CommandODBC.Parameters.Add("@nId", OdbcType.Int).Value = empId;
            //DataSet ds = new DataSet();
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";
            nResultCode = 0;
            strResult = "Pass";

            foreach (DataRow row in m_DataSet.Tables[0].Rows)
            {

                ob.Add(new LeadStatusBar()
                {
                    Name = row["Name"].ToString(),
                    open = Convert.ToInt32(row["OPEN"]),
                    working = Convert.ToInt32(row["WORKING"]),
                    deferred = Convert.ToInt32(row["DEFERRED"]),
                    close = Convert.ToInt32(row["CLOSE"]),
                });
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
        return ob;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<LeadStatus> LeadReasonWiseReport(DateTime dtfrom, DateTime dtto)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        List<LeadStatus> ob = new List<LeadStatus>();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC MIS_Lead_Reason ?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@dtFrom", OdbcType.DateTime).Value = dtfrom;
            m_CommandODBC.Parameters.Add("@dtTo", OdbcType.DateTime).Value = dtto;
            //DataSet ds = new DataSet();
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";
            nResultCode = 0;
            strResult = "Pass";

            foreach (DataRow row in m_DataSet.Tables[0].Rows)
            {

                ob.Add(new LeadStatus()
                {
                    Name = row["Name"].ToString(),
                    count = Convert.ToInt32(row["leadcount"]),
                });
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
        return ob;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<LeadStatus> LeadSourceWiseReport(DateTime dtfrom, DateTime dtto)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        List<LeadStatus> ob = new List<LeadStatus>();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC MIS_Lead_Source ?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@dtFrom", OdbcType.DateTime).Value = dtfrom;
            m_CommandODBC.Parameters.Add("@dtTo", OdbcType.DateTime).Value = dtto;
            //DataSet ds = new DataSet();
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";
            nResultCode = 0;
            strResult = "Pass";

            foreach (DataRow row in m_DataSet.Tables[0].Rows)
            {

                ob.Add(new LeadStatus()
                {
                    Name = row["Name"].ToString(),
                    count = Convert.ToInt32(row["leadcount"]),
                });
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
        return ob;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<LeadStatusBar> LeadStatusYearWiseReport(DateTime dtfrom, DateTime dtto)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        List<LeadStatusBar> ob = new List<LeadStatusBar>();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC MIS_Lead_status_year_Wise ?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@dtFrom", OdbcType.DateTime).Value = dtfrom;
            m_CommandODBC.Parameters.Add("@dtTo", OdbcType.DateTime).Value = dtto;
            //DataSet ds = new DataSet();
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";
            nResultCode = 0;
            strResult = "Pass";

            foreach (DataRow row in m_DataSet.Tables[0].Rows)
            {

                ob.Add(new LeadStatusBar()
                {
                    Name = row["LeadMonth"].ToString(),
                    open = Convert.ToInt32(row["OpenC"]),
                    working = Convert.ToInt32(row["WorkingC"]),
                    deferred = Convert.ToInt32(row["DeferredC"]),
                    close = Convert.ToInt32(row["CloseC"]),
                });
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
        return ob;
    }

    // Vehicle master
    // Broker Master
    #region Get Vehicile Master Details Data
    [WebMethod]
    public DataSet GetVehicileMasterDetails()
    {

        DataBase m_Connection = new DataBase();
        DataSet dsDataSet = new DataSet();
        int nResultCode = -1;
        string strResult = "-Fail";

        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("select id,vehicle_no,Region,Project,FuelFillerName,vehicle_Type,vehicle_DriverName,vehicle_DriverID,EFunnelDeviceId from crm_vehicle_master order by vehicle_DriverName ", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            DataSet ds = new DataSet();
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(dsDataSet);
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
        dsDataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return dsDataSet;
    }
    #endregion

    #region Delete Vehicile Master Details
    public DataSet DeleteVehicileMasterDetails(int Id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "delete from crm_vehicle_master where id = " + Id.ToString();
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
        //try
        //{
        //    m_Connection.OpenDB("Galaxy");

        //    m_CommandODBC = new OdbcCommand("delete from crm_vehicle_master where id = " + Id + "", m_Connection.oCon);
        //    int nRows = m_CommandODBC.ExecuteNonQuery();
        //}
        //catch (OdbcException ex)
        //{
        //    nResultCode = ex.ErrorCode;
        //    strResult = ex.Message;
        //}
        //catch (Exception ex)
        //{
        //    nResultCode = -1;
        //    strResult = ex.Message;
        //}
        //finally
        //{
        //    m_Connection.CloseDB();
        //}
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Insert Update Vehicile
    public DataSet InserUpdateVehicileMaster(int nID, string strVehicleNo, string strRegion, string strProject, string strFuelFillerName, 
                                                string strType, string strDriverName, string strDriverId, string strEFunnelDeviceId)
    {

        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            if (nID == 0 && nID == 0) // for inserting the record
            {
                strTemp = "INSERT into crm_vehicle_master (vehicle_no,Region,Project,FuelFillerName,vehicle_Type,vehicle_DriverName,vehicle_DriverID,created_date,EFunnelDeviceId)" +
                          " Values (?,?,?,?,?,?,?,?,?)";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@vehicle_no", OdbcType.VarChar).Value = strVehicleNo;
                m_CommandODBC.Parameters.Add("@Region", OdbcType.VarChar).Value = strRegion;
                m_CommandODBC.Parameters.Add("@Project", OdbcType.VarChar).Value = strProject;
                m_CommandODBC.Parameters.Add("@FuelFillerName", OdbcType.VarChar).Value = strFuelFillerName;
                m_CommandODBC.Parameters.Add("@vehicle_Type", OdbcType.VarChar).Value = strType;
                m_CommandODBC.Parameters.Add("@vehicle_DriverName", OdbcType.VarChar).Value = strDriverName;
                m_CommandODBC.Parameters.Add("@vehicle_DriverID", OdbcType.Int).Value = strDriverId;
                m_CommandODBC.Parameters.Add("@CreatedDate", OdbcType.VarChar).Value = DateTime.Now.ToString();
                m_CommandODBC.Parameters.Add("@EFunnelDeviceId", OdbcType.VarChar).Value = strEFunnelDeviceId.ToString();
                m_CommandODBC.ExecuteNonQuery();
            }
            else  // for updating the record
            {
                strTemp = "UPDATE crm_vehicle_master  " +
                          "SET  vehicle_no = ?,Region=? ,Project=? ,FuelFillerName=? ,vehicle_Type=? ,vehicle_DriverName=?,vehicle_DriverID=?,modified_date=?,EFunnelDeviceId=? " +
                          "Where id = " + nID + "";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@vehicle_no", OdbcType.VarChar).Value = strVehicleNo;
                m_CommandODBC.Parameters.Add("@Region", OdbcType.VarChar).Value = strRegion;
                m_CommandODBC.Parameters.Add("@Project", OdbcType.VarChar).Value = strProject;
                m_CommandODBC.Parameters.Add("@FuelFillerName", OdbcType.VarChar).Value = strFuelFillerName;
                m_CommandODBC.Parameters.Add("@vehicle_Type", OdbcType.VarChar).Value = strType;
                m_CommandODBC.Parameters.Add("@vehicle_DriverName", OdbcType.VarChar).Value = strDriverName;
                m_CommandODBC.Parameters.Add("@vehicle_DriverID", OdbcType.Int).Value = strDriverId;
                m_CommandODBC.Parameters.Add("@ModifyDate", OdbcType.VarChar).Value = DateTime.Now.ToString();
                m_CommandODBC.Parameters.Add("@EFunnelDeviceId", OdbcType.VarChar).Value = strEFunnelDeviceId.ToString();

                m_CommandODBC.ExecuteNonQuery();
            }
            if (nID == 0)
            {
                nResultCode = 0;
                strResult = "Vehicle Added successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            nResultCode = 0;
            strResult = "Vehicle Updated successfully!";
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
    #endregion


    #region Get filling station Master Details Data
    [WebMethod]
    public DataSet GetfillingstationDetails()
    {

        DataBase m_Connection = new DataBase();
        DataSet dsDataSet = new DataSet();
        int nResultCode = -1;
        string strResult = "-Fail";

        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("select id,station_name,address,Region,latitute,longitute,enabled,created_date,contact_person," +
                " contact_person_mobileno from crm_fillingstation_master order by station_name,region ", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            DataSet ds = new DataSet();
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(dsDataSet);
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
        dsDataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return dsDataSet;
    }
    #endregion

    #region Delete filling station Master Details
    public DataSet DeleteFillingStationDetails(int Id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "delete from crm_fillingstation_master where id = " + Id.ToString();
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

    #region Insert Update Fillling Station 
    public DataSet InserUpdateFilllingStation(int nID, string strStaionName, string strAddress, string strRegion, string strLatitute,
                                                string strLongitute,int nContactId, string strContact, string strContactPhone)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            if (nID == 0 && nID == 0) // for inserting the record
            {
                strTemp = "INSERT into crm_fillingstation_master (station_name,address,Region,latitute,longitute,created_by,created_date,contact_person,contact_person_mobileno)" +
                          " Values (?,?,?,?,?,?,?,?,?)";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@station_name", OdbcType.VarChar).Value = strStaionName;
                m_CommandODBC.Parameters.Add("@address", OdbcType.VarChar).Value = strAddress;
                m_CommandODBC.Parameters.Add("@Region", OdbcType.VarChar).Value = strRegion;
                m_CommandODBC.Parameters.Add("@latitute", OdbcType.VarChar).Value = strLatitute;
                m_CommandODBC.Parameters.Add("@longitute", OdbcType.VarChar).Value = strLongitute;
                m_CommandODBC.Parameters.Add("@CreatedBy", OdbcType.Int).Value = nContactId;
                m_CommandODBC.Parameters.Add("@CreatedDate", OdbcType.VarChar).Value = DateTime.Now.ToString();

                m_CommandODBC.Parameters.Add("@Contact", OdbcType.VarChar).Value = strContact;
                m_CommandODBC.Parameters.Add("@ContactPhone", OdbcType.VarChar).Value = strContactPhone;

                m_CommandODBC.ExecuteNonQuery();
            }
            else  // for updating the record
            {
                strTemp = "UPDATE crm_fillingstation_master  " +
                          "SET  station_name = ?,address=? ,Region=? ,latitute=? ,longitute=?,modified_by=?,modified_date=?,contact_person=?,contact_person_mobileno=? " +
                          "Where id = " + nID + "";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@station_name", OdbcType.VarChar).Value = strStaionName;
                m_CommandODBC.Parameters.Add("@address", OdbcType.VarChar).Value = strAddress;
                m_CommandODBC.Parameters.Add("@Region", OdbcType.VarChar).Value = strRegion;
                m_CommandODBC.Parameters.Add("@latitute", OdbcType.VarChar).Value = strLatitute;
                m_CommandODBC.Parameters.Add("@longitute", OdbcType.VarChar).Value = strLongitute;
                m_CommandODBC.Parameters.Add("@modified_by", OdbcType.Int).Value = nContactId;
                m_CommandODBC.Parameters.Add("@ModifyDate", OdbcType.VarChar).Value = DateTime.Now.ToString();

                m_CommandODBC.Parameters.Add("@Contact", OdbcType.VarChar).Value = strContact;
                m_CommandODBC.Parameters.Add("@ContactPhone", OdbcType.VarChar).Value = strContactPhone;

                m_CommandODBC.ExecuteNonQuery();
            }
            if (nID == 0)
            {
                nResultCode = 0;
                strResult = "Filling Station Added successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            nResultCode = 0;
            strResult = "Filling Station Updated successfully!";
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
    #endregion

    // GET TICKET DUMP

    #region Get Ticket Dump
    [WebMethod]
    public DataSet GetTicketDump(DateTime dtfrom, DateTime dtto, string reportname)
    {

        DataBase m_Connection = new DataBase();
        DataSet dsDataSet = new DataSet();
        int nResultCode = -1;
        string strResult = "-Fail";
        string temp = "";
        try
        {
            m_Connection.OpenDB("Galaxy");
            if(reportname=="1")
            {
                temp = " select A.id as Id, A.transaction_number AS TicketNo, convert(varchar(20),dbo.getLocalZoneTime(A.created_date,330),13)  as TicketCreateDate,convert(varchar(20),dbo.getLocalZoneTime(A.end_time,330),13) as TicketEndDate,a.end_remarks as CloseRemarks, " +
              " A.case_customer_name as AccountName,a.case_caller_name as CallerName,b.driver_name as DriverName,cust_site_name as SiteName, " +
              " dbo.GetContactAddress(case_caller_id) as CustAddress,dbo.GetCustPhone(case_caller_id) as CallerPhone, cust_vehicleno as VehicleNo, cust_carmodel as CarModel," +
              " dbo.GetCustEmail(case_caller_id) as CallerEmail,a.case_status_name as TicketStatus,A.case_severity_desc as TicketSeverity," +
              " a.cust_location as CustLocation,cust_latitute + ',' +cust_longitute as CustLatituteLongitute,A.cust_req_type as RequestType,A.cust_filling_type as FillingType," +
              " A.cust_filling_contactno as CustFillingPhone,A.cust_loc_permission as LocationPermission,A.cust_approchaccess as CustApproch, a.cust_fule_type as FuelType, " +
             "  b.gensate_model_name as GensetModel,b.gensate_capecity as GensetCapecity, a.cust_fule_qty as RequestFuelQty,b.FuelfilledQty as FuelfilledQty," +
             " A.EFunnel_FilledQty,a.Fuel_rate as FuelRate ,a.Fuel_cost as FuleCost,a.final_Fuel_cost as TotalFuelCost," +
             " a.fuel_tax as FuelTax, a.Fuel_tax_amount as TaxAmount," +

            "B.ReadingFuelFillingBefore, dbo.GetDocumentPath1(a.id) as Image1,B.ReadingFuelFillingAfter,dbo.GetDocumentPath2(a.id) as Image2," +
             "B.ReadingGensetHMR,dbo.GetDocumentPath3(a.id) as Image3,B.ReadingFuelTank,dbo.GetDocumentPath4(a.id) as Image4," +
            "B.ReadingCMAfterFuelTank,dbo.GetDocumentPath5(a.id) as Image5,B.FuelLevelReadingFlowMetorBefore,dbo.GetDocumentPath6(a.id) as Image6," +
            "B.FuelLevelReadingFlowMetorAfter,dbo.GetDocumentPath7(a.id) as Image7," +

             " B.gensate_capecity as GensetCapicity, B.fueltank_capecity as FuelTankCapecity,B.site_accessbility as SiteAccessbility,convert(varchar(20),dbo.getLocalZoneTime(B.filling_date,330),13) as FillingDate," +
             " b.SiteExpensesValue1 as ConvenyanceExpense,b.SiteExpensesValue2 as LabourExpense,b.SiteExpensesValue3 as OthersExpense," +
             " b.TotalSiteExpenseAmt AS TotalSiteExpenseAmt ,A.EFunnel_DeviceId,A.EFunnel_FillTime,A.EFunnel_gpsLatitude,A.EFunnel_gpsLongitude" +
             " from crm_cases A inner join crm_documents_checklist B ON A.id = B.related_to_id and A.case_status_id = 2 and convert(date,A.created_date) between '" + dtfrom + "' and '" + dtto + "'  order by a.created_date";

            }
            if (reportname == "2")
            {
              temp = " select A.id as Id, A.transaction_number AS FuelRequestNo, dbo.GetContactName(a.inward_driver_id) as 'FuelerName',dbo.GetDriverVehicleNo(a.inward_driver_id) as 'VehicleNo',req_status as Status,convert(varchar(20),dbo.getLocalZoneTime(A.created_date,330),13)  as CreateDate,req_ApprovalByName as ApprovedBy,convert(varchar(20),dbo.getLocalZoneTime(A.req_approval_date,330),13)  as ApprovalDate," +
              " A.inward_petropump_name as StationName,a.inward_petropump_location as Address, convert(varchar(20),dbo.getLocalZoneTime(A.req_close_date,330),13) as CloseDate," +
             " A.inward_fuelQty as ApprovedFuelQty,dbo.GetFuelRequestPrices(a.transaction_number) as 'Rate',a.Invoice_amount as InvoiceAmount," +

                "dbo.GetFuelReqDocPath(a.id) as InvoiceLink" +

             " from crm_requisitions A inner join crm_documents B ON A.transaction_number = B.related_to_name and A.req_status = 'CLOSE' and convert(date,A.created_date) between '" + dtfrom + "' and '" + dtto + "'  order by a.created_date desc";

            }
            m_CommandODBC = new OdbcCommand(temp, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            DataSet ds = new DataSet();
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(dsDataSet);
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
        dsDataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return dsDataSet;
    }
    #endregion

}
