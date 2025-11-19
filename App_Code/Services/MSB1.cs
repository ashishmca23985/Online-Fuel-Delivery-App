using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Odbc;
using System.Web.Services;
using SharedVO;
public partial class MasterWS
{
    #region Get Contact Wise Product
    [WebMethod]
    
    public DataSet FetchContactProduct(string text,int id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select product_id,product_name  from crm_products where product_enabled='Y' and product_name like '%" + text + "%' order by product_name";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_products";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_products", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_products", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetAllProduct
    [WebMethod]

    public DataSet FetchProductfilter(string text)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select product_id,product_name  from crm_products where product_enabled='Y' and  parent_id = 0 and product_name like '%"+text+"%' order by product_name";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "crm_products";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_products", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "crm_products", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    


    #region Fetch All Contact
    [WebMethod]
    public DataSet GetContact(string text,string type)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select id,contact_full_name Name from crm_contacts  where Useable='Y' and contact_enabled='Y' and contact_type_id='" + type + "' and contact_full_name like '%" + text + "%' order by Name";
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Contact";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Contact", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Contact", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Fetch All Contact
    [WebMethod]
    public DataSet GetContactwithProduct(string text, string type)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select id,contact_full_name + case when isnull( related_to_name,'')<>''then '('+isnull(related_to_name,'')+' )' else '' end Name, "+
                       "cust_location,cust_latitute,cust_req_type,cust_filling_type,cust_filling_contactno,cust_loc_permission," +
                        "cust_approchaccess,cust_fule_type,cust_fule_qty,cust_paymentmode,cust_service,cust_longitute from crm_contacts where Useable = 'Y' and contact_enabled = 'Y' and  contact_type_id ='" + type + "' and contact_full_name like '%" + text + "%' order by Name";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Contact";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Contact", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Contact", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Fetch All Account
    [WebMethod]
    public DataSet GetAccount(string text, string type)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select id,cust_name as Name from crm_accounts  where cust_enabled ='Y' and cust_name='" + type + "' and cust_name like '%" + text + "%' order by cust_name";
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Account";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Account", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Account", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion




    #region Fetch Contact Details
    [WebMethod]
    public DataSet GetCallDetails(int id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
              strTemp = "select id,contact_full_name,contact_phone,contact_emailid,contact_address1 as Address,contact_zone as Region ," +
                "related_to_id,related_to_name,cust_location,cust_req_type,cust_filling_type,cust_filling_contactno,cust_loc_permission," +
                "cust_approchaccess,cust_fule_type,cust_fule_qty,cust_paymentmode,cust_service,cust_latitute + ',' + cust_longitute as LatLongitute " +
                "  from crm_contacts where useable='Y' and id =" + id;

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "caller";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "caller", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "caller", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion


    #region Fetch All Contact
    [WebMethod]
    public DataSet GetContactList(string phoneno, string email)
    {
        int count = 0;
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "select id " +
                " from crm_contacts" +
                " where Useable='Y' and contact_type_id='C' and (";
           
                strTemp += "(contact_phone ='" + phoneno + "' or id in (select related_to_id from crm_contact_details where related_to='CNT' and type='Phone' and value='" + phoneno + "'))";
            if (email != "")
                strTemp += "or (contact_emailid ='" + email + "' or id in (select related_to_id from crm_contact_details where related_to='CNT' and type='Email' and value='" + email + "'))";
                strTemp += ") ";
              OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
             m_DataAdapterOdbc.Fill(m_DataSet);
            nResultCode = count;
            strResult = "";
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Fetch Contact Product
    [WebMethod]
    public DataSet GetContactProduct(string id)
    {
        int count = 0;
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "select contact_product,contact_card_no from crm_contacts where id="+id;
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "contacts";
            nResultCode = 1;
            strResult = "";
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
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion



    #region Fetch  Details
    [WebMethod]
    public DataSet GetDetails(int id,string type)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            if (type == "E")
            {
                strTemp = "select id as Id,mail_number as tab   from EMAIL_Mails where  related_to='CAS' and mail_folder='Inbox' and related_to_id=" + id;
            }
            else if(type=="C")
            {
                strTemp = "select id as Id,chat_visitor_name as tab  from crm_chat_sessions WHERE related_to='CAS' and related_to_id=" + id;
            }
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Details";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Details", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Details", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion
    #region Fetch All Driver
    [WebMethod]
    public DataSet GetDriverList(string text, string type)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select id,contact_full_name Name from crm_contacts  where Useable='Y' and contact_enabled='Y' and contact_role_id = 9 and contact_type_id='" + type + "' and contact_full_name like '%" + text + "%' order by Name";
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Contact";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Contact", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Contact", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Fetch All Driver
    [WebMethod]
    public DataSet GetDriverListMaster()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select id,contact_full_name Name from crm_contacts  where Useable='Y' and contact_enabled='Y' and contact_role_id = 9 order by Name";
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Contact";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Contact", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Contact", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Get CheckList Content Details
    [WebMethod]
    public DataSet GetCheckListContentDetails(int id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "select a.id as TicketId,A.transaction_number,a.created_date as created_date, dbo.GetContactName(A.created_by) as created_by ,a.case_caller_name as CustomarName,b.driver_name as DriverName," +
               "  dbo.GetContactAddress(case_caller_id) as CustAddress,dbo.GetCustPhone(case_caller_id) as case_caller_phone " +
                  ",dbo.GetCustEmail(case_caller_id) as case_caller_email,a.case_status_name as TicketStatus,A.case_cust_Sig_Path AS CustSignature," +
                  "a.cust_location as CustLocation,a.cust_fule_type as FuelType, b.vehicle_no as VehicleNo,b.gensate_model_name as GensetModel,b.gensate_capecity as GensetCapecity," +
                  "a.Fuel_rate as FuelRate ,a.Fuel_cost as FuleCost,a.fuel_tax as FuelTax,b.Fuel_tax_amount as TaxAmount,B.filling_date as filling_date,B.cust_latLong as cust_latLong,b.allocation_date as allocation_date, " +
                  "b.site_accessbility as site_accessbility,b.fueltank_capecity as fueltank_capecity," +
                  "b.SiteExpensesValue1,b.SiteExpensesValue2,b.SiteExpensesValue3,b.ReadingFuelFillingBefore,b.ReadingFuelFillingAfter,b.ReadingGensetHMR,b.ReadingFuelTank,b.ReadingCMAfterFuelTank," +
                  "b.FuelLevelReadingFlowMetorBefore,b.FuelLevelReadingFlowMetorAfter,b.FuelfilledQty,b.fuel_tax_amount,b.final_Fuel_cost,b.TotalSiteExpenseAmt,A.cust_fule_qty from crm_cases A inner   " +
                  "join crm_documents_checklist B ON A.id = B.related_to_id and A.case_status_id=2 and (a.id= " + id + ")";

            //strTemp = "select id,contact_full_name,contact_phone,contact_emailid,contact_address1 as Address ," +
            //  "related_to_name,cust_location,cust_req_type,cust_filling_type,cust_filling_contactno,cust_loc_permission," +
            //  "cust_approchaccess,cust_fule_type,cust_fule_qty,cust_paymentmode,cust_service,cust_latitute + ',' + cust_longitute as LatLongitute " +
            //  "  from crm_contacts where useable='Y' and id =" + id;

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "CheckListContent";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetCheckListContentDetails", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "GetCheckListContentDetails", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Fetch All Contact
    [WebMethod]
    public DataSet GetCallerList(string text, string type)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select distinct id,contact_full_name + case when isnull( related_to_name,'')<>''then '('+isnull(related_to_name,'')+' )' else '' end Name, " +
                       "cust_location,cust_latitute,cust_req_type,cust_filling_type,cust_filling_contactno,cust_loc_permission," +
                        "cust_approchaccess,cust_fule_type,cust_fule_qty,cust_paymentmode,cust_service,cust_longitute from crm_contacts where Useable = 'Y' and contact_enabled = 'Y' and  contact_type_id ='" + type + "' and contact_full_name like '%" + text + "%' order by Name";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Contact";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Contact", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Contact", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region Fetch All E-Funnel DeviceId
    [WebMethod]
    public DataSet GetEFunnelDeviceId()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select distinct EFunnelDeviceId as Name from crm_vehicle_master where enabled = 'Y'";
            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Name";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Contact", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "Contact", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion
}