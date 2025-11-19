using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;

public partial class loginform : System.Web.UI.Page
{
    string UserId = string.Empty;
    string Password = string.Empty;

    CtiWS.CtiWS cti = new CtiWS.CtiWS();

    protected void Page_Init(object Sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        Response.Cache.SetNoStore();
        Response.Cache.SetNoServerCaching();
        Response.Cache.SetAllowResponseInBrowserHistory(false);
    }
    protected void Page_Preinit(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Session["ctilogin"] = "N";
//        Page.Theme = "Transparent";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
            return;
        Session["RUNNINGSERIES"] = 0;    
        try
        {
            /*----------------AUTO LOGIN-------------------------*/
            if (Session["UserId"] != null && Convert.ToInt32(Session["UserId"]) > 0)
            {
                string strBaseTerminalId = "";
                string strTerminalId = "";
                string strLoginId = "";
                string strPassword = "";

                DataSet ds = cti.GetAgentInfo(Convert.ToInt32(Session["UserId"]));
                DataSet ds1 = cti.GetTerminalInfo(Request.UserHostName, Request.UserHostAddress);

                if (ds.Tables["AgentInfo"].Rows.Count > 0)
                {
                    strLoginId = ds.Tables["AgentInfo"].Rows[0]["Login_Id"].ToString();
                    strPassword = ds.Tables["AgentInfo"].Rows[0]["Login_Password"].ToString();
                }
                if (ds.Tables["UserInfo"].Rows.Count > 0)
                    strTerminalId = ds.Tables["UserInfo"].Rows[0]["AssociatedAgentTerminal"].ToString();
                if (ds1.Tables["TerminalInfo"].Rows.Count > 0)
                    strBaseTerminalId = ds1.Tables["TerminalInfo"].Rows[0]["Terminal_Id"].ToString();

                //  Auto close call and logout incase browser closed // 
//                cti.CloseCall("", "", "NOR", "", "", "BROWSER CLOSED", Request.UserHostAddress);
//                cti.Logout("", "", Request.UserHostAddress);
                //  Auto close call and logout incase browser closed // 
                
                    LoginAgent(strLoginId, strPassword, strTerminalId, strBaseTerminalId, 0);
                
            }
            /*----------------AUTO LOGIN-------------------------*/
        }
        catch (Exception ex)
        {
            LogMessage(ex, 1);
        }
    }
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        if (Session["login_id"] != null)
            UserId = Session["login_id"].ToString();

        if (Session["login_pwd"] != null)
            Password = Session["login_pwd"].ToString();

        if (UserId.ToString() == txtLoginId.Text && Password == txtPassword.Text)
        {
            LoginAgent(txtLoginId.Text, txtPassword.Text, txtTerminalId.Text, txtTerminalId.Text, 2);
        }
        else
        {
            lblMessage.Text = "Please Enter Valid UserNamee and Password.";
        }
       // LoginAgent(txtLoginId.Text, txtPassword.Text, txtTerminalId.Text, txtTerminalId.Text, 2);
    }
    public void LoginAgent(string strAgentId, string strTerminalPassword, string strTerminalId, string strBaseTerminalId, int Source)
    {
        try
        {
            Session["LOGINID"] = txtLoginId.Text;
            string status = "";
            DataTable dtTable = cti.GetStatus("", "", Request.UserHostAddress, Convert.ToInt32(Session["RUNNINGSERIES"]));
            if (dtTable != null)
            {
                if (dtTable.Rows.Count == 0 || Convert.ToInt32(dtTable.Rows[0]["ResultCode"]) == -1)
                {
                    lblMessage.Text = dtTable.Rows[0]["ResultString"].ToString();
                    return;
                }
                status = dtTable.Rows[0]["ResultString"].ToString();
                dtTable.Dispose();
            }
            if (status.Contains("IDLE") || status.Contains("BREAK") || status.Contains("RESERVED") || status.Contains("INCALL") || status.Contains("WRAPUP"))
            {
                HttpCookie CkUserId = new HttpCookie("USERID");
                CkUserId.Value = strAgentId;
                DateTime dtNow = DateTime.Now;
                TimeSpan tsMinute = new TimeSpan(0, 0, 500, 0);
                CkUserId.Expires = dtNow + tsMinute;
                Response.Cookies.Add(CkUserId);

                Session["HOSTID"] = Request.UserHostAddress;// +"." + Session["Counter"];
                Session["LOGINID"] = txtLoginId.Text;
                Session["ctilogin"] = "Y";
                Response.Redirect("homepage.aspx", false);
                return;
            }
            else
            {
                if (strTerminalId.Length <= 0)
                {
                    lblMessage.Text = "No Terminal Defined, Please contact administrator";
                    return;
                }
                dtTable = cti.Login("", "", strAgentId, strTerminalPassword, strTerminalId, strBaseTerminalId, Request.UserHostAddress, Source);

                if (dtTable != null)
                {
                    if (dtTable.Rows.Count == 0 || Convert.ToInt32(dtTable.Rows[0]["ResultCode"]) == -1)
                    {
                        lblMessage.Text = dtTable.Rows[0]["ResultString"].ToString();
                        return;
                    }
                    status = dtTable.Rows[0]["ResultString"].ToString();
                    dtTable.Dispose();
                }

                int nn = 0;
                while (nn++ < 10)
                {
                    System.Threading.Thread.Sleep(1000);

                    dtTable = cti.GetStatus("", "", Request.UserHostAddress, Convert.ToInt32(Session["RUNNINGSERIES"]));
                    if (dtTable.Rows.Count == 0 || Convert.ToInt32(dtTable.Rows[0]["ResultCode"]) == -1)
                    {
                        lblMessage.Text = dtTable.Rows[0]["ResultString"].ToString();
                        return;
                    }

                    status = dtTable.Rows[0]["ResultString"].ToString();
                    if (status.Contains("FAIL-NOTFOUND"))
                    {
                        return;
                    }
                    else if (status.Contains("IDLE") || status.Contains("BREAK") || status.Contains("RESERVED") || status.Contains("INCALL") || status.Contains("WRAPUP"))
                    {
                        HttpCookie CkUserId = new HttpCookie("USERID");
                        CkUserId.Value = strAgentId;
                        DateTime dtNow = DateTime.Now;
                        TimeSpan tsMinute = new TimeSpan(0, 0, 500, 0);
                        CkUserId.Expires = dtNow + tsMinute;
                        Response.Cookies.Add(CkUserId);

                        Session["HOSTID"] = Request.UserHostAddress;// +"." + Session["Counter"];
                        Session["LOGINID"] = txtLoginId.Text;
                        Response.Redirect("homepage.aspx", false);
                        return;
                    }
                    else if (status.Contains("Terminal ID already used")
                        || status.Contains("Agent already LOGGED IN")
                        || status.Contains("Agent id does not exist")
                        || status.Contains("Agent is not active")
                        || status.Contains("Password is not correct")
                        || status.Contains("Already Logged in")
                        || status.Contains("not connected to database")
                        || status.Contains("Maximum Login Sessions LIMIT")
                        || status.Contains("Invalid Version")
                        || status.Contains("less parameters for login"))
                    {
                        lblMessage.Text = status;
                        return;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex, 1);
        }
    }

    #region Log Message
    void LogMessage(Exception e, Int32 param)
    {
        if (param == 1)
        {
            lblMessage.Text = e.Message;
            lblMessage.CssClass = "error";
        }
        else
        {
            lblMessage.Text = e.Message;
            lblMessage.CssClass = "success";
        }
    }
    #endregion
}