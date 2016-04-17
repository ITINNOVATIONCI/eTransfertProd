using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTransfert.Models
{
    public class RechargeCptePrincTrace
    {

        public string Id { get; set; }
        public string useremail { get; set; }        
        public string Etat { get; set; }
        public Double Montant { get; set; }
        public Double Benef { get; set; }
        public DateTime DateTransaction { get; set; }


    }
}
