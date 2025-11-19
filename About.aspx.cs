using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;

public partial class About : ThemeBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            GlobalWS objGlobalWS = new GlobalWS();
            lblUserName.Text = TB_strUserName;
            lblEmployeeName.Text = TB_strEmployeeName;
            lblIP.Text = Request.UserHostAddress;
            
           
            string strQuery = "SELECT #GetDepartmentName(" + TB_nDepartmentID + ")";
            

            strQuery = "SELECT #GetTeamName(" + TB_nTeamID + ")";
            

            strQuery = "SELECT #GetRoleName(" + TB_nUserRoleID + ")";
            lblRoleName.Text = objGlobalWS.ExecuteQuery(strQuery);
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }

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

