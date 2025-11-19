using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.Odbc;
using System.Data;

/// <summary>
/// Summary description for IVRWS
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class IVRWS : System.Web.Services.WebService
{

    public IVRWS()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string IVRTicket(string CustomerName,
                            string Mobile,
                            string Mail,
                            string Title,
                            string ProductCode,
                            string SubProductCode,
                            string TicketType,
                            string Category,
                            string SubCategory,
                            string CaseRemark,
                            string EndRemark,
                            string Status,
                            string CallNumber,
                            string CLI,
                            string AccountNo)
    {
        DataBase m_Connection = new DataBase();
        int nResultCode = -1;
        string strResult = "Fail";
        string strTemp = "";

        if (CustomerName.Length <= 0)
        {
            nResultCode = -1;
            strResult = "Missing Parameter CustomerName.";
            return strResult;
        }
        if (Mobile.Length <= 0)
        {
            nResultCode = -1;
            strResult = "Missing Parameter Mobile.";
            return strResult;
        }
        if (ProductCode.Length <= 0)
        {
            nResultCode = -1;
            strResult = "Missing Parameter ProductCode.";
            return strResult;
        }
        if (SubProductCode.Length <= 0)
        {
            nResultCode = -1;
            strResult = "Missing Parameter SubProductCode.";
            return strResult;
        }
        if (TicketType.Length <= 0)
        {
            nResultCode = -1;
            strResult = "Missing Parameter TicketType.";
            return strResult;
        }
        if (Category.Length <= 0)
        {
            nResultCode = -1;
            strResult = "Missing Parameter Category.";
            return strResult;
        }
        if (Status.Length <= 0)
        {
            nResultCode = -1;
            strResult = "Missing Parameter Status.";
            return strResult;
        }
        if (CallNumber.Length <= 0)
        {
            nResultCode = -1;
            strResult = "Missing Parameter CallNumber.";
            return strResult;
        }
        if (CLI.Length <= 0)
        {
            nResultCode = -1;
            strResult = "Missing Parameter CLI.";
            return strResult;
        }
        if (AccountNo.Length <= 0)
        {
            nResultCode = -1;
            strResult = "Missing Parameter AccountNo.";
            return strResult;
        }
        try
        {

            if (EndRemark == null || EndRemark.Length <= 0)
                EndRemark = CaseRemark;

            m_Connection.OpenDB("Galaxy");

            OdbcCommand m_CommandODBC = new OdbcCommand("IVR_Case_Insert ?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;

            m_CommandODBC.Parameters.Add("@CustomerName", OdbcType.VarChar).Value = CustomerName;
            m_CommandODBC.Parameters.Add("@Mobile", OdbcType.VarChar).Value = Mobile;
            m_CommandODBC.Parameters.Add("@Mail", OdbcType.VarChar).Value = Mail;
            m_CommandODBC.Parameters.Add("@Title", OdbcType.VarChar).Value = Title;
            m_CommandODBC.Parameters.Add("@ProductCode", OdbcType.VarChar).Value = ProductCode;
            m_CommandODBC.Parameters.Add("@SubProductCode", OdbcType.VarChar).Value = SubProductCode;
            m_CommandODBC.Parameters.Add("@TicketType", OdbcType.VarChar).Value = TicketType;
            m_CommandODBC.Parameters.Add("@Category", OdbcType.VarChar).Value = Category;
            m_CommandODBC.Parameters.Add("@SubCategory", OdbcType.VarChar).Value = SubCategory;
            m_CommandODBC.Parameters.Add("@Status", OdbcType.VarChar).Value = Status;
            m_CommandODBC.Parameters.Add("@CaseRemarks", OdbcType.VarChar).Value = CaseRemark;
            m_CommandODBC.Parameters.Add("@EndRemark", OdbcType.VarChar).Value = EndRemark;
            m_CommandODBC.Parameters.Add("@CallNumber", OdbcType.VarChar).Value = CallNumber;
            m_CommandODBC.Parameters.Add("@CLI", OdbcType.VarChar).Value = CLI;
            m_CommandODBC.Parameters.Add("@AccountNo", OdbcType.VarChar).Value = AccountNo;
            m_CommandODBC.Parameters.Add("@TicketId", OdbcType.VarChar, 70).Direction = ParameterDirection.Output;

            m_CommandODBC.ExecuteNonQuery();
            //   strResult = (m_CommandODBC.ExecuteNonQuery()>1)? "Pass":"Fail";
            m_Connection.CloseDB();
            strResult = (m_CommandODBC.Parameters["@TicketId"].Value.ToString().Trim().Length > 0) ? "Pass|" + m_CommandODBC.Parameters["@TicketId"].Value.ToString() : "Fail";

        }
        catch (OdbcException ex)
        {

            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
        }
        catch (Exception ex)
        {

            nResultCode = -1;
            strResult = ex.Message;
        }
        finally
        {


            m_Connection.CloseDB();
        }
        return strResult;
    }

    #region Log Error Messages
    void LogMessage(string szMessage, string szMethodName, string szMethodParams)
    {
        CreateLog objCreateLog = new CreateLog();

        try
        {
            szMessage = "IVRWS.cs - " + szMethodName +
                        "(" + szMethodParams + ") " + szMessage;

            objCreateLog.ErrorLog(szMessage);
        }
        catch (Exception ex)
        {
            string str = ex.Message;
        }
    }
    #endregion
}
