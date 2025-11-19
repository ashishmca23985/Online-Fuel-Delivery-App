using System;
using System.Web.UI;
using System.Data;
using Telerik.Web.UI;
using System.Web.UI.WebControls;
using IDGLicenseLib;
using System.Web;

public partial class HomePage : ThemeBase
{
    string strHostID = string.Empty;
    GlobalWS objGlobal = new GlobalWS();
    public string configcalltype = "I";
    public string lead_id = "";
    public string serviceid = "";
    public string callid = "";
    public string agentid = "";
    public string agentidpassword = "";
    public string terminal = "";
    public string calltype = "";
    public string callStartTime = "";
    public string campaign = "";
    public string session_id = "";
    public string phone = "";
    public string RecordNumber = "";
    public string recording_filename = "";
    public string recording_id = "";
    public string call_id = "";
    public string session_name = "";


    #region Page Load Event
    protected void Page_Load(object sender, EventArgs e)
    {
        configcalltype = System.Configuration.ConfigurationManager.AppSettings["CALLTYPE"].ToString();
        if (!Page.IsPostBack)
        {
            DataSet ds = objFrameworkWS.GetHomePageInformation(TB_nUserRoleID.ToString());
            if (Convert.ToInt32(ds.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(ds.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
                return;
            }
            if (ds.Tables.Contains("HeaderInfo"))
                HeaderInfo(ds.Tables["HeaderInfo"]);    /*** used to display home page header info i.e logo,login user ***/
            if (ds.Tables.Contains("MenuInfo"))
                BindMenu(ds);         /*** used to populate menu ***/
            if (ds.Tables.Contains("ToolBarInfo"))
                BindToolbar(ds.Tables["ToolBarInfo"]);

            if (ds.Tables.Contains("PanelInfo"))
                BindPanel(ds.Tables["PanelInfo"]);

            // Elision Dialer 

            //if (Request.QueryString["user"] != null)
            //    agentid = Request.QueryString["user"];
            //    lblAgentId.Text = agentid.ToString();

            //if (Request.QueryString["lead_id"] != null)
            //    lead_id = Request.QueryString["lead_id"];
            //    lblLeadId.Text = lead_id.ToString();

            //if (Request.QueryString["campaign"] != null)
            //    campaign = Request.QueryString["campaign"];
            //    lblCampaign.Text = campaign.ToString();

            //if (Request.QueryString["session_id"] != null)
            //    session_id = Request.QueryString["session_id"];

            //if (Request.QueryString["phone"] != null)
            //    phone = Request.QueryString["phone"];
            //    lblCLI.Text = phone.ToString();

            //if (Request.QueryString["recording_filename"] != null)
            //    recording_filename = Request.QueryString["recording_filename"];

            //if (Request.QueryString["recording_id"] != null)
            //    recording_id = Request.QueryString["recording_id"];

            //if (Request.QueryString["call_id"] != null)
            //    call_id = Request.QueryString["call_id"];

            //if (Request.QueryString["session_name"] != null)
            //    session_name = Request.QueryString["session_name"];

            //if (Request.QueryString["SIPexten"] != null)
            //    terminal = Request.QueryString["SIPexten"];
            //   lblTerminal.Text = terminal.ToString();


            if (Session["LeadId"].ToString() != null)
                lead_id = Session["LeadId"].ToString();
            lblLeadId.Text = lead_id.ToString();

            if (Session["CTIagentid"].ToString() != null)
                agentid = Session["CTIagentid"].ToString();
            lblAgentId.Text = agentid.ToString();

            if (Session["CTIPassword"].ToString() != null)
                agentidpassword = Session["CTIPassword"].ToString();

            if (Session["Campaign"].ToString() != null)
                campaign = Session["Campaign"].ToString();
            lblCampaign.Text = campaign.ToString();

            if (Session["SessionId"].ToString() != null)
                session_id = Session["SessionId"].ToString();

            if (Session["Phone"].ToString() != null)
                phone = Session["Phone"].ToString();
            lblCLI.Text = phone.ToString();

            if (Session["RecFile"].ToString() != null)
                recording_filename = Session["RecFile"].ToString();

            if (Session["CallId"].ToString() != null)
                call_id = Session["CallId"].ToString();

            if (Session["SessionName"].ToString() != null)
                session_name = Session["SessionName"].ToString();

            if (Session["Terminal"].ToString() != null)
                terminal = Session["Terminal"].ToString();
            lblTerminal.Text = terminal.ToString();

        }
    }
    #endregion

    void CreateTab(string name, string image, string url, string general, string value)
    {
        RadTab rdItem = new RadTab();

        rdItem.Text = name;
        rdItem.Value = value;
        rdItem.ImageUrl = image;
        RadPageView rpv = new RadPageView();
        rpv.ContentUrl = url;
        rpv.Height = Unit.Pixel(700);
        rdItem.PageViewID = rpv.UniqueID;
        rdMainTabStrip.Tabs.Add(rdItem);
        rdMainPageView.PageViews.Add(rpv);
    }

    #region Get Header Info
    private void HeaderInfo(DataTable TableInfo)
    {
        try
        {
            string ImageSize;
            string ImageHeight;
            string ImageWidth;
            int SepratorPosition;

            if (!String.IsNullOrEmpty(TableInfo.Rows[0]["header_logo"].ToString()))
            {
                imgLogo.Src = TableInfo.Rows[0]["header_logo"].ToString();

                if (!String.IsNullOrEmpty(TableInfo.Rows[0]["header_logo_size"].ToString()))
                {
                    ImageSize = TableInfo.Rows[0]["header_logo_size"].ToString();
                    SepratorPosition = ImageSize.IndexOf(",");
                    ImageHeight = ImageSize.Substring(0, SepratorPosition);
                    ImageWidth = ImageSize.Substring(SepratorPosition + 1, ImageSize.Length - (SepratorPosition + 1));
                    if (Convert.ToInt32(ImageHeight) != 0)
                    {
                        imgLogo.Height = Convert.ToInt32(ImageHeight);
                    }
                    if (Convert.ToInt32(ImageWidth) != 0)
                    {
                        imgLogo.Width = Convert.ToInt32(ImageWidth);
                    }
                }
            }
            else
            {
                imgLogo.Visible = false;
            }

            //btnLogout.InnerText = TableInfo.Rows[0]["header_logout_text"].ToString();
            lblUserInfo.Text = TableInfo.Rows[0]["header_welcome_message"].ToString() + " " + System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(TB_strEmployeeName);

            //--show login time as per local zone time                        
            lblUserLoginDate.Text = "Logged in at:- " + DateTime.UtcNow.Add(new TimeSpan(0, TB_nTimeZoneTimeSpan, 0)).ToString("dd MMM yyyy HH:mm:ss");

            //if (!String.IsNullOrEmpty(TableInfo.Rows[0]["customization_image"].ToString()))
            //{
            //    //imgCustom.Src = TableInfo.Rows[0]["customization_image"].ToString();
            //    ImageSize = string.Empty;
            //    ImageHeight = string.Empty;
            //    ImageWidth = string.Empty;
            //    SepratorPosition = 0;

            //    if (!String.IsNullOrEmpty(TableInfo.Rows[0]["customization_image_size"].ToString()))
            //    {
            //        ImageSize = TableInfo.Rows[0]["customization_image_size"].ToString();
            //        SepratorPosition = ImageSize.IndexOf(",");
            //        ImageHeight = ImageSize.Substring(0, SepratorPosition);
            //        ImageWidth = ImageSize.Substring(SepratorPosition + 1, ImageSize.Length - (SepratorPosition + 1));
            //        if (Convert.ToInt32(ImageHeight) != 0)
            //        {
            //            imgCustom.Height = Convert.ToInt32(ImageHeight);
            //        }
            //        if (Convert.ToInt32(ImageWidth) != 0)
            //        {
            //            imgCustom.Width = Convert.ToInt32(ImageWidth);
            //        }
            //    }
            //}
            //else
            //{
            //    imgCustom.Visible = false;
            //}

            if (TB_strShowAlertbar == "N")
                trAlerts.Style.Add("display", "none");
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
            return;
        }
    }
    #endregion

    #region Bind Menu
    private void BindMenu(DataSet ds)
    {
        DataTable TableInfo = ds.Tables["MenuInfo"];
        string strResult = string.Empty;
        try
        {
            foreach (DataRow row in TableInfo.Rows)
            {
                if (String.IsNullOrEmpty(row["activity_id"].ToString()) || String.IsNullOrEmpty(row["activity_name"].ToString()))
                    continue;
                RadMenuItem rmi = new RadMenuItem(row["activity_name"].ToString());
                int nActivityId = Convert.ToInt32(row["activity_id"]);
                string strActivityAction = row["activity_action_url"].ToString();
                string strContainer = row["activity_action_container"].ToString();

                if (!String.IsNullOrEmpty(strActivityAction))
                    rmi.Value = strActivityAction;

                DataTable InnerTableInfo = objFrameworkWS.GetSubMenuList(nActivityId, TB_nUserRoleID.ToString());
                if (Convert.ToInt32(InnerTableInfo.Rows[0]["ResultCode"]) == 0 && InnerTableInfo.Rows.Count > 0)
                {
                    foreach (DataRow submenurow in InnerTableInfo.Rows)
                    {
                        RadMenuItem rsmi = new RadMenuItem(submenurow["activity_name"].ToString());
                        nActivityId = Convert.ToInt32(submenurow["activity_id"]);
                        strActivityAction = submenurow["activity_action_url"].ToString();
                        strContainer = submenurow["activity_action_container"].ToString();
                        if (!String.IsNullOrEmpty(strActivityAction))
                            rsmi.Value = strActivityAction;

                        rmi.Items.Add(rsmi);
                    }
                }
                //if (row["activity_name"].ToString() == "Options")
                //{
                //    if (ds.Tables.Contains("ThemeInfo"))
                //        Customization(ds.Tables["ThemeInfo"], ref rmi);
                //}
                rdMenu.Items.Add(rmi);
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region Bind Panel
    private void BindPanel(DataTable TableInfo)
    {
        string strResult = string.Empty;
        try
        {
            for (int j = 0; j < TableInfo.Rows.Count; j++)
            {
                string strActivityId = TableInfo.Rows[j]["activity_id"].ToString();
                if (strActivityId == "2909")
                {
                   // slpCtiPanel.Visible = true;
                    // ifCtiAgent.Attributes.Add("src", "cti/login.aspx");
                }
                //else if (strActivityId == "2409")
                //{
                //    slpQuickCase.Visible = true;
                //}
                //else if (strActivityId == "2410")
                //{
                //    slpQuickTask.Visible = true;
                //}
                //else if (strActivityId == "2215")
                //{
                //    slpQueue.Visible = true;
                //}
                else if (strActivityId == "2418")
                {
                    test.Attributes.Add("onload", "javascript:GetNewChatRequset();");
                    //slpChat.Visible = true;
                    objFrameworkWS.ChangeContactStatus(TB_nContactID, "Available");
                }
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
            return;
        }
    }
    #endregion

    #region Bind Toolbar
    private void BindToolbar(DataTable TableInfo)
    {
        string strResult = string.Empty;
        try
        {
            for (int j = 0; j < TableInfo.Rows.Count; j++)
            {
                string strActivityId = TableInfo.Rows[j]["activity_id"].ToString();
                if (string.IsNullOrEmpty(strActivityId))
                    continue;
                string strActivityAction = TableInfo.Rows[j]["activity_action_url"].ToString();
                if (string.IsNullOrEmpty(strActivityAction))
                    continue;
                string strContainer = TableInfo.Rows[j]["activity_action_container"].ToString();
                if (string.IsNullOrEmpty(TableInfo.Rows[j]["image_path"].ToString()) && string.IsNullOrEmpty(TableInfo.Rows[j]["activity_name"].ToString()))
                    continue;
                RadToolBarButton rdItem = new RadToolBarButton();
                RadToolTip tooltip = new RadToolTip();
                rdItem.Value = strActivityId;
                if (!string.IsNullOrEmpty(TableInfo.Rows[j]["image_path"].ToString()))
                    rdItem.ImageUrl = TableInfo.Rows[j]["image_path"].ToString();
                if (!string.IsNullOrEmpty(TableInfo.Rows[j]["activity_name"].ToString()))
                    rdItem.Text = TableInfo.Rows[j]["activity_name"].ToString();

                //if (strActivityAction.Contains("?"))
                //{
                //    strActivityAction = strActivityAction + "&ActivityId=" + strActivityId;
                //}
                //else
                //{
                //    strActivityAction = strActivityAction + "?ActivityId=" + strActivityId;
                //}

                rdItem.CheckOnClick = true;
                rdItem.AllowSelfUnCheck = true;
                rdItem.Value = strActivityAction;

                rdItem.ToolTip = TableInfo.Rows[j]["activity_description"].ToString();

                toolbarMain.Items.Add(rdItem);
            }
            if (toolbarMain.Items.Count <= 0 || TB_strShowToolbar == "N")
            {
                trToolbarMain.Style.Add("display", "none");
                return;
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
            return;
        }
    }
    #endregion

    #region Customization
    private void Customization(DataTable TableInfo, ref RadMenuItem MainItem)
    {
        string strResult = string.Empty;
        try
        {
            RadMenuItem itmParentTheme = new RadMenuItem();
            itmParentTheme.Text = "Theme";
            itmParentTheme.Value = "Theme";
            for (int i = 0; i < TableInfo.Rows.Count; i++)
            {
                RadMenuItem itemChildItem = new RadMenuItem();
                itemChildItem.Text = TableInfo.Rows[i]["theme_name"].ToString();
                itemChildItem.Value = TableInfo.Rows[i]["theme_id"].ToString();
                itmParentTheme.Items.Add(itemChildItem);
            }

            RadMenuItem itemChildThemeApply = new RadMenuItem();
            itemChildThemeApply.Text = "Apply";
            itemChildThemeApply.Value = "Apply";
            itemChildThemeApply.BackColor = System.Drawing.Color.Gainsboro;
            itmParentTheme.Items.Add(itemChildThemeApply);

            if (TB_strTheme != null && TB_strTheme != string.Empty)
            {
                itmParentTheme.Items.FindItemByText(TB_strTheme).ImageUrl = "~/CommonImages/checked.gif";
            }
            MainItem.Items.Add(itmParentTheme);

            RadMenuItem itmShowToolbar = new RadMenuItem();
            if (TB_strShowToolbar == "Y")
                itmShowToolbar.Text = "Hide Toolbar";
            else
                itmShowToolbar.Text = "Show Toolbar";
            itmShowToolbar.Value = "Toolbar";
            MainItem.Items.Add(itmShowToolbar);

            //RadMenuItem itmShowAlerts = new RadMenuItem();
            //if (TB_strShowAlertbar == "Y")
            //    itmShowAlerts.Text = "Hide Alerts";
            //else
            //    itmShowAlerts.Text = "Show Alerts";
            //itmShowAlerts.Value = "Alerts";
            //MainItem.Items.Add(itmShowAlerts);

            //RadMenuItem itmBlockPopup = new RadMenuItem();
            //if (TB_strShowPopup == "Y")
            //    itmBlockPopup.Text = "Hide Popup";
            //else
            //    itmBlockPopup.Text = "Show Popup";

            //itmBlockPopup.Value = "BlockPopup";
            //MainItem.Items.Add(itmBlockPopup);
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region Apply selected theme
    protected void lnkApplyTheme_Click(object sender, EventArgs e)
    {
        try
        {
            /*** apply theme after re-load home page again  ***/
            Session["Theme"] = hdnApplyTheme.Value;
            TB_strTheme = hdnApplyTheme.Value;
            objFrameworkWS.SetUserTheme(TB_nContactID, TB_strTheme);
            //Customization();         
            Response.Redirect("HomePage.aspx");
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
            return;
        }
    }
    #endregion

    // added by ashish 

    protected void tmrupdate_Tick(object sender, EventArgs e)
    {

        //GlobalWS objcheck = new GlobalWS();
        //if (Convert.ToString(Session["nsessionid"]) != string.Empty)
        //{
        //    objcheck.SetHealthCheck(Convert.ToInt32(Session["nsessionid"].ToString()));
        //    IDGLicenseLib.ClientClass lic = new ClientClass();
        //    lic.HeartBeat(Convert.ToInt32(Session["nsessionid"].ToString()));
        //}
        //else
        //    Response.Redirect("login.aspx");

    }

    #region Log Message
    void LogMessage(string message, Int32 param)
    {

        /*** used to show the response as a label text ***/
        lblMessage.Text = message;
        if (param == 1)
        {
            lblMessage.Visible = true;
            lblMessage.ForeColor = System.Drawing.Color.Red;
        }
        else if (param == 3)
        {
            lblMessage.Visible = true;
            lblMessage.ForeColor = System.Drawing.Color.Blue;
        }
        else if (param == 100)
        {
            lblMessage.Visible = false;
            lblMessage.ForeColor = System.Drawing.Color.Green;
        }
        else
        {
            lblMessage.Visible = true;
            lblMessage.ForeColor = System.Drawing.Color.Green;
        }
    }
    #endregion

    [System.Web.Services.WebMethod()]
    public static string Logout()
    {
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        CtiWS1.Logout("", "", HttpContext.Current.Session["HOSTID"].ToString());
        if (HttpContext.Current.Session["ctilogin"] != null && HttpContext.Current.Session["ctilogin"].ToString() =="Y" )
        {
            return "Success";
        }
        else
            return "fail";
   }
    [System.Web.Services.WebMethod()]
    public static string ForCurrentLoginStatus()
    {
        GlobalWS objGlobal = new GlobalWS();
        string User = HttpContext.Current.Session["login_id"].ToString();
        string Passwod = HttpContext.Current.Session["login_pwd"].ToString();
        int i = objGlobal.RemoveUserSession(User);
        if (i > 0)
        {
            User = null;
            Passwod = null;
        }
        return "Success";
    }
}
