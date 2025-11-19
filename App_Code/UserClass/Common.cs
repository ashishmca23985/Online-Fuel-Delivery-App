using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Telerik.Web.UI;

/// <summary>
/// Summary description for Common
/// </summary>
public class Common
{
    public Common()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public static bool CheckResponce(DataSet objDataSet)
    {
        if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
        {
            LogMessage(objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
            return false;
        }
        else
            return true;
        //else
        //{
        //    LogMessage(objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 2);
        //}
    }
    public static void CheckResponce(DataSet objDataSet, bool status)
    {
        if (Convert.ToInt32(objDataSet.Tables["Response"].Rows[0]["ResultCode"]) != 0)
        {
            LogMessage(objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 1);
            return;
        }
        else if (status)
        {
            LogMessage(objDataSet.Tables["Response"].Rows[0]["ResultString"].ToString(), 2);
        }
    }

    #region Log Message
    public static void LogMessage(string Message, int param)
    {
        Page page = (Page)HttpContext.Current.Handler;
        Label lblMessage = (Label)page.FindControl("lblMessage");
        if (lblMessage != null)
        {
            lblMessage.LogMessage(Message, param);
        }
    }
    #endregion
}
public static class Extensions
{
    #region DropDownList Common function
    public static void dropDownListBind(this DropDownList ddl, DataSet ds, string textFild, string textValue, int index, string text)
    {
        try
        {
            ddl.dropDownListBind(ds, textFild, textValue, index);
            ddl.Items.Insert(0, text);
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex.Message, 1);
        }
    }
    public static void dropDownListBind(this DropDownList ddl, DataSet ds, string textFild, string textValue, int index)
    {
        try
        {
            if (ddl != null)
                ddl.Items.Clear();
            Common.CheckResponce(ds);
            if (ds.Tables[index].Rows.Count > 0)
            {
                ddl.DataTextField = textFild;
                ddl.DataValueField = textValue;
                ddl.DataSource = ds.Tables[index];
                ddl.DataBind();
            }
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex.Message, 1);
        }
    }
    public static void radComboBoxBindALL(this RadComboBox ddl, DataSet ds, string textFild, string textValue, int index)
    {
        try
        {
            ddl.radComboBoxBind(ds, textFild, textValue, index);
            RadComboBoxItem lstItem = new RadComboBoxItem("All", "0");
            ddl.Items.Insert(0, lstItem);
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex.Message, 1);
        }
    }
    public static void radComboBoxBindBlanck(this RadComboBox ddl, DataSet ds, string textFild, string textValue, int index)
    {
        try
        {
            ddl.radComboBoxBind(ds, textFild, textValue, index);
            RadComboBoxItem lstItem = new RadComboBoxItem("", "0");
            ddl.Items.Insert(0, lstItem);
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex.Message, 1);
        }
    }
    public static void radComboBoxBind(this RadComboBox ddl, DataSet ds, string textFild, string textValue, int index, string text)
    {
        try
        {
            ddl.radComboBoxBind(ds, textFild, textValue, index);
            RadComboBoxItem lstItem = new RadComboBoxItem(text, "0");
            ddl.Items.Insert(0, lstItem);
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex.Message, 1);
        }
    }
    public static void radComboBoxBind(this RadComboBox ddl, DataSet ds, string textFild, string textValue, string tableName, string text)
    {
        try
        {
            ddl.radComboBoxBind(ds, textFild, textValue, tableName);
            RadComboBoxItem lstItem = new RadComboBoxItem(text, "0");
            ddl.Items.Insert(0, lstItem);
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex.Message, 1);
        }
    }
    public static void radComboBoxBind(this RadComboBox ddl, DataSet ds, string textFild, string textValue, int index)
    {
        try
        {
            if (ddl != null)
                ddl.Items.Clear();
            Common.CheckResponce(ds);
            if (ds.Tables[index].Rows.Count > 0)
            {
                ddl.DataTextField = textFild;
                ddl.DataValueField = textValue;
                ddl.DataSource = ds.Tables[index];
                ddl.DataBind();
            }
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex.Message, 1);
        }
    }
    public static void radComboBoxBind(this RadComboBox ddl, DataSet ds, string textFild, string textValue, string tableName)
    {
        try
        {
            if (ddl != null)
                ddl.Items.Clear();
            Common.CheckResponce(ds);
            if (ds.Tables[tableName].Rows.Count > 0)
            {
                ddl.DataTextField = textFild;
                ddl.DataValueField = textValue;
                ddl.DataSource = ds.Tables[tableName];
                ddl.DataBind();
            }
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region RadGridView Bind common Function
    public static void radRadGrid(this RadGrid rgrid, DataTable dt)
    {
        try
        {
            rgrid.DataSource = dt;
            rgrid.DataBind();

        }
        catch (Exception ex)
        {
            Common.LogMessage(ex.Message, 1);
        }
    }
    public static void radRadGrid(this RadGrid rgrid, DataSet ds, int index)
    {
        try
        {
            if (Common.CheckResponce(ds))
            {

                rgrid.DataSource = ds.Tables[index];
                rgrid.DataBind();
            }
        }
        catch (Exception ex)
        {
            Common.LogMessage(ex.Message, 1);
        }
    }
    public static void radRadGrid(this RadGrid rgrid, DataSet ds, string tableName)
    {
        try
        {
            if (Common.CheckResponce(ds))
            {

                rgrid.DataSource = ds.Tables[tableName];
                rgrid.DataBind();
            }

        }
        catch (Exception ex)
        {
            Common.LogMessage(ex.Message, 1);
        }
    }
    #endregion

    #region Log Message
    public static void LogMessage(this Label lblMessage, Exception e, int param)
    {
        lblMessage.LogMessage(e.Message, param);
    }
    public static void LogMessage(this Label lblMessage, string Message, int param)
    {
        lblMessage.Text = Message;
        if (param == 1)
            lblMessage.CssClass = "error";
        else
            lblMessage.CssClass = "success";
    }
    #endregion
}
public static class Helpter
{
    public static string MinutToStringConvert(int minute)
    {
        string strminute = "";
        if (minute > 0)
        {
            if (minute >= 1440)
                strminute = (minute / 1440).ToString() + ":" + ((minute % 1440) / 60).ToString("00") + ":" + ((minute % 1440) % 60).ToString("00");
            else
                strminute = (minute / 60).ToString() + ":" + (minute % 60).ToString("00");
        }
        return strminute;
    }
    public static int IntimeOutTimeDiff(int intime, int outtime)
    {
        int minute = 0;
        int dhour = ((outtime / 100) - (intime / 100));
        int dminutes = ((outtime % 100) - (intime % 100));
        if (dminutes < 0)
        {
            dhour += -1;
            dminutes = 60 + dminutes;
        }
        minute = (dhour * 60) + dminutes;
        return minute;
    }
    public static string NullEmpltyRetNA(string obj)
    {
        if (obj == null || obj == "")
            return "NA";
        else
            return obj;
    }
    public static DateTime MonthStartDate(DateTime dt)
    {
        return dt.AddDays(-dt.Day + 1);
    }
    public static DateTime MonthLastDate(DateTime dt)
    {
        return (new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59));
    }

    public static string kendotheme(string name)
    {
        string theme = "";
        switch (name.ToString().ToLower())
        {
            case "green":
                theme = "highcontrast";
                break;
            case "orange":
                theme = "metroblack";
                break;
            case "WebBlue":
                theme = "blueopal";
                break;
            case "pink":
                theme = "moonlight";
                break;
            default:
                theme = "default";
                break;
        }
        return theme;
    }

    public static string AccountEncrypt(string account)
    {
        string newaccount = "";
        char[] array = account.ToCharArray();
        for (int i = 0; i < array.Length; i++)
        {
            if (i < (array.Length - 4))
            {
                newaccount += "x";
            }
            else
            {
                newaccount += array[i];
            }
        }
     return newaccount;
    }
    public static string getBetween(string strSource, string strStart, string strEnd)
    {
        int Start, End;
        if (strSource.Contains(strStart) && strSource.Contains(strEnd))
        {
            Start = strSource.IndexOf(strStart, 0) + strStart.Length;
            End = strSource.IndexOf(strEnd, Start);
            return strSource.Substring(Start, End - Start);
        }
        else
        {
            return "";
        }
    }
}

