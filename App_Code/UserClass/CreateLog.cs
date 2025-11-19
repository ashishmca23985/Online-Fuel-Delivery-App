using System;
using System.IO;
using System.Text;
using System.Configuration;

public class CreateLog
{
    private string szLogFormat;
    private string szErrorTime;
    //		private string sLogFileName;
    public CreateLog()
    {
    }

    public string CreateLogFileName(int szType, string szUser)
    {
        return "";
    }

    public void ErrorLog(string szErrMsg)
    {
        string szPath = "";
        try
        {
            szPath = Convert.ToString(ConfigurationManager.AppSettings["LogPath"]);
            if (!Directory.Exists(szPath))
            {
                Directory.CreateDirectory(szPath);
            }            
            //szLogFormat used to create log files format :
            // dd/mm/yyyy hh:mm:ss AM/PM ==> Log Message
            szLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + "   ";

            //this variable used to create log filename format "
            //for example filename : ErrorLogYYYYMMDD
            string sYear = DateTime.Now.Year.ToString();
            string sMonth = DateTime.Now.Month.ToString();
            string sDay = DateTime.Now.Day.ToString();
            szErrorTime = sYear + sMonth + sDay;

            StreamWriter sw = new StreamWriter(szPath + szErrorTime + ".LOG", true);
            sw.WriteLine(szLogFormat + szErrMsg);
            sw.Flush();
            sw.Close();
        }
        catch(Exception ex)
        {
            string str = ex.Message;
        }
    }
}