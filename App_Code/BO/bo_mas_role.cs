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
/// Summary description for bo_mas_role
/// </summary>
public class bo_mas_role
{

    public int CustId { get; set; }
    public string sExist { get; set; }
    public string sFinal { get; set; }
    public int ResultCode { get; set; }
    public string ResultString { get; set; }
    public string ErrorSource { get; set; }
    public string ErrorStackTrace { get; set; }
    public int iRoleID { get; set; }
    public string RoleID { get; set; }
    public string SearchText { get; set; }
    public string Name { get; set; }
    public string Enabled { get; set; }
    public string Desc { get; set; }
    public int i { get; set; }   


}
