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

/// <summary>
/// Summary description for bo_CTIHistory
/// </summary>
public class bo_CTIHistory
{
    string strPatientID= string.Empty;   
    int intnID = 0;
    int intResultCode = 0;
    string strResultString = string.Empty;
    string strErrorSource = string.Empty;
    string strErrorStackTrace = string.Empty;
    string strType = string.Empty;
    string strCLI = string.Empty;

    string strLeadId = string.Empty;
    string strServiceId = string.Empty;

    string strUserId = string.Empty;
    string strCustomerId = string.Empty;

    string strCallnumber = string.Empty;
    string strCallmasterTable = string.Empty;


    string strudfCallMasterTableName = string.Empty;
   
    public string CallMasterTableName
    {
        get { return strudfCallMasterTableName; }
        set { strudfCallMasterTableName = value; }
    }
    public string CLI
    {
        get { return strCLI; }
        set { strCLI = value; }
    }

    public string LeadId
    {
        get { return strLeadId; }
        set { strLeadId = value; }
    }   

    public string Type
    {
        get { return strType; }
        set { strType = value; }
    }
    public string ServiceId
    {
        get { return strServiceId; }
        set { strServiceId = value; }
    }  
    public int ResultCode
    {
        get { return intResultCode; }
        set { intResultCode = value; }
    }
    public string ResultString
    {
        get { return strResultString; }
        set { strResultString = value; }
    }
    public string ErrorSource
    {
        get { return strErrorSource; }
        set { strErrorSource = value; }
    }
    public string ErrorStackTrace
    {
        get { return strErrorStackTrace; }
        set { strErrorStackTrace = value; }
    }
    public int nID
    {
        get { return intnID; }
        set { intnID = value; }
    }

    public string UserId
    {
        get { return strUserId; }
        set { strUserId = value; }
    }

    public string CustId
    {
        get { return strCustomerId; }
        set { strCustomerId = value; }
    }

    public string CallNumber
    {
        get { return strCallnumber; }
        set { strCallnumber = value; }
    }

    public string CallMasterTB
    {
        get { return strCallmasterTable; }
        set { strCallmasterTable = value; }
    }   


}
