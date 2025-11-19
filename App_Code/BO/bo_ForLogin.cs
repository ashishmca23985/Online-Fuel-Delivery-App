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
/// Summary description for bo_ForLogin
/// </summary>
public class bo_ForLogin
{

    public string Message { get; set; }
    public string SessionVal { get; set; }
    public string CustID { get; set; }

    public int AgentID { get; set; }
    public string ChangePagePassword { get; set; }
    public string RoleIDExist { get; set; }
    public string RoleID { get; set; }
    public string CtiPassword { get; set; }
    public string CtiLoginName { get; set; }
    public string Output { get; set; }
    public string Password { get; set; }
    public string LoginName { get; set; }
    public int ResultCode { get; set; }
    public string ResultString { get; set; }
    public string ErrorSource { get; set; }
    public string ErrorStackTrace { get; set; }   
}
