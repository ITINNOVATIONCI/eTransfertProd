using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eTransfert.ViewModels.Account
{
    public class LoginViewModel
    {
        public string UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Role { get; set; }

        public Double CompteUnite { get; set; }
        public Double SeuilUnite { get; set; }

        [Display(Name = "Se souvenir de moi?")]
        public bool RememberMe { get; set; }

        [Display(Name = "Est Connecté?")]
        public bool IsConnected { get; set; }


    }
}
