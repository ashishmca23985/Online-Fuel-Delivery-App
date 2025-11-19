using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Data.Odbc;

/// <summary>
/// Summary description for SaveData
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.ComponentModel.ToolboxItem(false)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]

public class SaveData : System.Web.Services.WebService
{
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string Save(List<DataField> crmfields)
    {
        string result = "Fail";
        try
        {
            DataBase con = new DataBase();
            con.OpenDB("IDGDB");

            string actiontype = crmfields[0].Value;
            string tablename = crmfields[1].Value;
            string tblCallmaster = crmfields[2].Value;

            DataTable dtStructure = new DataTable("dtStructure");
            OdbcCommand cmd = new OdbcCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con.oCon;
            bool phoneindex = false;

            if (actiontype == "insert")
            {
                string strfield = "(service_id,call_number,lead_id,cli,start_time,end_time,callback_datetime,agent,disposition,comments";
                string strval = ") values(?,?,?,?,?,?,?,?,?,?";

                /* added by ashish to get data_type of Table Structure */
                string[] strTableName = tablename.Split('.');
                string strfieldDataType = "select column_name,data_type from information_schema.columns where table_name = '" + strTableName[1].ToString() + "' ";
                OdbcDataAdapter adStructure = new OdbcDataAdapter(strfieldDataType, con.oCon);
                adStructure.Fill(dtStructure);

                cmd.Parameters.AddWithValue("@ServiceId", crmfields[2].Value);
                cmd.Parameters.AddWithValue("@CallId", crmfields[4].Value);
                cmd.Parameters.AddWithValue("@ClientID", crmfields[5].Value);
                cmd.Parameters.AddWithValue("@DialedPhone", crmfields[6].Value);
                cmd.Parameters.AddWithValue("@CallStartTime", crmfields[7].Value);
                cmd.Parameters.AddWithValue("@CallEndTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                if (crmfields[8].Value.Length > 10)
                    cmd.Parameters.AddWithValue("@CallBackDateTime", Convert.ToDateTime(crmfields[8].Value).ToString("yyyy-MM-dd HH:mm:ss"));
                else
                    cmd.Parameters.AddWithValue("@CallBackDateTime", DBNull.Value);
                cmd.Parameters.AddWithValue("@AgentID", crmfields[9].Value);
                cmd.Parameters.AddWithValue("@Disposition", crmfields[10].Value);
                cmd.Parameters.AddWithValue("@Comment", crmfields[11].Value);


                for (int i = 12; i < crmfields.Count; i++ )
                {
                    if (crmfields[i].Name != "lead_phone_index" && crmfields[i].Name != "lead_phone_index_value" && crmfields[i].Name != "lead_phone_index_name")
                    {
                        DataField field = crmfields[i];
                        bool added = false;
                        strfield += "," + field.Name;
                        strval += ",?";
                        /* added by ashish to convert datetime data_type in varchar */
                        foreach (DataRow col_name in dtStructure.Rows)
                        {
                            if (col_name.ItemArray[0].ToString() == field.Name.ToString())
                            {
                                if (col_name.ItemArray[1].ToString() == "datetime")
                                {
                                    added = true;
                                    cmd.Parameters.AddWithValue("@" + field.Name, Convert.ToDateTime(field.Value).ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                            }
                        }
                        if (!added)
                            cmd.Parameters.AddWithValue("@" + field.Name, field.Value);
                    }
                    else
                        phoneindex = true;
                    
                    cmd.CommandText = "INSERT INTO " + tablename + strfield + strval + ")";
                
            }
                int count = Convert.ToInt32(cmd.ExecuteNonQuery());
                if (count == 1)
                    result = "Success";
                if (phoneindex)
                {
                    // update lead_phone_index,lead_phone_value,lead_phone in cti_lead_master table
                    OdbcCommand odcmd1 = new OdbcCommand();
                    odcmd1.CommandType = CommandType.Text;
                    odcmd1.Connection = con.oCon;
                    odcmd1.CommandText = "UPDATE " + con.DB_TOPU_SQL + " " + crmfields[3].Value + " SET ";
                    string strphonefield = "";
                    string strphonevalue="";
                    for (int j = 12; j < crmfields.Count; j++)
                    {
                        if (crmfields[j].Name == "lead_phone_index")
                        {
                            odcmd1.CommandText += " lead_phone_index = convert(smallint," + crmfields[j].Value+")";
                        }
                        if (crmfields[j].Name == "lead_phone_index_value")
                        {
                            odcmd1.CommandText += ",lead_phone = '" + crmfields[j].Value + "'";
                            strphonevalue = crmfields[j].Value;
                        }
                        if (crmfields[j].Name == "lead_phone_index_name")
                        {
                            strphonefield = crmfields[j].Value; 
                        }
                    }
                    if (strphonevalue != "" && strphonefield != "")
                    {
                        odcmd1.CommandText += "," + strphonefield + " = '" + strphonevalue + "'";
                    }
                    odcmd1.CommandText += " WHERE lead_id=" + crmfields[5].Value + "" + con.DB_TOP_MYSQL;

                    int ncount1 = Convert.ToInt32(odcmd1.ExecuteNonQuery());
                    if (ncount1 == 1)
                        result = "Success";
                }
            }
           
            // added by ashish for UPDATE Lead master table in case of field_import = 'Y' AND field_data_capture = 'Y' in CTI_lead structure.
            string strsql = "SELECT " +
                  "field_id,field_name,field_display_name,field_data_type," +
                   con.DB_NULL + "(field_data_min,'') as field_data_min," +
                   con.DB_NULL + "(field_data_max,'') as field_data_max," +
                  "field_compulsory,field_readonly,field_import,field_data_capture,field_dropdown_lookup,field_script ," +
                   con.DB_NULL + "(field_row,'') as field_row," +
                   con.DB_NULL + "(field_col,'') as field_col " +
                 "FROM cti_service_fields " +
                 "WHERE field_import = 'Y' AND field_data_capture = 'Y' AND field_service_id=" + crmfields[2].Value + " " +
                 "ORDER BY field_row,field_col,field_id";

            DataTable dtLeadMaster = new DataTable();
            OdbcDataAdapter adStr = new OdbcDataAdapter(strsql, con.oCon);
            adStr.Fill(dtLeadMaster);

            bool flage = false;
            OdbcCommand odcmd = new OdbcCommand();
            odcmd.CommandType = CommandType.Text;
            odcmd.Connection = con.oCon;

            if (dtLeadMaster.Rows.Count > 0) // && crmfields[12].Value != "" || crmfields[12].Value != null
            {
                odcmd.CommandText = "UPDATE " + con.DB_TOPU_SQL + " " + crmfields[3].Value + " SET ";
                for (int i = 0; i < dtLeadMaster.Rows.Count; i++)
                {
                    for (int k = 0; k < crmfields.Count; k++)
                    {
                        DataField field = crmfields[k];
                        if (field.Name == dtLeadMaster.Rows[i]["field_name"].ToString())
                        {
                            if (flage)
                                odcmd.CommandText += "," + field.Name + "= ?";
                            else
                                odcmd.CommandText += field.Name + "= ?";
                            odcmd.Parameters.AddWithValue("@" + field.Name, field.Value);
                            flage = true;
                            break;
                        }
                    }
                }
                odcmd.CommandText += " WHERE lead_id=" + crmfields[5].Value + " " + con.DB_TOP_MYSQL;
                int ncount = Convert.ToInt32(odcmd.ExecuteNonQuery());
                if (ncount == 1)
                    result = "Success";
                con.CloseDB();
            }
        }
        catch (OdbcException ex)
        {
            result = ex.Message;
        }
        return result;
    }
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<DataField> Lookup(List<DataField> filters)
    {
        System.Collections.Generic.List<DataField> data = new List<DataField>();
        try
        {
            DataBase con = new DataBase();
            con.OpenDB("IDGDB");
            DataTable dtLookup = new DataTable("Lookup");
            string strsql = "";
            if (filters[0].Value == "DISPOSITIONS")
            {
                strsql = "SELECT servdesp_desp_desc as lookup_value," +
                            "servdesp_desp_code+'|'+servdesp_bucket_code as lookup_code " +
                              " FROM cti_service_desposition " +
                              "WHERE servdesp_service_id=" + filters[2].Value + " ";
                if (filters[1].Value.Length > 0)
                    strsql += "AND servdesp_parent_desp_code='" + filters[1].Value + "' ";
                strsql += "ORDER BY servdesp_desp_desc";
            }
            else
            {
                strsql = "SELECT param_field_value as lookup_value, param_field_value as lookup_code " +
                              "FROM cti_service_parameters " +
                              "WHERE param_field_name='" + filters[0].Value + "' " +
                    //"AND ifnull(param_field_value2,'')='" + filters[1].Value + "' " +
                              "ORDER BY param_field_value";
            }
            OdbcDataAdapter ad = new OdbcDataAdapter(strsql, con.oCon);
            ad.Fill(dtLookup);
            foreach (DataRow row in dtLookup.Rows)
            {
                data.Add(new DataField() { Name = row["lookup_value"].ToString(), Value = row["lookup_code"].ToString() });
            }
            con.CloseDB();
        }
        catch (OdbcException ex)
        {
            string aa = ex.Message;
        }
        return data;
    }

}

public class DataField
{
    public string Name { get; set; }
    public string Value { get; set; }
}


