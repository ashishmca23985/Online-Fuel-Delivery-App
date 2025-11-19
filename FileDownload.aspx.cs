using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class FileDownload : System.Web.UI.Page
{
    GlobalWS objGlobalWS = new GlobalWS();
    DataBase m_Connection = new DataBase();
    int AttachmentID = 0;
    string strFileName = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["AttachmentID"] == null)
        {
            LogMessage("QueryString parameter 'AttachmentID' is missing!!", 1);
            return;
        }
        AttachmentID = Convert.ToInt32(Request.QueryString["AttachmentID"]);

        if (IsPostBack)
            return;
        string strExtn = "";
        byte[] buffer = objGlobalWS.DownloadFile(returnFilePath(), ref strExtn);
        if (buffer.Length > 0)
        {            
            Response.AddHeader("content-disposition", "attachment;filename=" + strFileName + "");
            Response.ContentType = ReturnExtension(strExtn);
            Response.BinaryWrite(buffer);
            Response.End();
            return;
        }
        else
        {
            string script = "alert(\" No file Exist  \");";
            ScriptManager.RegisterStartupScript(this, GetType(), "ServerControlScript", script, true);
            return;

        }

    }

    public string returnFilePath()
    {
        DataSet objDataSet = objGlobalWS.GetAttachmentFile(AttachmentID);
        if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
        {
            LogMessage(Convert.ToString(objDataSet.Tables["Response"].Rows[0]["ResultString"]), 1);
            return "";
        }
        DataRow row = objDataSet.Tables["MailAttachment"].Rows[0];
        strFileName = Convert.ToString(row["FileName"]);
        return Convert.ToString(row["FilePath"]);
    }

    private string ReturnExtension(string fileExtension)
    {
        switch (fileExtension)
        {
            case ".htm":
            case ".html":
            case ".log":
                return "text/HTML";
            case ".txt":
                return "text/plain";
            case ".doc":
                return "application/ms-word";
            case ".tiff":
            case ".tif":
                return "image/tiff";
            case ".asf":
                return "video/x-ms-asf";
            case ".avi":
                return "video/avi";
            case ".zip":
                return "application/zip";
            case ".xls":
            case ".csv":
                return "application/vnd.ms-excel";
            case ".gif":
                return "image/gif";
            case ".jpg":
            case "jpeg":
                return "image/jpeg";
            case ".bmp":
                return "image/bmp";
            case ".wav":
                return "audio/wav";
            case ".mp3":
                return "audio/mpeg3";
            case ".mpg":
            case "mpeg":
                return "video/mpeg";
            case ".rtf":
                return "application/rtf";
            case ".asp":
                return "text/asp";
            case ".pdf":
                return "application/pdf";
            case ".fdf":
                return "application/vnd.fdf";
            case ".ppt":
                return "application/mspowerpoint";
            case ".dwg":
                return "image/vnd.dwg";
            case ".msg":
                return "application/msWebBlue";
            case ".xml":
            case ".sdxl":
                return "application/xml";
            case ".xdp":
                return "application/vnd.adobe.xdp+xml";
            default:
                return "application/octet-stream";
        }
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
