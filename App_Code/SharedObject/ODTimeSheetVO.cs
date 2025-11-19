using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedVO
{
    /// <summary>
    /// Summary description for ODTimeSheetVO
    /// </summary>
    public class ODTimeSheetVO : BaseVO
    {
        public int nId { get; set; }
        public string sType { get; set; }
        public int nEmpid { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public int nFromLoc { get; set; }
        public string sFromLoc { get; set; }
        public int nToLoc { get; set; }
        public string sToLoc { get; set; }
        public string sStatus { get; set; }
        public string sActionType { get; set; }
        public int nCaseId { get; set; }
        public string sCaseNumber { get; set; }
        public int nTaskId { get; set; }
        public string sTaskNumber { get; set; }
        public string sRemarks { get; set; }
    }
    public class OutstationVO : BaseVO
    {
        public int nId { get; set; }
        public int nEmpid { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public int nFromLoc { get; set; }
        public string sFromLoc { get; set; }
        public int nToLoc { get; set; }
        public string sToLoc { get; set; }
        public decimal dAdvance { get; set; }
        public string sRemarks { get; set; }
        public string sStatus { get; set; }
    }
    public class LeaveVO : BaseVO
    {
        public int nId { get; set; }
        public int nEmpid { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public string sLocType { get; set; }
        public string sRequestType { get; set; }
        public string sRemarks { get; set; }
        public string EmergencyNo { get; set; }
        public string sStatus { get; set; }
        public string HalfLeave { get; set; }
    }

    public class BaseVO
    {
        public string sIP { get; set; }
        public int nContactId { get; set; }

    }



    public class Quote
    {
        private string _stockTicker;
        private Decimal _stockQuote;
        private DateTime _lastUpdated;
        private Decimal _change;
        private Decimal _dailyMinRange;
        private Decimal _dailyMaxRange;

        public Quote()
        {

        }

        public Quote(string _stockTicker, Decimal _stockQuote, DateTime _lastUpdated, Decimal _change, Decimal _dailyMinRange, Decimal _dailyMaxRange)
        {
            this._stockTicker = _stockTicker;
            this._stockQuote = _stockQuote;
            this._lastUpdated = _lastUpdated;
            this._change = _change;
            this._dailyMinRange = _dailyMinRange;
            this._dailyMaxRange = _dailyMaxRange;
        }
        //New


        public string StockTicker
        {
            get { return _stockTicker; }
        }


        public Decimal StockQuote
        {
            get { return _stockQuote; }
            set { _stockQuote = value; }
        }


        public DateTime LastUpdated
        {
            get { return _lastUpdated; }
            set { _lastUpdated = value; }
        }


        public Decimal Change
        {
            get { return _change; }
            set { _change = value; }
        }


        public Decimal DailyMinRange
        {
            get { return _dailyMinRange; }
            set { _dailyMinRange = value; }
        }


        public Decimal DailyMaxRange
        {
            get { return _dailyMaxRange; }
            set { _dailyMaxRange = value; }
        }
    }

    public class dailyActivity
    {
        public int contactId { get; set; }
        public string contactName { get; set; }
        public int countinout { get; set; }
        public int countod { get; set; }
        public int counttimesheet { get; set; }
        public int countleave { get; set; }
        public int coluntexpense { get; set; }
        public int countOutstation { get; set; }

    }

    public class Contactslist
    {
        public int Id { get; set; }
        public string contact { get; set; }

    }
    public class Account
    {
        public int Id { get; set; }
        public string sAccount { get; set; }

    }

    public class TarnsactionDetails
    {
        public int Id { get; set; }
        public DateTime FromDt { get; set; }
        public DateTime Todt { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string Approvalremark { get; set; }
        public decimal Duration { get; set; }
        public string ToLocation { get; set; }
        public string Actiontype { get; set; }
        public decimal Amount { get; set; }
        public decimal ApproveAmt { get; set; }
        public string HrRemark { get; set; }
        public string FromLocation { get; set; }
        public bool Statuschange { get; set; }
        public string strFromdt
        {
            get
            {
                if (Actiontype == "TimeSheet" || Actiontype == "OD")
                    return FromDt.ToString("hh:mm tt");
                else
                    return FromDt.ToString("dd MMM yyyy");
            }
        }
        public string strTodt
        {
            get
            {
                if (Actiontype == "TimeSheet" || Actiontype == "OD")
                    return Todt.ToString("hh:mm tt");
                else
                    return Todt.ToString("dd MMM yyyy");
            }
        }
        public string sDate
        {
            get
            {
                return FromDt.ToString("dd MMM yyyy");
            }
        }
        public string strDuration
        {
            get
            {
                if (Actiontype == "Leave")
                    return Duration.ToString();
                else
                    return Helpter.MinutToStringConvert(Convert.ToInt32(Duration));
            }
        }

    }

    public class Chart
    {
        public int Id { get; set; }
        public DateTime WorkingDate { get; set; }
        public string DayType { get; set; }
        public string Location { get; set; }
        public string Outstation { get; set; }
        public int InTime { get; set; }
        public int OutTime { get; set; }
        public string DayRemark { get; set; }
        public string ModifyFlage { get; set; }
        public int OdCount { get; set; }
        public int OdDuration { get; set; }
        public int TSCount { get; set; }
        public int TSDuration { get; set; }
        public int ExtraDuration { get; set; }
        public decimal ExpClaimed { get; set; }
        public decimal ExpApproved { get; set; }
        public string sDate
        {
            get
            {
                return WorkingDate.ToString("dd MMM yyyy ");
            }
        }


    }

    public class ChatMessage
    {
           
        public string Talker { get; private set; }
        
        public string MessageData { get; private set; }
        
        public DateTime SendTime { get; private set; }
        
        
        public bool IsFriend { get; private set; }

       
    }
    public class CaseAnalysis
    {
        public string caseNumber { get; set; }
        public string CustomerName { get; set; }
    }



    public class LeadStatus
    {
        public string Name { get; set; }
        public int count { get; set; }
        public string color { get; set; }
    }
    public class LeadStatusBar
    {
        public string Name { get; set; }
        public int open { get; set; }
        public int working { get; set; }
        public int deferred { get; set; }
        public int close { get; set; }
        
    }

    public class ContactList
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string zone { get; set; }
    }
    public class GeneralField
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}