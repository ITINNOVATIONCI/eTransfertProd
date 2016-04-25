using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eTransfert.ViewModels.Account
{
    public class LoginViewModel
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Role { get; set; }

        [Display(Name = "Se souvenir de moi?")]
        public bool RememberMe { get; set; }

        [Display(Name = "Est Connecté?")]
        public bool IsConnected { get; set; }
    }
}
