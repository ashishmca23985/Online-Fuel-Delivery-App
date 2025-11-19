using System;
using System.Data;
using System.Web;

public partial class Logout : System.Web.UI.Page
{
    GlobalWS objGlobal = new GlobalWS();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //ClearUserSession(Session["login_id"].ToString());
            FrameworkWS objFrameworkWS = new FrameworkWS();
          //  Session["login_id"] = null;
           // Session["login_pwd"] = null;
            int nSessionID = Convert.ToInt32(Session["Session_ID"]);
            objFrameworkWS.DisposeSession(nSessionID);
            int ContactID = Convert.ToInt32(Session["contact_id"]);

            ChatWS chatws = new ChatWS();
            chatws.DisposeSession(ContactID);
            ctiLogout();
            Response.Buffer = true;
            Response.Expires = -1000;
            Response.CacheControl = "no-cache";
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

         
            //divMessage.InnerText = "Thank you for using Galaxy!! You have been successfully logged out.";
        }
    }

    public void ClearUserSession(string user)
    {
        int nClearSession = objGlobal.RemoveUserSession(Session["login_id"].ToString());
    }

    public string ctiLogout()
    {
        if (HttpContext.Current.Session["ctilogin"] != null)
        {
            CtiWS.CtiWS CtiWS1 = new CtiWS.CtiWS();

            CtiWS1.Logout("", "", HttpContext.Current.Session["HOSTID"].ToString());
            return "Success";
        }
        else
            return "fail";

    }

}
