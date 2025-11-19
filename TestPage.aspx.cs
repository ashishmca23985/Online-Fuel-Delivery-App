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


public partial class VehicleLocation : System.Web.UI.Page
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

    
    protected void Page_Load(object sender, EventArgs e)
    {
 
        //if (Request.QueryString["CaseID"] != null)
        //    strTicketId = Convert.ToInt32(Request.QueryString["CaseID"]);

        //if (Request.QueryString["SourceID"] != null)
        //    SourceID = Convert.ToString(Request.QueryString["SourceID"]);

        //if (Request.QueryString["Source"] != null)
        //    Source = Convert.ToString(Request.QueryString["Source"]);

        //if (!this.IsPostBack)
        //{

        //    string strQuery = "SELECT #GetCustLatLong('" + SourceID + "')";
        //    string strLatLong = objGlobalWS.ExecuteQuery(strQuery);
        //    if(strLatLong!="")
        //    {
        //        string[] strLatiLongTute = strLatLong.ToString().Split(',');
        //        CustLatitute = strLatiLongTute[0].ToString();
        //        CustLongtitute = strLatiLongTute[1].ToString();
        //    }

        //    DataTable dt = this.GetData("select B.latitute as DlrLatitute, B.longititute as DlrLongitute, B.driver_name as DriverName, B.vehicle_no as VehicleNo " +
        //                                " from crm_driverlocation B where updatedate is not null ORDER BY updatedate DESC");
            
        //    if(dt.Rows.Count > 0)
        //    {

        //        GridView1.DataSource = dt;
        //        GridView1.DataBind();

        //      //  JavaScriptSerializer serializer = new JavaScriptSerializer();
        //        //var output = serializer.Serialize(places);
        //        //ClientScript.RegisterClientScriptBlock(GetType(), "points", "var points = " + output + ";var currentLoc = { 'Latitude' : " + CustLatitute + ", 'Longitude':" + CustLongtitute + " }", true);


        //        // CustLocation = dt.Rows[0]["CustLocation"].ToString();
        //        // CustLatitute = dt.Rows[0]["CustLatitute"].ToString();
        //        //CustLongtitute = dt.Rows[0]["CustLongtitute"].ToString();
        //        DvrLatitute = dt.Rows[0]["DlrLatitute"].ToString();
        //        DvrLongitute = dt.Rows[0]["DlrLongitute"].ToString();
        //        VehicleNo = dt.Rows[0]["VehicleNo"].ToString();

        //        txtSource.Value = DvrLatitute + "," + DvrLongitute;
        //        txtDestination.Value = CustLatitute + "," + CustLongtitute;
        //    }
           
        //}

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
