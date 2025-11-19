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
 
public class wsForLoginDetail : System.Web.Services.WebService {

   [WebMethod]   
    public DataTable CheckRolIdPresent(bo_ForLogin ForLogin, Label lblMessage)
    {
        string strCon = System.Configuration.ConfigurationManager.ConnectionStrings["IDGDB"].ToString();
        using (OdbcConnection con3 = new OdbcConnection(strCon))
        {
            DataTable dt = new DataTable();  
            using (OdbcCommand cmd = new OdbcCommand())
            {
                try
                {
                    cmd.Connection = con3;
                    con3.Open();
                    cmd.CommandText = "SELECT *, agent_login_id,agent_login_password from cti_agents  WHERE agent_login_id = ? and agent_login_password=? LIMIT 1";
                    cmd.Parameters.AddWithValue("@login_name", ForLogin.LoginName);
                    cmd.Parameters.AddWithValue("@passsword", ForLogin.CtiPassword);
                    using (OdbcDataAdapter oda1 = new OdbcDataAdapter(cmd))
                    {
                        oda1.Fill(dt);
                    }
                    if (dt.Rows.Count <= 0)
                    {
                        lblMessage.Text = "Invalid username or password!";
                        return null;
                    }
                    if (Convert.ToString(dt.Rows[0]["Status"]) != "Active")
                    {
                        lblMessage.Text = "User is deactive, please contact administrator!";
                        return null;
                    }

                    //if (Convert.ToString(dt.Rows[0]["session_value"]) == "Y")
                   // {
                       // if( Convert.ToString(dt.Rows[0]["Role_id"])!="1")
                        UpdateLoginStatus("Y", ForLogin.LoginName, ForLogin.Password);
                        ForLogin.RoleIDExist = Convert.ToString(dt.Rows[0]["RoleCti"]);
                   // }
                    //else
                   // {
                      //  dt = null;
                     //   lblMessage.Text = "Max login limit, Please terminate existing session!";
                    //}
                    return dt;

                }

                catch (OdbcException ex)
                {
                    string strResult = "";
                    CreateLog objlog = new CreateLog();
                    strResult = "|Function:CheckRolIdPresent|PageName:WsForLoginDetails|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                    objlog.ErrorLog(strResult);
                    DataTable dt1 = new DataTable();
                    return dt1;
                }
                catch (Exception ex)
                {
                    string strResult = "";
                    CreateLog objlog = new CreateLog();
                    strResult = "|Function:CheckRolIdPresent|PageName:WsForLoginDetails|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                    objlog.ErrorLog(strResult);
                    DataTable dt1 = new DataTable();
                    return dt1;
                }
                finally
                {
                    con3.Close();
                    con3.Dispose();
                }
            }
        }
    }

   public int UpdateLoginStatus(String strModule, string strUser, string strPasswod)
   {
       string strCon = System.Configuration.ConfigurationManager.ConnectionStrings["IDGDB"].ToString();
       using (OdbcConnection con3 = new OdbcConnection(strCon))
       {
           int i = 0;
           try
           {

               string strSQL = "update UserLoginDetails  set session_value ='Y',session_live=getdate()"+               
               "WHERE login_name = '" + strUser + "' and passsword='" + strPasswod + "'";
               OdbcCommand cmd5 = new OdbcCommand(strSQL, con3);
               con3.Open();
               Object obj_val = cmd5.ExecuteScalar();
               i = Convert.ToInt32(obj_val);
               return i;
           }
           catch (OdbcException ex)
           {
               string strResult = "";
               CreateLog objlog = new CreateLog();
               strResult = "|Function:UpdateLoginStatus|PageName:WsForLoginDetails|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
               objlog.ErrorLog(strResult);
               return - 1;
           }
           catch (Exception ex)
           {
               string strResult = "";
               CreateLog objlog = new CreateLog();
               strResult = "|Function:UpdateLoginStatus|PageName:WsForLoginDetails|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
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
    [WebMethod]
    public int ChangePassword(bo_ForLogin ForLogin)
    {
        string strCon = System.Configuration.ConfigurationManager.ConnectionStrings["IDGDB"].ToString();
        int VALUE = 0;
        using (OdbcConnection con3 = new OdbcConnection(strCon))
        {
            DataTable dt1 = new DataTable();

            OdbcCommand cmd = null;
            try
            {
                con3.Open();
                cmd = new OdbcCommand("update cti_agents set agent_login_password='" + ForLogin.ChangePagePassword + "' where agent_agent_id='" + ForLogin.LoginName + "' ", con3);
                return VALUE = cmd.ExecuteNonQuery();
            }
            catch (OdbcException ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:ChangePassword|PageName:cti_agents|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                objlog.ErrorLog(strResult);
                return -1;
            }
            catch (Exception ex)
            {
                string strResult = "";
                CreateLog objlog = new CreateLog();
                strResult = "|Function:ChangePassword|PageName:cti_agents|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
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

    // added by for checking user name and passowrd

    [WebMethod]
    public DataTable CheckUserNamePresent(bo_ForLogin ForLogin, Label lblMessage)
    {
        string strCon = System.Configuration.ConfigurationManager.ConnectionStrings["IDGDB"].ToString();
        using (OdbcConnection con3 = new OdbcConnection(strCon))
        {
            DataTable dt = new DataTable();
            using (OdbcCommand cmd = new OdbcCommand())
            {
                try
                {
                    cmd.Connection = con3;
                    con3.Open();
                    cmd.CommandText = "SELECT *, agent_login_id,agent_login_password from cti_agents  WHERE agent_login_id = ? LIMIT 1";
                    cmd.Parameters.AddWithValue("@login_name", ForLogin.LoginName);
                     using (OdbcDataAdapter oda1 = new OdbcDataAdapter(cmd))
                    {
                        oda1.Fill(dt);
                    }
                  
                     return dt;
                  }

                catch (OdbcException ex)
                {
                    string strResult = "";
                    CreateLog objlog = new CreateLog();
                    strResult = "|Function:CheckUserNamePresent|PageName:WsForLoginDetails|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                    objlog.ErrorLog(strResult);
                    DataTable dt1 = new DataTable();
                    return dt1;
                }
                catch (Exception ex)
                {
                    string strResult = "";
                    CreateLog objlog = new CreateLog();
                    strResult = "|Function:CheckUserNamePresent|PageName:WsForLoginDetails|Error Msg:|" + ex.Message + "|ErrorTye:|" + ex.GetType() + "|SourceName:|" + ex.Source;
                    objlog.ErrorLog(strResult);
                    DataTable dt1 = new DataTable();
                    return dt1;
                }
                finally
                {
                    con3.Close();
                    con3.Dispose();
                }
            }
        }
    }

}

