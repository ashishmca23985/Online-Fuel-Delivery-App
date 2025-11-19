using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Data;
using System.Net;
using System.Web.Script.Services;
using SharedVO;


/// <summary>
/// Summary description for QuoteWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[ScriptService]
public class QuoteWebService : WebService
{
    private Random random;
    private string yahooFeedTemplate = "http://finance.yahoo.com/d/quotes.csv?s={0}&f=sl1d1t1c1ohgv&e=.csv";

    private string SessionQuotesData
    {
        get { return (string)HttpContext.Current.Session["TelerikAspAjaxGridApplication"]; }
        set { HttpContext.Current.Session["TelerikAspAjaxGridApplication"] = value; }
    }

    public QuoteWebService()
    {
        random = new Random(DateTime.Now.Millisecond);
    }

    [WebMethod]
    public string HelloWorld()
    {
        return "Hello World";
    }

    [WebMethod(), Description("Gets DataSet of stock quote for a specific symbol")]
    public DataSet GetDSOfStockQuotes()
    {
        string stockTickers = "MSFT, YHOO, GOOG, HPQ, DELL, AAPL, NOVL";
        string[] stockTickerList = stockTickers.Split(',');
        int index;
        for (index = 0; index <= stockTickerList.Length - 1; index++)
        {
            stockTickerList[index] = stockTickerList[index].Trim();
        }

        List<Quote> quotes = new List<Quote>();
        if (SessionQuotesData != null)
        {
            quotes = GetQuotesData(SessionQuotesData);
            foreach (Quote quote in quotes)
            {
                ModifyQuote(quote);
            }
        }
        else
        {
            string url = string.Empty;
            WebRequest webRequest = null;
            WebResponse webResponse = null;
            foreach (string stockTicker in stockTickerList)
            {
                url = string.Format(yahooFeedTemplate, HttpUtility.UrlEncode(stockTicker));
                webRequest = HttpWebRequest.Create(url);
                webResponse = webRequest.GetResponse();

                Quote quote = GetQuoteData(webResponse.GetResponseStream());
                quotes.Add(quote);
            }
        }

        SerializeQuotesData(quotes);

        DataSet dataset = new DataSet("QuotesDataSet");
        XmlReader data = XmlReader.Create(new System.IO.StringReader(SessionQuotesData));
        dataset.ReadXml(data);
        return dataset;
    }

    [WebMethod(EnableSession = true), Description("Gets list of stock quote for a specific symbol")]
    public List<Quote> GetListOfStockQuotes()
    {
        string stockTickers = "MSFT, YHOO, GOOG, HPQ, DELL, AAPL, NOVL";
        string[] stockTickerList = stockTickers.Split(',');
        int index;
        for (index = 0; index <= stockTickerList.Length - 1; index++)
        {
            stockTickerList[index] = stockTickerList[index].Trim();
        }

        List<Quote> quotes = new List<Quote>();
        if (SessionQuotesData != null)
        {
            quotes = GetQuotesData(SessionQuotesData);
            foreach (Quote quote in quotes)
            {
                ModifyQuote(quote);
            }
        }
        else
        {
            string url = string.Empty;
            WebRequest webRequest = null;
            WebResponse webResponse = null;
            foreach (string stockTicker in stockTickerList)
            {
                url = string.Format(yahooFeedTemplate, HttpUtility.UrlEncode(stockTicker));
                webRequest = HttpWebRequest.Create(url);
                webResponse = webRequest.GetResponse();

                Quote quote = GetQuoteData(webResponse.GetResponseStream());
                if (quote != null)
                {
                    quotes.Add(quote);
                }
            }
        }

        SerializeQuotesData(quotes);

        return quotes;
    }

    private void ModifyQuote(Quote quote)
    {
        Decimal oldQuote = quote.StockQuote;
        Decimal change = (Decimal)random.NextDouble();

        if (change != 0)
        {
            change = Decimal.Round(change, random.Next(1, 5));
        }

        if (oldQuote > change && random.NextDouble() <= 0.5)
        {
            change = -change;
        }

        quote.Change = change;
        quote.StockQuote += change;
        quote.LastUpdated = DateTime.Now;

        if (quote.StockQuote < quote.DailyMinRange)
        {
            quote.DailyMinRange = quote.StockQuote;
        }
        else if (quote.StockQuote > quote.DailyMaxRange)
        {
            quote.DailyMaxRange = quote.StockQuote;
        }

        if (quote.DailyMinRange == 0)
        {
            quote.DailyMinRange = quote.StockQuote;
        }
    }

    private Quote GetQuote(string[] bufferList)
    {
        try
        {
            string stockTicker = bufferList[0];
            Decimal stockQuote = Decimal.Parse(bufferList[1], CultureInfo.InvariantCulture);
            DateTime lastUpdated = DateTime.Parse((bufferList[2] + " " + bufferList[3]), CultureInfo.InvariantCulture);
            Decimal change = Decimal.Parse(bufferList[4], CultureInfo.InvariantCulture);
            Decimal dailyMaxRange = 0m;
            if ((bufferList[6] != "N/A"))
            {
                dailyMaxRange = Decimal.Parse(bufferList[6], CultureInfo.InvariantCulture);
            }

            Decimal dailyMinRange = 0m;
            if ((bufferList[7] != "N/A"))
            {
                dailyMinRange = Decimal.Parse(bufferList[7], CultureInfo.InvariantCulture);
            }

            return new Quote(stockTicker, stockQuote, lastUpdated, change, dailyMinRange, dailyMaxRange);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private Quote GetQuote(XmlNode quoteNode)
    {
        string stockTicker = quoteNode.Attributes["stockTicker"].Value;
        Decimal stockQuote = Decimal.Parse(quoteNode.Attributes["lastTrade"].Value, CultureInfo.InvariantCulture);
        DateTime lastUpdated = DateTime.Parse(quoteNode.Attributes["lastUpdated"].Value, CultureInfo.InvariantCulture);
        Decimal change = Decimal.Parse(quoteNode.Attributes["change"].Value, CultureInfo.InvariantCulture);
        Decimal dailyMaxRange = quoteNode.Attributes["dailyMaxRange"].Value == "N/A" ? 0m : Decimal.Parse(quoteNode.Attributes["dailyMaxRange"].Value, CultureInfo.InvariantCulture);
        Decimal dailyMinRange = quoteNode.Attributes["dailyMinRange"].Value == "N/A" ? 0m : Decimal.Parse(quoteNode.Attributes["dailyMinRange"].Value, CultureInfo.InvariantCulture);

        return new Quote(stockTicker, stockQuote, lastUpdated, change, dailyMinRange, dailyMaxRange);
    }

    private Quote GetQuoteData(Stream dataStream)
    {
        string buffer = null;
        StreamReader sr = new StreamReader(dataStream);
        try
        {
            buffer = sr.ReadToEnd();
        }
        finally
        {
            sr.Dispose();
        }

        buffer = buffer.Replace("\"", "");

        string[] bufferList = buffer.Split(new char[] { ',' });
        Quote quote = GetQuote(bufferList);

        return quote;
    }

    private List<Quote> GetQuotesData(string xmlData)
    {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xmlData);

        List<Quote> quotes = new List<Quote>();
        Quote quote = null;
        foreach (XmlNode quoteNode in xmlDocument.DocumentElement.ChildNodes)
        {
            quote = GetQuote(quoteNode);
            quotes.Add(quote);
        }

        return quotes;
    }

    private void SerializeQuotesData(List<Quote> quotes)
    {
        StringBuilder str = new StringBuilder("<quotes>");
        try
        {
            foreach (Quote quote in quotes)
            {
                str.Append("<quote ");
                str.Append("stockTicker=\"" + quote.StockTicker + "\" ");
                str.Append("lastTrade=\"" + quote.StockQuote.ToString(CultureInfo.InvariantCulture) + "\" ");
                str.Append("change=\"" + quote.Change.ToString(CultureInfo.InvariantCulture) + "\" ");
                str.Append("lastUpdated=\"" + quote.LastUpdated.ToString("MM/dd/yyyy HH:mm:ss") + "\" ");
                str.Append("dailyMaxRange=\"" + quote.DailyMaxRange.ToString(CultureInfo.InvariantCulture) + "\" ");
                str.Append("dailyMinRange=\"" + quote.DailyMinRange.ToString(CultureInfo.InvariantCulture) + "\" ");
                str.Append(" />");
            }
        }
        catch (Exception e)
        {
        }
        str.Append("</quotes>");
        SessionQuotesData = str.ToString();
    }

   

}
