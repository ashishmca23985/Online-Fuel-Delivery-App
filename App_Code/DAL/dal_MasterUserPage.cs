using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Telerik.Web.UI;
using System.Data.Odbc;

/// <summary>
/// Summary description for dal_MasterUserPage
/// </summary>
public class dal_MasterUserPage
{
    string strCon = System.Configuration.ConfigurationManager.AppSettings["Galaxy"].ToString();
    string strInterdialog = System.Configuration.ConfigurationManager.AppSettings["IDGDB"].ToString();
    DataBase m_Connection = new DataBase();
    string strTemp = "";
    OdbcDataAdapter m_DataAdapter;
    OdbcCommand m_Command;
    DataSet m_Dataset = new DataSet();
    int nResultCode = 0;
    string strResult = "";
    Label labMessage = new Label();
    int nid = 0;
	public dal_MasterUserPage()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public DataTable MasterUserPageSearDetails(bo_MasterUserPage MasterUserPageDetails)
    {
        m_Connection.OpenDB("Galaxy");
        {
            DataTable dt1 = new DataTable();
            try
            {
                strTemp = "SELECT * FROM crm_contacts WHERE 1=1 and Contact_type_Id ='E' and Useable='Y' ";
                if (MasterUserPageDetails.LoginName  != string.Empty)
                    strTemp = strTemp + "And Contact_login_id like'" + MasterUserPageDetails.LoginName + "%'";
                if (MasterUserPageDetails.CTIName != string.Empty)
                    strTemp = strTemp + "And agent_name like'" + MasterUserPageDetails.CTIName + "%'";
                if (MasterUserPageDetails.RoleId != string.Empty)
                    strTemp = strTemp + "And Contact_role_id='" + MasterUserPageDetails.RoleId + "'";
                strTemp = strTemp + " order by Created_date desc";
              //  strTemp = strTemp + "order by id desc";
                m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                m_DataAdapter.Fill(dt1);
                m_Connection.CloseDB();
            }
            catch (Exception ex)
            {
                string Result = ex.Message;

            }
            return dt1;
        }
    }

    public int insupDelMasterUserPage(bo_MasterUserPage MasterUserPageDetails)
    {
        using (OdbcConnection con3 = new OdbcConnection(strCon))
        {
            using (OdbcCommand cmd = new OdbcCommand())
            {
                try
                {
                    cmd.Connection = con3;
                    con3.Open();
                    if (MasterUserPageDetails.Type == "INSERT")
                        cmd.CommandText = "insert into crm_contact_details (related_to,related_to_id,Caption,Value,Type) " +
                               "values('USR','" + MasterUserPageDetails.nID + "','Email1','" + MasterUserPageDetails.ContactEmailID + "','Email') "+
                               "insert into crm_contact_details (related_to,related_to_id,Caption,Value,Type) " +
                               "values('USR','" + MasterUserPageDetails.nID + "','Mobile','" + MasterUserPageDetails.ContactContactNumber + "','Phone')";
                    if (MasterUserPageDetails.Type == "UPDATE")
                        cmd.CommandText = "update  crm_contact_details set related_to='USR',Caption='Email1',Value='" + MasterUserPageDetails.ContactEmailID + "',Type='Email' where related_to_id='" + MasterUserPageDetails.nID + "' " +
                             "update  crm_contact_details set related_to='USR',Caption='Mobile',Value='" + MasterUserPageDetails.ContactContactNumber + "',Type='Phone' where related_to_id='" + MasterUserPageDetails.nID + "'";                              
                            return cmd.ExecuteNonQuery();                  
                }
                catch
                {
                    throw;
                }
                finally
                {
                    con3.Close();
                    con3.Dispose();
                }
            }
        }
    }

    public DataTable GetRolId(bo_MasterUserPage MasterUserPageDetails)
    {
        string strConnection=string.Empty ;
        if(MasterUserPageDetails.ForInterdailogCon!="ConInter")  
            strConnection=strCon;              
            else 
            strConnection=strInterdialog;
        using (OdbcConnection con3 = new OdbcConnection(strConnection))
        {
            DataTable dt = new DataTable();
            try
            {
                string strTemp = string.Empty;
                if (MasterUserPageDetails.ForInterdailogCon != "ConInter")
                    strTemp = "SELECT * FROM crm_roles";
                else
                    strTemp = "select agent_agent_id,agent_name,agent_login_id from CTI_Agents where agent_enabled='Y' and agent_login_id<>'Admin' and not exists(select agent_id from " +
                        Convert.ToString(ConfigurationManager.AppSettings["GalaxyDBName"]) + "..crm_contacts where agent_id=agent_agent_id and agent_id<>" +
                        MasterUserPageDetails.CtiAgentId+ ")";
                    //strTemp = "select agent_agent_id,agent_name,agent_login_id from CTI_Agents where agent_enabled='Y'";
                OdbcCommand cmd = new OdbcCommand(strTemp, con3);
                con3.Open();
                using (OdbcDataAdapter oda = new OdbcDataAdapter(cmd))
                {
                    oda.Fill(dt);
                }
                con3.Close();
            }
            catch (Exception ex)
            {
                string Result = ex.Message;
            }
            return dt;
        }
    }

    public DataTable GetCTIAgentAlredyExist(bo_MasterUserPage MasterUserPageDetails)
    {
        string strConnection = "";
        if (MasterUserPageDetails.CtiExistAgent == "ExAgenUp" )
            strConnection = strCon;
        else
            strConnection = strInterdialog;
        using (OdbcConnection con3 = new OdbcConnection(strConnection))
            {
                DataTable dt = new DataTable();
                try
                {
                    string strTemp = string.Empty;
                    if (MasterUserPageDetails.CtiExistAgent == "ExAgenIns")
                        strTemp = "select agent_agent_id,agent_name,agent_login_id from CTI_Agents where agent_enabled='Y' and agent_login_id   in(select agent_name from  " + Convert.ToString(ConfigurationManager.AppSettings["GalaxyDBName"]) + "..crm_contacts where agent_name='" + MasterUserPageDetails.CtiAgentName + "') ";
                    else if (MasterUserPageDetails.CtiExistAgent == "ExAgenUp")

                        strTemp = "select * from " + Convert.ToString(ConfigurationManager.AppSettings["GalaxyDBName"]) + "..crm_contacts where id=" + MasterUserPageDetails.nID + " ";
                    else
                        strTemp = "select agent_agent_id,agent_name,agent_login_id from CTI_Agents where agent_enabled='Y' and agent_login_id   in(select isnull(agent_name,'''') from  " + Convert.ToString(ConfigurationManager.AppSettings["GalaxyDBName"]) + "..crm_contacts) ";
                    OdbcCommand cmd = new OdbcCommand(strTemp, con3);
                    con3.Open();
                    using (OdbcDataAdapter oda = new OdbcDataAdapter(cmd))
                    {
                        oda.Fill(dt);
                    }
                    con3.Close();
                }
                catch (Exception ex)
                {
                    string Result = ex.Message;
                }
                return dt;
            }
    }
    public int ChangePassword(bo_ForLogin ForLogin)
    {
        string strCon = System.Configuration.ConfigurationManager.ConnectionStrings["ConDB"].ToString();
        int VALUE = 0;
        using (OdbcConnection con3 = new OdbcConnection(strCon))
        {
            DataTable dt1 = new DataTable();

            OdbcCommand cmd = null;
            try
            {
                con3.Open();
                cmd = new OdbcCommand("update crm_contacts set contact_password='" + ForLogin.ChangePagePassword + "' where id=" + ForLogin.AgentID + " ", con3);
                return VALUE = cmd.ExecuteNonQuery();
            }
            catch (OdbcException ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:ChangePassword|PageName:WsForLoginDetails|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                objlog.ErrorLog(strResult);
                return -1;
            }
            catch (Exception ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:ChangePassword|PageName:WsForLoginDetails|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                objlog.ErrorLog(strResult);
                return -1;
            }
            finally
            {
                con3.Close();
                con3.Dispose();
            }
        }
    } 
}
