using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using System.Text;

namespace CtiWS
{
    public class Logging
    {
        public void AddLog(string LogLine, string strType)
        {
            string szLogFile = String.Format("{0:ddMMyyyy}", DateTime.Now);
            string szLogPath = ConfigurationManager.AppSettings["LOGPATH"];
            szLogPath += "//" + szLogFile + ".Log";

            LogLine = String.Format("{0:hh:mm:ss}", DateTime.Now) + "\t" + strType + "\t" + LogLine;

            StreamWriter sw = new StreamWriter(szLogPath, true);
            sw.WriteLine(LogLine);
            sw.Flush();
            sw.Close();
        }

        public void AddLog(string strMethod, string strMessage, string strIP)
        {
            string szLogPath = ConfigurationManager.AppSettings["LOGPATH"] + "//" + String.Format("{0:ddMMyyyy}", DateTime.Now);
            if (!Directory.Exists(szLogPath))
            {
                Directory.CreateDirectory(szLogPath);
            }
            szLogPath += "//" + strIP + ".Log";
            string LogLine = String.Format("{0:hh:mm:ss}", DateTime.Now) + "\t" + strMethod + "\t" + strMessage;
            StreamWriter sw = new StreamWriter(szLogPath, true);
            sw.WriteLine(LogLine);
            sw.Flush();
            sw.Close();
        }

      
    }
}
