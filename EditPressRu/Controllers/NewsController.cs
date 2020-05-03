using EditPressRu.Models;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Controllers
{
    public class NewsController : BaseController
    {
        // GET: News
        public ActionResult Index(int page=1)
        {
            ViewBag.Title = "Новости ЭдитПресс";
            ViewBag.metakeywords = @"Рекламная продукция, услуги полиграфии в Москве, сувениры оптом, интернет магазин сувениров, где купить подарки оптом, корпоративные подарки.";
            ViewBag.metadesc = "ЭдитПресс в Москве - рекламно-полиграфический комплекс. Сувенирная и рекламная продукция. Печать на сувенирах. Бизнес сувениры и подарки, полиграфия.Наши новости.";
            ViewBag.PageName ="News";

            EditPressRuEntities Db = new EditPressRuEntities();
            ArtiklesIndexViewModel model = new ArtiklesIndexViewModel();

            int pageSize = 5;

            model.Paging = new Helpers.PageInfo();
            model.Paging.page = page;
            model.Paging.PageSize = pageSize;

            model.Paging.TotalItems = Db.ArtiklesIndexes.Where(c => c.Publish == true && c.PriznakArtiklActNews == 2).Count();
            model.ArtiklesIndexPL = Db.ArtiklesIndexes.Where(c => c.Publish == true && c.PriznakArtiklActNews == 2).OrderBy(c => c.Id).Skip((page - 1) * pageSize).Take(model.Paging.PageSize);

            return View(model);
        }

        public ActionResult MultyKabel()
        {
            ViewBag.metakeywords = @"зарядное устройство для iPhone, зарядное устройство для смартфонов, зарядное устройство для планшета, зарядное устройство для телефона,
                                зарядное устройство для iPad, зарядное устройство для спорта, мультизарядное устройство, зарядное устройство с мультикабелем, 
                                зарядное устройство с мультиинтерфейсом, зарядное устройство для всего, зарядка для телефона, зарядка для iPad,
                                зарядка с USB кабелем, зарядка iPhone 5/6/7/8/Х кабель,зарядка iPhone 4 кабель, зарядка TYPE C кабель,зарядка микро USB кабель, зарядка мини USB кабель";
            ViewBag.metadesc = "Компания ЭдитПресс представляет новый продукт на рынке гаджетов  - универсальное зарядное устройство, с  мультикабелем";
            return View();
        }

    }
}

