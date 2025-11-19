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
using Telerik.Web.UI;
using System.Net;
using System.Web.Services;
using System.Data.Odbc;
using System.Xml;

public partial class CompaignMortgage : ThemeBase
{
    CampaignWS obj_CampaignWS = new CampaignWS();
    int ServiceId=2 ;
    int LeadId=1 ;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            if (Request.QueryString["lead_id"] != null && Request.QueryString["service_id"] != null)
            {
                ServiceId = Convert.ToInt32(Request.QueryString["service_id"].ToString());
                LeadId = Convert.ToInt32(Request.QueryString["lead_id"].ToString());

                BindDropDowndata(ref cmbTitle, ref cmbAustralian, ref cmbStates,
                                ref cmbMaritalStatus, ref cmbMortgage, ref cmbPersonalLoan,
                                ref cmbCreditCard, ref cmbIncomeProtection, ref cmbHomeContents,
                                ref cmbBusiness, ref cmbInterestType, ref cmbSuburbState,
                                ref cmbPreviousState, ref cmbPreviousYearState, ref cmbCurrentResidential,
                                ref cmbDayTime, ref cmbEveningNumber, ref cmbEmpStatus);
                GetLeadDetails(ServiceId, LeadId);
            }
        }
    }
    public void BindDropDowndata(ref RadComboBox cmbTitle, ref RadComboBox cmbAustralian, ref RadComboBox cmbStates,
       ref RadComboBox cmbMaritalStatus, ref RadComboBox cmbMortgage, ref RadComboBox cmbPersonalLoan,
       ref RadComboBox cmbCreditCard, ref RadComboBox cmbIncomeProtection, ref RadComboBox cmbHomeContents,
       ref RadComboBox cmbBusiness, ref RadComboBox cmbInterestType, ref RadComboBox cmbSuburbState,
       ref RadComboBox cmbPreviousState, ref RadComboBox cmbPreviousYearState, ref RadComboBox cmbCurrentResidential,
       ref RadComboBox cmbDayTime, ref RadComboBox cmbEveningNumber, ref RadComboBox cmbEmpStatus)
    {

        try
        {
            DataSet m_DataSetDropdownData = new DataSet();
            m_DataSetDropdownData = obj_CampaignWS.getdropdowndata(ServiceId);
            DataRow[] FilterData;

            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_TITLE'");
            ArrayList Title = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Title.Add(dr["param_field_value"]);
            }
            cmbTitle.DataSource = Title;
            cmbTitle.DataBind();

            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_AUS_CITIZEN'");
            ArrayList Citizen = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Citizen.Add(dr["param_field_value"]);
            }
            cmbAustralian.DataSource = Citizen;
            cmbAustralian.DataBind();

            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_STATES'");
            ArrayList States = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                States.Add(dr["param_field_value"]);
            }
            cmbStates.DataSource = States;
            cmbStates.DataBind();

            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_MARITAL_STATUS'");
            ArrayList Marital = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Marital.Add(dr["param_field_value"]);
            }
            cmbMaritalStatus.DataSource = Marital;
            cmbMaritalStatus.DataBind();

            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_MORTGAGE'");
            ArrayList Mortgage = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Mortgage.Add(dr["param_field_value"]);
            }
            cmbMortgage.DataSource = Mortgage;
            cmbMortgage.DataBind();


            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_PERSONAL_LOAN'");
            ArrayList Personal = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Personal.Add(dr["param_field_value"]);
            }
            cmbPersonalLoan.DataSource = Personal;
            cmbPersonalLoan.DataBind();


            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_CREDIT_CARD'");
            ArrayList Credit = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Credit.Add(dr["param_field_value"]);
            }
            cmbCreditCard.DataSource = Credit;
            cmbCreditCard.DataBind();


            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_INCOME_PROTECTION_INSURANCE'");
            ArrayList Protection = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Protection.Add(dr["param_field_value"]);
            }
            cmbIncomeProtection.DataSource = Protection;
            cmbIncomeProtection.DataBind();




            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_HOME_CONTENTS_INSURANCE'");
            ArrayList Home = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Home.Add(dr["param_field_value"]);
            }
            cmbHomeContents.DataSource = Home;
            cmbHomeContents.DataBind();

            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_INTEREST_TYPE'");
            ArrayList Interest = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Interest.Add(dr["param_field_value"]);
            }
            cmbInterestType.DataSource = Interest;
            cmbInterestType.DataBind();

            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_BUSINESS'");
            ArrayList Business = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Business.Add(dr["param_field_value"]);
            }
            cmbBusiness.DataSource = Business;
            cmbBusiness.DataBind();

            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_SUBURB_STATE'");
            ArrayList Suburb = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Suburb.Add(dr["param_field_value"]);
            }
            cmbSuburbState.DataSource = Suburb;
            cmbSuburbState.DataBind();

            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_PREVIOUS_STATE'");
            ArrayList Previous = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Previous.Add(dr["param_field_value"]);
            }
            cmbPreviousState.DataSource = Previous;
            cmbPreviousState.DataBind();

            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_PREVIOUSYEAR_STATE'");
            ArrayList PreviousYear = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                PreviousYear.Add(dr["param_field_value"]);
            }
            cmbPreviousYearState.DataSource = PreviousYear;
            cmbPreviousYearState.DataBind();

            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_RESIDENTIAL_STATUS'");
            ArrayList Residential = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Residential.Add(dr["param_field_value"]);
            }
            cmbCurrentResidential.DataSource = Residential;
            cmbCurrentResidential.DataBind();

            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_DAY_TIME'");
            ArrayList dayTime = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                dayTime.Add(dr["param_field_value"]);
            }
            cmbDayTime.DataSource = dayTime;
            cmbDayTime.DataBind();

            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_EVE_NUMBER'");
            ArrayList Evening = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Evening.Add(dr["param_field_value"]);
            }
            cmbEveningNumber.DataSource = Evening;
            cmbEveningNumber.DataBind();

            FilterData = m_DataSetDropdownData.Tables["DropdownTable"].Select("param_field_name='VAC_EMPLOYMENT_STATUS'");
            ArrayList Empstatus = new ArrayList();
            foreach (DataRow dr in FilterData)
            {
                Empstatus.Add(dr["param_field_value"]);
            }
            cmbEmpStatus.DataSource = Empstatus;
            cmbEmpStatus.DataBind();

        }
        catch (OdbcException ex)
        {

            LogMessage(ex.Message, 1);
        }
        catch (Exception ex)
        {

            LogMessage(ex.Message, 1);
        }
        finally
        {
        }
    }




    public void GetLeadDetails(int ServiceId, int LeadId)
    {

        try
        {
            DataSet m_DataSet = new DataSet();
            m_DataSet = obj_CampaignWS.GetLeadData(ServiceId, LeadId);
            if (Convert.ToInt32(m_DataSet.Tables["Response"].Rows[0]["ResultCode"]) == -1)
            {
                lblMessage.Text = m_DataSet.Tables["Response"].Rows[0]["ResultCode"].ToString();
                return;
            }
            if (m_DataSet.Tables[0].Rows.Count <= 0)
            {
                lblMessage.Text = "No record found for Lead-" + LeadId;
                return;
            }
            DataTable dt = m_DataSet.Tables["leaddata"];

            cmbTitle.SelectedValue = dt.Rows[0]["Lead_Title"].ToString();
            txtSuffix.Text = dt.Rows[0]["Lead_Suffix"].ToString();
            txtSurname.Text = dt.Rows[0]["udf_Lead_Surname"].ToString();
            txtGivenName.Text = dt.Rows[0]["udf_Lead_Given_Name"].ToString();
            //txtDOB.SelectedDate = Convert.ToDateTime(dt.Rows[0]["udf_Lead_Date_of_Birth"].ToString());
            txtAusiResident.Text = dt.Rows[0]["udf_Lead_Australian_Resident"].ToString();
            cmbAustralian.SelectedValue = dt.Rows[0]["udf_Lead_AustralianCitizen"].ToString();
            txtDrivingLicense.Text = dt.Rows[0]["udf_Lead_Drivers_License"].ToString();
            cmbStates.SelectedValue = dt.Rows[0]["udf_Lead_State_of_Issue"].ToString();
            cmbMaritalStatus.SelectedValue = dt.Rows[0]["udf_Lead_Marital_Status"].ToString();
            txtTFN.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_TFN"]);
            txtAdults.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Adults"]);
            txtdependents.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_No_Of_Dependents"]);
            txtMinors.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Minors"]);
            txtGrossIncome.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_FinancialDetails_Gross_Annual_Income$"]);
            txtHouseIncome.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_FinancialDetails_Household_Income"]);
            cmbMortgage.SelectedValue = dt.Rows[0]["udf_Lead_FinancialDetails_Mortgage"].ToString();
            txtMortgageLender1_Name1.Text = dt.Rows[0]["udf_Lead_FinancialDetails_MortgageName_of_the_Lender1"].ToString();
            txtMonthlyRepaymentLender1.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender1"]);
            txtBalanceOwingLender1.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_FinancialDetails_MortgageBalanceOwingLender1"]);
            txtMortgageLender2_Name1.Text = dt.Rows[0]["udf_Lead_FinancialDetails_MortgageName_of_the_Lender2"].ToString();
            txtMonthlyRepaymentLender2.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender2"]);
            txtBalanceOwingLender2.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_FinancialDetails_MortgageBalanceOwingLender2"]);
            txtMortgageLender3_Name1.Text = dt.Rows[0]["udf_Lead_FinancialDetails_MortgageName_of_the_Lender3"].ToString();
            txtMonthlyRepaymentLender3.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_FinancialDetails_MortgageMonthlyRepaymentLender3"]);
            txtBalanceOwingLender3.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_FinancialDetails_MortgageBalanceOwingLender3"]);
            cmbPersonalLoan.SelectedValue = dt.Rows[0]["udf_Lead_Personal_Loan_Debts"].ToString();
            txtLoanRepaymentLender1_Name1.Text = dt.Rows[0]["udf_Lead_PersonalLoan_Name_of_the_Lender1"].ToString();
            txtLoanMonthlyRepaymentLender1.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_PersonalLoan_MonthlyRepaymentLender1"]);
            txtLoanBalanceOwingLender1.Text = dt.Rows[0]["udf_Lead_PersonalLoan_BalanceOwingLender1"].ToString();
            txtLoanRepaymentLender2_Name1.Text = dt.Rows[0]["udf_Lead_PersonalLoan_Name_of_the_Lender2"].ToString();
            txtLoanMonthlyRepaymentLender2.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_PersonalLoan_MonthlyRepaymentLender2"]);
            txtLoanBalanceOwingLender2.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_PersonalLoan_BalanceOwingLender2"]);
            txtLoanRepaymentLender3_Name1.Text = dt.Rows[0]["udf_Lead_PersonalLoan_Name_of_the_Lender3"].ToString();
            txtLoanMonthlyRepaymentLender3.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_PersonalLoan_MonthlyRepaymentLender3"]);
            txtLoanBalanceOwingLender3.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_PersonalLoan_BalanceOwingLender3"]);
            cmbCreditCard.SelectedValue = dt.Rows[0]["udf_Lead_Credit_Card"].ToString();
            txtCreditRepaymentLender1_Name1.Text = dt.Rows[0]["udf_Lead_Credit_Name_of_the_Lender1"].ToString();
            txtCreditMonthlyRepaymentLender1.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Credit_MonthlyRepaymentLender1"]);
            txtCreditBalanceOwingLender1.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Credit_BalanceOwingLender1"]);
            txtCreditRepaymentLender2_Name1.Text = dt.Rows[0]["udf_Lead_Credit_Name_of_the_Lender2"].ToString();
            txtCreditMonthlyRepaymentLender2.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Credit_MonthlyRepaymentLender2"]);
            txtCreditBalanceOwingLender2.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Credit_BalanceOwingLender2"]);
            txtCreditRepaymentLender3_Name1.Text = dt.Rows[0]["udf_Lead_Credit_Name_of_the_Lender3"].ToString();
            txtCreditMonthlyRepaymentLender3.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Credit_MonthlyRepaymentLender3"]);
            txtCreditBalanceOwing_Name3.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Credit_BalanceOwingLender3"]);
            txtHouseLand1.Text = dt.Rows[0]["udf_Lead_Asset_House1"].ToString();
            txtHouseLandName1.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_House1_CurrentValue"]);
            txtLand1.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_House1_AmountInsured"]);
            txtHouse1.Text = dt.Rows[0]["udf_Lead_Asset_House1_InsuranceCompany"].ToString();
            txtHouseLand2.Text = dt.Rows[0]["udf_Lead_Asset_House2"].ToString();
            txtHouseLandName2.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_House2_CurrentValue"]);
            txtLand2.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_House2_AmountInsured"]);
            txtHouse2.Text = dt.Rows[0]["udf_Lead_Asset_House2_InsuranceCompany"].ToString();
            txtCarOneName1.Text = dt.Rows[0]["udf_Lead_Asset_Car1"].ToString();
            txtCarOneName2.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_Car1_CurrentValue"]);
            txtCarOneName3.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_Car1_AmountInsured"]);
            txtCarOneName4.Text = dt.Rows[0]["udf_Lead_Asset_Car1_InsuranceCompany"].ToString();
            txtCarTwoName1.Text = dt.Rows[0]["udf_Lead_Asset_Car2"].ToString();
            txtCarTwoName2.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_Car2_CurrentValue"]);
            txtCarTwoName3.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_Car2_AmountInsured"]);
            txtCarTwoName4.Text = dt.Rows[0]["udf_Lead_Asset_Car2_InsuranceCompany"].ToString();
            txtInstitution1_Name1.Text = dt.Rows[0]["udf_Lead_Account_Institution1_Name"].ToString();
            txtInstitution1_Name2.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Account_Institution1_Number"]);
            txtInstitution1_Name3.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Account_Institution1_Balance"]);
            txtInstitution2_Name1.Text = dt.Rows[0]["udf_Lead_Account_Institution2_Name"].ToString();
            txtInstitution2_Name2.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Account_Institution2_Number"]);
            txtInstitution2_Name3.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Account_Institution2_Balance"]);
            txtInstitution3_Name1.Text = dt.Rows[0]["udf_Lead_Account_Institution3_Name"].ToString();
            txtInstitution3_Name2.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Account_Institution3_Number"]);
            txtInstitution3_Name3.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Account_Institution3_Balance"]);
            txtJewellery.Text = dt.Rows[0]["udf_Lead_Asset_Jewellery"].ToString();
            txtJewellery1.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_Jewellery_CurrentValue"]);
            txtHomeContent.Text = dt.Rows[0]["udf_Lead_Asset_HomeContents"].ToString();
            txtHomeContent1.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_HomeContents_CurrentValue"]);
            txtLifeInsuranceFaceValue.Text = dt.Rows[0]["udf_Lead_Asset_FaceValue"].ToString();
            txtLifeInsurance.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_FaceValue_CurrentValue"]);
            txtSuperAnnuation.Text = dt.Rows[0]["udf_Lead_Asset_SuperAnnuation"].ToString();
            txtAnnuation.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_SuperAnnuation_CurrentValue"]);
            txtShareInvestment.Text = dt.Rows[0]["udf_Lead_Asset_OtherInvestments"].ToString();
            txtInvestment.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_OtherInvestments_CurrentValue"]);
            txtInsuranceIncomeOne.Text = dt.Rows[0]["udf_Lead_Asset_Income"].ToString();
            cmbIncomeProtection.SelectedValue = dt.Rows[0]["udf_Lead_Asset_IncomeInsurance"].ToString();
            txtIncomeProtection.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_IncomeInsurance_Value"]);
            txtIncomeProtection1.Text = dt.Rows[0]["udf_Lead_Asset_IncomeInsurance_Company"].ToString();
            cmbHomeContents.SelectedValue = dt.Rows[0]["udf_Lead_Asset_HomeInsurance"].ToString();
            txtHomeContents.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_HomeInsurance_Value"]);
            txtHomeContents1.Text = dt.Rows[0]["udf_Lead_Asset_HomeInsurance_Company"].ToString();
            cmbBusiness.SelectedValue = dt.Rows[0]["udf_Lead_Asset_BusinessInsurance"].ToString();
            txtBusiness.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Asset_BusinessInsurance_Value"]);
            txtBusiness1.Text = dt.Rows[0]["udf_Lead_Asset_BusinessInsurance_Company"].ToString();
            txtLocalAmount.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Mortgage_Loan_Amount"]);
            txtRepayment.Value = Convert.ToInt32(dt.Rows[0]["udf_Lead_Mortgage_Repayment_Tenure"]);
            cmbInterestType.SelectedValue = dt.Rows[0]["udf_Lead_Mortgage_Interest_Type"].ToString();
            txtComments.Text = dt.Rows[0]["udf_Lead_Comments"].ToString();
            txtHomeAddressLine1.Text = dt.Rows[0]["udf_Lead_AddressDetails_HomeAddressLine1"].ToString();
            txtHomeAddressLine2_Text1.Text = dt.Rows[0]["udf_Lead_AddressDetails_HomeAddressLine2"].ToString();
            txtHomeAddressLine2_Text2.Text = dt.Rows[0]["udf_Lead_AddressDetails_HomeAddressLine2_Add1"].ToString();
            txtHomeAddressLine2_Text3.Text = dt.Rows[0]["udf_Lead_AddressDetails_HomeAddressLine2_Add2"].ToString();
            txtSuburb.Text = dt.Rows[0]["udf_Lead_AddressDetails_Suburb"].ToString();
            cmbSuburbState.SelectedValue = dt.Rows[0]["udf_Lead_AddressDetails_State"].ToString();
            txtAddressPostCode.Text = dt.Rows[0]["udf_Lead_AddressDetails_Post_Code"].ToString();
            txtYearAtAddress.Text = dt.Rows[0]["udf_Lead_AddressDetails_Years_at_Address"].ToString();
            txtPreviousAddress.Text = dt.Rows[0]["udf_Lead_AddressDetails_PreviousAddress"].ToString();
            cmbPreviousState.SelectedValue = dt.Rows[0]["udf_Lead_AddressDetails_PreviousAddress_State"].ToString();
            txtPreviousAddressPostCode.Text = dt.Rows[0]["udf_Lead_AddressDetails_PreviousAddress_PostCode"].ToString();
            txtPreviousYearAtAddress.Text = dt.Rows[0]["udf_Lead_AddressDetails_PreviousAddress_YearAddress"].ToString();
            cmbPreviousYearState.SelectedValue = dt.Rows[0]["udf_Lead_AddressDetails_Address_State"].ToString();
            txtPreviousYearPost.Text = dt.Rows[0]["udf_Lead_AddressDetails_Address_PostCode"].ToString();
            txtCurrentResidential.Text = dt.Rows[0]["udf_Lead_AddressDetails_CurrentResident"].ToString();
            cmbCurrentResidential.SelectedValue = dt.Rows[0]["udf_Lead_AddressDetails_CurrentResident_Status"].ToString();
            txtPostAddress.Text = dt.Rows[0]["udf_Lead_AddressDetails_CurrentResident_Post"].ToString();
            txtHomePhone.Text = dt.Rows[0]["udf_Lead_ContactDetails_HomePhoneNumber"].ToString();
            txtWorkPhone.Text = dt.Rows[0]["udf_Lead_ContactDetails_WorkPhoneNumber"].ToString();
            txtExtension.Text = dt.Rows[0]["udf_Lead_ContactDetails_Extension"].ToString();
            txtMobileNo.Text = dt.Rows[0]["udf_Lead_ContactDetails_MobilePhoneNumber"].ToString();
            cmbDayTime.SelectedValue = dt.Rows[0]["udf_Lead_ContactDetails_Day_Time_Number"].ToString();
            txtFax.Text = dt.Rows[0]["udf_Lead_ContactDetails_FaxNumber"].ToString();
            cmbEveningNumber.SelectedValue = dt.Rows[0]["udf_Lead_ContactDetails_EveningNumber"].ToString();
            txtEmail.Text = dt.Rows[0]["udf_Lead_ContactDetails_Eail_ID"].ToString();
            cmbEmpStatus.SelectedValue = dt.Rows[0]["udf_Lead_EmploymentDetails_EmploymentStatus"].ToString();
            txtEmpStatus.Text = dt.Rows[0]["udf_Lead_EmploymentDetails_EmploymentStatus1"].ToString();
            txtEmpDetail1.Text = dt.Rows[0]["udf_Lead_EmploymentDetails_Employer_Name_Address_Number1"].ToString();
            txtEmpDetail2.Text = dt.Rows[0]["udf_Lead_EmploymentDetails_Employer_Name_Address_Number2"].ToString();
            txtOccupation.Text = dt.Rows[0]["udf_Lead_EmploymentDetails_Occupation"].ToString();
            txtJobTitle.Text = dt.Rows[0]["udf_Lead_EmploymentDetails_JobTitle"].ToString();
            txtAccFirmName.Text = dt.Rows[0]["udf_Lead_AccountantDetails_AccountingFirmName"].ToString();
            txtAccName.Text = dt.Rows[0]["udf_Lead_AccountantDetails_AccountantsName"].ToString();
            txtAccAddress.Text = dt.Rows[0]["udf_Lead_AccountantDetails_Address"].ToString();
            txtAccPhoneNumber.Text = dt.Rows[0]["udf_Lead_AccountantDetails_PhoneNumber"].ToString();
            string strSold = dt.Rows[0]["udf_Lead_Sold"].ToString();
            if (strSold == "1")
                chkSold.Checked = true;
        }
        catch (OdbcException ex)
        {

            LogMessage(ex.Message, 1);
        }
        catch (Exception ex)
        {

            LogMessage(ex.Message, 1);
        }
        finally
        {
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            string szTitle = cmbTitle.SelectedValue;
            string szSuffix = txtSuffix.Text;
            string szSurname = txtSurname.Text;
            string szGivenName = txtGivenName.Text;
            string szDOB = Convert.ToString(txtDOB.SelectedDate);
            string szAustralianResident = txtAusiResident.Text;
            string szAustralianCitizen = cmbAustralian.SelectedValue;
            string szStateofIssue = cmbStates.SelectedValue;
            string szDriversLicense = txtDrivingLicense.Text;
            string szMaritalStatus = cmbMaritalStatus.SelectedValue;
            int szTFN = Convert.ToInt32(txtTFN.Text);
            int szAdults = Convert.ToInt32(txtAdults.Text);
            int szDependents = Convert.ToInt32(txtdependents.Text);
            int szMinors = Convert.ToInt32(txtMinors.Text);
            int szGrossAnnualIncome = Convert.ToInt32(txtGrossIncome.Text);
            int szHouseholdIncome = Convert.ToInt32(txtHouseIncome.Text);
            string szMortgage = cmbMortgage.SelectedValue;
            string szMortgageLender1 = txtMortgageLender1_Name1.Text;
            int szMortgageMonthlyRepaymentLender1 = Convert.ToInt32(txtMonthlyRepaymentLender1.Text);
            int szMortgageBalanceOwingLender1 = Convert.ToInt32(txtBalanceOwingLender1.Text);
            string szMortgageLender2 = txtMortgageLender2_Name1.Text;
            int szMortgageMonthlyRepaymentLender2 = Convert.ToInt32(txtMonthlyRepaymentLender2.Text);
            int szMortgageBalanceOwingLender2 = Convert.ToInt32(txtBalanceOwingLender2.Text);
            string szMortgageLender3 = txtMortgageLender3_Name1.Text;
            int szMortgageMonthlyRepaymentLender3 = Convert.ToInt32(txtMonthlyRepaymentLender3.Text);
            int szMortgageBalanceOwingLender3 = Convert.ToInt32(txtBalanceOwingLender3.Text);
            string szPersonalLoanDebts = cmbPersonalLoan.SelectedValue;
            string sPersonalLoanLender1 = txtLoanRepaymentLender1_Name1.Text;
            int szPersonalLoanMonthlyRepaymentLender1 = Convert.ToInt32(txtLoanMonthlyRepaymentLender1.Text);
            int szPersonalLoanBalanceOwingLender1 = Convert.ToInt32(txtLoanBalanceOwingLender1.Text);
            string szPersonalLoanLender2 = txtLoanRepaymentLender2_Name1.Text;
            int szPersonalLoanMonthlyRepaymentLender2 = Convert.ToInt32(txtLoanMonthlyRepaymentLender2.Text);
            int szPersonalLoanBalanceOwingLender2 = Convert.ToInt32(txtLoanBalanceOwingLender2.Value);
            string szPersonalLoanLender3 = txtLoanRepaymentLender3_Name1.Text;
            int szPersonalLoanMonthlyRepaymentLender3 = Convert.ToInt32(txtLoanMonthlyRepaymentLender3.Text);
            int szPersonalLoanBalanceOwingLender3 = Convert.ToInt32(txtLoanBalanceOwingLender3.Text);
            string szCreditCard = cmbCreditCard.SelectedValue;
            string szCreditLender1 = txtCreditRepaymentLender1_Name1.Text;
            int szCreditMonthlyRepaymentLender1 = Convert.ToInt32(txtCreditMonthlyRepaymentLender1.Text);
            int szCreditBalanceOwingLender1 = Convert.ToInt32(txtCreditBalanceOwingLender1.Text);
            string szCreditLender2 = txtCreditRepaymentLender2_Name1.Text;
            int szCreditMonthlyRepaymentLender2 = Convert.ToInt32(txtCreditMonthlyRepaymentLender2.Text);
            int szCreditBalanceOwingLender2 = Convert.ToInt32(txtCreditBalanceOwingLender2.Text);
            string szCreditLender3 = txtCreditRepaymentLender3_Name1.Text;
            int szCreditMonthlyRepaymentLender3 = Convert.ToInt32(txtCreditMonthlyRepaymentLender3.Text);
            int szCreditBalanceOwingLender3 = Convert.ToInt32(txtCreditBalanceOwing_Name3.Text);
            string szAssetHouse1 = txtHouseLand1.Text;
            int szAssetHouse1CurrentValue = Convert.ToInt32(txtHouseLandName1.Text);
            int szAssetHouse1AmountInsured = Convert.ToInt32(txtLand1.Text);
            string szAssetHouse1InsuranceCompany = txtHouse1.Text;
            string szAssetHouse2 = txtHouseLand2.Text;
            int szAssetHouse2CurrentValue = Convert.ToInt32(txtHouseLandName2.Text);
            int szAssetHouse2AmountInsured = Convert.ToInt32(txtLand2.Text);
            string szAssetHouse2InsuranceCompany = txtHouse2.Text;
            string szAssetCar1 = txtCarOneName1.Text;
            int szAssetCar1CurrentValue = Convert.ToInt32(txtCarOneName2.Text);
            int szAssetCar1AmountInsured = Convert.ToInt32(txtCarOneName3.Text);
            string szAssetCar1InsuranceCompany = txtCarOneName4.Text;
            string szLeadAssetCar2 = txtCarTwoName1.Text;
            int szAssetCar2CurrentValue = Convert.ToInt32(txtCarTwoName2.Text);
            int szAssetCar2AmountInsured = Convert.ToInt32(txtCarTwoName3.Text);
            string szAssetCar2InsuranceCompany = txtCarTwoName4.Text;
            string szAccountInstitution1_Name = txtInstitution1_Name1.Text;
            int szAccountInstitution1_Number = Convert.ToInt32(txtInstitution1_Name2.Text);
            int szAccountInstitution1Balance = Convert.ToInt32(txtInstitution1_Name3.Text);
            string szAccountInstitution2Name = txtInstitution2_Name1.Text;
            int szAccountInstitution2Number = Convert.ToInt32(txtInstitution2_Name2.Text);
            int szAccountInstitution2Balance = Convert.ToInt32(txtInstitution2_Name3.Text);
            string szAccountInstitution3Name = txtInstitution3_Name1.Text;
            int szAccountInstitution3Number = Convert.ToInt32(txtInstitution3_Name2.Text);
            int szAccountInstitution3Balance = Convert.ToInt32(txtInstitution3_Name3.Text);
            string szJewellery = txtJewellery.Text;
            int szJewelleryCurrentValue = Convert.ToInt32(txtJewellery1.Text);
            string szAssetHomeContents = txtHomeContent.Text;
            int szAssetHomeContentsCurrentValue = Convert.ToInt32(txtHomeContent1.Text);
            string szAssetFaceValue = txtLifeInsuranceFaceValue.Text;
            int szFaceValueCurrentValue = Convert.ToInt32(txtLifeInsurance.Text);
            string szSuperAnnuation = txtSuperAnnuation.Text;
            int szSuperAnnuationCurrentValue = Convert.ToInt32(txtAnnuation.Text);
            string szOtherInvestments = txtShareInvestment.Text;
            int szOtherInvestmentsCurrentValue = Convert.ToInt32(txtInvestment.Text);
            string szAssetIncome = txtInsuranceIncomeOne.Text;
            string szAssetIncomeInsurance = cmbIncomeProtection.SelectedValue;
            int szAssetIncomeInsuranceValue = Convert.ToInt32(txtIncomeProtection.Text);
            string szAssetIncomeInsuranceCompany = txtIncomeProtection1.Text;
            string szAssetHomeInsurance = cmbHomeContents.SelectedValue;
            int szAssetHomeInsuranceValue = Convert.ToInt32(txtHomeContents.Text);
            string szAssetHomeInsuranceCompany = txtHomeContents1.Text;
            string szAssetBusinessInsurance = cmbBusiness.SelectedValue;
            int szAssetBusinessInsuranceValue = Convert.ToInt32(txtBusiness.Text);
            string szAssetBusinessInsuranceCompany = txtBusiness1.Text;
            int szdMortgageLoanAmount = Convert.ToInt32(txtLocalAmount.Text);
            int szMortgageRepaymentTenure = Convert.ToInt32(txtRepayment.Text);
            string szMortgageInterestType = cmbInterestType.SelectedValue;
            string szComments = txtComments.Text;
            string szHomeAddressLine1 = txtHomeAddressLine1.Text;
            string szHomeAddressLine2 = txtHomeAddressLine2_Text1.Text;
            string szHomeAddressLine2Add1 = txtHomeAddressLine2_Text2.Text;
            string szHomeAddressLine2Add2 = txtHomeAddressLine2_Text3.Text;
            string szAddressSuburb = txtSuburb.Text;
            string szAddressState = cmbSuburbState.SelectedValue;
            string szAddressPostCode = txtAddressPostCode.Text;
            string szAddress_YearsatAddress = txtYearAtAddress.Text;
            string szAddress_PreviousAddress = txtPreviousAddress.Text;
            string szPreviousAddress_State = cmbPreviousState.SelectedValue;
            string szPreviousAddress_PostCode = txtPreviousAddressPostCode.Text;
            string szPreviousAddress_YearAddress = txtPreviousYearAtAddress.Text;
            string szAddressDetails_State = cmbPreviousYearState.SelectedValue;
            string szAddress_PostCode = txtPreviousYearPost.Text;
            string szCurrentResident = txtCurrentResidential.Text;
            string szResident_Status = cmbCurrentResidential.SelectedValue;
            string szResident_Post = txtPostAddress.Text;
            string szHomePhoneNumber = txtHomePhone.Text;
            string szWorkPhoneNumber = txtWorkPhone.Text;
            string szExtension = txtExtension.Text;
            string szDayTimeNumber = cmbDayTime.SelectedValue;
            string szEveningNumber = cmbEveningNumber.SelectedValue;
            string szMobilePhoneNumber = txtMobileNo.Text;
            string szFaxNumber = txtFax.Text;
            string szEail_ID = txtEmail.Text;
            string szEmploymentStatus = cmbEmpStatus.SelectedValue;
            string szEmploymentStatus1 = txtEmpStatus.Text;
            string szEmployerAddress_Number1 = txtEmpDetail1.Text;
            string szEmployerAddress_Number2 = txtEmpDetail2.Text;
            string szOccupation = txtOccupation.Text;
            string szJobTitle = txtJobTitle.Text;
            string szAccountingFirmName = txtAccFirmName.Text;
            string szAccountantsName = txtAccName.Text;
            string szAccountantDetails_Address = txtAccAddress.Text;
            string szAccountantDetails_PhoneNumber = txtAccPhoneNumber.Text;
            string strSold = "0";
            if (chkSold.Checked)
                strSold = "1";
            DataSet m_DataSet = new DataSet();
            m_DataSet = obj_CampaignWS.UpdateLeaddata(ServiceId, LeadId, szTitle, szSuffix, szSurname, szGivenName, szDOB,
                szAustralianResident, szAustralianCitizen, szStateofIssue, szDriversLicense, szMaritalStatus, szTFN,
                szAdults, szDependents, szMinors, szGrossAnnualIncome, szHouseholdIncome, szMortgage,
                szMortgageLender1, szMortgageMonthlyRepaymentLender1, szMortgageBalanceOwingLender1,
                szMortgageLender2, szMortgageMonthlyRepaymentLender2, szMortgageBalanceOwingLender2,
                szMortgageLender3, szMortgageMonthlyRepaymentLender3, szMortgageBalanceOwingLender3,
                szPersonalLoanDebts, sPersonalLoanLender1, szPersonalLoanMonthlyRepaymentLender1,
                szPersonalLoanBalanceOwingLender1, szPersonalLoanLender2, szPersonalLoanMonthlyRepaymentLender2,
                szPersonalLoanBalanceOwingLender2, szPersonalLoanLender3, szPersonalLoanMonthlyRepaymentLender3,
                szPersonalLoanBalanceOwingLender3, szCreditCard, szCreditLender1, szCreditMonthlyRepaymentLender1,
                szCreditBalanceOwingLender1, szCreditLender2, szCreditMonthlyRepaymentLender2,
                szCreditBalanceOwingLender2, szCreditLender3, szCreditMonthlyRepaymentLender3,
                szCreditBalanceOwingLender3, szAssetHouse1, szAssetHouse1CurrentValue, szAssetHouse1AmountInsured,
                szAssetHouse1InsuranceCompany, szAssetHouse2, szAssetHouse2CurrentValue, szAssetHouse2AmountInsured,
                szAssetHouse2InsuranceCompany, szAssetCar1, szAssetCar1CurrentValue, szAssetCar1AmountInsured,
                szAssetCar1InsuranceCompany, szLeadAssetCar2, szAssetCar2CurrentValue, szAssetCar2AmountInsured,
                szAssetCar2InsuranceCompany, szAccountInstitution1_Name, szAccountInstitution1_Number,
                szAccountInstitution1Balance, szAccountInstitution2Name, szAccountInstitution2Number,
                szAccountInstitution2Balance, szAccountInstitution3Name, szAccountInstitution3Number,
                szAccountInstitution3Balance, szJewellery, szJewelleryCurrentValue, szAssetHomeContents,
                szAssetHomeContentsCurrentValue, szAssetFaceValue, szFaceValueCurrentValue, szSuperAnnuation,
                szSuperAnnuationCurrentValue, szOtherInvestments, szOtherInvestmentsCurrentValue, szAssetIncome,
                szAssetIncomeInsurance, szAssetIncomeInsuranceValue, szAssetIncomeInsuranceCompany,
                szAssetHomeInsurance, szAssetHomeInsuranceValue, szAssetHomeInsuranceCompany,
                szAssetBusinessInsurance, szAssetBusinessInsuranceValue, szAssetBusinessInsuranceCompany,
                szdMortgageLoanAmount, szMortgageRepaymentTenure, szMortgageInterestType, szComments,
                szHomeAddressLine1, szHomeAddressLine2, szHomeAddressLine2Add1, szHomeAddressLine2Add2,
                szAddressSuburb, szAddressState, szAddressPostCode, szAddress_YearsatAddress, szAddress_PreviousAddress,
                szPreviousAddress_State, szPreviousAddress_PostCode, szPreviousAddress_YearAddress,
                szAddressDetails_State, szAddress_PostCode, szCurrentResident, szResident_Status, szResident_Post,
                szHomePhoneNumber, szWorkPhoneNumber, szExtension, szDayTimeNumber, szEveningNumber,
                szMobilePhoneNumber, szFaxNumber, szEail_ID, szEmploymentStatus, szEmploymentStatus1,
                szEmployerAddress_Number1, szEmployerAddress_Number2, szOccupation, szJobTitle, szAccountingFirmName,
                szAccountantsName, szAccountantDetails_Address, szAccountantDetails_PhoneNumber, strSold);

            
            if (Convert.ToInt64(m_DataSet.Tables["Response"].Rows[0]["ResultCode"]) ==- 1)
            {
                lblMessage.Text = m_DataSet.Tables["Response"].Rows[0]["ResultCode"].ToString();
            }
            else
            {
                string message = "Data Updated successfully";
                if (strSold == "1")
                {
                    DataBase m_Connection = new DataBase();
                    m_Connection.OpenDB("Galaxy");
                    XmlDocument xMainDoc = m_Connection.createParameterXML();
                    
                    m_Connection.fillParameterXML(ref xMainDoc, "owner_id", TB_nContactID.ToString(), "int", "0");
                    m_Connection.fillParameterXML(ref xMainDoc, "owner_name", TB_strEmployeeName, "varchar", "100");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_name", txtGivenName.Text, "varchar", "100");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_name2", txtSurname.Text, "varchar", "100");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_short_name", txtCustShortName.Text, "varchar", "100");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_type", "C", "varchar", "100");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_zone", cmbZone.SelectedValue, "varchar", "100");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_address1", txtHomeAddressLine1.Text, "varchar", "100");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_address2", txtHomeAddressLine2_Text1.Text, "varchar", "100");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_address3", txtHomeAddressLine2_Text2.Text, "varchar", "100");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_city_code", cmbCity.SelectedValue, "varchar", "100");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_state_code", cmbStates.SelectedValue, "varchar", "100");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_zip_code", txtAddressPostCode.Text, "varchar", "100");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_tin_no", txtTinNo.Text, "varchar", "100");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_pan_no", txtPanNo.Text, "varchar", "100");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_category", cmbCategory.SelectedValue, "varchar", "100");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_rating", cmbRating.SelectedValue, "varchar", "100");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_phone", txtHomePhone.Text, "varchar", "100");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_tax_scheme_id", cmbTaxScheme.SelectedValue, "int", "0");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_emailid", txtEmail.Text, "varchar", "100");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_website_address", txtWebsite.Text, "varchar", "100");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_country_code", cmbCountry.SelectedValue, "varchar", "100");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_tan_no", txtTanNo.Text, "varchar", "100");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_service_tax_no", txtTFN.Text, "varchar", "100");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_enabled", "Y", "varchar", "2");


                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_ref_no", txtRefNo.Text, "varchar", "200");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_title", cmbTitle.SelectedValue, "varchar", "20");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_suffix", txtSuffix.Text, "varchar", "20");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_first_name", txtGivenName.Text, "varchar", "100");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_last_name", txtSurname.Text, "varchar", "100");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_address_type", cmbAddressType.SelectedValue, "varchar", "50");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_abn", txtABN.Text, "varchar", "50");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_gender", cmbSex.SelectedValue, "varchar", "50");
                    //if (dtDateofBirth.SelectedDate.HasValue)
                    //    m_Connection.fillParameterXML(ref xMainDoc, "cust_dob", dtDateofBirth.SelectedDate.Value.ToString("dd MMM yyyy"), "datetime", "0");
                    m_Connection.fillParameterXML(ref xMainDoc, "cust_address_suburb", txtSuburb.Text, "varchar", "100");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_desc", txtDescription.Text, "varchar", "500");
                    //m_Connection.fillParameterXML(ref xMainDoc, "cust_reffered_by", txtReferredby.Text, "varchar", "100");

                    lblMessage.Text = "";
                    DataTable dt1 = null;
                    m_Connection.BeginTransaction();

                    dt1 = m_Connection.SaveTransactionData("CST", 0, "Y", DateTime.UtcNow, TB_nContactID, Request.UserHostAddress, xMainDoc);

                    if (Convert.ToInt32(dt1.Rows[0]["ResultCode"]) >= 0)
                    {                       
                        dt1 = m_Connection.SaveRecentActivity(0, "CST", Convert.ToInt32(dt1.Rows[0]["ResultCode"]), "", txtGivenName.Text, TB_nContactID, "P", 2401, "");
                        m_Connection.Commit();
                        message = "New Account has been created with data updation successfully!";
                    }
                    else
                    {
                        m_Connection.Rollback();
                    }                  
                }
                
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", "javascript:alert('" + message + "')", true);
                lblMessage.Text = message;
            }
        }
        catch (OdbcException ex)
        {
           
            LogMessage(ex.Message, 1);
        }
        catch (Exception ex)
        {
           
            LogMessage(ex.Message, 1);
        }
        finally
        {
        }

    }
    #region Log Message
    void LogMessage(string Message, Int32 param)
    {
        lblMessage.Text = Message;
        if (param == 1)
            lblMessage.CssClass = "error";
        else
            lblMessage.CssClass = "success";
    }
    #endregion
}
