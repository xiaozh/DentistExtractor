using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace PlumberExtractor
{
    internal class PlumberExtractor
    {
        private const string RootUrl = @"http://www.ganet.org";
        private static StreamWriter file = new System.IO.StreamWriter("c:\\TEMP\\plumbers.txt");

        private static void Main(string[] args)
        {
            file.WriteLine("name,address,city,phone");
            var pe = new PlumberExtractor();
            var cities = pe.ExtractState("washington");
            for (int i = 0; i < cities.Count; i++)
            {
               pe.ExtractCity("washington", cities[i]);

            }
            file.Close();
            System.Console.ReadKey();
        }

        //find out all the cities in a state
        private List<string> ExtractState(string state)
        {
            var ret = new List<string>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(RootUrl + @"/plumbers"+@"/" + state + @"/");
            var linksOnPage = from lnks in doc.DocumentNode.Descendants()
                              where lnks.Name == "a" &&
                                    lnks.Attributes["href"] != null &&
                                    lnks.InnerText.Trim().Length > 0
                              select lnks;
            foreach (var link in linksOnPage)
            {
                string url = link.Attributes["href"].Value;
                if (url.EndsWith("-plumbers"))
                {
                    string text = link.InnerText.Trim();
                    ret.Add(text.ToLower().Replace(' ', '-'));
                }
            }
            return ret;

        }

        //find out all the plumber information in a city
        private void ExtractCity(string state, string city)
        {
            HtmlWeb web = new HtmlWeb();
            string cityUrl = RootUrl + @"/" + state + @"/" + city + @"?" + @"page=1&ipp=All";
            HtmlDocument doc = web.Load(cityUrl);

            var linksOnPage = from lnks in doc.DocumentNode.Descendants()
                              where lnks.Name == "a" &&
                                    lnks.Attributes["href"] != null &&
                                    lnks.InnerText.Trim().Length > 0
                              select lnks;

            foreach (var li in linksOnPage)
            {
                if (li.InnerText == "Phone")
                {
                    string phone, name, address;
                    phone = li.ParentNode.NextSibling.InnerText;
                    Console.WriteLine();
                    Console.WriteLine("phone: "+phone);
                    name = li.ParentNode.ParentNode.ParentNode.ParentNode.FirstChild.NextSibling.InnerText.Split('\n')[1].Trim();
                    address = li.ParentNode.ParentNode.ParentNode.ParentNode.FirstChild.NextSibling.InnerText.Split('\n')[2].Trim();
                    Console.WriteLine("name: "+ name);
                    Console.WriteLine("address: " + address);
                    file.WriteLine(name + "," + address + "," + city.Replace("-plumbers","") + "," + phone.Replace(" ", ""));
                }

            }

        }
    }
}
