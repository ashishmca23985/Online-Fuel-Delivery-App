using System;
using Telerik.Web.UI;
using System.Data;

public partial class General_CaseSummary : ThemeBase
{
    bool hr = false;
    GlobalWS objGlobalWS = new GlobalWS();
    MasterWS objmasterws = new MasterWS();

    protected void Page_Load(object sender, EventArgs e)
    {

        lblMessage.Text = "";
        if (IsPostBack)
            return;
        GetAccount();
    }

    #region Getting account
    private void GetAccount()
    {
        try
        {
           // cmbLocation.radComboBoxBind(objGlobalWS.Fetchaccountbyshort(), "cust_name", "id", "crm_accounts", "All");
        }
        catch (Exception ex)
        {
            lblMessage.LogMessage(ex.Message, 1);
        }
    }
    #endregion
    protected void cmbLocation_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        // FillDepartment(cmbLocation.SelectedIndex > 0 ? Convert.ToInt32(cmbLocation.SelectedValue) : 0);
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        SearchOpenCase();
    }

    private void SearchOpenCase()
    {

        int accountid = cmbLocation.SelectedIndex > 0 ? Convert.ToInt32(cmbLocation.SelectedValue) : 0;
        try
        {
            string sp = "";
            if (rbnttype.SelectedValue == "O")
            {
                sp = "Case_Summary";
            }
            else if (rbnttype.SelectedValue == "T")
            {
                sp = "Case_Summary_team";
            }
            else
            {
                sp = "Case_Summary_Account";
            }
            if (rbnttype.SelectedValue == "O")
            {
                rdgRating.Columns[2].Visible = true;
                rdgRating.Columns[1].HeaderText = "Team Name";
            }
            else if (rbnttype.SelectedValue == "T")
            {
                rdgRating.Columns[2].Visible = false;
                rdgRating.Columns[1].HeaderText = "Team Name";
            }
            else
            {
                rdgRating.Columns[2].Visible = false;
                rdgRating.Columns[1].HeaderText = "Account Name";
            }

            DataSet m_DataSet;
            m_DataSet = objmasterws.GetSearchOpenCaseSummary(accountid,TB_nContactID, sp);

            rdgRating.radRadGrid(m_DataSet, 0);

            ViewState["dt"] = m_DataSet.Tables[0];
        }
        catch (Exception ex)
        {
            lblMessage.LogMessage(ex.Message, 1);
            return;
        }
    }
    protected void rdgRating_PageIndexChanged(object sender, GridPageChangedEventArgs e)
    {
        rdgRating.CurrentPageIndex = e.NewPageIndex;
        SearchOpenCase();
    }

    protected void rdgRating_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
    {
        rdgRating.PageSizeChanged -= new GridPageSizeChangedEventHandler(rdgRating_PageSizeChanged);
        rdgRating.PageSize = e.NewPageSize;
        rdgRating.PageSizeChanged += new GridPageSizeChangedEventHandler(rdgRating_PageSizeChanged);
        SearchOpenCase();

    }

    protected void rdgcaserating_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem item = (GridDataItem)e.Item;
            //item.Attributes.Add("class", "rgAltRow");
            //item.CssClass = "rgAltRow";
            string type = "owner_id";
            string value = "";
            //type = item.Cells[3].Text == "Case" ? "CAS" : "TSK";

            if (rbnttype.SelectedValue == "O")
                type = "owner_id";
            else if (rbnttype.SelectedValue == "T")
                type = "Team_id";
            else
                type = "location_id@" + item.Cells[10].Text + "@@case_customer_id";
            value = "@@" + type + "@" + item.Cells[9].Text;
            if (item.Cells[5].Text != "0")
            {
                item.Cells[5].Attributes.Add("onclick", "CaseStatus('" + value + "@@case_status_id<>2" + "','" + "Case View" + "','" + item.Cells[5].Text + "');");
                item.Cells[5].CssClass = "OpenCases";
            }
            if (item.Cells[6].Text != "0")
            {
                item.Cells[6].Attributes.Add("onclick", "CaseStatus('" + value + "@@case_status_id@1" + "','" + "Open Case" + "','" + item.Cells[6].Text + "');");
                item.Cells[6].CssClass = "RatingPending";
            }
            if (item.Cells[7].Text != "0")
            {
                item.Cells[7].Attributes.Add("onclick", "CaseStatus('" + value + "@@case_status_id@3" + "','" + "Open Working Case" + "','" + item.Cells[7].Text + "');");
                item.Cells[7].CssClass = "RatingClose";
            }
            if (item.Cells[8].Text != "0")
            {
                item.Cells[8].Attributes.Add("onclick", "CaseStatus('" + value + "@@case_status_id@7" + "','" + "Pending For Feedback Case" + "','" + item.Cells[8].Text + "');");
                item.Cells[8].CssClass = "RatingDiscard";
            }
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (ViewState["dt"] != null)
        {
            rdgRating.AllowPaging = false;

            rdgRating.DataSource = (DataTable)ViewState["dt"];
            rdgRating.DataBind();

            if (rdgRating.MasterTableView.Items.Count > 0)
            {
                ConfigureExport();
                rdgRating.MasterTableView.ExportToExcel();
            }
            rdgRating.AllowPaging = true;
        }
    }
    public void ConfigureExport()
    {
        rdgRating.MasterTableView.Caption = (rbnttype.SelectedValue == "O" ? "Owner Wise " : "Team Wise ") + "Open Case Summary Report ";
        rdgRating.ExportSettings.ExportOnlyData = true;
        rdgRating.ExportSettings.IgnorePaging = false;
        rdgRating.ExportSettings.OpenInNewWindow = true;
    }
    protected void rdgRating_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName == Telerik.Web.UI.RadGrid.ExportToExcelCommandName ||
            e.CommandName == Telerik.Web.UI.RadGrid.ExportToWordCommandName ||
            e.CommandName == Telerik.Web.UI.RadGrid.ExportToCsvCommandName ||
            e.CommandName == Telerik.Web.UI.RadGrid.ExportToPdfCommandName)
        {
            ConfigureExport();
        }
    }
}
