using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Xml;
using System.Data.Odbc;

public partial class LookUp : ThemeBase
{
    public int oldObjectID = 0;
    int ActivityID;
    string objectType = "";
    public string objectTypeName = "";
    string objectTableName = "";

    DataSet dsDataset = null;
    GlobalWS objGlobalWS = new GlobalWS();

    protected void Page_Load(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        if (string.IsNullOrEmpty(Request.QueryString["ObjectType"]))
        {
            lblMessage.Text = "Object Type not specified for ShowList";
            return;
        }

        objectType = Request.QueryString["ObjectType"].ToString();

        if (!string.IsNullOrEmpty(Request.QueryString["SourceID"]))
            oldObjectID = Convert.ToInt32(Request.QueryString["SourceID"]);
                
        DataSet dsDataSet = objFrameworkWS.GetObjectPermissionLevel(TB_nUserRoleID, objectType);
        if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
        {
            lblMessage.Text = Convert.ToString(dsDataSet.Tables["Response"].Rows[0]["ResultString"]);
            return;
        }
        if (dsDataSet.Tables["Permisson"].Rows.Count == 0)
        {
            dsDataSet = objFrameworkWS.GetObjectPermissionLevel(0, objectType);
            if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                lblMessage.Text = Convert.ToString(dsDataSet.Tables["Response"].Rows[0]["ResultString"]);
                return;
            }
        }

        objectTypeName = dsDataSet.Tables["Permisson"].Rows[0]["object_name"].ToString();
        objectTableName = dsDataSet.Tables["Permisson"].Rows[0]["object_table_name"].ToString();
        ActivityID = Convert.ToInt32(dsDataSet.Tables["Permisson"].Rows[0]["ActivityID"]);

        btnNew.Visible = objFrameworkWS.Add;
        
        if (IsPostBack)
            return;
        if (objectTypeName.ToLower() == "customers")
        {
            btnNew.Visible = false;
        }
        btnNew.Text = "Create New " + objectTypeName + "!!";
        //--set search text and Fetch Cases
        if (!string.IsNullOrEmpty(Request.QueryString["SearchText"]))
            txtSearchText.Text = Request.QueryString["SearchText"].ToString();

        BindSearchCombo();

        // Read View Settings for the objectType (Show My only, Show Recent and select view id)
        if (GetViewSettingsForUser() == false)
            return;

        thCustomerHeader.InnerHtml = "Select " + objectTypeName;
        string[] strColumns = new string[2];
        strColumns[0] = "ID";
        strColumns[1] = "ID";
        rdgCommon.MasterTableView.DataKeyNames = strColumns;
        rdgCommon.MasterTableView.ClientDataKeyNames = strColumns;
        rdgCommon.ClientSettings.ClientEvents.OnRowClick = "GridSingleClick";
        rdgCommon.ClientSettings.ClientEvents.OnRowDblClick = "GridDoubleClick";

        if (objectType != "CNT" && objectType != "CST")
            btnNew.Visible = false;

        GetData();
    }
   
    #region GetViewSettingsForUser
    public bool GetViewSettingsForUser()
    {
        bool returnValue = false;
        try
        {
            //--get Recent Account Detail
            DataSet dsDataSet = objGlobalWS.GetViewSettingsForUser(0, objectType);
            if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(dsDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
                return returnValue;
            }
            if (dsDataSet.Tables["RecentDetails"].Rows.Count > 0)
            {
                hdnCurrentViewId.Value = "0";
                if (Convert.ToString(dsDataSet.Tables["RecentDetails"].Rows[0]["selected_view_id"]) != "0")
                {                    
                    hdnCurrentViewId.Value = Convert.ToString(dsDataSet.Tables["RecentDetails"].Rows[0]["selected_view_id"]);
                    returnValue = true;
                }                
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
        return returnValue;
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
        GetData();
        rdgCommon.Rebind();
    }
    #endregion

    #region RadGrid Column Created
    protected void rdgCommon_ColumnCreated(object sender, GridColumnCreatedEventArgs e)
    {
        try
        {
            if (e.Column.ColumnType == "GridBoundColumn" || e.Column.ColumnType == "GridDateTimeColumn")
            {
                //((GridBoundColumn)e.Column).DataFormatString = "<nobr>{0}</nobr>";
                if (((GridBoundColumn)e.Column).UniqueName == "ID")
                {
                    ((GridBoundColumn)e.Column).Visible = false;
                }
                else
                {
                    //--set col. width defined in TableDefinition/CustomColumns
                    DataTable dtTable = (DataTable)ViewState["TableDefinition"];
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
        rdgCommon.DataSource = (DataTable)ViewState["TableData"];
    }
 
    #region Get Data
    private void GetData()
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
                szSearchColumnName = cmbSearch.SelectedValue;
                szSearchColumnText = txtSearchText.Text;
            }

            string strCriteria = GetFilterCriteriaForView();

            //--call method            
            dsDataset = objGlobalWS.FetchCommonData(szSearchColumnName, szSearchColumnText, 0,
                                                           0, "", false, 0,
                                                           0, nViewID, false,
                                                           TB_nTimeZoneTimeSpan, objectType, strCriteria, Convert.ToBoolean(Session["ShowDial"]), objectTableName);

            if (Convert.ToInt32(dsDataset.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(dsDataset.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }
            ViewState["TableData"] = dsDataset.Tables["Common"];
            ViewState["TableDefinition"] = dsDataset.Tables["TableDefinition"];

            //--show/hide paging panel
            if (dsDataset.Tables["Common"].Rows.Count > rdgCommon.PageSize)
                rdgCommon.MasterTableView.PagerStyle.AlwaysVisible = true;
            else
                rdgCommon.MasterTableView.PagerStyle.AlwaysVisible = false;
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region GetFilterCriteriaForView()
    protected string GetFilterCriteriaForView()
    {
        string strCriteria = "";
        switch (objectType)
        {
            case "CST":
                {
                }
                break;
            case "CNT":
                {
                    strCriteria += "AND contact_type_id <> 'Q' ";
                }
                break;
            case "INV":
                {
                }
                break;
            case "CAS":
                {
                }
                break;
            case "DOC":
                {
                }
                break;
            case "TSK":
                {
                }
                break;
            case "EML":
                {
                }
                break;
            case "MLB":
                {
                }
                break;
            case "MAP":
                {
                }
                break;
            case "QUE":
                {
                    strCriteria += "AND (contact_type_id = 'Q' OR (contact_type_id <> 'Q' AND isnull(contact_login_id,'') <> ''))";
                }
                break;
            case "USR":
                {
                    strCriteria += "AND contact_type_id = 'E' ";
                }
                break;
        }
        return strCriteria;
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

    protected void btnNew_Click(object sender, EventArgs e)
    {
        if (txtSearchText.Text.Trim().Length == 0)
        {
            lblMessage.Text = "Blank Value";
            return;
        }
        DataBase m_Connection = new DataBase();
        try
        {
            if (objectType == "CST")
            {
                m_Connection.OpenDB("Galaxy");

                XmlDocument xMainDoc = m_Connection.createParameterXML();
                m_Connection.fillParameterXML(ref xMainDoc, "cust_name", txtSearchText.Text, "varchar", "100");

                m_Connection.BeginTransaction();

                DataTable dt = m_Connection.SaveTransactionData("CST", 0, "Y", DateTime.UtcNow, Convert.ToInt32(Session["contact_id"]), Request.UserHostAddress, xMainDoc);
                if (Convert.ToInt32(dt.Rows[0]["ResultCode"]) >= 0)
                {
                    int CustomerID = Convert.ToInt32(dt.Rows[0]["ResultCode"]);
                    dt = m_Connection.SaveRecentActivity(0, "CST", CustomerID, "Created", txtSearchText.Text, Convert.ToInt32(Session["contact_id"]), "P", 2401,"");
                    m_Connection.Commit();
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", "javascript:CloseWindow(" + CustomerID + ");", true);
                }
            }
            else if (objectType == "CNT")
            {
                m_Connection.OpenDB("Galaxy");

                XmlDocument xMainDoc = m_Connection.createParameterXML();
                m_Connection.fillParameterXML(ref xMainDoc, "contact_full_name", txtSearchText.Text, "varchar", "100");

                m_Connection.BeginTransaction();

                DataTable dt = m_Connection.SaveTransactionData("CNT", 0, "Y", DateTime.UtcNow, Convert.ToInt32(Session["contact_id"]), Request.UserHostAddress, xMainDoc);
                if (Convert.ToInt32(dt.Rows[0]["ResultCode"]) >= 0)
                {
                    int ContactID = Convert.ToInt32(dt.Rows[0]["ResultCode"]);
                    dt = m_Connection.SaveRecentActivity(0, "CNT", ContactID, "Created", txtSearchText.Text, Convert.ToInt32(Session["contact_id"]), "P", 2403,"");
                    m_Connection.Commit();
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", "javascript:CloseWindow(" + ContactID + ");", true);
                }
            }
        }
        catch (OdbcException ex)
        {
            lblMessage.Text = ex.Message;
            m_Connection.Rollback();
        }
        catch (Exception ex)
        {
            lblMessage.Text = ex.Message;
            m_Connection.Rollback();
        }
        finally
        {
            m_Connection.CloseDB();
        }
    }

}
