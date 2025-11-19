using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Chat_Getimage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string strtemp = "Chatlogin.aspx?queue_id=55";
        imagechat.Attributes.Add("onclick", "window.open('" + strtemp + "')");
        imagechat.Src = "image.aspx?queue_id=55";
    }
}
