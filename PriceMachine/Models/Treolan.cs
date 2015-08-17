using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PriceMachine.Models
{
    public static class Treolan
    {
        public static string Login = "unitgroup_rai";
        public static string Password = "awklhy6w";
        static TreolanDataService.WebServiceSoapPortClient Client = new TreolanDataService.WebServiceSoapPortClient();
        static TreolanProductService.B2BWebServiceSoapClient ClientProduct = new TreolanProductService.B2BWebServiceSoapClient();
        static ProductProvider Provider = new ProductProvider("TREOLAN");

        public static void Open()
        {
            if (Client.State != CommunicationState.Opened)
            {
                Client.Open();
            }
            if (ClientProduct.State != CommunicationState.Opened)
            {
                ClientProduct.Open();
            }
        }

        public static void Close()
        {
            if (Client.State != CommunicationState.Closed)
            {
                Client.Close();
            }
            if (ClientProduct.State != CommunicationState.Closed)
            {
                ClientProduct.Close();
            }
        }

        public static void Sync()
        {
            Open();

            string cat = String.Empty;
            string kw = String.Empty;
            int crit = 0;
            bool inArt = false;
            bool inName = false;
            bool inMark = false;
            int shNc = 0;
            var request = Client.GenCatalog(ref Login, ref Password, ref cat, ref kw, ref crit,
                ref inArt, ref inName, ref inMark, ref shNc);
            var categories = new List<CatalogCategory>();


            if (!string.IsNullOrEmpty(request))
            {

                var xml = XDocument.Parse(request);
                var root = xml.Root;
                if (root != null)
                {
                    var catalog = root.Descendants("category");
                    foreach (var category in catalog)
                    {
                        var cId = category.Attribute("id").Value;
                        var cIdParent = category.Attribute("parent").Value;
                        var cName = category.Attribute("name").Value;

                        var cItem = new CatalogCategory();
                        cItem.Id = cId;
                        cItem.IdParent = cIdParent;
                        cItem.Name = cName;
                        categories.Add(cItem);

                        if (category.HasElements)
                        {
                            var products = new List<CatalogProduct>();
                            cItem.Products = products;
                            var positions = category.Descendants("position");
                            foreach (var position in positions)
                            {
                                var pId = position.Attribute("id").Value;
                                var pArticul = position.Attribute("articul").Value;
                                var pName = position.Attribute("name").Value;
                                var pVendor = position.Attribute("vendor").Value;
                                var pPrice = position.Attribute("price").Value.Replace(".", ",");
                                var pCurrency = position.Attribute("currency").Value;
                                //var dprice = position.Attribute("dprice").Value;
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
                            }
                        }
                        cItem.Provider = Provider;
                        try
                        {
                            cItem.Save();
                        }catch{continue;}
                    }
                }
            }

            Close();
        }

        public static Catalog GetCatalog()
        {
            //return null;
            Open();

            string cat = String.Empty;
            string kw = String.Empty;
            int crit = 0;
            bool inArt = false;
            bool inName = false;
            bool inMark = false;
            int shNc = 0;
            var request = Client.GenCatalog(ref Login, ref Password, ref cat, ref kw, ref crit,
                ref inArt, ref inName, ref inMark, ref shNc);
            var categories = new List<CatalogCategory>();


            if (!string.IsNullOrEmpty(request))
            {
                var xml = XDocument.Parse(request);
                var root = xml.Root;
                if (root != null)
                {
                    var catalog = root.Descendants("category");
                    foreach (var category in catalog)
                    {


                        var cId = category.Attribute("id").Value;
                        var cIdParent = category.Attribute("parent").Value;
                        var cName = category.Attribute("name").Value;

                        var cItem = new CatalogCategory();
                        cItem.Id = cId;
                        cItem.IdParent = cIdParent;
                        cItem.Name = cName;
                        categories.Add(cItem);

                        if (category.HasElements)
                        {
                            var products = new List<CatalogProduct>();
                            cItem.Products = products;
                            var positions = category.Descendants("position");
                            foreach (var position in positions)
                            {
                                var pId = position.Attribute("id").Value;
                                var pArticul = position.Attribute("articul").Value;
                                var pName = position.Attribute("name").Value;
                                var pVendor = position.Attribute("vendor").Value;
                                var pPrice = position.Attribute("price").Value.Replace(".", ",");
                                var pCurrency = position.Attribute("currency").Value;
                                //var dprice = position.Attribute("dprice").Value;
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
                            }

                        }
                    }
                }
            }

            Close();

            return new Catalog() { Categories = categories, Provider = Provider };
        }
    }
}
