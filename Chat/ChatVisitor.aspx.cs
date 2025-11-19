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
using System.Web.Services;
public partial class Chat_ChatVisitor : System.Web.UI.Page
{
    DataBase m_Connection = new DataBase();
    
    public string chatsessionid=null ;
    public int sourcid=0;
    public string sourcename = "";
    public DateTime currentDate = DateTime.Now;
    protected void Page_Load(object sender, EventArgs e)
    {

      //  if (!IsPostBack)
      //      return;   
        if (Session["contact_id"] != null)
        {
            sourcid = Convert.ToInt32(Session["contact_id"]);
           
        }
        else
        {
            sourcid = 0;
        }
        if (Request.QueryString["chatsessionid"] == null)
        {
            Response.Redirect("chatlogin.aspx");
        }
        else
        {
            
            chatsessionid = Request.QueryString["chatsessionid"].ToString();
            GetCustomerName();
            Session["csid"] = chatsessionid;
        }

        
    }
    public void GetCustomerName()
    { 
        ChatWS chatws=new ChatWS ();
        DataSet dsDataSet = chatws.GetCustomerName(Request.QueryString["chatsessionid"].ToString());

        
        if (Convert.ToInt32(Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"])) != 0)
        {
            LogMessage(dsDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
            return;
        }
                     

        else if (dsDataSet.Tables["chatsessiondetail"].Rows.Count > 0)
        {

            if (dsDataSet.Tables["chatsessiondetail"].Rows[0]["chat_session_status"].ToString() == "E")
            {
                if (Request.Cookies["userInfo"] != null)
                    Request.Cookies["userInfo"].Expires.AddDays(-1);
                Response.Redirect("chatlogin.aspx");
            }
            if (Convert.ToInt32(dsDataSet.Tables["chatsessiondetail"].Rows[0]["SourceID"].ToString()) == sourcid)
            {
                sourcename = dsDataSet.Tables["chatsessiondetail"].Rows[0]["chat_destination_name"].ToString();
            }
            else
            {
                sourcename = dsDataSet.Tables["chatsessiondetail"].Rows[0]["chat_visitor_name"].ToString();
                
            }
            sourcename = "Chat with " + sourcename;

        }
    }
    #region Log Message
    void LogMessage(string Message, Int32 param)
    {
        if (param == 1)
        {
            lblmessage.Text = Message;
            lblmessage.CssClass = "error";
        }
        else
        {
            lblmessage.Text = Message;
            lblmessage.CssClass = "success";
        }
    }
    #endregion


    
    
    
    
    
   
}
