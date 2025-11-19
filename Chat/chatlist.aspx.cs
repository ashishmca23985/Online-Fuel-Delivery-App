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

public partial class Chat_chatlist : ThemeBase
{
    DataBase m_Connection = new DataBase();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {

            GetcustomerStatus();
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Clientscript1", "javascript:setTimeout('document.getElementById(\"btnRefreshRecent\").click()', 5000);", true);
        }
    }
    #region Get Customer Status

    public void GetcustomerStatus()
    {
        string szdestinationid = string.Empty;
        string szsourceid = TB_nContactID.ToString();

        try
        {
            ChatWS objChatWS = new ChatWS();
            DataSet dsDataSet = objChatWS.GetContactStatus(TB_nContactID);

            if (Convert.ToInt32(Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"])) != 0)
            {
                LogMessage(dsDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
                return;
            }
            else
            {
                spnloginagent.InnerHtml = dsDataSet.Tables["customerstatus"].Rows[0]["customername"].ToString() + "(Logged In)";
                radcombomodes.Text = dsDataSet.Tables["customerstatus"].Rows[0]["session_user_mode"].ToString();
                //radcombomodes.SelectedItem.Value = dsDataSet.Tables["customerstatus"].Rows[0]["session_user_mode"].ToString();

                if (dsDataSet.Tables["customerstatus"].Rows[0]["session_user_mode"].ToString() == "Available")
                {
                    imgmodes.Src = @"../Images/greensymbol.bmp";

                }
                else if (dsDataSet.Tables["customerstatus"].Rows[0]["session_user_mode"].ToString() == "Busy")
                {

                    imgmodes.Src = @"../Images/breakmode.bmp";
                }
                else if (dsDataSet.Tables["customerstatus"].Rows[0]["session_user_mode"].ToString() == "Away")
                {
                    imgmodes.Src = @"../Images/orangebusy.bmp";
                }
                else if (dsDataSet.Tables["customerstatus"].Rows[0]["session_user_mode"].ToString() == "Invisible")
                {
                    imgmodes.Src = @"../Images/invisiblesymbol.jpg";
                }
                imgmodes.Style.Add("height", "10px");
                imgmodes.Style.Add("width", "10px");
            }

        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion
    #region Get Customerlist
    public void GetCustomerlist()
    {
        string szdestinationid = string.Empty;
        string szsourceid = TB_nContactID.ToString();
        string szdestinyname = "";
        string szimagename = "";
        string szbreakmode = "";

        try
        {
            ChatWS objChatWS = new ChatWS();
            DataSet dsDataSet = objChatWS.GetContactList(TB_nContactID);

            if (Convert.ToInt32(Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"])) != 0)
            {
                LogMessage(dsDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
                return;
            }
            else if (dsDataSet.Tables["CustomerList"].Rows.Count > 0)
            {
               
                for (int i = 0; i < dsDataSet.Tables["CustomerList"].Rows.Count; i++)
                {
                    if (dsDataSet.Tables["CustomerList"].Rows[i]["session_user_mode"].ToString() == "Available")
                    {
                        szimagename = @"../Images/greensymbol.bmp";
                        szbreakmode = "Available";
                    }
                    else if (dsDataSet.Tables["CustomerList"].Rows[i]["session_user_mode"].ToString() == "Busy")
                    {
                        szimagename = @"../Images/breakmode.bmp";
                        szbreakmode = "Busy";
                    }
                    else if (dsDataSet.Tables["CustomerList"].Rows[i]["session_user_mode"].ToString() == "Away")
                    {
                        szimagename = @"../Images/orangebusy.bmp";

                        szbreakmode = "Away";
                    }
                    else if (dsDataSet.Tables["CustomerList"].Rows[i]["session_user_mode"].ToString() == "Invisible")
                    {
                        szimagename = @"../Images/invisiblesymbol.jpg";
                        szbreakmode = "Invisible";
                    }
                    TableRow tr = new TableRow();
                    TableCell td = new TableCell();
                    HtmlAnchor aLink = new HtmlAnchor();

                    tr.Height = Unit.Pixel(20);
                    szdestinyname = "<img src='" + szimagename + "' style='width:10px;height:10px' " +
                             "border=0 />&nbsp;";

                    td.Text = szdestinyname;
                    td.Wrap = false;
                    tr.Cells.Add(td);
                    td = new TableCell();
                    szdestinyname = dsDataSet.Tables["CustomerList"].Rows[i]["customername"].ToString();
                    szdestinationid = dsDataSet.Tables["CustomerList"].Rows[i]["session_contact_id"].ToString();
                    aLink.Title = szdestinyname;
                    aLink.InnerText = szdestinyname + "     " + "(" + szbreakmode + ")"; ;
                    aLink.HRef = "javascript:parent.Insertchatrequest('" + szsourceid + "','" + szdestinationid + "');";

                    td.Controls.Add(aLink);
                    td.Wrap = false;
                    tr.Cells.Add(td);
                    tblcustomerlist.Rows.Add(tr);
                }
                

            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }
    #endregion

    protected void btnRefreshRecent_OnClick(object sender, EventArgs e)
    {
        GetCustomerlist();
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Clientscript1", "javascript:setTimeout('document.getElementById(\"btnRefreshRecent\").click()', 5000);", true);
    }

    #region Log Message
    void LogMessage(string Message, Int32 param)
    {
        if (param == 1)
        {
            lblMessage.Text = Message;
            lblMessage.CssClass = "error";
        }
        else
        {
            lblMessage.Text = Message;
            lblMessage.CssClass = "success";
        }
    }
    #endregion
    protected void btnsetstatus_Click(object sender, EventArgs e)
    {
        ChangeCustomerStatus();

    }

    #region change customer Status
    public void ChangeCustomerStatus()
    {
        string szsourceid = TB_nContactID.ToString();
        try
        {
            ChatWS objChatWS = new ChatWS();
            DataSet dsDataSet = objChatWS.ChangeCustomerStatus(TB_nContactID, radcombomodes.SelectedItem.Text);

            if (Convert.ToInt32(Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"])) == -1)
            {
                LogMessage(dsDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
                return;
            }
            else
            {
                GetcustomerStatus();
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex.Message, 1);
        }
    }

    #endregion

}
