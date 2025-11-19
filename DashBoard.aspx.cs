using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using Telerik.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
[Serializable]
public partial class DashBoard : ThemeBase
{
    int levelid = 0;
    private bool _dockStateCleared = false;
    private List<DockState> CurrentDockStates
    {
        get
        {
            //Store the info about the added docks in the session. For real life
            // applications we recommend using database or other storage medium 
            // for persisting this information.
            List<DockState> _currentDockStates = (List<DockState>)Session["CurrentDockStates"];
            if (Object.Equals(_currentDockStates, null))
            {
                DashBoardWS objDashBoard = new DashBoardWS();
                DataSet objDataSet = objDashBoard.GetUserDashboard(TB_nContactID);
                if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) == -1)
                {
                    LogMessage(objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
                }
                else if (objDataSet.Tables["DockState"].Rows.Count > 0)
                {
                    string strDockState = objDataSet.Tables["DockState"].Rows[0]["dockstate"].ToString();
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    _currentDockStates = serializer.Deserialize<List<DockState>>(strDockState);
                }

                // check again for null.
                if (Object.Equals(_currentDockStates, null))
                    _currentDockStates = new List<DockState>();
                // store in session
                Session["CurrentDockStates"] = _currentDockStates;
            }
            return _currentDockStates;
        }
        set
        {
            Session["CurrentDockStates"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string strtheme = theme;
        //theme = "blueopal";
        // theme = "flat";
        //HtmlLink cssMasterLink = new HtmlLink();
        //cssMasterLink.Href = "~/css/datastyles/kendo.dataviz." + theme + ".min.css";
        //cssMasterLink.Attributes.Add("rel", "stylesheet");
        //cssMasterLink.Attributes.Add("type", "text/css");
        //Header.Controls.Add(cssMasterLink);
        //HtmlLink cssMasterLink2 = new HtmlLink();
        //cssMasterLink2.Href = "~/css/datastyles/kendo." + theme + ".min.css";
        //cssMasterLink2.Attributes.Add("rel", "stylesheet");
        //cssMasterLink2.Attributes.Add("type", "text/css");
        //Header.Controls.Add(cssMasterLink2);
        //hdntheme.Value = theme;
        LogMessage("Updated at " + DateTime.UtcNow.Add(new TimeSpan(0, TB_nTimeZoneTimeSpan, 0)).ToShortTimeString(), 0);
        if (!IsPostBack)
        {

            string strDocks = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DASHLETZONES"]);
            if (strDocks == "1")
            {
                tdZone1.Style.Add("width", "99%");
                tdZone2.Style.Add("width", "0%");
                tdZone2.Style.Add("display", "none");
                tdZone3.Style.Add("width", "0%");
                tdZone3.Style.Add("display", "none");
            }
            else if (strDocks == "2")
            {
                tdZone1.Style.Add("width", "49.5%");
                tdZone2.Style.Add("width", "49.5%");
                tdZone3.Style.Add("width", "0%");
                tdZone3.Style.Add("display", "none");
            }
            else // default is 3
            {
                tdZone1.Style.Add("width", "33%");
                tdZone2.Style.Add("width", "33%");
                tdZone3.Style.Add("width", "33%");
            }
            GetDashlets();
            foreach (DockState state in CurrentDockStates)
            {
                if (state.Closed == false)
                {
                    RadDock dock = CreateRadDockFromState(state);
                    RadComboBoxItem li = cmbDashlets.Items.FindItemByText(dock.Title);
                    if (li != null)
                    {
                        cmbDashlets.Items.Remove(li);
                    }
                }
            }
            ////RadScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", hdndock.Value, true);
        }
        hdndock.Value = "";
       // //RadScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", @"javascript: alert('" + hdndock.Value + "');", true);
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        lblMessage.Text = "";
       
        //Recreate the docks in order to ensure their proper operation
        foreach (DockState state in CurrentDockStates)
        {
            if (state.Closed == false)
            {
                RadDock dock = CreateRadDockFromState(state);
                //We will just add the RadDock control to the RadDockLayout.
                // You could use any other control for that purpose, just ensure
                // that it is inside the RadDockLayout control.
                // The RadDockLayout control will automatically move the RadDock
                // controls to their corresponding zone in the LoadDockLayout
                // event (see below).
                RadDockLayout1.Controls.Add(dock);
                //We want to save the dock state every time a dock is moved.
                CreateSaveStateTrigger(dock);
                //Load the selected widget
                LoadWidget(dock);
                RadComboBoxItem li = cmbDashlets.Items.FindItemByText(dock.Title);
                if (li != null)
                {
                    cmbDashlets.Items.Remove(li);
                }
            }
        }
    }
    //protected void Page_PreRender(object sender, EventArgs e)
    //{
    //   Page.ClientScript.RegisterStartupScript(Page.GetType(), "SaveSuccess", @"javascript:docksplit('" + hdndock.Value + "');", true);
    //}
    protected void RadDockLayout1_LoadDockLayout(object sender, DockLayoutEventArgs e)
    {
        //Populate the event args with the state information. The RadDockLayout control
        // will automatically move the docks according that information.

        foreach (DockState state in CurrentDockStates)
        {
            e.Positions[state.UniqueName] = state.DockZoneID;
            e.Indices[state.UniqueName] = state.Index;
        }
      //  //RadScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", hdndock.Value, true);
    }

    protected void RadDockLayout1_SaveDockLayout(object sender, DockLayoutEventArgs e)
    {
        if (!_dockStateCleared)
        {
            //Save the dock state in the session. This will enable us 
            // to recreate the dock in the next Page_Init.
            CurrentDockStates = RadDockLayout1.GetRegisteredDocksState();
        }
        else
        {
            //the clear state button was clicked, so we refresh the page and start over.
            Response.Redirect(Request.RawUrl, false);
        }
     //   //RadScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", hdndock.Value, true);
    }

    private RadDock CreateRadDockFromState(DockState state)
    {
        RadDock dock = new RadDock();
        dock.DockMode = DockMode.Docked;
        dock.EnableRoundedCorners = true;
        dock.Resizable = true;
        dock.ID = string.Format("RadDock{0}", state.UniqueName);
        dock.ApplyState(state);
        dock.Commands.Add(new DockCloseCommand());
        dock.Commands.Add(new DockExpandCollapseCommand());

        return dock;
    }

    private void CreateSaveStateTrigger(RadDock dock)
    {
        //Ensure that the RadDock control will initiate postback
        // when its position changes on the client or any of the commands is clicked.
        //Using the trigger we will "ajaxify" that postback.
        dock.AutoPostBack = true;
        dock.CommandsAutoPostBack = true;

        AsyncPostBackTrigger saveStateTrigger = new AsyncPostBackTrigger();
        saveStateTrigger.ControlID = dock.ID;
        saveStateTrigger.EventName = "DockPositionChanged";
        UpdatePanel1.Triggers.Add(saveStateTrigger);

        saveStateTrigger = new AsyncPostBackTrigger();
        saveStateTrigger.ControlID = dock.ID;
        saveStateTrigger.EventName = "Command";
        UpdatePanel1.Triggers.Add(saveStateTrigger);

    }

    private void LoadWidget(RadDock dock)
    {
        try
        {


            if (string.IsNullOrEmpty(dock.Tag))
            {
                return;
            }
            if (dock.Text.Contains("ascx"))
            {
                Control widget = (Control)LoadControl(dock.Text);
                dock.ContentContainer.Controls.Add(widget);
            }
            else
            {
                if (dock.Text.ToLower() == "graph")
                {
                 //   levelid++;
                 //   Literal widget = new Literal();
                  //  widget.Text = @"<div id='example" + levelid.ToString() + "' class='k-content' style='height:290px;' ><div id='chart" + levelid.ToString() + "'  class='small-chart'>hgfghfghfgh hgfg </div></div>";
                        //<script>javascript:javascript:createChart(" + dock.Tag
                        //+ ", 'chart" + levelid.ToString() + "', '" + theme + "', 'example" + levelid.ToString() + "');</script>";
                   // hdndock.Value += levelid.ToString() + ",";
                    string url = "~/UserControls/Graph.ascx";
                    Graph widget = (Graph)LoadControl(url);
                    widget.DockTag = dock.Tag;
                    dock.ContentContainer.Controls.Add(widget);
                }
                else
                {
                    string url = "~/UserControls/ShowDashlet.ascx";
                    GalaxyDashlet widget = (GalaxyDashlet)LoadControl(url);
                    widget.DockTag = dock.Tag;
                    dock.ContentContainer.Controls.Add(widget);
                }

            }
        }
        catch (Exception ex)
        {

            lblMessage.LogMessage(ex, 2);
        }
    }

    protected void ButtonAddDock_Click(object sender, EventArgs e)
    {
        try
        {


            if (cmbDashlets.SelectedIndex < 0 || cmbDashlets.SelectedValue == "")
                return;

            RadDock dock = new RadDock();
            dock.DockMode = DockMode.Docked;
            dock.UniqueName = Guid.NewGuid().ToString().Replace("-", "a");
            dock.ID = string.Format("RadDock{0}", dock.UniqueName);
            dock.Title = cmbDashlets.SelectedItem.Text;
            string[] strDashlet = cmbDashlets.SelectedValue.Split('|');
            dock.Text = strDashlet[0];
            dock.Tag = strDashlet[1];

            dock.Commands.Add(new DockCloseCommand());
            dock.Commands.Add(new DockExpandCollapseCommand());

            //find the target zone and add the new dock there
            RadDockZone dz = RadDockZone1;
            dz.Controls.Add(dock);
            CreateSaveStateTrigger(dock);

            //Load the selected widget in the RadDock control
            LoadWidget(dock);
            cmbDashlets.Items.Remove(cmbDashlets.SelectedIndex);
            //  SaveDashlet();
            LogMessage("Dashboard added successfully. Use 'Save Dashboard' to save the new layout", 0);
            
        }
        catch (Exception ex)
        {


        }
        //RadScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess",hdndock.Value, true);
    }

    protected void ButtonSaveDock_Click(object sender, EventArgs e)
    {
        SaveDashlet();
        LogMessage("Dashboard saved successfully", 0);
        GetDashlets();
        //RadScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", hdndock.Value, true);
    }

    private void SaveDashlet()
    {
        try
        {
            string strDashlets = "0";
            foreach (DockState state in CurrentDockStates)
            {
                if (state.Closed == false)
                    strDashlets += "," + state.Tag;
            }

            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string strDockStates = serializer.Serialize(CurrentDockStates);
            DashBoardWS objDashBoard = new DashBoardWS();
            DataSet objDataSet = objDashBoard.SaveUserDashboard(strDockStates, strDashlets, TB_nContactID);
            if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) == -1)
            {
                LogMessage(objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
                return;
            }
        }
        catch (Exception EX)
        {
        }
    }

    protected void ButtonClear_Click(object sender, EventArgs e)
    {
        try
        {


            //clear docks state from the session
            CurrentDockStates.Clear();
            _dockStateCleared = true;

            LogMessage("Dashboard added successfully. Use 'Save Dashboard' to save the new layout", 0);
        }
        catch (Exception ex)
        {


        }
    }

    protected void imgServerEvent_Click(object sender, EventArgs e)
    {
        //RadScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", hdndock.Value, true);
    }

    #region GetDashlets
    private void GetDashlets()
    {
        try
        {
            DashBoardWS objDashBoard = new DashBoardWS();
            DataSet objDataSet = objDashBoard.FetchDashlets(TB_nContactID, TB_nUserRoleID);
            if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) == -1)
            {
                LogMessage(objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
            }
            cmbDashlets.DataSource = objDataSet.Tables[0];
            cmbDashlets.DataValueField = "dashlet_value";
            cmbDashlets.DataTextField = "dashlet_name";
            cmbDashlets.DataBind();

            RadComboBoxItem lstItem = new RadComboBoxItem("", "");
            cmbDashlets.Items.Insert(0, lstItem);
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

