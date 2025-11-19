using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

/*
 *  JSON is Javascript Object Notation, a standard way of serializing objects in Javascript and
 *  other languages.  For more information see http://www.json.org/.
 * 
 *  JSONP is a technique to enable the execution of Javascript that resides in a different domain.  It 
 *  works by exploiting the exception granted to the <script> tag which allows content to be loaded
 *  from a different domain.  By contrast, making "regular" AJAX calls to a different domain will
 *  not work, usually throwing an "Access Denied" or "No Transport" error.
 *  
 *  JSONP (the "P" stands for "Padding") is regular JSON wrapped in a Javascript function call (the
 *  "Padding").  Take for example this standard JSON object:
 *      { "Name" : "John", "Age" : 14, "Gender" : "Male" }
 *      
 *  JSONP will turn that JSON into a valid Javascript function call by using the JSON as an argument
 *  to the callback function provided by the caller.  For example, if the caller provides a callback
 *  value of 'processResults', the resulting JSONP looks like this:
 *      processResults({ "Name" : "John", "Age" : 14, "Gender" : "Male" });
 *      
 *  The processResults() function will then be able to use the JSON object just like a regular object.
 *  Note that the callback function must be implemented on the page that receives the JSONP, otherwise
 *  a standard Javascript error will occur.
 *  
 *  The real "trick" to cross-domain script execution is dynamically creating a "script" tag on the
 *  client for every JSONP request, using the web service URL as the "src" attribute.  This will cause
 *  the browser to automatically download and execute the script that is loaded from the URL,
 *  effectively bypassing the same-domain origin policy.
 */
public static class JsonPUtility
{
    /*
     * SendJsonP(string callback, string json)
     *  
     *  This method takes the provided 'json' string, wraps it so that it is a parameter to the 'callback'
     *  function, clears any existing response text, writes the resulting Javascript code to the 
     *  response, and ends the response.
     *  
     *  For example, given these two parameters...
     *      callback    = "callbackFunction"
     *      json        = "{ 'FOO': 'BAR', 'JOO': 'MAR' }"
     *  
     *  ... the following code is returned to the client in an HTTP response with a content-type of
     *  'application/javascript':
     *      callbackFunction({ 'FOO': 'BAR', 'JOO': 'MAR' });
     *      
     */
    public static void SendJsonP(string callback, string json)
    {
        // Clear any response that has already been prepared.
        HttpContext.Current.Response.Clear();

        // Set the content type to javascript, since we are technically returning Javascript code.
        HttpContext.Current.Response.ContentType = "application/javascript";

        // Create a function call by wrapping the JSON with the callback function name.
        HttpContext.Current.Response.Write(String.Format("{0}({1});", callback, json));

        // Complete this request, to prevent the ASMX web service from doing anything else.
        HttpContext.Current.ApplicationInstance.CompleteRequest();
    }

    /*
     * bool IsJsonPRequest()
     * 
     *  Determines whether or not the current request is for JSONP javascript code.
     *  
     *  This is the criteria for making a JSONP request to this web service:
     *      1. Include the jsonp parameter.  Its value is not important - we recommend using jsonp=true
     *         to increase clarity.
     *      2. Include the callback=string parameter so we know what function call to wrap around
     *         the requested JSON.
     */
    public static bool IsJsonPRequest()
    {
        // Store the context to the current request.
        var request = HttpContext.Current.Request;

        // If a 'jsonp' or a 'callback' parameter was not provided, this isn't a JSONP request.
        //if (request.QueryString["jsonp"] == null || String.IsNullOrEmpty(request.QueryString["callback"]))
        //    return false;
        if (String.IsNullOrEmpty(request.QueryString["callback"]))
            return false;
        // Since both parameters were provided, this is a jsonp request.
        return true;
    }

    /*
     * ProcessJsonPRequest()
     * 
     *  Manual processing is required for JSONP requests due to limitations in ASMX web services.
     */
    public static void ProcessJsonPRequest()
    {
        
        // If this isn't a JSONP request, simply return and continue regular request processing.
        if (!IsJsonPRequest())
            return;

        // Store the context to the HTTP request.
        var request = HttpContext.Current.Request;

        // Store the callback function that will be wrapped around the JSON string.
        string callback = request.QueryString["callback"];

        // Create a place to store the object that will be serialized into JSON.
        object objectForJson = null;

        // Store the web service method name that is being requested.  It is always going to follow the
        // final slash after the .asmx extension, and will continue until the question mark that marks
        // the query string.
        int methodNameStartIndex = request.RawUrl.ToUpper().IndexOf(".ASMX/") + 6;
        int methodNameLength = (request.RawUrl.IndexOf("?")) - methodNameStartIndex;
        string requestMethod = request.RawUrl.Substring(methodNameStartIndex, methodNameLength);

        // Create a place to store the string ID of the object that is going to be looked-up.
        string lookupId = null;

        // Based on the request URL, figure out the method that will create a reference for the objectForJson variable.
        MasterWS masterWS = new MasterWS();
        switch (requestMethod)
        {
           
        
            case "CaseEditList":
                DateTime fromdate = Convert.ToDateTime(request.QueryString["fromdate"]);
                DateTime todate = Convert.ToDateTime(request.QueryString["toDate"]);
                int timespan = Convert.ToInt32(request.QueryString["timespan"]);
                int userid = Convert.ToInt32(request.QueryString["userid"]);
                int LocationId = Convert.ToInt32(request.QueryString["LocationId"]);
                int TeamId = Convert.ToInt32(request.QueryString["TeamId"]);
                int CategoryId = Convert.ToInt32(request.QueryString["CategoryId"]);
                int OwnerId = Convert.ToInt32(request.QueryString["OwnerId"]);
                string CaseNo = Convert.ToString(request.QueryString["CaseNo"]);
                objectForJson = masterWS.CaseEditList(fromdate, todate, userid,timespan,LocationId,TeamId,CategoryId,OwnerId,CaseNo);
                break;
            case "CaseUpdate":
                lookupId = request.QueryString["models"];
                if (!String.IsNullOrEmpty(lookupId))
                    objectForJson = masterWS.CaseUpdate(JsonConvert.DeserializeObject<List<object>>(lookupId));
                break;
            case "GeneralList":
                lookupId = request.QueryString["opfId"];
                if (!String.IsNullOrEmpty(lookupId))
                    objectForJson = masterWS.GeneralList(lookupId);
                break;
            case "InsertVisitor":
                ChatWS chatws = new ChatWS();
                lookupId = request.QueryString["type"];
                objectForJson = chatws.InsertVisitor(lookupId,"","","",0);
                break;
            //case "CustomerDetails":
            //    lookupId = request.QueryString["opfId"];
            //    if (!String.IsNullOrEmpty(lookupId))
            //        objectForJson = masterWS.CustomerDetails(1);
            //    break;
            //case "GetManager":
            //    // Get the manager's ID from the query string.
            //    lookupId = request.QueryString["managerId"];

            //    // If the manager ID was provided, get a Manager object.
            //    if (!String.IsNullOrEmpty(lookupId))
            //        objectForJson = Factory.GetManager(lookupId);

            //    break;

            //case "GetOrder":
            //    // Get the order ID from the query string.
            //    lookupId = request.QueryString["orderId"];

            //    // If the order ID was provided, get the  object.
            //    if (!String.IsNullOrEmpty(lookupId))
            //        objectForJson = Factory.GetOrder(lookupId);

            //    break;

            default:
                // If the request method wasn't handled, throw an exception.
                throw new ArgumentException("Unknown request method '" + requestMethod + "'.");
        }

        // Create a .NET framework object to serialize the object into JSON.
        JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

        // Serialize the object into JSON.  If objectForJson is null, the callback function will be passed a parameter of null (e.g. callback(null)).
        string json = jsonSerializer.Serialize(objectForJson);

        // Send the JSONP string back to the caller.
        SendJsonP(callback, json);
    }
}

public class ImageConvert
{
    public Image Base64ToImage(string base64String)
    {
        // Convert Base64 String to byte[]
        byte[] imageBytes = Convert.FromBase64String(base64String);
        MemoryStream ms = new MemoryStream(imageBytes, 0,
          imageBytes.Length);

        // Convert byte[] to Image
        ms.Write(imageBytes, 0, imageBytes.Length);
        Image image = Image.FromStream(ms, true);
        return image;
    }
}
