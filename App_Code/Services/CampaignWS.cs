using System;
using System.Collections;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data.SqlClient;
using System.Xml;
using System.Data;
using System.Web.Script.Services;
using System.Configuration;
using System.Data.SqlTypes;
using System.Data.Odbc;
using System.IO;
using System.Net;

/// <summary>
/// Summary description for CampaignWS
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class CampaignWS : System.Web.Services.WebService
{

    DataBase m_Connection;
    string strTemp = "";
    string strParameters = "";
    DataSet m_DataSet;
    public CampaignWS()
    {

        m_Connection = new DataBase();
    }

    [WebMethod]
    public DataSet getdropdowndata(int ServiceId)
    {
        DataSet m_DataSet = new DataSet();
        int nResultCode = -1;
        string strResult = "Fail - ";
        m_Connection.OpenDB("Interdailog");
        try
        {

            strTemp = "select param_field_itemdata,param_field_value,param_field_name " +
                           "from cti_service_parameters " +
                            "where param_service_id= " + ServiceId + " ";

            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapterODBC.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "DropdownTable";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "getdropdowndata", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "getdropdowndata", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    [WebMethod]
    public DataSet getdatabsename(int ServiceId)
    {
        DataSet m_DataSet = new DataSet();
        int nResultCode = -1;
        string strResult = "Fail - ";
        try
        {
            m_Connection.OpenDB("Interdailog");
            string strTemp = "Select service_id,service_leadstructure_master_tablename,service_outbound_lead_db_name from cti_services where service_id=" + ServiceId;
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapterODBC.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "GetTableName";
            nResultCode = 0;
            strResult = "Pass";

        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "getdatabsename", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "getdatabsename", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    public DataSet GetLeadData(int ServiceId, int lead_id)
    {
        DataSet m_DataSet = new DataSet(); 
        m_DataSet= getdatabsename(ServiceId);
        DataTable leaddata = new DataTable("leaddata");
        int nResultCode = -1;
        string strResult = "Fail - ";
        try
        {
            m_Connection.OpenDB("Interdailog");
            string databasename = m_DataSet.Tables["GetTableName"].Rows[0]["service_outbound_lead_db_name"].ToString();
            string tablename = m_DataSet.Tables["GetTableName"].Rows[0]["service_leadstructure_master_tablename"].ToString();
            string strTemp = "Select " + m_Connection.DB_TOP_SQL + " " + m_Connection.DB_NULL + "(udf_Lead_Reference_id, '') as udf_Lead_Reference_id, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Title, '') AS Lead_Title, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Suffix, '') AS Lead_Suffix, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Surname, '') AS udf_Lead_Surname, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Given_Name, '') AS udf_Lead_Given_Name, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Date_of_Birth, '') AS udf_Lead_Date_of_Birth, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Australian_Resident, '') AS udf_Lead_Australian_Resident, " +
            "" + m_Connection.DB_NULL + "([udf_Lead_AustralianCitizen], '') AS udf_Lead_AustralianCitizen, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Drivers_License, '') AS udf_Lead_Drivers_License, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_State_of_Issue, '') AS udf_Lead_State_of_Issue, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Marital_Status, '') AS udf_Lead_Marital_Status, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_TFN, '') AS udf_Lead_TFN, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Adults, '') AS udf_Lead_Adults, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_No_Of_Dependents, '') AS udf_Lead_No_Of_Dependents, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Minors,'')AS udf_Lead_Minors, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_FinancialDetails_Gross_Annual_Income$,'')AS udf_Lead_FinancialDetails_Gross_Annual_Income$, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_FinancialDetails_Household_Income,'')AS udf_Lead_FinancialDetails_Household_Income, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_FinancialDetails_Mortgage,'')AS udf_Lead_FinancialDetails_Mortgage, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_FinancialDetails_MortgageName_of_the_Lender1,'')AS udf_Lead_FinancialDetails_MortgageName_of_the_Lender1, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender1,'')AS udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender1, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_FinancialDetails_MortgageBalanceOwingLender1, '') AS udf_Lead_FinancialDetails_MortgageBalanceOwingLender1, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_FinancialDetails_MortgageName_of_the_Lender2, '') AS udf_Lead_FinancialDetails_MortgageName_of_the_Lender2, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender2, '') AS udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender2, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_FinancialDetails_MortgageBalanceOwingLender2, '') AS udf_Lead_FinancialDetails_MortgageBalanceOwingLender2, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_FinancialDetails_MortgageName_of_the_Lender3, '') AS udf_Lead_FinancialDetails_MortgageName_of_the_Lender3, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender3, '') AS udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender3, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_FinancialDetails_MortgageBalanceOwingLender3, 0) AS udf_Lead_FinancialDetails_MortgageBalanceOwingLender3, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Personal_Loan_Debts, '') AS udf_Lead_Personal_Loan_Debts, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_PersonalLoan_Name_of_the_Lender1, '') AS udf_Lead_PersonalLoan_Name_of_the_Lender1, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_PersonalLoan_MonthlyRepaymentLender1, '') AS udf_Lead_PersonalLoan_MonthlyRepaymentLender1, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_PersonalLoan_BalanceOwingLender1, '') AS udf_Lead_PersonalLoan_BalanceOwingLender1, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_PersonalLoan_Name_of_the_Lender2, '') AS udf_Lead_PersonalLoan_Name_of_the_Lender2, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_PersonalLoan_MonthlyRepaymentLender2, '') AS udf_Lead_PersonalLoan_MonthlyRepaymentLender2, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_PersonalLoan_BalanceOwingLender2, '') AS udf_Lead_PersonalLoan_BalanceOwingLender2, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_PersonalLoan_Name_of_the_Lender3, '') AS udf_Lead_PersonalLoan_Name_of_the_Lender3, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_PersonalLoan_MonthlyRepaymentLender3, '') AS udf_Lead_PersonalLoan_MonthlyRepaymentLender3, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_PersonalLoan_BalanceOwingLender3, '') AS udf_Lead_PersonalLoan_BalanceOwingLender3, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Credit_Card, '') AS udf_Lead_Credit_Card, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Credit_Name_of_the_Lender1, '') AS udf_Lead_Credit_Name_of_the_Lender1, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Credit_MonthlyRepaymentLender1, '') AS udf_Lead_Credit_MonthlyRepaymentLender1, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Credit_BalanceOwingLender1, '') AS udf_Lead_Credit_BalanceOwingLender1, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Credit_Name_of_the_Lender2, '') AS udf_Lead_Credit_Name_of_the_Lender2, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Credit_MonthlyRepaymentLender2, '') AS udf_Lead_Credit_MonthlyRepaymentLender2, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Credit_BalanceOwingLender2, '') AS udf_Lead_Credit_BalanceOwingLender2, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Credit_Name_of_the_Lender3,'')AS udf_Lead_Credit_Name_of_the_Lender3, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Credit_MonthlyRepaymentLender3, '') As udf_Lead_Credit_MonthlyRepaymentLender3, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Credit_BalanceOwingLender3, '') AS udf_Lead_Credit_BalanceOwingLender3, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_House1, '') AS udf_Lead_Asset_House1, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_House1_CurrentValue, '') AS udf_Lead_Asset_House1_CurrentValue, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_House1_AmountInsured, '') AS udf_Lead_Asset_House1_AmountInsured, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_House1_InsuranceCompany, '') AS udf_Lead_Asset_House1_InsuranceCompany, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_House2, '') AS udf_Lead_Asset_House2, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_House2_CurrentValue, '') AS udf_Lead_Asset_House2_CurrentValue, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_House2_AmountInsured,'')AS udf_Lead_Asset_House2_AmountInsured, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_House2_InsuranceCompany, '') AS udf_Lead_Asset_House2_InsuranceCompany, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_Car1, '') AS udf_Lead_Asset_Car1, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_Car1_CurrentValue, '') AS udf_Lead_Asset_Car1_CurrentValue, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_Car1_AmountInsured, '') AS udf_Lead_Asset_Car1_AmountInsured, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_Car1_InsuranceCompany, '') AS udf_Lead_Asset_Car1_InsuranceCompany, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_Car2, '') AS udf_Lead_Asset_Car2, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_Car2_CurrentValue,'')AS udf_Lead_Asset_Car2_CurrentValue, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_Car2_AmountInsured, '') AS udf_Lead_Asset_Car2_AmountInsured, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_Car2_InsuranceCompany, '') AS udf_Lead_Asset_Car2_InsuranceCompany, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Account_Institution1_Name, '') AS udf_Lead_Account_Institution1_Name, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Account_Institution1_Number, '') AS udf_Lead_Account_Institution1_Number, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Account_Institution1_Balance, '') AS udf_Lead_Account_Institution1_Balance, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Account_Institution2_Name, '') AS udf_Lead_Account_Institution2_Name, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Account_Institution2_Number,'')AS udf_Lead_Account_Institution2_Number, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Account_Institution2_Balance, '') AS udf_Lead_Account_Institution2_Balance, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Account_Institution3_Name, '') AS udf_Lead_Account_Institution3_Name, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Account_Institution3_Number, '') AS udf_Lead_Account_Institution3_Number, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Account_Institution3_Balance, '') AS udf_Lead_Account_Institution3_Balance, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_Jewellery, '') AS udf_Lead_Asset_Jewellery, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_Jewellery_CurrentValue, '') AS udf_Lead_Asset_Jewellery_CurrentValue, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_HomeContents,'')AS udf_Lead_Asset_HomeContents, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_HomeContents_CurrentValue, '') AS udf_Lead_Asset_HomeContents_CurrentValue, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_FaceValue, '') AS udf_Lead_Asset_FaceValue, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_FaceValue_CurrentValue, '') AS udf_Lead_Asset_FaceValue_CurrentValue, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_SuperAnnuation, '') AS udf_Lead_Asset_SuperAnnuation, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_SuperAnnuation_CurrentValue, '') AS udf_Lead_Asset_SuperAnnuation_CurrentValue, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_OtherInvestments, '') AS udf_Lead_Asset_OtherInvestments, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_OtherInvestments_CurrentValue,'')AS udf_Lead_Asset_OtherInvestments_CurrentValue, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_Income, '') AS udf_Lead_Asset_Income, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_IncomeInsurance, '') AS udf_Lead_Asset_IncomeInsurance, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_IncomeInsurance_Value, '') AS udf_Lead_Asset_IncomeInsurance_Value, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_IncomeInsurance_Company, '') AS udf_Lead_Asset_IncomeInsurance_Company, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_HomeInsurance, '') AS udf_Lead_Asset_HomeInsurance, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_HomeInsurance_Value, '') AS udf_Lead_Asset_HomeInsurance_Value, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_HomeInsurance_Company, '') AS udf_Lead_Asset_HomeInsurance_Company, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_BusinessInsurance, '') AS udf_Lead_Asset_BusinessInsurance, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_BusinessInsurance_Value, '') AS udf_Lead_Asset_BusinessInsurance_Value, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Asset_BusinessInsurance_Company, '') AS udf_Lead_Asset_BusinessInsurance_Company, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Mortgage_Loan_Amount, '') AS udf_Lead_Mortgage_Loan_Amount, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Mortgage_Repayment_Tenure, '') AS udf_Lead_Mortgage_Repayment_Tenure, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Mortgage_Interest_Type, '') AS udf_Lead_Mortgage_Interest_Type, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_Comments, '') AS udf_Lead_Comments, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_HomeAddressLine1,'')AS udf_Lead_AddressDetails_HomeAddressLine1, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_HomeAddressLine2, '') AS udf_Lead_AddressDetails_HomeAddressLine2, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_HomeAddressLine2_Add1,'')AS udf_Lead_AddressDetails_HomeAddressLine2_Add1, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_HomeAddressLine2_Add2, '') AS udf_Lead_AddressDetails_HomeAddressLine2_Add2, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_Suburb, '') AS udf_Lead_AddressDetails_Suburb, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_State, '') AS udf_Lead_AddressDetails_State, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_Post_Code, '') AS udf_Lead_AddressDetails_Post_Code, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_Years_at_Address, '') AS udf_Lead_AddressDetails_Years_at_Address, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_PreviousAddress, '') AS udf_Lead_AddressDetails_PreviousAddress, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_PreviousAddress_State, '') AS udf_Lead_AddressDetails_PreviousAddress_State, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_PreviousAddress_PostCode, '') AS udf_Lead_AddressDetails_PreviousAddress_PostCode, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_PreviousAddress_YearAddress, '') AS udf_Lead_AddressDetails_PreviousAddress_YearAddress, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_Address_State, '') AS udf_Lead_AddressDetails_Address_State, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_Address_PostCode, '') AS udf_Lead_AddressDetails_Address_PostCode, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_CurrentResident, '') AS udf_Lead_AddressDetails_CurrentResident, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_CurrentResident_Status, '') AS udf_Lead_AddressDetails_CurrentResident_Status, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AddressDetails_CurrentResident_Post, '') AS udf_Lead_AddressDetails_CurrentResident_Post, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_ContactDetails_HomePhoneNumber, '') AS udf_Lead_ContactDetails_HomePhoneNumber, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_ContactDetails_WorkPhoneNumber,'')AS udf_Lead_ContactDetails_WorkPhoneNumber, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_ContactDetails_Extension, '') AS udf_Lead_ContactDetails_Extension, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_ContactDetails_Day_Time_Number, '') AS udf_Lead_ContactDetails_Day_Time_Number, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_ContactDetails_EveningNumber, '') AS udf_Lead_ContactDetails_EveningNumber, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_ContactDetails_MobilePhoneNumber, '') AS udf_Lead_ContactDetails_MobilePhoneNumber, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_ContactDetails_FaxNumber, '') AS udf_Lead_ContactDetails_FaxNumber, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_ContactDetails_Eail_ID, '') AS udf_Lead_ContactDetails_Eail_ID, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_EmploymentDetails_EmploymentStatus, '') AS udf_Lead_EmploymentDetails_EmploymentStatus, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_EmploymentDetails_EmploymentStatus1, '') AS udf_Lead_EmploymentDetails_EmploymentStatus1, " +

            "" + m_Connection.DB_NULL + "(udf_Lead_EmploymentDetails_Employer_Name_Address_Number1, '') AS udf_Lead_EmploymentDetails_Employer_Name_Address_Number1, " +
             "" + m_Connection.DB_NULL + "(udf_Lead_EmploymentDetails_Employer_Name_Address_Number2, '') AS udf_Lead_EmploymentDetails_Employer_Name_Address_Number2, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_EmploymentDetails_Occupation, '') AS udf_Lead_EmploymentDetails_Occupation, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_EmploymentDetails_JobTitle, '') AS udf_Lead_EmploymentDetails_JobTitle, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AccountantDetails_AccountingFirmName,'')AS udf_Lead_AccountantDetails_AccountingFirmName, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AccountantDetails_AccountantsName, '') AS udf_Lead_AccountantDetails_AccountantsName, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AccountantDetails_Address, '') AS udf_Lead_AccountantDetails_Address, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_AccountantDetails_PhoneNumber, '') AS udf_Lead_AccountantDetails_PhoneNumber, " +
            "" + m_Connection.DB_NULL + "(udf_Lead_sold, '0') AS udf_Lead_sold " +
            "from " + databasename + ".." + tablename +
            " where lead_id = " + lead_id;
            m_DataSet = new DataSet();
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            OdbcDataAdapter m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapterODBC.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "leaddata";
            nResultCode = 0;
            strResult = "Pass";
           


        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetLeadData", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetLeadData", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }

    public DataSet UpdateLeaddata(int ServiceId, int lead_id, string szTitle, string szSuffix, string szSurname,
                              string szGivenName, string szDOB, string szAustralianResident, string szAustralianCitizen,
                            string szStateofIssue, string szDriversLicense, string szMaritalStatus, int szTFN,
                            int szAdults, int szDependents, int szMinors, int szGrossAnnualIncome,
                            int szHouseholdIncome, string szMortgage, string szMortgageLender1, int szMortgageMonthlyRepaymentLender1,
                            int szMortgageBalanceOwingLender1, string szMortgageLender2, int szMortgageMonthlyRepaymentLender2,
                            int szMortgageBalanceOwingLender2, string szMortgageLender3, int szMortgageMonthlyRepaymentLender3,
                            int szMortgageBalanceOwingLender3, string szPersonalLoanDebts, string sPersonalLoanLender1,
                            int szPersonalLoanMonthlyRepaymentLender1, int szPersonalLoanBalanceOwingLender1, string szPersonalLoanLender2,
                            int szPersonalLoanMonthlyRepaymentLender2, int szPersonalLoanBalanceOwingLender2, string szPersonalLoanLender3,
                            int szPersonalLoanMonthlyRepaymentLender3, int szPersonalLoanBalanceOwingLender3, string szCreditCard,
                            string szCreditLender1, int szCreditMonthlyRepaymentLender1, int szCreditBalanceOwingLender1, string szCreditLender2,
                            int szCreditMonthlyRepaymentLender2, int szCreditBalanceOwingLender2, string szCreditLender3, int szCreditMonthlyRepaymentLender3,
                            int szCreditBalanceOwingLender3, string szAssetHouse1, int szAssetHouse1CurrentValue, int szAssetHouse1AmountInsured,
                            string szAssetHouse1InsuranceCompany, string szAssetHouse2, int szAssetHouse2CurrentValue, int szAssetHouse2AmountInsured
                           , string szAssetHouse2InsuranceCompany, string szAssetCar1, int szAssetCar1CurrentValue, int szAssetCar1AmountInsured,
                            string szAssetCar1InsuranceCompany, string szLeadAssetCar2, int szAssetCar2CurrentValue, int szAssetCar2AmountInsured
                            , string szAssetCar2InsuranceCompany, string szAccountInstitution1_Name, int szAccountInstitution1_Number,
                            int szAccountInstitution1Balance, string szAccountInstitution2Name, int szAccountInstitution2Number
                            , int szAccountInstitution2Balance, string szAccountInstitution3Name, int szAccountInstitution3Number,
                             int szAccountInstitution3Balance, string szJewellery, int szJewelleryCurrentValue, string szAssetHomeContents
                            , int szAssetHomeContentsCurrentValue, string szAssetFaceValue, int szFaceValueCurrentValue, string szSuperAnnuation,
                            int szSuperAnnuationCurrentValue, string szOtherInvestments, int szOtherInvestmentsCurrentValue, string szAssetIncome
                            , string szAssetIncomeInsurance, int szAssetIncomeInsuranceValue, string szAssetIncomeInsuranceCompany, string szAssetHomeInsurance
                            , int szAssetHomeInsuranceValue, string szAssetHomeInsuranceCompany, string szAssetBusinessInsurance, int szAssetBusinessInsuranceValue
                            , string szAssetBusinessInsuranceCompany, int szdMortgageLoanAmount, int szMortgageRepaymentTenure, string szMortgageInterestType
                            , string szComments, string szHomeAddressLine1, string szHomeAddressLine2, string szHomeAddressLine2Add1, string szHomeAddressLine2Add2,
                             string szAddressSuburb, string szAddressState, string szAddressPostCode, string szAddress_YearsatAddress, string szAddress_PreviousAddress
                            , string szPreviousAddress_State, string szPreviousAddress_PostCode, string szPreviousAddress_YearAddress, string szAddressDetails_State
                            , string szAddress_PostCode, string szCurrentResident, string szResident_Status, string szResident_Post, string szHomePhoneNumber, string szWorkPhoneNumber
                            , string szExtension, string szDayTimeNumber, string szEveningNumber, string szMobilePhoneNumber, string szFaxNumber, string szEail_ID, string szEmploymentStatus
                            , string szEmploymentStatus1, string szEmployerAddress_Number1, string szEmployerAddress_Number2, string szOccupation, string szJobTitle
                            , string szAccountingFirmName, string szAccountantsName, string szAccountantDetails_Address, string szAccountantDetails_PhoneNumber,string sold)
    {
        DataSet m_DataSet = getdatabsename(ServiceId);
        DataTable leaddata = new DataTable("leaddata");
        int nResultCode = -1;
        string strResult = "Fail - ";
        try
        {

            m_Connection.OpenDB("Interdailog");
            string databasename = m_DataSet.Tables["GetTableName"].Rows[0]["service_outbound_lead_db_name"].ToString();
            string tablename = m_DataSet.Tables["GetTableName"].Rows[0]["service_leadstructure_master_tablename"].ToString();
            string strTemp = "update " + databasename + ".." + tablename + " Set " +
                            "udf_Lead_Title = ?," +
            "udf_Lead_Suffix = ?," +
            "udf_Lead_Surname = ?," +
            "udf_Lead_Given_Name = ?," +
            "udf_Lead_Date_of_Birth = ?," +
            "udf_Lead_Australian_Resident = ?," +
            "udf_Lead_AustralianCitizen = ?," +
            "udf_Lead_Drivers_License = ?," +
            "udf_Lead_State_of_Issue = ?," +
            "udf_Lead_Marital_Status = ?," +
            "udf_Lead_TFN = ?," +
            "udf_Lead_Adults = ?," +
            "udf_Lead_No_Of_Dependents = ?," +
            "udf_Lead_Minors = ?," +
            "udf_Lead_FinancialDetails_Gross_Annual_Income$ = ?," +
            "udf_Lead_FinancialDetails_Household_Income = ?," +
            "udf_Lead_FinancialDetails_Mortgage = ?," +
            "udf_Lead_FinancialDetails_MortgageName_of_the_Lender1 = ?," +
            "udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender1 = ?," +
            "udf_Lead_FinancialDetails_MortgageBalanceOwingLender1 = ?," +
            "udf_Lead_FinancialDetails_MortgageName_of_the_Lender2 = ?," +
            "udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender2 = ?," +
            "udf_Lead_FinancialDetails_MortgageBalanceOwingLender2 = ?," +
            "udf_Lead_FinancialDetails_MortgageName_of_the_Lender3 = ?," +
            "udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender3 = ?," +
            "udf_Lead_FinancialDetails_MortgageBalanceOwingLender3 = ?," +
            "udf_Lead_Personal_Loan_Debts = ?," +
            "udf_Lead_PersonalLoan_Name_of_the_Lender1 = ?," +
            "udf_Lead_PersonalLoan_MonthlyRepaymentLender1 = ?," +
            "udf_Lead_PersonalLoan_BalanceOwingLender1 = ?," +
            "udf_Lead_PersonalLoan_Name_of_the_Lender2 = ?," +
            "udf_Lead_PersonalLoan_MonthlyRepaymentLender2 = ?," +
            "udf_Lead_PersonalLoan_BalanceOwingLender2 = ?," +
            "udf_Lead_PersonalLoan_Name_of_the_Lender3 = ?," +
            "udf_Lead_PersonalLoan_MonthlyRepaymentLender3 = ?," +
            "udf_Lead_PersonalLoan_BalanceOwingLender3 = ?," +
            "udf_Lead_Credit_Card = ?," +
            "udf_Lead_Credit_Name_of_the_Lender1 = ?," +
            "udf_Lead_Credit_MonthlyRepaymentLender1 = ?," +
            "udf_Lead_Credit_BalanceOwingLender1 = ?," +
            "udf_Lead_Credit_Name_of_the_Lender2 = ?," +
            "udf_Lead_Credit_MonthlyRepaymentLender2 = ?," +
            "udf_Lead_Credit_BalanceOwingLender2 = ?," +
            "udf_Lead_Credit_Name_of_the_Lender3 = ?," +
            "udf_Lead_Credit_MonthlyRepaymentLender3 = ?," +
            "udf_Lead_Credit_BalanceOwingLender3 = ?," +
            "udf_Lead_Asset_House1 = ?," +
            "udf_Lead_Asset_House1_CurrentValue = ?," +
            "udf_Lead_Asset_House1_AmountInsured = ?," +
            "udf_Lead_Asset_House1_InsuranceCompany = ?," +
            "udf_Lead_Asset_House2 = ?," +
            "udf_Lead_Asset_House2_CurrentValue = ?," +
            "udf_Lead_Asset_House2_AmountInsured = ?," +
            "udf_Lead_Asset_House2_InsuranceCompany = ?," +
            "udf_Lead_Asset_Car1 = ?," +
            "udf_Lead_Asset_Car1_CurrentValue = ?," +
            "udf_Lead_Asset_Car1_AmountInsured = ?," +
            "udf_Lead_Asset_Car1_InsuranceCompany = ?," +
            "udf_Lead_Asset_Car2 = ?," +
            "udf_Lead_Asset_Car2_CurrentValue = ?," +
            "udf_Lead_Asset_Car2_AmountInsured = ?," +
            "udf_Lead_Asset_Car2_InsuranceCompany = ?," +
            "udf_Lead_Account_Institution1_Name = ?," +
            "udf_Lead_Account_Institution1_Number = ?," +
            "udf_Lead_Account_Institution1_Balance = ?," +
            "udf_Lead_Account_Institution2_Name = ?," +
            "udf_Lead_Account_Institution2_Number = ?," +
            "udf_Lead_Account_Institution2_Balance = ?," +
            "udf_Lead_Account_Institution3_Name = ?," +
            "udf_Lead_Account_Institution3_Number = ?," +
            "udf_Lead_Account_Institution3_Balance = ?," +
            "udf_Lead_Asset_Jewellery = ?," +
            "udf_Lead_Asset_Jewellery_CurrentValue = ?," +
            "udf_Lead_Asset_HomeContents = ?," +
            "udf_Lead_Asset_HomeContents_CurrentValue = ?," +
            "udf_Lead_Asset_FaceValue = ?," +
            "udf_Lead_Asset_FaceValue_CurrentValue = ?," +
            "udf_Lead_Asset_SuperAnnuation = ?," +
            "udf_Lead_Asset_SuperAnnuation_CurrentValue = ?," +
            "udf_Lead_Asset_OtherInvestments = ?," +
            "udf_Lead_Asset_OtherInvestments_CurrentValue = ?," +
            "udf_Lead_Asset_Income = ?," +
            "udf_Lead_Asset_IncomeInsurance = ?," +
            "udf_Lead_Asset_IncomeInsurance_Value = ?," +
            "udf_Lead_Asset_IncomeInsurance_Company = ?," +
            "udf_Lead_Asset_HomeInsurance = ?," +
            "udf_Lead_Asset_HomeInsurance_Value = ?," +
            "udf_Lead_Asset_HomeInsurance_Company = ?," +
            "udf_Lead_Asset_BusinessInsurance = ?," +
            "udf_Lead_Asset_BusinessInsurance_Value = ?," +
            "udf_Lead_Asset_BusinessInsurance_Company = ?," +
            "udf_Lead_Mortgage_Loan_Amount = ?," +
            "udf_Lead_Mortgage_Repayment_Tenure = ?," +
            "udf_Lead_Mortgage_Interest_Type = ?," +
            "udf_Lead_Comments = ?," +
            "udf_Lead_AddressDetails_HomeAddressLine1 = ?," +
            "udf_Lead_AddressDetails_HomeAddressLine2 = ?," +
            "udf_Lead_AddressDetails_HomeAddressLine2_Add1 = ?," +
            "udf_Lead_AddressDetails_HomeAddressLine2_Add2 = ?," +
            "udf_Lead_AddressDetails_Suburb = ?," +
            "udf_Lead_AddressDetails_State = ?," +
            "udf_Lead_AddressDetails_Post_Code = ?," +
            "udf_Lead_AddressDetails_Years_at_Address = ?," +
            "udf_Lead_AddressDetails_PreviousAddress = ?," +
            "udf_Lead_AddressDetails_PreviousAddress_State = ?," +
            "udf_Lead_AddressDetails_PreviousAddress_PostCode = ?," +
            "udf_Lead_AddressDetails_PreviousAddress_YearAddress = ?," +
            "udf_Lead_AddressDetails_Address_State = ?," +
            "udf_Lead_AddressDetails_Address_PostCode = ?," +
            "udf_Lead_AddressDetails_CurrentResident = ?," +
            "udf_Lead_AddressDetails_CurrentResident_Status = ?," +
            "udf_Lead_AddressDetails_CurrentResident_Post = ?," +
            "udf_Lead_ContactDetails_HomePhoneNumber = ?," +
            "udf_Lead_ContactDetails_WorkPhoneNumber = ?," +
            "udf_Lead_ContactDetails_Extension = ?," +
            "udf_Lead_ContactDetails_Day_Time_Number = ?," +
            "udf_Lead_ContactDetails_EveningNumber = ?," +
            "udf_Lead_ContactDetails_MobilePhoneNumber = ?," +
            "udf_Lead_ContactDetails_FaxNumber = ?," +
            "udf_Lead_ContactDetails_Eail_ID = ?," +
            "udf_Lead_EmploymentDetails_EmploymentStatus = ?," +
            "udf_Lead_EmploymentDetails_EmploymentStatus1 = ?," +

            "udf_Lead_EmploymentDetails_Employer_Name_Address_Number1 = ?," +
             "udf_Lead_EmploymentDetails_Employer_Name_Address_Number2 = ?," +
            "udf_Lead_EmploymentDetails_Occupation = ?," +
            "udf_Lead_EmploymentDetails_JobTitle = ?," +
            "udf_Lead_AccountantDetails_AccountingFirmName = ?," +
            "udf_Lead_AccountantDetails_AccountantsName = ?," +
            "udf_Lead_AccountantDetails_Address = ?," +
            "udf_Lead_AccountantDetails_PhoneNumber = ?," +
            "udf_Lead_sold= ?" +
             " where lead_id = " + lead_id;
            m_DataSet = new DataSet();
            OdbcCommand m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Title", szTitle));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Suffix", szSuffix));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Surname", szSurname));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Given_Name", szGivenName));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Date_of_Birth", szDOB));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Australian_Resident", szAustralianResident));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AustralianCitizen", szAustralianCitizen));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_State_of_Issue", szStateofIssue));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Drivers_License", szDriversLicense));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Marital_Status", szMaritalStatus));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_TFN", szTFN));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Adults", szAdults));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_No_Of_Dependents", szDependents));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Minors", szMinors));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_FinancialDetails_Gross_Annual_Income$", szGrossAnnualIncome));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_FinancialDetails_Household_Income", szHouseholdIncome));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_FinancialDetails_Mortgage", szMortgage));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_FinancialDetails_MortgageName_of_the_Lender1", szMortgageLender1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender1", szMortgageMonthlyRepaymentLender1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_FinancialDetails_MortgageBalanceOwingLender1", szMortgageBalanceOwingLender1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_FinancialDetails_MortgageName_of_the_Lender2", szMortgageLender2));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender2", szMortgageMonthlyRepaymentLender2));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_FinancialDetails_MortgageBalanceOwingLender2", szMortgageBalanceOwingLender2));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_FinancialDetails_MortgageName_of_the_Lender3", szMortgageLender3));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender3", szMortgageMonthlyRepaymentLender3));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_FinancialDetails_MortgageBalanceOwingLender3", szMortgageBalanceOwingLender3));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Personal_Loan_Debts", szPersonalLoanDebts));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_PersonalLoan_Name_of_the_Lender1", sPersonalLoanLender1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_PersonalLoan_MonthlyRepaymentLender1", szPersonalLoanMonthlyRepaymentLender1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_PersonalLoan_BalanceOwingLender1", szPersonalLoanBalanceOwingLender1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_PersonalLoan_Name_of_the_Lender2", szPersonalLoanLender2));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_PersonalLoan_MonthlyRepaymentLender2", szPersonalLoanMonthlyRepaymentLender2));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_PersonalLoan_BalanceOwingLender2", szPersonalLoanBalanceOwingLender2));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_PersonalLoan_Name_of_the_Lender3", szPersonalLoanLender3));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_PersonalLoan_MonthlyRepaymentLender3", szPersonalLoanMonthlyRepaymentLender3));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_PersonalLoan_BalanceOwingLender3", szPersonalLoanBalanceOwingLender3));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Credit_Card", szCreditCard));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Credit_Name_of_the_Lender1", szCreditLender1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Credit_MonthlyRepaymentLender1", szCreditMonthlyRepaymentLender1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Credit_BalanceOwingLender1", szCreditBalanceOwingLender1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Credit_Name_of_the_Lender2", szCreditLender2));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Credit_MonthlyRepaymentLender2", szCreditMonthlyRepaymentLender2));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Credit_BalanceOwingLender2", szCreditBalanceOwingLender2));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Credit_Name_of_the_Lender3", szCreditLender3));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Credit_MonthlyRepaymentLender3", szCreditMonthlyRepaymentLender3));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Credit_BalanceOwingLender3", szCreditBalanceOwingLender3));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_House1", szAssetHouse1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_House1_CurrentValue", szAssetHouse1CurrentValue));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_House1_AmountInsured", szAssetHouse1AmountInsured));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_House1_InsuranceCompany", szAssetHouse1InsuranceCompany));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_House2", szAssetHouse2));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_House2_CurrentValue", szAssetHouse2CurrentValue));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_House2_AmountInsured", szAssetHouse2AmountInsured));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_House2_InsuranceCompany", szAssetHouse2InsuranceCompany));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_Car1", szAssetCar1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_Car1_CurrentValue", szAssetCar1CurrentValue));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_Car1_AmountInsured", szAssetCar1AmountInsured));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_Car1_InsuranceCompany", szAssetCar1InsuranceCompany));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_Car2", szLeadAssetCar2));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_Car2_CurrentValue", szAssetCar2CurrentValue));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_Car2_AmountInsured", szAssetCar2AmountInsured));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_Car2_InsuranceCompany", szAssetCar2InsuranceCompany));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Account_Institution1_Name", szAccountInstitution1_Name));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Account_Institution1_Number", szAccountInstitution1_Number));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Account_Institution1_Balance", szAccountInstitution1Balance));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Account_Institution2_Name", szAccountInstitution2Name));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Account_Institution2_Number", szAccountInstitution2Number));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Account_Institution2_Balance", szAccountInstitution2Balance));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Account_Institution3_Name", szAccountInstitution3Name));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Account_Institution3_Number", szAccountInstitution3Number));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Account_Institution3_Balance", szAccountInstitution3Balance));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_Jewellery", szJewellery));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_Jewellery_CurrentValue", szJewelleryCurrentValue));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_HomeContents", szAssetHomeContents));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_HomeContents_CurrentValue", szAssetHomeContentsCurrentValue));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_FaceValue", szAssetFaceValue));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_FaceValue_CurrentValue", szFaceValueCurrentValue));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_SuperAnnuation", szSuperAnnuation));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_SuperAnnuation_CurrentValue", szSuperAnnuationCurrentValue));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_OtherInvestments", szOtherInvestments));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_OtherInvestments_CurrentValue", szOtherInvestmentsCurrentValue));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_Income", szAssetIncome));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_IncomeInsurance", szAssetIncomeInsurance));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_IncomeInsurance_Value", szAssetIncomeInsuranceValue));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_IncomeInsurance_Company", szAssetIncomeInsuranceCompany));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_HomeInsurance", szAssetHomeInsurance));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_HomeInsurance_Value", szAssetHomeInsuranceValue));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_HomeInsurance_Company", szAssetHomeInsuranceCompany));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_BusinessInsurance", szAssetBusinessInsurance));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_BusinessInsurance_Value", szAssetBusinessInsuranceValue));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Asset_BusinessInsurance_Company", szAssetBusinessInsuranceCompany));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Mortgage_Loan_Amount", szdMortgageLoanAmount));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Mortgage_Repayment_Tenure", szMortgageRepaymentTenure));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Mortgage_Interest_Type", szMortgageInterestType));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_Comments", szComments));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_HomeAddressLine1", szHomeAddressLine1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_HomeAddressLine2", szHomeAddressLine2));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_HomeAddressLine2_Add1", szHomeAddressLine2Add1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_HomeAddressLine2_Add2", szHomeAddressLine2Add2));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_Suburb", szAddressSuburb));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_State", szAddressState));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_Post_Code", szAddressPostCode));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_Years_at_Address", szAddress_YearsatAddress));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_PreviousAddress", szAddress_PreviousAddress));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_PreviousAddress_State", szPreviousAddress_State));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_PreviousAddress_PostCode", szPreviousAddress_PostCode));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_PreviousAddress_YearAddress", szPreviousAddress_YearAddress));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_Address_State", szAddressDetails_State));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_Address_PostCode", szAddress_PostCode));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_CurrentResident", szCurrentResident));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_CurrentResident_Status", szResident_Status));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AddressDetails_CurrentResident_Post", szResident_Post));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_ContactDetails_HomePhoneNumber", szHomePhoneNumber));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_ContactDetails_WorkPhoneNumber", szWorkPhoneNumber));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_ContactDetails_Extension", szExtension));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_ContactDetails_Day_Time_Number", szDayTimeNumber));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_ContactDetails_EveningNumber", szEveningNumber));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_ContactDetails_MobilePhoneNumber", szMobilePhoneNumber));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_ContactDetails_FaxNumber", szFaxNumber));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_ContactDetails_Eail_ID", szEail_ID));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_EmploymentDetails_EmploymentStatus", szEmploymentStatus));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_EmploymentDetails_EmploymentStatus1", szEmploymentStatus1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_EmploymentDetails_Employer_Name_Address_Number1", szEmployerAddress_Number1));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_EmploymentDetails_Employer_Name_Address_Number2", szEmployerAddress_Number2));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_EmploymentDetails_Occupation", szOccupation));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_EmploymentDetails_JobTitle", szJobTitle));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AccountantDetails_AccountingFirmName", szAccountingFirmName));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AccountantDetails_AccountantsName", szAccountantsName));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AccountantDetails_Address", szAccountantDetails_Address));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_AccountantDetails_PhoneNumber", szAccountantDetails_PhoneNumber));
            m_CommandODBC.Parameters.Add(new OdbcParameter("@udf_Lead_sold", sold));
            nResultCode = m_CommandODBC.ExecuteNonQuery();
            strResult = "pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "UpdateLeaddata", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "UpdateLeaddata", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    public DataSet GetLeadDataForOutboundProcess(int ServiceId, int lead_id)
    {
        DataSet dataSet = new DataSet();
        dataSet = this.getdatabsename(ServiceId);
        new DataTable("leaddata");
        int errorCode = -1;
        string strResult = "Fail - ";
        try
        {
            this.m_Connection.OpenDB("Interdailog");
            string str2 = dataSet.Tables["GetTableName"].Rows[0]["service_outbound_lead_db_name"].ToString();
            string str3 = dataSet.Tables["GetTableName"].Rows[0]["service_leadstructure_master_tablename"].ToString();
            string cmdText = string.Concat(new object[] { 
            "Select ", this.m_Connection.DB_TOP_SQL, " ", this.m_Connection.DB_NULL, "(udf_Lead_Title, '') AS udf_Lead_Title, ", this.m_Connection.DB_NULL, "(udf_Lead_FirstName, '') AS udf_Lead_FirstName, ", this.m_Connection.DB_NULL, "(udf_Lead_MiddleName, '') AS udf_Lead_MiddleName, ", this.m_Connection.DB_NULL, "(udf_Lead_LastName, '') AS udf_Lead_LastName, ", this.m_Connection.DB_NULL, "(udf_Lead_DOB, '') AS udf_Lead_DOB, ", this.m_Connection.DB_NULL, "(udf_Lead_Emailid, '') AS udf_Lead_Emailid, ", this.m_Connection.DB_NULL, 
            "([udf_Lead_MaritalStatus], '') AS udf_Lead_MaritalStatus, ", this.m_Connection.DB_NULL, "(udf_Lead_SpouseName, '') AS udf_Lead_SpouseName, ", this.m_Connection.DB_NULL, "(udf_Lead_DrivingLicense, '') AS udf_Lead_DrivingLicense, ", this.m_Connection.DB_NULL, "(udf_Lead_Passport, '') AS udf_Lead_Passport, ", this.m_Connection.DB_NULL, "(udf_Lead_IdentificationNumber, '') AS udf_Lead_IdentificationNumber, ", this.m_Connection.DB_NULL, "(udf_Lead_BankName, '') AS udf_Lead_BankName, ", this.m_Connection.DB_NULL, "(udf_Lead_BankAccountNumber, '') AS udf_Lead_BankAccountNumber, ", this.m_Connection.DB_NULL, "(udf_Lead_BankBranch,'')AS udf_Lead_BankBranch, ", this.m_Connection.DB_NULL, 
            "(udf_Lead_DebitCardNumber,'')AS udf_Lead_DebitCardNumber, ", this.m_Connection.DB_NULL, "(udf_Lead_StartDate,'')AS udf_Lead_StartDate, ", this.m_Connection.DB_NULL, "(udf_Lead_EndDate,'')AS udf_Lead_EndDate, ", this.m_Connection.DB_NULL, "(udf_Lead_CardType,'')AS udf_Lead_CardType, ", this.m_Connection.DB_NULL, "(udf_Lead_Cvv,'')AS udf_Lead_Cvv, ", this.m_Connection.DB_NULL, "(udf_Lead_CreditCardNo, '') AS udf_Lead_CreditCardNo, ", this.m_Connection.DB_NULL, "(udf_Lead_CreditCardStartDate, '') AS udf_Lead_CreditCardStartDate, ", this.m_Connection.DB_NULL, "(udf_Lead_CreditCardEndDate, '') AS udf_Lead_CreditCardEndDate, ", this.m_Connection.DB_NULL, 
            "(udf_Lead_CreditCardCvv, '') AS udf_Lead_CreditCardCvv, ", this.m_Connection.DB_NULL, "(udf_Lead_CreditCardType, '') AS udf_Lead_CreditCardType, ", this.m_Connection.DB_NULL, "(udf_Lead_CurrentAddressNoAndStreet, '') AS udf_Lead_CurrentAddressNoAndStreet, ", this.m_Connection.DB_NULL, "(udf_Lead_CurrentAddressCountry, 0) AS udf_Lead_CurrentAddressCountry, ", this.m_Connection.DB_NULL, "(udf_Lead_CurrentAddressState, '') AS udf_Lead_CurrentAddressState, ", this.m_Connection.DB_NULL, "(udf_Lead_CurrentAddressCity, '') AS udf_Lead_CurrentAddressCity, ", this.m_Connection.DB_NULL, "(udf_Lead_CurrentAddressPostZipCode, '') AS udf_Lead_CurrentAddressPostZipCode, ", this.m_Connection.DB_NULL, "(udf_Lead_PreviousAddress1NoAndStreet, '') AS udf_Lead_PreviousAddress1NoAndStreet, ", this.m_Connection.DB_NULL, 
            "(udf_Lead_PreviousAddress1Country, '') AS udf_Lead_PreviousAddress1Country, ", this.m_Connection.DB_NULL, "(udf_Lead_PreviousAddress1State, '') AS udf_Lead_PreviousAddress1State, ", this.m_Connection.DB_NULL, "(udf_Lead_PreviousAddress1City, '') AS udf_Lead_PreviousAddress1City, ", this.m_Connection.DB_NULL, "(udf_Lead_PreviousAddress1PostZipCode, '') AS udf_Lead_PreviousAddress1PostZipCode, ", this.m_Connection.DB_NULL, "(udf_Lead_PreviousAddress2NoAndStreet, '') AS udf_Lead_PreviousAddress2NoAndStreet, ", this.m_Connection.DB_NULL, "(udf_Lead_PreviousAddress2Country, '') AS udf_Lead_PreviousAddress2Country, ", this.m_Connection.DB_NULL, "(udf_Lead_PreviousAddress2State, '') AS udf_Lead_PreviousAddress2State, ", this.m_Connection.DB_NULL, "(udf_Lead_PreviousAddress2City, '') AS udf_Lead_PreviousAddress2City, ", this.m_Connection.DB_NULL, 
            "(udf_Lead_PreviousAddress2PostZipCode, '') AS udf_Lead_PreviousAddress2PostZipCode, ", this.m_Connection.DB_NULL, "(udf_Lead_CompanyAddressNoAndStreet, '') AS udf_Lead_CompanyAddressNoAndStreet, ", this.m_Connection.DB_NULL, "(udf_Lead_CompanyAddressCountry, '') AS udf_Lead_CompanyAddressCountry, ", this.m_Connection.DB_NULL, "(udf_Lead_CompanyAddressState, '') AS udf_Lead_CompanyAddressState, ", this.m_Connection.DB_NULL, "(udf_Lead_CompanyAddressCity, '') AS udf_Lead_CompanyAddressCity, ", this.m_Connection.DB_NULL, "(udf_Lead_CompanyAddressPostZipCode,'')AS udf_Lead_CompanyAddressPostZipCode, ", this.m_Connection.DB_NULL, "(udf_Lead_HomePhone, '') As udf_Lead_HomePhone, ", this.m_Connection.DB_NULL, "(udf_Lead_WorkPhone, '') AS udf_Lead_WorkPhone, ", this.m_Connection.DB_NULL, 
            "(udf_Lead_MobileNo, '') AS udf_Lead_MobileNo, ", this.m_Connection.DB_NULL, "(udf_Lead_Fax, '') AS udf_Lead_Fax, ", this.m_Connection.DB_NULL, "(udf_Lead_sold, '0') AS udf_Lead_sold, ", this.m_Connection.DB_NULL, "(udf_Lead_Comment, '') AS udf_Lead_Comment from [", str2, "]..", str3, " where lead_id = ", lead_id
         });
            dataSet = new DataSet();
            OdbcCommand selectCommand = new OdbcCommand(cmdText, this.m_Connection.oCon);
            new OdbcDataAdapter(selectCommand).Fill(dataSet);
            dataSet.Tables[0].TableName = "leaddata";
            errorCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException exception)
        {
            errorCode = exception.ErrorCode;
            this.LogMessage(this.strTemp + exception.Message, "GetLeadData", this.strParameters);
        }
        catch (Exception exception2)
        {
            errorCode = -1;
            strResult = exception2.Message;
            this.LogMessage(this.strTemp + strResult, "GetLeadData", this.strParameters);
        }
        finally
        {
            this.m_Connection.CloseDB();
        }
        dataSet.Tables.Add(this.m_Connection.GetResponseTable((long)errorCode, strResult));
        return dataSet;
    }

    public DataSet UpdateUdfLead(int ServiceId, int lead_id, string udf_Lead_Title, string udf_Lead_FirstName, string udf_Lead_MiddleName, string udf_Lead_LastName, string udf_Lead_DOB, string udf_Lead_Emailid, string udf_Lead_MaritalStatus, string udf_Lead_SpouseName, string udf_Lead_DrivingLicense, string udf_Lead_Passport, string udf_Lead_IdentificationNumber, string udf_Lead_BankName, string udf_Lead_BankAccountNumber, string udf_Lead_BankBranch, string udf_Lead_DebitCardNumber, string udf_Lead_StartDate, string udf_Lead_EndDate, string udf_Lead_CardType, string udf_Lead_Cvv, string udf_Lead_CreditCardNo, string udf_Lead_CreditCardStartDate, string udf_Lead_CreditCardEndDate, string udf_Lead_CreditCardCvv, string udf_Lead_CreditCardType, string udf_Lead_CurrentAddressNoAndStreet, string udf_Lead_CurrentAddressCountry, string udf_Lead_CurrentAddressState, string udf_Lead_CurrentAddressCity, string udf_Lead_CurrentAddressPostZipCode, string udf_Lead_PreviousAddressstringNoAndStreet, string udf_Lead_PreviousAddressstringCountry, string udf_Lead_PreviousAddressstringState, string udf_Lead_PreviousAddressstringCity, string udf_Lead_PreviousAddressstringPostZipCode, string udf_Lead_PreviousAddress2NoAndStreet, string udf_Lead_PreviousAddress2Country, string udf_Lead_PreviousAddress2State, string udf_Lead_PreviousAddress2City, string udf_Lead_PreviousAddress2PostZipCode, string udf_Lead_CompanyAddressNoAndStreet, string udf_Lead_CompanyAddressCountry, string udf_Lead_CompanyAddressState, string udf_Lead_CompanyAddressCity, string udf_Lead_CompanyAddressPostZipCode, string udf_Lead_HomePhone, string udf_Lead_WorkPhone, string udf_Lead_MobileNo, string udf_Lead_Fax, string udf_Lead_Sold, string udf_Lead_Comment)
    {
        DataSet set = this.getdatabsename(ServiceId);
        new DataTable("leaddata");
        int errorCode = -1;
        string strResult = "Fail - ";
        try
        {
            this.m_Connection.OpenDB("Interdailog");
            string str2 = set.Tables["GetTableName"].Rows[0]["service_outbound_lead_db_name"].ToString();
            string str3 = set.Tables["GetTableName"].Rows[0]["service_leadstructure_master_tablename"].ToString();
            string cmdText = string.Concat(new object[] { "update [", str2, "]..", str3, " Set udf_Lead_Title = ?,udf_Lead_FirstName = ?,udf_Lead_MiddleName = ?,udf_Lead_LastName = ?,udf_Lead_DOB = ?,udf_Lead_Emailid = ?,udf_Lead_MaritalStatus = ?,udf_Lead_SpouseName = ?,udf_Lead_DrivingLicense = ?,udf_Lead_Passport = ?,udf_Lead_IdentificationNumber = ?,udf_Lead_BankName = ?,udf_Lead_BankAccountNumber = ?,udf_Lead_BankBranch = ?,udf_Lead_DebitCardNumber = ?,udf_Lead_StartDate = ?,udf_Lead_EndDate = ?,udf_Lead_CardType = ?,udf_Lead_Cvv = ?,udf_Lead_CreditCardNo = ?,udf_Lead_CreditCardStartDate = ?,udf_Lead_CreditCardEndDate = ?,udf_Lead_CreditCardCvv = ?,udf_Lead_CreditCardType = ?,udf_Lead_CurrentAddressNoAndStreet = ?,udf_Lead_CurrentAddressCountry = ?,udf_Lead_CurrentAddressState = ?,udf_Lead_CurrentAddressCity = ?,udf_Lead_CurrentAddressPostZipCode = ?,udf_Lead_PreviousAddress1NoAndStreet = ?,udf_Lead_PreviousAddress1Country = ?,udf_Lead_PreviousAddress1State = ?,udf_Lead_PreviousAddress1City = ?,udf_Lead_PreviousAddress1PostZipCode = ?,udf_Lead_PreviousAddress2NoAndStreet = ?,udf_Lead_PreviousAddress2Country = ?,udf_Lead_PreviousAddress2State = ?,udf_Lead_PreviousAddress2City = ?,udf_Lead_PreviousAddress2PostZipCode = ?,udf_Lead_CompanyAddressNoAndStreet = ?,udf_Lead_CompanyAddressCountry = ?,udf_Lead_CompanyAddressState = ?,udf_Lead_CompanyAddressCity = ?,udf_Lead_CompanyAddressPostZipCode = ?,udf_Lead_HomePhone = ?,udf_Lead_WorkPhone = ?,udf_Lead_MobileNo = ?,udf_Lead_Fax = ?,udf_Lead_Comment= ?,udf_Lead_sold= ? where lead_id = ", lead_id });
            set = new DataSet();
            OdbcCommand command = new OdbcCommand(cmdText, this.m_Connection.oCon);
            command.Parameters.Add(new OdbcParameter("@udf_Lead_Title", udf_Lead_Title));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_FirstName", udf_Lead_FirstName));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_MiddleName", udf_Lead_MiddleName));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_LastName", udf_Lead_LastName));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_DOB", udf_Lead_DOB));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_Emailid", udf_Lead_Emailid));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_MaritalStatus", udf_Lead_MaritalStatus));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_SpouseName", udf_Lead_SpouseName));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_DrivingLicense", udf_Lead_DrivingLicense));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_Passport", udf_Lead_Passport));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_IdentificationNumber", udf_Lead_IdentificationNumber));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_BankName", udf_Lead_BankName));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_BankAccountNumber", udf_Lead_BankAccountNumber));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_BankBranch", udf_Lead_BankBranch));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_DebitCardNumber", udf_Lead_DebitCardNumber));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_StartDate", udf_Lead_StartDate));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_EndDate", udf_Lead_EndDate));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CardType", udf_Lead_CardType));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_Cvv", udf_Lead_Cvv));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CreditCardNo", udf_Lead_CreditCardNo));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CreditCardStartDate", udf_Lead_CreditCardStartDate));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CreditCardEndDate", udf_Lead_CreditCardEndDate));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CreditCardCvv", udf_Lead_CreditCardCvv));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CreditCardType", udf_Lead_CreditCardType));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CurrentAddressNoAndStreet", udf_Lead_CurrentAddressNoAndStreet));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CurrentAddressCountry", udf_Lead_CurrentAddressCountry));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CurrentAddressState", udf_Lead_CurrentAddressState));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CurrentAddressCity", udf_Lead_CurrentAddressCity));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CurrentAddressPostZipCode", udf_Lead_CurrentAddressPostZipCode));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_PreviousAddress1NoAndStreet", udf_Lead_PreviousAddressstringNoAndStreet));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_PreviousAddress1Country", udf_Lead_PreviousAddressstringCountry));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_PreviousAddress1State", udf_Lead_PreviousAddressstringState));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_PreviousAddress1City", udf_Lead_PreviousAddressstringCity));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_PreviousAddress1PostZipCode", udf_Lead_PreviousAddressstringPostZipCode));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_PreviousAddress2NoAndStreet", udf_Lead_PreviousAddress2NoAndStreet));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_PreviousAddress2Country", udf_Lead_PreviousAddress2Country));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_PreviousAddress2State", udf_Lead_PreviousAddress2State));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_PreviousAddress2City", udf_Lead_PreviousAddress2City));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_PreviousAddress2PostZipCode", udf_Lead_PreviousAddress2PostZipCode));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CompanyAddressNoAndStreet", udf_Lead_CompanyAddressNoAndStreet));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CompanyAddressCountry", udf_Lead_CompanyAddressCountry));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CompanyAddressState", udf_Lead_CompanyAddressState));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CompanyAddressCity", udf_Lead_CompanyAddressCity));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_CompanyAddressPostZipCode", udf_Lead_CompanyAddressPostZipCode));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_HomePhone", udf_Lead_HomePhone));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_WorkPhone", udf_Lead_WorkPhone));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_MobileNo", udf_Lead_MobileNo));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_Fax", udf_Lead_Fax));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_Comment", udf_Lead_Comment));
            command.Parameters.Add(new OdbcParameter("@udf_Lead_sold", udf_Lead_Sold));
            errorCode = command.ExecuteNonQuery();
            strResult = "pass";
        }
        catch (OdbcException exception)
        {
            errorCode = exception.ErrorCode;
            this.LogMessage(this.strTemp + exception.Message, "insertUdfLead", this.strParameters);
        }
        catch (Exception exception2)
        {
            errorCode = -1;
            strResult = exception2.Message;
            this.LogMessage(this.strTemp + strResult, "insertUdfLead", this.strParameters);
        }
        finally
        {
            this.m_Connection.CloseDB();
        }
        set.Tables.Add(this.m_Connection.GetResponseTable((long)errorCode, strResult));
        return set;
    }

    #region Log Error Messages
    void LogMessage(string szMessage, string szMethodName, string szMethodParams)
    {
        CreateLog objCreateLog = new CreateLog();

        try
        {
            szMessage = "GlobalWS.cs - " + szMethodName +
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

