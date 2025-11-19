using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Data.SqlTypes;
using System.Data.Odbc;
using System.IO;
using System.Net;
using System.Configuration;
using System.Xml;
using System.Web.Script.Services;
namespace CtiWS
{
    /// <summary>
    /// Summary description for CtiWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // [ScriptService]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class CtiWS : System.Web.Services.WebService
    {
        [WebMethod(CacheDuration = 0, Description = "This method gets the current status of the Terminal Id from the server.")]
        public DataTable GetStatus(string Ip, string Port, string TerminalId, int nRunningSeries)
        {
            string command = "GETSTATUS&" + TerminalId + "&" + nRunningSeries + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for Login.")]
        public DataTable Login(string Ip, string Port, string LoginId, string Password, string TerminalId, string BaseTerminalId, string Host, int Source)
        {
            string command = "AGENTLOGIN&" + LoginId + "&" + Password + "&" + TerminalId + "&" + BaseTerminalId + "&" + Host + "&" + Source + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for Logout.")]
        public DataTable Logout(string Ip, string Port, string TerminalId)
        {
            string command = "AGENTLOGOUT&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for changing the current status of the Agent : Idle | Break.")]
        public DataTable ChangeStatus(string Ip, string Port, int Status, int Reason, string TerminalId)
        {
            string command = "AGENTSTATUS&" + Status.ToString() + "&" + Reason.ToString() + "&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for Generating a call for the Agent : On demand call without Lead.")]
        public DataTable MakeCall(string Ip, string Port, string Dni, string Cli, string Addon, string TerminalId)
        {
            string command = "AGENTMAKECALL&" + Dni + "&" + Cli + "&" + Addon + "&&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for Generating a call for the Agent : On demand call with Lead.")]
        public DataTable PreviewCall(string Ip, string Port, string Dni, string Cli, int LeadId, string TerminalId)
        {
            string command = "AGENTPREVIEWCALL&" + Dni + "&" + Cli + "&" + LeadId + "&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for setting additional Parameters against a call for further processing.")]
        public DataTable SetCallParameters(string Ip, string Port, string Field, string Value, string TerminalId)
        {
            string command = "AGENTCALLPARAMETERS&" + Field + "&" + Value + "&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for wrap the call with some disposition.")]
        public DataTable CloseCall(string Ip, string Port, string Disposition, string NextDialTime, string VoiceFile, string Remarks, string TerminalId)
        {
            string command = "AGENTCLOSECALL&" + Disposition + "&" + NextDialTime + "&" + VoiceFile + "&" + Remarks + "&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for Generating the same call again.")]
        public DataTable CloseCallAndDialAgain(string Ip, string Port, string Disposition, string NextDialTime, string VoiceFile, string Remarks, string Cli, string TerminalId)
        {
            string command = "AGENTCLOSECALLANDDIALAGAIN&" + Disposition + "&" + NextDialTime + "&" + VoiceFile + "&" + Remarks + "&" + Cli + "&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for sending any commands to the PBX.")]
        public DataTable SendPbxDigits(string Ip, string Port, string Digits, string TerminalId)
        {
            string command = "AGENTSENDPBXDIGITS&" + Digits + "&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }

        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for dialing a number.")]
        public DataTable Dial(string Ip, string Port, string Number, string callnumber, string TerminalId)
        {
            string command = "AGENTDIAL&" + Number + "&" + callnumber + "&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        // added by ashish for CRM Dialing
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for dialing a number.")]
        public DataTable DialCRM(string Ip, string Port, string Number, string TerminalId)
        {
            string command = "AGENTDIAL&" + Number + "&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for Hangup the connected call.")]
        public DataTable Hangup(string Ip, string Port, string TerminalId)
        {
            string command = "AGENTHANGUP&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for transfer the connected call to another number along with screen.")]

        public DataTable ScreenTransfer(string Ip, string Port, string Number, string Number1, string Url, string TerminalId)
        {
            string command = "AGENTSCREENXFER&&" + Number + "&" + TerminalId + "&" + Url + "&";
            return CTIService(Ip, Port, command);
        }

        //public DataTable ScreenTransfer(string Ip, string Port, string Number, string Url, string TerminalId)
        //{
        //    string command = "AGENTSCREENXFER&&" + Number + "&" + TerminalId + "&" + Url + "&";     
        //    return CTIService(Ip, Port, command);
        //}
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for transfer the connected call to another number.")]


        public DataTable TransferCall(string Ip, string Port, string Number, string TerminalId)
        {
            string command = "AGENTXFER&" + "&" + Number + "&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        //public DataTable TransferCall(string Ip, string Port,string strXfer, string Number, string TerminalId)
        //{
        //    string command = "AGENTXFER&" + strXfer + "&" + Number + "&" + TerminalId + "&";
        //    return CTIService(Ip, Port, command);
        //}
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for completing the transfer process.")]
        public DataTable TransferComplete(string Ip, string Port, string TerminalId)
        {
            string command = "AGENTXFERCOMPLETE&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for Holding the connected call.")]
        public DataTable Hold(string Ip, string Port, string TerminalId)
        {
            string command = "AGENTHOLD&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server taking the holded call again.")]
        public DataTable UnHold(string Ip, string Port, string TerminalId)
        {
            string command = "AGENTUNHOLD&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for conference the connected call with another number.")]
        public DataTable Conference(string Ip, string Port, string Number, string TerminalId)
        {
            string command = "AGENTCONFERENCE&" + Number + "&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for completing the conference process.")]
        public DataTable ConferenceComplete(string Ip, string Port, string TerminalId)
        {
            string command = "AGENTCONFERENCECOMPLETE&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for Answering the call.")]
        public DataTable Answer(string Ip, string Port, string TerminalId)
        {
            string command = "AGENTANSWER&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for changing the password of the Agent.")]
        public DataTable ChangePassword(string Ip, string Port, string Password, string TerminalId)
        {
            string command = "AGENTCHANGEPASSWORD&" + Password + "&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for changing the Dialer service of the agent.")]
        public DataTable ChangeService(string Ip, string Port, int Service, string TerminalId)
        {
            string command = "CHANGEDIALERSERVICE&" + Service.ToString() + "&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        [WebMethod(CacheDuration = 0, Description = "This method sends a request to the server for tag the recording against the call.")]
        public DataTable TagRecord(string Ip, string Port, int CallNumber, string Tag1, string Tag2, string Tag3, string Tag4, string Tag5, string StartTime, string Trunk, string TerminalId)
        {
            string command = "AGENTTAGRECORD&" + CallNumber.ToString() + "&" + Tag1 + "&" + Tag2 + "&" + Tag3 + "&" + Tag4 + "&" + Tag5 + "&" + StartTime + "&" + Trunk + "&" + TerminalId + "&";
            return CTIService(Ip, Port, command);
        }
        public DataTable CTIService(string Ip, string Port, string url)
        {
            int ResultCode = -1;
            string ResultString = "Fail";
            Logging clsLogging = new Logging();
            try
            {
                string urlSource = "";
                if (Ip.Length == 0)
                {
                    if (ConfigurationManager.AppSettings.Get("URL_CTISERVER") != null)
                        urlSource = ConfigurationManager.AppSettings.Get("URL_CTISERVER") + url;
                    else
                        return null;
                }
                else
                {
                    if (Port.Length == 0)
                        Port = "9090";
                    urlSource = "http://" + Ip + ":" + Port + "/IDG?cmd=" + url;
                }
                clsLogging.AddLog(urlSource, "TX");
              
                WebRequest req = WebRequest.Create(urlSource);
                WebResponse resp = req.GetResponse();
                Stream stream = resp.GetResponseStream();
                StreamReader readStream = new StreamReader(stream, System.Text.ASCIIEncoding.UTF8);
                ResultString = readStream.ReadToEnd();
              
                clsLogging.AddLog(ResultString, "RX");
                readStream.Close();
                stream.Close();
                resp.Close();
                ResultCode = 0;
            }
            catch (WebException we)
            {
                ResultString = we.Message;
                clsLogging.AddLog(ResultString, "ERR");
            }
            catch (Exception ex)
            {
                ResultString = ex.Message;
                clsLogging.AddLog(ResultString, "ERR");
            }
            return GetResponseTable(ResultCode, ResultString);
        }
        //karan
        #region update lead for close call
        public DataSet UpdateLead(Hashtable hs, int LeadId, int ServiceId)
        {
            if (hs.Count <= 0)
                return null;
            if (LeadId <= 0 && ServiceId <= 0)
                return null;
            DataBase m_Connection = new DataBase();
            string strTemp = "";
            DataSet m_DataSet = new DataSet();
            int ResultCode = 0;
            string ResultString = "Success";
            string LeadTable = "";
            try
            {
                m_Connection.OpenDB("IDGDB");
                strTemp = "SELECT TOP 1 service_outbound_lead_db_name + '..' + service_leadstructure_master_tablename " +
                            "FROM cti_services WHERE Service_Id = " + ServiceId;
                OdbcCommand m_command = new OdbcCommand(strTemp, m_Connection.oCon);
                LeadTable = Convert.ToString(m_command.ExecuteScalar());

                strTemp = "UPDATE " + LeadTable + " SET ";
                string pTemp = "";
                foreach (DictionaryEntry entry in hs)
                {
                    if (pTemp.Length <= 0)
                        pTemp = entry.Key + " = '" + entry.Value + "'";
                    else
                        pTemp = pTemp + ", " + entry.Key + " = '" + entry.Value + "'";
                }
                strTemp = strTemp + pTemp + " WHERE Lead_Id = " + LeadId + " AND lead_service_id = " + ServiceId;
                m_command = new OdbcCommand(strTemp, m_Connection.oCon);
                m_command.ExecuteNonQuery();
            }
            catch (OdbcException ex)
            {
                ResultCode = -1;
                ResultString = ex.Message;
            }
            catch (Exception ex)
            {
                ResultCode = -1;
                ResultString = ex.Message;
            }
            finally
            {
                m_Connection.CloseDB();
            }
            m_DataSet.Tables.Add(GetResponseTable(ResultCode, ResultString));
            return m_DataSet;
        }
        #endregion
        #region GetCTIStatus
        [WebMethod(CacheDuration = 0, Description = "This method can be use to get current status of the agent along with some additional details like history of the call and lead data also.\n\rStored Procedure-idg_sp_gethistory")]
        public DataSet GetCTIStatus(string Ip, string Port, string terminalID, string oldStatusNumber)
        {
            DataBase m_Connection = new DataBase();
            string strTemp = "";
            OdbcDataAdapter m_OdbcDataAdapter;
            DataTable dtTable;
            DataSet m_DataSet = new DataSet();
            int ResultCode = -1;
            string ResultString = "Fail";
            try
            {
                string urlSource = "";
                if (Ip.Length == 0)
                {
                    if (ConfigurationManager.AppSettings.Get("URL_CTISERVER") != null)
                        urlSource = ConfigurationManager.AppSettings.Get("URL_CTISERVER") + "GETSTATUS&" + terminalID + "&";
                    else
                        return null;
                }
                else
                {
                    if (Port.Length == 0)
                        Port = "9090";
                    urlSource = "http://" + Ip + ":" + Port + "/IDG?cmd=" + "GETSTATUS&" + terminalID + "&";
                }

                WebRequest req = WebRequest.Create(urlSource);
                WebResponse resp = req.GetResponse();
                Stream stream = resp.GetResponseStream();
                StreamReader readStream = new StreamReader(stream, System.Text.ASCIIEncoding.UTF8);
                string status = readStream.ReadToEnd();
                readStream.Close();
                stream.Close();
                resp.Close();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(status);
                XmlElement root = doc.DocumentElement;
                XmlNodeList NodeListINFO = root.GetElementsByTagName("INFO");
                XmlNodeList NodeListCALL = root.GetElementsByTagName("CALL");
                XmlNodeList NodeListPORT = root.GetElementsByTagName("PORT");
                XmlNode marquee = root.LastChild;
                string strMarqueeText = marquee.InnerText;
                if (NodeListINFO.Count > 0)
                {
                    string strInfoUser = "";
                    string strInfoTerminal = "";
                    string strInfoReason = "";
                    string strInfoSubReason = "";
                    string strInfoText = "";
                    string strInfoDuration = "";
                    string strInfoSeries = "";
                    XmlNode category = NodeListINFO.Item(0);
                    strInfoUser = category["USER"].InnerText;
                    strInfoTerminal = category["TERMINAL"].InnerText;
                    strInfoReason = category["REASON"].InnerText;
                    strInfoSubReason = category["SUBREASON"].InnerText;
                    strInfoText = category["TEXT"].InnerText;
                    strInfoDuration = category["DURATION"].InnerText;
                    strInfoSeries = category["SERIES"].InnerText;
                    m_Connection.OpenDB("IDGDB");
                    strTemp = "SELECT " +
                                "'" + strInfoUser + "' as InfoUser, " +
                                "'" + strInfoTerminal + "' as InfoTerminal, " +
                                "'" + strInfoReason + "' as InfoReason, " +
                                "'" + strInfoSubReason + "' as InfoSubReason, " +
                                "'" + strInfoText + "' as InfoText, " +
                                "'" + strInfoDuration + "' as InfoDuration, " +
                                "'" + strInfoSeries + "' as InfoSeries, " +
                                "'" + strMarqueeText + "' as marque_text ";
                    dtTable = new DataTable("INFO");
                    m_OdbcDataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                    m_OdbcDataAdapter.Fill(dtTable);
                    m_OdbcDataAdapter.Dispose();
                    if (dtTable.Rows.Count > 0)
                        m_DataSet.Tables.Add(dtTable);
                    if (!string.IsNullOrEmpty(strInfoSeries) && strInfoSeries != oldStatusNumber)
                    {
                        if (strInfoReason == "3" || strInfoReason == "5")
                        {
                            if (NodeListCALL.Count > 0)
                            {
                                string strCallNumber = "";
                                string strCallType = "";
                                string strCallServiceId = "";
                                string strCallLeadId = "";
                                string strCallPreviewTime = "";
                                string strCallCli = "";
                                string strCallDni = "";
                                string strCallUrl = "";
                                string strCallCity = "";
                                string strCallState = "";
                                string strCallRegion = "";
                                string strCallServiceName = "";
                                string strCallModule = "";
                                string strCallAddon = "";
                                category = NodeListCALL.Item(0);
                                strCallNumber = category["NUMBER"].InnerText;
                                strCallType = category["TYPE"].InnerText;
                                strCallServiceId = category["SERVICEID"].InnerText;
                                strCallLeadId = category["LEADID"].InnerText;
                                strCallPreviewTime = category["PREVIEWTIME"].InnerText;
                                strCallCli = category["CLI"].InnerText;
                                strCallDni = category["DNI"].InnerText;
                                strCallUrl = category["URL"].InnerText;
                                strCallCity = category["CITY"].InnerText;
                                strCallState = category["STATE"].InnerText;
                                strCallRegion = category["REGION"].InnerText;
                                strCallServiceName = category["SERVICENAME"].InnerText;
                                strCallModule = category["MODULE"].InnerText;
                                strCallAddon = category["ADDON"].InnerText;
                                strTemp = "SELECT " +
                                            "'" + strCallNumber + "' as CallNumber, " +
                                            "'" + strCallType + "' as CallType, " +
                                            "'" + strCallServiceId + "' as CallServiceId, " +
                                            "'" + strCallLeadId + "' as CallLeadId, " +
                                            "'" + strCallPreviewTime + "' as CallPreviewTime, " +
                                            "'" + strCallCli + "' as CallCli, " +
                                            "'" + strCallDni + "' as CallDni, " +
                                            "'" + strCallUrl + "' as CallUrl, " +
                                            "'" + strCallCity + "' as CallCity, " +
                                            "'" + strCallState + "' as CallState, " +
                                            "'" + strCallRegion + "' as CallRegion, " +
                                            "'" + strCallServiceName + "' as CallServiceName, " +
                                            "'" + strCallModule + "' as CallModule, " +
                                            "'" + strCallAddon + "' as CallAddon";
                                dtTable = new DataTable("CALL");
                                m_OdbcDataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                                m_OdbcDataAdapter.Fill(dtTable);
                                m_OdbcDataAdapter.Dispose();
                                if (dtTable.Rows.Count > 0)
                                    m_DataSet.Tables.Add(dtTable);

                                if (ConfigurationManager.AppSettings.Get("STATUSWITHLEADHISTORY") != null && ConfigurationManager.AppSettings.Get("STATUSWITHLEADHISTORY") == "Y")
                                {
                                    if (System.Convert.ToInt32(strCallLeadId) > 0)
                                    {
                                        if (m_Connection.nDatabaseType == 2)
                                            strTemp = "select concat(service_outbound_lead_db_name, '.', service_leadstructure_master_tablename) from cti_services where service_id=" + System.Convert.ToInt32(strCallServiceId) + " LIMIT 1";
                                        else
                                            strTemp = "select TOP 1 service_outbound_lead_db_name + '..' + service_leadstructure_master_tablename from cti_services where service_id=" + System.Convert.ToInt32(strCallServiceId);

                                        OdbcCommand selectCommand = new OdbcCommand(strTemp, m_Connection.oCon);
                                        string ServiceLeadTableName = Convert.ToString(selectCommand.ExecuteScalar());
                                        selectCommand.Dispose();
                                        if (ServiceLeadTableName.Length > 0)
                                        {
                                            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " * " +
                                                       "FROM " + ServiceLeadTableName + " " +
                                                       "WHERE lead_id=" + strCallLeadId + " AND lead_service_id = " + strCallServiceId + " " + m_Connection.DB_TOP_MYSQL;
                                            dtTable = new DataTable("LeadDetails");
                                            m_OdbcDataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                                            m_OdbcDataAdapter.Fill(dtTable);
                                            m_OdbcDataAdapter.Dispose();
                                            if (dtTable.Rows.Count > 0)
                                            {
                                                m_DataSet.Tables.Add(dtTable);
                                                // Fill Lead History
                                                string strCallMasterTableNames = dtTable.Rows[0]["lead_callmaster_table"].ToString();
                                                if (strCallMasterTableNames.Length > 0)
                                                {
                                                    if (m_Connection.nDatabaseType == 1)
                                                        strTemp = "EXEC idg_sp_gethistory " + System.Convert.ToInt32(strCallLeadId) + "," + System.Convert.ToInt32(strCallServiceId) + ",'" + strCallMasterTableNames + "'";
                                                    else
                                                        strTemp = "CALL idg_sp_gethistory(" + System.Convert.ToInt32(strCallLeadId) + "," + System.Convert.ToInt32(strCallServiceId) + ",'" + strCallMasterTableNames + "')";
                                                    dtTable = new DataTable("LeadHistory");
                                                    m_OdbcDataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                                                    m_OdbcDataAdapter.Fill(dtTable);
                                                    m_OdbcDataAdapter.Dispose();
                                                    if (dtTable.Rows.Count > 0)
                                                        m_DataSet.Tables.Add(dtTable);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    ResultCode = 0;
                    ResultString = "Pass";
                }
            }
            catch (OdbcException ex)
            {
                ResultString = ex.Message;
            }
            catch (Exception ex)
            {
                ResultString = ex.Message;
            }
            finally
            {
                m_Connection.CloseDB();
            }
            m_DataSet.Tables.Add(GetResponseTable(ResultCode, ResultString));
            return m_DataSet;
        }
        #endregion
        #region GetServiceList(int AgentId)
        [WebMethod(CacheDuration = 0, Description = "This method can be use to get all the services list configured in server.\n\rStored Procedure-idg_sp_getagentservices")]
        public DataSet GetServiceList(string strLoginId)
        {
            DataBase m_Connection = new DataBase();
            string strTemp = "";
            DataSet m_DataSet = new DataSet();
            int ResultCode = -1;
            string ResultString = "Fail - No records found";
            try
            {
                m_Connection.OpenDB("IDGDB");
                strTemp = "SELECT service_name, service_id, service_from_dni " +
                            "FROM CTI_services, cti_agent_services " +
                            "WHERE service_enabled = 'Y' " +
                            "AND agserv_service_id = service_id " +
                            "and agserv_agent_id = " + strLoginId + " " +
                            "and agserv_allow_manual_calls = 'Y'";
                OdbcDataAdapter DbAdaptor = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                if (DbAdaptor.Fill(m_DataSet) > 0)
                {
                    ResultCode = 0;
                    ResultString = "Pass";
                    m_DataSet.Tables[0].TableName = "ServiceList";
                }
                DbAdaptor.Dispose();
            }
            catch (OdbcException ex)
            {
                ResultString = ex.Message;
            }
            catch (Exception ex)
            {
                ResultString = ex.Message;
            }
            finally
            {
                m_Connection.CloseDB();
            }
            m_DataSet.Tables.Add(GetResponseTable(ResultCode, ResultString));
            return m_DataSet;
        }
        #endregion
        #region GetServiceGroupDispositions(int ServiceId)
        [WebMethod(CacheDuration = 0, Description = "This method can be use to get group dispositions of the service.")]
        public DataTable GetServiceGroupDispositions(int ServiceId, string Module)
        {
            DataBase m_Connection = new DataBase();
            string strTemp = "";
            DataSet m_DataSet = new DataSet();
            int ResultCode = -1;
            string ResultString = "Fail - No records found";
            try
            {
                m_Connection.OpenDB("IDGDB");
                if (m_Connection.nDatabaseType == 2)
                    strTemp = "SELECT DISTINCT servdesp_parent_desp_code AS Code, servdesp_parent_desp_desc AS Description ";
                else
                    strTemp = "SELECT DISTINCT servdesp_parent_desp_code AS Code, servdesp_parent_desp_desc AS Description ";
                strTemp += "FROM cti_service_desposition " +
                    "WHERE servdesp_service_id = " + ServiceId + " " +
                    "AND servdesp_desp_agent = 'Y' ";
                if (Module.Length > 0)
                {
                    if (m_Connection.nDatabaseType == 2)
                        strTemp += "AND concat(',',servdesp_modules,',') like '%," + Module + ",%' ";
                    else
                        strTemp += "AND ',' + servdesp_modules + ',' like '%," + Module + ",%' ";
                }
                strTemp += "ORDER BY servdesp_parent_desp_desc";

                OdbcDataAdapter DbAdaptor = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                if (DbAdaptor.Fill(m_DataSet) > 0)
                {
                    ResultCode = 0;
                    ResultString = "Pass";
                    m_DataSet.Tables[0].TableName = "GroupDispositions";
                }
                DbAdaptor.Dispose();
            }
            catch (OdbcException ex)
            {
                ResultString = ex.Message;
            }
            catch (Exception ex)
            {
                ResultString = ex.Message;
            }
            finally
            {
                m_Connection.CloseDB();
            }
            m_DataSet.Tables.Add(GetResponseTable(ResultCode, ResultString));
            return m_DataSet.Tables["GroupDispositions"];
        }
        #endregion
        #region GetServiceSubDispositions(int ServiceId, String DispositionType)
        [WebMethod(CacheDuration = 0, Description = "This method can be use to get all dispositions associated with selected group dispositions of the service.")]
        public DataTable GetServiceSubDispositions(int ServiceId, string DispositionType, string Module)
        {
            DataBase m_Connection = new DataBase();
            string strTemp = "";
            DataSet m_DataSet = new DataSet();
            int ResultCode = -1;
            string ResultString = "Fail - No records found";
            try
            {
                m_Connection.OpenDB("IDGDB");
                strTemp = "SELECT servdesp_desp_code AS Code, servdesp_bucket_code AS Bucket, servdesp_desp_desc AS Description " +
                            "FROM cti_service_desposition " +
                            "WHERE servdesp_service_id = " + ServiceId + " " +
                            "AND servdesp_desp_agent = 'Y' " +
                            "AND servdesp_parent_desp_code = '" + DispositionType + "' ";
                if (Module.Length > 0)
                {
                    if (m_Connection.nDatabaseType == 2)
                        strTemp += "AND concat(',',servdesp_modules,',') like '%," + Module + ",%' ";
                    else
                        strTemp += "AND ',' + servdesp_modules + ',' like '%," + Module + ",%' ";
                }
                strTemp += "ORDER BY servdesp_desp_code";
                OdbcDataAdapter DbAdaptor = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                if (DbAdaptor.Fill(m_DataSet) > 0)
                {
                    ResultCode = 0;
                    ResultString = "Pass";
                    m_DataSet.Tables[0].TableName = "SubDispositions";
                }
                DbAdaptor.Dispose();
            }
            catch (OdbcException ex)
            {
                ResultString = ex.Message;
            }
            catch (Exception ex)
            {
                ResultString = ex.Message;
            }
            finally
            {
                m_Connection.CloseDB();
            }
            m_DataSet.Tables.Add(GetResponseTable(ResultCode, ResultString));

            return m_DataSet.Tables["SubDispositions"];
        }
        #endregion
        #region GetBreakModes()
        [WebMethod(CacheDuration = 0, Description = "This method can be use to get break modes configured in the server.")]
        public DataSet GetBreakModes()
        {
            DataBase m_Connection = new DataBase();
            string strTemp = "";
            DataSet m_DataSet = new DataSet();
            int ResultCode = -1;
            string ResultString = "Fail - No records found";
            try
            {
                m_Connection.OpenDB("IDGDB");
                //strTemp = "SELECT " + m_Connection.DB_TOP_SQL +
                //                " ltrim(rtrim(CASE WHEN busy_header_1 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_1) <= 0 THEN 'busy1' ELSE busy_header_1 END)) AS Break1, " +
                //                "(CASE WHEN busy_duration_1 IS NULL OR busy_duration_1 <= 0 THEN 15 ELSE busy_duration_1 END) AS Break1Duration, " +
                //                "1 AS Break1Id, " +
                //                "ltrim(rtrim(CASE WHEN busy_header_2 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_2) <= 0 THEN 'busy2' ELSE busy_header_2 END))  AS Break2, " +
                //                "(CASE WHEN busy_duration_2 IS NULL OR busy_duration_2 <= 0 THEN 15 ELSE busy_duration_2 END) AS Break2Duration, " +
                //                "2 AS Break2Id, " +
                //                "ltrim(rtrim(CASE WHEN busy_header_3 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_3) <= 0 THEN 'busy3' ELSE busy_header_3 END)) AS Break3, " +
                //                "(CASE WHEN busy_duration_3 IS NULL OR busy_duration_3 <= 0 THEN 15 ELSE busy_duration_3 END)  AS Break3Duration, " +
                //                "3 AS Break3Id, " +
                //                "ltrim(rtrim(CASE WHEN busy_header_4 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_4) <= 0 THEN 'busy4' ELSE busy_header_4 END)) AS Break4, " +
                //                "(CASE WHEN busy_duration_4 IS NULL OR busy_duration_4 <= 0 THEN 15 ELSE busy_duration_4 END)  AS Break4Duration, " +
                //                "4 AS Break4Id, " +
                //                "ltrim(rtrim(CASE WHEN busy_header_5 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_5) <= 0 THEN 'Unsolicited' ELSE busy_header_5 END))  AS Break5, " +
                //                "(CASE WHEN busy_duration_5 IS NULL OR busy_duration_5 <= 0 THEN 15 ELSE busy_duration_5 END)  AS Break5Duration, " +
                //                "5  AS Break5Id " +
                //                "FROM CTI_parameters " + m_Connection.DB_TOP_MYSQL;
                strTemp = "SELECT " + m_Connection.DB_TOP_SQL +

" ltrim(rtrim(CASE WHEN busy_header_1 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_1) <= 0 THEN 'busy1' ELSE busy_header_1 END)) AS Break1, " +

"(CASE WHEN busy_duration_1 IS NULL OR busy_duration_1 <= 0 THEN 15 ELSE busy_duration_1 END) AS Break1Duration, " +

"1 AS Break1Id, " +

"ltrim(rtrim(CASE WHEN busy_header_2 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_2) <= 0 THEN 'busy2' ELSE busy_header_2 END)) AS Break2, " +

"(CASE WHEN busy_duration_2 IS NULL OR busy_duration_2 <= 0 THEN 15 ELSE busy_duration_2 END) AS Break2Duration, " +

"2 AS Break2Id, " +

"ltrim(rtrim(CASE WHEN busy_header_3 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_3) <= 0 THEN 'busy3' ELSE busy_header_3 END)) AS Break3, " +

"(CASE WHEN busy_duration_3 IS NULL OR busy_duration_3 <= 0 THEN 15 ELSE busy_duration_3 END) AS Break3Duration, " +

"3 AS Break3Id, " +

"ltrim(rtrim(CASE WHEN busy_header_4 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_4) <= 0 THEN 'busy4' ELSE busy_header_4 END)) AS Break4, " +

"(CASE WHEN busy_duration_4 IS NULL OR busy_duration_4 <= 0 THEN 15 ELSE busy_duration_4 END) AS Break4Duration, " +

"4 AS Break4Id, " +

"ltrim(rtrim(CASE WHEN busy_header_5 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_5) <= 0 THEN 'Unsolicited' ELSE busy_header_5 END)) AS Break5, " +

"(CASE WHEN busy_duration_5 IS NULL OR busy_duration_5 <= 0 THEN 15 ELSE busy_duration_5 END) AS Break5Duration, " +

"5 AS Break5Id, " +

"ltrim(rtrim(CASE WHEN busy_header_6 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_6) <= 0 THEN 'busy6' ELSE busy_header_6 END)) AS Break6, " +

"(CASE WHEN busy_duration_6 IS NULL OR busy_duration_6 <= 0 THEN 15 ELSE busy_duration_6 END) AS Break6Duration, " +

"6 AS Break6Id, " +

"ltrim(rtrim(CASE WHEN busy_header_7 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_7) <= 0 THEN 'busy7' ELSE busy_header_7 END)) AS Break7, " +

"(CASE WHEN busy_duration_7 IS NULL OR busy_duration_7 <= 0 THEN 15 ELSE busy_duration_7 END) AS Break7Duration, " +

"7 AS Break7Id, " +

"ltrim(rtrim(CASE WHEN busy_header_8 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_8) <= 0 THEN 'busy8' ELSE busy_header_8 END)) AS Break8, " +

"(CASE WHEN busy_duration_8 IS NULL OR busy_duration_8 <= 0 THEN 15 ELSE busy_duration_8 END) AS Break8Duration, " +

"8 AS Break8Id, " +

"ltrim(rtrim(CASE WHEN busy_header_9 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_9) <= 0 THEN 'busy9' ELSE busy_header_9 END)) AS Break9, " +

"(CASE WHEN busy_duration_9 IS NULL OR busy_duration_9 <= 0 THEN 15 ELSE busy_duration_9 END) AS Break9Duration, " +

"9 AS Break9Id, " +

"ltrim(rtrim(CASE WHEN busy_header_10 IS NULL OR " + m_Connection.DB_DATALENGTH + "(busy_header_10) <= 0 THEN 'busy10' ELSE busy_header_10 END)) AS Break10, " +

"(CASE WHEN busy_duration_10 IS NULL OR busy_duration_10 <= 0 THEN 15 ELSE busy_duration_10 END) AS Break10Duration, " +

"10 AS Break10Id " +

"FROM CTI_parameters " + m_Connection.DB_TOP_MYSQL;
                OdbcDataAdapter DbAdaptor = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                if (DbAdaptor.Fill(m_DataSet) > 0)
                {
                    ResultCode = 0;
                    ResultString = "Pass";
                    m_DataSet.Tables[0].TableName = "BreakModes";
                }
                DbAdaptor.Dispose();
            }
            catch (OdbcException ex)
            {
                ResultString = ex.Message;
            }
            catch (Exception ex)
            {
                ResultString = ex.Message;
            }
            finally
            {
                m_Connection.CloseDB();
            }
            m_DataSet.Tables.Add(GetResponseTable(ResultCode, ResultString));
            return m_DataSet;
        }
        #endregion
        #region GetServiceParameters(int ServiceId, string ParameterType)
        [WebMethod(CacheDuration = 0, Description = "This method can be use to get configured global parameters of the service.")]
        public DataSet GetServiceParameters(int ServiceId, string ParameterType)
        {
            DataBase m_Connection = new DataBase();
            string strTemp = "";
            DataSet m_DataSet = new DataSet();
            int ResultCode = -1;
            string ResultString = "Fail - No records found";
            try
            {
                m_Connection.OpenDB("IDGDB");
                strTemp = "SELECT param_field_value AS Parameter, param_field_itemdata AS ParameterValue " +
                    "FROM CTI_Service_Parameters " +
                    "WHERE param_service_id = " + ServiceId + " " +
                    "AND param_field_name = '" + ParameterType + "' " +
                    "ORDER BY param_field_display_sequence";
                OdbcDataAdapter DbAdaptor = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                if (DbAdaptor.Fill(m_DataSet) > 0)
                {
                    ResultCode = 0;
                    ResultString = "Pass";
                    m_DataSet.Tables[0].TableName = "GroupDispositions";
                }
                DbAdaptor.Dispose();
            }
            catch (OdbcException ex)
            {
                ResultString = ex.Message;
            }
            catch (Exception ex)
            {
                ResultString = ex.Message;
            }
            finally
            {
                m_Connection.CloseDB();
            }
            m_DataSet.Tables.Add(GetResponseTable(ResultCode, ResultString));
            return m_DataSet;
        }
        #endregion
        #region GetLeadDetails
        [WebMethod(CacheDuration = 0, Description = "This method can be use to get lead details with some additional details like history of the call and lead data also.\n\rStored Procedure-idg_sp_gethistory")]
        public DataSet GetLeadDetails(int nServiceId, int nLeadId, string ColumnFilters)
        {
            DataBase m_Connection = new DataBase();
            string strTemp = "";
            OdbcDataAdapter m_OdbcDataAdapter;
            DataTable dtTable;
            DataSet m_DataSet = new DataSet();
            int ResultCode = -1;
            string ResultString = "Fail";
            try
            {
                if (nLeadId > 0 && nServiceId > 0)
                {
                    m_Connection.OpenDB("IDGDB");
                    if (m_Connection.nDatabaseType == 2)
                        strTemp = "select concat(service_outbound_lead_db_name, '.', service_leadstructure_master_tablename) from cti_services where service_id=" + nServiceId + " LIMIT 1";
                    else
                        strTemp = "select TOP 1 service_outbound_lead_db_name + '..' + service_leadstructure_master_tablename from cti_services where service_id=" + nServiceId;
                    OdbcCommand selectCommand = new OdbcCommand(strTemp, m_Connection.oCon);
                    string ServiceLeadTableName = Convert.ToString(selectCommand.ExecuteScalar());
                    selectCommand.Dispose();
                    if (ServiceLeadTableName.Length > 0)
                    {
                        if (ColumnFilters.Length > 0)
                        {
                            if (ColumnFilters.Contains("lead_callmaster_table") == false)
                                ColumnFilters += ",lead_callmaster_table";

                            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " " + ColumnFilters + " " +
                                       "FROM " + ServiceLeadTableName + " " +
                                       "WHERE lead_id=" + nLeadId + " AND lead_service_id = " + nServiceId + " " + m_Connection.DB_TOP_MYSQL;
                        }
                        else
                            strTemp = "SELECT " + m_Connection.DB_TOP_SQL + " * " +
                                       "FROM " + ServiceLeadTableName + " " +
                                       "WHERE lead_id=" + nLeadId + " AND lead_service_id = " + nServiceId + " " + m_Connection.DB_TOP_MYSQL;
                        dtTable = new DataTable("LeadDetails");
                        m_OdbcDataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                        m_OdbcDataAdapter.Fill(dtTable);
                        m_OdbcDataAdapter.Dispose();
                        if (dtTable.Rows.Count > 0)
                        {
                            m_DataSet.Tables.Add(dtTable);
                            // Fill Lead History
                            string strCallMasterTableNames = dtTable.Rows[0]["lead_callmaster_table"].ToString();
                            if (strCallMasterTableNames.Length > 0)
                            {
                                if (m_Connection.nDatabaseType == 1)
                                    strTemp = "EXEC idg_sp_gethistory " + nLeadId + "," + nServiceId + ",'" + strCallMasterTableNames + "'";
                                else
                                    strTemp = "CALL idg_sp_gethistory(" + nLeadId + "," + nServiceId + ",'" + strCallMasterTableNames + "')";
                                dtTable = new DataTable("LeadHistory");
                                m_OdbcDataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                                m_OdbcDataAdapter.Fill(dtTable);
                                m_OdbcDataAdapter.Dispose();
                                if (dtTable.Rows.Count > 0)
                                    m_DataSet.Tables.Add(dtTable);
                            }
                        }
                    }
                }
                ResultCode = 0;
                ResultString = "Pass";
            }
            catch (OdbcException ex)
            {
                ResultString = ex.Message;
            }
            catch (Exception ex)
            {
                ResultString = ex.Message;
            }
            finally
            {
                m_Connection.CloseDB();
            }
            m_DataSet.Tables.Add(GetResponseTable(ResultCode, ResultString));
            return m_DataSet;
        }
        #endregion
        #region GetDatabaseType
        [WebMethod(CacheDuration = 0, Description = "This method can be use to get current database type (SQL / MYSQL)")]
        public int GetDatabaseType()
        {
            int ResultCode = -1;
            DataBase m_Connection = new DataBase();
            try
            {
                m_Connection.OpenDB("IDGDB");
                ResultCode = m_Connection.nDatabaseType;
            }
            catch (OdbcException ex)
            {
            }
            catch (Exception ex)
            {
            }
            finally
            {
                m_Connection.CloseDB();
            }
            return ResultCode;
        }
        #endregion
        #region GetTerminalInfo()
        [WebMethod(CacheDuration = 0, Description = "This method can be use to get terminal info configured in the system for the Host Name or Host IP.")]
        public DataSet GetTerminalInfo(string HostName, string HostIp)
        {
            DataBase m_Connection = new DataBase();
            string strTemp = "";
            DataSet m_DataSet = new DataSet();
            int ResultCode = -1;
            string ResultString = "Fail - No records found";
            try
            {
                m_Connection.OpenDB("IDGDB");
                strTemp = "SELECT " + m_Connection.DB_TOP_SQL +
                                           m_Connection.DB_NULL + "(Terminal_Id, '') AS Terminal_Id " +
                                           "FROM Cti_Machine_Master " +
                                           "WHERE Machine_Name = '" + HostName + "' OR Machine_Name = '" + HostIp + "' " + m_Connection.DB_TOP_MYSQL;

                OdbcDataAdapter DbAdaptor = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                if (DbAdaptor.Fill(m_DataSet) > 0)
                {
                    ResultCode = 0;
                    ResultString = "Pass";
                }
                m_DataSet.Tables[0].TableName = "TerminalInfo";
                DbAdaptor.Dispose();
            }
            catch (OdbcException ex)
            {
                ResultString = ex.Message;
            }
            catch (Exception ex)
            {
                ResultString = ex.Message;
            }
            finally
            {
                m_Connection.CloseDB();
            }
            m_DataSet.Tables.Add(GetResponseTable(ResultCode, ResultString));
            return m_DataSet;
        }
        #endregion
        #region GetAgentInfo()
        [WebMethod(CacheDuration = 0, Description = "This method can be use to get agent info.")]
        public DataSet GetAgentInfo(int UserId)
        {
            DataBase m_Connection = new DataBase();
            string strTemp = "";
            DataSet m_DataSet = new DataSet();
            int ResultCode = -1;
            string ResultString = "Fail - No records found";
            try
            {
                m_Connection.OpenDB("MENUCONNECTIONSTRING");
                strTemp = "SELECT " + m_Connection.DB_TOP_SQL +
                                           m_Connection.DB_NULL + "(user_associated_with_agent, 0) AS AssociatedAgent, " +
                                           m_Connection.DB_NULL + "(user_associated_with_agent_termid, '') AS AssociatedAgentTerminal " +
                                           "FROM IDG_Users " +
                                           "WHERE user_id = " + UserId + " " + m_Connection.DB_TOP_MYSQL;
                OdbcDataAdapter DbAdaptor = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                if (DbAdaptor.Fill(m_DataSet) > 0)
                {
                    ResultCode = 0;
                    ResultString = "Pass";
                }
                m_DataSet.Tables[0].TableName = "UserInfo";
                m_Connection.CloseDB();
                m_Connection.OpenDB("IDGDB");
                strTemp = "SELECT " + m_Connection.DB_TOP_SQL +
                                           m_Connection.DB_NULL + "(agent_login_id, '') AS Login_Id, " +
                                           m_Connection.DB_NULL + "(agent_login_password, '') AS Login_Password " +
                                           "FROM Cti_Agents " +
                                           "WHERE agent_agent_id = " + m_DataSet.Tables["UserInfo"].Rows[0]["AssociatedAgent"].ToString() + " " + m_Connection.DB_TOP_MYSQL;

                DataTable dt = new DataTable("AgentInfo");
                DbAdaptor = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                if (DbAdaptor.Fill(dt) > 0)
                {
                    ResultCode = 0;
                    ResultString = "Pass";
                }
                m_DataSet.Tables.Add(dt);
                DbAdaptor.Dispose();
            }
            catch (OdbcException ex)
            {
                ResultString = ex.Message;
            }
            catch (Exception ex)
            {
                ResultString = ex.Message;
            }
            finally
            {
                m_Connection.CloseDB();
            }
            m_DataSet.Tables.Add(GetResponseTable(ResultCode, ResultString));
            return m_DataSet;
        }
        #endregion
        #region Response Table
        private DataTable GetResponseTable(long nResultCode, string strResult)
        {
            DataTable dtTable = new DataTable("Response");

            DataColumn dc;
            dc = new DataColumn();
            dc.DataType = System.Type.GetType("System.String");
            dc.ColumnName = "ResultCode";
            dtTable.Columns.Add(dc);
            dc = new DataColumn();
            dc.DataType = System.Type.GetType("System.String");
            dc.ColumnName = "ResultString";
            dtTable.Columns.Add(dc);
            DataRow dr = dtTable.NewRow();
            dr[0] = nResultCode.ToString();
            dr[1] = strResult;
            dtTable.Rows.Add(dr);
            return dtTable;
        }
        #endregion
    }
}