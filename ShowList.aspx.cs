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
using System.IO;
using Telerik.Web.UI;

public partial class ShowList : ThemeBase
{
    bool ApplyOwnerFilter;
    bool ApplyViewFilter;
    int ActivityID;
    public string objectType = "";
    public string objectTypeName = "";
    public string objectTableName = "";
    public string objectTypeNameadd = "";
    public string sourceType = "";
    public string sourceId = "0";
    public string queryfilter = "";
    int nUniqueID = 0;
    public string objectaddurl = "";
    public string CLI = "";
    public string email = "";

    GlobalWS objGlobalWS = new GlobalWS();
    DataBase m_Connection = new DataBase();

    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        m_Connection.OpenDB("Galaxy");
        if (string.IsNullOrEmpty(Request.QueryString["ObjectType"]))
        {
            lblMessage.Text = "Object Type not specified for ShowList";
            return;
        }

        if (!string.IsNullOrEmpty(Request.QueryString["Source"]))
            sourceType = Request.QueryString["Source"].ToString();
        if (!string.IsNullOrEmpty(Request.QueryString["SourceID"]))
        {
            sourceId = Request.QueryString["SourceID"].ToString();
            if (Convert.ToInt32(sourceId) <= 0)
                rdpLeft.Visible = true;
            else
                rdpLeft.Visible = false;
        }
        objectType = Request.QueryString["ObjectType"].ToString();
        
        //--set search text and Fetch Cases
        if (!string.IsNullOrEmpty(Request.QueryString["SearchText"]))
            txtSearchText.Text = Request.QueryString["SearchText"].ToString();

        if (!string.IsNullOrEmpty(Request.QueryString["CLI"]))
        {
            CLI = Request.QueryString["CLI"].ToString();
        }
        if (!string.IsNullOrEmpty(Request.QueryString["email"]))
        {
            email = Request.QueryString["email"].ToString();
        }
        
        ApplyOwnerFilter = true;
        ApplyViewFilter = true;
        if (!string.IsNullOrEmpty(sourceType) && !string.IsNullOrEmpty(sourceId)) // from another object
        {
            ApplyOwnerFilter = false;
            ApplyViewFilter = false;
        }

        //--check permission level and disable buttons accordingly
        DataSet dsDataSet = objFrameworkWS.GetObjectPermissionLevel(TB_nUserRoleID, objectType);
        if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
        {
            lblMessage.Text = Convert.ToString(dsDataSet.Tables["Response"].Rows[0]["ResultString"]);
            return;
        }
        if (dsDataSet.Tables["Permisson"].Rows.Count == 0)
        {
            lblMessage.Text = "No Permisson defined";
            return;
        }

        
        objectTypeName = dsDataSet.Tables["Permisson"].Rows[0]["object_name"].ToString();
        objectTypeNameadd = dsDataSet.Tables["Permisson"].Rows[0]["object_add_name"].ToString();
        objectTableName = dsDataSet.Tables["Permisson"].Rows[0]["object_table_name"].ToString();
        ActivityID = Convert.ToInt32(dsDataSet.Tables["Permisson"].Rows[0]["ActivityID"]);
        objectaddurl = dsDataSet.Tables["Permisson"].Rows[0]["object_add_url"].ToString();
      //  objectaddurl.Replace("#ID#", sourceId);
        btnAdd.Visible = objFrameworkWS.Add;
        btnEdit.Visible = objFrameworkWS.Edit;
        if (objFrameworkWS.Export)
        {
            //25-march
            trExport.Visible = true;
            trExportcontrol.Visible = true;

            // trExport.Style.Add("display", "block");
            // trExportDesign.Style.Add("display", "block");
        }
        else
        {
            trExport.Visible = false;
            trExportcontrol.Visible = false;
            //25-march
            //trExport.Style.Add("display", "none");
            // trExportDesign.Style.Add("display", "none");        
        }
        //lnkExport.Visible = objFrameworkWS.Export;
        lnkDelete.Visible = objFrameworkWS.Delete;
        //if (sourceType != "")
        //    lnkCustomize.Visible = false;
        //else
        //    lnkCustomize.Visible = objFrameworkWS.View;
        if (objFrameworkWS.Admin == true)
            ApplyOwnerFilter = false;
        if (objFrameworkWS.View == false)
        {
            lblMessage.Text = "No permssions assigned for " + objectTypeName;
            return;
        }

        if (ViewState["UniqueListID"] != null)
            nUniqueID = Convert.ToInt32(ViewState["UniqueListID"]);

        if (IsPostBack)
            return;

        if (Session["UniqueListID"] != null)
            nUniqueID = Convert.ToInt32(Session["UniqueListID"]);
        ViewState["UniqueListID"] = nUniqueID;
        Session["UniqueListID"] = nUniqueID + 1;

        BindViewCombo();
        BindExportCombo();
        BindSearchCombo();
        // by Query String filteration 
     
         
        // Read View Settings for the objectType (Show My only, Show Recent and select view id)
        if (GetViewSettingsForUser() == false)
            return;
        if (!string.IsNullOrEmpty(Request.QueryString["Query"]))
        {
            queryfilter = Request.QueryString["Query"].ToString();
            chkMyAccount.Checked = false;
            ApplyOwnerFilter = false;
           // ApplyViewFilter = false;
        }
        if (Request.QueryString["Header"] != null)
        {
            hdnCurrentViewTitle.Value = Request.QueryString["Header"].ToString();
        }

        imgType.Src = dsDataSet.Tables["Permisson"].Rows[0]["image_path"].ToString();
        imgType.Alt = objectTypeName;
        lblType.Text = objectTypeName;

        string[] strColumns = new string[2];
        strColumns[0] = "ID";
        strColumns[1] = "IDName";
        rdgCommon.MasterTableView.DataKeyNames = strColumns;
        rdgCommon.MasterTableView.ClientDataKeyNames = strColumns;
        rdgCommon.ClientSettings.ClientEvents.OnRowSelected = "GetSelectedID";
        rdgCommon.ClientSettings.ClientEvents.OnRowDeselected = "GetDeSelectedID";
        rdgCommon.ClientSettings.ClientEvents.OnRowClick = "GetID";
        rdgCommon.ClientSettings.ClientEvents.OnRowDblClick = "OpenRecord";
        if (objectTypeNameadd.ToLower() == "opf")
            btnAdd.Attributes.Add("onclick", "return AddItem('"+objectaddurl+"')");
        else
            btnAdd.Attributes.Add("onclick", "return AddItem('ShowObject.aspx?ID=0&ObjectType=" + objectType + "&Source=" + sourceType + "&SourceID=" + sourceId + "')");
        btnEdit.Attributes.Add("onclick", "return EditItem()");
        lnkDelete.Attributes.Add("onclick", "return OpenCommaonAttributesWindow('D')");
        //chkMyAccount.Text = "My " + objectTypeName;

        RadToolTipManager1.TargetControls.Clear();
        if (!string.IsNullOrEmpty(Request.QueryString["CLI"]))
        {
           // txtSearchText.Text = Request.QueryString["CLI"].ToString();
            string url=Request.Url.Query;
            url=url.Replace("&ObjectType=CNT","");
            hdnurl.Value=url.Replace('?','&');
            chkMyAccount.Checked = false;
            chkRecentActivity.Checked = false;
            
        }
        if (objectType == "CAS")
        {
            tropendate.Visible = true;
            trclosedate.Visible = true;
        }
        else
        {
            tropendate.Visible = false;
            trclosedate.Visible = false;
        }
        GetData(true);
    }
    #endregion

    #region Bind Export Content Type Combo Box
    public void BindExportCombo()
    {
        try
        {
            DataSet objDataSet = objGlobalWS.FetchGeneralValues("CONTENTTYPE");

            if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(objDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }
            cmbExport.DataSource = objDataSet.Tables[0];
            cmbExport.DataValueField = "code";
            cmbExport.DataTextField = "name";
            cmbExport.DataBind();

            RadComboBoxItem rdItem = new RadComboBoxItem("Select Format", "");
            cmbExport.Items.Insert(0, rdItem);
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region GetViewSettingsForUser
    public bool GetViewSettingsForUser()
    {
        bool returnValue = false;
        try
        {
            //--get Recent Account Detail
            DataSet dsDataSet = objGlobalWS.GetViewSettingsForUser(TB_nContactID, objectType, ApplyViewFilter);
           // DataSet dsDataSet = objGlobalWS.GetViewSettingsForUser(TB_nContactID, objectType);
            if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(dsDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
                return returnValue;
            }
            if (dsDataSet.Tables["RecentDetails"].Rows.Count > 0)
            {
                if (Convert.ToString(dsDataSet.Tables["RecentDetails"].Rows[0]["recent_activity"]) == "Y")
                    chkRecentActivity.Checked = true;
                else
                    chkRecentActivity.Checked = false;

                if (Convert.ToString(dsDataSet.Tables["RecentDetails"].Rows[0]["my_account"]) == "Y")
                    chkMyAccount.Checked = true;
                else
                    chkMyAccount.Checked = false;
                txtRecentDays.Text = Convert.ToString(dsDataSet.Tables["RecentDetails"].Rows[0]["recent_activity_days"]);
                hdnCurrentViewId.Value = "0";
                if (Convert.ToString(dsDataSet.Tables["RecentDetails"].Rows[0]["selected_view_id"]) != "0")
                {
                    //cmbView.SelectedValue = Convert.ToString(dsDataSet.Tables["RecentDetails"].Rows[0]["selected_view_id"]);
                    hdnCurrentViewId.Value = Convert.ToString(dsDataSet.Tables["RecentDetails"].Rows[0]["selected_view_id"]);
                    hdnCurrentViewTitle.Value = objGlobalWS.ExecuteQuery("SELECT #GetSelectedViewName(" + hdnCurrentViewId.Value + ")");
                    treeView1.Nodes.FindNodeByValue(hdnCurrentViewId.Value).Selected = true;
                }
                //if (cmbView.SelectedItem != null)
                //    hdnCurrentViewTitle.Value = cmbView.SelectedItem.Text;
                returnValue = true;
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
        return returnValue;
    }
    #endregion

    #region Bind View ComboBox
    public void BindViewCombo()
    {
        try
        {
            DataSet objDataSet = objGlobalWS.GetViewsForUser(objectType, TB_nContactID);

            if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(objDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }
            //cmbView.DataSource = objDataSet.Tables["UserViews"];
            //cmbView.DataValueField = "view_id";
            //cmbView.DataTextField = "view_name";
            //cmbView.DataBind();

            foreach (DataRow dr in objDataSet.Tables["UserViews"].Rows)
            {
                RadTreeNode trNode = new RadTreeNode("<img src='images/folder.png' /> " + Convert.ToString(dr["view_name"]), Convert.ToString(dr["view_id"]));
                treeView1.Nodes.Add(trNode);
            }




        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region Bind Search Combo Box
    public void BindSearchCombo()
    {
        try
        {
            DataSet objDataSet = objGlobalWS.GetTableColumns(objectType);

            if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(objDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }
            cmbSearch.DataSource = objDataSet.Tables[0];
            cmbSearch.DataValueField = "tabledef_column_name";
            cmbSearch.DataTextField = "tabledef_column_header";
            cmbSearch.DataBind();

            RadComboBoxItem rdItem = new RadComboBoxItem("All", "All");
            cmbSearch.Items.Insert(0, rdItem);
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region Search ImageButton Click
    protected void SearchData(object sender, EventArgs e)
    {
        GetData(false);
        rdgCommon.Rebind();

    }
    #endregion

    #region Search ImageButton Click
    protected void SelectView(object sender, EventArgs e)
    {
        //hdnCurrentViewId.Value = cmbView.SelectedValue;
        //hdnCurrentViewTitle.Value = cmbView.SelectedItem.Text;
        GetData(true);
        rdgCommon.Rebind();
    }
    #endregion

    #region RadGrid Column Created
    protected void rdgCommon_ColumnCreated(object sender, GridColumnCreatedEventArgs e)
    {
        try
        {
            if (e.Column.ColumnType == "GridBoundColumn" || e.Column.ColumnType == "GridDateTimeColumn" || e.Column.ColumnType== "GridNumericColumn")
            {
                //((GridBoundColumn)e.Column).DataFormatString = "<nobr>{0}</nobr>";
                if (((GridBoundColumn)e.Column).HeaderText == "ID")
                {
                    ((GridBoundColumn)e.Column).Visible = false;
                }
                else if (((GridBoundColumn)e.Column).HeaderText == "ID Name")
                {
                    ((GridBoundColumn)e.Column).Visible = false;
                }
                else
                {
                    //--set col. width defined in TableDefinition/CustomColumns
                    string strTemp = "TableDefinition" + nUniqueID;

                    DataTable dtTable = (DataTable)Session[strTemp];
                    foreach (DataRow row in dtTable.Rows)
                    {
                        if (((GridBoundColumn)e.Column).UniqueName == Convert.ToString(row["ColumnName"]))
                        {
                            string szVisible = Convert.ToString(row["ColVisible"]);
                            int nWidth = Convert.ToInt32(row["ColWidth"]);

                            if (szVisible == "N")
                                ((GridBoundColumn)e.Column).Visible = false;
                            else if (nWidth > 0)
                                ((GridBoundColumn)e.Column).HeaderStyle.Width = Unit.Pixel(nWidth);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    protected void rdgCommon_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        string strTemp = "TableData" + nUniqueID;
        rdgCommon.DataSource = (DataTable)Session[strTemp];
    }

    #region Get Data
    private void GetData(bool bReloadView)
    {
        try
        {
            int nViewID = Convert.ToInt32(hdnCurrentViewId.Value);
            if (nViewID <= 0)
            {
                lblMessage.Text = "No view selected";
                return;
            }
            string szSearchColumnName = string.Empty;
            string szSearchColumnText = string.Empty;
            if (!String.IsNullOrEmpty(cmbSearch.SelectedValue))
            {
                if (objectType == "CNT" && (cmbSearch.SelectedItem.Text.ToLower() != "phone" || cmbSearch.SelectedItem.Text.ToLower() != "emailid") )
                {
                    szSearchColumnName = cmbSearch.SelectedValue;
                    szSearchColumnText = txtSearchText.Text;
                }
                else if (objectType != "CNT")
                {
                    szSearchColumnName = cmbSearch.SelectedValue;
                    szSearchColumnText = txtSearchText.Text;
                }
            }

            string strCriteria = GetFilterCriteriaForView();

            strCriteria += GetFilterCriteriaByQueryStringForView();
            //--call method            
            DataSet dsDataset = objGlobalWS.FetchCommonData(szSearchColumnName, szSearchColumnText, TB_nContactID,
                                                           TB_nAccountID, TB_strAccountType, ApplyOwnerFilter, TB_nDepartmentID,
                                                           TB_nDesignationLevel, nViewID, ApplyViewFilter,
                                                           TB_nTimeZoneTimeSpan, objectType, strCriteria, Convert.ToBoolean(Session["ShowDial"]), objectTableName);

            if (Convert.ToInt32(dsDataset.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(dsDataset.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }
            string strTemp = "TableData" + nUniqueID;
            Session[strTemp] = dsDataset.Tables["Common"];
            strTemp = "TableDefinition" + nUniqueID;
            Session[strTemp] = dsDataset.Tables["TableDefinition"];

            if (bReloadView == true)
            {
                DataRow row = dsDataset.Tables["ViewDetail"].Rows[0];
                rdgCommon.PageSize = Convert.ToInt32(row["view_pagesize"]);

                LosFormatter formatter = new LosFormatter();

                // set grouping as per view
                string strInput = row["view_group_expression"].ToString();
                if (strInput != "")
                {
                    rdgCommon.MasterTableView.GroupByExpressions.Clear();
                    StringReader reader = new StringReader(strInput);
                    ArrayList arrayGroupExpressions = (ArrayList)formatter.Deserialize(reader);
                    foreach (object obj in arrayGroupExpressions)
                    {
                        GridGroupByExpression expression = new GridGroupByExpression();
                        ((IStateManager)expression).LoadViewState(obj);
                        rdgCommon.MasterTableView.GroupByExpressions.Add(expression);
                    }
                }

                // set sorting as per view
                strInput = row["view_sort_expression"].ToString();
                if (strInput != "")
                {
                    rdgCommon.MasterTableView.SortExpressions.Clear();
                    StringReader reader = new StringReader(strInput);
                    object objSortExpressions = (object)formatter.Deserialize(reader);
                    ((IStateManager)rdgCommon.MasterTableView.SortExpressions).LoadViewState(objSortExpressions);
                }

                rdgCommon.MasterTableView.FilterExpression = row["view_filter_expression"].ToString();
            }

            //--show/hide paging panel
            if (dsDataset.Tables["Common"].Rows.Count > rdgCommon.PageSize)
                rdgCommon.MasterTableView.PagerStyle.AlwaysVisible = true;
            else
                rdgCommon.MasterTableView.PagerStyle.AlwaysVisible = false;

            if (ApplyViewFilter == true)
                lblCount.Text = hdnCurrentViewTitle.Value + " - [ " + dsDataset.Tables["Common"].Rows.Count + " ]";
            else
                lblCount.Text = "";
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region RadGrid Save Layout

    protected void SaveColumnDetails(object sender, EventArgs e)
    {
        try
        {
            int nViewID = Convert.ToInt32(hdnCurrentViewId.Value);
            if (nViewID > 0)
            {
                ArrayList arrayColumns = new ArrayList();
                string strMyAccount = "N";
                string strRecentActivity = "N";
                int nRecentDays = 0;

                if (chkMyAccount.Checked == true)
                    strMyAccount = "Y";

                if (chkRecentActivity.Checked == true)
                    strRecentActivity = "Y";
                nRecentDays = Convert.ToInt32(txtRecentDays.Text);

                // all the predefined columns
                foreach (GridColumn gc in rdgCommon.MasterTableView.Columns)
                {
                    if (gc.Visible == false || gc.UniqueName.Length <= 0)
                        continue;
                    Pair p = new Pair(gc.UniqueName, gc.HeaderStyle.Width.Value.ToString());
                    arrayColumns.Add(p);
                }

                // all the autogenerated columns
                foreach (GridColumn gc in rdgCommon.MasterTableView.AutoGeneratedColumns)
                {
                    if (gc.Visible == false || gc.UniqueName.Length <= 0)
                        continue;
                    Pair p = new Pair(gc.UniqueName, gc.HeaderStyle.Width.Value.ToString());
                    arrayColumns.Add(p);
                }

                // all the groupbyexpression
                string strGroupExpression = "";
                if (rdgCommon.MasterTableView.GroupByExpressions.Count > 0)
                {
                    GridGroupByExpressionCollection groupByExpressions = rdgCommon.MasterTableView.GroupByExpressions;
                    ArrayList arrayGroupExpressions = new ArrayList();
                    foreach (GridGroupByExpression expression in groupByExpressions)
                        arrayGroupExpressions.Add(((IStateManager)expression).SaveViewState());

                    LosFormatter formatter = new LosFormatter();
                    StringWriter writer = new StringWriter();
                    formatter.Serialize(writer, arrayGroupExpressions);
                    strGroupExpression = writer.ToString();
                }

                // all the sortexpressions
                string strSortExpression = "";
                if (rdgCommon.MasterTableView.SortExpressions.Count > 0)
                {
                    object objSortExpressions = ((IStateManager)rdgCommon.MasterTableView.SortExpressions).SaveViewState();
                    LosFormatter formatter = new LosFormatter();
                    StringWriter writer = new StringWriter();
                    formatter.Serialize(writer, objSortExpressions);
                    strSortExpression = writer.ToString();
                }
                string strFilterExpression = rdgCommon.MasterTableView.FilterExpression;
                objGlobalWS.SaveViewLayout(TB_nContactID, nViewID, arrayColumns, objectType, strMyAccount, strRecentActivity, nRecentDays, strGroupExpression, strSortExpression, strFilterExpression);
                lblMessage.Text = "View layout saved successfully.";
            }
            else
            {
                lblMessage.Text = "No View Selected. Open view using GO first.";
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region Export To Excel Without Pagging
    protected void imgExport_Click(object sender, EventArgs e)
    {
        try
        {
            string strContentType = cmbExport.SelectedValue;
            if (String.IsNullOrEmpty(strContentType))
                return;
            switch (strContentType)
            {
                case "application/ms.doc":
                    {
                        rdgCommon.ExportSettings.IgnorePaging = true;
                        rdgCommon.ExportSettings.OpenInNewWindow = true;
                        rdgCommon.ExportSettings.FileName = lblType.Text;
                        rdgCommon.MasterTableView.ExportToWord();
                    }
                    break;
                case "application/ms.xls":
                    {
                        rdgCommon.ExportSettings.IgnorePaging = true;
                        rdgCommon.ExportSettings.OpenInNewWindow = true;
                        rdgCommon.ExportSettings.ExportOnlyData = true;
                        rdgCommon.ExportSettings.FileName = lblType.Text;
                        rdgCommon.MasterTableView.ExportToExcel();
                    }
                    break;
                case "application/ms.pdf":
                    {
                        rdgCommon.ExportSettings.IgnorePaging = true;
                        rdgCommon.ExportSettings.OpenInNewWindow = true;
                        rdgCommon.ExportSettings.FileName = lblType.Text;
                        rdgCommon.MasterTableView.ExportToPdf();
                    }
                    break;
                case "application/ms.csv":
                    {
                        rdgCommon.ExportSettings.IgnorePaging = true;
                        rdgCommon.ExportSettings.OpenInNewWindow = true;
                        rdgCommon.ExportSettings.FileName = lblType.Text;
                        rdgCommon.MasterTableView.ExportToCSV();
                    }
                    break;
                case "application/ms.txt":
                    {
                        rdgCommon.ExportSettings.IgnorePaging = true;
                        rdgCommon.ExportSettings.OpenInNewWindow = true;
                        rdgCommon.ExportSettings.ExportOnlyData = true;
                        rdgCommon.ExportSettings.Csv.FileExtension = "txt";
                        rdgCommon.ExportSettings.FileName = lblType.Text;
                        rdgCommon.MasterTableView.ExportToCSV();
                    }
                    break;
            }
            cmbExport.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region ToolTip
    protected void OnAjaxUpdate(object sender, ToolTipUpdateEventArgs args)
    {
        try
        {
            HtmlTextArea text = new HtmlTextArea();
            text.Value = "Tooltip for - " + objectTypeName + "/" + args.Value;
            args.UpdatePanel.ContentTemplateContainer.Controls.Add(text);
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

    protected string GetFilterCriteriaByQueryStringForView()
    {
        string strCriteria = "";
        if (queryfilter != "")
        {
            queryfilter = queryfilter.Replace("@@", " AND ");
            strCriteria = queryfilter.Replace("@", "=");
        }
        return strCriteria;
    }



    #region GetFilterCriteriaForView()
    protected string GetFilterCriteriaForView()
    {
        string strCriteria = "";
        bool bRecentActivity = false;
        int nRecentActivityDays = 0;

        if (!String.IsNullOrEmpty(txtRecentDays.Text))
            nRecentActivityDays = Convert.ToInt32(txtRecentDays.Text);
        bRecentActivity = chkRecentActivity.Checked;

        int nSourceID = Convert.ToInt32(sourceId);

        switch (objectType)
        {
            case "CST":
                {
                    if (sourceType == "INV" && nSourceID > 0)
                    {
                        //-- get customers associated with the given ProductModel
                        strCriteria += "AND id in (" +
                                   "SELECT related_to_id " +
                                   "FROM CRM_Inventories " +
                                   "WHERE inv_product_id=" + nSourceID + ") ";
                    }
                }
                break;
            case "CNT":
                {
                    strCriteria += "AND contact_type_id = 'C' ";
                 
                    
                    
                    if (cmbSearch.SelectedItem.Text.ToLower()== "phone" && txtSearchText.Text.Length>0)
                    {
                         strCriteria += " AND (contact_phone='" + txtSearchText.Text + "' or id in(select related_to_id from crm_contact_details where related_to='CNT' and type='Phone' and value='" + txtSearchText.Text + "')) ";
                        
                    }
                    if (cmbSearch.SelectedItem.Text.ToLower() == "emailid" && txtSearchText.Text.Length > 0)
                    {
                        strCriteria += " AND (contact_emailid='" + txtSearchText.Text + "' or id in(select related_to_id from crm_contact_details where related_to='CNT' and type='Email' and value='" + txtSearchText.Text + "')) ";

                    }
                    if (sourceType == "CST" && nSourceID > 0)
                    {
                        //-- get contacts associated with the given customer
                        strCriteria += "AND related_to_id=" + nSourceID + " ";

                    }
                    if (!string.IsNullOrEmpty(Request.QueryString["CLI"]) )
                    {
                        strCriteria += "AND (";
                        strCriteria += "(contact_phone='" + CLI + "' or id in(select related_to_id from crm_contact_details where related_to='CNT' and type='Phone' and value='" + CLI + "')) ";
                        if (email!="")
                        strCriteria += "or (contact_emailid ='" + email + "' or id in (select related_to_id from crm_contact_details where related_to='CNT' and type='Email' and value='" + email + "'))";
                        strCriteria += ")";
                    }
                }
                break;
            case "INV":
                {
                    if (!string.IsNullOrEmpty(sourceType) && nSourceID > 0)
                    {
                        strCriteria += "AND related_to='" + sourceType + "' " +
                        "AND related_to_id=" + nSourceID + " ";
                    }
                }
                break;
            case "CAS":
                {
                    if (sourceType == "CST" && nSourceID > 0)
                    {
                        //-- get cases associated with the given customer
                       // strCriteria += "AND case_customer_id=" + nSourceID + " ";
                        strCriteria += "AND owner_id in (select id from crm_contacts where Useable='Y' and contact_type_id='E' and related_to_id=" + nSourceID+") ";
                    }
                    else if ((sourceType == "CNT" || sourceType == "QUE") && nSourceID > 0)
                    {
                        //-- get cases associated with the given contact
                        strCriteria += "AND (case_contact_id=" + nSourceID + " " +
                                   "OR case_caller_id=" + nSourceID + " " +
                                   "OR owner_id=" + nSourceID + ") ";
                    }
                   
                }
                break;
            case "DOC":
                {
                    if (!string.IsNullOrEmpty(sourceType) && nSourceID > 0)
                    {
                        strCriteria += "AND related_to_id=" + nSourceID + " " +
                                    "AND related_to = '" + sourceType + "'";
                    }
                }
                break;
          
            case "EML":
                {
                    if (!string.IsNullOrEmpty(sourceType) && nSourceID > 0)
                    {
                        if (sourceType == "CNT")
                        {
                            strCriteria += "AND related_to='CAS' and  related_to_id in (select ID FROM crm_cases WHERE Useable='Y' AND case_caller_id=" + nSourceID + ")";
                        }
                        else if (sourceType == "QUE")
                        {
                            strCriteria += "AND owner_id=" + nSourceID ;
                        }
                        else
                        {
                            strCriteria += "AND related_to='" + sourceType + "' " +
                                   "AND related_to_id=" + nSourceID + " ";
                        }

                    }
                }
                break;
      
           
            case "QUE":
                {
                    strCriteria += "AND contact_type_id = 'Q' ";
                   
                }
                break;
            case "USR":
                {
                    strCriteria += "AND contact_type_id = 'E' ";
                    if (sourceType == "CST" && nSourceID > 0)
                    {
                        //-- get contacts associated with the given customer
                        strCriteria += "AND related_to_id=" + nSourceID + " ";
                    }
                }
                break;
            case "CHT":
                {
                  //  strCriteria += " and chat_session_id in (select chat_session_id from crm_chat ) ";
                    if (sourceType == "USR" && nSourceID > 0)
                    {
                        strCriteria += " AND (chat_source_id=" + nSourceID + " or chat_destination_id=" + nSourceID + ")";
                    }
                    if (sourceType == "CNT" && nSourceID > 0)
                    {
                        strCriteria += " AND (related_to='CNT' and  related_to_id=" + nSourceID + ")";
                    }

                }
                break;
        }

        if (nSourceID <= 0)
        {
            if (chkMyAccount.Checked == true)
            {
                strCriteria += "AND owner_id=" + TB_nContactID + " ";
            }
            if (bRecentActivity == true && nRecentActivityDays > 0)
            {
                DateTime dtFrom = DateTime.Now.AddDays(-nRecentActivityDays);
                DateTime dtTo = DateTime.Now;
                strCriteria += "AND last_activity_date between '" + Convert.ToDateTime(dtFrom).ToString("yyyy-MM-dd hh:mm:ss") + "' AND  '" + Convert.ToDateTime(dtTo).ToString("yyyy-MM-dd hh:mm:ss") + "' ";
            }
        }
        // newly added by diwakar on 16 Jan 2013 -- Start

        if (dtCreatedDateFrom.SelectedDate.HasValue && dtCreatedDateTo.SelectedDate.HasValue)
            strCriteria += "AND created_date between '" + dtCreatedDateFrom.SelectedDate.Value.ToString("dd MMM yyyy") + " 00:00:00' AND '" + dtCreatedDateTo.SelectedDate.Value.ToString("dd MMM yyyy") + " 23:59:59' ";
        if (dtOpenDateFrom.SelectedDate.HasValue && dtOpenDateTo.SelectedDate.HasValue)
            strCriteria += "AND open_time between '" + dtOpenDateFrom.SelectedDate.Value.ToString("dd MMM yyyy") + " 00:00:00' AND '" + dtOpenDateTo.SelectedDate.Value.ToString("dd MMM yyyy") + " 23:59:59' ";
        if (dtCloseDateFrom.SelectedDate.HasValue && dtCloseDateTo.SelectedDate.HasValue)
            strCriteria += "AND end_time between '" + dtCloseDateFrom.SelectedDate.Value.ToString("dd MMM yyyy") + " 00:00:00' AND '" + dtCloseDateTo.SelectedDate.Value.ToString("dd MMM yyyy") + " 23:59:59' ";
        if (txtOwnerName.Text.Trim() != "")
            strCriteria += "AND " + m_Connection.DB_FUNCTION + "GetContactName(owner_id) like '%" + txtOwnerName.Text.Trim() + "%' ";
        if (txtStatus.Text.Trim() != "")
            strCriteria += "AND status_name = '" + txtStatus.Text.Trim() + "' ";

        // newly added by diwakar on 16 Jan 2013 -- End

        return strCriteria;
    }
    #endregion

    protected void treeView1_NodeClick(object sender, RadTreeNodeEventArgs e)
    {
        hdnCurrentViewId.Value = e.Node.Value;
        hdnCurrentViewTitle.Value = (e.Node.Text.Split('>'))[1];
        GetData(true);
        rdgCommon.Rebind();
    }
}
