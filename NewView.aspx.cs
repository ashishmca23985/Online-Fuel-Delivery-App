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
using Telerik.Web.UI;

public partial class NewView : ThemeBase
{
    GlobalWS objGlobalWS = new GlobalWS();   
    string strRelatedTo = string.Empty;
    int nViewID = 0;
    string strRelatedToName = string.Empty;
    string strType = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["ViewID"] != null)
            nViewID = Convert.ToInt32(Request.QueryString["ViewID"]);
      //  else
       //   nViewID = Convert.ToInt32(cmbView.SelectedValue == "" ? "0" : cmbView.SelectedValue);

        if (Request.QueryString["RelatedTo"] != null)
            strRelatedTo = Convert.ToString(Request.QueryString["RelatedTo"]);
        else
            strRelatedTo = cmbRelatedto.SelectedValue == "" ? "0" : cmbRelatedto.SelectedValue;
        if (Request.QueryString["RelatedToName"] != null)
           strRelatedToName = Convert.ToString(Request.QueryString["RelatedToName"]);

        FillOperaotrInViewState();
        if (Page.IsPostBack)
            return;
        roletype();
        GetRelatedTo();
        
        if (Request.QueryString["ViewID"] != null && Request.QueryString["RelatedTo"] != null)
        {            
            GetViewDetails();
        }
        
    }

    #region FillTableColumns
    private void FillOperaotrInViewState()
    {
        try
        {
            DataSet objDataSet = objGlobalWS.FetchGeneralValues("OPERATOR");
            if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(objDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }
            ViewState["OperatorDT"] = objDataSet.Tables[0];
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region rdgTableDifinedColumns_ItemDataBound
    protected void rdgTableDifinedColumns_ItemDataBound(object sender, GridItemEventArgs e)
    {
        try
        {
            if (!(e.Item is GridEditFormItem))
            {
                if ((e.Item is GridItem) && e.Item.IsDataBound)
                {
                    DataRowView row = (DataRowView)e.Item.DataItem;
                    if (row["column_selected"].ToString() == "true")
                        ((CheckBox)e.Item.FindControl("chkSelect")).Checked = true;
                }
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region Criteria RadGrid Item DataBound
    protected void rdgCriteria_ItemDataBound(object sender, GridItemEventArgs e)
    {
        //--fill Columns and Operators
        try
        {
            
            if ((e.Item is GridItem) && e.Item.IsDataBound)
            {
                DataTable dtColumns = (DataTable)ViewState["ViewColumns"];
                DataTable dtOperator = (DataTable)ViewState["OperatorDT"];
                //-set SNo
                ((Label)e.Item.FindControl("lblSNo")).Text = (e.Item.ItemIndex + 1).ToString();

                RadComboBox cmbColumn = (RadComboBox)e.Item.FindControl("edit_ColumnName");
                cmbColumn.DataSource = dtColumns;
                cmbColumn.DataTextField = "column_name";
                cmbColumn.DataValueField = "column_expression";
                cmbColumn.DataBind();

                RadComboBox cmbOperator = (RadComboBox)e.Item.FindControl("edit_Operator");
                cmbOperator.DataSource = dtOperator;
                cmbOperator.DataTextField = "name";
                cmbOperator.DataValueField = "code";
                cmbOperator.DataBind();

                RadComboBoxItem rdItem = new RadComboBoxItem("", "");
                cmbColumn.Items.Insert(0, rdItem);
                rdItem = new RadComboBoxItem("", "");
                cmbOperator.Items.Insert(0, rdItem);

                cmbColumn.SelectedValue = ((Label)e.Item.FindControl("lblColName")).Text;
                cmbOperator.SelectedValue = ((Label)e.Item.FindControl("lblOperator")).Text;

                RadComboBox cmbSeparator = (RadComboBox)e.Item.FindControl("edit_Separator");
                cmbSeparator.SelectedValue = ((Label)e.Item.FindControl("lblSeparator")).Text;
            }
            else if ((e.Item is GridFooterItem))
            {
                DataTable dtColumns = (DataTable)ViewState["ViewColumns"];
                DataTable dtOperator = (DataTable)ViewState["OperatorDT"];

                RadComboBox cmbColumn = (RadComboBox)e.Item.FindControl("add_ColumnName");
                cmbColumn.DataSource = dtColumns;
                cmbColumn.DataTextField = "column_name";
                cmbColumn.DataValueField = "column_expression";
                cmbColumn.DataBind();

                RadComboBox cmbOperator = (RadComboBox)e.Item.FindControl("add_Operator");
                cmbOperator.DataSource = dtOperator;
                cmbOperator.DataTextField = "name";
                cmbOperator.DataValueField = "code";
                cmbOperator.DataBind();

                RadComboBoxItem rdItem = new RadComboBoxItem("", "");
                cmbColumn.Items.Insert(0, rdItem);
                rdItem = new RadComboBoxItem("", "");
                cmbOperator.Items.Insert(0, rdItem);
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region Criteria RadGrid ItemCommand
    protected void rdgCriteria_ItemCommand(object source, GridCommandEventArgs e)
    {
        try
        {
            using (DataTable dtCriteria = (DataTable)ViewState["FilterDetail"])
            {
                if (e.CommandName == "Delete")
                {
                    dtCriteria.Rows[e.Item.ItemIndex].Delete();
                }
                else if (e.CommandName == "Add")
                {
                    if (((RadComboBox)e.Item.FindControl("add_ColumnName")).SelectedValue == string.Empty)
                    {
                        LogMessage("Please select Column to add in criteria !", 2);
                        return;
                    }
                    if (((RadComboBox)e.Item.FindControl("add_Operator")).SelectedValue == string.Empty)
                    {
                        LogMessage("Please select Operator to add in criteria !", 2);
                        return;
                    }
                    DataRow dr = dtCriteria.NewRow();
                    dr[0] = ((RadComboBox)e.Item.FindControl("add_ColumnName")).SelectedValue;
                    dr[1] = ((RadComboBox)e.Item.FindControl("add_ColumnName")).SelectedItem.Text;
                    dr[2] = ((RadComboBox)e.Item.FindControl("add_Operator")).SelectedValue;
                    dr[3] = ((RadComboBox)e.Item.FindControl("add_Operator")).SelectedItem.Text;
                    dr[4] = ((TextBox)e.Item.FindControl("add_FilterValue")).Text;
                    dr[5] = ((RadComboBox)e.Item.FindControl("add_Separator")).SelectedValue;

                    dtCriteria.Rows.Add(dr);
                }
                rdgCriteria.DataSource = dtCriteria;
                rdgCriteria.DataBind();
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region Get View Details and Populate controls
    private void GetViewDetails()
    {
        try
        {
            DataSet objDataSet = objGlobalWS.FetchViewDetails(nViewID, strRelatedTo);

            if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(objDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }
            if (objDataSet.Tables["ViewDetail"].Rows.Count > 0)
            {
                lblCaption.Text = "Editing View [" + Convert.ToString(objDataSet.Tables["ViewDetail"].Rows[0]["view_name"]) + "]";

                txtCreatedAt.Text = Convert.ToString(objDataSet.Tables["ViewDetail"].Rows[0]["CreatedDate"]);
                txtCreatedBy.Text = Convert.ToString(objDataSet.Tables["ViewDetail"].Rows[0]["CreatedBy"]);

                txtViewName.Text = Convert.ToString(objDataSet.Tables["ViewDetail"].Rows[0]["view_name"]);
                txtViewDescription.Text = Convert.ToString(objDataSet.Tables["ViewDetail"].Rows[0]["view_description"]);

                cmbPageSize.SelectedValue = Convert.ToString(objDataSet.Tables["ViewDetail"].Rows[0]["view_pagesize"]);
                txtCiteria.Text = Convert.ToString(objDataSet.Tables["ViewDetail"].Rows[0]["view_filter_condition"]);

                txtOredrByCriteria.Text = Convert.ToString(objDataSet.Tables["ViewDetail"].Rows[0]["view_order_by_expression"]);

                //
                if (txtOredrByCriteria.Text.Length > 0)
                {
                    string[] words = txtOredrByCriteria.Text.ToLower().Replace("order by", "").Replace(',', '|').Replace(" ", "").Split('|');
                    foreach (string word in words)
                    {
                        if (ChkId.Text.ToLower().Replace(" ", "").ToString() == word.ToLower().Replace("asc", "").Replace("desc", "").Replace("_", "").ToString())
                        {
                            ChkId.Checked = true;
                            radbutid.SelectedValue = word.ToLower().Replace("asc", "").Replace("desc", "").ToString() + "|" + word.ToLower().Replace(word.ToLower().Replace("asc", "").Replace("desc", ""), "");
                        }
                        else if (ChkCreatedDate.Text.ToLower().Replace(" ", "").ToString() == word.ToLower().Replace("asc", "").Replace("desc", "").Replace("_", "").ToString())
                        {
                            ChkCreatedDate.Checked = true;
                            radbutcreated_date.SelectedValue = word.ToLower().Replace("asc", "").Replace("desc", "").ToString() + "|" + word.ToLower().Replace(word.ToLower().Replace("asc", "").Replace("desc", ""), "");
                        }
                        else if (ChkModifiedDate.Text.ToLower().Replace(" ", "").ToString() == word.ToLower().Replace("asc", "").Replace("desc", "").Replace("_", "").ToString())
                        {
                            ChkModifiedDate.Checked = true;
                            radbutmodified_date.SelectedValue = word.ToLower().Replace("asc", "").Replace("desc", "").ToString() + "|" + word.ToLower().Replace(word.ToLower().Replace("asc", "").Replace("desc", ""), "");
                        }
                        else if (ChkOwnerId.Text.ToLower().Replace(" ", "").ToString() == word.ToLower().Replace("asc", "").Replace("desc", "").Replace("_", "").ToString())
                        {
                            ChkOwnerId.Checked = true;
                            radbutowner_id.SelectedValue = word.ToLower().Replace("asc", "").Replace("desc", "").ToString() + "|" + word.ToLower().Replace(word.ToLower().Replace("asc", "").Replace("desc", ""), "");
                        }
                    }
                }
                //


                if (Convert.ToString(objDataSet.Tables["ViewDetail"].Rows[0]["view_custom_filter"]) == "Y")
                {
                    chkCustomFilter.Checked = true;
                    trCustomFilter.Attributes.Remove("style");
                    trCustomFilter.Attributes.Add("style", "display:block;");
                    trFilterCriteria.Attributes.Remove("style");
                    trFilterCriteria.Attributes.Add("style", "display:none;");
                }
                else
                {
                    chkCustomFilter.Checked = false;
                    trCustomFilter.Attributes.Remove("style");
                    trCustomFilter.Attributes.Add("style", "display:none;");
                    trFilterCriteria.Attributes.Remove("style");
                    trFilterCriteria.Attributes.Add("style", "display:block;");
                }

                if (Convert.ToString(objDataSet.Tables["ViewDetail"].Rows[0]["view_private"]) == "Y")
                    chkPrivate.Checked = true;
                else
                {
                    if (hdnroletype.Value == "1")
                    {
                        chkPrivate.Checked = false;
                    }
                }

                //--disable save for system view
                if (Convert.ToString(objDataSet.Tables["ViewDetail"].Rows[0]["view_default"]) == "Y")
                {
                    if (hdnroletype.Value != "1")
                    {
                        btnSave.Enabled = false;
                        btnSaveNew.Enabled = false;
                    }

                }
                else {
                    btnSave.Enabled = true;
                    btnSaveNew.Enabled = true;
                    
                }
            }
            else
            {
                lblCaption.Text = "Create New View [ " + strRelatedToName + " ]";
                refrash();
            }
            ViewState["ViewColumns"] = objDataSet.Tables["ViewColumns"];
            ViewState["FilterDetail"] = objDataSet.Tables["FilterDetail"];
            DataTable datatableDestination = new DataTable();
            datatableDestination.Columns.Add("column_name");
            datatableDestination.Columns.Add("tabledef_id");
            DataTable datatableSource = new DataTable();
            datatableSource.Columns.Add("column_name");
            datatableSource.Columns.Add("tabledef_id");
           
            foreach (DataRow dr in objDataSet.Tables["ViewColumns"].Rows)
            {
                if (dr["column_selected"].ToString() == "true")
                {
                    datatableDestination.Rows.Add(dr["column_name"], dr["tabledef_id"]);
                 
                }
                else if (dr["column_selected"].ToString() == "false")
                {
                    datatableSource.Rows.Add(dr["column_name"], dr["tabledef_id"]);
                 
                }
            }
            RadListBoxDestination.DataSource = datatableDestination;
            RadListBoxDestination.DataTextField = "column_name";
            RadListBoxDestination.DataValueField = "tabledef_id";
            RadListBoxDestination.DataBind();

            RadListBoxSource.DataSource = datatableSource;
            RadListBoxSource.DataTextField = "column_name";
            RadListBoxSource.DataValueField = "tabledef_id";
            RadListBoxSource.DataBind();
           
            rdgCriteria.DataSource = objDataSet.Tables["FilterDetail"];
            rdgCriteria.DataBind();
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region SaveView
    protected void Save_Click(object sender, EventArgs e)
    {
        SaveView("N");
    }

    protected void btnSaveNew_Click(object sender, EventArgs e)
    {
        SaveView("Y");
    } 
    
    private void SaveView(string strCreateNew)
    {
        string strViewDesc = string.Empty;
        string strViewName = string.Empty;
        string strPrivate = string.Empty;
        string strFilterCondition = string.Empty;
        string strColumnType = string.Empty;
        string strCustomFilter = string.Empty;

        string[] arrTableDefIDs = null;
        string[] arrSequence = null;
        string[] arrFilterColumn = new string[rdgCriteria.Items.Count];
        string[] arrFilterOperator = new string[rdgCriteria.Items.Count];
        string[] arrFilterValue = new string[rdgCriteria.Items.Count];
        string[] arrFilterSeparator = new string[rdgCriteria.Items.Count];

        int nCount = 0;
        int nColumnCount = 0;
        int nPageSize = 0;
        DataTable dtColumns;
        lblMessage.Text="";

        try
        {
            if ((RadListBoxDestination.Items.Count > 25) ||    (RadListBoxDestination.Items.Count==0))             
                lblMessage.Text = "You can select maximum 25 columns in a view!";
            else
            {
                dtColumns = (DataTable)ViewState["ViewColumns"];
                strViewDesc = txtViewDescription.Text.Trim();
                strViewName = txtViewName.Text.Trim();

                if (cmbPageSize.SelectedValue != string.Empty)
                    nPageSize = Convert.ToInt32(cmbPageSize.SelectedValue);
                else
                    nPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultPageSize"]);

                if (chkPrivate.Checked == true)
                    strPrivate = "Y";
                else
                    strPrivate = "N";

                if (chkCustomFilter.Checked == true)
                {
                    strFilterCondition = txtCiteria.Text.Trim();
                    strCustomFilter = "Y";
                    if (strFilterCondition == string.Empty)
                    {
                        LogMessage("Please enter custom filter criteria !", 2);
                        txtCiteria.Focus();
                        return;
                    }
                }
                else
                {
                    strFilterCondition = "";
                    strCustomFilter = "N";
                }

                //--validations
                if (strViewName == string.Empty)
                {
                    LogMessage("Please enter View Name !", 2);
                    return;
                }
                string szValue = "";
                RadComboBox rdbCombo;
                TextBox txtText;
                for (int i = 0; i < rdgCriteria.Items.Count; i++)
                {
                    rdbCombo = ((RadComboBox)rdgCriteria.Items[i].FindControl("edit_ColumnName"));
                    arrFilterColumn[i] = rdbCombo.SelectedValue.Split('|')[0];
                    rdbCombo = ((RadComboBox)rdgCriteria.Items[i].FindControl("edit_Operator"));
                    arrFilterOperator[i] = rdbCombo.SelectedValue;
                    txtText = ((TextBox)rdgCriteria.Items[i].FindControl("edit_FilterValue"));
                    arrFilterValue[i] = txtText.Text;
                    arrFilterSeparator[i] = "";

                    if (arrFilterColumn[i] == string.Empty)
                        continue;

                    if (arrFilterOperator[i] == string.Empty)
                    {
                        LogMessage("Please select Operator in Filter Criteria " + i + 1 + " !", 2);
                        return;
                    }
                    if (arrFilterValue[i] == string.Empty)
                    {
                        LogMessage("Please enter some Value in Filter Criteria " + i + 1 + " !", 2);
                        return;
                    }
                    rdbCombo = ((RadComboBox)rdgCriteria.Items[i].FindControl("edit_ColumnName"));
                    strColumnType = dtColumns.Rows[rdbCombo.SelectedIndex - 1]["column_type"].ToString();

                    switch (arrFilterOperator[i])
                    {
                        case "in":
                        case "not in":
                            {
                                szValue = "('";
                                szValue += arrFilterValue[i].Replace(",", "','");
                                szValue += "') ";
                                break;
                            }
                        case "like":
                        case "not like":
                            {
                                szValue = "'%" + arrFilterValue[i] + "%' ";
                                break;
                            }
                        default:
                            {
                                if (strColumnType == "D" || strColumnType == "N")
                                    szValue = arrFilterValue[i];
                                else
                                    szValue = "'" + arrFilterValue[i] + "' ";
                                break;
                            }
                    }
                    if (i > 0)
                    {
                        rdbCombo = ((RadComboBox)rdgCriteria.Items[i - 1].FindControl("edit_Separator"));
                        arrFilterSeparator[i - 1] = rdbCombo.SelectedValue;
                    }
                    if (strCustomFilter == "N")
                    {
                        if (strFilterCondition == string.Empty)
                            strFilterCondition = "AND ";

                        if (i > 0)
                            strFilterCondition += arrFilterSeparator[i - 1] + " ";

                        strFilterCondition += arrFilterColumn[i] + " " + arrFilterOperator[i] + " " + szValue + " ";
                    }
                }
                  arrTableDefIDs = new string[RadListBoxDestination.Items.Count];
                  arrSequence = new string[RadListBoxDestination.Items.Count];
                // heerak
                //for (int i = 0; i < nColumnCount; i++)
                // {
                //     if (((CheckBox)rdgTableDifinedColumns.Items[i].FindControl("chkSelect")).Checked == true)
                //     {

                //     arrTableDefIDs[nCount] = Convert.ToString(rdgTableDifinedColumns.MasterTableView.DataKeyValues[i]["tabledef_id"]);
                //         nCount++;
                //     }
                // }

                foreach (RadListBoxItem currentItem in RadListBoxDestination.Items)
                {
                    arrTableDefIDs[nCount] = currentItem.Value;
                    arrSequence[nCount] = (nCount + 1).ToString();
                    nCount++;
                }
           
                
                    txtOredrByCriteria.Text = "";
                    int i1 = txtOredrByCriteria.Text.Length;
                    if (ChkId.Checked==true)
                    {
                        txtOredrByCriteria.Text += (txtOredrByCriteria.Text.Length <= 0) ? radbutid.SelectedValue.Replace("|", " ").ToString() : "," + radbutid.SelectedValue.Replace("|", " ").ToString();
                    }
                    if (ChkCreatedDate.Checked == true)
                    {
                        txtOredrByCriteria.Text += txtOredrByCriteria.Text.Length <= 0 ? radbutcreated_date.SelectedValue.Replace("|", " ").ToString() : " ," + radbutcreated_date.SelectedValue.Replace("|", " ").ToString();
                       
                    }
                    if (ChkModifiedDate.Checked == true)
                    {
                        txtOredrByCriteria.Text += txtOredrByCriteria.Text.Length <= 0 ? radbutmodified_date.SelectedValue.Replace("|", " ").ToString() : " , " + radbutmodified_date.SelectedValue.Replace("|", " ").ToString();
                        
                    }
                     if (ChkOwnerId.Checked == true)
                    {
                        txtOredrByCriteria.Text += txtOredrByCriteria.Text.Length <= 0 ? radbutowner_id.SelectedValue.Replace("|", " ").ToString() : " , " + radbutowner_id.SelectedValue.Replace("|", " ").ToString();
                      
                    }
                    if (txtOredrByCriteria.Text.Length > 0)
                        txtOredrByCriteria.Text = "Order by " + txtOredrByCriteria.Text;
                    else
                        txtOredrByCriteria.Text = "";

                    string strOrderByCriteria = txtOredrByCriteria.Text;


                //DataSet dsDataSet = objGlobalWS.SaveView(TB_nSessionID, nViewID, strViewDesc,
                //                                        strViewName, strRelatedTo, nPageSize,
                //                                        strPrivate, strFilterCondition,
                //                                        arrTableDefIDs, arrFilterColumn, arrFilterOperator,
                //                                        arrFilterValue, arrFilterSeparator, strCustomFilter, strOrderByCriteria,
                //                                        TB_nContactID);

               DataSet dsDataSet = objGlobalWS.SaveView(TB_nSessionID, int.Parse(lbview.SelectedValue.ToString()), strViewDesc,
                                                       strViewName, strRelatedTo, nPageSize,
                                                       strPrivate, strFilterCondition,
                                                       arrTableDefIDs, arrFilterColumn, arrFilterOperator,
                                                      arrFilterValue, arrFilterSeparator, strCustomFilter, strOrderByCriteria,
                                                      TB_nContactID, arrSequence);


                if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
                {
                    LogMessage(Convert.ToString(dsDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
                    return;
                }
                LogMessage("View Saved Successfully!!!", 0);
                if (strCreateNew == "Y")
                {
                    string strTemp = "NewView.aspx?RelatedTo=" + strRelatedTo + "&RelatedToName=" + strRelatedToName;
                    Response.Redirect(strTemp);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", "javascript:CloseWindow();", true);
                }
                if (lbview.SelectedIndex == 0)
                {
                    getview("N");
                    lbview.SelectedValue = lbview.Items[lbview.Items.Count - 1].Value;
                }
                
           }
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

    #region Get RelatedTo
    protected void GetRelatedTo()
    {
        try
        {
            DataSet dsDataSet = objGlobalWS.GetObjectTableNames();

            if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(dsDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }
            cmbRelatedto.DataSource = dsDataSet.Tables["TableFor"];
            cmbRelatedto.DataValueField = "object_code";
            cmbRelatedto.DataTextField = "object_name";
            cmbRelatedto.DataBind();

            RadComboBoxItem lstItem = new RadComboBoxItem("--Select--", "0");
            cmbRelatedto.Items.Insert(0, lstItem);
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    
    protected void cmbRelatedto_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
    {
         if (cmbRelatedto.SelectedIndex != -1)
            {
                tbview.Visible = true;
                getview("Y");
                tddetail.Visible = false;
         }
    }
    void getview(string SourceListbox)
    {
        try
        {
                           strRelatedTo = cmbRelatedto.SelectedValue;
                DataSet objDataSet = objGlobalWS.FetchViewName(cmbRelatedto.SelectedValue);
                if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
                {
                    LogMessage(Convert.ToString(objDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
                    return;
                }
                if (objDataSet.Tables["viewname"].Rows.Count > 0)
                {
                    lbview.DataSource = objDataSet.Tables["viewname"];
                    lbview.DataTextField = "view_name";
                    lbview.DataValueField = "view_id";
                    lbview.DataBind();
                    RadListBoxItem lstitem = new RadListBoxItem("New View ", "0");
                    lbview.Items.Insert(0, lstitem);

                }
                if (SourceListbox == "Y")
                {
                    RadListBoxSource.DataSource = objDataSet.Tables["FetchViewColumn"];
                    RadListBoxSource.DataTextField = "column_name";
                    RadListBoxSource.DataValueField = "tabledef_id";
                    RadListBoxSource.DataBind();
                }

        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }

    protected void lbview_SelectedIndexChanged(object o, EventArgs e)
    {

        try
        {
            refrash();
            tddetail.Visible = true;
            strRelatedTo = cmbRelatedto.SelectedValue;
            nViewID = int.Parse(lbview.SelectedValue.ToString());
           
            GetViewDetails();
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }

    }

    #region role
    protected void roletype()
    {
        try
        {
           DataSet dsDataSet = objGlobalWS.roletype(TB_nUserRoleID);
            

             if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(dsDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }
             if (dsDataSet.Tables["RoleType"].Rows[0][0].ToString().Length <= 0)
             {
                 chkPrivate.Checked = true;
                 chkPrivate.Enabled = false;
                 hdnroletype.Value = "0";
             }

        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion
    void refrash()
    { 
        txtCreatedAt.Text = "";
        txtCreatedBy.Text ="";
        txtViewName.Text ="";
        txtViewDescription.Text = "";
        cmbPageSize.SelectedValue ="10";
        txtCiteria.Text ="";
        txtOredrByCriteria.Text = "";
        chkCustomFilter.Checked = false;
        trCustomFilter.Attributes.Remove("style");
        trCustomFilter.Attributes.Add("style", "display:none;");
        trFilterCriteria.Attributes.Remove("style");
        trFilterCriteria.Attributes.Add("style", "display:block;");
        if (hdnroletype.Value != "1")
        {
            chkPrivate.Checked = true;
         chkPrivate.Enabled = false;
        }
        else
            chkPrivate.Enabled = true;
            ChkId.Checked=false;
            ChkCreatedDate.Checked=false;
            ChkModifiedDate.Checked=false;
            ChkOwnerId.Checked = false;
                
    }

}
