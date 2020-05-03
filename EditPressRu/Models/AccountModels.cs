using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EditPressRu.Models
{

    //[Table("UserProfile")]
    //public class UserProfile
    //{
    //    [Key]
    //    [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
    //    public int UserId { get; set; }
    //    public string UserName { get; set; }
    //    public string Pass { get; set; }
    //    public virtual ICollection<UserProfileDetails> UserProfileDetail { get; set; }
    //}


    //public class UserProfileDetailsContext : DbContext
    //{
    //    public UserProfileDetailsContext()
    //        : base("EditPressConnection")
    //    {
    //    }

    //    public DbSet<UserProfileDetails> UserProfileDetails { get; set; }
    //}

    //[Table("UserProfileDetails")]
    //public class UserProfileDetails
    //{
    //    [Key]
    //    [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
    //    public int UserProfileDetailsID { get; set; }
    //    public int UserID { get; set; }
    //    public int DiscountID { get; set; }
    //    public string ShipmentAddress { get; set; }
    //    public string OrgName { get; set; }

    //    public string INN { get; set; }
    //    public string FIO { get; set; }
    //    public string Phone { get; set; }
    //    public string email { get; set; }

    //    public virtual UserProfile UserProfile { get; set; }
    //    public virtual Discounts Discount { get; set; }
    //}

    

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        //[Required]
        //[DataType(DataType.Password)]
        //[Display(Name = "Текущий пароль")]
        //public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Значение \"{0}\" должно содержать не менее {2} символа.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("NewPassword", ErrorMessage = "Новый пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Почта(email):")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль:")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Ошибка в адресе")]
        [Display(Name = "Email")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Значение \"{0}\" должно содержать не менее {2} символов.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }

        public string Pass { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }

    public class ChangePass
    {
        public string UserEmailOrLogin { get; set; }
        public string ErrMessage { get; set; }

    }
}
