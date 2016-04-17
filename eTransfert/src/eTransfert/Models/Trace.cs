using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTransfert.Models
{
    public class Trace
    {
        public string Id { get; set; }
        public string Senderemail { get; set; }
        public string Receiveremail { get; set; }
        public string TypeTransaction { get; set; }
        public Double Montant { get; set; }
        public DateTime DateTransaction { get; set; }


    }
}
