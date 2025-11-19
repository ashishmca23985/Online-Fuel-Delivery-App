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
using Telerik.Web.UI;

/// <summary>
/// Summary description for bal_MasterUserPage
/// </summary>
public class bal_MasterUserPage
{
	public bal_MasterUserPage()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public bo_MasterUserPage MasterUserPage(ref RadGrid grvMUP, bo_MasterUserPage MasterUserPage)
    {
        dal_MasterUserPage MasterUserPageDetailDAL = new dal_MasterUserPage();
        int nResultCode = 0;
        try
        {
            DataTable dt = new DataTable();
            dt = MasterUserPageDetailDAL.MasterUserPageSearDetails(MasterUserPage);
            grvMUP.DataSource = dt;
          //  grvMUP.DataBind();
            MasterUserPage.ResultCode = nResultCode;
            return MasterUserPage;
        }

        catch (Exception ex)
        {
            MasterUserPage.ResultCode = -1;
            MasterUserPage.ResultString = ex.Message;
            MasterUserPage.ErrorSource = ex.Source;
            MasterUserPage.ErrorStackTrace = ex.StackTrace;
            WritelogFile.WriteError("BAL:MasterUserPage,DAl:MasterUserPageSearDetails", ex.Message, ex.Source, ex.StackTrace, Convert.ToString(ex.GetType()));
            return MasterUserPage;
        }
        finally
        {
            MasterUserPageDetailDAL = null;
        }
    }
    public int Insert(bo_MasterUserPage MasterUserPage)
    {
        dal_MasterUserPage MasterUserPageDetailDAL = new dal_MasterUserPage();
        try
        {
            return MasterUserPage.OutPutUserPage = MasterUserPageDetailDAL.insupDelMasterUserPage(MasterUserPage);
        }
        catch (Exception ex)
        {
            MasterUserPage.ResultCode = -1;
            MasterUserPage.ResultString = ex.Message;
            MasterUserPage.ErrorSource = ex.Source;
            MasterUserPage.ErrorStackTrace = ex.StackTrace;
            WritelogFile.WriteError("BAL:Insert,DAl:insupDelMasterUserPage", ex.Message, ex.Source, ex.StackTrace, Convert.ToString(ex.GetType()));
            return 0;
        }
        finally
        {
            MasterUserPageDetailDAL = null;
        }
    }

    public int Update(bo_MasterUserPage MasterUserPage)
    {
        dal_MasterUserPage MasterUserPageDetailDAL = new dal_MasterUserPage();
        try
        {
            return MasterUserPageDetailDAL.insupDelMasterUserPage(MasterUserPage);
        }
        catch (Exception ex)
        {
            MasterUserPage.ResultCode = -1;
            MasterUserPage.ResultString = ex.Message;
            MasterUserPage.ErrorSource = ex.Source;
            MasterUserPage.ErrorStackTrace = ex.StackTrace;
            WritelogFile.WriteError("BAL:Update,DAl:insupDelMasterUserPage", ex.Message, ex.Source, ex.StackTrace, Convert.ToString(ex.GetType()));
            return 0;
        }
        finally
        {
            MasterUserPageDetailDAL = null;
        }
    }

    public bo_MasterUserPage FillDropDown(ref RadComboBox ddlMUP, bo_MasterUserPage MasterUserPage)
    {
        dal_MasterUserPage MasterUserPageDetailDAL = new dal_MasterUserPage();
        int nResultCode = 0;
        try
        {
            DataTable dt = new DataTable();
            dt = MasterUserPageDetailDAL.GetRolId(MasterUserPage);
            ddlMUP.DataSource = dt;
            if(MasterUserPage.ForInterdailogCon != "ConInter")
            {
            ddlMUP.DataValueField = "role_id";
            ddlMUP.DataTextField = "role_name";
            }
            else
            {
            ddlMUP.DataValueField = "agent_agent_id";
            ddlMUP.DataTextField = "agent_login_id";
            }
            ddlMUP.DataBind();            
            MasterUserPage.ResultCode = nResultCode;
            return MasterUserPage;
        }
        catch (Exception ex)
        {
            MasterUserPage.ResultCode = -1;
            MasterUserPage.ResultString = ex.Message;
            MasterUserPage.ErrorSource = ex.Source;
            MasterUserPage.ErrorStackTrace = ex.StackTrace;
            WritelogFile.WriteError("BAL:MasterUserPage,DAl:MasterUserPageSearDetails", ex.Message, ex.Source, ex.StackTrace, Convert.ToString(ex.GetType()));
            return MasterUserPage;
        }
        finally
        {
            MasterUserPageDetailDAL = null;
        }
    }
    public bo_MasterUserPage MasterCTIAgentAlredyExist(ref DataTable dt, bo_MasterUserPage MasterUserPage)
    {
        dal_MasterUserPage MasterUserPageDetailDAL = new dal_MasterUserPage();
        int nResultCode = 0;
        try
        {
            
            dt = MasterUserPageDetailDAL.GetCTIAgentAlredyExist(MasterUserPage);           
            MasterUserPage.ResultCode = nResultCode;
            return MasterUserPage;
        }
        catch (Exception ex)
        {
            MasterUserPage.ResultCode = -1;
            MasterUserPage.ResultString = ex.Message;
            MasterUserPage.ErrorSource = ex.Source;
            MasterUserPage.ErrorStackTrace = ex.StackTrace;
            WritelogFile.WriteError("BAL:MasterUserPage,DAl:MasterUserPageSearDetails", ex.Message, ex.Source, ex.StackTrace, Convert.ToString(ex.GetType()));
            return MasterUserPage;
        }
        finally
        {
            MasterUserPageDetailDAL = null;
        }
     }
}
