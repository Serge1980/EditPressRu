﻿using EditPressRu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EditPressRu.Repository
{
    public partial class DataRepository
    {

        public virtual MetaTagsViewModel GetMetaTags(int CatId = 0, int ProdId = 0, string indexName = "")
        {
            MetaTagsViewModel rezult = new MetaTagsViewModel();

            try
            {



                if (CatId == 0)
                {
                    rezult.MetaKey = "Сувениры из Китая EditPress, Уникальные Сувениры из Китая, Полиграфическая продукция,Рекламная плиграфия,Сувенирка, Рекламные сувениры.";
                    rezult.MetaDesk = "Сувениры из Китая РПК ЭдитПресс";
                }

                if (CatId > 0)
                {
                    rezult.MetaDesk = db.Categories.FirstOrDefault(x => x.Id == CatId).MetaDesc;
                    rezult.MetaKey = db.Categories.FirstOrDefault(x => x.Id == CatId).MetaKey;
                }
                if (ProdId > 0)
                {
                    rezult.MetaKey = db.Products.FirstOrDefault(x => x.Id == ProdId).MetaKeyWords;
                    rezult.MetaDesk = db.Products.FirstOrDefault(x => x.Id == ProdId).MetaDescript;
                }

                switch (indexName)
                {
                    case "Index":
                        rezult.MetaKey = "полиграфия оптом, флаеры оптом,визитки оптом, сувенирная продукция оптом, сувенирная продукция для печати логотипа, сувенирная продукция с вашим логотипом, оригинальная сувенирная продукция, сувениры с логотипом, подарки оптом,корпоративные сувениры, рекламные подарки и сувениры, сувенирка, изготовление сувенирной продукции, деловые сувениры, изготовление сувениров, деловые подарки, необычные корпоративные подарки, корпоративные подарки, эксклюзивные подарки для бизнеса, оригинальные сувениры, элитные корпоративные подарки, vip сувениры";
                        rezult.MetaDesk = "Компания &#171;ЭдитПресс&#187; Это оптовый склад сувенирной продукции в Москве, огромный выбор, низкие цены, полиграфические услуги, нанесение логотипа.";
                        break;
                    case "AboutCompany":
                        rezult.MetaKey = "сувениры оптом, подарки к праздникам, тематические сувениры, промо сувениры, сувениры для нанесения, нанесение логотипа, доставка сувениров из Китая, поставщики сувениров";
                        rezult.MetaDesk = "Компания &#171;ЭдитПресс&#187; это огромный выбор сувениров, полиграфические услуги, приятные цены";
                        break;
                    case "Proizvodstvo":
                        rezult.MetaKey = "Полиграфия в Москве, печать флаеров в Москве, изготовление визиток в москве, нанесение шелкографии, уф-печать на кружках, сублимация на ткани, нанесение символики на одежду,офсетная печать, надглазурная деколь, печать на рулонах, печать А1 в Москве";
                        rezult.MetaDesk = "Компания &#171;ЭдитПресс&#187; это рекламно-полиграфический холдинг в Москве. Мы осуществляем доставку и оптовую продажу сувениров, нанесение логотипа, рекламная полиграфия, широкоформатная печать.";
                        break;
                    case "Uslugy":
                        rezult.MetaKey = "Полиграфия в Москве, печать флаеров в Москве, изготовление визиток в москве, нанесение шелкографии, уф-печать на кружках, сублимация на ткани, нанесение символики на одежду,офсетная печать, надглазурная деколь, печать на рулонах, печать А1 в Москве";
                        rezult.MetaDesk = "Компания &#171;ЭдитПресс&#187; это рекламно-полиграфический холдинг в Москве. Мы осуществляем доставку и оптовую продажу сувениров, нанесение логотипа, рекламная полиграфия, широкоформатная печать.";
                        break;
                    case "Cases":
                        rezult.MetaKey = "Полиграфия в Москве, печать флаеров в Москве, изготовление визиток в москве, нанесение шелкографии, уф-печать на кружках, сублимация на ткани, нанесение символики на одежду,офсетная печать, надглазурная деколь, печать на рулонах, печать А1 в Москве";
                        rezult.MetaDesk = "Компания &#171;ЭдитПресс&#187; это рекламно-полиграфический холдинг в Москве. Мы осуществляем доставку и оптовую продажу сувениров, нанесение логотипа, рекламная полиграфия, широкоформатная печать.";
                        break;

                    case "Brands":
                        rezult.MetaKey = "Брендовая продукция, Брендовая продукция оптом, логотип на брендовую продукцию, бренды ведущих европейских марок, брендовые сувениры.";
                        rezult.MetaDesk = "Компания ЭдитПресс это рекламно-полиграфический холдинг в Москве. Мы осуществляем доставку и оптовую продажу сувениров, нанесение логотипа, рекламная полиграфия, широкоформатная печать.";
                        break;


                        //case "Cases":
                        //    rezult.MetaKey = "fff";
                        //    rezult.MetaDesk = "ttt";
                        //    break;
                        //case "Cases":
                        //    rezult.MetaKey = "fff";
                        //    rezult.MetaDesk = "ttt";
                        //    break;
                        //case "Cases":
                        //    rezult.MetaKey = "fff";
                        //    rezult.MetaDesk = "ttt";
                        //    break;
                        //case "Cases":
                        //    rezult.MetaKey = "fff";
                        //    rezult.MetaDesk = "ttt";
                        //    break;
                        //case "Cases":
                        //    rezult.MetaKey = "fff";
                        //    rezult.MetaDesk = "ttt";
                        //    break;
                        //case "Cases":
                        //    rezult.MetaKey = "fff";
                        //    rezult.MetaDesk = "ttt";
                        //    break;
                        //default:
                        //    //do a different thing
                        //break;
                }
            }
            catch (Exception)
            {
                rezult.MetaKey = @"сувениры оптом,подарки с логотипом, сувениры с логотипом,нанесение, нанесение логотипа, полиграфия в Москве, стать дилером сувенирной продукции, 
                                    поставщии сувенирной продукции, найти поставщика сувениров, оптовый склад сувениров в Москве";
                rezult.MetaDesk = "РПК ЭдитПресс оптовая торговля сувенирами и полиграфические услуги в Москве. Нанесение логотипа. Лазерная гравировка";
            }

            return rezult;
        }
    }

}

