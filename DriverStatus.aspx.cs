using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using System.Xml;
using Newtonsoft.Json.Linq;

public partial class VehicleLocation : ThemeBase
{
    int strTicketId = 0;
    string TicketNo = "";
    string CustLocation = "";
    string CustLatitute = "";
    string CustLongtitute = "";
    string DvrLatitute = "";
    string DvrLongitute = "";
    string VehicleNo = "";
    string SourceID = "";
    string Source = "";

    GlobalWS objGlobalWS = new GlobalWS();

    protected void Page_Load(object sender, EventArgs e)
    {
   //     m_Connection.OpenDB("Galaxy");

        if (Request.QueryString["CaseID"] != null)
            strTicketId = Convert.ToInt32(Request.QueryString["CaseID"]);

        if (Request.QueryString["SourceID"] != null)
            SourceID = Convert.ToString(Request.QueryString["SourceID"]);

        if (Request.QueryString["Source"] != null)
            Source = Convert.ToString(Request.QueryString["Source"]);

        if (!this.IsPostBack)
        {
            string strQuery = "SELECT #GetCustLatLong('" + SourceID + "')";
            string strLatLong = objGlobalWS.ExecuteQuery(strQuery);
            if(strLatLong!="")
            {
                string[] strLatiLongTute = strLatLong.ToString().Split(',');
                CustLatitute = strLatiLongTute[0].ToString();
                CustLongtitute = strLatiLongTute[1].ToString();
                txtDestination.Value = CustLatitute + "," + CustLongtitute;
            }
            else
            {
                strQuery = "SELECT #GetCustLatLongByTicketId('" + strTicketId + "')";
                strLatLong = objGlobalWS.ExecuteQuery(strQuery);
                if (strLatLong != "")
                {
                    string[] strLatiLongTute = strLatLong.ToString().Split(',');
                    CustLatitute = strLatiLongTute[0].ToString();
                    CustLongtitute = strLatiLongTute[1].ToString();
                    txtDestination.Value = CustLatitute + "," + CustLongtitute;
                }

            }
            DataTable dt = this.GetData("select B.driver_name as DriverName, B.vehicle_no as VehicleNo,B.latitute as Latitute, B.longititute as Longitute  " +
                                        " from crm_driverlocation B where updatedate is not null AND CONVERT(DATE,updatedate) = CONVERT(DATE,GETDATE()) ORDER BY updatedate DESC");
            
            if(dt.Rows.Count > 0)
            {

                string Distance = "";
                string Duration = "";

                    DataTable dtNew = new DataTable();
                    dtNew.Columns.AddRange(new DataColumn[3] {
                    new DataColumn("Driver", typeof(string)),
                    new DataColumn("Distance", typeof(string)),
                    new DataColumn("Duration",typeof(string)) });

                //16.8277764,96.154744   ///////
                foreach (DataRow dr in dt.Rows)
                {
                    string Lat = dr["Latitute"].ToString();
                    string Long = dr["Longitute"].ToString();
                    string DriverName = dr["DriverName"].ToString();
                    string LatLong = Lat + "," + Long;

                    string strResponse = getDistanceTest(txtDestination.Value, LatLong, DriverName); //16.7773549,96.15851889999999
                    string[] szstrResponse = strResponse.ToString().Split('|');
                    Distance = szstrResponse[0].ToString();
                    Duration = szstrResponse[1].ToString();

                    DataRow row = dtNew.NewRow();
                    row["Driver"] = DriverName;
                    row["Distance"] = Distance;
                    row["Duration"] = Duration;
                    dtNew.Rows.Add(row);

                }

                GridView1.DataSource = dtNew;
                GridView1.DataBind();

                rptMarkers.DataSource = dt;
                    rptMarkers.DataBind();
            }
        }
    }

    public string getDistanceTest(string source, string destination,string DriverName)
    {

        string distance = "";
        string duration = "";
        string nDistance = "";

        string content = GetJsonData("https://maps.googleapis.com/maps/api/directions/json?key=AIzaSyCs-N8XxSQWIfllyifIcpNLKANPTWnd2jk&origin=" + source + "&destination=" + destination + "&sensor=false");

        //string content = GetJsonData("https://maps.googleapis.com/maps/api/directions/json?key=AIzaSyAmSZwEGMWpCdpj0_3I-IzAkyF2NOF0AD8&origin=" + source + "&destination=" + destination + "&sensor=false");
        JObject obj = JObject.Parse(content);
        try
        {
            distance = (string)obj.SelectToken("routes[0].legs[0].distance.text");
            duration = (string)obj.SelectToken("routes[0].legs[0].duration.text");
            nDistance = distance;

            //lblDistance.Text = DriverName + " - " + nDistance.ToString();
            //lblDuration.Text = duration;
            // dt.Rows.Add(1, DriverName, nDistance, duration);
         }
        catch (Exception ex)
        {
            lblMessage.Text = ex.Message;
        }
        return Convert.ToString(distance + "|" + duration);
    }

    protected string GetJsonData(string url)
    {
        string sContents = string.Empty;
        string me = string.Empty;
        try
        {
            if (url.ToLower().IndexOf("https:") > -1)
            {
                System.Net.WebClient client = new System.Net.WebClient();
                byte[] response = client.DownloadData(url);
                sContents = System.Text.Encoding.ASCII.GetString(response);
            }
            else
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(url);
                sContents = sr.ReadToEnd();
                sr.Close();
            }
        }
        catch
        {
            sContents = "unable to connect to server ";
        }
        return sContents;
    }

    private DataTable GetData(string query)
    {
        string conString = ConfigurationManager.ConnectionStrings["GalaxyReports"].ConnectionString;
        SqlCommand cmd = new SqlCommand(query);
        using (SqlConnection con = new SqlConnection(conString))
        {
            using (SqlDataAdapter sda = new SqlDataAdapter())
            {
                cmd.Connection = con;

                sda.SelectCommand = cmd;
                using (DataTable dt = new DataTable())
                {
                    sda.Fill(dt);
                    return dt;
                }
            }
        }
    }
}



