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

/// <summary>
/// Summary description for bo_CTIHistory
/// </summary>
public class bo_History
{
   
    public string CallMasterTableName { get; set; }
    public string CustID { get; set; }
    public string Type { get; set; }    
    public int ResultCode { get; set; }
    public string ResultString { get; set; }
    public string ErrorSource { get; set; }
    public string ErrorStackTrace { get; set; }
    public int nID { get; set; }


}
