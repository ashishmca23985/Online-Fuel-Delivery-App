using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;

public partial class GetFileAttachment : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            GlobalWS objGlobalWS = new GlobalWS();
            DataBase m_Connection = new DataBase();
            string path = "";


            if (Request.QueryString["ID"] == null)
            {
                LogMessage("QueryString parameter 'ID' is missing!!", 1);
                return;
            }


            if (IsPostBack)
                return;

            DataSet objDataSet = objGlobalWS.GetAttachmentPath(Convert.ToString(Request.QueryString["id"]));
            if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(objDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }
            path = Convert.ToString(objDataSet.Tables["Path"].Rows[0]["attchmnt_file_path"]);
            ReadFile(path);

        }
        catch { }
    }

    private void ReadFile(string ImagePath)
    {
      //  string path = Server.MapPath("~");
        ImagePath = ImagePath.Replace("~/", "\\");
      //  path = path + ImagePath;
        byte[] Buffer;
        FileStream MyFileStream = new FileStream(ImagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        Int64 FileSize = MyFileStream.Length;
        Buffer = new byte[(int)FileSize];
        MyFileStream.Read(Buffer, 0, (int)(MyFileStream.Length));
        MyFileStream.Close();
        Response.ContentType = "image/jpeg";
        Response.BinaryWrite(Buffer);
    }
    #region Log Message
    void LogMessage(string Message, Int32 param)
    {
        if (param == 1)
            lblMessage.CssClass = "error";
        else
            lblMessage.CssClass = "success";
    }
    #endregion
}