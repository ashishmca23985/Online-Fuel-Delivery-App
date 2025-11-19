using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using SharedVO;

/// <summary>
/// Summary description for OpfDAL
/// </summary>
public class OpfDAL
{
    DataBase m_Connection = new DataBase();
    OdbcCommand m_CommandODBC;
    OdbcDataAdapter m_DataAdapterODBC;
    public OpfDAL()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public List<GeneralField> OpfGeneral(string type, ref ErrorVO error)
    {

        DataSet m_DataSet = new DataSet();
        List<GeneralField> ob = new List<GeneralField>();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC opf_general_field ? ", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@szType", OdbcType.VarChar).Value = type;
            OdbcDataReader dr = m_CommandODBC.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    ob.Add(new GeneralField()
                    {
                        name = dr["name"].ToString(),
                        value = dr["value"].ToString()
                    });
                }
            }
        }
        catch (OdbcException ex)
        {
            error.errorCode = -2;
            error.errorResult = ex.Message;
        }
        catch (Exception ex)
        {
            error.errorCode = -1;
            error.errorResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
        }
        return ob;
    }

    public OpfVo OpfDetails(int opfId, ref ErrorVO error)
    {

        OpfVo ob = new OpfVo();

        try
        {
            m_Connection.OpenDB("Galaxy");
            string strsql = "select * from opf_master where id=" + opfId + " and useable='Y'";

            m_CommandODBC = new OdbcCommand(strsql, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            OdbcDataReader dr = m_CommandODBC.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    ob.transactionNumber = dr["transaction_number"].ToString();
                    ob.opfDate = Convert.ToDateTime(dr["opf_date"] ?? DateTime.MinValue);
                    ob.opfType = dr["opf_type"].ToString();
                    ob.opfBillingType = dr["opf_billing_type"].ToString();
                    ob.subject = dr["subject"].ToString();
                    ob.status = dr["status"].ToString();
                    ob.currentLevel = Convert.ToInt32(dr["current_level"] ?? 0);
                    ob.statusReason = dr["status_reason"].ToString();
                    ob.region = dr["opf_region"].ToString();
                    ob.salePersonId = Convert.ToInt32(dr["opf_sales_person_id"] ?? 0);
                    ob.salePersonName = dr["opf_sales_person_name"].ToString();
                    ob.sysnopsis = dr["opf_synopsis"].ToString();
                    ob.billingCustId = Convert.ToInt32(dr["opf_custid_billing"] ?? 0);
                    ob.billingCustName = dr["opf_billing_cust_name"].ToString();
                    ob.billingCustShortName = dr["opf_billing_cust_short_name"].ToString();
                    ob.billingCustAddress = dr["opf_billing_cust_address"].ToString();
                    ob.billingCstNo = dr["opf_billing_cst_no"].ToString();
                    ob.billingLstNo = dr["opf_billing_lst_no"].ToString();
                    ob.billingPanNo = dr["opf_billing_pan_no"].ToString();
                    ob.shippingCustName = dr["opf_shipping_cust_name"].ToString();
                    ob.shippingCustShortName = dr["opf_shipping_cust_short_name"].ToString();
                    ob.shippingAddress = dr["opf_shipping_cust_address"].ToString();
                    ob.installationCustId = Convert.ToInt32(dr["opf_custid_installation"] ?? 0);
                    ob.installationCustName = dr["opf_installation_cust_name"].ToString();
                    ob.installationCustShortName = dr["opf_installation_cust_short_name"].ToString();
                    ob.installationAddress = dr["opf_installation_cust_address"].ToString();
                    ob.custOrderContactId = Convert.ToInt32(dr["opf_cust_order_contact_id"] ?? 0);
                    ob.custOrderContactName = dr["opf_cust_order_contact_name"].ToString();
                    ob.custOrderContactPhoneNo = dr["opf_cust_order_phone_numbers"].ToString();
                    ob.custOrderContactEmailId = dr["opf_cust_order_email_ids"].ToString();
                    ob.custInstallationContactId = Convert.ToInt32(dr["opf_cust_installation_contact_id"] ?? 0);
                    ob.custInstallationContactName = dr["opf_cust_installation_contact_name"].ToString();
                    ob.custInstallationContactPhoneNo = dr["opf_cust_installation_phone_numbers"].ToString();
                    ob.custOrderContactEmailId = dr["opf_cust_installation_email_ids"].ToString();
                    ob.custFinanceContactId = Convert.ToInt32(dr["opf_cust_finance_contact_id"] ?? 0);
                    ob.custFinanceContactName = dr["opf_cust_finance_contact_name"].ToString();
                    ob.custFinanceContactPhoneNo = dr["opf_cust_finance_phone_numbers"].ToString();
                    ob.custInstallationContactEmailId = dr["opf_cust_finance_email_ids"].ToString();
                    ob.poSource = dr["opf_po_source"].ToString();
                    ob.poNumber = dr["opf_po_number"].ToString();
                    ob.poDate = Convert.ToDateTime(dr["opf_po_date"] ?? DateTime.MinValue);
                    ob.orcName = dr["opf_orc_name"].ToString();
                    ob.orcPanNo = dr["opf_orc_pan_no"].ToString();
                    ob.orcBankName = dr["opf_orc_bank_name"].ToString();
                    ob.orcBankDetails = dr["opf_orc_bank_details"].ToString();
                    ob.orcBankAccountNo = dr["opf_orc_bank_account_no"].ToString();
                    ob.orcAmount = Convert.ToDecimal(dr["opf_orc_amount"] ?? 0);
                    ob.advanceAmount = Convert.ToDecimal(dr["opf_advance_amount"] ?? 0);
                    ob.AdvanceMode = dr["opf_advance_mode"].ToString();
                    ob.billingFrequency = dr["opf_billing_frequency"].ToString();
                    ob.installationExpectedDate = Convert.ToDateTime(dr["opf_installation_date_expected"] ?? DateTime.MinValue);
                    ob.installationActual = Convert.ToDateTime(dr["opf_installation_date_actual"] ?? DateTime.MinValue);
                    ob.totalBasicAmount = Convert.ToDecimal(dr["opf_total_basic_amount"] ?? 0);
                    ob.totalTaxAmount = Convert.ToDecimal(dr["opf_total_tax_amount"] ?? 0);
                    ob.totalDicountAmount = Convert.ToDecimal(dr["opf_total_discount_amount"] ?? 0);
                    ob.totalwriteoffAmount = Convert.ToDecimal(dr["opf_total_writeoff_amount"] ?? 0);
                    ob.totalAmount = Convert.ToDecimal(dr["opf_total_amount"] ?? 0);
                    ob.totalBilledAmount = Convert.ToDecimal(dr["opf_total_billed_amount"] ?? 0);
                    ob.totalCollectedAmount = Convert.ToDecimal(dr["opf_total_collected_amount"] ?? 0);
                    ob.saleHeadConfirmationId = Convert.ToInt32(dr["opf_sales_head_confirmation_id"] ?? 0);
                    ob.saleHeadConfirmationName = dr["opf_sales_head_confirmation_name"].ToString();
                    ob.saleHeadConfirmationDate = Convert.ToDateTime(dr["opf_sales_head_confirmation_date"] ?? DateTime.MinValue);
                    ob.saleHeadConfirmationRemarks = dr["opf_sales_head_confirmation_remarks"].ToString();
                    ob.lastModifiedDate = Convert.ToDateTime(dr["last_activity_date"] ?? DateTime.MinValue);
                    ob.useable = dr["useable"].ToString();
                    ob.createdBy = Convert.ToInt32(dr["created_by"] ?? 0);
                    ob.createdDate = Convert.ToDateTime(dr["created_date"] ?? DateTime.MinValue);
                    ob.modifiedBy = Convert.ToInt32(dr["modified_by"] ?? 0);
                    ob.modifiedDate = Convert.ToDateTime(dr["modified_date"] ?? DateTime.MinValue);
                }
            }
        }
        catch (Exception ex)
        {
            error.errorCode = -1;
            error.errorResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
        }
        return ob;
    }

    /// <summary>
    /// For insert OPF Items details
    /// </summary>
    /// <param name="opfid"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public int createItemDetails(int opfid, ref ErrorVO error)
    {
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC Opf_Item_details_insert ? ", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@nOpfId", OdbcType.Int).Value = opfid;
            error.errorCode = m_CommandODBC.ExecuteNonQuery();
        }
        catch (OdbcException ex)
        {
            error.errorCode = -1;
            error.errorResult = ex.Message;
        }
        catch (Exception ex)
        {
            error.errorCode = -2;
            error.errorResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
        }
        return error.errorCode;
    }



    public List<ItemDetails> ProductItemList(int id, ref ErrorVO error)
    {
        List<ItemDetails> itemDetails = new List<ItemDetails>();
        try
        {
            m_Connection.OpenDB("Galaxy");
            string strsql = "select * from opf_item_details where itemdet_opfdet_id=" + id;
            m_CommandODBC = new OdbcCommand(strsql, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            OdbcDataReader dr = m_CommandODBC.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    ItemDetails ob = new ItemDetails();
                    ob.Id = Convert.ToInt32(dr["id"] ?? 0);
                    ob.opfNumber = dr["itemdet_opf_number"].ToString();
                    ob.opfItemId = dr["itemdet_opfdet_id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["itemdet_opfdet_id"]);
                    ob.partCode = dr["itemdet_partcode"].ToString();
                    ob.partCodeSNo = Convert.ToInt16(dr["itemdet_partcode_sno"] == DBNull.Value ? 0 : dr["itemdet_partcode_sno"]);
                    ob.sNo = dr["itemdet_serial_no"].ToString();
                    ob.warrantyApplicable = dr["itemdet_warranty_applicable"].ToString();
                    ob.warrantyEndDate = Convert.ToDateTime(dr["itemdet_warranty_start_date"] == DBNull.Value ? DateTime.MinValue : dr["itemdet_warranty_start_date"]);
                    ob.warrantyStartDate = Convert.ToDateTime(dr["itemdet_warranty_end_date"] == DBNull.Value ? DateTime.MinValue : dr["itemdet_warranty_end_date"]);
                    ob.amcApplicable = dr["itemdet_amc_applicable"].ToString();
                    ob.amcEndDate = Convert.ToDateTime(dr["itemdet_amc_start_date"] == DBNull.Value ? DateTime.MinValue : dr["itemdet_amc_start_date"]);
                    ob.amcStartDate = Convert.ToDateTime(dr["itemdet_amc_end_date"] == DBNull.Value ? DateTime.MinValue : dr["itemdet_amc_end_date"]);
                    ob.BillNo = dr["itemdet_bill_no"].ToString();
                    ob.deliveryDate = Convert.ToDateTime(dr["itemdet_delivery_date"] == DBNull.Value ? DateTime.MinValue : dr["itemdet_delivery_date"]);
                    ob.deliveryRemarks = dr["itemdet_delivery_remarks"].ToString();
                    itemDetails.Add(ob);
                }
            }
        }
        catch (Exception ex)
        {
            error.errorCode = -1;
            error.errorResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
        }
        return itemDetails;
    }

    public List<ItemDetails> ProductSave(List<ItemDetails> items, ref ErrorVO error)
    {

        try
        {
            m_Connection.OpenDB("Galaxy");
            foreach (var item in items)
            {
                m_CommandODBC = new OdbcCommand("exec opf_item_details_update ?,?,?,?,?,?,?,?,?,?,?,?", m_Connection.oCon);
                m_CommandODBC.CommandType = CommandType.StoredProcedure;
                m_CommandODBC.Parameters.Add("@nId", OdbcType.Int).Value = item.Id;
                m_CommandODBC.Parameters.Add("@partcode_sno", OdbcType.SmallInt).Value = item.partCodeSNo;
                m_CommandODBC.Parameters.Add("@serial_no", OdbcType.VarChar).Value = item.sNo;
                m_CommandODBC.Parameters.Add("@warranty_applicable", OdbcType.VarChar).Value = item.warrantyApplicable;
                m_CommandODBC.Parameters.Add("@warranty_start_date", OdbcType.DateTime).Value = (item.warrantyStartDate == DateTime.MinValue ? Convert.ToDateTime("1900-01-01") : item.warrantyStartDate); ;
                m_CommandODBC.Parameters.Add("@warranty_end_date", OdbcType.DateTime).Value = (item.warrantyEndDate == DateTime.MinValue ? Convert.ToDateTime("1900-01-01") : item.warrantyEndDate); ;
                m_CommandODBC.Parameters.Add("@amc_applicable", OdbcType.VarChar).Value = item.amcApplicable;
                m_CommandODBC.Parameters.Add("@amc_start_date", OdbcType.DateTime).Value = (item.amcStartDate == DateTime.MinValue ? Convert.ToDateTime("1900-01-01") : item.amcStartDate); ;
                m_CommandODBC.Parameters.Add("@amc_end_date", OdbcType.DateTime).Value = (item.amcEndDate == DateTime.MinValue ? Convert.ToDateTime("1900-01-01") : item.amcEndDate);
                m_CommandODBC.Parameters.Add("@bill_no", OdbcType.VarChar).Value = item.BillNo;
                m_CommandODBC.Parameters.Add("@delivery_date", OdbcType.DateTime).Value = (item.deliveryDate == DateTime.MinValue ? Convert.ToDateTime("1900-01-01") : item.deliveryDate);
                m_CommandODBC.Parameters.Add("@delivery_remarks", OdbcType.VarChar).Value = item.deliveryRemarks;
                m_CommandODBC.ExecuteNonQuery();
            }

            //string strsql = "select * from opf_item_details where itemdet_opfdet_id=" + id;
            //m_CommandODBC = new OdbcCommand(strsql, m_Connection.oCon);
            //m_CommandODBC.CommandType = CommandType.StoredProcedure;
            //OdbcDataReader dr = m_CommandODBC.ExecuteReader();
            //if (dr.HasRows)
            //{
            //    while (dr.Read())
            //    {
            //        ItemDetails ob = new ItemDetails();
            //        ob.Id = Convert.ToInt32(dr["id"] ?? 0);
            //        ob.opfNumber = dr["itemdet_opf_number"].ToString();
            //        ob.opfItemId = dr["itemdet_opfdet_id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["itemdet_opfdet_id"]);
            //        ob.partCode = dr["itemdet_partcode"].ToString();
            //        ob.partCodeSNo = Convert.ToInt16(dr["itemdet_partcode_sno"] == DBNull.Value ? 0 : dr["itemdet_partcode_sno"]);
            //        ob.sNo = dr["itemdet_serial_no"].ToString();
            //        ob.warrantyApplicable = dr["itemdet_warranty_applicable"].ToString();
            //        ob.warrantyEndDate = Convert.ToDateTime(dr["itemdet_warranty_start_date"] == DBNull.Value ? DateTime.MinValue : dr["itemdet_warranty_start_date"]);
            //        ob.warrantyStartDate = Convert.ToDateTime(dr["itemdet_warranty_end_date"] == DBNull.Value ? DateTime.MinValue : dr["itemdet_warranty_end_date"]);
            //        ob.amcApplicable = dr["itemdet_amc_applicable"].ToString();
            //        ob.amcEndDate = Convert.ToDateTime(dr["itemdet_amc_start_date"] == DBNull.Value ? DateTime.MinValue : dr["itemdet_amc_start_date"]);
            //        ob.amcStartDate = Convert.ToDateTime(dr["itemdet_amc_end_date"] == DBNull.Value ? DateTime.MinValue : dr["itemdet_amc_end_date"]);
            //        ob.BillNo = dr["itemdet_bill_no"].ToString();
            //        ob.deliveryDate = Convert.ToDateTime(dr["itemdet_delivery_date"] == DBNull.Value ? DateTime.MinValue : dr["itemdet_delivery_date"]);
            //        ob.deliveryRemarks = dr["itemdet_delivery_remarks"].ToString();
            //        itemDetails.Add(ob);
            //    }
            //}
        }
        catch (Exception ex)
        {
            error.errorCode = -1;
            error.errorResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
        }
        return items;
    }
    public List<ProductMaster> ProductList(ref ErrorVO error)
    {
        List<ProductMaster> itemDetails = new List<ProductMaster>();
        try
        {
            m_Connection.OpenDB("Galaxy");
            string strsql = "select * from crm_products";
            m_CommandODBC = new OdbcCommand(strsql, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            OdbcDataReader dr = m_CommandODBC.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    ProductMaster ob = new ProductMaster();
                    ob.Id = Convert.ToInt32(dr["product_id"] ?? 0);
                    ob.transactionNumber = dr["transaction_number"].ToString();
                    ob.Name = dr["product_name"].ToString();
                    ob.useable = dr["product_enabled"].ToString();
                    ob.customAttribute = dr["product_custom_attribute"].ToString();
                    ob.billingType = dr["product_billing_type"].ToString();
                    ob.category = dr["product_category"].ToString();
                    ob.subCategory = dr["product_subcategory"].ToString();
                    ob.manufacturer = dr["product_manufacturer"].ToString();
                    ob.model = dr["product_model"].ToString();
                    ob.warrantyApplicable = dr["product_warranty_applicable"].ToString();
                    ob.warrantStartDate = Convert.ToDateTime(dr["product_warranty_start_date"] == DBNull.Value ? DateTime.MinValue : dr["product_warranty_start_date"]);
                    ob.warrantyDuration = Convert.ToInt16(dr["product_warranty_duration"] == DBNull.Value ? 0 : dr["product_warranty_duration"]);
                    ob.amcApplicable = dr["product_amc_applicable"].ToString();
                    ob.localTaxName1 = dr["product_local_tax1_name"].ToString();
                    ob.localTaxName2 = dr["product_local_tax2_name"].ToString();
                    ob.localTaxName3 = dr["product_local_tax3_name"].ToString();
                    ob.cstTaxName1 = dr["product_cst_tax1_name"].ToString();
                    ob.cstTaxName2 = dr["product_cst_tax2_name"].ToString();
                    ob.cstTaxName3 = dr["product_cst_tax3_name"].ToString();
                    ob.minBasicRate = Convert.ToDecimal(dr["product_min_basic_rate"] == DBNull.Value ? 0 : dr["product_min_basic_rate"]);
                    ob.maxBasicRate = Convert.ToDecimal(dr["product_max_basic_rate"] == DBNull.Value ? 0 : dr["product_max_basic_rate"]);
                    itemDetails.Add(ob);
                }
            }
        }
        catch (Exception ex)
        {
            error.errorCode = -1;
            error.errorResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
        }
        return itemDetails;
    }

    public List<Tax> TaxList(ref ErrorVO error)
    {

        List<Tax> items = new List<Tax>();
        try
        {
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("select * from opf_tax_master", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            OdbcDataReader dr = m_CommandODBC.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    Tax ob = new Tax();
                    ob.Type = dr["tax_type"].ToString();
                    ob.Code = dr["tax_code"].ToString();
                    ob.Rate = Convert.ToDecimal(dr["tax_rate"] == null ? 0 : dr["tax_rate"]);
                    ob.StartDate = Convert.ToDateTime(dr["tax_start_date"] == null ? DateTime.MinValue : dr["tax_start_date"]);
                    ob.EndDate = Convert.ToDateTime(dr["tax_end_date"] == null ? DateTime.MinValue : dr["tax_end_date"]);
                    items.Add(ob);
                }
            }
        }
        catch (Exception ex)
        {
            error.errorCode = -1;
            error.errorResult = ex.Message;
        }
        finally
        {
            m_Connection.CloseDB();
        }
        return items;
    }
}

