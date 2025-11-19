using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;
using SharedVO;
using Newtonsoft.Json;
/// <summary>
/// Summary description for OPFWS
/// </summary>
public partial class MasterWS
{
     #region
    [WebMethod]
    public DataSet FetchProduct(int parentid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterODBC = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = " SELECT " + m_Connection.DB_NULL + "(product_name,'') as ProductName," +
                      m_Connection.DB_NULL + "(transaction_number,'') as Code,product_id " +
                      " FROM crm_products WHERE product_enabled = 'Y' and parent_id= " +parentid.ToString()+
                      " ORDER BY product_name ASC";

           m_DataAdapterODBC = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterODBC.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Product";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchProduct", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchProduct", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion

    #region
    [WebMethod]
    public DataSet FetchProductCategories()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterODBC = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(categ_name,'') as Name," +
                      m_Connection.DB_NULL + "(categ_code,'') as Code " +
                      "FROM crm_product_categories WHERE categ_enabled = 'Y' " +
                      "ORDER BY categ_name ASC";

            m_DataAdapterODBC = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterODBC.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Product";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchProductCategories", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchProductCategories", strParameters);
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion
    #region
    [WebMethod]
    public DataSet newCallinfo( string CLI, string Language,string Product, string Category,string CardNo,string  Name,string  EmailID,string ContactNo,string  Address,string ip ,string	date,int by )
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
       
        try
        {
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("EXEC newcallinfo ?,?,?,?,?,?,?,?,?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@CLI", OdbcType.VarChar).Value =CLI ;
            m_CommandODBC.Parameters.Add("@Language", OdbcType.VarChar).Value = Language;
            m_CommandODBC.Parameters.Add("@Product", OdbcType.VarChar).Value = Product;
            m_CommandODBC.Parameters.Add("@Category", OdbcType.VarChar).Value =Category ;
            m_CommandODBC.Parameters.Add("@CardNo", OdbcType.VarChar).Value = CardNo;
            m_CommandODBC.Parameters.Add("@Name", OdbcType.VarChar).Value = Name;
            m_CommandODBC.Parameters.Add("@EmailID", OdbcType.VarChar).Value = EmailID;
            m_CommandODBC.Parameters.Add("@ContactNo", OdbcType.VarChar).Value = ContactNo;
            m_CommandODBC.Parameters.Add("@Address", OdbcType.VarChar).Value = Address;

            m_CommandODBC.Parameters.Add("@ip", OdbcType.VarChar).Value = ip;
            m_CommandODBC.Parameters.Add("@date", OdbcType.VarChar).Value = date;
            m_CommandODBC.Parameters.Add("@by", OdbcType.Int).Value = by;
         //   DataSet ds = new DataSet();
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            
          
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
      //  m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;
    }
    #endregion
    #region
    [WebMethod]
    public DataSet getCallinfo( string Product, string CardNo)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataSet ds = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("EXEC getcalldetails ?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@Product", OdbcType.VarChar).Value = Product;
            m_CommandODBC.Parameters.Add("@CardNo", OdbcType.VarChar).Value =CardNo ;
          
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
             m_DataAdapter.Fill(ds);
          
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
        return ds;
    }
    #endregion


    #region
    [WebMethod]
    public DataSet InsertCTIHistory(string CallNumber, string UserId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataSet ds = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("EXEC sp_crm_cti_Inbound_History '" + UserId + "','" + CallNumber + "'", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
          nResultCode=  m_CommandODBC.ExecuteNonQuery();
         //   m_CommandODBC.Parameters.Add("@CallNumber", OdbcType.VarChar).Value = CallNumber;
         //   m_CommandODBC.Parameters.Add("@UserId", OdbcType.VarChar).Value = UserId;

          //  OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
          //  m_DataAdapter.Fill(ds);

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
        return ds;
    }
    #endregion


    #region
    [WebMethod]
    public DataSet GetProductId(string productcode)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        m_DataAdapterODBC = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = " SELECT top 1 product_id " +
                      " FROM crm_products WHERE product_enabled = 'Y' and product_custom_attribute= '" + productcode + "'";

            m_DataAdapterODBC = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterODBC.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Product";
            nResultCode = 0;
            strResult = "Pass";
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchProduct", strParameters);
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            LogMessage(strTemp + strResult, "FetchProduct", strParameters);
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