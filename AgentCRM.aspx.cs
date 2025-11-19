using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Data.Odbc;
using CtiWS;

public partial class AgentCRM : ThemeBase
{
    public string leadid = "";
    public string serviceid = "";
    public string cli = "";
    public string callnumber = "";
    public string agentid = "";
    public string calltype = "";
    public string maxcallbackday = "30";
    public string serviceName = "";
    public string strHostID = "";

    DataBase con = null;
    public string readtable = "";
    public string writetable = "";
    public string actiontype = "insert";
    public string servicedb = "";
    int column_count = 1;
    public string column_width = "50%";
    CtiWS.CtiWS cti = new CtiWS.CtiWS();
    bo_CTIHistory CTIHistoryDetails = new bo_CTIHistory();
    bal_CTIHistory CtiHistoryDetailBal = new bal_CTIHistory();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
            return;
        // take values from querystring
        if (Request.QueryString["LEADID"] != null)
            leadid = Request.QueryString["LEADID"];
        if (Request.QueryString["SERVICEID"] != null)
            serviceid = Request.QueryString["SERVICEID"];
        if (Request.QueryString["CLI"] != null)
            cli = Request.QueryString["CLI"];
        if (Request.QueryString["CALLNUMBER"] != null)
            callnumber = Request.QueryString["CALLNUMBER"];
        //if (Request.QueryString["AGENT"] != null)
        //    agentid = Request.QueryString["AGENTID"];
        if (Request.QueryString["CALLTYPE"] != null)
            calltype = Request.QueryString["CALLTYPE"];
        if (Request.QueryString["CALLSERVICENAME"] != null)
            serviceName = Request.QueryString["CALLSERVICENAME"];

        if (HttpContext.Current.Session["LOGINID"] != null)
            agentid = HttpContext.Current.Session["LOGINID"].ToString();
        else if (Request.QueryString["AGENT"] != null)
            agentid = Request.QueryString["AGENT"].ToString();

        if (Session["HOSTID"] != null)
        {
            strHostID = Session["HOSTID"].ToString();
            hdnHostId.Value = strHostID;
        }
        else
        {
            strHostID = HttpContext.Current.Request.UserHostAddress;
            hdnHostId.Value = strHostID;
        }
        Session["CallNumber"] = callnumber.ToString();
        //lblmessage.Text = "LeadId : " + leadid + " & " + "ServiceId : " + serviceid + " & " + "CLI : " + cli + " & " + "CallNumber : " + callnumber + " & " + "Calltype : " + calltype + " & " + "ServiceName : " + serviceName + " & " + "AgentName : " + agentid;
        //lblmessage.BackColor = System.Drawing.Color.Yellow;
        con = new DataBase();
        con.OpenDB("IDGDB");

        // read cti_services table
        DataTable dtService = GetService();
        string serviceheader = dtService.Rows[0]["service_name"].ToString();
        if (serviceheader.Length > 0)
        {
            crmheader.InnerHtml = serviceheader;
        }
        if (dtService.Rows.Count == 0)
        {
            if (crmheader.InnerHtml.Length == 0)
                crmheader.InnerHtml = "No campaign for serviceid-" + serviceid;
            return;
        }
        if (serviceName.Length == 0)
        {
            serviceName = dtService.Rows[0]["service_name"].ToString();
        }
        readtable = dtService.Rows[0]["service_leadstructure_master_tablename"].ToString();
        if (readtable.Length == 0)
        {
            crmheader.InnerHtml = "No service table for serviceid-" + serviceid;
            return;
        }

        servicedb = dtService.Rows[0]["service_outbound_lead_db_name"].ToString();
        if (servicedb.Length == 0)
        {
            crmheader.InnerHtml = "No service DB key-" + serviceid;
            return;
        }
        writetable = dtService.Rows[0]["service_leadstructure_details_tablename"].ToString();
        if (writetable.Length == 0 || readtable == writetable)
        {
            writetable = readtable;
            actiontype = "update";
        }
        writetable = servicedb + con.DB_SEPERATOR + writetable;
        readtable = servicedb + con.DB_SEPERATOR + readtable;

        // read data fields
        DataTable dtFields = GetDataFields();
        if (dtFields.Rows.Count == 0)
        {
            if (crmheader.InnerHtml.Length == 0)
                crmheader.InnerHtml = "No data fields for serviceid-" + serviceid;
            return;
        }
        // read data from datatable
        DataTable dtData = GetCampaignData(dtFields);
        if (dtData.Rows.Count == 0)
        {
            if (crmheader.InnerHtml.Length == 0)
                crmheader.InnerHtml = "No data for leadid-" + leadid;
            //return;
        }
        //string serviceheader = dtService.Rows[0]["service_remarks"].ToString();
        //if (serviceheader.Length > 0)
        //{
        //    crmheader.InnerHtml = serviceheader;
        //}

        CreateCRM(dtFields, dtData);
        get_udf_callmaster_table();

    }

    protected DataTable GetService()
    {
        DataTable dtService = new DataTable("SERVICE");
        try
        {
            string strsql = "SELECT " + con.DB_TOP_SQL +
                          " service_name" +
                          ",service_leadstructure_master_tablename" +
                          ",service_leadstructure_details_tablename" +
                          ",service_remarks" +
                          ",service_outbound_lead_db_name " +
                          ",2 as service_max_callback_day " +
                          "FROM cti_services " +
                          "WHERE service_id=" + serviceid + " " + con.DB_TOP_MYSQL;
            OdbcDataAdapter ad = new OdbcDataAdapter(strsql, con.oCon);
            ad.Fill(dtService);
        }
        catch (OdbcException ex)
        {
            crmheader.InnerHtml = "GetService()-" + ex.Message;
        }
        return dtService;
    }

    protected DataTable GetDataFields()
    {
        DataTable dtFields = new DataTable("FIELDS");
        try
        {

            string strsql = "SELECT " +
                         "field_id,field_name,field_display_name,field_data_type ," +
                          con.DB_NULL + "(field_data_min,0) as field_data_min," +
                          con.DB_NULL + "(field_data_max,0) as field_data_max," +
                         "field_compulsory,field_readonly,field_import,field_system,field_data_capture,field_dropdown_lookup,field_script ," +
                          con.DB_NULL + "(field_row,0) as field_row," +
                          con.DB_NULL + "(field_col,0) as field_col " +
                        "FROM cti_service_fields " +
                        "WHERE field_row > 0 and field_col > 0 and field_service_id=" + serviceid + " " +
                        "ORDER BY field_row,field_col,field_id";

            OdbcDataAdapter ad = new OdbcDataAdapter(strsql, con.oCon);
            ad.Fill(dtFields);
        }
        catch (OdbcException ex)
        {
            crmheader.InnerHtml = "GetDataFields()-" + ex.Message;
        }
        return dtFields;
    }

    protected DataTable GetCampaignData(DataTable dtFields)
    {
        DataTable dtData = new DataTable("DATA");
        try
        {
            string strsql = "SELECT " + con.DB_TOP_SQL + " lead_id";
            foreach (DataRow row in dtFields.Rows)
            {
                int nCols = Convert.ToInt32(row["field_col"]);
                if (nCols > column_count)
                    column_count = nCols;
                if (row["field_import"].ToString() == "Y" || row["field_system"].ToString() == "Y")
                    strsql += "," + row["field_name"].ToString();
                if (row["field_data_type"].ToString() == "GroupBox" || row["field_data_type"].ToString() == "URL")
                {
                    row["field_data_capture"] = "N";
                    row["field_compulsory"] = "N";
                    row["field_readonly"] = "N";
                }
            }
            strsql += " FROM " + readtable + " WHERE lead_id=" + leadid + " " + con.DB_TOP_MYSQL;

            OdbcDataAdapter ad = new OdbcDataAdapter(strsql, con.oCon);
            ad.Fill(dtData);
            if (column_count == 2)
                column_width = "25%";
            else if (column_count == 3)
                column_width = "18%";
        }
        catch (OdbcException ex)
        {
            crmheader.InnerHtml = "GetCampaignData()-" + ex.Message;
        }
        return dtData;
    }

    protected DataTable GetLookup(string lookupname)
    {
        DataTable dtLookup = new DataTable("Lookup");
        try
        {
            string strsql = "SELECT param_field_value as lookup_value, param_field_value as lookup_code " +
                          "FROM cti_service_parameters " +
                           "WHERE param_service_id ='" + serviceid + "' and param_field_name='" + lookupname + "' ORDER BY param_field_value";


            OdbcDataAdapter ad = new OdbcDataAdapter(strsql, con.oCon);
            ad.Fill(dtLookup);
        }
        catch (OdbcException ex)
        {
            crmheader.InnerHtml = "GetLookup()" + lookupname + "-" + ex.Message;
        }
        return dtLookup;
    }

    protected DataTable GetDispositionGroup()
    {
        DataTable dtGroup = new DataTable("Lookup");
        try
        {
            string strsql = "SELECT DISTINCT servdesp_parent_desp_desc,servdesp_parent_desp_code " +
                          "FROM cti_service_desposition " +
                          "WHERE servdesp_service_id=" + serviceid +
                          " ORDER BY servdesp_parent_desp_desc";

            OdbcDataAdapter ad = new OdbcDataAdapter(strsql, con.oCon);
            ad.Fill(dtGroup);
        }
        catch (OdbcException ex)
        {
            crmheader.InnerHtml = "GetDispositionGroup()-" + ex.Message;
        }
        return dtGroup;
    }

    public class listvalue
    {
        public string textfield { get; set; }
        public string valuefield { get; set; }
    }

    protected bool CreateCRM(DataTable dtFields, DataTable dtData)
    {
        bool retvalue = false;
        try
        {
            Table tbl = new Table();
            string scriptvalidate = "var crmfields;function validatecrm(){try{crmfields = new Array();";
            scriptvalidate += "var obj=new Object();obj.Name='ZActionTypeZ';obj.Value='" + actiontype + "';crmfields.push(obj);";
            scriptvalidate += "var obj=new Object();obj.Name='ZTablenameZ';obj.Value='" + writetable + "';crmfields.push(obj);";
            // Added by ashish
            scriptvalidate += "var obj=new Object();obj.Name='ZServiceIDZ';obj.Value='" + serviceid + "';crmfields.push(obj);";
            scriptvalidate += "var obj=new Object();obj.Name='ZCallmasterTableZ';obj.Value='" + readtable + "';crmfields.push(obj);";

            if (actiontype == "update")
            {
                scriptvalidate += "var obj=new Object();obj.Name='ZleadidZ';obj.Value='" + leadid + "';crmfields.push(obj);";
            }
            else
            {
                scriptvalidate += "var obj=new Object();obj.Name='ZCallNumberZ';obj.Value='" + callnumber + "';crmfields.push(obj);";
                scriptvalidate += "var obj=new Object();obj.Name='ZLeadIDZ';obj.Value='" + leadid + "';crmfields.push(obj);";
                scriptvalidate += "var obj=new Object();obj.Name='ZDialedPhoneZ';obj.Value='" + cli + "';crmfields.push(obj);";
                scriptvalidate += "var obj=new Object();obj.Name='ZCallStartTimeZ';obj.Value='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "';crmfields.push(obj);";
                scriptvalidate += "var obj=new Object();obj.Name='ZCallBackDateTimeZ';obj.Value='" + hdCallbackTime + "';crmfields.push(obj);";
                scriptvalidate += "var obj=new Object();obj.Name='ZAgentIDZ';obj.Value='" + agentid + "';crmfields.push(obj);";
                scriptvalidate += "var obj=new Object();obj.Name='ZDispositionZ';var x_ZDispositionZ='" + hddisposition + "';";
                scriptvalidate += "var disp = x_ZDispositionZ.split('|');if(disp.length==2){if(disp[1]=='07'){if(validatedateday('" + hdCallbackTime + "','" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "'," + maxcallbackday + ") == false)";
                scriptvalidate += "{'" + hdCallbackTime + "'.focus();return false;}}";
                scriptvalidate += "}obj.Value=disp[0];crmfields.push(obj);";
                scriptvalidate += "var obj=new Object();obj.Name='ZCommentZ';obj.Value='" + hdRemarks + "';crmfields.push(obj);";

                // added by ashish Set call back number
                // scriptvalidate += "var obj=new Object();obj.Name='ZSetCallbackZ';obj.Value=document.forms['crmform']['setcallback'].value;crmfields.push(obj);";

            }

            TableRow tr = new TableRow();
            int lastrow = 0;
            foreach (DataRow row in dtFields.Rows)
            {
                int rownum = Convert.ToInt32(row["field_row"]);
                if (lastrow > 0 && rownum != lastrow)
                {
                    tbl.Rows.Add(tr);
                    tr = new TableRow();
                    lastrow = rownum;
                }
                if (lastrow == 0)
                    lastrow = rownum;

                TableCell td = new TableCell();
                Label lbl = new Label();
                lbl.Text = row["field_display_name"].ToString();
                //  lbl.Text.Replace("<br>", Environment.NewLine);
                // lbl.Style.Add("wrap", "true");
                td.Controls.Add(lbl);
                td.Style.Add("width", column_width);
                tr.Cells.Add(td);

                string crm_field_name = row["field_name"].ToString();
                string crm_field_data = "";
                if (dtData.Rows.Count > 0 /*&& row["field_readonly"].ToString() == "Y"*/ && dtData.Columns.Contains(crm_field_name))
                    crm_field_data = dtData.Rows[0][crm_field_name].ToString();
                if (row["field_data_capture"].ToString() == "Y" || row["field_compulsory"].ToString() == "Y")
                {
                    scriptvalidate += "var x_" + crm_field_name + "=document.forms['crmform']['" + crm_field_name + "'].value;";
                }
                if (row["field_data_capture"].ToString() == "Y")
                    scriptvalidate += "var obj=new Object();obj.Name='" + crm_field_name + "';obj.Value=x_" + crm_field_name + ";crmfields.push(obj);";

                if (row["field_readonly"].ToString() == "Y" && row["field_data_type"].ToString() != "Phone")
                {
                    td = new TableCell();
                    td.Style.Add("width", column_width);
                    TextBox tbox = new TextBox();
                    tbox.Text = crm_field_data; // crm_data
                    tbox.ID = crm_field_name;
                    string fieldscript = row["field_script"].ToString();
                    if (fieldscript.Length > 0)
                        tbox.Attributes.Add("onclick", fieldscript);

                    tbox.BackColor = System.Drawing.Color.LightGray;
                    tbox.ReadOnly = true;
                    td.Controls.Add(tbox);
                    tr.Cells.Add(td);
                }
                else if (row["field_data_type"].ToString() == "Edit")
                {
                    td = new TableCell();
                    td.Style.Add("width", column_width);
                    TextBox tbox = new TextBox();
                    tbox.Text = crm_field_data; // crm_data
                    tbox.ID = crm_field_name;
                    string fieldscript = row["field_script"].ToString();
                    if (fieldscript.Length > 0)
                        tbox.Attributes.Add("onblur", fieldscript);
                    if (row["field_data_max"].ToString() != "0")
                        tbox.Attributes.Add("maxlength", row["field_data_max"].ToString());
                    if (row["field_compulsory"].ToString() == "Y")
                    {
                        tbox.BackColor = System.Drawing.Color.Yellow;
                        scriptvalidate += "if(validatetext(x_" + tbox.ID + "," + row["field_data_min"].ToString() + "," + row["field_data_max"].ToString() + ") == false)" +
                            "{alert('Invalid value in " + lbl.Text + "');document.forms['crmform']['" + tbox.ID + "'].focus();return false;}";
                    }
                    // if(field_display_name  == "Email:")
                    //     tbox.Width = Unit.Pixel((int)tbox.Width.Value + 200); // to increase width of textbox 

                    td.Controls.Add(tbox);
                    tr.Cells.Add(td);

                    // style="width:100" id="tb" onkeyup="changeWidth(this);"
                    //  string f = tbox.Text.ToString();
                    // string ff = f.Replace("\r\n", "<br/>");


                }
                else if (row["field_data_type"].ToString() == "Password")
                {
                    td = new TableCell();
                    TextBox tbox = new TextBox();
                    td.Style.Add("width", column_width);
                    tbox.Text = crm_field_data; // crm_data
                    tbox.ID = crm_field_name;
                    string fieldscript = row["field_script"].ToString();
                    if (fieldscript.Length > 0)
                        tbox.Attributes.Add("onblur", fieldscript);
                    if (row["field_data_max"].ToString() != "0")
                        tbox.Attributes.Add("maxlength", row["field_data_max"].ToString());
                    if (row["field_compulsory"].ToString() == "Y")
                    {
                        tbox.BackColor = System.Drawing.Color.Yellow;

                        scriptvalidate += "if(validatetext(x_" + tbox.ID + "," + row["field_data_min"].ToString() + "," + row["field_data_max"].ToString() + ") == false)" +
                            "{alert('Invalid value in " + lbl.Text + "');document.forms['crmform']['" + tbox.ID + "'].focus();return false;}";
                    }
                    tbox.TextMode = TextBoxMode.Password;
                    // tbox.Width = Unit.Pixel((int)tbox.Width.Value + 100);
                    td.Controls.Add(tbox);
                    tr.Cells.Add(td);
                }
                else if (row["field_data_type"].ToString() == "Text")
                {
                    td = new TableCell();
                    td.Style.Add("width", column_width);
                    TextBox tbox = new TextBox();
                    tbox.Text = crm_field_data; // crm_data
                    tbox.ID = crm_field_name;
                    string fieldscript = row["field_script"].ToString();
                    if (fieldscript.Length > 0)
                        tbox.Attributes.Add("onblur", fieldscript);
                    if (row["field_data_max"].ToString() != "0")
                        tbox.Attributes.Add("maxlength", row["field_data_max"].ToString());

                    if (row["field_compulsory"].ToString() == "Y")
                    {
                        tbox.BackColor = System.Drawing.Color.Yellow;

                        scriptvalidate += "if(validatetext(x_" + tbox.ID + "," + row["field_data_min"].ToString() + "," + row["field_data_max"].ToString() + ") == false)" +
                            "{alert('Invalid value in " + lbl.Text + "');document.forms['crmform']['" + tbox.ID + "'].focus();return false;}";
                    }
                    tbox.TextMode = TextBoxMode.MultiLine;
                    //tbox.Width = Unit.Pixel((int)tbox.Width.Value + 200);
                    td.Controls.Add(tbox);
                    tr.Cells.Add(td);
                }
                else if (row["field_data_type"].ToString() == "Email")
                {
                    td = new TableCell();
                    td.Style.Add("width", column_width);
                    TextBox tbox = new TextBox();
                    tbox.Text = crm_field_data; // crm_data
                    tbox.ID = crm_field_name;
                    string fieldscript = row["field_script"].ToString();
                    if (fieldscript.Length > 0)
                        tbox.Attributes.Add("onblur", fieldscript);
                    if (row["field_data_max"].ToString() != "0")
                        tbox.Attributes.Add("maxlength", row["field_data_max"].ToString());

                    if (row["field_compulsory"].ToString() == "Y")
                    {
                        tbox.BackColor = System.Drawing.Color.Yellow;

                        scriptvalidate += "if(validateemail(x_" + tbox.ID + "," + row["field_data_min"].ToString() + "," + row["field_data_max"].ToString() + ") == false)" +
                            "{alert('Invalid value in " + lbl.Text + "');document.forms['crmform']['" + tbox.ID + "'].focus();return false;}";
                    }
                    //tbox.Width = Unit.Pixel((int)tbox.Width.Value + 100);
                    td.Controls.Add(tbox);
                    tr.Cells.Add(td);
                }
                else if (row["field_data_type"].ToString() == "Number")
                {
                    td = new TableCell();
                    td.Style.Add("width", column_width);
                    TextBox tbox = new TextBox();
                    tbox.Text = crm_field_data; // crm_data
                    tbox.ID = crm_field_name;
                    string fieldscript = row["field_script"].ToString();
                    if (fieldscript.Length > 0)
                        tbox.Attributes.Add("onblur", fieldscript);
                    if (row["field_data_max"].ToString() != "0")
                        tbox.Attributes.Add("maxlength", row["field_data_max"].ToString());

                    tbox.Attributes.Add("onkeypress", "javascript:return IsNumeric(event);");
                    if (row["field_compulsory"].ToString() == "Y")
                    {
                        tbox.BackColor = System.Drawing.Color.Yellow;
                        scriptvalidate += "if(validatenumber(x_" + tbox.ID + "," + row["field_data_min"].ToString() + "," + row["field_data_max"].ToString() + ") == false)" +
                            "{alert('Invalid value in " + lbl.Text + "');document.forms['crmform']['" + tbox.ID + "'].focus();return false;}";
                    }
                    // tbox.Width = Unit.Pixel((int)tbox.Width.Value + 100);
                    td.Controls.Add(tbox);
                    tr.Cells.Add(td);
                }
                else if (row["field_data_type"].ToString() == "Mobile")
                {
                    td = new TableCell();
                    td.Style.Add("width", column_width);
                    TextBox tbox = new TextBox();
                    tbox.Text = crm_field_data; // crm_data
                    tbox.ID = crm_field_name;
                    string fieldscript = row["field_script"].ToString();
                    if (fieldscript.Length > 0)
                        tbox.Attributes.Add("onblur", fieldscript);
                    if (row["field_data_max"].ToString() != "0")
                        tbox.Attributes.Add("maxlength", row["field_data_max"].ToString());

                    tbox.Attributes.Add("onkeypress", "javascript:return IsNumeric(event);");
                    if (row["field_compulsory"].ToString() == "Y")
                    {
                        tbox.BackColor = System.Drawing.Color.Yellow;
                        scriptvalidate += "if(validatemobile(x_" + tbox.ID + "," + row["field_data_min"].ToString() + "," + row["field_data_max"].ToString() + ") == false)" +
                            "{alert('Invalid value in " + lbl.Text + "');document.forms['crmform']['" + tbox.ID + "'].focus();return false;}";
                    }
                    // tbox.Width = Unit.Pixel((int)tbox.Width.Value + 100);
                    td.Controls.Add(tbox);
                    HtmlAnchor alink = new HtmlAnchor();
                    alink.HRef = "#";
                    //  alink.InnerHtml = "<input type='checkbox' name='checkbox' id='checkbox_id' value='value' alt='Dial Number' onclick=javascript:dialnumber(document.forms[\'crmform\'][\'" + tbox.ID + "\'].value);>;";
                    alink.InnerHtml = "<img src='Images/phone.png' width='18' height='18' border='0' alt='Dial Number' onclick=javascript:dialnumber(document.forms[\'crmform\'][\'" + tbox.ID + "\'].value); />";
                    td.Controls.Add(alink);

                    tr.Cells.Add(td);
                }
                else if (row["field_data_type"].ToString() == "Phone")
                {
                    td = new TableCell();
                    TextBox tbox = new TextBox();
                    tbox.Text = crm_field_data; // crm_data
                    tbox.ID = crm_field_name;
                    string fieldscript = row["field_script"].ToString();
                    if (fieldscript.Length > 0)
                        tbox.Attributes.Add("onblur", fieldscript);
                    if (row["field_data_max"].ToString() != "0")
                        tbox.Attributes.Add("maxlength", row["field_data_max"].ToString());
                    tbox.Attributes.Add("onkeypress", "javascript:return IsNumeric(event);");
                    if (row["field_compulsory"].ToString() == "Y")
                    {
                        tbox.BackColor = System.Drawing.Color.Yellow;
                        scriptvalidate += "if(validatephone(x_" + tbox.ID + "," + row["field_data_min"].ToString() + "," + row["field_data_max"].ToString() + ") == false)" +
                            "{alert('Invalid value in " + lbl.Text + "');document.forms['crmform']['" + tbox.ID + "'].focus();return false;}";
                    }
                    if (row["field_readonly"].ToString() == "Y")
                    {
                        tbox.BackColor = System.Drawing.Color.LightGray;

                        tbox.ReadOnly = true;
                    }
                    // tbox.Width = Unit.Pixel((int)tbox.Width.Value + 100);
                    td.Controls.Add(tbox);

                    HtmlAnchor alinkCheck = new HtmlAnchor();
                    alinkCheck.HRef = "#";
                    alinkCheck.InnerHtml = "<input type='checkbox' class='check'  name='checkbox' id='checkbox_id' value='" + tbox.ID + "' alt='Dial Number' />";
                    td.Controls.Add(alinkCheck);
                    if (row["field_readonly"].ToString() == "Y")
                    {
                        alinkCheck.Disabled = true;
                    }
                    HtmlAnchor alink = new HtmlAnchor();
                    alink.HRef = "#";
                    alink.InnerHtml = "<img src='Images/phone.png' width='16' height='16' border='0' alt='Dial Number' onclick=javascript:dialnumber(document.forms[\'crmform\'][\'" + tbox.ID + "\'].value); />";
                    td.Controls.Add(alink);

                    tr.Cells.Add(td);
                }
                else if (row["field_data_type"].ToString() == "Droplist" || row["field_data_type"].ToString() == "Dropdown")
                {
                    td = new TableCell();
                    td.Style.Add("width", column_width);
                    DropDownList tlst = new DropDownList();
                    tlst.ID = crm_field_name;
                    string fieldscript = row["field_script"].ToString();
                    if (fieldscript.Length > 0)
                        tlst.Attributes.Add("onchange", fieldscript);

                    if (row["field_compulsory"].ToString() == "Y")
                    {
                        tlst.BackColor = System.Drawing.Color.Yellow;

                        scriptvalidate += "if(validatetext(x_" + tlst.ID + "," + row["field_data_min"].ToString() + "," + row["field_data_max"].ToString() + ") == false)" +
                            "{alert('Invalid value in " + lbl.Text + "');document.forms['crmform']['" + tlst.ID + "'].focus();return false;}";
                    }
                    DataTable dtLookup = GetLookup(row["field_dropdown_lookup"].ToString());
                    tlst.Items.Add(new ListItem("", ""));
                    foreach (DataRow lookup in dtLookup.Rows)
                    {
                        tlst.Items.Add(new ListItem(lookup["lookup_value"].ToString(), lookup["lookup_code"].ToString()));
                    }
                    tlst.SelectedValue = crm_field_data; // crm_data
                    // tlst.Width = Unit.Pixel((int)tlst.Width.Value + 100);
                    td.Controls.Add(tlst);
                    tr.Cells.Add(td);
                }

                else if (row["field_data_type"].ToString() == "GroupBox")
                {
                    string fieldscript = row["field_script"].ToString();
                    if (fieldscript.Length > 0)
                        tr.Attributes.Add("onclick", fieldscript);
                    td.ColumnSpan = 10;
                    TextBox tbox = new TextBox();
                    tbox.Text = crm_field_data; // crm_data
                    tbox.ID = crm_field_name;
                    tbox.Style.Add(HtmlTextWriterStyle.Display, "none");
                    td.Controls.Add(tbox);
                    tr.BackColor = System.Drawing.Color.SkyBlue;//.LightBlue;
                    tr.ForeColor = System.Drawing.Color.White;
                    tr.Cells.Add(td);
                }
                else if (row["field_data_type"].ToString() == "Datetime")
                {
                    td = new TableCell();
                    td.Style.Add("width", column_width);
                    TextBox tbox = new TextBox();
                    tbox.Text = crm_field_data; // crm_data
                    tbox.ID = crm_field_name;
                    string fieldscript = row["field_script"].ToString();
                    if (fieldscript.Length > 0)
                        tbox.Attributes.Add("onblur", fieldscript);
                    if (row["field_data_max"].ToString() != "0")
                        tbox.Attributes.Add("maxlength", row["field_data_max"].ToString());
                    if (row["field_compulsory"].ToString() == "Y")
                    {
                        tbox.BackColor = System.Drawing.Color.Yellow;

                        scriptvalidate += "if(validatedate(x_" + tbox.ID + "," + row["field_data_min"].ToString() + "," + row["field_data_max"].ToString() + ") == false)" +
                            "{alert('Invalid value in " + lbl.Text + "');document.forms['crmform']['" + tbox.ID + "'].focus();return false;}";
                    }
                    // td.Width = Unit.Pixel((int)td.Width.Value + 100);
                    td.Controls.Add(tbox);
                    HtmlAnchor alink = new HtmlAnchor();
                    alink.HRef = "javascript:NewCal('" + tbox.ID + "','ddmmmyyyy',true,24);";
                    alink.InnerHtml = "<img src='Images/cal.gif' width='16' height='16' border='0' alt='Pick a date'/>";
                    td.Controls.Add(alink);
                    tr.Cells.Add(td);
                }
                else if (row["field_data_type"].ToString() == "Date")
                {
                    td = new TableCell();
                    td.Style.Add("width", column_width);
                    TextBox tbox = new TextBox();
                    tbox.Text = crm_field_data; // crm_data
                    tbox.ID = crm_field_name;
                    string fieldscript = row["field_script"].ToString();
                    if (fieldscript.Length > 0)
                        tbox.Attributes.Add("onblur", fieldscript);
                    if (row["field_data_max"].ToString() != "0")
                        tbox.Attributes.Add("maxlength", row["field_data_max"].ToString());
                    if (row["field_compulsory"].ToString() == "Y")
                    {
                        tbox.BackColor = System.Drawing.Color.Yellow;
                        scriptvalidate += "if(validatedate(x_" + tbox.ID + "," + row["field_data_min"].ToString() + "," + row["field_data_max"].ToString() + ") == false)" +
                            "{alert('Invalid value in " + lbl.Text + "');document.forms['crmform']['" + tbox.ID + "'].focus();return false;}";
                    }
                    // td.Width = Unit.Pixel((int)td.Width.Value + 100);
                    td.Controls.Add(tbox);
                    HtmlAnchor alink = new HtmlAnchor();
                    alink.HRef = "javascript:NewCal('" + tbox.ID + "','ddmmmyyyy',false,24);";
                    alink.InnerHtml = "<img src='Images/cal.gif' width='16' height='16' border='0' alt='Pick a Date'/>";
                    td.Controls.Add(alink);
                    tr.Cells.Add(td);
                }
                else if (row["field_data_type"].ToString() == "URL")
                {
                    td = new TableCell();
                    HtmlAnchor alink = new HtmlAnchor();
                    alink.HRef = crm_field_data;
                    alink.InnerHtml = crm_field_name;
                    alink.ID = crm_field_name;
                    string fieldscript = row["field_script"].ToString();
                    if (fieldscript.Length > 0)
                        alink.Attributes.Add("onclick", fieldscript);
                    td.Controls.Add(alink);
                    tr.Cells.Add(td);
                }

                if (rownum == 0 || rownum != lastrow)
                {
                    tbl.Rows.Add(tr);
                    tr = new TableRow();
                    lastrow = rownum;
                }
            }
            tbl.Rows.Add(tr);
            crmcontent.Controls.Add(tbl);
            scriptvalidate += "}catch(e){alert(e.description);return false;}}";
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "validatecrm", scriptvalidate, true);
            retvalue = true;
        }
        catch (Exception ex)
        {
            crmheader.InnerHtml = "CreateCRM()-" + ex.Message;
        }
        return retvalue;
    }

    protected void Page_Unload(object sender, EventArgs e)
    {
        if (con != null)
            con.CloseDB();
    }

    [System.Web.Services.WebMethod()]
    public static string DialCRM(string Number)
    {
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        //CtiWS1.Dial("", "", Number, callnumber, HttpContext.Current.Session["HOSTID"].ToString());
        CtiWS1.DialCRM("", "", Number, HttpContext.Current.Session["HOSTID"].ToString());
        return "Success";
    }


    protected void get_udf_callmaster_table()
    {
        try
        {
            DataTable dt1 = new DataTable();
            rdCtiHistory.Rebind();
        }
        catch (Exception ex)
        {
            string Result = ex.Message;
        }
    }

    protected void rdCtiHistory_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        try
        {
            CTIHistoryDetails.ServiceId = serviceid;
            CTIHistoryDetails.CLI = cli;
            CTIHistoryDetails.LeadId = leadid;
            CTIHistoryDetails.CallMasterTableName = "";//strCallmaster;

            CtiHistoryDetailBal.CtiHistory(ref rdCtiHistory, CTIHistoryDetails);
            // rdCtiHistory.DataBind();
        }
        catch (Exception ex)
        {
            string Result = ex.Message;
        }
    }

    // Get All lead_phone Number in case of CallbackPhone Number from CTI_Lead_master Table call 
    protected DataSet GetCallBackPhoneNo()
    {
        using (OdbcConnection con3 = new OdbcConnection())
        {
            DataSet dsCallbackPhone = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                string strTemp = "SELECT " + con.DB_TOP_SQL +
               "CASE WHEN [lead_phone1] IS NULL THEN '' ELSE [lead_phone1] END  AS Phone1, 1 AS PhoneId1, " +
               "CASE WHEN [lead_phone2] IS NULL THEN '' ELSE [lead_phone2] END  AS Phone2, 2 AS PhoneId2, " +
               "CASE WHEN [lead_phone3] IS NULL THEN '' ELSE [lead_phone3] END  AS Phone3, 3 AS PhoneId3 , " +
               "CASE WHEN [lead_phone4] IS NULL THEN '' ELSE [lead_phone4] END  AS Phone4, 4 AS PhoneId4, " +
               "CASE WHEN [lead_phone5] IS NULL THEN '' ELSE [lead_phone5] END  AS Phone5, 5 AS PhoneId5, " +
               "CASE WHEN [lead_phone6] IS NULL THEN '' ELSE [lead_phone6] END  AS Phone6, 6 AS PhoneId6, " +
               "CASE WHEN [lead_phone7] IS NULL THEN '' ELSE [lead_phone7] END  AS Phone7, 7 AS PhoneId7, " +
               "CASE WHEN [lead_phone8] IS NULL THEN '' ELSE [lead_phone8] END  AS Phone8, 8 AS PhoneId8, " +
               "CASE WHEN [lead_phone9] IS NULL THEN '' ELSE [lead_phone9] END  AS Phone9, 9 AS PhoneId9 , " +
               "CASE WHEN [lead_phone10] IS NULL THEN '' ELSE [lead_phone10] END  AS Phone10,10  AS PhoneId10, " +
               "CASE WHEN [lead_phone11] IS NULL THEN '' ELSE [lead_phone11] END  AS Phone11, 11 AS PhoneId11, " +
               "CASE WHEN [lead_phone12] IS NULL THEN '' ELSE [lead_phone12] END  AS Phone12, 12 AS PhoneId12 ," +
               "CASE WHEN [lead_phone13] IS NULL THEN '' ELSE [lead_phone13] END  AS Phone13, 13 AS PhoneId13 ," +
               "CASE WHEN [lead_phone14] IS NULL THEN '' ELSE [lead_phone14] END  AS Phone14, 14 AS PhoneId14," +
               "CASE WHEN [lead_phone15] IS NULL THEN '' ELSE [lead_phone15] END  AS Phone15, 15 AS PhoneId15 " +
               "FROM " + readtable + " WHERE lead_id = " + leadid + " AND lead_service_id =" + serviceid + "" + con.DB_TOP_MYSQL;

                OdbcDataAdapter ad = new OdbcDataAdapter(strTemp, con.oCon);
                if (ad.Fill(dsCallbackPhone) > 0)
                {

                    dsCallbackPhone.Tables[0].TableName = "CTIPhoneNo";
                }
            }
            catch (Exception ex)
            {
                string Result = ex.Message;
            }
            return dsCallbackPhone;
        }

    }

}