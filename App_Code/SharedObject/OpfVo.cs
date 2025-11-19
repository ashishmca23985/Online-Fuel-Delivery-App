using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for OpfVo
/// </summary>
public class OpfVo:BaseVo
{
	public OpfVo()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public int Id { get; set; }
    public int MyProperty { get; set; }
    public DateTime opfDate { get; set; }
    public string opfBillingType { get; set; }
    public string opfType { get; set; }
    public string subject { get; set; }
    public int currentLevel { get; set; }
    public string status { get; set; }
    public string statusReason { get; set; }
    public string region { get; set; }
    public int  salePersonId { get; set; }
    public string salePersonName { get; set; }
    public string sysnopsis { get; set; }
    
    public int billingCustId { get; set; }
    public string billingCustName { get; set; }
    public string billingCustShortName { get; set; }
    public string billingCustAddress { get; set; }
    public string billingCstNo { get; set; }
    public string billingLstNo { get; set; }
    public string billingPanNo { get; set; }
    
    public int installationCustId { get; set; }
    public string installationCustName { get; set; }
    public string installationCustShortName { get; set; }
    public string installationAddress { get; set; }

    public string shippingCustName { get; set; }
    public string shippingCustShortName { get; set; }
    public string shippingAddress { get; set; }

    public int custOrderContactId { get; set; }
    public string custOrderContactName { get; set; }
    public string custOrderContactPhoneNo { get; set; }
    public string custOrderContactEmailId { get; set; }

    public int custInstallationContactId { get; set; }
    public string custInstallationContactName { get; set; }
    public string custInstallationContactPhoneNo { get; set; }
    public string custInstallationContactEmailId { get; set; }

    public int custFinanceContactId { get; set; }
    public string custFinanceContactName { get; set; }
    public string custFinanceContactPhoneNo { get; set; }
    public string custFinanceContactEmailId { get; set; }

    public string poSource { get; set; }
    public string poNumber { get; set; }
    public DateTime poDate { get; set; }

    public string orcName { get; set; }
    public string orcPanNo { get; set; }
    public string orcBankName { get; set; }
    public string orcBankDetails { get; set; }
    public string orcBankAccountNo { get; set; }
    public decimal orcAmount { get; set; }

    public decimal advanceAmount { get; set; }
    public string AdvanceMode { get; set; }
    public string billingFrequency { get; set; }
    public DateTime installationExpectedDate { get; set; }
    public DateTime installationActual { get; set; }
    public decimal totalBasicAmount { get; set; }
    public decimal totalTaxAmount { get; set; }
    public decimal totalDicountAmount { get; set; }
    public decimal totalwriteoffAmount { get; set; }
    public decimal totalAmount { get; set; }
    public decimal totalBilledAmount { get; set; }
    public decimal totalCollectedAmount { get; set; }

    public int saleHeadConfirmationId { get; set; }
    public string saleHeadConfirmationName { get; set; }
    public DateTime saleHeadConfirmationDate { get; set; }
    public string saleHeadConfirmationRemarks { get; set; }
    public string currentRemarks { get; set; }
    List<OPFItems> listOpfDetails { get; set; }
}

public class OPFItems:BaseVo
{
    public int Id { get; set; }
    public int opfId { get; set; }
    public string opfNumber { get; set; }
    public short sNo { get; set; }
    public string partCode { get; set; }
    public string partName { get; set; }
    public string billingDesc { get; set; }
    public int quantity { get; set; }
    public decimal basicRate { get; set; }
    public decimal taxRate1 { get; set; }
    public string taxName1 { get; set; }
    public decimal taxAmount1 { get; set; }
    public decimal taxRate2 { get; set; }
    public string taxName2 { get; set; }
    public decimal taxAmount2 { get; set; }
    public decimal taxRate3 { get; set; }
    public string taxName3 { get; set; }
    public decimal taxAmount3 { get; set; }
    public decimal discountAmount { get; set; }
    public string inventoryStatus { get; set; }
    public string inventorystatusRemarks { get; set; }
    public short billedQty { get; set; }

}
public class ItemDetails
{
    public int Id { get; set; }
    public string opfNumber { get; set; }
    public int opfItemId { get; set; }
    public string partCode { get; set; }
    public short partCodeSNo { get; set; }
    public string sNo { get; set; }
    public string warrantyApplicable { get; set; }
    public DateTime warrantyStartDate { get; set; }
    public DateTime warrantyEndDate { get; set; }
    public string amcApplicable { get; set; }
    public DateTime amcStartDate { get; set; }
    public DateTime amcEndDate { get; set; }
    public string BillNo { get; set; }
    public DateTime deliveryDate { get; set; }
    public string deliveryRemarks { get; set; }
}

 public class ProductMaster:BaseVo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string customAttribute { get; set; }
    public string billingType { get; set; }
    public string category { get; set; }
    public string subCategory { get; set; }
    public string manufacturer { get; set; }
    public string model { get; set; }
    public string warrantyApplicable { get; set; }
    public DateTime warrantStartDate { get; set; }
    public short warrantyDuration { get; set; }
    public string amcApplicable { get; set; }
    public string localTaxName1 { get; set; }
    public string localTaxName2 { get; set; }
    public string localTaxName3 { get; set; }
    public string cstTaxName1 { get; set; }
    public string cstTaxName2 { get; set; }
    public string cstTaxName3 { get; set; }
    public decimal minBasicRate { get; set; }
    public decimal maxBasicRate { get; set; }
}

 public class OpfTax
 {
     public string taxcode { get; set; }
     public decimal rate { get; set; }
 }

 public class WorkFlowRule
 {
     public int Id { get; set; }
     public string relatedTo { get; set; }
     public int currentLevel { get; set; }
     public string currentLevelName { get; set; }
     public int approveLevel { get; set; }
     public string approveLavelName { get; set; }
     public string approveUrl { get; set; }
     public int rejectLevel { get; set; }
     public string rejectLavelName { get; set; }
     public string rejectUrl { get; set; }
     public int cancleLevel { get; set; }
     public string cancleLavelName { get; set; }
     public string cancleUrl { get; set; }
 }

 public class WorkFlowHistory
 {
     public int Id { get; set; }
     public string relatedTo { get; set; }
     public int relatedToId { get; set; }
     public DateTime date { get; set; }
     public int userId { get; set; }
     public string userName { get; set; }
     public string IPaddress { get; set; }
     public string remarks { get; set; }
     public int currentLevel { get; set; }
     public string CurrentLavelName { get; set; }
     public int nextLevel { get; set; }
     public string nextLavelName { get; set; }
 }