using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eTransfert.Models
{
    public class Transactions
    {
        public string Id { get; set; }
        public string idUtilisateur { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateTransaction { get; set; }
        public string buyer_name { get; set; }
        public string Numero { get; set; }
        public string Liste { get; set; }
        public Double Montant { get; set; }
        public Double Pourcentage { get; set; }
        public Double Total { get; set; }
        public string numerotransaction { get; set; }
        public string message { get; set; }
        public string reponse { get; set; }
        public string htmlMessage { get; set; }
        public string log { get; set; }
        public string statustransfert { get; set; }
        public string statuscinetpay { get; set; }
        public string TypeTransaction { get; set; }
        public string TypeTransfert { get; set; }
        public string ModePaiement { get; set; }
        public string status { get; set; }
        public string Etat { get; set; }


    }
}