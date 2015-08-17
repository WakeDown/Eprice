using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace PriceMachine.Models
{
    class Merlion
    {
        public static string Login = "MDC2909|IT";
        public static string Password = "123456789";

        static MerlionDataService.MLPortClient Client = new MerlionDataService.MLPortClient();
        static ProductProvider Provider = new ProductProvider("MERLION");

        public static void Open()
        {
            if (Client.State != CommunicationState.Opened)
            {
                Client.ClientCredentials.UserName.UserName = Login;
                Client.ClientCredentials.UserName.Password = Password;
                Client.Open();
            }
        }

        public static void Close()
        {
            if (Client.State != CommunicationState.Closed)
            {
                Client.Close();
            }
        }

        public static void Sync()
        {
            Open();
            var catalog = Client.getCatalog("All");

            var categories = new List<CatalogCategory>();

            foreach (var category in catalog)
            {
                var cItem = new CatalogCategory();
                cItem.Id = category.ID;
                cItem.Name = category.Description;
                cItem.IdParent = category.ID_PARENT;
                categories.Add(cItem);

                cItem.Provider = Provider;
                cItem.Save();
            }



            var cats4Prod = new List<CatalogCategory>();
            foreach (var category in categories)
            {
                if (categories.All(c => c.IdParent != category.Id)) cats4Prod.Add(category);
            }

            //int i = 0;
            foreach (var category in cats4Prod)
            {
                //i++;
                //if (i > 10) break;
                var products = new List<CatalogProduct>();
                var cat = categories.Find(c => c.Id == category.Id);
                cat.Products = products;
                var positions = Client.getItems(category.Id, null, null, null, 1000000);
                var productavail = Client.getItemsAvail(category.Id, null, null, null, null);
                foreach (var position in positions)
                {
                    var pId = position.No;
                    var pArticul = position.Vendor_part;
                    var pName = position.Name;
                    var pVendor = position.Brand;
                    string pPrice = String.Empty;
                    try { pPrice = productavail.First(p => p.No == position.No).PriceClient.ToString(); }
                    catch { }
                    //string pCurrency = null;
                    //var dprice = position.Attribute("dprice").Value;
                    var pItem = new CatalogProduct();
                    pItem.Id = pId;
                    pItem.Name = pName;
                    pItem.PartNumber = pArticul;
                    pItem.Vendor = pVendor;
                    decimal priceDec;
                    Decimal.TryParse(pPrice, NumberStyles.AllowDecimalPoint, new CultureInfo("ru-RU"), out priceDec);
                    pItem.Price = priceDec;
                    pItem.Currency = new Currency();
                    products.Add(pItem);
                }
                
                try
                {
                    cat.Save();
                }catch{continue;}
            }



            Close();
        }

        public static Catalog GetCatalog()
        {
            Open();
            var catalog = Client.getCatalog("All");

            var categories = new List<CatalogCategory>();

            foreach (var category in catalog)
            {
                var cItem = new CatalogCategory();
                cItem.Id = category.ID;
                cItem.Name = category.Description;
                cItem.IdParent = category.ID_PARENT;
                categories.Add(cItem);
            }

            var cats4Prod = new List<CatalogCategory>();
            foreach (var category in categories)
            {
                if (categories.All(c => c.IdParent != category.Id))cats4Prod.Add(category);
            }

            //int i = 0;
            foreach (var category in cats4Prod)
            {
                //i++;
                //if (i > 10) break;
                var products = new List<CatalogProduct>();
                categories.Find(c=>c.Id==category.Id).Products = products;
                var positions = Client.getItems(category.Id, null, null, null, 1000000);
                var productavail = Client.getItemsAvail(category.Id, null, null, null, null);
                foreach (var position in positions)
                {
                    var pId = position.No;
                    var pArticul = position.Vendor_part;
                    var pName = position.Name;
                    var pVendor = position.Brand;
                    string pPrice = String.Empty;
                    try{pPrice = productavail.First(p => p.No == position.No).PriceClient.ToString();}catch{}
                    //string pCurrency = null;
                    //var dprice = position.Attribute("dprice").Value;
                    var pItem = new CatalogProduct();
                    pItem.Id = pId;
                    pItem.Name = pName;
                    pItem.PartNumber = pArticul;
                    pItem.Vendor = pVendor;
                    decimal priceDec;
                    Decimal.TryParse(pPrice, NumberStyles.AllowDecimalPoint, new CultureInfo("ru-RU"), out priceDec);
                    pItem.Price = priceDec;
                    pItem.Currency = new Currency();
                    products.Add(pItem);
                }

                
            }

            Close();
            
            return new Catalog() { Categories = categories, Provider = Provider };
        }
    }
}
