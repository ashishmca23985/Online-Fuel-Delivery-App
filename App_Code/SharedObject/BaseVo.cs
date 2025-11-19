using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for BaseVo
/// </summary>
public class BaseVo
{
    public BaseVo()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int createdBy { get; set; }
    public string transactionNumber { get; set; }
    public DateTime _createdDate=DateTime.UtcNow;
    public DateTime createdDate
    {
        get
        {
            return _createdDate;
        }
        set { _createdDate = value; }
    }
    
    public string createdIp { get; set; }
    public int modifiedBy { get; set; }

    private DateTime _modifiedDate = DateTime.UtcNow;
    public DateTime modifiedDate
    {
        get { return _modifiedDate; }
        set { _modifiedDate = value; }
    }
    public string modifiedIp { get; set; }
    public string useable { get; set; }
    public DateTime _lastModifiedDate = DateTime.UtcNow;
    public DateTime lastModifiedDate
    {
        get
        {
            return _lastModifiedDate;
        }
        set
        {
            _lastModifiedDate = value;
        }
    }

}