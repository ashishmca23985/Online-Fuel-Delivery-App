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

public partial class General_AddNotes : ThemeBase
{
    string objectType = string.Empty;
    string objectId = string.Empty;
    GlobalWS objGlobalWS = new GlobalWS();

    protected void Page_Load(object sender, EventArgs e)
    {
        objectType = "CAS";

        if (!string.IsNullOrEmpty(Request.QueryString["CaseID"]))
            objectId = Request.QueryString["CaseID"].ToString();
        else if (!string.IsNullOrEmpty(Request.QueryString["LeadID"]))
        {
            objectId = Request.QueryString["LeadID"].ToString();
            objectType = "LED";
        }


        if (IsPostBack)
            return;
        dtstatus.MinDate = DateTime.Now;

        lblMessage.Text = "";

    }
    #region Save Notes General
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
                DataSet objDataSet = objGlobalWS.Save_Notes(objectType, objectId, txtNote.Text + ". Next Status Time :- " + dtstatus.SelectedDate.Value.ToString("dd MMM yyyy HH:mm "), TB_nContactID);

                if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
                {
                    LogMessage(objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
                    return;
                }
                DataSet objDataSet1 = objGlobalWS.addStatus_case(objectId,objectType, dtstatus.SelectedDate.Value.AddMinutes(-TB_nTimeZoneTimeSpan).ToString("dd MMM yyyy HH:mm"));

                if (Convert.ToInt32(objDataSet1.Tables["Response"].Rows[0]["ResultCode"]) != 0)
                {
                    LogMessage(objDataSet1.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
                    return;
                }
                lblMessage.Text = "Saved successfully";
                btnSave.Visible = false;
                btnClose.Visible = true;
                
            
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
