using System.ComponentModel.DataAnnotations;

namespace Anzoo.ViewModels.Account
{
    public class ChangePasswordFromProfileViewModel
    {
        [Required(ErrorMessage = "Parola actuală este obligatorie.")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola actuală")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Parola nouă este obligatorie.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "Parola trebuie să aibă între 8 și 40 de caractere.")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola nouă")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirmarea parolei este obligatorie.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmă parola nouă")]
        [Compare("NewPassword", ErrorMessage = "Parolele nu corespund.")]
        public string ConfirmNewPassword { get; set; }
    }
}
