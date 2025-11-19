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


public class bo_homepage
{

    public string Module { get; set; }
    public string AgentID { get; set; }
    public string BckTime { get; set; }
    public string ServiceID { get; set; }
    public string CallNumber { get; set; }
    public string Passwod { get; set; }
    public string User { get; set; }
    public string CategoryID { get; set; }
    public string CustomerId { get; set; }
    public int RoleID { get; set; }
    public int parentId { get; set; }
    public string Category { get; set; }
    public string Sub_Category { get; set; }
    public string Center { get; set; }
    public string Text { get; set; }
    public int ResultCode { get; set; }
    public string ResultString { get; set; }
    public string ErrorSource { get; set; }
    public string ErrorStackTrace { get; set; }   
}
