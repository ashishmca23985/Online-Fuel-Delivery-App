using System;

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Text;

public partial class Schedule : ThemeBase
{
    #region Local Variable Declaration
    ScheduleWS objScheduleWS;
    GlobalWS objGlobalWS = new GlobalWS();
    DataTable dtSchd = new DataTable();
    DataSet dsSchd = new DataSet();
    string custID = "";
    string transId = string.Empty;
    int ConsultantID = 0;
    DateTime ConsultationDate = DateTime.UtcNow;
    string strBranchCode = string.Empty;
    DataTable dt;
    string timeSlot = "";  
    bool isVerifier = false;
    #endregion
  

    #region Page Load Event
    protected void Page_Load(object sender, EventArgs e)
    {       
        if (!Page.IsPostBack)
        {
            if (Request.QueryString["ConsultantID"] != null && Request.QueryString["ConsultantID"] != "")
            {
                ConsultantID = Convert.ToInt32(Request.QueryString["ConsultantID"]);
                string strQuery = "SELECT #GetContactAccountId(" + ConsultantID + ")";
                strBranchCode = objGlobalWS.ExecuteQuery(strQuery);
            }
            if (Request.QueryString["CollectionDate"] != null && Request.QueryString["CollectionDate"] != "")
            {
                ConsultationDate = Convert.ToDateTime(Request.QueryString["CollectionDate"]);
            }
            if (Request.QueryString["CaseID"] != null && Request.QueryString["CaseID"] != "")
            {
                string strQuery = "SELECT #GetCaseAccountId(" + Request.QueryString["CaseID"] + ")";
                custID = objGlobalWS.ExecuteQuery(strQuery);
            }

            Session["id"] = "";
            BindEmptyGrid();
            FillStates("AUS");
            //FillContactTypeValue();
            FillBranches("0");
            GetConsultantList(0);
            if (ConsultantID > 0)
            {
                //cmdContactType.SelectedValue = ContactType;
                //GetConsultantList();
                cmbBranch.SelectedValue = strBranchCode;  
                cmbSchedularConsultant.SelectedValue = ConsultantID.ToString();
                rdDate.SelectedDate = ConsultationDate;
                GetSchedules(ConsultationDate, ConsultantID);
            }
            ViewState["Request"] = "N";
        }
        
        
    }
    #endregion

    #region Bind Empty Grid
    void BindEmptyGrid()
    {
        dt = new DataTable();
        DataColumn dc = new DataColumn("Slab", Type.GetType("System.String"));
        dt.Columns.Add(dc);
        dt.Constraints.Add("pk_slab", dc, true);
        dt.Columns.Add(new DataColumn("First", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Second", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Third", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("Fourth", Type.GetType("System.String")));
        dt.Columns.Add(new DataColumn("ID", Type.GetType("System.String")));
        dt.Columns.Add("Next1", Type.GetType("System.String"));
        dt.Columns.Add("Next2", Type.GetType("System.String"));
        dt.Columns.Add("Next3", Type.GetType("System.String"));
        dt.Columns.Add("Next4", Type.GetType("System.String"));
        dt.Columns.Add("Enabled", Type.GetType("System.String"));
        try
        {
            //Select start,end from Sche where start between curr + 5 days
            for (int i = 1; i <= 24; i++)
            {
                DataRow dr = dt.NewRow();
                //dr[0] = i.ToString() + val + "-" + ((i + 1) >= 24 ? "1" : (i + 1).ToString()) + (((i + 1) >= 12 && (i + 1) <= 24) ? " PM" : " AM");
                dr[0] = (i - 1).ToString("00") + "-" + i.ToString("00");
                dr[1] = "";
                dr[2] = "";
                dr[3] = "";
                dr[4] = "";
                dr[5] = "";
                //dr[6] = "";
                //dr[7] = "";
                //dr[8] = "";
                //dr[9] = "";
                //dr[10] = "";
                dt.Rows.Add(dr);

             
                //date + time
            }
            //rgdSchedule.DataSource = dt;
            //rgdSchedule.DataBind();
        }
        catch (Exception)
        {
        }
    }
    #endregion

    #region DataGrid Item Databound Event
    protected void rgdSchedule_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        try
        {
            if (e.Item.ItemType == Telerik.Web.UI.GridItemType.Header)
            {

                DateTime time = rdDate.SelectedDate.Value;
                time = time.AddDays(1);
                e.Item.Cells[8].Text = time.ToString("dd MMM yy") + "(" + time.ToString("ddd") + ")";
                time = time.AddDays(1);
                e.Item.Cells[9].Text = time.ToString("dd MMM yy") + "(" + time.ToString("ddd") + ")";
                time = time.AddDays(1);
                e.Item.Cells[10].Text = time.ToString("dd MMM yy") + "(" + time.ToString("ddd") + ")";
                time = time.AddDays(1);
                e.Item.Cells[11].Text = time.ToString("dd MMM yy") + "(" + time.ToString("ddd") + ")";
            }

            if (e.Item.ItemType == Telerik.Web.UI.GridItemType.Item || e.Item.ItemType == Telerik.Web.UI.GridItemType.AlternatingItem)
            {
                System.Drawing.Color totalFree = System.Drawing.Color.PaleTurquoise;
                System.Drawing.Color partialFill = System.Drawing.Color.LightPink;//OldLace;
                System.Drawing.Color totalBusy = System.Drawing.Color.LightGray;
                System.Drawing.Color leave = System.Drawing.Color.DarkBlue;
                System.Drawing.Color bookedWithLeave = System.Drawing.Color.Red;

                int time = DateTime.Now.Hour;
                int day = DateTime.Now.Day;
                int month = DateTime.Now.Month;
                int selectedMonth = month;
                int selectedDay = day;
                if (rdDate.SelectedDate != null)
                {
                    selectedDay = rdDate.SelectedDate.Value.Day;
                    selectedMonth = rdDate.SelectedDate.Value.Month;
                }

                #region set color for current day schedule
                string lblSlab = ((Label)e.Item.Cells[0].FindControl("lblSlab")).Text;
                lblSlab = lblSlab.Substring(lblSlab.IndexOf("-") + 1);
                string isLeave = ((Label)e.Item.Cells[0].FindControl("lblLeave")).Text;

                if ((Convert.ToInt32(lblSlab) <= time && selectedDay <= day && selectedMonth <= month) || isLeave == "Y")
                {
                    ((CheckBox)e.Item.Cells[1].FindControl("chkCode")).Enabled = false;
                    ((CheckBox)e.Item.Cells[2].FindControl("chkCode2")).Enabled = false;
                    ((CheckBox)e.Item.Cells[3].FindControl("chkCode3")).Enabled = false;
                    ((CheckBox)e.Item.Cells[4].FindControl("chkCode4")).Enabled = false;
                    if (isLeave == "Y")
                    {
                        //if(((CheckBox)e.Item.Cells[1].FindControl("chkCode")).Text.Replace("&nbsp;","").Length>0)
                        //    e.Item.Cells[3].BackColor = bookedWithLeave;
                        //else
                            e.Item.Cells[3].BackColor = leave;
                        e.Item.Cells[4].BackColor = leave;
                        e.Item.Cells[5].BackColor = leave;
                        e.Item.Cells[6].BackColor = leave;
                    }
                }
                else if ((selectedDay < day && selectedMonth <= month) || isLeave == "Y")
                {
                    ((CheckBox)e.Item.Cells[1].FindControl("chkCode")).Enabled = false;
                    ((CheckBox)e.Item.Cells[2].FindControl("chkCode2")).Enabled = false;
                    ((CheckBox)e.Item.Cells[3].FindControl("chkCode3")).Enabled = false;
                    ((CheckBox)e.Item.Cells[4].FindControl("chkCode4")).Enabled = false;
                    if (isLeave == "Y")
                    {
                        //if (((CheckBox)e.Item.Cells[1].FindControl("chkCode")).Text.Replace("&nbsp;", "").Length > 0)
                        //    e.Item.Cells[3].BackColor = bookedWithLeave;
                        //else
                        e.Item.Cells[3].BackColor = leave;
                        e.Item.Cells[4].BackColor = leave;
                        e.Item.Cells[5].BackColor = leave;
                        e.Item.Cells[6].BackColor = leave;
                    }
                }
                else if ((selectedMonth > month) || isLeave == "Y")
                {
                    ((CheckBox)e.Item.Cells[1].FindControl("chkCode")).Enabled = true;
                    ((CheckBox)e.Item.Cells[2].FindControl("chkCode2")).Enabled = true;
                    ((CheckBox)e.Item.Cells[3].FindControl("chkCode3")).Enabled = true;
                    ((CheckBox)e.Item.Cells[4].FindControl("chkCode4")).Enabled = true;
                    if (isLeave == "Y")
                    {
                        //if (((CheckBox)e.Item.Cells[1].FindControl("chkCode")).Text.Replace("&nbsp;", "").Length > 0)
                        //    e.Item.Cells[3].BackColor = bookedWithLeave;
                        //else
                        e.Item.Cells[3].BackColor = leave;
                        e.Item.Cells[4].BackColor = leave;
                        e.Item.Cells[5].BackColor = leave;
                        e.Item.Cells[6].BackColor = leave;
                    }
                }
                if (!isVerifier)
                {
                    //((CheckBox)e.Item.Cells[3].FindControl("chkCode2")).Enabled = false;
                   // ((CheckBox)e.Item.Cells[4].FindControl("chkCode4")).Enabled = false;
                }
                if (((CheckBox)e.Item.Cells[1].FindControl("chkCode")).Text.Replace("&nbsp;", "").Length > 0)
                {
                    ((CheckBox)e.Item.Cells[1].FindControl("chkCode")).Checked = true;
                    ((CheckBox)e.Item.Cells[1].FindControl("chkCode")).Enabled = false;
                }
                if (((CheckBox)e.Item.Cells[2].FindControl("chkCode2")).Text.Replace("&nbsp;", "").Length > 0)
                {
                    ((CheckBox)e.Item.Cells[2].FindControl("chkCode2")).Checked = true;
                    ((CheckBox)e.Item.Cells[2].FindControl("chkCode2")).Enabled = false;
                }
                if (((CheckBox)e.Item.Cells[3].FindControl("chkCode3")).Text.Replace("&nbsp;", "").Length > 0)
                {
                    ((CheckBox)e.Item.Cells[3].FindControl("chkCode3")).Checked = true;
                    ((CheckBox)e.Item.Cells[3].FindControl("chkCode3")).Enabled = false;
                }
                if (((CheckBox)e.Item.Cells[4].FindControl("chkCode4")).Text.Replace("&nbsp;", "").Length > 0)
                {
                    ((CheckBox)e.Item.Cells[4].FindControl("chkCode4")).Checked = true;
                    ((CheckBox)e.Item.Cells[4].FindControl("chkCode4")).Enabled = false;
                }
                string next1 = ((Label)e.Item.Cells[8].FindControl("lblNext1")).Text.Replace("&nbsp;", "");
                string next2 = ((Label)e.Item.Cells[8].FindControl("lblNext2")).Text.Replace("&nbsp;", "");
                string next3 = ((Label)e.Item.Cells[8].FindControl("lblNext3")).Text.Replace("&nbsp;", "");
                string next4 = ((Label)e.Item.Cells[8].FindControl("lblNext4")).Text.Replace("&nbsp;", "");
#endregion

                #region set color for next 4 days schedule
                ////
                if (next1.Contains("Leave0"))
                {
                    e.Item.Cells[8].BackColor = leave;
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext1")).Visible = false;
                }
                else if (next1.Contains("Leave"))
                {
                    e.Item.Cells[8].BackColor = bookedWithLeave;
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext1")).Text = ((LinkButton)e.Item.Cells[9].FindControl("lnkNext1")).Text.Replace("Leave", " ");
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext1")).Visible = true;
                }
                else if (next1 == "4")
                {
                    e.Item.Cells[8].BackColor = totalBusy;
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext1")).Visible = false;
                }
                else if (Convert.ToInt32(next1) >= 1)
                {
                    e.Item.Cells[8].BackColor = partialFill;
                }
                else
                {
                    e.Item.Cells[8].BackColor = totalFree;
                }
                ///
                if (next2.Contains("Leave0"))
                {
                    e.Item.Cells[9].BackColor = leave;
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext2")).Visible = false;
                }
                else if (next2.Contains("Leave"))
                {
                    e.Item.Cells[9].BackColor = bookedWithLeave;
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext2")).Text = ((LinkButton)e.Item.Cells[9].FindControl("lnkNext2")).Text.Replace("Leave", " ");
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext2")).Visible = true;
                }
                else if (next2 == "4")
                {
                    e.Item.Cells[9].BackColor = totalBusy;
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext2")).Visible = false;
                }
                else if (Convert.ToInt32(next2) >= 1)
                {
                    e.Item.Cells[9].BackColor = partialFill;
                }
                else
                {
                    e.Item.Cells[9].BackColor = totalFree;
                }
                ////
                if (next3.Contains("Leave0"))
                {
                    e.Item.Cells[10].BackColor = leave;
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext3")).Visible = false;

                }
                else if (next3.Contains("Leave"))
                {
                    e.Item.Cells[10].BackColor = bookedWithLeave;
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext3")).Text = ((LinkButton)e.Item.Cells[9].FindControl("lnkNext3")).Text.Replace("Leave", " ");
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext3")).Visible = true;
                }
                else if (next3 == "4")
                {
                    e.Item.Cells[10].BackColor = totalBusy;
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext3")).Visible = false;
                }
                else if (Convert.ToInt32(next3) >= 1)
                {
                    e.Item.Cells[10].BackColor = partialFill;
                }
                else
                {
                    e.Item.Cells[10].BackColor = totalFree;
                }
                ///////
                if (next4.Contains("Leave0"))
                {
                    e.Item.Cells[11].BackColor = leave;
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext4")).Visible = false;
                }
                else if (next4.Contains("Leave"))
                {
                    e.Item.Cells[11].BackColor = bookedWithLeave;
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext4")).Text = ((LinkButton)e.Item.Cells[9].FindControl("lnkNext4")).Text.Replace("Leave", " ");
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext4")).Visible = true;

                }
                else if (next4 == "4")
                {
                    e.Item.Cells[11].BackColor = totalBusy;
                    ((LinkButton)e.Item.Cells[9].FindControl("lnkNext4")).Visible = false;
                }
                else if (Convert.ToInt32(next4) >= 1)
                {
                    e.Item.Cells[11].BackColor = partialFill;
                }
                else
                {
                    e.Item.Cells[11].BackColor = totalFree;
                }
                #endregion
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "errMsg", "javascript:alert(\"Error:- Page -> PhleboSchedule.aspx -> Method -> 'rgdSchedule_ItemDataBound' \\n Description:- \\n" + ex.Message + "\");", true);
            return;
        }
    }
    #endregion

    #region Check Values in Grid
    void CheckValuesInGrid()
    {
        try
        {
            string strStartDate = string.Empty;
            string strEndDate = string.Empty;
            string strConsultantId = string.Empty;            
            string strDescription = string.Empty;

            string query = string.Empty;

            strConsultantId = cmbSchedularConsultant.SelectedValue;
            if (strConsultantId == "All")
                strConsultantId = "";
            foreach (GridDataItem item in rgdSchedule.Items)
            {
                if (((CheckBox)item.Cells[1].FindControl("chkCode")).Text.Replace("&nbsp;", "").Length == 0 && ((CheckBox)item.Cells[1].FindControl("chkCode")).Checked == true
                    || ((CheckBox)item.Cells[2].FindControl("chkCode2")).Text.Replace("&nbsp;", "").Length == 0 && ((CheckBox)item.Cells[2].FindControl("chkCode2")).Checked == true
                    || ((CheckBox)item.Cells[3].FindControl("chkCode3")).Text.Replace("&nbsp;", "").Length == 0 && ((CheckBox)item.Cells[3].FindControl("chkCode3")).Checked == true
                    || ((CheckBox)item.Cells[4].FindControl("chkCode4")).Text.Replace("&nbsp;", "").Length == 0 && ((CheckBox)item.Cells[4].FindControl("chkCode4")).Checked == true)
                {
                    int time = 0;
                    int minute = 0;

                    minute = 15;
                    //if (((CheckBox)item.Cells[1].FindControl("chkCode")).Text.Replace("&nbsp;", "").Length == 0 && ((CheckBox)item.Cells[1].FindControl("chkCode")).Checked == true)
                    //    time = "15";
                    if (((CheckBox)item.Cells[2].FindControl("chkCode2")).Text.Replace("&nbsp;", "").Length == 0 && ((CheckBox)item.Cells[2].FindControl("chkCode2")).Checked == true)
                        time = 15;
                    else if (((CheckBox)item.Cells[3].FindControl("chkCode3")).Text.Replace("&nbsp;", "").Length == 0 && ((CheckBox)item.Cells[3].FindControl("chkCode3")).Checked == true)
                        time = 30;
                    else if (((CheckBox)item.Cells[4].FindControl("chkCode4")).Text.Replace("&nbsp;", "").Length == 0 && ((CheckBox)item.Cells[4].FindControl("chkCode4")).Checked == true)
                        time = 45;
                    else
                        time = 00;
                    //}
                    //((CheckBox)item.Cells[1].FindControl("chkCode")).Checked = true;
                    //((CheckBox)item.Cells[1].FindControl("chkCode")).Enabled = false;
                    string selectedTime = ((Label)item.Cells[1].FindControl("lblSlab")).Text;

                    //////
                    //timeSlot = selectedTime.Substring(0, selectedTime.IndexOf(" "));
                    timeSlot = selectedTime.Substring(0, selectedTime.IndexOf("-"));
                    //string meridiem = selectedTime.Substring(selectedTime.IndexOf(" "));
                    //////

                    strStartDate = rdDate.SelectedDate.ToString();
                    strStartDate = Convert.ToDateTime(strStartDate).ToString("dd MMM yyyy") + " " + timeSlot.Trim() + ":" + time.ToString();// +meridiem;
                    strStartDate = Convert.ToDateTime(strStartDate).ToString("dd MMM yyyy HH:mm:ss");
                    //strEndDate = Convert.ToDateTime(strStartDate).AddHours(1).ToString("dd MMM yyyy  HH:mm:ss");
                    strEndDate = Convert.ToDateTime(strStartDate).AddMinutes(minute).ToString("dd MMM yyyy  HH:mm:ss");
                    //strDescription = Convert.ToString(Request.QueryString["CustomerName"]) + ", " + Convert.ToString(cmbSchedularLablocation.SelectedItem.Text) + "";
                    if (Convert.ToString(Session["id"]).Length == 0)
                    {
                        CreateTableStructure();
                        DataRow dr = ((DataTable)ViewState["objDtScheduler"]).NewRow();
                        dr[0] = 0;                        
                        dr[1] = Convert.ToString(cmbSchedularConsultant.SelectedValue);//.Length > 3 ? Convert.ToString(cmbSchedularConsultant.SelectedValue) : Convert.ToString(cmbSchedularConsultant.SelectedItem.Text);
                        dr[2] = custID;                        
                        dr[3] = "";
                        dr[4] = strStartDate;
                        dr[5] = strEndDate;
                        dr[6] = strDescription;
                        dr[7] = TB_nContactID;
                        dr[8] = "I";
                        dr[9] = "0";
                        hdnScheduledDate.Value = strStartDate;
                        //hdnLocId.Value = Convert.ToString(cmbSchedularLablocation.SelectedValue);
                       
                        //hdnLocName.Value = Convert.ToString(cmbSchedularLablocation.SelectedItem.Text);
                        hdnConsultantId.Value = Convert.ToString(cmbSchedularConsultant.SelectedValue);
                        hdnConsultantName.Value = Convert.ToString(cmbSchedularConsultant.SelectedItem.Text);
                        ((DataTable)ViewState["objDtScheduler"]).Rows.Add(dr);
                        Session["Schedular"] = (DataTable)ViewState["objDtScheduler"];
                        Session["id"] = "";
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "closeWin", "javascript:CloseWindow();", true);
                    }
                    else
                    {
                        CreateTableStructure();
                        hdnScheduledDate.Value = strStartDate;
                        //hdnLocId.Value = Convert.ToString(cmbSchedularLablocation.SelectedValue);
                        //hdnLocName.Value = Convert.ToString(cmbSchedularLablocation.SelectedItem.Text);
                       
                        hdnConsultantId.Value = Convert.ToString(cmbSchedularConsultant.SelectedValue);
                        hdnConsultantName.Value = Convert.ToString(cmbSchedularConsultant.SelectedItem.Text);
                        DataRow dr = ((DataTable)ViewState["objDtScheduler"]).NewRow();
                        dr[0] = Convert.ToInt32(Session["id"]);
                        dr[1] = hdnConsultantId.Value;
                        dr[2] = custID;
                        dr[3] = "";                        
                        dr[4] = strStartDate;
                        dr[5] = strEndDate;
                        dr[6] = strDescription;
                        dr[7] = Convert.ToInt32(Session["UserId"]);
                        dr[8] = "U";
                        dr[9] = "0";

                        ((DataTable)ViewState["objDtScheduler"]).Rows.Add(dr);
                        Session["Schedular"] = (DataTable)ViewState["objDtScheduler"];
                        Session["id"] = "";

                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "closeWin", "javascript:CloseWindow();", true);
                    }
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "errMsg", "javascript:alert(\"Error:- Page -> PhleboSchedule.aspx -> Method -> 'CheckValuesInGrid' \\n Description:- \\n" + ex.Message + "\");", true);
            return;
        }
    }
    #endregion

    
    #region Get GetConsultantList
    public void GetConsultantList(int AccountID)
    {
        try
        {
            //if (cmdContactType.SelectedIndex >= 0)
            //{
                string Consultant = "";
                cmbSchedularConsultant.Items.Clear();
                objScheduleWS = new ScheduleWS();
                DataSet dtConsultant = objScheduleWS.GetConsultant(Consultant, System.Configuration.ConfigurationManager.AppSettings["ContactType"], AccountID);
                //string[] cols = new string[2] { "crm_sug_phlebotomist_id", "crm_sug_phlebotmist" };
                cmbSchedularConsultant.DataSource = dtConsultant.Tables[0];
                cmbSchedularConsultant.DataValueField = "id";
                cmbSchedularConsultant.DataTextField = "contact_full_name";
                cmbSchedularConsultant.DataBind();

                cmbSchedularConsultant.Items.Insert(0, new RadComboBoxItem("", "0"));
            //}
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "errMsg", "javascript:alert(\"Error:- Page -> PhleboSchedule.aspx -> Method -> 'CreateTableStructure' \\n Description:- \\n" + ex.Message + "\");", true);
            return;
        }
    }
    #endregion

    #region FillStates
    private void FillStates(string strCountryCode)
    {
        DataSet objDataSet = objGlobalWS.FetchStateForAppointment(strCountryCode);

        if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
        {
            LogMessage(objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
            return;
        }
        cmbStates.DataSource = objDataSet.Tables[0];
        cmbStates.DataValueField = "state_code";
        cmbStates.DataTextField = "state_name";
        cmbStates.DataBind();

        RadComboBoxItem lst = new RadComboBoxItem("", "");
        cmbStates.Items.Insert(0, lst);
    }
    #endregion

    #region FillCities
    private void FillCities(string strStateCode)
    {
        DataSet objDataSet = objGlobalWS.FetchCity(strStateCode);

        if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
        {
            LogMessage(objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
            return;
        }
        cmbCity.DataSource = objDataSet.Tables[0];
        cmbCity.DataValueField = "city_code";
        cmbCity.DataTextField = "city_name";
        cmbCity.DataBind();

        RadComboBoxItem lst = new RadComboBoxItem("", "");
        cmbCity.Items.Insert(0, lst);
    }
    #endregion

    #region FillBranches
    private void FillBranches(string strCityCode)
    {
        DataSet objDataSet = objGlobalWS.FetchCustomerCityWise(strCityCode);

        if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
        {
            LogMessage(objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
            return;
        }
        cmbBranch.DataSource = objDataSet.Tables[0];
        cmbBranch.DataValueField = "id";
        cmbBranch.DataTextField = "cust_name";
        cmbBranch.DataBind();

        RadComboBoxItem lst = new RadComboBoxItem("", "");
        cmbBranch.Items.Insert(0, lst);
    }
    #endregion


   
    #region Protected Schedule Button Click
    protected void btnSchedule_Click(object sender, EventArgs e)
    {
        try
        {
            GetSchedules(rdDate.SelectedDate.Value.Date, Convert.ToInt32(cmbSchedularConsultant.SelectedValue.ToString()));
            DivSchedule.Style.Add("display", "block");
            DivScheduleBranchWise.Style.Add("display", "none");
            ViewState["Request"] = "N";
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "errMsg", "javascript:alert(\"Error:- Page -> PhleboSchedule.aspx -> Method -> 'btnSchedule_Click' \\n Description:- \\n" + ex.Message + "\");", true);
            return;
        }
    }
    #endregion


    #region Get Schedules
    void GetSchedules(DateTime dtConsultationDate, int nConsultantID)
    {
        try
        {
            if (Page.IsPostBack)
                BindEmptyGrid();

            objScheduleWS = new ScheduleWS();
            DataSet dsDataset = objScheduleWS.GetSchedules(nConsultantID, dtConsultationDate, dt, TB_nTimeZoneTimeSpan);
            if (Convert.ToInt32(dsDataset.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(dsDataset.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }
            ViewState["dtSchedulerAll"] = dsDataset.Tables["Schedule"];
            rgdSchedule.DataSource = dsDataset.Tables["Schedule"];
            rgdSchedule.DataBind();
            btnFixedAppointment.Enabled = true;
        }
        catch (Exception ex)
        {

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "errMsg", "javascript:alert(\"Error:- Page -> PhleboSchedule.aspx -> Method -> 'GetSchedules' \\n Description:- \\n" + ex.Message + "\");", true);
            return;
        }
    }
    #endregion

    #region Filling Combo of Category for Contact
    private void FillContactTypeValue()
    {
        DataSet objDataSet = objGlobalWS.FetchGeneralValues("CNTTYPE");

        if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
        {
            LogMessage(objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
            return;
        }
        else
        {
            //cmdContactType.DataSource = objDataSet.Tables[0];
            //cmdContactType.DataValueField = "code";
            //cmdContactType.DataTextField = "name";
            //cmdContactType.DataBind();

            //RadComboBoxItem lst = new RadComboBoxItem("", "");
            //cmdContactType.Items.Insert(0, lst);
        }
    }
    #endregion

    #region Appointment Button Click Event
    protected void btnAppointment_Click(object sender, EventArgs e)
    {
        CheckValuesInGrid();
        Session["id"] = "";
    }
    #endregion

    #region seleceteddatechanged event
    protected void rdDate_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
    {
        //if (cmbSchedularConsultant.SelectedValue.Length > 5)
        //    GetSchedules(rdDate.SelectedDate.Value.Date.Day.ToString(), cmbSchedularConsultant.SelectedValue.ToString());
        //else
        //    GetSchedules(rdDate.SelectedDate.Value.Date.Day.ToString(), "ALL");
    }

    #endregion

    #region Private Create Table
    private void CreateTableStructure()
    {
        try
        {
            DataTable dt = new DataTable();
            dt = new DataTable();
            dt.Columns.Add("id", Type.GetType("System.Int32"));            
            dt.Columns.Add("schd_contact_id", Type.GetType("System.String"));
            dt.Columns.Add("schd_customer_id", Type.GetType("System.String"));
            dt.Columns.Add("schd_location_id", Type.GetType("System.String"));
            dt.Columns.Add("schd_start_time", Type.GetType("System.String"));
            dt.Columns.Add("schd_end_time", Type.GetType("System.String"));
            dt.Columns.Add("schd_description", Type.GetType("System.String"));
            dt.Columns.Add("created_by", Type.GetType("System.Int32"));
            dt.Columns.Add("is_updated", Type.GetType("System.String"));
            dt.Columns.Add("schd_appointment_id", Type.GetType("System.String"));
            //dt.Columns.Add("Next1", Type.GetType("System.String"));
            //dt.Columns.Add("Next2", Type.GetType("System.String"));
            //dt.Columns.Add("Next3", Type.GetType("System.String"));
            //dt.Columns.Add("Next4", Type.GetType("System.String"));

            ViewState["objDtScheduler"] = dt;
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "errMsg", "javascript:alert(\"Error:- Page -> PhleboSchedule.aspx -> Method -> 'CreateTableStructure' \\n Description:- \\n" + ex.Message + "\");", true);
            return;
        }
    }
    #endregion

    #region Get Next Schedule (Grid Link Click)
    protected void NextSchedule(object sender, EventArgs e)
    {
        try
        {
            string day = ((LinkButton)sender).CommandArgument;
            rdDate.SelectedDate = rdDate.SelectedDate.Value.AddDays(Convert.ToInt32(day));
            GetSchedules(rdDate.SelectedDate.Value.Date, Convert.ToInt32(cmbSchedularConsultant.SelectedValue));
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "errMsg", "javascript:alert(\"Error:- Page -> PhleboSchedule.aspx -> Method -> 'NextSchedule' \\n Description:- \\n" + ex.Message + "\");", true);
            return;
        }
    }
    #endregion

    #region Button Previous Schedule
    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        try
        {
            //string day = ((LinkButton)sender).CommandArgument;
            rdDate.SelectedDate = rdDate.SelectedDate.Value.AddDays(-1);
            if (Convert.ToString(ViewState["Request"]) == "N")
                GetSchedules(rdDate.SelectedDate.Value.Date, Convert.ToInt32(cmbSchedularConsultant.SelectedValue.ToString()));
            else
                GetSchedulesBranchWise(Convert.ToInt32(cmbBranch.SelectedValue), Convert.ToDateTime(rdDate.SelectedDate));
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "errMsg", "javascript:alert(\"Error:- Page -> PhleboSchedule.aspx -> Method -> 'btnPrevious_Click' \\n Description:- \\n" + ex.Message + "\");", true);
            return;
        }
    }
    #endregion

    #region Button Next Click
    protected void btnNext_Click(object sender, EventArgs e)
    {
        try
        {
            //string day = ((LinkButton)sender).CommandArgument;
            rdDate.SelectedDate = rdDate.SelectedDate.Value.AddDays(1);
            if (Convert.ToString(ViewState["Request"]) == "N")
                GetSchedules(rdDate.SelectedDate.Value.Date, Convert.ToInt32(cmbSchedularConsultant.SelectedValue.ToString()));
            else
                GetSchedulesBranchWise(Convert.ToInt32(cmbBranch.SelectedValue), Convert.ToDateTime(rdDate.SelectedDate));
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "errMsg", "javascript:alert(\"Error:- Page -> PhleboSchedule.aspx -> Method -> 'btnNext_Click' \\n Description:- \\n" + ex.Message + "\");", true);
            return;
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


    //protected void cmdContactType_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    //{
    //    GetConsultantList();
    //}

    protected void cmbStates_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (cmbStates.SelectedValue != null)
            FillCities(cmbStates.SelectedValue);
    }

    protected void cmbCity_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (cmbCity.SelectedValue != null)
            FillBranches(cmbCity.SelectedValue);
    }

    protected void cmbBranch_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (cmbBranch.SelectedValue != null)
            GetConsultantList(Convert.ToInt32(cmbBranch.SelectedValue));
    }

    protected void btnScheduleBranchwise_Click(object sender, EventArgs e)
    {
        try
        {
            GetSchedulesBranchWise(Convert.ToInt32(cmbBranch.SelectedValue), Convert.ToDateTime(rdDate.SelectedDate));
            DivSchedule.Style.Add("display", "none");
            DivScheduleBranchWise.Style.Add("display", "block");            
            ViewState["Request"] = "B";
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "errMsg", "javascript:alert(\"Error:- Page -> Schedule.aspx -> Method -> 'btnScheduleBranchwise_Click' \\n Description:- \\n" + ex.Message + "\");", true);
            return;
        }
        
    }

    #region Get Schedules Branch Wise
    void GetSchedulesBranchWise(int BranchID, DateTime dtConsultationDate)
    {
        try
        {
            objScheduleWS = new ScheduleWS();
            DataSet ds = objScheduleWS.GetSchedulesBranchWise(BranchID, dtConsultationDate, TB_nTimeZoneTimeSpan);
            //DataSet ds = objScheduleWS.GetSchedulesBranchWise(10, Convert.ToDateTime("22 May 2011"), 530);
            rdgDataBranchWise.DataSource = ds.Tables["FinalData"];
            rdgDataBranchWise.DataBind();
            btnFixedAppointment.Enabled = false;
            
        }
        catch (Exception ex)
        {

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "errMsg", "javascript:alert(\"Error:- Page -> Schedule.aspx -> Method -> 'GetSchedulesBranchWise' \\n Description:- \\n" + ex.Message + "\");", true);
            return;
        }
    }
    #endregion

    #region rdgDataBranchWise Item Databound Event
    protected void rdgDataBranchWise_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        try
        {           
            if (e.Item.ItemType == Telerik.Web.UI.GridItemType.Item || e.Item.ItemType == Telerik.Web.UI.GridItemType.AlternatingItem)
            {
                System.Drawing.Color totalFree = System.Drawing.Color.PaleTurquoise;
                System.Drawing.Color partialFill = System.Drawing.Color.LightPink;//OldLace;
                System.Drawing.Color totalBusy = System.Drawing.Color.LightGray;
                
                string next1 = ((Label)e.Item.Cells[3].FindControl("lblSchdStatus1")).Text;
                string next2 = ((Label)e.Item.Cells[4].FindControl("lblSchdStatus2")).Text;
                string next3 = ((Label)e.Item.Cells[5].FindControl("lblSchdStatus3")).Text;
                string next4 = ((Label)e.Item.Cells[6].FindControl("lblSchdStatus4")).Text;

                string[] strVal1 = next1.Split('/');
                string[] strVal2 = next2.Split('/');
                string[] strVal3 = next3.Split('/');
                string[] strVal4 = next4.Split('/');

                #region set color for next 4 days schedule
                ////

                if (strVal1[0] == "0")
                {
                    e.Item.Cells[3].BackColor = totalFree;
                    
                }
                else if (strVal1[0] == strVal1[1])
                {
                    e.Item.Cells[3].BackColor = totalBusy;
                }
                else
                {
                    e.Item.Cells[3].BackColor = partialFill;
                }

                if (strVal2[0] == "0")
                {
                    e.Item.Cells[4].BackColor = totalFree;

                }
                else if (strVal2[0] == strVal2[1])
                {
                    e.Item.Cells[4].BackColor = totalBusy;
                }
                else
                {
                    e.Item.Cells[4].BackColor = partialFill;
                }

                if (strVal3[0] == "0")
                {
                    e.Item.Cells[5].BackColor = totalFree;

                }
                else if (strVal3[0] == strVal3[1])
                {
                    e.Item.Cells[5].BackColor = totalBusy;
                }
                else
                {
                    e.Item.Cells[5].BackColor = partialFill;
                }

                if (strVal4[0] == "0")
                {
                    e.Item.Cells[6].BackColor = totalFree;

                }
                else if (strVal4[0] == strVal4[1])
                {
                    e.Item.Cells[6].BackColor = totalBusy;
                }
                else
                {
                    e.Item.Cells[6].BackColor = partialFill;
                }
                
                #endregion
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "errMsg", "javascript:alert(\"Error:- Page -> PhleboSchedule.aspx -> Method -> 'rgdSchedule_ItemDataBound' \\n Description:- \\n" + ex.Message + "\");", true);
            return;
        }
    }
    #endregion
}
