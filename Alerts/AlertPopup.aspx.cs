using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

public partial class Alerts_AlertPopup : ThemeBase
{
    DataSet m_DataSet = new DataSet();

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Page.IsPostBack)
            Get_AlertDetails();
    }

    #region Get Alert Details 
    private void Get_AlertDetails()
    {
        TableRow trRow0;
        TableRow trRow1;
        TableRow trRow2;
        TableRow trRow3;
        TableCell tdCell1;
        TableCell tdCell2;
        TableCell tdCell3;
        TableCell tdCell4;
        TableCell tdCell5;
        TableCell tdCell6;
        TableCell tdCell7;
        TableCell tdCell8;
        TableCell tdCell9;        
        TableCell tdCell10;
        TableCell tdCell11;
        TableCell tdCell12;
        TableCell tdCell13;
        TableCell tdCell14;

        TableRow trNewRow ;
        TableCell tdNewCell;

        Table tblContent;
        Table tblButton;
        HtmlInputButton inputBtn;
        HtmlInputButton inputBtn1;
        HtmlInputButton inputBtn2;
        int nUserId = 0;
        int nUserTimeZoneSpan = 0;

        int nSenderId = 0;
        string szSenderName = string.Empty;
        string checkBoxID = string.Empty;

        string szTemp = string.Empty;
        string szTemp1 = string.Empty;

        try
        {
            nUserId = TB_nContactID;
            nUserTimeZoneSpan = TB_nTimeZoneTimeSpan;
            int nAlertId = 0;
            //m_DataSet = objFrameworkWS.wmGetAlertTransactionDetails(nUserId, nUserTimeZoneSpan);
            DataTable dtTable = objFrameworkWS.GetUserAlerts(nUserId);
            m_DataSet.Tables.Add(dtTable);
            //if (Convert.ToInt32(m_DataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
            //{
            //    LogMessage(new Exception(m_DataSet.Tables["Response"].Rows[0]["ResultString"].ToString()), 1);
            //    return;
            //}            

            for (int i = 0; i < m_DataSet.Tables["Response"].Rows.Count; i++)
            {
                nAlertId = Convert.ToInt32(m_DataSet.Tables["Response"].Rows[i]["id"]);

                trRow0 = new TableRow();
                trRow1 = new TableRow();
                trRow2 = new TableRow();
                trRow3 = new TableRow();

                tdCell1 = new TableCell();
                tdCell2 = new TableCell();
                tdCell3 = new TableCell();
                tdCell4 = new TableCell();
                tdCell5 = new TableCell();
                tdCell6 = new TableCell();
                tdCell7 = new TableCell();
                tdCell8 = new TableCell();
                tdCell9 = new TableCell();
                tdCell10 = new TableCell();
                tdCell11 = new TableCell();
                tdCell12 = new TableCell();
                tdCell13 = new TableCell();
                tdCell14 = new TableCell();
                tblButton = new Table();
                tblContent = new Table();
                inputBtn = new HtmlInputButton();
                inputBtn1 = new HtmlInputButton();
                inputBtn2 = new HtmlInputButton();

                checkBoxID = "chkSnooze" + i;

                //nSenderId = Convert.ToInt32(m_DataSet.Tables["Response"].Rows[i]["alerttrn_sender_id"]);
                //szSenderName = Convert.ToString(m_DataSet.Tables["Response"].Rows[i]["alerttrn_sender_name"]);

                tdCell11.Text = m_DataSet.Tables["Response"].Rows[i]["Related_to"].ToString() + " [" +
                                m_DataSet.Tables["Response"].Rows[i]["Subject"].ToString() + "]";
                tdCell11.CssClass = "popupSub";
                tdCell11.ColumnSpan = 4;

                szTemp = "Re:" + m_DataSet.Tables["Response"].Rows[i]["Subject"].ToString();
                                
                inputBtn.Value = "Reply";
                inputBtn.Attributes.Add("class", "Button");
                inputBtn.Attributes.Add("onclick", "OpenCreateAlert('" + szTemp + "','" + nSenderId + "','" + szSenderName + "')");
                inputBtn1.Value = "Dismiss";
                inputBtn1.Attributes.Add("class", "Button");
                inputBtn1.Attributes.Add("onclick", "Dismiss('" + checkBoxID + "')");
                //inputBtn2.Value = "Snooze";
                //inputBtn2.Attributes.Add("onclick", "OpenCreateAlert('" + szTemp + "','" + nSenderId + "','" + szSenderName + "')");

                tdCell13.Controls.Add(inputBtn);
                tdCell14.Controls.Add(inputBtn1);

                trRow3.Cells.Add(tdCell13);
                trRow3.Cells.Add(tdCell14);
                tblButton.Rows.Add(trRow3);

                tdCell12.Controls.Add(tblButton);
                tdCell12.HorizontalAlign = HorizontalAlign.Right;

                tdCell1.Text = "<input type=checkbox id='" + checkBoxID + "' name=" + nAlertId + " />";
                tdCell1.Width = Unit.Percentage(5);
                tdCell1.HorizontalAlign = HorizontalAlign.Left;

                tdCell2.CssClass = "tdHeader";
                tdCell2.Width = Unit.Percentage(9);
                tdCell2.Text = "Due Date :";

                tdCell3.HorizontalAlign = HorizontalAlign.Left;
                tdCell3.Text = Convert.ToDateTime(m_DataSet.Tables["Response"].Rows[i]["Open_time"]).ToString("dd MMM yyyy hh:mm tt");
                tdCell3.CssClass = "tdValue";
                tdCell3.Width = Unit.Percentage(30);

                tdCell4.CssClass = "tdHeader";
                tdCell4.Width = Unit.Percentage(11);
                tdCell4.Text = "Sender :";

                tdCell5.HorizontalAlign = HorizontalAlign.Left;
                //tdCell5.Text = m_DataSet.Tables["Response"].Rows[i]["alerttrn_sender_name"].ToString();
                tdCell5.Text = "";
                tdCell5.CssClass = "tdValue";

                tdCell10.Text = "&nbsp;&nbsp;    " + m_DataSet.Tables["Response"].Rows[i]["Message"].ToString();
                tdCell10.ColumnSpan = 6;
                tdCell10.CssClass = "tdValue";

                trRow0.Cells.Add(tdCell11);
                trRow0.Cells.Add(tdCell12);

                trRow1.Cells.Add(tdCell1);
                trRow1.Cells.Add(tdCell2);
                trRow1.Cells.Add(tdCell3);
                trRow1.Cells.Add(tdCell4);
                trRow1.Cells.Add(tdCell5);

                tdCell6.Text = "&nbsp;";
                tdCell6.Width = Unit.Percentage(5);
                trRow2.Cells.Add(tdCell6);
                trRow2.Cells.Add(tdCell10);

                tblContent.Width = Unit.Percentage(100);
                //tblContent.BackColor = (System.Drawing.Color)m_DataSet.Tables["Response"].Rows[i]["alert_popup_back_color"];
                //tblContent. = m_DataSet.Tables["Response"].Rows[i]["alert_popup_font_color"].ToString();
                if(i%2 == 0)
                    tblContent.CssClass = "popupTable1";
                else
                    tblContent.CssClass = "popupTable2";

                tblContent.Rows.Add(trRow0);
                tblContent.Rows.Add(trRow1);
                tblContent.Rows.Add(trRow2);                

                trNewRow = new TableRow();
                tdNewCell = new TableCell();

                tdNewCell.Controls.Add(tblContent);
                trNewRow.Cells.Add(tdNewCell);                
                tblAlertDetails.Rows.Add(trNewRow);
            }
        }
        catch (Exception ex)
        {
            LogMessage(ex, 1);
        }
    }
    #endregion

    #region Log Message
    void LogMessage(Exception e, Int32 param)
    {

        /*** used to show the response as a label text ***/
        if (param == 1)
        {
            lblMessage.Visible = true;
            lblMessage.Text = e.Message.ToString().Substring(0, e.Message.Length);
            lblMessage.ForeColor = System.Drawing.Color.Red;
        }
        else if (param == 3)
        {
            lblMessage.Visible = true;
            lblMessage.Text = e.Message.ToString().Substring(0, e.Message.Length);
            lblMessage.ForeColor = System.Drawing.Color.Blue;
        }
        else if (param == 100)
        {
            lblMessage.Visible = false;
            lblMessage.Text = e.Message;
            lblMessage.ForeColor = System.Drawing.Color.Green;
        }
        else
        {
            lblMessage.Visible = true;
            lblMessage.Text = e.Message;
            lblMessage.ForeColor = System.Drawing.Color.Green;
        }
    }
    #endregion
}
