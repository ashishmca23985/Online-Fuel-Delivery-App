using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Web;
public static class Writelog
{
    public static void WriteError(string errorMessage)
    {
        try
        {
            string szLogFile = String.Format("{0:ddMMyyyy}", DateTime.Now);
            string szLogPath = System.Configuration.ConfigurationManager.AppSettings["LogPath"];
            szLogPath += "//" + "CRM-" + szLogFile + ".Log";
            string LogLine = String.Format("{0:hh:mm:ss}", DateTime.Now) + "\t" + HttpContext.Current.Request.UserHostAddress + "\t" + errorMessage;
            StreamWriter sw = new StreamWriter(szLogPath, true);
            sw.WriteLine(LogLine);
            sw.Flush();
            sw.Close();
        }
        catch (Exception)
        {
            // WriteError(ex.Message);
        }
    }
}


