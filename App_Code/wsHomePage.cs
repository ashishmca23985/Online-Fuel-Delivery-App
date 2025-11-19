using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Data.Odbc;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Telerik.Web.UI;
using System.Text;
using System.Drawing;
using System.Web.Script.Services;
using AjaxControlToolkit; 

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

public class wsHomePage : System.Web.Services.WebService
{
    [WebMethod]
    public DataTable HomeMenu(bo_homepage obj_bo_homepage)
    {
        string strCon = System.Configuration.ConfigurationManager.ConnectionStrings["IDGDB"].ToString();
        using (OdbcConnection con3 = new OdbcConnection(strCon))
        {
            DataTable dt1 = new DataTable();

            try
            {
                string strId = "2001";
                string strTemp = "SELECT activity_id,activity_name,activity_description, " +
                     "activity_image_id as image_path," +
                    "activity_action_url," +
                    "activity_action_container, " +
                    " AccessKey " +
                    "FROM CRM_activity_master,CRM_role_activity " +
                    "WHERE activity_parent_id='" + strId + "' " +
                      " AND activity_id=roledet_activity_id " +
                    " AND activity_toolbar='Y' " +
                  "  and roledet_role_id in ('" + obj_bo_homepage.RoleID + "') " +
                    "ORDER BY activity_order_by";
                OdbcCommand cmd5 = new OdbcCommand(strTemp, con3);
                con3.Open();
                using (OdbcDataAdapter oda = new OdbcDataAdapter(cmd5))
                {
                    oda.Fill(dt1);
                }               
            }
            catch (OdbcException ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:HomeMenu|PageName:WsHomepage|Function:HomeMenu|PageName:WsHomepage|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                objlog.ErrorLog(strResult);
            }
            catch (Exception ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:HomeMenu|PageName:WsHomepage|Function:HomeMenu|PageName:WsHomepage|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                objlog.ErrorLog(strResult);
            }
            finally
            {
                con3.Close();
                con3.Dispose();
            }
            return dt1;
        }
    }

    [WebMethod]
    public DataTable Bindmenu(bo_homepage obj_bo_homepage)
    {
        string strCon = System.Configuration.ConfigurationManager.ConnectionStrings["ConDB"].ToString();
        using (OdbcConnection con3 = new OdbcConnection(strCon))
        {
            DataTable dt1 = new DataTable();
            try
            {
                string strId = "2002";
                string strTemp = "SELECT activity_id,activity_name,activity_description as activity_description," +
                    "activity_action_url," +
                    "activity_action_container ," +
                    " AccessKey " +
                   "FROM CRM_activity_master,CRM_role_activity " +
                    "WHERE activity_parent_id='" + strId + "' AND activity_menu='Y' " +
                     " AND activity_id=roledet_activity_id " +
                      "  and roledet_role_id in ('" + obj_bo_homepage.RoleID + "') " +
                    "ORDER BY activity_order_by";
                OdbcCommand cmd5 = new OdbcCommand(strTemp, con3);
                con3.Open();
                using (OdbcDataAdapter oda = new OdbcDataAdapter(cmd5))
                {
                    oda.Fill(dt1);
                }
                 
            }
            catch (OdbcException ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:Bindmenu|PageName:WsHomepage|Function:HomeMenu|PageName:WsHomepage|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                objlog.ErrorLog(strResult);
            }
            catch (Exception ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:Bindmenu|PageName:WsHomepage|Function:HomeMenu|PageName:WsHomepage|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                objlog.ErrorLog(strResult);
            }
            finally
            {
                con3.Close();
                con3.Dispose();
            }
            return dt1;
        }
    }
    [WebMethod]
    public DataTable BindSubMenu(bo_homepage obj_bo_homepage)
    {
        string strCon = System.Configuration.ConfigurationManager.ConnectionStrings["ConDB"].ToString();
        using (OdbcConnection con3 = new OdbcConnection(strCon))
        {
            DataTable dt1 = new DataTable();
            try
            {
                string strSQL = "SELECT activity_id, activity_name,activity_order_by,activity_parent_id,activity_description as activity_description," +
                        "activity_image_id,activity_menu," +
                        "activity_action_url ," +
                        "activity_action_container ," +
                        " AccessKey " +
                        "FROM CRM_activity_master,CRM_role_activity " +
                        "WHERE activity_parent_id=" + obj_bo_homepage.parentId + " " +
                         " AND activity_id=roledet_activity_id " +
                        "AND activity_menu='Y'  " +
                        "  and roledet_role_id in ('" + obj_bo_homepage.RoleID + "') " +
                        "ORDER BY activity_id ";
                OdbcCommand cmd5 = new OdbcCommand(strSQL, con3);
                con3.Open();
                using (OdbcDataAdapter oda = new OdbcDataAdapter(cmd5))
                {
                    oda.Fill(dt1);
                }
                 
            }
            catch (OdbcException ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:BindSubMenu|PageName:WsHomepage|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                objlog.ErrorLog(strResult);
            }
            catch (Exception ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:BindSubMenu|PageName:WsHomepage|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                objlog.ErrorLog(strResult);
            }
            finally
            {
                con3.Close();
                con3.Dispose();
            }
            return dt1;
        }
    }


    public int InsertCtiHistory(string strCustId)
    {
        string strCon = System.Configuration.ConfigurationManager.ConnectionStrings["ConDB"].ToString();
        using (OdbcConnection con3 = new OdbcConnection(strCon))
        {
            DataTable dt1 = new DataTable();
            int value = 0;
            try
            {
                OdbcCommand cmd5 = new OdbcCommand("EXEC dbo.crm_cti_History ?", con3);
                con3.Open();
                cmd5.CommandType = CommandType.StoredProcedure;              
                cmd5.Parameters.Add(new OdbcParameter("@Cust_id", strCustId));
                value = cmd5.ExecuteNonQuery();
                return value;
            }
            catch (OdbcException ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:InsertCtiHistory|PageName:WsHomepage|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                objlog.ErrorLog(strResult);
                return -1;
            }
            catch (Exception ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:InsertCtiHistory|PageName:WsHomepage|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
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
    public int InserInboundCallBack(bo_homepage obj_bo_homepage)
        {
            String strIntreDia = System.Configuration.ConfigurationManager.AppSettings["IDGDB"].ToString();
        using (OdbcConnection con3 = new OdbcConnection(strIntreDia))
        {
            int i = 0;
            try
            {
                OdbcCommand cmd5 = new OdbcCommand("EXEC udsp_InboundCallBack ?,?,?,?", con3);
                con3.Open();
                cmd5.CommandType = CommandType.StoredProcedure;
                cmd5.Parameters.Add(new OdbcParameter("@nServiceId", obj_bo_homepage.ServiceID));
                cmd5.Parameters.Add(new OdbcParameter("@szCLI", obj_bo_homepage.CallNumber));
                cmd5.Parameters.Add(new OdbcParameter("@NDT", Convert.ToDateTime(obj_bo_homepage.BckTime).ToString("yyyy-MM-dd HH:mm:ss")));
                cmd5.Parameters.Add(new OdbcParameter("@agent_agent_id", Convert.ToInt32(obj_bo_homepage.AgentID)));
                i = (Int32)cmd5.ExecuteScalar();
                return i; 
            }
            catch (OdbcException ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:InserInboundCallBack|PageName:WsHomepage|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                objlog.ErrorLog(strResult);
                return -1;
            }
            catch (Exception ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:InserInboundCallBack|PageName:WsHomepage|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
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
    public int removeSession(bo_homepage obj_bo_homepage)
    {
        string strCon = System.Configuration.ConfigurationManager.ConnectionStrings["ConDB"].ToString();
        using (OdbcConnection con3 = new OdbcConnection(strCon))
        {
            int i = 0;
            try
            {
                string strSQL = "update UserLoginDetails set session_value ='N' , session_live = DATEADD(second ,-30,GETDATE())  WHERE login_name = '" + obj_bo_homepage.User + "' ";
                OdbcCommand cmd5 = new OdbcCommand(strSQL, con3);
                con3.Open();
                return i = cmd5.ExecuteNonQuery();
                 
            }
            catch (OdbcException ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:removeSession|PageName:WsHomepage|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                objlog.ErrorLog(strResult);
                return -1;
            }
            catch (Exception ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:removeSession|PageName:WsHomepage|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
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

