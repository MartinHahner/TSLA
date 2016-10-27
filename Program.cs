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
        p.Ask = Convert.ToDecimal(cols[2]);
        p.Bid = Convert.ToDecimal(cols[3]);

        prices.Add(p);

    }

    return prices;

  }

  private static void Main(string[] args) {

    string headline = "date\t\ttime\t\tname\t\task\tbid";

    Console.WriteLine(headline);
    Console.WriteLine("##########\t########\t############\t######\t######");

    while (true) {

      string csvData;

      string date = DateTime.Now.ToString("d.M.yyyy");

      string time = DateTime.Now.ToString("HH:mm:ss");

      try{
        string homepage = "http://finance.yahoo.com/d/quotes.csv?s=TSLA&f=nab";

        using (WebClient web = new WebClient()) {

          csvData = web.DownloadString(homepage);

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

        Console.WriteLine(s);

        using (StreamWriter w = File.AppendText("TSLA.txt")) {
          w.WriteLine(s);
        }

      }

      // wait one minute

      int seconds = 1000;

      System.Threading.Thread.Sleep(60 * seconds);

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
