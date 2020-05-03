using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;
using System.IO;
using System.Text;
using EditPressRu.Models.DB;
using EditPressRu.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System.Text.RegularExpressions;
using System.Globalization;
using Newtonsoft.Json;
using System.Threading;

namespace EditPressRu.Controllers
{
    public class KpStoreController:Controller
    {
        private static EditPressRuEntities Db = new EditPressRuEntities();

        // GET: KpStore
        [Authorize]
        public ActionResult Index()
        {
            UserInf user = GetUser();
            ViewBag.UserName = user.UserName;
            int userId = user.UserId;

            //List<int> listOrderId = Db.Orders.Where(x => x.UserID == userId).Select(x => x.OrderID).ToList();
            KPlistViewModel model = new KPlistViewModel();

            //model.KPstoreList = Db.KP_Store.Where(x => listOrderId.Contains(x.OrderId)).ToList();
            try
            {
                model.KPstoreList = Db.Database.SqlQuery<KpStoreView>(@"select 
                    a.DateDoc as DateDoc,a.Id as KpId,a.KpImg as KpImg,a.KpNumber as KpNumber,sum(b.Count) as TotalItem from KP_Store a
                    inner join OrderDetails b on a.OrderId=b.OrderID
                    where UserId={0}
                    group by a.DateDoc,a.Id,a.KpImg,a.KpNumber", userId).ToList(); //Db.KP_Store.Where(x => x.UserId == userId).ToList();
            }
            catch
            {
            }


            model.Availability = false;
            if (model.KPstoreList != null && model.KPstoreList.Count > 0)
            {
                model.Availability = true;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateKP(int orderId)
        {
            UserInf user = GetUser();
            ViewBag.UserName = user.UserName;
            int userId = user.UserId;

            EditKPViewModel model = new EditKPViewModel();

            KP_Store kp = new KP_Store();

            try
            {
                kp = Db.KP_Store.FirstOrDefault(x => x.OrderId == orderId);
            }
            catch
            {
            }

            if (kp != null)
            {
                model.KpStore = kp;
            }
            else
            {
                model.KpStore = new KP_Store()
                {
                    Title = "Коммерческое предложение",
                    UserId = userId,
                    Currency = "Руб",
                    DateDoc = DateTime.Now,
                    Description = "",
                    СurrencyValue = 1M,
                    UserP = true,
                    TotalP = true,
                    ArticleP = true,
                    CharacterP = true,
                    DescrP = true,
                    SumTirageP = true,
                    OrderId = orderId,
                    KpNumber = "",
                    KpForeColor = "/images/kpfone/2-bridge.jpg"

                };

                Db.KP_Store.Add(model.KpStore);
                Db.SaveChanges();
            }

            model.KpStore.KpNumber = String.Format("KP{0}_{1}", orderId, model.KpStore.Id);

            int prodId = Db.OrderDetails.FirstOrDefault(x => x.OrderID == orderId).ProductId;

            try
            {
                ProdImages image = Db.ProdImages.FirstOrDefault(x => x.ProdId == prodId && x.Main);
                if (image.Small != null)
                {
                    model.KpStore.KpImg = image.Small;
                }
                else
                {
                    model.KpStore.KpImg = image.Big;
                }
            }
            catch 
            {
                model.KpStore.KpImg = "/images/no_image.png";
            }
           
            

            Db.SaveChanges();

            EditKPViewModel returnModel = GetEditKpModel(model.KpStore.Id);

            //model.ProdList = Db.Database.
            //    SqlQuery<OrderProduct>(String.Format("[dbo].[GetOrderProductsForKp]  @OrderId = {0},@par='getOrdL'", orderId)).ToList();

            //model.UserInfo = Db.UserProfileDetails.FirstOrDefault(x => x.UserID == userId);

            //model.ImagesList = Db.Database.
            //    SqlQuery<string>(String.Format("[dbo].[GetOrderProductsForKp]  @OrderId = {0},@par='getImgL'", orderId)).ToList();

            ViewBag.PageName = "EditKP";
            return View("EditKP", returnModel);
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditKP(int id)
        {
            UserInf user = GetUser();
            ViewBag.UserName = user.UserName;
            ViewBag.PageName = "EditKP";
            int userId = user.UserId;

            EditKPViewModel model = GetEditKpModel(id);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditKp(KpToSaveViewModel model)
        {
            KP_Store kpItem = Db.KP_Store.SingleOrDefault(x => x.Id == model.KpId);
            
            UserProfileDetails userProf = Db.UserProfileDetails.SingleOrDefault(x => x.UserID == kpItem.UserId);

            userProf.LogoImg = model.Logo;

            kpItem.Title = model.KpTitle;
            kpItem.Description = model.KpDescript;
            kpItem.KpForeColor = String.Format("/images/kpfone/{0}", model.FoneColor);

            kpItem.AlternateContacts = model.KpContacts;
            kpItem.ArticleP = model.ArticleP;
            kpItem.UserP = model.UserP;
            kpItem.CharacterP = model.CharacterP;
            kpItem.DescrP = model.DescrP;
            kpItem.SumTirageP = model.SumTirageP;
            kpItem.TotalP = model.TotalP;

            if (model.PriceArr!=null && model.PriceArr.Count>0)
            {
                foreach (var item in model.PriceArr)
                {
                    try
                    {
                        int id = int.Parse(item.Value);
                        decimal price = decimal.Parse(item.Text);
                        OrderDetails orderDetls = Db.OrderDetails.FirstOrDefault(x => x.OrderID == kpItem.OrderId && x.ProductId == id);
                        orderDetls.ItemPrice = price;
                        Db.SaveChanges();
                    }
                    catch
                    {
                    }

                }
            }

            if (model.CntArr != null && model.CntArr.Count > 0)
            {
                foreach (var item in model.CntArr)
                {
                    try
                    {
                        int id = int.Parse(item.Value);
                        int cnt = int.Parse(item.Text);
                        OrderDetails orderDetls = Db.OrderDetails.FirstOrDefault(x => x.OrderID == kpItem.OrderId && x.ProductId == id);
                        orderDetls.Count = cnt;
                        Db.SaveChanges();
                    }
                    catch
                    {
                    }

                }
            }

            if (model.DescrArr != null && model.DescrArr.Count > 0)
            {
                foreach (var item in model.DescrArr)
                {
                    try
                    {
                        int id = int.Parse(item.Value);
                        OrderDetails orderDetls = Db.OrderDetails.FirstOrDefault(x => x.OrderID == kpItem.OrderId && x.ProductId == id);
                        orderDetls.Descripts = item.Text;
                        Db.SaveChanges();
                    }
                    catch
                    {
                    }

                }
            }

            //перелистывание картинк
            UpdateImgFromList(kpItem, model.ImgBtn, model.ImgProdId, model.IdImg);

            Db.SaveChanges();

            EditKPViewModel returnModel = GetEditKpModel(model.KpId);

            ViewBag.PageName = "EditKP";
            return PartialView("_OsnPartial", returnModel);

        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteKp(int kpId)
        {
            Db.Database.ExecuteSqlCommand(String.Format("delete from Kp_Store_dop where KpId={0}", kpId));
            Db.Database.ExecuteSqlCommand(String.Format("delete from Kp_Store where Id={0}", kpId));

            return Json(Url.Action("Index", "KpStore"));
        }

        [Authorize]
        [HttpPost]
        public ActionResult SendKpLink(int kpId,int Verb)
        {
            string returnModel = null;
            if (Verb==1)
            {
                 returnModel = String.Format("/kpstore/kppublish/{0}", kpId);
                KP_Store kpItem = Db.KP_Store.SingleOrDefault(x => x.Id == kpId);
                kpItem.Publish = returnModel;
                Db.SaveChanges();
            }
            else
            {
                 
                KP_Store kpItem = Db.KP_Store.SingleOrDefault(x => x.Id == kpId);
                kpItem.Publish = null;
                Db.SaveChanges();
            }
           

            return PartialView("_SendLinkKpPartial", returnModel);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult KpPublish(int id)
        {
            EditKPViewModel model = GetEditKpModel(id);
            ViewBag.KpForeColor = model.KpStore.KpForeColor;
            if (String.IsNullOrEmpty(model.KpStore.Publish))
            {
                string url = String.Format("/kpstore/editkp/{0}", id);
                return Redirect(url);
            }
            else
            {
                return View(model);
            }
            
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult TreatmentFoto(int KpId=0, string ImgBtn="", int ImgProdId=0, long IdImg=0)
        {
            KP_Store kpItem = Db.KP_Store.SingleOrDefault(x => x.Id == KpId);
            UpdateImgFromList(kpItem, ImgBtn, ImgProdId, IdImg);

            List<DopImgList> model= Db.Database.
                SqlQuery<DopImgList>("[dbo].[GetDopImgList]  @OrderId = {0},@ProdId={1}", kpItem.OrderId, ImgProdId).ToList();

            return PartialView("_TreatFotoPartial",model);
        }

        [Authorize]
        [HttpGet]
        public ActionResult GetModal(int kpId,string partName)
        {
            EditKPViewModel model = GetEditKpModel(kpId);
            return PartialView(partName, model);

        }

        [Authorize]
        [HttpPost]
        //public ActionResult DopFunction(int kpId,int marg,int sale,List<string> arrSrv)
        public ActionResult DopFunction(int kpId, int marg, int sale,string cur, decimal curVal, List<SelectListItem> arrSrv)
        {
            KP_Store kpstore = Db.KP_Store.SingleOrDefault(x=>x.Id==kpId);

           
                //откатим на исходную цену, если была уже наценка
                if (kpstore.MarkUp.HasValue && kpstore.MarkUp.Value != marg)
                {

                    decimal oldMark = 1 + (decimal)kpstore.MarkUp.Value / (decimal)100;

                    Db.Database.ExecuteSqlCommand(@" Update OrderDetails
		                                     set ItemPrice=ItemPrice/{0}
		                                     where  OrderID={1}", oldMark, kpstore.OrderId);

                //высчитаем новую
                kpstore.MarkUp = marg;
                decimal mark = 1 + (decimal)marg / (decimal)100;

                Db.Database.ExecuteSqlCommand(@" Update OrderDetails
		                                     set ItemPrice=ItemPrice*{0}
		                                     where  OrderID={1}", mark, kpstore.OrderId);

            }

            if (!kpstore.MarkUp.HasValue)
            {
                //высчитаем новую
                kpstore.MarkUp = marg;
                decimal mark = 1 + (decimal)marg / (decimal)100;

                Db.Database.ExecuteSqlCommand(@" Update OrderDetails
		                                     set ItemPrice=ItemPrice*{0}
		                                     where  OrderID={1}", mark, kpstore.OrderId);
            }


            kpstore.Sale = sale;
            kpstore.Currency = cur;

            decimal oldCurVal = kpstore.СurrencyValue;
           
            kpstore.СurrencyValue = curVal;
            
            if (kpstore.СurrencyValue<=0)
            {
                kpstore.СurrencyValue = 1;
            }

           
                List<Kp_Store_dop> listDop = new List<Kp_Store_dop>();
                if (arrSrv != null && arrSrv.Count > 0)
                {

                    foreach (var item in arrSrv)
                    {
                        Decimal price;

                        if (!String.IsNullOrEmpty(item.Text) && Decimal.TryParse(item.Value.Replace(',','.'), NumberStyles.Any, new CultureInfo("en-US"), out price))
                        {
                            Kp_Store_dop kp_dop = new Kp_Store_dop();
                            kp_dop.KpId = kpId;
                            kp_dop.Price = price* oldCurVal/kpstore.СurrencyValue;
                            kp_dop.Service = item.Text;

                            listDop.Add(kp_dop);
                        }
                    }
                }

                listDop = listDop.Distinct().ToList();

                if (listDop.Count > 0)
                {
                    Db.Database.ExecuteSqlCommand("delete from Kp_Store_dop where KpId={0}", kpId);
                    Db.Kp_Store_dop.AddRange(listDop);
                }
          
          
            Db.SaveChanges();

            EditKPViewModel model = GetEditKpModel(kpId);

            //return Json(new { totalSumm = TotalSumm });
            return PartialView("_KpStorePartial", model);

        }

        public ActionResult GetPictureViewer(int kpId,int prodId,bool? next=null, int number=0)
        {
            PicterPartialModel model = new PicterPartialModel();

            int OrderId = Db.KP_Store.SingleOrDefault(x => x.Id == kpId).OrderId;

            model.DopImgList = Db.Database.SqlQuery<DopImgList>(@"select  
		            cast(g.Id as bigint) as ImgId,
		              g.SuperBig as [Image],
		              g.ProdId ,
		             Selected=cast(case when ltrim(rtrim(od.Img))=ltrim(rtrim(g.Small)) then 1 else 0 end as bit),
		             cast( ROW_NUMBER()OVER(Partition By g.ProdId Order By g.Id,g.Main DESC) as int)As NumberImg
		             from ProdImages g 
		             inner join OrderDetails od on od.ProductId=g.ProdId
		             where g.ProdId={0} and od.OrderID={1}", prodId, OrderId).ToList();
            //g.ProdId = 97003 and od.OrderID = 3335
            int num = number;
            if (number==0)
            {
                num = model.DopImgList.FirstOrDefault(x => x.Selected).NumberImg;
            }
            
            int count = model.DopImgList.Count;

            ///next=null то текущий 1 следующий 0 предыдущий
            if (next!=null)
            {
                if (next.Value==true)
                {
                   num = num + 1;
                    if (num>count)
                    {
                        num = 1;
                    }
                }
                else
                {
                    num = num - 1;
                    if (num==0)
                    {
                        num = count;
                    }
                }
            }

            model.Numbr = num;
            model.KpId = kpId;
            model.ProdId = prodId;

            return PartialView("_ImageViewerPartial", model);
        }

        [Authorize]
        public ActionResult PrintKP(int id, int print = 0)
        {
            Byte[] bytes;
            var ms = GetKPToWork(id);

            bytes = ms.ToArray();

            string file_name = Server.MapPath(String.Format("/Reps/kp{0}.pdf", id));

            System.IO.File.WriteAllBytes(file_name, bytes);


            return File(bytes, "application/pdf");

        }

        public FileResult DownloadKp(int id)
        {
            var ms = GetKPToWork(id);
            byte[] fileBytes = ms.ToArray();

            string fileName = Server.MapPath(String.Format("/Reps/kp{0}.pdf", id));
            System.IO.File.WriteAllBytes(fileName, fileBytes);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        

        [Authorize]
        public ActionResult PrintSchet(int id)
        {
            BaseFont baseFont = RegisterFonts();
            string file_name = null;


            //UsersContext dbU = new UsersContext();
            int UserId = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;
            //UserProfileDetailsContext dbUP = new UserProfileDetailsContext();
            UserProfileDetails upd = Db.UserProfileDetails.First(c => c.UserID == UserId);

            string userFio = upd.FIO;
            string userPhone = upd.Phone;
            string userEmail = upd.email;
            string userAdress = upd.ShipmentAddress;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<p>Клиент <strong>{1}</strong> сформировал счет на оплату по заказу <strong>№{0}</strong></p>", id, userFio);
            sb.AppendFormat("<p>Телефон клиента <strong>{0}</strong></p>", userPhone);
            sb.AppendFormat("<p>Электронка <strong>{0}</strong></p>", userEmail);
            sb.AppendFormat("<p>Адрес доставки <strong>{0}</strong></p>", userAdress);
            sb.Append("<p style='color:red;'>Счет находится во вложении</p>");
            string body = sb.ToString();

            Byte[] bytes;
            using (var ms = new MemoryStream())
            {
                iTextSharp.text.Document doc = new Document(iTextSharp.text.PageSize.A4, 10, 10, 10, 10);
                doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());

                var writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var phrase = new Phrase("Продавец: ООО\"ЭдитПресс\"", new Font(baseFont, 10, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                var paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_LEFT;
                doc.Add(paragraf);
                phrase = new Phrase("Адрес: 143080, РФ, Московская обл., Одинцовский р-он, д.п. Лесной городок, ул.Энергетиков, д.7", new Font(baseFont, 10, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_LEFT;
                doc.Add(paragraf);
                phrase = new Phrase("ИНН:5032265383", new Font(baseFont, 10, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_LEFT;
                doc.Add(paragraf);
                phrase = new Phrase("КПП:503201001", new Font(baseFont, 10, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_LEFT;
                doc.Add(paragraf);
                phrase = new Phrase("Расчетный счет: 40702810500120029844", new Font(baseFont, 10, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_LEFT;
                doc.Add(paragraf);
                phrase = new Phrase("Корр. счет: 30101810000000000201", new Font(baseFont, 10, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_LEFT;
                doc.Add(paragraf);

                phrase = new Phrase("БИК: 044525201", new Font(baseFont, 10, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_LEFT;
                doc.Add(paragraf);

                phrase = new Phrase("Банк: ОАО АКБ \"АВАНГАРД\" Г. МОСКВА", new Font(baseFont, 10, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_LEFT;
                doc.Add(paragraf);

                phrase = new Phrase(" ", new Font(baseFont, 10, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_LEFT;
                doc.Add(paragraf);


                if (upd != null)
                {
                    phrase = new Phrase(String.Format("Покупатель: {0}", upd.OrgName), new Font(baseFont, 10, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                    paragraf = new Paragraph(phrase);
                    paragraf.Alignment = Element.ALIGN_LEFT;
                    doc.Add(paragraf);

                    phrase = new Phrase(String.Format("Адрес: {0}", upd.ShipmentAddress), new Font(baseFont, 10, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                    paragraf = new Paragraph(phrase);
                    paragraf.Alignment = Element.ALIGN_LEFT;
                    doc.Add(paragraf);

                    phrase = new Phrase(String.Format("ИНН: {0}", upd.INN), new Font(baseFont, 10, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                    paragraf = new Paragraph(phrase);
                    paragraf.Alignment = Element.ALIGN_LEFT;
                    doc.Add(paragraf);
                }


                phrase = new Phrase(" ", new Font(baseFont, 18, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_LEFT;
                doc.Add(paragraf);

                Orders order = Db.Orders.FirstOrDefault(c => c.UserID == UserId && c.OrderID == id);
                if (order == null)
                {
                    return RedirectToAction("OrderDetails", new { id = id });
                }


                phrase = new Phrase(String.Format("Счет №{0} от {1}", order.OrderID, order.Data.ToShortDateString()), new Font(baseFont, 18, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_CENTER;
                doc.Add(paragraf);

                //table
                var table = new PdfPTable(6);
                table.SpacingBefore = 5f;

                PdfPCell cell = null;


                table.SetWidths(new[] { 70, 300, 70, 100, 100, 100 });
                table.WidthPercentage = 100;


                Font font8 = new Font(baseFont, 9);
                cell = new PdfPCell(new Phrase(new Chunk("№ п/п", font8)));
                ConfigureCellForTableHeader(cell);
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("Наименование", font8)));
                ConfigureCellForTableHeader(cell);
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("Ед.изм.", font8)));
                ConfigureCellForTableHeader(cell);
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("Цена", font8)));
                ConfigureCellForTableHeader(cell);
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("Количество", font8)));
                ConfigureCellForTableHeader(cell);
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(new Chunk("Стоимость", font8)));
                ConfigureCellForTableHeader(cell);
                table.AddCell(cell);


                decimal? disc;
                if (upd.Discounts == null)
                    disc = 0;
                else
                {
                    if (upd.Discounts.DiscountPercent > 0)
                        disc = upd.Discounts.DiscountPercent;
                    else
                        disc = 0;
                }
                PdfPCell[] cells;
                // OrderDetailsContext db = new OrderDetailsContext();
                List<OrderDetails> records = Db.OrderDetails.Where(c => c.OrderID == id).ToList();
                int i = 0;
                Decimal? total = 0;
                foreach (var rec in records)
                {
                    //ItemsContext ic = new ItemsContext();
                    i++;
                    cells = new PdfPCell[6];

                    cells[0] = new PdfPCell(new Phrase(new Chunk(i.ToString(), font8)));
                    cells[0].VerticalAlignment = Element.ALIGN_CENTER;
                    cells[0].HorizontalAlignment = Element.ALIGN_CENTER;

                    cells[1] = new PdfPCell(new Phrase(new Chunk(Db.Products.FirstOrDefault(x => x.Id == rec.ProductId).Name, font8)));
                    cells[1].VerticalAlignment = Element.ALIGN_CENTER;
                    cells[1].HorizontalAlignment = Element.ALIGN_JUSTIFIED;

                    cells[2] = new PdfPCell(new Phrase(new Chunk("шт.", font8)));
                    cells[2].VerticalAlignment = Element.ALIGN_CENTER;
                    cells[2].HorizontalAlignment = Element.ALIGN_CENTER;

                    decimal? PriceWithDiscount = rec.ItemPrice * ((100 - disc) / 100);

                    cells[3] = new PdfPCell(new Phrase(new Chunk(PriceWithDiscount.ToString(), font8)));
                    cells[3].VerticalAlignment = Element.ALIGN_CENTER;
                    cells[3].HorizontalAlignment = Element.ALIGN_CENTER;

                    cells[4] = new PdfPCell(new Phrase(new Chunk(rec.Count.ToString(), font8)));
                    cells[4].VerticalAlignment = Element.ALIGN_CENTER;
                    cells[4].HorizontalAlignment = Element.ALIGN_CENTER;

                    cells[5] = new PdfPCell(new Phrase(new Chunk((PriceWithDiscount * rec.Count).ToString(), font8)));
                    cells[5].VerticalAlignment = Element.ALIGN_CENTER;
                    cells[5].HorizontalAlignment = Element.ALIGN_CENTER;

                    PdfPRow row = new PdfPRow(cells);
                    table.Rows.Add(row);
                    total = total + PriceWithDiscount * rec.Count;
                }

                cells = new PdfPCell[6];
                cells[4] = new PdfPCell(new Phrase(new Chunk("Итого:", font8)));
                cells[4].VerticalAlignment = Element.ALIGN_CENTER;
                cells[4].HorizontalAlignment = Element.ALIGN_RIGHT;

                cells[5] = new PdfPCell(new Phrase(new Chunk(total.ToString(), font8)));
                cells[5].VerticalAlignment = Element.ALIGN_CENTER;
                cells[5].HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPRow _row = new PdfPRow(cells);
                table.Rows.Add(_row);

                doc.Add(table);
                decimal nds = (decimal)total / 120 * 20;
                string SummByWords = total.ToString(); //NumByWords.RurPhrase(total);
                phrase = new Phrase(String.Format("Сумма прописью: {0}, в том числе НДС 20% - {1} руб. {2} коп.", SummByWords, Math.Truncate(nds), Math.Round((nds - Math.Truncate(nds)), 2).ToString().Replace("0,", "")).Replace("-", ""), new Font(baseFont, 10, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_JUSTIFIED;
                doc.Add(paragraf);

                phrase = new Phrase(" ", new Font(baseFont, 18, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_LEFT;
                doc.Add(paragraf);

                phrase = new Phrase(" ", new Font(baseFont, 18, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_LEFT;
                doc.Add(paragraf);

                //--table

                phrase = new Phrase("Руководитель предприятия ___________________ Семенов Д.А.                             Бухгалтер ___________________ Семенов Д.А.", new Font(baseFont, 10, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                paragraf = new Paragraph(phrase);
                paragraf.Alignment = Element.ALIGN_CENTER;
                doc.Add(paragraf);

                doc.Close();

                bytes = ms.ToArray();

                file_name = Server.MapPath(String.Format("/Reps/schet{0}.pdf", id));

                System.IO.File.WriteAllBytes(file_name, bytes);
            }

            HelpFunctions.SendMail2("sales@editpress.ru", String.Format("Сформирован счет на оплату по заказу №{0}", id), body, file_name);
            HelpFunctions.SendMail2("editpress@mail.ru", String.Format("Сформирован счет на оплату по заказу №{0}", id), body, file_name);
            HelpFunctions.SendMail2("chernikoff80@yandex.ru", String.Format("Сформирован счет на оплату по заказу №{0}", id), body, file_name);

            return File(bytes, "application/pdf");
        }
        private static void ConfigureCellForTableHeader(PdfPCell cell)
        {
            cell.Padding = 5f;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //cell.Border= Rectangle.NO_BORDER;
        }

        private MemoryStream GetKPToWork(int id)
        {
            EditKPViewModel model = GetEditKpModel(id);

            BaseFont baseFont = RegisterFonts();

            Byte[] bytes;
            using (var ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 20f, 10f, 25f, 10f);
                doc.SetPageSize(PageSize.A4);

                var writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                //определим hr  между продуктами
                LineSeparator line = new LineSeparator(1f, 100f, BaseColor.GRAY, Element.ALIGN_CENTER, -1);
                PdfPCell hrCell = new PdfPCell(new Phrase(new Chunk(line)));
                hrCell.Colspan = 12;
                hrCell.Border = 0;
                hrCell.PaddingTop = 10f;
                hrCell.PaddingBottom = 10f;

                //string logoImg = "~/images/logo2.png";
                //if (!string.IsNullOrEmpty(model.UserInfo.LogoImg))
                //{
                //    logoImg = model.UserInfo.LogoImg;
                //}

                string logoImg = model.ImageLogo;

                Image img = Image.GetInstance(Server.MapPath(logoImg));
                img.Alignment = Element.ALIGN_LEFT;
                img.ScaleAbsolute(160f, 80f);

                ///////////////////////////////////////////////////////////
                var table = new PdfPTable(12);
                //tablePart1.SpacingBefore = 2f;

                PdfPCell cellP1 = null;

                //table.SetWidths(new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f });
                //table.WidthPercentage = 100.0f;
                //table.LockedWidth = true;

                table.TotalWidth = 500f;
                table.LockedWidth = true;
                table.KeepTogether = false;

                //добавили лого
                if (model.KpStore.UserP)
                {
                    cellP1 = new PdfPCell(img);
                    ConfigureCellForTableHeader(cellP1);
                    cellP1.Colspan = 6;
                    cellP1.Border = 0;
                    table.AddCell(cellP1);

                    //Добавим имя и почту и завершим первую строку

                    var phrase = new Phrase(model.UserInfo.FIO, new Font(baseFont, 12, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                    var paragraf = new Paragraph(phrase);
                    paragraf.Alignment = Element.ALIGN_LEFT;

                    cellP1.AddElement(paragraf);

                    phrase = new Phrase(model.UserInfo.email, new Font(baseFont, 12, Font.NORMAL, new BaseColor(System.Drawing.Color.Black)));
                    paragraf = new Paragraph(phrase);
                    paragraf.Alignment = Element.ALIGN_LEFT;
                    cellP1.AddElement(paragraf);
                    cellP1.Colspan = 6;
                    table.AddCell(cellP1);

                }
                //Добавим заголовок

                PdfPCell cellP2 = new PdfPCell(new Phrase(model.KpStore.Title.Trim(), new Font(baseFont, 19, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                cellP2.Colspan = 12;
                cellP2.PaddingTop = 40f;
                cellP2.PaddingBottom = 50f;
                cellP2.Border = 0;
                cellP2.HorizontalAlignment = 1;
                table.AddCell(cellP2);

                //Добавим комментарий к коммерческому

                if (!String.IsNullOrEmpty(model.KpStore.Description))
                {
                    PdfPCell cellP22 = new PdfPCell(new Phrase(StripHTML(model.KpStore.Description.Trim()), new Font(baseFont, 14, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                    cellP22.Colspan = 12;
                    cellP22.PaddingTop = 10f;
                    cellP22.PaddingBottom = 20f;
                    cellP22.Border = 0;
                    cellP22.HorizontalAlignment = 1;
                    table.AddCell(cellP22);
                }


                //Добавим продукты
                int i = 1;
                int count = model.ProdList.Count;

                foreach (var item in model.ProdList)
                {
                    string imgPathWeb = item.Image.Remove(0, 1).Replace('/', '\\');
                    string imgPath = Server.MapPath("~") + imgPathWeb;
                    if (!String.IsNullOrEmpty(imgPath))
                    {
                        //Binary bin = file.picture;
                        //MemoryStream pic_ms = new MemoryStream(bin.ToArray());
                        //System.Drawing.Image pic_img = System.Drawing.Image.FromStream((Stream)pic_ms);

                        System.Drawing.Image pic_img = System.Drawing.Image.FromFile(imgPath);

                        //System.Drawing.Image.GetThumbnailImageAbort myCallback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);

                        int width = 140;
                        int height = (int)(width *
                             ((double)pic_img.Height / (double)pic_img.Width));

                        //System.Drawing.Image myThumbnail = pic_img.F //pic_img.GetThumbnailImage(width, height, myCallback, IntPtr.Zero);
                        img = iTextSharp.text.Image.GetInstance(imgPath);
                        img.ScaleToFit(width, height);
                        //Give space before image
                        img.SpacingBefore = 10f;
                        //Give some space after the image
                        img.SpacingAfter = 1f;
                        img.Alignment = Element.ALIGN_CENTER;
                    }
                    PdfPCell cellP3 = new PdfPCell(new Phrase(new Chunk(img, 10f, 10f)));
                    cellP3.Colspan = 4;
                    cellP3.Border = 0;
                    table.AddCell(cellP3);

                    //формируемвнутреннюю таблицу
                    PdfPTable nested = new PdfPTable(3);

                    //Добавим заголовок продукта

                    PdfPCell nestedCell1 = new PdfPCell(new Phrase(item.Name.Trim(), new Font(baseFont, 14, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                    nestedCell1.Colspan = 3;
                    //nestedCell1.Padding = 8f;
                    nestedCell1.Border = 0;
                    nestedCell1.HorizontalAlignment = 0;
                    nested.AddCell(nestedCell1);

                    if (model.KpStore.SumTirageP)
                    {
                        PdfPCell priceChunk = new PdfPCell(new Phrase("Цена (руб)", new Font(baseFont, 11, Font.NORMAL, new BaseColor(System.Drawing.Color.Gray))));
                        PdfPCell tiragChunk = new PdfPCell(new Phrase("Тираж (шт)", new Font(baseFont, 11, Font.NORMAL, new BaseColor(System.Drawing.Color.Gray))));
                        PdfPCell emptyChunk = new PdfPCell(new Phrase("", new Font(baseFont, 11, Font.NORMAL, new BaseColor(System.Drawing.Color.Gray))));
                        priceChunk.PaddingTop = 10f;
                        tiragChunk.PaddingTop = 10f;
                        priceChunk.Border = 0;
                        tiragChunk.Border = 0;
                        emptyChunk.Border = 0;
                        nested.AddCell(priceChunk);
                        nested.AddCell(tiragChunk);
                        nested.AddCell(emptyChunk);

                        PdfPCell priceInfoCell = new PdfPCell(new Phrase(String.Format("{0:C2}", item.Price), new Font(baseFont, 11, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                        priceInfoCell.Border = 0;
                        nested.AddCell(priceInfoCell);
                        priceInfoCell = new PdfPCell(new Phrase(item.Cnt.ToString(), new Font(baseFont, 11, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                        priceInfoCell.Border = 0;
                        nested.AddCell(priceInfoCell);
                        priceInfoCell = new PdfPCell(new Phrase(String.Format("= {0:C2}", item.ItemSumm), new Font(baseFont, 11, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                        priceInfoCell.Border = 0;
                        nested.AddCell(priceInfoCell);
                    }
                    else
                    {
                        PdfPCell priceChunk = new PdfPCell(new Phrase("Цена (руб)", new Font(baseFont, 11, Font.NORMAL, new BaseColor(System.Drawing.Color.Gray))));
                        PdfPCell emptyChunk = new PdfPCell(new Phrase("", new Font(baseFont, 11, Font.NORMAL, new BaseColor(System.Drawing.Color.Gray))));

                        priceChunk.PaddingTop = 10f;
                        priceChunk.Border = 0;

                        emptyChunk.Border = 0;
                        emptyChunk.Colspan = 2;
                        nested.AddCell(priceChunk);
                        nested.AddCell(emptyChunk);

                        PdfPCell priceInfoCell = new PdfPCell(new Phrase(String.Format("{0:C2}", item.Price), new Font(baseFont, 11, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                        priceInfoCell.Border = 0;
                        nested.AddCell(priceInfoCell);

                        nested.AddCell(emptyChunk);
                    }



                    //item.Article
                    if (!String.IsNullOrEmpty(item.Article) && model.KpStore.ArticleP)
                    {
                        PdfPCell artiklCell = new PdfPCell(new Phrase(String.Format("АРТИКУЛ: {0}", item.Article.Trim()), new Font(baseFont, 11, Font.NORMAL, new BaseColor(System.Drawing.Color.Gray))));
                        artiklCell.Colspan = 3;
                        artiklCell.PaddingTop = 15f;
                        artiklCell.Border = 0;
                        nested.AddCell(artiklCell);
                    }

                    if (model.KpStore.CharacterP)
                    {


                        // item.Size
                        if (!String.IsNullOrEmpty(item.Size))
                        {
                            PdfPCell sizeCell = new PdfPCell(new Phrase(String.Format("РАЗМЕРЫ: {0}", item.Size.Trim()), new Font(baseFont, 11, Font.NORMAL, new BaseColor(System.Drawing.Color.Gray))));
                            sizeCell.Colspan = 3;
                            sizeCell.PaddingTop = 2f;
                            sizeCell.Border = 0;
                            nested.AddCell(sizeCell);
                        }

                        //item.Material
                        //if (!String.IsNullOrEmpty(item.Material))
                        if (item.Material != null && item.Material.Count > 0)
                        {
                            PdfPCell matCell = new PdfPCell(new Phrase(String.Format("МАТЕРИАЛ: {0}", item.Material.FirstOrDefault().Name.Trim()), new Font(baseFont, 11, Font.NORMAL, new BaseColor(System.Drawing.Color.Gray))));
                            matCell.Colspan = 3;
                            matCell.PaddingTop = 2f;
                            matCell.Border = 0;
                            nested.AddCell(matCell);
                        }


                        //item.Weight
                        if (!String.IsNullOrEmpty(item.Weight))
                        {
                            PdfPCell wgtCell = new PdfPCell(new Phrase(String.Format("ВЕС: {0}", item.Weight.Trim()), new Font(baseFont, 11, Font.NORMAL, new BaseColor(System.Drawing.Color.Gray))));
                            wgtCell.Colspan = 3;
                            wgtCell.PaddingTop = 2f;
                            wgtCell.Border = 0;
                            nested.AddCell(wgtCell);
                        }


                        //item.Nanesenie
                        //if (!String.IsNullOrEmpty(item.Nanesenie))
                        if (item.Nanesenie != null && item.Material.Count > 0)
                        {
                            PdfPCell nanCell = new PdfPCell(new Phrase(String.Format("НАНЕСЕНИЕ: {0}", item.Nanesenie.FirstOrDefault().Name.Trim()), new Font(baseFont, 11, Font.NORMAL, new BaseColor(System.Drawing.Color.Gray))));
                            nanCell.Colspan = 3;
                            nanCell.PaddingTop = 2f;
                            nanCell.PaddingBottom = 7f;
                            nanCell.Border = 0;
                            nested.AddCell(nanCell);
                        }

                    }
                    //item.Description
                    if (!String.IsNullOrEmpty(item.Description) && model.KpStore.DescrP)
                    {
                        CustomDashedLineSeparator separator = new CustomDashedLineSeparator();
                        separator.SetDash(10);
                        separator.Gap = 7;
                        separator.LineWidth = 1;

                        PdfPCell separCell = new PdfPCell(new Phrase(new Chunk(separator)));
                        separCell.Colspan = 3;
                        separCell.Border = 0;
                        nested.AddCell(separCell);

                        PdfPCell descCell = new PdfPCell(new Phrase(StripHTML(item.Description.Trim()), new Font(baseFont, 9, Font.NORMAL, new BaseColor(System.Drawing.Color.Gray))));
                        descCell.Colspan = 3;
                        descCell.PaddingTop = 7f;
                        descCell.Border = 0;

                        nested.AddCell(descCell);
                    }

                    //nested.AddCell("Nested Col 1");
                    //nested.AddCell("Nested Col 2");
                    //nested.AddCell("Nested Col 3");

                    //Контейнер для внутренней таблицы
                    cellP3 = new PdfPCell(nested);
                    cellP3.Padding = 15f;
                    cellP3.Colspan = 8;
                    cellP3.Border = 0;
                    table.AddCell(cellP3);

                    if (count - i != 0)
                    {
                        table.AddCell(hrCell);
                    }
                    i++;
                }

                //Добавим ИТОГО
                if (model.KpStore.TotalP)
                {
                    //сумма за тираж
                    PdfPCell cellEmpt1 = new PdfPCell();
                    cellEmpt1.Colspan = 5;
                    cellEmpt1.Border = 0;
                    table.AddCell(cellEmpt1);

                    PdfPCell cellItg1 = new PdfPCell(new Phrase("Сумма за тираж  ", new Font(baseFont, 12, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                    cellItg1.Colspan = 4;
                    cellItg1.Border = 0;
                    table.AddCell(cellItg1);

                    PdfPCell cellItg2 = new PdfPCell(new Phrase(model.Cost.ToString("0.##"), new Font(baseFont, 12, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                    cellItg2.Colspan = 3;
                    cellItg2.Border = 0;
                    table.AddCell(cellItg2);

                    //сумма скидки
                    if (model.CostSale > 0)
                    {
                        PdfPCell cellEmpt3 = new PdfPCell();
                        cellEmpt3.Colspan = 5;
                        cellEmpt3.Border = 0;
                        table.AddCell(cellEmpt3);

                        PdfPCell cellSale1 = new PdfPCell(new Phrase("Сумма скидки ", new Font(baseFont, 12, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                        cellSale1.Colspan = 4;
                        cellSale1.Border = 0;
                        table.AddCell(cellSale1);

                        PdfPCell cellSale2 = new PdfPCell(new Phrase(model.CostSale.ToString("0.##"), new Font(baseFont, 12, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                        cellSale2.Colspan = 3;
                        cellSale2.Border = 0;
                        table.AddCell(cellSale2);
                    }



                    //сумма наценки
                    if (model.CostMark > 0)
                    {
                        PdfPCell cellEmpt4 = new PdfPCell();
                        cellEmpt4.Colspan = 5;
                        cellEmpt4.Border = 0;
                        table.AddCell(cellEmpt4);

                        PdfPCell cellMark1 = new PdfPCell(new Phrase("Сумма наценки ", new Font(baseFont, 12, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                        cellMark1.Colspan = 4;
                        cellMark1.Border = 0;
                        table.AddCell(cellMark1);

                        PdfPCell cellMark2 = new PdfPCell(new Phrase(model.CostMark.ToString("0.##"), new Font(baseFont, 12, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                        cellMark2.Colspan = 3;
                        cellMark2.Border = 0;
                        table.AddCell(cellMark2);
                    }


                    //сумма за допуслуги
                    if (model.KpStoreDopList != null && model.KpStoreDopList.Count > 0)
                    {
                        foreach (var item in model.KpStoreDopList)
                        {
                            PdfPCell cellEmpt2 = new PdfPCell();
                            cellEmpt2.Colspan = 5;
                            cellEmpt2.Border = 0;
                            table.AddCell(cellEmpt2);

                            PdfPCell cellSrv2 = new PdfPCell(new Phrase(item.Service, new Font(baseFont, 12, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                            cellSrv2.Colspan = 4;
                            cellSrv2.Border = 0;
                            table.AddCell(cellSrv2);

                            PdfPCell cellSrv3 = new PdfPCell(new Phrase(item.Price.ToString("0.00"), new Font(baseFont, 12, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                            cellSrv3.Colspan = 3;
                            cellSrv3.Border = 0;
                            table.AddCell(cellSrv3);
                        }

                    }


                    //hr последний
                    table.AddCell(hrCell);

                    //Итого
                    PdfPCell cellItog = new PdfPCell(new Phrase("Итого  " + model.TotalSumm.ToString("0.00"), new Font(baseFont, 14, Font.NORMAL, new BaseColor(System.Drawing.Color.Black))));
                    cellItog.Colspan = 12;
                    cellItog.PaddingTop = 10f;
                    cellItog.PaddingBottom = 20f;
                    cellItog.Border = 0;
                    cellItog.HorizontalAlignment = 2;
                    table.AddCell(cellItog);
                }


                doc.Add(table);


                doc.Close();

                return ms;
              
            }

        }

        private static BaseFont RegisterFonts()
        {
            string[] fontNames = { "Arial.ttf" };
            string fontFile = fontFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf");

            FontFactory.Register(fontFile);
            return BaseFont.CreateFont(fontFile, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        }
        private bool ThumbnailCallback()
        {
            return false;
        }

        private EditKPViewModel GetEditKpModel(int kpId)
        {
            EditKPViewModel model = new EditKPViewModel();
            model.KpStore = Db.KP_Store.FirstOrDefault(x => x.Id == kpId);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-Ru");
            model.DateKp = model.KpStore.DateDoc.ToLongDateString();

            //model.ProdList = Db.Database.
            //    SqlQuery<OrderProduct>(String.Format("[dbo].[GetOrderProductsForKp]  @OrderId = {0},@par='getOrdL'", model.KpStore.OrderId)).ToList();

            var res = string.Concat(Db.Database.
                SqlQuery<string>("[dbo].[GetProdListForKp]  @OrderId = {0}", model.KpStore.OrderId).ToList());


            model.ProdList = JsonConvert.DeserializeObject<List<OrderProduct>>(res); //.Replace(" .\n ", "")


            model.UserInfo = Db.Database.SqlQuery<UserProfileDetails>("select * from UserProfileDetails where UserID={0}", model.KpStore.UserId).SingleOrDefault(); //Db.UserProfileDetails.SingleOrDefault(x => x.UserID == model.KpStore.UserId);

            if (!String.IsNullOrEmpty(model.UserInfo.LogoImg))
            {
                model.ImageLogo = model.UserInfo.LogoImg;
            }
            else
            {
                model.ImageLogo = "/images/logo2.png";
            }
            

            //model.ImagesList = Db.Database.
            //    SqlQuery<ImageListItem>(String.Format("[dbo].[GetOrderProductsForKp]  @OrderId = {0},@par='getImgL'", model.KpStore.OrderId)).ToList();

            model.KpStoreDopList = Db.Database.
                SqlQuery<Kp_Store_dop>("select Id,KpId,Service,Price from Kp_Store_dop where KpId={0}", kpId).ToList();

            decimal summDopCost = model.KpStoreDopList.Sum(x=>x.Price);

            model.Cost = Db.OrderDetails.Where(x => x.OrderID == model.KpStore.OrderId).Sum(x => x.Count*x.ItemPrice);

            model.TotalSumm = model.Cost+ summDopCost;

            decimal markUp = GetPricemarkUp(kpId);
            if (markUp>1)
            {
                model.CostMark = model.Cost - model.Cost / markUp;
                //model.TotalSumm = model.TotalSumm + model.CostMark;
            }

            if (model.KpStore.Sale.HasValue)
            {
                model.CostSale = model.Cost * model.KpStore.Sale.Value / 100;
                model.TotalSumm = model.TotalSumm - model.CostSale;
            }

            model.Cost = model.Cost / model.KpStore.СurrencyValue;
            model.TotalSumm = model.TotalSumm / model.KpStore.СurrencyValue;
            model.CostMark= model.CostMark / model.KpStore.СurrencyValue;
            model.CostSale = model.CostSale / model.KpStore.СurrencyValue;

            return model;
        }

        private void UpdateImgFromList(KP_Store kpItem, string ImgBtn, int ImgProdId, long IdImg)
        {
            
            //перелистывание картинк
            if (IdImg > 0 && ImgProdId > 0)
            {
                if (!String.IsNullOrEmpty(ImgBtn))
                {
                    string img = Db.ProdImages.SingleOrDefault(x => x.Id == IdImg).Small;
                    List<DopImgList> imgList = Db.Database.SqlQuery<DopImgList>(@"select  

                                cast(g.Id as bigint) as ImgId,
                                  g.Small as [Image],
                                  g.ProdId,
                                 Selected =cast(case when ltrim(rtrim(od.Img))=ltrim(rtrim(g.Small)) then 1 else 0 end as bit),
                                 cast(ROW_NUMBER()OVER(Partition By g.ProdId Order By g.Id, g.Main DESC) as int)As NumberImg

                                 from ProdImages g

                                 inner
                                 join OrderDetails od on od.ProductId = g.ProdId

                                 where g.ProdId = {0} and od.OrderID = {1}", ImgProdId, kpItem.OrderId).ToList();

                    if (ImgBtn == "next")
                    {
                        int tekNbr = imgList.FirstOrDefault(x => x.ImgId == IdImg).NumberImg;
                        int nextNbr = tekNbr + 1;

                        if (imgList.FirstOrDefault(x => x.NumberImg == nextNbr) != null)
                        {
                            img = imgList.FirstOrDefault(x => x.NumberImg == nextNbr).Image;
                        }


                    }
                    else
                    {
                        int tekNbr = imgList.FirstOrDefault(x => x.ImgId == IdImg).NumberImg;
                        int prevNbr = tekNbr - 1;

                        if (imgList.FirstOrDefault(x => x.NumberImg == prevNbr) != null)
                        {
                            img = imgList.FirstOrDefault(x => x.NumberImg == prevNbr).Image;
                        }
                    }

                    //OrderDetails ordDet = Db.OrderDetails.FirstOrDefault(x => x.ProductId == model.ImgProdId);
                    //ordDet.Img = img;
                    //Db.SaveChanges();

                    Db.Database.ExecuteSqlCommand(@" Update OrderDetails
		                                             set Img={0}
		                                              where ProductId={1} and OrderID={2}", img, ImgProdId, kpItem.OrderId);

                }


            }
        }

        private UserInf GetUser()
        {
            UserInf result = new UserInf();

            var user = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName));
            int userId = user.UserId;
            string userName = user.UserProfileDetails.FirstOrDefault(x => x.UserID == userId).FIO;

            result.UserId = userId;

            if (String.IsNullOrEmpty(userName))
            {
                result.UserName = user.UserName;
            }
            else
            {
                result.UserName = userName;
            }

            return result;
        }

        private class UserInf
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
        }

        private class CustomDashedLineSeparator : DottedLineSeparator
        {
            protected float dash = 5;
            protected float phase = 2.5f;

            public float GetDash()
            {
                return dash;
            }

            public float GetPhase()
            {
                return phase;
            }

            public void SetDash(float dash)
            {
                this.dash = dash;
            }

            public void SetPhase(float phase)
            {
                this.phase = phase;
            }

            public void Draw(PdfContentByte canvas, float llx, float lly, float urx, float ury, float y)
            {
                canvas.SaveState();
                canvas.SetLineWidth(lineWidth);
                canvas.SetLineDash(dash, gap, phase);
                DrawLine(canvas, llx, urx, y);
                canvas.RestoreState();
            }
        }

        private decimal GetPricemarkUp(int kpId)
        {
            decimal priceKoef = 1;
            KP_Store kpstore = Db.KP_Store.FirstOrDefault(x=>x.Id==kpId);
            if (kpstore.MarkUp.HasValue)
            {
              
                priceKoef = 1 + (decimal)kpstore.MarkUp.Value /(decimal)100;

            }

            return priceKoef;
        }

        private string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        
        

    }
}



