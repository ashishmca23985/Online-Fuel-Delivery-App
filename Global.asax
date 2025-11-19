<%@ Application Language="C#" %>
<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
        //--Initialise App. variable for Telerik DLL
        Application["Telerik.Web.UI.Key.Teckinfo"] = "YellowStone";
        
        //--Application Version
        Application["GalaxyVersion"] = "1.0.0";

        Application["UserCount"] = 50; 
        Application["Themes"] = "Yellowstone";
    }
    void Application_End(object sender, EventArgs e)
    {
    }
    void Application_Error(object sender, EventArgs e)
    {
    }
    protected void Application_BeginRequest(object sender, EventArgs e)
    {
        // Requests for JSONP requests must be handled manually due to the limitations of JSONP ASMX.
        JsonPUtility.ProcessJsonPRequest();
    }
    void Session_Start(object sender, EventArgs e)
    {
        Session["Session_id"] = "0";
    }
    void Session_End(object sender, EventArgs e) 
    {
        FrameworkWS objframework = new FrameworkWS();
        if (Session["Session_ID"] != null && !string.IsNullOrEmpty(Session["Session_ID"].ToString()))
        {
            int nSessionID = Convert.ToInt32(Session["Session_ID"]);
            objframework.DisposeSession(nSessionID);
        }
        GetSessionDispose();
    }
    public void GetSessionDispose()
    {
        int ContactID = Convert.ToInt32(Session["contact_id"]);
        ChatWS chatws = new ChatWS();
        chatws.DisposeSession(ContactID);
        
        
        
    
    }
</script>
