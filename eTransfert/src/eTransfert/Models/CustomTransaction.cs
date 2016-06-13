using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTransfert.Models
{
    public class CustomTransaction
    {
        

        public string Id { get; set; }
        public string Idtrans { get; set; }
        public DateTime DateTransaction { get; set; }
        //public DateTime DateCreation { get; set; }
        public string Numero { get; set; }
        public Double Montant { get; set; }
        public Double Pourcentage { get; set; }
        public Double Total { get; set; }
        public Double Benefice { get; set; }
        public string TypeTransaction { get; set; }
        public string TypeTransfert { get; set; }
        public string status { get; set; }
        public string Email { get; set; }
        public Double CompteUnite { get; set; }
        public Double SeuilUnite { get; set; }


    }
}
