using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.UI.WebControls.WebParts;
using System.Data.SqlClient;
using Telerik.Web.UI;
using System.ServiceModel;
using System.Data.Odbc;
using System.IO;
using System.Threading;
using CtiWS;
public class Agent
{
    public string RCode { get; set; }
    public string AStatusCode { get; set; }
    public string AStatus { get; set; }
    public string AStatusSince { get; set; }
    public string PStatus { get; set; }
    public string PStatusSince { get; set; }
    public string ACli { get; set; }
    public string AServiceId { get; set; }
    public string AServiceName { get; set; }
    public string AModule { get; set; }
    public string AUrl { get; set; }
    public string ACallNumber { get; set; }
    public string ACallType { get; set; }
    public string ACallLeadId { get; set; }
    public string ADni { get; set; }
    public string ACity { get; set; }
    public string AState { get; set; }
    public string ARegion { get; set; }
    public string AAddon { get; set; }
    public string ADialable { get; set; }
    public string APreviewTime { get; set; }
    public string ACtiMessage { get; set; }

}

public partial class Master_Homepage : System.Web.UI.Page
{
    classCtiHistory obj = new classCtiHistory();
    CtiWS.CtiWS cti = new CtiWS.CtiWS();
    string strLoginID = "";
    string strHostID = "";
    
    public string szDialPrefix = "0";

    protected void Page_Init(object Sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        Response.Cache.SetNoStore();
        Response.Cache.SetNoServerCaching();
        Response.Cache.SetAllowResponseInBrowserHistory(false);
    }
    protected void Page_Preinit(object sender, EventArgs e)
    {
        Page.Theme = "Transparent";
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        szDialPrefix = System.Configuration.ConfigurationManager.AppSettings["DIALPREFIX"];
        if (Session["HOSTID"] == null || Session["LOGINID"] == null)
        {
            Session["ctilogin"] = "N";
            Response.Redirect("login.aspx", false);
            return;
        }
        strHostID = Session["HOSTID"].ToString();
        strLoginID = Session["LOGINID"].ToString();
        Session["CallNumber"] = "";
        Session["StatusCode"] = "";
        if (!Page.IsPostBack)
        {
            try
            {
                Session["ERRORCOUNT"] = 0;
                Session["NBUSYCOUNT"] = 0;
                Session["RUNNINGSERIES"] = 0;

                /*----------------Break Modes----------------------------*/
                DataSet m_DataSet = cti.GetBreakModes();
                if (m_DataSet.Tables["Response"].Rows.Count == 0 || Convert.ToInt32(m_DataSet.Tables["Response"].Rows[0]["ResultCode"]) == -1)
                {
                    cti.Logout("", "", strHostID);
                    //Response.Redirect("../login.aspx");
                    return;
                }

                for (int i = 0; i < 30; i += 3)
                {
                    int j = i + 2;
                    RadComboBoxItem lst = new RadComboBoxItem(m_DataSet.Tables["BreakModes"].Rows[0][i].ToString(), m_DataSet.Tables["BreakModes"].Rows[0][j].ToString());
                    //                    ListItem lst = new ListItem(m_DataSet.Tables["BreakModes"].Rows[0][i].ToString(), m_DataSet.Tables["BreakModes"].Rows[0][j].ToString());
                    if (m_DataSet.Tables["BreakModes"].Rows[0][i].ToString() != "Unsolicited")
                        cmbBreakModes.Items.Add(lst);
                }
                /*----------------Break Modes----------------------------*/

                DataTable dtTable = cti.GetStatus("", "", strHostID, Convert.ToInt32(Session["RUNNINGSERIES"]));
                string status = dtTable.Rows[0]["ResultString"].ToString();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(status);
                XmlElement root = doc.DocumentElement;
                XmlNodeList NodeListINFO = root.GetElementsByTagName("INFO");
                string strStatusCode = "";
                string strStatusReason = "";
                string strStatusText = "";
                string strUserId = "";
                string strDialerService = "";
                if (NodeListINFO.Count > 0)
                {
                    foreach (XmlNode category in NodeListINFO)
                    {
                        lblTerminalId.Text = category["TERMINAL"].InnerText;
                        lblAgenName.Text = category["USER"].InnerText;
                        strUserId = category["USERID"].InnerText;
                        strStatusCode = category["REASON"].InnerText;
                        strStatusReason = category["SUBREASON"].InnerText;
                        strStatusText = category["TEXT"].InnerText;
                        strDialerService = category["DIALERSERVICENAME"].InnerText;
                    }
                }
                if (strStatusText == "LOGGEDOUT" || strStatusText == "")
                {
                    Session["ctilogin"] = "N";
                    Response.Redirect("login.aspx", false);
                    
                    return;
                }

                /*----------------Services----------------------------*/
                m_DataSet = cti.GetServiceList(strUserId);
                if (m_DataSet.Tables["Response"].Rows.Count == 0 || Convert.ToInt32(m_DataSet.Tables["Response"].Rows[0]["ResultCode"]) == -1)
                {
                    //                    Response.Redirect("login.aspx");
                    //                    return;
                }
                else
                {
                    for (int i = 0; i < m_DataSet.Tables["ServiceList"].Rows.Count; i++)
                    {
                        RadComboBoxItem lst = new RadComboBoxItem(m_DataSet.Tables["ServiceList"].Rows[i][0].ToString(), m_DataSet.Tables["ServiceList"].Rows[i][2].ToString());
                        cmbServices.Items.Add(lst);
                    }
                    /*                    if (strDialerService.Length > 0)
                                        {
                                            ListItem lst1 = cmbServices.Items.FindByText(strDialerService);
                                            cmbServices.SelectedValue = lst1.Value;
                                        }*/
                }
                /*----------------Services----------------------------*/

                string[] strTelephonicCom = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["KeyTelephonicCom"]).Split(',');
                foreach (string value in strTelephonicCom)
                {
                    if (value == "1") { btnHangup.Style.Add("display", "block"); }
                    if (value == "2") { btnHold.Style.Add("display", "block"); }
                    if (value == "3") { btnTransfer.Style.Add("display", "block"); }
                    if (value == "4") { btnconfrence.Style.Add("display", "block"); }
                    if (value == "5") { trAgentStatusTransfer.Style.Add("display", "block"); }
                    if (value == "6") { btnconfrenceDisconn.Style.Add("display", "block"); }
                    if (value == "7") { btnExTransfer.Style.Add("display", "block"); }

                }
                FillDropDown1(ddlAgentNameTransfer, Convert.ToInt32(strUserId), 1, 1);
                lblagentstatus.Text = string.Empty;
                lblagentextstatus.Text = string.Empty;
                lblnagentterminalid.Text = string.Empty;
            }
            catch (Exception ex)
            {
                lblMessage.InnerHtml = ex.Message;
            }
        }
    }
    protected void ddlAgentNameTransfer_SelectedIndexChanged(object sender, EventArgs e)
    {
        tblAgentStatusTransfer.Style.Add("Display", "block");
        DataTable dt = new DataTable();
        if (ddlAgentNameTransfer.SelectedValue != string.Empty)
        {
            int AgentId = Convert.ToInt32(ddlAgentNameTransfer.SelectedValue);
            dt = obj.GetAgentDetail(AgentId);
            lblagentstatus.Text = Convert.ToString(dt.Rows[0]["agentstatus"]);
            lblnagentterminalid.Text = Convert.ToString(dt.Rows[0]["nagentterminalid"]);
            lblagentextstatus.Text = Convert.ToString(dt.Rows[0]["agentextstatus"]);
            btnTransfer.Enabled = (lblagentstatus.Text == "Idle" && lblagentextstatus.Text == "Idle") ? btnTransfer.Enabled = true : btnTransfer.Enabled = false;
            btnExTransfer.Enabled = (lblagentstatus.Text == "Idle" && lblagentextstatus.Text == "Idle") ? btnExTransfer.Enabled = true : btnExTransfer.Enabled = false;

        }
        else
        {
            lblagentstatus.Text = string.Empty;
            lblagentextstatus.Text = string.Empty;
            lblnagentterminalid.Text = string.Empty;
        }
    }
    protected void FillDropDown1(RadComboBox cmb, int userid, int No, int ZeroInsert)
    {
        obj.GEtAgentNameWithEmailId1(cmb, userid, No, ZeroInsert);
        if (ZeroInsert == 1)
        {
            RadComboBoxItem newAccount = new RadComboBoxItem("", "");
            cmb.Items.Insert(0, newAccount);
        }
    }
    [System.Web.Services.WebMethod()]
    public static string GetAgentStatus()
    {
        
        CtiWS.CtiWS cti = new CtiWS.CtiWS();
        int nDuration = 0;
        string strDuration = "";
        string CNo = "";
        Agent pAgent = new Agent();
        pAgent.RCode = "";
        pAgent.AStatusCode = "";
        pAgent.AStatus = "";
        pAgent.AStatusSince = "00:00:00";
        pAgent.PStatus = "";
        pAgent.PStatusSince = "00:00:00";
        pAgent.ACli = "";
        pAgent.AServiceId = "0";
        pAgent.AServiceName = "";
        pAgent.AModule = "";
        pAgent.AUrl = "";
        pAgent.ACallNumber = "";
        pAgent.ACallType = "";
        pAgent.ACallLeadId = "0";
        pAgent.ADni = "";
        pAgent.ACity = "";
        pAgent.AState = "";
        pAgent.ARegion = "";
        pAgent.AAddon = "";
        pAgent.ADialable = "";
        pAgent.APreviewTime = "0";
        pAgent.ACtiMessage = "";

        try
        {
        
            HttpContext.Current.Session["NOW"] = DateTime.Now;
            if (HttpContext.Current.Session["HOSTID"] == null)
                HttpContext.Current.Session["HOSTID"] = HttpContext.Current.Request.UserHostAddress;
            if (HttpContext.Current.Session["RUNNINGSERIES"] == null)
                HttpContext.Current.Session["RUNNINGSERIES"] = 0;
            DataTable dtTable = cti.GetStatus("", "", HttpContext.Current.Session["HOSTID"].ToString(), Convert.ToInt32(HttpContext.Current.Session["RUNNINGSERIES"]));

            if (dtTable.Rows.Count == 0 || Convert.ToInt32(dtTable.Rows[0]["ResultCode"]) == -1)
            {
                HttpContext.Current.Session["ERRORCOUNT"] = Convert.ToInt32(HttpContext.Current.Session["ERRORCOUNT"]) + 1;
                if (Convert.ToInt32(HttpContext.Current.Session["ERRORCOUNT"]) > 10)
                {
                    // for log out
                //    pAgent.AStatusCode = "0";
                   // HttpContext.Current.Session.Clear();
                }
                return SerializeObjectIntoJson(pAgent);
            }
            HttpContext.Current.Session["ERRORCOUNT"] = 0;
            string status = dtTable.Rows[0]["ResultString"].ToString();
            status = status.Replace('#', '#');
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(status);
            XmlElement root = doc.DocumentElement;
            XmlNodeList NodeListStatus = root.GetElementsByTagName("MARQUEE");
            if (NodeListStatus.Count > 0)
            {
                foreach (XmlNode category in NodeListStatus)
                {
                    pAgent.ACtiMessage = category.InnerText;
                    break;
                }
            }
            XmlNodeList NodeListINFO = root.GetElementsByTagName("INFO");
            bool bNewCall = false;
            string strStatusCode = "";
            bool bNumeric = true;
            if (NodeListINFO.Count > 0)
            {
                foreach (XmlNode category in NodeListINFO)
                {
                    if (category["REASON"] != null)
                        strStatusCode = category["REASON"].InnerText;
                    for (int i = 0; i < category["DURATION"].InnerText.Length; i++)
                    {
                        if (char.IsNumber(category["DURATION"].InnerText[i]) == false)
                        {
                            bNumeric = false;
                            break;
                        }
                    }
                    if (bNumeric == true)
                        nDuration = Convert.ToInt32(category["DURATION"].InnerText);
                    strDuration = (nDuration / 3600).ToString("00") + ":" + ((nDuration % 3600) / 60).ToString("00") + ":" + ((nDuration % 3600) % 60).ToString("00");
                    pAgent.AStatusSince = strDuration;
                }
                if (Convert.ToString(HttpContext.Current.Session["StatusCode"]) != strStatusCode)
                {
                    bNewCall = true;
                }
                else
                {
                    XmlNodeList NodeListCALL = root.GetElementsByTagName("CALL");
                    if (NodeListCALL.Count > 0)
                    {
                        foreach (XmlNode category in NodeListCALL)
                        {
                            if (Convert.ToString(HttpContext.Current.Session["CallNumber"]) != Convert.ToString(category["NUMBER"].InnerText))
                                bNewCall = true;
                        }
                    }
                }
            }

            //  Port Status Section
            XmlNodeList NodeListPORT = root.GetElementsByTagName("PORT");
            if (NodeListPORT.Count > 0)
            {
                bNumeric = true;
                nDuration = 0;
                foreach (XmlNode category in NodeListPORT)
                {
                    if (category["TEXT"] != null)
                    {
                        pAgent.PStatus = category["TEXT"].InnerText;
                        HttpContext.Current.Session["sessPStatus"] = category["TEXT"].InnerText;
                        for (int i = 0; i < category["DURATION"].InnerText.Length; i++)
                        {
                            if (char.IsNumber(category["DURATION"].InnerText[i]) == false)
                            {
                                bNumeric = false;
                                break;
                            }
                        }
                    }
                    if (bNumeric == true && category["DURATION"] != null)
                        nDuration = Convert.ToInt32(category["DURATION"].InnerText);
                    strDuration = (nDuration / 3600).ToString("00") + ":" + ((nDuration % 3600) / 60).ToString("00") + ":" + ((nDuration % 3600) % 60).ToString("00");
                    pAgent.PStatusSince = strDuration;
                    break;
                }
            }
            else
            {
                if ((string)HttpContext.Current.Session["sessPStatus"] != null)
                    pAgent.PStatus = (string)HttpContext.Current.Session["sessPStatus"];

            }
            //  Port Status Section

            XmlNodeList NodeListRESULT = root.GetElementsByTagName("RESULT");
            //  Result section
            if (NodeListRESULT.Count > 0)
            {
                foreach (XmlNode category in NodeListRESULT)
                {
                    if (category["CODE"] != null)
                    {
                        bNumeric = true;
                        pAgent.RCode = category["CODE"].InnerText;
                        if (bNewCall == false)
                            pAgent.RCode = "-3";
                        if (pAgent.RCode == "-3")
                        {
                            if (category["DURATION"] != null)
                            {
                                for (int i = 0; i < category["DURATION"].InnerText.Length; i++)
                                {
                                    if (char.IsNumber(category["DURATION"].InnerText[i]) == false)
                                    {
                                        bNumeric = false;
                                        break;
                                    }
                                }
                                if (bNumeric == true)
                                    nDuration = Convert.ToInt32(category["DURATION"].InnerText);
                                strDuration = (nDuration / 3600).ToString("00") + ":" + ((nDuration % 3600) / 60).ToString("00") + ":" + ((nDuration % 3600) % 60).ToString("00");
                                pAgent.AStatusSince = strDuration;
                            }

                            bNumeric = true;
                            if (category["PORTDURATION"] != null)
                            {
                                for (int i = 0; i < category["PORTDURATION"].InnerText.Length; i++)
                                {
                                    if (char.IsNumber(category["PORTDURATION"].InnerText[i]) == false)
                                    {
                                        bNumeric = false;
                                        break;
                                    }
                                }
                                if (bNumeric == true)
                                    nDuration = Convert.ToInt32(category["PORTDURATION"].InnerText);
                                strDuration = (nDuration / 3600).ToString("00") + ":" + ((nDuration % 3600) / 60).ToString("00") + ":" + ((nDuration % 3600) % 60).ToString("00");
                                pAgent.PStatusSince = strDuration;
                            }
                            return SerializeObjectIntoJson(pAgent);
                        }
                    }
                }
            }
            //  Result section

            //  Record section
            XmlNodeList NodeListRECORD = root.GetElementsByTagName("RECORD");
            if (NodeListRECORD.Count > 0)
            {
                ArrayList arCNo = null;
                if (HttpContext.Current.Session["CNO"] != null)
                    arCNo = (ArrayList)HttpContext.Current.Session["CNO"];
                else
                    arCNo = new ArrayList();
                foreach (XmlNode category in NodeListRECORD)
                {
                    if (category["STATUS"] != null && category["STATUS"].InnerText == "Active")
                    {
                        CNo = category["NUMBER"].InnerText + "|" + category["TRUNK"].InnerText;

                        if (!arCNo.Contains(CNo) && Convert.ToInt32(category["NUMBER"].InnerText) > 0)
                        {
                            arCNo.Add(CNo);
                        }
                    }
                }
                HttpContext.Current.Session["CNO"] = arCNo;
            }
            //  Record section

            //  Info Section
            if (NodeListINFO.Count > 0)
            {
                XmlNodeList NodeListCALL = root.GetElementsByTagName("CALL");
                string strStatusReason = "";
                string strStatusText = "";
                string strAgentName = "";
                bNumeric = true;
                nDuration = 0;

                foreach (XmlNode category in NodeListINFO)
                {
                    if (category["REASON"] != null)
                        strStatusCode = category["REASON"].InnerText;
                    if (category["SUBREASON"] != null)
                        strStatusReason = category["SUBREASON"].InnerText;
                    if (category["TEXT"] != null)
                        strStatusText = category["TEXT"].InnerText;
                    if (category["USER"] != null)
                    {
                        strAgentName = category["USER"].InnerText;
                    }
                    HttpContext.Current.Session["RUNNINGSERIES"] = category["SERIES"].InnerText;
                }
                if (strStatusCode == "0")
                {
                    pAgent.AStatusCode = strStatusCode;
                }
                pAgent.AStatusCode = strStatusCode;
                pAgent.AStatus = strStatusText;

                HttpContext.Current.Session["StatusCode"] = strStatusCode;
                switch (strStatusCode)
                {
                    case "0":
                        break;
                    case "1":
                        HttpContext.Current.Session["NBUSYCOUNT"] = 0;
                        HttpContext.Current.Session["CallNumber"] = "";
                        break;
                    case "2":
                        HttpContext.Current.Session["NBUSYCOUNT"] = 1;
                        HttpContext.Current.Session["CallNumber"] = "";
                        break;
                    case "3":
                        // Call Section
                        foreach (XmlNode category in NodeListCALL)
                        {
                            pAgent.ACli = Convert.ToString(category["CLI"].InnerText);
                            pAgent.AServiceId = Convert.ToString(category["SERVICEID"].InnerText);
                            pAgent.AServiceName = Convert.ToString(category["SERVICENAME"].InnerText);
                            pAgent.AModule = Convert.ToString(category["MODULE"].InnerText);
                            pAgent.ACallNumber = Convert.ToString(category["NUMBER"].InnerText);
                            pAgent.ACallType = Convert.ToString(category["TYPE"].InnerText);
                            pAgent.ACallLeadId = Convert.ToString(category["LEADID"].InnerText);
                            pAgent.ADni = Convert.ToString(category["DNI"].InnerText);
                            pAgent.ACity = Convert.ToString(category["CITY"].InnerText);
                            pAgent.AState = Convert.ToString(category["STATE"].InnerText);
                            pAgent.ARegion = Convert.ToString(category["REGION"].InnerText);
                            pAgent.AAddon = Convert.ToString(category["ADDON"].InnerText);
                            pAgent.ADialable = Convert.ToString(category["DIALABLE"].InnerText);
                            pAgent.APreviewTime = Convert.ToString(category["PREVIEWTIME"].InnerText);
                            HttpContext.Current.Session["CallNumber"] = pAgent.ACallNumber;
                            pAgent.AUrl = MakeUrl(category);
                            HttpContext.Current.Session["NBUSYCOUNT"] = 0;
                            break;
                        }
                        break;
                    case "4":
                        HttpContext.Current.Session["NBUSYCOUNT"] = 0;
                        HttpContext.Current.Session["CallNumber"] = "";
                        break;
                    case "5":
                        // Call Section
                        foreach (XmlNode category in NodeListCALL)
                        {
                            pAgent.ACli = Convert.ToString(category["CLI"].InnerText);
                            pAgent.AServiceId = Convert.ToString(category["SERVICEID"].InnerText);
                            pAgent.AServiceName = Convert.ToString(category["SERVICENAME"].InnerText);
                            pAgent.AModule = Convert.ToString(category["MODULE"].InnerText);
                            pAgent.ACallNumber = Convert.ToString(category["NUMBER"].InnerText);
                            pAgent.ACallType = Convert.ToString(category["TYPE"].InnerText);
                            pAgent.ACallLeadId = Convert.ToString(category["LEADID"].InnerText);
                            pAgent.ADni = Convert.ToString(category["DNI"].InnerText);
                            pAgent.ACity = Convert.ToString(category["CITY"].InnerText);
                            pAgent.AState = Convert.ToString(category["STATE"].InnerText);
                            pAgent.ARegion = Convert.ToString(category["REGION"].InnerText);
                            pAgent.AAddon = Convert.ToString(category["ADDON"].InnerText);
                            pAgent.ADialable = Convert.ToString(category["DIALABLE"].InnerText);
                            pAgent.APreviewTime = Convert.ToString(category["PREVIEWTIME"].InnerText);
                            HttpContext.Current.Session["CallNumber"] = pAgent.ACallNumber;
                            pAgent.AUrl = MakeUrl(category);
                            HttpContext.Current.Session["NBUSYCOUNT"] = 0;
                            break;
                        }
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            pAgent.ACtiMessage = ex.Message;
        }
        return SerializeObjectIntoJson(pAgent);
    }

    private static string SerializeObjectIntoJson(Agent pAgent)
    {
        System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(pAgent.GetType());
        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
        {
            serializer.WriteObject(ms, pAgent);
            ms.Flush();
            byte[] bytes = ms.GetBuffer();
            string jsonString = System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length).Trim('\0');
            return jsonString;
        }
    }

    [System.Web.Services.WebMethod()]
    public static string CloseCall(string Disposition, string NextDialTime, string Remarks)
    {
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        CtiWS1.SendPbxDigits("", "", "L:all", HttpContext.Current.Session["HOSTID"].ToString());
        //CtiWS1.SendPbxDigits("", "", "R", HttpContext.Current.Session["HOSTID"].ToString());
        CtiWS1.CloseCall("", "", Disposition, NextDialTime, "", Remarks, HttpContext.Current.Session["HOSTID"].ToString());
        return "Success";
    }
    [System.Web.Services.WebMethod()]
    public static string MakeCall(string Dni, string Cli)
    {
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        CtiWS1.MakeCall("", "", Dni, Cli, "", HttpContext.Current.Session["HOSTID"].ToString());
        return "Success";
    }

    [System.Web.Services.WebMethod()]
    public static string Dial(string Number, string callnumber)
    {
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        CtiWS1.Dial("", "", Number, callnumber, HttpContext.Current.Session["HOSTID"].ToString());
        return "Success";
    }

    [System.Web.Services.WebMethod()]
    public static string SendPbxDigits(string Digits)
    {
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        CtiWS1.SendPbxDigits("", "", Digits, HttpContext.Current.Session["HOSTID"].ToString());
        return "Success";
    }
    [System.Web.Services.WebMethod()]
    public static string ChangeStatus(int BreakMode)
    {
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        if (BreakMode == 0 || Convert.ToInt32(HttpContext.Current.Session["NBUSYCOUNT"]) == 1)
        {
            CtiWS1.ChangeStatus("", "", 1, 0, HttpContext.Current.Session["HOSTID"].ToString());
        }
        else
        {
            CtiWS1.ChangeStatus("", "", 2, BreakMode - 1, HttpContext.Current.Session["HOSTID"].ToString());
        }
        return "Success";
    }

    [System.Web.Services.WebMethod()]
    public static string Logout()
    {
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        CtiWS1.Logout("", "", HttpContext.Current.Session["HOSTID"].ToString());
        return "Success";
    }

    [System.Web.Services.WebMethod()]
    public static string Answer()
    {
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        CtiWS1.Answer("", "", HttpContext.Current.Session["HOSTID"].ToString());
        return "Success";
    }

    [System.Web.Services.WebMethod()]
    public static string Decline()
    {
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        CtiWS1.Hangup("", "", HttpContext.Current.Session["HOSTID"].ToString());
        return "Success";
    }

    [System.Web.Services.WebMethod()]
    public static string Hangup()
    {
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        CtiWS1.Hangup("", "", HttpContext.Current.Session["HOSTID"].ToString());
        return "Success";
    }

    [System.Web.Services.WebMethod()]
    public static string Hold()
    {
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        CtiWS1.SendPbxDigits("", "", "H", HttpContext.Current.Session["HOSTID"].ToString());
        //CtiWS1.SendPbxDigits("", "", "C", HttpContext.Current.Session["HOSTID"].ToString());
        // CtiWS1.SendPbxDigits("", "", "R", HttpContext.Current.Session["HOSTID"].ToString());       //return "Success";
        //CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        //CtiWS1.Hold("", "", HttpContext.Current.Session["HOSTID"].ToString());
        return "Success";
    }

    [System.Web.Services.WebMethod()]
    public static string Xfer(string Ip, string CallNumber, string DestiNumber, string TerminalId, string Url)
    {
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        CtiWS1.ScreenTransfer("", "", CallNumber, DestiNumber, HttpContext.Current.Session["HOSTID"].ToString(), "");
        //CtiWS1.ScreenTransfer("", "", Number, "", HttpContext.Current.Session["HOSTID"].ToString());
        return "Success";
    }
    // added by ashish for external tranfer
    [System.Web.Services.WebMethod()]
    public static string XferExternal111(string Number)
    {
        string strNumber = "X" + Number;
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        CtiWS1.SendPbxDigits("", "", strNumber, HttpContext.Current.Session["HOSTID"].ToString());
        //CtiWS1.TransferCall("","", Number, HttpContext.Current.Session["HOSTID"].ToString());
        return "Success";
    }

    [System.Web.Services.WebMethod()]
    public static string Confrence(string Digit)
    {
        CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();
        CtiWS1.SendPbxDigits("", "", "C", HttpContext.Current.Session["HOSTID"].ToString());
        // CtiWS1.SendPbxDigits("", "", "R", HttpContext.Current.Session["HOSTID"].ToString());
        // CtiWS1.Conference("", "", "C", HttpContext.Current.Session["HOSTID"].ToString());
        return "Success";
    }

    [System.Web.Services.WebMethod()]
    public static string Record()
    {
        return "Success";
    }

    private static string MakeUrl(XmlNode category)
    {

        string Url = "";
        int nCallNumber = 0;
        string strCallType = "O";
        int nServiceId = 0;
        int nLeadId = 0;
        int nDialable = 0;
        int nPreviewTime = 0;
        string strCLI = "";
        string strDNI = "";
        string strCity = "Unknown";
        string strState = "Unknown";
        string strRegion = "Unknown";
        string strServiceName = "";
        string strModule = "";
        string strAddon = "";
        // added by ashish
        string strL1 = "";
        string strL2 = "";
        string strL3 = "";
        string strLanguage = "";
        string strAccountType = "";
        string strctype = "";


        if (category["NUMBER"] != null)
            nCallNumber = Convert.ToInt32(category["NUMBER"].InnerText);
        if (category["TYPE"] != null)
            strCallType = Convert.ToString(category["TYPE"].InnerText);
        if (category["SERVICEID"] != null)
            nServiceId = Convert.ToInt32(category["SERVICEID"].InnerText);
        HttpContext.Current.Session["ServiceId"] = nServiceId.ToString();
        if (category["LEADID"] != null)
            nLeadId = Convert.ToInt32(category["LEADID"].InnerText);
        HttpContext.Current.Session["LeadID"] = nLeadId.ToString();
        if (category["DIALABLE"] != null)
            nDialable = Convert.ToInt32(category["DIALABLE"].InnerText);
        if (category["PREVIEWTIME"] != null)
        {
            nPreviewTime = Convert.ToInt32(category["PREVIEWTIME"].InnerText);
            if (nPreviewTime <= 0)
                nPreviewTime = 1;
        }
        if (category["CLI"] != null)
            //strDialPrefix = ConfigurationManager.AppSettings["DIALPREFIX"];
            strCLI = Convert.ToString(category["CLI"].InnerText);
        if (category["DNI"] != null)
            strDNI = Convert.ToString(category["DNI"].InnerText);
        if (category["URL"] != null)
            Url = Convert.ToString(category["URL"].InnerText);
        if (category["CITY"] != null)
            strCity = Convert.ToString(category["CITY"].InnerText);
        if (category["STATE"] != null)
            strState = Convert.ToString(category["STATE"].InnerText);
        if (category["REGION"] != null)
            strRegion = Convert.ToString(category["REGION"].InnerText);
        if (category["SERVICENAME"] != null)
            strServiceName = Convert.ToString(category["SERVICENAME"].InnerText);
        if (category["MODULE"] != null)
            strModule = Convert.ToString(category["MODULE"].InnerText);
        if (category["ADDON"] != null)
            strAddon = Convert.ToString(category["ADDON"].InnerText);

        Url = Url.Replace("~", "#");
        Url = Url.Replace("#CALLNUMBER#", nCallNumber.ToString());
        Url = Url.Replace("#CALLTYPE#", strCallType);
        Url = Url.Replace("#CALLSERVICEID#", nServiceId.ToString());
        Url = Url.Replace("#CALLLEADID#", nLeadId.ToString());
        Url = Url.Replace("#CALLDIALABLE#", nDialable.ToString());
        Url = Url.Replace("#CALLPREVIEWTIME#", nPreviewTime.ToString());
        Url = Url.Replace("#CALLCLI#", strCLI);
        Url = Url.Replace("#CALLDNI#", strDNI);
        Url = Url.Replace("#CALLCITY#", strCity);
        Url = Url.Replace("#CALLSTATE#", strState);
        Url = Url.Replace("#CALLREGION#", strRegion);
        Url = Url.Replace("#CALLSERVICENAME#", strServiceName);
        Url = Url.Replace("#CALLDIALABLE#", nDialable.ToString());
        Url = Url.Replace("#CALLMODULE#", strModule);
        Url = Url.Replace("#CALLADDON#", strAddon);
        Url = Url.Replace("#AGENT#", HttpContext.Current.Session["LOGINID"].ToString());
        //Cti/CallerDetails.aspx?CallNumber=#CALLNUMBER##ServiceId=#CALLSERVICEID##CallServiceName=#CALLSERVICENAME##CallType=#CALLTYPE##LeadId=#CALLLEADID##CLI=#CALLCLI##Addon=#CALLADDON#

        // added by ashish
        // Url = Url.Replace("#Product#", strProduct);
        // Url = Url.Replace("#CardNo#", strCardNo);
        // Url = Url.Replace("#Language#", strLanguage);
        //  Url = Url.Replace("#Category#", strCategory);
        //  Url = Url.Replace("#ctype#", strctype);

        Url = Url.Replace("#L1#", strL1);
        Url = Url.Replace("#L2#", strL2);
        Url = Url.Replace("#L3#", strL3);
        Url = Url.Replace("#Language#", strLanguage);
        Url = Url.Replace("#ctype#", strctype);
        Url = Url.Replace("#AccountType#", strAccountType);



        //Url = Url.Replace("#Product#", "Credit Card");
        //Url = Url.Replace("#CardNo#", "1234567890123456");
        //Url = Url.Replace("#Language#", "English");
        //Url = Url.Replace("#Category#", "Loan Enquiry");
        //Url = Url.Replace("#ctype#", "New");  
        Url = Url.Replace("#", "&");
        return Url;

    }

    public static string GetCLI(string strACli)
    {
        CtiWS.CtiWS CTI = new CtiWS.CtiWS();
        DataTable dt = new DataTable("Login");
        DataBase m_Connection = new DataBase();
        OdbcDataAdapter m_DataAdapterOdbc;
        m_Connection.OpenDB("IDGDB");
        string strTemp = string.Empty;
        if (strACli.Length > 5)
        {
            strTemp = "UPDATE CTI_Agents SET Agent_cli2 = '" + strACli + "' " +
                        "WHERE agent_agent_id = " + Convert.ToString(HttpContext.Current.Session["contact_id"]);
            m_Connection.ExecuteQuery(strTemp);
            CTI.SetCallParameters("", "", "USERFIELD1", "PHONE", HttpContext.Current.Session["HOSTID"].ToString());
        }
        else
            CTI.SetCallParameters("", "", "USERFIELD1", "IVR", HttpContext.Current.Session["HOSTID"].ToString());
        m_Connection.OpenDB("IDGDB");
        strTemp = "SELECT Agent_cli2 FROM CTI_Agents WHERE agent_agent_id = " + Convert.ToString(HttpContext.Current.Session["contact_id"]);
        m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
        if (dt.Rows.Count > 0)
            strACli = Convert.ToString(dt.Rows[0]["Agent_cli2"]);
        CTI.SetCallParameters("", "", "USERFIELD2", strACli, HttpContext.Current.Session["HOSTID"].ToString());
        return strACli;
    }
}