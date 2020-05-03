﻿using EditPressRu.Models;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace EditPressRu.Controllers
{
    public class ArticlesController : BaseController
    {
        // GET: Articles
        public ActionResult Index(int page = 1)
        {
            EditPressRuEntities Db = new EditPressRuEntities();
            ArtiklesIndexViewModel model = new ArtiklesIndexViewModel();
            
            ViewBag.metakeywords = @"Рекламная продукция, услуги полиграфии в Москве, сувениры оптом, интернет магазин сувениров, где купить подарки оптом, корпоративные подарки.";
            ViewBag.metadesc = "ЭдитПресс в Москве - рекламно-полиграфический комплекс. Сувенирная и рекламная продукция. Печать на сувенирах. Бизнес сувениры и подарки, полиграфия.";
            ViewBag.PageName = "Artikles";
            int pageSize = 5;

            model.Paging = new Helpers.PageInfo();
            model.Paging.page = page;
            model.Paging.PageSize = pageSize;
            model.Paging.TotalItems = Db.ArtiklesIndexes.Where(c => c.Publish == true && c.PriznakArtiklActNews == 1).Count();

            if (model.Paging.TotalItems > 1)
            {
                int pNext = page + 1;
                int pPrev = page - 1;

                if (page > 1)
                {
                    ViewBag.Canonical = "https://editpress.ru/articles";
                }
                if (page == 1)
                {
                    ViewBag.Next = String.Format("https://editpress.ru/articles?page={0}", page);
                }
                if (page ==2 && page < model.Paging.TotalItems)
                {
                    ViewBag.Next = String.Format("https://editpress.ru/articles?page={0}", pNext);
                    ViewBag.Prev = "https://editpress.ru/articles";
                }
                if (page > 2 && page < model.Paging.TotalItems)
                {
                    ViewBag.Next = String.Format("https://editpress.ru/articles?page={0}", pNext);
                    ViewBag.Prev = String.Format("https://editpress.ru/articles?page={0}", pPrev);
                }
                if (page == model.Paging.TotalItems)
                {
                    if (page>2)
                    {
                        ViewBag.Prev = String.Format("https://editpress.ru/articles?page={0}", pPrev);
                    }
                    else
                    {
                        ViewBag.Prev = "https://editpress.ru/articles";
                    }
                }

            }

            model.ArtiklesIndexPL = Db.ArtiklesIndexes.Where(c => c.Publish == true && c.PriznakArtiklActNews==1).OrderByDescending(c => c.Id).Skip((page-1)*pageSize).Take(model.Paging.PageSize);
            
            return View(model);
        }

        #region Контроллеры конкретных статей
        public ActionResult Kalendar()
        {
            
            ViewBag.metakeywords = @"Календари оптом, календарь на 201 год, календарь с логотипом, календари в москве оптом, купить календари, настенные календари, календари настольные, вечный календарь, сувенир календарь,
            календарь с картинкой, изготовление календарей, календарь сувенир, корпоративный календарь, календари оригинвльные.";
            ViewBag.metadesc = "Календари с логотипом - любые модели в наличии и на заказ. Печать логотипа и рекламной информации на календарях. Календари на новый год.";
            
            return View();
        }

        public ActionResult Personalisacia()
        {
            ViewBag.metakeywords = @"Ежедневники оптом, планнинги оптом, планинги купить, ежедневники датированные, ежедневники с логотипом, что такое персонализация ежедневников, блокноты купить, тиснение на ежедневниках, фольгирование.";
            ViewBag.metadesc = "ЭдитПресс в Москве - рекламно-полиграфический комплекс. Сувенирная и рекламная продукция. Печать на сувенирах. Бизнес сувениры и подарки, полиграфия.Персонализация ежедневников";
            return View();
        }

        public ActionResult InernetMagazin()
        {
            ViewBag.metakeywords = @"Недорогой интернет магазин подарков, подарки оптом, сезонные подарки, купить онлайн, бизнес сувериры, сувениры оптом, сувениры для офиса, корпоративные подарки, нанесение логотипа
            подарки с логотипом, футболки с нанесением, ежедневники оптом, что подарить мужчинам на 23 февраля, сезонные подарки, бизнес подарки, подарки на 8 марта, что подарить женщинам, подарки на рождество.";
            ViewBag.metadesc = "ЭдитПресс в Москве - рекламно-полиграфический комплекс. Сувенирная и рекламная продукция. Печать на сувенирах. Бизнес сувениры и подарки, полиграфия.Интернет магазин подарков";
            return View();
        }

        public ActionResult Fleshka()
        {
            ViewBag.metakeywords = @"Недорогой интернет магазин подарков, подарки оптом, сезонные подарки, купить онлайн, бизнес сувериры, сувениры оптом, сувениры для офиса, корпоративные подарки, нанесение логотипа
            подарки с логотипом, футболки с нанесением, ежедневники оптом, что подарить мужчинам на 23 февраля, сезонные подарки, бизнес подарки, подарки на 8 марта, что подарить женщинам, подарки на рождество.";
            ViewBag.metadesc = "ЭдитПресс в Москве - рекламно-полиграфический комплекс. Сувенирная и рекламная продукция. Печать на сувенирах. Бизнес сувениры и подарки, полиграфия.Интернет магазин подарков";
            
            return View();
        }

        public ActionResult Gadget()
        {
            ViewBag.metakeywords = @"Недорогой интернет магазин подарков, подарки оптом, купить онлайн, бизнес сувериры, сувениры оптом, сувениры для офиса, корпоративные подарки, нанесение логотипа,
            подарки с логотипом, гаджеты с логотипом,мышь с логотипом, powerbank купить оптом, бизнес подарки, подарки на 8 марта, что подарить женщинам, подарки на рождество.";
            ViewBag.metadesc = "ЭдитПресс в Москве - рекламно-полиграфический комплекс. Сувенирная и рекламная продукция. Печать на сувенирах. Бизнес сувениры и подарки, полиграфия.Интернет магазин подарков";
            
            return View();
        }

        public ActionResult Ruchka()
        {
            ViewBag.metakeywords = @"Ручки оптом, ручки под нанесение логотипа, купить ручки в Москве, купить ручки в Калуге, купить перьевые ручки, ручки с брендом, ручки с логотипом, ручки в подарок.";
            ViewBag.metadesc = "ЭКомпания ЭдитПресс предлагает купить оптом ручки для нанесения логотипа.Различный ценовой диапазон, самые различные виды ручек. Брендовая продукция.";
            
            return View();
        }

        public ActionResult Brelok()
        {
            ViewBag.metakeywords = @"брелоки оптом, брелоки под нанесение логотипа, купить брелоки в Москве, купить брелоки в Калуге, брелоки с фонариком, брелоки промо, брелоки с логотипом, брелоки отвертки, брелоки многофункциональные.";
            ViewBag.metadesc = "Компания ЭдитПресс предлагает купить оптом брелоки для нанесения логотипа.Различный ценовой диапазон.";
            
            return View();
        }

        public ActionResult Chasy()
        {
            ViewBag.metakeywords = @"часы настенные,часы настольные, часы с логотипом, часы яркие, часы яркое время, часы синий человек, погодные станции, часы погодные станции, барометр, часы наручные, часы на 8 марта, часы оптом, часы на 23 февраля.";
            ViewBag.metadesc = "Компания ЭдитПресс предлагает купить оптом часы. Нанесем на них рекламу и логотип компании. Тампопечать, гравировка, деколь, УФ печать, шелкография и др.";
            
            return View();
        }

        public ActionResult Kuhnya()
        {
            ViewBag.metakeywords = @"Что подарить на день св. Валентина, что подарить на 23 февраля, что подарить на Татьянин день, что подарить на 8 марта, подарки на весь офис, винные наборы купить оптом, 
                                    чашки и кружки купить оптом, печать на кружках, нанесение логотипа на кружки, купить термосы оптом, купить термокружки оптом,купить фартуки оптом,  купить прихватки оптом, 
                                    вышивка на фартуках логотипа, вышивка логотипа на кухонных прихватках, сувениры из фарфора оптом";
            ViewBag.metadesc = @"Компания ЭдитПресс предлагает купить оптом товары из категории кухня и посуда. Винные наборы, наборы стаканов, рюмок и бокалов, наборы для коктейлей и баров, наборы кухонные, кружки и чашки.
            Нанесем на них рекламу и логотип компании. Тампопечать, гравировка, деколь, УФ печать,шелкография и др.";
            
            return View();
        }

        public ActionResult SezonniePodarki()
        {
            ViewBag.metakeywords = @"Что подарить на день св. Валентина, что подарить на 23 февраля, что подарить на Татьянин день, что подарить на 8 марта, подарки на весь офис, сезонные подарки,
            подарки на лето, пляжные игры, пляжные мячи,наборы для барбекю, мангалы, пляжные тапочки оптом, носки оптом, одежда с логотипом оптом, зонты оптом, нанесение логотипа на зонты, дождевики купить оптом";
            ViewBag.metadesc = @"Компания ЭдитПресс предлагает купить оптом товары из категории сезонные подарки. Подарки ко всем праздникам. Одежда для дресс-кода. Одежда с логотипом.
            Нанесем на них рекламу и логотип компании. Тампопечать, гравировка, деколь, УФ печать,шелкография и др.";
            
            return View();
        }


        public ActionResult Knife()
        {
            ViewBag.metakeywords = @"Что подарить на 23 февраля, подарки на день отечества,  подарки на весь офис,  нанесение логотипа на ножи, ножи купить оптом, многофункциональные ножи оптом, кухонные ножи оптом, ножи с инструментом, складные ножи с инструментом, шведский нож, нож летчика.";
            ViewBag.metadesc = @"Компания ЭдитПресс предлагает купить оптом различные ножи. Как сувенирные, так и кухонные.
            Нанесем на них рекламу и логотип компании. Тампопечать, гравировка, деколь, УФ печать,шелкография и др.";
            
            return View();
        }

        public ActionResult VosmoeMarto()
        {
            ViewBag.metakeywords = @"Что подарить на 8 марта, подарки на 8 марта оптом,  подарки на весь офис,  нанесение логотипа на подарки, нанесение логотипа на сувениры, сувениры для женщин оптом, 
            подарки на международный женский день, весенние подарки, подарки для женщин оптом, товары для женщин оптом.";
            ViewBag.metadesc = @"Компания ЭдитПресс предлагает купить оптом подарки для женщин. Бизнес сувениры оптом.
            Нанесем на них рекламу и логотип компании. Тампопечать, гравировка, деколь, УФ печать,шелкография и др.";
            
            return View();
        }

        public ActionResult Zont()
        {
            ViewBag.metakeywords = @"зонты оптом,купить зонты оптом,дешевые зонты оптом,зонты оптом +в москве,зонт трость оптом,пляжные зонты оптом,зонты женские оптом,зонты три слона оптом,	
            купить зонты оптом дешево,купить зонты оптом +в москве,зонты оптом цена,купить оптом пляжные зонты,зонт трость оптом купить,зонты оптом +по низкой цене,мужские зонты оптом,зонты оптом +в москве дешево,	
            итальянские зонты оптом,купить пляжный зонт +с напылением оптом,зонты опт под зонтом,оптом складные зонты,зонт белый опт,зонты +с логотипом оптом,зонты мелким оптом,зонты zest оптом,	
            зонты китай оптом,зонты +для пляжа оптом,зонты три слона купить оптом,белые зонты купить оптом,орион зонты оптом,зонты оптом +для сп,качественные зонты оптом,продажа зонтов оптом,	
            белый зонт трость оптом,зонты однотонные оптом,зонт джедай оптом,зонты трости оптом +в москве";

            ViewBag.metadesc = @"Компания ЭдитПресс предлагает купить оптом подарки для женщин. Бизнес сувениры оптом.
            Нанесем на них рекламу и логотип компании. Тампопечать, гравировка, деколь, УФ печать,шелкография и др.";
            
            return View();
        }

        public ActionResult PowerBank()
        {
            ViewBag.metakeywords = @"power bank оптом, купить power bank оптом, power bank оптом +в москве, аккумулятор power bank оптом, power bank xiaomi оптом,
            внешний аккумулятор power bank оптом, solar power bank оптом, power bank купить оптом +в москве, power bank 20000 оптом,power bank 20000 mah оптом, купить power bank xiaomi оптом,
            power bank mi оптом, xiaomi mi power bank оптом, power bank +на солнечных батареях оптом, power bank solar купить оптом, купить power bank оптом оригинал, power bank 2600 mah оптом,
            solar power bank 20000 mah оптом, solar charger power bank оптом, power bank proda remax оптом, power bank menu оптом, xiaomi power bank 10000 оптом, xiaomi power bank 20800 оптом,
            power bank xiaomi оригинал оптом, power bank 2600 mah remax оптом, power bank оптом mobilvape, power bank xiaomi 20800 mah оптом, power bank xiaomi оптом москва, power bank 10400 оптом,
            power bank xiaomi 16000 оптом, solar power bank 12000 mah оптом, power bank xiaomi 20000 mah оптом, xiaomi power bank 2 20000 оптом, xiaomi power bank 10400 оптом, цена power bank 20800 mah оптом,
            power bank кроссовок оптом, power bank 5000 remax оптом, внешний аккумулятор power bank купить оптом, power bank 10400 mah оптом, power bank menu купить оптом +в москве, hiper power bank купить оптом,
            купить power bank nomi оптом, оригинальные power bank оптом, power bank +с фонариком лого оптом, power bank 35000 оптом, power bank 20 000 mah оптом";

            ViewBag.metadesc = @"Компания ЭдитПресс предлагает купить оптом корпоративные подарки. Бизнес сувениры оптом.
            Нанесем на них рекламу и логотип компании. Тампопечать, гравировка, деколь, УФ печать,шелкография и др.";
            
            return View();
        }

        #endregion



        //***************************************************************************************************************//

        #region Акции

        public ActionResult Akciya()
        {
            
            ViewBag.metakeywords = @"Рекламная продукция, услуги полиграфии в Москве,услуги полиграфии в Московской области, сувениры оптом, интернет магазин сувениров, где купить подарки оптом, корпоративные подарки, купить подарки по акции, нанесение логотипа по акции, экономия на логотипе.";
            ViewBag.metadesc = "ЭдитПресс в Москве - рекламно-полиграфический комплекс. Сувенирная и рекламная продукция. Печать на сувенирах. Бизнес сувениры и подарки, полиграфия.";
            
            return View();
        }

        public ActionResult Vosmoe_marta()
        {
            
            ViewBag.metakeywords = @"Подарки к 8 марта оптом, купить подарки на 23 февраля, подарки на 8 марта оптом, подарки к дню защитника отечества оптом, подарки мелкий опт, подарки большой опт,
            Рекламная продукция, сувениры оптом, интернет магазин сувениров, где купить подарки оптом, 
                корпоративные подарки, купить подарки по акции, нанесение логотипа по акции, экономия на логотипе.";
            ViewBag.metadesc = "ЭдитПресс в Москве - рекламно-полиграфический комплекс. Сувенирная и рекламная продукция. Печать на сувенирах. Бизнес сувениры и подарки, полиграфия.";
            
            return View();
        }

        public ActionResult Pasha()
        {
            
            ViewBag.metakeywords = @"Подарки на пасху оптом, купить подарки на пасху, пасхальные сувениры оптом, подарки мелкий опт, подарки большой опт,
            Рекламная продукция, сувениры оптом, интернет магазин сувениров, где купить подарки оптом, сувениры на пасху оптом, яйца шоколадные пасхальные оптом, визитки печать, визитки оптом,  флаеры оптом, полиграфия в Москве,
                корпоративные подарки, купить подарки по акции, нанесение логотипа по акции, экономия на логотипе.";
            ViewBag.metadesc = "ЭдитПресс в Москве - рекламно-полиграфический комплекс. Сувенирная и рекламная продукция. Печать на сувенирах. Бизнес сувениры и подарки, полиграфия.";
            
            return View();
        }

        public ActionResult Vizitka()
        {
            
            ViewBag.metakeywords = @"Визитки в Москве, визитки недорого, визитки большой тираж, визитки оперативно, визитки шелкография, визитки тиснение, флешки-визитки оптом, визитка нож, 
                                        пластиковая карта нож визитка, полиграфия в Москве, полиграфические услуги в Москве, полиграфия в Московской области, полиграфические услуги.";
            ViewBag.metadesc = "ЭдитПресс в Москве - рекламно-полиграфический комплекс. Сувенирная и рекламная продукция. Печать на сувенирах. Бизнес сувениры и подарки, полиграфия.";
            
            return View();
        }

        public ActionResult Fifa2018()
        {
            ViewBag.metakeywords = @"лицензионные сувениры чемпионата мира 2018, сувениры к чемпионату мира, сувениры fifa 2018, сувениры fifa, футбольные сувениры.";
            ViewBag.metadesc = "Футбольные сувениры и подарки под нанесение логотипа от компании ЭдитПресс. Гарантия низкой цены и высокого качества. Быстрая доставка по Москве и России. РПК «ЭдитПресс».";
            return View();
        }

        public ActionResult RuchkaWithKlip()
        {
            ViewBag.metakeywords = @"Ручки со специальными клипами,Ручка шариковая с индивидуальным дизайном клипа,Ручки с фигурным акриловым клипом,
            Ручки с фигурным пластиковым клипом оптом,Ручки с фигурным пластиковым клипом под логотип,
            Закажите Ручки с фигурным пластиковым клипом 2018 2019 под нанесение логотипа. Скидки от тиража,Ручки с индивидуальным клипом в виде логотипа,
            изготовление клипов по индивидуальному дизайну,Ручки индивидуальные на заказ с логотипом,Ручки индивидуального дизайна,Ручки по индивидуальному дизайну";
            ViewBag.metadesc = "Ручки с фигурным пластиковым клипом оптом, под нанесение логотипа от компании ЭдитПресс. Гарантия низкой цены и высокого качества.Быстрая доставка по Москве и России. РПК «ЭдитПресс».";
            return View();
        }


        #endregion




        #region Спецпредложения

        #endregion

        #region Новинки

        #endregion

    }
}