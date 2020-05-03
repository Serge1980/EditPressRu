using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EditPressRu.Models
{
    public class AccountDetailsViewModel
    {
        public int UserID { get; set; }

        public int ProfileID { get; set; }

        [Display(Name = "E-Mail:")]
        [Required(ErrorMessage = "Это ваш идентификатор на сайте")]
        [EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        public string UserName { get; set; }

        [Display(Name = "Адрес Доставки:")]
        public string Shipment { get; set; }

        [Display(Name = "Вид скидки:")]
        public decimal? DiscountValue { get; set; }

        public string OrgName { get; set; }

        [Display(Name = "ИНН организации:")]
        public string INN { get; set; }

        [Display(Name = "Контактное лицо(Ф.И.О):")]
        public string FIO { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Телефон:")]
        public string PhoneNumber { get; set; }

        public string LogoImg { get; set; }

        public LoginModel LogMod { get; set; }

        //public Controllers.AjaxController.Result PartialModel { get; set; }
        //[Display(Name = "E-Mail:")]
        //[EmailAddress(ErrorMessage = "Неверный адрес электронной почты")]
        //public string Email { get; set; }
        public int OrderId { get; set; }
        public int StatusId { get; set; }
        
    }
}