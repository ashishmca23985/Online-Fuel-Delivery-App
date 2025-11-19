using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

public partial class Alerts_AlertPopup : ThemeBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Page.IsPostBack)
            Get_AlertDetails();
    }

    #region Get Alert Details 
    private void Get_AlertDetails()
    {
        try
        {
            DataTable dtTable = objFrameworkWS.GetUserAlerts(TB_nContactID);
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region Log Message
    void LogMessage(string Message, Int32 param)
    {
        lblMessage.Text = Message;
        if (param == 1)
            lblMessage.CssClass = "error";
        else
            lblMessage.CssClass = "success";
    }
    #endregion
}
