using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PriceMachine.Helpers;
using PriceMachine.Objects;

namespace PriceMachine.Models
{
    public class CatalogCategory:DbModel
    {
        public int Sid { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string IdParent { get; set; }
        public ProductProvider Provider { get; set; }

        public List<CatalogProduct> Products { get; set; }

        public CatalogCategory()
        {
            Products = new List<CatalogProduct>();
        }

        public void Save()
        {
            Name = Name.Replace("\"", "");

            SqlParameter pSid = new SqlParameter() { ParameterName = "sid", SqlValue = Sid, SqlDbType = SqlDbType.BigInt };
            SqlParameter pId = new SqlParameter() { ParameterName = "id", SqlValue = Id, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pIdParent = new SqlParameter() { ParameterName = "id_parent", SqlValue = IdParent, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pName = new SqlParameter() { ParameterName = "name", SqlValue = Name, SqlDbType = SqlDbType.NVarChar };
            SqlParameter pProviderId = new SqlParameter() { ParameterName = "id_provider", SqlValue = Provider.Id, SqlDbType = SqlDbType.Int };

            var dt = Db.Eprice.ExecuteQueryStoredProcedure("save_catalog_category", pId, pIdParent, pName, pProviderId, pSid);
            if (dt.Rows.Count > 0)
            {
                int sid = Db.DbHelper.GetValueInt(dt.Rows[0]["sid"]);
                Sid = sid;
            }

            if (Products != null && Products.Any())
            {
                foreach (CatalogProduct prod in Products)
                {
                    prod.CategorySid = Sid;
                    //try
                    //{
                        prod.Save();
                    //}
                    //catch (Exception ex)
                    //{
                    //    throw ex;
                    //}
                }
            }
        }

        //public bool Save(out ResponseMessage responseMessage)
        //{
        //    Uri uri = new Uri(String.Format("{0}/CatalogCategory/Save", OdataServiceUri));
        //    string json = JsonConvert.SerializeObject(this);
        //    bool result = PostJson(uri, json, out responseMessage);
        //    return result;
        //}

        //public bool Save()
        //{
        //    ResponseMessage responseMessage;
        //    return Save(out responseMessage);
        //}
    }
}
