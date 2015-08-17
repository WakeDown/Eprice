using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using PriceMachine.OcsDataService;

namespace PriceMachine.Models
{
    class Ocs
    {
        public static string Login = "tXSoC8PKs";
        public static string Token = "SL6q1#98MqmiplEevRyNVGg6Iws_pw";
        static ProductProvider Provider = new ProductProvider("OCS");
        static OcsDataService.B2BWebServiceSoapClient Client = new B2BWebServiceSoapClient();

        public static void Open()
        {
            if (Client.State != CommunicationState.Opened)
            {
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
            var catalog = Client.GetCatalog(Login, Token);

            var categories = new List<CatalogCategory>();

            foreach (var category in catalog.Categories)
            {
                var item = new CatalogCategory();
                item.Id = category.CategoryID;
                item.Name = category.CategoryName;
                item.IdParent = category.ParentCategoryID;
                categories.Add(item);

                item.Provider = Provider;
                item.Save();
            }
            //Загружаем товары только по нижним категориям
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
                var positions = Client.GetProductAvailability(Login, Token, 0, null,
                    new ArrayOfString() { category.Id }, null, null, 1);
                //var productavail = Client.getItemsAvail(category.Id, null, null, null, null);
                foreach (var position in positions.Products)
                {
                    var pId = position.ItemID;
                    var pArticul = position.PartNumber;
                    var pName = position.ItemNameRus;
                    var pVendor = position.Producer;
                    var pPrice = position.Price.ToString();
                    var pCurrency = position.Currency;
                    var pItem = new CatalogProduct();
                    pItem.Id = pId;
                    pItem.Name = pName;
                    pItem.PartNumber = pArticul;
                    pItem.Vendor = pVendor;
                    decimal priceDec;
                    Decimal.TryParse(pPrice, NumberStyles.AllowDecimalPoint, new CultureInfo("ru-RU"), out priceDec);
                    pItem.Price = priceDec;
                    pItem.Currency = new Currency() { ProviderName = pCurrency };
                    products.Add(pItem);
                    try
                    {
                        cat.Save();
                    }
                    catch { continue; }
                }
            }


            Close();
        }

        public static Catalog GetCatalog()
        {
            Open();
            var catalog = Client.GetCatalog(Login, Token);

            var categories = new List<CatalogCategory>();

            foreach (var category in catalog.Categories)
            {
                var item = new CatalogCategory();
                item.Id = category.CategoryID;
                item.Name = category.CategoryName;
                item.IdParent = category.ParentCategoryID;
                categories.Add(item);

                
            }
            //Загружаем товары только по нижним категориям
            var cats4Prod = new List<CatalogCategory>();
            foreach (var category in categories)
            {
                if (categories.All(c => c.IdParent != category.Id))cats4Prod.Add(category);
            }
            //int i = 0;
            //var positions = Client.GetProductAvailability(Login, Token, 0, null,new ArrayOfString() { category.Id }, null, null, 1);
            var positions = Client.GetProductAvailability(Login, Token, 0, null, null, null, null, 1).Products.ToList();
            foreach (CatalogCategory category in cats4Prod)
            {
                //i++;
                //if (i > 10) break;
                var products = new List<CatalogProduct>();
                categories.Find(c=>c.Id==category.Id).Products = products;
                var catPositions = positions.Where(p => p.CategoryID == category.Id);
                //var productavail = Client.getItemsAvail(category.Id, null, null, null, null);
                foreach (var position in catPositions)
                {
                    var pId = position.ItemID;
                    var pArticul = position.PartNumber;
                    var pName = position.ItemNameRus;
                    var pVendor = position.Producer;
                    var pPrice = position.Price.ToString();
                    var pCurrency = position.Currency;
                    var pItem = new CatalogProduct();
                    pItem.Id = pId;
                    pItem.Name = pName;
                    pItem.PartNumber = pArticul;
                    pItem.Vendor = pVendor;
                    decimal priceDec;
                    Decimal.TryParse(pPrice, NumberStyles.AllowDecimalPoint, new CultureInfo("ru-RU"), out priceDec);
                    pItem.Price = priceDec;
                    pItem.Currency = new Currency(){ProviderName = pCurrency};
                    products.Add(pItem);
                }
            }
            

            Close();
            return new Catalog() { Categories = categories, Provider = Provider };
        }
    }
}
