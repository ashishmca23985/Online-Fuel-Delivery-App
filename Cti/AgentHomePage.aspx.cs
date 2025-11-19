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
using Telerik.Web.UI;
using System.Data.SqlClient;
using System.Xml;
using System.IO;
using System.Drawing;

public partial class AgentHomePage : System.Web.UI.Page
{
    string strLoginId = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["LoginId"] != null)
            strLoginId = Request.QueryString["LoginId"].ToString();
        else if(Session["LOGINID"] != null)
            strLoginId = Session["LOGINID"].ToString();
        else
            strLoginId = "0";
        if (!Page.IsPostBack)
        {
            try
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Clientscript1", "javascript:setTimeout('document.getElementById(\"btnStatus\").click()', 2000);", true);
            }
            catch (Exception)
            {
            }
        }
    }

    public void GetAgentStatus()
    {
        try
        {
            string url = "MyProfile.aspx?LoginId=" + strLoginId;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Clientscript3", "javascript:SetMyProfileTab(\"" + url + "\", 'My Home Page');", true);
        }
        catch (Exception)
        {
        }
    }

    protected void btnStatus_Click(object sender, EventArgs e)
    {
        GetAgentStatus();
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Clientscript", "javascript:setTimeout('document.getElementById(\"btnStatus\").click()', 30000);", true);
    }
}