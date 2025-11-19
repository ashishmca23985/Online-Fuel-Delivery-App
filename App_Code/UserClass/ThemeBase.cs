using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Telerik.Web.UI;

/// <summary>
/// This class will inherited in all the pages that are included in this application,
/// so that theme is applied to all the pages dynamically.
/// </summary>
public class ThemeBase : System.Web.UI.Page
{
    public int TB_nSessionID = 0;
    public string TB_strShowMenubar = "Y";
    public string TB_strShowToolbar = "Y";
    public string TB_strShowAlertbar = "Y";
    public string TB_strShowPopup = "Y";

    public int TB_nUserRoleID = 0;
    public int TB_nContactID = 0;
    public string TB_strEmployeeName = "";
    public string TB_strUserName = "";
    public string TB_strTheme = "Yellowstone";
    public string TB_strAccountName = "";
    public string TB_strTeamName = "";
    public int TB_nAccountID = 0;
    public int TB_nTeamID = 0;
    public int TB_nDepartmentID = 0;
    public int TB_nDesignationID = 0;
    public int TB_nDesignationLevel = 0;
    public string TB_strAccountType = "";
    public int TB_nTimeZoneID = 0;
    public int TB_nTimeZoneTimeSpan = 0;
    public int TB_ErrorCode = 0;
    public string TB_ErrorString = string.Empty;
    public FrameworkWS objFrameworkWS;    
    string strAppThemes = string.Empty;
    public string theme = "Yellowstone";
    public ThemeBase()
	{
        objFrameworkWS = new FrameworkWS();
    }

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        try
        {
            //--set no cache
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
            Response.Cache.SetNoStore();
            Response.Cache.SetNoServerCaching();

            if (Session["Session_ID"] != null && !string.IsNullOrEmpty(Session["Session_ID"].ToString()))
                TB_nSessionID = Convert.ToInt32(Session["Session_ID"]);

            if (TB_nSessionID > 0)
            {
                TB_strAccountType = Session["AccountType"].ToString();
                TB_strAccountName = Session["AccountName"].ToString();
                TB_strTeamName = Session["TeamName"].ToString();
                TB_nAccountID = Convert.ToInt32(Session["AccountID"]);
                TB_nTeamID = Convert.ToInt32(Session["TeamID"]);
                TB_nDepartmentID = Convert.ToInt32(Session["DeptID"]);
                TB_nDesignationID = Convert.ToInt32(Session["DesigID"]);
                TB_nDesignationLevel = Convert.ToInt32(Session["DesigLevel"]);
                TB_nTimeZoneID = Convert.ToInt32(Session["TimeZoneID"]);

                TB_nContactID = Convert.ToInt32(Session["contact_id"]);
                TB_strUserName = Session["login_id"].ToString();
                TB_strEmployeeName = Session["login_name"].ToString();
                TB_nUserRoleID = Convert.ToInt32(Session["role_id"]);
                TB_strTheme = Session["theme"].ToString();
                TB_strShowMenubar = Session["show_menubar"].ToString();
                TB_strShowToolbar = Session["show_toolbar"].ToString();
                TB_strShowAlertbar = Session["show_alertbar"].ToString();
                TB_strShowPopup = Session["show_show_popup"].ToString();
                TB_nTimeZoneTimeSpan = Convert.ToInt32(Session["TimeZoneTimeSpan"]);
            }

            string strAppThemes = Convert.ToString(Application["Themes"]);
            if (strAppThemes.Contains(TB_strTheme))
            {
                Page.Theme = TB_strTheme;
                theme = Helpter.kendotheme(TB_strTheme);
            }
            else
                Page.Theme = "Yellowstone";
            
            if (TB_ErrorCode != 0 || TB_nSessionID <= 0)
            {
                if (TB_ErrorCode != 0)
                    Response.Write(TB_ErrorString);
                else
                    Response.Redirect("~/SessionExpired.aspx");
            }
        }
        catch (Exception ex)
        {
            TB_ErrorCode = -1;
            TB_ErrorString = ex.Message;
            Page.Theme = "Yellowstone";
            Response.Redirect("~/SessionExpired.aspx");
        }
    }

}
public class UserControlBase : System.Web.UI.UserControl
{
    public int TB_nSessionID = 0;
    public string TB_strShowMenubar = "Y";
    public string TB_strShowToolbar = "Y";
    public string TB_strShowAlertbar = "Y";
    public string TB_strShowPopup = "Y";

    public int TB_nUserRoleID = 0;
    public int TB_nContactID = 0;
    public string TB_strEmployeeName = "";
    public string TB_strUserName = "";
    public string TB_strTheme = "Yellowstone";
    public string TB_strAccountName = "";
    public int TB_nAccountID = 0;
    public int TB_nTeamID = 0;
    public int TB_nDepartmentID = 0;
    public int TB_nDesignationID = 0;
    public int TB_nDesignationLevel = 0;
    public string TB_strAccountType = "";
    public int TB_nTimeZoneID = 0;
    public int TB_nTimeZoneTimeSpan = 0;
    public int TB_ErrorCode = 0;
    public string TB_ErrorString = string.Empty;
    public FrameworkWS objFrameworkWS;
    string strAppThemes = string.Empty;
    public UserControlBase()
    {
        objFrameworkWS = new FrameworkWS();
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        try
        {
            if (Session["Session_ID"] != null && !string.IsNullOrEmpty(Session["Session_ID"].ToString()))
                TB_nSessionID = Convert.ToInt32(Session["Session_ID"]);

            if (TB_nSessionID > 0)
            {
                TB_strAccountType = Session["AccountType"].ToString();
                TB_strAccountName = Session["AccountName"].ToString();
                TB_nAccountID = Convert.ToInt32(Session["AccountID"]);
                TB_nTeamID = Convert.ToInt32(Session["TeamID"]);
                TB_nDepartmentID = Convert.ToInt32(Session["DeptID"]);
                TB_nDesignationID = Convert.ToInt32(Session["DesigID"]);
                TB_nDesignationLevel = Convert.ToInt32(Session["DesigLevel"]);
                TB_nTimeZoneID = Convert.ToInt32(Session["TimeZoneID"]);

                TB_nContactID = Convert.ToInt32(Session["contact_id"]);
                TB_strUserName = Session["login_id"].ToString();
                TB_strEmployeeName = Session["login_name"].ToString();
                TB_nUserRoleID = Convert.ToInt32(Session["role_id"]);
                TB_strTheme = Session["theme"].ToString();
                TB_strShowMenubar = Session["show_menubar"].ToString();
                TB_strShowToolbar = Session["show_toolbar"].ToString();
                TB_strShowAlertbar = Session["show_alertbar"].ToString();
                TB_strShowPopup = Session["show_show_popup"].ToString();
                TB_nTimeZoneTimeSpan = Convert.ToInt32(Session["TimeZoneTimeSpan"]);
            }

          
        }
        catch (Exception ex)
        {
          
        }
    }
}


