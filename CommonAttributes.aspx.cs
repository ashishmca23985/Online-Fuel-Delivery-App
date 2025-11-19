using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CommonAttributes : ThemeBase
{
    GlobalWS objGlobalWS = new GlobalWS();
    DataBase m_Connection = new DataBase();
    string ObjectType = "";
    string strID = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        m_Connection.OpenDB("Galaxy");

        if (Request.QueryString["ObjectType"] == null || Request.QueryString["ID"] == null || Request.QueryString["ID"] == "|" || Request.QueryString["action"] == null)
            return;
        
        ObjectType = Request.QueryString["ObjectType"];


        
        strID = Request.QueryString["ID"].Replace('|', ',') + "0";
        string strAction = Request.QueryString["action"];

        if (ObjectType == "USR")
        {
            string sid = Request.QueryString["ID"].Split('|')[1];
           
            //.Split("|")[1];
            if (Convert.ToInt32(sid) == TB_nContactID)
            {
                tdDeleteConfirmation.InnerHtml = "Can't delete this user";
                return;
            }
        }
        if (strAction == "U")
        {
          //  tbl_Attributes.Style.Add("display", "block");
            tbl_DeletedStatus.Style.Add("display", "none");
        }
        else if (strAction == "D")
        {
         //   tbl_Attributes.Style.Add("display", "none");
            tbl_DeletedStatus.Style.Add("display", "block");
            DataSet dsDataset = objGlobalWS.DeleteRecordsfromDatabase(ObjectType, strID);
            if (Convert.ToInt32(dsDataset.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            {
                LogMessage(Convert.ToString(dsDataset.Tables["Response"].Rows[0]["ResultString"]), 1);
                return;
            }
            int nCount = dsDataset.Tables["NotDeleted"].Rows.Count;
            if (nCount <= 0)
            {
                tdDeleteConfirmation.InnerHtml = "Record has been deleted successfully!";
                return;
            }
            string strNotDeletedID = "";
            for (int i = 0; i < nCount; i++)
            {
                strNotDeletedID += dsDataset.Tables["NotDeleted"].Rows[i]["transaction_number"].ToString();
                if (i != nCount - 1)
                    strNotDeletedID += " , ";
            }
            tdDeleteConfirmation.InnerHtml = nCount.ToString() + " records [ " + strNotDeletedID + " ] can not be deleted!<br/>Rest are deleted!<br/>Please delete the dependencies first..";
        }
        else
            return;
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
}
