using System;
using System.Collections;
using System.Web;
using System.Data;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Data.SqlTypes;
using System.Web.Script.Services;
using System.Data.SqlClient;
using System.Xml;
using System.Data.Odbc;
using System.Configuration;
using System.Xml.Linq;
using SharedVO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Web.Script.Serialization;


/// <summary>
/// Summary description for MasterWS
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
[ScriptService]
public partial class MasterWS : System.Web.Services.WebService
{
    DataBase m_Connection = new DataBase();
    OdbcCommand m_CommandODBC;
    OdbcDataAdapter m_DataAdapterODBC;
    string strTemp = "";
    string strIPAddress = HttpContext.Current.Request.UserHostAddress;
    string strParameters = "";

    public MasterWS()
    {
    }

    #region Loading Call Type
    [WebMethod]
    public DataSet LoadCallTypes()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT type_id,type_name,type_enabled," +
                      "type_reference_field_caption,SLA_Required " +
                      "FROM crm_types " +
                      "WHERE type_for='C' and type_enabled='Y'";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "CallType";

            nResultCode = 0;
            strResult = "Pass";
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
    #region Loading Call Type
    [WebMethod]
    public DataSet LoadTypes(string types)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT type_id,type_name,type_enabled," +
                      "type_reference_field_caption " +
                      "FROM crm_types " +
                      "WHERE type_for='" + types + "' and type_enabled='Y'";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "CallType";

            nResultCode = 0;
            strResult = "Pass";
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

    #region Loading Task Type
    [WebMethod]
    public DataSet LoadTaskTypes()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("CallType");

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT type_id,type_name,type_enabled " +
                      "FROM crm_types " +
                      "WHERE type_for='T' and type_enabled='Y' ";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "CallType";

            nResultCode = 0;
            strResult = "Pass";
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

    #region Update or Insert the Call Type
    [WebMethod]
    public DataSet InsertUpdate(long nTypeID, string strName, string strEnabled, string strReference, string strServiceReference, string strRatingCategIds, string strEnabledSLR)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            string strTemp = "Select Count(*) as count from crm_types " +
                                " where type_name = ?";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);


            if (nName > 0 && nTypeID == 0) // checking the existence the record
            {
                nResultCode = -1;
                strResult = "Ticket Type already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else if (nTypeID == 0 && nTypeID == 0) // for inserting the record
            {

                strTemp = "INSERT into crm_types (type_name,type_enabled,type_reference_field_caption,type_for,type_service_reference_caption,SLA_Required)" +
                          " Values (?,'" + strEnabled + "',?,'C',?,?)";

                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
                m_CommandODBC.Parameters.Add("@Reference", OdbcType.VarChar).Value = strReference;
                m_CommandODBC.Parameters.Add("@Reference1", OdbcType.VarChar).Value = strServiceReference;
                m_CommandODBC.Parameters.Add("@SLA_Required", OdbcType.VarChar).Value = strEnabledSLR;
                m_CommandODBC.ExecuteNonQuery();
                
                    nResultCode = 0;
                    strResult = "Ticket Type Added successfully!";
                    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                    return m_DataSet;
                
            }

            else  // for updating the record
            {
                strTemp = "UPDATE crm_types  " +
                          "SET  type_name = ?, type_enabled = '" + strEnabled + "'" +
                           ",type_reference_field_caption = ?" +
                           ",type_service_reference_caption= ? " +
                           ",SLA_Required= ? " +
                           " Where type_id = " + nTypeID + "";

                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
                m_CommandODBC.Parameters.Add("@Reference", OdbcType.VarChar).Value = strReference;
                m_CommandODBC.Parameters.Add("@Reference1", OdbcType.VarChar).Value = strServiceReference;
                m_CommandODBC.Parameters.Add("@SLA_Required", OdbcType.VarChar).Value = strEnabledSLR;
                m_CommandODBC.ExecuteNonQuery();
                nResultCode = 0;
                strResult = "Ticket Type Updated successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }

            
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



    #region Update or Insert the For The Task
    [WebMethod]
    public DataSet InsertUpdateForTask(long nTypeID, string strName, string strEnabled, string strReference, string strRatingIds)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            string strTemp = "Select Count(*) as count from crm_types " +
                                " where type_name = ?";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);

            if (nName > 0 && nTypeID == 0) // checking the existence the record
            {
                nResultCode = -1;
                strResult = "Task Type already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else if (nTypeID == 0 && nTypeID == 0) // for inserting the record
            {

                strTemp = "INSERT into crm_types (type_name,type_enabled,type_reference_field_caption,type_for)" +
                          " Values (?,'" + strEnabled + "','" + strReference + "','T')";

                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
                m_CommandODBC.ExecuteNonQuery();
                    nResultCode = 0;
                    strResult = "Task Type Added successfully!";
                    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                    return m_DataSet;
                
            }

            else  // for updating the record
            {
                strTemp = "UPDATE crm_types  " +
                          "SET  type_name = ?, type_enabled = '" + strEnabled + "'" +
                           ",type_reference_field_caption = ?" +
                           " Where type_id = " + nTypeID + "";

                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
                m_CommandODBC.Parameters.Add("@Reference", OdbcType.VarChar).Value = strReference;
                m_CommandODBC.ExecuteNonQuery();
                nResultCode = 0;
                strResult = "Task Type Updated successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }        

            //if (nTypeID == 0)
            //{
            //    strTemp = "Select type_id from crm_types where  type_name=?";
            //    m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            //    m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            //    m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
            //    DataTable dtid = new DataTable("Types");
            //    m_DataAdapterODBC.Fill(dtid);
            //    if (dtid.Rows.Count > 0)
            //        nTypeID = Convert.ToInt32(dtid.Rows[0][0]);

            //}
            //if (strRatingIds != "" && strRatingIds != "," && nTypeID > 0)
            //{

            //    strTemp = "update crm_rating_categories set  categ_task_types= (REPLACE(categ_task_types,'," + nTypeID + ",',',')) from crm_rating_categories where categ_task_types like '%," + nTypeID + ",%' " +
            //        "update crm_rating_categories set  categ_task_types= " + m_Connection.DB_NULL + "(categ_task_types,',')+'" + nTypeID + ",'  where categ_id in (select Items from dbo.Split('" + strRatingIds + "',',') ) ";
            //    //   strTemp = "update crm_rating_categories set  categ_case_types= (REPLACE(categ_case_types,'," + nTypeID + ",',','))+'" + nTypeID + ",'  where categ_id in (select Items from dbo.Split('" + strRatingCategIds + "',',') ) ";
            //    m_CommandODBC = new OdbcCommand();
            //    m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            //    m_CommandODBC.ExecuteNonQuery();
            //}
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

    #region Loading Zones
    [WebMethod]
    public DataSet LoadZones()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("ZoneType");

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT zone_id,zone_name,zone_enabled " +
                      "FROM crm_zones ";


            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Update or Insert the Zone
    [WebMethod]
    public DataSet InsertUpdateZone(long nTypeID, string strName, string strEnabled)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            string strTemp = "Select Count(*) as count from crm_zones " +
                                " where zone_name = ?";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);


            if (nName > 0 && nTypeID == 0) // checking the existence the record
            {
                nResultCode = -1;
                strResult = "Zone Type already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else if (nTypeID == 0 && nTypeID == 0) // for inserting the record
            {

                strTemp = "INSERT into crm_zones (zone_name,zone_enabled)" +
                          " Values (?,'" + strEnabled + "')";

            }

            else  // for updating the record
            {
                strTemp = "UPDATE crm_zones  " +
                          "SET  zone_name = ?, zone_enabled = '" + strEnabled + "'" +
                          "Where zone_id = " + nTypeID + "";
            }

            m_CommandODBC = new OdbcCommand();
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
            m_CommandODBC.ExecuteNonQuery();

            if (nTypeID == 0)
            {
                nResultCode = 0;
                strResult = "Zone  Added successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            nResultCode = 0;
            strResult = "Zone Type Updated successfully!";
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
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

    #region Loading Teams
    [WebMethod]
    public DataSet LoadTeams()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("TeamType");

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT team_id,team_name,team_enabled " +
                      "FROM crm_teams ";


            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Update or Insert the Teams
    [WebMethod]
    public DataSet InsertUpdateTeam(long nTypeID, string strName, string strEnabled)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            string strTemp = "Select Count(*) as count from crm_teams " +
                                " where team_name = ?";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);


            if (nName > 0 && nTypeID == 0) // checking the existence the record
            {
                nResultCode = -1;
                strResult = "Team Name already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else if (nTypeID == 0 && nTypeID == 0) // for inserting the record
            {

                strTemp = "INSERT into crm_teams (team_name,team_enabled)" +
                          " Values (?,'" + strEnabled + "')";

            }

            else  // for updating the record
            {
                strTemp = "UPDATE crm_teams  " +
                          "SET  team_name = ?, team_enabled = '" + strEnabled + "'" +
                          "Where team_id = " + nTypeID + "";
            }

            m_CommandODBC = new OdbcCommand();
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
            m_CommandODBC.ExecuteNonQuery();

            if (nTypeID == 0)
            {
                nResultCode = 0;
                strResult = "Team  Added successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            nResultCode = 0;
            strResult = "Team Type Updated successfully!";
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
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

    #region Loading Skills
    [WebMethod]
    public DataSet LoadSkill()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT skill_id,skill_name,skill_enabled " +
                      "FROM crm_skills ";


            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "SkillType";

            nResultCode = 0;
            strResult = "Pass";
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

    #region Update or Insert the Skills
    [WebMethod]
    public DataSet InsertUpdateSkill(long nTypeID, string strName, string strEnabled)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            string strTemp = "Select Count(*) as count from crm_skills " +
                                " where skill_name = ?";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);


            if (nName > 0 && nTypeID == 0) // checking the existence the record
            {
                nResultCode = -1;
                strResult = "Skill Name already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else if (nTypeID == 0 && nTypeID == 0) // for inserting the record
            {

                strTemp = "INSERT into crm_skills (skill_name,skill_enabled)" +
                          " Values (?,'" + strEnabled + "')";

            }

            else  // for updating the record
            {
                strTemp = "UPDATE crm_skills  " +
                          "SET  skill_name = ?, skill_enabled = '" + strEnabled + "'" +
                          "Where skill_id = " + nTypeID + "";
            }

            m_CommandODBC = new OdbcCommand();
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
            m_CommandODBC.ExecuteNonQuery();

            if (nTypeID == 0)
            {
                nResultCode = 0;
                strResult = "Skill Added successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            nResultCode = 0;
            strResult = "Skill Type Updated successfully!";
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
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

    #region Loading Designation
    [WebMethod]
    public DataSet LoadDesignation()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("DesigType");

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT desig_id,desig_name,desig_enabled " +
                      "FROM crm_designations ORDER BY desig_name";


            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Update or Insert the Designation
    [WebMethod]
    public DataSet InsertUpdateDesignation(long nTypeID, string strName, string strEnabled)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            string strTemp = "Select Count(*) as count from crm_designations " +
                                " where desig_name = ?";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);


            if (nName > 0 && nTypeID == 0) // checking the existence the record
            {
                nResultCode = -1;
                strResult = "Designation Name already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else if (nTypeID == 0 && nTypeID == 0) // for inserting the record
            {

                strTemp = "INSERT into crm_designations (desig_name,desig_enabled)" +
                          " Values (?,'" + strEnabled + "')";

            }

            else  // for updating the record
            {
                strTemp = "UPDATE crm_designations  " +
                          "SET  desig_name = ?, desig_enabled = '" + strEnabled + "'" +
                          "Where desig_id = " + nTypeID + "";
            }

            m_CommandODBC = new OdbcCommand();
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
            m_CommandODBC.ExecuteNonQuery();

            if (nTypeID == 0)
            {
                nResultCode = 0;
                strResult = "Designation Added successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            nResultCode = 0;
            strResult = "Designation Updated successfully!";
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
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

    #region Update or Insert the Department
    [WebMethod]
    public DataSet InsertUpdateDepartment(long nTypeID, string strName, string strEnabled)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            string strTemp = "Select Count(*) as count from crm_departments " +
                                " where dept_name = ?";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);


            if (nName > 0 && nTypeID == 0) // checking the existence the record
            {
                nResultCode = -1;
                strResult = "Department Name already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else if (nTypeID == 0 && nTypeID == 0) // for inserting the record
            {

                strTemp = "INSERT into crm_departments (dept_name,dept_enabled)" +
                          " Values (?,'" + strEnabled + "')";

            }

            else  // for updating the record
            {
                strTemp = "UPDATE crm_departments  " +
                          "SET  dept_name = ?, dept_enabled = '" + strEnabled + "'" +
                          "Where dept_id = " + nTypeID + "";
            }

            m_CommandODBC = new OdbcCommand();
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.Add("@TypeName", OdbcType.VarChar).Value = strName;
            m_CommandODBC.ExecuteNonQuery();

            if (nTypeID == 0)
            {
                nResultCode = 0;
                strResult = "Department Added successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            nResultCode = 0;
            strResult = "Department Type Updated successfully!";
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
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

    #region Insert the Account Department
    [WebMethod]
    public DataSet InsertAccountDepartment(int nCustomerID, string strName)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "insert into crm_account_departments(accdept_name,accdept_account_id)" +
                      " Values (?," + nCustomerID + ")";

            m_CommandODBC = new OdbcCommand();
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_CommandODBC.Parameters.Add("@DeptName", OdbcType.VarChar).Value = strName;
            m_CommandODBC.ExecuteNonQuery();

            nResultCode = 0;
            strResult = "Department Added successfully!";
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

    #region Update or Insert the Country
    [WebMethod]
    public DataSet InsertUpdateCountry(string strOriginalCode, string strName, string strCode)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            string strTemp = "Select Count(*) as count from crm_countries " +
                                " where country_name = ? or country_code = ?";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@country_name", OdbcType.VarChar).Value = strName;
            m_CommandODBC.Parameters.Add("@country_code", OdbcType.VarChar).Value = strCode;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);


            if (nName > 0) // checking the existence the record
            {
                nResultCode = -1;
                strResult = "Country Name and code  already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            if (strOriginalCode == "") // for inserting the record
            {

                strTemp = "INSERT into crm_countries (country_name, country_code)" +
                          "VALUES (?, ?)";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@Name", OdbcType.VarChar).Value = strName;
                m_CommandODBC.Parameters.Add("@Code", OdbcType.VarChar).Value = strCode;

            }
            else  // for updating the record
            {
                strTemp = "UPDATE crm_countries  " +
                          "SET country_name = ?, country_code = ? " +
                          "WHERE country_code = ?";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@Name", OdbcType.VarChar).Value = strName;
                m_CommandODBC.Parameters.Add("@Code", OdbcType.VarChar).Value = strCode;
                m_CommandODBC.Parameters.Add("@OriginalCode", OdbcType.VarChar).Value = strOriginalCode;
            }

            m_CommandODBC.ExecuteNonQuery();

            if (strOriginalCode == "")
            {
                nResultCode = 0;
                strResult = "Country Added successfully!";
            }
            else
            {
                nResultCode = 0;
                strResult = "Country Updated successfully!";
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            if (nResultCode == 2627)
                strResult = "Country code already exists !";
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

    #region Update or Insert the State
    [WebMethod]
    public DataSet InsertUpdateState(string strOriginalCode, string strName, string strCode, string strParentCode)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            string strTemp = "Select Count(*) as count from crm_states " +
                                " where state_country_code=? and (state_name = ? or state_code=? )";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@state_country_code", OdbcType.VarChar).Value = strParentCode;
            m_CommandODBC.Parameters.Add("@state_name", OdbcType.VarChar).Value = strName;
            m_CommandODBC.Parameters.Add("@Code", OdbcType.VarChar).Value = strCode;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);


            if (nName > 0 ) // checking the existence the record
            {
                nResultCode = -1;
                strResult = "State Name and Code already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }


            if (strOriginalCode == "") // for inserting the record
            {
                strTemp = "INSERT into crm_states (state_name, state_code," +
                          "state_country_code)" +
                          "VALUES (?, ?, ?)";

                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@Name", OdbcType.VarChar).Value = strName;
                m_CommandODBC.Parameters.Add("@Code", OdbcType.VarChar).Value = strCode;
                m_CommandODBC.Parameters.Add("@ParentCode", OdbcType.VarChar).Value = strParentCode;

            }
            else  // for updating the record
            {
                strTemp = "UPDATE crm_states  " +
                          "SET  state_name = ?," +
                          "state_code = ?," +
                          "state_country_code = ?  " +
                          "WHERE state_code = ?";

                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@Name", OdbcType.VarChar).Value = strName;
                m_CommandODBC.Parameters.Add("@Code", OdbcType.VarChar).Value = strCode;
                m_CommandODBC.Parameters.Add("@ParentCode", OdbcType.VarChar).Value = strParentCode;
                m_CommandODBC.Parameters.Add("@OriginalCode", OdbcType.VarChar).Value = strOriginalCode;
            }
            m_CommandODBC.ExecuteNonQuery();

            if (strOriginalCode == "")
            {
                nResultCode = 0;
                strResult = "State added successfully!";
            }
            else
            {
                nResultCode = 0;
                strResult = "State updated successfully!";
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            if (nResultCode == 2627)
                strResult = "State code already exists !";
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

    #region Update or Insert the City
    [WebMethod]
    public DataSet InsertUpdateCity(string strOriginalCode, string strName, string strCode, string strParentCode)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            string strTemp = "Select Count(*) as count from crm_cities " +
                                " where city_state_code=? and ( city_name = ? or city_code=?)  ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@city_state_code", OdbcType.VarChar).Value = strParentCode;
            m_CommandODBC.Parameters.Add("@city_name", OdbcType.VarChar).Value = strName;
            m_CommandODBC.Parameters.Add("@city_code", OdbcType.VarChar).Value = strCode;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);


            if (nName > 0 ) // checking the existence the record
            {
                nResultCode = -1;
                strResult = "City Name and code Type already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            if (strOriginalCode == "") // for inserting the record
            {

                strTemp = "INSERT into crm_cities (city_name, city_code, city_state_code)" +
                          " Values (?, ?, ?)";

                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@Name", OdbcType.VarChar).Value = strName;
                m_CommandODBC.Parameters.Add("@Code", OdbcType.VarChar).Value = strCode;
                m_CommandODBC.Parameters.Add("@ParentCode", OdbcType.VarChar).Value = strParentCode;

            }
            else  // for updating the record
            {
                strTemp = "UPDATE crm_cities  " +
                          "SET  city_name = ?, city_code = ? " +
                          " Where city_code = ?";

                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@Name", OdbcType.VarChar).Value = strName;
                m_CommandODBC.Parameters.Add("@Code", OdbcType.VarChar).Value = strCode;
                m_CommandODBC.Parameters.Add("@OriginalCode", OdbcType.VarChar).Value = strOriginalCode;
            }

            m_CommandODBC.ExecuteNonQuery();

            if (strOriginalCode == "")
            {
                nResultCode = 0;
                strResult = "City Added successfully!";
            }
            else
            {
                nResultCode = 0;
                strResult = "City Updated successfully!";
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            if (nResultCode == 2627)
                strResult = "City code already exists !";
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

    #region Get Departments
    [WebMethod]
    public DataSet GetDepartments(int AccountID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Department");

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT dept_id,dept_name,dept_sc_id,dept_enabled," +
                      "(case when dept_enabled = 'Y' then 'Yes' else 'No' end) as DepatmentEnabled  " +
                      "FROM crm_departments";

            if (AccountID > 0)
                strTemp += " WHERE dept_name NOT IN (SELECT accdept_name FROM crm_account_departments " +
                           "WHERE accdept_account_id = " + AccountID + ")";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Queues
    [WebMethod]
    public DataSet GetQueues(int nQueueID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Queue");

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT id as contact_id,contact_full_name,contact_enabled," +
                      "contact_enabled " +
                      "FROM crm_contacts where contact_type_id = 'Q' and id = " + nQueueID + "";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Deginations
    [WebMethod]
    public DataSet GetDesignations()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Designation");
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT desig_id,desig_name,desig_enabled," +
                      "(case when desig_enabled = 'Y' then 'Yes' else 'No' end) as DesignEnabled  " +
                      "FROM crm_designations ";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Call Types
    [WebMethod]
    public DataSet GetCallType(string strType)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("CallType");
        string type = "categ_case_types";
        if (strType == "C")
            type = "categ_case_types";
        else
            type = "categ_task_types";
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT type_id, " + m_Connection.DB_NULL + "(type_name,'')as type_name,(case when type_enabled='y' then 'Yes' else 'No' end) as type_enabled," +
                        m_Connection.DB_NULL + "(type_reference_field_caption,'') as type_reference_field_caption , " + m_Connection.DB_NULL + "(type_service_reference_caption,'') as" +
                        " type_service_reference_caption,SLA_Required FROM crm_types WHERE type_for='" + strType + "'";

            //strTemp = "SELECT type_id," + m_Connection.DB_NULL + "(type_name,'')as type_name," +
            //          "(case when type_enabled='y' then 'Yes' else 'No' end) as type_enabled," +
            //          m_Connection.DB_NULL + "(type_reference_field_caption,'') as type_reference_field_caption ," +
            //          m_Connection.DB_NULL + "(type_service_reference_caption,'') as type_service_reference_caption " +
            //          "FROM crm_types WHERE type_for='" + strType + "'";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Call Types
    [WebMethod]
    public DataSet GetSkill()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("SkillType");

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT skill_id,skill_name,skill_enabled," +
                      "(case when skill_enabled='Y' then 'Yes' else 'No' end)as skillenable " +
                       "FROM crm_skills";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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


    #region Get Call Team
    [WebMethod]
    public DataSet GetTeams()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT team_id,team_name,team_enabled," +
                      "(case when team_enabled = 'Y' then 'Yes' else 'No' end)as teamenabled " +
                     "FROM crm_teams";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Teams";

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Service Type
    [WebMethod]
    public DataSet GetServiceType()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT type_id,type_name,type_enabled,type_case_types," +
                      "(case when type_enabled='Y' then 'Yes' else 'No' end) as Enabled," +
                      "type_reference_field_caption, type_for," +
                      m_Connection.DB_FUNCTION + "GetTypesDesc(type_case_types,',') as case_types " +
                      "FROM crm_types where type_for = 'S'";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "ServiceType";

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Zone Type
    [WebMethod]
    public DataSet GetZoneType()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("");

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT zone_id,zone_name,zone_enabled," +
                     "(CASE WHEN zone_enabled='Y' then 'Yes' else 'No' end) as zoneenabled " +
                     "FROM crm_zones";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "ZoneType";

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Case Type
    [WebMethod]
    public DataSet GetCaseType()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select type_name, type_id from crm_types where type_for = 'C'";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "CaseType";

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Countries
    [WebMethod]
    public DataSet GetCountries()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select country_code,country_name from crm_countries";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Countries";

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get States
    [WebMethod]
    public DataSet GetStates(string CountryCode)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("States");

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(state_code,'') as state_code," + m_Connection.DB_NULL + "(state_name,'') as state_name," +
                      m_Connection.DB_NULL + "(state_country_code,'') as state_country_code " +
                      "FROM CRM_States Where state_country_code='" + CountryCode + "'";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get City
    [WebMethod]
    public DataSet GetCity(string StateCode)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("City");

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT " + m_Connection.DB_NULL + "(city_code,'') as city_code," + m_Connection.DB_NULL + "(city_name,'') as city_name," +
                      m_Connection.DB_NULL + "(city_state_code,'') as city_state_code " +
                      "FROM crm_cities  Where city_state_code='" + StateCode + "'";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Loading Category
    [WebMethod]
    public DataSet LoadCategory()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Category");

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT categ_id,categ_name,categ_enabled,categ_case_types,categ_code," +
                                "(case when categ_enabled='Y' then 'Yes' else 'No' end) as Enabled,Categ_SLALevel0/1440 as Categ_SLALevel0,Categ_SLALevel1/1440 as Categ_SLALevel1,Categ_SLALevel2/1440 as Categ_SLALevel2,Categ_SLALevel3/1440 Categ_SLALevel3, " +
                                m_Connection.DB_FUNCTION + "GetTypesDesc(categ_case_types,',') as case_types " +
                                "FROM crm_categories Where categ_parent_id=0 order by case_types,categ_name";


            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Loading Status
    [WebMethod]
    public DataSet LoadStatus()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Status");

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = @"select status_id,status_name, status_for, case when status_for='CAS' then 'Case'  When status_for='LED'
                        then 'Lead' when status_for='OPF' then 'OPF' else 'Task' end as StatusType,status_sequence,status_enable,case when status_enable='Y' then
                        'Yes' else 'No' end Enabled from crm_status where status_for in ('CAS','TSK','LED','OPF') 
                        order by status_for,status_sequence";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Load Reason
    [WebMethod]
    public DataSet LoadReason(int StatusID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = @"select reason_id,reason_description,reason_status_id,reason_enable,case when reason_enable='Y' then 'Yes' else 'No' 
                        end Enabled,ISNULL( reason_type_ids,'')reason_type_ids ,dbo.GetTypesDesc (ISNULL( reason_type_ids,''),',') reason_type
                        from crm_reasons 
                        where reason_status_id=" + StatusID + "";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Reason";

            nResultCode = 0;
            strResult = "Pass";
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

    #region Loading Task Category
    [WebMethod]
    public DataSet LoadTaskCategory()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT categ_id,categ_name,categ_enabled,categ_task_types," +
                               "(case when categ_enabled='Y' then 'Yes' else 'No' end) as Enabled," +
                               m_Connection.DB_FUNCTION + "GetTypesDesc(categ_task_types,',') as task_types " +
                               "FROM crm_task_categories Where categ_parent_id=0 ";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "TaskCategory";

            nResultCode = 0;
            strResult = "Pass";
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

    #region Load Sub Category
    [WebMethod]
    public DataSet LoadSubCategory(int CategoryID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT categ_id,categ_name,categ_enabled,categ_parent_id,categ_code," +
                       "(case when categ_enabled='Y' then 'Yes' else 'No' end) as Enabled,Categ_cbnSLA " +
                       "FROM crm_categories Where categ_parent_id=" + CategoryID + " ";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "SubCategory";

            nResultCode = 0;
            strResult = "Pass";
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

    #region Update or Insert the City
    [WebMethod]
    public DataSet InsertUpdateService(long nTypeID, string strName, string strEnabled, string strServiceType)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        string strTemp = "";
        try
        {
            m_Connection.OpenDB("Galaxy");
            string strTemp1 = "Select Count(*) as count from crm_types " +
                               " where type_name = '" + strName + "'";

            m_DataAdapterODBC = new OdbcDataAdapter(strTemp1, m_Connection.oCon);
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);

            if (nName > 0 && nTypeID == 0) // for checking the existence the record
            {
                strResult = "Service Type already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else if (nName == 0 && nTypeID == 0) // for inserting the record
            {

                strTemp = "INSERT into crm_types (type_name,type_enabled,type_case_types,type_for)" +
                          " Values ('" + strName + "','" + strEnabled + "','" + strServiceType + "','S')";
            }

            else // for updating the existing record
            {
                strTemp = "UPDATE crm_types  " +
                          "SET  type_name='" + strName + "', type_enabled = '" + strEnabled + "'" +
                           ",type_case_types = '" + strServiceType + "'" +
                           " Where type_id = " + nTypeID + "";
            }

            m_CommandODBC = new OdbcCommand();
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);

            m_CommandODBC.ExecuteNonQuery();

            if (nTypeID == 0)
            {
                nResultCode = 0;
                strResult = "Service Type added successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else
            {
                nResultCode = 0;
                strResult = "Service Type updated successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            if (nResultCode == 2627)
                strResult = "City code already exists !";
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

    #region Log Error Messages
    void LogMessage(string szMessage, string szMethodName, string szMethodParams)
    {
        CreateLog objCreateLog = new CreateLog();

        try
        {
            szMessage = "MasterWS.cs - " + szMethodName +
                        "(" + szMethodParams + ") " + szMessage;

            objCreateLog.ErrorLog(szMessage);
        }
        catch (Exception ex)
        {
            string str = ex.Message;
        }
    }
    #endregion

    #region Fetch Roles List
    [WebMethod]
    /*** fetch Roles list ***/
    public DataSet wmGetRoles(string szUserRoleIds, string szSearchText)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT role_id,role_name,role_description as Description,role_enabled as Enabled " +
                        "FROM crm_roles WHERE role_id >0 ";

            if (szUserRoleIds != string.Empty)
                strTemp += " AND role_id IN (" + szUserRoleIds + " ) ";
            if (szSearchText != string.Empty)
                strTemp += " AND role_name like '%" + szSearchText + "%'";

            strTemp += " ORDER BY role_name";
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Main";
            nResultCode = 0;
            strResult = "Pass";

            if (m_DataSet.Tables["Main"].Rows.Count <= 0)
                strResult = "No Matching Record Found";
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

    #region Update Role BS [not done]
    [WebMethod]
    public DataSet wmRoleInsertUpdate(int nRoleId,
                                    string szRoleName,
                                    string szRoleDesc, string szEnabled)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Roles");

        try
        {
            m_Connection.OpenDB("Galaxy");

            string strTemp = "Select Count(*) as count from crm_roles " +
                                " where role_name = ?";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@role_name", OdbcType.VarChar).Value = szRoleName;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);
            if (nName > 0 ) // checking the existence the record
            {
                nResultCode = -1;
                strResult = "Name  already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            
            //---

                        m_Connection.BeginTransaction();
            if (nRoleId == 0)
                strTemp = "INSERT INTO crm_roles (role_name,role_description,role_enabled) values(?,?,'" + szEnabled + "')";
            else
                strTemp = "UPDATE crm_roles SET role_name=?,role_description=?,role_enabled='" + szEnabled + "' " +
                    "WHERE role_id=" + nRoleId;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.Parameters.Add("@role_name", OdbcType.VarChar).Value = szRoleName;
            m_CommandODBC.Parameters.Add("@role_desc", OdbcType.VarChar).Value = szRoleDesc;
            m_CommandODBC.ExecuteNonQuery();

            nResultCode = 0;
            strResult = "Record saved successfully !";
            m_Connection.Commit();
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_Connection.Rollback();
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            m_Connection.Rollback();
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion

    #region GetProductList
    [WebMethod]
    public DataSet GetProductList()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Product");
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT product_id," + m_Connection.DB_NULL + "(product_name,'')as product_name," +
                        m_Connection.DB_NULL + "(product_custom_attribute,'')as product_custom_attribute," +
                      "(case when product_enabled='y' then 'Yes' else 'No' end) as product_enabled " +
                      "FROM crm_products where parent_id =0";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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
    #region Load Sub Product
    [WebMethod]
    public DataSet LoadSubProduct(int ProductID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = strTemp = "SELECT product_id," + m_Connection.DB_NULL + "(product_name,'')as product_name," +
                       m_Connection.DB_NULL + "(product_custom_attribute,'')as product_custom_attribute," +
                      "(case when product_enabled='y' then 'Yes' else 'No' end) as product_enabled,parent_id " +
                      "FROM crm_products where parent_id =" + ProductID;

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "SubProduct";

            nResultCode = 0;
            strResult = "Pass";
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

    #region InsertUpdateForProduct
    [WebMethod]
    public DataSet InsertUpdateForProduct(long nProductID, string strName, string strAttribute, string strEnabled, int parent_id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            string strTemp = "Select Count(*) as count from crm_products " +
                                " where product_name = ?  and product_custom_attribute =? ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@productName", OdbcType.VarChar).Value = strName;
            m_CommandODBC.Parameters.Add("@product_custom_attribute", OdbcType.VarChar).Value = strAttribute;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);


           // if (nName > 0 && nProductID == 0) // checking the existence the record
            if (nName > 0)
            {
                nResultCode = -1;
                strResult = "Product or code already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else if (nProductID == 0) // for inserting the record
            {

                strTemp = "INSERT into crm_products (product_name,product_custom_attribute,product_enabled,parent_id)" +
                          " Values (?,?,'" + strEnabled + "'," + parent_id + ")";

                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@productName", OdbcType.VarChar).Value = strName;
                m_CommandODBC.Parameters.Add("@attribute", OdbcType.VarChar).Value = strAttribute;

            }

            else  // for updating the record
            {
                strTemp = "UPDATE crm_products  " +
                          "SET  product_name = ?, product_custom_attribute = ?, product_enabled = '" + strEnabled + "'" +
                           " ,parent_id =" + parent_id + " Where product_id = " + nProductID + "";

                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@productName", OdbcType.VarChar).Value = strName;
                m_CommandODBC.Parameters.Add("@attribute", OdbcType.VarChar).Value = strAttribute;
            }

            m_CommandODBC.ExecuteNonQuery();

            if (nProductID == 0)
            {
                nResultCode = 0;
                strResult = "Product Added successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            nResultCode = 0;
            strResult = "Product Updated successfully!";
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
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

    #region Loading Call Type
    [WebMethod]
    public DataSet LoadGeneralValueTypes()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT DISTINCT type FROM crm_general";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "GeneralType";

            nResultCode = 0;
            strResult = "Pass";
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

    #region Load Sub Category
    [WebMethod]
    public DataSet LoadGeneralValues(string Type)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("GeneralType");

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT id, type, code, name " +
                       "FROM crm_general Where type='" + Type + "' ";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Update or Insert the Call Type
    [WebMethod]
    public DataSet InsertUpdateForGeneral(long nValueID, string strType, string strName, string strCode)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
        }

        try
        {

            string strTemp = "Select Count(*) as count from crm_general " +
                                " where name = '" + strName + "'" +
                                " AND type = '" + strType + "'";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);


            if (nName > 0 && nValueID == 0) // checking the existence the record
            {
                nResultCode = -1;
                strResult = "General Value already exist in this type!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else if (nValueID == 0) // for inserting the record
            {

                strTemp = "INSERT into crm_general (type,name,code)" +
                          " Values (?,?,?)";

                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@Type", OdbcType.VarChar).Value = strType;
            }

            else  // for updating the record
            {
                strTemp = "UPDATE crm_general  " +
                          "SET  name = ?, code = ?" +
                           " Where id = " + nValueID;

                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);

            }
            m_CommandODBC.Parameters.Add("@Name", OdbcType.VarChar).Value = strName;
            m_CommandODBC.Parameters.Add("@Code", OdbcType.VarChar).Value = strCode;
            m_CommandODBC.ExecuteNonQuery();

            if (nValueID == 0)
            {
                nResultCode = 0;
                strResult = "Value Added successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            nResultCode = 0;
            strResult = "Value Updated successfully!";
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
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

    #region Load Table Columns
    [WebMethod]
    public DataSet LoadTableColumns(string strRelatedTo,string forlist)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT ('#' + UPPER(tabledef_column_header) + '#') as FieldName FROM crm_table_columns " +
                      "WHERE tabledef_for='" + strRelatedTo + "' and tabledef_column_visible = 'Y'";
            if (forlist == "CHT")
                strTemp += " and tabledef_column_type='C' ";
            strTemp += " order by tabledef_column_header";
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "TableColumns";

            nResultCode = 0;
            strResult = "Pass";
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

    #region GetRoleActivities
    [WebMethod]
    /*** fetch Role BS ***/
    public DataSet GetRoleActivities(int nRoleId, string strCategory)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            if (m_Connection.nDatabaseType == 2) //-- MySql
            {
                strTemp = "SELECT concat(cast(roledet_activity_id as varchar(10)),'|',cast(" + m_Connection.DB_NULL + "(roledet_permission_level,0)as varchar(10))) as ColumnValue," +
                        "concat(activity_name, ' - ', cast(roledet_activity_id as varchar(10))) as AcitivityName ";
            }
            else
            {
                strTemp = "SELECT(Convert(varchar(10), roledet_activity_id)+'|'+Convert(varchar(10)," + m_Connection.DB_NULL + "(roledet_permission_level,0))) as ColumnValue," +
                        "(activity_name + ' - ' + Convert(varchar(10), roledet_activity_id)) as AcitivityName ";
            }
            strTemp += "FROM crm_Role_Activity,crm_activity_master " +
                     "WHERE roledet_role_id=" + nRoleId +
                     "AND activity_id=roledet_activity_id ";
            if (strCategory != "All")
                strTemp += " AND " + m_Connection.DB_NULL + "(activity_description,'') = '" + strCategory + "' ";
            strTemp += " ORDER BY AcitivityName";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "AssignedActivity";

            if (m_Connection.nDatabaseType == 2) //-- MySql
            {
                strTemp = "SELECT concat(cast(activity_id as varchar(10)),'|62' as ColumnValue, ";
            }
            else
            {
                strTemp = "SELECT Convert(varchar(10), activity_id)+'|62' as ColumnValue, ";
            }
            strTemp += "activity_name as AcitivityName " +
                       "from crm_Activity_master " +
                       "where activity_id not in " +
                       "(select roledet_activity_id from crm_Role_Activity where roledet_role_id=" + nRoleId + ") ";
            if (strCategory != "All")
                strTemp += " AND " + m_Connection.DB_NULL + "(activity_description,'') = '" + strCategory + "' ";
            strTemp += "ORDER BY activity_name";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[1].TableName = "UnAssignedActivity";

            nResultCode = 0;
            strResult = "Pass";
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

    #region  Getting All Activity Category
    [WebMethod]
    public DataSet FetchActivityCategory()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        OdbcDataAdapter m_DataAdapterOdbc = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "select distinct activity_description from crm_Activity_master where activity_description is not null";

            strTemp += " ORDER BY activity_description";

            m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "ActivityCategory";

            nResultCode = 0;
            strResult = "Pass";
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

    #region Fetch Permission List
    [WebMethod]
    /*** fetch Permission List ***/
    public DataSet wmGetPermissionList()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "Select * from Permission_master";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataSet);

            nResultCode = 0;
            strResult = "Pass";
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

    #region  Insert Roles Activity
    [WebMethod]
    public DataSet InsertRolesActivity(int ActivityID, int RoleID, int Permission)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "INSERT INTO crm_role_activity (roledet_activity_id,roledet_role_id,roledet_permission_level) " +
                "VALUES(" + ActivityID.ToString() + "," + RoleID.ToString() + "," + Permission.ToString() + ")";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
            {
                nResultCode = 0;
                strResult = "Pass";
            }
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

    #region  Update Roles Activity
    [WebMethod]
    public DataSet UpdateRolesActivityPermission(int ActivityID, int RoleID, int Permission)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "UPDATE crm_role_activity SET roledet_permission_level = " + Permission +
                " WHERE roledet_activity_id = " + ActivityID + " AND roledet_role_id = " + RoleID;
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
            {
                nResultCode = 0;
                strResult = "Pass";
            }
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

    #region  Delete Roles Activity
    [WebMethod]
    public DataSet DeleteRolesActivity(string ActivityID, int RoleID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "DELETE FROM crm_role_activity WHERE roledet_role_id = " + RoleID.ToString() + " AND roledet_activity_id IN (" + ActivityID + ")";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
            {
                nResultCode = 0;
                strResult = "Pass";
            }
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

    #region GetCrmColumns
    [WebMethod]
    public DataSet GetCrmColumns()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("CrmColumns");
        try
        {
            m_Connection.OpenDB("Galaxy");
            //selecT object_code,object_name from crm_tables
            strTemp = "selecT object_code,object_name " +
                      "FROM crm_tables";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Getrdgtablecolumns
    [WebMethod]
    public DataSet Getrdgtablecolumns(string ObjectCode, string ObjectID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("CrmColumns");
        try
        {
            m_Connection.OpenDB("Galaxy");
            //selecT object_code,object_name from crm_tables
            strTemp = "selecT " +
                       m_Connection.DB_NULL + "(id,'')as id, " +
                       m_Connection.DB_NULL + "(caption,'')as caption, " +
                       m_Connection.DB_NULL + "(value,'')as value " +
                      "FROM crm_contact_details Where related_to_id=" + Convert.ToInt32(ObjectID) + " " +
                      " AND related_to='" + ObjectCode + "'";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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


    #region Getrdgtablecolumns

    [WebMethod]
    public DataSet Getrdgtablecolumns(string object_code)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("CrmColumns");
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "selecT tabledef_id, tabledef_column_header," + m_Connection.DB_NULL +
                       "(tabledef_column_visible,'')as tabledef_column_visible, " +
                       m_Connection.DB_NULL + "(tabledef_column_name,'')as tabledef_column_name, " +
                       m_Connection.DB_NULL + "(tabledef_operator,'')as tabledef_operator, " +
                       m_Connection.DB_NULL + "(tabledef_column_sequence,'')as tabledef_column_sequence " +
                      "FROM crm_table_columns Where tabledef_for='" + object_code + "' ORDER BY tabledef_column_name ASC";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region InsertUpdateForProduct
    [WebMethod]
    public DataSet InsertUpdateFORcrm_table_columns(string strcolumnheader, string strEnabled, string strcolumnname, string stredit_operator, string stredit_table_column_sequence, int ntabledef_id, string type, string strtabledef_for)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            if (type == "update")
            {
                string strTemp = "update crm_table_columns set tabledef_column_header=?, " +
                             "tabledef_column_visible=?," +
                             "tabledef_column_name=?," +
                             "tabledef_operator=?," +
                             "tabledef_column_sequence=?" +
                             " where tabledef_id =" + ntabledef_id + "";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@tabledef_column_header", OdbcType.VarChar).Value = strcolumnheader;
                m_CommandODBC.Parameters.Add("@tabledef_column_visible", OdbcType.VarChar).Value = strEnabled;
                m_CommandODBC.Parameters.Add("@tabledef_column_name", OdbcType.VarChar).Value = strcolumnname;
                m_CommandODBC.Parameters.Add("@tabledef_operator", OdbcType.VarChar).Value = stredit_operator;
                m_CommandODBC.Parameters.Add("@tabledef_column_sequence", OdbcType.VarChar).Value = stredit_table_column_sequence;

                m_CommandODBC.ExecuteNonQuery();
                nResultCode = 0;
                strResult = "Column Updated successfully!";
            }
            else
            {
                string strTemp = "INSERT INTO  crm_table_columns (tabledef_column_header,tabledef_column_visible,tabledef_column_name,tabledef_operator,tabledef_column_sequence,tabledef_for)" +
                                 "values(?,?,?,?,?,?)";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@tabledef_column_header", OdbcType.VarChar).Value = strcolumnheader;
                m_CommandODBC.Parameters.Add("@tabledef_column_visible", OdbcType.VarChar).Value = strEnabled;
                m_CommandODBC.Parameters.Add("@tabledef_column_name", OdbcType.VarChar).Value = strcolumnname;
                m_CommandODBC.Parameters.Add("@tabledef_operator", OdbcType.VarChar).Value = stredit_operator;
                m_CommandODBC.Parameters.Add("@tabledef_column_sequence", OdbcType.VarChar).Value = stredit_table_column_sequence;
                m_CommandODBC.Parameters.Add("@tabledef_for", OdbcType.VarChar).Value = strtabledef_for;

                m_CommandODBC.ExecuteNonQuery();
                nResultCode = 0;
                strResult = "Column Inserted successfully!";
            }
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
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

    #region GetCrmTable
    [WebMethod]
    public DataSet GetCrmTable()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("CrmColumns");
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "selecT " +
                       m_Connection.DB_NULL + "(object_code,'')as object_code, " +
                       m_Connection.DB_NULL + "(object_name,'')as object_name, " +
                       m_Connection.DB_NULL + "(object_image_id,'')as object_image_id, " +
                       m_Connection.DB_NULL + "(object_table_name,'')as object_table_name, " +
                       m_Connection.DB_NULL + "(object_permission_activity_id,'')as object_permission_activity_id, " +
                       m_Connection.DB_NULL + "(object_transaction_number_format,'')as object_transaction_number_format, " +
                       m_Connection.DB_NULL + "(object_transaction_series_month,'')as object_transaction_series_month, " +
                       m_Connection.DB_NULL + "(object_transaction_series_year,'')as object_transaction_series_year, " +
                       m_Connection.DB_NULL + "(object_owner_assignment,'')as object_owner_assignment, " +
                       m_Connection.DB_NULL + "(object_custom_tab_field,'')as object_custom_tab_field " +
                      "FROM crm_tables ";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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


    #region InsertUpdateForCrmTables
    [WebMethod]
    public DataSet InsertUpdateForCrmTables(string strobjectcode, string strobjectname, string strobjectimageid, string strobjectablename, string strpermissionactivityid, string strtransactionnumberformat, string strcustomfield, string strmonthseriesenabled, string stryearseriesenabled, string strownerassingmentenabled, string type)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            if (type == "update")
            {
                string strTemp = "update crm_tables set " +
                             "object_code=?," +
                             "object_name=?," +
                             "object_image_id=?," +
                             "object_table_name=?," +
                             "object_permission_activity_id=?," +
                             "object_transaction_number_format=?," +
                             "object_transaction_series_month=?," +
                             "object_transaction_series_year=?," +
                             "object_owner_assignment=?," +
                             "object_custom_tab_field=?" +
                             " where object_table_name ='" + strobjectablename + "'";

                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@object_code", OdbcType.VarChar).Value = strobjectcode;
                m_CommandODBC.Parameters.Add("@object_name", OdbcType.VarChar).Value = strobjectname;
                m_CommandODBC.Parameters.Add("@object_image_id", OdbcType.Int).Value = Convert.ToInt32(strobjectimageid);
                m_CommandODBC.Parameters.Add("@object_table_name", OdbcType.VarChar).Value = strobjectablename;
                m_CommandODBC.Parameters.Add("@object_permission_activity_id", OdbcType.Int).Value = Convert.ToInt32(strpermissionactivityid);
                m_CommandODBC.Parameters.Add("@object_transaction_number_format", OdbcType.VarChar).Value = strtransactionnumberformat;
                m_CommandODBC.Parameters.Add("@object_transaction_series_month", OdbcType.VarChar).Value = strmonthseriesenabled;
                m_CommandODBC.Parameters.Add("@object_transaction_series_year", OdbcType.VarChar).Value = stryearseriesenabled;
                m_CommandODBC.Parameters.Add("@object_owner_assignment", OdbcType.VarChar).Value = strownerassingmentenabled;
                m_CommandODBC.Parameters.Add("@object_custom_tab_field", OdbcType.VarChar).Value = strcustomfield;

                m_CommandODBC.ExecuteNonQuery();
                nResultCode = 0;
                strResult = "Column Updated successfully!";
            }
            else
            {
                string strTemp = "INSERT INTO  crm_tables (object_code,object_name,object_image_id," +
                                 "object_table_name,object_permission_activity_id,object_transaction_number_format," +
                                 "object_transaction_series_month,object_transaction_series_year,object_owner_assignment,object_custom_tab_field)" +
                                 "values(?,?,?,?,?,?,?,?,?,?)";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@object_code", OdbcType.VarChar).Value = strobjectcode;
                m_CommandODBC.Parameters.Add("@object_name", OdbcType.VarChar).Value = strobjectname;
                m_CommandODBC.Parameters.Add("@object_image_id", OdbcType.Int).Value = Convert.ToInt32(strobjectimageid);
                m_CommandODBC.Parameters.Add("@object_table_name", OdbcType.VarChar).Value = strobjectablename;
                m_CommandODBC.Parameters.Add("@object_permission_activity_id", OdbcType.Int).Value = Convert.ToInt32(strpermissionactivityid);
                m_CommandODBC.Parameters.Add("@object_transaction_number_format", OdbcType.VarChar).Value = strtransactionnumberformat;
                m_CommandODBC.Parameters.Add("@object_transaction_series_month", OdbcType.VarChar).Value = strmonthseriesenabled;
                m_CommandODBC.Parameters.Add("@object_transaction_series_year", OdbcType.VarChar).Value = stryearseriesenabled;
                m_CommandODBC.Parameters.Add("@object_owner_assignment", OdbcType.VarChar).Value = strownerassingmentenabled;
                m_CommandODBC.Parameters.Add("@object_custom_tab_field", OdbcType.VarChar).Value = strcustomfield;
                m_CommandODBC.ExecuteNonQuery();
                nResultCode = 0;
                strResult = "Column Inserted successfully!";
            }
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
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


    #region GetOperatorType
    [WebMethod]
    public DataSet GetOperatorType()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("CrmColumns");
        try
        {
            m_Connection.OpenDB("Galaxy");
            //selecT object_code,object_name from crm_tables
            strTemp = "selecT code,id  from crm_general where type ='OPERATOR' ";
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region  cases for time sheet
    [WebMethod]
    public DataSet Timesheetcases(int id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            OdbcCommand m_command = new OdbcCommand("EXEC usp_TimeSheetCases ?", m_Connection.oCon);
            m_command.CommandType = CommandType.StoredProcedure;
            m_command.Parameters.Add("@id", OdbcType.Int).Value = id;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_command);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "timesheetcases";

            nResultCode = 0;
            strResult = "Pass";
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

    #region  cases for time sheet
    [WebMethod]
    public DataSet TimesheetTask(int id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            OdbcCommand m_command = new OdbcCommand("EXEC usp_TimeSheetTask ?", m_Connection.oCon);
            m_command.CommandType = CommandType.StoredProcedure;
            m_command.Parameters.Add("@id", OdbcType.Int).Value = id;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_command);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "timesheetTask";

            nResultCode = 0;
            strResult = "Pass";
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

    #region  Geting Case Releted To task for time sheet
    [WebMethod]
    public DataSet TimesheetTaskReletedToCase(int id, string CaseNumber)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Timesheet");

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "Select * FROM [crm_tasks] where [Useable]='Y' AND task_status_id <> 5 AND [related_to]='CAS'  and related_to_id ='" + CaseNumber + "'";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region GetTimesheet
    [WebMethod]
    public DataSet GetTimesheet(int id, string oDate)
    //, int owner_id
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Timesheet");

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT [id],[related_to_id_case],[related_to_name_case],[related_to_id_task],[related_to_name_task],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,CONVERT(varchar , Time_Entry_Date,106)as Time_Entry_Date,[TimeDiffernce],[SheetType],[Remarks],[Approval_Status] as Reject_Status ,customer_name,Destination_customer_name as desti_customer_name,ODSource_Point,Distance,ISNULL(CONVERT(varchar(15),Approval_Date,106),'N/A')as Approval_Date,ISNULL(CONVERT(varchar(15),Approval_Date_By_HR,106),'N/A')as Approval_Date " +
                      "FROM [crm_timesheet] where [Useable]='Y' and Approval_Status = 'PENDING' AND convert(varchar, Time_Entry_Date, 106) = '" + oDate + "' AND [created_by]=" + id;

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Appored for Time Sheet
    [WebMethod]
    public DataSet GetTimesheetdata(string type, int id, string strStatus)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Timesheet");
        if (strStatus == string.Empty)
            strStatus = "PENDING";
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = " SELECT dbo.GetContact(created_by)as Name,[id],[related_to_id_case],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[TimeDiffernce],[Approval_Status],[related_to_name_case],[related_to_id_task],[related_to_name_task],[Time_start],[Time_end],[SheetType],[Remarks],CONVERT(varchar ,Time_Entry_Date,106)as Time_Entry_Date,Approval_Status_By_HR,ApprovalRemark,Reject_Remark_by_hr,customer_name,ODSource_Point,Distance FROM [crm_timesheet] where Sheettype = 'OD' AND  Approval_Status = '" + strStatus + "'  AND created_by =" + id;

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region update time sheet approval
    [WebMethod]
    public DataSet updateTimesheet(int id, string RejectlRemark, string RejectDate)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Timesheet");

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "UPDATE crm_timesheet set Approval_Status = 'REJECTED', " + "ApprovalRemark = '" + RejectlRemark + "' ,Approval_Date = '" + RejectDate + "'" + " where SheetType = 'OD' AND id ='" + id + "'";
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Data Updated successfully !";

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

    // Total timesheet time new add by ashish 
    #region GetTimesheet Total Time
    [WebMethod]
    public DataSet GetTotalTimesheetTime(int id, string oDate)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("TotalTimesheetTime");
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT convert(varchar(8), dateadd(second, SUM(DATEDIFF(SECOND, [Time_start], [Time_end])), 0),  108) as TimeTotal from crm_timesheet where convert(varchar, Time_Entry_Date, 106) = '" + oDate + "' AND [created_by] =" + id;


            OdbcCommand m_Command = new OdbcCommand(strTemp, m_Connection.oCon);
            strResult = Convert.ToString(m_Command.ExecuteScalar());
            nResultCode = 0;
            strResult = (strResult == "" ? "00:00:00" : strResult);
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

    //  #region Search TimeSheet Data According to Employee new add by ashish 
    public DataSet GetTimesheetselectdate(string selectdate, int id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Timesheet");

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT [id],[related_to_id_case],[related_to_name_case],[related_to_id_task],[related_to_name_task],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[Time_Entry_Date],[TimeDiffernce],[SheetType],[Remarks],customer_name,ODSource_Point,Distance" +
                      "FROM [crm_timesheet] where [Useable]='Y' and [created_by]=" + id +
                      " and  convert(datetime,convert(varchar(10),[Time_Entry_Date],103),103)=convert(datetime,convert(varchar(12),'" + selectdate + "',109))";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Search Employee TimeSheet
    [WebMethod]
    public DataSet GetSearchEmployeeTimeSheet(int id, string EmployeeID, string Status, string DateFrom, string DateTo)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Timesheet");
        try
        {
            m_Connection.OpenDB("Galaxy");
            if (EmployeeID == "0" && Status.ToString() == "All")
            {
                strTemp = "Select dbo.GetContact(created_by)as Name, [id],[related_to_id_case],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[TimeDiffernce],Approval_Status,[related_to_name_case],[related_to_id_task],[related_to_name_task],[Time_start],[Time_end],[SheetType],[Remarks],CONVERT(varchar ,Time_Entry_Date,106)as Time_Entry_Date,Approval_Status,Approval_Status_By_HR,(CASE WHEN ISNULL(ApprovalRemark, '') = '' THEN 'N/A' ELSE ApprovalRemark END) AS ApprovalRemark ,(CASE WHEN ISNULL(Reject_Remark_by_hr, '') = '' THEN 'N/A' ELSE Reject_Remark_by_hr END) AS ApprovalRemark,customer_name,ODSource_Point,Distance From crm_timesheet  where Sheettype = 'OD'and Approval_Status IN ('PENDING','APPROVED','REJECTED')and Time_Entry_Date BETWEEN '" + DateFrom + "' and '" + DateTo + "' and reporttoapproved_id = '" + id + "'ORDER BY Time_Entry_Date DESC";
            }
            else if (EmployeeID == "0" && (Status == "PENDING" || Status == "APPROVED" || Status == "REJECTED"))
            {
                strTemp = "Select dbo.GetContact(created_by)as Name,[id],[related_to_id_case],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[TimeDiffernce],Approval_Status,[related_to_name_case],[related_to_id_task],[related_to_name_task],[Time_start],[Time_end],[SheetType],[Remarks],CONVERT(varchar ,Time_Entry_Date,106)as Time_Entry_Date,Approval_Status,Approval_Status_By_HR,(CASE WHEN ISNULL(ApprovalRemark, '') = '' THEN 'N/A' ELSE ApprovalRemark END) AS ApprovalRemark ,(CASE WHEN ISNULL(Reject_Remark_by_hr, '') = '' THEN 'N/A' ELSE Reject_Remark_by_hr END) AS ApprovalRemark,customer_name,ODSource_Point,Distance From crm_timesheet  where Sheettype = 'OD'and Approval_Status ='" + Status.ToString() + "' and Time_Entry_Date BETWEEN '" + DateFrom + "' and '" + DateTo + "' and reporttoapproved_id = '" + id + "'ORDER BY Time_Entry_Date DESC";
            }
            else if (EmployeeID != "0" && Status.ToString() == "All")
            {
                strTemp = "Select dbo.GetContact(created_by)as Name, [id],[related_to_id_case],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[TimeDiffernce],Approval_Status,[related_to_name_case],[related_to_id_task],[related_to_name_task],[Time_start],[Time_end],[SheetType],[Remarks],CONVERT(varchar ,Time_Entry_Date,106)as Time_Entry_Date,Approval_Status,Approval_Status_By_HR,(CASE WHEN ISNULL(ApprovalRemark, '') = '' THEN 'N/A' ELSE ApprovalRemark END) AS ApprovalRemark ,(CASE WHEN ISNULL(Reject_Remark_by_hr, '') = '' THEN 'N/A' ELSE Reject_Remark_by_hr END) AS ApprovalRemark,customer_name,ODSource_Point,Distance From crm_timesheet  where Sheettype = 'OD'and created_by='" + EmployeeID + "' AND Approval_Status IN ('PENDING','APPROVED','REJECTED')and Time_Entry_Date BETWEEN '" + DateFrom + "' and '" + DateTo + "' and reporttoapproved_id = '" + id + "'ORDER BY Time_Entry_Date DESC";
            }
            else if (EmployeeID != "0" && (Status == "PENDING" || Status == "APPROVED" || Status == "REJECTED"))
            {
                strTemp = "Select dbo.GetContact(created_by)as Name, [id],[related_to_id_case],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[TimeDiffernce],Approval_Status,[related_to_name_case],[related_to_id_task],[related_to_name_task],[Time_start],[Time_end],[SheetType],[Remarks],CONVERT(varchar ,Time_Entry_Date,106)as Time_Entry_Date,Approval_Status,Approval_Status_By_HR,(CASE WHEN ISNULL(ApprovalRemark, '') = '' THEN 'N/A' ELSE ApprovalRemark END) AS ApprovalRemark ,(CASE WHEN ISNULL(Reject_Remark_by_hr, '') = '' THEN 'N/A' ELSE Reject_Remark_by_hr END) AS ApprovalRemark,customer_name,ODSource_Point,Distance From crm_timesheet  where Sheettype = 'OD'and created_by='" + EmployeeID + "' AND Approval_Status ='" + Status.ToString() + "'and Time_Entry_Date BETWEEN '" + DateFrom + "' and '" + DateTo + "' and reporttoapproved_id = '" + id + "'ORDER BY Time_Entry_Date DESC";
            }
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Multiple Select Update time sheet approval
    [WebMethod]
    public DataSet MultipleUpdateTimesheet(string id, string ApprovalDate)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Timesheet");

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "UPDATE crm_timesheet set Approval_Status = 'APPROVED'," +
                        "Approval_Date = '" + ApprovalDate + "'" +
                         " where CONVERT (varchar ,id) IN (" + id + ")";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Data Updated successfully !";

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

    #region  HR Approval Update TimeSheet
    [WebMethod]
    public DataSet UpdateTimesheetByHR(int id, string RejectlRemark, string RejectDate)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Timesheet");

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "UPDATE crm_timesheet set Approval_Status_By_HR = 'REJECTED', " + "ApprovalRemark = '" + RejectlRemark + "'," + "Approval_Date_By_HR = '" + RejectDate + "'" + " where SheetType = 'OD' AND id ='" + id + "'";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Data Updated successfully !";

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


    #region Get Appored byHR Time Sheet
    [WebMethod]
    public DataSet GetTimesheetdataByHR(string Status, int id, string strStatus)
    //, int owner_id
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Timesheet");

        if (strStatus == string.Empty)
            strStatus = "PENDING";
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = " SELECT  dbo.GetContact(created_by)as Name,dbo.GetContact(reporttoapproved_id)as ReportingName,[id],[related_to_id_case],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[TimeDiffernce],[Approval_Status],[related_to_name_case],[related_to_id_task],[related_to_name_task],[Time_start],[Time_end],[SheetType],[Remarks],CONVERT(varchar ,Time_Entry_Date,106)as Time_Entry_Date,Approval_Status_By_HR,CONVERT(varchar ,Approval_Date,106)as Approval_Date,(CASE WHEN ISNULL(Reject_Remark_by_hr, '') = '' THEN 'N/A' ELSE Reject_Remark_by_hr END) AS Reject_Remark_by_hr,customer_name,ODSource_Point,Distance FROM [crm_timesheet] where Sheettype = 'OD' AND Approval_Status = 'APPROVED' AND Approval_Status_By_HR = '" + strStatus + "' AND  Approval_Status_By_HR = 'PENDING' AND created_by =" + id;

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Search Employee by HR TimeSheet
    [WebMethod]
    public DataSet GetSearchEmployeeByHRTimeSheet(string EmployeeID, string Status, string DateFrom, string DateTo)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Timesheet");
        try
        {
            m_Connection.OpenDB("Galaxy");

            if (EmployeeID == "0" && Status.ToString() == "All")
            {
                strTemp = "SELECT dbo.GetContact(created_by)as Name,dbo.GetContact(reporttoapproved_id)as ReportingName,[id],[related_to_id_case],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[TimeDiffernce],[Approval_Status],[related_to_name_case],[related_to_id_task],[related_to_name_task],[Time_start],[Time_end],[SheetType],[Remarks],Approval_Status_By_HR,CONVERT(varchar ,Time_Entry_Date,106)as Time_Entry_Date,Approval_Status_By_HR,(CASE WHEN ISNULL(Reject_Remark_by_hr, '') = '' THEN 'N/A' ELSE Reject_Remark_by_hr END) AS Reject_Remark_by_hr,customer_name,ODSource_Point,Distance,CONVERT(varchar ,Approval_Date,106)as Approval_Date FROM [crm_timesheet] where Sheettype = 'OD' AND Time_Entry_Date BETWEEN '" + DateFrom + "' and '" + DateTo + "' AND  Approval_Status_By_HR IN ('PENDING','APPROVED','REJECTED') ORDER BY Time_Entry_Date DESC"; //and created_by = '" + id + "'";
            }

            else if (EmployeeID == "0" && (Status == "PENDING" || Status == "APPROVED" || Status == "REJECTED"))
            {
                strTemp = "SELECT dbo.GetContact(created_by)as Name,dbo.GetContact(reporttoapproved_id)as ReportingName,[id],[related_to_id_case],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[TimeDiffernce],[Approval_Status],[related_to_name_case],[related_to_id_task],[related_to_name_task],[Time_start],[Time_end],[SheetType],[Remarks],Approval_Status_By_HR,CONVERT(varchar ,Time_Entry_Date,106)as Time_Entry_Date,Approval_Status_By_HR,(CASE WHEN ISNULL(Reject_Remark_by_hr, '') = '' THEN 'N/A' ELSE Reject_Remark_by_hr END) AS Reject_Remark_by_hr,customer_name,ODSource_Point,Distance,CONVERT(varchar ,Approval_Date,106)as Approval_Date FROM [crm_timesheet] where Sheettype = 'OD' and Approval_Status_By_HR ='" + Status + "' AND Time_Entry_Date BETWEEN '" + DateFrom + "' and '" + DateTo + "' ORDER BY Time_Entry_Date DESC"; //";    
            }

            //else if (EmployeeID == "0" && (Status == "PENDING" || Status == "APPROVED" || Status == "REJECTED"))
            //{
            //    strTemp = "SELECT dbo.GetContact(created_by)as Name,dbo.GetContact(reporttoapproved_id)as ReportingName,[id],[related_to_id_case],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[TimeDiffernce],[Approval_Status],[related_to_name_case],[related_to_id_task],[related_to_name_task],[Time_start],[Time_end],[SheetType],[Remarks],Approval_Status_By_HR,CONVERT(varchar ,Time_Entry_Date,106)as Time_Entry_Date,Approval_Status_By_HR,(CASE WHEN ISNULL(Reject_Remark_by_hr, '') = '' THEN 'N/A' ELSE Reject_Remark_by_hr END) AS Reject_Remark_by_hr,customer_name,ODSource_Point,Distance,CONVERT(varchar ,Approval_Date,106)as Approval_Date FROM [crm_timesheet] where Sheettype = 'OD' and Approval_Status_By_HR ='" + Status + "' or Approval_Status_By_HR IN ('PENDING','APPROVED','REJECTED') AND Time_Entry_Date BETWEEN '" + DateFrom + "' and '" + DateTo + "'and created_by = '" + EmployeeID + "' ORDER BY Time_Entry_Date DESC"; //";    
            //}
            else if (EmployeeID == "0" && (Status == "PENDING" || Status == "APPROVED" || Status == "REJECTED"))
            {
                strTemp = "SELECT dbo.GetContact(created_by)as Name,dbo.GetContact(reporttoapproved_id)as ReportingName,[id],[related_to_id_case],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[TimeDiffernce],[Approval_Status],[related_to_name_case],[related_to_id_task],[related_to_name_task],[Time_start],[Time_end],[SheetType],[Remarks],Approval_Status_By_HR,CONVERT(varchar ,Time_Entry_Date,106)as Time_Entry_Date,Approval_Status_By_HR,(CASE WHEN ISNULL(Reject_Remark_by_hr, '') = '' THEN 'N/A' ELSE Reject_Remark_by_hr END) AS Reject_Remark_by_hr,customer_name,ODSource_Point,Distance,CONVERT(varchar ,Approval_Date,106)as Approval_Date FROM [crm_timesheet] where Sheettype = 'OD' and Approval_Status_By_HR IN ('PENDING','APPROVED','REJECTED') AND Time_Entry_Date BETWEEN '" + DateFrom + "' and '" + DateTo + "'and created_by = '" + EmployeeID + "' ORDER BY Time_Entry_Date DESC"; //";    
            }
            else if (EmployeeID != "0" && (Status == "All"))
            {
                strTemp = "SELECT dbo.GetContact(created_by)as Name,dbo.GetContact(reporttoapproved_id)as ReportingName,[id],[related_to_id_case],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[TimeDiffernce],[Approval_Status],[related_to_name_case],[related_to_id_task],[related_to_name_task],[Time_start],[Time_end],[SheetType],[Remarks],Approval_Status_By_HR,CONVERT(varchar ,Time_Entry_Date,106)as Time_Entry_Date,Approval_Status_By_HR,(CASE WHEN ISNULL(Reject_Remark_by_hr, '') = '' THEN 'N/A' ELSE Reject_Remark_by_hr END) AS Reject_Remark_by_hr,customer_name,ODSource_Point,Distance,CONVERT(varchar ,Approval_Date,106)as Approval_Date FROM [crm_timesheet] where Sheettype = 'OD' and Approval_Status_By_HR IN ('PENDING','APPROVED','REJECTED') AND Time_Entry_Date BETWEEN '" + DateFrom + "' and '" + DateTo + "' AND created_by = '" + EmployeeID + "' ORDER BY Time_Entry_Date DESC";
            }
            else if (EmployeeID != "0" && (Status == "PENDING" || Status == "APPROVED" || Status == "REJECTED"))
            {
                strTemp = "SELECT dbo.GetContact(created_by)as Name,dbo.GetContact(reporttoapproved_id)as ReportingName,[id],[related_to_id_case],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[TimeDiffernce],[Approval_Status],[related_to_name_case],[related_to_id_task],[related_to_name_task],[Time_start],[Time_end],[SheetType],[Remarks],Approval_Status_By_HR,CONVERT(varchar ,Time_Entry_Date,106)as Time_Entry_Date,Approval_Status_By_HR,(CASE WHEN ISNULL(Reject_Remark_by_hr, '') = '' THEN 'N/A' ELSE Reject_Remark_by_hr END) AS Reject_Remark_by_hr,customer_name,ODSource_Point,Distance,CONVERT(varchar ,Approval_Date,106)as Approval_Date FROM [crm_timesheet] where Sheettype = 'OD' and Approval_Status_By_HR ='" + Status + "' AND Time_Entry_Date BETWEEN '" + DateFrom + "' and '" + DateTo + "'and created_by = '" + EmployeeID + "' ORDER BY Time_Entry_Date DESC"; //";    
            }

            //    strTemp = "SELECT dbo.GetContact(created_by)as Name,dbo.GetContact(reporttoapproved_id)as ReportingName,[id],[related_to_id_case],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[TimeDiffernce],Approval_Status,[related_to_name_case],[related_to_id_task],[related_to_name_task],[Time_start],[Time_end],[SheetType],[Remarks],CONVERT(varchar ,Time_Entry_Date,106)as Time_Entry_Date,Approval_Status_By_HR,(CASE WHEN ISNULL(Reject_Remark_by_hr, '') = '' THEN 'N/A' ELSE Reject_Remark_by_hr END) AS Reject_Remark_by_hr,customer_name,ODSource_Point,Distance From crm_timesheet  where  Approval_Status = 'APPROVED' AND SheetType ='OD'  AND Approval_Status = 'APPROVED' and Approval_Status_By_HR ='" + Status + "' and Time_Entry_Date BETWEEN '" + DateFrom + "' and '" + DateTo + "' and created_by ='" + EmployeeID + "'ORDER BY Time_Entry_Date DESC";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get  GetSearch All Employee TimeSheet
    [WebMethod]
    public DataSet GetAllEmployeeByHRTimeSheet(string Status)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Timesheet");

        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT dbo.GetContact(created_by)as Name,dbo.GetContact(reporttoapproved_id)as ReportingName,[id],[related_to_id_case],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,[TimeDiffernce],[Approval_Status],[related_to_name_case],[related_to_id_task],[related_to_name_task],[Time_start],[Time_end],[SheetType],[Remarks],Approval_Status_By_HR,CONVERT(varchar ,Time_Entry_Date,106)as Time_Entry_Date,Approval_Status_By_HR FROM [crm_timesheet] where Approval_Status = 'APPROVED' AND Sheettype = 'OD' AND hrapproved = 'N' and  Approval_Status_By_HR='" + Status + "'"; //and created_by = '" + id + "'";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Multiple Select Update time sheet approval BY HR
    [WebMethod]
    public DataSet MultipleUpdateTimesheetbyHR(string id, string ApprovalDate)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Timesheet");

        try
        {
            m_Connection.OpenDB("Galaxy");


            strTemp = "UPDATE crm_timesheet set [reporttoapproved] ='Y', Approval_Status_By_HR = 'APPROVED', " +
                        "Approval_Date_By_HR = '" + ApprovalDate + "'" +
                         " where CONVERT (varchar ,id) IN (" + id + ")";
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Data Updated successfully !";

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


    #region Insert TimeSheet Data
    [WebMethod]
    public DataSet InsertTimeSheetData(int UserID, string hrapproved_id, string TimeFrom, string TimeTo, string TimeDiffernce, string CaseNumber, string TaskNumber, string Remarks, string TimeSheetType, string Status, string TimeEntryDate, string Customar, string SourcePoint, string distance, string DistinationAccountName)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        if ((validateOd(UserID, TimeFrom).ToString()) == "No")
        {
            try
            {
                this.m_Connection.OpenDB("Galaxy");

                string strTemp = "INSERT INTO  crm_timesheet (reporttoapproved_id,created_by,hrapproved_id,[Time_start],[Time_end], [TimeDiffernce],related_to_name_case,related_to_name_task,Remarks,[SheetType],Approval_Status,Time_Entry_Date,created_date,modified_by,Useable,Approval_Status_By_HR,customer_name,ODSource_Point,Distance,Destination_customer_name)" +
                  "values(dbo.getreportingto(" + UserID + "),?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);

                m_CommandODBC.Parameters.Add("@created_by", OdbcType.Int).Value = UserID;
                //m_CommandODBC.Parameters.Add("@reporttoapproved", OdbcType.VarChar).Value = "N";
                m_CommandODBC.Parameters.Add("@hrapproved_id", OdbcType.VarChar).Value = hrapproved_id;
                // m_CommandODBC.Parameters.Add("@hrapproved", OdbcType.VarChar).Value = "N";
                m_CommandODBC.Parameters.Add("@Time_start", OdbcType.VarChar).Value = TimeFrom;
                m_CommandODBC.Parameters.Add("@Time_end", OdbcType.VarChar).Value = TimeTo;
                m_CommandODBC.Parameters.Add("@TimeDiffernce", OdbcType.VarChar).Value = TimeDiffernce;
                m_CommandODBC.Parameters.Add("@related_to_name_case", OdbcType.VarChar).Value = CaseNumber;
                m_CommandODBC.Parameters.Add("@related_to_name_task", OdbcType.VarChar).Value = TaskNumber;
                m_CommandODBC.Parameters.Add("@Remarks", OdbcType.VarChar).Value = Remarks;
                m_CommandODBC.Parameters.Add("@Sheet_type", OdbcType.VarChar).Value = TimeSheetType;
                m_CommandODBC.Parameters.Add("@Approval_Status", OdbcType.VarChar).Value = "PENDING";
                m_CommandODBC.Parameters.Add("@Time_Entry_Date", OdbcType.VarChar).Value = TimeEntryDate;

                m_CommandODBC.Parameters.Add("@created_date", OdbcType.VarChar).Value = DateTime.Now.ToString();
                m_CommandODBC.Parameters.Add("@modified_by", OdbcType.VarChar).Value = UserID;
                m_CommandODBC.Parameters.Add("@Useable", OdbcType.VarChar).Value = "Y";
                m_CommandODBC.Parameters.Add("@Approval_Status_By_HR", OdbcType.VarChar).Value = "PENDING";

                m_CommandODBC.Parameters.Add("@customer_name", OdbcType.VarChar).Value = Customar;
                m_CommandODBC.Parameters.Add("@ODSource_Point", OdbcType.VarChar).Value = SourcePoint;
                m_CommandODBC.Parameters.Add("@Distance", OdbcType.VarChar).Value = distance;
                m_CommandODBC.Parameters.Add("@Destination_customer_name", OdbcType.VarChar).Value = DistinationAccountName;

                m_CommandODBC.ExecuteNonQuery();
                nResultCode = 0;

                strResult = "Data Inserted successfully!";

                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
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
        }
        else
        {
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "SaveSuccess", "javascript:OnSaveSuccess('" + CreateNew + "', 'OD Already Exits !!');", true);

        }
        return m_DataSet;
    }

    public string validateOd(int userid, string oddate)
    {
        string validentry = "No";

        DataTable m_DataTable = new DataTable();
        try
        {
            this.m_Connection.OpenDB("Galaxy");
            string strTemp = "select * from crm_timesheet  where created_by=" + userid + " and   '" + oddate + "' between Time_start and Time_end";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);

            if (m_DataTable.Rows.Count > 0)
            {
                validentry = "Yes";
            }
        }
        catch (Exception ex)
        {

        }
        finally
        {
            m_Connection.CloseDB();
        }
        return validentry;
    }

    #endregion


    #region Delete Time Sheet Using ID
    [WebMethod]
    public DataSet DeleteTimesheet(int id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Timesheet");

        try
        {
            m_Connection.OpenDB("Galaxy");


            strTemp = "DELETE From  crm_timesheet" +
                                " where id =" + id + "";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Data Delete Successfully !";

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

    #region update time sheet approval by HR
    [WebMethod]
    public DataSet updateTimesheetByHR(int id, string RejectlRemark, string RejectDate)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Timesheet");
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "UPDATE crm_timesheet set Approval_Status_By_HR = 'REJECTED', " + "Reject_Remark_by_hr = '" + RejectlRemark + "'," + "Approval_Date = '" + RejectDate + "'" + " where SheetType = 'OD' AND id ='" + id + "'";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Data Updated successfully !";
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

    #region Get Search TimeSheet Data
    [WebMethod]
    public DataSet SearchTimeSheetData(int id, string SheetType, string TimeStatus, string DateFrom, string DateTo)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("TimeSheetUserData");

        //if (TimeStatus == string.Empty)
        //    TimeStatus = "PENDING";
        try
        {
            m_Connection.OpenDB("Galaxy");

            if (TimeStatus == "REJECTED BY HR")
            {
                strTemp = "SELECT id,related_to_id_case,[related_to_name_case],[related_to_id_task],[related_to_name_task],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,CONVERT(varchar , Time_Entry_Date,106)as Time_Entry_Date ,[TimeDiffernce],[SheetType],[Remarks],Approval_Status_By_HR as Reject_Status,Reject_Remark_by_hr as RejectRemark,customer_name,Destination_customer_name as desti_customer_name,ODSource_Point,Distance,ISNULL(CONVERT(varchar(15),Approval_Date,106),'N/A')as Approval_Date,ISNULL(CONVERT(varchar(15),Approval_Date_By_HR,106),'N/A')as Approval_Date FROM [crm_timesheet] where [Useable]='Y' AND Approval_Status_By_HR = 'REJECTED' and SheetType = '" + SheetType + "'  and Time_Entry_Date BETWEEN '" + DateFrom + "' and '" + DateTo + "'  and created_by = '" + id + "'ORDER BY Time_Entry_Date DESC";
            }

            else if (TimeStatus == "APPROVED BY HR")
            {
                strTemp = "SELECT id,related_to_id_case,[related_to_name_case],[related_to_id_task],[related_to_name_task],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,CONVERT(varchar , Time_Entry_Date,106)as Time_Entry_Date,[TimeDiffernce],[SheetType],[Remarks], ApprovalRemark as RejectRemark,customer_name,Destination_customer_name as desti_customer_name,ODSource_Point,Distance,ISNULL(CONVERT(varchar(15),Approval_Date,106),'N/A')as Approval_Date,ISNULL(CONVERT(varchar(15),Approval_Date_By_HR,106),'N/A')as Approval_Date,Approval_Status_By_HR AS Reject_Status FROM [crm_timesheet] where [Useable]='Y' and SheetType = '" + SheetType + "' AND Approval_Status_By_HR = 'APPROVED' and Time_Entry_Date BETWEEN '" + DateFrom + "' and '" + DateTo + "'  and created_by = '" + id + "'ORDER BY Time_Entry_Date DESC";

            }
            else
                strTemp = "SELECT id,related_to_id_case,[related_to_name_case],[related_to_id_task],[related_to_name_task],RIGHT(Convert(VARCHAR(20), Time_start,100),7) as Time_start,RIGHT(Convert(VARCHAR(20), Time_end,100),7) as Time_end,CONVERT(varchar , Time_Entry_Date,106)as Time_Entry_Date,[TimeDiffernce],[SheetType],[Remarks], Approval_Status as Reject_Status,ApprovalRemark as RejectRemark,customer_name,Destination_customer_name as desti_customer_name,ODSource_Point,Distance,ISNULL(CONVERT(varchar(15),Approval_Date,106),'N/A')as Approval_Date,ISNULL(CONVERT(varchar(15),Approval_Date_By_HR,106),'N/A')as Approval_Date FROM [crm_timesheet] where [Useable]='Y' and SheetType = '" + SheetType + "' and Approval_Status = '" + TimeStatus + "' and Time_Entry_Date BETWEEN '" + DateFrom + "' and '" + DateTo + "'  and created_by = '" + id + "'ORDER BY Time_Entry_Date DESC";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    //Jay Prakash Maurya

    #region Get Search Employee by HR Rating
    [WebMethod]
    public DataSet GetSearchEmployeeByHRRating(string EmployeeID, string month, int year)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("HRRatingSheet");
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC RATING_BY_HRSHEET ?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@CONTACT_ID", OdbcType.Int).Value = Convert.ToInt32(EmployeeID);
            m_CommandODBC.Parameters.Add("@Month", OdbcType.VarChar).Value = month;
            m_CommandODBC.Parameters.Add("@year", OdbcType.Int).Value = year;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            //m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region   Update Rating status
    [WebMethod]
    public DataSet RatingSubminInsert(int id, int count, decimal max, decimal min, decimal avg, decimal total, int contactid, int by, string Host, string month, int year, string status)
    {

        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        OdbcCommand m_CommandODBC = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC insert_crm_rating_submit ?,?,?,?,?,?,?,?,?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@rating_id", OdbcType.Int).Value = id;
            m_CommandODBC.Parameters.Add("@TotalCount", OdbcType.Int).Value = count;
            m_CommandODBC.Parameters.Add("@MaxRating", OdbcType.Decimal).Value = max;
            m_CommandODBC.Parameters.Add("@MinRating", OdbcType.Decimal).Value = min;
            m_CommandODBC.Parameters.Add("@AvgRating", OdbcType.Decimal).Value = avg;
            m_CommandODBC.Parameters.Add("@Total", OdbcType.Decimal).Value = total;
            m_CommandODBC.Parameters.Add("@Contact_Id", OdbcType.Int).Value = contactid;
            m_CommandODBC.Parameters.Add("@By", OdbcType.Int).Value = by;
            m_CommandODBC.Parameters.Add("@Host", OdbcType.VarChar).Value = Host;
            m_CommandODBC.Parameters.Add("@Month", OdbcType.VarChar).Value = month;
            m_CommandODBC.Parameters.Add("@Year", OdbcType.Int).Value = year;
            m_CommandODBC.Parameters.Add("@status", OdbcType.VarChar).Value = status;
            m_CommandODBC.ExecuteNonQuery();
            nResultCode = 0;
            strResult = "Pass";
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

    #region   Update Rating status
    [WebMethod]
    public DataSet RatingUpdateStatus(string id, int UserId, string Host, string status)
    {

        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        OdbcCommand m_CommandODBC = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC crm_rating_update_status ?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@id", OdbcType.VarChar).Value = id;
            m_CommandODBC.Parameters.Add("@by", OdbcType.Int).Value = UserId;
            m_CommandODBC.Parameters.Add("@ip", OdbcType.VarChar).Value = Host;
            m_CommandODBC.Parameters.Add("@status", OdbcType.VarChar).Value = status;
            m_CommandODBC.ExecuteNonQuery();
            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Search Employee for rating activities
    [WebMethod]
    public DataSet GetSearchRatingActivity(int EmployeeID, int accountid, string month, int year)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("RatingSheet");
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC hr_Rating_activity ?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@Id", OdbcType.Int).Value = EmployeeID;
            m_CommandODBC.Parameters.Add("@AccountId", OdbcType.Int).Value = accountid;
            m_CommandODBC.Parameters.Add("@Month", OdbcType.VarChar).Value = month;
            m_CommandODBC.Parameters.Add("@Year", OdbcType.Int).Value = year;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            //m_DataSet.Tables.Add(m_DataTable);
            nResultCode = 0;
            strResult = "Pass";
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
    [WebMethod]
    public DataSet GetGeneralRatingActivity(int EmployeeID, int accountid, string month, int year)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("RatingSheet");
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC general_Rating_activity ?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@Id", OdbcType.Int).Value = EmployeeID;
            m_CommandODBC.Parameters.Add("@reportingToId", OdbcType.Int).Value = accountid;
            m_CommandODBC.Parameters.Add("@Month", OdbcType.VarChar).Value = month;
            m_CommandODBC.Parameters.Add("@Year", OdbcType.Int).Value = year;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            //m_DataSet.Tables.Add(m_DataTable);
            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Search Employee Rating Dashboard
    [WebMethod]
    public DataSet GetSearchRatingDashBoard(string month, int year, int id, int reportingid)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("RatingSheet");
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC Rating_Dashboard ?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@Month", OdbcType.VarChar).Value = month;
            m_CommandODBC.Parameters.Add("@Year", OdbcType.Int).Value = year;
            m_CommandODBC.Parameters.Add("@Id", OdbcType.Int).Value = id;
            m_CommandODBC.Parameters.Add("@reportingToId", OdbcType.Int).Value = reportingid;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            //m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Search Employee by HR Rating
    [WebMethod]
    public DataSet GetSearchEmployeeByHRRating(string EmployeeID, string Status, string DateFrom)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("RatingAA");
        try
        {
            m_Connection.OpenDB("Galaxy");
            //  strTemp = "declare @rating table(categ_name varchar(50),rating int ,id int)insert into @rating(categ_name,rating,id)" +
            //      " SELECT crm_rating_categories.categ_name, rating,crm_cases_rating.id FROM crm_cases_rating INNER JOIN crm_rating_categories ON " +
            //      "crm_cases_rating.rating_id = crm_rating_categories.categ_id INNER JOIN crm_cases ON crm_cases_rating.related_to_id = crm_cases.id" +
            //      " WHERE (CONVERT(datetime, crm_cases_rating.created_date, 103) >= CONVERT(datetime, '" + DateFrom + "', 103)) AND " +
            //      "(CONVERT(datetime, crm_cases_rating.created_date, 103) <= CONVERT(datetime,'" + DateTo + "', 103)) AND" +
            //      " (ISNULL(crm_cases_rating.Status, 'Pending') = '" + Status + "') AND (crm_cases_rating.related_to = 'CAS') AND" +
            //      " (crm_cases.owner_id =  " + EmployeeID + ") union all SELECT crm_rating_categories.categ_name,rating,crm_cases_rating.id" +
            //      " FROM crm_cases_rating INNER JOIN crm_rating_categories ON crm_cases_rating.rating_id = crm_rating_categories.categ_id INNER JOIN " +
            //      "crm_tasks ON crm_cases_rating.related_to_id = crm_tasks.id WHERE     (CONVERT(datetime, crm_cases_rating.created_date, 103)" +
            //      " >= CONVERT(datetime,'" + DateFrom + "', 103)) AND (CONVERT(datetime, crm_cases_rating.created_date, 103) <= CONVERT(datetime,'" + DateTo +
            //      "', 103)) AND (ISNULL(crm_cases_rating.Status, 'Pending') = '" + Status + "') AND (crm_cases_rating.related_to = 'TSK') AND " +
            //      "(crm_tasks.owner_id =" + EmployeeID + ")SELECT     categ_name,COUNT(rating) AS Rcount, MAX(rating) AS maxrating, MIN(rating) AS minrating," +
            //      " AVG(CONVERT(float, rating)) AS avgrating,Id = REPLACE(STUFF((SELECT ',' + convert(varchar,id) AS [data()] FROM @rating AS x " +
            //      "WHERE x.categ_name = t.categ_name FOR XML PATH ('') ), 1, 1, ''), ' ,', ',')FROM @rating AS t group by categ_name ";



            //  strTemp = "with Rating as(SELECT     crm_rating_categories.categ_name, rating FROM crm_cases_rating INNER JOIN crm_rating_categories ON " +
            //      "crm_cases_rating.rating_id = crm_rating_categories.categ_id INNER JOIN crm_cases ON crm_cases_rating.related_to_id = crm_cases.id WHERE"
            // + " (CONVERT(datetime, crm_cases_rating.created_date, 103) >= CONVERT(datetime, '" + DateFrom + "', 103)) AND " +
            //     "(CONVERT(datetime, crm_cases_rating.created_date, 103) <= CONVERT(datetime,'" + DateTo + "', 103)) AND (ISNULL(crm_cases_rating.Status, 'Pending') = '" + Status + "')" +
            //     " AND (crm_cases_rating.related_to = 'CAS') AND (crm_cases.owner_id = " + EmployeeID + ") union all SELECT     crm_rating_categories.categ_name,rating" +
            //  " FROM         crm_cases_rating INNER JOIN crm_rating_categories ON crm_cases_rating.rating_id = crm_rating_categories.categ_id INNER JOIN" +
            //           " crm_tasks ON crm_cases_rating.related_to_id = crm_tasks.id WHERE     (CONVERT(datetime, crm_cases_rating.created_date, 103) >= CONVERT(datetime,'" +
            //          DateFrom + "', 103)) AND (CONVERT(datetime, crm_cases_rating.created_date, 103) <= CONVERT(datetime,'" + DateTo + "', 103)) AND " +
            //          "(ISNULL(crm_cases_rating.Status, 'Pending') = '" + Status + "') AND (crm_cases_rating.related_to = 'TSK') AND (crm_tasks.owner_id =" + EmployeeID + "))" +
            //"SELECT categ_name, COUNT(rating) AS Rcount, MAX(rating) AS maxrating, MIN(rating) AS minrating, AVG(CONVERT(float, rating)) AS avgrating  from Rating group by categ_name";


            m_CommandODBC = new OdbcCommand("EXEC RATING_BY_CONTACT ?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@CONTACT_ID", OdbcType.Int).Value = Convert.ToInt32(EmployeeID);
            m_CommandODBC.Parameters.Add("@FROMDATE", OdbcType.VarChar).Value = DateFrom;
            m_CommandODBC.Parameters.Add("@STATUS", OdbcType.VarChar).Value = Status;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            //  OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Search Employee Rating Report
    [WebMethod]
    public DataSet GetSearchEmployeeByRatingReport(string EmployeeID, string DateFrom)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("RatingReport");
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC Rating_report ?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@Date", OdbcType.VarChar).Value = DateFrom;
            m_CommandODBC.Parameters.Add("@Id", OdbcType.Int).Value = Convert.ToInt32(EmployeeID);

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            //  OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            // m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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


    #region Get Search Employee Rating Report Category
    [WebMethod]
    public DataSet GetSearchEmployeeByCaseCategory(DateTime FromDate, DateTime ToDate, string EmployeeID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("RatingReport");
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "select  RIGHT('0000'+CAST(id as varchar(4)),4) id,related_to_name,contact_title+' '+contact_full_name Name,(select dept_name from crm_departments where dept_id=contact_dept_id)dept_name from crm_contacts  where id= " + EmployeeID +
                "select distinct 'Case' Type, case_type_id,dbo.GetCaseTypeName(case_type_id) caseType from crm_cases  where crm_cases.end_time between '" + FromDate + "' and '" + ToDate + "' and owner_id= " + EmployeeID +
                " union " +
                "select distinct 'Task' Type, task_type_id,dbo.GetCaseTypeName(task_type_id) caseType from crm_tasks  where crm_tasks.end_time between '" + FromDate + "' and '" + ToDate + "' and owner_id= " + EmployeeID;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            //  m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";




            //m_Connection.OpenDB("Galaxy");
            //m_CommandODBC = new OdbcCommand("EXEC Rating_report ?,?", m_Connection.oCon);
            //m_CommandODBC.CommandType = CommandType.StoredProcedure;
            //m_CommandODBC.Parameters.Add("@Date", OdbcType.VarChar).Value = DateFrom;
            //m_CommandODBC.Parameters.Add("@Id", OdbcType.Int).Value = Convert.ToInt32(EmployeeID);

            //OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            //  OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            //  m_DataAdapter.Fill(m_DataSet);
            // m_DataSet.Tables.Add(m_DataTable);

            //nResultCode = 0;
            //strResult = "Pass";
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
    #region Get Search Employee Rating Report Category
    [WebMethod]
    public DataSet TypeWiseCaseRateng(DateTime FromDate, DateTime ToDate, string EmployeeID, string type, string type_id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("RateCateg");
        string strcategory = "";
        try
        {
            m_Connection.OpenDB("Galaxy");
            if (type == "Case")
            {
                strTemp = "select categ_name from crm_rating_categories where categ_enabled='Y' and categ_case_types like  '%," + type_id + ",%' ";
            }
            else
            {
                strTemp = "select categ_name from crm_rating_categories where categ_enabled='Y' and categ_task_types like  '%," + type_id + ",%' ";
            }
            if (strTemp == "")
            {
                return null;
            }
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            if (m_DataTable.Rows.Count > 0)
            {
                for (int i = 0; i < m_DataTable.Rows.Count; i++)
                {
                    if (strcategory != "")
                        strcategory += ",";
                    strcategory += "[" + m_DataTable.Rows[i][0].ToString() + "]";
                }



                if (strcategory != "")
                {
                    strTemp = ";WITH CTE_TestData as ( ";
                    if (type == "Case")
                    {
                        strTemp += "select related_to,CONVERT(decimal, CONVERT(decimal, rating))rating," +
                            "'<a href=\"javascript:window.parent.parent.OpenItem(''ShowObject.aspx?ObjectType=CAS&ID=' + convert(varchar,crm_cases.id) + ''', ''Cases [' + crm_cases.transaction_number + ']'')\">' " +
                            " + crm_cases.transaction_number + '</a>' transaction_number,    " +
                            "crm_rating_categories.categ_name from crm_cases_rating      " +
                            "inner join crm_cases on crm_cases.id=crm_cases_rating.related_to_id      " +
                            "inner join crm_rating_categories on crm_cases_rating.rating_id=crm_rating_categories.categ_id      " +
                            "where crm_cases.end_time between '" + FromDate + "' and '" + ToDate + "' and owner_id=" + EmployeeID + "  and crm_cases.case_type_id= " + type_id;
                    }
                    else
                    {
                        strTemp += "select crm_cases_rating.related_to,CONVERT(decimal, CONVERT(decimal, rating))rating, " +
                            "'<a href=\"javascript:window.parent.parent.OpenItem(''ShowObject.aspx?ObjectType=TSK&ID=' + convert(varchar,crm_tasks.id) + ''', ''Cases [' + crm_tasks.transaction_number + ']'')\">'" +
                            " + crm_tasks.transaction_number + '</a>' transaction_number, crm_rating_categories.categ_name from crm_cases_rating inner join crm_tasks on crm_tasks.id=crm_cases_rating.related_to_id" +
                            "  inner join crm_rating_categories on crm_cases_rating.rating_id=crm_rating_categories.categ_id " +
                            "  where crm_tasks.end_time between '" + FromDate + "' and '" + ToDate + "' and owner_id=" + EmployeeID + "  and crm_tasks.task_type_id= " + type_id;
                    }
                    strTemp += ")";
                    strTemp += " select transaction_number [Case/Task No.],case when related_to='CAS' then 'Case' else 'Task' end as Type, " + strcategory + " from (select * from CTE_TestData t ) unpvt PIVOT (sum(rating)FOR categ_name in (" + strcategory + ")) pvt";
                    m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
                    m_DataAdapter.Fill(m_DataSet);
                }
            }
            nResultCode = 0;
            strResult = "Pass";
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

    #region Update or Insert categories rating
    [WebMethod]
    public DataSet InsertUpdateCategoriesRating(long nTypeID, string strName, string strEnabled, string strServiceType, string strTaskServiceType, string strDiscard)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        string strTemp = "";
        try
        {
            m_Connection.OpenDB("Galaxy");
            string strTemp1 = "Select Count(*) as count from crm_rating_categories " +
                               " where categ_name = '" + strName + "'";

            m_DataAdapterODBC = new OdbcDataAdapter(strTemp1, m_Connection.oCon);
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);

            if (nName > 0 && nTypeID == 0) // for checking the existence the record
            {
                strResult = "Rating already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else if (nName == 0 && nTypeID == 0) // for inserting the record
            {

                strTemp = "INSERT into crm_rating_categories (categ_name,categ_enabled,categ_case_types,categ_task_types,categ_Discard)" +
                          " Values ('" + strName + "','" + strEnabled + "','" + strServiceType + "','" + strTaskServiceType + "','" + strDiscard + "')";
            }

            else // for updating the existing record
            {
                strTemp = "UPDATE crm_rating_categories  " +
                          "SET  categ_name='" + strName + "', categ_enabled = '" + strEnabled + "'" +
                           ",categ_case_types = '" + strServiceType + "',categ_task_types='" + strTaskServiceType + "',categ_Discard='" + strDiscard + "'" +
                           " Where categ_id = " + nTypeID + "";
            }

            m_CommandODBC = new OdbcCommand();
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);

            m_CommandODBC.ExecuteNonQuery();

            if (nTypeID == 0)
            {
                nResultCode = 0;
                strResult = "Rating Type added successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else
            {
                nResultCode = 0;
                strResult = "Rating Type updated successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            if (nResultCode == 2627)
                strResult = "Rating already exists !";
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

    #region  rating
    [WebMethod]
    public DataSet RatingInsertUpdate(int caseid, int cat_id, decimal rating, int UserId, string Host, string related_to, string status, int contactId)
    {

        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        OdbcCommand m_CommandODBC = null;

        try
        {

            // strTemp = "SELECT " + id.ToString() + " AS CaseId, [categ_id],[categ_name],[categ_enabled],[categ_case_types]  FROM [crm_rating_categories] where categ_case_types like '%" + id + "%'";
            // strTemp = "SELECT" + id.ToString() + " AS CaseId,  crm_rating_categories.[categ_id],crm_rating_categories.[categ_name],isnull(crm_cases_rating.rating ,0) as rating FROM [crm_rating_categories] left join crm_cases_rating on crm_rating_categories.categ_id = crm_cases_rating.rating_id where crm_rating_categories.categ_case_types like '%" + typeid + "%'and crm_rating_categories.[categ_enabled]='Y' or crm_cases_rating.related_to_id=" + id;

            //  m_CommandODBC = new OdbcCommand("EXEC crm_rating_update ?,?,?,?,?,?,?", oCon, m_TransactionODBC);
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC crm_rating_update ?,?,?,?,?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@related_to", OdbcType.VarChar).Value = related_to;
            m_CommandODBC.Parameters.Add("@caseid", OdbcType.Int).Value = caseid;
            m_CommandODBC.Parameters.Add("@cat_id", OdbcType.Int).Value = cat_id;
            m_CommandODBC.Parameters.Add("@rating", OdbcType.Decimal).Value = rating;
            m_CommandODBC.Parameters.Add("@by", OdbcType.Int).Value = UserId;

            // m_CommandODBC.Parameters.Add("@date", OdbcType.DateTime).Value = DB_UTC_DATE;
            m_CommandODBC.Parameters.Add("@ip", OdbcType.VarChar).Value = Host;
            m_CommandODBC.Parameters.Add("@status", OdbcType.VarChar).Value = status;
            m_CommandODBC.Parameters.Add("@contactId", OdbcType.Int).Value = contactId;

            m_CommandODBC.ExecuteNonQuery();
            nResultCode = 0;
            strResult = "Pass";

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

    #region Search case
    [WebMethod]
    public DataSet SearchCase(string startdate, string enddateto, string srchvalue, string status, string member, string casetype, int reportto, string strcustmer)
    //, int owner_id
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Category");

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "";
            if (srchvalue == "CAS" || srchvalue == "Both")
            {
                strTemp += "SELECT  [id] as id," +
                     "'<a href=\"javascript:window.parent.parent.OpenItem(''ShowObject.aspx?ObjectType=CAS&ID=' + convert(varchar,id) + ''', ''Cases [' + transaction_number + ']'')\">' + transaction_number + '</a>' as casenumber," +
                    //    "[transaction_number] as casenumber
                "[case_subject] as subjects,[case_description] as descriptions,REPLACE(convert(varchar(24),open_time,113),':00:000','') as opentime,REPLACE(convert(varchar(24),end_time,113),':00:000','')  as endtime ,dbo.GetDateDiffHM(open_time,end_time)  as datediffer" +
                       ",[case_type_id],dbo.GetCaseTypeName(case_type_id) CaseType, dbo.GetContact(owner_id) as ownername,owner_id,dbo.GetAccountName(case_customer_id) as customer_name,case_caller_name as caller_name,case_caller_email as caller_email,case_category_name as category_name,case_subcategory_name as subcategory_name,end_remarks as remarks,'CAS' AS type  FROM [crm_cases] where  " +
                  m_Connection.DB_NULL + "(case_type_id,0)>0 ";
                if (startdate != "")
                    strTemp += " and  convert(datetime,end_time,103)  >= convert(datetime,'" + startdate + "',103)  and   convert(datetime,end_time,103)  <= convert(datetime,'" + enddateto + "',103)";
                if (status == "Pending")
                    strTemp += "and id not in(select related_to_id from crm_cases_rating  where  (Status='Discard' or Status='Rated' or Status='Close')  and related_to='CAS')";
                else
                    strTemp += " and id in(select related_to_id from crm_cases_rating  where  Status='" + status + "'   and related_to='CAS')";
                if (member != "" && member != "0")
                    strTemp += " and owner_id in (" + member + ")";
                else
                    strTemp += " and owner_id in (select id from crm_contacts where cust_reportingtocolumn =" + reportto.ToString() + ")";


                if (casetype != "0")
                    strTemp += " and case_type_id =" + casetype;
                if (strcustmer != "" && strcustmer != "0")
                    strTemp += " and case_customer_id=" + strcustmer;

            }
            if (srchvalue == "Both")
            {
                strTemp += " union ";
            }

            if (srchvalue == "TSK" || srchvalue == "Both")
            {
                strTemp += "SELECT  task.id as id ," +
                    "'<a href=\"javascript:javascript:window.parent.parent.OpenItem(''ShowObject.aspx?ObjectType=TSK&ID=' + convert(varchar,task.id) + ''', ''Tasks [' + task.transaction_number + ']'')\">' + task.transaction_number + '</a>' as casenumber," +
                    //    "task.transaction_number as casenumber,"++
                "task.task_subject as subjects,task.task_description as descriptions,REPLACE(convert(varchar(24),task.open_time,113),':00:000','') as opentime,REPLACE(convert(varchar(24),task.end_time,113),':00:000','') as endtime,dbo.GetDateDiffHM(task.open_time,task.end_time) as datediffer , task.task_type_id as case_type_id,dbo.GetCaseTypeName(task.task_type_id ) CaseType, " +
                    " owner_name as ownername,task.owner_id,dbo.GetAccountName(cases.case_customer_id) as customer_name,cases.case_caller_name as caller_name,cases.case_caller_email as caller_email,cases.case_category_name as category_name,cases.case_subcategory_name as subcategory_name,cases.end_remarks as remarks ,'TSK' AS type " +
                    " FROM dbo.crm_cases as cases RIGHT JOIN dbo.crm_tasks as task  ON cases.id = task.related_to_id " +
                         " where " + m_Connection.DB_NULL + "(task.task_type_id,0)>0 "; ;
                if (startdate != "")
                    strTemp += " and convert(datetime,task.end_time,103)  >= convert(datetime,'" + startdate + "',103)  and   convert(datetime,task.end_time,103)  <= convert(datetime,'" + enddateto + "',103)";
                if (status == "Pending")
                    strTemp += "and task.id not in(select related_to_id from crm_cases_rating  where  (Status='Discard'  or Status='Rated' or Status='Close')  and related_to='TSK')";
                else
                    strTemp += " and task.id in(select related_to_id from crm_cases_rating  where  Status='" + status + "'   and related_to='TSK')";
                if (member != "" && member != "0")
                    strTemp += " and task.owner_id in (" + member + ")";
                else
                    strTemp += " and task.owner_id in (select id from crm_contacts where cust_reportingtocolumn =" + reportto + ")";

                //else
                //    strTemp += " and " + m_Connection.DB_NULL + "(task.owner_id,0)>0 ";
                if (casetype != "0")
                    strTemp += " and task_type_id =" + casetype;
                if (strcustmer != "" && strcustmer != "0")
                    strTemp += " and cases.case_customer_id=" + strcustmer;

            }

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region  Type Category
    [WebMethod]
    public DataSet TypeCategory(int id, int typeid, string related_to, string status)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            // strTemp = "SELECT " + id.ToString() + " AS CaseId, [categ_id],[categ_name],[categ_enabled],[categ_case_types]  FROM [crm_rating_categories] where categ_case_types like '%" + id + "%'";
            // strTemp = "SELECT" + id.ToString() + " AS CaseId,  crm_rating_categories.[categ_id],crm_rating_categories.[categ_name],isnull(crm_cases_rating.rating ,0) as rating FROM [crm_rating_categories] left join crm_cases_rating on crm_rating_categories.categ_id = crm_cases_rating.rating_id where crm_rating_categories.categ_case_types like '%" + typeid + "%'and crm_rating_categories.[categ_enabled]='Y' or crm_cases_rating.related_to_id=" + id;
            OdbcCommand m_command = new OdbcCommand("EXEC crm_caserating ?,?,?,?", m_Connection.oCon);
            m_command.CommandType = CommandType.StoredProcedure;
            m_command.Parameters.Add("@caseid", OdbcType.Int).Value = id;
            m_command.Parameters.Add("@casetype", OdbcType.VarChar).Value = "%" + typeid + ",%";
            m_command.Parameters.Add("@related_to", OdbcType.VarChar).Value = related_to;
            m_command.Parameters.Add("@status", OdbcType.VarChar).Value = status;

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_command);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "TypeCategory";

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get categories rating
    [WebMethod]
    public DataSet GetCategoriesRating()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT categ_id,categ_name,categ_enabled,categ_Discard," +
                      "(case when categ_enabled='Y' then 'Yes' else 'No' end) as Enabled,categ_case_types,(case when categ_Discard='Y' then 'Yes' else 'No' end) as Discard," +

                      m_Connection.DB_FUNCTION + "GetTypesDesc(categ_case_types,',') as case_types,categ_task_types," + m_Connection.DB_FUNCTION + "GetTypesDesc(categ_task_types, ',') AS task_types " +
                      "FROM crm_rating_categories order by categ_name";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "CategoriesRating";

            nResultCode = 0;
            strResult = "Pass";
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

    //#region Get Enable categories rating
    //[WebMethod]
    //public DataSet GetEnableCategoriesRating()
    //{
    //    int nResultCode = -1;
    //    string strResult = "Fail - ";
    //    DataSet m_DataSet = new DataSet();

    //    try
    //    {
    //        m_Connection.OpenDB("Galaxy");
    //        strTemp = "SELECT categ_id,categ_name,categ_enabled,categ_Discard," +
    //                  "(case when categ_enabled='Y' then 'Yes' else 'No' end) as Enabled,categ_case_types,(case when categ_Discard='Y' then 'Yes' else 'No' end) as Discard," +

    //                  m_Connection.DB_FUNCTION + "GetTypesDesc(categ_case_types,',') as case_types,categ_task_types," + m_Connection.DB_FUNCTION + "GetTypesDesc(categ_task_types, ',') AS task_types " +
    //                  "FROM crm_rating_categories where categ_enabled='Y' order by categ_name";

    //        OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
    //        m_DataAdapter.Fill(m_DataSet);
    //        m_DataSet.Tables[0].TableName = "CategoriesRating";

    //        nResultCode = 0;
    //        strResult = "Pass";
    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //    }
    //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
    //    return m_DataSet;
    //}
    //#endregion

    #region HR Rating Categories
    [WebMethod]
    public DataSet GetHRRatingCategory()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("HRRating");
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC HR_RATING_CATEGORY ", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region   Insert HR Rating

    [WebMethod]
    public DataSet HRRatingSubmitInsert(int ratingid, int contactId, decimal rating, string month, int year, int by, string refno)
    {

        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        OdbcCommand m_CommandODBC = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC hr_rating_insert ?,?,?,?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@rating_id", OdbcType.Int).Value = ratingid;
            m_CommandODBC.Parameters.Add("@Contact_Id", OdbcType.Int).Value = contactId;
            m_CommandODBC.Parameters.Add("@Rating", OdbcType.Decimal).Value = rating;
            m_CommandODBC.Parameters.Add("@Month", OdbcType.VarChar).Value = month;
            m_CommandODBC.Parameters.Add("@Year", OdbcType.Int).Value = year;
            m_CommandODBC.Parameters.Add("@Created_By", OdbcType.Int).Value = by;
            m_CommandODBC.Parameters.Add("@refno", OdbcType.VarChar).Value = refno;
            m_CommandODBC.ExecuteNonQuery();
            nResultCode = 0;
            strResult = "Pass";
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

    #region HR update status rating
    [WebMethod]
    public DataSet HRRatingSubmitUpdateStatus(string id, string refno)
    {

        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        OdbcCommand m_CommandODBC = null;
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC rating_sbmit_status_update ?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@Ids", OdbcType.VarChar).Value = id;
            m_CommandODBC.Parameters.Add("@refno", OdbcType.VarChar).Value = refno;
            m_CommandODBC.ExecuteNonQuery();
            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Search Open case summary Owner Wise
    //[WebMethod]
    //public DataSet GetSearchOpenCaseSummary1(int accountid, int userid, string sp)
    //{
    //    int nResultCode = -1;
    //    string strResult = "Fail - ";
    //    DataSet m_DataSet = new DataSet();
    //    DataTable m_DataTable = new DataTable("RatingSheet");
    //    string szFilter = "";
    //    try
    //    {
    //        string type = "O";
    //        m_Connection.OpenDB("Galaxy");
    //        if (bApplyOwnerFilter == true)
    //        {
    //            DataSet dsDataSet = CreateFilter(nLoginContactID, strCallType, nLoginAccountID,
    //                                                        strLoginAccType, nLoginDeptID, nLoginDesigLevel);
    //            if (Convert.ToInt32(dsDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
    //                return dsDataSet;

    //            if (dsDataSet.Tables["Filter"].Rows.Count > 0)
    //                szFilter = dsDataSet.Tables["Filter"].Rows[0]["FilterString"].ToString();
    //        }
    //       // strTemp = "set nocount on ;With T as (SELECT owner_id,isnull([OPEN],0) openCase, isnull([OPEN WORKING],0) WorkingCase, isnull([PENDING FOR FEEDBACK],0) FeedbackCase FROM (SELECT  isnull(owner_id,0) owner_id,[dbo].[GetStatusName]('CAS',case_status_id) AS CaseStatus,count(*) as CaseCount FROM crm_cases WHERE Useable = 'Y' AND  case_status_id in (1,3,7)  AND (owner_id in (129,130,192,188,123) OR created_by in (129,130,192,188,123)  OR case_customer_id in (select map_account_id from crm_mappings where map_to_contact_id = 123)) group by owner_id,case_status_id ) p PIVOT (sum(CaseCount) FOR CaseStatus IN ([OPEN], [OPEN WORKING], [PENDING FOR FEEDBACK]) ) AS pvt) select case when dbo.GetAccountName(crm_contacts.related_to_id)='' then 'Not Available' else dbo.GetAccountName(crm_contacts.related_to_id)end AccountName ,case when dbo.GetTeamName(crm_contacts.contact_team_id)='' then 'Not Available' else dbo.GetTeamName(crm_contacts.contact_team_id) end TeamName ,isnull(contact_full_name,'Not Available') contactName,T.openCase,T.WorkingCase,T.FeedbackCase,T.openCase+T.WorkingCase+T.FeedbackCase as Total, T.owner_id,0 location_id from T left outer join crm_contacts on t.owner_id=id order by AccountName,TeamName,contactName ";
    //        strTemp = "set nocount on SELECT";
    //        if (type == "O")
    //            strTemp += "dbo.GetAccountName(dbo.GetContactAccountID(owner_id)) as AccountName,dbo.GetTeamName(dbo.GetContactTeamID(owner_id)) TeamName,dbo.GetContactName(owner_id)contactName,owner_id,0 location_id";
    //        else if (type == "T")
    //            strTemp += "dbo.GetAccountName(dbo.GetTeamAccountID(Team_Id)) as AccountName,dbo.GetTeamName(Team_Id) TeamName,'' contactName,,Team_Id owner_id,0 location_id ";
    //        else
    //            strTemp += "dbo.GetAccountName(Location_Id) as AccountName,'' TeamName,dbo.GetAccountName(case_customer_id) contactName, case_customer_id owner_id, location_id";
    //        strTemp += "";


    //        strTemp = "SELECT categ_id,categ_name,categ_enabled," +
    //                  "(case when categ_enabled='Y' then 'Yes' else 'No' end) as Enabled,categ_case_types," +

    //                  m_Connection.DB_FUNCTION + "GetTypesDesc(categ_case_types,',') as case_types,categ_task_types," + m_Connection.DB_FUNCTION + "GetTypesDesc(categ_task_types, ',') AS task_types " +
    //                  "FROM crm_rating_categories ";

    //        OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
    //        m_DataAdapter.Fill(m_DataSet);
    //        m_DataSet.Tables[0].TableName = "CategoriesRating";




    //        m_Connection.OpenDB("Galaxy");
    //        m_CommandODBC = new OdbcCommand("EXEC " + sp + " ?,?", m_Connection.oCon);
    //        m_CommandODBC.CommandType = CommandType.StoredProcedure;
    //        m_CommandODBC.Parameters.Add("@AccountId", OdbcType.Int).Value = accountid;
    //        m_CommandODBC.Parameters.Add("@ContactId", OdbcType.Int).Value = userid;
    //        OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
    //        m_DataAdapter.Fill(m_DataSet);
    //        //m_DataSet.Tables.Add(m_DataTable);

    //        nResultCode = 0;
    //        strResult = "Pass";
    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //    }
    //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
    //    return m_DataSet;
    //}
    [WebMethod]
    public DataSet GetSearchOpenCaseSummary(int accountid, int userid, string sp)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("RatingSheet");
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC " + sp + " ?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@AccountId", OdbcType.Int).Value = accountid;
            m_CommandODBC.Parameters.Add("@ContactId", OdbcType.Int).Value = userid;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            //m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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

    #region Get Search TimeSheet-OD Summary AccountWise
    [WebMethod]
    public DataSet GetTimeSheetODSummaryAccWise(int accountid, string sp)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("TimeSheetODSummary");
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC " + sp + " ?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@AccountId", OdbcType.Int).Value = accountid;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            //m_DataSet.Tables.Add(m_DataTable);

            nResultCode = 0;
            strResult = "Pass";
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


    #region Get Search TimeSheet/OD Working Details Emloyee Wise
    [WebMethod]
    public DataSet GetSearchEmployeeTimeODWorkingDetails(int viewtype, int EmployeeID, DateTime startDate, DateTime EndDate, string sIP, int nUserID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("TimesheetODWorkingDetails");
        try
        {
            DataSet ds = new DataSet();
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("EXEC TS_chart_insert ?,?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@nEmployeeID", OdbcType.Int).Value = EmployeeID;
            m_CommandODBC.Parameters.Add("@dtStartDate", OdbcType.DateTime).Value = startDate;
            m_CommandODBC.Parameters.Add("@dtEndDate", OdbcType.DateTime).Value = EndDate;
            m_CommandODBC.Parameters.Add("@szIP", OdbcType.VarChar).Value = sIP;
            m_CommandODBC.Parameters.Add("@nCreatedBy", OdbcType.VarChar).Value = nUserID;
            //  m_CommandODBC.Parameters.Add("@nResultCode", OdbcType.Int).Direction = ParameterDirection.Output;
            //m_CommandODBC.Parameters.Add("@szResultString", OdbcType.VarChar, 100).Direction = ParameterDirection.Output;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                nResultCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResultCode"].ToString());// 0;// (int)m_CommandODBC.Parameters["@nResultCode"].Value;
                strResult = ds.Tables[0].Rows[0]["ResultString"].ToString();// (string)m_CommandODBC.Parameters["@szResultString"].Value;
            }
            //  m_CommandODBC.ExecuteNonQuery();

            // nResultCode = 0;// (int)m_CommandODBC.Parameters["@nResultCode"].Value;
            //  nResultCode =  (int)m_CommandODBC.Parameters["@nResultCode"].Value;
            // strResult = "Pass";// (string)m_CommandODBC.Parameters["@szResultString"].Value;
            if (nResultCode >= 0)
            {
                m_CommandODBC = new OdbcCommand("EXEC TS_chart_details ?,?,?,?", m_Connection.oCon);
                m_CommandODBC.CommandType = CommandType.StoredProcedure;
                m_CommandODBC.Parameters.Add("@nChartType", OdbcType.Int).Value = viewtype;
                m_CommandODBC.Parameters.Add("@nEmployeeID", OdbcType.Int).Value = EmployeeID;
                m_CommandODBC.Parameters.Add("@dtStartDate", OdbcType.DateTime).Value = startDate;
                m_CommandODBC.Parameters.Add("@dtEndDate", OdbcType.DateTime).Value = EndDate;
                m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
                m_DataAdapter.Fill(m_DataSet);
                nResultCode = 0;
                strResult = "Pass";
            }
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


    #region Update UpdateInTimeOutTime
    [WebMethod]
    public DataSet UpdateInTimeOutTime(int id, string Intime, string OutTime)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("UpdateInTimeOutTime");

        try
        {
            m_Connection.OpenDB("Galaxy");
            m_Connection.BeginTransaction();

            strTemp = "UPDATE crm_employee_day_chart SET in_time=?,out_time=? " +
                "WHERE emp_day_chart_id=" + id;

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon, m_Connection.m_TransactionODBC);
            m_CommandODBC.Parameters.Add("@in_time", OdbcType.VarChar).Value = Intime;
            m_CommandODBC.Parameters.Add("@out_time", OdbcType.VarChar).Value = OutTime;
            m_CommandODBC.ExecuteNonQuery();

            nResultCode = 0;
            strResult = "Record Update Successfully !";
            m_Connection.Commit();
        }
        catch (OdbcException ex)
        {
            nResultCode = ex.ErrorCode;
            strResult = ex.Message;
            m_Connection.Rollback();
        }
        catch (Exception ex)
        {
            nResultCode = -1;
            strResult = ex.Message;
            m_Connection.Rollback();
        }
        finally
        {
            m_Connection.CloseDB();
        }
        m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return m_DataSet;

    }
    #endregion
    public DataSet GetUpdateInTimeOutTimeData()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "SELECT emp_day_chart_id as id ,convert(varchar,chart_date,106)as Working_Date,day_type,location_name, RIGHT(Convert(VARCHAR(20), in_time,100),7) as in_time,RIGHT(Convert(VARCHAR(20), out_time,100),7) as out_time   " +
                        "FROM crm_employee_day_chart";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Main";
            nResultCode = 0;
            strResult = "Pass";

            if (m_DataSet.Tables["Main"].Rows.Count <= 0)
                strResult = "No Matching Record Found";
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
    //JP 26-8-2013
    #region  employee day chart Update Intime and Outtime
    /// <summary>
    /// employee day chart Update multiple row Intime and Outtime 
    /// </summary>
    /// <param name="sChartIDs">multiple ids  seperated by '|'</param>
    /// <param name="nIntime"> in Time</param>
    /// <param name="nOutTime">Out Time</param>
    /// <returns></returns>
    [WebMethod]
    public DataSet UpdateEmployeeInOutTimeDayChart(string sChartIDs, int nIntime, int nOutTime)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("EXEC usp_updateinoutTime_emp_Day_Chart ?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@szChartIds", OdbcType.VarChar).Value = sChartIDs;
            m_CommandODBC.Parameters.Add("@nInTime", OdbcType.Int).Value = nIntime;
            m_CommandODBC.Parameters.Add("@nOutTime", OdbcType.Int).Value = nOutTime;
            //m_CommandODBC.Parameters.Add("@nResultCode", OdbcType.Int).Direction = ParameterDirection.Output;
            //m_CommandODBC.Parameters.Add("@szResultString", OdbcType.VarChar, 100).Direction = ParameterDirection.Output;
            //m_CommandODBC.ExecuteNonQuery();

            //nResultCode = (int)m_CommandODBC.Parameters["@nResultCode"].Value;
            //strResult = (string)m_CommandODBC.Parameters["@szResultString"].Value;
            DataSet ds = new DataSet();
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                nResultCode = Convert.ToInt32(ds.Tables[0].Rows[0]["ResultCode"].ToString());// 0;// (int)m_CommandODBC.Parameters["@nResultCode"].Value;
                strResult = ds.Tables[0].Rows[0]["ResultString"].ToString();// (string)m_CommandODBC.Parameters["@szResultString"].Value;
            }
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


    #region GetRoleCaseActivities
    [WebMethod]
    /*** fetch Role BS ***/
    public DataSet GetRoleCaseActivities(int nRoleId, string strCategory)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC role_case_Detail ?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@nRoleId", OdbcType.Int).Value = nRoleId;
            m_CommandODBC.Parameters.Add("@szType", OdbcType.VarChar).Value = strCategory;
            //DataSet ds = new DataSet();
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);

            nResultCode = 0;
            strResult = "Pass";

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

    #region  Insert Roles Activity
    [WebMethod]
    public DataSet InsertRolesCaseActivity(int ActivityID, int RoleID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "insert into crm_role_case_activity(role_id,type_id) values (" + RoleID.ToString() + "," + ActivityID.ToString() + ")";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
            {
                nResultCode = 0;
                strResult = "Pass";
            }
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

    #region  Delete Roles Case Activity
    [WebMethod]
    public DataSet DeleteRolesCaseActivity(string ActivityID, int RoleID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = "delete from crm_role_case_activity where role_id=" + RoleID.ToString() + " and type_id IN (" + ActivityID + ")";//" "DELETE FROM crm_role_activity WHERE roledet_role_id = " + RoleID.ToString() + " AND roledet_activity_id IN (" + ActivityID + ")";
            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            int nRowCount = m_CommandODBC.ExecuteNonQuery();

            if (nRowCount > 0)
            {
                nResultCode = 0;
                strResult = "Pass";
            }
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

    #region LeadStatusWiseReport
    //[WebMethod]

    //public DataSet LeadStatusWiseReport(string sp)
    //{
    //    int nResultCode = -1;
    //    string strResult = "Fail - ";
    //    DataSet m_DataSet = new DataSet();
    //    try
    //    {
    //        m_Connection.OpenDB("Galaxy");
    //        m_CommandODBC = new OdbcCommand("EXEC "+sp, m_Connection.oCon);
    //        m_CommandODBC.CommandType = CommandType.StoredProcedure;
    //        //DataSet ds = new DataSet();
    //        OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
    //        m_DataAdapter.Fill(m_DataSet);
    //        m_DataSet.Tables[0].TableName = "Data";
    //        nResultCode = 0;
    //        strResult = "Pass";

    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //    }
    //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
    //    return m_DataSet;
    //}

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<ContactList> AllEmployeeList()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        List<ContactList> ob = new List<ContactList>();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("EXEC sp_All_Contact_list ", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;

            //DataSet ds = new DataSet();
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "Data";
            nResultCode = 0;
            strResult = "Pass";

            foreach (DataRow row in m_DataSet.Tables[0].Rows)
            {

                ob.Add(new ContactList()
                {
                    name = row["name"].ToString(),
                    Id = Convert.ToInt32(row["id"]),
                    zone = row["zone"].ToString()
                });
            }

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
        return ob;
    }









    //[WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public List<LeadStatus> LeadAssignWiseReport(DateTime dtfrom, DateTime dtto, int empId)
    //{
    //    int nResultCode = -1;
    //    string strResult = "Fail - ";
    //    DataSet m_DataSet = new DataSet();
    //    List<LeadStatus> ob = new List<LeadStatus>();
    //    try
    //    {
    //        m_Connection.OpenDB("Galaxy");
    //        m_CommandODBC = new OdbcCommand("EXEC MIS_assign_Lead_status ?,?,?", m_Connection.oCon);
    //        m_CommandODBC.CommandType = CommandType.StoredProcedure;
    //        m_CommandODBC.Parameters.Add("@dtFrom", OdbcType.DateTime).Value = dtfrom;
    //        m_CommandODBC.Parameters.Add("@dtTo", OdbcType.DateTime).Value = dtto;
    //        m_CommandODBC.Parameters.Add("@nId", OdbcType.Int).Value = empId;
    //        //DataSet ds = new DataSet();
    //        OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
    //        m_DataAdapter.Fill(m_DataSet);
    //        m_DataSet.Tables[0].TableName = "Data";
    //        nResultCode = 0;
    //        strResult = "Pass";

    //        foreach (DataRow row in m_DataSet.Tables[0].Rows)
    //        {
    //            ob.Add(new LeadStatus()
    //            {
    //                Name = row["Name"].ToString(),
    //                count = Convert.ToInt32(row["leadcount"]),
    //            });
    //        }

    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //    }
    //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
    //    return ob;
    //}

    //[WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public List<LeadStatus> LeadOwnerWiseReport(DateTime dtfrom, DateTime dtto, int empId)
    //{
    //    int nResultCode = -1;
    //    string strResult = "Fail - ";
    //    DataSet m_DataSet = new DataSet();
    //    List<LeadStatus> ob = new List<LeadStatus>();
    //    try
    //    {
    //        m_Connection.OpenDB("Galaxy");
    //        m_CommandODBC = new OdbcCommand("EXEC MIS_owner_Lead_status ?,?,?", m_Connection.oCon);
    //        m_CommandODBC.CommandType = CommandType.StoredProcedure;
    //        m_CommandODBC.Parameters.Add("@dtFrom", OdbcType.DateTime).Value = dtfrom;
    //        m_CommandODBC.Parameters.Add("@dtTo", OdbcType.DateTime).Value = dtto;
    //        m_CommandODBC.Parameters.Add("@nId", OdbcType.Int).Value = empId;
    //        //DataSet ds = new DataSet();
    //        OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
    //        m_DataAdapter.Fill(m_DataSet);
    //        m_DataSet.Tables[0].TableName = "Data";
    //        nResultCode = 0;
    //        strResult = "Pass";

    //        foreach (DataRow row in m_DataSet.Tables[0].Rows)
    //        {
    //            ob.Add(new LeadStatus()
    //            {
    //                Name = row["Name"].ToString(),
    //                count = Convert.ToInt32(row["leadcount"]),
    //            });
    //        }

    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //    }
    //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
    //    return ob;
    //}

    //[WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public List<LeadStatusBar> LeadYearWiseReport(int year)
    //{
    //    int nResultCode = -1;
    //    string strResult = "Fail - ";
    //    DataSet m_DataSet = new DataSet();
    //    List<LeadStatusBar> ob = new List<LeadStatusBar>();
    //    try
    //    {
    //        m_Connection.OpenDB("Galaxy");
    //        m_CommandODBC = new OdbcCommand("EXEC MIS_Lead_Year_Wise ?", m_Connection.oCon);
    //        m_CommandODBC.CommandType = CommandType.StoredProcedure;
    //        m_CommandODBC.Parameters.Add("@Year", OdbcType.Int).Value = year;

    //        //DataSet ds = new DataSet();
    //        OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
    //        m_DataAdapter.Fill(m_DataSet);
    //        m_DataSet.Tables[0].TableName = "Data";
    //        nResultCode = 0;
    //        strResult = "Pass";

    //        foreach (DataRow row in m_DataSet.Tables[0].Rows)
    //        {

    //            ob.Add(new LeadStatusBar()
    //            {
    //                Name = row["LeadMonth"].ToString(),
    //                open = Convert.ToInt32(row["OpenC"]),
    //                working = Convert.ToInt32(row["WorkingC"]),
    //                deferred = Convert.ToInt32(row["DeferredC"]),
    //                close = Convert.ToInt32(row["CloseC"]),
    //            });
    //        }

    //    }
    //    catch (OdbcException ex)
    //    {
    //        nResultCode = ex.ErrorCode;
    //        strResult = ex.Message;
    //    }
    //    catch (Exception ex)
    //    {
    //        nResultCode = -1;
    //        strResult = ex.Message;
    //    }
    //    finally
    //    {
    //        m_Connection.CloseDB();
    //    }
    //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
    //    return ob;
    //}


    #endregion

    #region Update or Insert Holiday
    [WebMethod]
    public DataSet InsertUpdateHoliday(long nLocationID, DateTime date, string strOccasion)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("EXEC ts_calender_Holiday_update ?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@nLocationId", OdbcType.Int).Value = nLocationID;
            m_CommandODBC.Parameters.Add("@dtCalender", OdbcType.DateTime).Value = date;
            m_CommandODBC.Parameters.Add("@szRemark", OdbcType.VarChar).Value = strOccasion;
            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataSet);
            nResultCode = 0;
            strResult = "Pass";
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




    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<CaseAnalysis> GetCaseAnalysis()
    {
        List<CaseAnalysis> data = new List<CaseAnalysis>();
        try
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Galaxy"].ConnectionString;
            con.Open();
            DataTable dtUser = new DataTable("User");
            string strsql = "";


            strsql = "select top 50 transaction_number,case_customer_name from crm_cases where Useable='Y' and case_status_name='Close' and case_status_reason_id<>11";

            SqlDataAdapter ad = new SqlDataAdapter(strsql, con);
            ad.Fill(dtUser);
            foreach (DataRow row in dtUser.Rows)
            {

                data.Add(new CaseAnalysis()
                {
                    caseNumber = row["transaction_number"].ToString(),
                    CustomerName = row["case_customer_name"].ToString()

                });
            }
            con.Close();
        }
        catch (SqlException ex)
        {
            string aa = ex.Message;
        }
        return data;
    }

    #region multiple case edit for Help desk
    [WebMethod(EnableSession=true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<object> CaseEditList(DateTime fromdate, DateTime toDate,int userid,int timespan,int LocationId,int TeamId,int CategoryId,int OwnerId,string CaseNo)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("Status");
        List<object> obcaselist = new List<object>();
        
        //if (HttpContext.Current.Session["contact_id"] != null)
       //     TB_nTimeZoneTimeSpan = Convert.ToInt32(HttpContext.Current.Session["TimeZoneTimeSpan"]);
        try
        {
            m_Connection.OpenDB("Galaxy");
            //strTemp = @"exec case_multiple_edit_helpdesk " + TB_nTimeZoneTimeSpan.ToString();

            m_CommandODBC = new OdbcCommand("EXEC case_multiple_edit_helpdesk ?,?,?,?,?,?,?,?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@TIMESPAN", OdbcType.Int,4).Value = timespan;
            m_CommandODBC.Parameters.Add("@fromdate", OdbcType.DateTime,8).Value = fromdate;
            m_CommandODBC.Parameters.Add("@toDate", OdbcType.DateTime,8).Value = toDate;
            m_CommandODBC.Parameters.Add("@userId", OdbcType.Int,4).Value = userid;
            m_CommandODBC.Parameters.Add("@nLocationId", OdbcType.Int,4).Value = LocationId;
            m_CommandODBC.Parameters.Add("@nTeamId", OdbcType.Int,4).Value = TeamId;
            m_CommandODBC.Parameters.Add("@CaseNumber", OdbcType.VarChar, 50).Value = CaseNo;
            m_CommandODBC.Parameters.Add("@nOwnerId", OdbcType.Int,4).Value = OwnerId;
            m_CommandODBC.Parameters.Add("@nCategoryId", OdbcType.Int,4).Value = CategoryId;

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);




            nResultCode = 0;
            strResult = "Pass";


            foreach (DataRow item in m_DataTable.Rows)
            {
                var ob = new
                {
                    ID = Convert.ToInt32(item["ID"] == null ? 0 : item["ID"]),
                    caseTypeId = Convert.ToInt32(item["case_type_id"] == null ? 0 : item["case_type_id"]),
                    caseCategoryId = Convert.ToInt32(item["case_category_id"] == null ? 0 : item["case_category_id"]),
                    caseSubCategoryId = Convert.ToInt32(item["case_subcategory_id"] == null ? 0 : item["case_subcategory_id"]),
                    CaseNumber = item["CaseNumber"].ToString(),
                    Subject = item["Subject"].ToString(),
                    Account = item["Account"].ToString(),
                    CaseType = item["CaseType"].ToString(),
                    Category = item["Category"].ToString(),
                    SubCategory = item["SubCategory"].ToString(),
                    EndRemarks = item["EndRemarks"].ToString(),
                    AssignName = item["AssignName"].ToString(),
                    OwnerName = item["OwnerName"].ToString(),
                    SeverityId = item["SeverityId"].ToString(),
                    SeverityDesc = item["SeverityDesc"].ToString(),
                    ResposeTime = item["ResposeTime"].ToString(),
                    WorkCloseTime = item["WorkCloseTime"].ToString(),
                    TotalWorkingTime = item["TotalWorkingTime"].ToString(),
                    Level = item["Level"].ToString(),
                    LevelName = item["LevelName"].ToString(),
                    Description = item["case_description"].ToString(),
                    repeate = item["case_repeat_call"].ToString(),


                };
                obcaselist.Add(ob);
            }
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
        return obcaselist;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<object> GeneralList(string sztype)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";

        DataTable m_DataTable = new DataTable("Status");
        List<object> obcaselist = new List<object>();
        try
        {
            m_Connection.OpenDB("Galaxy");

            strTemp = "SELECT name, code FROM crm_general " +
                     "WHERE type='" + sztype + "' ";
            //"ORDER BY name";

            OdbcDataAdapter m_DataAdapterOdbc = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapterOdbc.Fill(m_DataTable);
            nResultCode = 0;
            strResult = "Pass";
            foreach (DataRow item in m_DataTable.Rows)
            {
                var ob = new
                {
                    Name = item["name"].ToString(),
                    Code = item["code"].ToString(),
                };
                obcaselist.Add(ob);
            }
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
        // m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
        return obcaselist;
    }
    #endregion


    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public object CaseUpdate(object items)
    {
        
        int TB_nContactID = Convert.ToInt32(HttpContext.Current.Session["contact_id"]);
        XmlDocument xMainDoc;
        string strHistoryText = "";
        string ResultCode = "0";
        //foreach (var item in items)
        //{
        var json_serializer = new JavaScriptSerializer();
        //try
        //{
        //    object a = JsonConvert.DeserializeObject<object>(items.ToString());
        //}
        //catch (Exception ex)
        //{
            
        //    throw;
        //}
        try
        {
        string tmp = JsonConvert.SerializeObject(items);
        var routes_list = (IDictionary<string, object>)json_serializer.DeserializeObject(tmp);
            xMainDoc = m_Connection.createParameterXML();
            int CaseID = 0;
            foreach (var ob in routes_list)
            {
                strHistoryText = "";
                
                    switch (ob.Key)
                    {
                        case "Subject":
                            m_Connection.fillParameterXML(ref xMainDoc, "case_subject", ob.Value.ToString(), "varchar", "100");
                            break;
                        case "CaseTypeName":
                            m_Connection.fillParameterXML(ref xMainDoc, "case_type_name", ob.Value.ToString(), "varchar", "100");
                            break;
                        case "CaseType":
                            m_Connection.fillParameterXML(ref xMainDoc, "case_type_id", ob.Value.ToString(), "int", "0");
                            break;
                        case "Category":
                            m_Connection.fillParameterXML(ref xMainDoc, "case_category_id", ob.Value.ToString(), "int", "0");
                            break;
                        case "CategoryName":
                            m_Connection.fillParameterXML(ref xMainDoc, "case_category_name", ob.Value.ToString(), "varchar", "100");
                            break;
                        case "SubCategory":
                            m_Connection.fillParameterXML(ref xMainDoc, "case_subcategory_id", ob.Value.ToString(), "int", "0");
                            break;
                        case "SubCategoryName":
                            m_Connection.fillParameterXML(ref xMainDoc, "case_subcategory_name", ob.Value.ToString(), "varchar", "100");
                            break;
                        case "EndRemarks":
                            m_Connection.fillParameterXML(ref xMainDoc, "end_remarks", ob.Value.ToString(), "varchar", "500");
                            break;
                        case "Severity":
                            m_Connection.fillParameterXML(ref xMainDoc, "case_severity_id", ob.Value.ToString(), "int", "0");
                            break;
                        case "SeverityName":
                            m_Connection.fillParameterXML(ref xMainDoc, "case_severity_desc", ob.Value.ToString(), "varchar", "100");
                            break;
                        case "ResposeTime":
                            DateTime response = Convert.ToDateTime(ob.Value);
                            m_Connection.fillParameterXML(ref xMainDoc, "response_time", response.AddMinutes(-330).ToString("dd MMM yyyy HH:mm"), "datetime", "0");
                            break;
                        case "WorkCloseTime":
                            DateTime workcolse = Convert.ToDateTime(ob.Value);
                            m_Connection.fillParameterXML(ref xMainDoc, "work_close_time", workcolse.AddMinutes(-330).ToString("dd MMM yyyy HH:mm"), "datetime", "0");
                            break;
                        case "TotalWorkingTime":
                            m_Connection.fillParameterXML(ref xMainDoc, "working_time", ob.Value.ToString(), "int", "4");
                            break;
                        case "Description":
                            m_Connection.fillParameterXML(ref xMainDoc, "case_description", ob.Value.ToString(), "varchar", "500");
                            break;
                        case "Level":
                            m_Connection.fillParameterXML(ref xMainDoc, "assign_level", ob.Value.ToString(), "int", "4");
                            break;
                        case "repeate":
                            m_Connection.fillParameterXML(ref xMainDoc, "case_repeat_call", ob.Value.ToString(), "varchar", "1");
                            break;
                        case "ID":
                            CaseID = Convert.ToInt32(ob.Value.ToString());
                            break;
                        default:
                            break;
                    }
                    

                


            }
            m_Connection.OpenDB("Galaxy");
            DataTable dt1 = null;
            m_Connection.BeginTransaction();

            dt1 = m_Connection.SaveTransactionData("CAS", CaseID, "N", DateTime.Now, TB_nContactID, HttpContext.Current.Request.UserHostAddress, xMainDoc);

            if (Convert.ToInt32(dt1.Rows[0]["ResultCode"]) >= 0)
            {
                m_Connection.Commit();
                ResultCode = dt1.Rows[0]["ResultCode"].ToString();
                strHistoryText = "Update after closed case";
                dt1 = m_Connection.SaveRecentActivity(CaseID, "CAS", CaseID, "Modified", "", TB_nContactID, "P", 2409, strHistoryText);
            }
            else
            {
                m_Connection.Rollback();
            }
                }
                catch (OdbcException ex)
                {
                    m_Connection.Rollback();

                }
                catch (Exception ex)
                {
                    m_Connection.Rollback();

                }
                finally
                {
                }
            // var aaa = routes_list["ID"].ToString();


     //   }


        //OpfBAL opfBal = new OpfBAL();
        //ErrorVO error = new ErrorVO();
        //try
        //{
        //    object itemslist = opfBal.ProductSave(items, ref error);
        //    var item = new
        //    {
        //        itemlist = itemslist,
        //        error = error
        //    };
        //    return itemslist;
        //    //return item;
        //}
        //catch (Exception ex)
        //{
        //    error.errorCode = -1;
        //    error.errorResult = ex.Message;

        //}




        return ResultCode;
    }


    //[WebMethod(EnableSession = true)]
    //[ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    //public object casegeneralfield()
    //{

    //    ErrorVO error = new ErrorVO();
    //    GlobalWS globalws = new GlobalWS();

    //    DataTable dt = globalws.GetCaseTypes();
    //    List<object> oblist = new List<object>();
    //    foreach (DataRow item in dt.Rows)
    //    {
    //        var ob = new
    //        {
    //            Name = item["type_name"].ToString(),
    //            id = item["type_id"].ToString(),
    //        };
    //        oblist.Add(ob);
    //    }


    //    try
    //    {

    //        var item = new
    //        {
    //            level = GeneralList("CASELEVEL"),
    //            severity = GeneralList("CASESEVERITY"),
    //            casetype = oblist,
    //            location=globalws.Fetchaccountbyshort(),
    //            error = error
    //        };
    //        return item;
    //    }
    //    catch (Exception ex)
    //    {
    //        error.errorCode = -1;
    //        error.errorResult = ex.Message;

    //    }
    //    return error;
    //}

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    public object teamAndEmployeeGet(int location)
    {

        ErrorVO error = new ErrorVO();
        GlobalWS globalws = new GlobalWS();

        DataSet dt = globalws.FetchTeam(location);
        List<object> team = new List<object>();
        foreach (DataRow item in dt.Tables["crm_account_teams"].Rows)
        {
            var ob = new
            {
                Name = item["accteam_name"].ToString(),
                id = item["accountteamid"].ToString(),
            };
            team.Add(ob);
        }

        dt = globalws.FetchownerLocationWise(location,0);
        List<object> contact = new List<object>();
        foreach (DataRow item in dt.Tables["crm_contacts"].Rows)
        {
            var ob = new
            {
                Name = item["contact_full_name"].ToString(),
                id = item["id"].ToString(),
            };
            contact.Add(ob);
        }

        try
        {

            var item = new
            {
                team=team,
                contact=contact,
                error = error
            };
            return item;
        }
        catch (Exception ex)
        {
            error.errorCode = -1;
            error.errorResult = ex.Message;

        }
        return error;
    }


    [WebMethod]
    [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
    public object CaseCategory(string ParentCategID, string CaseTypeID)
    {

        ErrorVO error = new ErrorVO();
        GlobalWS globalws = new GlobalWS();

        DataSet dt = globalws.FetchCaseCategories(ParentCategID, CaseTypeID);
        List<object> oblist = new List<object>();
        foreach (DataRow item in dt.Tables["Categories"].Rows)
        {
            var ob = new
            {
                Name = item["categ_name"].ToString(),
                id = item["categ_id"].ToString(),
            };
            oblist.Add(ob);
        }


        try
        {

            var item = new
            {
                casetype = oblist,
                error = error
            };
            return item;
        }
        catch (Exception ex)
        {
            error.errorCode = -1;
            error.errorResult = ex.Message;

        }
        return error;
    }
    //manjuRana

    #region fetch attendance
    [WebMethod]
    public DataSet Getattendance(int owner_id)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("attendance_Cal");
        DataTable m_DataTable_attendance = new DataTable("attendance");
        DataTable m_DataTable_ = new DataTable("attendance_time");
        try
        {

            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand("fetch_attendance_FMs ?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@owner_id", OdbcType.Int).Value = owner_id;
            m_CommandODBC.ExecuteNonQuery();

            OdbcDataAdapter m_DataAdapter;
            //  OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);

            //   m_DataAdapter.Fill(m_DataTable);
            //   m_DataSet.Tables.Add(m_DataTable);


            m_CommandODBC = new OdbcCommand("select ROW_NUMBER() over (order by id) as Sno,id,convert(varchar(5),cast(check_in as time) )as check_in" +
                                            ", convert(varchar(5), cast(check_out_time as time))  as check_out_time,dbo.[GetContact](owner_id) as Name," +
                                            "REPLACE(CONVERT(varchar(11),date,6), ' ', '-')   as date,convert(varchar(5),DateDiff(s, check_in, check_out_time)/3600)+':'+convert(varchar(5),DateDiff(s, check_in, check_out_time)%3600/60)+':'+convert(varchar(5),(DateDiff(s, check_in, check_out_time)%60))   as timeDiff " +
                                            " ,[check_in_ip],[check_in_ip_location],[check_out_ip]  ,[check_out_ip_location] from [crm_attendance_FMs] where DATEPART(m,[date]) =DATEPART(m, GETDATE()) and owner_id=" + owner_id, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataTable_attendance);
            m_DataSet.Tables.Add(m_DataTable_attendance);


            m_CommandODBC = new OdbcCommand("select top 1 isnull(convert(varchar(30),check_in),'') as check_in,isnull(convert(varchar(30),check_out_time),'') as check_out  from [crm_attendance_FMs] where owner_id=" + owner_id +" and  DATEPART(d, [date]) = DATEPART(d, GETDATE()) ", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataTable_);
            m_DataSet.Tables.Add(m_DataTable_);

            nResultCode = 0;
            strResult = "Pass";
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
    public DataSet GetDateattendance(string date, string month)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        DataTable m_DataTable = new DataTable("attendance");

        try
        {

            m_Connection.OpenDB("Galaxy");



            OdbcDataAdapter m_DataAdapter;



            m_CommandODBC = new OdbcCommand("select ROW_NUMBER() over (order by id) as Sno,id,convert(varchar(5),cast(check_in as time) )as check_in ," +
                                            "convert(varchar(5), cast(check_out_time as time))  as check_out_time,dbo.[GetContact](owner_id) as Name,REPLACE(CONVERT(varchar(11),date,6), ' ', '-') " +
                                             "as date,convert(varchar(5),DateDiff(s, check_in, check_out_time)/3600)+':'+convert(varchar(5),DateDiff(s, check_in, check_out_time)%3600/60)+':'+convert(varchar(5),(DateDiff(s, check_in, check_out_time)%60)) as timeDiff " +
                                              ",[check_in_ip],[check_in_ip_location],[check_out_ip],[check_out_ip_location]from [crm_attendance_FMs] " +
                                             "where DATEPART(m,[date]) = " + month + " and  DATEPART(D,[date]) =" + date, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_DataAdapter = new OdbcDataAdapter(m_CommandODBC);
            m_DataAdapter.Fill(m_DataTable);
            m_DataSet.Tables.Add(m_DataTable);




            nResultCode = 0;
            strResult = "Pass";
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

    public DataSet insert_attendance(int owner_id, string host, string status, string location_name)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();



        try
        {
            string subquery;
            if (status == "Check In")
            {
                subquery = "[check_in]=GETDATE() ,check_in_ip='" + host + "' ,check_in_ip_location='" + location_name + "'";
            }
            else
                subquery = "[check_out_time]=GETDATE() ,check_out_ip='" + host + "' ,check_out_ip_location='" + location_name + "'";

            string query = "UPDATE [crm_attendance_FMs]" +
                         "SET " + subquery + " WHERE  [owner_id] = " + owner_id + "  and DATEPART(d, [date]) = DATEPART(d, GETDATE())";
            m_Connection.OpenDB("Galaxy");

            m_CommandODBC = new OdbcCommand(query, m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;

            m_CommandODBC.ExecuteNonQuery();

            nResultCode = 0;
            strResult = "Pass";
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet; ;




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

    // Added by ashish
    #region
    [WebMethod]
    public DataSet InsertCTIHistory1(string CallNumber ,int UserId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataSet ds = new DataSet();
        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("sp_crm_cti_Inbound_History ?,?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.StoredProcedure;
            m_CommandODBC.Parameters.Add("@CallNumber", OdbcType.VarChar).Value = CallNumber;
            m_CommandODBC.Parameters.Add("@UserId", OdbcType.VarChar).Value = UserId;
            nResultCode = m_CommandODBC.ExecuteNonQuery();
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


    //Added by JP


    [WebMethod]
    public DataSet CheckContactValidation(int nContactID)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            string strTemp = @"select top 1 id,contact_full_name from crm_contacts where id=? and
                                (isnull(contact_title,'')='' 
                                or isnull(contact_fname,'')=''
                                or isnull(contact_lname,'')=''
                                or isnull(contact_address1,'')=''
                                or isnull(contact_city_code,'')=''
                                or isnull(contact_state_code,'')=''
                                or isnull(contact_country_code,'')=''
                                or isnull(contact_phone,'')=''
                                or isnull(customer_type,'')=''
                                or isnull(contact_city_code,'')=''
                                or isnull(contact_currency,'')=''
                                or isnull(contact_product,'')=''
                                or isnull(contact_subproduct,'')=''
                                or isnull(contact_accountno,'')=''
                                )";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@id", OdbcType.Int).Value = nContactID;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(m_DataSet);
            //  int nCount = Convert.ToInt32(dtTable.Rows[0][0]);

            //if (nCount > 0)
            //{
            nResultCode = 1;
            strResult = "";
          
            //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            //    return m_DataSet;
            //}

        }
        catch (OdbcException ex)
        {
            nResultCode = -1;
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


    #region InsertUpdateForProduct
    [WebMethod]
    public DataSet CheckContact(int nContactID, string Phone, int nProductId, int nSubProductId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            string strTemp = "select top 1 id,contact_full_name from crm_contacts where id<>? and contact_phone=? and contact_product_id=? and contact_Subproduct_id=?";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@id", OdbcType.Int).Value = nContactID;
            m_CommandODBC.Parameters.Add("@Phone", OdbcType.VarChar).Value = Phone;
            m_CommandODBC.Parameters.Add("@contact_product_id", OdbcType.Int).Value = nProductId;
            m_CommandODBC.Parameters.Add("@contact_Subproduct_id", OdbcType.Int).Value = nSubProductId;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(m_DataSet);
          //  int nCount = Convert.ToInt32(dtTable.Rows[0][0]);

            //if (nCount > 0)
            //{
                nResultCode = 1;
                strResult = "Contact already exist!";
            //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            //    return m_DataSet;
            //}

        }
        catch (OdbcException ex)
        {
            nResultCode = -1;
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

    [WebMethod]
    public DataSet CheckContactAcc(int nContactID, string accountNo)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            string strTemp = "select top 1 id from crm_contacts where   id<>?  and contact_accountno=?";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@id", OdbcType.Int).Value = nContactID;
            m_CommandODBC.Parameters.Add("@contact_card_no", OdbcType.VarChar).Value = accountNo;
            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(m_DataSet);
            //  int nCount = Convert.ToInt32(dtTable.Rows[0][0]);

            //if (nCount > 0)
            //{
            nResultCode = 1;
            strResult = "Account No  already exist!";
            //    m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            //    return m_DataSet;
            //}

        }
        catch (OdbcException ex)
        {
            nResultCode = -1;
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


    [WebMethod]
    public DataSet CbnSubCategoryChak(int categoryId)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");
            m_CommandODBC = new OdbcCommand("exec cbn_subcategory_check ?", m_Connection.oCon);
            m_CommandODBC.CommandType = CommandType.Text;
            m_CommandODBC.Parameters.Add("@id", OdbcType.Int).Value = categoryId;
            nResultCode = Convert.ToInt32(m_CommandODBC.ExecuteScalar());
            strResult = "";
        }
        catch (OdbcException ex)
        {
            nResultCode = -1;
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

    // Invocie Genration

    #region Get Invoice Calcalution Details
    [WebMethod]
    public DataSet GetInvocieCalDetails()
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();
        DataTable m_DataTable = new DataTable("PropCalDetails");
        try
        {
            m_Connection.OpenDB("Galaxy");
            strTemp = " SELECT [id],[invoice_cal_serviceTax],[invoice_cal_Service_part],[invoice_cal_wct_tax],[invoice_cal_wct_value]" +
                        ",[invoice_cal_region],invoice_fuel_rate,[invoice_cal_type],upload_date FROM [crm_invocie_calculation_master] order by invoice_cal_region ";

            OdbcDataAdapter m_DataAdapter = new OdbcDataAdapter(strTemp, m_Connection.oCon);
            m_DataAdapter.Fill(m_DataSet);
            m_DataSet.Tables[0].TableName = "PropCalDetails";

            nResultCode = 0;
            strResult = "Pass";
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


    #region Insert UpdateFor Invoice Calculation
    [WebMethod]
    public DataSet InsertUpdateInvoiceCalculation(int id, string strPropType, string strfuelRate, string strServicePart, string strServiceTax, string strWCTValue, string strWCTTax,string strZone)
    {
        int nResultCode = -1;
        string strResult = "Fail - ";
        DataSet m_DataSet = new DataSet();

        try
        {
            m_Connection.OpenDB("Galaxy");

            string strTemp = "Select Count(*) as count from crm_invocie_calculation_master " +
                                " where invoice_cal_region = ? and invoice_cal_type = ? ";

            m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
            m_DataAdapterODBC = new OdbcDataAdapter(m_CommandODBC);
            m_CommandODBC.Parameters.Add("@strPropType", OdbcType.VarChar).Value = strPropType;
            m_CommandODBC.Parameters.Add("@region", OdbcType.VarChar).Value = strZone;

            DataTable dtTable = new DataTable("Count");
            m_DataAdapterODBC.Fill(dtTable);
            int nName = Convert.ToInt32(dtTable.Rows[0][0]);

            if (nName > 0 && id == 0) // checking the existence the record
            {
                nResultCode = -1;
                strResult = "Fuel Prices already exist!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            else if (id == 0) // for inserting the record
            {

                strTemp = "INSERT into crm_invocie_calculation_master (invoice_cal_type,invoice_fuel_rate,invoice_cal_serviceTax,invoice_cal_region,upload_date)" +
                          " Values (?,?,?,?,?)";

                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@strPropType", OdbcType.VarChar).Value = strPropType;
                m_CommandODBC.Parameters.Add("@strfuelRate", OdbcType.VarChar).Value = strfuelRate;
                m_CommandODBC.Parameters.Add("@strServiceTax", OdbcType.VarChar).Value = strServiceTax;
                m_CommandODBC.Parameters.Add("@Region", OdbcType.VarChar).Value = strZone;
                m_CommandODBC.Parameters.Add("@Date", OdbcType.VarChar).Value = DateTime.Now;
            }

            else  // for updating the record
            {
                strTemp = "UPDATE crm_invocie_calculation_master " +
                          "SET  invoice_cal_type = ?, invoice_fuel_rate = ?, invoice_cal_serviceTax= ? ,invoice_cal_region=?,upload_date=? " +
                           " Where id = " + id + "";

                m_CommandODBC = new OdbcCommand();
                m_CommandODBC = new OdbcCommand(strTemp, m_Connection.oCon);
                m_CommandODBC.Parameters.Add("@strPropType", OdbcType.VarChar).Value = strPropType;
                m_CommandODBC.Parameters.Add("@strfuelRate", OdbcType.VarChar).Value = strfuelRate;
                m_CommandODBC.Parameters.Add("@strServiceTax", OdbcType.VarChar).Value = strServiceTax;
                m_CommandODBC.Parameters.Add("@Region", OdbcType.VarChar).Value = strZone;
                m_CommandODBC.Parameters.Add("@Date", OdbcType.VarChar).Value = DateTime.Now;

            }

            m_CommandODBC.ExecuteNonQuery();

            if (id == 0)
            {
                nResultCode = 0;
                strResult = "Fuel Prices Added successfully!";
                m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
                return m_DataSet;
            }
            nResultCode = 0;
            strResult = "Fuel Prices Updated successfully!";
            m_DataSet.Tables.Add(m_Connection.GetResponseTable(nResultCode, strResult));
            return m_DataSet;
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

}

