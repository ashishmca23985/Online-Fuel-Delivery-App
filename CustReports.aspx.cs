using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GalaxyReports;
using ReportsClasslibrary;
using System.ComponentModel;
using IVRReportPortal;

public partial class Invoice : ThemeBase
{
    DateTime FromDate = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day), ToDate = DateTime.Now.Date;
    protected void Page_Load(object sender, EventArgs e)
    {
        
        FromDate = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day);
        ToDate = DateTime.Now.Date;
        if (ReportViewer1.Report != null)
        {
            var list = ReportViewer1.Report.ReportParameters.ToList();
            if (list.Count > 0)
            {
                if (list[0].Type == Telerik.Reporting.ReportParameterType.DateTime)
                {
                    if (Convert.ToDateTime(list[0].Value) > DateTime.MinValue)
                        FromDate = Convert.ToDateTime(list[0].Value);
                }
                if (list[1].Type == Telerik.Reporting.ReportParameterType.DateTime)
                {
                    if (Convert.ToDateTime(list[1].Value) > DateTime.MinValue)
                        ToDate = Convert.ToDateTime(list[1].Value);
                }
            }
        }
    }
    protected void RadTreeView1_NodeClick(object sender, Telerik.Web.UI.RadTreeNodeEventArgs e)
    {
        if (e.Node.Value == "CAS-Status")
        {
            IVRReportPortal.CaseSummaryStatuswise rptCaseSummaryStatuswise = new IVRReportPortal.CaseSummaryStatuswise(TB_strUserName);
            rptCaseSummaryStatuswise.ReportParameters[0].Value = FromDate;
            rptCaseSummaryStatuswise.ReportParameters[1].Value = ToDate;
            ReportViewer1.Report = rptCaseSummaryStatuswise;
            ReportViewer1.DataBind();
        }
        else if (e.Node.Value == "CAS-Account")
        {
            IVRReportPortal.CaseSummaryAccountwise rptCaseSummaryAccountwise = new IVRReportPortal.CaseSummaryAccountwise(TB_strUserName);
            rptCaseSummaryAccountwise.ReportParameters[0].Value = FromDate;
            rptCaseSummaryAccountwise.ReportParameters[1].Value = ToDate;
            ReportViewer1.Report = rptCaseSummaryAccountwise;
            ReportViewer1.DataBind();
        }
        else if (e.Node.Value == "INV-Account")
        {
            IVRReportPortal.InvoicePrintAccount rptInvoiceAccountwise = new IVRReportPortal.InvoicePrintAccount();
            rptInvoiceAccountwise.ReportParameters[0].Visible = false;
            rptInvoiceAccountwise.ReportParameters[0].Value = TB_nAccountID.ToString();
            ReportViewer1.Report = rptInvoiceAccountwise;
            ReportViewer1.DataBind();
        }
        else if (e.Node.Value == "INV-SummaeryAccount")
        {
            IVRReportPortal.InvoiceAccountwise rptInvAccountwise = new IVRReportPortal.InvoiceAccountwise();
            rptInvAccountwise.ReportParameters[2].Visible = false;
            rptInvAccountwise.ReportParameters[0].Value = FromDate;
            rptInvAccountwise.ReportParameters[1].Value = ToDate;
            rptInvAccountwise.ReportParameters[2].Value = TB_nAccountID.ToString();
            ReportViewer1.Report = rptInvAccountwise;
            ReportViewer1.DataBind();
        }
        else if (e.Node.Value == "CAS-AccountCount")
        {
            IVRReportPortal.CaseCountLocationWise rptCaseCountLocationWise = new IVRReportPortal.CaseCountLocationWise();
            rptCaseCountLocationWise.ReportParameters[0].Value = FromDate;
            rptCaseCountLocationWise.ReportParameters[1].Value = ToDate;
            rptCaseCountLocationWise.ReportParameters[2].Value = TB_nAccountID.ToString();
            ReportViewer1.Report = rptCaseCountLocationWise;
            ReportViewer1.DataBind();
        }

    }

}
