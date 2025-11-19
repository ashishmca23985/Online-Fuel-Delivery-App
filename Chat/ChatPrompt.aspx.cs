using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml;

public partial class ChatPrompt : ThemeBase
{
    public string promptString = "";
   
    DataBase m_Connection = new DataBase();
    public int Id = 0;
    public string SessionID = "";
    public string VisitorName = "";
    string VisitorQuery = "";
    string CreatedDate = "";
    public string CLI = "";
    public string email = "";
    public string Product = "";
    public string chatwaitnigtime = "0";
    protected void Page_Load(object sender, EventArgs e)
    {
        promptString = "<div style=color:#903d8e;'><strong>Chat Request Available !</strong><br/></div>";
        if (Request.QueryString["id"] != null)
        {
            Id =Convert.ToInt32(Request.QueryString["id"]);
            //GlobalWS objGlobalWS = new GlobalWS();
            //string strQuery = "SELECT #GetChatSessionID(" + Id + ")";
            //SessionID = objGlobalWS.ExecuteQuery(strQuery);
        }
        if (Request.QueryString["CLI"] != null)
        {
            CLI = Request.QueryString["CLI"];
        }
        if (Request.QueryString["email"] != null)
        {
            email = Request.QueryString["email"];
        }
        if (Request.QueryString["cdate"] != null)
        {
            CreatedDate = Request.QueryString["cdate"];
            CreatedDate = CreatedDate.Substring(0, 25);
            promptString += "<strong>Date : </strong>" + CreatedDate + "<br/><br/>";
        }
        if (Request.QueryString["vname"] != null)
        {
            VisitorName = Request.QueryString["vname"];
            promptString += "<strong>From : </strong>" + VisitorName + "<br/>";
        }
        if (Request.QueryString["vquery"] != null)
        {
            VisitorQuery = Request.QueryString["vquery"];
            promptString += "<strong>Query : </strong>" + VisitorQuery + "<br/>";
        }
       
        if(!IsPostBack)
            chatwaitnigtime = System.Configuration.ConfigurationManager.AppSettings["CHATWAITTIME"].ToString();
    }
    protected void btnAccept_Click(object sender, EventArgs e)
    {
        try
        {
            SessionID= System.Guid.NewGuid().ToString();
            ChatWS chatws=new ChatWS();
            DataTable dt1 = new DataTable();
            DataSet ds = chatws.InsertChatRequest(Id, TB_nContactID, SessionID, Request.UserHostAddress);
            if (Convert.ToInt32(ds.Tables[1].Rows[0]["ResultCode"].ToString()) >= 0)
            {
                if (ds.Tables[0].Rows.Count>0 && Convert.ToInt32(ds.Tables[0].Rows[0][0]) > 0)
                {
                    promptString = promptString.Replace("<strong>", "").Replace("</strong>", "");
                    dt1 = m_Connection.SaveRecentActivity(0, "CHT", Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString()), "Accepted", promptString, TB_nContactID, "P", 2409, "");
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", "javascript:OnSaveSuccess('" + TB_nContactID.ToString() + "', '" + SessionID + "','" + ds.Tables[0].Rows[0][0].ToString() + "');", true);
                    //    Response.Redirect("~/Chat/ChatCustomer.aspx?source=1&chatsessionid=" + SessionID);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", "javascript:OnAccept('" + ds.Tables[0].Rows[0][1].ToString() + "');", true);
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

    #region Log Message
    void LogMessage(string errorMessage, Int32 param)
    {
        if (param == 1)
        {
            lblMessage.Text = errorMessage;
            lblMessage.CssClass = "error";
        }
        else
        {
            lblMessage.Text = errorMessage;
            lblMessage.CssClass = "success";
            // Page.ClientScript.RegisterStartupScript(this.GetType(), "Script", "javascript:alert('" + e.Message + "')", true);
        }
    }
    #endregion       

}
