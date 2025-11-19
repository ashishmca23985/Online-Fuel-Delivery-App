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
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using LitJson;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;


public partial class Reports : ThemeBase
{
    private DataBase m_Connection = null;
    private DataTable dtReports = null;
    ReportWS reportws = new ReportWS();
    GlobalWS objGlobalWS = new GlobalWS();
    string nLocationId = string.Empty;

    #region Page Load
    protected void Page_Load(object sender, EventArgs e)
    {
        lblMessage.Text = "";
        if (IsPostBack)
            return;

        Session["REPORT"] = null;
        GetDriverList();
        BindExportCombo();

    }
    #endregion

    #region Bind Driver List
    private void GetDriverList()
    {
        MasterWS ob = new MasterWS();
        try
        {
            DataSet ds_drlist = ob.GetEFunnelDeviceId();
            if (Convert.ToInt32(ds_drlist.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(ds_drlist.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }
            cmbDiviceList.DataSource = ds_drlist.Tables[0];
            cmbDiviceList.DataValueField = "Name";
            cmbDiviceList.DataTextField = "Name";
            cmbDiviceList.DataBind();

            RadComboBoxItem rdItem = new RadComboBoxItem("", "");
            cmbDiviceList.Items.Insert(0, rdItem);

//            ViewState["dtDrList"] = ds_drlist.Tables[0];

        }
        catch (Exception ex)
        {
            lblMessage.LogMessage(ex.Message, 1);
            return;
        }
    }
    #endregion
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            lblHeader.Text = "[ From Date :" + (dtFrom.SelectedDate ?? DateTime.MinValue).ToString("dd MMM yyyy") + " - To Date : " + (dtTo.SelectedDate ?? DateTime.MinValue).ToString("dd MMM yyyy") + "   -  E-Fuunel DeviceId :- "+cmbDiviceList.SelectedValue+"]";
            if (dtFrom.SelectedDate.HasValue && dtTo.SelectedDate.HasValue)
            {
                DateTime dtfrom = dtFrom.SelectedDate.Value;
                DateTime dtto = dtTo.SelectedDate.Value;
                GetEFunnel_Daywise(dtfrom, dtto, cmbDiviceList.SelectedValue);
             }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }

    public void GetEFunnel_Daywise( DateTime dtfrom, DateTime dtto, string EFunnelDeviceId)
    {
        string szId = "";
        string szUserId = "";
        string sztoken = "";
        //string strEFunnelDeviceId = "b827eb98dd75"; //EFunnelDeviceId; // "MYANMAR-FUEL-001"; // b827eb98dd75",
        try
        {
            string userAuthenticationURI = "https://www.smartdg.in/api/Account/AuthenticateUser?userId=TXlhbm1hcjAwMQ==&password=VHNlY29uZEAxNA==";
            if (!string.IsNullOrEmpty(userAuthenticationURI))
            {

               
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(userAuthenticationURI);
                request.Method = "GET";
                request.KeepAlive = false;

                request.ContentType = "application/json";
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
              
                WebResponse response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var ApiStatus = reader.ReadToEnd();
                    string json = ApiStatus;
                    APIResponseData obj = JsonConvert.DeserializeObject<APIResponseData>(json);
                    string status = obj.message.ToString();
                    if (status.ToLower() == "success")
                    {
                        szId = obj.data.id.ToString();
                        szUserId = obj.data.userID.ToString();
                        sztoken = obj.data.token.ToString();
                        if (sztoken != "")
                        {
                            GetEFunnelAPIData_Daywise(sztoken, EFunnelDeviceId, dtfrom, dtto);
                        }
                    }
                    else
                    {
                        
                        //AddLog("Main", "status" + status);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
            // AddLog("Main", ex.Message);
        }
    }
    public void GetEFunnelAPIData_Daywise(string accessToken, string EFunnelDeviceId, DateTime dtfrom, DateTime dtto)
    {
        string outputJson = "";
        string deviceId = "";
        string fuel = "";
        string timeStamp = "";
        string gpsLatitude = "";
        string gpsLongitude = "";

        try
        {
            
            string dtStartTime = dtfrom.ToString("yyyy-MM-dd") + " 00:00:01";
            string dtEndTime = dtto.ToString("yyyy-MM-dd") + " 23:59:59";

            //string dtStartTime = System.DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:01";
            //string dtEndTime = System.DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";

            string GetFuelUrl = "https://www.smartdg.in/api/Dashboard/GetFuelFillingBetweenDates?deviceId=" + EFunnelDeviceId + "&startDate=" + dtStartTime + "&toDate=" + dtEndTime + "&offsetMin=390";
            using (var clients = new HttpClient())
            {
                clients.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var content1 = new StringContent("", Encoding.UTF8, "application/json");
                var response = clients.GetAsync(new Uri(GetFuelUrl)).Result;
                var responseString = response.Content.ReadAsStringAsync();
                outputJson = responseString.Result;
                
                DataTable dtResponse = ConvertJsonToDatatable(outputJson);
                if(dtResponse.Rows.Count > 0)
                {
                    Session["REPORT"] = dtResponse;
                    rdgReports.Rebind();
                }
               else
                {
                    lblMessage.Text = "No Data Available";
    
                }
            }

        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    public static DataTable ConvertJsonToDatatable(string jsonString)
    {
        var jsonLinq = JObject.Parse(jsonString);
        // Find the first array using Linq
        var linqArray = jsonLinq.Descendants().Where(x => x is JArray).First();
        var jsonArray = new JArray();
        foreach (JObject row in linqArray.Children<JObject>())
        {
            var createRow = new JObject();
            foreach (JProperty column in row.Properties())
            {
                // Only include JValue types
                if (column.Value is JValue)
                {
                    createRow.Add(column.Name, column.Value);
                }
            }
            jsonArray.Add(createRow);
        }
        return JsonConvert.DeserializeObject<DataTable>(jsonArray.ToString());
    }

    protected void OnAjaxUpdate(object sender, ToolTipUpdateEventArgs args)
    {
        this.UpdateToolTip(args.Value, args.UpdatePanel);
    }

    private void UpdateToolTip(string elementID, UpdatePanel panel)
    {
        try
        {
            //m_Connection = new DataBase();
            //m_Connection.OpenDB("CRM");

            //int index = Convert.ToInt32(elementID);
            //HtmlGenericControl ctrl = new HtmlGenericControl();
            //ctrl.TagName = "div";
            //panel.ContentTemplateContainer.Controls.Add(ctrl);

            //string strSQL = "exec smsGetToolTip 'EMAIL'," + index;
            //SqlCommand m_CommandOdbc = new SqlCommand(strSQL, m_Connection.oCon);

            //ctrl.InnerHtml = m_CommandOdbc.ExecuteScalar().ToString();
        }
        catch (SqlException ex)
        {
            LogMessage(ex.Message, 1);
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
        finally
        {
            //m_Connection.CloseDB();
        }
    }

    protected void rdgReports_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        try
        {
            if (Session["REPORT"] != null)
                rdgReports.DataSource = (DataTable)Session["REPORT"];
        }

        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }

    }

    #region Log Message
    void LogMessage(string errorMessage, Int32 param)
    {
        lblMessage.Text = errorMessage;
        if (param == 1)
            lblMessage.CssClass = "error";
        else
            lblMessage.CssClass = "success";
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

    #region Export To Excel Without Pagging
    protected void imgExport_Click(object sender, EventArgs e)
    {
        try
        {
            string strContentType = cmbExport.SelectedValue;
            if (String.IsNullOrEmpty(strContentType))
                return;
            if (rdgReports.Items.Count <= 0)
                return;
            switch (strContentType)
            {
                case "application/ms.xls":
                    {
                        rdgReports.ExportSettings.IgnorePaging = true;
                        rdgReports.ExportSettings.OpenInNewWindow = true;
                        rdgReports.ExportSettings.ExportOnlyData = true;
                        rdgReports.ExportSettings.FileName = lblHeader.Text;
                        rdgReports.MasterTableView.ExportToExcel();
                    }
                    break;
                case "application/ms.pdf":
                    {
                        rdgReports.ExportSettings.IgnorePaging = true;
                        rdgReports.ExportSettings.OpenInNewWindow = true;
                        rdgReports.ExportSettings.FileName = lblHeader.Text;
                        rdgReports.MasterTableView.ExportToPdf();
                    }
                    break;
                case "application/ms.csv":
                    {

                        DataTable dt = new DataTable();
                        string strFileName = "";//
                        if (Session["REPORT"] != null)
                            dt = (DataTable)Session["REPORT"];
                        strFileName = "TICKET_DUMP" + DateTime.Now.ToString("ddMMyyyy");
                        Export_CSVFile_DataTable_SFTP(strFileName, dt);

                    }
                    break;
                case "application/ms.txt":
                    {
                        DataTable dt = new DataTable();
                        string strFileName = "";
                        string strPath = "";

                        //if (Session["REPORT"] != null)
                        //    dt = (DataTable)Session["REPORT"];
                        //if (hdPerportId.Value == "1193" || hdPerportId.Value == "1222" || hdPerportId.Value == "1233")
                        //{
                        //    strFileName ="ISON_DEL_FLOW_" + DateTime.Now.ToString("ddMMyyyy");
                        //    strPath = "C:\\SBI_EXPORT_FILE\\ " + strFileName + ".txt";  //Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
                        //    WriteCSVFile(dt, strPath);
                        //} 
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


    public void ExportDataTabletoFile(DataTable datatable, string delimited, bool exportcolumnsheader, string file)
    {
        StreamWriter str = new StreamWriter(file, false, System.Text.Encoding.Default);
        if (exportcolumnsheader)
        {
            string Columns = string.Empty;
            foreach (DataColumn column in datatable.Columns)
            {
                Columns += column.ColumnName + delimited;
            }
            str.WriteLine(Columns.Remove(Columns.Length - 1, 1));
        }
        foreach (DataRow datarow in datatable.Rows)
        {
            string row = string.Empty;
            foreach (object items in datarow.ItemArray)
            {
                row += items.ToString() + delimited;
            }
            str.WriteLine(row.Remove(row.Length - 1, 1));
        }
        str.Flush();
        str.Close();

    }


    protected void Export_CSVFile_DataTable_SFTP(String strFileName, DataTable dt)
    {
        HttpResponse response = HttpContext.Current.Response;
        response.Clear();
        response.Charset = "";
        response.ContentType = "application/text";
        response.AddHeader("content-disposition", string.Format("attachment;filename={0}", "" + strFileName + ".txt")); //DestinationFileName
        StringBuilder sb = new StringBuilder();

        // Adding ColumnNames using DataTable.

        int iColCount = dt.Columns.Count;
        int iRowCount = dt.Rows.Count;
        for (int i = 0; i < iColCount; i++)
        {
            sb.Append(dt.Columns[i]);
            if (i < iColCount - 1)
            {
                sb.Append("|");
            }
        }
        sb.Append("\r\n");
        //---------------------------------------------------------------------------------------------------

        // Adding Data Rows.
        foreach (DataRow dr in dt.Rows)
        {
            for (int i = 0; i < iColCount; i++)
            {
                if (!Convert.IsDBNull(dr[i]))
                {
                    sb.Append(dr[i].ToString());
                }
                if (i < iColCount - 1)
                {
                    sb.Append("|");
                }
            }
            sb.Append("\r\n");
        }

        Response.Write(sb.ToString());
        Response.Flush();
        Response.End();
    }
    
    protected void rdgReports_ExcelExportCellFormatting(object source, ExcelExportCellFormattingEventArgs e)
    {
        e.Cell.Style["mso-number-format"] = @"\@"; //To export in text format
        //e.Cell.Style["font-size"] = "80%";
    }

    public void WriteCSVFile(DataTable dataTable, string filePath)
    {
        StreamWriter sw = new StreamWriter(filePath, false);
        int iColCount = dataTable.Columns.Count;
        for (int i = 0; i < iColCount; i++)
        {
            sw.Write(dataTable.Columns[i]);

            if (i < iColCount - 1)
            {
                sw.Write("|");
            }
        }
        sw.Write(sw.NewLine);
        foreach (DataRow row in dataTable.Rows)
        {
            for (int i = 0; i < iColCount; i++)
            {
                if (!Convert.IsDBNull(row[i]))
                {
                    sw.Write(row[i].ToString());
                }
                if (i < iColCount - 1)
                {
                    sw.Write("|");
                }
            }
            sw.Write(sw.NewLine);
        }
        sw.Close();      
    }
    public class APIResponseData
    {
        public string message { get; set; }
        public string statusCode { get; set; }
        public RespData data { get; set; }
    }
    public class RespData
    {
        public string id { get; set; }
        public string userID { get; set; }
        public string token { get; set; }
        public string isDelete { get; set; }
    }

    public class APIResponseFuelData
    {
        public string message { get; set; }
        public string statusCode { get; set; }
        public RespFuelData data { get; set; }
    }
    public class RespFuelData
    {
        public string deviceId { get; set; }
        public string fuel { get; set; }
        public string timeStamp { get; set; }
        public string gpsLatitude { get; set; }
        public string gpsLongitude { get; set; }
    }

}
