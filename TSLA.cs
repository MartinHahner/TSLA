using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

internal class Program {

  public static List<Price> Parse(string csvData) {

    List<Price> prices = new List<Price>();

    string[] rows = csvData.Replace("\r", "").Split('\n');

    foreach (string row in rows) {

        if (string.IsNullOrEmpty(row)) continue;

        string[] cols = row.Split(',');

        Price p = new Price();
        p.Name = cols[0].Substring(1);

        if(cols[2] != "N/A") {

          cols[2] = cols[2].Substring(0, Math.Min(cols[2].Length, 6));

          p.Ask = Convert.ToDecimal(cols[2]);

        }

        if(cols[3] != "N/A") {

          cols[3] = cols[3].Substring(0, Math.Min(cols[3].Length, 6));

          p.Bid = Convert.ToDecimal(cols[3]);

        }

        prices.Add(p);

    }

    return prices;

  }

  private static void Main(string[] args) {

    // string headline = "date\t\ttime\t\tname\t\task\tbid";

    // Console.WriteLine(headline);
    // Console.WriteLine("##########\t########\t############\t######\t######");

    while (File.Exists("continue.txt")) {

      string csvData;

      string date = DateTime.Now.ToString("d.M.yyyy");

      string time = DateTime.Now.ToString("HH:mm:ss");

      if(time.Substring(time.Length - 2) == "00"){

        try{

          string homepage = "http://finance.yahoo.com/d/quotes.csv?s=TSLA&f=";

          // string query = homepage + "nb2b3";
          string query = homepage + "nab";

          using (WebClient web = new WebClient()) {

            csvData = web.DownloadString(query);

          }

        }

        catch (System.Net.WebException e) {

          using (StreamWriter w = File.AppendText("errorlog.txt")) {
            w.WriteLine(e);
            w.WriteLine();
          }

          continue;
        }

        List<Price> prices = Parse(csvData);

        foreach (Price price in prices) {

          string name = string.Format("{0}", price.Name);
          string ask = string.Format("{0}", price.Ask);
          string bid = string.Format("{0}", price.Bid);

          string s = date + "\t" + time + "\t" + name + "\t" + ask + "\t" + bid;

          if(ask != "0" && bid != "0") {

            // Console.WriteLine(s);

            using (StreamWriter w = File.AppendText("TSLA.txt")) {
              w.WriteLine(s);
            }
          }
          else {

            string message = "stock price not available";

            string t = date + "\t" + time + "\t" + name + "\t" + message;

            using (StreamWriter w = File.AppendText("errorlog.txt")) {
              w.WriteLine(t);
            }
          }

        }

        string day = date.Split('.')[0];

        string month = date.Split('.')[1];

        string year = date.Split('.')[2];

        string hour = time.Split(':')[0];

        string nextminute = time.Split(':')[1];

        int nextmin = 0;
        Int32.TryParse(nextminute, out nextmin);
        nextmin = (nextmin + 1) % 60;

        nextminute = nextmin.ToString();

        int nexthour = 99;
        if(nextmin == 0) {
          Int32.TryParse(hour, out nexthour);
          nexthour++;
          if(nexthour == 24) {
            nexthour = 0;
          }
          hour = nexthour.ToString();
        }

        int nextday = 99;
        if(nexthour == 0) {
          Int32.TryParse(day, out nextday);
          nextday++;
          int y = 0;
          int m = 0;
          Int32.TryParse(year, out y);
          Int32.TryParse(month, out m);
          if(nextday == (DateTime.DaysInMonth(y, m) + 1)) {
            nextday = 1;
          }
          day = nextday.ToString();
        }

        int nextMonth = 99;
        if(nextday == 0) {
          Int32.TryParse(month, out nextMonth);
          nextMonth++;
          if(nextMonth == 13) {
            nextMonth = 1;
          }
          month = nextMonth.ToString();
        }

        int nextYear = 99;
        if(nextMonth == 1) {
          Int32.TryParse(year, out nextYear);
          nextYear = nextYear + 1;
          year = nextYear.ToString();
        }

        if(day.Length == 1) {
          day = "0" + day;
        }

        if(month.Length == 1) {
          month = "0" + month;
        }

        if(hour.Length == 1) {
          hour = "0" + hour;
        }

        if(nextminute.Length == 1) {
          nextminute = "0" + nextminute;
        }

        string myTime = day + "/" + month + "/" + year + " " + hour + ":" + nextminute + ":00";
        DateTime myDate = DateTime.ParseExact(myTime, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

        TimeSpan span = myDate.Subtract(DateTime.Now);

        int secondsLeft = span.Seconds - 1;

        // wait till end of minute
        int seconds = 1000;
        System.Threading.Thread.Sleep(secondsLeft * seconds);

      }

    }

  }

}

public class Price {

  public string Symbol { get; set; }
  public string Name { get; set; }
  public decimal Bid { get; set; }
  public decimal Ask { get; set; }
  public decimal Open { get; set; }
  public decimal PreviousClose { get; set; }
  public decimal Last { get; set; }

}
