using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using PriceMachine.OldiDataService;

namespace PriceMachine.Models
{
    class Oldi
    {
        public static string Login = "tXSoC8PKs";
        public static string Token = "SL6q1#98MqmiplEevRyNVGg6Iws_pw";
        static ProductProvider Provider = new ProductProvider("OLDI");
        static OldiDataService.B2bPublicPortTypeClient Client = new OldiDataService.B2bPublicPortTypeClient();

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

        public static Catalog GetCatalog()
        {
            Open();
            var msg = new Error();
            var catalog = Client.GetItems(null, null, Token, out msg);

            var categories = new List<CatalogCategory>();

            //foreach (var category in catalog)
            //{
            //    var item = new CatalogCategory();
            //    item.Id = category.;
            //    item.Name = category.CategoryName;
            //    item.IdParent = category.ParentCategoryID;
            //    categories.Add(item);


            //}
            //Загружаем товары только по нижним категориям
            //var cats4Prod = new List<CatalogCategory>();
            //foreach (var category in categories)
            //{
            //    if (categories.All(c => c.IdParent != category.Id)) cats4Prod.Add(category);
            //}
            ////int i = 0;
            //foreach (var category in cats4Prod)
            //{
            //    //i++;
            //    //if (i > 10) break;
            //    var products = new List<CatalogProduct>();
            //    categories.Find(c => c.Id == category.Id).Products = products;
            //    var positions = Client.GetProductAvailability(Login, Token, 0, null,
            //        new ArrayOfString() { category.Id }, null, null, 1);
            //    //var productavail = Client.getItemsAvail(category.Id, null, null, null, null);
            //    foreach (var position in positions.Products)
            //    {
            //        var pId = position.ItemID;
            //        var pArticul = position.PartNumber;
            //        var pName = position.ItemNameRus;
            //        var pVendor = position.Producer;
            //        var pPrice = position.Price.ToString();
            //        var pCurrency = position.Currency;
            //        var pItem = new CatalogProduct();
            //        pItem.Id = pId;
            //        pItem.Name = pName;
            //        pItem.PartNumber = pArticul;
            //        pItem.Vendor = pVendor;
            //        decimal priceDec;
            //        Decimal.TryParse(pPrice, NumberStyles.AllowDecimalPoint, new CultureInfo("ru-RU"), out priceDec);
            //        pItem.Price = priceDec;
            //        pItem.Currency = new Currency() { ProviderName = pCurrency };
            //        products.Add(pItem);
            //    }
            //}


            Close();
            return new Catalog() { Categories = categories, Provider = Provider };
        }
    }
}
