using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using PriceMachine.Objects;

namespace PriceMachine.Models
{
    public class Catalog:DbModel
    {
        public IEnumerable<CatalogCategory> Categories { get; set; }
        public ProductProvider Provider { get; set; }

        public void Save()
        {
            foreach (CatalogCategory category in Categories)
            {
                category.Provider = Provider;
                category.Save();
            }
        }

        //public bool Save(out ResponseMessage responseMessage)
        //{
        //    //foreach (var category in Categories)
        //    //{
        //        Uri uri = new Uri(String.Format("{0}/CatalogCategory/SaveList", OdataServiceUri));
        //        string json = JsonConvert.SerializeObject(this);
        //        //string json = JsonConvert.SerializeObject(new Catalog(){Categories = new List<CatalogCategory>() {category}, Provider = Provider});
        //        bool result = PostJson(uri, json, out responseMessage);
        //    return result;
        //    //}
        //    //responseMessage = new ResponseMessage();
        //    //return true;
        //}

        
    }
}
