using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PriceMachine.Models;
using PriceMachine.Objects;

namespace PriceMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            LoadCatalogFromProviders();
        }

        public static void LoadCatalogFromProviders()
        {
            ResponseMessage mgs;
            try
            {
                //Treolan.Sync();
                var trCat = Treolan.GetCatalog();
                trCat.Save();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            try
            {
                Merlion.Sync();
                var merCat = Merlion.GetCatalog();
                merCat.Save();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            try
            {
                //У OCS есть лимит на количество обращений в день и в час
                Ocs.Sync();
                var OcsCat = Ocs.GetCatalog();
                OcsCat.Save();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            try
            {
            //var oldiCat = Oldi.GetCatalog();
            //oldiCat.Save(out mgs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
