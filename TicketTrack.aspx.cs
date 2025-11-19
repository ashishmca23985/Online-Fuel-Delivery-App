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

    GlobalWS objGlobalWS = new GlobalWS();

    protected void Page_Load(object sender, EventArgs e)
    {
   //     m_Connection.OpenDB("Galaxy");

        if (Request.QueryString["CaseID"] != null)
            strTicketId = Convert.ToInt32(Request.QueryString["CaseID"]);

        if (!this.IsPostBack)
        {

            string strQuery = "SELECT #GetTicketName('" + strTicketId+ "')";
            string TicketNo = objGlobalWS.ExecuteQuery(strQuery);
            
            DataTable dt = this.GetData("select TOP 1 A.id AS ID, A.transaction_number AS TicketNo, a.cust_location as CustLocation, A.cust_latitute as CustLatitute, A.cust_longitute as CustLongtitute,"+
                                        "B.latitute as DvrLatitute, B.longititute as DvrLongitute, B.driver_name as DriverName, B.vehicle_no as VehicleNo " +
                                        " from crm_cases A inner join crm_driverlocation B on a.transaction_number = b.TicketId  where B.TicketId = '" + TicketNo + "' ORDER BY updatedate DESC" );
            
            if(dt.Rows.Count > 0)
            {
                CustLocation = dt.Rows[0]["CustLocation"].ToString();
                CustLatitute = dt.Rows[0]["CustLatitute"].ToString();
                CustLongtitute = dt.Rows[0]["CustLongtitute"].ToString();
                DvrLatitute = dt.Rows[0]["DvrLatitute"].ToString();
                DvrLongitute = dt.Rows[0]["DvrLongitute"].ToString();
                VehicleNo = dt.Rows[0]["VehicleNo"].ToString();

                txtSource.Value = DvrLatitute + "," + DvrLongitute;
                txtDestination.Value = CustLatitute + "," + CustLongtitute;
            }
           
        }

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
