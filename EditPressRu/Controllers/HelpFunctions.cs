using EditPressRu.Models;
using EditPressRu.Models.DB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebMatrix.WebData;

namespace EditPressRu.Controllers
{
    public class HelpFunctions
    {

        private const string HEADER = "<html><head><title>{0}</title></head><body>";
        private const string FOOTER = "</body></html>";

        public static void SendMail2(string adress, string subject, string body, string fileName = null)
        {
            SmtpClient client = new SmtpClient("smtp.mail.ru", 587);
            client.Credentials = new NetworkCredential("admin@editpress.ru", "#9mT2qYazcCP");
            MailMessage message = new MailMessage();

            message.To.Add(adress);
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.GetEncoding("windows-1251");
            message.IsBodyHtml = true;
            message.From = new MailAddress("admin@editpress.ru");

            message.Subject = subject;
            message.Body = HEADER + body + FOOTER;
            client.EnableSsl = true;

            if (!String.IsNullOrEmpty(fileName))
            {
                Attachment sendFile = new Attachment(fileName);
                message.Attachments.Add(sendFile);
            }


            try
            {
                client.Send(message);
            }
            catch
            {

            }


        }




        //public void CopyFiles(string srcpath, List<string> sourcefiles, string destpath)
        //{

        //    DirectoryInfo dsourceinfo = new DirectoryInfo(srcpath);
        //    if (!Directory.Exists(destpath))
        //    {
        //        Directory.CreateDirectory(destpath);
        //    }
        //    DirectoryInfo dtargetinfo = new DirectoryInfo(destpath);
        //    List<FileInfo> fileinfo = sourcefiles.Select(s => new FileInfo(s)).ToList();
        //    foreach (var item in fileinfo)
        //    {

        //        string destNewPath = CreateDirectoryStructure(item.Directory.FullName, destpath) + "\\" + item.Name;
        //        System.IO.File.Copy(item.FullName, destNewPath);
        //    }

        //}


        //public string CreateDirectoryStructure(string newPath, string destPath)
        //{
        //    Queue<string> queue = new Queue<string>(newPath.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries));
        //    queue.Dequeue();
        //    while (queue.Count > 0)
        //    {
        //        string dirName = queue.Dequeue();
        //        if (!new DirectoryInfo(destPath).GetDirectories().Any(a => a.Name == dirName))
        //        {
        //            Directory.CreateDirectory(destPath + "\\" + dirName);
        //            destPath += "\\" + dirName;
        //        }
        //    }
        //    return destPath;

        //}

        public static sbyte SaveOrder(int userId, int orderId, string password = "", int statusId = 0)
        {
            EditPressRuEntities Db = new EditPressRuEntities();
            UserProfileDetails rec = Db.UserProfileDetails.SingleOrDefault(x => x.UserID == userId);


            Orders order = Db.Orders.SingleOrDefault(x => x.OrderID == orderId);
            order.UserID = userId;
            //cохраним изменения  пользователя - и если сумма прокатит потом и статус сохраним или не сохраним-если сумма не прокатит
            Db.SaveChanges();

            switch (statusId)
            {
                case 5:
                    order.StatusId = 2;
                    break;
                case 6:
                    order.StatusId = 3;
                    break;
                case 7:
                    decimal summ = order.OrderDetails.Sum(x => x.ItemPrice * x.Count);
                    if (summ < 15000)
                    {
                        return -1;
                    }
                    else
                    {
                        order.StatusId = 1;
                    }
                    break;

                    //default:
                    //    order.StatusId = 1;
                    //    break;
            }

            Db.SaveChanges();


            string uri = "editpress.ru";
            //string uri = HttpContext.Request.Url.Authority.ToString();

            try
            {
                string bodyClient = String.Format("<p>Уважаемый(ая), {1}! Вами сформирован заказ №{0}. Подробнее Вы можете посмотреть по <a href='https://{2}/orders/orderdetails/{0}'>ссылке</a></p><p>Телефон, который Вы оставили:{3}, его можно поменять в <a href='https://{2}/Account'>личном кабинете</a></p>", orderId.ToString(), rec.FIO, uri, rec.Phone);
                string bodyMagaz = String.Format("<p>Менеджер магазина Editpress. Клиент {1} сформировал <a href='https://{4}/Account/AdmOrderDetails/{0}'>заказ №{0}</a>. Телефон {2}. Почта {3}</p>", orderId, rec.FIO, rec.Phone, rec.email, uri);

                if (!String.IsNullOrEmpty(password))
                {
                    string bodyClientRegister = String.Format("<p>Уважаемый(ая), {1}! Вы зарегистрировались в Интернет магазине оптовой торговли сувенирами <a href='https://editpress.ru'>EditPress</a>.<p>Ваш пароль <b>{4}</b>, логином является адрес ЭТОГО Вашего почтового ящика</p> <p>Телефон, который Вы оставили:{3}, его можно поменять в <a href='https://{2}/Account'>личном кабинете</a></p>", orderId, rec.FIO, uri, rec.Phone, password);
                    SendMail2(rec.email, "Регистрация. Интернет магазин Editpress.ru", bodyClientRegister);
                }

                SendMail2(rec.email, "Счет. Интернет магазин Editpress.ru", bodyClient);
                SendMail2("sales@editpress.ru", String.Format("Клиент сформировал заказ №{0}", orderId), bodyMagaz);
                SendMail2("editpress@mail.ru", String.Format("Клиент сформировал заказ №{0}", orderId), bodyMagaz);
                SendMail2("chernikoff80@yandex.ru", String.Format("Клиент сформировал заказ №{0}", orderId), bodyMagaz);
            }
            catch
            {
            }

            return 1;
        }

        public static int UpdateCartLogin(int userId, int orderId)
        {
            EditPressRuEntities Db = new EditPressRuEntities();

            return Db.Database.SqlQuery<int>("exec UpdateCartAutorization @orderId={0}, @userId={1}", orderId, userId).SingleOrDefault();


        }

        public static void ImageResizeSmall(string path1, string path2)
        {
            using (var image = Image.FromFile(path1))
            using (var newImage = ScaleImage(image, 200, 190))
            {
                newImage.Save(path2);
            }

        }

        public static void ImageResize(string path1, string path2)
        {
            using (var image = Image.FromFile(path1))
            using (var newImage = ScaleImage(image, 448, 336))
            {
                newImage.Save(path2);
            }
            File.Delete(path1);
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        public static OrderDetailsView GetOrderCartModel(int orderId, decimal price,decimal summa_free, string par = "")
        {
            EditPressRuEntities Db = new EditPressRuEntities();
            OrderDetailsView model = new OrderDetailsView();

            int UserId = 0;
            try
            {
                ///UserId = Db.UserProfile.First(c => c.UserName.Equals(WebSecurity.CurrentUserName)).UserId;
                UserId = Db.Orders.FirstOrDefault(x => x.OrderID == orderId).UserID;
            }
            catch 
            {
            }

            List<OrderDetailsKucha> listOrderKucha = new List<OrderDetailsKucha>();

            if (UserId>0)
            {
                listOrderKucha = Db.Database.SqlQuery<OrderDetailsKucha>(@"select c.OrderID as OrderId,c.StatusId,b.Article as Articul,
                                                                                        b.Id as ProductId,a.OrderDetailID as OrderDetailsId, b.ShName as ProdName,
                                                                                        a.ItemPrice,a.Img,a.[Count] as Cnt,a.[Count]*a.ItemPrice as ItemSumm,
                                                                                        c.Data as DateOrder,d.TotalSumm as TotalSummOrder,
																						c.moscow_shipment as MoskShipmnt,u.ShipmentAddress as AdrShipmnt
																						 from OrderDetails a
                                                                                        inner join Products b on a.ProductId=b.Id
                                                                                        inner join Orders c on a.OrderID=c.OrderID
																						inner join UserProfileDetails u on u.UserID=c.UserId
                                                                                        inner join (select OrderID,sum(ItemPrice*Count) as TotalSumm  from OrderDetails dd group by OrderID) d on d.OrderID=c.OrderId
                                                                                        where c.OrderID={0}", orderId).ToList();
            }

            else
            {
                listOrderKucha = Db.Database.SqlQuery<OrderDetailsKucha>(@"select c.OrderID as OrderId,c.StatusId,b.Article as Articul,
                                                                                        b.Id as ProductId,a.OrderDetailID as OrderDetailsId, b.ShName as ProdName,
                                                                                        a.ItemPrice,a.Img,a.[Count] as Cnt,a.[Count]*a.ItemPrice as ItemSumm,
                                                                                        c.Data as DateOrder,d.TotalSumm as TotalSummOrder,
																						c.moscow_shipment as MoskShipmnt
																						,'' as AdrShipmnt
																						 from OrderDetails a
                                                                                        inner join Products b on a.ProductId=b.Id
                                                                                        inner join Orders c on a.OrderID=c.OrderID
                                                                                        inner join (select OrderID,sum(ItemPrice*Count) as TotalSumm  from OrderDetails dd group by OrderID) d on d.OrderID=c.OrderId
                                                                                        where c.OrderID={0}", orderId).ToList();
            }
        
          

            model.OrderDetail = listOrderKucha;

            if (listOrderKucha.Count > 0)
            {
                //на страничку индекса заказов не надо выводить сведения о доставке
                if (par == "index")
                {
                    model.OrderDetail = listOrderKucha.Where(x => x.ProductId != 0).ToList();
                }

                //Поправим сумму доставки - если больше 30 000(summa_free) то бесплатно, если меньше то 300(price) р

                //для этого определим-есть ли доставка
                bool deliveryYes = false;

                if (listOrderKucha.FirstOrDefault(x => x.ProductId == 0) != null)
                {
                    deliveryYes = true;
                }

                if (deliveryYes)
                {
                    decimal sum = model.OrderDetail.Where(x => x.ProductId != 0).Sum(x => x.ItemSumm);

                    int ordDetailsId = listOrderKucha.FirstOrDefault(x => x.ProductId == 0).OrderDetailsId;
                    OrderDetails ordInfo = Db.OrderDetails.SingleOrDefault(x => x.OrderDetailID == ordDetailsId);

                    if (sum >= summa_free)
                    {
                        ordInfo.ItemPrice = 0;
                        Db.SaveChanges();

                    }
                    else
                    {
                        ordInfo.ItemPrice = price;
                        Db.SaveChanges();
                    }
                }


                model.ModelIsEmpty = false;

                model.TotalItems = listOrderKucha.Where(x => x.ProductId != 0).Sum(x => x.Cnt);

                int status = listOrderKucha.FirstOrDefault().StatusId;

                model.PriznakZakaz = Db.OrdersStatus_spr.SingleOrDefault(x => x.Id == status).Name;

                if (model.OrderDetail.FirstOrDefault().TotalSummOrder < 15000 && model.OrderDetail.FirstOrDefault().StatusId == 7)
                {
                    model.Message = "Оформление покупки возможно при стоимости заказа от 15 000 рублей";
                }

            }
            else
            {
                model.ModelIsEmpty = true;
            }

            return model;
        }






    }
}