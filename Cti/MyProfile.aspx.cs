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
using System.IO;
using System.Xml;
using Telerik.Web.UI;
using Telerik.Charting;
using System.Drawing;
using System.Collections.Generic;

public partial class MyProfile : System.Web.UI.Page
{
    string strLoginId = "";
    private static List<Color> Colors
    {
        get
        {
            List<Color> colors = new List<Color>();

            colors.Add(Color.LightBlue);
            colors.Add(Color.Yellow);
            colors.Add(Color.Pink);
            colors.Add(Color.ForestGreen);
            colors.Add(Color.LemonChiffon);
            colors.Add(Color.LavenderBlush);
            colors.Add(Color.RosyBrown);
            colors.Add(Color.LightSalmon);
            colors.Add(Color.Gold);
            colors.Add(Color.Purple);
            colors.Add(Color.DarkSeaGreen);
            colors.Add(Color.MediumAquamarine);
            colors.Add(Color.DarkOrchid);
            colors.Add(Color.SlateGray);
            colors.Add(Color.Orange);
            colors.Add(Color.BlanchedAlmond);
            colors.Add(Color.Beige);
            colors.Add(Color.AliceBlue);
            colors.Add(Color.AntiqueWhite);
            colors.Add(Color.Azure);
            colors.Add(Color.Bisque);
            colors.Add(Color.Coral);
            colors.Add(Color.CornflowerBlue);
            colors.Add(Color.Cornsilk);
            colors.Add(Color.Crimson);

            colors.Add(Color.Cyan);
            colors.Add(Color.DarkGoldenrod);
            colors.Add(Color.DarkGray);

            colors.Add(Color.DarkSlateGray);
            colors.Add(Color.DarkTurquoise);
            colors.Add(Color.DeepSkyBlue);

            colors.Add(Color.DimGray);
            colors.Add(Color.DodgerBlue);
            colors.Add(Color.Fuchsia);
            return colors;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.QueryString["LoginId"] != null)
            strLoginId = Request.QueryString["LoginId"].ToString();
        else if (Session["LOGINID"] != null)
            strLoginId = Session["LOGINID"].ToString();
        else
            strLoginId = "0";

        if (!Page.IsPostBack)
        {
            PopulateSkins();
            CallListWS m_callList = new CallListWS();
            DataSet m_dataSet = m_callList.GetServiceDetails(strLoginId);

            if (m_dataSet.Tables["SessionStats"] != null)
                PopulateStatistics(m_dataSet.Tables["SessionStats"]);
            //if (m_dataSet.Tables["VMStats"] != null)
              //  PopulateVM(m_dataSet.Tables["VMStats"]);
            if (m_dataSet.Tables["CallStats"] != null)
                PopulateChart(m_dataSet.Tables["AgentDetails"].Rows[0]["agent_skin"].ToString(), m_dataSet.Tables["CallStats"]);
        }
    }

    private void PopulateStatistics(DataTable dtSessionStats)
    {
        tdLoginCount.InnerText = dtSessionStats.Rows[0]["LoginCount"].ToString();
     //   tdTotalLogin.InnerText = dtSessionStats.Rows[0]["EffectiveLoginDuration"].ToString();
        tdTotalLogin.InnerText = dtSessionStats.Rows[0]["LoginDuration"].ToString();
        tdTotalEffLogin.InnerText = dtSessionStats.Rows[0]["EffectiveLoginDuration"].ToString();
        tdTotalIdle.InnerText = dtSessionStats.Rows[0]["IdleDuration"].ToString();
        tdTotalIncall.InnerText = dtSessionStats.Rows[0]["IncallDuration"].ToString();
        tdTotalBreak.InnerText = dtSessionStats.Rows[0]["BreakDuration"].ToString();

        tdTotalCalls.InnerText = dtSessionStats.Rows[0]["TotalCalls"].ToString();
        tdManualCalls.InnerText = dtSessionStats.Rows[0]["ManualCalls"].ToString();
        tdInboundCalls.InnerText = dtSessionStats.Rows[0]["InboundCalls"].ToString();
        tdOutboundCalls.InnerText = dtSessionStats.Rows[0]["OutboundCalls"].ToString();

        tdBucket1.InnerText = dtSessionStats.Rows[0]["Bucket1"].ToString();
        tdBucket2.InnerText = dtSessionStats.Rows[0]["Bucket2"].ToString();
        tdBucket3.InnerText = dtSessionStats.Rows[0]["Bucket3"].ToString();
        tdBucket4.InnerText = dtSessionStats.Rows[0]["Bucket4"].ToString();
        tdBucket5.InnerText = dtSessionStats.Rows[0]["Bucket5"].ToString();
        tdBucket6.InnerText = dtSessionStats.Rows[0]["Bucket6"].ToString();
        tdBucket7.InnerText = dtSessionStats.Rows[0]["Bucket7"].ToString();
        tdBucket8.InnerText = dtSessionStats.Rows[0]["Bucket8"].ToString();
        tdBucket9.InnerText = dtSessionStats.Rows[0]["Bucket9"].ToString();
        tdBucket10.InnerText = dtSessionStats.Rows[0]["Bucket10"].ToString();
    }

    private void PopulateVM(DataTable dtVMStats)
    {
        tdVMNew.InnerText = dtVMStats.Rows[0]["New"].ToString();
        tdVMSaved.InnerText = dtVMStats.Rows[0]["Saved"].ToString();
    }

    private void PopulateChart(string skin, DataTable dtCallStats)
    {
        RadComboBoxSkin.SelectedValue = skin;
        RadChart1.Skin = skin;
        RadChart1.DataSource = dtCallStats;
        RadChart1.DataBind();
    }

    private void PopulateSkins()
    {
        RadComboBoxItem radComboBoxItem = new RadComboBoxItem();
        radComboBoxItem.Text = "Black";
        radComboBoxItem.Value = "Black";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "Default";
        radComboBoxItem.Value = "Default";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "Hay";
        radComboBoxItem.Value = "Hay";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "Inox";
        radComboBoxItem.Value = "Inox";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "Office2007";
        radComboBoxItem.Value = "Office2007";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "WebBlue";
        radComboBoxItem.Value = "WebBlue";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "WebBlue";
        radComboBoxItem.Value = "WebBlue";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "Telerik";
        radComboBoxItem.Value = "Telerik";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "Vista";
        radComboBoxItem.Value = "Vista";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "Web20";
        radComboBoxItem.Value = "Web20";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "WebBlue";
        radComboBoxItem.Value = "WebBlue";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "Marble";
        radComboBoxItem.Value = "Marble";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "Metal";
        radComboBoxItem.Value = "Metal";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "Wood";
        radComboBoxItem.Value = "Wood";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "BlueStripes";
        radComboBoxItem.Value = "BlueStripes";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "DeepBlue";
        radComboBoxItem.Value = "DeepBlue";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "DeepGray";
        radComboBoxItem.Value = "DeepGray";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "DeepGreen";
        radComboBoxItem.Value = "DeepGreen";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "DeepRed";
        radComboBoxItem.Value = "DeepRed";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "GrayStripes";
        radComboBoxItem.Value = "GrayStripes";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "GreenStripes";
        radComboBoxItem.Value = "GreenStripes";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "LightBlue";
        radComboBoxItem.Value = "LightBlue";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "LightBrown";
        radComboBoxItem.Value = "LightBrown";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
        radComboBoxItem = new RadComboBoxItem(); 
        radComboBoxItem.Text = "LightGreen";
        radComboBoxItem.Value = "LightGreen";
        RadComboBoxSkin.Items.Add(radComboBoxItem);
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

    }
    protected void RadChart1_ItemDataBound(object sender, ChartItemDataBoundEventArgs e)
    {
        e.SeriesItem.Name = ((DataRowView)e.DataItem)["disposition"].ToString();
        e.SeriesItem.Appearance.FillStyle.FillType = Telerik.Charting.Styles.FillType.Solid;
        e.SeriesItem.Appearance.FillStyle.MainColor = Colors[e.SeriesItem.Index];
    }
    protected void btnSetSkin_Click(object sender, EventArgs e)
    {
        RadChart1.Skin = RadComboBoxSkin.SelectedValue;
        CallListWS m_callList = new CallListWS();
        m_callList.SetAgentSkin(RadComboBoxSkin.SelectedValue, strLoginId);
    }
}