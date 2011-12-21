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
            file.WriteLine("name,address,city,phone");
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
        private void ExtractCity(string state,string city)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(RootUrl + @"/" + state + @"/"+city+@"/");
            var linksOnPage = from lnks in doc.DocumentNode.Descendants()
                              where lnks.Name == "a" &&
                                    lnks.Attributes["href"] != null &&
                                    lnks.InnerText.Trim().Length > 0
                              select lnks;            
            foreach (var link in linksOnPage)
            {
                string url = link.Attributes["href"].Value;
                string name,address,phone;
                 name  = link.InnerText;
                if (url.StartsWith(state + @"/" + city.Replace(' ','-') + @"/"))   //city name with space replace with - in url
                {
                    address = link.ParentNode.ParentNode.ParentNode.LastChild.InnerText;
                    address = address.Substring(0,address.IndexOf(city)).Trim();
                    phone = link.ParentNode.NextSibling.InnerText;
                    System.Console.WriteLine();
                    System.Console.WriteLine("name: "+name);
                    System.Console.WriteLine("address: "+ address);
                    System.Console.WriteLine("phone: " + phone);

                    file.WriteLine(name+","+address+","+city+","+phone );
                  
                }
            }

        }
    }
}
