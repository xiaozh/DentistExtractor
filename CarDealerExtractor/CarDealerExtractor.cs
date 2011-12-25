using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace CarDealerExtractor
{
    class CarDealerExtractor
    {
        const string RootUrl = "http://www.autodealerdirectory.us/wa_s_madd.html";
        static StreamWriter file = new System.IO.StreamWriter("c:\\TEMP\\cardealers.txt");

        static void Main()
        {
            file.WriteLine("name,address,city,phone");
            var ce = new CarDealerExtractor();
            ce.ExtractDealers();
            file.Close();
            System.Console.ReadKey();
        }

        private void ExtractDealers()
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(RootUrl);
            var mainNode = doc.DocumentNode.SelectNodes("//*[@id='bodyText']").First();
            var startNode = mainNode.SelectNodes("//*[@id='centerClear']").First();
            var endNode = mainNode.SelectNodes("/html/body/div/div[2]/div/h1[2]").First();
    
            var siblingNode = startNode.NextSibling;
            while (siblingNode!=null)
            {
                if (siblingNode==endNode)
                    break;
                if (siblingNode.InnerText.Trim().Length > 0)
                {
                    string name, address, city, phone;
                    name = siblingNode.InnerText.Trim();
                    siblingNode = siblingNode.NextSibling.NextSibling;  //skip br
                    address = siblingNode.InnerText.Trim();
                    siblingNode = siblingNode.NextSibling.NextSibling;
                    city = siblingNode.InnerText.Trim();
                    city = city.Substring(0, city.IndexOf('&'));
                    siblingNode = siblingNode.NextSibling.NextSibling;
                    phone = siblingNode.InnerText.Trim();
                    phone = phone.Substring(0, phone.IndexOf('&')).Trim();
                 

                    System.Console.WriteLine("name: " + name);
                    System.Console.WriteLine("address: " + address);
                    System.Console.WriteLine("city: " + city);
                    System.Console.WriteLine("phone: " + phone);
                    System.Console.WriteLine();


                    file.WriteLine(name + "," + address + "," + city + "," + phone.Replace(" ",""));
                }
                siblingNode = siblingNode.NextSibling;

            }
        }

        private HtmlNode findNextNonEmptySibling(HtmlNode current)
        {
            var siblingNode = current.NextSibling;
            while (siblingNode.InnerText.Trim().Length == 0)
                siblingNode = siblingNode.NextSibling;
            return siblingNode;
        }
    }
}
