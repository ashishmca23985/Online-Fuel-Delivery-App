using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Data.Odbc;

public partial class GalaxyConfig : ThemeBase
{
    string strTemp = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        DataBase m_Connection = null;

        try
        {
            //lblLogFilePath.Text = Convert.ToString(ConfigurationManager.AppSettings["LogPath"]);
            //lblVersion.Text = Convert.ToString(Application["GalaxyVersion"]);

            //--get ViaSMS/Email Status
            m_Connection = new DataBase();
            m_Connection.OpenDB("Galaxy");

            if (m_Connection.nDatabaseType == 2) //-- MySql
            {
                strTemp = "SELECT Case when param_status_time < DATE_ADD(now(),INTERVAL -1 HOUR) then 'Stopped' Else param_status_desc End as param_status_desc " +
                                 "FROM viasms_parameters LIMIT 1";
            }
            else
            {
                strTemp = "SELECT module_name,(CASE WHEN DATEDIFF(second,module_status_updated_time,GETUTCDATE())>60 then 'Stopped' else 'Running' end )as status  FROM interdialog_modules  ";
            }
            DataSet ds = new DataSet();
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            OdbcDataAdapter odbc_adapter = new OdbcDataAdapter(m_CommandODBC);
            odbc_adapter.Fill(ds);
            m_CommandODBC.Dispose();
            strTemp = "<table class='aspxMainTable' align=center  ><tr><th class='tdMainHeader1'><img alt='GalaxyLogo' src='images/favicon.ico' />About - Galaxy CRM [Configurations]</th></tr><table>";
            strTemp += "<table class='pages-grids' style='width:400px;margin-top:30px;'>";
            strTemp += "<tr><td><strong>Module</strong></td><td><strong>Status</strong></td></tr>";
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                strTemp += "<tr><td>";
                strTemp += row[0].ToString();
                strTemp += "</td><td ";
                strTemp += (row[1].ToString() == "Stopped" ? "style='background-color:red;'" : "style='background-color:green;'");
                strTemp += ">";
                strTemp += row[1].ToString();
                strTemp += "</td></tr>";
            }
            strTemp += "</table>";
            dv.InnerHtml = strTemp;
        }
        catch (Exception)
        {
           // LogMessage(ex.Message, 1);
        }
        finally
        {
            m_Connection.CloseDB();            
        }
    }

    //#region Log Message
    //void LogMessage(string Message, Int32 param)
    //{
    //    lblMessage.Text = Message;
    //    if (param == 1)
    //        lblMessage.CssClass = "error";
    //    else
    //        lblMessage.CssClass = "success";
    //}
    //#endregion
    protected void tmrTimer_Tick(object sender, EventArgs e)
    {
        
    }
}
