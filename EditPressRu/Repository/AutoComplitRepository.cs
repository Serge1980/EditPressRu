using EditPressRu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Repository
{
    public partial class DataRepository {

       public virtual AutoComplSearchViewModel GetAutoComplModel(string term)
        {
           AutoComplSearchViewModel rezult = new AutoComplSearchViewModel();

            rezult.ListTovar = db.Products.Where(x => x.Name.Contains(term)).Select(x => new SelectListItem { Text = x.ShName, Value = x.Id.ToString() }).Take(15).Distinct().ToList();
            rezult.ListCat = db.Categories.Where(x => x.Name.Contains(term)).Select(x => new SelectListItem { Text = x.Name, Value = x.CpuPath.ToString() }).Take(10).Distinct().ToList();
            return rezult;
        }
    }
    
}