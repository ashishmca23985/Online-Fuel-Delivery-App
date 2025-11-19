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
using System.Data.Odbc;

/// <summary>
/// Summary description for dal_InboundCTIHistory
/// </summary>
public class dal_CTIHistory
{
    String strIntreDia = System.Configuration.ConfigurationManager.AppSettings["IDGDB"].ToString();
    //String strIntreDia = "";
    public DataTable ConnectGEtSpGetCtiHistory(bo_CTIHistory CTIHistoryDetails)
    {
        using (OdbcConnection con3 = new OdbcConnection(strIntreDia))
        {
            DataTable dt1 = new DataTable();
            OdbcCommand cmd5 = null;
            try
            {
                cmd5 = new OdbcCommand("EXEC idg_sp_gethistory_Outbound ?,?,?", con3);
                con3.Open();
                cmd5.CommandType = CommandType.StoredProcedure;
                cmd5.Parameters.Add(new OdbcParameter("@nLeadId", CTIHistoryDetails.LeadId));
                cmd5.Parameters.Add(new OdbcParameter("@ServiceId", CTIHistoryDetails.ServiceId));
                cmd5.Parameters.Add(new OdbcParameter("@szCallmasterTable",""));//CTIHistoryDetails.CallMasterTableName)

                using (OdbcDataAdapter oda = new OdbcDataAdapter(cmd5))
                {
                    oda.Fill(dt1);
                }
                con3.Close();
            }
            catch (Exception ex)
            {
                string Result = ex.Message;
            }
            return dt1;
        }
    }
   public DataTable GetInboundCtiHistory(bo_CTIHistory CTIHistoryDetails)
    {
        using (OdbcConnection con3 = new OdbcConnection(strIntreDia))
        {
            DataTable dt1 = new DataTable();
            OdbcCommand cmd5 = null;
            try
            {
                cmd5 = new OdbcCommand("EXEC idg_sp_getInboundhistory_Galaxy ?,?", con3);
                con3.Open();
                cmd5.CommandType = CommandType.StoredProcedure;
                cmd5.Parameters.Add(new OdbcParameter("@CallNumber", CTIHistoryDetails.CallNumber));
                cmd5.Parameters.Add(new OdbcParameter("@CallmasterTB", CTIHistoryDetails.CallMasterTableName));
              
                using (OdbcDataAdapter oda = new OdbcDataAdapter(cmd5))
                {
                    oda.Fill(dt1);
                }
                con3.Close();
            }
            catch (Exception ex)
            {
                string Result = ex.Message;
            }
            return dt1;
        }
    }
   

}
