using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MongoDB;
using MongoDB.Linq;
 

namespace DbImporter
{
    class DbImporter
    {
        static StreamReader dealerFile = new System.IO.StreamReader("c:\\TEMP\\cardealers.txt");
        static StreamReader dentistFile = new System.IO.StreamReader("c:\\TEMP\\dentists.txt");
        private const string Professional = "Professional";
        private const string Business = "Business";
        private IMongoCollection<AdvicesSubject> _collection; 
        static void Main(string[] args)
        {
            DbImporter di = new DbImporter();
            di.ConnectMongo();
            di.ImportOneBusiness(dealerFile, Professional, "WA", "US");
            di.ImportOneBusiness(dentistFile, Business, "WA", "US");
        }

        private void ImportOneBusiness(StreamReader file,string type, string state, string country)
        {
            String str;
            str = file.ReadLine();  //skip first line
            string name = string.Empty, address = string.Empty, city = string.Empty, phone = string.Empty;
          
           
            while ((str = file.ReadLine()) != null)
            {
                string[] items = str.Split(',');

                if (items.Length == 4)
                {
                    name = items[0];
                    address = items[1];
                    city = items[2];
                    phone = items[3];
                }
                Console.WriteLine(name);
                WriteMongo(new AdvicesSubject() { name = name, address = address, city = city, tel = phone, type = type, state = state, country = country });
            }

            file.Close();
            
        }

        private void ConnectMongo()
        {
            var mongo = new Mongo();
            mongo.Connect();

            var db = mongo.GetDatabase("AdvicesDB");

            _collection = db.GetCollection<AdvicesSubject>();

          //  collection.Remove(p => true);
        }
        private void WriteMongo(AdvicesSubject business)
        {

            //save or update use tel as primary
            var sub = _collection.Linq().FirstOrDefault(x => x.tel == business.tel); 
            if(sub!=null)
            {
                sub.name =business.name;
                sub.tel = business.tel;
                sub.address = business.address;
                sub.city = business.city;
                _collection.Save(sub);
            }
            else
                _collection.Save(business);
            

        }
    }

    internal class AdvicesSubject
    {
        public Oid Id { get; private set; }
        public string name { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string tel { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string type { get; set; }
//todo category
    }
}
