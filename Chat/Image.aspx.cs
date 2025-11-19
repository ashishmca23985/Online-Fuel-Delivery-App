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
using System.IO;

public partial class Chat_Image : System.Web.UI.Page
{
    DataSet dsDataSet=new DataSet();
    int mode = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["queue_id"] != null)
        {
            ChatWS obj_chatws = new ChatWS();
            string imagepath = "";
            dsDataSet=obj_chatws.GetChatImage(Convert.ToInt32(Request.QueryString["queue_id"]),"");

            if (Convert.ToInt32(Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"])) == -1)
            {
                LogMessage(dsDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
                return;
            }
            else
            {

                if (dsDataSet.Tables["Response"].Rows[0]["ResultString"] != null)
                {
                    mode=Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]);
                    imagepath = dsDataSet.Tables["Response"].Rows[0]["ResultString"].ToString();
                    ReadFile(imagepath);
            
                }
            }
            //obj_chatws.InsertVisitor();
        }
    }

    #region readfile 
    private void ReadFile(string ImagePath)
    {
        string path = Server.MapPath("~");
        ImagePath = ImagePath.Replace("~/", "\\");
        path = path + ImagePath;
        byte[] Buffer;
        FileStream MyFileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        Int64 FileSize = MyFileStream.Length;
        Buffer = new byte[(int)FileSize];
        MyFileStream.Read(Buffer, 0, (int)(MyFileStream.Length));
        MyFileStream.Close();
        Response.ContentType = "image/jpeg";
        Response.BinaryWrite(Buffer);
    }
    
    #endregion

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
