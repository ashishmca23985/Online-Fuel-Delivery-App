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

public partial class Alerts_CreateAlerts : ThemeBase
{
    static int nAlertID = 0;
    static string szPopupURL = string.Empty;
    static string szPopupContainer = string.Empty;
    static string szUserName = string.Empty;
    static string szAlertType = string.Empty;
    static int nUserTimeZoneSpan = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            nAlertID = Convert.ToInt32(Request.QueryString["AlertId"]);
            szAlertType = Convert.ToString(Request.QueryString["AType"]);

            lblCaption.Text = "New Alert";

            if (szAlertType == "B")
            {
                //btnCancel.Text = "Cancel";
                btnCancel.Attributes.Add("onclick", "return CloseCancel('B')");
            }
            else
            {
                //btnCancel.Text = "Close";
                btnCancel.Attributes.Add("onclick", "return CloseCancel('C')");
            }

            if (!String.IsNullOrEmpty(Convert.ToString(Request.QueryString["Date"])))
                txtAlertDate.SelectedDate = Convert.ToDateTime(Request.QueryString["Date"]);
            else
            {
                if (!String.IsNullOrEmpty(Request.QueryString["TimeSpan"]))
                    nUserTimeZoneSpan = Convert.ToInt32(Request.QueryString["TimeSpan"]);
                else
                {
                    if (Session["contact_timezone_timespan"] != null)
                        nUserTimeZoneSpan = Convert.ToInt32(Session["contact_timezone_timespan"]);
                }

                txtAlertDate.SelectedDate = DateTime.UtcNow.Add(new TimeSpan(0, nUserTimeZoneSpan, 0));
            }

            txtSubject.Text = Convert.ToString(Request.QueryString["Subject"]);
            txtDescription.Content = Convert.ToString(Request.QueryString["Description"]);
            szPopupURL = Convert.ToString(Request.QueryString["Url"]);
            if (szPopupURL != null)
            {
                szPopupURL = szPopupURL.Replace("$", "&");
            }
            szPopupContainer = Convert.ToString(Request.QueryString["UrlContainer"]);

            string szUsersIds = Convert.ToString(Request.QueryString["UserId"]);
            string szUsersNames = Convert.ToString(Request.QueryString["UserName"]);
            if (!String.IsNullOrEmpty(szUsersIds) && !String.IsNullOrEmpty(szUsersNames))
            {
                string[] arrUserIds = new string[3];
                arrUserIds = szUsersIds.Split(',');

                string[] arrUserNames = new string[3];
                arrUserNames = szUsersNames.Split(',');
                lstUsers.Items.Clear();
                for (int i = 0; i < arrUserIds.Length; i++)
                {
                    ListItem lstItem = new ListItem();
                    lstItem.Value = Convert.ToString(arrUserIds[i]);
                    lstItem.Text = Convert.ToString(arrUserNames[i]);
                    lstUsers.Items.Add(lstItem);
                }
            }
        }
    }

    #region Button Submit Click
    protected void btnSave_Click(object sender, EventArgs e)
    {
        CreateAlertConfiguration();
    }
    #endregion

    #region Create Alerts
    private void CreateAlertConfiguration()
    {
        string szDate = string.Empty;
        string szSubject = string.Empty;
        string szDescription = string.Empty;
        int nCounter = 0;
        string szUserIDs = string.Empty;
        string szSMSText = string.Empty;
        string szEMailBody = string.Empty;
        string szRelatedTo = string.Empty;
        string szRelatedToName = string.Empty;
        int nRelatedToID = 0;

        try
        {
            DataTable dtTable = new DataTable();
            szDate = Convert.ToString(txtAlertDate.SelectedDate);
            szSubject = txtSubject.Text.Trim();
            szDescription = txtDescription.Content.Trim();

            if (nAlertID <= 0)
                nAlertID = 1; //- set as manual alert

            for (int i = 0; i < lstUsers.Items.Count; i++)
            {
                if(String.IsNullOrEmpty(szUserIDs))
                    szUserIDs = lstUsers.Items[i].Value;
                else
                    szUserIDs += "," + lstUsers.Items[i].Value;
            }

            szSMSText = szSubject;
            szEMailBody = szDescription;

            if (!String.IsNullOrEmpty(Request.QueryString["RTo"]))
            {
                szRelatedTo = Convert.ToString(Request.QueryString["RTo"]);
                szRelatedToName = Convert.ToString(Request.QueryString["RToName"]);
                nRelatedToID = Convert.ToInt32(Request.QueryString["RToID"]);
            }

            dtTable = new DataTable();
            /*dtTable = (DataTable)objFrameworkWS.wmAlertTransactionSetDetails(nAlertID,
                                                                szUserIDs, szDate,
                                                                szSubject, 
                                                                szUserName, szDescription,
                                                                szStatus, szPopupURL,
                                                                szPopupContainer, TB_nContactID, 0, 
                                                                TB_nSessionID, szEMailBody,
                                                                szSMSText, nRelatedToID, szRelatedTo,
                                                                szRelatedToName);*/

            if (Convert.ToInt32(dtTable.Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(new Exception(dtTable.Rows[0]["ResultString"].ToString()), 1);
                return;
            }
            nCounter++;

            if (nCounter > 0)
            {
                LogMessage(new Exception("Alert saved successfully"),2);
                string szTemp = "javascript:self.close();";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "clientScript", szTemp, true);
            }

        }
        catch (Exception ex)
        {
            LogMessage(ex, 1);
            return;
        }
    }
    #endregion            

    #region lnkHidden Click
    protected void lnkHiddenClickRemove_Click(object sender, EventArgs e)
    {
        string szUsersIds = hdnUserIds.Value.Trim();
        ListItem lstItem = lstUsers.Items.FindByValue(szUsersIds);
        lstUsers.Items.Remove(lstItem);
    }
      #endregion

    #region lnkHidden Click
    protected void lnkHiddenClick_Click(object sender, EventArgs e)
    {
        
        string szUsersIds = string.Empty;
        string szUsersNamess = string.Empty;
        try
        {
            string[] arrUserIds;
            string[] arrUserNames;
            char[] szsplitter = { ',' };

            szUsersIds = hdnUserIds.Value.Trim();
            arrUserIds = szUsersIds.Split(szsplitter);

            szUsersNamess = hdnUserNames.Value.Trim();
            arrUserNames = szUsersNamess.Split(szsplitter);
            if (lstUsers.Items.Count > 0)
            {
                for (int j = 0; j < lstUsers.Items.Count; j++)
                {
                   for(int k=0; k<arrUserIds.Length ;k++)
                   {
                       if (lstUsers.Items.Count > 0)
                       {
                           if (Convert.ToInt32(lstUsers.Items[j].Value) == Convert.ToInt32(arrUserIds[k]))
                           {
                               lstUsers.Items.RemoveAt(j);
                           }
                       }
                   }
                }
            }
            for (int i = 0; i < arrUserIds.Length; i++)
            {
                if (arrUserIds[i] != null)
                {
                    ListItem lstItem = new ListItem();
                    lstItem.Value = Convert.ToString(arrUserIds[i]);
                    lstItem.Text = Convert.ToString(arrUserNames[i]);
                    lstUsers.Items.Add(lstItem);
                }
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex, 1);
            return;
        }
    }
    #endregion

    #region Log Message
    void LogMessage(Exception e, Int32 param)
    {

        /*** used to show the response as a label text ***/
        if (param == 1)
        {
            lblMessage.Visible = true;
            lblMessage.Text = e.Message.ToString().Substring(0, e.Message.Length);
            lblMessage.ForeColor = System.Drawing.Color.Red;
        }
        else if (param == 3)
        {
            lblMessage.Visible = true;
            lblMessage.Text = e.Message.ToString().Substring(0, e.Message.Length);
            lblMessage.ForeColor = System.Drawing.Color.Blue;
        }
        else if (param == 100)
        {
            lblMessage.Visible = false;
            lblMessage.Text = e.Message;
            lblMessage.ForeColor = System.Drawing.Color.Green;
        }
        else
        {
            lblMessage.Visible = true;
            lblMessage.Text = e.Message;
            lblMessage.ForeColor = System.Drawing.Color.Green;
        }
    }
    #endregion
}
