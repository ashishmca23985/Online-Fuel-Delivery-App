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

public partial class Reports : ThemeBase
{
    //ThemeBase ob = new ThemeBase();

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
            //else
            //{
            //    FromDate = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day);
            //    ToDate = DateTime.Now.Date;
            //}


        }
        //else
        //{
        //    FromDate = DateTime.Now.Date.AddDays(1-DateTime.Now.Day);
        //    ToDate = DateTime.Now.Date;
        //}
    }
    protected void RadTreeView1_NodeClick(object sender, Telerik.Web.UI.RadTreeNodeEventArgs e)
    {
        if (e.Node.Value == "CAS-Status")
        {
            IVRReportPortal.CaseSummaryStatuswise rptCaseSummaryStatuswise = new IVRReportPortal.CaseSummaryStatuswise(TB_strUserName);

           // CaseSummaryStatuswise rptCaseSummaryStatuswise = new CaseSummaryStatuswise(TB_strUserName);
            rptCaseSummaryStatuswise.ReportParameters[0].Value = FromDate;
            rptCaseSummaryStatuswise.ReportParameters[1].Value = ToDate;
            ReportViewer1.Report = rptCaseSummaryStatuswise;
            ReportViewer1.DataBind();
        }
        else if (e.Node.Value == "CAS-Account")
        {
            IVRReportPortal.CaseSummaryAccountwise rptCaseSummaryAccountwise = new IVRReportPortal.CaseSummaryAccountwise(TB_strUserName);

//            CaseSummaryAccountwise rptCaseSummaryAccountwise = new CaseSummaryAccountwise(TB_strUserName);
            //CaseSummaryStatuswise rptCaseSummaryStatuswise = new CaseSummaryStatuswise();
            rptCaseSummaryAccountwise.ReportParameters[0].Value = FromDate;
            rptCaseSummaryAccountwise.ReportParameters[1].Value = ToDate;
            ReportViewer1.Report = rptCaseSummaryAccountwise;
            ReportViewer1.DataBind();
        }
        else if (e.Node.Value == "CAS-Owner")
        {
            IVRReportPortal.CaseSummaryOwnerwise rptCaseSummaryOwnerwise = new IVRReportPortal.CaseSummaryOwnerwise(TB_strUserName);
            // CaseSummaryOwnerwise rptCaseSummaryOwnerwise = new CaseSummaryOwnerwise(TB_strUserName);
            //CaseSummaryStatuswise rptCaseSummaryStatuswise = new CaseSummaryStatuswise();
            rptCaseSummaryOwnerwise.ReportParameters[0].Value = FromDate;
            rptCaseSummaryOwnerwise.ReportParameters[1].Value = ToDate;
            ReportViewer1.Report = rptCaseSummaryOwnerwise;
            ReportViewer1.DataBind();
        }
        else if (e.Node.Value == "CAS-Assign")
        {
            IVRReportPortal.CaseSummaryAssignWise rptCaseSummaryAssignwise = new IVRReportPortal.CaseSummaryAssignWise(TB_strUserName);
            // CaseSummaryOwnerwise rptCaseSummaryOwnerwise = new CaseSummaryOwnerwise(TB_strUserName);
            //CaseSummaryStatuswise rptCaseSummaryStatuswise = new CaseSummaryStatuswise();
            rptCaseSummaryAssignwise.ReportParameters[0].Value = FromDate;
            rptCaseSummaryAssignwise.ReportParameters[1].Value = ToDate;
            ReportViewer1.Report = rptCaseSummaryAssignwise;
            ReportViewer1.DataBind();
        }
        else if (e.Node.Value == "ACT-Created")
        {
            IVRReportPortal.ActivitySummary rptActivitySummary = new IVRReportPortal.ActivitySummary(TB_strUserName);

           // ActivitySummary rptActivitySummary = new ActivitySummary(TB_strUserName);

            ReportViewer1.Report = rptActivitySummary;
            ReportViewer1.DataBind();
        }
        else if (e.Node.Value == "Inward")
        {
            IVRReportPortal.FuelInwardSummary rptInwawrdSummary = new IVRReportPortal.FuelInwardSummary();
            rptInwawrdSummary.ReportParameters[0].Value = FromDate;
            rptInwawrdSummary.ReportParameters[1].Value = ToDate;
            ReportViewer1.Report = rptInwawrdSummary;
            ReportViewer1.DataBind();
        }
        else if (e.Node.Value == "TOP-Five")
        {
            TopFiveCases rptTopFiveCases = new TopFiveCases();

            ReportViewer1.Report = rptTopFiveCases;
            ReportViewer1.DataBind();
        }

        else if (e.Node.Value == "INV-Account")
        {
            IVRReportPortal.InvoicePrintAccount rptInvoiceAccountwise = new IVRReportPortal.InvoicePrintAccount();
            rptInvoiceAccountwise.ReportParameters[0].Value = FromDate;
            rptInvoiceAccountwise.ReportParameters[1].Value = ToDate;
            ReportViewer1.Report = rptInvoiceAccountwise;
            ReportViewer1.DataBind();
        }
        else if (e.Node.Value == "INV-SummaeryAccount")
        {
            IVRReportPortal.InvoiceAccountwise rptInvAccountwise = new IVRReportPortal.InvoiceAccountwise();
            rptInvAccountwise.ReportParameters[0].Value = FromDate;
            rptInvAccountwise.ReportParameters[1].Value = ToDate;
            ReportViewer1.Report = rptInvAccountwise;
            ReportViewer1.DataBind();
        }
        else if (e.Node.Value == "CAS-AccountCount")
        {
            IVRReportPortal.CaseCountLocationWise rptCaseCountLocationWise = new IVRReportPortal.CaseCountLocationWise();
            rptCaseCountLocationWise.ReportParameters[0].Value = FromDate;
            rptCaseCountLocationWise.ReportParameters[1].Value = ToDate;
            ReportViewer1.Report = rptCaseCountLocationWise;
            ReportViewer1.DataBind();
        }
        else if (e.Node.Value == "EFN-Account")
        {
            IVRReportPortal.EFunnelReport rptEFunnelReport = new IVRReportPortal.EFunnelReport();
            rptEFunnelReport.ReportParameters[0].Value = FromDate;
            rptEFunnelReport.ReportParameters[1].Value = ToDate;
            ReportViewer1.Report = rptEFunnelReport;
            ReportViewer1.DataBind();
        }

        else if (e.Node.Value == "CAS-SiteId")
        {
            IVRReportPortal.CaseCountSiteNameWise rptSiteNameReport = new IVRReportPortal.CaseCountSiteNameWise();
            rptSiteNameReport.ReportParameters[0].Value = FromDate;
            rptSiteNameReport.ReportParameters[1].Value = ToDate;
            ReportViewer1.Report = rptSiteNameReport;
            ReportViewer1.DataBind();
        }


    }

}
