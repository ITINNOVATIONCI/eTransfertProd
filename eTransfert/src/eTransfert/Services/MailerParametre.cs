using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookAutomation.Services
{
    public class MailerParametre
    {

        public  string fromAddress = "laressource@live.fr";
        public string recipients { get; set; }
        public string subject { get; set; }
        public string text { get; set; }
        public string html { get; set; }
        public string templateEngine { get; set; }


        public Dictionary<string, string> Substitution { get; set; }
         
        //public static string sgUsername = "azure_da0713702f435dcc22edd7c82dd05341@azure.com";
        //public static string sgPassword = "P@ssw0rd2012";


    }
}
