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
public partial class Chat_ChatCustomer :System.Web.UI.Page
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
           // Gettemplate();
            Session["csid"] = chatsessionid;
            if(Request.QueryString["source"] !=null)
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", "javascript:parent.updateArrayonAccept('" + chatsessionid + "');", true);

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
        //manju
        if (dsDataSet.Tables["details"].Rows.Count > 0)
        {
            Label lblCaseTooptip = new Label();
            lblCaseTooptip.Text = Convert.ToString(dsDataSet.Tables["details"].Rows[0][0]);
            RadToolTip.Controls.Add(lblCaseTooptip);
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


    private const int ItemsPerRequest = 5;
    [WebMethod]
    public static RadComboBoxData GetContact(RadComboBoxContext context)
    {
        ChatWS ob = new ChatWS();
        DataSet objDataSet = ob.GetContact(context.Text);
       // string str= HttpContext.Current.Request.QueryString["chatsessionid"].ToString();

        DataTable data = objDataSet.Tables[0];

        RadComboBoxData comboData = new RadComboBoxData();
        int itemOffset = context.NumberOfItems;
        int endOffset = Math.Min(itemOffset + ItemsPerRequest, data.Rows.Count);
        comboData.EndOfItems = endOffset == data.Rows.Count;

        List<RadComboBoxItemData> result = new List<RadComboBoxItemData>(endOffset - itemOffset);

        for (int i = itemOffset; i < endOffset; i++)
        {
            RadComboBoxItemData itemData = new RadComboBoxItemData();
            itemData.Text = data.Rows[i]["customername"].ToString();
            itemData.Value = data.Rows[i]["session_contact_id"].ToString();
            result.Add(itemData);
        }
        comboData.Message = GetStatusMessage(endOffset, data.Rows.Count);
        comboData.Items = result.ToArray();
        return comboData;
    }
    private static string GetStatusMessage(int offset, int total)
    {
        if (total <= 0)
            return "No matches";

        return String.Format("Items <strong>1</strong>-<strong>{0}</strong> out of <strong>{1}</strong>", offset, total);
    }
    //public void Gettemplate()
    //{
    //    cmbtemplate.Items.Clear();
    //    ChatWS chatws = new ChatWS();
    //    DataSet dsDataSet = chatws.getchattemplatedetails(Request.QueryString["chatsessionid"].ToString());


    //    if (Convert.ToInt32(Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"])) != 0)
    //    {
    //        LogMessage(dsDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
    //        return;
    //    }


    //    cmbtemplate.DataSource = dsDataSet.Tables["Templatedata"];
    //    cmbtemplate.DataValueField = "Value";
    //    cmbtemplate.DataTextField = "Text";
    //    cmbtemplate.DataBind();

    //    RadComboBoxItem lstItem = new RadComboBoxItem("", "0");
    //    cmbtemplate.Items.Insert(0, lstItem);
       
       
    //}

 
}
