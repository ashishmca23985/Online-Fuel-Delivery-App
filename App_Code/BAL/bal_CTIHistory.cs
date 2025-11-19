using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Telerik.Web.UI;

/// <summary>
/// Summary description for bal_CTIHistory
/// </summary>
public class bal_CTIHistory
{
    public bo_CTIHistory CtiHistory(ref RadGrid grvInquiry, bo_CTIHistory CTIHistoryDetails)
    {
        dal_CTIHistory CtiHistoryDetailDAL = new dal_CTIHistory();
        int nResultCode = 0;
        try
        {
            DataTable dt = new DataTable();
            dt = CtiHistoryDetailDAL.ConnectGEtSpGetCtiHistory(CTIHistoryDetails);           
            grvInquiry.DataSource = dt;
           // grvInquiry.DataBind();
            CTIHistoryDetails.ResultCode = nResultCode;
            return CTIHistoryDetails;
        }
        catch (Exception ex)
        {
            CTIHistoryDetails.ResultCode = -1;
            CTIHistoryDetails.ResultString = ex.Message;
            CTIHistoryDetails.ErrorSource = ex.Source;
            CTIHistoryDetails.ErrorStackTrace = ex.StackTrace;
            WritelogFile.WriteError("BAL:CtiHistory,DAl:ConnectGEtSpGetCtiHistory", ex.Message, ex.Source, ex.StackTrace, Convert.ToString(ex.GetType()));
            return CTIHistoryDetails;
        }
        finally
        {
            CtiHistoryDetailDAL = null;
        }
    }


    public bo_CTIHistory CtiInboundHistory(ref RadGrid grvInquiry, bo_CTIHistory CTIHistoryDetails)
    {
        dal_CTIHistory CtiInboundHistoryDetailDAL = new dal_CTIHistory();
        int nResultCode = 0;
        try
        {
            DataTable dt = new DataTable();
            dt = CtiInboundHistoryDetailDAL.GetInboundCtiHistory(CTIHistoryDetails);
            if (dt != null && dt.Columns.Count>0)
            {
                grvInquiry.DataSource = dt;
            }
            else
                grvInquiry.DataSource = null;
            // grvInquiry.DataBind();
            CTIHistoryDetails.ResultCode = nResultCode;
            return CTIHistoryDetails;
        }
        catch (Exception ex)
        {
            CTIHistoryDetails.ResultCode = -1;
            CTIHistoryDetails.ResultString = ex.Message;
            CTIHistoryDetails.ErrorSource = ex.Source;
            CTIHistoryDetails.ErrorStackTrace = ex.StackTrace;
            WritelogFile.WriteError("BAL:CtiInbundHistory,DAl:GetInboundCtiHistory", ex.Message, ex.Source, ex.StackTrace, Convert.ToString(ex.GetType()));
            return CTIHistoryDetails;
        }
        finally
        {
            CtiInboundHistoryDetailDAL = null;
        }
    }

    public DataTable CtiInboundHistory1(ref RadGrid grvInquiry, bo_CTIHistory CTIHistoryDetails)
    {
        dal_CTIHistory CtiInboundHistoryDetailDAL = new dal_CTIHistory();
        int nResultCode = 0;
        DataTable dt = new DataTable();
        try
        {
           
            dt = CtiInboundHistoryDetailDAL.GetInboundCtiHistory(CTIHistoryDetails);
            grvInquiry.DataSource = dt;
            // grvInquiry.DataBind();
          //  CTIHistoryDetails.ResultCode = nResultCode;
            return dt;
        }
        catch (Exception ex)
        {
            CTIHistoryDetails.ResultCode = -1;
            CTIHistoryDetails.ResultString = ex.Message;
            CTIHistoryDetails.ErrorSource = ex.Source;
            CTIHistoryDetails.ErrorStackTrace = ex.StackTrace;
            WritelogFile.WriteError("BAL:CtiInbundHistory,DAl:GetInboundCtiHistory", ex.Message, ex.Source, ex.StackTrace, Convert.ToString(ex.GetType()));
            return dt;
        }
        finally
        {
            CtiInboundHistoryDetailDAL = null;
        }
    }


}
