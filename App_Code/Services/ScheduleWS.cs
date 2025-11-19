using System;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Collections;
using System.Web.UI;
using System.Collections.Generic;

/// <summary>
/// Summary description for ScheduleWS
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class ScheduleWS : System.Web.Services.WebService {

    DataBase m_Connection;
    OdbcDataAdapter m_DataAdapterOdbc;
    OdbcCommand m_CommandODBC;

    string strTemp = "";
    string strParameters = "";
    string strIPAddress = HttpContext.Current.Request.UserHostAddress;


    public ScheduleWS () {

        m_Connection = new DataBase();

    }

    #region Get GetConsultant
    [WebMethod]
    public DataSet GetConsultant(string Consultant,string ContactType, int AccountID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();        

        try
        {
            m_Connection.OpenDB("Galaxy");
            if (ContactType.Length > 1)
                ContactType = ContactType.Insert(1, "','");
            strTemp = "select id, contact_full_name from crm_contacts where related_to = 'CST'" +
                " AND contact_type_id IN('" + ContactType + "')";
            if (AccountID != 0)
                strTemp += " AND related_to_id = " + AccountID;
            if (Consultant != "")
                strTemp += " AND contact_full_name LIKE '%" + Consultant + "%'";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Consultant";

            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetConsultant", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetConsultant", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region Get Schedules
    public DataSet GetSchedules(int ConsultantID, DateTime ConsultationDate, DataTable dt, int TB_nTimeZoneTimeSpan)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();                

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp += "SELECT *,DATEPART(HH,dbo.getLocalZoneTime(udf_schd_start_time,#TIMESPAN#)) AS SCHD_TIME FROM crm_tasks WHERE " +
                        "convert(varchar,dbo.getLocalZoneTime(udf_schd_start_time,#TIMESPAN#),106) = '" + ConsultationDate.ToString("dd MMM yyyy") + "' " +
                        "AND udf_schd_contact_id='" + ConsultantID + "'" +
                        ";SELECT *,DATEPART(HH,dbo.getLocalZoneTime(udf_schd_start_time,#TIMESPAN#)) AS SCHD_TIME FROM crm_tasks WHERE " +
                        "convert(varchar,dbo.getLocalZoneTime(udf_schd_start_time,#TIMESPAN#),106) = '" + ConsultationDate.AddDays(1).ToString("dd MMM yyyy") + "' " +
                        "AND udf_schd_contact_id='" + ConsultantID + "'" +
                        ";SELECT *,DATEPART(HH,dbo.getLocalZoneTime(udf_schd_start_time,#TIMESPAN#)) AS SCHD_TIME FROM crm_tasks WHERE " +
                        "convert(varchar,dbo.getLocalZoneTime(udf_schd_start_time,#TIMESPAN#),106) = '" + ConsultationDate.AddDays(2).ToString("dd MMM yyyy") + "' " +
                        "AND udf_schd_contact_id='" + ConsultantID + "'" +
                        ";SELECT *,DATEPART(HH,dbo.getLocalZoneTime(udf_schd_start_time,#TIMESPAN#)) AS SCHD_TIME FROM crm_tasks WHERE " +
                        "convert(varchar,dbo.getLocalZoneTime(udf_schd_start_time,#TIMESPAN#),106) = '" + ConsultationDate.AddDays(3).ToString("dd MMM yyyy") + "' " +
                        "AND udf_schd_contact_id='" + ConsultantID + "'" +
                        ";SELECT *,DATEPART(HH,dbo.getLocalZoneTime(udf_schd_start_time,#TIMESPAN#)) AS SCHD_TIME FROM crm_tasks WHERE " +
                        "convert(varchar,dbo.getLocalZoneTime(udf_schd_start_time,#TIMESPAN#),106) = '" + ConsultationDate.AddDays(4).ToString("dd MMM yyyy") + "' " +
                        "AND udf_schd_contact_id='" + ConsultantID + "'";

            strTemp = strTemp.Replace("#TIMESPAN#", TB_nTimeZoneTimeSpan.ToString());
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            
            
            string strId = "";
            for (int tblCount = 0; tblCount < m_DataSet.Tables.Count; tblCount++)
            {
                #region Next four days

                foreach (DataRow row in dt.Rows)
                {
                    string timeRows = row[0].ToString();

                    if (tblCount == 1)
                        dt.Rows.Find(timeRows)[6] = "0";
                    else if (tblCount == 2)
                        dt.Rows.Find(timeRows)[7] = "0";
                    else if (tblCount == 3)
                        dt.Rows.Find(timeRows)[8] = "0";
                    else if (tblCount == 4)
                        dt.Rows.Find(timeRows)[9] = "0";
                }
                #endregion

                #region fill schedule for phlebo
                for (int count = 0; count < m_DataSet.Tables[tblCount].Rows.Count; count++)
                {
                    //string mm = (Convert.ToInt32(m_DataSet.Tables[tblCount].Rows[count]["schd_time"]) - 1).ToString("00") + "-" + (Convert.ToInt32(m_DataSet.Tables[tblCount].Rows[count]["schd_time"])).ToString("00");
                    string mm = (Convert.ToInt32(m_DataSet.Tables[tblCount].Rows[count]["schd_time"])).ToString("00") + "-" + (Convert.ToInt32(m_DataSet.Tables[tblCount].Rows[count]["schd_time"]) + 1).ToString("00");
                    int minutes = (Convert.ToDateTime(m_DataSet.Tables[tblCount].Rows[count]["udf_schd_start_time"])).AddMinutes(TB_nTimeZoneTimeSpan).Minute;
                    if (tblCount == 0)
                    {
                        string szDescription = m_DataSet.Tables[tblCount].Rows[count]["udf_schd_description"].ToString();
                        if (minutes == 0)
                        {
                            dt.Rows.Find(mm)[1] = szDescription;
                            
                        }
                        else if (minutes == 15)
                        {
                            dt.Rows.Find(mm)[2] = szDescription;
                            
                        }
                        else if (minutes == 30)
                            dt.Rows.Find(mm)[3] = szDescription;
                        else if (minutes == 45)
                            dt.Rows.Find(mm)[4] = szDescription;

                        strId = dt.Rows.Find(mm)[5] + "," + m_DataSet.Tables[tblCount].Rows[count]["id"].ToString();
                        dt.Rows.Find(mm)[5] = strId;
                    }
                    else if (tblCount == 1)
                    {
                        if (minutes == 0 || minutes == 15 || minutes == 30 || minutes == 45)
                        {
                            if (dt.Rows.Find(mm)[6].ToString().Length > 0)
                                dt.Rows.Find(mm)[6] = Convert.ToInt32(dt.Rows.Find(mm)[6]) + 1;
                            else
                                dt.Rows.Find(mm)[6] = "1";
                        }


                    }
                    else if (tblCount == 2)
                    {
                        if (minutes == 0 || minutes == 15 || minutes == 30 || minutes == 45)
                        {
                            if (dt.Rows.Find(mm)[7].ToString().Length > 0)
                                dt.Rows.Find(mm)[7] = Convert.ToInt32(dt.Rows.Find(mm)[7]) + 1;
                            else
                                dt.Rows.Find(mm)[7] = "1";
                        }

                    }
                    else if (tblCount == 3)
                    {
                        if (minutes == 0 || minutes == 15 || minutes == 30 || minutes == 45)
                        {
                            if (dt.Rows.Find(mm)[8].ToString().Length > 0)
                                dt.Rows.Find(mm)[8] = Convert.ToInt32(dt.Rows.Find(mm)[8]) + 1;
                            else
                                dt.Rows.Find(mm)[8] = "1";
                        }
                    }
                    else if (tblCount == 4)
                    {
                        if (minutes == 0 || minutes == 15 || minutes == 30 || minutes == 45)
                        {
                            if (dt.Rows.Find(mm)[9].ToString().Length > 0)
                                dt.Rows.Find(mm)[9] = Convert.ToInt32(dt.Rows.Find(mm)[9]) + 1;
                            else
                                dt.Rows.Find(mm)[9] = "1";
                        }
                    }


                }
                #endregion

            }
           
            m_DataSet = new DataSet();
            m_DataSet.Tables.Add(dt);
            m_DataSet.Tables[0].TableName = "Schedule";

            nResultCode = 0;
            strResult = "Pass";

        }
        
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetSchedules", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetSchedules", strParameters);
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
    #region Get Schedules
    public DataSet GetSchedulesBranchWise(int BranchID, DateTime ConsultationDate, int TB_nTimeZoneTimeSpan)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable dtConsultant = new DataTable();
        int ConsultantCount = 0;
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "select id, contact_full_name FROM crm_contacts WHERE related_to = 'CST' AND contact_type_id = 'T' " +
                        "AND related_to_id=" + BranchID;

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(dtConsultant);
            ConsultantCount = dtConsultant.Rows.Count;
            string strTable = "<table style='width:100%;'><tr style='background-color:Black; color:White;'><th>Consultant Name</th><th>Status</th></tr>";
            for (int i = 0; i < dtConsultant.Rows.Count; i++)
            {
                strTable += "<tr style='background-color:Green;'><td>" + dtConsultant.Rows[i]["contact_full_name"].ToString() + "</td><td>Free</td></tr>";
            
            }
            strTable += "</table>";

            strTemp = "SELECT *,DATEPART(HH,dbo.getLocalZoneTime(udf_schd_start_time,#TIMESPAN#)) AS SCHD_TIME FROM crm_tasks WHERE " +
                        "convert(varchar,dbo.getLocalZoneTime(udf_schd_start_time,#TIMESPAN#),106) = '" + ConsultationDate.ToString("dd MMM yyyy") + "' " +
                        "AND udf_schd_account_id=" + BranchID;
            strTemp = strTemp.Replace("#TIMESPAN#", TB_nTimeZoneTimeSpan.ToString());
            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            int nCount = 0;
            int pCount = 0;
            int mCount = 0;
            int rCount = 0;
            string strText0 = "";
            string strText15 = "";
            string strText30 = "";
            string strText45 = "";

            Dictionary<string, Array> d = new Dictionary<string, Array>();
            string strKey = "";
            string strKey1 = "";
            for (int nStartHour = 0; nStartHour < 24; nStartHour++)
            {
                strKey = nStartHour.ToString("00") + (nStartHour + 1).ToString("00");
                for (int j = 0; j < 4; j++)
                {
                    if (j == 0)
                    {
                        strKey1 = "0015";
                    }
                    else if (j == 1)
                    {
                        strKey1 = "1630";
                    }
                    else if (j == 2)
                    {
                        strKey1 = "3145";
                    }
                    else if (j == 3)
                    {
                        strKey1 = "4660";
                    }

                    Object[] objs = new Object[2];
                    objs[0] = "0/" + ConsultantCount.ToString();
                    objs[1] = strTable;
                    d.Add(strKey + strKey1, objs);
                    strKey1 = "";
                }
            }


            for (int k = 0; k < ConsultantCount; k++)
            {
                DataRow[] dr = m_DataSet.Tables[0].Select("udf_schd_contact_id = " + dtConsultant.Rows[k]["id"].ToString());
                foreach (DataRow drData in dr)
                {
                    string mm = (Convert.ToInt32(drData["schd_time"])).ToString("00") + "-" + (Convert.ToInt32(drData["schd_time"]) + 1).ToString("00");
                    int minutes = (Convert.ToDateTime(drData["udf_schd_start_time"])).AddMinutes(TB_nTimeZoneTimeSpan).Minute;

                    mm = mm.Replace("-", "");

                   // string szDescription = drData["udf_schd_description"].ToString();
                    if (minutes > 45)
                        mm += "4660"; 
                    else if (minutes > 30)
                        mm += "3145"; 
                    else if (minutes > 15)
                        mm += "1630"; 
                    else if (minutes >= 0)
                        mm += "0015";

                    Object[] objs = (Object[])d[mm];
                    string[] strCount = Convert.ToString(objs[0]).Split('/');

                    objs[0] = (Convert.ToInt32(strCount[0]) + 1).ToString() + "/" + strCount[1].ToString();
                    objs[1] = Convert.ToString(objs[1]).Replace("<tr style='background-color:Green;'><td>" + dtConsultant.Rows[k]["contact_full_name"].ToString() + "</td><td>Free</td></tr>", "<tr style='background-color:Red;'><td>" + dtConsultant.Rows[k]["contact_full_name"].ToString() + "</td><td>Fixed</td></tr>");
                    d[mm] = objs;
                }
            }

            DataTable dtFinalData = CreateTableStructure();


            DataRow drValue = dtFinalData.NewRow();
            int nIndex = 1;
            foreach (KeyValuePair<string, Array> pair in d)
            {
                Object[] objs = (Object[])pair.Value;
                string KeyValue = pair.Key;
                drValue["schd_hour"] = KeyValue.Substring(0, 4).Insert(2, "-");
                switch (nIndex)
                {
                    case 1:
                        drValue["schd_status1"] = Convert.ToString(objs[0]);
                        drValue["schd_status_tooltip1"] = Convert.ToString(objs[1]);
                        break;
                    case 2:
                        drValue["schd_status2"] = Convert.ToString(objs[0]);
                        drValue["schd_status_tooltip2"] = Convert.ToString(objs[1]);
                        break;
                    case 3:
                        drValue["schd_status3"] = Convert.ToString(objs[0]);
                        drValue["schd_status_tooltip3"] = Convert.ToString(objs[1]);
                        break;
                    case 4:
                        drValue["schd_status4"] = Convert.ToString(objs[0]);
                        drValue["schd_status_tooltip4"] = Convert.ToString(objs[1]);
                        break;
                }
                nIndex++;

                if (nIndex > 4)
                {
                    dtFinalData.Rows.Add(drValue);
                    nIndex = 1;
                    drValue = dtFinalData.NewRow();
                }
              
            }

            m_DataSet = new DataSet();
            m_DataSet.Tables.Add(dtFinalData);
           
            nResultCode = 0;
            strResult = "Pass";

        }

        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetSchedules", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetSchedules", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion
    #region Private Create Table
    private DataTable CreateTableStructure()
    {
        DataTable dt = new DataTable("FinalData");  
        try
        {
                      
            dt.Columns.Add("schd_hour", Type.GetType("System.String"));
            dt.Columns.Add("schd_status1", Type.GetType("System.String"));
            dt.Columns.Add("schd_status_tooltip1", Type.GetType("System.String"));
            dt.Columns.Add("schd_status2", Type.GetType("System.String"));
            dt.Columns.Add("schd_status_tooltip2", Type.GetType("System.String"));
            dt.Columns.Add("schd_status3", Type.GetType("System.String"));
            dt.Columns.Add("schd_status_tooltip3", Type.GetType("System.String"));
            dt.Columns.Add("schd_status4", Type.GetType("System.String"));
            dt.Columns.Add("schd_status_tooltip4", Type.GetType("System.String"));

           

        }
        catch (Exception ex)
        {
           
        }
        return dt;
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
}

