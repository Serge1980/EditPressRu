using EditPressRu.Models;
using EditPressRu.Models.DB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace EditPressRu.Repository
{
    public partial class DataRepository
    {

        public virtual PagerResult<ProdList> GetAllProducts(LineJsonModel incommingModel)
        {
            PagerResult<ProdList> rezult = new PagerResult<ProdList>();

            List<ProdList> list = new List<ProdList>();


            List<ProdList> tovarListJson1 = GetGroupProds(incommingModel,1);
            List<ProdList> tovarListJson2 = GetGroupProds(incommingModel,0);

            list = tovarListJson1.Concat(tovarListJson2).ToList();
           

            if (incommingModel.Hit)
            {
                list = list.Where(x => x.Hit).ToList();
            }
            if (incommingModel.Sale)
            {
                list = list.Where(x => x.Sale).ToList();
            }
            if (incommingModel.New)
            {
                list = list.Where(x => x.New).ToList();
            }

            #region Implemntation Filters to productsList


            if (incommingModel.Materials != null && incommingModel.Materials.Count > 0)
            {
                List<int> lstId = db.Prods_Materials.Where(x => incommingModel.Materials.Contains(x.Materials_spr.Name)).Select(x => x.ProdId).ToList();
                list = list.Where(x => lstId.Contains(x.ProdId)).ToList();
            }

            if (incommingModel.Nanesenie != null && incommingModel.Nanesenie.Count > 0)
            {
                List<int> lstId = db.Prods_Nanesenie.Where(x => incommingModel.Nanesenie.Contains(x.Nanesenie_spr.Name)).Select(x => x.ProdId).ToList();
                list = list.Where(x => lstId.Contains(x.ProdId)).ToList();
            }

            if (incommingModel.Brand != null && incommingModel.Brand.Count > 0)
            {
                List<int> brdList = db.Brands.Where(x => incommingModel.Brand.Contains(x.Name)).Select(x => x.ID).ToList();
                List<int> prdListBrd = db.Products.Where(x => brdList.Contains(x.BrandId.Value)).Select(x => x.Id).ToList();
                list = list.Where(x => prdListBrd.Contains(x.ProdId)).ToList();
            }

            if (incommingModel.Color != null && incommingModel.Color.Count > 0)
            {
                List<string> colors = db.Colors_spr.Where(x => incommingModel.Color.Contains(x.Id.ToString())).Select(x => x.Name.Replace("ый", "").Replace("ой", "")).ToList();

                List<int> lstId = new List<int>();

                foreach (var item in colors)
                {
                    List<int> idList = list.Where(x => x.Name.Contains(item)).Select(x => x.ProdId).ToList();
                    lstId.AddRange(idList);
                }

                lstId = lstId.Distinct().ToList();

                list = list.Where(x => lstId.Contains(x.ProdId)).ToList();
            }
            #endregion

            rezult.TotalCount = list.Count();
            rezult.ListProdId = list.Select(x => x.ProdId).ToList();
            rezult.minPrice = (int)list.Select(x => x.Price).Min();
            rezult.maxPrice = (int)list.Select(x => x.Price).Max();

            //По цене вверх по умолчанию
            rezult.Items = list.OrderBy(x => x.Sort).Skip((incommingModel.page - 1) * incommingModel.PageProductCount).Take(incommingModel.PageProductCount).ToList();

            //По цене вниз 
            if (!incommingModel.UpPriceOrder.Value)
            {
                //rezult.Items = rezult.Items.OrderByDescending(x => x.Sort).ToList();
                rezult.Items = list.OrderByDescending(x => x.Sort).Skip((incommingModel.page - 1) * incommingModel.PageProductCount).Take(incommingModel.PageProductCount).ToList();
            }

            return rezult;
        }

        public virtual PagerResult<ProdList> GetSearch(LineJsonModel incommingModel)
        {
            PagerResult<ProdList> rezult = new PagerResult<ProdList>();

            #region Get IQueryable productsList 
            
            List<ProdList> list = GetProdListFTS(incommingModel);
            
            rezult.QueryString = list.FirstOrDefault().QueryString;

            if (incommingModel.Hit)
            {
                list = list.Where(x => x.Hit).ToList();
            }
            if (incommingModel.Sale)
            {
                list = list.Where(x => x.Sale).ToList();
            }
            if (incommingModel.New)
            {
                list = list.Where(x => x.New).ToList();
            }
            #endregion

            rezult.TotalCount = list.Count();
            rezult.ListProdId = list.Select(x => x.ProdId).ToList();
            rezult.minPrice = (int)list.Select(x => x.Price).Min();
            rezult.maxPrice = (int)list.Select(x => x.Price).Max();

            //По релевантности по умолчанию
            rezult.Items = list.OrderByDescending(x => x.Rank).Skip((incommingModel.page - 1) * incommingModel.PageProductCount).Take(incommingModel.PageProductCount).ToList();

            //Из поиска по цене вверх
            if (incommingModel.UpPriceOrder.Value)
            {
                //rezult.Items = rezult.Items.OrderByDescending(x => x.Rank).ThenBy(x => x.Sort).ToList();
                rezult.Items = rezult.Items.OrderBy(x => x.Sort).ToList();
            }

            //Из поиска по цене вниз
            if (!incommingModel.UpPriceOrder.Value)
            {
                //rezult.Items = rezult.Items.OrderByDescending(x => x.Rank).ThenByDescending(x => x.Sort).ToList();
                rezult.Items = rezult.Items.OrderByDescending(x => x.Sort).ToList();
            }

            return rezult;
        }

        public virtual FilterModel GetFilterAtribut(List<int> LisstProdId, List<String> MatSeltedList = null, List<String> NanSeltedList = null, List<String> BrdSeltedList = null, List<String> ClrSeltedList = null)
        {
            FilterModel rezult = new FilterModel();
            rezult.Materials = new List<SelectListItem>();
            rezult.Nanesenie = new List<SelectListItem>();
            rezult.Brands = new List<SelectListItem>();
            rezult.ColorsAll = new List<SelectListItem>();
            rezult.ColorsSelected = new List<SelectListItem>();

            rezult.Materials.AddRange(db.Prods_Materials.Where(x => LisstProdId.Contains(x.ProdId)).
                Select(x => new SelectListItem { Text = x.Materials_spr.Name, Value = "0" }).Distinct().AsEnumerable());

            rezult.Nanesenie.AddRange(db.Prods_Nanesenie.Take(1000).Where(x => LisstProdId.Contains(x.ProdId)).
                Select(x => new SelectListItem { Text = x.Nanesenie_spr.Name, Value = "0" }).Distinct().AsEnumerable());

            rezult.Brands.AddRange(db.Products.Where(x => LisstProdId.Contains(x.Id) && !String.IsNullOrEmpty(x.Brands.Name)).
             Select(x => new SelectListItem { Text = x.Brands.Name, Value = "0" }).Distinct().AsEnumerable());

            List<int> clrListId = db.Prods_Colors.Where(x => LisstProdId.Contains(x.ProdId))
                .Select(x => x.ColorId).Distinct().ToList();

            if (ClrSeltedList != null && ClrSeltedList.Count() > 0)
            {
                rezult.ColorsSelected = db.Colors_spr.Where(x => ClrSeltedList.Contains(x.Id.ToString()))
               .Select(x => new SelectListItem { Text = x.RGB, Value = x.Id.ToString(), Selected = true }).ToList();

                clrListId = clrListId.Except(ClrSeltedList.Select(x => int.Parse(x))).ToList();
            }

            rezult.ColorsAll = db.Colors_spr.Where(x => clrListId.Contains(x.Id))
              .Select(x => new SelectListItem { Text = x.RGB, Value = x.Id.ToString(), Selected = false }).ToList();

            if (MatSeltedList != null && MatSeltedList.Count() > 0)
            {
                foreach (var item in MatSeltedList)
                {
                    rezult.Materials.Where(x => x.Text == item).Select(x => { x.Value = "1"; return x; }).ToList();
                }
            }

            if (NanSeltedList != null && NanSeltedList.Count() > 0)
            {
                foreach (var item in NanSeltedList)
                {
                    rezult.Nanesenie.Where(x => x.Text == item).Select(x => { x.Value = "1"; return x; }).ToList();
                }
            }

            if (BrdSeltedList != null && BrdSeltedList.Count() > 0)
            {
                foreach (var item in BrdSeltedList)
                {
                    rezult.Brands.Where(x => x.Text == item).Select(x => { x.Value = "1"; return x; }).ToList();
                }
            }

            return rezult;
        }

        public virtual List<ProdList> GetProdListFTS(LineJsonModel model)
        {
            List<ProdList> rezult = new List<ProdList>();
            //-------------------param1-----------------------//
            var dataTable1 = new DataTable();
            dataTable1.TableName = "dbo.ListCatId";
            dataTable1.Columns.Add("CatId", typeof(int));
            if (model.ListCatId!=null && model.ListCatId.Count>0)
            {
                foreach (var item in model.ListCatId)
                {
                    dataTable1.Rows.Add(item); // Id of '1' is valid for the Person table
                }
            }

            SqlParameter tableParam1 = new SqlParameter("CatIdArray", SqlDbType.Structured);
            tableParam1.TypeName = dataTable1.TableName;
            tableParam1.Value = dataTable1;

            //-------------------param2-----------------------//
            SqlParameter stringParam = new SqlParameter("@query", model.QueryString);

            //-------------------param3-----------------------//
            var dataTable2 = new DataTable();
            dataTable2.TableName = "dbo.ListString";
            dataTable2.Columns.Add("Name", typeof(string));
            if (model.Materials!=null && model.Materials.Count>0)
            {
                foreach (var item in model.Materials.Distinct())
                {
                    dataTable2.Rows.Add(item); // Id of '1' is valid for the Person table
                }
            }

            SqlParameter tableParam2 = new SqlParameter("MaterialArray", SqlDbType.Structured);
            tableParam2.TypeName = dataTable2.TableName;
            tableParam2.Value = dataTable2;

            //-------------------param4-----------------------//
            var dataTable3 = new DataTable();
            dataTable3.TableName = "dbo.ListString";
            dataTable3.Columns.Add("Name", typeof(string));
            if (model.Nanesenie!=null && model.Nanesenie.Count>0)
            {
                foreach (var item in model.Nanesenie.Distinct())
                {
                    dataTable3.Rows.Add(item); // Id of '1' is valid for the Person table
                }
            }

            SqlParameter tableParam3 = new SqlParameter("NanesenieArray", SqlDbType.Structured);
            tableParam3.TypeName = dataTable3.TableName;
            tableParam3.Value = dataTable3;

            //-------------------param4-----------------------//
            var dataTable4 = new DataTable();
            dataTable4.TableName = "dbo.ListString";
            dataTable4.Columns.Add("Name", typeof(string));
            if (model.Brand!=null && model.Brand.Count>0)
            {
                foreach (var item in model.Brand.Distinct())
                {
                    dataTable4.Rows.Add(item); // Id of '1' is valid for the Person table
                }
            }

            SqlParameter tableParam4 = new SqlParameter("BrandArray", SqlDbType.Structured);
            tableParam4.TypeName = dataTable4.TableName;
            tableParam4.Value = dataTable4;

            //-------------------param5-----------------------//
            var dataTable5 = new DataTable();
            dataTable5.TableName = "dbo.ListCatId";
            dataTable5.Columns.Add("Name", typeof(int));
            if (model.Color!=null && model.Color.Count>0)
            {
                foreach (var item in model.Color.Distinct())
                {
                    dataTable5.Rows.Add(item); // Id of '1' is valid for the Person table
                }
            }

            SqlParameter tableParam5 = new SqlParameter("ColorArray", SqlDbType.Structured);
            tableParam5.TypeName = dataTable5.TableName;
            tableParam5.Value = dataTable5;

            //-------------------param6-----------------------//
            SqlParameter minPrice = new SqlParameter("@price_min", model.MinPrice);

            //-------------------param7-----------------------//
            SqlParameter maxPrice = new SqlParameter("@price_max", model.MaxPrice);

            //-------------------param8-----------------------//
            SqlParameter priceZprs = new SqlParameter("@price_zps", model.SpriceZprs);

            rezult = db.Database.SqlQuery<ProdList>("exec FTSinProducts @query,@price_min,@price_max,@price_zps,@CatIdArray,@MaterialArray,@NanesenieArray,@BrandArray,@ColorArray", stringParam, minPrice, maxPrice, priceZprs,tableParam1, tableParam2,tableParam3,tableParam4,tableParam5).ToList();

            return rezult;
        }

        public virtual List<ProdList> GetGroupProds(LineJsonModel model,int par)
        {
            List<ProdList> rezult = new List<ProdList>();

            //-------------------param1-----------------------//
            SqlParameter catId = new SqlParameter("@CatId", model.id);
            SqlParameter main = new SqlParameter("@Main",par);

            //-------------------param2-----------------------//
            SqlParameter minPrice = new SqlParameter("@price_min", model.MinPrice);

            //-------------------param3-----------------------//
            SqlParameter maxPrice = new SqlParameter("@price_max", model.MaxPrice);

            var res = string.Concat(db.Database.SqlQuery<string>("exec GetListGroupProds @CatId,@Main,@price_min,@price_max", catId, main, minPrice, maxPrice).ToList());
            rezult = JsonConvert.DeserializeObject<List<ProdList>>(res);

            return rezult;
        }
       


    }
}