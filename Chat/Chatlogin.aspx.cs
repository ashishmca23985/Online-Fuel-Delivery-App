using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Data.Odbc;
using Telerik.Web.UI;
using System.Data.Odbc;
using Telerik.Web.UI;
using System.Web.Services;
using System.Configuration;
using System.Web.Configuration;


public partial class Chat_Chatlogin : System.Web.UI.Page
{
    DataBase m_Connection = new DataBase();
    ChatWS obj_chatws = new ChatWS();
    GlobalWS objGlobalWS = new GlobalWS();
    string status = string.Empty;
    int sourceid = 0;
    public string chatwaitnigtime = "0";
    private const int ItemsPerRequest = 10;
    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (Request.Cookies["userInfo"] != null && Request.Cookies["userInfo"]["type"] != "Offline" && Request.Cookies["userInfo"]["chatreqid"] != null)
        {
//            DataSet ds = obj_chatws.ChatResponseCheck(Request.QueryString["chatid"].ToString());
            DataSet ds = obj_chatws.ChatResponseCheck(Request.Cookies["userInfo"]["chatreqid"].ToString());
            
            if (ds.Tables["Response"].Rows[0]["ResultString"].ToString() != "")
            {
                Response.Redirect("ChatVisitor.aspx?type=E&chatsessionid=" + ds.Tables["Response"].Rows[0]["ResultString"].ToString());
            }
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!IsPostBack)
        {

           
            chatwaitnigtime = System.Configuration.ConfigurationManager.AppSettings["CHATWAITTIME"].ToString();


            if (Request.Cookies["userInfo"] != null)
            {
                //if (Request.Cookies["userInfo"]["type"] == "Offline")
                //{
                //    lblMessage.Text = "You are already send query. Our executive will get back to you !!!";
                //}
                //else
                //{
                    if (Request.QueryString["page"] != null)
                    {
                        if (Response.Cookies["userInfo"]["sale"] != null && Response.Cookies["userInfo"]["sale"].ToString() == "S"
                            )
                        {
                            btnstartchat.Enabled = false;
                           // lblMessage.Text = "You are send query. Our executive will get back to you .Please Wait !!!";
                            hdnstatus.Value = Request.Cookies["userInfo"]["status"].ToString();
                            hdnchatid.Value = Request.Cookies["userInfo"]["chatreqid"].ToString();
                            hdnpagetype.Value = Request.Cookies["userInfo"]["pagetype"].ToString();
                        }
                        else if (
                            Response.Cookies["userInfo"]["help"] != null && Response.Cookies["userInfo"]["help"].ToString() == "H")
                        {
                            btnstartchat.Enabled = false;
                            //lblMessage.Text = "You are send query. Our executive will get back to you .Please Wait !!!";
                            hdnstatus.Value = Request.Cookies["userInfo"]["status"].ToString();
                            hdnchatid.Value = Request.Cookies["userInfo"]["chatreqid"].ToString();
                            hdnpagetype.Value = Request.Cookies["userInfo"]["pagetype"].ToString();
                        }
                        else
                        {
                         //RANA   btnstartchat.Enabled = true;
                        }
                    }
                    else
                    {
                        //RANA  btnstartchat.Enabled = false;
                    }
                //}
            }
            else
            {
                btnstartchat.Enabled = true;
               

            }

            GetQueueMembersAvailability();
            if (status == "Offline")
                btnstartchat.Text = "Submit";
            ViewState["chatsessionid"] = System.Guid.NewGuid().ToString();
        }

    }
    #region Chat function
    protected void btnstartchat_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid)
        {
          return;
        }
           
      Savechatdetails();
    }


    public void Savechatdetails()
    {

        try
        {
            if (ViewState["chatsessionid"] != null && Request.QueryString["queue_id"] != null)
            {

                string queueid = "0";
                string pagetype = "H";
                int chatid = 0;
                if (Request.QueryString["queue_id"] != null)
                {
                    queueid = Request.QueryString["queue_id"];
                    GetQueueMembersAvailability();

                }
                //  System.Threading.Thread.Sleep(10000);
                if (Request.QueryString["chatid"] != null)
                {
                    pagetype = Request.QueryString["page"].ToString();
                    chatid = Convert.ToInt32(Request.QueryString["chatid"].ToString());
                }
                DataSet ds = new DataSet();
                ds = obj_chatws.InsertChatQueue(chatid, txtusername.Value, txtemail.Value, txtphoneno.Value, txtCompanyName.Value, txtreason.Value,Convert.ToInt32(queueid), status, pagetype);
                if (Convert.ToInt32(ds.Tables[0].Rows[0]["ResultCode"].ToString()) >= 0)
                {
                    btnstartchat.Enabled = false;
                    Response.Cookies["userInfo"]["type"] = status;
                    Response.Cookies["userInfo"]["Name"] = txtusername.Value;
                    Response.Cookies["userInfo"]["mobile"] = txtphoneno.Value;
                    Response.Cookies["userInfo"]["email"] = txtemail.Value;
                    Response.Cookies["userInfo"]["companyname"] = txtCompanyName.Value;
                    Response.Cookies["userInfo"]["query"] = txtreason.Value;
                    Response.Cookies["userInfo"]["lastVisit"] = DateTime.Now.ToString();
                    Response.Cookies["userInfo"]["pagetype"] = pagetype;

                    if (pagetype == "S")
                        Response.Cookies["userInfo"]["sale"] = pagetype;
                    else
                        Response.Cookies["userInfo"]["help"] = pagetype;
                    if (status == "Offline")
                    {

                        lblMessage.Text = System.Configuration.ConfigurationManager.AppSettings["OFFLINECHATMSG"].ToString(); //WebConfigurationManager.ConnectionStrings["chatMsg"].ConnectionString.ToString();
                        Response.Cookies["userInfo"].Expires = DateTime.Now.AddHours(2);
                        Response.Cookies["userInfo"]["chatreqid"] = "0";
                        obj_chatws.ChatTicket(Convert.ToInt32(ds.Tables[0].Rows[0]["ResultCode"].ToString()));
                        divdetails.Visible = false;
                    }
                    else
                    {
                        //lblMessage.Text = "Thank you " + txtusername.Value + ". Our executive will get back to you shortly!!!";
                        hdnstatus.Value = "SENDREDUEST";
                        Response.Cookies["userInfo"]["status"] = "SENDREDUEST";
                        Response.Cookies["userInfo"]["chatreqid"] = ds.Tables[0].Rows[0]["ResultCode"].ToString();
                        Response.Cookies["userInfo"].Expires = DateTime.Now.AddMinutes(15);

                        //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "", "javascript:CheckResponse();", true);

                        //Response.Redirect("Chatcustomer.aspx?chatsessionid=" + Convert.ToString(ViewState["chatsessionid"]), false);
                    }
                    hdnchatid.Value = ds.Tables[0].Rows[0]["ResultCode"].ToString();
                    GetExistingcustomer(txtemail.Value, txtphoneno.Value);
                }

            }
        }
        catch (OdbcException ex)
        {
            m_Connection.Rollback();
            LogMessage(ex.Message, 1);
        }
        catch (Exception ex)
        {
            m_Connection.Rollback();
            LogMessage(ex.Message, 1);
        }
        finally
        {
        }
    }
    #endregion

    #region GetQueueMembersAvailability

    public void GetQueueMembersAvailability()
    {
        DataSet dsDataSet;
        // dsDataSet = obj_chatws.GetChatUser(Convert.ToInt32(Request.QueryString["queue_id"]));
        dsDataSet = obj_chatws.GetChatImage(Convert.ToInt32(Request.QueryString["queue_id"]),"");

        if (Convert.ToInt32(Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"])) == -1)
        {
            LogMessage(dsDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
            return;
        }
        else
        {

            if (dsDataSet.Tables["Response"].Rows[0]["ResultString"] != null)
            {
                sourceid = Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]);
                if (sourceid <= 0)
                {
                    status = "Offline";
                }
                else
                {
                    status = "New";
                }
            }
        }
    }

    #endregion

    #region GetExistingcustomer

    public void GetExistingcustomer(string emailid, string contactno)
    {
        DataSet dsDataSet;
        dsDataSet = obj_chatws.GetExistingcustomer(emailid, contactno, hdnchatid.Value);

        if (Convert.ToInt32(Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"])) == -1)
        {
            LogMessage(dsDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
            //sourceid = 0;
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

    
    private static string GetStatusMessage(int offset, int total)
    {
        if (total <= 0)
            return "No matches";

        return String.Format("Items <strong>1</strong>-<strong>{0}</strong> out of <strong>{1}</strong>", offset, total);
    }
}
