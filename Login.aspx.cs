using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Telerik.Web.UI;
using System.Configuration;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Data.Odbc;

public partial class LoginPage : System.Web.UI.Page
{
    CtiWS.CtiWS cti = new CtiWS.CtiWS();
    GlobalWS objGlobal = new GlobalWS();
    DataBase m_Connection = new DataBase();
    bo_ForLogin obj_Login = new bo_ForLogin();
    string nsessionid = string.Empty;
    string strUser = string.Empty;
    string strPwd = string.Empty;
    string strTerminal = string.Empty;

    // elision dialer query sting value 
    string CTIagentid = "";
    string CTIagentidpassword = "";
    string lead_id = "";
    string serviceid = "";
    string callid = "";
    string terminal = "";
    string calltype = "";
    string callStartTime = "";
    string campaign = "";
    string session_id = "";
    string phone = "";
    string recording_filename = "";
    string recording_id = "";
    string call_id = "";
    string session_name = "";
    
    protected void Page_Load(object sender, EventArgs e)
    {    
        if (!Page.IsPostBack)
        {
            if (Request.QueryString["user"] != null)
                CTIagentid = Request.QueryString["user"];
            Session["CTIagentid"] = CTIagentid.ToString();

            if (Request.QueryString["pass"] != null)
                CTIagentidpassword = Request.QueryString["pass"];
            Session["CTIPassword"] = CTIagentidpassword.ToString();

            if (Request.QueryString["lead_id"] != null)
                lead_id = Request.QueryString["lead_id"];
            Session["LeadId"] = lead_id.ToString();

            if (Request.QueryString["campaign"] != null)
                campaign = Request.QueryString["campaign"];
            Session["Campaign"] = campaign.ToString();

            if (Request.QueryString["session_id"] != null)
                session_id = Request.QueryString["session_id"];
            Session["SessionId"] = session_id.ToString();

            if (Request.QueryString["phone"] != null)
                phone = Request.QueryString["phone"];
            Session["Phone"] = phone.ToString();

            if (Request.QueryString["recording_filename"] != null)
                recording_filename = Request.QueryString["recording_filename"];
            Session["RecFile"] = recording_filename.ToString();

            if (Request.QueryString["recording_id"] != null)
                recording_id = Request.QueryString["recording_id"];

            if (Request.QueryString["call_id"] != null)
                call_id = Request.QueryString["call_id"];
            Session["CallId"] = call_id.ToString();

            if (Request.QueryString["session_name"] != null)
                session_name = Request.QueryString["session_name"];
            Session["SessionName"] = session_name.ToString();

            if (Request.QueryString["SIPexten"] != null)
                terminal = Request.QueryString["SIPexten"];
            Session["Terminal"] = terminal.ToString();
            
            if(CTIagentid!="" && CTIagentidpassword != "" && hdnUserId.Value=="0")
            {
                hdnUserId.Value = CTIagentid.ToString();
                LoginCTI(CTIagentid, CTIagentidpassword);
            }
            else if(CTIagentid.ToString()== hdnUserId.Value.ToString())
            {
                Response.Redirect("HomePage.aspx", false);
            }
            
            Page.Form.DefaultFocus = ctlLogin.FindControl("UserName").UniqueID;
            Page.Form.DefaultButton = ctlLogin.FindControl("Login").UniqueID;
            ctlLogin.Focus();
            //Login();          
        }
    }

    protected void ctlLogin_Authenticate(object sender, AuthenticateEventArgs e)
    {
        Login();
    }

    private void Login()
    {
        try
        {
            DataTable dt;
            strUser = ((RadTextBox)ctlLogin.FindControl("UserName")).Text;
            strPwd = ((RadTextBox)ctlLogin.FindControl("Password")).Text;
            {              
                FrameworkWS objFrameworkWS = new FrameworkWS();
                if(strUser=="" && strPwd == "")
                {
                    dt = objFrameworkWS.ValidateLogin(CTIagentid, CTIagentidpassword, Request.UserHostAddress);
                }
                else
                {
                    dt = objFrameworkWS.ValidateLogin(strUser, strPwd, Request.UserHostAddress);
                }
                if (Convert.ToInt32(dt.Rows[0]["ResultCode"]) != 0)
                {
                    ctlLogin.FailureText = dt.Rows[0]["ResultString"].ToString();
                    return;
                }
                Session["contact_id"] = dt.Rows[0]["contact_id"].ToString();
                 Session["login_id"] = dt.Rows[0]["contact_login_id"].ToString();
                Session["login_name"] = dt.Rows[0]["contact_full_name"].ToString();
                Session["login_pwd"] = strPwd;
                Session["role_id"] = dt.Rows[0]["contact_role"].ToString();
                Session["theme"] = dt.Rows[0]["contact_current_theme"].ToString();
                Session["HOSTID"] = Request.UserHostAddress;
                Session["show_menubar"] = dt.Rows[0]["contact_show_menubar"].ToString();
                Session["show_toolbar"] = dt.Rows[0]["contact_show_toolbar"].ToString();
                Session["show_alertbar"] = dt.Rows[0]["contact_show_alertbar"].ToString();
                Session["show_show_popup"] = dt.Rows[0]["contact_show_popup"].ToString();
                Session["AccountType"] = dt.Rows[0]["AccountType"].ToString();
                Session["AccountName"] = dt.Rows[0]["AccountName"].ToString();
                Session["AccountID"] = dt.Rows[0]["AccountID"].ToString();
                Session["TeamID"] = dt.Rows[0]["TeamID"].ToString();
                Session["TeamName"] = dt.Rows[0]["TeamName"].ToString();
                Session["DeptID"] = dt.Rows[0]["DeptID"].ToString();
                Session["DesigID"] = dt.Rows[0]["DesigID"].ToString();
                Session["TimeZoneID"] = dt.Rows[0]["TimeZoneID"].ToString();
                Session["TimeZoneTimeSpan"] = dt.Rows[0]["TimeZoneTimeSpan"].ToString();
                Session["DesigLevel"] = dt.Rows[0]["DesigLevel"].ToString();
                Session["CtiURL"] = dt.Rows[0]["CtiUrl"].ToString();
                Session["LoggedIn"] = true;
                Session["Session_ID"] = dt.Rows[0]["ResultString"];
                
                //LoginAgent(strName, strPwd, strTerminal, strTerminal, 2);
                Response.Redirect("HomePage.aspx", false);
            }

        }
        catch (Exception ex)
        {
            ctlLogin.FailureText = ex.Message;
        }
    }

    private void LoginCTI(string strLoginId,string strPassword)
    {
        try
        {
            DataTable dt;
            {
                FrameworkWS objFrameworkWS = new FrameworkWS();
                dt = objFrameworkWS.ValidateLogin(strLoginId, strPassword, Request.UserHostAddress);
                if (Convert.ToInt32(dt.Rows[0]["ResultCode"]) != 0)
                {
                    ctlLogin.FailureText = dt.Rows[0]["ResultString"].ToString();
                    return;
                }
                Session["contact_id"] = dt.Rows[0]["contact_id"].ToString();
                Session["login_id"] = dt.Rows[0]["contact_login_id"].ToString();
                Session["login_name"] = dt.Rows[0]["contact_full_name"].ToString();
                Session["login_pwd"] = strPwd;
                Session["role_id"] = dt.Rows[0]["contact_role"].ToString();
                Session["theme"] = dt.Rows[0]["contact_current_theme"].ToString();
                Session["HOSTID"] = Request.UserHostAddress;
                Session["show_menubar"] = dt.Rows[0]["contact_show_menubar"].ToString();
                Session["show_toolbar"] = dt.Rows[0]["contact_show_toolbar"].ToString();
                Session["show_alertbar"] = dt.Rows[0]["contact_show_alertbar"].ToString();
                Session["show_show_popup"] = dt.Rows[0]["contact_show_popup"].ToString();
                Session["AccountType"] = dt.Rows[0]["AccountType"].ToString();
                Session["AccountName"] = dt.Rows[0]["AccountName"].ToString();
                Session["AccountID"] = dt.Rows[0]["AccountID"].ToString();
                Session["TeamID"] = dt.Rows[0]["TeamID"].ToString();
                Session["TeamName"] = dt.Rows[0]["TeamName"].ToString();
                Session["DeptID"] = dt.Rows[0]["DeptID"].ToString();
                Session["DesigID"] = dt.Rows[0]["DesigID"].ToString();
                Session["TimeZoneID"] = dt.Rows[0]["TimeZoneID"].ToString();
                Session["TimeZoneTimeSpan"] = dt.Rows[0]["TimeZoneTimeSpan"].ToString();
                Session["DesigLevel"] = dt.Rows[0]["DesigLevel"].ToString();
                Session["CtiURL"] = dt.Rows[0]["CtiUrl"].ToString();
                Session["LoggedIn"] = true;
                Session["Session_ID"] = dt.Rows[0]["ResultString"];

                Response.Redirect("HomePage.aspx", false);
            }

        }
        catch (Exception ex)
        {
            ctlLogin.FailureText = ex.Message;
        }
    }

    protected void Login_Click(object sender, ImageClickEventArgs e)
    {

    }

}