using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharedVO;

/// <summary>
/// Summary description for OpfBAL
/// </summary>
public class OpfBAL
{
    OpfDAL opfDal=new OpfDAL();
	public OpfBAL()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public List<GeneralField> OpfGeneral(string type, ref ErrorVO error)
    {
        return opfDal.OpfGeneral(type, ref error);
    }
    public OpfVo OpfDetails(int opfId, ref ErrorVO error)
    {
        return opfDal.OpfDetails(opfId, ref  error);
    }
    public int createItemDetails(int opfid, ref ErrorVO error)
    {
        return opfDal.createItemDetails(opfid,ref error);
    }

    
    public object ProductItemList(int id, ref ErrorVO error)
    {
        object itemslist = new object();
        List<ItemDetails> ob = opfDal.ProductItemList(id, ref error);
        try
        {
             itemslist = ob.Select(t => new
            {
                Id = t.Id,
                opfItemId = t.opfItemId,
                opfNumber = t.opfNumber,
                sNo = t.sNo,
                partCode = t.partCode,
                partCodeSNo = t.partCodeSNo,
                warrantyApplicable = t.warrantyApplicable,
                warrantyEndDate = t.warrantyEndDate,
                warrantyStartDate = t.warrantyStartDate,
                amcApplicable = t.amcApplicable,
                amcEndDate = t.amcEndDate,
                amcStartDate = t.amcStartDate,
                BillNo = t.BillNo,
                deliveryDate = t.deliveryDate,
                deliveryRemarks = t.deliveryRemarks,

            }).ToList();
           
        }
        catch (Exception ex)
        {
            error.errorResult = ex.Message;
            error.errorCode = -1;
          
        }
        return itemslist;
     
    }
    public object ProductSave(List<ItemDetails> items, ref ErrorVO error)
    {
        object itemslist = new object();
        List<ItemDetails> ob = opfDal.ProductSave(items, ref error);
        try
        {
            itemslist = ob.Select(t => new
            {
                Id = t.Id,
                opfItemId = t.opfItemId,
                opfNumber = t.opfNumber,
                sNo = t.sNo,
                partCode = t.partCode,
                partCodeSNo = t.partCodeSNo,
                warrantyApplicable = t.warrantyApplicable,
                warrantyEndDate = t.warrantyEndDate,
                warrantyStartDate = t.warrantyStartDate,
                amcApplicable = t.amcApplicable,
                amcEndDate = t.amcEndDate,
                amcStartDate = t.amcStartDate,
                BillNo = t.BillNo,
                deliveryDate = t.deliveryDate,
                deliveryRemarks = t.deliveryRemarks,

            }).ToList();

        }
        catch (Exception ex)
        {
            error.errorResult = ex.Message;
            error.errorCode = -1;

        }
        return itemslist;

    }
    public List<ProductMaster> ProductList( ref ErrorVO error)
    {
      //  object itemslist = new object();
        List<ProductMaster> ob = opfDal.ProductList(ref error);
        //try
        //{
        //    itemslist = ob.Select(t => new
        //    {
        //        Id = t.Id,
        //        Name = t.Name,
        //        transactionNumber = t.transactionNumber,
        //        useable = t.useable,
        //        customAttribute = t.customAttribute,
        //        billingType = t.billingType,
        //        category = t.category,
        //        subCategory = t.subCategory,
        //        manufacturer = t.manufacturer,
        //        model = t.model,
        //        warrantyApplicable = t.warrantyApplicable,
        //        warrantStartDate = t.warrantStartDate,
        //        warrantyDuration = t.warrantyDuration,
        //        amcApplicable = t.amcApplicable,
        //        localTaxName1 = t.localTaxName1,
        //        localTaxName2 = t.localTaxName2,
        //        localTaxName3 = t.localTaxName3,
        //        cstTaxName1 = t.cstTaxName1,
        //        cstTaxName2 = t.cstTaxName2,
        //        cstTaxName3 = t.cstTaxName3,
        //        minBasicRate = t.minBasicRate,
        //        maxBasicRate = t.maxBasicRate,
        //    }).ToList();
            //object category = ob.Select(t => new { category=t.category }).Distinct().ToList();
            //object subcategory = ob.Select(t => new { subCategory=t.subCategory }).Distinct().ToList();
            //object subcategory = ob.Select(t => new { subCategory = t.manufacturer }).Distinct().ToList();
        //}
        //catch (Exception ex)
        //{
        //    error.errorResult = ex.Message;
        //    error.errorCode = -1;

        //}
        return ob;

    }

    public object TaxList(ref ErrorVO error)
    {
        object itemslist = new object();
        List<ProductMaster> ob = opfDal.ProductList(ref error);
        try
        {
            itemslist = ob.Select(t => new
            {
                Id = t.Id,
                Name = t.Name,
                transactionNumber = t.transactionNumber,
                useable = t.useable,
                customAttribute = t.customAttribute,
                billingType = t.billingType,
                category = t.category,
                subCategory = t.subCategory,
                manufacturer = t.manufacturer,
                model = t.model,
                warrantyApplicable = t.warrantyApplicable,
                warrantStartDate = t.warrantStartDate,
                warrantyDuration = t.warrantyDuration,
                amcApplicable = t.amcApplicable,
                localTaxName1 = t.localTaxName1,
                localTaxName2 = t.localTaxName2,
                localTaxName3 = t.localTaxName3,
                cstTaxName1 = t.cstTaxName1,
                cstTaxName2 = t.cstTaxName2,
                cstTaxName3 = t.cstTaxName3,
                minBasicRate = t.minBasicRate,
                maxBasicRate = t.maxBasicRate,
            }).ToList();

        }
        catch (Exception ex)
        {
            error.errorResult = ex.Message;
            error.errorCode = -1;

        }
        return itemslist;

    }
}