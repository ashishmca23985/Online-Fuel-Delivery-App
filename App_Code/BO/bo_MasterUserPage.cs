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
/// Summary description for bo_MasterUserPage
/// </summary>
public class bo_MasterUserPage
{

    public string CtiExistAgent { get; set; }
    public int OutPutUserPage { get; set; }
    public string CustID { get; set; }
    public string CtiAgentName { get; set; }
    public string CtiAgentId { get; set; }
    public string ForInterdailogCon { get; set; }
    public string RoleName { get; set; }
    public string Status { get; set; }
    public int nID { get; set; }
    public string Type { get; set; }
    public string LoginName { get; set; }
    public string CTIName { get; set; }
    public string RoleId { get; set; }
    public string CtiLoginNameGd { get; set; }
    public string CtiPasswordGd { get; set; }
    public string LoginNameGd { get; set; }
    public string PasswordGd { get; set; }
    public string RoleIdGd { get; set; }
    public int ResultCode { get; set; }
    public string ResultString { get; set; }
    public string ErrorSource { get; set; }
    public string ErrorStackTrace { get; set; }
    public string EscalationLevel { get; set; }
    public string ContactFullName { get; set; }
    public string ContactEmailID { get; set; }
    public string ContactContactNumber { get; set; }
    public string ContactTypeId { get; set; }
}
