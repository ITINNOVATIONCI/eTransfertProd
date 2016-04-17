using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eTransfert.Models
{
    public class Promotion
    {
       
        public string Id { get; set; }
        
        public string operateur { get; set; }
        
        public string offre { get; set; }
        
        public string description { get; set; }
        
        public string etat { get; set; }


    }
}
