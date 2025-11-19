using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Telerik.Web.UI;
using System.Xml;


public partial class ShowObject : ThemeBase
{
    public string objectType = string.Empty;
    public string objectId = string.Empty;
    string objectName = string.Empty;
    public string sourceType = string.Empty;
    public string sourceId = string.Empty;
    public string url = string.Empty;
    int nTabBreakIndex = 8;
   // DataSet dsDataset = null;
    GlobalWS objGlobalWS = new GlobalWS();

    protected void Page_Load(object sender, EventArgs e)
    {

        url = Request.Url.Query;

        lblMessage.Text = "";

        objectId = "0";
        if (ViewState["OBJECTID"] == null)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["ID"]))
                objectId = Request.QueryString["ID"].ToString();
        }
        else
            objectId = ViewState["OBJECTID"].ToString();

        if (!string.IsNullOrEmpty(Request.QueryString["ObjectType"]))
            objectType = Request.QueryString["ObjectType"].ToString();

        if (!string.IsNullOrEmpty(Request.QueryString["Source"]))
            sourceType = Request.QueryString["Source"].ToString();

        if (!string.IsNullOrEmpty(Request.QueryString["SourceId"]))
            sourceId = Request.QueryString["SourceId"].ToString();
        //if (!string.IsNullOrEmpty(Request.QueryString["CLI"]))
        //{
        //    url = Request.Url.Query;
        //    url = url.Replace(Helpter.getBetween(url, "ObjectType", "&"), "");
        //    url = url.Replace(Helpter.getBetween(url, "Source", "&"), "");
        //    url = url.Replace(Helpter.getBetween(url, "SourceId", "&"), "");
        //    url = url.Replace(Helpter.getBetween(url, "ID", "&"), "");
        //}
        if (IsPostBack)
            return;

        DataBase m_Connection = new DataBase();
        m_Connection.OpenDB("Galaxy");

        if (sourceType == "QUE")
        {
            DataTable dt1 = m_Connection.ChangeTransactionOwner(objectType, objectId, sourceId, TB_nContactID.ToString());
            if (Convert.ToInt32(dt1.Rows[0]["ResultCode"]) < 0)
            {
                //LogMessage(Convert.ToString(dt1.Rows[0]["ResultString"]), 1);
                LogMessage("You are not able to add from queue", 1);
                return;
            }
        }

        //--check permission level and disable buttons accordingly
        DataSet dsDataSet = objFrameworkWS.GetObjectPermissionLevel(TB_nUserRoleID, objectType);
        if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
        {
            lblMessage.Text = Convert.ToString(dsDataSet.Tables["Response"].Rows[0]["ResultString"]);
            return;
        }
        else if (dsDataSet.Tables["Permisson"].Rows.Count == 0)
        {
            lblMessage.Text = "No Permisson defined";
            return;
        }
        else if (objFrameworkWS.Edit == false && Convert.ToInt32(objectId) > 0)
        {
            lblMessage.Text = "No Permisson for Edit";
            return;
        }
        else if (objFrameworkWS.Add == false && Convert.ToInt32(objectId) == 0)
        {
            lblMessage.Text = "No Permisson for Add";
            return;
        }

        objectName = dsDataSet.Tables["Permisson"].Rows[0]["object_add_name"].ToString();

        imgType.Src = dsDataSet.Tables["Permisson"].Rows[0]["image_path"].ToString();
        imgType.Alt = objectName;
        
        if (Convert.ToInt32(objectId) > 0)
        {
            lblCaption.Text = "Edit " + objectName;
        }
        else
        {
            lblCaption.Text = "Add " + objectName;

            XmlDocument xMainDoc = m_Connection.createParameterXML();
            DataTable dt1 = m_Connection.SaveTransactionData(objectType, 0, "N", DateTime.UtcNow, TB_nContactID, Request.UserHostAddress, xMainDoc);
            if (Convert.ToInt32(dt1.Rows[0]["ResultCode"]) <= 0)
            {
                lblMessage.Text = Convert.ToString(dsDataSet.Tables["Response"].Rows[0]["ResultString"]);
                return;
            }

            objectId = dt1.Rows[0]["ResultCode"].ToString();
        }
        ViewState["OBJECTID"] = objectId;

        BindTabs();
      //  BindCustomTab();

        if(rdTabCommon.Tabs.Count > 0)
        {
            rdTabCommon.Tabs[0].Selected = true;
            rdTabCommon.SelectedIndex = 0;
            rdMPCommon.PageViews[0].Selected = true;
        }
    }

    #region Bind Custom Tab
    private void BindCustomTab()
    {
        string strActivityAction = "";  
        string strImageUrl = "";
        string strActivityName = "Custom";

        try
        {
            DataBase m_Connection = new DataBase();
            m_Connection.OpenDB("Galaxy");
            string strColumns = "'TabName','TabURL'";
            DataSet objDataSet = m_Connection.FetchTransactionData(objectType, "", strColumns, Convert.ToInt32(objectId), TB_nTimeZoneTimeSpan);

            if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) == -1)
            {
                LogMessage(objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
                return;
            }

            if (objDataSet.Tables.Contains("Data"))
            {
                DataRow row = objDataSet.Tables["Data"].Rows[0];

                strActivityName = row["TabName"].ToString();
                strActivityAction = row["TabURL"].ToString();
                if (string.IsNullOrEmpty(strActivityName))
                    strActivityName = "Custom";
                CreateTab(strActivityName, strImageUrl, strActivityAction, "Y", "Custom");
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
            return;
        }
    }
    #endregion

    #region Bind Tabs
    private void BindTabs()
    {
        string strResult = string.Empty;
        try
        {
            DataSet TabInfo = objFrameworkWS.GetTabList(objectType, TB_nUserRoleID.ToString(), objectId);
            if (Convert.ToInt64(TabInfo.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(TabInfo.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
                return;
            }
            else if (TabInfo.Tables["Main"].Rows.Count == 0)
            {
                LogMessage("No tabs defined", 1);
                return;
            }
            string strActivityAction = "";
            string strImageUrl = "";
            string strActivityName = "";
            string strGeneral = "";
            if (TabInfo.Tables["Main"].Rows.Count < 15 && TabInfo.Tables["Main"].Rows.Count > 8)
                nTabBreakIndex = TabInfo.Tables["Main"].Rows.Count / 2;
            for (int j = 0; j < TabInfo.Tables["Main"].Rows.Count; j++)
            {
                strActivityName = TabInfo.Tables["Main"].Rows[j]["tab_name"].ToString() + TabInfo.Tables["Main"].Rows[j]["tab_count"].ToString();
                strActivityAction = TabInfo.Tables["Main"].Rows[j]["action_url"].ToString();
                if (string.IsNullOrEmpty(strActivityAction) || string.IsNullOrEmpty(strActivityName))
                    continue;
                if(TabInfo.Tables["Main"].Rows[j]["tab_show_image"].ToString() != "N")
                    strImageUrl = TabInfo.Tables["Main"].Rows[j]["image_path"].ToString();
                strGeneral = TabInfo.Tables["Main"].Rows[j]["general"].ToString();

                CreateTab(strActivityName, strImageUrl, strActivityAction, strGeneral, "System");
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
            return;
        }
    }
    #endregion

    void CreateTab(string name, string image, string url, string general, string value)
    {
        string newURL = url;
        newURL = newURL.Replace("#ID#", objectId);
        if (!string.IsNullOrEmpty(Request.QueryString["CLI"]) && name.ToLower()=="general")
        {
            string strurl = Request.Url.Query;

            if (newURL.Contains("?"))
            {
                strurl = strurl.Replace('?', '&');
              
            }
            newURL += strurl;

        }
        else if(general == "N")// Make object as source for General='N' tabs
        {
            if (newURL.Contains("?"))
                newURL += "&Source=" + objectType + "&SourceID=" + objectId;
            else
                newURL += "?Source=" + objectType + "&SourceID=" + objectId;
        }
        else if(sourceType.Length > 0)
        {
            if (newURL.Contains("?"))
                newURL += "&Source=" + sourceType + "&SourceID=" + sourceId;
            else
                newURL += "?Source=" + sourceType + "&SourceID=" + sourceId;
        }
      

        int index = rdTabCommon.Tabs.Count;
        RadTab rdItem = new RadTab();
        if (index == nTabBreakIndex)
            rdItem.IsBreak = true;

        rdItem.Text = name;
        rdItem.Value = value;
        rdItem.ImageUrl = image;
        RadPageView rpv = new RadPageView();
        if (value == "Custom")
        {
            Literal lt = new Literal();
            lt.Text = "<iframe id='ifcustom' frameborder=0 style='width:100%;height:600px;' scrolling='none' src=" + newURL + "></iframe>";
            rpv.Controls.Add(lt);
        }
        else
        {
            rpv.ContentUrl = newURL;
        }
        rpv.Height = Unit.Pixel(600);
        rdItem.PageViewID = rpv.UniqueID;
        rdTabCommon.Tabs.Add(rdItem);
        rdMPCommon.PageViews.Insert(index, rpv);
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
