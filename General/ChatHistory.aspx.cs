using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Odbc;
using System.Data;

public partial class General_ChatHistory : ThemeBase
{

    DataBase m_Connection = new DataBase();
    ChatWS obj_chatws = new ChatWS();
    public string source_id;
   
    public int Id;
    protected void Page_Load(object sender, EventArgs e)
    {
        m_Connection.OpenDB("Galaxy");
        if (Request.QueryString["id"] == null)
        {
            LogMessage("QueryString parameter ' id; is missing!!", 1);
           return;
        }
        Id= Convert.ToInt32(Request.QueryString["id"]);
        if (IsPostBack)
            return;

        getdetails();
      
    }
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


    public void getdetails()
    {
        DataSet objDataSet = obj_chatws.getchatdetails(Id);
        if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
        {
            LogMessage(Convert.ToString(objDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
            return;
        }
        if (Convert.ToString(objDataSet.Tables[0].Rows[0]["chat_type"]) != "I")
        {
            txtvisitorname.Text = Convert.ToString(objDataSet.Tables[0].Rows[0]["chat_visitor_name"]);
            txtgalaxyusername.Text = Convert.ToString(objDataSet.Tables[0].Rows[0]["chat_destination_name"]);
            txtEmail.Text = Convert.ToString(objDataSet.Tables[0].Rows[0]["chat_visitor_email"]);
            txtphone.Text = Convert.ToString(objDataSet.Tables[0].Rows[0]["chat_visitor_phone"]);
            txtcompanyname.Text = Convert.ToString(objDataSet.Tables[0].Rows[0]["chat_visitor_companyname"]);

            txtQuery.Text = Convert.ToString(objDataSet.Tables[0].Rows[0]["chat_visitor_query"]);
            string d1 = Convert.ToString(objDataSet.Tables[0].Rows[0]["chat_start_time"]);
            txtTime.Text = Convert.ToDateTime(d1).AddMinutes(+TB_nTimeZoneTimeSpan).ToString("dd MMM yyyy HH:mm");
            source_id = Convert.ToString((objDataSet.Tables[0].Rows[0]["related_to_id"]));
            string full_name = Convert.ToString(objDataSet.Tables[0].Rows[0]["Name"]);
            if (source_id!=null && source_id.Trim()!="" && Convert.ToInt32(source_id) > 0 && full_name.Length > 0)
            {
                callername.Attributes.Add("onclick", "return OpenItem('ShowObject.aspx?ObjectType=CNT&ID=" + source_id + "','Customer[" + full_name + "]')");
                callername.InnerHtml =  full_name;
            }
        }
        else
        {
            tr1.Visible = false;
        }
        getchathistory(Convert.ToString(objDataSet.Tables[0].Rows[0]["chat_session_id"]));
    }

    public void getchathistory(string session_id)
    {

        DataSet objDataSet = obj_chatws.getchathistory(session_id);
        if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
        {
            LogMessage(Convert.ToString(objDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
            return;
        }

        RepComment.DataSource = objDataSet.Tables[0];
        RepComment.DataBind();
    }
}