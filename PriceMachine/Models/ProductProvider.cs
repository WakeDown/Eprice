using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PriceMachine.Objects;

namespace PriceMachine.Models
{
    public class ProductProvider:DbModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SysName { get; set; }

        public ProductProvider(){}

        public ProductProvider(string sysName)
        {
            Uri uri = new Uri(String.Format("{0}/ProductProvider/Get?sysName={1}", OdataServiceUri, sysName));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<ProductProvider>(jsonString);
            FillSelf(model);
        }

        public ProductProvider(int id)
        {
            Uri uri = new Uri(String.Format("{0}/ProductProvider/Get?id={1}", OdataServiceUri, id));
            string jsonString = GetJson(uri);
            var model = JsonConvert.DeserializeObject<ProductProvider>(jsonString);
            FillSelf(model);
        }

        private void FillSelf(ProductProvider model)
        {
            Id = model.Id;
            Name = model.Name;
            SysName = model.SysName;
        }

    }
}
