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
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            DataTable dt = this.GetData("select customer_pinlocation as Name,latitute as Latitude ,longititute as Longitude,discription as Description " +
                " from crm_driverlocation where CONVERT(DATE,updatedate) = CONVERT(DATE,GETDATE()) ORDER BY updatedate DESC");
            rptMarkers.DataSource = dt;
            rptMarkers.DataBind();
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
