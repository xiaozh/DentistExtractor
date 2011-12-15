using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;

namespace DentistExtractor
{
    class DentistExtractor
    {
        const string RootUrl = "http://www.thedentistdatabase.com";
        static StreamWriter file = new System.IO.StreamWriter("c:\\TEMP\\dentists.txt"); 

        static void Main()
        {
            
            var de = new DentistExtractor();
            var cities = de.ExtractState("WA");
            for (int i = 0; i < cities.Count; i++)
            {
                System.Console.WriteLine(cities[i]);
                de.ExtractCity("WA", cities[i]);
            }
            file.Close();
            System.Console.ReadKey();
        }


        //find out all the cities in a state
        private List<string> ExtractState(string state)
        {
            var ret = new List<string>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(RootUrl+@"/"+state+@"/");
            var linksOnPage = from lnks in doc.DocumentNode.Descendants()
                    where lnks.Name == "a" && 
                         lnks.Attributes["href"] != null && 
                         lnks.InnerText.Trim().Length > 0
                    select new
                    {
                       Url = lnks.Attributes["href"].Value,
                       Text = lnks.InnerText
                    };
              foreach (var link in linksOnPage)
              {
                  if(link.Url.StartsWith("/"+state))
                  ret.Add(link.Text);
              }
            return ret;
        }

        //find out all the dentists formation in a city
        private List<string> ExtractCity(string state,string city)
        {
            var ret = new List<string>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(RootUrl + @"/" + state + @"/"+city+@"/");
            var linksOnPage = from lnks in doc.DocumentNode.Descendants()
                              where lnks.Name == "a" &&
                                    lnks.Attributes["href"] != null &&
                                    lnks.InnerText.Trim().Length > 0
                              select lnks;
            file.WriteLine(city);                  
            foreach (var link in linksOnPage)
            {
                string url = link.Attributes["href"].Value;
                string text = link.InnerText;
                if (url.StartsWith(state + @"/" + city.Replace(' ','-') + @"/"))   //city name with space replace with - in url
                {
                    System.Console.WriteLine();
                    System.Console.WriteLine("name: "+text);
                    System.Console.WriteLine("address: "+ link.ParentNode.ParentNode.ParentNode.LastChild.InnerText);
                    System.Console.WriteLine("phone: " + link.ParentNode.NextSibling.InnerText);

                    file.WriteLine();
                    file.WriteLine("name: " + text);
                    file.WriteLine("address: " + link.ParentNode.ParentNode.ParentNode.LastChild.InnerText);
                    file.WriteLine("phone: " + link.ParentNode.NextSibling.InnerText);  

                }
            }

            return null;
        }
    }
}
