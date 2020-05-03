using EditPressRu.Helpers;
using System.Text;
using EditPressRu.Models;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace EditPressRu.Controllers
{
    public class AjaxController : BaseController
    {
        EditPressRuEntities Db = new EditPressRuEntities();
        // GET: Ajax
        [HttpPost]
        public ActionResult AutoCoplete(String term)
        {
            AutoComplSearchViewModel model = new AutoComplSearchViewModel();
            try
            {
                model = db.GetAutoComplModel(term);
            }
            catch (Exception)
            {
            }

            return PartialView("_AutocomplitSearchPartial", model);
        }

        [HttpPost]
        public ActionResult AutoComplCity(String term)
        {
            term = term.Replace(@"\r\n", "").Trim();
            List<SelectListItem> model = new List<SelectListItem>();
            try
            {
                model = Db.Cityes.Where(x => x.Name.Contains(term)).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).Take(15).Distinct().ToList();
            }
            catch (Exception)
            {
            }

            return PartialView("_AutocomplitCityPartial", model);
        }

        public ActionResult BackCall(string name, string email, string sub, string msgBody)
        {
            try
            {
                string subject = String.IsNullOrEmpty(sub) ? "Без темы" : sub;
                string body = String.Format(@"<p>Менеджер магазина Editpress. Клиент {0} оставил заявку с сайта.  
                                                Телефон или почта {1}</p>.<p><strong>Его сообщение: </strong>{2}</p>", name, email, msgBody);
                HelpFunctions.SendMail2("opt@editpress.ru", subject, body);
                HelpFunctions.SendMail2("sales@editpress.ru", subject, body);
                HelpFunctions.SendMail2("zakaz@editpress.ru", subject, body);
                HelpFunctions.SendMail2("antipova@editpress.ru", subject, body);
                HelpFunctions.SendMail2("olga@editpress.ru", subject, body);
                HelpFunctions.SendMail2("editpress@mail.ru", subject, body);
                HelpFunctions.SendMail2("chernikoff80@yandex.ru", subject, body);

                return Content("Спасибо за проявленный интерес к нашим услугам! Мы обязательно с Вами свяжемся.");
            }
            catch (System.Exception)
            {
                return Content("Что-то пошло нетак. Письмо не отправлено.");
            }

        }

        [HttpPost]
        public JsonResult MultiUpload()
        {
            int userId = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;
            string userNameFolder = "LogoFolder" + userId.ToString();
            string __filepath = Server.MapPath("~/Images/UsersUploads/" + userNameFolder);
            int __maxSize = 10 * 1024 * 1024;    // максимальный размер файла 2 Мб
            // допустимые MIME-типы для файлов
            List<string> mimes = new List<string>
            {
                "image/jpeg", "image/jpg", "image/png"
            };

            var result = new Result
            {
                MultiFiles = new List<string>()
            };

            if (Request.Files.Count > 0)
            {
                foreach (string f in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[f];

                    // Выполнить проверки на допустимый размер файла и формат
                    if (file.ContentLength > __maxSize)
                    {
                        result.Error = "Размер файла не должен превышать 10 Мб";
                        break;
                    }
                    else if (mimes.FirstOrDefault(m => m == file.ContentType) == null)
                    {
                        result.Error = "Недопустимый формат файла";
                        break;
                    }

                    if (!Directory.Exists(__filepath))
                    {
                        Directory.CreateDirectory(__filepath);
                    }
                    // Сохранить файл и вернуть URL



                    file.SaveAs($@"{__filepath}\{file.FileName}");
                    result.MultiFiles.Add("/Images/UsersUploads/" + userNameFolder + "/" + file.FileName);

                }
            }

            return Json(result);
        }

        [HttpPost]
        public ActionResult UploadLogo()
        {

            int userId = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;
            UserProfileDetails userPrDet = Db.UserProfileDetails.FirstOrDefault(x => x.UserID == userId);

            string userNameFolder = "LogoFolder" + userId.ToString();
            string __filepath = Server.MapPath("~/Images/UsersUploads/" + userNameFolder);
            int __maxSize = 10 * 1024 * 1024;    // максимальный размер файла 2 Мб
            // допустимые MIME-типы для файлов
            List<string> mimes = new List<string>
            {
                "image/jpeg", "image/jpg", "image/png"
            };

            var result = new Result();


            if (Request.Files.Count > 0)
            {
                //foreach (string f in Request.Files)
                //{


                HttpPostedFileBase file = Request.Files[0];

                // Выполнить проверки на допустимый размер файла и формат
                if (file.ContentLength > __maxSize)
                {
                    result.Error = "Размер файла не должен превышать 10 Мб";
                    //return PartialView("_UplImgErrPartial", result.Error);
                }
                else if (mimes.FirstOrDefault(m => m == file.ContentType) == null)
                {
                    result.Error = "Недопустимый формат файла";
                    //return PartialView("_UplImgErrPartial", result.Error);
                }

                if (!Directory.Exists(__filepath))
                {
                    Directory.CreateDirectory(__filepath);
                }
                // Сохранить файл и вернуть URL


                string webPathImg = "/Images/UsersUploads/" + userNameFolder + "/" + file.FileName;
                file.SaveAs($@"{__filepath}\{file.FileName}");

                userPrDet.LogoImg = webPathImg;
                Db.SaveChanges();

                result.File = webPathImg;

            }

            return PartialView("_UplImgPartial", result.File);
        }

        [HttpPost]
        public ActionResult UploadLogoKp()
        {

            int userId = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;
            UserProfileDetails userPrDet = Db.UserProfileDetails.FirstOrDefault(x => x.UserID == userId);

            string userNameFolder = "LogoFolder" + userId.ToString();
            string __filepath = Server.MapPath("~/Images/UsersUploads/" + userNameFolder);
            int __maxSize = 10 * 1024 * 1024;    // максимальный размер файла 2 Мб
            // допустимые MIME-типы для файлов
            List<string> mimes = new List<string>
            {
                "image/jpeg", "image/jpg", "image/png"
            };

            var result = new Result();


            if (Request.Files.Count > 0)
            {
                //foreach (string f in Request.Files)
                //{


                HttpPostedFileBase file = Request.Files[0];

                // Выполнить проверки на допустимый размер файла и формат
                if (file.ContentLength > __maxSize)
                {
                    result.Error = "Размер файла не должен превышать 10 Мб";
                    //return PartialView("_UplImgErrPartial", result.Error);
                }
                else if (mimes.FirstOrDefault(m => m == file.ContentType) == null)
                {
                    result.Error = "Недопустимый формат файла";
                    //return PartialView("_UplImgErrPartial", result.Error);
                }

                if (!Directory.Exists(__filepath))
                {
                    Directory.CreateDirectory(__filepath);
                }
                // Сохранить файл и вернуть URL


                string webPathImg = "/Images/UsersUploads/" + userNameFolder + "/" + file.FileName;


                var scaleImg = System.Drawing.Image.FromStream(file.InputStream);
                scaleImg = HelpFunctions.ScaleImage(scaleImg, 380, 160);
                scaleImg.Save(Server.MapPath(webPathImg));

               // file.SaveAs($@"{__filepath}\{file.FileName}");

                

                userPrDet.LogoImg = webPathImg;
                Db.SaveChanges();

                result.File = webPathImg;

            }

            return PartialView("_UplImgKpPartial", result.File);
        }

        [HttpPost]
        public ActionResult UploadBgKp()
        {
            int userId = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;

            int kpId = int.Parse(Request.Form["KpId"]);

            KP_Store kpStore = Db.KP_Store.SingleOrDefault(x=>x.Id==kpId);

            string userNameFolder = "BgFolder" + userId.ToString();
            string __filepath = Server.MapPath("~/Images/UsersUploads/" + userNameFolder);
            int __maxSize = 10 * 1024 * 1024;    // максимальный размер файла 2 Мб
            // допустимые MIME-типы для файлов
            List<string> mimes = new List<string>
            {
                "image/jpeg", "image/jpg", "image/png"
            };

            var result = new Result();

            result.Error = "";

            if (Request.Files.Count > 0)
            {
                //foreach (string f in Request.Files)
                //{


                HttpPostedFileBase file = Request.Files[0];

                // Выполнить проверки на допустимый размер файла и формат
                if (file.ContentLength > __maxSize)
                {
                    result.Error = "Размер файла не должен превышать 10 Мб";
                    //return PartialView("_UplImgErrPartial", result.Error);
                }
                else if (mimes.FirstOrDefault(m => m == file.ContentType) == null)
                {
                    result.Error = "Недопустимый формат файла";
                    //return PartialView("_UplImgErrPartial", result.Error);
                }

                if (!Directory.Exists(__filepath))
                {
                    Directory.CreateDirectory(__filepath);
                }
                // Сохранить файл и вернуть URL


                string webPathImg = "/Images/UsersUploads/" + userNameFolder + "/" + file.FileName;


                var scaleImg = System.Drawing.Image.FromStream(file.InputStream);
                //scaleImg = HelpFunctions.ScaleImage(scaleImg, 380, 160);
                scaleImg.Save(Server.MapPath(webPathImg));

                // file.SaveAs($@"{__filepath}\{file.FileName}");

                kpStore.KpForeColor = webPathImg;
                Db.SaveChanges();

                result.File = webPathImg;

            }

            return Json(new { srcBg = result.File, err = result.Error});
           
        }


        //[HttpPost]
        //public ActionResult UploadLogo(HttpPostedFileBase[] files, string orderId = "")
        //{
        //    int userId = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;
        //    string userNameFolder = "LogoFolder" + userId.ToString() + "_" + orderId.ToString() + '\\';
        //    string fileName1 = "";
        //    string fileName2 = "";

        //    //Ensure model state is valid  
        //    if (ModelState.IsValid)
        //    {   //iterating through multiple file collection   
        //        foreach (HttpPostedFileBase file in files)
        //        {
        //            //Checking file is available to save.  
        //            if (file != null)
        //            {
        //                var InputFileName = Path.GetFileName(file.FileName);
        //                var ServerSavePath = Path.Combine(Server.MapPath("~/Images/UsersUploads/") + userNameFolder + InputFileName);
        //                System.IO.Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Images/UsersUploads/") + userNameFolder));
        //                fileName1 = userNameFolder + InputFileName;
        //                string extpath2 = InputFileName.Split('.').Last();
        //                fileName2 = userNameFolder + InputFileName.Replace("." + extpath2, "small." + extpath2);
        //                var ServerSavePath2 = Path.Combine(Server.MapPath("~/Images/UsersUploads/") + fileName2);
        //                //Save file to server folder  
        //                file.SaveAs(ServerSavePath);
        //                HelpFunctions.ImageResize(ServerSavePath, ServerSavePath2);
        //                //assigning file uploaded status to ViewBag for showing message to user.  
        //                ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";
        //            }

        //        }
        //    }
        //    string url = String.Format("/KpStore/EditKP?id={0}&logoPth=/Images/UsersUploads/{1}&updLogo={2}", orderId.ToString(), fileName2.Replace('\\', '/'), true);
        //    return Redirect(url);

        //}

        [HttpGet]
        public PartialViewResult GetCaseContent(int page = 2)
        {
            // get the products for this category
            int pageSize = 30;
            List<string> model = new List<string>();

            model = db.GetImgFiles().Skip((page - 1) * pageSize)
                .Take(pageSize).ToList();

            return PartialView("_LazyContentPartial", model);
        }

        [HttpGet]
        public ActionResult GetBigSrc(string src)
        {
            var img = Db.ProdImages.FirstOrDefault(x => x.ThumbNail == src);

            string srcBig = img.Big;
            int prodId = img.ProdId;

            var prod = Db.Products.SingleOrDefault(x => x.Id == prodId);

            decimal price = prod.Price;
            string name = prod.ShName;

            //return Content(srcBig);
            return Json(new { srcBig = srcBig, prodId = prodId, price = price, name = name }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Sitemap1()
        {
            Generator sitemapGenerator = new Generator();
            var sitemapNodes = sitemapGenerator.GetSitemapNodes1(this.Url);

            string xml = sitemapGenerator.GetSitemapDocument(sitemapNodes);

            string xmlPath = Path.Combine(Server.MapPath("~/") + "sitemap1.xml");
            System.IO.File.WriteAllText(xmlPath, xml);

            return this.Content(xml, "text/xml", Encoding.UTF8);
        }
        public ActionResult Sitemap2()
        {
            Generator sitemapGenerator = new Generator();
            var sitemapNodes = sitemapGenerator.GetSitemapNodes2(this.Url);

            string xml = sitemapGenerator.GetSitemapDocument(sitemapNodes);

            string xmlPath = Path.Combine(Server.MapPath("~/") + "sitemap2.xml");
            System.IO.File.WriteAllText(xmlPath, xml);

            return this.Content(xml, "text/xml", Encoding.UTF8);
        }
        public ActionResult Sitemap3()
        {
            Generator sitemapGenerator = new Generator();
            var sitemapNodes = sitemapGenerator.GetSitemapNodes3(this.Url);

            string xml = sitemapGenerator.GetSitemapDocument(sitemapNodes);

            string xmlPath = Path.Combine(Server.MapPath("~/") + "sitemap3.xml");
            System.IO.File.WriteAllText(xmlPath, xml);

            return this.Content(xml, "text/xml", Encoding.UTF8);
        }
        public ActionResult Sitemap4()
        {
            Generator sitemapGenerator = new Generator();
            var sitemapNodes = sitemapGenerator.GetSitemapNodes4(this.Url);

            string xml = sitemapGenerator.GetSitemapDocument(sitemapNodes);

            string xmlPath = Path.Combine(Server.MapPath("~/") + "sitemap4.xml");
            System.IO.File.WriteAllText(xmlPath, xml);

            return this.Content(xml, "text/xml", Encoding.UTF8);
        }
        public ActionResult Sitemap5()
        {
            Generator sitemapGenerator = new Generator();
            var sitemapNodes = sitemapGenerator.GetSitemapNodes5(this.Url);

            string xml = sitemapGenerator.GetSitemapDocument(sitemapNodes);

            string xmlPath = Path.Combine(Server.MapPath("~/") + "sitemap5.xml");
            System.IO.File.WriteAllText(xmlPath, xml);

            return this.Content(xml, "text/xml", Encoding.UTF8);
        }
        public ActionResult Sitemap6()
        {
            Generator sitemapGenerator = new Generator();
            var sitemapNodes = sitemapGenerator.GetSitemapNodes6(this.Url);

            string xml = sitemapGenerator.GetSitemapDocument(sitemapNodes);

            string xmlPath = Path.Combine(Server.MapPath("~/") + "sitemap6.xml");
            System.IO.File.WriteAllText(xmlPath, xml);

            return this.Content(xml, "text/xml", Encoding.UTF8);
        }
        public ActionResult Sitemap7()
        {
            Generator sitemapGenerator = new Generator();
            var sitemapNodes = sitemapGenerator.GetSitemapNodes7(this.Url);

            string xml = sitemapGenerator.GetSitemapDocument(sitemapNodes);

            string xmlPath = Path.Combine(Server.MapPath("~/") + "sitemap7.xml");
            System.IO.File.WriteAllText(xmlPath, xml);

            return this.Content(xml, "text/xml", Encoding.UTF8);
        }
        public ActionResult Sitemap8()
        {
            Generator sitemapGenerator = new Generator();
            var sitemapNodes = sitemapGenerator.GetSitemapNodes8(this.Url);

            string xml = sitemapGenerator.GetSitemapDocument(sitemapNodes);

            string xmlPath = Path.Combine(Server.MapPath("~/") + "sitemap8.xml");
            System.IO.File.WriteAllText(xmlPath, xml);

            return this.Content(xml, "text/xml", Encoding.UTF8);
        }



        public class Result
        {
            public string Error { get; set; }
            public string File { get; set; }
            public List<string> MultiFiles { get; set; }
        }
    }
}